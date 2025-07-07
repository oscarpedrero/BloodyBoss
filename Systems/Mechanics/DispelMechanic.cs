using System;
using System.Collections.Generic;
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
    public class DispelMechanic : IMechanic
    {
        public string Type => "dispel";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var dispelType = GetParameter<string>(parameters, "dispel_type", "all");
            var target = GetParameter<string>(parameters, "target", "all_players");
            var radius = GetParameter<float>(parameters, "radius", 25f);
            var maxDispels = GetParameter<int>(parameters, "max_dispels", 5);
            var selfCleanse = GetParameter<bool>(parameters, "self_cleanse", true);
            var announcement = GetParameter<string>(parameters, "announcement", "✨ Magic is stripped away!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            
            // Dispel from players
            if (target != "self_only")
            {
                DispelFromPlayers(bossPos, radius, dispelType, maxDispels);
            }
            
            // Self cleanse
            if (selfCleanse || target == "self_only")
            {
                CleanseDebuffs(bossEntity);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Dispel mechanic executed: {dispelType} dispel on {target}");
        }

        private void DispelFromPlayers(float3 center, float radius, string dispelType, int maxDispels)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;
            var totalDispelled = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(center, pos) <= radiusSq)
                    {
                        var dispelled = DispelBuffs(user.Character.Entity, dispelType, maxDispels);
                        totalDispelled += dispelled;
                        
                        // Apply dispel visual effect
                        var dispelEffect = new PrefabGUID(1183622977); // Dispel magic effect
                        BuffCharacter(user.Character.Entity, dispelEffect);
                    }
                }
            }
            
            Plugin.Logger.LogInfo($"Dispelled {totalDispelled} buffs from players");
        }

        private int DispelBuffs(Entity target, string dispelType, int maxDispels)
        {
            if (!target.Has<BuffBuffer>())
                return 0;

            var buffBuffer = target.ReadBuffer<BuffBuffer>();
            var dispelledCount = 0;
            var buffsToRemove = new List<PrefabGUID>();

            foreach (var buff in buffBuffer)
            {
                if (dispelledCount >= maxDispels)
                    break;

                if (ShouldDispel(buff.PrefabGuid, dispelType))
                {
                    buffsToRemove.Add(buff.PrefabGuid);
                    dispelledCount++;
                }
            }

            // Apply cleanse to remove buffs
            if (buffsToRemove.Count > 0)
            {
                var cleanseBuff = new PrefabGUID(1807499593); // Cleanse
                BuffCharacter(target, cleanseBuff);
            }

            return dispelledCount;
        }

        private bool ShouldDispel(PrefabGUID buffGuid, string dispelType)
        {
            // Simplified categorization
            var magicBuffs = new HashSet<int> { -1133938228, 476036897, -901503997 };
            var physicalBuffs = new HashSet<int> { 581443919, 788443104, 2085766220 };
            var defensiveBuffs = new HashSet<int> { -1970513771, 584227282, 697337290 };

            switch (dispelType.ToLower())
            {
                case "magic":
                    return magicBuffs.Contains(buffGuid.GuidHash);
                case "physical":
                    return physicalBuffs.Contains(buffGuid.GuidHash);
                case "defensive":
                    return defensiveBuffs.Contains(buffGuid.GuidHash);
                case "all":
                default:
                    return magicBuffs.Contains(buffGuid.GuidHash) || 
                           physicalBuffs.Contains(buffGuid.GuidHash) || 
                           defensiveBuffs.Contains(buffGuid.GuidHash);
            }
        }

        private void CleanseDebuffs(Entity bossEntity)
        {
            // Apply full cleanse to remove all debuffs
            var cleanseBuff = new PrefabGUID(1807499593); // Cleanse
            BuffNPC(bossEntity, cleanseBuff);
            
            // Apply cleanse visual
            var cleanseVisual = new PrefabGUID(-1044788755); // Holy cleanse effect
            BuffNPC(bossEntity, cleanseVisual);
            
            Plugin.Logger.LogDebug("Boss cleansed of debuffs");
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
            var maxDispels = GetParameter<int>(parameters, "max_dispels", 5);
            return maxDispels > 0 && maxDispels <= 20;
        }

        public string GetDescription()
        {
            return "Removes buffs from players or debuffs from boss";
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