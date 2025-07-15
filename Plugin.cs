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
using ProjectM;

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
        
        // Clean up boss states on server startup
        CleanupBossStatesOnStartup();
        
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

    private static void CleanupBossStatesOnStartup()
    {
        try
        {
            Logger.LogInfo("[STARTUP] Starting boss state cleanup...");
            
            // For efficiency with large boss counts, we'll use a single EntityQuery to find all entities with NameableInteractable
            var entityManager = SystemsCore.EntityManager;
            var nameableQuery = entityManager.CreateEntityQuery(ComponentType.ReadOnly<NameableInteractable>());
            var entities = nameableQuery.ToEntityArray(Unity.Collections.Allocator.Temp);
            
            // Create a dictionary for O(1) lookups of entity names
            var entityNameMap = new Dictionary<string, Entity>(entities.Length);
            
            for (int i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                if (!entityManager.Exists(entity)) continue;
                
                var nameable = entityManager.GetComponentData<NameableInteractable>(entity);
                var name = nameable.Name.ToString();
                
                // Store all entities by name for efficient lookup
                if (!string.IsNullOrEmpty(name))
                {
                    entityNameMap[name] = entity;
                }
            }
            
            entities.Dispose();
            nameableQuery.Dispose();
            
            // Now check each boss configuration
            int cleanedCount = 0;
            int verifiedCount = 0;
            int nameHashUpdatedCount = 0;
            
            foreach (var boss in Database.BOSSES)
            {
                // Check if nameHash is already a GUID (32 hex characters)
                bool isAlreadyGuid = !string.IsNullOrEmpty(boss.nameHash) && 
                                   boss.nameHash.Length == 32 && 
                                   System.Text.RegularExpressions.Regex.IsMatch(boss.nameHash, "^[0-9a-fA-F]{32}$");
                
                string newGuid;
                string oldNameHash = boss.nameHash;
                
                if (isAlreadyGuid)
                {
                    // Keep existing GUID
                    newGuid = boss.nameHash;
                    BLogger.Debug(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' already has GUID nameHash: '{newGuid}'");
                }
                else
                {
                    // Generate new GUID-based nameHash to prevent collisions
                    newGuid = System.Guid.NewGuid().ToString("N"); // "N" format removes hyphens
                    boss.nameHash = newGuid;
                    nameHashUpdatedCount++;
                    BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' nameHash updated from '{oldNameHash}' to '{newGuid}'");
                }
                
                if (boss.bossSpawn)
                {
                    // Boss is marked as spawned, verify it exists
                    bool bossExists = false;
                    
                    // First check if the stored entity is still valid
                    if (boss.bossEntity != Entity.Null && entityManager.Exists(boss.bossEntity))
                    {
                        // Verify it's still the same boss by checking the name
                        var nameable = entityManager.GetComponentData<NameableInteractable>(boss.bossEntity);
                        var entityName = nameable.Name.ToString();
                        
                        if (entityName == boss.name || entityName == oldNameHash + "bb" || entityName == newGuid + "bb")
                        {
                            bossExists = true;
                            verifiedCount++;
                            BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' verified as still spawned");
                            
                            // Only update entity name if nameHash changed
                            if (!isAlreadyGuid)
                            {
                                nameable.Name = new Unity.Collections.FixedString64Bytes(newGuid + "bb");
                                entityManager.SetComponentData(boss.bossEntity, nameable);
                                BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' entity name updated to '{newGuid}bb'");
                            }
                            
                            // Try to find and update the icon
                            if (boss.iconEntity == Entity.Null || !entityManager.Exists(boss.iconEntity))
                            {
                                var iconEntities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
                                foreach (var iconEntity in iconEntities)
                                {
                                    var iconNameable = iconEntity.Read<NameableInteractable>();
                                    if (iconNameable.Name.Value == oldNameHash + "ibb" || iconNameable.Name.Value == newGuid + "ibb")
                                    {
                                        boss.iconEntity = iconEntity;
                                        // Only update icon name if nameHash changed
                                        if (!isAlreadyGuid)
                                        {
                                            iconNameable.Name = new Unity.Collections.FixedString64Bytes(newGuid + "ibb");
                                            iconEntity.Write(iconNameable);
                                            BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' icon found and updated to '{newGuid}ibb'");
                                        }
                                        else
                                        {
                                            BLogger.Debug(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' icon found and linked");
                                        }
                                        break;
                                    }
                                }
                                iconEntities.Dispose();
                            }
                            else if (!isAlreadyGuid)
                            {
                                // Update existing icon with new GUID only if nameHash changed
                                var iconNameable = boss.iconEntity.Read<NameableInteractable>();
                                iconNameable.Name = new Unity.Collections.FixedString64Bytes(newGuid + "ibb");
                                boss.iconEntity.Write(iconNameable);
                                BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' existing icon updated to '{newGuid}ibb'");
                            }
                        }
                    }
                    
                    // If stored entity is invalid, try to find by name or old nameHash
                    if (!bossExists)
                    {
                        Entity foundEntity = Entity.Null;
                        
                        // Try to find by boss name
                        if (entityNameMap.ContainsKey(boss.name))
                        {
                            foundEntity = entityNameMap[boss.name];
                        }
                        // Try to find by old nameHash pattern
                        else if (entityNameMap.ContainsKey(oldNameHash + "bb"))
                        {
                            foundEntity = entityNameMap[oldNameHash + "bb"];
                        }
                        // Try to find by new nameHash pattern (in case entity was already updated)
                        else if (entityNameMap.ContainsKey(newGuid + "bb"))
                        {
                            foundEntity = entityNameMap[newGuid + "bb"];
                        }
                        
                        if (foundEntity != Entity.Null)
                        {
                            boss.bossEntity = foundEntity;
                            bossExists = true;
                            verifiedCount++;
                            BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' found and re-linked to entity");
                            
                            // Only update entity name if nameHash changed
                            if (!isAlreadyGuid)
                            {
                                var nameable = entityManager.GetComponentData<NameableInteractable>(foundEntity);
                                nameable.Name = new Unity.Collections.FixedString64Bytes(newGuid + "bb");
                                entityManager.SetComponentData(foundEntity, nameable);
                                BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' entity name updated to '{newGuid}bb'");
                            }
                            
                            // Try to find and update the icon
                            var iconEntities = QueryComponents.GetEntitiesByComponentTypes<NameableInteractable, MapIconData>(EntityQueryOptions.IncludeDisabledEntities);
                            foreach (var iconEntity in iconEntities)
                            {
                                var iconNameable = iconEntity.Read<NameableInteractable>();
                                if (iconNameable.Name.Value == oldNameHash + "ibb" || iconNameable.Name.Value == newGuid + "ibb")
                                {
                                    boss.iconEntity = iconEntity;
                                    // Only update icon name if nameHash changed
                                    if (!isAlreadyGuid)
                                    {
                                        iconNameable.Name = new Unity.Collections.FixedString64Bytes(newGuid + "ibb");
                                        iconEntity.Write(iconNameable);
                                        BLogger.Info(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' icon found and updated to '{newGuid}ibb'");
                                    }
                                    else
                                    {
                                        BLogger.Debug(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' icon found and linked");
                                    }
                                    break;
                                }
                            }
                            iconEntities.Dispose();
                        }
                    }
                    
                    // If boss doesn't exist, clean up its state
                    if (!bossExists)
                    {
                        boss.bossSpawn = false;
                        boss.bossEntity = Entity.Null;
                        boss.RemoveKillers();
                        BossMechanicSystem.CleanupBossMechanics(boss);
                        cleanedCount++;
                        BLogger.Warning(LogCategory.Boss, $"[STARTUP] Boss '{boss.name}' marked as not spawned (entity not found)");
                    }
                }
            }
            
            // Save database if any changes were made
            if (cleanedCount > 0 || nameHashUpdatedCount > 0)
            {
                Database.saveDatabase();
                Logger.LogInfo($"[STARTUP] Boss cleanup complete: {cleanedCount} cleaned, {verifiedCount} verified, {nameHashUpdatedCount} nameHash updated");
            }
            else if (verifiedCount > 0)
            {
                Logger.LogInfo($"[STARTUP] Boss verification complete: {verifiedCount} bosses verified as spawned");
            }
            else
            {
                Logger.LogInfo("[STARTUP] No spawned bosses found to verify");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError($"[STARTUP] Error during boss state cleanup: {ex.Message}");
        }
    }

}
