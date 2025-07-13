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
using ProjectM;
using Unity.Entities;
using Stunlock.Core;
using BloodyBoss.Exceptions;
using Bloody.Core;

namespace BloodyBoss.Command
{
    [CommandGroup("bb")]
    public static class AbilityCommands
    {
        private static KeyValuePair<string, int>? FindVBlood(string searchTerm, ChatCommandContext ctx = null)
        {
            var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
            
            // First try exact match
            var vblood = knownVBloods.FirstOrDefault(x => x.Key.ToLower() == searchTerm.ToLower());
            
            // If not found, try contains
            if (vblood.Key == null)
            {
                vblood = knownVBloods.FirstOrDefault(x => x.Key.ToLower().Contains(searchTerm.ToLower()));
            }
            
            // If still not found, try matching individual words
            if (vblood.Key == null)
            {
                var searchWords = searchTerm.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var matches = knownVBloods.Where(x => 
                    searchWords.All(word => x.Key.ToLower().Contains(word))
                ).ToList();
                
                if (matches.Count == 1)
                {
                    vblood = matches.First();
                }
                else if (matches.Count > 1 && ctx != null)
                {
                    ctx.Reply($"Multiple VBloods found matching '{searchTerm}':");
                    foreach (var match in matches.Take(5))
                    {
                        ctx.Reply($"  - {match.Key}");
                    }
                    if (matches.Count > 5)
                    {
                        ctx.Reply($"  ... and {matches.Count - 5} more");
                    }
                    return null;
                }
            }
            
            return vblood.Key != null ? vblood : (KeyValuePair<string, int>?)null;
        }
        [Command("ability-debug", usage: "<SearchTerm>", description: "Debug VBlood search", adminOnly: true)]
        public static void DebugVBloodSearch(ChatCommandContext ctx, string searchTerm)
        {
            var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
            ctx.Reply($"{FontColorChatSystem.Blue("[DEBUG]")} Debugging search for: '{searchTerm}'");
            ctx.Reply($"Total VBloods loaded: {knownVBloods.Count}");
            
            // Show first 10 VBloods for reference
            ctx.Reply($"Sample VBloods:");
            foreach (var vb in knownVBloods.Take(10))
            {
                ctx.Reply($"  - '{vb.Key}'");
            }
            
            // Test exact match
            var exactMatch = knownVBloods.FirstOrDefault(x => x.Key.ToLower() == searchTerm.ToLower());
            ctx.Reply($"Exact match: {(exactMatch.Key != null ? exactMatch.Key : "None")}");
            
            // Test contains
            var containsMatches = knownVBloods.Where(x => x.Key.ToLower().Contains(searchTerm.ToLower())).ToList();
            ctx.Reply($"Contains matches: {containsMatches.Count}");
            if (containsMatches.Any())
            {
                foreach (var match in containsMatches.Take(5))
                {
                    ctx.Reply($"  - '{match.Key}'");
                }
            }
        }
        
        [Command("ability-info", usage: "<VBloodName> [SlotIndex]", description: "Show detailed ability information from a VBlood", adminOnly: true)]
        public static void ShowAbilityInfo(ChatCommandContext ctx, string vbloodName, int slotIndex = -1)
        {
            var result = FindVBlood(vbloodName, ctx);
            if (!result.HasValue)
            {
                ctx.Reply($"{FontColorChatSystem.Red($"VBlood '{vbloodName}' not found")}");
                ctx.Reply("Use .bb ability-list to see available VBloods");
                return;
            }
            
            var vblood = result.Value;

            var vbloodInfo = VBloodDatabase.GetVBlood(vblood.Value);
            if (vbloodInfo == null)
            {
                ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} No database information for {vblood.Key}");
                return;
            }

            ctx.Reply($"{FontColorChatSystem.Blue("[VBLOOD]")} {vbloodInfo.Name} (Level {vbloodInfo.Level})");
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
                ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} No ability at slot {slotIndex}");
            }
        }

        private static void ShowAbilityDetails(ChatCommandContext ctx, int slot, AbilityStaticInfo ability, string prefix)
        {
            ctx.Reply($"{prefix}{FontColorChatSystem.Blue("[SLOT]")} {slot}: {ability.Category}");
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
                ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} Boss '{bossName}' not found");
                return;
            }

            var bossInfo = VBloodDatabase.GetVBlood(boss.PrefabGUID);
            if (bossInfo == null)
            {
                ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} No database information for boss");
                return;
            }

            ctx.Reply($"{FontColorChatSystem.Blue("[ABILITIES]")} Compatible abilities for {bossInfo.Name}:");
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
                ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} Boss '{bossName}' not found");
                return;
            }

            var result = FindVBlood(sourceVBlood, ctx);
            if (!result.HasValue)
            {
                ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} VBlood '{sourceVBlood}' not found");
                return;
            }
            
            var source = result.Value;

            var compatibility = AbilityCompatibilitySystem.CheckAbilityCompatibility(
                boss.PrefabGUID, source.Value, slotIndex);

            var bossInfo = VBloodDatabase.GetVBlood(boss.PrefabGUID);
            var sourceInfo = VBloodDatabase.GetVBlood(source.Value);

            ctx.Reply($"{FontColorChatSystem.Blue("[TEST]")} Compatibility Test:");
            ctx.Reply($"├─ Boss: {bossInfo?.Name ?? bossName}");
            ctx.Reply($"├─ Source: {sourceInfo?.Name ?? source.Key}");
            ctx.Reply($"├─ Slot: {slotIndex}");
            ctx.Reply($"└─ Result: {GetCompatibilityIcon(compatibility.Level)} {compatibility.Level}");

            if (sourceInfo?.Abilities.TryGetValue(slotIndex, out var ability) == true)
            {
                ctx.Reply("");
                ctx.Reply($"{FontColorChatSystem.Blue("[DETAILS]")} Ability Details:");
                ShowAbilityDetails(ctx, slotIndex, ability, "");
            }

            if (compatibility.Errors.Count > 0)
            {
                ctx.Reply("");
                ctx.Reply($"{FontColorChatSystem.Red("[ERRORS]")} Errors:");
                foreach (var error in compatibility.Errors)
                {
                    ctx.Reply($"├─ {error}");
                }
            }

            if (compatibility.Warnings.Count > 0)
            {
                ctx.Reply("");
                ctx.Reply($"{FontColorChatSystem.Yellow("[WARNINGS]")} Warnings:");
                foreach (var warning in compatibility.Warnings)
                {
                    ctx.Reply($"├─ {warning}");
                }
            }

            if (compatibility.IsCompatible)
            {
                ctx.Reply("");
                ctx.Reply($"{FontColorChatSystem.Green("[SUCCESS]")} This ability can be used with command:");
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
                    ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} Boss '{bossName}' not found");
                    return;
                }

                var result = FindVBlood(sourceVBlood, ctx);
                if (!result.HasValue)
                {
                    ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} VBlood '{sourceVBlood}' not found");
                    ctx.Reply("Use .bb ability-info to see available VBloods");
                    return;
                }
                
                var source = result.Value;

                // Check compatibility first
                var compatResult = AbilityCompatibilitySystem.CheckAbilityCompatibility(
                    boss.PrefabGUID, 
                    source.Value, 
                    slotIndex
                );

                if (!compatResult.IsCompatible)
                {
                    ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} Ability is incompatible!");
                    foreach (var error in compatResult.Errors)
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
                    
                    var icon = GetCompatibilityIcon(compatResult.Level);
                    ctx.Reply($"{icon} Configured slot {slotIndex} with ability from {source.Key}");
                    
                    if (compatResult.Warnings.Any())
                    {
                        ctx.Reply($"{FontColorChatSystem.Yellow("[WARNINGS]")} Warnings:");
                        foreach (var warning in compatResult.Warnings)
                        {
                            ctx.Reply($"  - {warning}");
                            Plugin.BLogger.Debug(LogCategory.Command, $"Ability compatibility warning: {warning}");
                        }
                    }
                }
                else
                {
                    if (boss.AbilitySwaps != null && boss.AbilitySwaps.ContainsKey(slotIndex))
                    {
                        boss.AbilitySwaps.Remove(slotIndex);
                        Database.saveDatabase();
                        ctx.Reply($"{FontColorChatSystem.Green("[SUCCESS]")} Removed ability swap from slot {slotIndex}");
                    }
                    else
                    {
                        ctx.Reply($"{FontColorChatSystem.Blue("[INFO]")} No ability swap configured for slot {slotIndex}");
                    }
                }
            }
            catch (Exception e)
            {
                ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} Error configuring ability: {e.Message}");
                Plugin.BLogger.Error(LogCategory.Command, $"Error in ability-slot command: {e}");
            }
        }

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

        // ===== MODULAR ABILITY SYSTEM COMMANDS =====

        [Command("ability-list", usage: "[filter]", description: "List known VBlood PrefabGUIDs for ability swapping", adminOnly: true)]
        public static void ListVBloodPrefabs(ChatCommandContext ctx, string filter = "")
        {
            var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
            
            // Apply filter if provided
            var filteredVBloods = string.IsNullOrEmpty(filter) 
                ? knownVBloods 
                : knownVBloods.Where(kvp => kvp.Key.ToLower().Contains(filter.ToLower())).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            
            ctx.Reply($"{FontColorChatSystem.Blue("[VBLOODS]")} Known VBloods ({filteredVBloods.Count} of {knownVBloods.Count}):");
            ctx.Reply($"────────────────────────────────────────────────");
            
            int count = 0;
            foreach (var vblood in filteredVBloods.OrderBy(kvp => kvp.Key))
            {
                if (count < 20) // Limit output to avoid spam
                {
                    ctx.Reply($"├─ {vblood.Key}: {vblood.Value}");
                }
                count++;
            }
            
            if (count > 20)
            {
                ctx.Reply($"└─ ... and {count - 20} more");
            }
            else if (count > 0)
            {
                ctx.Reply($"└─ Total: {count} VBloods");
            }
            
            if (count == 0)
            {
                ctx.Reply($"No VBloods found matching '{filter}'");
            }
            
            // Debug info
            ctx.Reply($"");
            ctx.Reply($"{FontColorChatSystem.Blue("[EXAMPLES]")} Examples:");
            ctx.Reply($"   .bb ability-info \"alpha\"");
            ctx.Reply($"   .bb ability-list wolf");
        }

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
                        ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} This ability is incompatible with {bossInfo?.Name ?? "this boss"}");
                        foreach (var error in compatibilityResult.Errors)
                        {
                            ctx.Reply($"├─ {FontColorChatSystem.Red(error)}");
                        }
                        ctx.Reply($"└─ This ability cannot be used on this boss");
                        return;
                    }
                    else if (compatibilityResult.Level != AbilityCompatibilitySystem.CompatibilityLevel.Perfect)
                    {
                        ctx.Reply($"{FontColorChatSystem.Yellow("[WARNING]")} Compatibility level: {compatibilityResult.Level}");
                        foreach (var warning in compatibilityResult.Warnings)
                        {
                            ctx.Reply($"├─ {FontColorChatSystem.Yellow(warning)}");
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
                    
                    // Crear o actualizar el slot usando AbilitySwaps
                    if (boss.AbilitySwaps == null)
                    {
                        boss.AbilitySwaps = new Dictionary<int, AbilitySwapConfig>();
                    }
                    
                    // Parse slot name to index (e.g., "Slot1" -> 0, "Slot2" -> 1)
                    int slotIndex = -1;
                    if (slotName.ToLower().StartsWith("slot"))
                    {
                        if (int.TryParse(slotName.Substring(4), out int slotNum))
                        {
                            slotIndex = slotNum - 1; // Convert to 0-based index
                        }
                    }
                    else if (int.TryParse(slotName, out int directIndex))
                    {
                        slotIndex = directIndex;
                    }
                    
                    if (slotIndex < 0)
                    {
                        throw ctx.Error($"Invalid slot name '{slotName}'. Use 'Slot1', 'Slot2', etc. or numeric index");
                    }
                    
                    if (enabled)
                    {
                        // Get VBlood name
                        var vbloodList = AbilitySwapSystem.GetKnownVBloodPrefabs();
                        var sourceName = vbloodList.FirstOrDefault(x => x.Value == sourcePrefabGUID).Key ?? sourceInfo?.Name ?? "Unknown VBlood";
                        
                        boss.AbilitySwaps[slotIndex] = new AbilitySwapConfig
                        {
                            SourcePrefabGUID = sourcePrefabGUID,
                            SourceVBloodName = sourceName,
                            SlotIndex = abilityIndex,
                            Description = description
                        };
                    }
                    else if (boss.AbilitySwaps.ContainsKey(slotIndex))
                    {
                        boss.AbilitySwaps.Remove(slotIndex);
                    }
                    
                    Database.saveDatabase();
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[CONFIG]")} Configured ability slot '{slotName}' for boss '{bossName}':");
                    ctx.Reply($"├─ Source PrefabGUID: {sourcePrefabGUID}");
                    ctx.Reply($"├─ Ability Index: {abilityIndex}");
                    ctx.Reply($"├─ Enabled: {(enabled ? FontColorChatSystem.Green("Yes") : FontColorChatSystem.Red("No"))}");
                    ctx.Reply($"├─ Description: {(string.IsNullOrEmpty(description) ? "None" : description)}");
                    ctx.Reply($"└─ Compatibility: {GetCompatibilityIcon(compatibilityResult.Level)} {compatibilityResult.Level}");
                    
                    var knownVBloods = AbilitySwapSystem.GetKnownVBloodPrefabs();
                    var vbloodName = knownVBloods.FirstOrDefault(x => x.Value == sourcePrefabGUID).Key ?? "Unknown VBlood";
                    ctx.Reply($"{FontColorChatSystem.Blue("[SOURCE]")} VBlood: {vbloodName}");
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
                    // Parse slot name to index
                    int slotIndex = -1;
                    if (slotName.ToLower().StartsWith("slot"))
                    {
                        if (int.TryParse(slotName.Substring(4), out int slotNum))
                        {
                            slotIndex = slotNum - 1; // Convert to 0-based index
                        }
                    }
                    else if (int.TryParse(slotName, out int directIndex))
                    {
                        slotIndex = directIndex;
                    }
                    
                    if (slotIndex < 0)
                    {
                        throw ctx.Error($"Invalid slot name '{slotName}'. Use 'Slot1', 'Slot2', etc. or numeric index");
                    }
                    
                    if (boss.AbilitySwaps == null || !boss.AbilitySwaps.ContainsKey(slotIndex))
                    {
                        throw ctx.Error($"Ability slot {slotIndex} does not exist for boss '{bossName}'");
                    }
                    
                    boss.AbilitySwaps.Remove(slotIndex);
                    Database.saveDatabase();
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[REMOVED]")} Removed ability slot {slotIndex} from boss '{bossName}'");
                    ctx.Reply($"└─ Remaining slots: {boss.AbilitySwaps?.Count ?? 0}");
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
                    if (boss.AbilitySwaps == null || boss.AbilitySwaps.Count == 0)
                    {
                        ctx.Reply($"{FontColorChatSystem.Blue("[INFO]")} Boss '{bossName}' has no ability swaps configured");
                        ctx.Reply($"└─ Use .bb ability-slot to configure abilities");
                        return;
                    }
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[SWAPS]")} Ability swaps for boss '{bossName}':");
                    ctx.Reply($"────────────────────────────────────────────────");
                    
                    foreach (var swap in boss.AbilitySwaps.OrderBy(x => x.Key))
                    {
                        ctx.Reply($"├─ Slot {swap.Key}:");
                        ctx.Reply($"│  ├─ Source: {swap.Value.SourceVBloodName} ({swap.Value.SourcePrefabGUID})");
                        ctx.Reply($"│  ├─ Source Slot: {swap.Value.SlotIndex}");
                        ctx.Reply($"│  └─ Description: {(string.IsNullOrEmpty(swap.Value.Description) ? "None" : swap.Value.Description)}");
                    }
                    
                    ctx.Reply($"└─ Total: {boss.AbilitySwaps.Count} slots configured");
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
                    var removedCount = boss.AbilitySwaps?.Count ?? 0;
                    if (boss.AbilitySwaps != null)
                    {
                        boss.AbilitySwaps.Clear();
                    }
                    Database.saveDatabase();
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[CLEARED]")} Cleared all ability swaps from boss '{bossName}'");
                    ctx.Reply($"└─ Removed {removedCount} swaps");
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
                
                ctx.Reply($"{FontColorChatSystem.Blue("[INSPECT]")} Detailed abilities for {vbloodName} (PrefabGUID: {sourcePrefabGUID}):");
                ctx.Reply($"────────────────────────────────────────────────");
                
                // Check AbilityBar_Shared
                if (sourceEntity.Has<AbilityBar_Shared>())
                {
                    ctx.Reply($"{FontColorChatSystem.Green("[FOUND]")} Has AbilityBar_Shared component");
                }
                else
                {
                    ctx.Reply($"{FontColorChatSystem.Red("[MISSING]")} No AbilityBar_Shared component");
                    return;
                }
                
                // Check AbilityGroupSlotBuffer with detailed analysis
                if (entityManager.HasBuffer<AbilityGroupSlotBuffer>(sourceEntity))
                {
                    var buffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(sourceEntity);
                    ctx.Reply($"{FontColorChatSystem.Green("[FOUND]")} {buffer.Length} ability groups:");
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
                    ctx.Reply($"{FontColorChatSystem.Blue("[USAGE]")} Examples:");
                    for (int i = 0; i < Math.Min(buffer.Length, 3); i++)
                    {
                        ctx.Reply($"   .bb ability-slot-set \"YourBoss\" \"slot{i + 1}\" {sourcePrefabGUID} {i} true \"{vbloodName} ability {i + 1}\"");
                    }
                }
                else
                {
                    ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} No AbilityGroupSlotBuffer - VBlood may not be compatible with modular system");
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
            catch (Exception)
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
                    // Define presets using AbilitySwaps format
                    Dictionary<int, AbilitySwapConfig> preset = null;
                    
                    switch (presetName.ToLower())
                    {
                        case "dracula-mix":
                            preset = new Dictionary<int, AbilitySwapConfig>
                            {
                                [0] = new AbilitySwapConfig { SourcePrefabGUID = -327335305, SourceVBloodName = "Dracula", SlotIndex = 0, Description = "Dracula melee attack" },
                                [1] = new AbilitySwapConfig { SourcePrefabGUID = -327335305, SourceVBloodName = "Dracula", SlotIndex = 2, Description = "Dracula spell" },
                                [2] = new AbilitySwapConfig { SourcePrefabGUID = 939467639, SourceVBloodName = "Vincent", SlotIndex = 1, Description = "Vincent frost ability" }
                            };
                            break;
                            
                        case "frost-warrior":
                            preset = new Dictionary<int, AbilitySwapConfig>
                            {
                                [0] = new AbilitySwapConfig { SourcePrefabGUID = 1112948824, SourceVBloodName = "Tristan", SlotIndex = 0, Description = "Tristan melee" },
                                [1] = new AbilitySwapConfig { SourcePrefabGUID = 1112948824, SourceVBloodName = "Tristan", SlotIndex = 1, Description = "Tristan charge" },
                                [2] = new AbilitySwapConfig { SourcePrefabGUID = 939467639, SourceVBloodName = "Vincent", SlotIndex = 2, Description = "Vincent frost blast" }
                            };
                            break;
                            
                        case "spell-caster":
                            preset = new Dictionary<int, AbilitySwapConfig>
                            {
                                [0] = new AbilitySwapConfig { SourcePrefabGUID = -99012450, SourceVBloodName = "Christina", SlotIndex = 1, Description = "Christina heal" },
                                [1] = new AbilitySwapConfig { SourcePrefabGUID = -99012450, SourceVBloodName = "Christina", SlotIndex = 2, Description = "Christina light" },
                                [2] = new AbilitySwapConfig { SourcePrefabGUID = -327335305, SourceVBloodName = "Dracula", SlotIndex = 3, Description = "Dracula dark spell" }
                            };
                            break;
                            
                        default:
                            ctx.Reply($"{FontColorChatSystem.Red("[ERROR]")} Unknown preset '{presetName}'. Available presets:");
                            ctx.Reply($"├─ dracula-mix");
                            ctx.Reply($"├─ frost-warrior");
                            ctx.Reply($"└─ spell-caster");
                            return;
                    }
                    
                    // Clear existing swaps and apply preset
                    if (boss.AbilitySwaps == null)
                    {
                        boss.AbilitySwaps = new Dictionary<int, AbilitySwapConfig>();
                    }
                    else
                    {
                        boss.AbilitySwaps.Clear();
                    }
                    
                    foreach (var slot in preset)
                    {
                        boss.AbilitySwaps[slot.Key] = slot.Value;
                    }
                    
                    Database.saveDatabase();
                    
                    ctx.Reply($"{FontColorChatSystem.Blue("[PRESET]")} Applied preset '{presetName}' to boss '{bossName}':");
                    ctx.Reply($"└─ Configured {preset.Count} ability swaps");
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
        
    }
}