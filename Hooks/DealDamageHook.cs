using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using BloodyBoss.Configuration;
using BloodyBoss.DB;
using ProjectM.Gameplay.Systems;
using ProjectM;
using System;
using System.Linq;
using Unity.Entities;
using Unity.Collections;
using Bloody.Core;
using ProjectM.Network;
using HarmonyLib;
using Stunlock.Core;
using BloodyBoss.Systems;
using System.Collections.Generic;

namespace BloodyBoss.Hooks
{
    [HarmonyPatch(typeof(DealDamageSystem), nameof(DealDamageSystem.DealDamage))]
    internal class DealDamageHook
    {

        private static EntityManager _entityManager = Plugin.SystemsCore.EntityManager;
        private static PrefabCollectionSystem _prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
        private static Dictionary<string, int> _lastHpLog = new Dictionary<string, int>();

        internal static bool Prefix(DealDamageSystem __instance)
        {
            var damageTakenEvent = __instance._Query.ToComponentDataArray<DealDamageEvent>(Allocator.Temp);
            
            if (damageTakenEvent.Length > 0)
            {
                Plugin.Logger.LogDebug($"[DAMAGE-HOOK] Processing {damageTakenEvent.Length} damage events");
            }
            
            foreach (var event_damage in damageTakenEvent)
            {
                
                try
                {
                    if (!event_damage.Target.Has<PrefabGUID>())
                        continue;
                        
                    var targetPrefab = event_damage.Target.Read<PrefabGUID>();
                    var npcAssetName = _prefabCollectionSystem._PrefabDataLookup[targetPrefab].AssetName;
                    
                    // Log all damage events to bosses
                    if (event_damage.Target.Has<NameableInteractable>())
                    {
                        var nameable = event_damage.Target.Read<NameableInteractable>();
                        Plugin.Logger.LogDebug($"[DAMAGE-HOOK] Damage to: {nameable.Name.Value}, Asset: {npcAssetName}");
                    }
                    
                    var modelBosses = Database.BOSSES.Where(x => x.AssetName == npcAssetName.ToString() && x.bossSpawn == true).ToList();
                    
                    if (modelBosses.Count > 0)
                    {
                        Plugin.Logger.LogInfo($"[DAMAGE-HOOK] Found {modelBosses.Count} bosses matching asset: {npcAssetName}");
                    }
                    foreach (var modelBoss in modelBosses)
                    {
                        if (modelBoss != null && modelBoss.GetBossEntity() && modelBoss.bossEntity.Has<NameableInteractable>())
                        {
                            NameableInteractable _nameableInteractable = modelBoss.bossEntity.Read<NameableInteractable>();
                            Plugin.Logger.LogDebug($"[DAMAGE-HOOK] Checking boss entity: Expected name={modelBoss.nameHash}bb, Actual name={_nameableInteractable.Name.Value}");
                            
                            if (_nameableInteractable.Name.Value == (modelBoss.nameHash + "bb"))
                            {
                                Plugin.Logger.LogInfo($"[DAMAGE-HOOK] MATCH! Processing damage for boss {modelBoss.name}");
                                if (_entityManager.HasComponent<PlayerCharacter>(event_damage.SpellSource.Read<EntityOwner>().Owner))
                                {

                                    if (modelBoss.bossEntity.Has<VBloodUnit>())
                                    {
                                        // For VBlood bosses, only check mechanics and then continue
                                        if (modelBoss.bossEntity.Has<Health>())
                                        {
                                            var health = modelBoss.bossEntity.Read<Health>();
                                            float currentHpPercent = (health.Value / health.MaxHealth._Value) * 100f;
                                            
                                            // Log every 10% HP change
                                            var hpBracket = ((int)currentHpPercent / 10) * 10;
                                            var lastLogKey = $"{modelBoss.nameHash}_hp_log";
                                            if (!_lastHpLog.ContainsKey(lastLogKey) || Math.Abs(_lastHpLog[lastLogKey] - hpBracket) >= 10)
                                            {
                                                Plugin.Logger.LogInfo($"[DAMAGE-HOOK] VBlood Boss {modelBoss.name} HP: {currentHpPercent:F1}% ({health.Value:F0}/{health.MaxHealth._Value:F0})");
                                                _lastHpLog[lastLogKey] = hpBracket;
                                            }
                                            
                                            BossMechanicSystem.CheckHpThresholdMechanics(modelBoss.bossEntity, modelBoss, currentHpPercent);
                                        }
                                        continue;
                                    }

                                    var owner = event_damage.SpellSource.Read<EntityOwner>().Owner;

                                    var player = _entityManager.GetComponentData<PlayerCharacter>(event_damage.SpellSource.Read<EntityOwner>().Owner);
                                    var user = _entityManager.GetComponentData<User>(player.UserEntity);

                                    modelBoss.AddKiller(user.CharacterName.ToString());
                                    
                                    // Check mechanics based on HP threshold for non-VBlood bosses
                                    if (modelBoss.bossEntity.Has<Health>())
                                    {
                                        var health = modelBoss.bossEntity.Read<Health>();
                                        float currentHpPercent = (health.Value / health.MaxHealth._Value) * 100f;
                                        
                                        // Log every 10% HP change
                                        var hpBracket = ((int)currentHpPercent / 10) * 10;
                                        var lastLogKey = $"{modelBoss.nameHash}_hp_log";
                                        if (!_lastHpLog.ContainsKey(lastLogKey) || Math.Abs(_lastHpLog[lastLogKey] - hpBracket) >= 10)
                                        {
                                            Plugin.Logger.LogInfo($"[DAMAGE-HOOK] Boss {modelBoss.name} HP: {currentHpPercent:F1}% ({health.Value:F0}/{health.MaxHealth._Value:F0})");
                                            _lastHpLog[lastLogKey] = hpBracket;
                                        }
                                        
                                        BossMechanicSystem.CheckHpThresholdMechanics(modelBoss.bossEntity, modelBoss, currentHpPercent);
                                    }
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

                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogError($"Error processing damage event: {ex.Message}");
                    continue;
                }
            }

            return true;
        }
    }
}
