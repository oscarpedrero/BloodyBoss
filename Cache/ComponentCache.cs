using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using ProjectM;
using ProjectM.Network;
using Stunlock.Core;
using Bloody.Core;

namespace BloodyBoss.Cache
{
    /// <summary>
    /// Caches frequently used component queries to improve performance
    /// </summary>
    internal static class ComponentCache
    {
        // Cache update interval in seconds
        private const float CACHE_UPDATE_INTERVAL = 1.0f;
        
        // Cached entity arrays
        private static NativeArray<Entity> _bossEntities;
        private static NativeArray<Entity> _playerEntities;
        private static NativeArray<Entity> _vBloodEntities;
        
        // Cache timestamps
        private static float _lastBossCacheUpdate = 0;
        private static float _lastPlayerCacheUpdate = 0;
        private static float _lastVBloodCacheUpdate = 0;
        
        // Cache validity flags
        private static bool _bossCacheValid = false;
        private static bool _playerCacheValid = false;
        private static bool _vBloodCacheValid = false;
        
        /// <summary>
        /// Gets all boss entities (with NameableInteractable and UnitStats)
        /// </summary>
        public static NativeArray<Entity> GetBossEntities()
        {
            var currentTime = UnityEngine.Time.time;
            
            // Check if cache needs update
            if (!_bossCacheValid || currentTime - _lastBossCacheUpdate > CACHE_UPDATE_INTERVAL)
            {
                // Dispose old cache if valid
                if (_bossCacheValid && _bossEntities.IsCreated)
                {
                    _bossEntities.Dispose();
                }
                
                // Create query
                var query = Plugin.SystemsCore.EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<NameableInteractable>(),
                    ComponentType.ReadOnly<UnitStats>()
                );
                
                // Get entities
                _bossEntities = query.ToEntityArray(Allocator.Persistent);
                
                // Update cache info
                _lastBossCacheUpdate = currentTime;
                _bossCacheValid = true;
                
                // Dispose query
                query.Dispose();
            }
            
            return _bossEntities;
        }
        
        /// <summary>
        /// Gets all player entities (with User component)
        /// </summary>
        public static NativeArray<Entity> GetPlayerEntities()
        {
            var currentTime = UnityEngine.Time.time;
            
            // Check if cache needs update
            if (!_playerCacheValid || currentTime - _lastPlayerCacheUpdate > CACHE_UPDATE_INTERVAL)
            {
                // Dispose old cache if valid
                if (_playerCacheValid && _playerEntities.IsCreated)
                {
                    _playerEntities.Dispose();
                }
                
                // Create query
                var query = Plugin.SystemsCore.EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<User>()
                );
                
                // Get entities
                _playerEntities = query.ToEntityArray(Allocator.Persistent);
                
                // Update cache info
                _lastPlayerCacheUpdate = currentTime;
                _playerCacheValid = true;
                
                // Dispose query
                query.Dispose();
            }
            
            return _playerEntities;
        }
        
        /// <summary>
        /// Gets all VBlood entities
        /// </summary>
        public static NativeArray<Entity> GetVBloodEntities()
        {
            var currentTime = UnityEngine.Time.time;
            
            // Check if cache needs update
            if (!_vBloodCacheValid || currentTime - _lastVBloodCacheUpdate > CACHE_UPDATE_INTERVAL)
            {
                // Dispose old cache if valid
                if (_vBloodCacheValid && _vBloodEntities.IsCreated)
                {
                    _vBloodEntities.Dispose();
                }
                
                // Create query
                var query = Plugin.SystemsCore.EntityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<VBloodUnit>()
                );
                
                // Get entities
                _vBloodEntities = query.ToEntityArray(Allocator.Persistent);
                
                // Update cache info
                _lastVBloodCacheUpdate = currentTime;
                _vBloodCacheValid = true;
                
                // Dispose query
                query.Dispose();
            }
            
            return _vBloodEntities;
        }
        
        /// <summary>
        /// Invalidates all caches, forcing refresh on next access
        /// </summary>
        public static void InvalidateAllCaches()
        {
            _bossCacheValid = false;
            _playerCacheValid = false;
            _vBloodCacheValid = false;
        }
        
        /// <summary>
        /// Invalidates boss cache only
        /// </summary>
        public static void InvalidateBossCache()
        {
            _bossCacheValid = false;
        }
        
        /// <summary>
        /// Invalidates player cache only
        /// </summary>
        public static void InvalidatePlayerCache()
        {
            _playerCacheValid = false;
        }
        
        /// <summary>
        /// Disposes all cached arrays - should be called on plugin unload
        /// </summary>
        public static void Dispose()
        {
            if (_bossCacheValid && _bossEntities.IsCreated)
            {
                _bossEntities.Dispose();
                _bossCacheValid = false;
            }
            
            if (_playerCacheValid && _playerEntities.IsCreated)
            {
                _playerEntities.Dispose();
                _playerCacheValid = false;
            }
            
            if (_vBloodCacheValid && _vBloodEntities.IsCreated)
            {
                _vBloodEntities.Dispose();
                _vBloodCacheValid = false;
            }
        }
        
        /// <summary>
        /// Gets cache statistics for debugging
        /// </summary>
        public static string GetCacheStats()
        {
            return $"ComponentCache Stats - Bosses: {(_bossCacheValid ? _bossEntities.Length : 0)} (Valid: {_bossCacheValid}), " +
                   $"Players: {(_playerCacheValid ? _playerEntities.Length : 0)} (Valid: {_playerCacheValid}), " +
                   $"VBloods: {(_vBloodCacheValid ? _vBloodEntities.Length : 0)} (Valid: {_vBloodCacheValid})";
        }
    }
}