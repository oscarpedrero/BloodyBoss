using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using Bloody.Core.API.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;
using System.Linq;
using ProjectM.Shared;

namespace BloodyBoss.Systems.Mechanics
{
    public class SlowMechanic : IMechanic
    {
        public string Type => "slow";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var radius = GetParameter<float>(parameters, "radius", 15f);
            var announcement = GetParameter<string>(parameters, "announcement", "🐌 Time slows down!");
            
            // New mechanic: Minimum players required in range
            var minPlayersRequired = GetParameter<int>(parameters, "min_players", 0);
            var globalRadius = GetParameter<float>(parameters, "global_radius", 50f);

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            
            // Apply slow effect with min players mechanic
            ApplySlowWithMinPlayers(bossEntity, bossPos, radius, 
                minPlayersRequired, globalRadius);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Slow mechanic executed in radius {radius}m");
        }

        private void ApplySlowWithMinPlayers(Entity bossEntity, float3 bossPos, float radius,
            int minPlayersRequired, float globalRadius)
        {
            var radiusSq = radius * radius;
            var globalRadiusSq = globalRadius * globalRadius;
            var users = GameData.Users.Online.ToList();
            var playersInRange = new List<UserModel>();
            var allPlayersInGlobalRange = new List<UserModel>();
            
            // First count players in normal range and global range
            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    var distSq = math.distancesq(bossPos, pos);
                    
                    if (distSq <= radiusSq)
                    {
                        playersInRange.Add(user);
                    }
                    
                    if (distSq <= globalRadiusSq)
                    {
                        allPlayersInGlobalRange.Add(user);
                    }
                }
            }
            
            // Check if minimum players requirement is met
            if (minPlayersRequired > 0 && playersInRange.Count < minPlayersRequired)
            {
                // Not enough players in range! Global punishment!
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Only {playersInRange.Count}/{minPlayersRequired} players in range! GLOBAL SLOW ACTIVATED!");
                
                // Send warning to all players
                var warningMsg = $"⚠️ NOT ENOUGH PLAYERS IN RANGE! {playersInRange.Count}/{minPlayersRequired} - GLOBAL SLOW ACTIVE!";
                var warningRef = (FixedString512Bytes)warningMsg;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref warningRef);
                
                // Slow ALL players in global radius
                foreach (var user in allPlayersInGlobalRange)
                {
                    ApplySlowToPlayer(user.Character.Entity);
                }
                
                Plugin.BLogger.Info(LogCategory.Mechanic, $"GLOBAL SLOW: Slowed {allPlayersInGlobalRange.Count} players!");
            }
            else
            {
                // Normal slow from players in range
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Normal slow: {playersInRange.Count} players in range");
                
                foreach (var user in playersInRange)
                {
                    ApplySlowToPlayer(user.Character.Entity);
                }
                
                Plugin.BLogger.Info(LogCategory.Mechanic, $"Slowed {playersInRange.Count} players");
            }
        }

        private void ApplySlowToPlayer(Entity target)
        {
            // Apply actual slow buff
            var slowBuff = new PrefabGUID(2072256768); // Buff_General_Slow - This actually slows!
            BuffCharacter(target, slowBuff);
            
            // Apply visual effect
            var energyDrainVisual = new PrefabGUID(178387762); // AB_Blood_VampiricCurse_Buff_Lesser
            BuffCharacter(target, energyDrainVisual);
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied slow to player");
        }

        private void BuffCharacter(Entity character, PrefabGUID buffGuid, float duration = 0f)
        {
            try
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
                
                // If duration specified, remove buff after duration
                if (duration > 0)
                {
                    CoroutineHandler.StartGenericCoroutine(() =>
                    {
                        RemoveBuff(character, buffGuid);
                    }, duration);
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to apply buff: {ex.Message}");
            }
        }
        
        private void RemoveBuff(Entity character, PrefabGUID buffGuid)
        {
            try
            {
                if (BuffUtility.TryGetBuff(Core.SystemsCore.EntityManager, character, buffGuid, out Entity buffEntity))
                {
                    DestroyUtility.Destroy(Core.SystemsCore.EntityManager, buffEntity);
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Warning(LogCategory.Mechanic, $"Failed to remove buff: {ex.Message}");
            }
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var radius = GetParameter<float>(parameters, "radius", 15f);
            
            return radius > 0 && radius <= 50;
        }

        public string GetDescription()
        {
            return "Slows players - energy theft mechanic";
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