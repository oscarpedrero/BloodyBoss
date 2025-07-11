using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using Bloody.Core;
using Unity.Collections;

namespace BloodyBoss.Utils
{
    /// <summary>
    /// Extension methods for Entity to simplify boss-related operations
    /// </summary>
    public static class BossEntityExtensions
    {
        /// <summary>
        /// Checks if an entity is a BloodyBoss (has "bb" suffix in name)
        /// </summary>
        public static bool IsBloodyBoss(this Entity entity)
        {
            if (!entity.Has<NameableInteractable>())
                return false;
                
            var nameable = entity.Read<NameableInteractable>();
            return nameable.Name.Value.Contains("bb");
        }
        
        /// <summary>
        /// Gets the BossEncounterModel associated with this entity
        /// Returns null if not found or not a BloodyBoss
        /// </summary>
        internal static BossEncounterModel GetBossModel(this Entity entity)
        {
            if (!entity.IsBloodyBoss())
                return null;
                
            var nameable = entity.Read<NameableInteractable>();
            var nameWithoutBB = nameable.Name.Value.Replace("bb", "");
            
            return Database.BOSSES.FirstOrDefault(x => x.nameHash == nameWithoutBB);
        }
        
        /// <summary>
        /// Checks if the boss entity is alive
        /// </summary>
        public static bool IsBossAlive(this Entity entity)
        {
            if (!entity.Has<Health>())
                return false;
                
            var health = entity.Read<Health>();
            return !health.IsDead && health.Value > 0;
        }
        
        /// <summary>
        /// Gets the current and maximum health of the boss
        /// Returns (0, 0) if entity doesn't have Health component
        /// </summary>
        public static (float current, float max) GetBossHealth(this Entity entity)
        {
            if (!entity.Has<Health>())
                return (0, 0);
                
            var health = entity.Read<Health>();
            return (health.Value, health.MaxHealth);
        }
        
        /// <summary>
        /// Gets the health percentage of the boss (0-100)
        /// </summary>
        public static float GetBossHealthPercentage(this Entity entity)
        {
            var (current, max) = entity.GetBossHealth();
            if (max <= 0) return 0;
            
            return (current / max) * 100f;
        }
        
        /// <summary>
        /// Checks if the boss has a specific component
        /// </summary>
        public static bool HasComponent<T>(this Entity entity) where T : struct
        {
            return entity.Has<T>();
        }
        
        /// <summary>
        /// Safely reads a component, returns default if not present
        /// </summary>
        public static T SafeRead<T>(this Entity entity, T defaultValue = default) where T : struct
        {
            return entity.Has<T>() ? entity.Read<T>() : defaultValue;
        }
        
        /// <summary>
        /// Gets the position of the boss entity
        /// </summary>
        public static Unity.Mathematics.float3 GetBossPosition(this Entity entity)
        {
            if (!entity.Has<LocalToWorld>())
                return Unity.Mathematics.float3.zero;
                
            return entity.Read<LocalToWorld>().Position;
        }
        
        /// <summary>
        /// Gets the level of the boss from UnitLevel component
        /// </summary>
        public static int GetBossLevel(this Entity entity)
        {
            if (!entity.Has<UnitLevel>())
                return 0;
                
            return entity.Read<UnitLevel>().Level;
        }
        
        /// <summary>
        /// Checks if this boss entity matches a specific PrefabGUID
        /// </summary>
        public static bool IsPrefabGUID(this Entity entity, int prefabGUID)
        {
            if (!entity.Has<PrefabGUID>())
                return false;
                
            return entity.Read<PrefabGUID>().GuidHash == prefabGUID;
        }
        
        /// <summary>
        /// Gets the team information of the boss
        /// </summary>
        public static (Team team, TeamReference teamRef) GetBossTeam(this Entity entity)
        {
            var team = entity.SafeRead<Team>();
            var teamRef = entity.SafeRead<TeamReference>();
            return (team, teamRef);
        }
        
        /// <summary>
        /// Gets all players within a specified radius of the boss
        /// </summary>
        public static List<Entity> GetPlayersInRadius(this Entity bossEntity, float radius)
        {
            var players = new List<Entity>();
            var bossPos = bossEntity.GetBossPosition();
            
            var userEntities = Plugin.SystemsCore.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>()).ToEntityArray(Allocator.Temp);
            
            foreach (var userEntity in userEntities)
            {
                if (!userEntity.Has<User>()) continue;
                
                var user = userEntity.Read<User>();
                if (!Plugin.SystemsCore.EntityManager.Exists(user.LocalCharacter._Entity)) continue;
                
                var playerEntity = user.LocalCharacter._Entity;
                var playerPos = playerEntity.GetBossPosition();
                var distance = Unity.Mathematics.math.distance(bossPos, playerPos);
                
                if (distance <= radius)
                {
                    players.Add(playerEntity);
                }
            }
            
            userEntities.Dispose();
            return players;
        }
    }
}