using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.Models.v1;
using BloodyBoss.DB;
using ProjectM.Gameplay.Systems;
using ProjectM;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Collections;
using ProjectM.Network;
using Bloodstone.API;
using BloodyBoss.Configuration;
using BloodyBoss.DB.Models;
using System.Text;

namespace BloodyBoss.Systems
{
    internal class NpcSystem
    {

        public static Dictionary<string, HashSet<string>> vbloodKills = new();
        public static Dictionary<string, HashSet<Entity>> vbloodKillsEntity = new();
        private static Dictionary<string, DateTime> lastKillerUpdate = new();
        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;


        internal static void OnDamageNpc(DealDamageSystem sender, NativeArray<DealDamageEvent> damageEvents)
        {
            foreach (var event_damage in damageEvents)
            {
                if (_entityManager.HasComponent<PlayerCharacter>(event_damage.SpellSource.Read<EntityOwner>().Owner))
                {
                    var player = _entityManager.GetComponentData<PlayerCharacter>(event_damage.SpellSource.Read<EntityOwner>().Owner);
                    var user = _entityManager.GetComponentData<User>(player.UserEntity);
                    try
                    {
                        NpcModel npc = GameData.Npcs.FromEntity(event_damage.Target);
                        var npcAssetName = _prefabCollectionSystem._PrefabDataLookup[npc.PrefabGUID].AssetName;
                        var modelBoss = Database.BOSSES.Where(x => x.AssetName == npcAssetName.ToString() && x.bossSpawn == true).FirstOrDefault();

                        if (modelBoss != null && modelBoss.GetBossNpcEntity())
                        {
                            AddKiller(npcAssetName.ToString(), user.CharacterName.ToString());
                            AddKillerEntity(npcAssetName.ToString(), event_damage.Target);
                            lastKillerUpdate[npcAssetName.ToString()] = DateTime.Now;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }


        internal static void OnDeathNpc(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            var processedEntities = new HashSet<Entity>(); // Para asegurarse de que solo procesamos cada evento una vez
            string keyToRemove = null; // Variable para almacenar la clave a eliminar

            foreach (var deathEvent in deathEvents)
            {
                var npcGUID = deathEvent.Died.Read<PrefabGUID>();
                var npc = _prefabCollectionSystem._PrefabDataLookup[npcGUID].AssetName;
                var modelBoss = Database.BOSSES.Where(x => x.AssetName == npc.ToString() && x.bossSpawn == true).FirstOrDefault();

                if (modelBoss != null && !processedEntities.Contains(deathEvent.Died))
                {
                    processedEntities.Add(deathEvent.Died); // Marcar la entidad como procesada

                    foreach (var kvp in lastKillerUpdate.ToList())
                    {
                        var lastUpdateTime = kvp.Value;
                        var timeDifference = DateTime.Now - lastUpdateTime;

                        if (timeDifference < TimeSpan.FromSeconds(5) && timeDifference.TotalSeconds > 0.1) // Incrementar el intervalo de tiempo a 5 segundos y permitir diferencias muy pequeñas
                        {
                            continue;
                        }

                        var npcs = QueryComponents.GetEntitiesByComponentTypes<UnitLevel, NameableInteractable, LifeTime>(default, true);
                        foreach (var entity in npcs)
                        {
                            if (entity.Equals(modelBoss.bossEntity))
                            {
                                if (entity.Has<VBloodUnit>())
                                {
                                    break;
                                }

                                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                                if (_nameableInteractable.Name.Value.Contains("bb"))
                                {
                                    var health = entity.Read<Health>();

                                    if (health.IsDead)
                                    {
                                        Entity user = UserSystem.GetOneUserOnline();
                                        modelBoss.RemoveIcon(user);
                                        modelBoss.bossSpawn = false;
                                        SendAnnouncementMessage(kvp.Key, modelBoss);
                                        keyToRemove = kvp.Key; // Almacenar la clave para eliminar después
                                        break;
                                    }
                                    else
                                    {
                                        RemoveKillers(kvp.Key);
                                        RemoveKillersEntity(kvp.Key);
                                        keyToRemove = kvp.Key; // Almacenar la clave para eliminar después
                                        break;
                                    }
                                }
                            }
                        }

                        if (keyToRemove != null)
                        {
                            break;
                        }
                    }

                    // Limpiar los asesinos solo después de mostrar el mensaje o remover asesinos
                    if (keyToRemove != null)
                    {
                        RemoveKillers(keyToRemove);
                        RemoveKillersEntity(keyToRemove);
                        lastKillerUpdate.Remove(keyToRemove);
                        keyToRemove = null; // Resetear la clave para eliminar
                    }
                }
            }
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

                // Mover estas líneas fuera del if para asegurarnos de que los asesinos se limpian solo cuando se ha enviado un mensaje correctamente
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




        /*
        internal static void OnDeathNpc(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            var didSkip = false;
            var showmessage = false;
            foreach (var deathEvent in deathEvents)
            {
                var npcGUID = deathEvent.Died.Read<PrefabGUID>();
                var npc = _prefabCollectionSystem._PrefabDataLookup[npcGUID].AssetName;
                var modelBoss = Database.BOSSES.Where(x => x.AssetName == npc.ToString() && x.bossSpawn == true).FirstOrDefault();
                if (modelBoss != null)
                {
                    foreach (KeyValuePair<string, DateTime> kvp in lastKillerUpdate)
                    {

                        var lastUpdateTime = kvp.Value;
                        if (DateTime.Now - lastUpdateTime < TimeSpan.FromSeconds(2))
                        {
                            didSkip = true;
                            continue;
                        }
                        var npcs = QueryComponents.GetEntitiesByComponentTypes<UnitLevel, NameableInteractable, LifeTime>(default, true);
                        foreach (var entity in npcs)
                        {
                            if (entity.Equals(modelBoss.bossEntity))
                            {
                                if(entity.Has<VBloodUnit>())
                                {
                                    break;
                                }
                                NameableInteractable _nameableInteractable = entity.Read<NameableInteractable>();
                                if (_nameableInteractable.Name.Value.Contains("bb"))
                                {

                                    var health = entity.Read<Health>();

                                    if (health.IsDead)
                                    {
                                        Entity user = UserSystem.GetOneUserOnline();
                                        modelBoss.RemoveIcon(user);
                                        modelBoss.bossSpawn = false;
                                        Plugin.Logger.LogInfo("MIERDAAAAAAAAAAAAAAA");
                                        SendAnnouncementMessage(kvp.Key, modelBoss);
                                        RemoveKillers(kvp.Key);
                                        RemoveKillersEntity(kvp.Key);
                                        showmessage = true;
                                        break;
                                    }
                                    else
                                    {

                                        RemoveKillers(kvp.Key);
                                        RemoveKillersEntity(kvp.Key);
                                        break;
                                    }
                                }
                            }
                        }

                        if (showmessage)
                        {
                            break;
                        }

                    }
                }
            }
        }*/


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
    }
}
