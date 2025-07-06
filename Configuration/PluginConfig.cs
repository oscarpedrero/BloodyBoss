using System.IO;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
using ProjectM;
using Unity.DebugDisplay;


namespace BloodyBoss.Configuration
{
    internal class PluginConfig
    {
        private static ConfigFile _mainConfig;

        public static ConfigEntry<bool> Enabled { get; private set; }
        public static ConfigEntry<int> BuffForWorldBoss { get; private set; }
        public static ConfigEntry<string> SpawnMessageBossTemplate { get; private set; }
        public static ConfigEntry<string> DespawnMessageBossTemplate { get; private set; }
        public static ConfigEntry<string> KillMessageBossTemplate { get; private set; }
        public static ConfigEntry<string> VBloodFinalConcatCharacters { get; private set; }
        public static ConfigEntry<bool> PlayersMultiplier { get; private set; }
        public static ConfigEntry<bool> ClearDropTable { get; private set; }
        public static ConfigEntry<bool> MinionDamage { get; private set; }
        public static ConfigEntry<bool> RandomBoss { get; private set; }
        public static ConfigEntry<bool> BuffAfterKillingEnabled { get; private set; }
        public static ConfigEntry<int> BuffAfterKillingPrefabGUID { get; private set; }
        public static ConfigEntry<bool> TeamBossEnable { get; private set; }

        // Dynamic Scaling Configuration
        public static ConfigEntry<bool> EnableDynamicScaling { get; private set; }
        public static ConfigEntry<float> BaseHealthMultiplier { get; private set; }
        public static ConfigEntry<float> HealthPerPlayer { get; private set; }
        public static ConfigEntry<float> DamagePerPlayer { get; private set; }
        public static ConfigEntry<int> MaxPlayersForScaling { get; private set; }

        // Progressive Difficulty Configuration
        public static ConfigEntry<bool> EnableProgressiveDifficulty { get; private set; }
        public static ConfigEntry<float> DifficultyIncrease { get; private set; }
        public static ConfigEntry<float> MaxDifficultyMultiplier { get; private set; }
        public static ConfigEntry<bool> ResetDifficultyOnKill { get; private set; }

        // Teleport Configuration
        public static ConfigEntry<bool> EnableTeleportCommand { get; private set; }
        public static ConfigEntry<bool> TeleportAdminOnly { get; private set; }
        public static ConfigEntry<float> TeleportCooldown { get; private set; }
        public static ConfigEntry<bool> TeleportOnlyToActiveBosses { get; private set; }
        public static ConfigEntry<bool> TeleportRequireBossAlive { get; private set; }
        public static ConfigEntry<string> TeleportCostItemGUID { get; private set; }
        public static ConfigEntry<int> TeleportCostAmount { get; private set; }
        
        // Phase Announcement Configuration
        public static ConfigEntry<bool> EnablePhaseAnnouncements { get; private set; }
        public static ConfigEntry<bool> AnnounceEveryPhase { get; private set; }
        public static ConfigEntry<bool> AnnounceMilestoneSpawns { get; private set; }
        
        // Phase Message Templates
        public static ConfigEntry<string> PhaseNormalTemplate { get; private set; }
        public static ConfigEntry<string> PhaseHardTemplate { get; private set; }
        public static ConfigEntry<string> PhaseEpicTemplate { get; private set; }
        public static ConfigEntry<string> PhaseLegendaryTemplate { get; private set; }
        public static ConfigEntry<string> PhaseVeteranSuffix { get; private set; }
        public static ConfigEntry<string> PhaseEnragedSuffix { get; private set; }
        public static ConfigEntry<string> PhaseEpicPrefix { get; private set; }
        public static ConfigEntry<string> PhaseLegendaryPrefix { get; private set; }
        
        // Phase Names (translatable)
        public static ConfigEntry<string> PhaseNormalName { get; private set; }
        public static ConfigEntry<string> PhaseHardName { get; private set; }
        public static ConfigEntry<string> PhaseEpicName { get; private set; }
        public static ConfigEntry<string> PhaseLegendaryName { get; private set; }
        
        // Additional Message Templates
        public static ConfigEntry<string> ConsecutiveInfoTemplate { get; private set; }

        public static void Initialize()
        {

            var bepInExConfigFolder = BepInEx.Paths.ConfigPath ?? Path.Combine("BepInEx", "config");
            var configFolder = Path.Combine(bepInExConfigFolder, "BloodyBoss");
            if (!Directory.Exists(configFolder))
            {
                Directory.CreateDirectory(configFolder);
            }
            var mainConfigFilePath = Path.Combine(bepInExConfigFolder, "BloodyBoss.cfg");
            _mainConfig = File.Exists(mainConfigFilePath) ? new ConfigFile(mainConfigFilePath, false) : new ConfigFile(mainConfigFilePath, true);

            Enabled = _mainConfig.Bind("Main", "Enabled", true, "Determines whether the boss spawn timer is enabled or not.");
            KillMessageBossTemplate = _mainConfig.Bind("Main", "KillMessageBossTemplate", "The #vblood# boss has been defeated by the following brave warriors:", "The message that will appear globally once the boss gets killed.");
            SpawnMessageBossTemplate = _mainConfig.Bind("Main", "SpawnMessageBossTemplate", "A Boss #worldbossname# has been summon you got #time# minutes to defeat it!.", "The message that will appear globally one the boss gets spawned.");
            DespawnMessageBossTemplate = _mainConfig.Bind("Main", "DespawnMessageBossTemplate", "You failed to kill the Boss #worldbossname# in time.", "The message that will appear globally if the players failed to kill the boss.");
            BuffForWorldBoss = _mainConfig.Bind("Main", "BuffForWorldBoss", 1163490655, "Buff that applies to each of the Bosses that we create with our mod.");
            PlayersMultiplier = _mainConfig.Bind("Main", "PlayersOnlineMultiplier", false, "If you activate this option, the boss life formula changes from \"bosslife * multiplier\" to \"bosslife * multiplier * numberofonlineplayers\".");
            ClearDropTable = _mainConfig.Bind("Main", "ClearDropTable", false, "If you activate this option it will remove the original vblood droptable.");
            MinionDamage = _mainConfig.Bind("Main", "MinionDamage", true, "Disable minion damage to bosses.");
            RandomBoss = _mainConfig.Bind("Main", "RandomBoss", false, "If you activate this option instead of spawning a specific boss at a specific time, the system will search for a random boss and spawn the random boss instead of the original boss at the original boss's specific time..");
            BuffAfterKillingEnabled = _mainConfig.Bind("Main", "BuffAfterKillingEnabled", true, "Deactivates the buff animation received by players who have participated in the battle for three seconds.");
            BuffAfterKillingPrefabGUID = _mainConfig.Bind("Main", "BuffAfterKillingPrefabGUID", -2061047741, "PrefabGUID of the buff received by players who have participated in the battle for three seconds.");
            TeamBossEnable = _mainConfig.Bind("Main", "TeamBossEnable", false, "If you activate this option, the bosses will not attack each other and will team up if two bosses are summoned together..");

            // Dynamic Scaling Configuration
            EnableDynamicScaling = _mainConfig.Bind("Dynamic Scaling", "Enable", false, "Enable dynamic scaling based on online players");
            BaseHealthMultiplier = _mainConfig.Bind("Dynamic Scaling", "BaseHealthMultiplier", 1.0f, "Base health multiplier for all bosses");
            HealthPerPlayer = _mainConfig.Bind("Dynamic Scaling", "HealthPerPlayer", 0.25f, "Additional health percentage per online player (0.25 = +25%)");
            DamagePerPlayer = _mainConfig.Bind("Dynamic Scaling", "DamagePerPlayer", 0.15f, "Additional damage percentage per online player (0.15 = +15%)");
            MaxPlayersForScaling = _mainConfig.Bind("Dynamic Scaling", "MaxPlayersForScaling", 10, "Maximum players considered for scaling");

            // Progressive Difficulty Configuration
            EnableProgressiveDifficulty = _mainConfig.Bind("Progressive Difficulty", "Enable", false, "Enable progressive difficulty increase");
            DifficultyIncrease = _mainConfig.Bind("Progressive Difficulty", "DifficultyIncrease", 0.1f, "Difficulty increase per consecutive spawn (0.1 = +10%)");
            MaxDifficultyMultiplier = _mainConfig.Bind("Progressive Difficulty", "MaxDifficultyMultiplier", 2.0f, "Maximum difficulty multiplier (2.0 = 200%)");
            ResetDifficultyOnKill = _mainConfig.Bind("Progressive Difficulty", "ResetDifficultyOnKill", true, "Reset difficulty when boss is killed");

            // Teleport Configuration
            EnableTeleportCommand = _mainConfig.Bind("Teleport", "Enable", true, "Enable teleport to boss command");
            TeleportAdminOnly = _mainConfig.Bind("Teleport", "AdminOnly", true, "Restrict teleport command to admins only");
            TeleportCooldown = _mainConfig.Bind("Teleport", "CooldownSeconds", 60.0f, "Cooldown between teleports in seconds (0 = no cooldown)");
            TeleportOnlyToActiveBosses = _mainConfig.Bind("Teleport", "OnlyToActiveBosses", true, "Only allow teleport to currently spawned bosses");
            TeleportRequireBossAlive = _mainConfig.Bind("Teleport", "RequireBossAlive", true, "Require boss to be alive (not dead) for teleport");
            TeleportCostItemGUID = _mainConfig.Bind("Teleport", "CostItemGUID", "0", "PrefabGUID of item required for teleport (0 = no cost)");
            TeleportCostAmount = _mainConfig.Bind("Teleport", "CostAmount", 1, "Amount of cost item required");
            
            // Phase Announcement Configuration
            EnablePhaseAnnouncements = _mainConfig.Bind("Phase Announcements", "Enable", true, "Enable boss phase change announcements");
            AnnounceEveryPhase = _mainConfig.Bind("Phase Announcements", "AnnounceEveryPhase", false, "Announce every phase change (not just increases)");
            AnnounceMilestoneSpawns = _mainConfig.Bind("Phase Announcements", "AnnounceMilestoneSpawns", true, "Announce milestone consecutive spawns (every 3)");
            
            // Phase Message Templates
            PhaseNormalTemplate = _mainConfig.Bind("Phase Messages", "NormalTemplate", "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage#", "Template for normal phase messages. Placeholders: #bossname#, #phasename#, #phase#, #players#, #damage#, #health#, #consecutive#");
            PhaseHardTemplate = _mainConfig.Bind("Phase Messages", "HardTemplate", "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage#", "Template for hard phase messages");
            PhaseEpicTemplate = _mainConfig.Bind("Phase Messages", "EpicTemplate", "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage##consecutive_info#", "Template for epic phase messages");
            PhaseLegendaryTemplate = _mainConfig.Bind("Phase Messages", "LegendaryTemplate", "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage##consecutive_info#", "Template for legendary phase messages");
            PhaseVeteranSuffix = _mainConfig.Bind("Phase Messages", "VeteranSuffix", " Veteran", "Suffix added to phase names for veteran bosses (3+ consecutive spawns)");
            PhaseEnragedSuffix = _mainConfig.Bind("Phase Messages", "EnragedSuffix", " Enraged", "Suffix added to phase names for enraged bosses (5+ consecutive spawns)");
            PhaseEpicPrefix = _mainConfig.Bind("Phase Messages", "EpicPrefix", "⚡ EPIC ENCOUNTER! ", "Prefix added to epic phase messages");
            PhaseLegendaryPrefix = _mainConfig.Bind("Phase Messages", "LegendaryPrefix", "💀 LEGENDARY THREAT! ", "Prefix added to legendary phase messages");
            
            // Phase Names (translatable)
            PhaseNormalName = _mainConfig.Bind("Phase Names", "Normal", "Normal", "Name for normal difficulty phase");
            PhaseHardName = _mainConfig.Bind("Phase Names", "Hard", "Hard", "Name for hard difficulty phase");
            PhaseEpicName = _mainConfig.Bind("Phase Names", "Epic", "Epic", "Name for epic difficulty phase");
            PhaseLegendaryName = _mainConfig.Bind("Phase Names", "Legendary", "Legendary", "Name for legendary difficulty phase");
            
            // Additional Message Templates
            ConsecutiveInfoTemplate = _mainConfig.Bind("Phase Messages", "ConsecutiveInfoTemplate", " | Consecutive: #consecutive#", "Template for consecutive spawn information. Use #consecutive# placeholder");
        }
        public static void Destroy()
        {
            _mainConfig.Clear();
        }

        private static string CleanupName(string name)
        {
            var rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(name, "");
        }

    }
}
