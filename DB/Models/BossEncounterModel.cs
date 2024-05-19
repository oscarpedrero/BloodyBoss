using Bloodstone.API;
using BloodyBoss.Configuration;
using BloodyBoss.Exceptions;
using BloodyBoss.Systems;
using Bloody.Core.API;
using Bloody.Core.Helper;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Bloody.Core.Patch.Server;
using Bloody.Core;
using BloodyBoss.Patch;

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
        public List<ItemEncounterModel> items { get; set; } = [];
        public bool bossSpawn = false;

        public Entity bossEntity;
        public float Lifetime { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

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
            SpawnSystem.SpawnUnitWithCallback(sender, new PrefabGUID(PrefabGUID), new(x, z), Lifetime+5, (Entity e) => {
                ModifyBoss(sender, e);
            });
            var _message = PluginConfig.SpawnMessageBossTemplate.Value;
            _message = _message.Replace("#time#", FontColorChatSystem.Yellow($"{Lifetime / 60}"));
            _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));
            ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, FontColorChatSystem.Green($"{_message}"));

            return true;
        }

        public bool DropItems(string vblood)
        {

            foreach (var item in items)
            {
                if (probabilityGeneratingReward(item.Chance))
                {

                    var users = BossSystem.GetKillersEntity(vblood);
                    foreach (var user in users)
                    {

                        var itemGuid = new PrefabGUID(item.ItemID);
                        var stacks = item.Stack;

                        UserSystem.TryAddInventoryItemOrDrop(user, itemGuid, stacks);

                    }
                }
            }

            if(string.Empty != Hour)
            {
                HourDespawn = DateTime.Parse(Hour).AddSeconds(Lifetime).ToString("HH:mm:ss");
            }
            
            return true;
        }

        private static bool probabilityGeneratingReward(int percentage)
        {
            var number = new System.Random().Next(1, 100);

            if (number <= (percentage * 100))
            {
                return true;
            }

            return false;
        }

        public void ModifyBoss(Entity user, Entity boss)
        {
            var players = Core.Users.Online.ToList().Count;
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
            
            health.Value = health.MaxHealth.Value;
            boss.Write(health);
            BuffSystem.BuffNPC(boss, user, new PrefabGUID(PluginConfig.BuffForWorldBoss.Value), 0);
            RenameBoss(boss);
            bossEntity = boss;
            bossSpawn = true;
            AddIcon(boss);

        }

        private void AddIcon(Entity boss)
        {

            var action = () =>
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
            };
            ActionSchedulerPatch.RunActionOnceAfterFrames(action, 10);
            
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
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, VBloodUnit>(EntityQueryOptions.IncludeDisabledEntities);
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

        internal void RemoveIcon(Entity user)
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
            HourDespawn = DateTime.Parse(hour).AddSeconds(Lifetime).ToString("HH:mm:ss");
            Database.saveDatabase();
        }

        internal void SetHourDespawn()
        {
            HourDespawn = DateTime.Now.AddSeconds(Lifetime).ToString("HH:mm:ss");
            Database.saveDatabase();
        }

        internal void KillBoss(Entity user)
        {

            if (GetBossEntity())
            {
                StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, bossEntity, user, user, 0, StatChangeReason.Any, true);
            }
            
        }

        internal void DespawnBoss(Entity user)
        {
            RemoveIcon(user);
            KillBoss(user);
            bossSpawn = false;
        }

        internal void CheckSpawnDespawn()
        {
            var date = DateTime.Now;
            if (date.ToString("HH:mm") == Hour)
            {
                var user = UserSystem.GetOneUserOnline();
                Plugin.Logger.LogInfo("Spawn Boss");
                Spawn(user);
                bossSpawn = true;
            }

            if (date.ToString("HH:mm:ss") == HourDespawn)
            {
                if (bossSpawn == true)
                {
                    Entity user = UserSystem.GetOneUserOnline();
                    Plugin.Logger.LogInfo("Despawn Boss");
                    var _message = PluginConfig.DespawnMessageBossTemplate.Value;
                    _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));
                    ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, FontColorChatSystem.Green($"{_message}"));
                    DespawnBoss(user);
                    
                }
                
            }
        }
    }
}
