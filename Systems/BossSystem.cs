using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bloodstone.API;
using BloodyBoss.Configuration;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using ProjectM;
using Unity.Entities;
using Unity.Collections;
using ProjectM.Network;
using BloodyBoss.Patch;
using Bloody.Core.Helper.v1;
using Bloody.Core.API.v1;
using Stunlock.Core;
using Unity.Entities.UniversalDelegates;
using ProjectM.Gameplay.Systems;
using Bloody.Core.Models.v1;
using Bloody.Core.GameData.v1;
using UnityEngine.Rendering.HighDefinition;


namespace BloodyBoss.Systems
{
    internal class BossSystem
    {
        private const double SendMessageDelay = 2;
        public static Dictionary<string, HashSet<string>> vbloodKills = new();
        public static Dictionary<string, HashSet<Entity>> vbloodKillsEntity = new();
        private static DateTime lastDateMinute = DateTime.Now;
        private static DateTime lastDateSecond = DateTime.Now;
        private static bool checkKiller = false;
        private static Dictionary<string, DateTime> lastKillerUpdate = new();
        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
        public static Action bossAction;



        public static void OnDeathVblood(VBloodSystem sender, NativeList<VBloodConsumed> deathEvents)
        {
            if (deathEvents.Length > 0)
            {
                foreach (var event_vblood in deathEvents)
                {
                    if (_entityManager.HasComponent<PlayerCharacter>(event_vblood.Target))
                    {
                        
                        var player = _entityManager.GetComponentData<PlayerCharacter>(event_vblood.Target);
                        var user = _entityManager.GetComponentData<User>(player.UserEntity);
                        var vblood = _prefabCollectionSystem._PrefabDataLookup[event_vblood.Source].AssetName;

                        var modelBoss = Database.BOSSES.Where(x => x.AssetName == vblood.ToString() && x.bossSpawn == true).FirstOrDefault();

                        //Entity entity = _prefabCollectionSystem._PrefabLookupMap[event_vblood.Source];
                        
                        if (modelBoss != null && modelBoss.GetBossEntity())
                        {
                            AddKiller(vblood.ToString(), user.CharacterName.ToString());
                            AddKillerEntity(vblood.ToString(), event_vblood.Target);
                            lastKillerUpdate[vblood.ToString()] = DateTime.Now;
                            checkKiller = true;
                        }
                      
                    }
                }
            }
            else if (checkKiller)
            {
                var didSkip = false;
                foreach (KeyValuePair<string, DateTime> kvp in lastKillerUpdate)
                {

                    var lastUpdateTime = kvp.Value;
                    if (DateTime.Now - lastUpdateTime < TimeSpan.FromSeconds(SendMessageDelay))
                    {
                        didSkip = true;
                        continue;
                    }
                    var modelBoss = Database.BOSSES.Where(x => x.AssetName == kvp.Key && x.bossSpawn == true).FirstOrDefault();
                    if (modelBoss != null)
                    {
                        var vBloods = QueryComponents.GetEntitiesByComponentTypes<VBloodUnit, LifeTime>(default, true);

                        foreach (var entity in vBloods)
                        {
                            if (entity.Equals(modelBoss.bossEntity))
                            {

                                var health = entity.Read<Health>();
                                
                                if (health.IsDead)
                                {
                                    Entity user = UserSystem.GetOneUserOnline();
                                    modelBoss.RemoveIcon(user);
                                    modelBoss.bossSpawn = false;
                                    Plugin.Logger.LogInfo("PUTAAAAAAAAAAAAAAAAAA");
                                    SendAnnouncementMessage(kvp.Key, modelBoss);
                                    break;
                                } else
                                {
                                    RemoveKillers(kvp.Key);
                                    RemoveKillersEntity(kvp.Key);
                                    break;
                                }
                            }
                        }
                    }
                }
                checkKiller = didSkip;
            }
        }
        
        public static void AddKiller(string vblood, string killerCharacterName)
        {
            if (!vbloodKills.ContainsKey(vblood))
            {
                vbloodKills[vblood] = new HashSet<string>();
            }
            vbloodKills[vblood].Add(killerCharacterName);
        }
        
        public static void AddKillerEntity(string vblood, Entity killerEntity)
        {
            if (!vbloodKillsEntity.ContainsKey(vblood))
            {
                vbloodKillsEntity[vblood] = new HashSet<Entity>();
            }
            vbloodKillsEntity[vblood].Add(killerEntity);
        }
        
        public static void RemoveKillers(string vblood)
        {
            vbloodKills[vblood] = new HashSet<string>();
        }
        
        public static void RemoveKillersEntity(string vblood)
        {
            vbloodKillsEntity[vblood] = new HashSet<Entity>();
        }
        
        public static List<string> GetKillers(string vblood)
        {
            return vbloodKills[vblood].ToList();
        }
        
        public static List<Entity> GetKillersEntity(string vblood)
        {
            return vbloodKillsEntity[vblood].ToList();
        }

        public static void SendAnnouncementMessage(string vblood, BossEncounterModel bossModel)
        {
            var message = GetAnnouncementMessage(vblood, bossModel.name);
            if (message != null)
            {
                var killers = GetKillers(vblood);
                bossModel.DropItems(vblood);

                ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, message);

                foreach (var killer in killers)
                {
                    ServerChatUtils.SendSystemMessageToAllClients(VWorld.Server.EntityManager, $"{FontColorChatSystem.Yellow($"- {killer}")}, ");
                }
                

                RemoveKillers(vblood);
                RemoveKillersEntity(vblood);
            }

        }

        public static string GetAnnouncementMessage(string vblood, string name)
        {
            var killers = GetKillers(vblood);
            var vbloodLabel = name;
            var _message = PluginConfig.KillMessageBossTemplate.Value;
            _message = _message.Replace("#vblood#", $"{FontColorChatSystem.Red(vbloodLabel)}");
            return FontColorChatSystem.Green($"{_message}");
        }

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
                            //var entityUnit = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(spawnBoss.PrefabGUID)];

                            //if (entityUnit.Has<VBloodUnit>())
                            //{
                                spawnBoss.CheckSpawnDespawn();
                            /*} else
                            {
                                Plugin.Logger.LogError($"The PrefabGUID does not correspond to a VBlood Unit. Ignore Spawn");
                            }*/
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
                                deSpawnBoss.CheckSpawnDespawn();
                            }
                            else
                            {
                                Plugin.Logger.LogError($"The PrefabGUID does not correspond to a VBlood Unit. Ignore Spawn");
                            }
                        }
                    }
                }
                ActionSchedulerPatch.RunActionOnceAfterFrames(bossAction, 30);
            };
            ActionSchedulerPatch.RunActionOnceAfterFrames(bossAction, 30);

        }

    }
}
