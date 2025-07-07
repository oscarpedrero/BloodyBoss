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

        _harmony.PatchAll(typeof(DealDamageHook));
        //_harmony.PatchAll(typeof(StatChangeHook));
        //_harmony.PatchAll(typeof(VBloodSystemHook));

        EventsHandlerSystem.OnInitialize += GameDataOnInitialize;

        Logger = new(Log);
        

        // Register all commands in the assembly with VCF
        CommandRegistry.RegisterAll();
    }

    public override bool Unload()
    {
        EventsHandlerSystem.OnInitialize -= GameDataOnInitialize;
        BossSystem.StopTimer(); // Detener el timer independiente
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
        
        // Inicializar escáner de VBloods para compatibilidad
        Logger.LogInfo("Initializing VBlood Scanner for ability compatibility");
        try
        {
            // Ejecutar escaneo después de un breve retraso
            bool scanExecuted = false;
            CoroutineHandler.StartRepeatingCoroutine(() => 
            {
                if (!scanExecuted)
                {
                    // COMENTADO: Scanner y debugger ya no son necesarios con la base de datos estática
                    // El comando vblood-export ya genera toda la información necesaria
                    
                    // // Primero ejecutar el scanner normal con LogInfo
                    // Logger.LogInfo("=== Starting VBlood Prefab Scanner (Info Level) ===");
                    // VBloodPrefabScanner.ScanVBloodPrefabs();
                    // var vbloods = VBloodPrefabScanner.GetAllVBloods();
                    // Logger.LogInfo($"VBlood Scanner completed: Found {vbloods.Count} VBloods");
                    
                    // // Luego ejecutar el debugger con LogWarning para análisis detallado
                    // Logger.LogWarning("=== Starting VBlood Component Debugger (Warning Level) ===");
                    // VBloodComponentDebugger.DebugVBloodComponents();
                    // Logger.LogWarning("=== VBlood Component Debugger Completed ===");
                    
                    scanExecuted = true;
                }
            }, 5f);
        }
        catch (System.Exception ex)
        {
            Logger.LogError($"Failed to initialize VBlood Scanner: {ex.Message}");
        }

        EventsHandlerSystem.OnDeathVBlood += VBloodSystemHook.OnDeathVblood;
        //EventsHandlerSystem.OnDamage += NpcSystem.OnDamageNpc;
        EventsHandlerSystem.OnDeath += DeathNpcHook.OnDeathNpc;
        BossSystem.GenerateStats();
        BossSystem.CheckBoss();
        BossSystem.StartTimer(); // Iniciar el timer independiente
    }

}
