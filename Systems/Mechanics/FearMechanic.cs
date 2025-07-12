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
using BloodyBoss.Data;

namespace BloodyBoss.Systems.Mechanics
{
    public class FearMechanic : IMechanic
    {
        public string Type => "fear";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var target = GetParameter<string>(parameters, "target", "all");
            var duration = GetParameter<float>(parameters, "duration", 3f);
            var radius = GetParameter<float>(parameters, "radius", 20f);
            var maxTargets = GetParameter<int>(parameters, "max_targets", 5);
            var announcement = GetParameter<string>(parameters, "announcement", "😱 Terror strikes!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            var targets = GetTargetsInRadius(bossPos, radius, maxTargets, target);

            foreach (var targetEntity in targets)
            {
                ApplyFear(targetEntity, duration);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Fear mechanic executed: {targets.Count} targets feared for {duration}s");
        }

        private List<Entity> GetTargetsInRadius(float3 center, float radius, int maxTargets, string targetType)
        {
            var targets = new List<Entity>();
            var radiusSq = radius * radius;
            var users = GameData.Users.Online.ToList();

            if (targetType.ToLower() == "random")
            {
                var random = new System.Random();
                users = users.OrderBy(x => random.Next()).ToList();
            }

            foreach (var user in users)
            {
                if (targets.Count >= maxTargets)
                    break;

                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(center, pos) <= radiusSq)
                    {
                        targets.Add(user.Character.Entity);
                    }
                }
            }

            return targets;
        }

        private void ApplyFear(Entity target, float duration)
        {
            BuffCharacter(target, PrefabConstants.Fear);
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied fear to entity for {duration}s");
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
            var duration = GetParameter<float>(parameters, "duration", 3f);
            return duration > 0 && duration <= 10;
        }

        public string GetDescription()
        {
            return "Causes players to flee in terror";
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