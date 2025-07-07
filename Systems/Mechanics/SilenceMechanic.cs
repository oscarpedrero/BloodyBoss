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
    public class SilenceMechanic : IMechanic
    {
        public string Type => "silence";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var radius = GetParameter<float>(parameters, "radius", 20f);
            var duration = GetParameter<float>(parameters, "duration", 4f);
            var disableItems = GetParameter<bool>(parameters, "disable_items", true);
            var announcement = GetParameter<string>(parameters, "announcement", "🔇 Silence falls upon the battlefield!");

            var bossPos = bossEntity.Read<LocalToWorld>().Position;
            ApplySilenceInArea(bossPos, radius, duration, disableItems);

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Silence mechanic executed: {duration}s silence in {radius} radius");
        }

        private void ApplySilenceInArea(float3 center, float radius, float duration, bool disableItems)
        {
            var radiusSq = radius * radius;
            var users = GameData.Users.Online;
            var silencedCount = 0;

            foreach (var user in users)
            {
                if (user.Character.Entity.Has<LocalToWorld>())
                {
                    var pos = user.Character.Entity.Read<LocalToWorld>().Position;
                    if (math.distancesq(center, pos) <= radiusSq)
                    {
                        ApplySilence(user.Character.Entity, duration, disableItems);
                        silencedCount++;
                    }
                }
            }
            
            Plugin.Logger.LogInfo($"Silenced {silencedCount} players");
        }

        private void ApplySilence(Entity target, float duration, bool disableItems)
        {
            // Apply silence buff (prevents spell casting)
            var silenceBuff = new PrefabGUID(1645032245); // Silence
            BuffCharacter(target, silenceBuff);
            
            // Apply item disable if requested
            if (disableItems)
            {
                var itemDisableBuff = new PrefabGUID(-106495630); // Disable consumables
                BuffCharacter(target, itemDisableBuff);
            }
            
            Plugin.Logger.LogDebug($"Applied silence to entity for {duration}s");
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
            var duration = GetParameter<float>(parameters, "duration", 4f);
            var radius = GetParameter<float>(parameters, "radius", 20f);
            
            return duration > 0 && duration <= 15 && radius > 0 && radius <= 50;
        }

        public string GetDescription()
        {
            return "Prevents players from casting spells";
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