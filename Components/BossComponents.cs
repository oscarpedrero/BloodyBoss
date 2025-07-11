using System;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Stunlock.Core;
using ProjectM;
using ProjectM.Network;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using BloodyBoss.Utils;
using BloodyBoss.DB;

namespace BloodyBoss.Components
{
    /// <summary>
    /// Component definitions and data structures for BloodyBoss system
    /// Note: We cannot add new components to entities, but we can define structures
    /// to organize our data and use with existing components
    /// </summary>
    public static class BossComponents
    {
        /// <summary>
        /// Identifies a BloodyBoss entity by checking NameableInteractable suffix
        /// </summary>
        public const string BOSS_SUFFIX = "ibb";
        public const string BOSS_SUFFIX_ALT = "bb";
        
        /// <summary>
        /// Data structure to hold boss state (stored externally, not as component)
        /// </summary>
        public struct BossState
        {
            public Entity Entity;
            public string BossName;
            public int NameHash;
            public DateTime SpawnTime;
            public bool IsPaused;
            public int ConsecutiveSpawns;
            public float DifficultyMultiplier;
            public int LastAnnouncedPhase;
            
            public bool IsValid => Entity != Entity.Null;
        }
        
        /// <summary>
        /// Boss phase information
        /// </summary>
        public struct BossPhase
        {
            public int PhaseNumber;
            public float HealthThreshold;
            public string AnnouncementText;
            public bool HasBeenAnnounced;
            
            public BossPhase(int number, float threshold, string text)
            {
                PhaseNumber = number;
                HealthThreshold = threshold;
                AnnouncementText = text;
                HasBeenAnnounced = false;
            }
        }
        
        /// <summary>
        /// Boss spawn configuration
        /// </summary>
        public struct BossSpawnConfig
        {
            public int PrefabGUID;
            public float3 Position;
            public int Level;
            public float HealthMultiplier;
            public float DamageMultiplier;
            public string SpawnTime;
            public int LifetimeSeconds;
        }
        
        /// <summary>
        /// Boss combat stats snapshot
        /// </summary>
        public struct BossCombatStats
        {
            public float CurrentHealth;
            public float MaxHealth;
            public float HealthPercentage;
            public float PhysicalPower;
            public float SpellPower;
            public float PhysicalResistance;
            public float SpellResistance;
            public int PlayersInCombat;
            public float CombatDuration;
            
            public static BossCombatStats FromEntity(Entity entity)
            {
                var stats = new BossCombatStats();
                
                if (entity.Has<Health>())
                {
                    var health = entity.Read<Health>();
                    stats.CurrentHealth = health.Value;
                    stats.MaxHealth = health.MaxHealth;
                    stats.HealthPercentage = health.MaxHealth > 0 ? (health.Value / health.MaxHealth) * 100f : 0;
                }
                
                if (entity.Has<UnitStats>())
                {
                    var unitStats = entity.Read<UnitStats>();
                    stats.PhysicalPower = unitStats.PhysicalPower.Value;
                    stats.SpellPower = unitStats.SpellPower.Value;
                    stats.PhysicalResistance = unitStats.PhysicalResistance.Value;
                    stats.SpellResistance = unitStats.SpellResistance.Value;
                }
                
                return stats;
            }
        }
        
        /// <summary>
        /// Component query helper definitions
        /// </summary>
        public static class Queries
        {
            // Query for finding all potential boss entities
            public static EntityQuery GetBossQuery(EntityManager entityManager)
            {
                return entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<NameableInteractable>(),
                    ComponentType.ReadOnly<UnitStats>(),
                    ComponentType.ReadOnly<Health>()
                );
            }
            
            // Query for combat-ready bosses
            public static EntityQuery GetCombatBossQuery(EntityManager entityManager)
            {
                return entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<NameableInteractable>(),
                    ComponentType.ReadOnly<UnitStats>(),
                    ComponentType.ReadOnly<Health>(),
                    ComponentType.ReadOnly<LocalToWorld>(),
                    ComponentType.ReadOnly<Team>()
                );
            }
            
            // Query for VBlood bosses specifically
            public static EntityQuery GetVBloodBossQuery(EntityManager entityManager)
            {
                return entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<NameableInteractable>(),
                    ComponentType.ReadOnly<VBloodUnit>(),
                    ComponentType.ReadOnly<Health>()
                );
            }
        }
        
        /// <summary>
        /// Helper to check if an entity is a BloodyBoss
        /// </summary>
        public static bool IsBloodyBoss(Entity entity, EntityManager entityManager)
        {
            if (!entityManager.HasComponent<NameableInteractable>(entity))
                return false;
                
            var nameable = entityManager.GetComponentData<NameableInteractable>(entity);
            var name = nameable.Name.Value;
            return name.EndsWith(BOSS_SUFFIX) || name.EndsWith(BOSS_SUFFIX_ALT);
        }
        
        /// <summary>
        /// Extract boss identifier from entity
        /// </summary>
        public static string GetBossIdentifier(Entity entity, EntityManager entityManager)
        {
            if (!entityManager.HasComponent<NameableInteractable>(entity))
                return null;
                
            var nameable = entityManager.GetComponentData<NameableInteractable>(entity);
            var fullName = nameable.Name.Value;
            
            if (fullName.EndsWith(BOSS_SUFFIX))
            {
                // Extract hash from name like "-1478835352ibb"
                var hash = fullName.Substring(0, fullName.Length - BOSS_SUFFIX.Length);
                // Find boss by hash
                var boss = Database.BOSSES.FirstOrDefault(b => b.nameHash == hash);
                return boss?.name;
            }
            else if (fullName.EndsWith(BOSS_SUFFIX_ALT))
            {
                var hash = fullName.Substring(0, fullName.Length - BOSS_SUFFIX_ALT.Length);
                var boss = Database.BOSSES.FirstOrDefault(b => b.nameHash == hash);
                return boss?.name;
            }
            
            return null;
        }
        
        /// <summary>
        /// Phase definitions based on health thresholds
        /// </summary>
        public static class Phases
        {
            public static readonly BossPhase[] StandardPhases = new[]
            {
                new BossPhase(1, 100f, "⚔️ Boss encounter started!"),
                new BossPhase(2, 75f, "📢 Boss enters Phase 2 - Increased aggression!"),
                new BossPhase(3, 50f, "⚡ Boss enters Phase 3 - Unleashing full power!"),
                new BossPhase(4, 25f, "🔥 Boss enters Final Phase - Desperate measures!"),
                new BossPhase(5, 10f, "💀 Boss is nearly defeated - Stay focused!")
            };
            
            public static BossPhase GetCurrentPhase(float healthPercentage)
            {
                for (int i = StandardPhases.Length - 1; i >= 0; i--)
                {
                    if (healthPercentage <= StandardPhases[i].HealthThreshold)
                    {
                        return StandardPhases[i];
                    }
                }
                return StandardPhases[0];
            }
        }
    }
}