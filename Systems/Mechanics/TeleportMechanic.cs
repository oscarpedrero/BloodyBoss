using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using Bloody.Core.GameData.v1;

namespace BloodyBoss.Systems.Mechanics
{
    public class TeleportMechanic : IMechanic
    {
        public string Type => "teleport";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            var entityManager = world.EntityManager;
            
            // Get parameters
            var teleportType = GetParameter<string>(parameters, "teleport_type", "random");
            var range = GetParameter<float>(parameters, "range", 30f);
            var afterEffect = GetParameter<string>(parameters, "after_effect", "");
            var announcement = GetParameter<string>(parameters, "announcement", "");

            // Get current position
            var currentPos = bossEntity.Read<LocalToWorld>().Position;
            float3 targetPosition = currentPos;

            // Calculate target position based on type
            switch (teleportType.ToLower())
            {
                case "random":
                    targetPosition = GetRandomPosition(currentPos, range);
                    break;
                    
                case "to_player":
                    targetPosition = GetNearestPlayerPosition(currentPos, range);
                    break;
                    
                case "to_center":
                    targetPosition = GetArenaCenter(bossEntity);
                    break;
                    
                case "fixed_positions":
                    // TODO: Implement fixed position teleports
                    targetPosition = GetRandomPosition(currentPos, range);
                    break;
            }

            // Perform teleport
            TeleportBoss(bossEntity, targetPosition, entityManager);

            // Apply after effects
            if (!string.IsNullOrEmpty(afterEffect))
            {
                ApplyAfterEffect(bossEntity, afterEffect, entityManager);
            }

            // Send announcement if provided
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(entityManager, ref announcementRef);
            }
            
            Plugin.Logger.LogInfo($"Teleport mechanic executed: Type={teleportType}, Range={range}");
        }

        private float3 GetRandomPosition(float3 center, float range)
        {
            var random = new System.Random();
            var angle = random.NextDouble() * 2 * Math.PI;
            var distance = range * 0.5f + (float)random.NextDouble() * range * 0.5f;
            
            var x = center.x + distance * (float)Math.Cos(angle);
            var z = center.z + distance * (float)Math.Sin(angle);
            
            return new float3(x, center.y, z);
        }

        private float3 GetNearestPlayerPosition(float3 bossPos, float range)
        {
            var nearestPlayer = Entity.Null;
            var nearestDistance = float.MaxValue;
            
            // Get online users
            var users = GameData.Users.Online.ToList();
            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distance = math.distance(bossPos, playerPos);
                    
                    if (distance < nearestDistance && distance <= range * 2)
                    {
                        nearestDistance = distance;
                        nearestPlayer = user.Character.Entity;
                    }
                }
            }
            
            if (nearestPlayer != Entity.Null)
            {
                var targetPos = nearestPlayer.Read<LocalToWorld>().Position;
                // Teleport near player, not on top
                return GetRandomPosition(targetPos, 5f);
            }
            
            // Fallback to random if no player found
            return GetRandomPosition(bossPos, range);
        }

        private float3 GetArenaCenter(Entity bossEntity)
        {
            // TODO: Implement arena center detection
            // For now, return boss spawn position if available
            return bossEntity.Read<LocalToWorld>().Position;
        }

        private void TeleportBoss(Entity bossEntity, float3 targetPosition, EntityManager entityManager)
        {
            // Apply teleport effect
            var teleportBuff = new PrefabGUID(-1122234739); // Teleport visual effect
            BuffNPC(bossEntity, teleportBuff);
            
            // Update position
            if (bossEntity.Has<Translation>())
            {
                var translation = bossEntity.Read<Translation>();
                translation.Value = targetPosition;
                bossEntity.Write(translation);
            }
            
            // Update last position
            if (bossEntity.Has<LastTranslation>())
            {
                var lastTranslation = bossEntity.Read<LastTranslation>();
                lastTranslation.Value = targetPosition;
                bossEntity.Write(lastTranslation);
            }
            
            // Clear velocity to prevent sliding
            if (bossEntity.Has<Velocity>())
            {
                bossEntity.Write(new Velocity { Value = float3.zero });
            }
        }

        private void ApplyAfterEffect(Entity bossEntity, string effect, EntityManager entityManager)
        {
            switch (effect.ToLower())
            {
                case "aoe_damage":
                    // Create AoE damage at teleport location
                    var aoeBuff = new PrefabGUID(1320298810); // AoE explosion
                    BuffNPC(bossEntity, aoeBuff);
                    break;
                    
                case "stun_nearby":
                    // Apply stun to nearby players
                    var stunBuff = new PrefabGUID(355774169); // Buff_General_Stun
                    ApplyAreaBuff(bossEntity, stunBuff, 10f);
                    break;
                    
                case "speed_boost":
                    // Apply speed boost after teleport
                    var speedBuff = new PrefabGUID(788443104); // Speed buff
                    BuffNPC(bossEntity, speedBuff);
                    break;
            }
        }

        private void ApplyAreaBuff(Entity centerEntity, PrefabGUID buffGuid, float radius)
        {
            var center = centerEntity.Read<LocalToWorld>().Position;
            var radiusSq = radius * radius;
            
            var users = GameData.Users.Online.ToList();
            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(center, pos) <= radiusSq)
                    {
                        BuffCharacter(user.Character.Entity, buffGuid);
                    }
                }
            }
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
            var teleportType = GetParameter<string>(parameters, "teleport_type", "random");
            var validTypes = new[] { "random", "to_player", "to_center", "fixed_positions" };
            
            if (!Array.Exists(validTypes, t => t.Equals(teleportType, StringComparison.OrdinalIgnoreCase)))
                return false;
                
            var range = GetParameter<float>(parameters, "range", 30f);
            if (range <= 0 || range > 200) // Max 200 unit teleport
                return false;
                
            return true;
        }

        public string GetDescription()
        {
            return "Teleports the boss to different locations during combat";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<LocalToWorld>() && bossEntity.Has<Translation>();
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