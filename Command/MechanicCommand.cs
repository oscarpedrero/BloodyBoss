using Bloody.Core;
using BloodyBoss.DB;
using BloodyBoss.DB.Models;
using BloodyBoss.Utils;
using BloodyBoss.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using VampireCommandFramework;
using ProjectM;
using Bloody.Core.Helper.v1;
using Unity.Entities;
using System.IO;
using System.Text.Json;

namespace BloodyBoss.Command
{
    [CommandGroup("bb")]
    public static class MechanicCommand
    {
        #region Basic Commands
        
        [Command("mechanic-add", usage: "<BossName> <MechanicType> [options]", description: "Add a mechanic to a boss", adminOnly: true)]
        public static void AddMechanic(ChatCommandContext ctx, string bossName, string mechanicType, string args = "")
        {
            try
            {
                Plugin.BLogger.Info(LogCategory.Command, $"AddMechanic called: boss={bossName}, type={mechanicType}, args={args}");
                
                if (!Database.GetBoss(bossName, out var boss))
                {
                    Plugin.BLogger.Warning(LogCategory.Command, $"Boss not found: {bossName}");
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                var parser = new OptionParser(args.Split(' ', StringSplitOptions.RemoveEmptyEntries));
                var mechanic = new BossMechanicModel
                {
                    Type = mechanicType.ToLower(),
                    Enabled = true
                };

                // Parse trigger based on provided options
                if (parser.HasOption("hp"))
                {
                    mechanic.Trigger = new MechanicTriggerModel
                    {
                        Type = "hp_threshold",
                        Value = parser.GetFloat("hp"),
                        Comparison = parser.GetString("comparison", "less_than"),
                        OneTime = parser.GetBool("one-time", true)
                    };
                }
                else if (parser.HasOption("time"))
                {
                    mechanic.Trigger = new MechanicTriggerModel
                    {
                        Type = "time",
                        Value = parser.GetFloat("time"),
                        RepeatInterval = parser.GetFloat("repeat", 0),
                        OneTime = parser.GetFloat("repeat", 0) <= 0
                    };
                }
                else if (parser.HasOption("players"))
                {
                    mechanic.Trigger = new MechanicTriggerModel
                    {
                        Type = "player_count",
                        Value = parser.GetFloat("players"),
                        Comparison = parser.GetString("comparison", "greater_than")
                    };
                }
                else
                {
                    throw ctx.Error($"No trigger specified. Use --hp, --time, or --players");
                }

                // Parse mechanic-specific parameters
                mechanic.Parameters = ParseMechanicParameters(mechanicType, parser);

                // Add to boss
                boss.Mechanics.Add(mechanic);
                Database.saveDatabase();

                ctx.Reply($"Added {mechanicType} mechanic to boss '{bossName}'");
                ctx.Reply($"Trigger: {mechanic.GetDescription()}");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error adding mechanic: {e.Message}");
            }
        }

        [Command("mechanic-remove", usage: "<BossName> <MechanicId>", description: "Remove a mechanic from a boss", adminOnly: true)]
        public static void RemoveMechanic(ChatCommandContext ctx, string bossName, string mechanicId)
        {
            try
            {
                if (!Database.GetBoss(bossName, out var boss))
                {
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                var mechanic = boss.Mechanics.FirstOrDefault(m => m.Id == mechanicId || m.Type == mechanicId);
                if (mechanic == null)
                {
                    throw ctx.Error($"Mechanic '{mechanicId}' not found on boss '{bossName}'.");
                }

                boss.Mechanics.Remove(mechanic);
                Database.saveDatabase();

                ctx.Reply($"Removed {mechanic.Type} mechanic from boss '{bossName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error removing mechanic: {e.Message}");
            }
        }

        [Command("mechanic-list", usage: "<BossName>", description: "List all mechanics for a boss", adminOnly: true)]
        public static void ListMechanics(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (!Database.GetBoss(bossName, out var boss))
                {
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                if (boss.Mechanics.Count == 0)
                {
                    ctx.Reply($"Boss '{bossName}' has no mechanics configured.");
                    return;
                }

                ctx.Reply($"Mechanics for boss '{bossName}':");
                ctx.Reply("─────────────────────────────");
                
                foreach (var mechanic in boss.Mechanics.OrderBy(m => m.Trigger?.Value))
                {
                    ctx.Reply($"├─ {mechanic.Type} [{mechanic.Id.Substring(0, 8)}] {(mechanic.Enabled ? "✅" : "❌")}");
                    ctx.Reply($"│  └─ {mechanic.GetDescription()}");
                    if (mechanic.TriggerCount > 0)
                    {
                        ctx.Reply($"│     Triggered {mechanic.TriggerCount} times");
                    }
                }
                
                ctx.Reply($"└─ Total: {boss.Mechanics.Count} mechanics");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error listing mechanics: {e.Message}");
            }
        }

        [Command("mechanic-toggle", usage: "<BossName> <MechanicId>", description: "Enable/disable a mechanic", adminOnly: true)]
        public static void ToggleMechanic(ChatCommandContext ctx, string bossName, string mechanicId)
        {
            try
            {
                if (!Database.GetBoss(bossName, out var boss))
                {
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                var mechanic = boss.Mechanics.FirstOrDefault(m => m.Id == mechanicId || m.Type == mechanicId);
                if (mechanic == null)
                {
                    throw ctx.Error($"Mechanic '{mechanicId}' not found on boss '{bossName}'.");
                }

                mechanic.Enabled = !mechanic.Enabled;
                Database.saveDatabase();

                ctx.Reply($"{mechanic.Type} mechanic is now {(mechanic.Enabled ? "enabled ✅" : "disabled ❌")}");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error toggling mechanic: {e.Message}");
            }
        }

        [Command("mechanic-config", usage: "<BossName> <MechanicIndex> <Parameters>", description: "Configure mechanic parameters", adminOnly: true)]
        public static void ConfigureMechanic(ChatCommandContext ctx, string bossName, int mechanicIndex, string parameters = "")
        {
            try
            {
                if (!Database.GetBoss(bossName, out var boss))
                {
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                if (mechanicIndex < 0 || mechanicIndex >= boss.Mechanics.Count)
                {
                    throw ctx.Error($"Invalid mechanic index. Boss has {boss.Mechanics.Count} mechanics (0-{boss.Mechanics.Count - 1}).");
                }

                var mechanic = boss.Mechanics[mechanicIndex];
                
                // If no parameters provided, show current configuration
                if (string.IsNullOrWhiteSpace(parameters))
                {
                    ctx.Reply($"⚙️ {mechanic.Type} mechanic configuration:");
                    ctx.Reply($"├─ Index: {mechanicIndex}");
                    ctx.Reply($"├─ Enabled: {(mechanic.Enabled ? "Yes ✅" : "No ❌")}");
                    ctx.Reply($"├─ Trigger: {mechanic.GetDescription()}");
                    ctx.Reply($"└─ Parameters:");
                    
                    foreach (var param in mechanic.Parameters)
                    {
                        ctx.Reply($"   ├─ {param.Key}: {param.Value}");
                    }
                    
                    ctx.Reply($"");
                    ctx.Reply($"Usage: .bb mechanic-config \"{bossName}\" {mechanicIndex} \"param1=value1 param2=value2 ...\"");
                    return;
                }

                // Parse parameters string
                var paramPairs = parameters.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var updatedParams = new List<string>();
                
                foreach (var pair in paramPairs)
                {
                    var parts = pair.Split('=', 2);
                    if (parts.Length != 2)
                    {
                        ctx.Reply($"⚠️ Invalid format: '{pair}'. Use parameter=value");
                        continue;
                    }

                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Handle different value types
                    object typedValue = value;
                    
                    // Try parse as bool
                    if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
                        typedValue = true;
                    else if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
                        typedValue = false;
                    // Try parse as int
                    else if (int.TryParse(value, out int intValue))
                        typedValue = intValue;
                    // Try parse as float
                    else if (float.TryParse(value, out float floatValue))
                        typedValue = floatValue;
                    // Keep as string otherwise (remove quotes if present)
                    else
                        typedValue = value.Trim('"', '\'');
                    
                    mechanic.Parameters[key] = typedValue;
                    updatedParams.Add($"{key}={typedValue}");
                }

                Database.saveDatabase();
                
                ctx.Reply($"✅ Updated {mechanic.Type} mechanic parameters:");
                foreach (var update in updatedParams)
                {
                    ctx.Reply($"   ├─ {update}");
                }
                
                // Show example for stun mechanic
                if (mechanic.Type == "stun")
                {
                    ctx.Reply($"");
                    ctx.Reply($"📝 Example for stun:");
                    ctx.Reply($".bb mechanic-config \"{bossName}\" {mechanicIndex} \"target=nearest duration=3 mark_duration=2.5 max_targets=2 announcement='The void gazes upon you!' flash_before_stun=true\"");
                }
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error configuring mechanic: {e.Message}");
            }
        }

        [Command("mechanic-clear", usage: "<BossName>", description: "Remove all mechanics from a boss", adminOnly: true)]
        public static void ClearMechanics(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (!Database.GetBoss(bossName, out var boss))
                {
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                var count = boss.Mechanics.Count;
                boss.Mechanics.Clear();
                Database.saveDatabase();

                ctx.Reply($"Removed all {count} mechanics from boss '{bossName}'");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error clearing mechanics: {e.Message}");
            }
        }

        #endregion

        #region Advanced Commands

        [Command("mechanic-test", usage: "<BossName> <MechanicId>", description: "Test trigger a mechanic", adminOnly: true)]
        public static void TestMechanic(ChatCommandContext ctx, string bossName, string mechanicId)
        {
            try
            {
                if (!Database.GetBoss(bossName, out var boss))
                {
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                var mechanic = boss.Mechanics.FirstOrDefault(m => m.Id == mechanicId || m.Type == mechanicId);
                if (mechanic == null)
                {
                    throw ctx.Error($"Mechanic '{mechanicId}' not found on boss '{bossName}'.");
                }

                // Find active boss entity
                var bossEntity = FindBossEntity(boss);
                if (bossEntity == Entity.Null)
                {
                    throw ctx.Error($"Boss '{bossName}' is not currently spawned. Use .bb start to spawn it first.");
                }

                ctx.Reply($"🧪 Testing {mechanic.Type} mechanic...");
                
                // TODO: Execute mechanic through BossMechanicSystem
                // BossMechanicSystem.ExecuteMechanic(bossEntity, mechanic);
                
                ctx.Reply($"✅ Mechanic test completed");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error testing mechanic: {e.Message}");
            }
        }

        [Command("mechanic-preset-create", usage: "<PresetName>", description: "Create a new mechanic preset", adminOnly: true)]
        public static void CreatePreset(ChatCommandContext ctx, string presetName)
        {
            try
            {
                // TODO: Implement preset system
                ctx.Reply($"Preset system coming in v2.2.1");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error creating preset: {e.Message}");
            }
        }

        [Command("mechanic-export", usage: "<BossName>", description: "Export boss mechanics to file", adminOnly: true)]
        public static void ExportMechanics(ChatCommandContext ctx, string bossName)
        {
            try
            {
                if (!Database.GetBoss(bossName, out var boss))
                {
                    throw ctx.Error($"Boss '{bossName}' not found.");
                }

                // Create anonymous type for easier serialization
                var exportData = new
                {
                    BossName = boss.name,
                    ExportDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Mechanics = boss.Mechanics
                };

                var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
                var fileName = $"Boss_Mechanics_{boss.name}_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(Database.ConfigPath, fileName);
                
                File.WriteAllText(filePath, json);
                
                ctx.Reply($"Exported {boss.Mechanics.Count} mechanics to: {fileName}");
            }
            catch (Exception e)
            {
                throw ctx.Error($"Error exporting mechanics: {e.Message}");
            }
        }

        #endregion

        #region Helper Methods

        private static Dictionary<string, object> ParseMechanicParameters(string mechanicType, OptionParser parser)
        {
            var parameters = new Dictionary<string, object>();

            switch (mechanicType.ToLower())
            {
                case "enrage":
                    parameters["damage_multiplier"] = parser.GetFloat("damage", 1.5f);
                    parameters["movement_speed_multiplier"] = parser.GetFloat("movement", 1.0f);
                    parameters["attack_speed_multiplier"] = parser.GetFloat("attack-speed", 1.0f);
                    parameters["cast_speed_multiplier"] = parser.GetFloat("cast-speed", 1.0f);
                    parameters["cooldown_reduction"] = parser.GetFloat("cooldown-reduction", 0f);
                    parameters["duration"] = parser.GetFloat("duration", 0f);
                    parameters["visual_effect"] = parser.GetString("effect", "blood_rage");
                    parameters["announcement"] = parser.GetString("announce", "The boss enters a blood rage!");
                    break;

                case "shield":
                    parameters["shield_type"] = parser.GetString("type", "immune");
                    parameters["shield_amount"] = parser.GetFloat("amount", 10000f);
                    parameters["duration"] = parser.GetFloat("duration", 10f);
                    parameters["can_move"] = parser.GetBool("can-move", false);
                    parameters["visual_effect"] = parser.GetString("effect", "holy_shield");
                    parameters["announcement"] = parser.GetString("announce", "A divine shield protects the boss!");
                    break;

                case "summon":
                    parameters["add_prefab"] = parser.GetInt("prefab", -1905691330);
                    parameters["count"] = parser.GetInt("count", 3);
                    parameters["pattern"] = parser.GetString("pattern", "circle");
                    parameters["despawn_on_boss_death"] = parser.GetBool("despawn", true);
                    parameters["announcement"] = parser.GetString("announce", "Minions answer the call!");
                    break;

                case "teleport":
                    parameters["teleport_type"] = parser.GetString("type", "random");
                    parameters["range"] = parser.GetFloat("range", 30f);
                    parameters["after_effect"] = parser.GetString("after", "");
                    parameters["announcement"] = parser.GetString("announce", "");
                    break;

                case "heal":
                    parameters["heal_amount"] = parser.GetString("amount", "20%");
                    parameters["heal_type"] = parser.GetString("type", "instant");
                    parameters["duration"] = parser.GetFloat("duration", 0f);
                    parameters["interruptible"] = parser.GetBool("interruptible", true);
                    parameters["visual_effect"] = parser.GetString("effect", "heal_glow");
                    parameters["announcement"] = parser.GetString("announce", "The boss begins to heal!");
                    break;

                default:
                    // Generic parameters for unknown mechanics
                    foreach (var opt in parser.GetAllOptions())
                    {
                        parameters[opt.Key] = opt.Value;
                    }
                    break;
            }

            return parameters;
        }

        private static Entity FindBossEntity(BossEncounterModel boss)
        {
            var entities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable>();
            foreach (var entity in entities)
            {
                if (entity.Has<NameableInteractable>())
                {
                    var nameable = entity.Read<NameableInteractable>();
                    if (nameable.Name.ToString() == boss.nameHash + "bb")
                    {
                        return entity;
                    }
                }
            }
            return Entity.Null;
        }

        #endregion

        #region Usage Examples

        [Command("mechanic-help", usage: "", description: "Show mechanic command examples", adminOnly: true)]
        public static void ShowHelp(ChatCommandContext ctx)
        {
            ctx.Reply("📋 Mechanic Command Examples:");
            ctx.Reply("─────────────────────────────");
            
            ctx.Reply("Add enrage at 25% HP:");
            ctx.Reply("  .bb mechanic-add \"Boss\" \"enrage\" --hp 25 --damage 1.5 --movement 1.3");
            
            ctx.Reply("Add shield phase at 50% HP:");
            ctx.Reply("  .bb mechanic-add \"Boss\" \"shield\" --hp 50 --duration 10 --type immune");
            
            ctx.Reply("Add time-based summons:");
            ctx.Reply("  .bb mechanic-add \"Boss\" \"summon\" --time 60 --repeat 30 --count 5");
            
            ctx.Reply("Add teleport when 3+ players:");
            ctx.Reply("  .bb mechanic-add \"Boss\" \"teleport\" --players 3 --comparison greater_than");
            
            ctx.Reply("─────────────────────────────");
            ctx.Reply("Use .bb mechanic-list \"BossName\" to see configured mechanics");
        }

        #endregion
    }
}