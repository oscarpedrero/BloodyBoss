using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using ProjectM;
using ProjectM.Network;
using ProjectM.Gameplay.Systems;
using ProjectM.Shared;
using Stunlock.Core;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Configuration;
using BloodyBoss.Utils;
using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.Helper.v1;
using Bloody.Core.GameData.v1;
using System.Linq;

namespace BloodyBoss.Systems
{
    /// <summary>
    /// System to handle damage events for BloodyBoss entities
    /// Processes both damage detection and attacker tracking
    /// </summary>
    public static class BossGameplayEventSystem
    {
        // Track which entities are BloodyBosses
        private static Dictionary<Entity, BossEncounterModel> _trackedBosses = new Dictionary<Entity, BossEncounterModel>();
        private static Dictionary<string, int> _lastHpLog = new Dictionary<string, int>();
        private static DateTime _lastValidationTime = DateTime.UtcNow;
        private static readonly int MAX_TRACKED_BOSSES = 50; // Reasonable limit for concurrent bosses
        
        /// <summary>
        /// Handler for EventsHandlerSystem.OnDamage
        /// </summary>
        public static void OnDamageEvent(DealDamageSystem sender, NativeArray<DealDamageEvent> damageEvents)
        {
            if (damageEvents.Length > 0)
            {
                Plugin.Logger.LogInfo($"[BossGameplayEventSystem] OnDamageEvent called with {damageEvents.Length} damage events");
            }
            
            foreach (var damageEvent in damageEvents)
            {
                Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Checking damage to entity {damageEvent.Target.Index}:{damageEvent.Target.Version}");
                
                // Check if target is a tracked boss
                if (!IsTrackedBoss(damageEvent.Target))
                {
                    continue;
                }
                
                Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Damage detected to tracked boss!");
                ProcessDamageEvent(damageEvent);
            }
        }
        
        /// <summary>
        /// Process damage event for potential boss entities
        /// </summary>
        public static void ProcessDamageEvent(DealDamageEvent damageEvent)
        {
            try
            {
                Plugin.Logger.LogInfo($"[BossGameplayEventSystem] ProcessDamageEvent called for entity {damageEvent.Target.Index}:{damageEvent.Target.Version}");
                
                // Check if target is a tracked boss
                if (!_trackedBosses.TryGetValue(damageEvent.Target, out var modelBoss))
                {
                    Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Entity {damageEvent.Target.Index}:{damageEvent.Target.Version} is not a tracked boss");
                    return;
                }
                
                Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Processing damage for tracked boss {modelBoss.name}");
                
                var entityManager = Plugin.SystemsCore.EntityManager;
                
                // Verify the entity still exists and has required components
                if (!entityManager.Exists(damageEvent.Target) || 
                    !entityManager.HasComponent<Health>(damageEvent.Target) ||
                    !entityManager.HasComponent<NameableInteractable>(damageEvent.Target))
                {
                    Plugin.Logger.LogWarning($"[BossGameplayEventSystem] Boss entity no longer valid, removing from tracking");
                    _trackedBosses.Remove(damageEvent.Target);
                    return;
                }
                
                // Verify it's still the same boss by checking the name
                var nameable = entityManager.GetComponentData<NameableInteractable>(damageEvent.Target);
                if (nameable.Name.Value != (modelBoss.nameHash + "bb"))
                {
                    Plugin.Logger.LogWarning($"[BossGameplayEventSystem] Entity name mismatch, removing from tracking");
                    _trackedBosses.Remove(damageEvent.Target);
                    return;
                }
                
                // Check if damage is from a player
                if (!entityManager.HasComponent<EntityOwner>(damageEvent.SpellSource))
                {
                    return;
                }
                
                var owner = entityManager.GetComponentData<EntityOwner>(damageEvent.SpellSource).Owner;
                if (!entityManager.HasComponent<PlayerCharacter>(owner))
                {
                    // Handle minion damage settings
                    if (!PluginConfig.MinionDamage.Value)
                    {
                        Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Non-player damage to boss, MinionDamage disabled");
                    }
                    return;
                }
                
                // Process player damage
                var player = entityManager.GetComponentData<PlayerCharacter>(owner);
                var user = entityManager.GetComponentData<User>(player.UserEntity);
                
                // Add killer to the list
                modelBoss.AddKiller(user.CharacterName.ToString());
                
                // Get health component
                var health = entityManager.GetComponentData<Health>(damageEvent.Target);
                
                // Calculate HP percentage
                float currentHpPercent = (health.Value / health.MaxHealth._Value) * 100f;
                
                // Log HP changes every 10%
                var hpBracket = ((int)currentHpPercent / 10) * 10;
                var lastLogKey = $"{modelBoss.nameHash}_hp_log";
                
                if (!_lastHpLog.ContainsKey(lastLogKey) || Math.Abs(_lastHpLog[lastLogKey] - hpBracket) >= 10)
                {
                    var isVBlood = entityManager.HasComponent<VBloodUnit>(damageEvent.Target) ? "VBlood " : "";
                    Plugin.Logger.LogInfo($"[BossGameplayEventSystem] {isVBlood}Boss {modelBoss.name} HP: {currentHpPercent:F1}% ({health.Value:F0}/{health.MaxHealth._Value:F0})");
                    _lastHpLog[lastLogKey] = hpBracket;
                }
                
                // Check mechanics based on HP threshold
                BossMechanicSystem.CheckHpThresholdMechanics(damageEvent.Target, modelBoss, currentHpPercent);
                
                Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Damage processed successfully for {modelBoss.name}");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[BossGameplayEventSystem] Error processing boss damage: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Register a boss entity for damage tracking
        /// </summary>
        internal static void RegisterBoss(Entity bossEntity, BossEncounterModel model)
        {
            try
            {
                // Validate tracked bosses periodically
                ValidateTrackedBosses();
                
                if (_trackedBosses.ContainsKey(bossEntity))
                {
                    Plugin.Logger.LogWarning($"[BossGameplayEventSystem] Boss {model.name} already registered, updating model");
                }
                
                // Enforce maximum tracked bosses
                if (_trackedBosses.Count >= MAX_TRACKED_BOSSES)
                {
                    Plugin.Logger.LogWarning($"[BossGameplayEventSystem] Maximum tracked bosses reached ({MAX_TRACKED_BOSSES}). Cleaning up invalid entries.");
                    ForceValidateTrackedBosses();
                    
                    // If still at limit after cleanup, skip registration
                    if (_trackedBosses.Count >= MAX_TRACKED_BOSSES)
                    {
                        Plugin.Logger.LogError($"[BossGameplayEventSystem] Cannot register boss {model.name} - limit reached even after cleanup");
                        return;
                    }
                }
                
                _trackedBosses[bossEntity] = model;
                Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Registered boss {model.name} for damage tracking");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[BossGameplayEventSystem] Failed to register boss: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Unregister a boss entity from damage tracking
        /// </summary>
        public static void UnregisterBoss(Entity bossEntity)
        {
            try
            {
                if (_trackedBosses.Remove(bossEntity))
                {
                    Plugin.Logger.LogInfo("[BossGameplayEventSystem] Unregistered boss from damage tracking");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[BossGameplayEventSystem] Failed to unregister boss: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Check if an entity is a tracked boss
        /// </summary>
        public static bool IsTrackedBoss(Entity entity)
        {
            return _trackedBosses.ContainsKey(entity);
        }
        
        /// <summary>
        /// Get the model for a tracked boss
        /// </summary>
        internal static bool TryGetBossModel(Entity entity, out BossEncounterModel model)
        {
            return _trackedBosses.TryGetValue(entity, out model);
        }
        
        /// <summary>
        /// Clear all tracked bosses (useful for cleanup)
        /// </summary>
        public static void ClearAllTrackedBosses()
        {
            _trackedBosses.Clear();
            _lastHpLog.Clear();
            Plugin.Logger.LogInfo("[BossGameplayEventSystem] Cleared all tracked bosses");
        }
        
        /// <summary>
        /// Debug method to list all tracked bosses
        /// </summary>
        public static void DebugListTrackedBosses()
        {
            Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Currently tracking {_trackedBosses.Count} bosses:");
            foreach (var kvp in _trackedBosses)
            {
                Plugin.Logger.LogInfo($"  - Entity {kvp.Key.Index}:{kvp.Key.Version} = Boss {kvp.Value.name}");
            }
        }
        
        /// <summary>
        /// Validate tracked bosses periodically (every 30 seconds)
        /// </summary>
        private static void ValidateTrackedBosses()
        {
            var now = DateTime.UtcNow;
            if ((now - _lastValidationTime).TotalSeconds < 30)
                return;
                
            _lastValidationTime = now;
            ForceValidateTrackedBosses();
        }
        
        /// <summary>
        /// Force validation of all tracked bosses
        /// </summary>
        private static void ForceValidateTrackedBosses()
        {
            if (Plugin.SystemsCore?.EntityManager == null)
                return;
                
            var entityManager = Plugin.SystemsCore.EntityManager;
            var entitiesToRemove = new List<Entity>();
            
            foreach (var kvp in _trackedBosses)
            {
                var entity = kvp.Key;
                var model = kvp.Value;
                
                // Check if entity still exists and is alive
                if (!entityManager.Exists(entity))
                {
                    entitiesToRemove.Add(entity);
                    Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Removing non-existent entity from tracking: {model.name}");
                }
                else if (!entityManager.HasComponent<Health>(entity))
                {
                    entitiesToRemove.Add(entity);
                    Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Removing entity without Health component: {model.name}");
                }
                else
                {
                    var health = entityManager.GetComponentData<Health>(entity);
                    if (health.Value <= 0)
                    {
                        entitiesToRemove.Add(entity);
                        Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Removing dead boss from tracking: {model.name}");
                    }
                }
            }
            
            foreach (var entity in entitiesToRemove)
            {
                _trackedBosses.Remove(entity);
            }
            
            if (entitiesToRemove.Count > 0)
            {
                Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Validated tracked bosses. Removed {entitiesToRemove.Count} invalid entries. Current count: {_trackedBosses.Count}");
            }
        }
        
        /// <summary>
        /// Process damage from StatChangeEvent (called by DamageDetectionHook)
        /// </summary>
        public static void ProcessStatChangeEvent(Entity target, float damage, string npcAssetName)
        {
            try
            {
                var entityManager = Plugin.SystemsCore.EntityManager;
                var prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
                
                // Log all damage events to bosses
                if (entityManager.HasComponent<NameableInteractable>(target))
                {
                    var nameable = entityManager.GetComponentData<NameableInteractable>(target);
                    Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Damage to: {nameable.Name.Value}, Amount: {damage:F1}, Asset: {npcAssetName}");
                }
                
                // Find matching bosses
                var modelBosses = Database.BOSSES.Where(x => x.AssetName == npcAssetName && x.bossSpawn == true).ToList();
                
                if (modelBosses.Count > 0)
                {
                    Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Found {modelBosses.Count} bosses matching asset: {npcAssetName}");
                }
                
                foreach (var modelBoss in modelBosses)
                {
                    if (modelBoss != null && modelBoss.GetBossEntity() && modelBoss.bossEntity.Has<NameableInteractable>())
                    {
                        NameableInteractable _nameableInteractable = modelBoss.bossEntity.Read<NameableInteractable>();
                        Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Checking boss entity: Expected name={modelBoss.nameHash}bb, Actual name={_nameableInteractable.Name.Value}");
                        
                        if (_nameableInteractable.Name.Value == (modelBoss.nameHash + "bb"))
                        {
                            Plugin.Logger.LogInfo($"[BossGameplayEventSystem] MATCH! Processing damage for boss {modelBoss.name}");
                            
                            // Register boss if not already tracked
                            if (!IsTrackedBoss(modelBoss.bossEntity))
                            {
                                RegisterBoss(modelBoss.bossEntity, modelBoss);
                            }
                            
                            // Process HP-based mechanics
                            if (modelBoss.bossEntity.Has<Health>())
                            {
                                var health = modelBoss.bossEntity.Read<Health>();
                                float currentHpPercent = (health.Value / health.MaxHealth._Value) * 100f;
                                
                                // Log every 10% HP change
                                var hpBracket = ((int)currentHpPercent / 10) * 10;
                                var lastLogKey = $"{modelBoss.nameHash}_hp_log";
                                if (!_lastHpLog.ContainsKey(lastLogKey) || Math.Abs(_lastHpLog[lastLogKey] - hpBracket) >= 10)
                                {
                                    Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Boss {modelBoss.name} HP: {currentHpPercent:F1}% ({health.Value:F0}/{health.MaxHealth._Value:F0})");
                                    _lastHpLog[lastLogKey] = hpBracket;
                                }
                                
                                // Check mechanics based on HP threshold
                                BossMechanicSystem.CheckHpThresholdMechanics(modelBoss.bossEntity, modelBoss, currentHpPercent);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[BossGameplayEventSystem] Error processing stat change event: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Process attacker tracking from DamageTakenEvent (called by AttackerTrackingHook)
        /// </summary>
        public static void ProcessDamageTakenEvent(Entity target, Entity source, string npcAssetName)
        {
            try
            {
                var entityManager = Plugin.SystemsCore.EntityManager;
                
                // Get the actual attacker
                Entity attacker = Entity.Null;
                
                if (entityManager.HasComponent<EntityOwner>(source))
                {
                    var owner = entityManager.GetComponentData<EntityOwner>(source);
                    attacker = owner.Owner;
                    Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Found attacker from Source EntityOwner: {attacker.Index}");
                }
                else if (source != Entity.Null)
                {
                    attacker = source;
                    Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Using Source as attacker: {attacker.Index}");
                }
                
                // Find matching bosses
                var modelBosses = Database.BOSSES.Where(x => x.AssetName == npcAssetName && x.bossSpawn == true).ToList();
                
                foreach (var modelBoss in modelBosses)
                {
                    if (modelBoss != null && modelBoss.GetBossEntity() && modelBoss.bossEntity.Has<NameableInteractable>())
                    {
                        NameableInteractable _nameableInteractable = modelBoss.bossEntity.Read<NameableInteractable>();
                        
                        if (_nameableInteractable.Name.Value == (modelBoss.nameHash + "bb"))
                        {
                            Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Target is boss {modelBoss.name}");
                            
                            // Check if attacker is a player
                            if (attacker != Entity.Null && entityManager.HasComponent<PlayerCharacter>(attacker))
                            {
                                try
                                {
                                    var playerChar = entityManager.GetComponentData<PlayerCharacter>(attacker);
                                    var user = entityManager.GetComponentData<User>(playerChar.UserEntity);
                                    var playerName = user.CharacterName.ToString();
                                    
                                    modelBoss.AddKiller(playerName);
                                    Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Player {playerName} damaged boss {modelBoss.name}");
                                }
                                catch (Exception ex)
                                {
                                    Plugin.Logger.LogError($"[BossGameplayEventSystem] Error getting player info: {ex.Message}");
                                }
                            }
                            else if (!PluginConfig.MinionDamage.Value && attacker != Entity.Null)
                            {
                                Plugin.Logger.LogDebug($"[BossGameplayEventSystem] Skipping non-player damage (MinionDamage disabled)");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[BossGameplayEventSystem] Error processing damage taken event: {ex.Message}");
            }
        }


        /// <summary>
        /// Processes VBlood consumption events
        /// Called directly from BloodyCore's EventsHandlerSystem.OnDeathVBlood
        /// </summary>
        public static void OnVBloodConsumed(VBloodSystem __instance, NativeList<VBloodConsumed> deathEvents)
        {
            var entityManager = Plugin.SystemsCore.EntityManager;
            var prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;
            
            foreach (var event_vblood in deathEvents)
            {
                try
                {
                    if (!entityManager.HasComponent<PlayerCharacter>(event_vblood.Target))
                        continue;

                    var player = entityManager.GetComponentData<PlayerCharacter>(event_vblood.Target);
                    var user = entityManager.GetComponentData<User>(player.UserEntity);
                    var playerModel = Bloody.Core.GameData.v1.GameData.Users.GetUserByCharacterName(user.CharacterName.ToString());
                    var vblood = prefabCollectionSystem._PrefabDataLookup[event_vblood.Source].AssetName;
                    
                    Plugin.Logger.LogInfo($"[BossGameplayEventSystem] VBlood consumed: {vblood} by {user.CharacterName}");
                    
                    var modelBosses = Database.BOSSES.Where(x => x.AssetName == vblood.ToString() && x.bossSpawn == true).ToList();
                    foreach (var modelBoss in modelBosses)
                    {
                        if (modelBoss.vbloodFirstKill)
                        {
                            // First kill already processed
                            modelBoss.AddKiller(user.CharacterName.ToString());
                            modelBoss.BuffKillers();
                        } 
                        else
                        {
                            // Check if boss is actually dead
                            if (modelBoss.GetBossEntity())
                            {
                                var health = modelBoss.bossEntity.Read<Health>();
                                if (health.IsDead || health.Value == 0)
                                {
                                    Plugin.Logger.LogInfo($"[BossGameplayEventSystem] VBlood boss {modelBoss.name} killed for first time");
                                    
                                    modelBoss.vbloodFirstKill = true;
                                    modelBoss.AddKiller(user.CharacterName.ToString());
                                    modelBoss.BuffKillers();
                                    
                                    if (modelBoss.bossSpawn)
                                    {
                                        // Delay announcement to avoid overlap with death message
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
                catch (Exception ex)
                {
                    Plugin.Logger.LogError($"[BossGameplayEventSystem] Error processing VBlood consumption: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Processes NPC death events
        /// Called directly from BloodyCore's EventsHandlerSystem.OnDeath
        /// </summary>
        public static void OnDeathNpc(DeathEventListenerSystem sender, NativeArray<DeathEvent> deathEvents)
        {
            var entityManager = Plugin.SystemsCore.EntityManager;
            var prefabCollectionSystem = Plugin.SystemsCore.PrefabCollectionSystem;

            foreach (var deathEvent in deathEvents)
            {
                try
                {
                    if (!entityManager.HasComponent<PlayerCharacter>(deathEvent.Killer))
                        continue;

                    var npcGUID = deathEvent.Died.Read<PrefabGUID>();
                    var npc = prefabCollectionSystem._PrefabDataLookup[npcGUID].AssetName;
                    var player = entityManager.GetComponentData<PlayerCharacter>(deathEvent.Killer);
                    var user = entityManager.GetComponentData<User>(player.UserEntity);

                    Plugin.Logger.LogInfo($"[BossGameplayEventSystem] NPC death: {npc} killed by {user.CharacterName}");

                    var modelBosses = Database.BOSSES.Where(x => x.AssetName == npc.ToString() && x.bossSpawn == true).ToList();
                    foreach (var modelBoss in modelBosses)
                    {
                        try
                        {
                            modelBoss.GetBossEntity();
                            
                            // Skip VBlood units (handled by OnVBloodConsumed)
                            if (modelBoss.bossEntity.Has<VBloodUnit>())
                            {
                                continue;
                            }

                            var health = modelBoss.bossEntity.Read<Health>();
                            if (health.IsDead || health.Value == 0)
                            {
                                Plugin.Logger.LogInfo($"[BossGameplayEventSystem] Boss {modelBoss.name} killed by {user.CharacterName}");
                                
                                // Unregister from systems
                                UnregisterBoss(modelBoss.bossEntity);
                                BossTrackingSystem.UnregisterBoss(modelBoss.bossEntity);
                                
                                // Process kill
                                modelBoss.AddKiller(user.CharacterName.ToString());
                                modelBoss.BuffKillers();
                                modelBoss.SendAnnouncementMessage();
                                
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.Logger.LogError($"[BossGameplayEventSystem] Error processing boss death: {ex.Message}");
                            
                            // Handle entity not exist case
                            if (ex.Message.Contains("The entity does not exist"))
                            {
                                modelBoss.AddKiller(user.CharacterName.ToString());
                                modelBoss.BuffKillers();
                                modelBoss.SendAnnouncementMessage();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogError($"[BossGameplayEventSystem] Error processing death event: {ex.Message}");
                }
            }
        }
    }
}