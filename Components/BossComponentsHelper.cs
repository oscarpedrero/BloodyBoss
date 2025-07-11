using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using ProjectM;
using Stunlock.Core;
using BloodyBoss.DB.Models;
using Bloody.Core;

namespace BloodyBoss.Components
{
    /// <summary>
    /// Helper utilities for working with boss components and data
    /// </summary>
    internal static class BossComponentsHelper
    {
        // Cache of boss states indexed by entity
        private static readonly Dictionary<Entity, BossComponents.BossState> _bossStateCache = new();
        
        // Cache of boss phases indexed by boss name
        private static readonly Dictionary<string, BossComponents.BossPhase[]> _bossPhaseCache = new();
        
        /// <summary>
        /// Get or create boss state for an entity
        /// </summary>
        public static BossComponents.BossState GetOrCreateBossState(Entity entity, BossEncounterModel model)
        {
            if (_bossStateCache.TryGetValue(entity, out var existingState))
            {
                return existingState;
            }
            
            var newState = new BossComponents.BossState
            {
                Entity = entity,
                BossName = model.name,
                NameHash = model.nameHash.GetHashCode(),
                SpawnTime = DateTime.Now,
                IsPaused = model.IsPaused,
                ConsecutiveSpawns = model.ConsecutiveSpawns,
                DifficultyMultiplier = model.CurrentDifficultyMultiplier,
                LastAnnouncedPhase = model.LastAnnouncedPhase
            };
            
            _bossStateCache[entity] = newState;
            return newState;
        }
        
        /// <summary>
        /// Update boss state
        /// </summary>
        public static void UpdateBossState(Entity entity, Action<BossComponents.BossState> updateAction)
        {
            if (_bossStateCache.TryGetValue(entity, out var state))
            {
                updateAction(state);
                _bossStateCache[entity] = state;
            }
        }
        
        /// <summary>
        /// Remove boss state when entity is destroyed
        /// </summary>
        public static void RemoveBossState(Entity entity)
        {
            _bossStateCache.Remove(entity);
        }
        
        /// <summary>
        /// Get all active boss states
        /// </summary>
        public static IEnumerable<BossComponents.BossState> GetAllBossStates()
        {
            return _bossStateCache.Values;
        }
        
        /// <summary>
        /// Find all boss entities using optimized query
        /// </summary>
        public static List<Entity> FindAllBossEntities(EntityManager entityManager)
        {
            var query = BossComponents.Queries.GetBossQuery(entityManager);
            var entities = query.ToEntityArray(Allocator.Temp);
            var bosses = new List<Entity>();
            
            foreach (var entity in entities)
            {
                if (BossComponents.IsBloodyBoss(entity, entityManager))
                {
                    bosses.Add(entity);
                }
            }
            
            entities.Dispose();
            return bosses;
        }
        
        /// <summary>
        /// Get combat stats for a boss
        /// </summary>
        public static BossComponents.BossCombatStats GetBossCombatStats(Entity entity)
        {
            return BossComponents.BossCombatStats.FromEntity(entity);
        }
        
        /// <summary>
        /// Check and announce boss phase transitions
        /// </summary>
        public static bool CheckPhaseTransition(Entity entity, BossEncounterModel model)
        {
            var stats = GetBossCombatStats(entity);
            var currentPhase = BossComponents.Phases.GetCurrentPhase(stats.HealthPercentage);
            
            if (currentPhase.PhaseNumber > model.LastAnnouncedPhase)
            {
                // Phase transition detected
                model.LastAnnouncedPhase = currentPhase.PhaseNumber;
                
                // Update cached state
                UpdateBossState(entity, state => state.LastAnnouncedPhase = currentPhase.PhaseNumber);
                
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Get custom phases for a specific boss
        /// </summary>
        public static BossComponents.BossPhase[] GetBossPhases(string bossName)
        {
            if (_bossPhaseCache.TryGetValue(bossName, out var phases))
            {
                return phases;
            }
            
            // Return standard phases by default
            return BossComponents.Phases.StandardPhases;
        }
        
        /// <summary>
        /// Set custom phases for a specific boss
        /// </summary>
        public static void SetBossPhases(string bossName, BossComponents.BossPhase[] phases)
        {
            _bossPhaseCache[bossName] = phases;
        }
        
        /// <summary>
        /// Clear all caches
        /// </summary>
        public static void ClearAllCaches()
        {
            _bossStateCache.Clear();
            _bossPhaseCache.Clear();
        }
        
        /// <summary>
        /// Get spawn configuration from model
        /// </summary>
        public static BossComponents.BossSpawnConfig GetSpawnConfig(BossEncounterModel model)
        {
            return new BossComponents.BossSpawnConfig
            {
                PrefabGUID = model.PrefabGUID,
                Position = new Unity.Mathematics.float3(model.x, model.y, model.z),
                Level = model.level,
                HealthMultiplier = model.CurrentDifficultyMultiplier,
                DamageMultiplier = model.CurrentDifficultyMultiplier,
                SpawnTime = model.Hour,
                LifetimeSeconds = (int)model.Lifetime
            };
        }
    }
}