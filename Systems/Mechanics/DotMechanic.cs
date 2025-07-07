using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;

namespace BloodyBoss.Systems.Mechanics
{
    public class DotMechanic : IMechanic
    {
        public string Type => "dot";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var dotType = GetParameter<string>(parameters, "dot_type", "poison");
            var damagePerSecond = GetParameter<float>(parameters, "damage_per_second", 20f);
            var duration = GetParameter<float>(parameters, "duration", 10f);
            var target = GetParameter<string>(parameters, "target", "all");
            var radius = GetParameter<float>(parameters, "radius", 20f);
            var stackable = GetParameter<bool>(parameters, "stackable", false);
            var announcement = GetParameter<string>(parameters, "announcement", "☠️ Deadly affliction spreads!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            ApplyDotToTargets(bossPos, radius, dotType, damagePerSecond, duration, target);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"DoT mechanic executed: {dotType} dealing {damagePerSecond} DPS for {duration}s");
        }

        private void ApplyDotToTargets(float3 center, float radius, string dotType, float dps, float duration, string targetType)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online.ToList();
            
            if (targetType.ToLower() == "random")
            {
                var random = new System.Random();
                users = users.OrderBy(x => random.Next()).Take(5).ToList();
            }

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    
                    if (targetType.ToLower() == "all" || math.distancesq(center, pos) <= radiusSq)
                    {
                        ApplyDot(user.Character.Entity, dotType, dps, duration);
                    }
                }
            }
        }

        private void ApplyDot(Entity target, string dotType, float dps, float duration)
        {
            PrefabGUID dotBuff;
            
            switch (dotType.ToLower())
            {
                case "poison":
                    dotBuff = new PrefabGUID(1614409699); // Poison DoT
                    break;
                case "burn":
                    dotBuff = new PrefabGUID(-1968815368); // Ignite/Burn
                    break;
                case "bleed":
                    dotBuff = new PrefabGUID(411225544); // Bleed
                    break;
                case "corruption":
                    dotBuff = new PrefabGUID(-201240543); // Corruption
                    break;
                case "frost":
                    dotBuff = new PrefabGUID(-387588255); // Frost DoT
                    break;
                default:
                    dotBuff = new PrefabGUID(1614409699); // Default to poison
                    break;
            }

            BuffCharacter(target, dotBuff);
            Plugin.Logger.LogDebug($"Applied {dotType} DoT to entity: {dps} DPS for {duration}s");
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
            var dps = GetParameter<float>(parameters, "damage_per_second", 20f);
            var duration = GetParameter<float>(parameters, "duration", 10f);
            
            return dps > 0 && dps <= 500 && duration > 0 && duration <= 60;
        }

        public string GetDescription()
        {
            return "Applies damage over time effects";
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