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

namespace BloodyBoss.DB.Models
{
    internal class BossEncounterModel
    {
        private Entity iconEntity;

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
        
        // Ability Swap Properties
        public int? AbilitySwapPrefabGUID { get; set; } = null;
        
        // Modular Ability System Properties
        public Dictionary<string, CustomAbilitySlot> CustomAbilities { get; set; } = new Dictionary<string, CustomAbilitySlot>();
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
                Plugin.Logger.LogInfo($"Boss {name}: Spawn position adjusted to avoid castle territory");
            }
            
            // Mantener siempre el PrefabGUID original para la apariencia
            var spawnPrefabGUID = new PrefabGUID(PrefabGUID);
            
            SpawnSystem.SpawnUnitWithCallback(sender, spawnPrefabGUID, new float3(validX, y, validZ), Lifetime, (Entity e) => {
                
                bossEntity = e;
                ModifyBoss(sender, e);
                
                // Log spawn position once after boss is created
                Plugin.Logger.LogInfo($"Boss {name}: Spawned at position ({validX:F2}, {y:F2}, {validZ:F2})");
                
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
                Plugin.Logger.LogInfo($"Boss {name}: Spawn position adjusted to avoid castle territory");
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
            Plugin.Logger.LogInfo($"Boss {name} dropping items to {killers.Count} killers");
            
            if (killers.Count == 0)
            {
                Plugin.Logger.LogWarning($"Boss {name} has no killers registered, no items will drop");
                return false;
            }

            foreach (var item in items)
            {
                if (probabilityGeneratingReward(item.Chance))
                {
                    Plugin.Logger.LogInfo($"Boss {name} dropping item {item.ItemID} x{item.Stack}");

                    foreach (var user in killers)
                    {
                        try
                        {
                            UserModel player = GameData.Users.GetUserByCharacterName(user);
                            if (player == null)
                            {
                                Plugin.Logger.LogWarning($"Player {user} not found for item drop");
                                continue;
                            }

                            var itemGuid = new PrefabGUID(item.ItemID);
                            var stacks = item.Stack;

                            if(!player.TryGiveItem(itemGuid, stacks, out Entity itemEntity)){
                                player.DropItemNearby(itemGuid, stacks);
                                Plugin.Logger.LogInfo($"Item dropped near player {user}");
                            } else {
                                Plugin.Logger.LogInfo($"Item given to player {user}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.Logger.LogError($"Error dropping item to player {user}: {ex.Message}");
                        }
                    }
                } else {
                    Plugin.Logger.LogInfo($"Boss {name} item {item.ItemID} failed probability check ({item.Chance}%)");
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
                Plugin.Logger.LogInfo($"Boss {name} using dynamic scaling: x{healthMultiplier:F2}");
            }
            else if (PluginConfig.PlayersMultiplier.Value)
            {
                // Use legacy player multiplier system
                healthMultiplier = players * multiplier;
                Plugin.Logger.LogInfo($"Boss {name} using legacy scaling: {players} players x {multiplier} = x{healthMultiplier:F2}");
            }
            else
            {
                // Use base multiplier only
                Plugin.Logger.LogInfo($"Boss {name} using base multiplier: x{healthMultiplier:F2}");
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
                    Plugin.Logger.LogInfo($"Boss {name} consecutive spawns: {ConsecutiveSpawns}");
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
            
            // Aplicar intercambio de habilidades si está configurado
            if (AbilitySwapPrefabGUID.HasValue)
            {
                Plugin.Logger.LogInfo($"Boss {name}: Applying ability swap to {AbilitySwapPrefabGUID.Value} while maintaining visual appearance");
                ApplyAbilitySwapPostSpawn(boss, new PrefabGUID(AbilitySwapPrefabGUID.Value));
            }
            
            // Aplicar sistema modular de habilidades personalizadas
            if (CustomAbilities.Count > 0)
            {
                Plugin.Logger.LogInfo($"Boss {name}: Applying modular ability system with {CustomAbilities.Count} custom slots");
                ApplyModularAbilitiesPostSpawn(boss);
            }
            
            // Aplicar intercambio de habilidades individuales (nuevo sistema)
            if (AbilitySwaps != null && AbilitySwaps.Count > 0)
            {
                Plugin.Logger.LogInfo($"Boss {name}: Applying individual ability swaps for {AbilitySwaps.Count} slots");
                ApplyIndividualAbilitySwaps(boss);
            }
            
            // Initialize boss mechanics
            if (Mechanics.Count > 0)
            {
                Plugin.Logger.LogInfo($"Boss {name}: Initializing {Mechanics.Count} mechanics");
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

            // Unregister boss from damage tracking
            if (GetBossEntity())
            {
                BossGameplayEventSystem.UnregisterBoss(bossEntity);
            }
            
            bossSpawn = false;
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
                        Plugin.Logger.LogInfo("Spawn Random Boss");
                        bossRandom = Database.BOSSES[Random.Next(Database.BOSSES.Count)];
                        // Use the random boss position with its own Y coordinate
                        bossRandom.Spawn(user, bossRandom.x, bossRandom.y, bossRandom.z);
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

                // Reset progressive difficulty on successful kill
                if (PluginConfig.EnableProgressiveDifficulty.Value && PluginConfig.ResetDifficultyOnKill.Value)
                {
                    ConsecutiveSpawns = 0;
                    CurrentDifficultyMultiplier = 1.0f;
                    LastSuccessfulKill = DateTime.Now;
                    Plugin.Logger.LogInfo($"Boss {name} difficulty reset after successful kill");
                }

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
                Plugin.Logger.LogError($"Error checking castle position: {ex.Message}");
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
                        Plugin.Logger.LogInfo($"Found valid spawn position after {i + 1} attempts. Distance from original: {math.distance(originalPosition, testPosition):F2} units");
                        return testPosition;
                    }
                }

                Plugin.Logger.LogWarning($"Could not find valid spawn position after {attempts} attempts within {searchRadius} units radius");
                return null;
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error finding valid spawn position: {ex.Message}");
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
                Plugin.Logger.LogInfo($"Boss {name}: Castle detection disabled, using original position");
                return (originalX, originalZ, true);
            }
            
            var originalPosition = new float3(originalX, y, originalZ);
            
            // Verificar si la posición original está en un castillo
            if (!IsPositionInCastle(originalPosition))
            {
                Plugin.Logger.LogInfo($"Boss {name}: Original position is valid for spawn");
                return (originalX, originalZ, true);
            }

            Plugin.Logger.LogWarning($"Boss {name}: Original position is inside a castle territory, searching for alternative...");
            
            // Buscar posición alternativa
            var validPosition = FindValidSpawnPosition(originalPosition, 100f);
            
            if (validPosition.HasValue)
            {
                var newPos = validPosition.Value;
                Plugin.Logger.LogInfo($"Boss {name}: Found alternative spawn position ({newPos.x:F2}, {newPos.z:F2})");
                return (newPos.x, newPos.z, false);
            }
            
            // Si no se encuentra posición válida, usar la original como último recurso
            Plugin.Logger.LogError($"Boss {name}: Could not find valid spawn position, using original despite castle conflict");
            return (originalX, originalZ, false);
        }
        
        /// <summary>
        /// Aplica intercambio de habilidades manteniendo la apariencia visual original
        /// </summary>
        private void ApplyAbilitySwapPostSpawn(Entity bossEntity, PrefabGUID sourcePrefabGUID)
        {
            try
            {
                // Obtener la entidad fuente del prefab
                if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(sourcePrefabGUID, out Entity sourceEntity))
                {
                    Plugin.Logger.LogError($"Source prefab {sourcePrefabGUID.GuidHash} not found for ability swap");
                    return;
                }
                
                Plugin.Logger.LogInfo($"Applying ability swap from {sourcePrefabGUID.GuidHash} to boss while preserving visual appearance");
                
                // Usar el sistema existente pero preservando la apariencia
                var entityManager = Core.World.EntityManager;
                
                // 1. Copiar AbilityBar_Shared
                if (sourceEntity.Has<AbilityBar_Shared>())
                {
                    var sourceAbilityBar = sourceEntity.Read<AbilityBar_Shared>();
                    if (!bossEntity.Has<AbilityBar_Shared>())
                    {
                        bossEntity.Add<AbilityBar_Shared>();
                    }
                    bossEntity.Write(sourceAbilityBar);
                    Plugin.Logger.LogInfo("Applied AbilityBar_Shared from source");
                }
                
                // 2. Copiar AbilityGroupSlotBuffer
                if (entityManager.HasBuffer<AbilityGroupSlotBuffer>(sourceEntity))
                {
                    var sourceBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                    DynamicBuffer<AbilityGroupSlotBuffer> targetBuffer;
                    
                    if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(bossEntity))
                    {
                        targetBuffer = entityManager.AddBuffer<AbilityGroupSlotBuffer>(bossEntity);
                    }
                    else
                    {
                        targetBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(bossEntity);
                        targetBuffer.Clear();
                    }
                    
                    for (int i = 0; i < sourceBuffer.Length; i++)
                    {
                        targetBuffer.Add(sourceBuffer[i]);
                    }
                    Plugin.Logger.LogInfo($"Applied {sourceBuffer.Length} ability groups from source");
                }
                
                // 3. Forzar refresh de AI manteniendo la apariencia
                ForceAIRefreshPreservingAppearance(bossEntity);
                
                Plugin.Logger.LogInfo("Ability swap applied successfully while preserving visual appearance");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error applying ability swap post-spawn: {ex.Message}");
            }
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
                
                Plugin.Logger.LogInfo("AI refreshed while preserving original appearance");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error refreshing AI while preserving appearance: {ex.Message}");
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
                Plugin.Logger.LogInfo($"Starting individual ability swaps for boss {name}");
                
                foreach (var swap in AbilitySwaps)
                {
                    var slotIndex = swap.Key;
                    var config = swap.Value;
                    
                    Plugin.Logger.LogInfo($"Processing ability swap for slot {slotIndex} from {config.SourceVBloodName}");
                    
                    // Obtener la entidad fuente
                    var sourcePrefabGUID = new PrefabGUID(config.SourcePrefabGUID);
                    if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(sourcePrefabGUID, out Entity sourceEntity))
                    {
                        Plugin.Logger.LogError($"Source prefab {config.SourcePrefabGUID} not found for slot {slotIndex}");
                        continue;
                    }
                    
                    // Obtener las habilidades de la fuente
                    if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(sourceEntity))
                    {
                        Plugin.Logger.LogError($"Source entity does not have ability buffer");
                        continue;
                    }
                    
                    var sourceBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                    
                    // Verificar que el slot existe en la fuente
                    if (slotIndex >= sourceBuffer.Length)
                    {
                        Plugin.Logger.LogError($"Slot index {slotIndex} out of range for source (max: {sourceBuffer.Length - 1})");
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
                    
                    // Asegurar que el buffer tiene suficiente tamaño
                    while (bossBuffer.Length <= slotIndex)
                    {
                        bossBuffer.Add(new AbilityGroupSlotBuffer());
                    }
                    
                    // Reemplazar la habilidad en el slot específico
                    bossBuffer[slotIndex] = sourceBuffer[slotIndex];
                    
                    Plugin.Logger.LogInfo($"Successfully swapped ability at slot {slotIndex} with {config.Description}");
                }
                
                // Refrescar AI para activar las nuevas habilidades
                ForceAIRefreshPreservingAppearance(bossEntity);
                
                Plugin.Logger.LogInfo($"Individual ability swaps completed successfully");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error applying individual ability swaps: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Aplica el sistema modular de habilidades personalizadas
        /// </summary>
        private void ApplyModularAbilitiesPostSpawn(Entity bossEntity)
        {
            try
            {
                var entityManager = Core.World.EntityManager;
                Plugin.Logger.LogInfo($"Starting modular ability application for boss {name}");
                
                // Crear o obtener el buffer de habilidades
                DynamicBuffer<AbilityGroupSlotBuffer> abilityBuffer;
                if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(bossEntity))
                {
                    abilityBuffer = entityManager.AddBuffer<AbilityGroupSlotBuffer>(bossEntity);
                }
                else
                {
                    abilityBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(bossEntity);
                    abilityBuffer.Clear();
                }
                
                // Aplicar habilidades personalizadas por slot
                foreach (var slot in CustomAbilities)
                {
                    if (!slot.Value.Enabled)
                    {
                        Plugin.Logger.LogInfo($"Skipping disabled slot: {slot.Key}");
                        continue;
                    }
                    
                    Plugin.Logger.LogInfo($"Processing slot {slot.Key}: PrefabGUID={slot.Value.SourcePrefabGUID}, Index={slot.Value.AbilityIndex}");
                    
                    // Obtener la entidad fuente
                    var sourcePrefabGUID = new PrefabGUID(slot.Value.SourcePrefabGUID);
                    if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(sourcePrefabGUID, out Entity sourceEntity))
                    {
                        Plugin.Logger.LogError($"Source prefab {slot.Value.SourcePrefabGUID} not found for slot {slot.Key}");
                        continue;
                    }
                    
                    // Obtener las habilidades de la fuente
                    if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(sourceEntity))
                    {
                        Plugin.Logger.LogError($"Source entity does not have ability buffer for slot {slot.Key}");
                        continue;
                    }
                    
                    var sourceBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                    
                    // Verificar que el índice sea válido
                    if (slot.Value.AbilityIndex >= sourceBuffer.Length)
                    {
                        Plugin.Logger.LogError($"Ability index {slot.Value.AbilityIndex} out of range for slot {slot.Key} (max: {sourceBuffer.Length - 1})");
                        continue;
                    }
                    
                    // Copiar la habilidad específica
                    var abilitySlot = sourceBuffer[slot.Value.AbilityIndex];
                    abilityBuffer.Add(abilitySlot);
                    
                    Plugin.Logger.LogInfo($"Successfully copied ability {slot.Value.AbilityIndex} from {slot.Value.SourcePrefabGUID} to slot {slot.Key}");
                }
                
                // Actualizar AbilityBar_Shared si hay habilidades configuradas
                if (abilityBuffer.Length > 0)
                {
                    // Intentar obtener AbilityBar_Shared de la primera fuente como base
                    var firstSlot = CustomAbilities.Values.FirstOrDefault(s => s.Enabled);
                    if (firstSlot != null)
                    {
                        var firstSourcePrefab = new PrefabGUID(firstSlot.SourcePrefabGUID);
                        if (Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(firstSourcePrefab, out Entity firstSourceEntity))
                        {
                            if (firstSourceEntity.Has<AbilityBar_Shared>())
                            {
                                var sourceAbilityBar = firstSourceEntity.Read<AbilityBar_Shared>();
                                if (!bossEntity.Has<AbilityBar_Shared>())
                                {
                                    bossEntity.Add<AbilityBar_Shared>();
                                }
                                bossEntity.Write(sourceAbilityBar);
                                Plugin.Logger.LogInfo("Applied AbilityBar_Shared from modular system");
                            }
                        }
                    }
                }
                
                // Refrescar AI para activar las nuevas habilidades
                ForceAIRefreshPreservingAppearance(bossEntity);
                
                Plugin.Logger.LogInfo($"Modular ability system applied successfully: {abilityBuffer.Length} abilities configured");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error applying modular abilities: {ex.Message}");
            }
        }
        
    }
    
    /// <summary>
    /// Represents a custom ability slot configuration
    /// </summary>
    public class CustomAbilitySlot
    {
        public int SourcePrefabGUID { get; set; } = 0;
        public int AbilityIndex { get; set; } = 0;
        public bool Enabled { get; set; } = true;
        public string Description { get; set; } = "";
        
        public CustomAbilitySlot() { }
        
        public CustomAbilitySlot(int sourcePrefabGUID, int abilityIndex, bool enabled = true, string description = "")
        {
            SourcePrefabGUID = sourcePrefabGUID;
            AbilityIndex = abilityIndex;
            Enabled = enabled;
            Description = description;
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
