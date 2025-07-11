using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ProjectM;
using Bloody.Core;
using Bloody.Core.GameData.v1;
using Bloody.Core.Models.v1;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;

namespace BloodyBoss.Systems.Mechanics
{
    public class BuffStealMechanic : IMechanic
    {
        public string Type => "buff_steal";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var target = GetParameter<string>(parameters, "target", "random");
            var maxSteals = GetParameter<int>(parameters, "max_steals", 3);
            var stealFromAll = GetParameter<bool>(parameters, "steal_from_all", false);
            var applyDebuff = GetParameter<bool>(parameters, "apply_debuff", true);
            var announcement = GetParameter<string>(parameters, "announcement", "🎭 The boss steals your power!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            StealBuffsFromPlayers(bossEntity, target, maxSteals, stealFromAll, applyDebuff);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Buff steal mechanic executed: Stealing up to {maxSteals} buffs");
        }

        private void StealBuffsFromPlayers(Entity bossEntity, string targetType, int maxSteals, bool stealFromAll, bool applyDebuff)
        {
            var users = GameData.Users.Online.ToList();
            var stolenCount = 0;

            // Select targets based on type
            if (targetType.ToLower() == "random" && !stealFromAll)
            {
                var random = new System.Random();
                users = users.OrderBy(x => random.Next()).Take(1).ToList();
            }

            foreach (var user in users)
            {
                if (stolenCount >= maxSteals && !stealFromAll)
                    break;

                if (user.Character.Entity.Has<BuffBuffer>())
                {
                    var buffBuffer = user.Character.Entity.ReadBuffer<BuffBuffer>();
                    var buffsToSteal = new List<PrefabGUID>();
                    
                    // Identify positive buffs to steal
                    foreach (var buff in buffBuffer)
                    {
                        if (buffsToSteal.Count >= maxSteals)
                            break;
                            
                        // Check if it's a positive buff (simplified check)
                        if (IsPositiveBuff(buff.PrefabGuid))
                        {
                            buffsToSteal.Add(buff.PrefabGuid);
                        }
                    }
                    
                    // Apply stolen buffs to boss
                    foreach (var buffGuid in buffsToSteal)
                    {
                        BuffNPC(bossEntity, buffGuid);
                        stolenCount++;
                        
                        // Remove buff from player (simulate by applying dispel)
                        var dispelBuff = new PrefabGUID(1807499593); // Cleanse/Dispel
                        BuffCharacter(user.Character.Entity, dispelBuff);
                    }
                    
                    // Apply debuff to player if requested
                    if (applyDebuff && buffsToSteal.Count > 0)
                    {
                        var weaknessBuff = new PrefabGUID(-1584651229); // Weakness debuff
                        BuffCharacter(user.Character.Entity, weaknessBuff);
                    }
                    
                    Plugin.BLogger.Debug(LogCategory.Mechanic, $"Stole {buffsToSteal.Count} buffs from player");
                }
            }
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Total buffs stolen: {stolenCount}");
        }

        private bool IsPositiveBuff(PrefabGUID buffGuid)
        {
            // Simplified check - in real implementation would check buff categories
            var positiveBuff = new HashSet<int>
            {
                581443919,  // Blood buff
                -1133938228, // Speed buff
                788443104,  // Movement speed
                476036897,  // Damage buff
                2085766220  // AB_Blood_BloodRage_Buff_MagicSource
            };
            
            return positiveBuff.Contains(buffGuid.GuidHash);
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

        private void BuffCharacter(Entity character, PrefabGUID buffGuid)
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
        }

        public bool Validate(Dictionary<string, object> parameters)
        {
            var maxSteals = GetParameter<int>(parameters, "max_steals", 3);
            return maxSteals > 0 && maxSteals <= 10;
        }

        public string GetDescription()
        {
            return "Steals buffs from players and applies them to the boss";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<BuffBuffer>();
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