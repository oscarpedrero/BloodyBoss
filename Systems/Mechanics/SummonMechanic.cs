using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using Bloody.Core.API.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using ProjectM.Shared;

namespace BloodyBoss.Systems.Mechanics
{
    public class SummonMechanic : IMechanic
    {
        public string Type => "summon";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            var entityManager = world.EntityManager;
            
            // Get parameters
            var addPrefab = GetParameter<int>(parameters, "add_prefab", -1905691330); // Default: Alpha Wolf
            var count = GetParameter<int>(parameters, "count", 3);
            var pattern = GetParameter<string>(parameters, "pattern", "circle");
            var despawnOnBossDeath = GetParameter<bool>(parameters, "despawn_on_boss_death", true);
            var announcement = GetParameter<string>(parameters, "announcement", "⚔️ Minions answer the call!");

            // Get boss position
            var bossPosition = bossEntity.Read<LocalToWorld>().Position;
            
            // Calculate spawn positions based on pattern
            var spawnPositions = CalculateSpawnPositions(bossPosition, count, pattern);
            
            // Spawn adds
            var spawnedAdds = new List<Entity>();
            foreach (var position in spawnPositions)
            {
                SpawnSystem.SpawnUnitWithCallback(bossEntity, new PrefabGUID(addPrefab), position, -1, (Entity addEntity) =>
                {
                    // Set up the summoned add
                    SetupSummonedAdd(addEntity, bossEntity, despawnOnBossDeath);
                    spawnedAdds.Add(addEntity);
                    
                    Plugin.BLogger.Info(LogCategory.Mechanic, $"Spawned add at position {position}");
                });
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(entityManager, ref announcementRef);
            }
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Summon mechanic executed: Spawned {count} adds in {pattern} pattern");
        }

        private List<float2> CalculateSpawnPositions(float3 center, int count, string pattern)
        {
            var positions = new List<float2>();
            var baseRadius = 5f; // Base spawn radius
            
            switch (pattern.ToLower())
            {
                case "circle":
                    for (int i = 0; i < count; i++)
                    {
                        var angle = (2 * math.PI * i) / count;
                        var x = center.x + baseRadius * math.cos(angle);
                        var z = center.z + baseRadius * math.sin(angle);
                        positions.Add(new float2(x, z));
                    }
                    break;
                    
                case "line":
                    var spacing = 3f;
                    var startX = center.x - (count - 1) * spacing / 2;
                    for (int i = 0; i < count; i++)
                    {
                        var x = startX + i * spacing;
                        positions.Add(new float2(x, center.z + baseRadius));
                    }
                    break;
                    
                case "random":
                    var random = new System.Random();
                    for (int i = 0; i < count; i++)
                    {
                        var angle = random.NextDouble() * 2 * math.PI;
                        var radius = baseRadius + (float)random.NextDouble() * 5f;
                        var x = center.x + radius * math.cos((float)angle);
                        var z = center.z + radius * math.sin((float)angle);
                        positions.Add(new float2(x, z));
                    }
                    break;
                    
                default:
                    // Default to circle if pattern not recognized
                    return CalculateSpawnPositions(center, count, "circle");
            }
            
            return positions;
        }

        private void SetupSummonedAdd(Entity addEntity, Entity bossEntity, bool despawnOnBossDeath)
        {
            // Set the add's team to match the boss
            if (bossEntity.Has<Team>() && addEntity.Has<Team>())
            {
                var bossTeam = bossEntity.Read<Team>();
                addEntity.Write(bossTeam);
            }
            
            // Set faction alliance to match boss
            if (bossEntity.Has<FactionReference>() && addEntity.Has<FactionReference>())
            {
                var bossFaction = bossEntity.Read<FactionReference>();
                addEntity.Write(bossFaction);
            }
            
            // Apply aggro settings
            if (addEntity.Has<AggroConsumer>())
            {
                var aggro = addEntity.Read<AggroConsumer>();
                aggro.Active._Value = true;
                addEntity.Write(aggro);
            }
            
            // Mark as summoned by boss (for despawn on boss death)
            if (despawnOnBossDeath && addEntity.Has<EntityOwner>())
            {
                var owner = addEntity.Read<EntityOwner>();
                owner.Owner = bossEntity;
                addEntity.Write(owner);
            }
            
            // Disable VBlood consumption if this is a VBlood unit
            if (addEntity.Has<VBloodUnit>())
            {
                Plugin.BLogger.Info(LogCategory.Mechanic, "Summoned add is VBlood, disabling consumption");
                
                // Remove BloodConsumeSource to prevent feeding
                if (addEntity.Has<BloodConsumeSource>())
                {
                    addEntity.Remove<BloodConsumeSource>();
                    Plugin.BLogger.Info(LogCategory.Mechanic, "Removed BloodConsumeSource from summoned VBlood");
                }
                
                // Add VBloodConsumed component if not present
                if (!addEntity.Has<VBloodConsumed>())
                {
                    addEntity.Add<VBloodConsumed>();
                    Plugin.BLogger.Info(LogCategory.Mechanic, "Added VBloodConsumed to prevent V unlock");
                }
                
                // Try removing the Interactable component to prevent feeding
                try
                {
                    if (addEntity.Has<Interactable>())
                    {
                        addEntity.Remove<Interactable>();
                        Plugin.BLogger.Info(LogCategory.Mechanic, "Removed Interactable component from VBlood summon");
                    }
                    
                    // Also remove the NameableInteractable if present
                    if (addEntity.Has<NameableInteractable>())
                    {
                        addEntity.Remove<NameableInteractable>();
                        Plugin.BLogger.Info(LogCategory.Mechanic, "Removed NameableInteractable component from VBlood summon");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to modify VBlood interactions: {ex.Message}");
                }
                
                // Clear drop table so VBlood summon doesn't drop anything
                ClearSummonDropTable(addEntity);
                
                // Set a lifetime for the summon (they will despawn after this time)
                if (addEntity.Has<LifeTime>())
                {
                    var lifetime = addEntity.Read<LifeTime>();
                    lifetime.Duration = 300f; // 5 minutes
                    lifetime.EndAction = LifeTimeEndAction.Destroy;
                    addEntity.Write(lifetime);
                    Plugin.BLogger.Info(LogCategory.Mechanic, "Set VBlood summon lifetime to 5 minutes");
                }
            }
            
            // Apply any summon buffs/modifiers
            // Removed invalid buff that was causing errors
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
            var count = GetParameter<int>(parameters, "count", 3);
            if (count <= 0 || count > 20) // Max 20 summons
                return false;
                
            var pattern = GetParameter<string>(parameters, "pattern", "circle");
            var validPatterns = new[] { "circle", "line", "random" };
            
            if (!Array.Exists(validPatterns, p => p.Equals(pattern, StringComparison.OrdinalIgnoreCase)))
                return false;
                
            return true;
        }

        public string GetDescription()
        {
            return "Summons allied NPCs to assist the boss in combat";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<LocalToWorld>() && bossEntity.Has<Team>();
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
        
        private void ClearSummonDropTable(Entity entity)
        {
            try
            {
                var dropTableBuffer = entity.ReadBuffer<DropTableBuffer>();
                dropTableBuffer.Clear();
                Plugin.BLogger.Info(LogCategory.Mechanic, "Cleared drop table from VBlood summon");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Debug(LogCategory.Mechanic, $"Could not clear drop table from summon: {ex.Message}");
            }
        }
    }
}