using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Entities;
using Unity.Collections;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using ProjectM;
using Stunlock.Core;
using ProjectM.Shared;
using ProjectM.Gameplay.Scripting;

namespace BloodyBoss.Systems
{
    public static class VBloodComponentDebugger
    {
        private static HashSet<string> _processedTypes = new HashSet<string>();
        private static int _currentDepth = 0;
        private const int MAX_DEPTH = 3;
        
        public static void DebugVBloodComponents()
        {
            try
            {
                Plugin.Logger.LogWarning("========================================");
                Plugin.Logger.LogWarning("STARTING VBLOOD COMPONENT DEEP ANALYSIS");
                Plugin.Logger.LogWarning("========================================");
                
                var vBloods = QueryComponents.GetEntitiesByComponentTypes<VBloodUnit>(EntityQueryOptions.Default, true);
                Plugin.Logger.LogWarning($"Found {vBloods.Length} VBlood entities to analyze");
                
                // Analizar solo los primeros 3 VBloods diferentes
                var analyzedCount = 0;
                var analyzedPrefabs = new HashSet<int>();
                
                foreach (var entity in vBloods)
                {
                    if (analyzedCount >= 3) break;
                    
                    try
                    {
                        var prefabGuid = entity.Read<PrefabGUID>();
                        if (analyzedPrefabs.Contains(prefabGuid.GuidHash)) continue;
                        
                        analyzedPrefabs.Add(prefabGuid.GuidHash);
                        analyzedCount++;
                        
                        var vbloodName = GetVBloodName(entity, prefabGuid);
                        
                        Plugin.Logger.LogWarning("");
                        Plugin.Logger.LogWarning($"================================================");
                        Plugin.Logger.LogWarning($"ANALYZING VBLOOD #{analyzedCount}: {vbloodName}");
                        Plugin.Logger.LogWarning($"PrefabGUID: {prefabGuid.GuidHash}");
                        Plugin.Logger.LogWarning($"Entity: {entity.Index}:{entity.Version}");
                        Plugin.Logger.LogWarning($"================================================");
                        
                        // Obtener todos los componentes
                        var allComponents = GetAllComponents(entity);
                        Plugin.Logger.LogWarning($"Total components found: {allComponents.Count}");
                        Plugin.Logger.LogWarning("");
                        
                        // Analizar cada componente
                        foreach (var componentType in allComponents)
                        {
                            _processedTypes.Clear();
                            _currentDepth = 0;
                            
                            Plugin.Logger.LogWarning($">>> COMPONENT: {componentType}");
                            
                            try
                            {
                                AnalyzeComponent(entity, componentType, "    ");
                            }
                            catch (Exception ex)
                            {
                                Plugin.Logger.LogWarning($"    ERROR analyzing component: {ex.Message}");
                            }
                            
                            Plugin.Logger.LogWarning("");
                        }
                        
                        // Analizar componentes importantes del VBlood
                        AnalyzeVBloodComponents(entity);
                        
                        // Analizar buffers específicos
                        AnalyzeBuffers(entity);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"ERROR analyzing VBlood entity: {ex.Message}");
                    }
                }
                
                Plugin.Logger.LogWarning("========================================");
                Plugin.Logger.LogWarning("VBLOOD COMPONENT ANALYSIS COMPLETED");
                Plugin.Logger.LogWarning("========================================");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogError($"Fatal error in VBlood component debug: {ex}");
            }
        }
        
        private static List<ComponentType> GetAllComponents(Entity entity)
        {
            var componentTypes = new List<ComponentType>();
            var entityManager = Core.World.EntityManager;
            
            var types = entityManager.GetComponentTypes(entity, Allocator.Temp);
            foreach (var type in types)
            {
                componentTypes.Add(type);
            }
            types.Dispose();
            
            return componentTypes;
        }
        
        private static void AnalyzeComponent(Entity entity, ComponentType componentType, string indent)
        {
            if (_currentDepth > MAX_DEPTH) return;
            
            var typeName = componentType.ToString();
            if (_processedTypes.Contains(typeName)) 
            {
                Plugin.Logger.LogWarning($"{indent}[Already processed, skipping recursion]");
                return;
            }
            
            _processedTypes.Add(typeName);
            _currentDepth++;
            
            try
            {
                // Obtener el tipo real del componente
                var actualType = componentType.GetManagedType();
                if (actualType == null)
                {
                    Plugin.Logger.LogWarning($"{indent}Could not get managed type");
                    return;
                }
                
                // Si es un buffer, analizarlo de forma especial
                if (componentType.IsBuffer)
                {
                    Plugin.Logger.LogWarning($"{indent}[BUFFER] Length info will be shown in buffer section");
                    return;
                }
                
                // Si es ZeroSized, solo mostrar que existe
                if (componentType.IsZeroSized)
                {
                    Plugin.Logger.LogWarning($"{indent}[TAG/ZERO-SIZED COMPONENT]");
                    return;
                }
                
                // Simplemente mostrar que el componente existe
                Plugin.Logger.LogWarning($"{indent}[COMPONENT EXISTS] - Type: {actualType?.Name ?? "Unknown"}");
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"{indent}Could not read component: {ex.Message}");
            }
            finally
            {
                _currentDepth--;
            }
        }
        
        // Method removed to simplify compilation
        
        private static void AnalyzeVBloodComponents(Entity entity)
        {
            Plugin.Logger.LogWarning("");
            Plugin.Logger.LogWarning(">>> VBLOOD IMPORTANT COMPONENTS:");
            
            var entityManager = Core.World.EntityManager;
            
            // UnitStats - Estadísticas de combate
            if (entityManager.HasComponent<UnitStats>(entity))
            {
                Plugin.Logger.LogWarning($"  >>> UnitStats:");
                try
                {
                    var unitStats = entityManager.GetComponentData<UnitStats>(entity);
                    AnalyzeComponentWithReflection(unitStats, "    ");
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"    Error reading: {ex.Message}");
                }
            }
            
            // BuffResistances - Resistencias a buffs/debuffs
            if (entityManager.HasComponent<BuffResistances>(entity))
            {
                Plugin.Logger.LogWarning($"  >>> BuffResistances:");
                try
                {
                    var buffResistances = entityManager.GetComponentData<BuffResistances>(entity);
                    AnalyzeComponentWithReflection(buffResistances, "    ");
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"    Error reading: {ex.Message}");
                }
            }
            
            // DynamicallyWeakenAttackers - Escalado dinámico
            if (entityManager.HasComponent<DynamicallyWeakenAttackers>(entity))
            {
                Plugin.Logger.LogWarning($"  >>> DynamicallyWeakenAttackers:");
                try
                {
                    var dynamicWeaken = entityManager.GetComponentData<DynamicallyWeakenAttackers>(entity);
                    AnalyzeComponentWithReflection(dynamicWeaken, "    ");
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"    Error reading: {ex.Message}");
                }
            }
            
            // DropTableBuffer - Loot tables
            if (entityManager.HasComponent<DropTableBuffer>(entity))
            {
                Plugin.Logger.LogWarning($"  >>> DropTableBuffer:");
                try
                {
                    var dropTable = entityManager.GetComponentData<DropTableBuffer>(entity);
                    AnalyzeComponentWithReflection(dropTable, "    ");
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"    Error reading: {ex.Message}");
                }
            }
            
            // CastHistoryData - Historial de habilidades
            if (entityManager.HasComponent<CastHistoryData>(entity))
            {
                Plugin.Logger.LogWarning($"  >>> CastHistoryData:");
                try
                {
                    var castHistory = entityManager.GetComponentData<CastHistoryData>(entity);
                    AnalyzeComponentWithReflection(castHistory, "    ");
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"    Error reading: {ex.Message}");
                }
            }
            
            // ApplyBuffOnGameplayEvent buffer
            if (entityManager.HasBuffer<ApplyBuffOnGameplayEvent>(entity))
            {
                Plugin.Logger.LogWarning($"  >>> ApplyBuffOnGameplayEvent Buffer:");
                try
                {
                    var buffOnEventBuffer = entityManager.GetBuffer<ApplyBuffOnGameplayEvent>(entity);
                    Plugin.Logger.LogWarning($"    Buffer length: {buffOnEventBuffer.Length}");
                    for (int i = 0; i < Math.Min(buffOnEventBuffer.Length, 5); i++)
                    {
                        var buffOnEvent = buffOnEventBuffer[i];
                        Plugin.Logger.LogWarning($"    Entry {i}:");
                        AnalyzeComponentWithReflection(buffOnEvent, "      ");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"    Error reading buffer: {ex.Message}");
                }
            }
            
            // CreateGameplayEventOnDeath buffer
            if (entityManager.HasBuffer<CreateGameplayEventOnDeath>(entity))
            {
                Plugin.Logger.LogWarning($"  >>> CreateGameplayEventOnDeath Buffer:");
                try
                {
                    var deathEventBuffer = entityManager.GetBuffer<CreateGameplayEventOnDeath>(entity);
                    Plugin.Logger.LogWarning($"    Buffer length: {deathEventBuffer.Length}");
                    for (int i = 0; i < Math.Min(deathEventBuffer.Length, 3); i++)
                    {
                        var deathEvent = deathEventBuffer[i];
                        Plugin.Logger.LogWarning($"    Entry {i}:");
                        AnalyzeComponentWithReflection(deathEvent, "      ");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Logger.LogWarning($"    Error reading buffer: {ex.Message}");
                }
            }
        }
        
        private static void AnalyzeBuffers(Entity entity)
        {
            Plugin.Logger.LogWarning("");
            Plugin.Logger.LogWarning(">>> BUFFER ANALYSIS:");
            
            var entityManager = Core.World.EntityManager;
            
            // AbilityGroupSlotBuffer
            if (entityManager.HasBuffer<AbilityGroupSlotBuffer>(entity))
            {
                var buffer = entityManager.GetBuffer<AbilityGroupSlotBuffer>(entity);
                Plugin.Logger.LogWarning($"  AbilityGroupSlotBuffer: {buffer.Length} slots");
                
                for (int i = 0; i < Math.Min(buffer.Length, 10); i++)
                {
                    try
                    {
                        var slot = buffer[i];
                        Plugin.Logger.LogWarning($"    Slot {i}:");
                        
                        // Usar reflexión para obtener todos los campos
                        var slotType = slot.GetType();
                        var fields = slotType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                        
                        PrefabGUID abilityGroupGuid = default;
                        
                        foreach (var field in fields)
                        {
                            try
                            {
                                var value = field.GetValue(slot);
                                if (value is PrefabGUID prefabGuid && prefabGuid.GuidHash != 0)
                                {
                                    var name = prefabGuid.LookupName() ?? $"Unknown_{prefabGuid.GuidHash}";
                                    Plugin.Logger.LogWarning($"      {field.Name}: {prefabGuid.GuidHash} ({name})");
                                    
                                    if (field.Name == "BaseAbilityGroupOnSlot")
                                    {
                                        abilityGroupGuid = prefabGuid;
                                    }
                                }
                                else if (value != null)
                                {
                                    Plugin.Logger.LogWarning($"      {field.Name}: {value}");
                                }
                            }
                            catch { }
                        }
                        
                        // Analizar la entidad del ability group si existe
                        if (abilityGroupGuid.GuidHash != 0)
                        {
                            AnalyzeAbilityGroupEntity(abilityGroupGuid, i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"    Slot {i}: ERROR - {ex.Message}");
                    }
                }
                
                if (buffer.Length > 10)
                {
                    Plugin.Logger.LogWarning($"    ... and {buffer.Length - 10} more slots");
                }
            }
            
            // Otros buffers comunes
            CheckBuffer<BuffBuffer>(entity, "BuffBuffer");
            CheckBuffer<InventoryBuffer>(entity, "InventoryBuffer");
        }
        
        private static void CheckBuffer<T>(Entity entity, string bufferName) where T : struct
        {
            var entityManager = Core.World.EntityManager;
            if (entityManager.HasBuffer<T>(entity))
            {
                var buffer = entityManager.GetBuffer<T>(entity);
                Plugin.Logger.LogWarning($"  {bufferName}: {buffer.Length} entries");
            }
        }
        
        private static void AnalyzeAbilityGroupEntity(PrefabGUID abilityGroupGuid, int slotIndex)
        {
            try
            {
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                if (prefabSystem == null) return;
                
                if (!prefabSystem._PrefabGuidToEntityMap.TryGetValue(abilityGroupGuid, out Entity abilityEntity))
                {
                    Plugin.Logger.LogWarning($"      [ABILITY GROUP ENTITY NOT FOUND]");
                    return;
                }
                
                Plugin.Logger.LogWarning($"      === ANALYZING ABILITY GROUP ENTITY ===");
                
                // Obtener todos los componentes de la habilidad
                var componentTypes = GetAllComponents(abilityEntity);
                Plugin.Logger.LogWarning($"      Total ability components: {componentTypes.Count}");
                
                // Analizar componentes específicos de habilidades con reflection
                var entityManager = Core.World.EntityManager;
                
                // AbilityCastTimeData
                if (entityManager.HasComponent<AbilityCastTimeData>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"      >>> AbilityCastTimeData:");
                    try
                    {
                        var castTimeData = entityManager.GetComponentData<AbilityCastTimeData>(abilityEntity);
                        AnalyzeComponentWithReflection(castTimeData, "        ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"        Error reading: {ex.Message}");
                    }
                }
                
                // AbilityCooldownData
                if (entityManager.HasComponent<AbilityCooldownData>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"      >>> AbilityCooldownData:");
                    try
                    {
                        var cooldownData = entityManager.GetComponentData<AbilityCooldownData>(abilityEntity);
                        AnalyzeComponentWithReflection(cooldownData, "        ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"        Error reading: {ex.Message}");
                    }
                }
                
                // AbilityChargesData
                if (entityManager.HasComponent<AbilityChargesData>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"      >>> AbilityChargesData:");
                    try
                    {
                        var chargesData = entityManager.GetComponentData<AbilityChargesData>(abilityEntity);
                        AnalyzeComponentWithReflection(chargesData, "        ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"        Error reading: {ex.Message}");
                    }
                }
                
                // AbilityGroupComboState
                if (entityManager.HasComponent<AbilityGroupComboState>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"      >>> AbilityGroupComboState:");
                    try
                    {
                        var comboState = entityManager.GetComponentData<AbilityGroupComboState>(abilityEntity);
                        AnalyzeComponentWithReflection(comboState, "        ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"        Error reading: {ex.Message}");
                    }
                }
                
                // AbilityGroupInfo
                if (entityManager.HasComponent<AbilityGroupInfo>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"      >>> AbilityGroupInfo:");
                    try
                    {
                        var groupInfo = entityManager.GetComponentData<AbilityGroupInfo>(abilityEntity);
                        AnalyzeComponentWithReflection(groupInfo, "        ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"        Error reading: {ex.Message}");
                    }
                }
                
                // Analizar AbilityGroupStartAbilitiesBuffer
                if (entityManager.HasBuffer<AbilityGroupStartAbilitiesBuffer>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"      >>> AbilityGroupStartAbilitiesBuffer:");
                    try
                    {
                        var startAbilitiesBuffer = entityManager.GetBuffer<AbilityGroupStartAbilitiesBuffer>(abilityEntity);
                        Plugin.Logger.LogWarning($"        Buffer length: {startAbilitiesBuffer.Length}");
                        
                        for (int i = 0; i < startAbilitiesBuffer.Length; i++)
                        {
                            try
                            {
                                var startAbility = startAbilitiesBuffer[i];
                                Plugin.Logger.LogWarning($"        Entry {i}:");
                                
                                // Usar reflection para ver los campos
                                var startAbilityType = startAbility.GetType();
                                var fields = startAbilityType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                                
                                PrefabGUID abilityPrefabGuid = default;
                                
                                foreach (var field in fields)
                                {
                                    try
                                    {
                                        var value = field.GetValue(startAbility);
                                        Plugin.Logger.LogWarning($"          {field.Name}: {value}");
                                        
                                        if (value is PrefabGUID prefabGuid && field.Name == "PrefabGUID")
                                        {
                                            abilityPrefabGuid = prefabGuid;
                                            
                                            // Si encontramos un PrefabGUID de ability, analizar esa entidad
                                            if (prefabGuid.GuidHash != 0)
                                            {
                                                AnalyzeIndividualAbilityEntity(prefabGuid, i);
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                            catch (Exception ex)
                            {
                                Plugin.Logger.LogWarning($"        Entry {i}: ERROR - {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"        Error reading buffer: {ex.Message}");
                    }
                }
                
                // Listar otros componentes interesantes
                Plugin.Logger.LogWarning($"      >>> Other components:");
                foreach (var componentType in componentTypes)
                {
                    var typeName = componentType.ToString();
                    if (typeName.Contains("Ability") || typeName.Contains("Cast") || 
                        typeName.Contains("Cooldown") || typeName.Contains("Duration") ||
                        typeName.Contains("Channel") || typeName.Contains("Charge"))
                    {
                        Plugin.Logger.LogWarning($"        - {typeName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"      Error analyzing ability group: {ex.Message}");
            }
        }
        
        private static void AnalyzeIndividualAbilityEntity(PrefabGUID abilityPrefabGuid, int index)
        {
            try
            {
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                if (prefabSystem == null) return;
                
                if (!prefabSystem._PrefabGuidToEntityMap.TryGetValue(abilityPrefabGuid, out Entity abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          [INDIVIDUAL ABILITY ENTITY NOT FOUND]");
                    return;
                }
                
                var abilityName = abilityPrefabGuid.LookupName() ?? $"Unknown_{abilityPrefabGuid.GuidHash}";
                Plugin.Logger.LogWarning($"          ==== ANALYZING INDIVIDUAL ABILITY: {abilityName} ====");
                
                var entityManager = Core.World.EntityManager;
                
                // Buscar componentes específicos de tiempos y mecánicas
                
                // AbilityCastTimeData - Tiempos de casteo
                if (entityManager.HasComponent<AbilityCastTimeData>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> AbilityCastTimeData FOUND");
                    try
                    {
                        var castTimeData = entityManager.GetComponentData<AbilityCastTimeData>(abilityEntity);
                        AnalyzeComponentWithReflection(castTimeData, "            ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading: {ex.Message}");
                    }
                }
                
                // Los componentes de movimiento/rotación durante casteo están en el log pero con namespace Network
                // Los buscaremos por nombre en la lista de componentes
                
                // AbilitySpawnPrefabOnCast buffer - Qué spawneará
                if (entityManager.HasBuffer<AbilitySpawnPrefabOnCast>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> AbilitySpawnPrefabOnCast Buffer FOUND");
                    try
                    {
                        var spawnBuffer = entityManager.GetBuffer<AbilitySpawnPrefabOnCast>(abilityEntity);
                        Plugin.Logger.LogWarning($"            Buffer length: {spawnBuffer.Length}");
                        for (int i = 0; i < Math.Min(spawnBuffer.Length, 5); i++)
                        {
                            var spawn = spawnBuffer[i];
                            Plugin.Logger.LogWarning($"            Entry {i}:");
                            AnalyzeComponentWithReflection(spawn, "              ");
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading buffer: {ex.Message}");
                    }
                }
                
                // AbilityCastCondition buffer - Condiciones de casteo
                if (entityManager.HasBuffer<AbilityCastCondition>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> AbilityCastCondition Buffer FOUND");
                    try
                    {
                        var conditionBuffer = entityManager.GetBuffer<AbilityCastCondition>(abilityEntity);
                        Plugin.Logger.LogWarning($"            Buffer length: {conditionBuffer.Length}");
                        for (int i = 0; i < Math.Min(conditionBuffer.Length, 3); i++)
                        {
                            var condition = conditionBuffer[i];
                            Plugin.Logger.LogWarning($"            Condition {i}:");
                            AnalyzeComponentWithReflection(condition, "              ");
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading buffer: {ex.Message}");
                    }
                }
                
                if (entityManager.HasComponent<ForceCastOnGameplayEvent>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> ForceCastOnGameplayEvent FOUND");
                    try
                    {
                        var forceCast = entityManager.GetComponentData<ForceCastOnGameplayEvent>(abilityEntity);
                        AnalyzeComponentWithReflection(forceCast, "            ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading: {ex.Message}");
                    }
                }
                
                if (entityManager.HasComponent<Dash>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> Dash FOUND");
                    try
                    {
                        var dash = entityManager.GetComponentData<Dash>(abilityEntity);
                        AnalyzeComponentWithReflection(dash, "            ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading: {ex.Message}");
                    }
                }
                
                if (entityManager.HasComponent<Projectile>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> Projectile FOUND");
                    try
                    {
                        var projectile = entityManager.GetComponentData<Projectile>(abilityEntity);
                        AnalyzeComponentWithReflection(projectile, "            ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading: {ex.Message}");
                    }
                }
                
                // Buscar componentes de duración
                if (entityManager.HasComponent<LifeTime>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> LifeTime FOUND");
                    try
                    {
                        var lifeTime = entityManager.GetComponentData<LifeTime>(abilityEntity);
                        AnalyzeComponentWithReflection(lifeTime, "            ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading: {ex.Message}");
                    }
                }
                
                if (entityManager.HasComponent<Age>(abilityEntity))
                {
                    Plugin.Logger.LogWarning($"          >>> Age FOUND");
                    try
                    {
                        var age = entityManager.GetComponentData<Age>(abilityEntity);
                        AnalyzeComponentWithReflection(age, "            ");
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"            Error reading: {ex.Message}");
                    }
                }
                
                // Listar todos los componentes para ver qué más hay
                var componentTypes = GetAllComponents(abilityEntity);
                Plugin.Logger.LogWarning($"          Total components in individual ability: {componentTypes.Count}");
                
                foreach (var componentType in componentTypes)
                {
                    var typeName = componentType.ToString();
                    if ((typeName.Contains("Cast") || typeName.Contains("Duration") || 
                         typeName.Contains("Time") || typeName.Contains("Delay") ||
                         typeName.Contains("Speed") || typeName.Contains("Channel")) &&
                        !typeName.Contains("NetworkSnapshot"))
                    {
                        Plugin.Logger.LogWarning($"            Component: {typeName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"          Error analyzing individual ability: {ex.Message}");
            }
        }
        
        private static void AnalyzeComponentWithReflection<T>(T component, string indent) where T : struct
        {
            try
            {
                var componentType = typeof(T);
                var fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                
                foreach (var field in fields)
                {
                    try
                    {
                        var value = field.GetValue(component);
                        
                        // Manejar tipos Modifiable especiales
                        if (field.FieldType == typeof(ModifiableFloat))
                        {
                            var modFloat = (ModifiableFloat)value;
                            Plugin.Logger.LogWarning($"{indent}{field.Name}: {modFloat.Value} (ModifiableFloat)");
                        }
                        else if (field.FieldType == typeof(ModifiableInt))
                        {
                            var modInt = (ModifiableInt)value;
                            Plugin.Logger.LogWarning($"{indent}{field.Name}: {modInt.Value} (ModifiableInt)");
                        }
                        else if (field.FieldType == typeof(ModifiableBool))
                        {
                            var modBool = (ModifiableBool)value;
                            Plugin.Logger.LogWarning($"{indent}{field.Name}: {modBool.Value} (ModifiableBool)");
                        }
                        else if (field.FieldType == typeof(PrefabGUID))
                        {
                            var prefabGuid = (PrefabGUID)value;
                            var name = prefabGuid.LookupName() ?? $"Unknown_{prefabGuid.GuidHash}";
                            Plugin.Logger.LogWarning($"{indent}{field.Name}: {prefabGuid.GuidHash} ({name})");
                        }
                        else
                        {
                            Plugin.Logger.LogWarning($"{indent}{field.Name}: {value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogWarning($"{indent}{field.Name}: [Error: {ex.Message}]");
                    }
                }
                
                if (fields.Length == 0)
                {
                    Plugin.Logger.LogWarning($"{indent}[No public fields found]");
                }
            }
            catch (Exception ex)
            {
                Plugin.Logger.LogWarning($"{indent}[Reflection error: {ex.Message}]");
            }
        }
        
        private static string GetVBloodName(Entity entity, PrefabGUID prefabGuid)
        {
            try
            {
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                if (prefabSystem != null && prefabSystem._PrefabDataLookup.TryGetValue(prefabGuid, out var prefabData))
                {
                    var assetName = prefabData.AssetName.ToString();
                    return assetName
                        .Replace("CHAR_", "")
                        .Replace("_VBlood", "")
                        .Replace("_", " ");
                }
            }
            catch { }
            
            return $"VBlood_{prefabGuid.GuidHash}";
        }
    }
}