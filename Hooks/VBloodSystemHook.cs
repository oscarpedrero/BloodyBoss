using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Helper.v1;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using System;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace BloodyBoss.Hooks
{
    internal class VBloodSystemHook
    {

        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
        private static BossEncounterModel killVBloodyModel = null;

        public static void OnDeathVblood(VBloodSystem __instance, NativeList<VBloodConsumed> deathEvents)
        {

            Action killAction = () =>
            {   if(!killVBloodyModel.bossSpawn) killVBloodyModel.SendAnnouncementMessage();
            };

            foreach (var event_vblood in deathEvents)
            {
                if (_entityManager.HasComponent<PlayerCharacter>(event_vblood.Target))
                {
                    var player = _entityManager.GetComponentData<PlayerCharacter>(event_vblood.Target);
                    var user = _entityManager.GetComponentData<User>(player.UserEntity);
                    var playerModel = GameData.Users.GetUserByCharacterName(user.CharacterName.ToString());
                    var vblood = _prefabCollectionSystem._PrefabDataLookup[event_vblood.Source].AssetName;

                    var modelBoss = Database.BOSSES.Where(x => x.AssetName == vblood.ToString() && x.bossSpawn == true).FirstOrDefault();
                    if (modelBoss != null && modelBoss.GetBossEntity())
                    {
                        var health = modelBoss.bossEntity.Read<Health>();

                        if (health.IsDead && modelBoss.bossEntity.Has<VBloodUnit>())
                        {
                            Entity userOnline = UserSystem.GetOneUserOnline();
                            killVBloodyModel = modelBoss;
                            modelBoss.BuffKillers();
                            CoroutineHandler.StartGenericCoroutine(killAction, 3);
                        }
                    }

                }
            }
            
        }
    }
}
