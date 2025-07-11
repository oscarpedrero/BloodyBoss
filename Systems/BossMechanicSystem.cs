using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using ProjectM;
using Bloody.Core;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Systems.Mechanics;
using BloodyBoss.Cache;
using Unity.Collections;
using Stunlock.Core;
using Bloody.Core.Helper.v1;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using ProjectM.Network;
using Bloody.Core.GameData.v1;

namespace BloodyBoss.Systems
{
    internal static class BossMechanicSystem
    {
        // MechanicFactory now handles mechanic registration

        // Track time-based mechanics
        private static readonly Dictionary<string, DateTime> LastTimeCheck = new();
        private static readonly Dictionary<string, float> BossStartTimes = new();

        /// <summary>
        /// Initialize the mechanic system for a boss when it spawns
        /// </summary>
        public static void InitializeBossMechanics(Entity bossEntity, BossEncounterModel boss)
        {
            var bossKey = $"{boss.nameHash}";
            BossStartTimes[bossKey] = UnityEngine.Time.time;
            LastTimeCheck[bossKey] = DateTime.Now;
            
            // Reset all mechanic states
            foreach (var mechanic in boss.Mechanics)
            {
                mechanic.Reset();
            }
            
            Plugin.BLogger.Debug(LogCategory.Boss, $"Initialized mechanics for boss {boss.name} with {boss.Mechanics.Count} mechanics");
        }

        /// <summary>
        /// Check and execute mechanics based on HP threshold
        /// </summary>
        public static void CheckHpThresholdMechanics(Entity bossEntity, BossEncounterModel boss, float currentHpPercent, float previousHpPercent = 100f)
        {
            var triggeredMechanics = boss.Mechanics
                .Where(m => m.Enabled && !m.IsExpired)
                .Where(m => m.Trigger?.Type == "hp_threshold")
                .Where(m => ShouldTriggerHpMechanic(m, currentHpPercent, previousHpPercent))
                .ToList();

            if (triggeredMechanics.Count > 0)
            {
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"Boss {boss.name} at {currentHpPercent:F1}% HP - {triggeredMechanics.Count} mechanics triggered");
            }

            foreach (var mechanic in triggeredMechanics)
            {
                ExecuteMechanic(bossEntity, boss, mechanic);
            }
        }

        /// <summary>
        /// Check and execute time-based mechanics
        /// </summary>
        public static void CheckTimeMechanics(Entity bossEntity, BossEncounterModel boss)
        {
            var bossKey = $"{boss.nameHash}";
            if (!BossStartTimes.ContainsKey(bossKey))
                return;

            var elapsedTime = UnityEngine.Time.time - BossStartTimes[bossKey];
            
            var triggeredMechanics = boss.Mechanics
                .Where(m => m.Enabled && !m.IsExpired)
                .Where(m => m.Trigger?.Type == "time")
                .Where(m => ShouldTriggerTimeMechanic(m, elapsedTime))
                .ToList();

            foreach (var mechanic in triggeredMechanics)
            {
                ExecuteMechanic(bossEntity, boss, mechanic);
            }
        }

        /// <summary>
        /// Check and execute player count based mechanics
        /// </summary>
        public static void CheckPlayerCountMechanics(Entity bossEntity, BossEncounterModel boss)
        {
            // Get boss position
            var entityManager = Plugin.SystemsCore.EntityManager;
            if (!entityManager.Exists(bossEntity) || !entityManager.HasComponent<LocalToWorld>(bossEntity))
                return;
                
            var bossPos = entityManager.GetComponentData<LocalToWorld>(bossEntity).Position;
            var playerCount = CountNearbyPlayers(bossPos, 100f); // 100 unit radius
            
            var triggeredMechanics = boss.Mechanics
                .Where(m => m.Enabled && !m.IsExpired)
                .Where(m => m.Trigger?.Type == "player_count")
                .Where(m => m.Trigger.EvaluateCondition(playerCount))
                .Where(m => m.CanTriggerAgain())
                .ToList();

            foreach (var mechanic in triggeredMechanics)
            {
                ExecuteMechanic(bossEntity, boss, mechanic);
            }
        }

        /// <summary>
        /// Execute a specific mechanic
        /// </summary>
        public static void ExecuteMechanic(Entity bossEntity, BossEncounterModel boss, BossMechanicModel mechanic)
        {
            try
            {
                var implementation = MechanicFactory.GetMechanic(mechanic.Type);
                if (implementation == null)
                {
                    Plugin.BLogger.Warning(LogCategory.Boss, $"No implementation found for mechanic type: {mechanic.Type}");
                    return;
                }

                if (!implementation.CanApply(bossEntity))
                {
                    Plugin.BLogger.Warning(LogCategory.Boss, $"Cannot apply {mechanic.Type} mechanic to entity");
                    return;
                }

                // Log mechanic execution (single line, debug level)
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"{boss.name} executing {mechanic.Type} mechanic @ {mechanic.GetDescription()}");
                
                // Execute the mechanic
                implementation.Execute(bossEntity, mechanic.Parameters, Core.World);
                
                // Mark as triggered
                mechanic.MarkTriggered();
                
                // Save state
                Database.saveDatabase();
            }
            catch (Exception e)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"Error executing {mechanic.Type} mechanic: {e.Message}");
            }
        }

        /// <summary>
        /// Clean up when boss dies or despawns
        /// </summary>
        public static void CleanupBossMechanics(BossEncounterModel boss)
        {
            var bossKey = $"{boss.nameHash}";
            BossStartTimes.Remove(bossKey);
            LastTimeCheck.Remove(bossKey);
            
            // Reset all mechanics
            foreach (var mechanic in boss.Mechanics)
            {
                mechanic.Reset();
            }
        }

        #region Helper Methods

        private static bool ShouldTriggerHpMechanic(BossMechanicModel mechanic, float currentHpPercent, float previousHpPercent)
        {
            if (mechanic.Trigger == null || mechanic.IsExpired)
                return false;

            // For "less_than" comparison, check if we crossed the threshold
            if (mechanic.Trigger.Comparison == "less_than" || mechanic.Trigger.Comparison == "lt" || mechanic.Trigger.Comparison == "<")
            {
                // Previous HP was above or at threshold, current HP is below threshold
                bool crossedThreshold = previousHpPercent >= mechanic.Trigger.Value && currentHpPercent < mechanic.Trigger.Value;
                
                if (crossedThreshold)
                {
                    Plugin.BLogger.Debug(LogCategory.Mechanic, $"Threshold crossed: {previousHpPercent:F1}% -> {currentHpPercent:F1}% (trigger: {mechanic.Trigger.Value}%)");
                    return mechanic.CanTriggerAgain();
                }
            }
            else
            {
                // For other comparisons, use the normal evaluation
                if (mechanic.Trigger.EvaluateCondition(currentHpPercent))
                {
                    return mechanic.CanTriggerAgain();
                }
            }

            return false;
        }

        private static bool ShouldTriggerTimeMechanic(BossMechanicModel mechanic, float elapsedTime)
        {
            if (mechanic.Trigger == null || mechanic.IsExpired)
                return false;

            // For one-time triggers
            if (mechanic.Trigger.OneTime && !mechanic.HasTriggered)
            {
                return elapsedTime >= mechanic.Trigger.Value;
            }

            // For repeating triggers
            if (mechanic.Trigger.RepeatInterval > 0)
            {
                if (!mechanic.HasTriggered)
                {
                    return elapsedTime >= mechanic.Trigger.Value;
                }
                else if (mechanic.LastTriggered.HasValue)
                {
                    var timeSinceLastTrigger = (DateTime.Now - mechanic.LastTriggered.Value).TotalSeconds;
                    return timeSinceLastTrigger >= mechanic.Trigger.RepeatInterval;
                }
            }

            return false;
        }

        private static int CountNearbyPlayers(float3 position, float radius)
        {
            var count = 0;
            var radiusSq = radius * radius;
            
            var users = GameData.Users.Online.ToList();
            
            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distanceSq = math.distancesq(position, playerPos);
                    
                    if (distanceSq <= radiusSq)
                    {
                        count++;
                    }
                }
            }
            
            return count;
        }

        public static Entity FindBossEntity(BossEncounterModel boss)
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeDisabledEntities);
            foreach (var entity in entities)
            {
                if (entity.Has<NameableInteractable>())
                {
                    var nameable = entity.Read<NameableInteractable>();
                    if (nameable.Name.ToString() == boss.nameHash + "bb")
                    {
                        return entity;
                    }
                }
            }
            return Entity.Null;
        }

        #endregion
    }
}