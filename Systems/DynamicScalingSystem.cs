using System;
using System.Linq;
using BloodyBoss.Configuration;
using BloodyBoss.DB.Models;
using Bloody.Core.GameData.v1;
using ProjectM;
using Unity.Entities;
using Bloody.Core.Helper.v1;
using Bloody.Core;
using Bloody.Core.API.v1;
using Unity.Collections;
using ProjectM.Network;
using BloodyBoss.Utils;

namespace BloodyBoss.Systems
{
    internal static class DynamicScalingSystem
    {
        internal static UnitStatsModel ApplyDynamicScaling(BossEncounterModel boss, UnitStatsModel baseStats)
        {
            if (!PluginConfig.EnableDynamicScaling.Value)
                return baseStats;

            var scaledStats = new UnitStatsModel();
            
            // Copy base stats
            scaledStats.PhysicalPower = baseStats.PhysicalPower;
            scaledStats.SpellPower = baseStats.SpellPower;
            scaledStats.ResourcePower = baseStats.ResourcePower;
            scaledStats.SiegePower = baseStats.SiegePower;
            scaledStats.PhysicalResistance = baseStats.PhysicalResistance;
            scaledStats.SpellResistance = baseStats.SpellResistance;
            scaledStats.FireResistance = baseStats.FireResistance;
            scaledStats.PassiveHealthRegen = baseStats.PassiveHealthRegen;
            scaledStats.CCReduction = baseStats.CCReduction;
            scaledStats.HealthRecovery = baseStats.HealthRecovery;
            scaledStats.DamageReduction = baseStats.DamageReduction;
            scaledStats.HealingReceived = baseStats.HealingReceived;

            // Calculate online players
            var onlinePlayers = GetOnlinePlayersCount();
            var effectivePlayers = Math.Min(onlinePlayers, PluginConfig.MaxPlayersForScaling.Value);

            // Apply base scaling
            var damageMultiplier = 1.0f + (effectivePlayers * PluginConfig.DamagePerPlayer.Value);

            // Apply progressive difficulty
            if (PluginConfig.EnableProgressiveDifficulty.Value)
            {
                var progressiveMultiplier = 1.0f + (boss.ConsecutiveSpawns * PluginConfig.DifficultyIncrease.Value);
                progressiveMultiplier = Math.Min(progressiveMultiplier, PluginConfig.MaxDifficultyMultiplier.Value);

                damageMultiplier *= progressiveMultiplier;
                boss.CurrentDifficultyMultiplier = progressiveMultiplier;
            }

            // Apply multipliers to damage stats
            scaledStats.PhysicalPower *= damageMultiplier;
            scaledStats.SpellPower *= damageMultiplier;

            Plugin.BLogger.Info(LogCategory.System, $"Boss {boss.name} scaled for {effectivePlayers} players: " +
                                $"Damage x{damageMultiplier:F2} (Progressive: x{boss.CurrentDifficultyMultiplier:F2})");

            // Check if we need to announce a phase change
            CheckAndAnnouncePhaseChange(boss, effectivePlayers, damageMultiplier);

            return scaledStats;
        }

        internal static float CalculateHealthMultiplier(BossEncounterModel boss)
        {
            if (!PluginConfig.EnableDynamicScaling.Value)
                return PluginConfig.BaseHealthMultiplier.Value;

            var onlinePlayers = GetOnlinePlayersCount();
            var effectivePlayers = Math.Min(onlinePlayers, PluginConfig.MaxPlayersForScaling.Value);

            var healthMultiplier = PluginConfig.BaseHealthMultiplier.Value +
                                 (effectivePlayers * PluginConfig.HealthPerPlayer.Value);

            if (PluginConfig.EnableProgressiveDifficulty.Value)
            {
                var progressiveMultiplier = 1.0f + (boss.ConsecutiveSpawns * PluginConfig.DifficultyIncrease.Value);
                progressiveMultiplier = Math.Min(progressiveMultiplier, PluginConfig.MaxDifficultyMultiplier.Value);
                healthMultiplier *= progressiveMultiplier;
            }

            return healthMultiplier;
        }

        private static void CheckAndAnnouncePhaseChange(BossEncounterModel boss, int effectivePlayers, float damageMultiplier)
        {
            try
            {
                // Check if phase announcements are enabled
                if (!PluginConfig.EnablePhaseAnnouncements.Value)
                    return;
                    
                var phaseInfo = GetPhaseInfo(effectivePlayers, boss.ConsecutiveSpawns, damageMultiplier);
                
                // Only announce if phase changed or if it's a significant milestone
                if (ShouldAnnouncePhase(boss, phaseInfo))
                {
                    AnnouncePhaseChange(boss, phaseInfo, effectivePlayers, damageMultiplier);
                    boss.LastAnnouncedPhase = phaseInfo.Phase;
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error announcing phase change: {ex.Message}");
            }
        }

        private static PhaseInfo GetPhaseInfo(int players, int consecutiveSpawns, float damageMultiplier)
        {
            // Determine phase based on player count and difficulty
            var phase = 1;
            var basePhaseName = PluginConfig.PhaseNormalName.Value;
            var phaseColor = "White";
            
            if (players >= 8 || damageMultiplier >= 3.0f)
            {
                phase = 4;
                basePhaseName = PluginConfig.PhaseLegendaryName.Value;
                phaseColor = "Gold";
            }
            else if (players >= 6 || damageMultiplier >= 2.5f)
            {
                phase = 3;
                basePhaseName = PluginConfig.PhaseEpicName.Value;
                phaseColor = "Purple";
            }
            else if (players >= 4 || damageMultiplier >= 2.0f)
            {
                phase = 2;
                basePhaseName = PluginConfig.PhaseHardName.Value;
                phaseColor = "Orange";
            }
            
            // Build final phase name with suffixes
            var finalPhaseName = basePhaseName;
            
            // Progressive difficulty bonus phases
            if (consecutiveSpawns >= 5)
            {
                phase += 1;
                finalPhaseName += PluginConfig.PhaseEnragedSuffix.Value;
                phaseColor = "Red";
            }
            else if (consecutiveSpawns >= 3)
            {
                finalPhaseName += PluginConfig.PhaseVeteranSuffix.Value;
            }
            
            return new PhaseInfo
            {
                Phase = phase,
                Name = finalPhaseName,
                Color = phaseColor,
                Players = players,
                ConsecutiveSpawns = consecutiveSpawns,
                DamageMultiplier = damageMultiplier,
                BaseName = basePhaseName
            };
        }

        private static bool ShouldAnnouncePhase(BossEncounterModel boss, PhaseInfo newPhase)
        {
            // Always announce first spawn
            if (boss.LastAnnouncedPhase == 0)
                return true;
                
            // Announce if phase increased
            if (newPhase.Phase > boss.LastAnnouncedPhase)
                return true;
                
            // Announce every phase if configured
            if (PluginConfig.AnnounceEveryPhase.Value && newPhase.Phase != boss.LastAnnouncedPhase)
                return true;
                
            // Announce milestone consecutive spawns if configured
            if (PluginConfig.AnnounceMilestoneSpawns.Value && newPhase.ConsecutiveSpawns > 0 && newPhase.ConsecutiveSpawns % 3 == 0)
                return true;
                
            return false;
        }

        private static void AnnouncePhaseChange(BossEncounterModel boss, PhaseInfo phase, int players, float damageMultiplier)
        {
            // Select appropriate template based on phase
            string template = GetPhaseTemplate(phase);
            
            // Build consecutive info if needed
            string consecutiveInfo = "";
            if (phase.ConsecutiveSpawns > 0)
            {
                consecutiveInfo = PluginConfig.ConsecutiveInfoTemplate.Value
                    .Replace("#consecutive#", phase.ConsecutiveSpawns.ToString());
            }
            
            // Replace all placeholders
            var message = template
                .Replace("#bossname#", FontColorChatSystem.Yellow(boss.name))
                .Replace("#phasename#", GetColoredPhaseName(phase))
                .Replace("#phase#", phase.Phase.ToString())
                .Replace("#players#", players.ToString())
                .Replace("#damage#", damageMultiplier.ToString("F1"))
                .Replace("#consecutive#", phase.ConsecutiveSpawns.ToString())
                .Replace("#consecutive_info#", consecutiveInfo);
            
            // Add prefixes for special phases
            if (phase.Phase >= 4)
            {
                message = PluginConfig.PhaseLegendaryPrefix.Value + message + " 💀";
            }
            else if (phase.Phase >= 3)
            {
                message = PluginConfig.PhaseEpicPrefix.Value + message + " ⚡";
            }
            
            var finalMessage = FontColorChatSystem.Green(message);
            var refMessage = (FixedString512Bytes)finalMessage;
            
            ServerChatUtils.SendSystemMessageToAllClients(Plugin.SystemsCore.EntityManager, ref refMessage);
            
            Plugin.BLogger.Info(LogCategory.System, $"Phase announced: {boss.name} - {phase.Name} (Phase {phase.Phase})");
        }
        
        private static string GetPhaseTemplate(PhaseInfo phase)
        {
            return phase.BaseName switch
            {
                var name when name == PluginConfig.PhaseLegendaryName.Value => PluginConfig.PhaseLegendaryTemplate.Value,
                var name when name == PluginConfig.PhaseEpicName.Value => PluginConfig.PhaseEpicTemplate.Value,
                var name when name == PluginConfig.PhaseHardName.Value => PluginConfig.PhaseHardTemplate.Value,
                _ => PluginConfig.PhaseNormalTemplate.Value
            };
        }
        
        private static string GetColoredPhaseName(PhaseInfo phase)
        {
            var colorMethod = GetColorMethod(phase.Color);
            return colorMethod(phase.Name);
        }
        
        private static Func<string, string> GetColorMethod(string color)
        {
            return color switch
            {
                "Red" => FontColorChatSystem.Red,
                "Orange" => FontColorChatSystem.Yellow, // Fallback to yellow for orange
                "Yellow" => FontColorChatSystem.Yellow,
                "Green" => FontColorChatSystem.Green,
                "Blue" => FontColorChatSystem.Blue,
                "Purple" => FontColorChatSystem.Blue, // Fallback to blue for purple
                "Gold" => FontColorChatSystem.Yellow, // Fallback to yellow for gold
                _ => FontColorChatSystem.White
            };
        }

        internal static void ApplyHealthScaling(Entity bossEntity, float healthMultiplier)
        {
            try
            {
                // Use BloodyCore's QueryComponents to safely read/write Health component
                if (bossEntity.Has<Health>())
                {
                    var health = bossEntity.Read<Health>();
                    var newMaxHealth = health.MaxHealth._Value * healthMultiplier;

                    health.MaxHealth._Value = newMaxHealth;
                    health.Value = newMaxHealth; // Boss spawns at full health

                    bossEntity.Write(health);

                    Plugin.BLogger.Info(LogCategory.System, $"Boss health scaled to {newMaxHealth:F0} HP (x{healthMultiplier:F2})");
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error applying health scaling: {ex.Message}");
            }
        }

        private static int GetOnlinePlayersCount()
        {
            try
            {
                return GameData.Users.Online.Count();
            }
            catch
            {
                // Fallback if GameData is not available
                return 1;
            }
        }

        internal static ScalingInfo GetScalingInfo(BossEncounterModel boss)
        {
            var onlinePlayers = GetOnlinePlayersCount();
            var effectivePlayers = Math.Min(onlinePlayers, PluginConfig.MaxPlayersForScaling.Value);
            
            var healthMultiplier = CalculateHealthMultiplier(boss);
            var damageMultiplier = 1.0f + (effectivePlayers * PluginConfig.DamagePerPlayer.Value);
            
            if (PluginConfig.EnableProgressiveDifficulty.Value)
            {
                var progressiveMultiplier = 1.0f + (boss.ConsecutiveSpawns * PluginConfig.DifficultyIncrease.Value);
                progressiveMultiplier = Math.Min(progressiveMultiplier, PluginConfig.MaxDifficultyMultiplier.Value);
                damageMultiplier *= progressiveMultiplier;
            }

            return new ScalingInfo
            {
                OnlinePlayers = onlinePlayers,
                EffectivePlayers = effectivePlayers,
                HealthMultiplier = healthMultiplier,
                DamageMultiplier = damageMultiplier,
                ProgressiveMultiplier = boss.CurrentDifficultyMultiplier,
                ConsecutiveSpawns = boss.ConsecutiveSpawns
            };
        }
    }

    internal class ScalingInfo
    {
        public int OnlinePlayers { get; set; }
        public int EffectivePlayers { get; set; }
        public float HealthMultiplier { get; set; }
        public float DamageMultiplier { get; set; }
        public float ProgressiveMultiplier { get; set; }
        public int ConsecutiveSpawns { get; set; }
    }
    
    internal class PhaseInfo
    {
        public int Phase { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BaseName { get; set; } = string.Empty;
        public string Color { get; set; } = "White";
        public int Players { get; set; }
        public int ConsecutiveSpawns { get; set; }
        public float DamageMultiplier { get; set; }
    }
}