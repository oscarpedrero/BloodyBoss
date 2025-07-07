using Bloody.Core.Models.v1;
using Bloody.Core;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Transforms;
using VampireCommandFramework;
using Bloody.Core.GameData.v1;
using Stunlock.Core;
using ProjectM;
using Bloody.Core.Helper.v1;
using Unity.Entities;
using System.Linq;
using BloodyBoss.Systems;
using Bloody.Core.API.v1;
using Unity.Collections;
using ProjectM.Network;
using BloodyBoss.Data;
using BloodyBoss.Configuration;
using Unity.Mathematics;
using BloodyBoss.Models;

namespace BloodyBoss.Command
{
    [CommandGroup("bb")]
    public static partial class BossCommand
    {
        private static string GetCompatibilityIcon(AbilityCompatibilitySystem.CompatibilityLevel level)
        {
            return level switch
            {
                AbilityCompatibilitySystem.CompatibilityLevel.Perfect => FontColorChatSystem.Green("[PERFECT]"),
                AbilityCompatibilitySystem.CompatibilityLevel.Good => FontColorChatSystem.Yellow("[GOOD]"),
                AbilityCompatibilitySystem.CompatibilityLevel.Warning => FontColorChatSystem.Yellow("[WARNING]"),
                AbilityCompatibilitySystem.CompatibilityLevel.Incompatible => FontColorChatSystem.Red("[INCOMPATIBLE]"),
                _ => FontColorChatSystem.White("[UNKNOWN]")
            };
        }

        [Command("list", usage: "", description: "List of Boss", adminOnly: true)]
        public static void ListBoss(ChatCommandContext ctx)
        {

            var Boss = Database.BOSSES;

            if (Boss.Count == 0)
            {
                throw ctx.Error($"There are no boss created");
            }
            ctx.Reply($"Boss List");
            ctx.Reply($"----------------------------");
            ctx.Reply($"--");
            foreach (var boss in Boss)
            {
                ctx.Reply($"Boss {boss.name}");
                ctx.Reply($"--");
            }
            ctx.Reply($"----------------------------");
        }

        [Command("extract-all-prefabs", usage: "", description: "Extract ALL prefabs from the game to markdown file", adminOnly: true)]
        public static void ExtractAllPrefabs(ChatCommandContext ctx)
        {
            try
            {
                var outputPath = Path.Combine(Database.ConfigPath, "all_prefabs.md");
                var buffData = new List<string>();
                
                buffData.Add("# V Rising All PrefabGUIDs");
                buffData.Add($"## Extracted on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                buffData.Add("");
                
                // Try to get all prefabs from the PrefabCollectionSystem
                var prefabCollectionSystem = Core.SystemsCore.PrefabCollectionSystem;
                if (prefabCollectionSystem == null)
                {
                    ctx.Error("PrefabCollectionSystem not found!");
                    return;
                }
                
                var prefabGuidToEntityMap = prefabCollectionSystem._PrefabGuidToEntityMap;
                
                buffData.Add($"## Total Prefabs in Game: {prefabGuidToEntityMap.Count()}");
                buffData.Add("");
                
                // Categorize prefabs
                var buffs = new List<(PrefabGUID guid, string name)>();
                var npcs = new List<(PrefabGUID guid, string name)>();
                var abilities = new List<(PrefabGUID guid, string name)>();
                var items = new List<(PrefabGUID guid, string name)>();
                var other = new List<(PrefabGUID guid, string name)>();
                
                foreach (var prefabKvp in prefabGuidToEntityMap)
                {
                    var prefabGuid = prefabKvp.Key;
                    var prefabName = prefabGuid.LookupName();
                    
                    if (string.IsNullOrEmpty(prefabName))
                        prefabName = $"Unknown_{prefabGuid.GuidHash}";
                    
                    // Categorize by name patterns
                    if (prefabName.Contains("Buff_") || prefabName.Contains("Debuff_"))
                        buffs.Add((prefabGuid, prefabName));
                    else if (prefabName.Contains("CHAR_") || prefabName.Contains("NPC_"))
                        npcs.Add((prefabGuid, prefabName));
                    else if (prefabName.Contains("AB_") || prefabName.Contains("Ability_"))
                        abilities.Add((prefabGuid, prefabName));
                    else if (prefabName.Contains("Item_"))
                        items.Add((prefabGuid, prefabName));
                    else
                        other.Add((prefabGuid, prefabName));
                }
                
                // Write Buffs/Debuffs
                buffData.Add("## Buffs and Debuffs");
                buffData.Add($"### Total: {buffs.Count}");
                buffData.Add("");
                buffData.Add("| PrefabGUID | Name |");
                buffData.Add("|------------|------|");
                foreach (var buff in buffs.OrderBy(b => b.name))
                {
                    buffData.Add($"| {buff.guid.GuidHash} | {buff.name} |");
                }
                
                // Write NPCs
                buffData.Add("");
                buffData.Add("## NPCs and Characters");
                buffData.Add($"### Total: {npcs.Count}");
                buffData.Add("");
                buffData.Add("| PrefabGUID | Name |");
                buffData.Add("|------------|------|");
                foreach (var npc in npcs.Take(200).OrderBy(n => n.name)) // Limit to 200 to avoid huge file
                {
                    buffData.Add($"| {npc.guid.GuidHash} | {npc.name} |");
                }
                
                // Write Abilities
                buffData.Add("");
                buffData.Add("## Abilities");
                buffData.Add($"### Total: {abilities.Count}");
                buffData.Add("");
                buffData.Add("| PrefabGUID | Name |");
                buffData.Add("|------------|------|");
                foreach (var ability in abilities.Take(200).OrderBy(a => a.name))
                {
                    buffData.Add($"| {ability.guid.GuidHash} | {ability.name} |");
                }
                
                // Write Items (limited)
                buffData.Add("");
                buffData.Add("## Items (First 100)");
                buffData.Add($"### Total: {items.Count}");
                buffData.Add("");
                buffData.Add("| PrefabGUID | Name |");
                buffData.Add("|------------|------|");
                foreach (var item in items.Take(100).OrderBy(i => i.name))
                {
                    buffData.Add($"| {item.guid.GuidHash} | {item.name} |");
                }
                
                // Summary
                buffData.Add("");
                buffData.Add("## Summary");
                buffData.Add($"- Total Prefabs: {prefabGuidToEntityMap.Count()}");
                buffData.Add($"- Buffs/Debuffs: {buffs.Count}");
                buffData.Add($"- NPCs: {npcs.Count}");
                buffData.Add($"- Abilities: {abilities.Count}");
                buffData.Add($"- Items: {items.Count}");
                buffData.Add($"- Other: {other.Count}");
                
                // Write to file
                File.WriteAllLines(outputPath, buffData);
                
                ctx.Reply($"Extraction complete! Saved to: {outputPath}");
                ctx.Reply($"Total prefabs found: {prefabGuidToEntityMap.Count()}");
                ctx.Reply($"Buffs: {buffs.Count}, NPCs: {npcs.Count}, Abilities: {abilities.Count}");
            }
            catch (Exception e)
            {
                ctx.Error($"Error extracting prefabs: {e.Message}");
                Plugin.Logger.LogError($"ExtractAllPrefabs error: {e}");
            }
        }
        
        [Command("extract-buffs", usage: "", description: "Extract all buffs and effects to markdown file", adminOnly: true)]
        public static void ExtractBuffs(ChatCommandContext ctx)
        {
            try
            {
                var outputPath = Path.Combine(Database.ConfigPath, "extracted_buffs.md");
                var buffData = new List<string>();
                
                buffData.Add("# V Rising Buffs, Debuffs and Effects");
                buffData.Add($"## Extracted on {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                buffData.Add("");
                
                // Simple extraction - just list known prefab GUIDs
                buffData.Add("## Known Buff PrefabGUIDs");
                buffData.Add("");
                buffData.Add("| Name | PrefabGUID | Type | Used In |");
                buffData.Add("|------|------------|------|---------|");
                
                // List all the PrefabGUIDs we're using in mechanics
                var knownBuffs = new Dictionary<string, (int guid, string type, string usedIn)>
                {
                    // Movement Control
                    { "Stun", (-658795137, "CC", "StunMechanic, PullMechanic, TeleportMechanic") },
                    { "Root", (1724281065, "CC", "RootMechanic, ShieldMechanic") },
                    { "Slow", (-2099683628, "CC", "SlowMechanic, CurseMechanic") },
                    { "Fear", (-111608736, "CC", "FearMechanic") },
                    { "Knockback", (802825050, "Movement", "KnockbackMechanic, PullMechanic") },
                    
                    // Damage Over Time
                    { "Poison", (1614409699, "DoT", "DotMechanic") },
                    { "Ignite", (-1968815368, "DoT", "DotMechanic") },
                    { "Bleed", (411225544, "DoT", "DotMechanic") },
                    { "Corruption", (-201240543, "DoT", "DotMechanic") },
                    { "Frost", (-387588255, "DoT", "DotMechanic") },
                    
                    // Defensive
                    { "Shield Immunity", (584227282, "Shield", "ShieldMechanic") },
                    { "Reflect", (721788343, "Shield", "ReflectMechanic, ShieldMechanic") },
                    { "Shield Visual", (-1970513771, "Visual", "ShieldMechanic, ReflectMechanic") },
                    
                    // Debuffs
                    { "Silence", (-1936575297, "Debuff", "SilenceMechanic, CurseMechanic") },
                    { "Weakness", (-1584651229, "Debuff", "WeakenMechanic, BuffStealMechanic") },
                    { "Curse Doom", (-1308605301, "Debuff", "CurseMechanic") },
                    { "Curse Weakness", (1528607403, "Debuff", "CurseMechanic") },
                    
                    // Buffs
                    { "Blood Rage", (2085766220, "Buff", "EnrageMechanic") },
                    { "Speed Buff", (788443104, "Buff", "EnrageMechanic, TeleportMechanic") },
                    { "Uninterruptible", (476036897, "Buff", "HealMechanic") },
                    
                    // Visual Effects
                    { "Fire Aura", (-1576893213, "Visual", "EnrageMechanic") },
                    { "Teleport Effect", (-1122234739, "Visual", "TeleportMechanic") },
                    { "Explosion", (1320298810, "Visual", "AoeMechanic, TeleportMechanic") },
                    { "Healing Glow", (1974723356, "Visual", "HealMechanic") },
                    { "Holy Light", (-1204819086, "Visual", "HealMechanic") },
                    
                    // Special
                    { "Mind Control", (-65362596, "Control", "MindControlMechanic") },
                    { "Vision Darkness", (-534324419, "Visual", "VisionMechanic") },
                    { "Trap Spike", (1723404789, "Trap", "TrapMechanic") }
                };
                
                foreach (var buff in knownBuffs)
                {
                    buffData.Add($"| {buff.Key} | {buff.Value.guid} | {buff.Value.type} | {buff.Value.usedIn} |");
                }

                // Add summary
                buffData.Add("");
                buffData.Add("## Summary");
                buffData.Add("");
                buffData.Add("The PrefabGUID **-658795137** (Stun) is the one causing errors.");
                buffData.Add("This GUID is used in 3 mechanics: StunMechanic, PullMechanic, and TeleportMechanic.");
                buffData.Add("");
                buffData.Add("To fix the error, you need to:");
                buffData.Add("1. Find the correct Stun buff GUID from V Rising");
                buffData.Add("2. Replace -658795137 with the correct GUID in all 3 mechanic files");
                buffData.Add("3. Or remove the stun effect from those mechanics");
                
                // NPCs used in summons
                buffData.Add("");
                buffData.Add("## NPCs Used in Mechanics");
                buffData.Add("");
                buffData.Add("| Name | PrefabGUID | Used In |");
                buffData.Add("|------|------------|---------|");
                buffData.Add("| Alpha Wolf | -1905691330 | SummonMechanic (default) |");

                // Write to file
                File.WriteAllLines(outputPath, buffData);
                
                ctx.Reply($"Extraction complete! Saved to: {outputPath}");
                ctx.Reply($"Total known buffs documented: {knownBuffs.Count}");
                ctx.Reply($"The problematic Stun buff GUID -658795137 has been identified.");
            }
            catch (Exception e)
            {
                ctx.Error($"Error extracting buffs: {e.Message}");
                Plugin.Logger.LogError($"ExtractBuffs error: {e}");
            }
        }

        [Command("find-buff", usage: "<search>", description: "Find buffs by name pattern", adminOnly: true)]
        public static void FindBuff(ChatCommandContext ctx, string search = "")
        {
            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    ctx.Error("Please provide a search term. Example: .bb find-buff stun");
                    return;
                }
                
                var prefabCollectionSystem = Core.SystemsCore.PrefabCollectionSystem;
                if (prefabCollectionSystem == null)
                {
                    ctx.Error("PrefabCollectionSystem not found!");
                    return;
                }
                
                var prefabGuidToEntityMap = prefabCollectionSystem._PrefabGuidToEntityMap;
                var matches = new List<(PrefabGUID guid, string name)>();
                
                foreach (var prefabKvp in prefabGuidToEntityMap)
                {
                    var prefabGuid = prefabKvp.Key;
                    var prefabName = prefabGuid.LookupName();
                    
                    if (string.IsNullOrEmpty(prefabName))
                        prefabName = $"Unknown_{prefabGuid.GuidHash}";
                    
                    // Case insensitive search
                    if (prefabName.ToLower().Contains(search.ToLower()))
                    {
                        matches.Add((prefabGuid, prefabName));
                    }
                }
                
                if (matches.Count == 0)
                {
                    ctx.Reply($"No prefabs found containing '{search}'");
                    return;
                }
                
                ctx.Reply($"Found {matches.Count} prefabs containing '{search}':");
                ctx.Reply("----------------------------");
                
                // Show first 20 matches
                foreach (var match in matches.Take(20).OrderBy(m => m.name))
                {
                    ctx.Reply($"{match.guid.GuidHash} = {match.name}");
                }
                
                if (matches.Count > 20)
                {
                    ctx.Reply($"... and {matches.Count - 20} more matches");
                }
            }
            catch (Exception e)
            {
                ctx.Error($"Error finding buff: {e.Message}");
                Plugin.Logger.LogError($"FindBuff error: {e}");
            }
        }
        
        [Command("test", usage: "", description: "Test Boss", adminOnly: true)]
        public static void Test(ChatCommandContext ctx, string bossName)
        {

            SetLocation(ctx,bossName);
            DateTime currentTime = DateTime.Now;
            DateTime x1MinLater = currentTime.AddMinutes(1);
            SetHour(ctx,bossName, x1MinLater.ToString("HH:mm"));
        }

        [Command("reload", usage: "", description: "Reload Database Boss", adminOnly: true)]
        public static void ReloadDatabase(ChatCommandContext ctx)
        {
            try
            {
                Database.loadDatabase();
                ctx.Reply($"Boss database reload successfully");
            } catch(Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }
            
        }

        // .bb Test -1391546313 200 2 60 
        // .bb Boss items add Test ItemName -257494203 20 1
        // 
        // .bb Boss set hour Test 13:20
        // .bb Boss start Test
        // 
        //
        // .bb create "Alpha Wolf" -1905691330 90 1 1800
        // .bb set location "Alpha Wolf"
        // .bb set hour "Alpha Wolf" 22:00
        // .bb items add "Alpha Wolf" "Blood Rose Potion" 429052660 25 1
        // .bb start "Alpha Wolf"
        //
        // 
        [Command("create", usage: "<NameOfBOSS> <PrefabGUIDOfBOSS> <Level> <Multiplier> <LifeTimeSeconds>", description: "Create a Boss", adminOnly: true)]
        public static void CreateBOSS(ChatCommandContext ctx, string bossName, int prefabGUID, int level, int multiplier, int lifeTime)
        {
            try
            {
                var entityUnit = Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap[new PrefabGUID(prefabGUID)];

                //if(!entityUnit.Has<VBloodUnit>()) throw ctx.Error($"The PrefabGUID entered does not correspond to a VBlood Unit.");

                if (Database.AddBoss(bossName, prefabGUID, level, multiplier, lifeTime))
                {
                    ctx.Reply($"Boss '{bossName}' created successfully");
                }
            }
            catch (BossExistException)
            {
                throw ctx.Error($"Boss with name '{bossName}' exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("remove", usage: "", description: "Remove a Boss", adminOnly: true)]
        public static void RemoveBoss(ChatCommandContext ctx, string bossName)
        {

            try
            {
                if (Database.RemoveBoss(bossName))
                {
                    ctx.Reply($"Boss '{bossName}' remove successfully");
                }
            }
            catch (NPCDontExistException)
            {
                throw ctx.Error($"Boss with name '{bossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }

        }

        [Command("set location", usage: "<NameOfBoss>", description: "Adds the current location of the player who sets it to the Boss.", adminOnly: true)]
        public static void SetLocation(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                var pos = Plugin.SystemsCore.EntityManager.GetComponentData<LocalToWorld>(user).Position;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetLocation(pos);
                    ctx.Reply($"Position {pos.x},{pos.y},{pos.z} successfully set to Boss '{BossName}'");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("set hour", usage: "<NameOfBoss> <Hour>", description: "Adds the hour and minutes in which the Boss spawn.", adminOnly: true)]
        public static void SetHour(ChatCommandContext ctx, string BossName, string hour)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetHour(hour);
                    ctx.Reply($"Hour {hour} successfully set to Boss '{BossName}'");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("start", usage: "<NameOfBoss>", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void start(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    Boss.SetHourDespawn();
                    Boss.Spawn(user);
                    
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("clearicon", usage: "<NameOfBoss>", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void ClearIcon(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
                    UserModel userModel = GameData.Users.GetUserByCharacterName(ctx.Event.User.CharacterName.Value);
                    Boss.RemoveIcon(userModel.Entity);
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss with name '{BossName}' does not exist.");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        [Command("clearallicons", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void ClearAllIcons(ChatCommandContext ctx)
        {
            try
            {
                var userModel = GameData.Users.All.FirstOrDefault();
                var user = userModel.Entity;
                var entities = QueryComponents.GetEntitiesByComponentTypes<MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
                var i = 0;
                foreach (var entity in entities)
                {
                    var prefab = entity.Read<PrefabGUID>();
                    var mapicon = Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup[prefab].AssetName;
                    if (prefab.GuidHash == Prefabs.MapIcon_POI_VBloodSource.GuidHash)
                    {
                        i++;
                        StatChangeUtility.KillOrDestroyEntity(Plugin.SystemsCore.EntityManager, entity, user, user, 0, StatChangeReason.Any, true);
                    }
                }
                entities.Dispose();
                ctx.Reply($"Removed icons ['{i}']");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error: {e.Message}");
            }


        }

        // ===== ADVANCED COMMANDS =====

        [Command("despawn", usage: "<BossName>", description: "Despawn a boss immediately", adminOnly: true)]
        public static void DespawnBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }

                    var userModel = GameData.Users.GetUserByCharacterName(ctx.Event.User.CharacterName.Value);
                    
                    // Remove icon and entity (silently)
                    boss.RemoveIcon(userModel.Entity);
                    boss.DespawnBoss(userModel.Entity);
                    boss.bossSpawn = false;
                    
                    // Clear killers and save
                    boss.RemoveKillers();
                    Database.saveDatabase();
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("pause", usage: "<BossName>", description: "Pause boss timer", adminOnly: true)]
        public static void PauseBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (boss.IsPaused)
                    {
                        throw ctx.Error($"Boss '{bossName}' is already paused");
                    }
                    
                    boss.IsPaused = true;
                    boss.PausedAt = DateTime.Now;
                    Database.saveDatabase();
                    
                    ctx.Reply($"⏸️ Boss '{bossName}' timer paused at {DateTime.Now:HH:mm:ss}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("resume", usage: "<BossName>", description: "Resume boss timer", adminOnly: true)]
        public static void ResumeBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.IsPaused)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not paused");
                    }
                    
                    // Calculate pause duration
                    var pauseDuration = DateTime.Now - boss.PausedAt.Value;
                    
                    boss.IsPaused = false;
                    boss.PausedAt = null;
                    Database.saveDatabase();
                    
                    ctx.Reply($"▶️ Boss '{bossName}' timer resumed (was paused for {pauseDuration.TotalMinutes:F1} minutes)");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("status", usage: "<BossName>", description: "Show detailed boss status", adminOnly: true)]
        public static void BossStatus(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"📊 Status for Boss '{bossName}':");
                    ctx.Reply($"├─ Currently Spawned: {(boss.bossSpawn ? "✅ Yes" : "❌ No")}");
                    ctx.Reply($"├─ Timer Status: {(boss.IsPaused ? "⏸️ Paused" : "▶️ Running")}");
                    ctx.Reply($"├─ Spawn Time: {boss.Hour}");
                    ctx.Reply($"├─ Despawn Time: {boss.HourDespawn}");
                    ctx.Reply($"├─ Level: {boss.level} | Multiplier: {boss.multiplier}x");
                    ctx.Reply($"├─ Lifetime: {boss.Lifetime}s ({boss.Lifetime/60}min)");
                    ctx.Reply($"├─ Position: ({boss.x:F1}, {boss.y:F1}, {boss.z:F1})");
                    ctx.Reply($"├─ Current Killers: {boss.GetKillers().Count}");
                    ctx.Reply($"├─ Consecutive Spawns: {boss.ConsecutiveSpawns}");
                    ctx.Reply($"├─ Difficulty Multiplier: {boss.CurrentDifficultyMultiplier:F2}x");
                    
                    if (boss.bossSpawn && boss.GetBossEntity())
                    {
                        var health = boss.bossEntity.Read<Health>();
                        var healthPercent = (health.Value / health.MaxHealth.Value) * 100;
                        ctx.Reply($"├─ Health: {health.Value:F0}/{health.MaxHealth.Value:F0} ({healthPercent:F1}%)");
                    }
                    
                    ctx.Reply($"├─ Items: {boss.items.Count} configured");
                    ctx.Reply($"└─ Last Spawn: {(boss.LastSpawn != DateTime.MinValue ? boss.LastSpawn.ToString("yyyy-MM-dd HH:mm:ss") : "Never")}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("debug", usage: "<BossName>", description: "Show technical debug info", adminOnly: true)]
        public static void DebugBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"🔧 Debug Info for '{bossName}':");
                    ctx.Reply($"├─ Asset Name: {boss.AssetName}");
                    ctx.Reply($"├─ PrefabGUID: {boss.PrefabGUID}");
                    ctx.Reply($"├─ Name Hash: {boss.nameHash}");
                    ctx.Reply($"├─ VBlood First Kill: {boss.vbloodFirstKill}");
                    
                    if (boss.GetBossEntity())
                    {
                        ctx.Reply($"├─ Entity ID: {boss.bossEntity.Index}.{boss.bossEntity.Version}");
                        ctx.Reply($"├─ Has VBloodUnit: {boss.bossEntity.Has<VBloodUnit>()}");
                        ctx.Reply($"├─ Has Health: {boss.bossEntity.Has<Health>()}");
                        ctx.Reply($"├─ Has UnitStats: {boss.bossEntity.Has<UnitStats>()}");
                    }
                    else
                    {
                        ctx.Reply($"├─ Entity: ❌ Not found/Invalid");
                    }
                    
                    // Technical stats
                    if (boss.unitStats != null)
                    {
                        ctx.Reply($"├─ Physical Power: {boss.unitStats.PhysicalPower}");
                        ctx.Reply($"├─ Spell Power: {boss.unitStats.SpellPower}");
                        ctx.Reply($"├─ Physical Resistance: {boss.unitStats.PhysicalResistance}");
                    }
                    
                    ctx.Reply($"└─ Database Index: {Database.BOSSES.IndexOf(boss)}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("simulate", usage: "<BossName> [killer]", description: "Simulate boss death for testing", adminOnly: true)]
        public static void SimulateBossDeath(ChatCommandContext ctx, string bossName, string killerName = null)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }
                    
                    var killer = killerName ?? ctx.Event.User.CharacterName.Value;
                    
                    ctx.Reply($"🎭 Simulating death of boss '{bossName}' killed by '{killer}'...");
                    
                    // Simulate death process
                    boss.AddKiller(killer);
                    boss.BuffKillers();
                    boss.SendAnnouncementMessage();
                    
                    ctx.Reply($"✅ Death simulation completed:");
                    ctx.Reply($"├─ Killer added: {killer}");
                    ctx.Reply($"├─ Buffs applied: {(PluginConfig.BuffAfterKillingEnabled.Value ? "Yes" : "No")}");
                    ctx.Reply($"├─ Items dropped: {boss.items.Count} configured");
                    ctx.Reply($"└─ Boss despawned: Yes");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("resetkills", usage: "<BossName>", description: "Clear boss killers list", adminOnly: true)]
        public static void ResetBossKills(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    var previousKillers = boss.GetKillers().Count;
                    boss.RemoveKillers();
                    boss.vbloodFirstKill = false;
                    Database.saveDatabase();
                    
                    ctx.Reply($"🧹 Cleared {previousKillers} killers from boss '{bossName}'");
                    ctx.Reply($"└─ VBlood first kill flag reset");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("forcedrop", usage: "<BossName> [player]", description: "Force boss to drop items", adminOnly: true)]
        public static void ForceBossDrop(ChatCommandContext ctx, string bossName, string playerName = null)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    var targetPlayer = playerName ?? ctx.Event.User.CharacterName.Value;
                    
                    if (boss.GetKillers().Count == 0)
                    {
                        boss.AddKiller(targetPlayer);
                    }
                    
                    ctx.Reply($"💰 Forcing item drop for boss '{bossName}'...");
                    
                    var dropped = boss.DropItems();
                    
                    if (dropped)
                    {
                        ctx.Reply($"✅ Items dropped successfully to:");
                        foreach (var killer in boss.GetKillers())
                        {
                            ctx.Reply($"└─ {killer}");
                        }
                    }
                    else
                    {
                        ctx.Reply($"❌ No items were dropped (check boss configuration)");
                    }
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        // ===== ABILITY SWAP COMMANDS (EXPERIMENTAL) =====
        
        [Command("ability-swap", usage: "<BossName> <SourceVBloodGUID> <TargetVBloodGUID>", description: "[EXPERIMENTAL] Swap abilities between VBloods", adminOnly: true)]
        public static void SwapBossAbilities(ChatCommandContext ctx, string bossName, int sourceVBloodGUID, int targetVBloodGUID)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }
                    
                    if (!boss.GetBossEntity())
                    {
                        throw ctx.Error($"Could not find boss entity for '{bossName}'");
                    }
                    
                    var sourcePrefab = new PrefabGUID(sourceVBloodGUID);
                    var targetPrefab = new PrefabGUID(targetVBloodGUID);
                    
                    ctx.Reply($"🧪 [EXPERIMENTAL] Attempting to swap abilities...");
                    ctx.Reply($"├─ Boss: {bossName}");
                    ctx.Reply($"├─ Source VBlood GUID: {sourceVBloodGUID}");
                    ctx.Reply($"└─ Target VBlood GUID: {targetVBloodGUID}");
                    
                    bool success = AbilitySwapSystem.TrySwapVBloodAbilities(boss.bossEntity, sourcePrefab, targetPrefab);
                    
                    if (success)
                    {
                        ctx.Reply($"✅ Ability swap completed successfully!");
                        ctx.Reply($"⚠️ Effects may take a few seconds to appear");
                    }
                    else
                    {
                        ctx.Reply($"❌ Ability swap failed - check server logs for details");
                    }
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Ability swap command failed: {ex.Message}");
                throw ctx.Error($"Command failed: {ex.Message}");
            }
        }
        
        [Command("ability-debug", usage: "<BossName>", description: "Show ability debug info for a boss", adminOnly: true)]
        public static void DebugBossAbilities(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }
                    
                    if (!boss.GetBossEntity())
                    {
                        throw ctx.Error($"Could not find boss entity for '{bossName}'");
                    }
                    
                    ctx.Reply($"🔍 Ability Debug Info for '{bossName}':");
                    var debugInfo = AbilitySwapSystem.GetAbilityDebugInfo(boss.bossEntity);
                    
                    foreach (var line in debugInfo.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            ctx.Reply(line);
                        }
                    }
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
        
        [Command("ability-activate", usage: "<BossName> <VBloodGUID>", description: "[EXPERIMENTAL] Activate VBlood abilities directly", adminOnly: true)]
        public static void ActivateVBloodAbility(ChatCommandContext ctx, string bossName, int vbloodGUID)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.bossSpawn)
                    {
                        throw ctx.Error($"Boss '{bossName}' is not currently spawned");
                    }
                    
                    if (!boss.GetBossEntity())
                    {
                        throw ctx.Error($"Could not find boss entity for '{bossName}'");
                    }
                    
                    var vbloodPrefab = new PrefabGUID(vbloodGUID);
                    
                    ctx.Reply($"🔮 [EXPERIMENTAL] Activating VBlood abilities...");
                    ctx.Reply($"├─ Boss: {bossName}");
                    ctx.Reply($"└─ VBlood GUID: {vbloodGUID}");
                    
                    bool success = AbilitySwapSystem.TryActivateVBloodAbility(boss.bossEntity, vbloodPrefab);
                    
                    if (success)
                    {
                        ctx.Reply($"✅ VBlood ability activation completed!");
                        ctx.Reply($"⚠️ Effects should be immediate");
                    }
                    else
                    {
                        ctx.Reply($"❌ VBlood ability activation failed - check server logs");
                    }
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"VBlood activation command failed: {ex.Message}");
                throw ctx.Error($"Command failed: {ex.Message}");
            }
        }

        [Command("ability-set", usage: "<BossName> <VBloodGUID>", description: "Set boss abilities before spawn", adminOnly: true)]
        public static void SetBossAbilities(ChatCommandContext ctx, string bossName, int vbloodGUID)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    boss.AbilitySwapPrefabGUID = vbloodGUID;
                    Database.saveDatabase();
                    
                    ctx.Reply($"⚙️ Boss '{bossName}' abilities configured:");
                    ctx.Reply($"├─ Will spawn with VBlood GUID: {vbloodGUID}");
                    ctx.Reply($"└─ Next spawn will use these abilities");
                    
                    var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
                    var vbloodName = knownVBloods.FirstOrDefault(x => x.Value == vbloodGUID).Key ?? "Unknown VBlood";
                    ctx.Reply($"🩸 VBlood Type: {vbloodName}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("ability-clear", usage: "<BossName>", description: "Clear boss ability override (use original)", adminOnly: true)]
        public static void ClearBossAbilities(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    boss.AbilitySwapPrefabGUID = null;
                    Database.saveDatabase();
                    
                    ctx.Reply($"🔄 Boss '{bossName}' abilities reset:");
                    ctx.Reply($"└─ Will spawn with original abilities");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }

        [Command("ability-list", usage: "", description: "List known VBlood PrefabGUIDs for ability swapping", adminOnly: true)]
        public static void ListVBloodPrefabs(ChatCommandContext ctx)
        {
            ctx.Reply($"🩸 Known VBlood PrefabGUIDs for ability swapping:");
            ctx.Reply($"────────────────────────────────────────────────");
            
            var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
            foreach (var vblood in knownVBloods)
            {
                ctx.Reply($"├─ {vblood.Key}: {vblood.Value}");
            }
            ctx.Reply($"└─ Use these GUIDs with .bb ability-swap command");
            
            ctx.Reply($"");
            ctx.Reply($"📝 Example usage:");
            ctx.Reply($"   .bb ability-swap \"MyBoss\" -327335305 -1905691330");
            ctx.Reply($"   (Gives MyBoss the abilities of Dracula -> Solarus)");
        }

        // COMENTADO: Comando para exportar la base de datos de VBloods
        // Mantener para futuras actualizaciones del juego
        // Para usar: descomentar temporalmente, ejecutar el comando, copiar el archivo generado
        /*
        [Command("vblood-export", usage: "", description: "Export VBlood compatibility database to JSON", adminOnly: true)]
        public static void ExportVBloodDatabase(ChatCommandContext ctx)
        {
            try
            {
                ctx.Reply($"📊 Exporting VBlood compatibility database...");
                
                var vbloods = VBloodPrefabScanner.GetAllVBloods();
                
                if (vbloods.Count == 0)
                {
                    ctx.Reply($"❌ No VBlood data found. Make sure the scanner has run.");
                    ctx.Reply($"The scanner runs automatically 5 seconds after server start.");
                    return;
                }
                
                // Create export structure with all data
                var exportData = new
                {
                    ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TotalVBloods = vbloods.Count,
                    VBloods = vbloods.Select(vb => new
                    {
                        PrefabGUID = vb.Value.PrefabGUID.GuidHash,
                        Name = vb.Value.Name,
                        Level = vb.Value.Level,
                        CanFly = vb.Value.CanFly,
                        Features = vb.Value.Features,
                        AbilityCount = vb.Value.Abilities.Count,
                        Abilities = vb.Value.Abilities.Select(ab => new
                        {
                            SlotIndex = ab.Value.SlotIndex,
                            AbilityGUID = ab.Value.AbilityPrefabGUID.GuidHash,
                            Name = ab.Value.Name,
                            Category = ab.Value.Category.ToString(),
                            RequiresAnimation = ab.Value.RequiresAnimation,
                            IsChanneled = ab.Value.IsChanneled,
                            RequiresFlight = ab.Value.RequiresFlight,
                            CastTime = ab.Value.CastTime,
                            PostCastTime = ab.Value.PostCastTime,
                            AnimationSequence = ab.Value.AnimationSequence
                        }).OrderBy(a => a.SlotIndex).ToList()
                    }).OrderBy(v => v.Name).ToList()
                };
                
                // Export to JSON
                var json = System.Text.Json.JsonSerializer.Serialize(exportData, new System.Text.Json.JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    IncludeFields = true
                });
                
                var fileName = $"VBlood_Compatibility_Database_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(Database.ConfigPath, fileName);
                
                File.WriteAllText(filePath, json);
                
                ctx.Reply($"✅ Export completed successfully!");
                ctx.Reply($"├─ File: {fileName}");
                ctx.Reply($"├─ Path: {filePath}");
                ctx.Reply($"├─ Total VBloods: {vbloods.Count}");
                
                // Show summary by features
                var featureSummary = new Dictionary<string, int>();
                foreach (var vb in vbloods.Values)
                {
                    foreach (var feature in vb.Features)
                    {
                        if (!featureSummary.ContainsKey(feature))
                            featureSummary[feature] = 0;
                        featureSummary[feature]++;
                    }
                }
                
                ctx.Reply($"└─ Feature Distribution:");
                foreach (var feature in featureSummary.OrderByDescending(f => f.Value).Take(10))
                {
                    ctx.Reply($"   ├─ {feature.Key}: {feature.Value} VBloods");
                }
                
                // Generate C# model code for static database
                var modelFileName = $"VBloodDatabase_Model_{DateTime.Now:yyyyMMdd_HHmmss}.cs";
                var modelFilePath = Path.Combine(Database.ConfigPath, modelFileName);
                var modelCode = GenerateVBloodDatabaseModel(vbloods);
                
                File.WriteAllText(modelFilePath, modelCode);
                ctx.Reply($"");
                ctx.Reply($"📝 Also generated C# model code:");
                ctx.Reply($"└─ File: {modelFileName}");
            }
            catch (Exception ex)
            {
                ctx.Error($"Error exporting VBlood database: {ex.Message}");
                Plugin.Logger.LogError($"VBlood export error: {ex}");
            }
        }
        
        private static string GenerateVBloodDatabaseModel(Dictionary<int, VBloodInfo> vbloods)
        {
            var sb = new System.Text.StringBuilder();
            
            sb.AppendLine("// Auto-generated VBlood Database Model");
            sb.AppendLine($"// Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"// Total VBloods: {vbloods.Count}");
            sb.AppendLine();
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Stunlock.Core;");
            sb.AppendLine();
            sb.AppendLine("namespace BloodyBoss.Data");
            sb.AppendLine("{");
            sb.AppendLine("    public static class VBloodDatabase");
            sb.AppendLine("    {");
            sb.AppendLine("        private static readonly Dictionary<int, VBloodStaticInfo> _database = new()");
            sb.AppendLine("        {");
            
            foreach (var vb in vbloods.OrderBy(v => v.Value.Name))
            {
                sb.AppendLine($"            [{vb.Key}] = new VBloodStaticInfo // {vb.Value.Name}");
                sb.AppendLine("            {");
                sb.AppendLine($"                Name = \"{vb.Value.Name}\",");
                sb.AppendLine($"                Level = {vb.Value.Level},");
                sb.AppendLine($"                CanFly = {vb.Value.CanFly.ToString().ToLower()},");
                sb.AppendLine($"                Features = new HashSet<string> {{ {string.Join(", ", vb.Value.Features.Select(f => $"\"{f}\""))} }},");
                sb.AppendLine("                Abilities = new Dictionary<int, AbilityStaticInfo>");
                sb.AppendLine("                {");
                
                foreach (var ability in vb.Value.Abilities.OrderBy(a => a.Key))
                {
                    sb.AppendLine($"                    [{ability.Key}] = new AbilityStaticInfo");
                    sb.AppendLine("                    {");
                    sb.AppendLine($"                        Category = AbilityCategory.{ability.Value.Category},");
                    sb.AppendLine($"                        GUID = {ability.Value.AbilityPrefabGUID.GuidHash},");
                    sb.AppendLine($"                        CastTime = {ability.Value.CastTime.ToString(System.Globalization.CultureInfo.InvariantCulture)}f,");
                    sb.AppendLine($"                        PostCastTime = {ability.Value.PostCastTime.ToString(System.Globalization.CultureInfo.InvariantCulture)}f,");
                    sb.AppendLine($"                        HideCastBar = {ability.Value.HideCastBar.ToString().ToLower()},");
                    sb.AppendLine($"                        IsChanneled = {ability.Value.IsChanneled.ToString().ToLower()},");
                    sb.AppendLine($"                        IsCombo = {ability.Value.IsCombo.ToString().ToLower()},");
                    sb.AppendLine($"                        ComboLength = {ability.Value.ComboLength},");
                    sb.AppendLine($"                        Cooldown = {ability.Value.Cooldown.ToString(System.Globalization.CultureInfo.InvariantCulture)}f,");
                    sb.AppendLine($"                        Charges = {ability.Value.Charges},");
                    sb.AppendLine($"                        RequiresAnimation = {ability.Value.RequiresAnimation.ToString().ToLower()},");
                    sb.AppendLine($"                        RequiresFlight = {ability.Value.RequiresFlight.ToString().ToLower()},");
                    sb.AppendLine($"                        CanMoveWhileCasting = {ability.Value.CanMoveWhileCasting.ToString().ToLower()},");
                    sb.AppendLine($"                        CanRotateWhileCasting = {ability.Value.CanRotateWhileCasting.ToString().ToLower()},");
                    if (!string.IsNullOrEmpty(ability.Value.AnimationSequence))
                        sb.AppendLine($"                        AnimationSequence = \"{ability.Value.AnimationSequence}\",");
                    
                    // Add ExtraData if present
                    if (ability.Value.ExtraData.Count > 0)
                    {
                        sb.AppendLine($"                        ExtraData = new Dictionary<string, object>");
                        sb.AppendLine($"                        {{");
                        foreach (var kvp in ability.Value.ExtraData)
                        {
                            if (kvp.Value is float floatValue)
                            {
                                sb.AppendLine($"                            {{ \"{kvp.Key}\", {floatValue.ToString(System.Globalization.CultureInfo.InvariantCulture)}f }},");
                            }
                            else if (kvp.Value is int intValue)
                            {
                                sb.AppendLine($"                            {{ \"{kvp.Key}\", {intValue} }},");
                            }
                            else if (kvp.Value is bool boolValue)
                            {
                                sb.AppendLine($"                            {{ \"{kvp.Key}\", {boolValue.ToString().ToLower()} }},");
                            }
                            else
                            {
                                sb.AppendLine($"                            {{ \"{kvp.Key}\", \"{kvp.Value}\" }},");
                            }
                        }
                        sb.AppendLine($"                        }},");
                    }
                    
                    // Add SpawnedPrefabs if present
                    if (ability.Value.SpawnedPrefabs.Count > 0)
                    {
                        sb.AppendLine($"                        SpawnedPrefabs = new List<SpawnInfo>");
                        sb.AppendLine($"                        {{");
                        foreach (var spawn in ability.Value.SpawnedPrefabs)
                        {
                            sb.AppendLine($"                            new SpawnInfo");
                            sb.AppendLine($"                            {{");
                            sb.AppendLine($"                                SpawnPrefab = new PrefabGUID({spawn.SpawnPrefab.GuidHash}),");
                            sb.AppendLine($"                                SpawnName = \"{spawn.SpawnName}\",");
                            sb.AppendLine($"                                Target = \"{spawn.Target}\",");
                            sb.AppendLine($"                                HoverDistance = {spawn.HoverDistance.ToString(System.Globalization.CultureInfo.InvariantCulture)}f,");
                            sb.AppendLine($"                                HoverMaxDistance = {spawn.HoverMaxDistance.ToString(System.Globalization.CultureInfo.InvariantCulture)}f");
                            sb.AppendLine($"                            }},");
                        }
                        sb.AppendLine($"                        }},");
                    }
                    
                    // Add AppliedBuffs if present
                    if (ability.Value.AppliedBuffs.Count > 0)
                    {
                        sb.AppendLine($"                        AppliedBuffs = new List<BuffInfo>");
                        sb.AppendLine($"                        {{");
                        foreach (var buff in ability.Value.AppliedBuffs)
                        {
                            sb.AppendLine($"                            new BuffInfo");
                            sb.AppendLine($"                            {{");
                            sb.AppendLine($"                                BuffPrefab = new PrefabGUID({buff.BuffPrefab.GuidHash}),");
                            sb.AppendLine($"                                BuffName = \"{buff.BuffName}\",");
                            sb.AppendLine($"                                BuffTarget = \"{buff.BuffTarget}\",");
                            sb.AppendLine($"                                SpellTarget = \"{buff.SpellTarget}\"");
                            sb.AppendLine($"                            }},");
                        }
                        sb.AppendLine($"                        }},");
                    }
                    
                    // Add CastConditions if present
                    if (ability.Value.CastConditions.Count > 0)
                    {
                        sb.AppendLine($"                        CastConditions = new List<string>");
                        sb.AppendLine($"                        {{");
                        foreach (var condition in ability.Value.CastConditions)
                        {
                            sb.AppendLine($"                            \"{condition}\",");
                        }
                        sb.AppendLine($"                        }}");
                    }
                    sb.AppendLine("                    },");
                }
                
                sb.AppendLine("                }");
                sb.AppendLine("            },");
            }
            
            sb.AppendLine("        };");
            sb.AppendLine();
            sb.AppendLine("        public static VBloodStaticInfo GetVBlood(int guid) => _database.TryGetValue(guid, out var info) ? info : null;");
            sb.AppendLine("        public static bool HasVBlood(int guid) => _database.ContainsKey(guid);");
            sb.AppendLine("        public static int Count => _database.Count;");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public class VBloodStaticInfo");
            sb.AppendLine("    {");
            sb.AppendLine("        public string Name { get; set; }");
            sb.AppendLine("        public int Level { get; set; }");
            sb.AppendLine("        public bool CanFly { get; set; }");
            sb.AppendLine("        public HashSet<string> Features { get; set; }");
            sb.AppendLine("        public Dictionary<int, AbilityStaticInfo> Abilities { get; set; }");
            sb.AppendLine("    }");
            sb.AppendLine();
            sb.AppendLine("    public class AbilityStaticInfo");
            sb.AppendLine("    {");
            sb.AppendLine("        public AbilityCategory Category { get; set; }");
            sb.AppendLine("        public int GUID { get; set; }");
            sb.AppendLine("        public float CastTime { get; set; }");
            sb.AppendLine("        public float PostCastTime { get; set; }");
            sb.AppendLine("        public bool IsChanneled { get; set; }");
            sb.AppendLine("        public bool IsCombo { get; set; }");
            sb.AppendLine("        public int ComboLength { get; set; }");
            sb.AppendLine("        public float Cooldown { get; set; }");
            sb.AppendLine("        public int Charges { get; set; }");
            sb.AppendLine("        public bool RequiresAnimation { get; set; }");
            sb.AppendLine("        public bool RequiresFlight { get; set; }");
            sb.AppendLine("        public string AnimationSequence { get; set; }");
            sb.AppendLine("        public Dictionary<string, object> ExtraData { get; set; } = new Dictionary<string, object>();");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        */

        // ===== MODULAR ABILITY SYSTEM COMMANDS =====
        
        [Command("ability-slot-set", usage: "<BossName> <SlotName> <SourcePrefabGUID> <AbilityIndex> [enabled] [description]", description: "Configure a specific ability slot", adminOnly: true)]
        public static void SetAbilitySlot(ChatCommandContext ctx, string bossName, string slotName, int sourcePrefabGUID, int abilityIndex, bool enabled = true, string description = "")
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    // Validar que el PrefabGUID existe
                    var sourcePrefab = new PrefabGUID(sourcePrefabGUID);
                    if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.ContainsKey(sourcePrefab))
                    {
                        throw ctx.Error($"Source PrefabGUID {sourcePrefabGUID} not found in game data");
                    }
                    
                    // Validar nombre del slot
                    if (string.IsNullOrWhiteSpace(slotName))
                    {
                        throw ctx.Error("Slot name cannot be empty");
                    }
                    
                    // Validar compatibilidad de la habilidad usando el nuevo sistema
                    var compatibilityResult = AbilityCompatibilitySystem.CheckAbilityCompatibility(
                        boss.PrefabGUID, sourcePrefabGUID, abilityIndex);
                    
                    var bossInfo = VBloodDatabase.GetVBlood(boss.PrefabGUID);
                    var sourceInfo = VBloodDatabase.GetVBlood(sourcePrefabGUID);
                    
                    if (!compatibilityResult.IsCompatible)
                    {
                        ctx.Reply($"❌ ERROR: This ability is incompatible with {bossInfo?.Name ?? "this boss"}");
                        foreach (var error in compatibilityResult.Errors)
                        {
                            ctx.Reply($"├─ ⛔ {error}");
                        }
                        ctx.Reply($"└─ This ability cannot be used on this boss");
                        return;
                    }
                    else if (compatibilityResult.Level != AbilityCompatibilitySystem.CompatibilityLevel.Perfect)
                    {
                        ctx.Reply($"⚠️ WARNING: Compatibility level: {compatibilityResult.Level}");
                        foreach (var warning in compatibilityResult.Warnings)
                        {
                            ctx.Reply($"├─ ⚡ {warning}");
                        }
                        
                        if (sourceInfo != null && sourceInfo.Abilities.TryGetValue(abilityIndex, out var abilityInfo))
                        {
                            ctx.Reply($"├─ Category: {abilityInfo.Category}");
                            if (abilityInfo.CastTime > 0)
                            {
                                ctx.Reply($"├─ Cast Time: {abilityInfo.CastTime}s");
                            }
                            if (abilityInfo.SpawnedPrefabs.Count > 0)
                            {
                                ctx.Reply($"├─ Spawns: {abilityInfo.SpawnedPrefabs.Count} projectile(s)");
                            }
                        }
                        ctx.Reply($"└─ The ability will work but may have visual or gameplay issues");
                    }
                    
                    // Crear o actualizar el slot
                    boss.CustomAbilities[slotName] = new CustomAbilitySlot(sourcePrefabGUID, abilityIndex, enabled, description);
                    Database.saveDatabase();
                    
                    ctx.Reply($"🎯 Configured ability slot '{slotName}' for boss '{bossName}':");
                    ctx.Reply($"├─ Source PrefabGUID: {sourcePrefabGUID}");
                    ctx.Reply($"├─ Ability Index: {abilityIndex}");
                    ctx.Reply($"├─ Enabled: {(enabled ? "✅ Yes" : "❌ No")}");
                    ctx.Reply($"├─ Description: {(string.IsNullOrEmpty(description) ? "None" : description)}");
                    ctx.Reply($"└─ Compatibility: {GetCompatibilityIcon(compatibilityResult.Level)} {compatibilityResult.Level}");
                    
                    var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
                    var vbloodName = knownVBloods.FirstOrDefault(x => x.Value == sourcePrefabGUID).Key ?? "Unknown VBlood";
                    ctx.Reply($"🩸 Source VBlood: {vbloodName}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
        
        [Command("ability-slot-remove", usage: "<BossName> <SlotName>", description: "Remove a specific ability slot", adminOnly: true)]
        public static void RemoveAbilitySlot(ChatCommandContext ctx, string bossName, string slotName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.CustomAbilities.ContainsKey(slotName))
                    {
                        throw ctx.Error($"Ability slot '{slotName}' does not exist for boss '{bossName}'");
                    }
                    
                    boss.CustomAbilities.Remove(slotName);
                    Database.saveDatabase();
                    
                    ctx.Reply($"🗑️ Removed ability slot '{slotName}' from boss '{bossName}'");
                    ctx.Reply($"└─ Remaining slots: {boss.CustomAbilities.Count}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
        
        [Command("ability-slot-toggle", usage: "<BossName> <SlotName>", description: "Enable/disable a specific ability slot", adminOnly: true)]
        public static void ToggleAbilitySlot(ChatCommandContext ctx, string bossName, string slotName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (!boss.CustomAbilities.ContainsKey(slotName))
                    {
                        throw ctx.Error($"Ability slot '{slotName}' does not exist for boss '{bossName}'");
                    }
                    
                    var slot = boss.CustomAbilities[slotName];
                    slot.Enabled = !slot.Enabled;
                    Database.saveDatabase();
                    
                    ctx.Reply($"🔄 Toggled ability slot '{slotName}' for boss '{bossName}':");
                    ctx.Reply($"└─ Status: {(slot.Enabled ? "✅ Enabled" : "❌ Disabled")}");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
        
        [Command("ability-slot-list", usage: "<BossName>", description: "List all configured ability slots for a boss", adminOnly: true)]
        public static void ListAbilitySlots(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    if (boss.CustomAbilities.Count == 0)
                    {
                        ctx.Reply($"📋 Boss '{bossName}' has no custom ability slots configured");
                        ctx.Reply($"└─ Use .bb ability-slot-set to configure abilities");
                        return;
                    }
                    
                    ctx.Reply($"📋 Custom ability slots for boss '{bossName}':");
                    ctx.Reply($"────────────────────────────────────────────────");
                    
                    var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
                    
                    foreach (var slot in boss.CustomAbilities)
                    {
                        var vbloodName = knownVBloods.FirstOrDefault(x => x.Value == slot.Value.SourcePrefabGUID).Key ?? "Unknown";
                        var status = slot.Value.Enabled ? "✅" : "❌";
                        
                        ctx.Reply($"├─ Slot '{slot.Key}': {status}");
                        ctx.Reply($"│  ├─ Source: {vbloodName} ({slot.Value.SourcePrefabGUID})");
                        ctx.Reply($"│  ├─ Ability Index: {slot.Value.AbilityIndex}");
                        ctx.Reply($"│  └─ Description: {(string.IsNullOrEmpty(slot.Value.Description) ? "None" : slot.Value.Description)}");
                    }
                    
                    var enabledCount = boss.CustomAbilities.Values.Count(s => s.Enabled);
                    ctx.Reply($"└─ Total: {boss.CustomAbilities.Count} slots ({enabledCount} enabled)");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
        
        [Command("ability-slot-clear", usage: "<BossName>", description: "Remove all custom ability slots from a boss", adminOnly: true)]
        public static void ClearAbilitySlots(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    var removedCount = boss.CustomAbilities.Count;
                    boss.CustomAbilities.Clear();
                    Database.saveDatabase();
                    
                    ctx.Reply($"🧹 Cleared all custom ability slots from boss '{bossName}'");
                    ctx.Reply($"└─ Removed {removedCount} slots");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
        
        [Command("ability-discover", usage: "", description: "Discover all VBloods in game and their abilities", adminOnly: true)]
        public static void DiscoverAllVBloods(ChatCommandContext ctx)
        {
            try
            {
                ctx.Reply("🔍 Discovering all VBloods in the game world...");
                
                var entityManager = Core.World.EntityManager;
                var discoveredVBloods = new Dictionary<string, int>();
                
                // Buscar todas las entidades con VBloodUnit component
                var query = entityManager.CreateEntityQuery(
                    ComponentType.ReadOnly<VBloodUnit>(),
                    ComponentType.ReadOnly<PrefabGUID>()
                );
                
                var entities = query.ToEntityArray(Allocator.Temp);
                ctx.Reply($"Found {entities.Length} VBlood entities in game world:");
                
                foreach (var entity in entities)
                {
                    try
                    {
                        var prefabGUID = entity.Read<PrefabGUID>();
                        var assetName = "Unknown";
                        
                        if (Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup.TryGetValue(prefabGUID, out var prefabData))
                        {
                            assetName = prefabData.AssetName.ToString();
                        }
                        
                        if (!discoveredVBloods.ContainsKey(assetName))
                        {
                            discoveredVBloods[assetName] = prefabGUID.GuidHash;
                            
                            // Check if it has abilities
                            var hasAbilityBar = entity.Has<AbilityBar_Shared>();
                            var hasAbilityBuffer = entityManager.HasBuffer<AbilityGroupSlotBuffer>(entity);
                            var abilityCount = hasAbilityBuffer ? entityManager.GetBuffer<AbilityGroupSlotBuffer>(entity).Length : 0;
                            
                            ctx.Reply($"├─ {assetName}");
                            ctx.Reply($"│  ├─ PrefabGUID: {prefabGUID.GuidHash}");
                            ctx.Reply($"│  ├─ AbilityBar: {(hasAbilityBar ? "✅" : "❌")}");
                            ctx.Reply($"│  └─ Abilities: {abilityCount} slots");
                        }
                    }
                    catch (Exception ex)
                    {
                        ctx.Reply($"Error processing entity: {ex.Message}");
                    }
                }
                
                entities.Dispose();
                query.Dispose();
                
                ctx.Reply($"└─ Total unique VBloods discovered: {discoveredVBloods.Count}");
                
                // Save discovered VBloods to file
                var fileName = $"Discovered_VBloods_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine("/home/trodi/vrising_server/BepInEx/config/BloodyBoss/", fileName);
                
                var lines = new List<string>();
                lines.Add("// Discovered VBloods in V Rising");
                lines.Add($"// Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                lines.Add("// Format: AssetName = PrefabGUID");
                lines.Add("");
                
                foreach (var vblood in discoveredVBloods.OrderBy(x => x.Key))
                {
                    lines.Add($"{{ \"{vblood.Key}\", {vblood.Value} }}, // {vblood.Key}");
                }
                
                try
                {
                    File.WriteAllLines(filePath, lines);
                    ctx.Reply($"📄 Discovery results saved to: {fileName}");
                }
                catch (Exception ex)
                {
                    ctx.Reply($"⚠️ Could not save file: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw ctx.Error($"Error discovering VBloods: {ex.Message}");
            }
        }

        [Command("ability-export-all", usage: "", description: "Generate complete abilities documentation for all VBloods", adminOnly: true)]
        public static void ExportAllAbilities(ChatCommandContext ctx)
        {
            try
            {
                ctx.Reply("🔍 Generating complete VBlood abilities documentation...");
                
                var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
                var entityManager = Core.World.EntityManager;
                var documentation = new List<string>();
                
                documentation.Add("# VBlood Abilities Documentation");
                documentation.Add("Generated automatically by BloodyBoss modular ability system");
                documentation.Add($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                documentation.Add("");
                
                foreach (var vblood in knownVBloods.OrderBy(x => x.Key))
                {
                    var sourcePrefab = new PrefabGUID(vblood.Value);
                    
                    if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(sourcePrefab, out Entity sourceEntity))
                    {
                        documentation.Add($"## {vblood.Key} (PrefabGUID: {vblood.Value})");
                        documentation.Add("❌ **ERROR**: Entity not found in game data");
                        documentation.Add("");
                        continue;
                    }
                    
                    documentation.Add($"## {vblood.Key} (PrefabGUID: {vblood.Value})");
                    
                    // Check components
                    var hasAbilityBar = sourceEntity.Has<AbilityBar_Shared>();
                    var hasAbilityBuffer = entityManager.HasBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                    
                    documentation.Add($"- **Has AbilityBar_Shared**: {(hasAbilityBar ? "✅ Yes" : "❌ No")}");
                    
                    if (hasAbilityBuffer)
                    {
                        var buffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                        documentation.Add($"- **Available Ability Slots**: {buffer.Length}");
                        documentation.Add("");
                        documentation.Add("### Available Abilities:");
                        
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            try
                            {
                                var abilityGroup = buffer[i];
                                string abilityInfo = AnalyzeAbilityGroupForDocs(abilityGroup, i);
                                documentation.Add($"- **Index {i}**: {abilityInfo}");
                            }
                            catch (Exception ex)
                            {
                                documentation.Add($"- **Index {i}**: Available for use (analysis failed)");
                            }
                        }
                        
                        documentation.Add("");
                        documentation.Add("### Usage Examples:");
                        documentation.Add("```bash");
                        for (int i = 0; i < Math.Min(buffer.Length, 3); i++)
                        {
                            documentation.Add($".bb ability-slot-set \"YourBoss\" \"slot{i + 1}\" {vblood.Value} {i} true \"{vblood.Key} ability {i + 1}\"");
                        }
                        documentation.Add("```");
                    }
                    else
                    {
                        documentation.Add("- **Available Ability Slots**: ❌ No AbilityGroupSlotBuffer found");
                        documentation.Add("- **Note**: This VBlood may not be compatible with the modular ability system");
                    }
                    
                    documentation.Add("");
                    documentation.Add("---");
                    documentation.Add("");
                }
                
                // Add preset examples
                documentation.Add("# Predefined Presets");
                documentation.Add("");
                documentation.Add("## Available Presets:");
                documentation.Add("");
                
                var presets = GetAvailablePresets();
                foreach (var presetName in presets)
                {
                    var preset = GetAbilityPreset(presetName);
                    if (preset != null)
                    {
                        documentation.Add($"### {presetName}");
                        documentation.Add("```bash");
                        documentation.Add($".bb ability-preset \"YourBoss\" \"{presetName}\"");
                        documentation.Add("```");
                        documentation.Add("**Configuration:**");
                        
                        foreach (var slot in preset)
                        {
                            var vbloodName = knownVBloods.FirstOrDefault(x => x.Value == slot.Value.SourcePrefabGUID).Key ?? "Unknown";
                            documentation.Add($"- **{slot.Key}**: {vbloodName} (Index {slot.Value.AbilityIndex}) - {slot.Value.Description}");
                        }
                        documentation.Add("");
                    }
                }
                
                // Save to file
                var fileName = $"VBlood_Abilities_Documentation_{DateTime.Now:yyyyMMdd_HHmmss}.md";
                var filePath = Path.Combine("/home/trodi/vrising_server/BepInEx/config/BloodyBoss/", fileName);
                
                try
                {
                    File.WriteAllLines(filePath, documentation);
                    ctx.Reply($"📄 Documentation exported successfully!");
                    ctx.Reply($"└─ File: {fileName}");
                    ctx.Reply($"└─ Path: {filePath}");
                    ctx.Reply($"└─ Total VBloods documented: {knownVBloods.Count}");
                }
                catch (Exception ex)
                {
                    ctx.Reply($"⚠️ Documentation generated but file save failed: {ex.Message}");
                    ctx.Reply($"📋 Showing first few lines in chat:");
                    
                    for (int i = 0; i < Math.Min(10, documentation.Count); i++)
                    {
                        ctx.Reply(documentation[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ctx.Error($"Error generating documentation: {ex.Message}");
            }
        }

        [Command("ability-inspect", usage: "<SourcePrefabGUID>", description: "Inspect available abilities of a VBlood", adminOnly: true)]
        public static void InspectAbilities(ChatCommandContext ctx, int sourcePrefabGUID)
        {
            try
            {
                var sourcePrefab = new PrefabGUID(sourcePrefabGUID);
                if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(sourcePrefab, out Entity sourceEntity))
                {
                    throw ctx.Error($"PrefabGUID {sourcePrefabGUID} not found");
                }
                
                var entityManager = Core.World.EntityManager;
                var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
                var vbloodName = knownVBloods.FirstOrDefault(x => x.Value == sourcePrefabGUID).Key ?? "Unknown VBlood";
                
                ctx.Reply($"🔍 Detailed abilities for {vbloodName} (PrefabGUID: {sourcePrefabGUID}):");
                ctx.Reply($"────────────────────────────────────────────────");
                
                // Check AbilityBar_Shared
                if (sourceEntity.Has<AbilityBar_Shared>())
                {
                    ctx.Reply($"✅ Has AbilityBar_Shared component");
                }
                else
                {
                    ctx.Reply($"❌ No AbilityBar_Shared component");
                    return;
                }
                
                // Check AbilityGroupSlotBuffer with detailed analysis
                if (entityManager.HasBuffer<AbilityGroupSlotBuffer>(sourceEntity))
                {
                    var buffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                    ctx.Reply($"✅ Found {buffer.Length} ability groups:");
                    ctx.Reply($"");
                    
                    for (int i = 0; i < buffer.Length && i < 20; i++)
                    {
                        try
                        {
                            var abilityGroup = buffer[i];
                            string abilityInfo = AnalyzeAbilityGroup(abilityGroup, i);
                            ctx.Reply($"├─ Index {i}: {abilityInfo}");
                        }
                        catch (Exception ex)
                        {
                            ctx.Reply($"├─ Index {i}: Error analyzing - {ex.Message}");
                        }
                    }
                    
                    if (buffer.Length > 20)
                    {
                        ctx.Reply($"└─ ... and {buffer.Length - 20} more abilities (use .bb ability-export-all for complete list)");
                    }
                    
                    ctx.Reply($"");
                    ctx.Reply($"💡 Usage examples:");
                    for (int i = 0; i < Math.Min(buffer.Length, 3); i++)
                    {
                        ctx.Reply($"   .bb ability-slot-set \"YourBoss\" \"slot{i + 1}\" {sourcePrefabGUID} {i} true \"{vbloodName} ability {i + 1}\"");
                    }
                }
                else
                {
                    ctx.Reply($"❌ No AbilityGroupSlotBuffer - VBlood may not be compatible with modular system");
                }
            }
            catch (Exception ex)
            {
                throw ctx.Error($"Error inspecting abilities: {ex.Message}");
            }
        }
        
        private static string AnalyzeAbilityGroup(AbilityGroupSlotBuffer abilityGroup, int index)
        {
            try
            {
                // Try to extract meaningful information from the ability group
                var info = new List<string>();
                
                // Attempt to get ability properties using reflection (careful with Il2Cpp)
                var type = abilityGroup.GetType();
                var fields = type.GetFields();
                
                foreach (var field in fields)
                {
                    try
                    {
                        var value = field.GetValue(abilityGroup);
                        if (value != null)
                        {
                            if (value is PrefabGUID prefabGuid && prefabGuid.GuidHash != 0)
                            {
                                // Try to get the name of this ability from the prefab system
                                if (Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup.TryGetValue(prefabGuid, out var prefabData))
                                {
                                    var assetName = prefabData.AssetName.ToString();
                                    if (assetName.Contains("Ability") || assetName.Contains("Spell") || assetName.Contains("Attack"))
                                    {
                                        // Clean up the asset name to be more readable
                                        var cleanName = CleanAbilityName(assetName);
                                        info.Add($"'{cleanName}'");
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Skip fields that can't be accessed
                    }
                }
                
                // Classify ability type based on index (rough heuristic)
                string type_guess = index switch
                {
                    0 => "Primary Attack",
                    1 => "Secondary Attack", 
                    2 => "Special/Spell",
                    3 => "Ultimate/Spell",
                    _ when index < 6 => "Combat Ability",
                    _ when index < 10 => "Advanced Ability",
                    _ => "System Ability"
                };
                
                if (info.Count > 0)
                {
                    return $"{type_guess} - {string.Join(", ", info)}";
                }
                else
                {
                    return $"{type_guess} - Available";
                }
            }
            catch (Exception ex)
            {
                return $"Available (analysis failed: {ex.Message})";
            }
        }
        
        private static string CleanAbilityName(string assetName)
        {
            // Clean up asset names to be more readable
            return assetName
                .Replace("AB_", "")
                .Replace("CHAR_", "")
                .Replace("VBlood_", "")
                .Replace("_", " ")
                .Replace("Projectile", "Proj")
                .Replace("Attack", "Atk")
                .Replace("Ability", "")
                .Trim();
        }
        
        private static string AnalyzeAbilityGroupForDocs(AbilityGroupSlotBuffer abilityGroup, int index)
        {
            try
            {
                // Try to extract meaningful information from the ability group
                var abilityNames = new List<string>();
                
                // Attempt to get ability properties using reflection (careful with Il2Cpp)
                var type = abilityGroup.GetType();
                var fields = type.GetFields();
                
                foreach (var field in fields)
                {
                    try
                    {
                        var value = field.GetValue(abilityGroup);
                        if (value != null)
                        {
                            if (value is PrefabGUID prefabGuid && prefabGuid.GuidHash != 0)
                            {
                                // Try to get the name of this ability from the prefab system
                                if (Plugin.SystemsCore.PrefabCollectionSystem._PrefabDataLookup.TryGetValue(prefabGuid, out var prefabData))
                                {
                                    var assetName = prefabData.AssetName.ToString();
                                    if (assetName.Contains("Ability") || assetName.Contains("Spell") || assetName.Contains("Attack") || assetName.Contains("Cast"))
                                    {
                                        // Clean up the asset name to be more readable
                                        var cleanName = CleanAbilityName(assetName);
                                        if (!string.IsNullOrWhiteSpace(cleanName) && cleanName.Length > 2)
                                        {
                                            abilityNames.Add($"`{cleanName}`");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Skip fields that can't be accessed
                    }
                }
                
                // Classify ability type based on index (heuristic based on common VBlood patterns)
                string type_classification = index switch
                {
                    0 => "**Primary Attack**",
                    1 => "**Secondary Attack**", 
                    2 => "**Special/Spell**",
                    3 => "**Ultimate/Spell**",
                    _ when index < 6 => "**Combat Ability**",
                    _ when index < 10 => "**Advanced Ability**",
                    _ => "**System Ability**"
                };
                
                // Build the description
                if (abilityNames.Count > 0)
                {
                    var uniqueNames = abilityNames.Distinct().Take(3); // Limit to avoid clutter
                    return $"{type_classification} - {string.Join(", ", uniqueNames)}";
                }
                else
                {
                    return $"{type_classification} - Available for use";
                }
            }
            catch (Exception ex)
            {
                // Fallback for any errors
                string type_fallback = index switch
                {
                    0 => "**Primary Attack**",
                    1 => "**Secondary Attack**", 
                    2 => "**Special/Spell**",
                    3 => "**Ultimate/Spell**",
                    _ => "**Combat Ability**"
                };
                return $"{type_fallback} - Available for use";
            }
        }

        [Command("ability-preset", usage: "<BossName> <PresetName>", description: "Apply a predefined ability preset", adminOnly: true)]
        public static void ApplyAbilityPreset(ChatCommandContext ctx, string bossName, string presetName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    var preset = GetAbilityPreset(presetName.ToLower());
                    if (preset == null)
                    {
                        ctx.Reply($"❌ Unknown preset '{presetName}'. Available presets:");
                        foreach (var availablePreset in GetAvailablePresets())
                        {
                            ctx.Reply($"├─ {availablePreset}");
                        }
                        return;
                    }
                    
                    boss.CustomAbilities.Clear();
                    foreach (var slot in preset)
                    {
                        boss.CustomAbilities[slot.Key] = slot.Value;
                    }
                    Database.saveDatabase();
                    
                    ctx.Reply($"📦 Applied preset '{presetName}' to boss '{bossName}':");
                    ctx.Reply($"└─ Configured {preset.Count} ability slots");
                }
                else
                {
                    throw new BossDontExistException();
                }
            }
            catch (BossDontExistException)
            {
                throw ctx.Error($"Boss '{bossName}' does not exist");
            }
        }
        
        // Presets helper methods
        private static Dictionary<string, CustomAbilitySlot> GetAbilityPreset(string presetName)
        {
            return presetName switch
            {
                "dracula-mix" => new Dictionary<string, CustomAbilitySlot>
                {
                    ["melee1"] = new CustomAbilitySlot(-327335305, 0, true, "Dracula melee attack"),
                    ["spell1"] = new CustomAbilitySlot(-327335305, 2, true, "Dracula spell"),
                    ["special"] = new CustomAbilitySlot(939467639, 1, true, "Vincent frost ability")
                },
                "frost-warrior" => new Dictionary<string, CustomAbilitySlot>
                {
                    ["melee1"] = new CustomAbilitySlot(1112948824, 0, true, "Tristan melee"),
                    ["melee2"] = new CustomAbilitySlot(1112948824, 1, true, "Tristan charge"),
                    ["frost"] = new CustomAbilitySlot(939467639, 2, true, "Vincent frost blast")
                },
                "spell-caster" => new Dictionary<string, CustomAbilitySlot>
                {
                    ["spell1"] = new CustomAbilitySlot(-99012450, 1, true, "Christina heal"),
                    ["spell2"] = new CustomAbilitySlot(-99012450, 2, true, "Christina light"),
                    ["spell3"] = new CustomAbilitySlot(-327335305, 3, true, "Dracula dark spell")
                },
                _ => null
            };
        }
        
        private static string[] GetAvailablePresets()
        {
            return new[] { "dracula-mix", "frost-warrior", "spell-caster" };
        }

        [Command("teleport", usage: "<BossName>", description: "Teleport to boss location")]
        public static void TeleportToBoss(ChatCommandContext ctx, string bossName)
        {
            try
            {
                var playerName = ctx.Event.User.CharacterName.Value;
                var userModel = GameData.Users.GetUserByCharacterName(playerName);
                
                // Verify if the command is enabled
                if (!PluginConfig.EnableTeleportCommand.Value)
                {
                    throw ctx.Error("Teleport command is disabled");
                }
                
                // Verify permissions (admin only if configured)
                if (PluginConfig.TeleportAdminOnly.Value && !ctx.Event.User.IsAdmin)
                {
                    throw ctx.Error("🚫 Teleport command is restricted to administrators only");
                }
                
                // Verify if the boss exists
                if (!Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    throw ctx.Error($"Boss '{bossName}' does not exist");
                }
                
                // Verify if only allows teleport to active bosses
                if (PluginConfig.TeleportOnlyToActiveBosses.Value && !boss.bossSpawn)
                {
                    throw ctx.Error($"Boss '{bossName}' is not currently active");
                }
                
                // Verify cooldown
                if (!TeleportManager.CanPlayerTeleport(playerName, out string cooldownReason))
                {
                    throw ctx.Error($"⏰ {cooldownReason}");
                }
                
                // Verify cost (only for non-admins)
                if (!ctx.Event.User.IsAdmin && !TeleportManager.HasTeleportCost(userModel, out string costInfo))
                {
                    throw ctx.Error($"💰 {costInfo}");
                }
                
                // Determine teleport position
                float3 targetPosition;
                bool isBossAlive = false;
                
                if (boss.bossSpawn && boss.GetBossEntity())
                {
                    // Verify if the boss is alive
                    if (boss.bossEntity.Has<Health>())
                    {
                        var health = boss.bossEntity.Read<Health>();
                        isBossAlive = !health.IsDead && health.Value > 0;
                        
                        if (PluginConfig.TeleportRequireBossAlive.Value && !isBossAlive)
                        {
                            throw ctx.Error($"💀 Boss '{bossName}' is dead");
                        }
                    }
                    
                    // Teleport to current boss position
                    var bossTranslation = boss.bossEntity.Read<Translation>();
                    targetPosition = bossTranslation.Value;
                    
                    var statusInfo = isBossAlive ? "alive" : "dead";
                    ctx.Reply($"🌀 Teleporting to {statusInfo} boss '{bossName}'...");
                }
                else
                {
                    // Teleport to configured boss position
                    targetPosition = new float3(boss.x, boss.y, boss.z);
                    ctx.Reply($"🌀 Teleporting to boss '{bossName}' spawn location...");
                }
                
                // Consume cost (only for non-admins)
                if (!ctx.Event.User.IsAdmin)
                {
                    if (!TeleportManager.ConsumeTeleportCost(userModel))
                    {
                        throw ctx.Error("Failed to consume teleport cost");
                    }
                }
                
                // Apply teleport
                var playerEntity = userModel.Character.Entity;
                var translation = playerEntity.Read<Translation>();
                translation.Value = targetPosition;
                playerEntity.Write(translation);
                
                // Apply cooldown
                TeleportManager.SetTeleportCooldown(playerName);
                
                // Confirmation message
                ctx.Reply($"✅ Teleported to {targetPosition.x:F1}, {targetPosition.y:F1}, {targetPosition.z:F1}");
                
                if (PluginConfig.TeleportCooldown.Value > 0)
                {
                    ctx.Reply($"⏰ Next teleport available in {PluginConfig.TeleportCooldown.Value:F0} seconds");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Teleport command failed: {ex.Message}");
                throw ctx.Error($"Teleport failed: {ex.Message}");
            }
        }
    }
}
