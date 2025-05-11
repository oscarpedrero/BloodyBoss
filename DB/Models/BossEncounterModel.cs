using BloodyBoss.Configuration;
using BloodyBoss.Exceptions;
using BloodyBoss.Systems;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using ProjectM.Shared;
using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Patch.Server;
using Bloody.Core.Methods;
using Bloody.Core.Models.v1;
using BloodyBoss.Utils;

namespace BloodyBoss.DB.Models
{
    internal class BossEncounterModel
    {
        private Entity icontEntity;

        public string name { get; set; } = string.Empty;
        public string nameHash { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public string Hour { get; set; } = string.Empty;
        public string HourDespawn { get; set; } = string.Empty;
        public int PrefabGUID { get; set; }
        public int level { get; set; }
        public int multiplier { get; set; }
        public List<ItemEncounterModel> items { get; set; } = new();
        public bool bossSpawn { get; set; } = false;
        
        public Entity bossEntity;
        public float Lifetime { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public UnitStatsModel unitStats { get; set; } = null;

        private const double SendMessageDelay = 2;
        public static List<string> vbloodKills = new();

        public bool vbloodFirstKill = false;

        private static readonly System.Random Random = new();

        private BossEncounterModel bossRandom;

        public List<ItemEncounterModel> GetItems()
        {
            return items;
        }

        public bool GetItem(int itemPrefabID, out ItemEncounterModel item)
        {
            item = items.Where(x => x.ItemID == itemPrefabID).FirstOrDefault();
            if (item == null)
            {
                return false;
            }
            return true;
        }

        public bool GetItemFromName(string ItemName, out ItemEncounterModel item)
        {
            item = items.Where(x => x.name == ItemName).FirstOrDefault();
            if (item == null)
            {
                return false;
            }
            return true;
        }

        public bool AddItem(string ItemName, int ItemPrefabID, int Stack, int Chance = 1)
        {
            if (!GetItem(ItemPrefabID, out ItemEncounterModel item))
            {
                item = new ItemEncounterModel();
                item.name = ItemName;
                item.ItemID = ItemPrefabID;
                item.Stack = Stack;
                item.Chance = Chance;
                items.Add(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductExistException();

        }

        public bool RemoveItem(string ItemName)
        {
            if (GetItemFromName(ItemName, out ItemEncounterModel item))
            {
                items.Remove(item);
                Database.saveDatabase();
                return true;
            }

            throw new ProductDontExistException();
        }

        public bool Spawn(Entity sender)
        {
            SpawnSystem.SpawnUnitWithCallback(sender, new PrefabGUID(PrefabGUID), new(x, z), Lifetime + 30, (Entity e) => {
                
                bossEntity = e;
                ModifyBoss(sender, e);
                if (PluginConfig.ClearDropTable.Value)
                {
                    var action = () =>
                    {
                        ClearDropTable(e);
                    };
                    CoroutineHandler.StartFrameCoroutine(action, 10, 1);
                }

                var actionTeams = () =>
                {
                    BossSystem.CheckTeams(e);
                };
                var random = new System.Random();
                CoroutineHandler.StartFrameCoroutine(actionTeams, random.Next(60), 1);


                var actionIcon = () =>
                {
                    AddIcon(bossEntity);
                };
                ActionScheduler.RunActionOnceAfterDelay(actionIcon, 3);
            });

            

            var _message = PluginConfig.SpawnMessageBossTemplate.Value;
            _message = _message.Replace("#time#", FontColorChatSystem.Yellow($"{Lifetime / 60}"));
            _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));
            HourDespawn = DateTime.Parse(Hour).AddSeconds(Lifetime).ToString("HH:mm:ss");
            var _ref_message = (FixedString512Bytes) FontColorChatSystem.Green($"{_message}");
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager,ref _ref_message);

            return true;
        }

        public bool Spawn(Entity sender, float locationX, float locationZ)
        {
            SpawnSystem.SpawnUnitWithCallback(sender, new PrefabGUID(PrefabGUID), new(locationX, locationZ), Lifetime + 30, (Entity e) => {

                bossEntity = e;
                ModifyBoss(sender, e);
                if (PluginConfig.ClearDropTable.Value)
                {
                    var action = () =>
                    {
                        ClearDropTable(e);
                    };
                    CoroutineHandler.StartFrameCoroutine(action, 10, 1);
                }


                var actionIcon = () =>
                {
                    AddIcon(bossEntity, locationX, locationZ);
                };
                ActionScheduler.RunActionOnceAfterDelay(actionIcon, 3);

                var actionTeams = () =>
                {
                    BossSystem.CheckTeams(e);
                };
                var random = new System.Random();
                ActionScheduler.RunActionOnceAfterDelay(actionTeams, 2);

            });



            var _message = PluginConfig.SpawnMessageBossTemplate.Value;
            _message = _message.Replace("#time#", FontColorChatSystem.Yellow($"{Lifetime / 60}"));
            _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));
            var _ref_message = (FixedString512Bytes)FontColorChatSystem.Green($"{_message}");
            HourDespawn = DateTime.Parse(Hour).AddSeconds(Lifetime).ToString("HH:mm:ss");
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, ref _ref_message);

            return true;
        }

        public void ClearDropTable(Entity boss)
        {
            var dropTableBuffer = boss.ReadBuffer<DropTableBuffer>();
            dropTableBuffer.Clear();
        }

        public bool DropItems()
        {

            foreach (var item in items)
            {
                if (probabilityGeneratingReward(item.Chance))
                {

                    foreach (var user in GetKillers())
                    {

                        UserModel player = GameData.Users.GetUserByCharacterName(user);
                        var itemGuid = new PrefabGUID(item.ItemID);
                        var stacks = item.Stack;

                        if(!player.TryGiveItem(itemGuid, stacks, out Entity itemEntity)){
                            player.DropItemNearby(itemGuid, stacks);
                        }

                    }
                }
            }

            if(string.Empty != Hour)
            {
                HourDespawn = DateTime.Parse(Hour).AddSeconds(Lifetime - 2).ToString("HH:mm:ss");
            }
            
            return true;
        }

        private static bool probabilityGeneratingReward(int percentage)
        {
            var number = new System.Random().Next(1, 100);

            if (number <= (percentage * 1))
            {
                return true;
            }

            return false;
        }

        public void ModifyBoss(Entity user, Entity boss)
        {
            AssetName = Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup[new PrefabGUID(PrefabGUID)].AssetName.ToString();
            var players = GameData.Users.Online.ToList().Count;
            var unit = boss.Read<UnitLevel>();
            unit.Level = new ModifiableInt(level);
            boss.Write(unit);
            var health = boss.Read<Health>();
            if (PluginConfig.PlayersMultiplier.Value)
            {
                health.MaxHealth._Value = (health.MaxHealth * (players * multiplier));
            } else
            {
                health.MaxHealth._Value = health.MaxHealth * multiplier;
            }

            if (!IsVBlood(boss))
            {
                if (boss.Has<BloodConsumeSource>())
                {
                    var blood = boss.Read<BloodConsumeSource>();
                    blood.CanBeConsumed = false;
                    boss.Write(blood);
                }
            }

            var BossUnitStats = boss.Read<UnitStats>();
            if (unitStats == null)
            {
                GenerateStats();
            }

            boss.Write(unitStats.FillStats(BossUnitStats));

            health.Value = health.MaxHealth.Value;
            boss.Write(health);
            BuffSystem.BuffNPC(boss, user, new PrefabGUID(PluginConfig.BuffForWorldBoss.Value), 0);
            RenameBoss(boss);
            bossSpawn = true;

        }

        private void AddIcon(Entity boss)
        {

            SpawnSystem.SpawnUnitWithCallback(boss, Prefabs.MapIcon_POI_VBloodSource, new float2(x, z), Lifetime + 5, (Entity e) => {
                icontEntity = e;
                e.Add<MapIconData>();
                e.Add<MapIconTargetEntity>();
                var mapIconTargetEntity = e.Read<MapIconTargetEntity>();
                mapIconTargetEntity.TargetEntity = NetworkedEntity.ServerEntity(boss);
                mapIconTargetEntity.TargetNetworkId = boss.Read<NetworkId>();
                e.Write(mapIconTargetEntity);
                e.Add<NameableInteractable>();
                NameableInteractable _nameableInteractable = e.Read<NameableInteractable>();
                _nameableInteractable.Name = new FixedString64Bytes(nameHash + "ibb");
                e.Write(_nameableInteractable);
            });
            
        }

        private void AddIcon(Entity boss, float locationX, float locationZ)
        {

            SpawnSystem.SpawnUnitWithCallback(boss, Prefabs.MapIcon_POI_VBloodSource, new float2(locationX, locationZ), Lifetime + 5, (Entity e) => {
                icontEntity = e;
                e.Add<MapIconData>();
                e.Add<MapIconTargetEntity>();
                var mapIconTargetEntity = e.Read<MapIconTargetEntity>();
                mapIconTargetEntity.TargetEntity = NetworkedEntity.ServerEntity(boss);
                mapIconTargetEntity.TargetNetworkId = boss.Read<NetworkId>();
                e.Write(mapIconTargetEntity);
                e.Add<NameableInteractable>();
                NameableInteractable _nameableInteractable = e.Read<NameableInteractable>();
                _nameableInteractable.Name = new FixedString64Bytes(nameHash + "ibb");
                e.Write(_nameableInteractable);
            });
            
        }

        public void GenerateStats()
        {
            Entity bossEntity = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(PrefabGUID)];
            var BossUnitStats = bossEntity.Read<UnitStats>();
            unitStats = new UnitStatsModel();
            unitStats.SetStats(BossUnitStats);
        }

        private bool GetIcon()
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
            foreach (var entity in entities)
            {
                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                if (_nameableInteractable.Name.Value == nameHash + "ibb")
                {
                    icontEntity = entity;
                    entities.Dispose();
                    return true;
                }
            }
            entities.Dispose();
            return false;
        }

        public bool GetBossEntity()
        {

            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, UnitStats>(EntityQueryOptions.IncludeDisabledEntities);

            foreach (var entity in entities)
            {
                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                if (_nameableInteractable.Name.Value == nameHash + "bb")
                {
                    bossEntity = entity;
                    entities.Dispose();
                    return true;
                }
            }
            entities.Dispose();
            return false;
        }

        public bool IsVBlood(Entity boss)
        {
            if (boss.Has<VBloodUnit>())
            {
                return true;
            } else
            {
                return false;
            }
        }

        public void RemoveIcon(Entity user)
        {
            if (GetIcon())
            {
                StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, icontEntity, user, user, 0, StatChangeReason.Any, true);
            }
        }
        internal void SetLocation(float3 position)
        {
            
            x = position.x;
            z = position.z;
            y = position.y;

            Database.saveDatabase();

        }

        private void RenameBoss(Entity boss)
        {
            boss.Add<NameableInteractable>();
            NameableInteractable _nameableInteractable = boss.Read<NameableInteractable>();
            _nameableInteractable.Name = new FixedString64Bytes(nameHash+"bb");
            boss.Write(_nameableInteractable);
        }

        internal void SetAssetName(string v)
        {
            AssetName = v;
            Database.saveDatabase();
        }

        internal void SetHour(string hour)
        {
            Hour = hour;
            HourDespawn = DateTime.Parse(hour).AddSeconds(Lifetime - 5).ToString("HH:mm:ss");
            Database.saveDatabase();
        }

        internal void SetHourDespawn()
        {
            HourDespawn = DateTime.Now.AddSeconds(Lifetime - 5).ToString("HH:mm:ss");
            Database.saveDatabase();
        }

        internal void KillBoss(Entity user)
        {

            if (GetIcon())
            {
                StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, bossEntity, user, user, 0, StatChangeReason.Any, true);
            }

            if (GetBossEntity())
            {
                StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, bossEntity, user, user, 0, StatChangeReason.Any, true);
            }
        }

        internal void DespawnBoss(Entity user)
        {
            try
            {
                ClearDropTable(bossEntity);
            } catch {

                Plugin.Logger.LogWarning($"The boss {name} error cleardrop");

            }

            try
            {
                RemoveIcon(user);
            }
            catch
            {

                Plugin.Logger.LogWarning($"The boss {name} error remove icon");

            }

            try
            {
                KillBoss(user);
            }
            catch
            {

                Plugin.Logger.LogWarning($"The boss {name} error kill boss");

            }

            
            
            bossSpawn = false;
        }

        internal void CheckSpawnDespawn()
        {
            
            var date = DateTime.Now;
            if (date.ToString("HH:mm") == Hour)
            {
                if (!bossSpawn)
                {
                    var userModel = GameData.Users.All.FirstOrDefault();
                    var user = userModel.Entity;
                    if (Database.BOSSES.Count > 1 && PluginConfig.RandomBoss.Value)
                    {
                        Plugin.Logger.LogInfo("Spawn Random Boss");
                        bossRandom = Database.BOSSES[Random.Next(Database.BOSSES.Count)];
                        bossRandom.Spawn(user,x,z);
                        bossRandom.bossSpawn = true;
                        Database.saveDatabase();
                    } else
                    {
                        Plugin.Logger.LogInfo("Spawn Boss");
                        Spawn(user);
                        bossSpawn = true;
                        Database.saveDatabase();
                    }
                    
                } else
                {
                    Plugin.Logger.LogWarning($"The boss {name} cannot be summoned again, since it is currently active");
                }
            }
            
            //Plugin.Logger.LogInfo($"Check {date.ToString("HH:mm:ss")} vs {HourDespawn}");
            if (date.ToString("HH:mm:ss") == HourDespawn)
            {
                var userModel = GameData.Users.All.FirstOrDefault();
                var user = userModel.Entity;
                if (Database.BOSSES.Count > 1 && PluginConfig.RandomBoss.Value)
                {
                    if (bossRandom.bossSpawn)
                    {

                        Plugin.Logger.LogInfo("Despawn Random Boss");
                        var _message = PluginConfig.DespawnMessageBossTemplate.Value;
                        _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));
                        var _ref_message = (FixedString512Bytes)FontColorChatSystem.Green($"{_message}");
                        ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref _ref_message);
                        bossRandom.DespawnBoss(user);
                        bossRandom.bossSpawn = false;
                        bossRandom = null;
                        Database.saveDatabase();

                    }
                }
                else
                {
                    if (bossSpawn)
                    {
                        Plugin.Logger.LogInfo("Despawn Boss");
                        var _message = PluginConfig.DespawnMessageBossTemplate.Value;
                        _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));
                        var _ref_message = (FixedString512Bytes)FontColorChatSystem.Green($"{_message}");
                        ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref _ref_message );
                        DespawnBoss(user);
                        bossSpawn = false;
                        Database.saveDatabase();

                    }
                }
            }
        }

        public void AddKiller(string killerCharacterName)
        {
            if (vbloodKills.Where(x => x == killerCharacterName).FirstOrDefault() == null)
            {
                vbloodKills.Add(killerCharacterName);
            }
            
        }

        public void RemoveKillers()
        {
            vbloodKills = new();
        }

        public List<string> GetKillers()
        {
            return vbloodKills;
        }

        public void SendAnnouncementMessage()
        {
            var message = GetAnnouncementMessage();
            if (message != null)
            {
                var killers = GetKillers();
                DropItems();
                var _ref_message = (FixedString512Bytes) message;
                ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, ref _ref_message);

                foreach (var killer in killers)
                {
                    _ref_message = (FixedString512Bytes)$"{FontColorChatSystem.Yellow($"- {killer}")}";
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, ref _ref_message);
                }

                RemoveKillers();
                bossSpawn = false;

                RemoveIcon(GameData.Users.All.FirstOrDefault().Entity);
                Database.saveDatabase();
            }

        }

        public void BuffKillers()
        {
            if (PluginConfig.BuffAfterKillingEnabled.Value)
            {
                foreach (var killer in GetKillers())
                {
                    var playerModel = GameData.Users.GetUserByCharacterName(killer);
                    BuffSystem.BuffPlayer(playerModel.Character.Entity, playerModel.Entity, new PrefabGUID(PluginConfig.BuffAfterKillingPrefabGUID.Value), 3, false);
                }
            }
            
        }

        public string GetAnnouncementMessage()
        {
            var _message = PluginConfig.KillMessageBossTemplate.Value;
            _message = _message.Replace("#vblood#", $"{FontColorChatSystem.Red(name)}");
            return FontColorChatSystem.Green($"{_message}");
        }
    }
}
