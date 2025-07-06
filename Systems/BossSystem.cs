using System;
using System.Linq;
using BloodyBoss.Configuration;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using ProjectM;
using Unity.Entities;
using Bloody.Core.Helper.v1;
using Bloody.Core.API.v1;
using Stunlock.Core;
using Bloody.Core;
using Bloody.Core.Patch.Server;
using Bloody.Core.GameData.v1;
using Newtonsoft.Json.Utilities;


namespace BloodyBoss.Systems
{
    
    internal class BossSystem
    {
        
        private static int lastMinute = DateTime.Now.Minute;
        private static int lastSecond = DateTime.Now.Second;
        
       
        public static Action bossAction;

        public static void StartTimer()
        {
            Plugin.Logger.LogInfo("Start Timner for BloodyBoss");
            bossAction = () =>
            {
                var now = DateTime.Now;
                bool minuteChanged = now.Minute != lastMinute;
                bool secondChanged = now.Second != lastSecond;
                
                if (minuteChanged)
                {
                    lastMinute = now.Minute;
                    var currentTime = now.ToString("HH:mm");
                    var spawnsBoss = Database.BOSSES.Where(x => x.Hour == currentTime && !x.IsPaused).ToList();
                    if (spawnsBoss.Count > 0)
                    {
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
                
                if (secondChanged)
                {
                    lastSecond = now.Second;
                    var currentTimeWithSeconds = now.ToString("HH:mm:ss");
                    var despawnsBoss = Database.BOSSES.Where(x => x.HourDespawn == currentTimeWithSeconds && x.bossSpawn == true && !x.IsPaused).ToList();
                    if (despawnsBoss.Count > 0)
                    {
                        foreach (var deSpawnBoss in despawnsBoss)
                        {
                            var entityUnit = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(deSpawnBoss.PrefabGUID)];

                            if (entityUnit.Has<VBloodUnit>())
                            {
                                Action actionSpawnDespawn = () =>
                                {
                                    deSpawnBoss.CheckSpawnDespawn();
                                };
                                
                                ActionScheduler.RunActionOnMainThread(actionSpawnDespawn);
                            }
                            else
                            {
                                Plugin.Logger.LogError($"The PrefabGUID does not correspond to a VBlood Unit. Ignore Spawn");
                            }
                        }
                    }
                }
            };
            CoroutineHandler.StartRepeatingCoroutine(bossAction, 1);

        }

        public void StartTimerteam()
        {
            var teamAction = () => {
                var vBloods = QueryComponents.GetEntitiesByComponentTypes<VBloodUnit, LifeTime>(default, true);
            };
        }

        internal static void CheckBoss()
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, UnitStats>(EntityQueryOptions.IncludeDisabledEntities);
            foreach (var entity in entities)
            {
                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                if (_nameableInteractable.Name.Value.Contains("bb"))
                {
                    BossEncounterModel bossModel = Database.BOSSES.Where(x => (x.nameHash + "bb") == _nameableInteractable.Name.Value).FirstOrDefault();
                    if (bossModel != null)
                    {
                        try
                        {
                            var now = DateTime.Now;
                            var dateTimeDespawn = DateTime.ParseExact(bossModel.HourDespawn, "HH:mm:ss", null, System.Globalization.DateTimeStyles.None);
                            var userModel = GameData.Users.All.FirstOrDefault();
                            var user = userModel.Entity;
                            if (now >= dateTimeDespawn)
                            {
                                bossModel.bossSpawn = false;
                                StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, entity, user, user, 0, StatChangeReason.Any, true);
                                bossModel.RemoveIcon(user);
                            }
                            else
                            {
                                bossModel.ModifyBoss(user, entity);
                                CheckTeams(entity);
                                if (PluginConfig.ClearDropTable.Value)
                                {
                                    var action = () =>
                                    {
                                        bossModel.ClearDropTable(entity);
                                    };
                                    CoroutineHandler.StartFrameCoroutine(action, 10, 1);
                                }
                                bossModel.bossSpawn = true;
                            }
                        } catch (Exception ex)
                        {
                            Plugin.Logger.LogError(ex.Message);
                            continue;
                        }
                        
                    }
                }
            }
            entities.Dispose();
            StartTimer();
        }

        internal static void CheckTeams(Entity boss)
        {
            if (PluginConfig.TeamBossEnable.Value)
            {
                Plugin.Logger.LogWarning("Adding the same team to the boss");
                if (Database.TeamDefault == null)
                {
                    Database.TeamDefault = boss.Read<Team>();
                    Database.TeamReferenceDefault = boss.Read<TeamReference>();
                    Plugin.Logger.LogWarning("There is no team created, so we create the new Team");
                }
                else
                {
                    Plugin.Logger.LogWarning("There is a team created, we are going to apply the same team to the boss.");
                    var TeamActual = boss.Read<Team>();
                    var TeamReferenceActual = boss.Read<TeamReference>();
                    var Team = Database.TeamDefault ?? new();
                    var TeamReference = Database.TeamReferenceDefault ?? new();
                    /*if (Team.Value == TeamActual.Value && TeamReference.Value.Value == TeamReferenceActual.Value.Value) {
                        Plugin.Logger.LogWarning("You already have the same equipment applied, we skip this step.");
                        return; 
                    }*/
                    boss.Write(Team);
                    boss.Write(TeamReference);
                    Plugin.Logger.LogWarning("We apply the changes so that they are from the same team.");
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
