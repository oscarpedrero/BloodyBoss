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
    public class PullMechanic : IMechanic
    {
        public string Type => "pull";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var radius = GetParameter<float>(parameters, "radius", 30f);
            var force = GetParameter<float>(parameters, "force", 20f);
            var stun = GetParameter<bool>(parameters, "stun", true);
            var stunDuration = GetParameter<float>(parameters, "stun_duration", 1f);
            var announcement = GetParameter<string>(parameters, "announcement", "🌪️ Get over here!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            PullPlayersToPosition(bossPos, radius, force, stun, stunDuration);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Pull mechanic executed: Force {force} in {radius} radius");
        }

        private void PullPlayersToPosition(float3 targetPos, float radius, float force, bool stun, float stunDuration)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;
            var pulledCount = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>() && user.Character.Entity.Has<Translation>())
                {
                    var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distanceSq = math.distancesq(targetPos, playerPos);
                    
                    if (distanceSq <= radiusSq && distanceSq > 4f) // Don't pull if already very close
                    {
                        // Calculate pull direction
                        var direction = math.normalize(targetPos - playerPos);
                        var pullDistance = math.min(force, math.sqrt(distanceSq) - 2f);
                        var newPos = playerPos + direction * pullDistance;
                        
                        // Update position
                        var translation = user.Character.Entity.Read<Translation>();
                        translation.Value = newPos;
                        user.Character.Entity.Write(translation);
                        
                        // Apply pull visual effect
                        var pullBuff = new PrefabGUID(802825050); // Pull/knockback effect
                        BuffCharacter(user.Character.Entity, pullBuff);
                        
                        // Apply stun if requested
                        if (stun)
                        {
                            var stunBuff = new PrefabGUID(355774169); // Buff_General_Stun
                            BuffCharacter(user.Character.Entity, stunBuff);
                        }
                        
                        pulledCount++;
                    }
                }
            }
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Pulled {pulledCount} players");
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
            var radius = GetParameter<float>(parameters, "radius", 30f);
            var force = GetParameter<float>(parameters, "force", 20f);
            
            return radius > 0 && radius <= 100 && force > 0 && force <= 50;
        }

        public string GetDescription()
        {
            return "Pulls all players towards the boss";
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