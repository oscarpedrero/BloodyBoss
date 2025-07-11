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
    public class VisionMechanic : IMechanic
    {
        public string Type => "vision";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var visionType = GetParameter<string>(parameters, "vision_type", "darkness");
            var radius = GetParameter<float>(parameters, "radius", 30f);
            var duration = GetParameter<float>(parameters, "duration", 10f);
            var intensity = GetParameter<float>(parameters, "intensity", 80f);
            var announcement = GetParameter<string>(parameters, "announcement", "👁️ Your vision betrays you!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            ApplyVisionEffect(bossPos, radius, visionType, duration, intensity);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.BLogger.Info(LogCategory.Mechanic, $"Vision mechanic executed: {visionType} for {duration}s at {intensity}% intensity");
        }

        private void ApplyVisionEffect(float3 center, float radius, string visionType, float duration, float intensity)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;
            var affectedCount = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(center, pos) <= radiusSq)
                    {
                        ApplyVisionDebuff(user.Character.Entity, visionType, duration, intensity);
                        affectedCount++;
                    }
                }
            }
            
            Plugin.BLogger.Info(LogCategory.Mechanic, $"Applied vision effect to {affectedCount} players");
        }

        private void ApplyVisionDebuff(Entity target, string visionType, float duration, float intensity)
        {
            PrefabGUID visionBuff;
            
            switch (visionType.ToLower())
            {
                case "darkness":
                    visionBuff = new PrefabGUID(-1269225330); // Darkness/Blind
                    break;
                case "blur":
                    visionBuff = new PrefabGUID(1199258540); // Blur vision
                    break;
                case "hallucination":
                    visionBuff = new PrefabGUID(-659039270); // Confusion/Hallucination
                    break;
                case "fog":
                    visionBuff = new PrefabGUID(1975317277); // Fog effect
                    break;
                default:
                    visionBuff = new PrefabGUID(-1269225330); // Default to darkness
                    break;
            }

            BuffCharacter(target, visionBuff);
            
            // Apply additional screen effects based on intensity
            if (intensity > 50)
            {
                var screenEffectBuff = new PrefabGUID(-106264639); // Screen distortion
                BuffCharacter(target, screenEffectBuff);
            }
            
            Plugin.BLogger.Debug(LogCategory.Mechanic, $"Applied {visionType} vision effect for {duration}s");
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
            var duration = GetParameter<float>(parameters, "duration", 10f);
            var intensity = GetParameter<float>(parameters, "intensity", 80f);
            
            return duration > 0 && duration <= 30 && intensity > 0 && intensity <= 100;
        }

        public string GetDescription()
        {
            return "Impairs player vision with various effects";
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