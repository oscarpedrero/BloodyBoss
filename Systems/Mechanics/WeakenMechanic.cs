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

            Plugin.Logger.LogInfo($"Weaken mechanic executed: {weakenType} reduced by {amount}% for {duration}s");
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
            
            Plugin.Logger.LogInfo($"Applied weaken effect to {affectedCount} players");
        }

        private void ApplyWeakenDebuffs(Entity target, string weakenType, float amount, float duration, bool stackable)
        {
            switch (weakenType.ToLower())
            {
                case "physical":
                    // Reduce physical power
                    var physicalWeakenBuff = new PrefabGUID(-1055766866); // Physical weakness
                    BuffCharacter(target, physicalWeakenBuff);
                    break;
                    
                case "spell":
                    // Reduce spell power
                    var spellWeakenBuff = new PrefabGUID(801183323); // Spell weakness
                    BuffCharacter(target, spellWeakenBuff);
                    break;
                    
                case "defense":
                    // Reduce armor/resistances
                    var defenseWeakenBuff = new PrefabGUID(-1894153850); // Armor reduction
                    BuffCharacter(target, defenseWeakenBuff);
                    break;
                    
                case "healing":
                    // Reduce healing received
                    var healWeakenBuff = new PrefabGUID(1723455773); // Healing reduction
                    BuffCharacter(target, healWeakenBuff);
                    break;
                    
                case "all":
                default:
                    // Apply all weakening effects
                    BuffCharacter(target, new PrefabGUID(-1055766866)); // Physical weakness
                    BuffCharacter(target, new PrefabGUID(801183323)); // Spell weakness
                    BuffCharacter(target, new PrefabGUID(-1894153850)); // Armor reduction
                    break;
            }
            
            // Visual effect for weakening
            var visualBuff = new PrefabGUID(-1464851863); // Weakness visual
            BuffCharacter(target, visualBuff);
            
            Plugin.Logger.LogDebug($"Applied {weakenType} weaken effect reducing by {amount}%");
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