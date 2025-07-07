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
    public class StunMechanic : IMechanic
    {
        public string Type => "stun";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var target = GetParameter<string>(parameters, "target", "nearest");
            var duration = GetParameter<float>(parameters, "duration", 2f);
            var radius = GetParameter<float>(parameters, "radius", 0f);
            var maxTargets = GetParameter<int>(parameters, "max_targets", 1);
            var announcement = GetParameter<string>(parameters, "announcement", "⚡ Stunning attack!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            var targets = GetTargets(bossPos, target, radius, maxTargets);

            foreach (var targetEntity in targets)
            {
                ApplyStun(targetEntity, duration);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Stun mechanic executed: {targets.Count} targets stunned for {duration}s");
        }

        private List<Entity> GetTargets(float3 bossPos, string targetType, float radius, int maxTargets)
        {
            var targets = new List<Entity>();
            var users = GameData.Users.Online.ToList();

            switch (targetType.ToLower())
            {
                case "all":
                    if (radius > 0)
                    {
                        var radiusSq = radius * radius;
                        foreach (var user in users)
                        {
                            if (user.Character.Entity.Has<LocalToWorld>())
                            {
                                var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                                if (math.distancesq(bossPos, pos) <= radiusSq)
                                {
                                    targets.Add(user.Character.Entity);
                                }
                            }
                        }
                    }
                    else
                    {
                        targets.AddRange(users.Select(u => u.Character.Entity));
                    }
                    break;

                case "nearest":
                    var nearestPlayer = GetNearestPlayer(bossPos, users);
                    if (nearestPlayer != Entity.Null)
                    {
                        targets.Add(nearestPlayer);
                    }
                    break;

                case "farthest":
                    var farthestPlayer = GetFarthestPlayer(bossPos, users);
                    if (farthestPlayer != Entity.Null)
                    {
                        targets.Add(farthestPlayer);
                    }
                    break;

                case "random":
                    var randomTargets = GetRandomPlayers(users, maxTargets);
                    targets.AddRange(randomTargets);
                    break;
            }

            // Limit to max targets
            if (targets.Count > maxTargets)
            {
                targets = targets.Take(maxTargets).ToList();
            }

            return targets;
        }

        private Entity GetNearestPlayer(float3 position, List<UserModel> users)
        {
            Entity nearest = Entity.Null;
            float nearestDist = float.MaxValue;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var dist = math.distance(position, pos);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = user.Character.Entity;
                    }
                }
            }

            return nearest;
        }

        private Entity GetFarthestPlayer(float3 position, List<UserModel> users)
        {
            Entity farthest = Entity.Null;
            float farthestDist = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var dist = math.distance(position, pos);
                    if (dist > farthestDist)
                    {
                        farthestDist = dist;
                        farthest = user.Character.Entity;
                    }
                }
            }

            return farthest;
        }

        private List<Entity> GetRandomPlayers(List<UserModel> users, int count)
        {
            var random = new System.Random();
            return users.OrderBy(x => random.Next())
                       .Take(count)
                       .Select(u => u.Character.Entity)
                       .ToList();
        }

        private void ApplyStun(Entity target, float duration)
        {
            var stunBuff = new PrefabGUID(355774169); // Buff_General_Stun
            BuffCharacter(target, stunBuff);
            
            Plugin.Logger.LogDebug($"Applied stun to entity for {duration}s");
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
            var duration = GetParameter<float>(parameters, "duration", 2f);
            if (duration <= 0 || duration > 10)
                return false;

            return true;
        }

        public string GetDescription()
        {
            return "Stuns players preventing all actions";
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