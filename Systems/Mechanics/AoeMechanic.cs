using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.API.v1;
using Bloody.Core.GameData.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using ProjectM.Shared;
using Bloody.Core.Helper.v1;
using BloodyBoss.Data;
using ProjectM.Gameplay.Systems;

namespace BloodyBoss.Systems.Mechanics
{
    public class AoeMechanic : IMechanic
    {
        public string Type => "aoe";
        
        // Telegraph effects (warning zones) - Using effects we know work
        private readonly Dictionary<string, int> TelegraphEffects = new Dictionary<string, int>
        {
            { "fire", 1520432556 },     // AB_Militia_HoundMaster_QuickShot_Buff - Floating Eye (works!)
            { "frost", 1520432556 },    // Same for all - we'll use floating eye for warning
            { "blood", 1520432556 },    
            { "holy", 1520432556 },     
            { "shadow", 1520432556 },   
            { "poison", 1520432556 },   
            { "electric", 1520432556 }, 
            { "default", 1520432556 }   
        };
        
        // VBlood AoE abilities - Real cast animations from VBloods
        private readonly Dictionary<string, int> VBloodCastAbilities = new Dictionary<string, int>
        {
            { "fire", -984481115 },     // AB_Lucie_PlayerAbility_LiquidFirePotion_Drink_AbilityGroup
            { "frost", -743963442 },    // AB_Militia_Guard_VBlood_IceBreaker_AbilityGroup  
            { "blood", -1284243288 },   // AB_Blood_BloodStorm_AbilityGroup
            { "holy", -536713174 },     // AB_Militia_BishopOfDunley_HolyBeam_AggroBuff_Cast
            { "shadow", 461701172 },    // AB_Undead_BishopOfShadows_ShadowSoldier_AbilityGroup
            { "poison", 1728652937 },   // Use poison buff for now
            { "electric", -2078217582 }, // AB_Monster_LightningPillar_SpeedBuff_Cast
            { "default", -1284243288 }  // Default to blood storm
        };
        
        // Damage effects (actual AoE) - Using UNIQUE and SPECTACULAR effects!
        private readonly Dictionary<string, int> DamageEffects = new Dictionary<string, int>
        {
            { "fire", 1533067119 },     // Buff_General_Ignite - Standard fire ignite effect!
            { "frost", 899218383 },     // Buff_Wendigo_Freeze - Wendigo freeze effect!
            { "blood", 2085766220 },    // AB_Blood_BloodRage_Buff_MagicSource - Blood rage!
            { "holy", -1807398295 },    // AB_Militia_Nun_Penance - Holy yellow glow (already good)
            { "shadow", -855125670 },   // Buff_Dracula_ShadowBatSwarm_Effect - Shadow bats!
            { "poison", -1965215729 },  // Buff_General_Sludge_Poison - Poison area (ya funciona bien)
            { "electric", -2118254056 }, // AB_Storm_LightningRodBuff - Lightning shield (el que te gusta!)
            { "default", 1520432556 }   // Default to floating eye
        };
        

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var aoeType = GetParameter<string>(parameters, "aoe_type", "fire");
            var radius = GetParameter<float>(parameters, "radius", 10f);
            var damage = GetParameter<float>(parameters, "damage", 50f);
            var count = GetParameter<int>(parameters, "count", 1);
            var pattern = GetParameter<string>(parameters, "pattern", "boss");
            var delay = GetParameter<float>(parameters, "delay", 2f);
            var telegraphDuration = GetParameter<float>(parameters, "telegraph_duration", 1.8f);
            var persistDuration = GetParameter<float>(parameters, "persist_duration", 0f);
            var tickRate = GetParameter<float>(parameters, "tick_rate", 0.5f);
            var announcement = GetParameter<string>(parameters, "announcement", "💥 Area attack incoming!");
            var targetPlayers = GetParameter<bool>(parameters, "target_players", false);
            
            var bossPos = bossEntity.Read<LocalToWorld>().Position;

            // Calculate AoE positions based on pattern
            var positions = CalculateAoePositions(bossPos, radius, count, pattern, targetPlayers);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            // Spawn telegraph effects immediately using boss entity
            foreach (var pos in positions)
            {
                SpawnTelegraphEffect(bossEntity, pos, radius, telegraphDuration, aoeType);
            }

            // Apply a simple visual indicator to the boss during cast time
            try 
            {
                // Use the same effect as the damage phase but on the boss during casting
                var bossEffect = GetBossEffectForType(aoeType);
                BuffCharacter(bossEntity, new PrefabGUID(bossEffect), delay * 0.8f);
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Boss is casting {aoeType} AoE ability...");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to apply boss casting visual: {ex.Message}");
                // Continue execution - don't crash the game
            }
            
            // Schedule the actual damage after delay
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Scheduling {aoeType} AoE ability in {delay} seconds...");
            
            // Store positions and parameters to avoid world context issues
            var savedPositions = positions.ToList();
            var savedRadius = radius;
            var savedDamage = damage;
            var savedAoeType = aoeType;
            
            CoroutineHandler.StartGenericCoroutine(() =>
            {
                try
                {
                    Plugin.BLogger.Info(LogCategory.Mechanic, $"Executing {savedAoeType} AoE ability now!");
                    
                    // Apply visual burst effect to boss
                    if (bossEntity.Exists() && bossEntity.Has<Health>())
                    {
                        var bossEffect = GetBossEffectForType(savedAoeType);
                        BuffCharacter(bossEntity, new PrefabGUID(bossEffect), 1f);
                        Plugin.BLogger.Info(LogCategory.Mechanic, $"Applied {savedAoeType} visual effect to boss");
                    }
                    
                    // Apply damage safely
                    foreach (var pos in savedPositions)
                    {
                        ApplyAreaDamageWithEffects(pos, savedRadius, savedDamage, savedAoeType);
                    }
                }
                catch (Exception ex)
                {
                    Plugin.BLogger.Error(LogCategory.Mechanic, $"Error in AoE execution: {ex.Message}");
                }
            }, delay);

            Plugin.BLogger.Info(LogCategory.Mechanic, $"AoE mechanic prepared: {aoeType} attack, {damage} damage, delay: {delay}s");
        }

        private List<float3> CalculateAoePositions(float3 center, float radius, int count, string pattern, bool targetPlayers)
        {
            var positions = new List<float3>();

            switch (pattern.ToLower())
            {
                case "boss":
                case "center":
                    positions.Add(center);
                    break;

                case "ring":
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (2 * math.PI * i) / count;
                        var x = center.x + radius * 1.5f * math.cos(angle);
                        var z = center.z + radius * 1.5f * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;

                case "random":
                    var random = new System.Random();
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (float)(random.NextDouble() * 2 * math.PI);
                        var dist = radius * 0.5f + (radius * 1.5f * (float)random.NextDouble());
                        var x = center.x + dist * math.cos(angle);
                        var z = center.z + dist * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;

                case "cross":
                    positions.Add(center);
                    positions.Add(new float3(center.x + radius * 1.5f, center.y, center.z));
                    positions.Add(new float3(center.x - radius * 1.5f, center.y, center.z));
                    positions.Add(new float3(center.x, center.y, center.z + radius * 1.5f));
                    positions.Add(new float3(center.x, center.y, center.z - radius * 1.5f));
                    break;

                case "line":
                    for (int i = 0; i < count; i++)
                    {
                        var offset = (i - count / 2) * radius;
                        positions.Add(new float3(center.x + offset, center.y, center.z));
                    }
                    break;

                case "players":
                    var users = GameData.Users.Online.ToList();
                    var actualCount = Math.Min(count, users.Count);
                    
                    if (targetPlayers)
                    {
                        // Target specific player positions
                        for (int i = 0; i < actualCount && i < users.Count; i++)
                        {
                            if (users[i].Character.Entity.Has<LocalToWorld>())
                            {
                                var playerPos = users[i].Character.Entity.Read<LocalToWorld>().Position;
                                positions.Add(playerPos);
                            }
                        }
                    }
                    else
                    {
                        // Random positions near players
                        var shuffled = users.OrderBy(x => Guid.NewGuid()).Take(actualCount);
                        foreach (var user in shuffled)
                        {
                            if (user.Character.Entity.Has<LocalToWorld>())
                            {
                                var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                                var offset = UnityEngine.Random.insideUnitCircle * (radius * 0.5f);
                                positions.Add(new float3(playerPos.x + offset.x, playerPos.y, playerPos.z + offset.y));
                            }
                        }
                    }
                    break;

                case "spiral":
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (2 * math.PI * i) / count + (i * 0.5f);
                        var dist = radius * 0.5f + (radius * i / count);
                        var x = center.x + dist * math.cos(angle);
                        var z = center.z + dist * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;
            }

            return positions;
        }

        private void SpawnTelegraphEffect(Entity sourceEntity, float3 position, float radius, float duration, string aoeType)
        {
            try
            {
                // For now, just apply warning buff to players
                // Ground effects need proper entity spawning which is complex
                var radiusSq = radius * radius;
                var users = GameData.Users.Online.ToList();
                
                // Show warning message with location
                var warningMsg = GetWarningMessage(aoeType, radius);
                var warningRef = (FixedString512Bytes)warningMsg;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref warningRef);
                
                foreach (var user in users)
                {
                    if (!user.Character.Entity.Has<LocalToWorld>())
                        continue;
                        
                    var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distanceSq = math.distancesq(position, playerPos);
                    
                    if (distanceSq <= radiusSq)
                    {
                        // Apply ONLY warning visual (floating eye) during telegraph phase
                        var warningEffect = TelegraphEffects.ContainsKey(aoeType.ToLower()) 
                            ? TelegraphEffects[aoeType.ToLower()] 
                            : TelegraphEffects["default"];
                        BuffCharacter(user.Character.Entity, new PrefabGUID(warningEffect), duration);
                        
                        // REMOVED: Don't apply the damage effect during warning phase!
                        
                        Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied telegraph warning to player at distance {math.sqrt(distanceSq):F1}");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, $"Failed to apply telegraph effect: {ex.Message}");
            }
        }

        private void SpawnDamageEffect(Entity sourceEntity, float3 position, float radius, float damage, string aoeType, float persistDuration, float tickRate)
        {
            try
            {
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applying {aoeType} damage effect at {position}");
                
                // Apply initial damage and visual effects to affected players
                ApplyAreaDamageWithEffects(position, radius, damage, aoeType);
                
                // If persist duration > 0, apply damage over time
                if (persistDuration > 0 && tickRate > 0)
                {
                    var ticks = (int)(persistDuration / tickRate);
                    for (int i = 1; i <= ticks; i++)
                    {
                        CoroutineHandler.StartGenericCoroutine(() =>
                        {
                            ApplyAreaDamageWithEffects(position, radius, damage * 0.5f, aoeType); // Reduced damage for ticks
                        }, tickRate * i);
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, $"Failed to apply damage effect: {ex.Message}");
            }
        }
        
        private void ApplyAreaDamageWithEffects(float3 position, float radius, float damage, string aoeType)
        {
            try
            {
                var radiusSq = radius * radius;
                var users = GameData.Users.Online.ToList();
                var hitCount = 0;
                
                // Get appropriate visual effect for damage type
                var effectGuid = DamageEffects.ContainsKey(aoeType.ToLower()) 
                    ? DamageEffects[aoeType.ToLower()] 
                    : DamageEffects["default"];
                
                foreach (var user in users)
                {
                    if (!user.Character.Entity.Has<LocalToWorld>() || !user.Character.Entity.Has<Health>())
                        continue;
                        
                    var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distanceSq = math.distancesq(position, playerPos);
                    
                    if (distanceSq <= radiusSq)
                    {
                        // Apply damage with falloff based on distance
                        var distance = math.sqrt(distanceSq);
                        var falloff = 1f - (distance / radius) * 0.3f; // 30% damage reduction at edge
                        var finalDamage = damage * falloff;
                        
                        // Deal damage
                        DealDamage(user.Character.Entity, finalDamage, aoeType);
                        
                        // Apply visual effect buff (brief)
                        BuffCharacter(user.Character.Entity, new PrefabGUID(effectGuid), 1f);
                        
                        hitCount++;
                        Plugin.BLogger.Debug(LogCategory.Mechanic, $"Dealt {finalDamage:F1} {aoeType} damage to player at distance {distance:F1}");
                    }
                }
                
                if (hitCount > 0)
                {
                    Plugin.BLogger.Info(LogCategory.Mechanic, $"AoE hit {hitCount} players for {damage} damage");
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, $"Failed to apply area damage: {ex.Message}");
            }
        }
        
        private void ApplyAreaDamage(float3 position, float radius, float damage, string aoeType)
        {
            try
            {
                var radiusSq = radius * radius;
                var users = GameData.Users.Online.ToList();
                var hitCount = 0;
                
                foreach (var user in users)
                {
                    if (!user.Character.Entity.Has<LocalToWorld>() || !user.Character.Entity.Has<Health>())
                        continue;
                        
                    var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distanceSq = math.distancesq(position, playerPos);
                    
                    if (distanceSq <= radiusSq)
                    {
                        // Apply damage with falloff based on distance
                        var distance = math.sqrt(distanceSq);
                        var falloff = 1f - (distance / radius) * 0.3f; // 30% damage reduction at edge
                        var finalDamage = damage * falloff;
                        
                        // Deal damage
                        DealDamage(user.Character.Entity, finalDamage, aoeType);
                        hitCount++;
                        
                        Plugin.BLogger.Debug(LogCategory.Mechanic, $"Dealt {finalDamage:F1} {aoeType} damage to player at distance {distance:F1}");
                    }
                }
                
                if (hitCount > 0)
                {
                    Plugin.BLogger.Info(LogCategory.Mechanic, $"AoE hit {hitCount} players for {damage} damage");
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, $"Failed to apply area damage: {ex.Message}");
            }
        }
        
        private void DealDamage(Entity target, float damage, string damageType)
        {
            try
            {
                if (!target.Has<Health>())
                    return;
                    
                // Direct health modification for simplicity
                var health = target.Read<Health>();
                health.Value = math.max(0, health.Value - damage);
                target.Write(health);
                
                // Log damage type for debugging
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied {damage} {damageType} damage to entity");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, $"Failed to apply damage: {ex.Message}");
            }
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var radius = GetParameter<float>(parameters, "radius", 10f);
            if (radius <= 0 || radius > 50)
                return false;

            var count = GetParameter<int>(parameters, "count", 1);
            if (count <= 0 || count > 20)
                return false;

            var delay = GetParameter<float>(parameters, "delay", 2f);
            if (delay < 0.5f || delay > 10f)
                return false;

            return true;
        }

        public string GetDescription()
        {
            return "Creates telegraphed area damage zones with various patterns";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<LocalToWorld>();
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
        
        private int GetGroundEffectForType(string aoeType)
        {
            // Return a default effect for now
            return 1688066724; // AB_ChurchOfLight_SlaveMaster_AoEBuff_Area
        }
        
        private string GetWarningMessage(string aoeType, float radius)
        {
            var typeMessage = aoeType.ToLower() switch
            {
                "fire" => "🔥 Fire zone",
                "frost" => "❄️ Frost zone",
                "blood" => "🩸 Blood zone",
                "holy" => "✨ Holy zone",
                "shadow" => "👁️ Shadow zone",
                "poison" => "☠️ Poison zone",
                "electric" => "⚡ Electric zone",
                _ => "⚠️ Danger zone"
            };
            
            return $"{typeMessage} incoming! Radius: {radius:F0}m";
        }
        
        
        private void CreateVisualBurst(Entity bossEntity, float3 bossPos, string aoeType)
        {
            try
            {
                // Apply visual effect to boss first
                var bossEffect = GetBossEffectForType(aoeType);
                if (bossEntity.Exists() && bossEntity.Has<Health>())
                {
                    BuffCharacter(bossEntity, new PrefabGUID(bossEffect), 1f);
                    Plugin.BLogger.Info(LogCategory.Mechanic, $"Applied {aoeType} visual effect to boss");
                }
                
                // Skip VBlood cast animations for now - they cause world context issues
                // We'll implement them differently later
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.Mechanic, $"Failed to create visual burst: {ex.Message}");
            }
        }
        
        private int GetBossEffectForType(string aoeType)
        {
            return aoeType.ToLower() switch
            {
                "electric" => -2118254056, // Lightning shield - ¡Este es el que te gusta!
                "shadow" => -855125670,    // Buff_Dracula_ShadowBatSwarm_Effect - Shadow bats!
                "fire" => 1533067119,      // Buff_General_Ignite - Fire ignite!
                "frost" => 899218383,      // Buff_Wendigo_Freeze - Wendigo freeze!
                "blood" => 2085766220,     // AB_Blood_BloodRage_Buff_MagicSource - Blood rage! (temporal)
                "holy" => -1807398295,     // Holy yellow (ya es espectacular)
                "poison" => -1965215729,   // Buff_General_Sludge_Poison - Poison area
                _ => -2118254056           // Default to lightning shield
            };
        }
        
        private void SpawnProjectileEffect(Entity source, float3 from, float3 to, string aoeType)
        {
            try
            {
                // Use working buff effects that we know exist
                var effectGuid = aoeType.ToLower() switch
                {
                    "fire" => 1491093272,      // Red aura - works!
                    "frost" => -1807398295,    // Holy yellow - works!
                    "blood" => 2085766220,     // AB_Blood_BloodRage_Buff_MagicSource - Blood rage! - works!
                    "holy" => -1807398295,     // Holy yellow - works!
                    "shadow" => -2118254056,   // Lightning shield - works!
                    "poison" => 491510554,     // Shadow buff - try different one
                    "electric" => -2118254056, // Lightning shield - works!
                    _ => 1491093272            // Default red
                };
                
                // Apply the effect to the BOSS temporarily to show the attack origin
                BuffCharacter(source, new PrefabGUID(effectGuid), 0.5f);
                
                // Create a visual line/trail from boss to target position
                // For now, we'll spawn the effect at intervals between boss and target
                var direction = math.normalize(to - from);
                var distance = math.distance(from, to);
                var steps = 5; // Number of effects along the path
                
                for (int i = 0; i <= steps; i++)
                {
                    var t = i / (float)steps;
                    var pos = from + direction * (distance * t);
                    
                    // Spawn a temporary effect at this position
                    SpawnSystem.SpawnUnitWithCallback(source, PrefabConstants.FloatingEyeMark,
                        new float2(pos.x, pos.z), 0.3f + (i * 0.1f), (Entity effect) =>
                    {
                        Plugin.BLogger.Debug(LogCategory.Mechanic, $"Created {aoeType} trail effect");
                    });
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to spawn projectile: {ex.Message}");
            }
        }
        
    }
}