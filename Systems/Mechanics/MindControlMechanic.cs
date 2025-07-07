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
    public class MindControlMechanic : IMechanic
    {
        public string Type => "mind_control";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var target = GetParameter<string>(parameters, "target", "random");
            var duration = GetParameter<float>(parameters, "duration", 5f);
            var forceAttackAllies = GetParameter<bool>(parameters, "force_attack_allies", true);
            var increaseSpeed = GetParameter<bool>(parameters, "increase_speed", true);
            var announcement = GetParameter<string>(parameters, "announcement", "🧠 Your mind belongs to me!");

            var targetEntity = SelectTarget(bossEntity, target);
            
            if (targetEntity != Entity.Null)
            {
                ApplyMindControl(targetEntity, duration, forceAttackAllies, increaseSpeed);
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Mind control mechanic executed on target for {duration}s");
        }

        private Entity SelectTarget(Entity bossEntity, string targetType)
        {
            var users = GameData.Users.Online.ToList();
            if (users.Count == 0) return Entity.Null;

            Entity selectedTarget = Entity.Null;
            var bossPos = bossEntity.Read<LocalToWorld>().Position;

            switch (targetType.ToLower())
            {
                case "random":
                    var random = new System.Random();
                    var randomUser = users[random.Next(users.Count)];
                    selectedTarget = randomUser.Character.Entity;
                    break;
                    
                case "highest_health":
                    var highestHp = users
                        .Where(u => u.Character.Entity.Has<Health>())
                        .OrderByDescending(u => u.Character.Entity.Read<Health>().Value)
                        .FirstOrDefault();
                    if (highestHp != null)
                        selectedTarget = highestHp.Character.Entity;
                    break;
                    
                case "lowest_health":
                    var lowestHp = users
                        .Where(u => u.Character.Entity.Has<Health>())
                        .OrderBy(u => u.Character.Entity.Read<Health>().Value)
                        .FirstOrDefault();
                    if (lowestHp != null)
                        selectedTarget = lowestHp.Character.Entity;
                    break;
                    
                case "farthest":
                    var farthest = users
                        .Where(u => u.Character.Entity.Has<LocalToWorld>())
                        .OrderByDescending(u => 
                        {
                            var pos = u.Character.Entity.Read<LocalToWorld>().Position;
                            return math.distance(bossPos, pos);
                        })
                        .FirstOrDefault();
                    if (farthest != null)
                        selectedTarget = farthest.Character.Entity;
                    break;
            }

            return selectedTarget;
        }

        private void ApplyMindControl(Entity target, float duration, bool forceAttackAllies, bool increaseSpeed)
        {
            // Apply mind control buff
            var mindControlBuff = new PrefabGUID(88549126); // Charm/Mind Control
            BuffCharacter(target, mindControlBuff);
            
            // Apply confusion to simulate attacking allies
            if (forceAttackAllies)
            {
                var confusionBuff = new PrefabGUID(-659039270); // Confusion
                BuffCharacter(target, confusionBuff);
            }
            
            // Apply speed buff if requested
            if (increaseSpeed)
            {
                var speedBuff = new PrefabGUID(788443104); // Movement speed
                BuffCharacter(target, speedBuff);
            }
            
            // Visual effect for mind control
            var visualBuff = new PrefabGUID(-893140707); // Purple glow effect
            BuffCharacter(target, visualBuff);
            
            Plugin.Logger.LogDebug($"Applied mind control to entity for {duration}s");
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
            var duration = GetParameter<float>(parameters, "duration", 5f);
            return duration > 0 && duration <= 15;
        }

        public string GetDescription()
        {
            return "Takes control of a player's mind";
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