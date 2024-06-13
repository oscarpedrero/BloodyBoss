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
                        if (modelBoss.GetBossEntity())
                        {

                            NameableInteractable _nameableInteractable = modelBoss.bossEntity.Read<NameableInteractable>();
                            if (_nameableInteractable.Name.Value == (modelBoss.nameHash + "bb"))
                            {
                                Entity userOnline = UserSystem.GetOneUserOnline();
                                modelBoss.BuffKillers();
                                if (modelBoss.bossSpawn)
                                {
                                    modelBoss.SendAnnouncementMessage();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
