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
using Bloody.Core.API.v1;
using BloodyBoss.Patch;

namespace BloodyBoss;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
[BepInDependency("gg.deca.Bloodstone")]
[BepInDependency("trodi.Bloody.Core")]
[Bloodstone.API.Reloadable]
public class Plugin : BasePlugin, IRunOnInitialized
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
        _harmony.PatchAll(typeof(ActionSchedulerPatch));

        EventsHandlerSystem.OnInitialize += GameDataOnInitialize;

        Logger = new(Log);
        

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }

    public override bool Unload()
    {
        EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
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

        BossSystem.StartTimer();
        

        EventsHandlerSystem.OnDeathVBlood += BossSystem.OnDeathVblood;
        EventsHandlerSystem.OnDamage += NpcSystem.OnDamageNpc;
        EventsHandlerSystem.OnDeath += NpcSystem.OnDeathNpc;

    }

    private static void GameFrame_OnUpdate()
    {
        Logger.LogInfo("GameFrame");
    }

    public void OnGameInitialized()
    {
        Logger.LogDebug("OnGameInitialized");
    }
}
