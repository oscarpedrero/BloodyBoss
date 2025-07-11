using System;
using System.Collections.Generic;
using Unity.Entities;
using ProjectM;
using Bloody.Core;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;

namespace BloodyBoss.Systems.Mechanics
{
    public class ReflectMechanic : IMechanic
    {
        public string Type => "reflect";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var reflectType = GetParameter<string>(parameters, "reflect_type", "all");
            var reflectPercent = GetParameter<float>(parameters, "reflect_percent", 100f);
            var duration = GetParameter<float>(parameters, "duration", 8f);
            var maxReflects = GetParameter<int>(parameters, "max_reflects", 0);
            var visualEffect = GetParameter<string>(parameters, "visual_effect", "mirror_shield");
            var announcement = GetParameter<string>(parameters, "announcement", "🪞 Attacks will be reflected!");

            // Apply reflect based on type
            ApplyReflect(bossEntity, reflectType, reflectPercent, duration, maxReflects, visualEffect);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Reflect mechanic executed: {reflectPercent}% {reflectType} damage reflection for {duration}s");
        }

        private void ApplyReflect(Entity bossEntity, string reflectType, float percent, float duration, int maxReflects, string visualEffect)
        {
            PrefabGUID reflectBuff;
            
            switch (reflectType.ToLower())
            {
                case "physical":
                    reflectBuff = new PrefabGUID(961570710); // Physical reflect
                    break;
                case "spell":
                    reflectBuff = new PrefabGUID(-1919691815); // Spell reflect
                    break;
                case "projectile":
                    reflectBuff = new PrefabGUID(1045091043); // Projectile reflect
                    break;
                case "all":
                default:
                    reflectBuff = new PrefabGUID(721788343); // All damage reflect
                    break;
            }

            // Apply main reflect buff
            BuffNPC(bossEntity, reflectBuff);
            
            // Apply visual effect
            ApplyVisualEffect(bossEntity, visualEffect);
            
            // If percent is less than 100, apply a damage reduction buff to simulate partial reflection
            if (percent < 100)
            {
                var damageReduction = new PrefabGUID(697337290); // Damage reduction
                BuffNPC(bossEntity, damageReduction);
            }
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied {reflectType} reflect at {percent}% for {duration}s");
        }

        private void ApplyVisualEffect(Entity bossEntity, string effectName)
        {
            PrefabGUID visualBuff;
            
            switch (effectName.ToLower())
            {
                case "mirror_shield":
                    visualBuff = new PrefabGUID(-221876810); // Mirror/glass effect
                    break;
                case "holy_barrier":
                    visualBuff = new PrefabGUID(584227282); // Holy barrier
                    break;
                case "arcane_shield":
                    visualBuff = new PrefabGUID(-1970513771); // Arcane shield
                    break;
                default:
                    visualBuff = new PrefabGUID(-221876810);
                    break;
            }

            BuffNPC(bossEntity, visualBuff);
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
            var percent = GetParameter<float>(parameters, "reflect_percent", 100f);
            var duration = GetParameter<float>(parameters, "duration", 8f);
            
            return percent > 0 && percent <= 200 && duration > 0 && duration <= 30;
        }

        public string GetDescription()
        {
            return "Reflects damage back to attackers";
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