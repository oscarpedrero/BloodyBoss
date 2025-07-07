using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.API.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using ProjectM.Shared;

namespace BloodyBoss.Systems.Mechanics
{
    public class AoeMechanic : IMechanic
    {
        public string Type => "aoe";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var aoeType = GetParameter<string>(parameters, "aoe_type", "explosion");
            var radius = GetParameter<float>(parameters, "radius", 10f);
            var damage = GetParameter<float>(parameters, "damage", 50f);
            var count = GetParameter<int>(parameters, "count", 1);
            var pattern = GetParameter<string>(parameters, "pattern", "center");
            var delay = GetParameter<float>(parameters, "delay", 1.5f);
            var announcement = GetParameter<string>(parameters, "announcement", "💥 Area attack incoming!");
            
            // Choose visual effect based on type
            int visualEffect = aoeType.ToLower() switch
            {
                "fire" => -2134900616, // AB_Shared_FireArea_VegetationSpread
                "frost" => -1766133599, // AB_Frost_IceBlockVortex_Buff_AreaDamageTrigger
                "holy" => 933561205, // AB_Militia_EyeOfGod_AreaInitBuff
                "blood" => 250152500, // AB_FeedDraculaBloodSoul_03_Complete_AreaDamage
                _ => 1688066724 // AB_ChurchOfLight_SlaveMaster_AoEBuff_Area (default)
            };

            var bossPos = bossEntity.Read<LocalToWorld>().Position;

            // Calculate AoE positions based on pattern
            var positions = CalculateAoePositions(bossPos, radius, count, pattern);

            foreach (var pos in positions)
            {
                // First spawn warning indicator
                SpawnWarningZone(pos, radius, delay);
                
                // Then spawn actual damage effect after delay
                var timer = delay;
                CoroutineHandler.StartFrameCoroutine(() =>
                {
                    SpawnAoeEffect(pos, radius, damage, visualEffect);
                }, (int)(delay * 60)); // Convert seconds to frames (assuming 60fps)
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"AoE mechanic executed: {count} {aoeType} areas with {damage} damage");
        }

        private List<float3> CalculateAoePositions(float3 center, float radius, int count, string pattern)
        {
            var positions = new List<float3>();

            switch (pattern.ToLower())
            {
                case "center":
                    positions.Add(center);
                    break;

                case "ring":
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (2 * math.PI * i) / count;
                        var x = center.x + radius * math.cos(angle);
                        var z = center.z + radius * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;

                case "random":
                    var random = new System.Random();
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (float)(random.NextDouble() * 2 * math.PI);
                        var dist = radius * (float)random.NextDouble();
                        var x = center.x + dist * math.cos(angle);
                        var z = center.z + dist * math.sin(angle);
                        positions.Add(new float3(x, center.y, z));
                    }
                    break;

                case "cross":
                    positions.Add(center);
                    positions.Add(new float3(center.x + radius, center.y, center.z));
                    positions.Add(new float3(center.x - radius, center.y, center.z));
                    positions.Add(new float3(center.x, center.y, center.z + radius));
                    positions.Add(new float3(center.x, center.y, center.z - radius));
                    break;
            }

            return positions;
        }

        private void SpawnWarningZone(float3 position, float radius, float duration)
        {
            try
            {
                // Use a red circle or warning effect
                var warningPrefab = new PrefabGUID(834863145); // AB_Illusion_Serpent_Area - red warning zone
                
                var entity = Core.SystemsCore.EntityManager.CreateEntity();
                
                Core.SystemsCore.EntityManager.AddComponentData(entity, new Translation { Value = position });
                Core.SystemsCore.EntityManager.AddComponentData(entity, new Rotation { Value = quaternion.identity });
                Core.SystemsCore.EntityManager.AddComponentData(entity, warningPrefab);
                
                // Set lifetime to match delay
                Core.SystemsCore.EntityManager.AddComponentData(entity, new LifeTime
                {
                    Duration = duration,
                    EndAction = LifeTimeEndAction.Destroy
                });
                
                // Scale to match radius (if possible)
                Core.SystemsCore.EntityManager.AddComponentData(entity, new NonUniformScale
                {
                    Value = new float3(radius / 5f, 1f, radius / 5f) // Adjust scale based on radius
                });
                
                Plugin.Logger.LogDebug($"Spawned warning zone at {position} for {duration}s");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to spawn warning zone: {ex.Message}");
            }
        }

        private void SpawnAoeEffect(float3 position, float radius, float damage, int effectPrefab)
        {
            try
            {
                // Get the AoE prefab
                var aoePrefab = new PrefabGUID(effectPrefab);
                
                // Create entity at position
                var entity = Core.SystemsCore.EntityManager.CreateEntity();
                
                // Set position
                Core.SystemsCore.EntityManager.AddComponentData(entity, new Translation { Value = position });
                Core.SystemsCore.EntityManager.AddComponentData(entity, new Rotation { Value = quaternion.identity });
                
                // Add the prefab
                Core.SystemsCore.EntityManager.AddComponentData(entity, aoePrefab);
                
                // Add lifetime so it disappears
                Core.SystemsCore.EntityManager.AddComponentData(entity, new LifeTime
                {
                    Duration = 3f,
                    EndAction = LifeTimeEndAction.Destroy
                });
                
                // Add damage component if needed
                if (damage > 0)
                {
                    // Apply damage to all players in radius
                    ApplyAreaDamage(position, radius, damage);
                }
                
                Plugin.Logger.LogDebug($"Spawned AoE effect at {position} with radius {radius} and damage {damage}");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to spawn AoE effect: {ex.Message}");
            }
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var radius = GetParameter<float>(parameters, "radius", 10f);
            if (radius <= 0 || radius > 100)
                return false;

            var count = GetParameter<int>(parameters, "count", 1);
            if (count <= 0 || count > 20)
                return false;

            return true;
        }

        public string GetDescription()
        {
            return "Creates area of effect damage zones";
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
        
        private void ApplyAreaDamage(float3 position, float radius, float damage)
        {
            try
            {
                var radiusSq = radius * radius;
                var users = Bloody.Core.GameData.v1.GameData.Users.Online;
                
                foreach (var user in users)
                {
                    if (user.Character.Entity.Has<LocalToWorld>() && user.Character.Entity.Has<Health>())
                    {
                        var playerPos = user.Character.Entity.Read<LocalToWorld>().Position;
                        var distanceSq = math.distancesq(position, playerPos);
                        
                        if (distanceSq <= radiusSq)
                        {
                            // Deal damage to player
                            var health = user.Character.Entity.Read<Health>();
                            health.Value -= damage;
                            
                            // Don't let health go below 0
                            if (health.Value < 0)
                                health.Value = 0;
                                
                            user.Character.Entity.Write(health);
                            
                            Plugin.Logger.LogDebug($"Dealt {damage} damage to player at distance {math.sqrt(distanceSq)}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Failed to apply area damage: {ex.Message}");
            }
        }
    }
}