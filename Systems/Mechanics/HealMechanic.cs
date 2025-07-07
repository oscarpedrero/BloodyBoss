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
    public class HealMechanic : IMechanic
    {
        public string Type => "heal";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            var entityManager = world.EntityManager;
            
            // Get parameters
            var healAmountStr = GetParameter<string>(parameters, "heal_amount", "20%");
            var healType = GetParameter<string>(parameters, "heal_type", "instant");
            var duration = GetParameter<float>(parameters, "duration", 0f);
            var interruptible = GetParameter<bool>(parameters, "interruptible", true);
            var visualEffect = GetParameter<string>(parameters, "visual_effect", "heal_glow");
            var announcement = GetParameter<string>(parameters, "announcement", "💚 The boss begins to heal!");

            // Get current health
            var health = bossEntity.Read<Health>();
            var maxHealth = health.MaxHealth._Value;
            var currentHealth = health.Value;
            
            // Calculate heal amount
            float healAmount = 0;
            if (healAmountStr.EndsWith("%"))
            {
                var percentage = float.Parse(healAmountStr.TrimEnd('%')) / 100f;
                healAmount = maxHealth * percentage;
            }
            else
            {
                healAmount = float.Parse(healAmountStr);
            }

            // Apply heal based on type
            switch (healType.ToLower())
            {
                case "instant":
                    ApplyInstantHeal(bossEntity, healAmount, entityManager);
                    break;
                    
                case "channel":
                    ApplyChanneledHeal(bossEntity, healAmount, duration, interruptible, entityManager);
                    break;
                    
                case "overtime":
                    ApplyHealOverTime(bossEntity, healAmount, duration, entityManager);
                    break;
            }

            // Apply visual effect
            ApplyHealVisual(bossEntity, visualEffect, entityManager);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(entityManager, ref announcementRef);
            }
            
            Plugin.Logger.LogInfo($"Heal mechanic applied: Amount={healAmount}, Type={healType}, Duration={duration}s");
        }

        private void ApplyInstantHeal(Entity bossEntity, float healAmount, EntityManager entityManager)
        {
            if (bossEntity.Has<Health>())
            {
                var health = bossEntity.Read<Health>();
                health.Value = Math.Min(health.Value + healAmount, health.MaxHealth._Value);
                bossEntity.Write(health);
                
                Plugin.Logger.LogInfo($"Instant heal applied: {healAmount} HP");
            }
        }

        private void ApplyChanneledHeal(Entity bossEntity, float healAmount, float channelTime, bool interruptible, EntityManager entityManager)
        {
            // Apply channeling buff
            var channelBuff = new PrefabGUID(-106026683); // Channeling buff
            BuffNPC(bossEntity, channelBuff);
            
            if (!interruptible)
            {
                // Apply uninterruptible buff
                var uninterruptibleBuff = new PrefabGUID(476036897); // Cannot be interrupted
                BuffNPC(bossEntity, uninterruptibleBuff);
            }
            
            // TODO: Schedule heal after channel completes
            // For now, apply instant heal after simulating channel time
            Plugin.Logger.LogInfo($"Channeled heal started: {healAmount} HP over {channelTime}s");
        }

        private void ApplyHealOverTime(Entity bossEntity, float totalHeal, float duration, EntityManager entityManager)
        {
            // Apply HoT buff
            var hotBuff = new PrefabGUID(-831047476); // Heal over time buff
            BuffNPC(bossEntity, hotBuff);
            
            // Calculate heal per tick (assuming 1 tick per second)
            var healPerTick = totalHeal / duration;
            
            Plugin.Logger.LogInfo($"Heal over time applied: {healPerTick} HP/s for {duration}s");
        }

        private void ApplyHealVisual(Entity bossEntity, string effectName, EntityManager entityManager)
        {
            var visualBuff = effectName.ToLower() switch
            {
                "heal_glow" => new PrefabGUID(1974723356), // Green healing glow
                "holy_heal" => new PrefabGUID(-1204819086), // Holy light heal
                "blood_heal" => new PrefabGUID(803329823), // Blood magic heal
                "nature_heal" => new PrefabGUID(1269522096), // Nature heal effect
                _ => new PrefabGUID(1974723356) // Default heal glow
            };

            if (visualBuff != PrefabGUID.Empty)
            {
                BuffNPC(bossEntity, visualBuff);
            }
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
            var healType = GetParameter<string>(parameters, "heal_type", "instant");
            var validTypes = new[] { "instant", "channel", "overtime" };
            
            if (!Array.Exists(validTypes, t => t.Equals(healType, StringComparison.OrdinalIgnoreCase)))
                return false;
                
            var healAmountStr = GetParameter<string>(parameters, "heal_amount", "20%");
            
            // Validate heal amount format
            if (healAmountStr.EndsWith("%"))
            {
                if (!float.TryParse(healAmountStr.TrimEnd('%'), out var percentage))
                    return false;
                if (percentage <= 0 || percentage > 100)
                    return false;
            }
            else
            {
                if (!float.TryParse(healAmountStr, out var amount))
                    return false;
                if (amount <= 0)
                    return false;
            }
            
            return true;
        }

        public string GetDescription()
        {
            return "Heals the boss by a fixed amount or percentage of max health";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<Health>();
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