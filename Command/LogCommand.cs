using VampireCommandFramework;
using BloodyBoss.Configuration;
using BloodyBoss.Systems;
using System;
using System.Linq;

namespace BloodyBoss.Command
{
    [CommandGroup("bb log", "Configure BloodyBoss logging")]
    public static class LogCommand
    {
        [Command("level", adminOnly: true, description: "Set global log level", usage: ".bb log level <None|Error|Warning|Info|Debug|Trace>")]
        public static void SetLogLevel(ChatCommandContext ctx, string level)
        {
            if (!Enum.TryParse<LogLevel>(level, true, out var logLevel))
            {
                ctx.Reply($"Invalid log level. Valid options: {string.Join(", ", Enum.GetNames(typeof(LogLevel)))}");
                return;
            }
            
            Plugin.BLogger.SetLogLevel(logLevel);
            ctx.Reply($"Global log level set to: {logLevel}");
        }
        
        [Command("category", adminOnly: true, description: "Set log level for specific category", usage: ".bb log category <category> <level>")]
        public static void SetCategoryLevel(ChatCommandContext ctx, string category, string level)
        {
            if (!Enum.TryParse<LogCategory>(category, true, out var logCategory))
            {
                ctx.Reply($"Invalid category. Valid options: {string.Join(", ", Enum.GetNames(typeof(LogCategory)))}");
                return;
            }
            
            if (!Enum.TryParse<LogLevel>(level, true, out var logLevel))
            {
                ctx.Reply($"Invalid log level. Valid options: {string.Join(", ", Enum.GetNames(typeof(LogLevel)))}");
                return;
            }
            
            Plugin.BLogger.SetCategoryLevel(logCategory, logLevel);
            ctx.Reply($"Log level for {logCategory} set to: {logLevel}");
        }
        
        [Command("disable", adminOnly: true, description: "Disable a log category", usage: ".bb log disable <category>")]
        public static void DisableCategory(ChatCommandContext ctx, string category)
        {
            if (!Enum.TryParse<LogCategory>(category, true, out var logCategory))
            {
                ctx.Reply($"Invalid category. Valid options: {string.Join(", ", Enum.GetNames(typeof(LogCategory)))}");
                return;
            }
            
            Plugin.BLogger.SetCategoryEnabled(logCategory, false);
            ctx.Reply($"Disabled logging for category: {logCategory}");
        }
        
        [Command("enable", adminOnly: true, description: "Enable a log category", usage: ".bb log enable <category>")]
        public static void EnableCategory(ChatCommandContext ctx, string category)
        {
            if (!Enum.TryParse<LogCategory>(category, true, out var logCategory))
            {
                ctx.Reply($"Invalid category. Valid options: {string.Join(", ", Enum.GetNames(typeof(LogCategory)))}");
                return;
            }
            
            Plugin.BLogger.SetCategoryEnabled(logCategory, true);
            ctx.Reply($"Enabled logging for category: {logCategory}");
        }
        
        [Command("list", adminOnly: true, description: "List all log categories", usage: ".bb log list")]
        public static void ListCategories(ChatCommandContext ctx)
        {
            var categories = Enum.GetNames(typeof(LogCategory));
            ctx.Reply($"Available log categories: {string.Join(", ", categories)}");
        }
        
        [Command("quiet", adminOnly: true, description: "Set quiet mode (only errors)", usage: ".bb log quiet")]
        public static void SetQuietMode(ChatCommandContext ctx)
        {
            Plugin.BLogger.SetLogLevel(LogLevel.Error);
            ctx.Reply("Quiet mode enabled - only errors will be logged");
        }
        
        [Command("verbose", adminOnly: true, description: "Set verbose mode (all logs)", usage: ".bb log verbose")]
        public static void SetVerboseMode(ChatCommandContext ctx)
        {
            Plugin.BLogger.SetLogLevel(LogLevel.Trace);
            ctx.Reply("Verbose mode enabled - all logs will be shown");
        }
        
        [Command("normal", adminOnly: true, description: "Set normal mode (info level)", usage: ".bb log normal")]
        public static void SetNormalMode(ChatCommandContext ctx)
        {
            Plugin.BLogger.SetLogLevel(LogLevel.Info);
            ctx.Reply("Normal mode enabled - info level and above will be logged");
        }
        
        [Command("performance", adminOnly: true, description: "Configure for performance (minimal logging)", usage: ".bb log performance")]
        public static void SetPerformanceMode(ChatCommandContext ctx)
        {
            // Set global to Warning
            Plugin.BLogger.SetLogLevel(LogLevel.Warning);
            
            // Disable verbose categories completely
            Plugin.BLogger.SetCategoryEnabled(LogCategory.Damage, false);
            Plugin.BLogger.SetCategoryEnabled(LogCategory.Hook, false);
            Plugin.BLogger.SetCategoryEnabled(LogCategory.Timer, false);
            Plugin.BLogger.SetCategoryEnabled(LogCategory.Debug, false);
            
            // Keep only essential info
            Plugin.BLogger.SetCategoryLevel(LogCategory.Spawn, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.Death, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.Reward, LogLevel.Info);
            
            ctx.Reply("Performance mode enabled - minimal logging for better performance");
        }
        
        [Command("combat", adminOnly: true, description: "Show only combat-related logs", usage: ".bb log combat")]
        public static void SetCombatMode(ChatCommandContext ctx)
        {
            // Set global to Warning
            Plugin.BLogger.SetLogLevel(LogLevel.Warning);
            
            // Enable combat categories
            Plugin.BLogger.SetCategoryLevel(LogCategory.Boss, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.Damage, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.Death, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.Mechanic, LogLevel.Info);
            
            // Disable non-combat
            Plugin.BLogger.SetCategoryEnabled(LogCategory.Timer, false);
            Plugin.BLogger.SetCategoryEnabled(LogCategory.Hook, false);
            Plugin.BLogger.SetCategoryEnabled(LogCategory.Debug, false);
            
            ctx.Reply("Combat mode enabled - showing only combat-related logs");
        }
        
        [Command("essential", adminOnly: true, description: "Show only essential logs (spawns, deaths, rewards)", usage: ".bb log essential")]
        public static void SetEssentialMode(ChatCommandContext ctx)
        {
            // Set everything to Error
            Plugin.BLogger.SetLogLevel(LogLevel.Error);
            
            // Enable only essential categories
            Plugin.BLogger.SetCategoryLevel(LogCategory.Spawn, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.Death, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.Reward, LogLevel.Info);
            Plugin.BLogger.SetCategoryLevel(LogCategory.System, LogLevel.Error);
            
            ctx.Reply("Essential mode enabled - showing only spawn, death, and reward logs");
        }
    }
}