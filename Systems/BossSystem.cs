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
        
        private static DateTime lastDateMinute = DateTime.Now;
        private static DateTime lastDateSecond = DateTime.Now;
        
       
        public static Action bossAction;

        public static void StartTimer()
        {
            Plugin.Logger.LogInfo("Start Timner for BloodyBoss");
            bossAction = () =>
            {
                var date = DateTime.Now;
                if (lastDateMinute.ToString("HH:mm") != date.ToString("HH:mm"))
                {
                    lastDateMinute = date;
                    var spawnsBoss = Database.BOSSES.Where(x => x.Hour == date.ToString("HH:mm")).ToList();
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
                if (lastDateSecond.ToString("HH:mm:ss") != date.ToString("HH:mm:ss"))
                {
                    lastDateSecond = date;
                    var despawnsBoss = Database.BOSSES.Where(x => x.HourDespawn == date.ToString("HH:mm:ss") && x.bossSpawn == true).ToList();
                    if (despawnsBoss != null)
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
                                DeleteIcon(bossModel);
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

        internal static void DeleteIcon(BossEncounterModel model)
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
            foreach (var entity in entities)
            {
                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                if (_nameableInteractable.Name.Value == model.nameHash + "ibb")
                {
                    var userModel = GameData.Users.All.FirstOrDefault();
                    var user = userModel.Entity;
                    StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, entity, user, user, 0, StatChangeReason.Any, true);
                    break;
                }
            }
            entities.Dispose();
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
