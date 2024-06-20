using Bloody.Core;
using Bloody.Core.GameData.v1;
using BloodyBoss.DB;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using System.Linq;
using Unity.Collections;
using Unity.Entities;

namespace BloodyBoss.Hooks
{
    internal class DeathNpcHook
    {
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;

        internal static void OnDeathNpc(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {

            foreach (var deathEvent in deathEvents)
            {

                if (_entityManager.HasComponent<PlayerCharacter>(deathEvent.Killer))
                {
                    var npcGUID = deathEvent.Died.Read<PrefabGUID>();
                    var npc = _prefabCollectionSystem._PrefabDataLookup[npcGUID].AssetName;
                    var player = _entityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer);
                    var user = _entityManager.GetComponentData<User>(player.UserEntity);

                    var modelBosses = Database.BOSSES.Where(x => x.AssetName == npc.ToString() && x.bossSpawn == true).ToList();
                    foreach (var modelBoss in modelBosses)
                    {

                        if (modelBoss.bossEntity.Has<VBloodUnit>())
                        {
                            continue;
                        }

                        var health = modelBoss.bossEntity.Read<Health>();
                        
                        if (health.IsDead || health.Value == 0)
                        {
                            var playerModel = GameData.Users.GetUserByCharacterName(user.CharacterName.Value);
                            modelBoss.BuffKillers();
                            modelBoss.SendAnnouncementMessage();
                            break;
                        }
                    }
                }
            }
        }
    }
}
