using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Entities;
using Unity.Collections;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using ProjectM;
using Stunlock.Core;
using ProjectM.Shared;
using ProjectM.Gameplay.Systems;
using ProjectM.Network;

namespace BloodyBoss.Systems
{
    public static class BuffAnalyzer
    {
        private static Dictionary<int, BuffAnalysisResult> _buffCache = new();
        private static List<string> _importantComponents = new()
        {
            // Damage/Defense
            "ModifyUnitStatBuff",
            "DamageReductionBuff",
            "AbsorbBuff", 
            "ShieldBuff",
            "ProtectedBuff",
            "DamageOverTime",
            "BloodBuff",
            
            // Movement/Control
            "ModifyMovementSpeedBuff",
            "Immobilize",
            "Stun",
            "Fear",
            "Freeze",
            "Root",
            "Slow",
            "Knockback",
            
            // Health/Healing
            "LifeLeech",
            "HealOverTime",
            "HealthRegenerationBuff",
            "ModifyHealthBuff",
            
            // Visual/Other
            "Script_Buff",
            "Buff",
            "ModifyAttackSpeed",
            "Channeling",
            "CC_Immunity",
            "Invulnerable"
        };

        public static void AnalyzeAllBuffs()
        {
            try
            {
                Plugin.Logger.LogWarning("=== Starting Buff Analysis ===");
                
                // Parse buff prefabs from file
                var buffPrefabs = ParseBuffsFromFile();
                Plugin.Logger.LogWarning($"Found {buffPrefabs.Count} buff prefabs to analyze");
                
                // Analyze each buff
                int analyzed = 0;
                foreach (var (guidHash, name) in buffPrefabs)
                {
                    if (analyzed % 100 == 0)
                    {
                        Plugin.Logger.LogWarning($"Progress: {analyzed}/{buffPrefabs.Count} buffs analyzed...");
                    }
                    
                    AnalyzeBuff(guidHash, name);
                    analyzed++;
                }
                
                // Generate report
                GenerateBuffReport();
                
                Plugin.Logger.LogWarning($"=== Buff Analysis Complete! Analyzed {_buffCache.Count} buffs ===");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Error in buff analysis: {ex}");
            }
        }

        private static List<(int guidHash, string name)> ParseBuffsFromFile()
        {
            var buffs = new List<(int, string)>();
            var filePath = "/home/trodi/vrising_server/BepInEx/config/BloodyBoss/all_prefabs.md";
            
            if (!File.Exists(filePath))
            {
                Plugin.Logger.LogError($"File not found: {filePath}");
                return buffs;
            }
            
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (line.Contains("Buff") && line.Contains("|"))
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 3)
                    {
                        var guidStr = parts[1].Trim();
                        var namePart = parts[2].Trim();
                        
                        if (int.TryParse(guidStr, out int guidHash))
                        {
                            // Extract name from "Name PrefabGuid(xxx)" format
                            var name = namePart.Split(' ')[0];
                            buffs.Add((guidHash, name));
                        }
                    }
                }
            }
            
            return buffs;
        }

        private static void AnalyzeBuff(int guidHash, string buffName)
        {
            try
            {
                var prefabGuid = new PrefabGUID(guidHash);
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                
                if (prefabSystem == null || !prefabSystem._PrefabGuidToEntityMap.TryGetValue(prefabGuid, out Entity buffEntity))
                {
                    return;
                }
                
                var result = new BuffAnalysisResult
                {
                    GuidHash = guidHash,
                    Name = buffName,
                    Components = new List<string>(),
                    Effects = new List<string>()
                };
                
                // Get all components
                var entityManager = Core.World.EntityManager;
                var componentTypes = entityManager.GetComponentTypes(buffEntity, Allocator.Temp);
                
                foreach (var componentType in componentTypes)
                {
                    var typeName = componentType.ToString();
                    result.Components.Add(typeName);
                    
                    // Check for important components
                    foreach (var important in _importantComponents)
                    {
                        if (typeName.Contains(important))
                        {
                            AnalyzeBuffComponent(buffEntity, componentType, result);
                            break;
                        }
                    }
                }
                
                componentTypes.Dispose();
                
                // Categorize buff
                CategorizeBuffEffects(result);
                
                if (result.Effects.Count > 0 || result.Category != BuffCategory.Visual)
                {
                    _buffCache[guidHash] = result;
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogDebug($"Error analyzing buff {buffName}: {ex.Message}");
            }
        }

        private static void AnalyzeBuffComponent(Entity entity, ComponentType componentType, BuffAnalysisResult result)
        {
            try
            {
                var typeName = componentType.ToString();
                
                // Movement speed modifications
                if (typeName.Contains("ModifyMovementSpeedBuff"))
                {
                    result.Effects.Add("Modifies movement speed");
                    result.HasGameplayEffect = true;
                }
                // Damage modifications
                else if (typeName.Contains("ModifyUnitStatBuff") || typeName.Contains("DamageReduction"))
                {
                    result.Effects.Add("Modifies combat stats");
                    result.HasGameplayEffect = true;
                }
                // Shield/Absorb
                else if (typeName.Contains("AbsorbBuff") || typeName.Contains("ShieldBuff"))
                {
                    result.Effects.Add("Provides damage absorption/shield");
                    result.HasGameplayEffect = true;
                }
                // Control effects
                else if (typeName.Contains("Stun") || typeName.Contains("Root") || typeName.Contains("Fear") || 
                         typeName.Contains("Freeze") || typeName.Contains("Immobilize"))
                {
                    result.Effects.Add($"Applies {typeName} control effect");
                    result.HasGameplayEffect = true;
                }
                // Healing effects
                else if (typeName.Contains("Heal") || typeName.Contains("LifeLeech"))
                {
                    result.Effects.Add("Provides healing");
                    result.HasGameplayEffect = true;
                }
                // Invulnerability
                else if (typeName.Contains("Invulnerable") || typeName.Contains("CC_Immunity"))
                {
                    result.Effects.Add("Provides immunity/invulnerability");
                    result.HasGameplayEffect = true;
                }
                // Damage over time
                else if (typeName.Contains("DamageOverTime") || typeName.Contains("DoT"))
                {
                    result.Effects.Add("Applies damage over time");
                    result.HasGameplayEffect = true;
                }
                // Attack speed
                else if (typeName.Contains("ModifyAttackSpeed"))
                {
                    result.Effects.Add("Modifies attack speed");
                    result.HasGameplayEffect = true;
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogDebug($"Error analyzing component {componentType}: {ex.Message}");
            }
        }

        private static void CategorizeBuffEffects(BuffAnalysisResult result)
        {
            if (!result.HasGameplayEffect)
            {
                result.Category = BuffCategory.Visual;
                return;
            }
            
            // Check for specific categories based on effects
            var effects = string.Join(" ", result.Effects).ToLower();
            
            if (effects.Contains("shield") || effects.Contains("absorb") || effects.Contains("damage reduction"))
            {
                result.Category = BuffCategory.Defensive;
            }
            else if (effects.Contains("movement") || effects.Contains("speed"))
            {
                result.Category = BuffCategory.Movement;
            }
            else if (effects.Contains("stun") || effects.Contains("root") || effects.Contains("fear") || 
                     effects.Contains("freeze") || effects.Contains("immobilize"))
            {
                result.Category = BuffCategory.Control;
            }
            else if (effects.Contains("heal") || effects.Contains("leech"))
            {
                result.Category = BuffCategory.Healing;
            }
            else if (effects.Contains("damage") || effects.Contains("attack"))
            {
                result.Category = BuffCategory.Offensive;
            }
            else
            {
                result.Category = BuffCategory.Other;
            }
        }

        private static void GenerateBuffReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("# V Rising Buff Analysis Report");
            sb.AppendLine();
            sb.AppendLine($"Generated: {DateTime.Now}");
            sb.AppendLine($"Total buffs analyzed: {_buffCache.Count}");
            sb.AppendLine();
            
            // Group by category
            var categories = Enum.GetValues<BuffCategory>();
            foreach (var category in categories)
            {
                var buffsInCategory = _buffCache.Values.Where(b => b.Category == category).ToList();
                if (buffsInCategory.Count == 0) continue;
                
                sb.AppendLine($"## {category} Buffs ({buffsInCategory.Count})");
                sb.AppendLine();
                
                foreach (var buff in buffsInCategory.OrderBy(b => b.Name))
                {
                    sb.AppendLine($"### {buff.Name}");
                    sb.AppendLine($"- **GUID**: {buff.GuidHash}");
                    sb.AppendLine($"- **Has Gameplay Effect**: {(buff.HasGameplayEffect ? "Yes" : "No")}");
                    
                    if (buff.Effects.Count > 0)
                    {
                        sb.AppendLine("- **Effects**:");
                        foreach (var effect in buff.Effects)
                        {
                            sb.AppendLine($"  - {effect}");
                        }
                    }
                    
                    sb.AppendLine("- **Key Components**:");
                    foreach (var comp in buff.Components.Where(c => _importantComponents.Any(i => c.Contains(i))))
                    {
                        sb.AppendLine($"  - {comp}");
                    }
                    
                    sb.AppendLine();
                }
            }
            
            // Write to file
            var outputPath = "/run/media/trodi/16902D94902D7AFF/MODS/BloodyBoss/docs/BUFF_ANALYSIS.md";
            File.WriteAllText(outputPath, sb.ToString());
            Plugin.Logger.LogWarning($"Buff analysis report saved to: {outputPath}");
        }

        private class BuffAnalysisResult
        {
            public int GuidHash { get; set; }
            public string Name { get; set; }
            public List<string> Components { get; set; }
            public List<string> Effects { get; set; }
            public bool HasGameplayEffect { get; set; }
            public BuffCategory Category { get; set; }
        }

        private enum BuffCategory
        {
            Visual,
            Defensive,
            Offensive,
            Movement,
            Control,
            Healing,
            Other
        }
    }
}