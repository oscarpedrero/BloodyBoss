using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using BloodyBoss.DB.Models;
using Bloody.Core;
using ProjectM;
using ProjectM.Network;

namespace BloodyBoss.Systems
{
    /// <summary>
    /// Tracks minions/clones spawned by bosses and ensures they are cleaned up properly
    /// </summary>
    internal static class MinionTrackingSystem
    {
        // Track minions by their entity and their owner boss
        private static readonly Dictionary<Entity, TrackedMinion> _activeMinions = new Dictionary<Entity, TrackedMinion>();
        
        // Quick lookup of minions by boss entity
        private static readonly Dictionary<Entity, HashSet<Entity>> _minionsByBoss = new Dictionary<Entity, HashSet<Entity>>();
        
        private class TrackedMinion
        {
            public Entity Entity { get; set; }
            public Entity OwnerBoss { get; set; }
            public DateTime SpawnTime { get; set; }
            public string MinionType { get; set; } // "clone", "summon", etc.
        }
        
        /// <summary>
        /// Register a minion for tracking
        /// </summary>
        public static void RegisterMinion(Entity minionEntity, Entity bossEntity, string minionType = "minion")
        {
            if (_activeMinions.ContainsKey(minionEntity))
            {
                Plugin.BLogger.Debug(LogCategory.Boss, $"[MinionTracking] Minion already registered, updating");
            }
            
            var tracked = new TrackedMinion
            {
                Entity = minionEntity,
                OwnerBoss = bossEntity,
                SpawnTime = DateTime.UtcNow,
                MinionType = minionType
            };
            
            _activeMinions[minionEntity] = tracked;
            
            // Add to boss lookup
            if (!_minionsByBoss.ContainsKey(bossEntity))
            {
                _minionsByBoss[bossEntity] = new HashSet<Entity>();
            }
            _minionsByBoss[bossEntity].Add(minionEntity);
            
            Plugin.BLogger.Info(LogCategory.Boss, $"[MinionTracking] Registered {minionType} for boss. Total minions: {_activeMinions.Count}");
        }
        
        /// <summary>
        /// Unregister a minion
        /// </summary>
        public static void UnregisterMinion(Entity minionEntity)
        {
            if (_activeMinions.TryGetValue(minionEntity, out var tracked))
            {
                // Remove from boss lookup
                if (_minionsByBoss.ContainsKey(tracked.OwnerBoss))
                {
                    _minionsByBoss[tracked.OwnerBoss].Remove(minionEntity);
                    if (_minionsByBoss[tracked.OwnerBoss].Count == 0)
                    {
                        _minionsByBoss.Remove(tracked.OwnerBoss);
                    }
                }
                
                _activeMinions.Remove(minionEntity);
                Plugin.BLogger.Info(LogCategory.Boss, $"[MinionTracking] Unregistered {tracked.MinionType}. Active minions: {_activeMinions.Count}");
            }
        }
        
        /// <summary>
        /// Called when a boss dies or despawns - cleans up all its minions
        /// </summary>
        public static void OnBossDeathOrDespawn(Entity bossEntity)
        {
            if (_minionsByBoss.TryGetValue(bossEntity, out var minions))
            {
                Plugin.BLogger.Info(LogCategory.Boss, $"[MinionTracking] Boss died/despawned, cleaning up {minions.Count} minions");
                
                var entityManager = Plugin.SystemsCore.EntityManager;
                var minionsToRemove = minions.ToList(); // Copy to avoid modification during iteration
                
                foreach (var minionEntity in minionsToRemove)
                {
                    try
                    {
                        if (entityManager.Exists(minionEntity))
                        {
                            // Destroy the minion
                            StatChangeUtility.KillOrDestroyEntity(entityManager, minionEntity, bossEntity, bossEntity, 0, StatChangeReason.Any, true);
                            Plugin.BLogger.Debug(LogCategory.Boss, $"[MinionTracking] Destroyed minion {minionEntity}");
                        }
                        
                        UnregisterMinion(minionEntity);
                    }
                    catch (Exception ex)
                    {
                        Plugin.BLogger.Error(LogCategory.Boss, $"[MinionTracking] Error destroying minion: {ex.Message}");
                    }
                }
                
                _minionsByBoss.Remove(bossEntity);
            }
        }
        
        /// <summary>
        /// Update all tracked minions - check if they're still alive
        /// </summary>
        public static void UpdateMinions()
        {
            var entitiesToRemove = new List<Entity>();
            var entityManager = Plugin.SystemsCore.EntityManager;
            
            foreach (var kvp in _activeMinions)
            {
                var minionEntity = kvp.Key;
                var tracked = kvp.Value;
                
                // Check if minion still exists
                if (!entityManager.Exists(minionEntity))
                {
                    entitiesToRemove.Add(minionEntity);
                    continue;
                }
                
                // Check if minion is dead
                if (minionEntity.Has<Health>())
                {
                    var health = minionEntity.Read<Health>();
                    if (health.Value <= 0)
                    {
                        entitiesToRemove.Add(minionEntity);
                        continue;
                    }
                }
                
                // Check if boss still exists and is alive
                if (!entityManager.Exists(tracked.OwnerBoss))
                {
                    // Boss no longer exists, destroy minion
                    Plugin.BLogger.Info(LogCategory.Boss, $"[MinionTracking] Boss no longer exists, destroying {tracked.MinionType}");
                    StatChangeUtility.KillOrDestroyEntity(entityManager, minionEntity, tracked.OwnerBoss, tracked.OwnerBoss, 0, StatChangeReason.Any, true);
                    entitiesToRemove.Add(minionEntity);
                }
                else if (tracked.OwnerBoss.Has<Health>())
                {
                    var bossHealth = tracked.OwnerBoss.Read<Health>();
                    if (bossHealth.Value <= 0)
                    {
                        // Boss is dead, destroy minion
                        Plugin.BLogger.Info(LogCategory.Boss, $"[MinionTracking] Boss is dead, destroying {tracked.MinionType}");
                        StatChangeUtility.KillOrDestroyEntity(entityManager, minionEntity, tracked.OwnerBoss, tracked.OwnerBoss, 0, StatChangeReason.Any, true);
                        entitiesToRemove.Add(minionEntity);
                    }
                }
            }
            
            // Clean up removed entities
            foreach (var entity in entitiesToRemove)
            {
                UnregisterMinion(entity);
            }
        }
        
        /// <summary>
        /// Get count of active minions for a boss
        /// </summary>
        public static int GetMinionCount(Entity bossEntity)
        {
            return _minionsByBoss.ContainsKey(bossEntity) ? _minionsByBoss[bossEntity].Count : 0;
        }
        
        /// <summary>
        /// Clear all tracked minions
        /// </summary>
        public static void Clear()
        {
            _activeMinions.Clear();
            _minionsByBoss.Clear();
            Plugin.BLogger.Info(LogCategory.Boss, "[MinionTracking] Cleared all tracked minions");
        }
    }
}