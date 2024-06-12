using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using BloodyBoss.Configuration;
using BloodyBoss.DB;
using ProjectM.Gameplay.Systems;
using ProjectM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Collections;
using Bloody.Core;
using ProjectM.Network;
using HarmonyLib;
using BloodyBoss.DB.Models;

namespace BloodyBoss.Hooks
{
    [HarmonyPatch(typeof(DealDamageSystem), nameof(DealDamageSystem.DealDamage))]
    internal class DealDamageHook
    {

        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;

        internal static bool Prefix(DealDamageSystem __instance)
        {

            var damageTakenEvent = __instance._Query.ToComponentDataArray<DealDamageEvent>(Allocator.Temp);
            foreach (var event_damage in damageTakenEvent)
            {
                
                try
                {
                    NpcModel npc = GameData.Npcs.FromEntity(event_damage.Target);
                    var npcAssetName = _prefabCollectionSystem._PrefabDataLookup[npc.PrefabGUID].AssetName;
                    var modelBoss = Database.BOSSES.Where(x => x.AssetName == npcAssetName.ToString() && x.bossSpawn == true).FirstOrDefault();
                    if (modelBoss != null && modelBoss.GetBossEntity() && modelBoss.bossEntity.Has<NameableInteractable>())
                    {
                        NameableInteractable _nameableInteractable = modelBoss.bossEntity.Read<NameableInteractable>();
                        if (_nameableInteractable.Name.Value == (modelBoss.nameHash + "bb"))
                        {
                            if (_entityManager.HasComponent<PlayerCharacter>(event_damage.SpellSource.Read<EntityOwner>().Owner))
                            {
                                var owner = event_damage.SpellSource.Read<EntityOwner>().Owner;

                                var player = _entityManager.GetComponentData<PlayerCharacter>(event_damage.SpellSource.Read<EntityOwner>().Owner);
                                var user = _entityManager.GetComponentData<User>(player.UserEntity);

                                modelBoss.AddKiller(user.CharacterName.ToString());
                            }
                            else
                            {
                                if (PluginConfig.MinionDamage.Value)
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }                       
                    }

                }
                catch
                {

                    continue;
                }
            }

            return true;
        }
    }
}
