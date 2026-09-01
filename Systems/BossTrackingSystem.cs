using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Collections;
using BloodyBoss.DB.Models;
using BloodyBoss.DB;
using BloodyBoss.Configuration;
using BloodyBoss.Components;
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
            public float LastHealthPercent { get; set; } = 100f;
            public bool WasEngaged { get; set; } = false;
            public bool NeedsTeamReferenceSync { get; set; } = false;
            public TeamReference? StoredTeamReference { get; set; } = null;

            // Timestamp of when health first hit 0. Used by HandleBossDeath to force
            // bossSpawn clear if the game never confirms the kill.
            public DateTime? DeathDetectedAt { get; set; } = null;
        }

        /// <summary>
        /// Register a spawned boss for tracking
        /// </summary>
        public static void RegisterSpawnedBoss(Entity entity, BossEncounterModel model)
        {
            if (_activeBosses.ContainsKey(entity))
            {
                Plugin.BLogger.Debug(LogCategory.Boss, $"[BossTracking] Boss {model.name} already registered, updating");
            }

            var tracked = new TrackedBoss
            {
                Entity = entity,
                Model = model,
                SpawnTime = DateTime.UtcNow,
                NextDespawnCheck = DateTime.UtcNow.AddSeconds(5), // Check every 5 seconds
                NextMechanicCheck = DateTime.UtcNow.AddSeconds(1), // Check mechanics every second
                NeedsTeamCheck = true,
                // Seed at 100% so HealthMonitorSystem doesn't see a false 0→100 jump
                // when it first observes this boss before ModifyBoss has run.
                LastHealthPercent = 100f
            };

            _activeBosses[entity] = tracked;
            _bossesByName[model.name] = entity;

            Plugin.BLogger.Debug(LogCategory.Boss, $"[BossTracking] Registered boss {model.name} for optimized tracking. Active bosses: {_activeBosses.Count}");
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

                // BUG FIX #13: BossComponentsHelper._bossStateCache was never cleared on despawn,
                // causing it to grow indefinitely and hold stale references to destroyed entities.
                // Clear the state entry here since UnregisterBoss is the canonical cleanup point.
                BossComponentsHelper.RemoveBossState(entity);

                Plugin.BLogger.Info(LogCategory.Boss, $"[BossTracking] Unregistered boss {tracked.Model.name}. Active bosses: {_activeBosses.Count}");
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
        /// Only checks what needs checking, to avoid unnecessary work.
        /// Schedules the update on the main thread via ActionScheduler.
        /// Use this only if you're not already on the main thread.
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
                    Plugin.BLogger.Error(LogCategory.Boss, $"[BossTracking] Error in UpdateActiveBosses: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Call this instead when already running on the main game thread
        /// (e.g. from inside BossSystem.bossAction, which is already wrapped
        /// in RunActionOnMainThread), to avoid scheduling a second one.
        /// </summary>
        public static void UpdateActiveBossesOnMainThread()
        {
            try
            {
                UpdateActiveBossesInternal();
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"[BossTracking] Error in UpdateActiveBossesOnMainThread: {ex.Message}");
            }
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
                    Plugin.BLogger.Info(LogCategory.Boss, $"[BossTracking] Boss {tracked.Model.name} entity no longer exists (despawned by LifeTime)");

                    // Clean up minions before removing boss
                    MinionTrackingSystem.OnBossDeathOrDespawn(entity);

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

                // Check if boss is dead or has regenerated
                if (entity.Has<ProjectM.Health>())
                {
                    var health = entity.Read<ProjectM.Health>();
                    if (health.Value <= 0)
                    {
                        if (HandleBossDeath(entity, tracked, now))
                        {
                            entitiesToRemove.Add(entity);
                        }
                        continue;
                    }

                    // Health went back above 0 before the kill was confirmed, so clear
                    // the pending death timer for next time.
                    if (tracked.DeathDetectedAt.HasValue)
                    {
                        tracked.DeathDetectedAt = null;
                    }

                    // Calculate current health percentage
                    float currentHealthPercent = (health.Value / health.MaxHealth.Value) * 100f;

                    // Check if boss has regenerated back to full health after being engaged
                    if (tracked.WasEngaged && currentHealthPercent >= 99f && tracked.LastHealthPercent < 99f)
                    {
                        Plugin.BLogger.Info(LogCategory.Boss, $"[BossTracking] Boss {tracked.Model.name} regenerated to full health, resetting mechanics");

                        // Reset all mechanics
                        foreach (var mechanic in tracked.Model.Mechanics)
                        {
                            mechanic.Reset();
                        }

                        // Reset boss start time for time-based mechanics
                        var bossKey = $"{tracked.Model.nameHash}";
                        BossMechanicSystem.InitializeBossMechanics(entity, tracked.Model);

                        // Save the database to persist the reset
                        Database.saveDatabase();

                        // Reset tracking flags
                        tracked.WasEngaged = false;
                    }

                    // Mark as engaged if health drops below 95%
                    if (currentHealthPercent < 95f)
                    {
                        tracked.WasEngaged = true;
                    }

                    // Update last health percent
                    tracked.LastHealthPercent = currentHealthPercent;
                }

                // Despawn is handled by EntityDestroyHook now: the LifeTime
                // component destroys the entity and our hook detects it for cleanup.

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

                    // If this is the first boss, store its TeamReference when it's valid
                    if (Database.TeamDefault != null && Database.TeamReferenceDefault == null)
                    {
                        if (entityManager.HasComponent<TeamReference>(entity))
                        {
                            var teamRef = entityManager.GetComponentData<TeamReference>(entity);
                            if (teamRef.Value._Value != Entity.Null)
                            {
                                Database.TeamReferenceDefault = teamRef;
                                Plugin.BLogger.Warning(LogCategory.Boss, $"[BossTracking] Stored valid TeamReference from first boss");
                            }
                            else
                            {
                                tracked.NeedsTeamReferenceSync = true;
                                Plugin.BLogger.Debug(LogCategory.Boss, $"[BossTracking] First boss TeamReference still null, will retry");
                            }
                        }
                    }
                    else if (Database.TeamReferenceDefault.HasValue)
                    {
                        // Mark other bosses to sync TeamReference
                        tracked.NeedsTeamReferenceSync = true;
                        tracked.StoredTeamReference = Database.TeamReferenceDefault;
                    }
                }

                // TeamReference sync check (wait for valid TeamReference)
                if (tracked.NeedsTeamReferenceSync)
                {
                    if (entityManager.HasComponent<TeamReference>(entity))
                    {
                        var currentTeamRef = entityManager.GetComponentData<TeamReference>(entity);

                        // If this is the first boss and we're waiting for a valid TeamReference
                        if (Database.TeamReferenceDefault == null && currentTeamRef.Value._Value != Entity.Null)
                        {
                            Database.TeamReferenceDefault = currentTeamRef;
                            tracked.NeedsTeamReferenceSync = false;
                            Plugin.BLogger.Warning(LogCategory.Boss, $"[BossTracking] First boss now has valid TeamReference, storing it");

                            // Mark all other tracked bosses to sync
                            foreach (var otherBoss in _activeBosses.Values.Where(b => b != tracked))
                            {
                                otherBoss.NeedsTeamReferenceSync = true;
                                otherBoss.StoredTeamReference = currentTeamRef;
                            }
                        }
                        // If we have a stored TeamReference to apply
                        else if (tracked.StoredTeamReference.HasValue && currentTeamRef.Value._Value != tracked.StoredTeamReference.Value.Value._Value)
                        {
                            entityManager.SetComponentData(entity, tracked.StoredTeamReference.Value);
                            tracked.NeedsTeamReferenceSync = false;
                            Plugin.BLogger.Warning(LogCategory.Boss, $"[BossTracking] Applied stored TeamReference to boss {tracked.Model.name}");
                        }
                    }
                }
            }

            // Remove invalid entities
            foreach (var entity in entitiesToRemove)
            {
                UnregisterBoss(entity);
            }

            // Update minion tracking
            MinionTrackingSystem.UpdateMinions();
        }

        /// <summary>
        /// Runs while a boss's health sits at 0, waiting for OnDeathNpc or
        /// OnVBloodConsumed to confirm the kill and clear bossSpawn. If nothing
        /// confirms it within the timeout, forces the same cleanup so the boss
        /// doesn't stay "active" and block the schedule.
        /// </summary>
        /// <returns>True once the boss is finalized and should be removed from tracking.</returns>
        private static bool HandleBossDeath(Entity entity, TrackedBoss tracked, DateTime now)
        {
            if (tracked.DeathDetectedAt == null)
            {
                tracked.DeathDetectedAt = now;
                // Clean up minions right away, regardless of when the kill gets confirmed.
                MinionTrackingSystem.OnBossDeathOrDespawn(entity);
                Plugin.BLogger.Info(LogCategory.Boss, $"[BossTracking] Boss {tracked.Model.name} health reached 0, waiting for kill confirmation");
            }

            var timeoutSeconds = PluginConfig.StuckBossRecoveryTimeoutSeconds.Value;
            if (timeoutSeconds > 0 && now - tracked.DeathDetectedAt.Value < TimeSpan.FromSeconds(timeoutSeconds))
            {
                // Still within the grace period, keep waiting for a normal kill confirmation.
                return false;
            }

            if (tracked.Model.bossSpawn)
            {
                Plugin.BLogger.Warning(LogCategory.Boss, $"[BossTracking] Boss {tracked.Model.name} died without a confirmed kill, forcing recovery so the schedule doesn't get stuck");

                try
                {
                    BossGameplayEventSystem.UnregisterBoss(entity);
                    tracked.Model.BuffKillers();
                    tracked.Model.SendAnnouncementMessage();
                }
                catch (Exception ex)
                {
                    Plugin.BLogger.Error(LogCategory.Boss, $"[BossTracking] Error force-finalizing stuck boss {tracked.Model.name}: {ex.Message}");
                    // Fallback in case the cleanup above threw partway through, so
                    // bossSpawn still gets cleared.
                    tracked.Model.bossSpawn = false;
                    tracked.Model.bossEntity = Entity.Null;
                    Database.saveDatabase();
                }
            }

            return true;
        }

        private static void CheckMechanics(Entity entity, TrackedBoss tracked)
        {
            try
            {
                // Check time-based and player count mechanics
                BossMechanicSystem.CheckTimeMechanics(entity, tracked.Model);
                BossMechanicSystem.CheckPlayerCountMechanics(entity, tracked.Model);

                // REMOVED: ModifyBoss should only be called once during spawn, not repeatedly
                // This was causing the boss to reset its health to 100% constantly
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Boss, $"[BossTracking] Error checking mechanics: {ex.Message}");
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
        /// Returns the last recorded health percentage for a tracked boss entity.
        /// Used by BossGameplayEventSystem.ProcessStatChangeEvent to supply a correct
        /// previousHpPercent to CheckHpThresholdMechanics instead of a hardcoded 100f.
        /// Returns 100f if the entity isn't tracked (assumes full health).
        /// </summary>
        public static float GetLastHealthPercent(Entity entity)
        {
            if (_activeBosses.TryGetValue(entity, out var tracked))
                return tracked.LastHealthPercent;
            return 100f;
        }

        /// <summary>
        /// Clear all tracked bosses
        /// </summary>
        public static void Clear()
        {
            _activeBosses.Clear();
            _bossesByName.Clear();
            Plugin.BLogger.Info(LogCategory.Boss, "[BossTracking] Cleared all tracked bosses");
        }
    }
}
