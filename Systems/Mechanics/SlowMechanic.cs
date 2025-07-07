using System;
using System.Collections.Generic;
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
    public class SlowMechanic : IMechanic
    {
        public string Type => "slow";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var slowType = GetParameter<string>(parameters, "slow_type", "movement");
            var amount = GetParameter<float>(parameters, "amount", 50f);
            var duration = GetParameter<float>(parameters, "duration", 5f);
            var radius = GetParameter<float>(parameters, "radius", 15f);
            var announcement = GetParameter<string>(parameters, "announcement", "🐌 Time slows down!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            ApplySlowInArea(bossPos, radius, amount, duration, slowType);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Slow mechanic executed: {amount}% {slowType} slow for {duration}s");
        }

        private void ApplySlowInArea(float3 center, float radius, float amount, float duration, string slowType)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(center, pos) <= radiusSq)
                    {
                        ApplySlow(user.Character.Entity, amount, slowType);
                    }
                }
            }
        }

        private void ApplySlow(Entity target, float amount, string slowType)
        {
            PrefabGUID slowBuff;
            
            switch (slowType.ToLower())
            {
                case "movement":
                    slowBuff = new PrefabGUID(-1376368117); // Movement slow
                    break;
                case "attack":
                    slowBuff = new PrefabGUID(-30951541); // Attack speed slow
                    break;
                case "cast":
                    slowBuff = new PrefabGUID(1934754107); // Cast speed slow
                    break;
                default:
                    slowBuff = new PrefabGUID(-1376368117); // Default to movement slow
                    break;
            }

            BuffCharacter(target, slowBuff);
            Plugin.Logger.LogDebug($"Applied {amount}% {slowType} slow to entity");
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
            var amount = GetParameter<float>(parameters, "amount", 50f);
            var duration = GetParameter<float>(parameters, "duration", 5f);
            
            return amount > 0 && amount <= 100 && duration > 0 && duration <= 30;
        }

        public string GetDescription()
        {
            return "Slows movement, attack speed, or casting speed";
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