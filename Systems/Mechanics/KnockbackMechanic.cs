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
    public class KnockbackMechanic : IMechanic
    {
        public string Type => "knockback";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var radius = GetParameter<float>(parameters, "radius", 15f);
            var force = GetParameter<float>(parameters, "force", 25f);
            var upwardForce = GetParameter<float>(parameters, "upward_force", 5f);
            var damage = GetParameter<float>(parameters, "damage", 0f);
            var announcement = GetParameter<string>(parameters, "announcement", "💥 Explosive force!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            KnockbackPlayersFromPosition(bossPos, radius, force, upwardForce, damage);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Knockback mechanic executed: Force {force} in {radius} radius");
        }

        private void KnockbackPlayersFromPosition(float3 center, float radius, float force, float upwardForce, float damage)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;
            var knockedCount = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>() && user.Character.Entity.Has<Translation>())
                {
                    var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distanceSq = math.distancesq(center, playerPos);
                    
                    if (distanceSq <= radiusSq && distanceSq > 0.1f)
                    {
                        // Calculate knockback direction
                        var distance = math.sqrt(distanceSq);
                        var direction = math.normalize(playerPos - center);
                        
                        // Scale force based on distance (closer = stronger)
                        var scaledForce = force * (1f - (distance / radius));
                        
                        // Calculate new position
                        var horizontalPush = direction * scaledForce;
                        var newPos = playerPos + horizontalPush;
                        newPos.y += upwardForce; // Add upward component
                        
                        // Update position
                        var translation = user.Character.Entity.Read<Translation>();
                        translation.Value = newPos;
                        user.Character.Entity.Write(translation);
                        
                        // Apply knockback visual effect
                        var knockbackBuff = new PrefabGUID(802825050); // Knockback effect
                        BuffCharacter(user.Character.Entity, knockbackBuff);
                        
                        // Apply damage if specified
                        if (damage > 0 && user.Character.Entity.Has<Health>())
                        {
                            var health = user.Character.Entity.Read<Health>();
                            health.Value = Math.Max(0, health.Value - damage);
                            user.Character.Entity.Write(health);
                        }
                        
                        knockedCount++;
                    }
                }
            }
            
            Plugin.Logger.LogInfo($"Knocked back {knockedCount} players");
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
            var radius = GetParameter<float>(parameters, "radius", 15f);
            var force = GetParameter<float>(parameters, "force", 25f);
            
            return radius > 0 && radius <= 50 && force > 0 && force <= 100;
        }

        public string GetDescription()
        {
            return "Pushes players away from the boss";
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