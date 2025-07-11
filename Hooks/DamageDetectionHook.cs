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
    // Hook simplificado para detectar daño - delega toda la lógica a BossGameplayEventSystem
    [HarmonyPatch(typeof(StatChangeSystem), nameof(StatChangeSystem.OnUpdate))]
    internal class DamageDetectionHook
    {
        [HarmonyPrefix]
        internal static void Prefix(StatChangeSystem __instance)
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
            
            Plugin.BLogger.Debug(LogCategory.Hook, $"[DAMAGE-DETECTION] Execute");

            // Usar _Query para acceder a StatChangeEvent
            var statChangeEvents = __instance._Query.ToComponentDataArray<StatChangeEvent>(Allocator.Temp);
            
            if (statChangeEvents.Length > 0)
            {
                Plugin.BLogger.Debug(LogCategory.Hook, $"[DAMAGE-DETECTION] Found {statChangeEvents.Length} stat change events");
            }
            
            foreach (var statChangeEvent in statChangeEvents)
            {
                // Solo procesar cambios de salud que sean daño (cambio negativo)
                if (statChangeEvent.StatType != StatType.Health || statChangeEvent.Change >= 0)
                    continue;
                
                var target = statChangeEvent.Entity;
                var damage = System.Math.Abs(statChangeEvent.Change);
                
                Plugin.BLogger.Debug(LogCategory.Hook, $"[DAMAGE-DETECTION] Health change - Target: {target.Index}, Damage: {damage:F1}");
                
                // Record damage for correlation
                DamageCorrelationSystem.RecordDamage(target, damage);
                
                if (!_entityManager.HasComponent<PrefabGUID>(target))
                    continue;
                    
                var targetPrefab = _entityManager.GetComponentData<PrefabGUID>(target);
                
                if (!_prefabCollectionSystem._PrefabDataLookup.TryGetValue(targetPrefab, out var prefabData))
                {
                    Plugin.BLogger.Debug(LogCategory.Hook, $"[DAMAGE-DETECTION] PrefabGUID {targetPrefab.GuidHash} not found in lookup");
                    continue;
                }
                
                var npcAssetName = prefabData.AssetName.ToString();
                
                // Delegar procesamiento a BossGameplayEventSystem
                BossGameplayEventSystem.ProcessStatChangeEvent(target, damage, npcAssetName);
            }
            
            statChangeEvents.Dispose();
        }
    }
}