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
    public class CurseMechanic : IMechanic
    {
        public string Type => "curse";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var curseType = GetParameter<string>(parameters, "curse_type", "random");
            var target = GetParameter<string>(parameters, "target", "all");
            var duration = GetParameter<float>(parameters, "duration", 15f);
            var spreadable = GetParameter<bool>(parameters, "spreadable", false);
            var spreadRadius = GetParameter<float>(parameters, "spread_radius", 10f);
            var stackLimit = GetParameter<int>(parameters, "stack_limit", 3);
            var announcement = GetParameter<string>(parameters, "announcement", "☠️ Ancient curse takes hold!");

            ApplyCurse(bossEntity, curseType, target, duration, spreadable, spreadRadius, stackLimit);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Curse mechanic executed: {curseType} curse for {duration}s");
        }

        private void ApplyCurse(Entity bossEntity, string curseType, string target, float duration, bool spreadable, float spreadRadius, int stackLimit)
        {
            var users = GameData.Users.Online;
            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            var random = new System.Random();

            foreach (var user in users)
            {
                if (ShouldCurseTarget(user, target, bossPos, random))
                {
                    ApplyCurseToTarget(user.Character.Entity, curseType, duration, spreadable, spreadRadius, stackLimit, random);
                }
            }
        }

        private bool ShouldCurseTarget(UserModel user, string target, float3 bossPos, System.Random random)
        {
            if (!user.Character.Entity.Has<LocalToWorld>())
                return false;

            switch (target.ToLower())
            {
                case "all":
                    return true;
                case "random":
                    return random.NextDouble() < 0.5;
                case "closest":
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    return math.distance(bossPos, pos) < 20f;
                default:
                    return true;
            }
        }

        private void ApplyCurseToTarget(Entity target, string curseType, float duration, bool spreadable, float spreadRadius, int stackLimit, System.Random random)
        {
            PrefabGUID curseBuff;
            
            switch (curseType.ToLower())
            {
                case "doom":
                    // Damage over time that increases
                    curseBuff = new PrefabGUID(-1308605301); // Doom curse
                    break;
                    
                case "weakness":
                    // Reduces all resistances
                    curseBuff = new PrefabGUID(1528607403); // Weakness curse
                    break;
                    
                case "silence":
                    // Prevents ability usage
                    curseBuff = new PrefabGUID(-1936575297); // Silence curse
                    break;
                    
                case "slow":
                    // Movement and attack speed reduction
                    curseBuff = new PrefabGUID(-372513663); // Slow curse
                    break;
                    
                case "random":
                    var curses = new PrefabGUID[]
                    {
                        new PrefabGUID(-1308605301), // Doom
                        new PrefabGUID(1528607403),  // Weakness
                        new PrefabGUID(-1936575297), // Silence
                        new PrefabGUID(-372513663)   // Slow
                    };
                    curseBuff = curses[random.Next(curses.Length)];
                    break;
                    
                default:
                    curseBuff = new PrefabGUID(-1308605301); // Default to doom
                    break;
            }

            // Apply the curse
            BuffCharacter(target, curseBuff);
            
            // Apply curse visual effect
            var curseVisual = new PrefabGUID(1945470215); // Dark curse visual
            BuffCharacter(target, curseVisual);
            
            // If spreadable, mark with contagion
            if (spreadable)
            {
                var contagionBuff = new PrefabGUID(-949672483); // Contagion marker
                BuffCharacter(target, contagionBuff);
            }
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied {curseType} curse for {duration}s (spreadable: {spreadable})");
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
            var duration = GetParameter<float>(parameters, "duration", 15f);
            var stackLimit = GetParameter<int>(parameters, "stack_limit", 3);
            
            return duration > 0 && duration <= 60 && stackLimit > 0 && stackLimit <= 10;
        }

        public string GetDescription()
        {
            return "Applies various powerful curses to players";
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