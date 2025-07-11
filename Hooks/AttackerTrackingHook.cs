using ProjectM;
using ProjectM.Gameplay.Systems;
using Unity.Entities;
using Unity.Collections;
using HarmonyLib;
using BloodyBoss.Systems;
using Stunlock.Core;
using Bloody.Core;

namespace BloodyBoss.Hooks
{
    // Hook simplificado para rastrear atacantes - delega toda la lógica a BossGameplayEventSystem
    [HarmonyPatch(typeof(StatChangeSystem), nameof(StatChangeSystem.OnUpdate))]
    internal class AttackerTrackingHook
    {
        [HarmonyPostfix]
        internal static void Postfix(StatChangeSystem __instance)
        {
            // Null safety checks
            if (__instance?.EntityManager == null)
            {
                return;
            }
            
            if (Plugin.SystemsCore?.EntityManager == null || Plugin.SystemsCore?.PrefabCollectionSystem == null)
            {
                return;
            }
            
            var _entityManager = __instance.EntityManager;
            var _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
            
            Plugin.BLogger.Debug(LogCategory.Hook, $"[ATTACKER-TRACKING] Execute");

            // Usar _DamageTakenEventQuery para acceder a DamageTakenEvent
            var entities = __instance._DamageTakenEventQuery.ToEntityArray(Allocator.Temp);
            var damageTakenEvents = __instance._DamageTakenEventQuery.ToComponentDataArray<DamageTakenEvent>(Allocator.Temp);
            
            if (damageTakenEvents.Length > 0)
            {
                Plugin.BLogger.Debug(LogCategory.Hook, $"[ATTACKER-TRACKING] Found {damageTakenEvents.Length} damage taken events");
            }
            
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];
                DamageTakenEvent damageTakenEvent = damageTakenEvents[i];
                
                var target = damageTakenEvent.Entity;
                var source = damageTakenEvent.Source;
                
                Plugin.BLogger.Debug(LogCategory.Hook, $"[ATTACKER-TRACKING] DamageTakenEvent - Target: {target.Index}, Source: {source.Index}");
                
                // Extract actual attacker and record for correlation
                Entity attacker = Entity.Null;
                if (_entityManager.HasComponent<EntityOwner>(source))
                {
                    var owner = _entityManager.GetComponentData<EntityOwner>(source);
                    attacker = owner.Owner;
                }
                else
                {
                    attacker = source;
                }
                
                DamageCorrelationSystem.RecordAttacker(target, attacker);
                
                if (!_entityManager.HasComponent<PrefabGUID>(target))
                    continue;
                    
                var targetPrefab = _entityManager.GetComponentData<PrefabGUID>(target);
                var npcAssetName = _prefabCollectionSystem._PrefabDataLookup[targetPrefab].AssetName.ToString();
                
                // Delegar procesamiento a BossGameplayEventSystem
                BossGameplayEventSystem.ProcessDamageTakenEvent(target, source, npcAssetName);
            }
            
            entities.Dispose();
            damageTakenEvents.Dispose();
        }
    }
}