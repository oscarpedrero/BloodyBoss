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

namespace BloodyBoss.Systems.Mechanics
{
    public class CloneMechanic : IMechanic
    {
        public string Type => "clone";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var count = GetParameter<int>(parameters, "count", 2);
            var healthPercent = GetParameter<float>(parameters, "health_percent", 25f);
            var damagePercent = GetParameter<float>(parameters, "damage_percent", 50f);
            var duration = GetParameter<float>(parameters, "duration", 30f);
            var shareHealth = GetParameter<bool>(parameters, "share_health", false);
            var announcement = GetParameter<string>(parameters, "announcement", "👥 The boss splits into multiple forms!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            var bossPrefab = bossEntity.Read<PrefabGUID>();
            
            // Calculate clone positions
            var positions = CalculateClonePositions(bossPos, count);
            
            // Spawn clones
            for (int i = 0; i < count && i < positions.Count; i++)
            {
                SpawnClone(bossEntity, bossPrefab, positions[i], healthPercent, damagePercent, duration);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Clone mechanic executed: {count} clones with {healthPercent}% HP");
        }

        private List<float2> CalculateClonePositions(float3 center, int count)
        {
            var positions = new List<float2>();
            var radius = 5f;
            
            for (int i = 0; i < count; i++)
            {
                var angle = (2 * math.PI * i) / count;
                var x = center.x + radius * math.cos(angle);
                var z = center.z + radius * math.sin(angle);
                positions.Add(new float2(x, z));
            }
            
            return positions;
        }

        private void SpawnClone(Entity originalBoss, PrefabGUID bossPrefab, float2 position, float healthPercent, float damagePercent, float duration)
        {
            SpawnSystem.SpawnUnitWithCallback(originalBoss, bossPrefab, position, -1, (Entity cloneEntity) =>
            {
                // Configure clone
                if (cloneEntity.Has<Health>() && originalBoss.Has<Health>())
                {
                    var originalHealth = originalBoss.Read<Health>();
                    var cloneHealth = cloneEntity.Read<Health>();
                    cloneHealth.MaxHealth._Value = originalHealth.MaxHealth._Value * (healthPercent / 100f);
                    cloneHealth.Value = cloneHealth.MaxHealth._Value;
                    cloneEntity.Write(cloneHealth);
                }
                
                if (cloneEntity.Has<UnitStats>() && originalBoss.Has<UnitStats>())
                {
                    var originalStats = originalBoss.Read<UnitStats>();
                    var cloneStats = cloneEntity.Read<UnitStats>();
                    cloneStats.PhysicalPower._Value = originalStats.PhysicalPower._Value * (damagePercent / 100f);
                    cloneStats.SpellPower._Value = originalStats.SpellPower._Value * (damagePercent / 100f);
                    cloneEntity.Write(cloneStats);
                }
                
                // Mark as clone with special buff
                var cloneBuff = new PrefabGUID(-1464851863); // Illusion/clone visual
                BuffNPC(cloneEntity, cloneBuff);
                
                // Add lifetime if duration is set
                if (duration > 0 && cloneEntity.Has<LifeTime>())
                {
                    var lifeTime = cloneEntity.Read<LifeTime>();
                    lifeTime.Duration = duration;
                    cloneEntity.Write(lifeTime);
                }
                
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Clone spawned at {position} with {healthPercent}% HP");
            });
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

        public bool Validate(Dictionary<string, object> parameters)
        {
            var count = GetParameter<int>(parameters, "count", 2);
            return count > 0 && count <= 5;
        }

        public string GetDescription()
        {
            return "Creates copies of the boss";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<LocalToWorld>() && bossEntity.Has<PrefabGUID>() && bossEntity.Has<Health>();
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