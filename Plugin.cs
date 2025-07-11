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

namespace BloodyBoss;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("trodi.Bloody.Core")]
public class Plugin : BasePlugin
{
    Harmony _harmony;

    public static Bloody.Core.Helper.v1.Logger Logger;
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
        

        EventsHandlerSystem.OnDeathVBlood += BossGameplayEventSystem.OnVBloodConsumed;
        EventsHandlerSystem.OnDeath += BossGameplayEventSystem.OnDeathNpc;
        BossSystem.GenerateStats();
        BossSystem.CheckBoss();
        BossSystem.StartTimer(); // Iniciar el timer independiente
    }

}
