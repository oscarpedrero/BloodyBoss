using System;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using ProjectM;
using ProjectM.Network;
using ProjectM.Gameplay.Systems;
using ProjectM.Shared;
using ProjectM.Gameplay.Scripting;
using Stunlock.Core;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Configuration;
using BloodyBoss.Components;
using BloodyBoss.Systems;
using BloodyBoss.Utils;
using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.Patch.Server;

namespace BloodyBoss.Factory
{
    /// <summary>
    /// Centralized factory for creating and configuring boss entities
    /// </summary>
    internal static class BossEntityFactory
    {
        /// <summary>
        /// Create a boss entity with all necessary configuration
        /// </summary>
        public static void CreateBoss(BossEncounterModel model, float3 position, Action<Entity> onComplete = null)
        {
            var sender = Plugin.SystemsCore.EntityManager.CreateEntity();
            CreateBoss(model, sender, position, onComplete);
        }
        
        /// <summary>
        /// Create a boss entity with specified sender
        /// </summary>
        public static void CreateBoss(BossEncounterModel model, Entity sender, float3 position, Action<Entity> onComplete = null)
        {
            // Use the position as-is, validation is done by BossEncounterModel.Spawn
            var finalPosition = position;
            
            // Create spawn configuration
            var spawnConfig = new BossSpawnConfig
            {
                Model = model,
                Position = finalPosition,
                Sender = sender,
                LifetimeDuration = model.Lifetime + 30
            };
            
            // Spawn the boss entity
            var spawnPrefabGUID = new PrefabGUID(model.PrefabGUID);
            SpawnSystem.SpawnUnitWithCallback(sender, spawnPrefabGUID, finalPosition, spawnConfig.LifetimeDuration, (Entity bossEntity) => 
            {
                // Configure the boss entity
                ConfigureBossEntity(bossEntity, model, spawnConfig);
                
                // Set up post-spawn actions
                SetupPostSpawnActions(bossEntity, model, finalPosition);
                
                // Initialize boss state in cache
                BossComponentsHelper.GetOrCreateBossState(bossEntity, model);
                
                // Notify completion
                onComplete?.Invoke(bossEntity);
            });
        }
        
        /// <summary>
        /// Configure all boss entity components
        /// </summary>
        private static void ConfigureBossEntity(Entity bossEntity, BossEncounterModel model, BossSpawnConfig config)
        {
            // Store boss entity reference
            model.bossEntity = bossEntity;
            
            // Configure name and identification
            ConfigureBossName(bossEntity, model);
            
            // Configure level
            ConfigureBossLevel(bossEntity, model);
            
            // Configure health and stats
            ConfigureBossStats(bossEntity, model);
            
            // Configure additional properties
            ConfigureAdditionalProperties(bossEntity, model);
            
            // Apply world boss buff from config
            BuffSystem.BuffNPC(bossEntity, config.Sender, new PrefabGUID(PluginConfig.BuffForWorldBoss.Value), 0);
            
            // Clear drop table if configured
            if (PluginConfig.ClearDropTable.Value)
            {
                var action = () =>
                {
                    ClearDropTable(bossEntity);
                };
                ActionScheduler.RunActionOnceAfterDelay(action, 0.2); // Small delay
            }
            
            // Initialize boss mechanics
            BossMechanicSystem.InitializeBossMechanics(bossEntity, model);
            
            // Boss will be registered for damage tracking when first damage event occurs
            // This ensures proper entity reference stability
            
            // Set boss spawn state
            model.bossSpawn = true;
            model.LastSpawn = DateTime.Now;
            
            Plugin.BLogger.Info(LogCategory.Boss, $"Boss {model.name}: Entity configured successfully");
        }
        
        /// <summary>
        /// Configure boss name with proper hash
        /// </summary>
        private static void ConfigureBossName(Entity bossEntity, BossEncounterModel model)
        {
            // Add NameableInteractable component if not present (like RenameBoss does)
            if (!bossEntity.Has<NameableInteractable>())
            {
                bossEntity.Add<NameableInteractable>();
            }
            
            var nameable = bossEntity.Read<NameableInteractable>();
            nameable.Name = new FixedString64Bytes(model.nameHash + "bb");
            bossEntity.Write(nameable);
        }
        
        /// <summary>
        /// Configure boss level
        /// </summary>
        private static void ConfigureBossLevel(Entity bossEntity, BossEncounterModel model)
        {
            if (bossEntity.Has<UnitLevel>())
            {
                var unitLevel = bossEntity.Read<UnitLevel>();
                unitLevel.Level = new ModifiableInt(model.level);
                bossEntity.Write(unitLevel);
            }
        }
        
        /// <summary>
        /// Configure boss health and combat stats
        /// </summary>
        private static void ConfigureBossStats(Entity bossEntity, BossEncounterModel model)
        {
            // Calculate multipliers
            float healthMultiplier = CalculateHealthMultiplier(model);
            float damageMultiplier = CalculateDamageMultiplier(model);
            
            // Apply health scaling
            if (bossEntity.Has<Health>())
            {
                var health = bossEntity.Read<Health>();
                health.MaxHealth = new ModifiableFloat(health.MaxHealth.Value * healthMultiplier);
                health.Value = health.MaxHealth.Value;
                bossEntity.Write(health);
            }
            
            // Apply unit stats
            if (bossEntity.Has<UnitStats>() && model.unitStats != null)
            {
                var unitStats = bossEntity.Read<UnitStats>();
                
                // Apply custom stats from model
                unitStats = model.unitStats.FillStats(unitStats);
                
                // Apply damage multiplier
                unitStats.PhysicalPower._Value *= damageMultiplier;
                unitStats.SpellPower._Value *= damageMultiplier;
                
                bossEntity.Write(unitStats);
            }
            
            Plugin.BLogger.Info(LogCategory.Boss, $"Boss {model.name}: Health x{healthMultiplier:F2}, Damage x{damageMultiplier:F2}");
        }
        
        /// <summary>
        /// Calculate health multiplier based on configuration
        /// </summary>
        private static float CalculateHealthMultiplier(BossEncounterModel model)
        {
            if (PluginConfig.EnableDynamicScaling.Value)
            {
                return DynamicScalingSystem.CalculateHealthMultiplier(model);
            }
            else
            {
                // Legacy calculation
                var players = GameData.Users.Online.ToList().Count;
                return model.multiplier * (players + 1);
            }
        }
        
        /// <summary>
        /// Calculate damage multiplier based on configuration
        /// </summary>
        private static float CalculateDamageMultiplier(BossEncounterModel model)
        {
            if (PluginConfig.EnableDynamicScaling.Value)
            {
                // Use dynamic scaling calculation
                var players = GameData.Users.Online.Count();
                var effectivePlayers = Math.Max(0, players - 1);
                var damageMultiplier = 1.0f + (effectivePlayers * PluginConfig.DamagePerPlayer.Value);
                
                // Apply progressive difficulty if enabled
                if (PluginConfig.EnableProgressiveDifficulty.Value)
                {
                    damageMultiplier *= model.CurrentDifficultyMultiplier;
                }
                
                return damageMultiplier;
            }
            else
            {
                // Use base difficulty multiplier
                return model.CurrentDifficultyMultiplier;
            }
        }
        
        /// <summary>
        /// Configure additional boss properties
        /// </summary>
        private static void ConfigureAdditionalProperties(Entity bossEntity, BossEncounterModel model)
        {
            // Set faction/team if needed
            var action = () => 
            {
                if (Plugin.SystemsCore.EntityManager.Exists(bossEntity))
                {
                    BossSystem.CheckTeams(bossEntity);
                }
            };
            ActionScheduler.RunActionOnceAfterFrames(action, new System.Random().Next(60));
            
            // Note: FleeOnDamage component removal was attempted here in original code
            // but the component doesn't exist in current V Rising version
            
            // Set aggro if configured
            if (bossEntity.Has<AggroConsumer>())
            {
                var aggroConsumer = bossEntity.Read<AggroConsumer>();
                aggroConsumer.MaxDistanceFromPreCombatPosition = 200;
                bossEntity.Write(aggroConsumer);
            }
        }
        
        /// <summary>
        /// Set up post-spawn actions
        /// </summary>
        private static void SetupPostSpawnActions(Entity bossEntity, BossEncounterModel model, float3 position)
        {
            var entityIndex = bossEntity.Index;
            var entityVersion = bossEntity.Version;
            
            // Add map icon after delay
            var actionIcon = () =>
            {
                AddMapIcon(bossEntity, model, position.x, position.z);
            };
            ActionScheduler.RunActionOnceAfterDelay(actionIcon, 3);
            
            // Log spawn position after small delay - using ActionScheduler to avoid "different world" errors
            var actionPosition = () =>
            {
                if (Plugin.SystemsCore.EntityManager.Exists(bossEntity) && Plugin.SystemsCore.EntityManager.HasComponent<LocalToWorld>(bossEntity))
                {
                    var ltw = Plugin.SystemsCore.EntityManager.GetComponentData<LocalToWorld>(bossEntity);
                    Plugin.BLogger.Info(LogCategory.Boss, $"Boss {model.name}: Final position ({ltw.Position.x:F2}, {ltw.Position.y:F2}, {ltw.Position.z:F2})");
                }
            };
            ActionScheduler.RunActionOnceAfterDelay(actionPosition, 0.1);
            
            // Send spawn announcement
            SendSpawnAnnouncement(model);
        }
        
        /// <summary>
        /// Clear drop table for boss
        /// </summary>
        private static void ClearDropTable(Entity boss)
        {
            try
            {
                if (boss.Has<DropTableBuffer>())
                {
                    var dropTableBuffer = boss.ReadBuffer<DropTableBuffer>();
                    dropTableBuffer.Clear();
                    Plugin.BLogger.Info(LogCategory.Boss, "Boss drop table cleared");
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Failed to clear drop table: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Add map icon for boss
        /// </summary>
        private static void AddMapIcon(Entity bossEntity, BossEncounterModel model, float x, float z)
        {
            try
            {
                SpawnSystem.SpawnUnitWithCallback(bossEntity, Prefabs.MapIcon_POI_VBloodSource, 
                    new float2(x, z), model.Lifetime + 5, (Entity e) => 
                {
                    // Store icon entity
                    e.Add<MapIconData>();
                    e.Add<MapIconTargetEntity>();
                    var mapIconTargetEntity = e.Read<MapIconTargetEntity>();
                    mapIconTargetEntity.TargetEntity = NetworkedEntity.ServerEntity(bossEntity);
                    
                    // Get NetworkId using EntityManager to avoid "different world" error
                    var entityManager = Plugin.SystemsCore.EntityManager;
                    if (entityManager.Exists(bossEntity) && entityManager.HasComponent<NetworkId>(bossEntity))
                    {
                        mapIconTargetEntity.TargetNetworkId = entityManager.GetComponentData<NetworkId>(bossEntity);
                    }
                    
                    e.Write(mapIconTargetEntity);
                    e.Add<NameableInteractable>();
                    var nameable = e.Read<NameableInteractable>();
                    nameable.Name = new FixedString64Bytes(model.nameHash + "ibb");
                    e.Write(nameable);
                    
                    Plugin.BLogger.Info(LogCategory.Boss, $"Boss {model.name}: Map icon added");
                });
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Failed to add map icon: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Send spawn announcement to all players
        /// </summary>
        private static void SendSpawnAnnouncement(BossEncounterModel model)
        {
            var message = PluginConfig.SpawnMessageBossTemplate.Value;
            message = message.Replace("#time#", FontColorChatSystem.Yellow($"{model.Lifetime / 60}"));
            message = message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{model.name}"));
            
            var formattedMessage = (FixedString512Bytes)FontColorChatSystem.Green($"{message}");
            ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref formattedMessage);
        }
        
        /// <summary>
        /// Internal spawn configuration
        /// </summary>
        private struct BossSpawnConfig
        {
            public BossEncounterModel Model;
            public float3 Position;
            public Entity Sender;
            public float LifetimeDuration;
        }
    }
}