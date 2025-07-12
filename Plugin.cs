using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using Unity.Entities;
using BepInEx.Logging;
using BloodyBoss.Configuration;
using BloodyBoss.DB;
using Bloody.Core;
using BloodyBoss.Systems;
using Bloody.Core.API.v1;
using BloodyBoss.Hooks;
using Bloody.Core.Helper.v1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BloodyBoss;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("trodi.Bloody.Core")]
public class Plugin : BasePlugin
{
    Harmony _harmony;

    public static Bloody.Core.Helper.v1.Logger Logger;
    public static BloodyLogger BLogger;
    public static SystemsCore SystemsCore;

    public override void Load()
    {


        if (!Core.IsServer)
        {
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is only for server!");
            return;
        }

        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(typeof(DamageDetectionHook));
        _harmony.PatchAll(typeof(AttackerTrackingHook));

        // Verificar que el parche se aplicó
        var patches = _harmony.GetPatchedMethods();
        foreach (var method in patches)
        {
            Log.LogWarning($"[HARMONY] Patched method: {method.DeclaringType}.{method.Name}");
        }

        EventsHandlerSystem.OnInitialize += GameDataOnInitialize;

        Logger = new(Log);
        BLogger = new BloodyLogger(Log);


        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }

    public override bool Unload()
    {
        EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
        BossSystem.StopTimer(); // Detener el timer independiente
        Cache.ComponentCache.Dispose(); // Limpiar cache de componentes
        CommandRegistry.UnregisterAssembly();
        _harmony?.UnpatchSelf();
        return true;
    }

    private static void GameDataOnInitialize(World world)
    {
        Logger.LogInfo("GameDataOnInitialize");

        SystemsCore = Core.SystemsCore;

        Logger.LogInfo("Loading main data");
        Database.Initialize();
        Logger.LogInfo("Binding configuration");
        PluginConfig.Initialize();

        // Configure BloodyLogger with settings from config
        ConfigureLogger();

        EventsHandlerSystem.OnDeathVBlood += BossGameplayEventSystem.OnVBloodConsumed;
        EventsHandlerSystem.OnDeath += BossGameplayEventSystem.OnDeathNpc;
        BossSystem.GenerateStats();
        BossSystem.CheckBoss();
        BossSystem.StartTimer(); // Iniciar el timer independiente

    }

    private static void ConfigureLogger()
    {
        try
        {
            // Parse global log level
            if (!Enum.TryParse<BloodyBoss.Systems.LogLevel>(PluginConfig.GlobalLogLevel.Value, true, out var globalLevel))
            {
                globalLevel = BloodyBoss.Systems.LogLevel.Info;
            }

            // Parse category levels
            var categoryLevels = new Dictionary<string, BloodyBoss.Systems.LogLevel>();
            if (!string.IsNullOrEmpty(PluginConfig.CategoryLogLevels.Value))
            {
                var pairs = PluginConfig.CategoryLogLevels.Value.Split(',');
                foreach (var pair in pairs)
                {
                    var parts = pair.Split(':');
                    if (parts.Length == 2 && Enum.TryParse<BloodyBoss.Systems.LogLevel>(parts[1].Trim(), true, out var level))
                    {
                        categoryLevels[parts[0].Trim()] = level;
                    }
                }
            }

            // Parse disabled categories
            var disabledCategories = new HashSet<string>();
            if (!string.IsNullOrEmpty(PluginConfig.DisabledLogCategories.Value))
            {
                var categories = PluginConfig.DisabledLogCategories.Value.Split(',');
                foreach (var category in categories)
                {
                    disabledCategories.Add(category.Trim());
                }
            }

            // Configure the logger
            BLogger.Configure(globalLevel, categoryLevels, disabledCategories);

            Logger.LogInfo($"BloodyLogger configured - Level: {globalLevel}, Categories: {categoryLevels.Count}, Disabled: {disabledCategories.Count}");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to configure BloodyLogger: {ex.Message}");
        }
    }

}
