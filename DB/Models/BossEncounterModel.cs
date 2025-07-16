using BloodyBoss.Configuration;
using BloodyBoss.Exceptions;
using BloodyBoss.Systems;
using ProjectM;
using ProjectM.Network;
using ProjectM.CastleBuilding;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM.Shared;
using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Patch.Server;
using Bloody.Core.Methods;
using Bloody.Core.Models.v1;
using ProjectM.Gameplay.Scripting;

namespace BloodyBoss.DB.Models
{
    internal class BossEncounterModel
    {
        public Entity iconEntity;

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

        // Progressive Difficulty Properties
        public int ConsecutiveSpawns { get; set; } = 0;
        public DateTime? LastSuccessfulKill { get; set; } = null;
        public float CurrentDifficultyMultiplier { get; set; } = 1.0f;

        // Timer Management Properties
        public bool IsPaused { get; set; } = false;
        public DateTime? PausedAt { get; set; } = null;
        public DateTime LastSpawn { get; set; } = DateTime.MinValue;
        
        // Phase Announcement Properties
        public int LastAnnouncedPhase { get; set; } = 0;
        
        public List<BossMechanicModel> Mechanics { get; set; } = new List<BossMechanicModel>();
        
        // Ability Swap Configuration
        public Dictionary<int, AbilitySwapConfig> AbilitySwaps { get; set; } = new Dictionary<int, AbilitySwapConfig>();

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

        public bool AddItem(string ItemName, int ItemPrefabID, int Stack, float Chance = 1)
        {
            if (!GetItem(ItemPrefabID, out ItemEncounterModel item))
            {
                item = new ItemEncounterModel();
                item.name = ItemName;
                item.ItemID = ItemPrefabID;
                item.Stack = Stack;
                item.Chance = (int)(Chance * 100);
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
            // Verificar y obtener posición válida para spawn
            var (validX, validZ, usedOriginal) = GetValidSpawnPosition(x, z);
            
            if (!usedOriginal)
            {
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name}: Spawn position adjusted to avoid castle territory");
            }
            
            // Mantener siempre el PrefabGUID original para la apariencia
            var spawnPrefabGUID = new PrefabGUID(PrefabGUID);
            
            SpawnSystem.SpawnUnitWithCallback(sender, spawnPrefabGUID, new float3(validX, y, validZ), Lifetime, (Entity e) => {
                
                bossEntity = e;
                ModifyBoss(sender, e);
                
                // Log spawn position once after boss is created
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name}: Spawned at position ({validX:F2}, {y:F2}, {validZ:F2})");
                
                // Registration happens inside ModifyBoss now
                
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
                    AddIcon(bossEntity, validX, validZ);
                };
                ActionScheduler.RunActionOnceAfterDelay(actionIcon, 3);
            });

            

            var _message = PluginConfig.SpawnMessageBossTemplate.Value;
            _message = _message.Replace("#time#", FontColorChatSystem.Yellow($"{Lifetime / 60}"));
            _message = _message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{name}"));
            HourDespawn = DateTime.Parse(Hour).AddSeconds(Lifetime).ToString("HH:mm:ss");
            var _ref_message = (FixedString512Bytes) FontColorChatSystem.Green($"{_message}");
            ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager,ref _ref_message);

            return true;
        }

        public bool Spawn(Entity sender, float locationX, float locationZ)
        {
            return Spawn(sender, locationX, y, locationZ);
        }
        
        public bool Spawn(Entity sender, float locationX, float locationY, float locationZ)
        {
            // Verificar y obtener posición válida para spawn
            var (validX, validZ, usedOriginal) = GetValidSpawnPosition(locationX, locationZ);
            
            if (!usedOriginal)
            {
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name}: Spawn position adjusted to avoid castle territory");
            }
            
            // Mantener siempre el PrefabGUID original para la apariencia
            var spawnPrefabGUID = new PrefabGUID(PrefabGUID);
            
            // Store the Y temporarily to use in ModifyBoss
            var originalY = y;
            y = locationY;
            
            SpawnSystem.SpawnUnitWithCallback(sender, spawnPrefabGUID, new float3(validX, locationY, validZ), Lifetime, (Entity e) => {

                bossEntity = e;
                ModifyBoss(sender, e);
                
                // Restore original Y after spawn
                y = originalY;
                
                // Initialize boss mechanics system
                BossMechanicSystem.InitializeBossMechanics(e, this);
                
                // Registration happens inside ModifyBoss now
                
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
                    AddIcon(bossEntity, validX, validZ);
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
            ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref _ref_message);

            return true;
        }

        public void ClearDropTable(Entity boss)
        {
            var dropTableBuffer = boss.ReadBuffer<DropTableBuffer>();
            dropTableBuffer.Clear();
        }

        public bool DropItems()
        {
            var killers = GetKillers();
            Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name} dropping items to {killers.Count} killers");
            
            if (killers.Count == 0)
            {
                Plugin.BLogger.Warning(LogCategory.Boss, $"Boss {name} has no killers registered, no items will drop");
                return false;
            }

            foreach (var item in items)
            {
                if (probabilityGeneratingReward(item.Chance))
                {
                    Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name} dropping item {item.ItemID} x{item.Stack}");

                    foreach (var user in killers)
                    {
                        try
                        {
                            UserModel player = GameData.Users.GetUserByCharacterName(user);
                            if (player == null)
                            {
                                Plugin.BLogger.Warning(LogCategory.Boss, $"Player {user} not found for item drop");
                                continue;
                            }

                            var itemGuid = new PrefabGUID(item.ItemID);
                            var stacks = item.Stack;

                            // Send chat message to player about received item
                            try
                            {
                                var itemNameColored = $"<color={item.Color}>{item.name}</color>";
                                var itemMessage = $"You received: {itemNameColored} x{stacks}";
                                Plugin.BLogger.Info(LogCategory.Boss, $"Sending item message to {user}: {itemMessage}");
                                
                                // Get user component and send message
                                var userEntity = Plugin.SystemsCore.EntityManager.GetComponentData<PlayerCharacter>(player.Character.Entity).UserEntity;
                                var userComponent = Plugin.SystemsCore.EntityManager.GetComponentData<User>(userEntity);
                                FixedString512Bytes unityMessage = itemMessage;
                                ServerChatUtils.SendSystemMessageToClient(Core.World.EntityManager, userComponent, ref unityMessage);
                                Plugin.BLogger.Info(LogCategory.Boss, $"Chat message sent successfully to {user}");
                            }
                            catch (Exception ex)
                            {
                                Plugin.BLogger.Error(LogCategory.Boss, $"Error sending chat message to {user}: {ex.Message}");
                            }

                            if(!player.TryGiveItem(itemGuid, stacks, out Entity itemEntity)){
                                player.DropItemNearby(itemGuid, stacks);
                                Plugin.BLogger.Info(LogCategory.Boss, $"Item dropped near player {user}");
                            } else {
                                Plugin.BLogger.Info(LogCategory.Boss, $"Item given to player {user}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.BLogger.Error(LogCategory.Boss, $"Error dropping item to player {user}: {ex.Message}");
                        }
                    }
                } else {
                    Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name} item {item.ItemID} failed probability check ({item.Chance}%)");
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
            
            // Calculate health multiplier using dynamic scaling or legacy system
            float healthMultiplier = multiplier;
            
            if (PluginConfig.EnableDynamicScaling.Value)
            {
                // Use new dynamic scaling system
                healthMultiplier = DynamicScalingSystem.CalculateHealthMultiplier(this);
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name} using dynamic scaling: x{healthMultiplier:F2}");
            }
            else if (PluginConfig.PlayersMultiplier.Value)
            {
                // Use legacy player multiplier system
                healthMultiplier = players * multiplier;
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name} using legacy scaling: {players} players x {multiplier} = x{healthMultiplier:F2}");
            }
            else
            {
                // Use base multiplier only
                Plugin.BLogger.Debug(LogCategory.Boss, $"Boss {name} using base multiplier: x{healthMultiplier:F2}");
            }
            
            health.MaxHealth._Value = health.MaxHealth * healthMultiplier;

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

            // Apply dynamic scaling to stats if enabled
            UnitStatsModel finalStats;
            if (PluginConfig.EnableDynamicScaling.Value)
            {
                finalStats = DynamicScalingSystem.ApplyDynamicScaling(this, unitStats);
                
                // Increment consecutive spawns for progressive difficulty
                if (PluginConfig.EnableProgressiveDifficulty.Value)
                {
                    ConsecutiveSpawns++;
                    Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name} consecutive spawns: {ConsecutiveSpawns}");
                }
            }
            else
            {
                finalStats = unitStats;
            }

            boss.Write(finalStats.FillStats(BossUnitStats));

            health.Value = health.MaxHealth.Value;
            boss.Write(health);
            BuffSystem.BuffNPC(boss, user, new PrefabGUID(PluginConfig.BuffForWorldBoss.Value), 0);
            RenameBoss(boss);
            bossSpawn = true;
            LastSpawn = DateTime.Now;
            
            // Remove VBloodUnlockTechBuffer to prevent tech unlocks from world boss
            RemoveVBloodUnlockBuffer(boss);
            
            // Save database to persist spawn state
            Database.saveDatabase();
            
            // Aplicar intercambio de habilidades individuales (sistema híbrido)
            if (AbilitySwaps != null && AbilitySwaps.Count > 0)
            {
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name}: Applying hybrid ability swaps for {AbilitySwaps.Count} slots");
                ApplyIndividualAbilitySwaps(boss);
            }
            
            // Initialize boss mechanics
            if (Mechanics.Count > 0)
            {
                Plugin.BLogger.Debug(LogCategory.Boss, $"Boss {name}: Initializing {Mechanics.Count} mechanics");
                BossMechanicSystem.InitializeBossMechanics(boss, this);
            }
            
            // Register boss for damage tracking
            BossGameplayEventSystem.RegisterBoss(boss, this);
            
            // Register boss for optimized tracking
            BossTrackingSystem.RegisterSpawnedBoss(boss, this);

        }

        private void AddIcon(Entity boss)
        {

            SpawnSystem.SpawnUnitWithCallback(boss, Prefabs.MapIcon_POI_VBloodSource, new float2(x, z), Lifetime + 5, (Entity e) => {
                iconEntity = e;
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
                iconEntity = e;
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
                    iconEntity = entity;
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
                StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, iconEntity, user, user, 0, StatChangeReason.Any, true);
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

                Plugin.BLogger.Warning(LogCategory.Boss, $"The boss {name} error cleardrop");

            }

            try
            {
                RemoveIcon(user);
            }
            catch
            {

                Plugin.BLogger.Warning(LogCategory.Boss, $"The boss {name} error remove icon");

            }

            try
            {
                KillBoss(user);
            }
            catch
            {

                Plugin.BLogger.Warning(LogCategory.Boss, $"The boss {name} error kill boss");

            }

            // Unregister boss from damage tracking
            if (GetBossEntity())
            {
                BossGameplayEventSystem.UnregisterBoss(bossEntity);
            }
            
            bossSpawn = false;
            Database.saveDatabase();
        }

        internal void CheckSpawnDespawn()
        {
            
            var date = DateTime.Now;
            if (date.ToString("HH:mm") == Hour)
            {
                // Check if we already spawned in this minute
                if (LastSpawn != DateTime.MinValue && LastSpawn.ToString("HH:mm") == date.ToString("HH:mm"))
                {
                    // Already spawned this minute, skip
                    return;
                }
                
                if (!bossSpawn)
                {
                    var userModel = GameData.Users.All.FirstOrDefault();
                    var user = userModel.Entity;
                    if (Database.BOSSES.Count > 1 && PluginConfig.RandomBoss.Value)
                    {
                        Plugin.BLogger.Info(LogCategory.Boss, "Spawn Random Boss");
                        bossRandom = Database.BOSSES[Random.Next(Database.BOSSES.Count)];
                        // Use the random boss position with its own Y coordinate
                        bossRandom.Spawn(user, bossRandom.x, bossRandom.y, bossRandom.z);
                        bossRandom.bossSpawn = true;
                        Database.saveDatabase();
                    } else
                    {
                        Plugin.BLogger.Info(LogCategory.Boss, "Spawn Boss");
                        Spawn(user);
                        bossSpawn = true;
                        Database.saveDatabase();
                    }
                    
                } else
                {
                    Plugin.BLogger.Warning(LogCategory.Boss, $"The boss {name} cannot be summoned again, since it is currently active");
                }
            }
            
            // Manual despawn by HourDespawn is now disabled - relying on LifeTime system
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
                ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref _ref_message);

                foreach (var killer in killers)
                {
                    _ref_message = (FixedString512Bytes)$"{FontColorChatSystem.Yellow($"- {killer}")}";
                    ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref _ref_message);
                }

                RemoveKillers();
                bossSpawn = false;
                Database.saveDatabase();

                // Reset progressive difficulty on successful kill
                if (PluginConfig.EnableProgressiveDifficulty.Value && PluginConfig.ResetDifficultyOnKill.Value)
                {
                    ConsecutiveSpawns = 0;
                    CurrentDifficultyMultiplier = 1.0f;
                    LastSuccessfulKill = DateTime.Now;
                    Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name} difficulty reset after successful kill");
                }

                RemoveIcon(GameData.Users.All.FirstOrDefault().Entity);
                Database.saveDatabase();
            }

        }

        public void BuffKillers()
        {
            // First, filter out players who are too far away
            FilterKillersByDistance();
            
            if (PluginConfig.BuffAfterKillingEnabled.Value)
            {
                foreach (var killer in GetKillers())
                {
                    var playerModel = GameData.Users.GetUserByCharacterName(killer);
                    BuffSystem.BuffPlayer(playerModel.Character.Entity, playerModel.Entity, new PrefabGUID(PluginConfig.BuffAfterKillingPrefabGUID.Value), 3, false);
                }
            }
        }
        
        /// <summary>
        /// Removes players from the killers list if they are too far from the boss
        /// </summary>
        private void FilterKillersByDistance()
        {
            // Get boss position for distance check
            float3 bossPosition = float3.zero;
            bool hasBossPosition = false;
            
            if (GetBossEntity() && bossEntity.Has<LocalToWorld>())
            {
                bossPosition = bossEntity.Read<LocalToWorld>().Position;
                hasBossPosition = true;
            }
            
            if (!hasBossPosition)
            {
                Plugin.BLogger.Warning(LogCategory.Reward, "Could not get boss position for distance filtering");
                return;
            }
            
            // Create a filtered list
            var validKillers = new List<string>();
            
            foreach (var killer in GetKillers())
            {
                var playerModel = GameData.Users.GetUserByCharacterName(killer);
                
                if (playerModel.Character.Entity != Entity.Null && playerModel.Character.Entity.Has<LocalToWorld>())
                {
                    var playerPosition = playerModel.Character.Entity.Read<LocalToWorld>().Position;
                    var distance = math.distance(playerPosition, bossPosition);
                    
                    if (distance <= 100f) // Only keep players within 100 units
                    {
                        validKillers.Add(killer);
                        Plugin.BLogger.Debug(LogCategory.Reward, $"Player {killer} validated (distance: {distance:F1} units)");
                    }
                    else
                    {
                        Plugin.BLogger.Info(LogCategory.Reward, $"Player {killer} removed from killers list (too far: {distance:F1} units)");
                    }
                }
                else
                {
                    // If we can't check distance (player offline?), don't include them
                    Plugin.BLogger.Debug(LogCategory.Reward, $"Player {killer} removed from killers list (entity not found)");
                }
            }
            
            // Replace the killers list with only valid killers
            vbloodKills = validKillers;
            Plugin.BLogger.Info(LogCategory.Reward, $"Filtered killers list: {vbloodKills.Count} valid players out of {GetKillers().Count} total");
        }

        public string GetAnnouncementMessage()
        {
            var _message = PluginConfig.KillMessageBossTemplate.Value;
            _message = _message.Replace("#vblood#", $"{FontColorChatSystem.Red(name)}");
            return FontColorChatSystem.Green($"{_message}");
        }

        /// <summary>
        /// Verifica si una posición está dentro de un territorio de castillo
        /// </summary>
        /// <param name="position">Posición a verificar</param>
        /// <param name="checkRadius">Radio de verificación en unidades (por defecto 10)</param>
        /// <returns>True si está en un castillo, False si es una posición válida para spawn</returns>
        private static bool IsPositionInCastle(float3 position, float checkRadius = 10f)
        {
            try
            {
                var query = Core.World.EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<PrefabGUID>(),
                    ComponentType.ReadOnly<LocalToWorld>(),
                    ComponentType.ReadOnly<UserOwner>(),
                    ComponentType.ReadOnly<CastleFloor>());

                var entities = query.ToEntityArray(Allocator.Temp);
                
                foreach (var entity in entities)
                {
                    if (!entity.Has<LocalToWorld>())
                    {
                        continue;
                    }
                    var localToWorld = entity.Read<LocalToWorld>();
                    var castlePosition = localToWorld.Position;
                    
                    var distance = math.distance(position, castlePosition);
                    if (distance < checkRadius)
                    {
                        entities.Dispose();
                        query.Dispose();
                        return true; // Está dentro de un castillo
                    }
                }
                entities.Dispose();
                query.Dispose();
                return false; // Posición válida, no está en castillo
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Error checking castle position: {ex.Message}");
                return false; // En caso de error, asumir que es válida
            }
        }

        /// <summary>
        /// Busca una posición válida para spawn cerca de la posición original
        /// </summary>
        /// <param name="originalPosition">Posición original donde se quería hacer spawn</param>
        /// <param name="searchRadius">Radio de búsqueda en unidades</param>
        /// <param name="attempts">Número de intentos de búsqueda</param>
        /// <returns>Nueva posición válida o null si no se encuentra</returns>
        private static float3? FindValidSpawnPosition(float3 originalPosition, float searchRadius = 100f, int attempts = 20)
        {
            try
            {
                for (int i = 0; i < attempts; i++)
                {
                    // Generar posición aleatoria dentro del radio
                    var angle = Random.NextDouble() * 2 * Math.PI;
                    var distance = Random.NextDouble() * searchRadius;
                    
                    var testPosition = new float3(
                        originalPosition.x + (float)(Math.Cos(angle) * distance),
                        originalPosition.y, // Mantener la Y original
                        originalPosition.z + (float)(Math.Sin(angle) * distance)
                    );

                    // Verificar si la nueva posición está libre de castillos
                    if (!IsPositionInCastle(testPosition))
                    {
                        Plugin.BLogger.Info(LogCategory.Boss, $"Found valid spawn position after {i + 1} attempts. Distance from original: {math.distance(originalPosition, testPosition):F2} units");
                        return testPosition;
                    }
                }

                Plugin.BLogger.Warning(LogCategory.Boss, $"Could not find valid spawn position after {attempts} attempts within {searchRadius} units radius");
                return null;
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Error finding valid spawn position: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene una posición válida para spawn, verificando castillos y buscando alternativas si es necesario
        /// </summary>
        /// <param name="originalX">Coordenada X original</param>
        /// <param name="originalZ">Coordenada Z original</param>
        /// <returns>Tupla con las coordenadas válidas y si se usó la posición original</returns>
        private (float x, float z, bool usedOriginal) GetValidSpawnPosition(float originalX, float originalZ)
        {
            // Si la detección de castillos está deshabilitada, usar posición original
            if (!PluginConfig.EnableCastleDetection.Value)
            {
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name}: Castle detection disabled, using original position");
                return (originalX, originalZ, true);
            }
            
            var originalPosition = new float3(originalX, y, originalZ);
            
            // Verificar si la posición original está en un castillo
            if (!IsPositionInCastle(originalPosition))
            {
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name}: Original position is valid for spawn");
                return (originalX, originalZ, true);
            }

            Plugin.BLogger.Warning(LogCategory.Boss, $"Boss {name}: Original position is inside a castle territory, searching for alternative...");
            
            // Buscar posición alternativa
            var validPosition = FindValidSpawnPosition(originalPosition, 100f);
            
            if (validPosition.HasValue)
            {
                var newPos = validPosition.Value;
                Plugin.BLogger.Info(LogCategory.Boss, $"Boss {name}: Found alternative spawn position ({newPos.x:F2}, {newPos.z:F2})");
                return (newPos.x, newPos.z, false);
            }
            
            // Si no se encuentra posición válida, usar la original como último recurso
            Plugin.BLogger.Error(LogCategory.Boss, $"Boss {name}: Could not find valid spawn position, using original despite castle conflict");
            return (originalX, originalZ, false);
        }
        
        /// <summary>
        /// Refresca la AI sin cambiar la apariencia visual
        /// </summary>
        private void ForceAIRefreshPreservingAppearance(Entity entity)
        {
            try
            {
                var entityManager = Core.World.EntityManager;
                
                // Guardar PrefabGUID original para mantener apariencia
                var originalPrefabGUID = entity.Read<PrefabGUID>();
                
                // Refrescar componentes de AI/comportamiento
                if (entity.Has<AggroConsumer>())
                {
                    var aggro = entity.Read<AggroConsumer>();
                    entity.Remove<AggroConsumer>();
                    entity.Add<AggroConsumer>();
                    entity.Write(aggro);
                }
                
                // Asegurar que el PrefabGUID se mantiene original para la apariencia
                entity.Write(originalPrefabGUID);
                
                Plugin.BLogger.Info(LogCategory.Boss, "AI refreshed while preserving original appearance");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Error refreshing AI while preserving appearance: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Aplica intercambio de habilidades individuales desde VBloods
        /// </summary>
        private void ApplyIndividualAbilitySwaps(Entity bossEntity)
        {
            try
            {
                var entityManager = Core.World.EntityManager;
                Plugin.BLogger.Info(LogCategory.Boss, $"Starting individual ability swaps for boss {name}");
                
                foreach (var swap in AbilitySwaps)
                {
                    var slotIndex = swap.Key;
                    var config = swap.Value;
                    
                    Plugin.BLogger.Info(LogCategory.Boss, $"Processing ability swap for slot {slotIndex} from {config.SourceVBloodName}");
                    
                    // Obtener la entidad fuente
                    var sourcePrefabGUID = new PrefabGUID(config.SourcePrefabGUID);
                    if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(sourcePrefabGUID, out Entity sourceEntity))
                    {
                        Plugin.BLogger.Error(LogCategory.Boss, $"Source prefab {config.SourcePrefabGUID} not found for slot {slotIndex}");
                        continue;
                    }
                    
                    // Obtener las habilidades de la fuente
                    if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(sourceEntity))
                    {
                        Plugin.BLogger.Error(LogCategory.Boss, $"Source entity does not have ability buffer");
                        continue;
                    }
                    
                    var sourceBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                    
                    // Verificar que el slot existe en la fuente
                    if (slotIndex >= sourceBuffer.Length)
                    {
                        Plugin.BLogger.Error(LogCategory.Boss, $"Slot index {slotIndex} out of range for source (max: {sourceBuffer.Length - 1})");
                        continue;
                    }
                    
                    // Obtener o crear el buffer del boss
                    DynamicBuffer<AbilityGroupSlotBuffer> bossBuffer;
                    if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(bossEntity))
                    {
                        bossBuffer = entityManager.AddBuffer<AbilityGroupSlotBuffer>(bossEntity);
                        // Copiar todas las habilidades originales primero
                        var originalPrefab = new PrefabGUID(PrefabGUID);
                        if (Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(originalPrefab, out Entity originalEntity))
                        {
                            if (entityManager.HasBuffer<AbilityGroupSlotBuffer>(originalEntity))
                            {
                                var originalBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(originalEntity);
                                for (int i = 0; i < originalBuffer.Length; i++)
                                {
                                    bossBuffer.Add(originalBuffer[i]);
                                }
                            }
                        }
                    }
                    else
                    {
                        bossBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(bossEntity);
                    }
                    
                    // Verificar que el boss tiene suficientes slots
                    if (slotIndex >= bossBuffer.Length)
                    {
                        Plugin.BLogger.Error(LogCategory.Boss, $"Cannot swap slot {slotIndex} - boss only has {bossBuffer.Length} ability slots");
                        continue;
                    }
                    
                    // Verificar que la fuente tiene esa habilidad en ese slot
                    if (slotIndex >= sourceBuffer.Length)
                    {
                        Plugin.BLogger.Error(LogCategory.Boss, $"Source {config.SourceVBloodName} doesn't have ability at slot {slotIndex} (only has {sourceBuffer.Length} slots)");
                        continue;
                    }
                    
                    // Reemplazar la habilidad en el slot específico
                    bossBuffer[slotIndex] = sourceBuffer[slotIndex];
                    
                    Plugin.BLogger.Info(LogCategory.Boss, $"Successfully swapped ability at slot {slotIndex} with {config.Description}");
                }
                
                // Refrescar AI para activar las nuevas habilidades
                ForceAIRefreshPreservingAppearance(bossEntity);
                
                Plugin.BLogger.Info(LogCategory.Boss, $"Individual ability swaps completed successfully");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Error applying individual ability swaps: {ex.Message}");
            }
        }
        
        private void RemoveVBloodUnlockBuffer(Entity boss)
        {
            try
            {
                var entityManager = Plugin.SystemsCore.EntityManager;
                
                // Check if boss has VBloodUnlockTechBuffer
                if (entityManager.HasBuffer<VBloodUnlockTechBuffer>(boss))
                {
                    // Get the buffer and clear it
                    var unlockBuffer = entityManager.GetBuffer<VBloodUnlockTechBuffer>(boss);
                    int techCount = unlockBuffer.Length;
                    
                    // Contents logged before clearing (if debug needed)
                    
                    // Try removing the entire buffer component instead of just clearing it
                    entityManager.RemoveComponent<VBloodUnlockTechBuffer>(boss);
                    
                    Plugin.BLogger.Info(LogCategory.Boss, $"Removed entire VBloodUnlockTechBuffer component ({techCount} techs) from boss {name} to prevent VBlood rewards");
                }
                else
                {
                    Plugin.BLogger.Debug(LogCategory.Boss, $"Boss {name} doesn't have VBloodUnlockTechBuffer");
                }
                
                // Also remove GiveProgressionOnConsume component which gives VBlood rewards
                if (boss.Has<GiveProgressionOnConsume>())
                {
                    entityManager.RemoveComponent<GiveProgressionOnConsume>(boss);
                    Plugin.BLogger.Info(LogCategory.Boss, $"Removed GiveProgressionOnConsume component from boss {name} to prevent VBlood rewards");
                }
                else
                {
                    Plugin.BLogger.Debug(LogCategory.Boss, $"Boss {name} doesn't have GiveProgressionOnConsume");
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Error removing VBloodUnlockTechBuffer: {ex.Message}");
            }
        }
        
        private void LogBossComponents(Entity boss)
        {
            try
            {
                var entityManager = Plugin.SystemsCore.EntityManager;
                
                Plugin.BLogger.Warning(LogCategory.Boss, $"=== BOSS COMPONENTS DEBUG for {name} ===");
                Plugin.BLogger.Warning(LogCategory.Boss, $"Boss Entity: {boss.Index}:{boss.Version}");
                
                // Get all component types on the entity
                var componentTypes = entityManager.GetComponentTypes(boss, Unity.Collections.Allocator.Temp);
                Plugin.BLogger.Warning(LogCategory.Boss, $"=== ALL COMPONENTS ({componentTypes.Length}) ===");
                
                foreach (var componentType in componentTypes)
                {
                    Plugin.BLogger.Warning(LogCategory.Boss, $"  Component: {componentType.GetManagedType().Name}");
                }
                componentTypes.Dispose();
                
                // List all buffs
                if (entityManager.HasBuffer<BuffBuffer>(boss))
                {
                    var buffBuffer = entityManager.GetBuffer<BuffBuffer>(boss);
                    Plugin.BLogger.Warning(LogCategory.Boss, $"=== BOSS BUFFS ({buffBuffer.Length}) ===");
                    
                    for (int i = 0; i < buffBuffer.Length; i++)
                    {
                        var buff = buffBuffer[i];
                        if (entityManager.Exists(buff.Entity))
                        {
                            var buffName = "Unknown";
                            var buffGuid = 0;
                            
                            if (buff.Entity.Has<PrefabGUID>())
                            {
                                var buffPrefab = buff.Entity.Read<PrefabGUID>();
                                buffGuid = buffPrefab.GuidHash;
                                
                                try
                                {
                                    if (Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup.ContainsKey(buffPrefab))
                                    {
                                        buffName = Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup[buffPrefab].AssetName.ToString();
                                    }
                                }
                                catch { }
                            }
                            
                            Plugin.BLogger.Warning(LogCategory.Boss, $"  Buff [{i}]: {buffGuid} - {buffName}");
                            
                            // Log buff components
                            var buffComponentTypes = entityManager.GetComponentTypes(buff.Entity, Unity.Collections.Allocator.Temp);
                            Plugin.BLogger.Warning(LogCategory.Boss, $"    Buff Components ({buffComponentTypes.Length}):");
                            foreach (var buffCompType in buffComponentTypes)
                            {
                                Plugin.BLogger.Warning(LogCategory.Boss, $"      - {buffCompType.GetManagedType().Name}");
                            }
                            buffComponentTypes.Dispose();
                        }
                    }
                }
                
                // Check specific components we care about
                Plugin.BLogger.Warning(LogCategory.Boss, "=== SPECIFIC COMPONENTS CHECK ===");
                Plugin.BLogger.Warning(LogCategory.Boss, $"VBloodUnit: {(boss.Has<VBloodUnit>() ? "YES" : "NO")}");
                if (boss.Has<VBloodUnit>())
                {
                    var vblood = boss.Read<VBloodUnit>();
                    Plugin.BLogger.Warning(LogCategory.Boss, $"  - VBloodUnit type: {vblood.GetType().FullName}");
                    
                    // Use reflection to show ALL fields and properties
                    var type = vblood.GetType();
                    
                    // Get all fields (public, private, instance)
                    var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    Plugin.BLogger.Warning(LogCategory.Boss, $"  - VBloodUnit FIELDS ({fields.Length}):");
                    foreach (var field in fields)
                    {
                        try
                        {
                            var value = field.GetValue(vblood);
                            Plugin.BLogger.Warning(LogCategory.Boss, $"    * Field: {field.Name} ({field.FieldType.Name}) = {value}");
                            
                            // If it's a struct or complex type, show its contents too
                            if (value != null && !field.FieldType.IsPrimitive && field.FieldType != typeof(string))
                            {
                                var valueType = value.GetType();
                                if (valueType.IsValueType && !valueType.IsEnum)
                                {
                                    var valueFields = valueType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                    foreach (var vf in valueFields)
                                    {
                                        try
                                        {
                                            var vfValue = vf.GetValue(value);
                                            Plugin.BLogger.Warning(LogCategory.Boss, $"      -> {vf.Name}: {vfValue}");
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.BLogger.Warning(LogCategory.Boss, $"    * {field.Name}: Error reading - {ex.Message}");
                        }
                    }
                    
                    // Also check properties
                    var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    Plugin.BLogger.Warning(LogCategory.Boss, $"  - VBloodUnit PROPERTIES ({properties.Length}):");
                    foreach (var prop in properties)
                    {
                        try
                        {
                            if (prop.CanRead)
                            {
                                var value = prop.GetValue(vblood);
                                Plugin.BLogger.Warning(LogCategory.Boss, $"    * Property: {prop.Name} ({prop.PropertyType.Name}) = {value}");
                            }
                            else
                            {
                                Plugin.BLogger.Warning(LogCategory.Boss, $"    * Property: {prop.Name} ({prop.PropertyType.Name}) [Write-Only]");
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.BLogger.Warning(LogCategory.Boss, $"    * {prop.Name}: Error reading - {ex.Message}");
                        }
                    }
                    
                    // Show base type info
                    if (type.BaseType != null && type.BaseType != typeof(object))
                    {
                        Plugin.BLogger.Warning(LogCategory.Boss, $"  - VBloodUnit BASE TYPE: {type.BaseType.FullName}");
                    }
                    
                    // Show implemented interfaces
                    var interfaces = type.GetInterfaces();
                    if (interfaces.Length > 0)
                    {
                        Plugin.BLogger.Warning(LogCategory.Boss, $"  - VBloodUnit INTERFACES ({interfaces.Length}):");
                        foreach (var iface in interfaces)
                        {
                            Plugin.BLogger.Warning(LogCategory.Boss, $"    * {iface.Name}");
                        }
                    }
                }
                Plugin.BLogger.Warning(LogCategory.Boss, $"BloodConsumeSource: {(boss.Has<BloodConsumeSource>() ? "YES" : "NO")}");
                if (boss.Has<BloodConsumeSource>())
                {
                    var bloodSource = boss.Read<BloodConsumeSource>();
                    Plugin.BLogger.Warning(LogCategory.Boss, $"  - CanBeConsumed: {bloodSource.CanBeConsumed}");
                }
                Plugin.BLogger.Warning(LogCategory.Boss, $"VBloodConsumed: {(boss.Has<VBloodConsumed>() ? "YES" : "NO")}");
                Plugin.BLogger.Warning(LogCategory.Boss, $"Interactable: {(boss.Has<Interactable>() ? "YES" : "NO")}");
                Plugin.BLogger.Warning(LogCategory.Boss, $"NameableInteractable: {(boss.Has<NameableInteractable>() ? "YES" : "NO")}");
                if (boss.Has<NameableInteractable>())
                {
                    var nameable = boss.Read<NameableInteractable>();
                    Plugin.BLogger.Warning(LogCategory.Boss, $"  - Name: {nameable.Name}");
                }
                
                // Check VBloodUnlockTechBuffer
                if (entityManager.HasBuffer<VBloodUnlockTechBuffer>(boss))
                {
                    var unlockBuffer = entityManager.GetBuffer<VBloodUnlockTechBuffer>(boss);
                    Plugin.BLogger.Warning(LogCategory.Boss, $"VBloodUnlockTechBuffer: YES ({unlockBuffer.Length} techs) - SHOULD BE CLEARED!");
                    
                    // Log contents of the buffer
                    for (int i = 0; i < unlockBuffer.Length; i++)
                    {
                        var unlock = unlockBuffer[i];
                        Plugin.BLogger.Warning(LogCategory.Boss, $"  Tech [{i}]: {unlock.Guid.GuidHash}");
                    }
                }
                else
                {
                    Plugin.BLogger.Warning(LogCategory.Boss, "VBloodUnlockTechBuffer: NO (Component removed) ✅");
                }
                
                
                Plugin.BLogger.Warning(LogCategory.Boss, "=== END BOSS COMPONENTS DEBUG ===");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Error logging boss components: {ex.Message}");
            }
        }
        
    }
    
    /// <summary>
    /// Configuration for ability swapping from VBloods
    /// </summary>
    public class AbilitySwapConfig
    {
        public int SourcePrefabGUID { get; set; }
        public string SourceVBloodName { get; set; }
        public int SlotIndex { get; set; }
        public string Description { get; set; }
    }
}
