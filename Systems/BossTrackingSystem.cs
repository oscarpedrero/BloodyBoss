using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Collections;
using BloodyBoss.DB.Models;
using BloodyBoss.DB;
using BloodyBoss.Configuration;
using Bloody.Core;
using Bloody.Core.Patch.Server;
using Bloody.Core.API.v1;
using Bloody.Core.Helper.v1;
using ProjectM;
using ProjectM.Network;

namespace BloodyBoss.Systems
{
    /// <summary>
    /// Optimized boss tracking system to avoid O(n²) performance issues
    /// </summary>
    internal static class BossTrackingSystem
    {
        // Active bosses indexed by entity for O(1) lookup
        private static readonly Dictionary<Entity, TrackedBoss> _activeBosses = new Dictionary<Entity, TrackedBoss>();
        
        // Quick lookup by boss name for spawn/despawn operations
        private static readonly Dictionary<string, Entity> _bossesByName = new Dictionary<string, Entity>();
        
        // Track last update time to avoid unnecessary checks
        private static DateTime _lastUpdateTime = DateTime.UtcNow;
        
        // Last update time to batch operations
        
        private class TrackedBoss
        {
            public Entity Entity { get; set; }
            public BossEncounterModel Model { get; set; }
            public DateTime NextDespawnCheck { get; set; }
            public DateTime NextMechanicCheck { get; set; }
            public DateTime SpawnTime { get; set; }
            public bool NeedsTeamCheck { get; set; }
        }
        
        /// <summary>
        /// Register a spawned boss for tracking
        /// </summary>
        public static void RegisterSpawnedBoss(Entity entity, BossEncounterModel model)
        {
            if (_activeBosses.ContainsKey(entity))
            {
                Plugin.Logger.LogWarning($"[BossTracking] Boss {model.name} already registered, updating");
            }
            
            var tracked = new TrackedBoss
            {
                Entity = entity,
                Model = model,
                SpawnTime = DateTime.UtcNow,
                NextDespawnCheck = DateTime.UtcNow.AddSeconds(5), // Check every 5 seconds
                NextMechanicCheck = DateTime.UtcNow.AddSeconds(1), // Check mechanics every second
                NeedsTeamCheck = true
            };
            
            _activeBosses[entity] = tracked;
            _bossesByName[model.name] = entity;
            
            Plugin.Logger.LogInfo($"[BossTracking] Registered boss {model.name} for optimized tracking. Active bosses: {_activeBosses.Count}");
        }
        
        /// <summary>
        /// Unregister a boss (death/despawn)
        /// </summary>
        public static void UnregisterBoss(Entity entity)
        {
            if (_activeBosses.TryGetValue(entity, out var tracked))
            {
                _bossesByName.Remove(tracked.Model.name);
                _activeBosses.Remove(entity);
                Plugin.Logger.LogInfo($"[BossTracking] Unregistered boss {tracked.Model.name}. Active bosses: {_activeBosses.Count}");
            }
        }
        
        /// <summary>
        /// Get boss by name
        /// </summary>
        public static bool TryGetBossByName(string name, out Entity entity)
        {
            return _bossesByName.TryGetValue(name, out entity);
        }
        
        /// <summary>
        /// Optimized update - only check what needs checking
        /// </summary>
        public static void UpdateActiveBosses()
        {
            // Schedule the update on the main thread to avoid thread safety issues
            ActionScheduler.RunActionOnMainThread(() =>
            {
                try
                {
                    UpdateActiveBossesInternal();
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogError($"[BossTracking] Error in UpdateActiveBosses: {ex.Message}");
                }
            });
        }
        
        private static void UpdateActiveBossesInternal()
        {
            var now = DateTime.UtcNow;
            var entitiesToRemove = new List<Entity>();
            var entityManager = Plugin.SystemsCore.EntityManager;
            
            foreach (var kvp in _activeBosses)
            {
                var entity = kvp.Key;
                var tracked = kvp.Value;
                
                // Validate entity still exists
                if (!entityManager.Exists(entity))
                {
                    Plugin.Logger.LogInfo($"[BossTracking] Boss {tracked.Model.name} entity no longer exists (despawned by LifeTime)");
                    entitiesToRemove.Add(entity);
                    
                    // Perform cleanup since entity was despawned (not killed)
                    tracked.Model.bossSpawn = false;
                    tracked.Model.bossEntity = Entity.Null;
                    BossGameplayEventSystem.UnregisterBoss(entity);
                    BossMechanicSystem.CleanupBossMechanics(tracked.Model);
                    Database.saveDatabase();
                    
                    // Send despawn message if configured
                    if (!string.IsNullOrEmpty(PluginConfig.DespawnMessageBossTemplate.Value))
                    {
                        var message = PluginConfig.DespawnMessageBossTemplate.Value;
                        message = message.Replace("#worldbossname#", FontColorChatSystem.Yellow($"{tracked.Model.name}"));
                        var fixedMessage = (FixedString512Bytes)FontColorChatSystem.Green($"{message}");
                        ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref fixedMessage);
                    }
                    
                    continue;
                }
                
                // Check if boss is dead
                if (entity.Has<ProjectM.Health>())
                {
                    var health = entity.Read<ProjectM.Health>();
                    if (health.Value <= 0)
                    {
                        entitiesToRemove.Add(entity);
                        continue;
                    }
                }
                
                // Despawn check removed - now handled by EntityDestroyHook
                // The LifeTime component will handle despawn automatically
                // and our hook will detect it for cleanup
                
                // Mechanic check (every second)
                if (now >= tracked.NextMechanicCheck)
                {
                    tracked.NextMechanicCheck = now.AddSeconds(1);
                    CheckMechanics(entity, tracked);
                }
                
                // Team check (only once after spawn)
                if (tracked.NeedsTeamCheck)
                {
                    BossSystem.CheckTeams(entity);
                    tracked.NeedsTeamCheck = false;
                }
            }
            
            // Remove invalid entities
            foreach (var entity in entitiesToRemove)
            {
                UnregisterBoss(entity);
            }
        }
        
        
        private static void CheckMechanics(Entity entity, TrackedBoss tracked)
        {
            try
            {
                // Check time-based and player count mechanics
                BossMechanicSystem.CheckTimeMechanics(entity, tracked.Model);
                BossMechanicSystem.CheckPlayerCountMechanics(entity, tracked.Model);
                
                // Modify boss stats if needed
                var userModel = Bloody.Core.GameData.v1.GameData.Users.All.FirstOrDefault();
                if (userModel != null)
                {
                    tracked.Model.ModifyBoss(userModel.Entity, entity);
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[BossTracking] Error checking mechanics: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get count of active bosses
        /// </summary>
        public static int GetActiveBossCount()
        {
            return _activeBosses.Count;
        }
        
        /// <summary>
        /// Clear all tracked bosses
        /// </summary>
        public static void Clear()
        {
            _activeBosses.Clear();
            _bossesByName.Clear();
            Plugin.Logger.LogInfo("[BossTracking] Cleared all tracked bosses");
        }
    }
}