using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.API.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using ProjectM.Shared;
using ProjectM.Gameplay.Systems;
using Bloody.Core.Helper.v1;
using ProjectM.Gameplay.Scripting;

namespace BloodyBoss.Systems.Mechanics
{
    public class CloneMechanic : IMechanic
    {
        public string Type => "clone";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var count = GetParameter<int>(parameters, "count", 2);
            var healthPercent = GetParameter<float>(parameters, "health_percent", 25f);
            var damagePercent = GetParameter<float>(parameters, "damage_percent", 50f);
            var duration = GetParameter<float>(parameters, "duration", 30f);
            var shareHealth = GetParameter<bool>(parameters, "share_health", false);
            var announcement = GetParameter<string>(parameters, "announcement", "👥 The boss splits into multiple forms!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            var bossPrefab = bossEntity.Read<PrefabGUID>();
            
            // Calculate clone positions
            var positions = CalculateClonePositions(bossPos, count);
            
            // Spawn clones
            for (int i = 0; i < count && i < positions.Count; i++)
            {
                SpawnClone(bossEntity, bossPrefab, positions[i], healthPercent, damagePercent, duration);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Clone mechanic executed: {count} clones with {healthPercent}% HP");
        }

        private List<float2> CalculateClonePositions(float3 center, int count)
        {
            var positions = new List<float2>();
            var radius = 5f;
            
            for (int i = 0; i < count; i++)
            {
                var angle = (2 * math.PI * i) / count;
                var x = center.x + radius * math.cos(angle);
                var z = center.z + radius * math.sin(angle);
                positions.Add(new float2(x, z));
            }
            
            return positions;
        }

        private void SpawnClone(Entity originalBoss, PrefabGUID bossPrefab, float2 position, float healthPercent, float damagePercent, float duration)
        {
            SpawnSystem.SpawnUnitWithCallback(originalBoss, bossPrefab, position, -1, (Entity cloneEntity) =>
            {
                // Configure clone - Copy ALL stats from customized boss
                if (cloneEntity.Has<UnitStats>() && originalBoss.Has<UnitStats>())
                {
                    var originalStats = originalBoss.Read<UnitStats>();
                    var cloneStats = cloneEntity.Read<UnitStats>();
                    
                    // First copy ALL basic stats from the customized boss
                    cloneStats.PhysicalPower._Value = originalStats.PhysicalPower._Value;
                    cloneStats.SpellPower._Value = originalStats.SpellPower._Value;
                    cloneStats.ResourcePower._Value = originalStats.ResourcePower._Value;
                    cloneStats.SiegePower._Value = originalStats.SiegePower._Value;
                    cloneStats.PhysicalResistance._Value = originalStats.PhysicalResistance._Value;
                    cloneStats.SpellResistance._Value = originalStats.SpellResistance._Value;
                    cloneStats.FireResistance._Value = originalStats.FireResistance._Value;
                    cloneStats.PassiveHealthRegen._Value = originalStats.PassiveHealthRegen._Value;
                    cloneStats.CCReduction._Value = originalStats.CCReduction._Value;
                    cloneStats.HealthRecovery._Value = originalStats.HealthRecovery._Value;
                    cloneStats.DamageReduction._Value = originalStats.DamageReduction._Value;
                    cloneStats.HealingReceived._Value = originalStats.HealingReceived._Value;
                    
                    // Now apply the damage reduction percentage
                    cloneStats.PhysicalPower._Value *= (damagePercent / 100f);
                    cloneStats.SpellPower._Value *= (damagePercent / 100f);
                    
                    cloneEntity.Write(cloneStats);
                }
                
                if (cloneEntity.Has<Health>() && originalBoss.Has<Health>())
                {
                    var originalHealth = originalBoss.Read<Health>();
                    var cloneHealth = cloneEntity.Read<Health>();
                    
                    // Copy customized max health and then apply percentage
                    cloneHealth.MaxHealth._Value = originalHealth.MaxHealth.Value * (healthPercent / 100f);
                    cloneHealth.MaxRecoveryHealth = cloneHealth.MaxHealth.Value;
                    cloneHealth.Value = cloneHealth.MaxHealth.Value;
                    cloneEntity.Write(cloneHealth);
                }
                
                // Copy unit level if present
                if (cloneEntity.Has<UnitLevel>() && originalBoss.Has<UnitLevel>())
                {
                    var originalLevel = originalBoss.Read<UnitLevel>();
                    cloneEntity.Write(originalLevel);
                }
                
                // Copy abilities from customized boss
                if (cloneEntity.Has<AbilityBar_Shared>() && originalBoss.Has<AbilityBar_Shared>())
                {
                    var originalAbilities = originalBoss.Read<AbilityBar_Shared>();
                    cloneEntity.Write(originalAbilities);
                    Plugin.BLogger.Info(LogCategory.Mechanic, "Copied ability bar from customized boss to clone");
                }
                
                // Copy ability groups if present
                var entityManager = Core.SystemsCore.EntityManager;
                if (entityManager.HasBuffer<AbilityGroupSlotBuffer>(originalBoss) && 
                    entityManager.HasBuffer<AbilityGroupSlotBuffer>(cloneEntity))
                {
                    var originalAbilityGroups = entityManager.GetBuffer<AbilityGroupSlotBuffer>(originalBoss);
                    var cloneAbilityGroups = entityManager.GetBuffer<AbilityGroupSlotBuffer>(cloneEntity);
                    
                    cloneAbilityGroups.Clear();
                    foreach (var abilityGroup in originalAbilityGroups)
                    {
                        cloneAbilityGroups.Add(abilityGroup);
                    }
                    Plugin.BLogger.Info(LogCategory.Mechanic, $"Copied {originalAbilityGroups.Length} ability groups from customized boss");
                }
                
                // Copy movement and AI stats
                if (cloneEntity.Has<Movement>() && originalBoss.Has<Movement>())
                {
                    var originalMovement = originalBoss.Read<Movement>();
                    cloneEntity.Write(originalMovement);
                }
                
                if (cloneEntity.Has<Vision>() && originalBoss.Has<Vision>())
                {
                    var originalVision = originalBoss.Read<Vision>();
                    cloneEntity.Write(originalVision);
                }
                
                if (cloneEntity.Has<AggroConsumer>() && originalBoss.Has<AggroConsumer>())
                {
                    var originalAggro = originalBoss.Read<AggroConsumer>();
                    cloneEntity.Write(originalAggro);
                }
                
                // NO BUFFS FOR CLONES - Removed visual buff to prevent issues
                // var cloneBuff = new PrefabGUID(-1464851863); // Illusion/clone visual - COMMENTED: This buff doesn't exist
                // var cloneBuff = Prefabs.AB_Illusion_Curse_Debuff; // Purple illusion effect - REMOVED: Has Buff_Persists_Through_Death
                // BuffNPC(cloneEntity, cloneBuff); // DISABLED: No buffs for clones
                
                // Add lifetime if duration is set
                if (duration > 0 && cloneEntity.Has<LifeTime>())
                {
                    var lifeTime = cloneEntity.Read<LifeTime>();
                    lifeTime.Duration = duration;
                    cloneEntity.Write(lifeTime);
                }
                
                // Mark clone as owned by boss so it despawns when boss dies
                if (cloneEntity.Has<EntityOwner>())
                {
                    var owner = cloneEntity.Read<EntityOwner>();
                    owner.Owner = originalBoss;
                    cloneEntity.Write(owner);
                    Plugin.BLogger.Info(LogCategory.Mechanic, "Clone marked as owned by boss - will despawn on boss death");
                }
                
                // Disable VBlood consumption if this is a VBlood unit
                if (cloneEntity.Has<VBloodUnit>())
                {
                    Plugin.BLogger.Info(LogCategory.Mechanic, "Clone is VBlood, disabling consumption and drops");
                    
                    // Remove BloodConsumeSource to prevent feeding
                    if (cloneEntity.Has<BloodConsumeSource>())
                    {
                        cloneEntity.Remove<BloodConsumeSource>();
                        Plugin.BLogger.Info(LogCategory.Mechanic, "Removed BloodConsumeSource from clone");
                    }
                    
                    // Remove VBloodConsumeSource to prevent VBlood feeding
                    if (cloneEntity.Has<VBloodConsumeSource>())
                    {
                        cloneEntity.Remove<VBloodConsumeSource>();
                        Plugin.BLogger.Info(LogCategory.Mechanic, "Removed VBloodConsumeSource from clone");
                    }
                    
                    // VBloodConsumed doesn't work on clones - REMOVED
                    // if (!cloneEntity.Has<VBloodConsumed>())
                    // {
                    //     cloneEntity.Add<VBloodConsumed>();
                    //     Plugin.BLogger.Info(LogCategory.Mechanic, "Added VBloodConsumed to prevent V unlock from clone");
                    // }
                    
                    // Remove Interactable component to prevent any interaction
                    if (cloneEntity.Has<Interactable>())
                    {
                        cloneEntity.Remove<Interactable>();
                        Plugin.BLogger.Info(LogCategory.Mechanic, "Removed Interactable component from clone");
                    }
                    
                    // Clear drop table so clone doesn't drop anything
                    ClearCloneDropTable(cloneEntity);
                    
                    // Clear VBloodUnlockTechBuffer to prevent tech unlocks
                    if (entityManager.HasBuffer<VBloodUnlockTechBuffer>(cloneEntity))
                    {
                        var unlockBuffer = entityManager.GetBuffer<VBloodUnlockTechBuffer>(cloneEntity);
                        int techCount = unlockBuffer.Length;
                        
                        // Contents logged before clearing (if debug needed)
                        
                        // Remove entire buffer component instead of just clearing it
                        entityManager.RemoveComponent<VBloodUnlockTechBuffer>(cloneEntity);
                        Plugin.BLogger.Info(LogCategory.Mechanic, $"Removed entire VBloodUnlockTechBuffer component ({techCount} techs) from VBlood clone");
                    }
                    
                    // Also remove GiveProgressionOnConsume component which gives VBlood rewards
                    if (cloneEntity.Has<GiveProgressionOnConsume>())
                    {
                        entityManager.RemoveComponent<GiveProgressionOnConsume>(cloneEntity);
                        Plugin.BLogger.Info(LogCategory.Mechanic, "Removed GiveProgressionOnConsume component from VBlood clone");
                    }
                    
                    // Add BlockFeedBuff to prevent the clone from being consumed for VBlood
                    if (!cloneEntity.Has<BlockFeedBuff>())
                    {
                        cloneEntity.Add<BlockFeedBuff>();
                        Plugin.BLogger.Info(LogCategory.Mechanic, "Added BlockFeedBuff to prevent VBlood consumption from clone");
                    }
                }
                
                // Removed all Buff_Persists_Through_Death handling - not needed
                
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Clone spawned at {position} with {healthPercent}% HP");
            });
        }

        private void BuffNPC(Entity entity, PrefabGUID buffGuid)
        {
            var des = new ApplyBuffDebugEvent()
            {
                BuffPrefabGUID = buffGuid,
            };

            var fromCharacter = new FromCharacter()
            {
                User = entity,
                Character = entity,
            };

            var debugSystem = Core.SystemsCore.DebugEventsSystem;
            debugSystem.ApplyBuff(fromCharacter, des);
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var count = GetParameter<int>(parameters, "count", 2);
            return count > 0 && count <= 5;
        }

        public string GetDescription()
        {
            return "Creates copies of the boss";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<LocalToWorld>() && bossEntity.Has<PrefabGUID>() && bossEntity.Has<Health>();
        }

        private T GetParameter<T>(Dictionary<string, object> parameters, string key, T defaultValue)
        {
            if (parameters.TryGetValue(key, out var value))
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
        
        private void ClearCloneDropTable(Entity entity)
        {
            try
            {
                var dropTableBuffer = entity.ReadBuffer<DropTableBuffer>();
                dropTableBuffer.Clear();
                Plugin.BLogger.Info(LogCategory.Mechanic, "Cleared drop table from VBlood clone");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"Could not clear drop table from clone: {ex.Message}");
            }
        }
        
        
        private void LogCloneComponents(Entity clone)
        {
            try
            {
                var entityManager = Core.SystemsCore.EntityManager;
                
                Plugin.BLogger.Warning(LogCategory.Mechanic, "=== CLONE COMPONENTS DEBUG ===");
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Clone Entity: {clone.Index}:{clone.Version}");
                
                // Get all component types on the entity
                var componentTypes = entityManager.GetComponentTypes(clone, Unity.Collections.Allocator.Temp);
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"=== ALL COMPONENTS ({componentTypes.Length}) ===");
                
                foreach (var componentType in componentTypes)
                {
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"  Component: {componentType.GetManagedType().Name}");
                }
                componentTypes.Dispose();
                
                // List all buffs
                if (entityManager.HasBuffer<BuffBuffer>(clone))
                {
                    var buffBuffer = entityManager.GetBuffer<BuffBuffer>(clone);
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"=== CLONE BUFFS ({buffBuffer.Length}) ===");
                    
                    for (int i = 0; i < buffBuffer.Length; i++)
                    {
                        var buff = buffBuffer[i];
                        if (entityManager.Exists(buff.Entity))
                        {
                            var buffName = "Unknown";
                            var buffGuid = 0;
                            
                            if (buff.Entity.Has<PrefabGUID>())
                            {
                                var buffPrefab = buff.Entity.Read<PrefabGUID>();
                                buffGuid = buffPrefab.GuidHash;
                                
                                // Try to get buff name if it has Buff component
                                if (buff.Entity.Has<Buff>())
                                {
                                    var buffComp = buff.Entity.Read<Buff>();
                                    if (buffComp.Target == clone)
                                    {
                                        buffName = $"BuffEntity_{buff.Entity.Index}";
                                    }
                                }
                            }
                            
                            Plugin.BLogger.Warning(LogCategory.Mechanic, $"  Buff [{i}]: {buffGuid} - {buffName}");
                            
                            // Log buff components
                            var buffComponentTypes = entityManager.GetComponentTypes(buff.Entity, Unity.Collections.Allocator.Temp);
                            Plugin.BLogger.Warning(LogCategory.Mechanic, $"    Buff Components ({buffComponentTypes.Length}):");
                            foreach (var buffCompType in buffComponentTypes)
                            {
                                Plugin.BLogger.Warning(LogCategory.Mechanic, $"      - {buffCompType.GetManagedType().Name}");
                            }
                            buffComponentTypes.Dispose();
                        }
                    }
                }
                
                // Check specific components we care about
                Plugin.BLogger.Warning(LogCategory.Mechanic, "=== SPECIFIC COMPONENTS CHECK ===");
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"VBloodUnit: {(clone.Has<VBloodUnit>() ? "YES" : "NO")}");
                if (clone.Has<VBloodUnit>())
                {
                    var vblood = clone.Read<VBloodUnit>();
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"  - VBloodUnit type: {vblood.GetType().FullName}");
                    
                    // Use reflection to show all fields
                    var type = vblood.GetType();
                    var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"  - VBloodUnit FIELDS ({fields.Length}):");
                    foreach (var field in fields)
                    {
                        try
                        {
                            var value = field.GetValue(vblood);
                            Plugin.BLogger.Warning(LogCategory.Mechanic, $"    * {field.Name} ({field.FieldType.Name}): {value}");
                        }
                        catch (Exception ex)
                        {
                            Plugin.BLogger.Warning(LogCategory.Mechanic, $"    * {field.Name}: Error reading - {ex.Message}");
                        }
                    }
                    
                    // Also check properties
                    var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"  - VBloodUnit PROPERTIES ({properties.Length}):");
                    foreach (var prop in properties)
                    {
                        try
                        {
                            if (prop.CanRead)
                            {
                                var value = prop.GetValue(vblood);
                                Plugin.BLogger.Warning(LogCategory.Mechanic, $"    * {prop.Name} ({prop.PropertyType.Name}): {value}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Plugin.BLogger.Warning(LogCategory.Mechanic, $"    * {prop.Name}: Error reading - {ex.Message}");
                        }
                    }
                }
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"BloodConsumeSource: {(clone.Has<BloodConsumeSource>() ? "YES" : "NO")}");
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"VBloodConsumed: {(clone.Has<VBloodConsumed>() ? "YES" : "NO")}");
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Interactable: {(clone.Has<Interactable>() ? "YES" : "NO")}");
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"EntityOwner: {(clone.Has<EntityOwner>() ? "YES" : "NO")}");
                
                // Check VBloodUnlockTechBuffer
                if (entityManager.HasBuffer<VBloodUnlockTechBuffer>(clone))
                {
                    var unlockBuffer = entityManager.GetBuffer<VBloodUnlockTechBuffer>(clone);
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"VBloodUnlockTechBuffer: YES ({unlockBuffer.Length} techs) - SHOULD BE CLEARED!");
                    
                    // Log contents of the buffer
                    for (int i = 0; i < unlockBuffer.Length; i++)
                    {
                        var unlock = unlockBuffer[i];
                        Plugin.BLogger.Warning(LogCategory.Mechanic, $"  Tech [{i}]: {unlock.Guid.GuidHash}");
                    }
                }
                else
                {
                    Plugin.BLogger.Warning(LogCategory.Mechanic, "VBloodUnlockTechBuffer: NO (Component removed) ✅");
                }
                
                // Check GiveProgressionOnConsume component
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"GiveProgressionOnConsume: {(clone.Has<GiveProgressionOnConsume>() ? "YES - NEEDS TO BE REMOVED!" : "NO ✅")}");
                
                // Check BlockFeedBuff component (prevents VBlood consumption)
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"BlockFeedBuff: {(clone.Has<BlockFeedBuff>() ? "YES ✅ (Blocks VBlood feed)" : "NO")}");
                
                Plugin.BLogger.Warning(LogCategory.Mechanic, "=== END CLONE COMPONENTS DEBUG ===");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, $"Error logging clone components: {ex.Message}");
            }
        }
    }
}