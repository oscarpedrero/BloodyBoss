using BepInEx;
using Bloodstone.API;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using ProjectM;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Unity.Entities;

namespace BloodyBoss.DB
{
    internal static class Database
    {
        private static readonly Random Random = new();

        public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "BloodyBoss");
        public static string BOSSESListFile = Path.Combine(ConfigPath, "Bosses.json");
        public static string LocalizationsFile = Path.Combine(ConfigPath, "Localization.json"); // Add localization file

        public static List<BossEncounterModel> BOSSES { get; set; } = new();
        public static Dictionary<string,string> LOCALIZATIONS { get; set; } = new();

        public static void Initialize()
        {
            createDatabaseFiles();
            loadDatabase();
        }

        /*
         * 
         * 
         * DATABASE
         * 
         * 
         * 
         */

        public static bool createDatabaseFiles()
        {
            if (!Directory.Exists(ConfigPath)) Directory.CreateDirectory(ConfigPath);
            if (!File.Exists(BOSSESListFile)) File.WriteAllText(BOSSESListFile, "[]");
                        if (!File.Exists(LocalizationsFile))
            {
                File.WriteAllText(LocalizationsFile, "{}"); // Add localization file
                saveLocalizationDatabase();
            }

            Plugin.Logger.LogDebug($"Create Database: OK");
            return true;
        }

                // Save Localization in file
        public static bool saveLocalizationDatabase()
        {
            try
            {
                var _jsonOutPut = JsonSerializer.Serialize(Config.Localization, new JsonSerializerOptions { WriteIndented = true});
                File.WriteAllText(LocalizationsFile, _jsonOutPut);
                Plugin.Logger.LogDebug($"Save Localization Database: OK");
                return true;
            }
            catch(Exception error)
            {
                Plugin.Logger.LogError($"Error Saving Localization Database: {error.Message}");
                return false;
            }
        }

        public static bool saveDatabase()
        {
            try
            {
                var jsonOutPut = JsonSerializer.Serialize(BOSSES, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(BOSSESListFile, jsonOutPut);
                Plugin.Logger.LogDebug($"Save Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error SaveDatabase: {error.Message}");
                return false;
            }
        }

        public static bool loadDatabase()
        {
            try
            {
                string json = File.ReadAllText(BOSSESListFile);
                string localizationJson = File.ReadAllText(LocalizationsFile);

                BOSSES = JsonSerializer.Deserialize<List<BossEncounterModel>>(json);
                LOCALIZATIONS = JsonSerializer.Deserialize<Dictionary<string,string>>(localizationJson);

                Plugin.Logger.LogDebug($"Load Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.Logger.LogError($"Error LoadDatabase: {error.Message}");
                return false;
            }
        }


        public static bool GetBoss(string NPCName, out BossEncounterModel boss)
        {
            boss = BOSSES.Where(x => x.name == NPCName).FirstOrDefault();
            if (boss == null)
            {
                return false;
            }
            return true;
        }

        public static bool GetBOSSFromEntity(Entity npcEntity , out BossEncounterModel boss)
        {
            boss = BOSSES.Where(x => x.bossEntity.Equals(npcEntity)).FirstOrDefault();
            if (boss == null)
            {
                return false;
            }
            return true;
        }

        public static bool AddBoss(string NPCName, int prefabGUIDOfNPC, int level, int multiplier, int lifetime)
        {
            if (GetBoss(NPCName, out BossEncounterModel boss))
            {
                throw new BossExistException();
            }

            var assetName = Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup[new PrefabGUID(prefabGUIDOfNPC)].AssetName.ToString();
            boss = new BossEncounterModel();
            boss.name = NPCName;
            boss.nameHash = NPCName.GetHashCode().ToString();
            boss.PrefabGUID = prefabGUIDOfNPC;
            boss.AssetName = assetName;
            boss.level = level;
            boss.multiplier = multiplier;
            boss.Lifetime = lifetime;


            BOSSES.Add(boss);
            saveDatabase();
            return true;

        }

        public static bool RemoveBoss(string BossName)
        {
            if (GetBoss(BossName, out BossEncounterModel boss))
            {

                BOSSES.Remove(boss);
                saveDatabase();
                return true;
            }

            throw new BossDontExistException();
        }

        public static T GetRandomItem<T>(this List<T> items)
        {
            if (items == null || items.Count == 0)
            {
                return default;
            }

            return items[Random.Next(items.Count)];
        }


    }
}
