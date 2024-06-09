using System.IO;
using System.Text.RegularExpressions;
using BepInEx.Configuration;
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
            VBloodFinalConcatCharacters = _mainConfig.Bind("Main", "WorldBossFinalConcatCharacters", "and", "Final string for concat two or more players kill a WorldBoss Boss.");
            PlayersMultiplier = _mainConfig.Bind("Main", "PlayersOnlineMultiplier", false, "If you activate this option, the boss life formula changes from \"bosslife * multiplier\" to \"bosslife * multiplier * numberofonlineplayers\".");
            ClearDropTable = _mainConfig.Bind("Main", "ClearDropTable", false, "If you activate this option it will remove the original vblood droptable.");
            MinionDamage = _mainConfig.Bind("Main", "MinionDamage", true, "Disable minion damage to bosses.");
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
