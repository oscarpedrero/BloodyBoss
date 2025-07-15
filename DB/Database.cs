using BepInEx;
using Bloody.Core;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using BloodyBoss.Systems;
using ProjectM;
using Stunlock.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Unity.Collections;
using Unity.Entities;

namespace BloodyBoss.DB
{
    internal static class Database
    {
        private static readonly Random Random = new();

        public static readonly string ConfigPath = Path.Combine(Paths.ConfigPath, "BloodyBoss");
        public static string BOSSESListFile = Path.Combine(ConfigPath, "Bosses.json");

        public static Team? TeamDefault { get; set; } = null;
        public static TeamReference? TeamReferenceDefault { get; set; } = null;

        public static List<BossEncounterModel> BOSSES { get; set; } = new();

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
            Plugin.BLogger.Debug(LogCategory.Database, $"Create Database: OK");
            return true;
        }

        public static bool saveDatabase()
        {
            try
            {
                var jsonOutPut = JsonSerializer.Serialize(BOSSES, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(BOSSESListFile, jsonOutPut);
                Plugin.BLogger.Debug(LogCategory.Database, $"Save Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.BLogger.Error(LogCategory.Database, $"Error SaveDatabase: {error.Message}");
                return false;
            }
        }

        public static bool loadDatabase()
        {
            try
            {
                string json = File.ReadAllText(BOSSESListFile);
                BOSSES = JsonSerializer.Deserialize<List<BossEncounterModel>>(json);
                Plugin.BLogger.Debug(LogCategory.Database, $"Load Database: OK");
                return true;
            }
            catch (Exception error)
            {
                Plugin.BLogger.Error(LogCategory.Database, $"Error LoadDatabase: {error.Message}");
                
                // Backup corrupted file instead of deleting it
                if (File.Exists(BOSSESListFile))
                {
                    string backupFile = Path.Combine(ConfigPath, $"Bosses-{DateTime.Now:yyyy-MM-dd-HHmmss}.json.backup");
                    try
                    {
                        File.Move(BOSSESListFile, backupFile);
                        Plugin.BLogger.Warning(LogCategory.Database, $"Corrupted Bosses.json backed up to: {backupFile}");
                        
                        // Create new empty file
                        File.WriteAllText(BOSSESListFile, "[]");
                        BOSSES = new List<BossEncounterModel>();
                        Plugin.BLogger.Info(LogCategory.Database, $"Created new empty Bosses.json file");
                        return true;
                    }
                    catch (Exception backupError)
                    {
                        Plugin.BLogger.Error(LogCategory.Database, $"Error creating backup: {backupError.Message}");
                    }
                }
                
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
            Entity bossEntity = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(prefabGUIDOfNPC)];
            boss = new BossEncounterModel();
            boss.name = NPCName;
            boss.nameHash = System.Guid.NewGuid().ToString("N"); // Use GUID instead of GetHashCode to prevent collisions
            boss.PrefabGUID = prefabGUIDOfNPC;
            boss.AssetName = assetName;
            boss.level = level;
            boss.multiplier = multiplier;
            boss.Lifetime = lifetime;

            var BossUnitStats = bossEntity.Read<UnitStats>();
            boss.unitStats = new UnitStatsModel();
            boss.unitStats.SetStats(BossUnitStats);


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
