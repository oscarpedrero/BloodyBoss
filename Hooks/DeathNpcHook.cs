using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using Bloody.Core.Helper.v1;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Services.Analytics.Internal;

namespace BloodyBoss.Hooks
{
    internal class DeathNpcHook
    {
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;
        private static BossEncounterModel killVBloodyModel;

        internal static void OnDeathNpc(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {

            Action killAction = () =>
            {
                killVBloodyModel.SendAnnouncementMessage();
            };


            if (deathEvents.Length > 0)
            {
                foreach (var deathEvent in deathEvents)
                {

                    if (_entityManager.HasComponent<PlayerCharacter>(deathEvent.Killer))
                    {
                        var npcGUID = deathEvent.Died.Read<PrefabGUID>();
                        var npc = _prefabCollectionSystem._PrefabDataLookup[npcGUID].AssetName;
                        var player = _entityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer);
                        var user = _entityManager.GetComponentData<User>(player.UserEntity);

                        var modelBoss = Database.BOSSES.Where(x => x.AssetName == npc.ToString() && x.bossSpawn == true).FirstOrDefault();

                        if (modelBoss != null)
                        {

                            if (modelBoss.bossEntity.Has<VBloodUnit>())
                            {
                                continue;
                            }

                            var health = modelBoss.bossEntity.Read<Health>();

                            if (health.IsDead)
                            {
                                var playerModel = GameData.Users.GetUserByCharacterName(user.CharacterName.Value);
                                killVBloodyModel = modelBoss;
                                modelBoss.BuffKillers();
                                CoroutineHandler.StartGenericCoroutine(killAction, 3);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
