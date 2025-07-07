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
    public class RootMechanic : IMechanic
    {
        public string Type => "root";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var target = GetParameter<string>(parameters, "target", "random");
            var duration = GetParameter<float>(parameters, "duration", 3f);
            var damagePerSecond = GetParameter<float>(parameters, "damage_per_second", 10f);
            var maxTargets = GetParameter<int>(parameters, "max_targets", 3);
            var visualEffect = GetParameter<string>(parameters, "visual_effect", "vines");
            var announcement = GetParameter<string>(parameters, "announcement", "🌿 Roots entangle the ground!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            var targets = SelectTargets(bossPos, target, maxTargets);

            foreach (var targetEntity in targets)
            {
                ApplyRoot(targetEntity, duration, damagePerSecond, visualEffect);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Root mechanic executed: {targets.Count} targets rooted for {duration}s");
        }

        private List<Entity> SelectTargets(float3 bossPos, string targetType, int maxTargets)
        {
            var targets = new List<Entity>();
            var users = GameData.Users.Online.ToList();

            switch (targetType.ToLower())
            {
                case "all":
                    targets.AddRange(users.Take(maxTargets).Select(u => u.Character.Entity));
                    break;
                    
                case "random":
                    var random = new System.Random();
                    var randomUsers = users.OrderBy(x => random.Next()).Take(maxTargets);
                    targets.AddRange(randomUsers.Select(u => u.Character.Entity));
                    break;
                    
                case "farthest":
                    var sortedByDistance = users
                        .Where(u => u.Character.Entity.Has<LocalToWorld>())
                        .OrderByDescending(u => 
                        {
                            var pos = u.Character.Entity.Read<LocalToWorld>().Position;
                            return math.distance(bossPos, pos);
                        })
                        .Take(maxTargets);
                    targets.AddRange(sortedByDistance.Select(u => u.Character.Entity));
                    break;
                    
                case "nearest":
                    var sortedByDistanceAsc = users
                        .Where(u => u.Character.Entity.Has<LocalToWorld>())
                        .OrderBy(u => 
                        {
                            var pos = u.Character.Entity.Read<LocalToWorld>().Position;
                            return math.distance(bossPos, pos);
                        })
                        .Take(maxTargets);
                    targets.AddRange(sortedByDistanceAsc.Select(u => u.Character.Entity));
                    break;
            }

            return targets;
        }

        private void ApplyRoot(Entity target, float duration, float dps, string visualEffect)
        {
            PrefabGUID rootBuff;
            
            switch (visualEffect.ToLower())
            {
                case "vines":
                    rootBuff = new PrefabGUID(1724281065); // Vine root
                    break;
                case "ice":
                    rootBuff = new PrefabGUID(-1055881716); // Frost root
                    break;
                case "chains":
                    rootBuff = new PrefabGUID(-1729014055); // Chain root
                    break;
                default:
                    rootBuff = new PrefabGUID(1724281065);
                    break;
            }

            BuffCharacter(target, rootBuff);
            
            // Apply DoT if damage is specified
            if (dps > 0)
            {
                var dotBuff = new PrefabGUID(-1894153998); // Generic DoT
                BuffCharacter(target, dotBuff);
            }
            
            Plugin.Logger.LogDebug($"Applied {visualEffect} root to entity for {duration}s");
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
            return "Immobilizes players in place";
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