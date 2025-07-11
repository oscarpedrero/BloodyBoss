using System;
using System.Linq;
using System.Threading;
using BloodyBoss.Configuration;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Utils;
using BloodyBoss.Cache;
using ProjectM;
using Unity.Entities;
using Bloody.Core.Helper.v1;
using Bloody.Core.API.v1;
using Stunlock.Core;
using Bloody.Core;
using Bloody.Core.Patch.Server;
using Bloody.Core.GameData.v1;


namespace BloodyBoss.Systems
{
    
    internal class BossSystem
    {
        
        private static int lastMinute = -1; // Initialize to -1 to force first check
        private static Timer bossTimer;
        private static bool isTimerRunning = false;
       
        public static Action bossAction;

        public static void StartTimer()
        {
            // Evitar múltiples timers
            if (isTimerRunning)
            {
                Plugin.BLogger.Debug(LogCategory.Timer, "BloodyBoss timer already running, skipping...");
                return;
            }

            Plugin.BLogger.Info(LogCategory.System, "Starting BloodyBoss timer (independent of player connections)");
            
            bossAction = () =>
            {
                try
                {
                    // Comprehensive null safety checks
                    if (Plugin.SystemsCore == null)
                    {
                        Plugin.BLogger.Trace(LogCategory.Timer, "BossSystem timer: SystemsCore not ready yet, skipping...");
                        return;
                    }
                    
                    if (Plugin.SystemsCore.PrefabCollectionSystem == null)
                    {
                        Plugin.BLogger.Trace(LogCategory.Timer, "BossSystem timer: Essential systems not ready yet, skipping...");
                        return;
                    }
                    
                    var now = DateTime.Now;
                    bool minuteChanged = now.Minute != lastMinute;
                    
                    if (minuteChanged)
                    {
                        lastMinute = now.Minute;
                        var currentTime = now.ToString("HH:mm");
                        var spawnsBoss = Database.BOSSES.Where(x => x.Hour == currentTime && !x.IsPaused).ToList();
                        if (spawnsBoss.Count > 0)
                        {
                            Plugin.BLogger.Info(LogCategory.Spawn, $"Found {spawnsBoss.Count} bosses to spawn at {currentTime}");
                            foreach (var spawnBoss in spawnsBoss)
                            {
                                Action actionSpawnDespawn = () =>
                                {
                                    spawnBoss.CheckSpawnDespawn();
                                };

                                ActionScheduler.RunActionOnMainThread(actionSpawnDespawn);
                            }
                        }
                    }
                    
                    // Manual despawn by HourDespawn is now disabled - using LifeTime system instead
                    // The boss will be automatically destroyed when LifeTime expires
                    
                    // Optimized boss update - only check active bosses
                    BossTrackingSystem.UpdateActiveBosses();
                }
                catch (Exception ex)
                {
                    Plugin.BLogger.Error(LogCategory.Timer, $"Error in BossSystem timer: {ex.Message}");
                }
            };

            // Usar System.Threading.Timer en lugar de CoroutineHandler
            // Esto funciona independientemente de si hay jugadores conectados
            // IMPORTANTE: Retrasar la primera ejecución 10 segundos para dar tiempo a que el mundo se inicialice
            bossTimer = new Timer(_ => bossAction?.Invoke(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1));
            isTimerRunning = true;
            Plugin.BLogger.Info(LogCategory.Timer, "BloodyBoss timer started successfully (first check in 10 seconds)");
        }

        public static void StopTimer()
        {
            if (bossTimer != null)
            {
                bossTimer.Dispose();
                bossTimer = null;
                isTimerRunning = false;
                Plugin.BLogger.Info(LogCategory.Timer, "BloodyBoss timer stopped");
            }
        }


        // This method is now only used for initial discovery and validation
        // Regular updates are handled by BossTrackingSystem.UpdateActiveBosses()
        internal static void CheckBoss()
        {
            // Null safety check
            if (Plugin.SystemsCore?.EntityManager == null)
            {
                Plugin.BLogger.Debug(LogCategory.Boss, "CheckBoss: EntityManager not ready");
                return;
            }
            
            // Only run if we have very few tracked bosses (might have missed some)
            if (BossTrackingSystem.GetActiveBossCount() > 5)
            {
                return; // Skip if we already have plenty of bosses tracked
            }
            
            // Use cached entities for discovery of untracked bosses
            var entities = ComponentCache.GetBossEntities();
            
            foreach (var entity in entities)
            {
                // Use extension method to check if it's a BloodyBoss
                if (!entity.IsBloodyBoss())
                    continue;
                    
                // Use extension method to get the boss model
                BossEncounterModel bossModel = entity.GetBossModel();
                if (bossModel != null && bossModel.bossSpawn)
                {
                    // Check if this boss is already tracked
                    string bossName = bossModel.name;
                    Entity trackedEntity;
                    
                    if (!BossTrackingSystem.TryGetBossByName(bossName, out trackedEntity))
                    {
                        // Boss not tracked yet, register it
                        Plugin.BLogger.Info(LogCategory.Boss, $"[CheckBoss] Found untracked boss {bossName}, registering for optimized tracking");
                        BossTrackingSystem.RegisterSpawnedBoss(entity, bossModel);
                        BossGameplayEventSystem.RegisterBoss(entity, bossModel);
                        
                        // Initial setup for discovered boss
                        var userModel = GameData.Users.All.FirstOrDefault();
                        if (userModel != null)
                        {
                            bossModel.ModifyBoss(userModel.Entity, entity);
                            CheckTeams(entity);
                            
                            if (PluginConfig.ClearDropTable.Value)
                            {
                                var action = () =>
                                {
                                    bossModel.ClearDropTable(entity);
                                };
                                ActionScheduler.RunActionOnceAfterFrames(action, 10);
                            }
                        }
                    }
                }
            }
            
            // Note: We don't dispose NativeArrays from cache as they're managed by ComponentCache
            // Ya no necesitamos llamar StartTimer() aquí porque el timer ya está funcionando independientemente
        }

        internal static void CheckTeams(Entity boss)
        {
            if (PluginConfig.TeamBossEnable.Value)
            {
                Plugin.BLogger.Warning(LogCategory.Boss, "Adding the same team to the boss");
                if (Database.TeamDefault == null)
                {
                    var entityManager = Plugin.SystemsCore.EntityManager;
                    if (entityManager.Exists(boss))
                    {
                        Database.TeamDefault = entityManager.GetComponentData<Team>(boss);
                        Database.TeamReferenceDefault = entityManager.GetComponentData<TeamReference>(boss);
                    }
                    Plugin.BLogger.Warning(LogCategory.Boss, "There is no team created, so we create the new Team");
                }
                else
                {
                    Plugin.BLogger.Warning(LogCategory.Boss, "There is a team created, we are going to apply the same team to the boss.");
                    var entityManager = Plugin.SystemsCore.EntityManager;
                    if (!entityManager.Exists(boss))
                        return;
                    
                    var TeamActual = entityManager.GetComponentData<Team>(boss);
                    var TeamReferenceActual = entityManager.GetComponentData<TeamReference>(boss);
                    var Team = Database.TeamDefault ?? new();
                    var TeamReference = Database.TeamReferenceDefault ?? new();
                    /*if (Team.Value == TeamActual.Value && TeamReference.Value.Value == TeamReferenceActual.Value.Value) {
                        Plugin.BLogger.Warning(LogCategory.Boss, "You already have the same equipment applied, we skip this step.");
                        return; 
                    }*/
                    entityManager.SetComponentData(boss, Team);
                    entityManager.SetComponentData(boss, TeamReference);
                    Plugin.BLogger.Warning(LogCategory.Boss, "We apply the changes so that they are from the same team.");
                }
            }
        } 


        internal static void GenerateStats()
        {
            var bossNeedStats = Database.BOSSES.Where(x=> x.unitStats == null).ToList();

            foreach (var bossModel in bossNeedStats)
            {
                bossModel.GenerateStats();
            }

            Database.saveDatabase();
        }
    }
}
