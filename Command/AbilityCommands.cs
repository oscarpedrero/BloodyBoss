using System;
using System.Collections.Generic;
using System.Linq;
using Bloody.Core.Models.v1;
using BloodyBoss.Data;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Systems;
using VampireCommandFramework;
using Bloody.Core.API.v1;

namespace BloodyBoss.Command
{
    public static partial class BossCommand
    {
        [Command("ability-info", usage: "<VBloodName> [SlotIndex]", description: "Show detailed ability information from a VBlood", adminOnly: true)]
        public static void ShowAbilityInfo(ChatCommandContext ctx, string vbloodName, int slotIndex = -1)
        {
            var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
            var vblood = knownVBloods.FirstOrDefault(x => x.Key.ToLower().Contains(vbloodName.ToLower()));
            
            if (vblood.Key == null)
            {
                ctx.Reply(FontColorChatSystem.Red($"VBlood '{vbloodName}' not found"));
                ctx.Reply("Use .bb ability-list to see available VBloods");
                Plugin.Logger.LogDebug($"VBlood '{vbloodName}' not found in database");
                return;
            }

            var vbloodInfo = VBloodDatabase.GetVBlood(vblood.Value);
            if (vbloodInfo == null)
            {
                ctx.Reply($"❌ No database information for {vblood.Key}");
                return;
            }

            ctx.Reply($"🩸 {vbloodInfo.Name} (Level {vbloodInfo.Level})");
            ctx.Reply($"├─ Category: {string.Join(", ", vbloodInfo.Features)}");
            ctx.Reply($"├─ Can Fly: {(vbloodInfo.CanFly ? "Yes" : "No")}");
            ctx.Reply($"└─ Abilities: {vbloodInfo.Abilities.Count}");

            if (slotIndex == -1)
            {
                // Show all abilities
                foreach (var ability in vbloodInfo.Abilities)
                {
                    ShowAbilityDetails(ctx, ability.Key, ability.Value, "  ");
                }
            }
            else if (vbloodInfo.Abilities.TryGetValue(slotIndex, out var ability))
            {
                ShowAbilityDetails(ctx, slotIndex, ability, "");
            }
            else
            {
                ctx.Reply($"❌ No ability at slot {slotIndex}");
            }
        }

        private static void ShowAbilityDetails(ChatCommandContext ctx, int slot, AbilityStaticInfo ability, string prefix)
        {
            ctx.Reply($"{prefix}🎯 Slot {slot}: {ability.Category}");
            ctx.Reply($"{prefix}├─ GUID: {ability.GUID}");
            
            if (ability.CastTime > 0 || ability.PostCastTime > 0)
            {
                ctx.Reply($"{prefix}├─ Cast: {ability.CastTime}s (post: {ability.PostCastTime}s)");
            }
            
            if (ability.IsCombo)
            {
                ctx.Reply($"{prefix}├─ Combo: {ability.ComboLength} hits");
            }
            
            if (ability.Cooldown > 0)
            {
                ctx.Reply($"{prefix}├─ Cooldown: {ability.Cooldown}s");
            }
            
            if (ability.Charges > 0)
            {
                ctx.Reply($"{prefix}├─ Charges: {ability.Charges}");
            }
            
            var features = new List<string>();
            if (ability.IsChanneled) features.Add("Channeled");
            if (ability.RequiresFlight) features.Add("Flight Required");
            if (ability.CanMoveWhileCasting) features.Add("Move While Cast");
            if (ability.CanRotateWhileCasting) features.Add("Rotate While Cast");
            if (ability.HideCastBar) features.Add("Hidden Cast Bar");
            
            if (features.Count > 0)
            {
                ctx.Reply($"{prefix}├─ Features: {string.Join(", ", features)}");
            }
            
            if (ability.SpawnedPrefabs.Count > 0)
            {
                ctx.Reply($"{prefix}├─ Spawns: {ability.SpawnedPrefabs.Count} projectile(s)");
                if (prefix == "") // Only show details for single ability view
                {
                    foreach (var spawn in ability.SpawnedPrefabs)
                    {
                        ctx.Reply($"{prefix}│  └─ {spawn.SpawnName}");
                    }
                }
            }
            
            if (ability.ExtraData.Count > 0 && ability.ExtraData.ContainsKey("BehaviorType"))
            {
                ctx.Reply($"{prefix}└─ Behavior: {ability.ExtraData["BehaviorType"]}");
            }
            else
            {
                ctx.Reply($"{prefix}└─ [End]");
            }
        }

        [Command("ability-suggest", usage: "<BossName> [Category]", description: "Suggest compatible abilities for a boss", adminOnly: true)]
        public static void SuggestAbilities(ChatCommandContext ctx, string bossName, string category = "")
        {
            if (!Database.GetBoss(bossName, out BossEncounterModel boss))
            {
                ctx.Reply($"❌ Boss '{bossName}' not found");
                return;
            }

            var bossInfo = VBloodDatabase.GetVBlood(boss.PrefabGUID);
            if (bossInfo == null)
            {
                ctx.Reply($"❌ No database information for boss");
                return;
            }

            ctx.Reply($"🎯 Compatible abilities for {bossInfo.Name}:");
            ctx.Reply($"├─ Boss Category: {string.Join(", ", bossInfo.Features)}");
            ctx.Reply($"├─ Can Fly: {(bossInfo.CanFly ? "Yes" : "No")}");
            ctx.Reply($"└─ Searching for compatible abilities...");

            var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
            var suggestions = new List<(string vbloodName, int slot, AbilityStaticInfo ability, AbilityCompatibilitySystem.CompatibilityLevel level)>();

            // Check all VBloods
            foreach (var vblood in knownVBloods)
            {
                var sourceInfo = VBloodDatabase.GetVBlood(vblood.Value);
                if (sourceInfo == null) continue;

                foreach (var abilityKvp in sourceInfo.Abilities)
                {
                    // Filter by category if specified
                    if (!string.IsNullOrEmpty(category) && 
                        !abilityKvp.Value.Category.ToString().ToLower().Contains(category.ToLower()))
                        continue;

                    var compatibility = AbilityCompatibilitySystem.CheckAbilityCompatibility(
                        boss.PrefabGUID, vblood.Value, abilityKvp.Key);

                    if (compatibility.IsCompatible)
                    {
                        suggestions.Add((vblood.Key, abilityKvp.Key, abilityKvp.Value, compatibility.Level));
                    }
                }
            }

            // Sort by compatibility level and name
            var sortedSuggestions = suggestions
                .OrderBy(s => s.level)
                .ThenBy(s => s.vbloodName)
                .ThenBy(s => s.slot)
                .Take(15); // Limit to 15 suggestions

            ctx.Reply("");
            ctx.Reply($"Found {suggestions.Count} compatible abilities:");
            
            foreach (var suggestion in sortedSuggestions)
            {
                var icon = GetCompatibilityIcon(suggestion.level);
                ctx.Reply($"{icon} {suggestion.vbloodName} - Slot {suggestion.slot} ({suggestion.ability.Category})");
                
                if (suggestion.ability.CastTime > 0)
                {
                    ctx.Reply($"   └─ Cast: {suggestion.ability.CastTime}s");
                }
            }

            if (suggestions.Count > 15)
            {
                ctx.Reply($"... and {suggestions.Count - 15} more");
            }
        }

        [Command("ability-test", usage: "<BossName> <SourceVBlood> <SlotIndex>", description: "Test ability compatibility before configuring", adminOnly: true)]
        public static void TestAbilityCompatibility(ChatCommandContext ctx, string bossName, string sourceVBlood, int slotIndex)
        {
            if (!Database.GetBoss(bossName, out BossEncounterModel boss))
            {
                ctx.Reply($"❌ Boss '{bossName}' not found");
                return;
            }

            var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
            var source = knownVBloods.FirstOrDefault(x => x.Key.ToLower().Contains(sourceVBlood.ToLower()));
            
            if (source.Key == null)
            {
                ctx.Reply($"❌ VBlood '{sourceVBlood}' not found");
                return;
            }

            var compatibility = AbilityCompatibilitySystem.CheckAbilityCompatibility(
                boss.PrefabGUID, source.Value, slotIndex);

            var bossInfo = VBloodDatabase.GetVBlood(boss.PrefabGUID);
            var sourceInfo = VBloodDatabase.GetVBlood(source.Value);

            ctx.Reply($"🧪 Compatibility Test:");
            ctx.Reply($"├─ Boss: {bossInfo?.Name ?? bossName}");
            ctx.Reply($"├─ Source: {sourceInfo?.Name ?? source.Key}");
            ctx.Reply($"├─ Slot: {slotIndex}");
            ctx.Reply($"└─ Result: {GetCompatibilityIcon(compatibility.Level)} {compatibility.Level}");

            if (sourceInfo?.Abilities.TryGetValue(slotIndex, out var ability) == true)
            {
                ctx.Reply("");
                ctx.Reply($"📊 Ability Details:");
                ShowAbilityDetails(ctx, slotIndex, ability, "");
            }

            if (compatibility.Errors.Count > 0)
            {
                ctx.Reply("");
                ctx.Reply($"❌ Errors:");
                foreach (var error in compatibility.Errors)
                {
                    ctx.Reply($"├─ {error}");
                }
            }

            if (compatibility.Warnings.Count > 0)
            {
                ctx.Reply("");
                ctx.Reply($"⚠️ Warnings:");
                foreach (var warning in compatibility.Warnings)
                {
                    ctx.Reply($"├─ {warning}");
                }
            }

            if (compatibility.IsCompatible)
            {
                ctx.Reply("");
                ctx.Reply($"✅ This ability can be used with command:");
                ctx.Reply($".bb ability-slot \"{bossName}\" \"custom_slot\" {source.Value} {slotIndex} true \"Test ability\"");
            }
        }

        [Command("ability-slot", usage: "<BossName> <SourceVBlood> <SlotIndex> <EnableSwap> [Description]", description: "Configure ability slot for a boss", adminOnly: true)]
        public static void ConfigureAbilitySlot(ChatCommandContext ctx, string bossName, string sourceVBlood, int slotIndex, bool enableSwap, string description = "")
        {
            try
            {
                if (!Database.GetBoss(bossName, out BossEncounterModel boss))
                {
                    ctx.Reply($"❌ Boss '{bossName}' not found");
                    return;
                }

                var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
                var source = knownVBloods.FirstOrDefault(x => x.Key.ToLower().Contains(sourceVBlood.ToLower()));
                
                if (source.Key == null)
                {
                    ctx.Reply($"❌ VBlood '{sourceVBlood}' not found");
                    ctx.Reply("Use .bb ability-info to see available VBloods");
                    return;
                }

                // Check compatibility first
                var result = AbilityCompatibilitySystem.CheckAbilityCompatibility(
                    boss.PrefabGUID, 
                    source.Value, 
                    slotIndex
                );

                if (!result.IsCompatible)
                {
                    ctx.Reply($"❌ Ability is incompatible!");
                    foreach (var error in result.Errors)
                    {
                        ctx.Reply($"  • {error}");
                    }
                    return;
                }

                // Configure the ability swap
                if (boss.AbilitySwaps == null)
                {
                    boss.AbilitySwaps = new Dictionary<int, AbilitySwapConfig>();
                }

                if (enableSwap)
                {
                    boss.AbilitySwaps[slotIndex] = new AbilitySwapConfig
                    {
                        SourcePrefabGUID = source.Value,
                        SourceVBloodName = source.Key,
                        SlotIndex = slotIndex,
                        Description = string.IsNullOrEmpty(description) ? $"Ability from {source.Key}" : description
                    };
                    
                    Database.saveDatabase();
                    
                    var icon = GetCompatibilityIcon(result.Level);
                    ctx.Reply($"{icon} Configured slot {slotIndex} with ability from {source.Key}");
                    
                    if (result.Warnings.Any())
                    {
                        ctx.Reply(FontColorChatSystem.Yellow("Warnings:"));
                        foreach (var warning in result.Warnings)
                        {
                            ctx.Reply($"  - {FontColorChatSystem.Yellow(warning)}");
                            Plugin.Logger.LogDebug($"Ability compatibility warning: {warning}");
                        }
                    }
                }
                else
                {
                    if (boss.AbilitySwaps != null && boss.AbilitySwaps.ContainsKey(slotIndex))
                    {
                        boss.AbilitySwaps.Remove(slotIndex);
                        Database.saveDatabase();
                        ctx.Reply($"✅ Removed ability swap from slot {slotIndex}");
                    }
                    else
                    {
                        ctx.Reply($"ℹ️ No ability swap configured for slot {slotIndex}");
                    }
                }
            }
            catch (Exception e)
            {
                ctx.Reply($"❌ Error configuring ability: {e.Message}");
                Plugin.Logger.LogError($"Error in ability-slot command: {e}");
            }
        }

        [Command("vblood-docs", usage: "", description: "Export VBlood documentation to markdown file", adminOnly: true)]
        public static void ExportVBloodDocumentation(ChatCommandContext ctx)
        {
            try
            {
                var outputPath = System.IO.Path.Combine(Database.ConfigPath, "VBlood_Abilities_Documentation.md");
                var docLines = new List<string>();
                
                // Header
                docLines.Add("# V Rising VBlood Abilities Documentation");
                docLines.Add($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                docLines.Add("");
                docLines.Add("This document lists all available VBlood bosses and their abilities that can be used with the BloodyBoss mod.");
                docLines.Add("");
                docLines.Add("## Command Usage");
                docLines.Add("```");
                docLines.Add(".bb ability-slot <BossName> <VBloodName> <SlotIndex> true <Description>");
                docLines.Add("```");
                docLines.Add("");
                docLines.Add("## Available VBloods and Their Abilities");
                docLines.Add("");
                
                var allVBloods = VBloodDatabase.GetAllVBloods().OrderBy(v => v.Value.Level).ThenBy(v => v.Value.Name);
                
                foreach (var vblood in allVBloods)
                {
                    var info = vblood.Value;
                    docLines.Add($"### {info.Name} (Level {info.Level})");
                    docLines.Add($"- **GUID**: {vblood.Key}");
                    docLines.Add($"- **Can Fly**: {(info.CanFly ? "Yes" : "No")}");
                    docLines.Add($"- **Features**: {string.Join(", ", info.Features)}");
                    docLines.Add($"- **Total Abilities**: {info.Abilities.Count}");
                    docLines.Add("");
                    
                    if (info.Abilities.Count > 0)
                    {
                        docLines.Add("#### Available Ability Slots:");
                        docLines.Add("| Slot | Category | Cast Time | Description |");
                        docLines.Add("|------|----------|-----------|-------------|");
                        
                        foreach (var ability in info.Abilities.OrderBy(a => a.Key))
                        {
                            var slot = ability.Key;
                            var abilityInfo = ability.Value;
                            
                            var description = GetAbilityDescription(abilityInfo);
                            var castTime = abilityInfo.CastTime > 0 ? $"{abilityInfo.CastTime}s" : "Instant";
                            
                            docLines.Add($"| {slot} | {abilityInfo.Category} | {castTime} | {description} |");
                        }
                    }
                    
                    docLines.Add("");
                    docLines.Add("**Example Commands:**");
                    
                    // Provide example commands for the first 3 abilities
                    var examples = info.Abilities.Take(3);
                    foreach (var example in examples)
                    {
                        var simpleName = info.Name.Replace(" the ", " ").Replace(" ", "_");
                        docLines.Add($"```");
                        docLines.Add($".bb ability-slot \"YourBoss\" \"{info.Name}\" {example.Key} true \"{GetAbilityDescription(example.Value)}\"");
                        docLines.Add($"```");
                    }
                    
                    docLines.Add("");
                    docLines.Add("---");
                    docLines.Add("");
                }
                
                // Footer with additional information
                docLines.Add("## Compatibility Notes");
                docLines.Add("");
                docLines.Add("- **Beast** abilities work best on beast-type bosses");
                docLines.Add("- **Humanoid** abilities work best on humanoid/vampire bosses");
                docLines.Add("- **Flight** abilities require bosses that can fly");
                docLines.Add("- **Transformation** abilities may have visual issues on incompatible models");
                docLines.Add("");
                docLines.Add("## Tips");
                docLines.Add("");
                docLines.Add("1. Use `.bb ability-suggest <BossName>` to get compatible ability suggestions");
                docLines.Add("2. Use `.bb ability-test <BossName> <VBloodName> <Slot>` to test compatibility before applying");
                docLines.Add("3. Some abilities may have warnings but still work with minor visual issues");
                docLines.Add("4. Avoid mixing abilities from very different creature types (e.g., spider abilities on humanoids)");
                
                // Write to file
                System.IO.File.WriteAllLines(outputPath, docLines);
                
                ctx.Reply(FontColorChatSystem.Green($"VBlood documentation exported successfully!"));
                ctx.Reply($"File location: {outputPath}");
                ctx.Reply($"Total VBloods documented: {allVBloods.Count()}");
                
                Plugin.Logger.LogInfo($"VBlood documentation exported to {outputPath}");
            }
            catch (Exception ex)
            {
                ctx.Reply(FontColorChatSystem.Red($"Error exporting documentation: {ex.Message}"));
                Plugin.Logger.LogError($"Error exporting VBlood documentation: {ex}");
            }
        }
        
        private static string GetAbilityDescription(AbilityStaticInfo ability)
        {
            var parts = new List<string>();
            
            if (ability.IsCombo)
                parts.Add($"{ability.ComboLength}-hit combo");
            
            if (ability.IsChanneled)
                parts.Add("Channeled");
                
            if (ability.RequiresFlight)
                parts.Add("Requires flight");
                
            if (ability.SpawnedPrefabs.Count > 0)
            {
                var spawnTypes = ability.SpawnedPrefabs.Select(s => 
                {
                    if (s.SpawnName.Contains("Projectile")) return "projectile";
                    if (s.SpawnName.Contains("Buff")) return "buff";
                    if (s.SpawnName.Contains("Area")) return "AoE";
                    if (s.SpawnName.Contains("Travel")) return "movement";
                    return "effect";
                }).Distinct();
                
                parts.Add(string.Join("/", spawnTypes));
            }
            
            if (ability.ExtraData.ContainsKey("BehaviorType"))
            {
                var behavior = ability.ExtraData["BehaviorType"].ToString();
                if (behavior != "None" && !parts.Any(p => p.ToLower().Contains(behavior.ToLower())))
                    parts.Add(behavior);
            }
            
            return parts.Count > 0 ? string.Join(", ", parts) : ability.Category.ToString();
        }
    }
}