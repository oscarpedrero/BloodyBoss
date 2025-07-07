using System;
using System.Collections.Generic;
using Unity.Entities;
using ProjectM;
using Bloody.Core;
using ProjectM.Network;
using Stunlock.Core;
using Unity.Collections;

namespace BloodyBoss.Systems.Mechanics
{
    public class PhaseMechanic : IMechanic
    {
        public string Type => "phase";

        public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
        {
            if (!CanApply(bossEntity))
                return;

            // Get parameters
            var phaseName = GetParameter<string>(parameters, "phase_name", "Phase 2");
            var announcement = GetParameter<string>(parameters, "announcement", $"⚔️ Boss enters {phaseName}!");
            var clearDebuffs = GetParameter<bool>(parameters, "clear_debuffs", true);
            var healPercent = GetParameter<float>(parameters, "heal_percent", 0f);
            var applyBuff = GetParameter<int>(parameters, "apply_buff", 0);

            // Clear debuffs if requested
            if (clearDebuffs && bossEntity.Has<BuffBuffer>())
            {
                // Apply cleanse buff
                var cleanseBuff = new PrefabGUID(1807499593); // Cleanse effect
                BuffNPC(bossEntity, cleanseBuff);
            }

            // Heal if requested
            if (healPercent > 0 && bossEntity.Has<Health>())
            {
                var health = bossEntity.Read<Health>();
                var healAmount = health.MaxHealth._Value * (healPercent / 100f);
                health.Value = Math.Min(health.Value + healAmount, health.MaxHealth._Value);
                bossEntity.Write(health);
            }

            // Apply phase buff
            if (applyBuff != 0)
            {
                BuffNPC(bossEntity, new PrefabGUID(applyBuff));
            }

            // Send announcement
            if (!string.IsNullOrEmpty(announcement))
            {
                var announcementRef = (FixedString512Bytes)announcement;
                ServerChatUtils.SendSystemMessageToAllClients(Core.SystemsCore.EntityManager, ref announcementRef);
            }

            Plugin.Logger.LogInfo($"Phase mechanic executed: {phaseName}");
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

        public bool Validate(Dictionary<string, object> parameters)
        {
            return true;
        }

        public string GetDescription()
        {
            return "Transitions the boss to a new combat phase";
        }

        public bool CanApply(Entity bossEntity)
        {
            return bossEntity.Has<Health>();
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