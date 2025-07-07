using Bloody.Core;
using BloodyBoss.DB;
using ProjectM;
using System;
using System.Collections.Generic;
using Unity.Entities;

namespace BloodyBoss.Systems
{
    internal static class HealthMonitorSystem
    {
        private static Dictionary<string, float> _lastHealthCheck = new Dictionary<string, float>();
        private static float _lastCheckTime = 0;
        private const float CHECK_INTERVAL = 0.5f; // Check every 0.5 seconds
        
        public static void Update()
        {
            try
            {
                var currentTime = UnityEngine.Time.time;
                if (currentTime - _lastCheckTime < CHECK_INTERVAL)
                    return;
                    
                _lastCheckTime = currentTime;
                
                // Check all spawned bosses
                foreach (var boss in Database.BOSSES)
                {
                    if (!boss.bossSpawn || !boss.GetBossEntity())
                        continue;
                        
                    if (boss.bossEntity.Has<Health>())
                    {
                        var health = boss.bossEntity.Read<Health>();
                        float currentHpPercent = (health.Value / health.MaxHealth._Value) * 100f;
                        
                        var healthKey = $"{boss.nameHash}_health";
                        
                        // First time check or health changed
                        if (!_lastHealthCheck.ContainsKey(healthKey))
                        {
                            _lastHealthCheck[healthKey] = currentHpPercent;
                            Plugin.Logger.LogInfo($"[HEALTH-MONITOR] Started monitoring boss {boss.name} at {currentHpPercent:F1}% HP");
                        }
                        else if (Math.Abs(_lastHealthCheck[healthKey] - currentHpPercent) > 0.1f)
                        {
                            // Health changed
                            float previousHpPercent = _lastHealthCheck[healthKey];
                            Plugin.Logger.LogWarning($"[HEALTH-MONITOR] Boss {boss.name} HP changed: {previousHpPercent:F1}% -> {currentHpPercent:F1}%");
                            
                            // Check mechanics - pass both previous and current HP
                            BossMechanicSystem.CheckHpThresholdMechanics(boss.bossEntity, boss, currentHpPercent, previousHpPercent);
                            
                            _lastHealthCheck[healthKey] = currentHpPercent;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"[HEALTH-MONITOR] Error: {ex.Message}");
            }
        }
        
        public static void RemoveBoss(string bossNameHash)
        {
            var healthKey = $"{bossNameHash}_health";
            _lastHealthCheck.Remove(healthKey);
        }
    }
}