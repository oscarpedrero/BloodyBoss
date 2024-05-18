using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VampireCommandFramework;
using Unity.Entities;
using BepInEx.Logging;
using BloodyBoss.Configuration;
using BloodyBoss.DB;
using Bloody.Core;
using Bloodstone.API;
using BloodyBoss.Systems;
using Bloody.Core.API;

namespace BloodyBoss;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
public class Plugin : BasePlugin, IRunOnInitialized
{
    Harmony _harmony;

    public static Bloody.Core.Helper.Logger Logger;
    public static SystemsCore SystemsCore;

    public override void Load()
    {
        
        // Plugin startup logic
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} version {MyPluginInfo.PLUGIN_VERSION} is loaded!");

        // Harmony patching
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());

        EventsHandlerSystem.OnInitialize += GameDataOnInitialize;

        Logger = new(Log);
        

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }

    public override bool Unload()
    {
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

        EventsHandlerSystem.OnGameFrameUpdate += TimerSystem.OnGameFrame;

        BossSystem.StartTimer();

        EventsHandlerSystem.OnDeathVBlood += BossSystem.OnDetahVblood;

    }

    public void OnGameInitialized()
    {
        Logger.LogDebug("OnGameInitialized");
    }
}
