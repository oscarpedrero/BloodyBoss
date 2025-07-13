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
using Bloody.Core.API.v1;

namespace BloodyBoss.Systems.Mechanics
{
    public class TrapMechanic : IMechanic
    {
        public string Type => "trap";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var trapType = GetParameter<string>(parameters, "trap_type", "spike");
            var pattern = GetParameter<string>(parameters, "pattern", "random");
            var count = GetParameter<int>(parameters, "count", 5);
            var damage = GetParameter<float>(parameters, "damage", 50f);
            var radius = GetParameter<float>(parameters, "radius", 30f);
            var triggerDelay = GetParameter<float>(parameters, "trigger_delay", 1.5f);
            var announcement = GetParameter<string>(parameters, "announcement", "Watch your step!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            SpawnTraps(bossPos, trapType, pattern, count, damage, radius, triggerDelay);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Trap mechanic executed: {count} {trapType} traps in {pattern} pattern");
        }

        private void SpawnTraps(float3 center, string trapType, string pattern, int count, float damage, float radius, float triggerDelay)
        {
            var positions = GenerateTrapPositions(center, pattern, count, radius);
            
            foreach (var pos in positions)
            {
                SpawnTrap(pos, trapType, damage, triggerDelay);
            }
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Spawned {positions.Count} traps");
        }

        private List<float3> GenerateTrapPositions(float3 center, string pattern, int count, float radius)
        {
            var positions = new List<float3>();
            var random = new System.Random();

            switch (pattern.ToLower())
            {
                case "circle":
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (float)(i * 2 * Math.PI / count);
                        var x = center.x + radius * math.cos(angle);
                        var z = center.z + radius * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;
                    
                case "spiral":
                    var spiralRadius = 5f;
                    var radiusIncrement = (radius - 5f) / count;
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (float)(i * 0.5f);
                        var r = spiralRadius + i * radiusIncrement;
                        var x = center.x + r * math.cos(angle);
                        var z = center.z + r * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;
                    
                case "grid":
                    var gridSize = (int)Math.Sqrt(count);
                    var spacing = radius * 2 / gridSize;
                    for (int i = 0; i < gridSize; i++)
                    {
                        for (int j = 0; j < gridSize; j++)
                        {
                            var x = center.x - radius + i * spacing;
                            var z = center.z - radius + j * spacing;
                            positions.Add(new float3(x, center.y, z));
                        }
                    }
                    break;
                    
                case "random":
                default:
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (float)(random.NextDouble() * 2 * Math.PI);
                        var r = (float)(random.NextDouble() * radius);
                        var x = center.x + r * math.cos(angle);
                        var z = center.z + r * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;
            }

            return positions;
        }

        private void SpawnTrap(float3 position, string trapType, float damage, float triggerDelay)
        {
            PrefabGUID trapPrefab;
            
            switch (trapType.ToLower())
            {
                case "spike":
                    trapPrefab = new PrefabGUID(1901522191); // Spike trap
                    break;
                    
                case "fire":
                    trapPrefab = new PrefabGUID(-1426222885); // Fire trap
                    break;
                    
                case "ice":
                    trapPrefab = new PrefabGUID(-355466479); // Ice trap
                    break;
                    
                case "poison":
                    trapPrefab = new PrefabGUID(652614258); // Poison trap
                    break;
                    
                case "explosive":
                    trapPrefab = new PrefabGUID(-1248239739); // Explosive trap
                    break;
                    
                default:
                    trapPrefab = new PrefabGUID(1901522191); // Default to spike
                    break;
            }

            // Spawn trap entity at position
            var spawnedEntity = Core.SystemsCore.EntityManager.CreateEntity();
            spawnedEntity.Add<LocalToWorld>();
            spawnedEntity.Write(new LocalToWorld { Value = float4x4.TRS(position, quaternion.identity, 1) });
            
            // Add trap components and visual indicator
            var trapIndicator = new PrefabGUID(405284549); // Warning circle
            var des = new ApplyBuffDebugEvent()
            {
                BuffPrefabGUID = trapIndicator,
            };

            var fromCharacter = new FromCharacter()
            {
                User = spawnedEntity,
                Character = spawnedEntity,
            };

            var debugSystem = Core.SystemsCore.DebugEventsSystem;
            debugSystem.ApplyBuff(fromCharacter, des);
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Spawned {trapType} trap at {position}");
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var count = GetParameter<int>(parameters, "count", 5);
            var damage = GetParameter<float>(parameters, "damage", 50f);
            var radius = GetParameter<float>(parameters, "radius", 30f);
            
            return count > 0 && count <= 50 && damage > 0 && damage <= 500 && radius > 0 && radius <= 100;
        }

        public string GetDescription()
        {
            return "Creates dangerous traps around the arena";
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