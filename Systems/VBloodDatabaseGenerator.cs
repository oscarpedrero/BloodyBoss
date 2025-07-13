using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Bloody.Core;
using BloodyBoss.Models;
using BloodyBoss.Data;
using Stunlock.Core;

namespace BloodyBoss.Systems
{
    public static class VBloodDatabaseGenerator
    {
        private static readonly string OUTPUT_PATH = "/run/media/trodi/16902D94902D7AFF/MODS/BloodyBoss/Data/VBloodDatabase_Complete.cs";
        
        public static void GenerateCompleteDatabase()
        {
            try
            {
                Plugin.BLogger.Info(LogCategory.System, "Starting complete VBlood database generation from server data...");
                
                // 1. Escanear servidor para obtener información completa
                VBloodPrefabScanner.ScanVBloodPrefabs(true); // Forzar escaneo con QueryComponents
                var serverData = VBloodPrefabScanner.GetAllVBloods();
                
                Plugin.BLogger.Info(LogCategory.System, $"Server scan found {serverData.Count} VBlood entities");
                
                // Contar cuántos tienen habilidades
                int withAbilities = serverData.Values.Count(v => v.Abilities.Count > 0);
                Plugin.BLogger.Info(LogCategory.System, $"Entities with abilities: {withAbilities}");
                
                // 2. Convertir datos del servidor a formato VBloodStaticInfo
                var databaseData = ConvertServerDataToDatabase(serverData);
                
                Plugin.BLogger.Info(LogCategory.System, $"Converted {databaseData.Count} entities to database format");
                
                // 3. Generar archivo VBloodDatabase.cs completo
                GenerateVBloodDatabaseFile(databaseData);
                
                Plugin.BLogger.Info(LogCategory.System, "Complete VBlood database generation finished successfully!");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error generating complete VBlood database: {ex.Message}");
            }
        }
        
        private static Dictionary<int, VBloodStaticInfo> ConvertServerDataToDatabase(Dictionary<int, VBloodInfo> serverData)
        {
            var databaseData = new Dictionary<int, VBloodStaticInfo>();
            
            try
            {
                // Cargar la base de datos existente para conservar nombres originales
                var existingDatabase = VBloodDatabase.GetAllVBloods();
                
                foreach (var kvp in serverData)
                {
                    var guidHash = kvp.Key;
                    var serverInfo = kvp.Value;
                    
                    // Crear VBloodStaticInfo con merge de datos existentes y del servidor
                    var staticInfo = new VBloodStaticInfo
                    {
                        Name = existingDatabase.ContainsKey(guidHash) ? existingDatabase[guidHash].Name : serverInfo.Name,
                        Level = serverInfo.Level,
                        CanFly = serverInfo.CanFly,
                        Features = new HashSet<string>(serverInfo.Features),
                        Abilities = ConvertAbilitiesToStaticFormat(serverInfo.Abilities)
                    };
                    
                    // Si existe en la base de datos, preservar características adicionales
                    if (existingDatabase.ContainsKey(guidHash))
                    {
                        var existing = existingDatabase[guidHash];
                        // Combinar features existentes con las del servidor
                        foreach (var feature in existing.Features)
                        {
                            staticInfo.Features.Add(feature);
                        }
                    }
                    
                    databaseData[guidHash] = staticInfo;
                }
                
                // Agregar entidades que existen en la base de datos pero no en el servidor
                foreach (var existingKvp in existingDatabase)
                {
                    if (!databaseData.ContainsKey(existingKvp.Key))
                    {
                        Plugin.BLogger.Info(LogCategory.System, $"Preserving existing entry not found on server: {existingKvp.Value.Name}");
                        databaseData[existingKvp.Key] = existingKvp.Value;
                    }
                }
                
                Plugin.BLogger.Info(LogCategory.System, $"Merged {serverData.Count} server entries with existing database");
                Plugin.BLogger.Info(LogCategory.System, $"Total database entries: {databaseData.Count}");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error converting server data: {ex.Message}");
            }
            
            return databaseData;
        }
        
        private static Dictionary<int, AbilityStaticInfo> ConvertAbilitiesToStaticFormat(Dictionary<int, VBloodAbilityInfo> abilities)
        {
            var staticAbilities = new Dictionary<int, AbilityStaticInfo>();
            
            foreach (var kvp in abilities)
            {
                var abilityInfo = kvp.Value;
                
                var staticAbility = new AbilityStaticInfo
                {
                    Category = abilityInfo.Category,
                    GUID = abilityInfo.AbilityPrefabGUID.GuidHash,
                    CastTime = abilityInfo.CastTime,
                    PostCastTime = abilityInfo.PostCastTime,
                    HideCastBar = abilityInfo.HideCastBar,
                    IsChanneled = abilityInfo.IsChanneled,
                    IsCombo = abilityInfo.IsCombo,
                    ComboLength = abilityInfo.ComboLength,
                    Cooldown = abilityInfo.Cooldown,
                    Charges = abilityInfo.Charges,
                    RequiresAnimation = abilityInfo.RequiresAnimation,
                    RequiresFlight = abilityInfo.RequiresFlight,
                    CanMoveWhileCasting = abilityInfo.CanMoveWhileCasting,
                    CanRotateWhileCasting = abilityInfo.CanRotateWhileCasting,
                    ExtraData = new Dictionary<string, object>(abilityInfo.ExtraData),
                    SpawnedPrefabs = abilityInfo.SpawnedPrefabs.Select(sp => new SpawnInfo
                    {
                        SpawnPrefab = sp.SpawnPrefab,
                        SpawnName = sp.SpawnName,
                        Target = sp.Target,
                        HoverDistance = sp.HoverDistance,
                        HoverMaxDistance = sp.HoverMaxDistance
                    }).ToList()
                };
                
                staticAbilities[kvp.Key] = staticAbility;
            }
            
            return staticAbilities;
        }
        
        private static void GenerateVBloodDatabaseFile(Dictionary<int, VBloodStaticInfo> data)
        {
            try
            {
                var sb = new StringBuilder();
                
                // Header
                sb.AppendLine("// Auto-generated COMPLETE VBlood Database");
                sb.AppendLine("// Generated from server scan - all entities available on server");
                sb.AppendLine($"// Total entries: {data.Count}");
                sb.AppendLine($"// Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using Stunlock.Core;");
                sb.AppendLine("using BloodyBoss.Models;");
                sb.AppendLine();
                sb.AppendLine("namespace BloodyBoss.Data");
                sb.AppendLine("{");
                sb.AppendLine("    public static class VBloodDatabase_Complete");
                sb.AppendLine("    {");
                sb.AppendLine("        private static readonly Dictionary<int, VBloodStaticInfo> _database = new()");
                sb.AppendLine("        {");
                
                // Generar entradas ordenadas por GUID
                var sortedEntries = data.OrderBy(kvp => kvp.Key).ToList();
                
                for (int i = 0; i < sortedEntries.Count; i++)
                {
                    var kvp = sortedEntries[i];
                    var guidHash = kvp.Key;
                    var info = kvp.Value;
                    
                    // Determinar si tiene habilidades completas
                    bool hasAbilities = info.Abilities.Count > 0;
                    var abilityTag = hasAbilities ? " [WITH_ABILITIES]" : " [NO_ABILITIES]";
                    
                    sb.AppendLine($"            [{guidHash}] = new VBloodStaticInfo // {info.Name}{abilityTag}");
                    sb.AppendLine("            {");
                    sb.AppendLine($"                Name = \"{info.Name.Replace("\"", "\\\"")}\",");
                    sb.AppendLine($"                Level = {info.Level},");
                    sb.AppendLine($"                CanFly = {info.CanFly.ToString().ToLower()},");
                    
                    // Features
                    if (info.Features.Count > 0)
                    {
                        var featuresStr = string.Join(", ", info.Features.Select(f => $"\"{f}\""));
                        sb.AppendLine($"                Features = new HashSet<string> {{ {featuresStr} }},");
                    }
                    else
                    {
                        sb.AppendLine("                Features = new HashSet<string>(),");
                    }
                    
                    // Abilities
                    if (info.Abilities.Count > 0)
                    {
                        sb.AppendLine("                Abilities = new Dictionary<int, AbilityStaticInfo>");
                        sb.AppendLine("                {");
                        
                        var sortedAbilities = info.Abilities.OrderBy(a => a.Key).ToList();
                        for (int j = 0; j < sortedAbilities.Count; j++)
                        {
                            var abilityKvp = sortedAbilities[j];
                            var abilityIndex = abilityKvp.Key;
                            var ability = abilityKvp.Value;
                            
                            sb.AppendLine($"                    [{abilityIndex}] = new AbilityStaticInfo");
                            sb.AppendLine("                    {");
                            sb.AppendLine($"                        Category = AbilityCategory.{ability.Category},");
                            sb.AppendLine($"                        GUID = {ability.GUID},");
                            sb.AppendLine($"                        CastTime = {ability.CastTime.ToString("F1", CultureInfo.InvariantCulture)}f,");
                            sb.AppendLine($"                        PostCastTime = {ability.PostCastTime.ToString("F1", CultureInfo.InvariantCulture)}f,");
                            sb.AppendLine($"                        HideCastBar = {ability.HideCastBar.ToString().ToLower()},");
                            sb.AppendLine($"                        IsChanneled = {ability.IsChanneled.ToString().ToLower()},");
                            sb.AppendLine($"                        IsCombo = {ability.IsCombo.ToString().ToLower()},");
                            sb.AppendLine($"                        ComboLength = {ability.ComboLength},");
                            sb.AppendLine($"                        Cooldown = {ability.Cooldown.ToString("F1", CultureInfo.InvariantCulture)}f,");
                            sb.AppendLine($"                        Charges = {ability.Charges},");
                            sb.AppendLine($"                        RequiresAnimation = {ability.RequiresAnimation.ToString().ToLower()},");
                            sb.AppendLine($"                        RequiresFlight = {ability.RequiresFlight.ToString().ToLower()},");
                            sb.AppendLine($"                        CanMoveWhileCasting = {ability.CanMoveWhileCasting.ToString().ToLower()},");
                            sb.AppendLine($"                        CanRotateWhileCasting = {ability.CanRotateWhileCasting.ToString().ToLower()},");
                            
                            // ExtraData
                            if (ability.ExtraData.Count > 0)
                            {
                                sb.AppendLine("                        ExtraData = new Dictionary<string, object>");
                                sb.AppendLine("                        {");
                                foreach (var extraKvp in ability.ExtraData)
                                {
                                    var value = FormatExtraDataValue(extraKvp.Value);
                                    sb.AppendLine($"                            {{ \"{extraKvp.Key}\", {value} }},");
                                }
                                sb.AppendLine("                        },");
                            }
                            else
                            {
                                sb.AppendLine("                        ExtraData = new Dictionary<string, object>(),");
                            }
                            
                            // SpawnedPrefabs
                            if (ability.SpawnedPrefabs.Count > 0)
                            {
                                sb.AppendLine("                        SpawnedPrefabs = new List<SpawnInfo>");
                                sb.AppendLine("                        {");
                                foreach (var spawn in ability.SpawnedPrefabs)
                                {
                                    sb.AppendLine("                            new SpawnInfo");
                                    sb.AppendLine("                            {");
                                    sb.AppendLine($"                                SpawnPrefab = new PrefabGUID({spawn.SpawnPrefab.GuidHash}),");
                                    sb.AppendLine($"                                SpawnName = \"{spawn.SpawnName?.Replace("\"", "\\\"") ?? "Unknown"}\",");
                                    sb.AppendLine($"                                Target = \"{spawn.Target ?? "Unknown"}\",");
                                    sb.AppendLine($"                                HoverDistance = {spawn.HoverDistance.ToString("F1", CultureInfo.InvariantCulture)}f,");
                                    sb.AppendLine($"                                HoverMaxDistance = {spawn.HoverMaxDistance.ToString("F1", CultureInfo.InvariantCulture)}f");
                                    sb.AppendLine("                            },");
                                }
                                sb.AppendLine("                        },");
                            }
                            else
                            {
                                sb.AppendLine("                        SpawnedPrefabs = new List<SpawnInfo>(),");
                            }
                            
                            // Cerrar ability
                            if (j < sortedAbilities.Count - 1)
                            {
                                sb.AppendLine("                    },");
                            }
                            else
                            {
                                sb.AppendLine("                    }");
                            }
                        }
                        
                        sb.AppendLine("                }");
                    }
                    else
                    {
                        sb.AppendLine("                Abilities = new Dictionary<int, AbilityStaticInfo>()");
                    }
                    
                    // Cerrar entrada
                    if (i < sortedEntries.Count - 1)
                    {
                        sb.AppendLine("            },");
                    }
                    else
                    {
                        sb.AppendLine("            }");
                    }
                }
                
                // Footer
                sb.AppendLine("        };");
                sb.AppendLine();
                sb.AppendLine("        public static Dictionary<int, VBloodStaticInfo> GetAllVBloods() => new Dictionary<int, VBloodStaticInfo>(_database);");
                sb.AppendLine("        public static VBloodStaticInfo GetVBlood(int guidHash) => _database.TryGetValue(guidHash, out var info) ? info : null;");
                sb.AppendLine("        public static int Count => _database.Count;");
                sb.AppendLine("    }");
                sb.AppendLine("}");
                
                // Escribir archivo
                Directory.CreateDirectory(Path.GetDirectoryName(OUTPUT_PATH));
                File.WriteAllText(OUTPUT_PATH, sb.ToString());
                
                Plugin.BLogger.Info(LogCategory.System, $"Generated complete VBlood database: {OUTPUT_PATH}");
                Plugin.BLogger.Info(LogCategory.System, $"Database contains {data.Count} total entries from server");
                
                // Estadísticas
                var withAbilities = data.Values.Count(v => v.Abilities.Count > 0);
                var withoutAbilities = data.Count - withAbilities;
                var canFly = data.Values.Count(v => v.CanFly);
                
                Plugin.BLogger.Info(LogCategory.System, $"- With abilities: {withAbilities} entries");
                Plugin.BLogger.Info(LogCategory.System, $"- Without abilities: {withoutAbilities} entries");
                Plugin.BLogger.Info(LogCategory.System, $"- Can fly: {canFly} entries");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error generating VBlood database file: {ex.Message}");
            }
        }
        
        private static string FormatExtraDataValue(object value)
        {
            if (value == null) return "null";
            
            if (value is string str)
                return $"\"{str.Replace("\"", "\\\"")}\"";
            else if (value is bool b)
                return b.ToString().ToLower();
            else if (value is float f)
                return $"{f.ToString("F1", CultureInfo.InvariantCulture)}f";
            else if (value is double d)
                return $"{d.ToString("F1", CultureInfo.InvariantCulture)}";
            else if (value is int i)
                return i.ToString();
            else
                return $"\"{value.ToString().Replace("\"", "\\\"")}\"";
        }
        
    }
}