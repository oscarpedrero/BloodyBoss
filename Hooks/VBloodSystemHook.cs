using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using BloodyBoss.DB;
using ProjectM;
using ProjectM.Network;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace BloodyBoss.Hooks
{
    internal class VBloodSystemHook
    {

        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;

        public static void OnDeathVblood(VBloodSystem __instance, NativeList<VBloodConsumed> deathEvents)
        {
            
            foreach (var event_vblood in deathEvents)
            {

                if (_entityManager.HasComponent<PlayerCharacter>(event_vblood.Target))
                {

                    var player = _entityManager.GetComponentData<PlayerCharacter>(event_vblood.Target);

                    var user = _entityManager.GetComponentData<User>(player.UserEntity);

                    var playerModel = GameData.Users.GetUserByCharacterName(user.CharacterName.ToString());
                    var vblood = _prefabCollectionSystem._PrefabDataLookup[event_vblood.Source].AssetName;
                    var modelBosses = Database.BOSSES.Where(x => x.AssetName == vblood.ToString() && x.bossSpawn == true).ToList();
                    foreach (var modelBoss in modelBosses)
                    {

                        if (modelBoss.vbloodFirstKill)
                        {
                            modelBoss.AddKiller(user.CharacterName.ToString());
                            modelBoss.BuffKillers();
                        } else
                        {
                            if (modelBoss.GetBossEntity())
                            {
                                var health = modelBoss.bossEntity.Read<Health>();
                                if (health.IsDead || health.Value == 0)
                                {
                                    modelBoss.vbloodFirstKill = true;
                                    modelBoss.AddKiller(user.CharacterName.ToString());
                                    modelBoss.BuffKillers();
                                    if (modelBoss.bossSpawn)
                                    {
                                        var killAction = () =>
                                        {
                                            modelBoss.vbloodFirstKill = false;
                                            modelBoss.SendAnnouncementMessage();
                                        };
                                        CoroutineHandler.StartGenericCoroutine(killAction, 2);
                                        
                                    }
                                }
                            }
                        }
                        
                    }
                }
            }
        }
    }
}
