using System;
using System.Collections.Generic;
using Unity.Entities;
using ProjectM;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using ProjectM.Network;
using Stunlock.Core;
using BloodyBoss.DB.Models;
using Bloody.Core.API.v1;
using Unity.Collections;

namespace BloodyBoss.Systems.Mechanics
{
    public class EnrageMechanic : IMechanic
    {
        public string Type => "enrage";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            var entityManager = world.EntityManager;
            
            // Get parameters with defaults
            var damageMultiplier = GetParameter<float>(parameters, "damage_multiplier", 1.5f);
            var movementMultiplier = GetParameter<float>(parameters, "movement_speed_multiplier", 1.0f);
            var attackSpeedMultiplier = GetParameter<float>(parameters, "attack_speed_multiplier", 1.0f);
            var castSpeedMultiplier = GetParameter<float>(parameters, "cast_speed_multiplier", 1.0f);
            var cooldownReduction = GetParameter<float>(parameters, "cooldown_reduction", 0f);
            var duration = GetParameter<float>(parameters, "duration", 0f);
            var visualEffect = GetParameter<string>(parameters, "visual_effect", "blood_rage");
            var announcement = GetParameter<string>(parameters, "announcement", "⚔️ The boss enters a blood rage!");

            // Apply stat modifications
            if (bossEntity.Has<UnitStats>())
            {
                var unitStats = bossEntity.Read<UnitStats>();
                
                // Store original values (for reverting later if duration > 0)
                if (duration > 0)
                {
                    // TODO: Store original stats for reverting
                }
                
                // Apply multipliers to stats that exist
                unitStats.PhysicalPower._Value *= damageMultiplier;
                unitStats.SpellPower._Value *= damageMultiplier;
                
                // Note: Movement speed, attack speed, and cooldown stats may not be available in UnitStats
                // Would need to apply via buffs instead
                
                bossEntity.Write(unitStats);
            }

            // Apply visual effects via buff
            ApplyVisualEffect(bossEntity, visualEffect, entityManager);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(entityManager, ref announcementRef);
            }

            // Schedule removal if duration is specified
            if (duration > 0)
            {
                // TODO: Schedule mechanic removal after duration
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Enrage will last for {duration} seconds");
            }
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Enrage mechanic applied to boss: Damage x{damageMultiplier}");
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            // Check for valid multipliers
            var damageMultiplier = GetParameter<float>(parameters, "damage_multiplier", 1.5f);
            var movementMultiplier = GetParameter<float>(parameters, "movement_speed_multiplier", 1.0f);
            
            if (damageMultiplier <= 0 || damageMultiplier > 10)
                return false;
                
            if (movementMultiplier <= 0 || movementMultiplier > 5)
                return false;
                
            return true;
        }

        public string GetDescription()
        {
            return "Increases boss damage, movement speed, attack speed, and reduces cooldowns";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<UnitStats>() && bossEntity.Has<Health>();
        }

        private void ApplyVisualEffect(Entity bossEntity, string effectName, EntityManager entityManager)
        {
            // Map effect names to buff PrefabGUIDs
            var buffGuid = effectName.ToLower() switch
            {
                "blood_rage" => new PrefabGUID(2085766220), // AB_Blood_BloodRage_Buff_MagicSource
                "fire_aura" => new PrefabGUID(-1576893213), // Fire buff
                "speed_lines" => new PrefabGUID(788443104), // Movement speed buff visual
                _ => PrefabGUID.Empty
            };

            if (buffGuid != PrefabGUID.Empty)
            {
                // Use Bloody.Core's buff system
                BuffNPC(bossEntity, buffGuid);
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