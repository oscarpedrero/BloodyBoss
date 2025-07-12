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
using BloodyBoss.Data;

namespace BloodyBoss.Systems.Mechanics
{
    public class WeakenMechanic : IMechanic
    {
        public string Type => "weaken";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var weakenType = GetParameter<string>(parameters, "weaken_type", "all");
            var amount = GetParameter<float>(parameters, "amount", 30f);
            var radius = GetParameter<float>(parameters, "radius", 25f);
            var duration = GetParameter<float>(parameters, "duration", 8f);
            var stackable = GetParameter<bool>(parameters, "stackable", false);
            var announcement = GetParameter<string>(parameters, "announcement", "💀 Your strength abandons you!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            ApplyWeakenEffect(bossPos, radius, weakenType, amount, duration, stackable);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Weaken mechanic executed: {weakenType} reduced by {amount}% for {duration}s");
        }

        private void ApplyWeakenEffect(float3 center, float radius, string weakenType, float amount, float duration, bool stackable)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;
            var affectedCount = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(center, pos) <= radiusSq)
                    {
                        ApplyWeakenDebuffs(user.Character.Entity, weakenType, amount, duration, stackable);
                        affectedCount++;
                    }
                }
            }
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Applied weaken effect to {affectedCount} players");
        }

        private void ApplyWeakenDebuffs(Entity target, string weakenType, float amount, float duration, bool stackable)
        {
            switch (weakenType.ToLower())
            {
                case "physical":
                    // Reduce physical power
                    BuffCharacter(target, PrefabConstants.PhysicalWeakness);
                    break;
                    
                case "spell":
                    // Reduce spell power
                    BuffCharacter(target, PrefabConstants.SpellWeakness);
                    break;
                    
                case "defense":
                    // Reduce armor/resistances
                    BuffCharacter(target, PrefabConstants.ArmorReduction);
                    break;
                    
                case "healing":
                    // Reduce healing received
                    BuffCharacter(target, PrefabConstants.HealingReduction);
                    break;
                    
                case "all":
                default:
                    // Apply all weakening effects
                    BuffCharacter(target, PrefabConstants.PhysicalWeakness);
                    BuffCharacter(target, PrefabConstants.SpellWeakness);
                    BuffCharacter(target, PrefabConstants.ArmorReduction);
                    break;
            }
            
            // Visual effect for weakening
            BuffCharacter(target, PrefabConstants.IllusionVisual);
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied {weakenType} weaken effect reducing by {amount}%");
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
            var amount = GetParameter<float>(parameters, "amount", 30f);
            var duration = GetParameter<float>(parameters, "duration", 8f);
            
            return amount > 0 && amount <= 90 && duration > 0 && duration <= 30;
        }

        public string GetDescription()
        {
            return "Weakens player stats and abilities";
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
    }
}