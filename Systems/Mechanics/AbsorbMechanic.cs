using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;

namespace BloodyBoss.Systems.Mechanics
{
    public class AbsorbMechanic : IMechanic
    {
        public string Type => "absorb";

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
            var announcement = GetParameter<string>(parameters, "announcement", "💀 Life force drains away!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            
            if (continuous)
            {
                // Apply continuous absorption buff
                ApplyContinuousAbsorb(bossEntity, absorbType, amount, radius, duration);
            }
            else
            {
                // Instant absorption
                PerformInstantAbsorb(bossEntity, bossPos, absorbType, amount, radius);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Absorb mechanic executed: {absorbType} absorption of {amount}");
        }

        private void PerformInstantAbsorb(Entity bossEntity, float3 bossPos, string absorbType, float amount, float radius)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;
            var totalAbsorbed = 0f;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>() && user.Character.Entity.Has<Health>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(bossPos, pos) <= radiusSq)
                    {
                        var absorbed = AbsorbFromTarget(user.Character.Entity, bossEntity, absorbType, amount);
                        totalAbsorbed += absorbed;
                        
                        // Apply drain visual effect
                        var drainBuff = new PrefabGUID(-106502421); // Life drain effect
                        BuffCharacter(user.Character.Entity, drainBuff);
                    }
                }
            }
            
            Plugin.Logger.LogInfo($"Total absorbed: {totalAbsorbed} {absorbType}");
        }

        private void ApplyContinuousAbsorb(Entity bossEntity, string absorbType, float amountPerSecond, float radius, float duration)
        {
            // Apply absorption aura buff to boss
            var auraBuff = new PrefabGUID(1045578888); // Vampiric aura
            BuffNPC(bossEntity, auraBuff);
            
            // Visual effect for continuous drain
            var visualBuff = new PrefabGUID(-1576800157); // Dark aura effect
            BuffNPC(bossEntity, visualBuff);
            
            Plugin.Logger.LogDebug($"Applied continuous {absorbType} absorption: {amountPerSecond}/s for {duration}s");
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
                        
                        actualAbsorbed = Math.Min(amount, targetHealth.Value - 1); // Don't kill
                        targetHealth.Value -= actualAbsorbed;
                        target.Write(targetHealth);
                        
                        bossHealth.Value = Math.Min(bossHealth.Value + actualAbsorbed, bossHealth.MaxHealth._Value);
                        boss.Write(bossHealth);
                    }
                    break;
                    
                case "shield":
                    if (target.Has<AbsorbBuff>() && boss.Has<AbsorbBuff>())
                    {
                        var targetShield = target.Read<AbsorbBuff>();
                        var bossShield = boss.Read<AbsorbBuff>();
                        
                        actualAbsorbed = Math.Min(amount, targetShield.AbsorbValue);
                        targetShield.AbsorbValue -= actualAbsorbed;
                        target.Write(targetShield);
                        
                        bossShield.AbsorbValue += actualAbsorbed;
                        boss.Write(bossShield);
                    }
                    break;
                    
                case "energy":
                    // Apply energy drain debuff
                    var energyDrainBuff = new PrefabGUID(560523291); // Energy drain
                    BuffCharacter(target, energyDrainBuff);
                    
                    // Give boss energy buff
                    var energyBuff = new PrefabGUID(-901503997); // Energy boost
                    BuffNPC(boss, energyBuff);
                    
                    actualAbsorbed = amount;
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

        private void BuffCharacter(Entity character, PrefabGUID buffGuid)
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
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var amount = GetParameter<float>(parameters, "amount", 20f);
            var radius = GetParameter<float>(parameters, "radius", 20f);
            
            return amount > 0 && amount <= 500 && radius > 0 && radius <= 50;
        }

        public string GetDescription()
        {
            return "Drains health, shields, or energy from players";
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