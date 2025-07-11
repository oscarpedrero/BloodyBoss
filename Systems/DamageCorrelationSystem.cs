using System;
using System.Collections.Generic;
using Unity.Entities;

namespace BloodyBoss.Systems
{
    /// <summary>
    /// Correlates damage events from different sources to provide complete damage information
    /// </summary>
    public static class DamageCorrelationSystem
    {
        private struct DamageRecord
        {
            public float Damage;
            public Entity Attacker;
            public DateTime Timestamp;
            public bool HasDamageInfo;
            public bool HasAttackerInfo;
        }
        
        // Cache to correlate events by target entity
        private static Dictionary<Entity, DamageRecord> _damageCache = new Dictionary<Entity, DamageRecord>();
        private static readonly TimeSpan CORRELATION_WINDOW = TimeSpan.FromMilliseconds(100); // 100ms window
        private static readonly int MAX_CACHE_SIZE = 100; // Maximum entries to prevent memory leaks
        private static DateTime _lastCleanupTime = DateTime.UtcNow;
        
        /// <summary>
        /// Record damage amount from StatChangeEvent
        /// </summary>
        public static void RecordDamage(Entity target, float damage)
        {
            if (_damageCache.TryGetValue(target, out var record))
            {
                // Update existing record
                record.Damage = damage;
                record.HasDamageInfo = true;
                record.Timestamp = DateTime.UtcNow;
                _damageCache[target] = record;
                
                // If we have both pieces of info, process immediately
                if (record.HasAttackerInfo)
                {
                    ProcessCorrelatedDamage(target, record);
                }
            }
            else
            {
                // Create new record
                _damageCache[target] = new DamageRecord
                {
                    Damage = damage,
                    HasDamageInfo = true,
                    Timestamp = DateTime.UtcNow
                };
            }
            
            // Enforce cache size limit
            EnforceCacheSizeLimit();
            CleanupOldRecords();
        }
        
        /// <summary>
        /// Record attacker from DamageTakenEvent
        /// </summary>
        public static void RecordAttacker(Entity target, Entity attacker)
        {
            if (_damageCache.TryGetValue(target, out var record))
            {
                // Update existing record
                record.Attacker = attacker;
                record.HasAttackerInfo = true;
                record.Timestamp = DateTime.UtcNow;
                _damageCache[target] = record;
                
                // If we have both pieces of info, process immediately
                if (record.HasDamageInfo)
                {
                    ProcessCorrelatedDamage(target, record);
                }
            }
            else
            {
                // Create new record
                _damageCache[target] = new DamageRecord
                {
                    Attacker = attacker,
                    HasAttackerInfo = true,
                    Timestamp = DateTime.UtcNow
                };
            }
            
            // Enforce cache size limit
            EnforceCacheSizeLimit();
            CleanupOldRecords();
        }
        
        /// <summary>
        /// Process correlated damage event
        /// </summary>
        private static void ProcessCorrelatedDamage(Entity target, DamageRecord record)
        {
            Plugin.Logger.LogInfo($"[DamageCorrelation] Correlated damage - Target: {target.Index}, Damage: {record.Damage:F1}, Attacker: {record.Attacker.Index}");
            
            // Remove from cache after processing
            _damageCache.Remove(target);
            
            // Note: Here we would call the unified damage processing
            // For now, the existing separate processing in BossGameplayEventSystem continues
        }
        
        /// <summary>
        /// Clean up old records that weren't correlated
        /// </summary>
        private static void CleanupOldRecords()
        {
            var now = DateTime.UtcNow;
            
            // Periodic cleanup every 5 seconds
            if ((now - _lastCleanupTime).TotalSeconds < 5)
                return;
                
            _lastCleanupTime = now;
            var keysToRemove = new List<Entity>();
            
            foreach (var kvp in _damageCache)
            {
                if (now - kvp.Value.Timestamp > CORRELATION_WINDOW)
                {
                    keysToRemove.Add(kvp.Key);
                    
                    // Log incomplete records for debugging
                    if (!kvp.Value.HasDamageInfo || !kvp.Value.HasAttackerInfo)
                    {
                        Plugin.Logger.LogDebug($"[DamageCorrelation] Expired incomplete record - Target: {kvp.Key.Index}, HasDamage: {kvp.Value.HasDamageInfo}, HasAttacker: {kvp.Value.HasAttackerInfo}");
                    }
                }
            }
            
            foreach (var key in keysToRemove)
            {
                _damageCache.Remove(key);
            }
            
            if (keysToRemove.Count > 0)
            {
                Plugin.Logger.LogDebug($"[DamageCorrelation] Cleaned up {keysToRemove.Count} expired records");
            }
        }
        
        /// <summary>
        /// Enforce maximum cache size to prevent memory leaks
        /// </summary>
        private static void EnforceCacheSizeLimit()
        {
            if (_damageCache.Count <= MAX_CACHE_SIZE)
                return;
                
            // Remove oldest entries
            var entriesToRemove = _damageCache.Count - MAX_CACHE_SIZE + 10; // Remove 10 extra for headroom
            var sortedEntries = new List<KeyValuePair<Entity, DamageRecord>>(_damageCache);
            sortedEntries.Sort((a, b) => a.Value.Timestamp.CompareTo(b.Value.Timestamp));
            
            for (int i = 0; i < entriesToRemove && i < sortedEntries.Count; i++)
            {
                _damageCache.Remove(sortedEntries[i].Key);
            }
            
            Plugin.Logger.LogWarning($"[DamageCorrelation] Cache size limit exceeded. Removed {entriesToRemove} oldest entries. Current size: {_damageCache.Count}");
        }
        
        /// <summary>
        /// Clear all cached records
        /// </summary>
        public static void Clear()
        {
            _damageCache.Clear();
        }
    }
}