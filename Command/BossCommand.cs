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
using ProjectM.Shared;
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

        [Command("start", usage: "<NameOfBoss>", description: "The confrontation with a Boss begins.", adminOnly: true)]
        public static void start(ChatCommandContext ctx, string BossName)
        {
            try
            {
                var user = ctx.Event.SenderUserEntity;
                if (Database.GetBoss(BossName, out BossEncounterModel Boss))
                {
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

        [Command("status", usage: "<BossName>", description: "Show detailed boss status", adminOnly: true)]
        public static void BossStatus(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"{FontColorChatSystem.Blue("[STATUS]")} Status for Boss '{bossName}':");
                    ctx.Reply($"├─ Currently Spawned: {(boss.bossSpawn ? FontColorChatSystem.Green("Yes") : FontColorChatSystem.Red("No"))}");
                    ctx.Reply($"├─ Timer Status: {(boss.IsPaused ? FontColorChatSystem.Yellow("Paused") : FontColorChatSystem.Green("Running"))}");
                    ctx.Reply($"├─ Spawn Time: {boss.Hour}");
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

        [Command("cleanup", description: "Clean up all BloodyBoss entities and icons from the server", adminOnly: true)]
        public static void CleanupServer(ChatCommandContext ctx)
        {
            try
            {
                ctx.Reply($"{FontColorChatSystem.Yellow("[CLEANUP]")} Starting server cleanup...");
                
                var entityManager = Plugin.SystemsCore.EntityManager;
                var userModel = GameData.Users.GetUserByCharacterName(ctx.Event.User.CharacterName.Value);
                
                int bossesRemoved = 0;
                int iconsRemoved = 0;
                int databaseUpdated = 0;
                
                // 1. Clean up ALL bosses from database (regardless of spawn state)
                foreach (var boss in Database.BOSSES.ToList())
                {
                    try
                    {
                        bool updated = false;
                        
                        // Remove boss entity if exists
                        if (boss.bossEntity != Entity.Null && entityManager.Exists(boss.bossEntity))
                        {
                            StatChangeUtility.KillOrDestroyEntity(entityManager, boss.bossEntity, userModel.Entity, userModel.Entity, 0, StatChangeReason.Any, true);
                            bossesRemoved++;
                            updated = true;
                        }
                        
                        // Remove icon entity if exists
                        if (boss.iconEntity != Entity.Null && entityManager.Exists(boss.iconEntity))
                        {
                            StatChangeUtility.KillOrDestroyEntity(entityManager, boss.iconEntity, userModel.Entity, userModel.Entity, 0, StatChangeReason.Any, true);
                            iconsRemoved++;
                            updated = true;
                        }
                        
                        // Update boss state (always reset state)
                        if (boss.bossSpawn || updated)
                        {
                            boss.bossSpawn = false;
                            boss.bossEntity = Entity.Null;
                            boss.iconEntity = Entity.Null;
                            boss.RemoveKillers();
                            BossMechanicSystem.CleanupBossMechanics(boss);
                            databaseUpdated++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.BLogger.Warning(LogCategory.Boss, $"Error cleaning boss {boss.name}: {ex.Message}");
                    }
                }
                
                // 2. Clean up orphaned boss entities (by nameHash pattern)
                var nameableEntities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>(EntityQueryOptions.IncludeDisabledEntities);
                foreach (var entity in nameableEntities)
                {
                    try
                    {
                        var nameable = entity.Read<NameableInteractable>();
                        var entityName = nameable.Name.ToString();
                        
                        // Check if it's a BloodyBoss entity (ends with "bb") OR matches a boss name
                        bool isBloodyBoss = false;
                        
                        // Pattern 1: nameHash + "bb"
                        if (entityName.EndsWith("bb") && entityName.Length >= 34) // 32 chars GUID + "bb"
                        {
                            var nameHash = entityName.Substring(0, entityName.Length - 2);
                            isBloodyBoss = Database.BOSSES.Any(b => b.nameHash == nameHash);
                        }
                        
                        // Pattern 2: Direct boss name match
                        if (!isBloodyBoss)
                        {
                            isBloodyBoss = Database.BOSSES.Any(b => b.name == entityName);
                        }
                        
                        // If it's a BloodyBoss pattern, remove it
                        if (isBloodyBoss || (entityName.EndsWith("bb") && entityName.Length >= 34))
                        {
                            // Clear drop table before destroying to prevent vanilla drops
                            try
                            {
                                var dropTableBuffer = entity.ReadBuffer<DropTableBuffer>();
                                dropTableBuffer.Clear();
                                Plugin.BLogger.Debug(LogCategory.Boss, $"Cleared drop table for boss entity: {entityName}");
                            }
                            catch (Exception ex)
                            {
                                Plugin.BLogger.Debug(LogCategory.Boss, $"Could not clear drop table: {ex.Message}");
                            }
                            
                            // This is a BloodyBoss entity (orphaned or not), remove it
                            StatChangeUtility.KillOrDestroyEntity(entityManager, entity, userModel.Entity, userModel.Entity, 0, StatChangeReason.Any, true);
                            bossesRemoved++;
                            Plugin.BLogger.Info(LogCategory.Boss, $"Removed boss entity: {entityName}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.BLogger.Warning(LogCategory.Boss, $"Error checking entity: {ex.Message}");
                    }
                }
                nameableEntities.Dispose();
                
                // 3. Clean up orphaned icon entities
                var iconEntities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
                foreach (var entity in iconEntities)
                {
                    try
                    {
                        var nameable = entity.Read<NameableInteractable>();
                        var entityName = nameable.Name.ToString();
                        
                        // Check if it's a BloodyBoss icon (ends with "ibb")
                        if (entityName.EndsWith("ibb") && entityName.Length >= 35) // 32 chars GUID + "ibb"
                        {
                            var nameHash = entityName.Substring(0, entityName.Length - 3);
                            
                            // Check if this nameHash exists in our database
                            bool foundInDatabase = Database.BOSSES.Any(b => b.nameHash == nameHash);
                            
                            if (!foundInDatabase)
                            {
                                // Orphaned icon entity, remove it
                                StatChangeUtility.KillOrDestroyEntity(entityManager, entity, userModel.Entity, userModel.Entity, 0, StatChangeReason.Any, true);
                                iconsRemoved++;
                                Plugin.BLogger.Info(LogCategory.Boss, $"Removed orphaned icon entity: {entityName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.BLogger.Warning(LogCategory.Boss, $"Error checking icon entity: {ex.Message}");
                    }
                }
                iconEntities.Dispose();
                
                // 4. Clean up tracking systems
                BossTrackingSystem.Clear();
                
                // 5. Save database changes
                if (databaseUpdated > 0)
                {
                    Database.saveDatabase();
                }
                
                // 6. Report results
                ctx.Reply($"{FontColorChatSystem.Green("[CLEANUP]")} Server cleanup completed:");
                ctx.Reply($"├─ Bosses removed: {FontColorChatSystem.Yellow(bossesRemoved.ToString())}");
                ctx.Reply($"├─ Icons removed: {FontColorChatSystem.Yellow(iconsRemoved.ToString())}");
                ctx.Reply($"├─ Database entries updated: {FontColorChatSystem.Yellow(databaseUpdated.ToString())}");
                ctx.Reply($"└─ Tracking systems cleared: {FontColorChatSystem.Green("Yes")}");
                
                Plugin.BLogger.Info(LogCategory.Boss, $"Server cleanup completed: {bossesRemoved} bosses, {iconsRemoved} icons removed, {databaseUpdated} database entries updated");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error during cleanup: {e.Message}");
            }
        }

        // COMENTADO: Comando para exportar la base de datos de VBloods
        // Mantener para futuras actualizaciones del juego
        // Para usar: descomentar temporalmente, ejecutar el comando, copiar el archivo generado
        /*
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
    }
}
