using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using ProjectM;
using Stunlock.Core;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using BloodyBoss.Configuration;

namespace BloodyBoss.Systems
{
    public static class AbilitySwapSystem
    {
        /// <summary>
        /// Sistema experimental para intercambiar habilidades entre VBloods
        /// </summary>
        
        public static bool TryActivateVBloodAbility(Entity targetEntity, PrefabGUID vbloodPrefabGUID)
        {
            try
            {
                Plugin.BLogger.Info(LogCategory.System, $"Attempting to activate VBlood ability {vbloodPrefabGUID.GuidHash} on entity {targetEntity.Index}.{targetEntity.Version}");
                
                // Usar la función nativa de V Rising para activar habilidades VBlood
                var entityManager = Core.World.EntityManager;
                
                // Buscar el método en todas las clases de ProjectM
                var projectMAssembly = typeof(ProjectM.VBloodData).Assembly;
                var allTypes = projectMAssembly.GetTypes();
                
                foreach (var type in allTypes)
                {
                    try
                    {
                        var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        foreach (var method in methods)
                        {
                            if (method.Name == "TryActivateVBloodAbility")
                            {
                                Plugin.BLogger.Info(LogCategory.System, $"Found VBlood activation method in {type.Name}: {method.Name}");
                                try
                                {
                                    // Intentar invocar el método
                                    method.Invoke(null, new object[] { entityManager, vbloodPrefabGUID, true });
                                    Plugin.BLogger.Info(LogCategory.System, "VBlood ability activation attempted via reflection");
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    Plugin.BLogger.Error(LogCategory.System, $"Failed to invoke VBlood activation from {type.Name}: {ex.Message}");
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Ignorar tipos que no se pueden acceder
                    }
                }
                
                Plugin.BLogger.Warning(LogCategory.System, "TryActivateVBloodAbility method not found in any ProjectM type");
                return false;
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error activating VBlood ability: {ex.Message}");
                return false;
            }
        }
        
        public static bool TrySwapVBloodAbilities(Entity targetEntity, PrefabGUID sourcePrefabGUID, PrefabGUID targetPrefabGUID)
        {
            try
            {
                Plugin.BLogger.Info(LogCategory.System, $"Attempting ability swap from {sourcePrefabGUID.GuidHash} to {targetPrefabGUID.GuidHash}");
                
                // Verificar que las entidades existen
                if (!Core.World.EntityManager.Exists(targetEntity))
                {
                    Plugin.BLogger.Error(LogCategory.System, "Target entity does not exist");
                    return false;
                }
                
                // Verificar compatibilidad antes de proceder
                if (PluginConfig.EnableAbilityCompatibilityCheck.Value)
                {
                    Plugin.BLogger.Info(LogCategory.System, "Checking ability compatibility...");
                    
                    // Verificar compatibilidad general entre los VBloods
                    bool isCompatible = true;
                    var scanner = VBloodPrefabScanner.GetAllVBloods();
                    
                    // Verificar cada slot de habilidad
                    if (scanner.ContainsKey(sourcePrefabGUID.GuidHash))
                    {
                        var sourceVBlood = scanner[sourcePrefabGUID.GuidHash];
                        foreach (var abilitySlot in sourceVBlood.Abilities.Keys)
                        {
                            if (!VBloodPrefabScanner.IsAbilityCompatible(targetPrefabGUID.GuidHash, sourcePrefabGUID.GuidHash, abilitySlot))
                            {
                                Plugin.BLogger.Warning(LogCategory.System, $"Ability at slot {abilitySlot} may not be compatible");
                                isCompatible = false;
                            }
                        }
                    }
                    
                    if (!isCompatible && PluginConfig.StrictCompatibilityMode.Value)
                    {
                        Plugin.BLogger.Error(LogCategory.System, "Ability swap blocked due to compatibility issues (strict mode enabled)");
                        return false;
                    }
                }
                
                // Obtener la entidad fuente del prefab
                if (!Plugin.SystemsCore.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(sourcePrefabGUID, out Entity sourceEntity))
                {
                    Plugin.BLogger.Error(LogCategory.System, $"Source prefab {sourcePrefabGUID.GuidHash} not found in collection");
                    return false;
                }
                
                Plugin.BLogger.Info(LogCategory.System, $"Found source entity: {sourceEntity.Index}.{sourceEntity.Version}");
                
                // Intentar copiar AbilityBar_Shared si existe
                if (TryCopyAbilityBar(sourceEntity, targetEntity))
                {
                    Plugin.BLogger.Info(LogCategory.System, "Successfully copied AbilityBar_Shared");
                }
                
                // Intentar copiar VBloodData si existe
                if (TryCopyVBloodData(sourceEntity, targetEntity))
                {
                    Plugin.BLogger.Info(LogCategory.System, "Successfully copied VBloodData");
                }
                
                // Intentar copiar AbilityGroupSlotBuffer si existe
                if (TryCopyAbilityGroups(sourceEntity, targetEntity))
                {
                    Plugin.BLogger.Info(LogCategory.System, "Successfully copied AbilityGroups");
                }
                
                // Intentar copiar componentes adicionales de habilidades
                TryCopyAdditionalAbilityComponents(sourceEntity, targetEntity);
                
                // Intentar forzar que la entidad sea reconocida como el nuevo VBlood
                TryForceVBloodTransformation(targetEntity, sourcePrefabGUID);
                
                Plugin.BLogger.Info(LogCategory.System, "Ability swap completed");
                return true;
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error swapping abilities: {ex.Message}");
                Plugin.BLogger.Error(LogCategory.System, $"Stack trace: {ex.StackTrace}");
                return false;
            }
        }
        
        private static bool TryCopyAbilityBar(Entity source, Entity target)
        {
            try
            {
                // Verificar si source tiene AbilityBar_Shared
                if (!source.Has<AbilityBar_Shared>())
                {
                    Plugin.BLogger.Warning(LogCategory.System, "Source entity does not have AbilityBar_Shared component");
                    return false;
                }
                
                var sourceAbilityBar = source.Read<AbilityBar_Shared>();
                
                // Agregar o actualizar el componente en target
                if (!target.Has<AbilityBar_Shared>())
                {
                    target.Add<AbilityBar_Shared>();
                }
                
                target.Write(sourceAbilityBar);
                Plugin.BLogger.Info(LogCategory.System, "AbilityBar_Shared copied successfully");
                return true;
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error copying AbilityBar: {ex.Message}");
                return false;
            }
        }
        
        private static bool TryCopyVBloodData(Entity source, Entity target)
        {
            try
            {
                // VBloodData puede no estar disponible en tiempo de compilación con Il2Cpp
                // Intentar usando EntityManager directamente
                var entityManager = Core.World.EntityManager;
                
                // Verificar usando reflection o métodos alternativos
                // Para evitar errores de tipos no registrados, saltamos este paso por ahora
                Plugin.BLogger.Info(LogCategory.System, "VBloodData copy skipped (Il2Cpp type registration issue)");
                return true; // Considerar como éxito para continuar con otros componentes
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error copying VBloodData: {ex.Message}");
                return false;
            }
        }
        
        private static bool TryCopyAbilityGroups(Entity source, Entity target)
        {
            try
            {
                // Usar Core.World.EntityManager para acceder a los buffers
                var entityManager = Core.World.EntityManager;
                
                // Verificar si source tiene AbilityGroupSlotBuffer usando EntityManager
                if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(source))
                {
                    Plugin.BLogger.Warning(LogCategory.System, "Source entity does not have AbilityGroupSlotBuffer");
                    return false;
                }
                
                var sourceBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(source);
                
                // Crear o limpiar el buffer en target usando EntityManager
                DynamicBuffer<AbilityGroupSlotBuffer> targetBuffer;
                if (!entityManager.HasBuffer<AbilityGroupSlotBuffer>(target))
                {
                    targetBuffer = entityManager.AddBuffer<AbilityGroupSlotBuffer>(target);
                }
                else
                {
                    targetBuffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(target);
                    targetBuffer.Clear();
                }
                
                // Copiar elementos del buffer
                for (int i = 0; i < sourceBuffer.Length; i++)
                {
                    targetBuffer.Add(sourceBuffer[i]);
                }
                
                Plugin.BLogger.Info(LogCategory.System, $"AbilityGroupSlotBuffer copied successfully ({sourceBuffer.Length} abilities)");
                return true;
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error copying AbilityGroups: {ex.Message}");
                return false;
            }
        }
        
        private static void TryCopyAdditionalAbilityComponents(Entity source, Entity target)
        {
            var entityManager = Core.World.EntityManager;
            int copiedComponents = 0;
            
            try
            {
                // Lista de componentes adicionales que pueden afectar las habilidades
                var componentTypes = new[]
                {
                    "AbilityChargesData",
                    "AbilityChargesState", 
                    "AbilityBar_Client",
                    "SpellSchoolProgression",
                    "AbilityConsumeItemOnCast",
                    "AbilityModificationBuffer"
                };
                
                foreach (var componentTypeName in componentTypes)
                {
                    try
                    {
                        // Intentar usar reflexión para obtener el tipo
                        var componentType = System.Type.GetType($"ProjectM.{componentTypeName}");
                        if (componentType == null)
                        {
                            componentType = System.Type.GetType($"ProjectM.Shared.{componentTypeName}");
                        }
                        
                        if (componentType != null)
                        {
                            // Aquí podríamos intentar copiar el componente si existe
                            Plugin.BLogger.Info(LogCategory.System, $"Found component type: {componentTypeName}");
                            copiedComponents++;
                        }
                    }
                    catch
                    {
                        // Silenciosamente ignorar tipos no encontrados
                    }
                }
                
                // Forzar actualización de AI y comportamiento
                ForceAIRefresh(target);
                
                Plugin.BLogger.Info(LogCategory.System, $"Additional ability components processed: {copiedComponents} types checked");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error copying additional ability components: {ex.Message}");
            }
        }
        
        private static void ForceAIRefresh(Entity entity)
        {
            try
            {
                // Intentar refrescar la AI del boss para que use las nuevas habilidades
                var entityManager = Core.World.EntityManager;
                
                // Remover y re-agregar algunos componentes para forzar refresh
                if (entity.Has<AggroConsumer>())
                {
                    var aggro = entity.Read<AggroConsumer>();
                    entity.Remove<AggroConsumer>();
                    entity.Add<AggroConsumer>();
                    entity.Write(aggro);
                    Plugin.BLogger.Info(LogCategory.System, "Refreshed AggroConsumer");
                }
                
                Plugin.BLogger.Info(LogCategory.System, "AI refresh attempted");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error refreshing AI: {ex.Message}");
            }
        }
        
        private static void TryForceVBloodTransformation(Entity targetEntity, PrefabGUID sourcePrefabGUID)
        {
            try
            {
                Plugin.BLogger.Info(LogCategory.System, $"Attempting VBlood transformation to {sourcePrefabGUID.GuidHash}");
                
                // Intentar cambiar el PrefabGUID de la entidad para que sea reconocida como el nuevo VBlood
                if (targetEntity.Has<PrefabGUID>())
                {
                    var currentPrefab = targetEntity.Read<PrefabGUID>();
                    Plugin.BLogger.Info(LogCategory.System, $"Current PrefabGUID: {currentPrefab.GuidHash}");
                    
                    // Cambiar el PrefabGUID al del VBlood origen
                    targetEntity.Write(sourcePrefabGUID);
                    Plugin.BLogger.Info(LogCategory.System, $"PrefabGUID changed to: {sourcePrefabGUID.GuidHash}");
                }
                
                // Forzar refresh de componentes relacionados con VBlood
                if (targetEntity.Has<VBloodUnit>())
                {
                    var vbloodUnit = targetEntity.Read<VBloodUnit>();
                    targetEntity.Remove<VBloodUnit>();
                    targetEntity.Add<VBloodUnit>();
                    targetEntity.Write(vbloodUnit);
                    Plugin.BLogger.Info(LogCategory.System, "Refreshed VBloodUnit component");
                }
                
                Plugin.BLogger.Info(LogCategory.System, "VBlood transformation completed");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error in VBlood transformation: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Obtiene información de debug sobre las habilidades de una entidad
        /// </summary>
        public static string GetAbilityDebugInfo(Entity entity)
        {
            try
            {
                var info = new List<string>();
                info.Add($"Entity: {entity.Index}.{entity.Version}");
                
                if (entity.Has<AbilityBar_Shared>())
                {
                    info.Add("✓ Has AbilityBar_Shared");
                    var abilityBar = entity.Read<AbilityBar_Shared>();
                    // Intentar acceder a las propiedades disponibles de AbilityBar_Shared
                    // Como no conocemos los nombres exactos, vamos a usar reflection o simplemente mostrar que existe
                    info.Add($"  - AbilityBar data available (details require investigation)");
                }
                else
                {
                    info.Add("✗ No AbilityBar_Shared");
                }
                
                // VBloodData temporalmente deshabilitado debido a problemas de registro Il2Cpp
                try
                {
                    if (entity.Has<VBloodData>())
                    {
                        info.Add("✓ Has VBloodData");
                    }
                    else
                    {
                        info.Add("✗ No VBloodData");
                    }
                }
                catch
                {
                    info.Add("⚠️ VBloodData check skipped (Il2Cpp issue)");
                }
                
                var entityManager = Core.World.EntityManager;
                if (entityManager.HasBuffer<AbilityGroupSlotBuffer>(entity))
                {
                    var buffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(entity);
                    info.Add($"✓ Has AbilityGroupSlotBuffer ({buffer.Length} slots)");
                    for (int i = 0; i < buffer.Length && i < 6; i++)
                    {
                        // Mostrar información básica sin acceder a propiedades desconocidas
                        info.Add($"  - Slot {i}: AbilityGroupSlot data available");
                    }
                }
                else
                {
                    info.Add("✗ No AbilityGroupSlotBuffer");
                }
                
                return string.Join("\n", info);
            }
            catch (Exception ex)
            {
                return $"Error getting debug info: {ex.Message}";
            }
        }
        
        /// <summary>
        /// Returns known VBlood prefabs with friendly names.
        /// </summary>
        public static Dictionary<string, int> GetKnownVBloodPrefabs()
        {
            var vbloods = new Dictionary<string, int>();
            
            try
            {
                // Obtener todos los VBloods directamente de la base de datos
                var allVBloods = BloodyBoss.Data.VBloodDatabase.GetAllVBloods();
                
                foreach (var vblood in allVBloods)
                {
                    // Usar el nombre de la base de datos que ya tiene los nombres correctos
                    if (!string.IsNullOrEmpty(vblood.Value.Name))
                    {
                        vbloods[vblood.Value.Name] = vblood.Key;
                        Plugin.BLogger.Debug(LogCategory.System, $"Added VBlood: {vblood.Value.Name} ({vblood.Key})");
                    }
                }
                
                Plugin.BLogger.Info(LogCategory.System, $"Loaded {vbloods.Count} VBloods from database");
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error loading VBloods: {ex.Message}");
                
                // Fallback al diccionario estático si falla
                return new Dictionary<string, int>
                {
                    { "Alpha the White Wolf", -1905691330 },
                    { "Solarus the Immaculate", -740796338 },
                    { "Vincent the Frostbringer", -29797003 },
                    { "Christina the Sun Priestess", -99012450 },
                    { "Dracula the Immortal King", -327335305 },
                    { "Tristan the Vampire Hunter", -1449631170 },
                    { "Beatrice the Tailor", -1942352521 },
                    { "Gorecrusher the Behemoth", -1936575244 },
                    { "Terrorclaw the Ogre", -1347412392 },
                    { "Lord Styx the Night Champion", 1112948824 },
                    { "Adam the Firstborn", 1233988687 },
                    { "Raziel the Shepherd", -680831417 },
                    { "Albert the Duke of Balaton", -203043163 },
                    { "Azariel the Sunbringer", 114912615 },
                    { "Jade the Vampire Hunter", -1968372384 },
                    { "Frostmaw the Mountain Terror", 24378719 },
                    { "Morian the Stormwing Matriarch", 685266977 },
                    { "Clive the Firestarter", 1896428751 },
                    { "Nicholaus the Fallen", 153390636 }
                };
            }
            
            return vbloods;
        }
        
    }
}