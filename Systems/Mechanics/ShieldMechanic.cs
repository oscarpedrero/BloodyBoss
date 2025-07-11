using System;
using System.Collections.Generic;
using Unity.Entities;
using ProjectM;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;

namespace BloodyBoss.Systems.Mechanics
{
    public class ShieldMechanic : IMechanic
    {
        public string Type => "shield";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            var entityManager = world.EntityManager;
            
            // Get parameters
            var shieldType = GetParameter<string>(parameters, "shield_type", "immune");
            var shieldAmount = GetParameter<float>(parameters, "shield_amount", 10000f);
            var duration = GetParameter<float>(parameters, "duration", 10f);
            var canMove = GetParameter<bool>(parameters, "can_move", false);
            var visualEffect = GetParameter<string>(parameters, "visual_effect", "holy_shield");
            var announcement = GetParameter<string>(parameters, "announcement", "🛡️ A divine shield protects the boss!");

            // Apply shield based on type
            switch (shieldType.ToLower())
            {
                case "immune":
                    ApplyImmunityShield(bossEntity, duration, entityManager);
                    break;
                    
                case "absorb":
                    ApplyAbsorbShield(bossEntity, shieldAmount, duration, entityManager);
                    break;
                    
                case "reflect":
                    ApplyReflectShield(bossEntity, duration, entityManager);
                    break;
            }

            // Apply movement restriction if needed
            if (!canMove)
            {
                // Apply root/stun buff
                var rootBuff = new PrefabGUID(1724281065); // Root buff
                BuffNPC(bossEntity, rootBuff);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(entityManager, ref announcementRef);
            }
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Shield mechanic applied: Type={shieldType}, Amount={shieldAmount}, Duration={duration}s");
        }

        private void ApplyImmunityShield(Entity bossEntity, float duration, EntityManager entityManager)
        {
            // Apply immunity buff
            var immunityBuff = new PrefabGUID(584227282); // General immunity buff
            BuffNPC(bossEntity, immunityBuff);
            
            // TODO: Schedule removal after duration
        }

        private void ApplyAbsorbShield(Entity bossEntity, float amount, float duration, EntityManager entityManager)
        {
            // Create absorb shield (similar to barrier)
            if (bossEntity.Has<AbsorbBuff>())
            {
                var absorbBuff = bossEntity.Read<AbsorbBuff>();
                absorbBuff.AbsorbValue = new ModifiableFloat { _Value = amount };
                bossEntity.Write(absorbBuff);
            }
            
            // Apply visual shield buff
            var shieldBuff = new PrefabGUID(-1970513771); // Shield visual
            BuffNPC(bossEntity, shieldBuff);
        }

        private void ApplyReflectShield(Entity bossEntity, float duration, EntityManager entityManager)
        {
            // Apply reflect damage buff
            var reflectBuff = new PrefabGUID(721788343); // Reflect buff
            BuffNPC(bossEntity, reflectBuff);
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
            var shieldType = GetParameter<string>(parameters, "shield_type", "immune");
            var validTypes = new[] { "immune", "absorb", "reflect" };
            
            if (!Array.Exists(validTypes, t => t.Equals(shieldType, StringComparison.OrdinalIgnoreCase)))
                return false;
                
            var duration = GetParameter<float>(parameters, "duration", 10f);
            if (duration <= 0 || duration > 300) // Max 5 minutes
                return false;
                
            return true;
        }

        public string GetDescription()
        {
            return "Applies various types of shields to protect the boss from damage";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<Health>() && bossEntity.Has<BuffBuffer>();
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