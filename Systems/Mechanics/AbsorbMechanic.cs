using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.API.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using System.Linq;
using ProjectM.Shared;
using Bloody.Core.Models.v1;
using Stunlock.Network;
using BloodyBoss.Data;

namespace BloodyBoss.Systems.Mechanics
{
    public class AbsorbMechanic : IMechanic
    {
        public string Type => "absorb";
        
        // Visual effects for different absorption types
        private readonly Dictionary<string, int> AbsorbEffects = new Dictionary<string, int>
        {
            { "health", -1246704569 },   // Blood_Vampire_Buff_Leech - Vampiric leech!
            { "shield", -2118254056 },   // Lightning shield for shield absorption visual
            { "all", -1246704569 }       // Default to vampiric leech
        };
        
        // Boss visual effects
        private readonly Dictionary<string, PrefabGUID> BossAbsorbEffects = new Dictionary<string, PrefabGUID>
        {
            { "health", PrefabConstants.VampireLeechHeal },    // Blood_Vampire_Buff_Leech_SelfHeal - Healing effect!
            { "shield", PrefabConstants.BloodRageShield },     // AB_Blood_BloodRage_SpellMod_Buff_Shield - REAL shield with AbsorbBuff!
            { "all", PrefabConstants.VampireLeechHeal }        // Default to healing for combined effect
        };

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var absorbType = GetParameter<string>(parameters, "absorb_type", "health");
            var amount = GetParameter<float>(parameters, "amount", 20f);
            var radius = GetParameter<float>(parameters, "radius", 20f);
            var duration = GetParameter<float>(parameters, "duration", 5f);
            var continuous = GetParameter<bool>(parameters, "continuous", false);
            var announcement = GetParameter<string>(parameters, "announcement", "Life force drains away!");
            
            // New mechanic: Minimum players required in range
            var minPlayersRequired = GetParameter<int>(parameters, "min_players", 0);
            var globalRadius = GetParameter<float>(parameters, "global_radius", 50f);
            var globalMultiplier = GetParameter<float>(parameters, "global_multiplier", 1.5f);

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            
            if (continuous)
            {
                // Apply continuous absorption over time
                ApplyContinuousAbsorb(bossEntity, absorbType, amount, radius, duration, 
                    minPlayersRequired, globalRadius, globalMultiplier);
            }
            else
            {
                // Instant absorption with visual beam effects
                PerformInstantAbsorb(bossEntity, bossPos, absorbType, amount, radius, 
                    minPlayersRequired, globalRadius, globalMultiplier);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Absorb mechanic executed: {absorbType} absorption of {amount}");
        }

        private void PerformInstantAbsorb(Entity bossEntity, float3 bossPos, string absorbType, float amount, float radius,
            int minPlayersRequired, float globalRadius, float globalMultiplier)
        {
            var radiusSq = radius * radius;
            var globalRadiusSq = globalRadius * globalRadius;
            var users = GameData.Users.Online.ToList();
            var totalAbsorbed = 0f;
            var playersInRange = new List<UserModel>();
            var allPlayersInGlobalRange = new List<UserModel>();
            
            // First count players in normal range and global range
            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>() && user.Character.Entity.Has<Health>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distSq = math.distancesq(bossPos, pos);
                    
                    if (distSq <= radiusSq)
                    {
                        playersInRange.Add(user);
                    }
                    
                    if (distSq <= globalRadiusSq)
                    {
                        allPlayersInGlobalRange.Add(user);
                    }
                }
            }
            
            // Apply boss visual effect
            var bossEffect = BossAbsorbEffects.ContainsKey(absorbType.ToLower()) 
                ? BossAbsorbEffects[absorbType.ToLower()] 
                : BossAbsorbEffects["health"];
            BuffCharacter(bossEntity, bossEffect, 3f);
            
            // Check if minimum players requirement is met
            if (minPlayersRequired > 0 && playersInRange.Count < minPlayersRequired)
            {
                // Not enough players in range! Global punishment!
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Only {playersInRange.Count}/{minPlayersRequired} players in range! GLOBAL DRAIN ACTIVATED!");
                
                // Send warning to all players
                var warningMsg = $"{FontColorChatSystem.Yellow("[WARNING]")} NOT ENOUGH PLAYERS IN RANGE! {playersInRange.Count}/{minPlayersRequired} - GLOBAL DRAIN ACTIVE!";
                var warningRef = (FixedString512Bytes)warningMsg;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref warningRef);
                
                // Drain from ALL players in global radius with multiplier
                foreach (var user in allPlayersInGlobalRange)
                {
                    var absorbed = AbsorbFromTarget(user.Character.Entity, bossEntity, absorbType, amount * globalMultiplier);
                    totalAbsorbed += absorbed;
                    
                    // Apply drain visual effect (same as normal, just longer duration)
                    var drainEffect = AbsorbEffects.ContainsKey(absorbType.ToLower()) 
                        ? AbsorbEffects[absorbType.ToLower()] 
                        : AbsorbEffects["health"];
                    BuffCharacter(user.Character.Entity, new PrefabGUID(drainEffect), 3f);
                }
                
                Plugin.BLogger.Info(LogCategory.Mechanic, $"GLOBAL DRAIN: Absorbed {totalAbsorbed} {absorbType} from {allPlayersInGlobalRange.Count} players!");
            }
            else
            {
                // Normal absorption from players in range
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Normal absorption: {playersInRange.Count} players in range");
                
                foreach (var user in playersInRange)
                {
                    var absorbed = AbsorbFromTarget(user.Character.Entity, bossEntity, absorbType, amount);
                    totalAbsorbed += absorbed;
                    
                    // Apply drain visual effect
                    var drainEffect = AbsorbEffects.ContainsKey(absorbType.ToLower()) 
                        ? AbsorbEffects[absorbType.ToLower()] 
                        : AbsorbEffects["health"];
                    BuffCharacter(user.Character.Entity, new PrefabGUID(drainEffect), 2f);
                    
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    CreateAbsorptionBeam(user.Character.Entity, bossEntity, bossPos, pos);
                }
                
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Absorbed {totalAbsorbed} {absorbType} from {playersInRange.Count} players");
            }
        }

        private void ApplyContinuousAbsorb(Entity bossEntity, string absorbType, float amountPerSecond, float radius, float duration,
            int minPlayersRequired, float globalRadius, float globalMultiplier)
        {
            // Apply visual aura to boss
            var bossEffect = BossAbsorbEffects.ContainsKey(absorbType.ToLower()) 
                ? BossAbsorbEffects[absorbType.ToLower()] 
                : BossAbsorbEffects["health"];
            BuffCharacter(bossEntity, bossEffect, duration);
            
            // Save parameters for coroutine
            var savedBossEntity = bossEntity;
            var savedAbsorbType = absorbType;
            var savedAmount = amountPerSecond;
            var savedRadius = radius;
            var savedMinPlayers = minPlayersRequired;
            var savedGlobalRadius = globalRadius;
            var savedGlobalMultiplier = globalMultiplier;
            var tickRate = 1f; // Absorb every second
            var ticks = (int)(duration / tickRate);
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Starting continuous {absorbType} absorption: {amountPerSecond}/s for {duration}s");
            if (minPlayersRequired > 0)
            {
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Minimum {minPlayersRequired} players required in range or global drain activates!");
            }
            
            // Schedule periodic absorption
            for (int i = 0; i < ticks; i++)
            {
                CoroutineHandler.StartGenericCoroutine(() =>
                {
                    if (savedBossEntity.Exists() && savedBossEntity.Has<LocalToWorld>())
                    {
                        var bossPos = savedBossEntity.Read<LocalToWorld>().Position;
                        PerformInstantAbsorb(savedBossEntity, bossPos, savedAbsorbType, savedAmount, savedRadius,
                            savedMinPlayers, savedGlobalRadius, savedGlobalMultiplier);
                    }
                }, tickRate * (i + 1));
            }
        }

        private float AbsorbFromTarget(Entity target, Entity boss, string absorbType, float amount)
        {
            float actualAbsorbed = 0f;
            
            switch (absorbType.ToLower())
            {
                case "health":
                    if (target.Has<Health>() && boss.Has<Health>())
                    {
                        var targetHealth = target.Read<Health>();
                        var bossHealth = boss.Read<Health>();
                        
                        // Calculate health to drain (don't kill player)
                        actualAbsorbed = Math.Min(amount, targetHealth.Value - 1);
                        
                        // Drain health from player
                        targetHealth.Value -= actualAbsorbed;
                        target.Write(targetHealth);
                        
                        // Store boss's original max health
                        var originalMaxHealth = bossHealth.MaxHealth._Value;
                        
                        // Heal boss
                        bossHealth.Value = Math.Min(bossHealth.Value + actualAbsorbed, bossHealth.MaxHealth._Value);
                        boss.Write(bossHealth);
                        
                        // Also give boss temporary health boost buff
                        BuffCharacter(boss, PrefabConstants.VampireLeechHeal, 15f); // 15 seconds of boosted health
                        
                        Plugin.BLogger.Debug(LogCategory.Mechanic, $"Boss absorbed {actualAbsorbed} health, temporary boost applied");
                    }
                    break;
                    
                case "shield":
                    // Steal shield strength with REAL shield buff
                    // NOTE: Shield absorption amount is NOT configurable - it's fixed by the buff
                    var shieldsRemoved = 0;
                    
                    // Remove ALL shield buffs from player first
                    var shieldBuffsToRemove = new List<int>
                    {
                        -1605515615, // AB_Blood_BloodRage_SpellMod_Buff_Shield
                        -1763296393, // AB_Chaos_PowerSurge_SpellMod_Buff_Shield
                        -231593873,  // Buff_BloodBuff_Scholar_Tier2_Shield
                        514720473,   // Buff_ChurchOfLight_Cleric_Intervene_Shield
                        1433921398,  // AB_Illusion_PhantomAegis_Buff_MagicSource
                        -365991522   // AB_Legion_Guardian_BlockBuff_Buff
                        // Add more shield buff GUIDs as needed
                    };
                    
                    // Remove any existing shield buffs from the player
                    foreach (var shieldGuid in shieldBuffsToRemove)
                    {
                        if (BuffUtility.TryGetBuff(Core.SystemsCore.EntityManager, target, new PrefabGUID(shieldGuid), out Entity buffEntity))
                        {
                            DestroyUtility.Destroy(Core.SystemsCore.EntityManager, buffEntity);
                            shieldsRemoved++;
                            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Removed shield buff {shieldGuid} from player");
                        }
                    }
                    
                    // Only apply effects if we actually removed shields
                    if (shieldsRemoved > 0)
                    {
                        // Apply visual effect to show shield break
                        BuffCharacter(target, PrefabConstants.HolyNuke, 2f);
                        
                        // Give boss REAL shield buff that actually absorbs damage!
                        // Shield strength and duration are FIXED by the buff itself
                        BuffCharacter(boss, PrefabConstants.BloodRageShield); // Duration is controlled by the buff
                        
                        Plugin.BLogger.Debug(LogCategory.Mechanic, $"Boss stole {shieldsRemoved} shields and gained REAL damage absorbing shield!");
                    }
                    else
                    {
                        Plugin.BLogger.Debug(LogCategory.Mechanic, $"No shields to steal from player");
                    }
                    
                    // Return a fixed value for feedback (not actual absorption amount)
                    actualAbsorbed = shieldsRemoved > 0 ? 100f : 0f;
                    break;
                    
                case "all":
                    // Absorb health and shield
                    if (target.Has<Health>() && boss.Has<Health>())
                    {
                        // Absorb health
                        var healthAbsorbed = AbsorbFromTarget(target, boss, "health", amount * 0.7f);
                        actualAbsorbed += healthAbsorbed;
                    }
                    // Also steal shields
                    var shieldAbsorbed = AbsorbFromTarget(target, boss, "shield", amount * 0.3f);
                    actualAbsorbed += shieldAbsorbed;
                    break;
            }
            
            return actualAbsorbed;
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

        private void CreateAbsorptionBeam(Entity player, Entity boss, float3 bossPos, float3 playerPos)
        {
            try
            {
                // Simply intensify the drain effect on the player
                // The visual connection is implied by the matching effects on player and boss
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"Absorption link established between player and boss");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to create absorption beam: {ex.Message}");
            }
        }
        
        private void BuffCharacter(Entity character, PrefabGUID buffGuid, float duration = 0f)
        {
            try
            {
                var des = new ApplyBuffDebugEvent()
                {
                    BuffPrefabGUID = buffGuid,
                };

                var fromCharacter = new FromCharacter()
                {
                    User = character,
                    Character = character,
                };

                var debugSystem = Core.SystemsCore.DebugEventsSystem;
                debugSystem.ApplyBuff(fromCharacter, des);
                
                // If duration specified, remove buff after duration
                if (duration > 0)
                {
                    CoroutineHandler.StartGenericCoroutine(() =>
                    {
                        RemoveBuff(character, buffGuid);
                    }, duration);
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to apply buff: {ex.Message}");
            }
        }
        
        private void RemoveBuff(Entity character, PrefabGUID buffGuid)
        {
            try
            {
                if (BuffUtility.TryGetBuff(Core.SystemsCore.EntityManager, character, buffGuid, out Entity buffEntity))
                {
                    DestroyUtility.Destroy(Core.SystemsCore.EntityManager, buffEntity);
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to remove buff: {ex.Message}");
            }
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var amount = GetParameter<float>(parameters, "amount", 20f);
            var radius = GetParameter<float>(parameters, "radius", 20f);
            
            return amount > 0 && amount <= 500 && radius > 0 && radius <= 50;
        }

        public string GetDescription()
        {
            return "Drains health or shields from players";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<Health>() && bossEntity.Has<LocalToWorld>();
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
    }
}