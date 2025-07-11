using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ProjectM;
using ProjectM.Gameplay.Scripting;
using ProjectM.Shared;
using Unity.Entities;
using Unity.Collections;
using Bloody.Core;
using Bloody.Core.Helper.v1;
using Stunlock.Core;
using BloodyBoss.Models;
using ProjectM.Gameplay.Systems;

namespace BloodyBoss.Systems
{
    public static class VBloodPrefabScanner
    {
        private static Dictionary<int, VBloodInfo> _vbloodCache = new();

        public static void ScanVBloodPrefabs()
        {
            try
            {
                _vbloodCache.Clear();
                Plugin.BLogger.Info(LogCategory.System, "Starting VBlood prefab scan...");
                
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                if (prefabSystem == null)
                {
                    Plugin.BLogger.Error(LogCategory.System, "PrefabCollectionSystem not available");
                    return;
                }

                // Usar QueryComponents como método principal
                ScanUsingQueryComponents();
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error in VBlood prefab scan: {ex.Message}");
            }
        }

        private static void ScanUsingQueryComponents()
        {
            try
            {
                Plugin.BLogger.Info(LogCategory.System, "Using QueryComponents to find VBloods...");
                
                // Usar el helper de BloodyCore
                var vBloods = Bloody.Core.Helper.v1.QueryComponents.GetEntitiesByComponentTypes<VBloodUnit>(EntityQueryOptions.Default, true);
                
                Plugin.BLogger.Info(LogCategory.System, $"QueryComponents found {vBloods.Length} VBlood entities");
                
                foreach (var entity in vBloods)
                {
                    try
                    {
                        var prefabGuid = entity.Read<PrefabGUID>();
                        if (!_vbloodCache.ContainsKey(prefabGuid.GuidHash))
                        {
                            var vbloodInfo = ExtractVBloodInfo(entity, prefabGuid);
                            if (vbloodInfo != null)
                            {
                                _vbloodCache[prefabGuid.GuidHash] = vbloodInfo;
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Error in QueryComponents scan: {ex.Message}");
            }
        }

        private static VBloodInfo ExtractVBloodInfo(Entity entity, PrefabGUID prefabGuid)
        {
            try
            {
                var vbloodInfo = new VBloodInfo
                {
                    PrefabGUID = prefabGuid,
                    Name = GetVBloodName(entity, prefabGuid),
                    Level = 0
                };

                // Intentar usar GameData.Npcs.FromEntity para obtener más información
                try
                {
                    var npcModel = Bloody.Core.GameData.v1.GameData.Npcs.FromEntity(entity);
                    
                    // El NpcModel tiene acceso a EntityCategory a través de Internals
                    var internals = npcModel.Internals;
                    if (internals?.EntityCategory != null)
                    {
                        var entityCategory = internals.EntityCategory.Value;
                        var unitCategory = entityCategory.UnitCategory;
                        
                        // Agregar categoría principal
                        vbloodInfo.Features.Add($"UnitCategory:{unitCategory}");
                        
                        // Marcar según categoría
                        switch (unitCategory)
                        {
                            case UnitCategory.Beast:
                                vbloodInfo.Features.Add("Beast");
                                break;
                            case UnitCategory.Human:
                                vbloodInfo.Features.Add("Human");
                                vbloodInfo.Features.Add("Humanoid");
                                break;
                            case UnitCategory.Undead:
                                vbloodInfo.Features.Add("Undead");
                                vbloodInfo.Features.Add("Humanoid");
                                break;
                            case UnitCategory.Demon:
                                vbloodInfo.Features.Add("Demon");
                                break;
                            case UnitCategory.Mechanical:
                                vbloodInfo.Features.Add("Mechanical");
                                break;
                        }
                    }
                }
                catch
                {
                    // Si falla GameData, usar análisis por nombre como fallback
                    Plugin.BLogger.Debug(LogCategory.System, $"Could not use GameData.Npcs for {vbloodInfo.Name}, using name analysis");
                }

                // Obtener nivel
                if (entity.Has<UnitLevel>())
                {
                    vbloodInfo.Level = entity.Read<UnitLevel>().Level;
                }

                // Analizar características
                if (entity.Has<CanFly>())
                {
                    vbloodInfo.CanFly = true;
                    vbloodInfo.Features.Add("CanFly");
                }
                
                // Analizar el nombre para determinar características adicionales
                var nameLower = vbloodInfo.Name.ToLower();
                
                // Detectar tipo de enemigo por nombre (como respaldo o información adicional)
                if (nameLower.Contains("vampire")) vbloodInfo.Features.Add("Vampire");
                if (nameLower.Contains("bandit")) vbloodInfo.Features.Add("Bandit");
                if (nameLower.Contains("militia")) vbloodInfo.Features.Add("Militia");
                if (nameLower.Contains("church")) vbloodInfo.Features.Add("Church");
                if (nameLower.Contains("gloomrot")) vbloodInfo.Features.Add("Gloomrot");
                if (nameLower.Contains("cursed")) vbloodInfo.Features.Add("Cursed");
                if (nameLower.Contains("werewolf")) vbloodInfo.Features.Add("Werewolf");
                
                // Detectar si es un GateBoss
                if (nameLower.Contains("gateboss"))
                {
                    vbloodInfo.Features.Add("GateBoss");
                    if (nameLower.Contains("major")) vbloodInfo.Features.Add("GateBoss_Major");
                    if (nameLower.Contains("minor")) vbloodInfo.Features.Add("GateBoss_Minor");
                }

                // Extraer habilidades
                ExtractAbilities(entity, vbloodInfo);

                return vbloodInfo;
            }
            catch
            {
                return null;
            }
        }

        private static void ExtractAbilities(Entity entity, VBloodInfo vbloodInfo)
        {
            try
            {
                if (!Core.World.EntityManager.HasBuffer<AbilityGroupSlotBuffer>(entity))
                {
                    return;
                }

                var abilityBuffer = Core.World.EntityManager.GetBuffer<AbilityGroupSlotBuffer>(entity);
                var entityManager = Core.World.EntityManager;
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                
                for (int i = 0; i < abilityBuffer.Length; i++)
                {
                    try
                    {
                        var slot = abilityBuffer[i];
                        var abilityPrefabGuid = slot.BaseAbilityGroupOnSlot;
                        
                        if (abilityPrefabGuid.GuidHash == 0)
                        {
                            continue; // Skip empty slots
                        }
                        
                        var abilityInfo = new VBloodAbilityInfo
                        {
                            SlotIndex = i,
                            AbilityPrefabGUID = abilityPrefabGuid,
                            Name = $"Ability_{abilityPrefabGuid.GuidHash}"
                        };
                        
                        // Agregar información adicional del slot
                        abilityInfo.ExtraData["ShowOnBar"] = slot.ShowOnBar;

                        // Try to get more information from the ability entity
                        if (prefabSystem != null)
                        {
                            var nameLower = "";
                            
                            // Try to get ability name from PrefabDataLookup
                            if (prefabSystem._PrefabDataLookup.TryGetValue(abilityPrefabGuid, out var prefabData))
                            {
                                var assetName = prefabData.AssetName.ToString();
                                abilityInfo.Name = assetName;
                                
                                // Analyze ability name for better categorization
                                nameLower = assetName.ToLower();
                                
                                // Better categorization based on ability name patterns
                                if (nameLower.Contains("melee") || nameLower.Contains("slash") || nameLower.Contains("strike") || 
                                    nameLower.Contains("swing") || nameLower.Contains("bite") || nameLower.Contains("claw"))
                                {
                                    abilityInfo.Category = AbilityCategory.BasicAttack;
                                }
                                else if (nameLower.Contains("projectile") || nameLower.Contains("spell") || nameLower.Contains("cast") ||
                                         nameLower.Contains("magic") || nameLower.Contains("frost") || nameLower.Contains("fire") ||
                                         nameLower.Contains("shadow") || nameLower.Contains("holy") || nameLower.Contains("blood"))
                                {
                                    abilityInfo.Category = AbilityCategory.Special;
                                }
                                else if (nameLower.Contains("dash") || nameLower.Contains("teleport") || nameLower.Contains("blink") ||
                                         nameLower.Contains("charge") || nameLower.Contains("jump") || nameLower.Contains("leap") ||
                                         nameLower.Contains("movement") || nameLower.Contains("travel"))
                                {
                                    abilityInfo.Category = AbilityCategory.Movement;
                                }
                                else if (nameLower.Contains("shield") || nameLower.Contains("block") || nameLower.Contains("parry") ||
                                         nameLower.Contains("counter") || nameLower.Contains("barrier") || nameLower.Contains("protect"))
                                {
                                    abilityInfo.Category = AbilityCategory.Defensive;
                                }
                                else if (nameLower.Contains("ultimate") || nameLower.Contains("veil") || nameLower.Contains("transform") ||
                                         nameLower.Contains("shapeshift") || nameLower.Contains("wolf_form") || nameLower.Contains("bat_form"))
                                {
                                    abilityInfo.Category = AbilityCategory.Ultimate;
                                }
                                else if (nameLower.Contains("summon") || nameLower.Contains("spawn") || nameLower.Contains("minion"))
                                {
                                    abilityInfo.Category = AbilityCategory.Summon;
                                }
                                else if (nameLower.Contains("buff") || nameLower.Contains("rage") || nameLower.Contains("frenzy") ||
                                         nameLower.Contains("empower") || nameLower.Contains("boost"))
                                {
                                    abilityInfo.Category = AbilityCategory.Buff;
                                }
                                else
                                {
                                    // Fall back to index-based categorization
                                    if (i < 3) abilityInfo.Category = AbilityCategory.BasicAttack;
                                    else if (i < 6) abilityInfo.Category = AbilityCategory.Special;
                                    else if (i < 10) abilityInfo.Category = AbilityCategory.Movement;
                                    else abilityInfo.Category = AbilityCategory.Unknown;
                                }
                            }
                            
                            // Try to get the ability group entity to check for cast time and other properties
                            if (prefabSystem._PrefabGuidToEntityMap.TryGetValue(abilityPrefabGuid, out Entity abilityGroupEntity))
                            {
                                try
                                {
                                    // First check if ability group itself has cast time (rare)
                                    if (abilityGroupEntity.Has<AbilityCastTimeData>())
                                    {
                                        // If it has cast time data component, it's likely a channeled ability
                                        abilityInfo.IsChanneled = true;
                                        
                                        try
                                        {
                                            var castTimeData = abilityGroupEntity.Read<AbilityCastTimeData>();
                                            ExtractCastTimeData(castTimeData, abilityInfo);
                                        }
                                        catch (Exception ex) 
                                        {
                                            Plugin.BLogger.Debug(LogCategory.System, $"Could not read AbilityCastTimeData from group: {ex.Message}");
                                        }
                                    }
                                    
                                    // IMPORTANT: Analyze AbilityGroupStartAbilitiesBuffer to find individual abilities
                                    if (entityManager.HasBuffer<AbilityGroupStartAbilitiesBuffer>(abilityGroupEntity))
                                    {
                                        try
                                        {
                                            var startAbilitiesBuffer = entityManager.GetBuffer<AbilityGroupStartAbilitiesBuffer>(abilityGroupEntity);
                                            Plugin.BLogger.Debug(LogCategory.System, $"Found {startAbilitiesBuffer.Length} individual abilities in group");
                                            
                                            // Analyze first individual ability (usually the main cast)
                                            if (startAbilitiesBuffer.Length > 0)
                                            {
                                                var startAbility = startAbilitiesBuffer[0];
                                                
                                                // Use reflection to get PrefabGUID field
                                                var startAbilityType = startAbility.GetType();
                                                var fields = startAbilityType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                                                
                                                foreach (var field in fields)
                                                {
                                                    if (field.FieldType == typeof(PrefabGUID) && field.Name == "PrefabGUID")
                                                    {
                                                        var individualAbilityGuid = (PrefabGUID)field.GetValue(startAbility);
                                                        if (individualAbilityGuid.GuidHash != 0)
                                                        {
                                                            // Analyze the individual ability entity
                                                            AnalyzeIndividualAbility(individualAbilityGuid, abilityInfo);
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Plugin.BLogger.Debug(LogCategory.System, $"Could not analyze AbilityGroupStartAbilitiesBuffer: {ex.Message}");
                                        }
                                    }
                                    
                                    // Check for animation requirements and sequence
                                    if (abilityGroupEntity.Has<PlaySequenceOnGameplayEventAuthoring>())
                                    {
                                        abilityInfo.RequiresAnimation = true;
                                        // Authoring components are reference types, not value types
                                        // We can only check for their existence, not read their data directly
                                        abilityInfo.AnimationSequence = "HasPlaySequence"; // Indicates it has a play sequence component
                                    }
                                    
                                    // Check for more ability information
                                    if (abilityGroupEntity.Has<AbilityGroupState>())
                                    {
                                        try
                                        {
                                            var abilityState = abilityGroupEntity.Read<AbilityGroupState>();
                                            var stateFields = typeof(AbilityGroupState).GetFields(BindingFlags.Public | BindingFlags.Instance);
                                            
                                            foreach (var field in stateFields)
                                            {
                                                var fieldValue = field.GetValue(abilityState);
                                                Plugin.BLogger.Debug(LogCategory.System, $"AbilityGroupState field: {field.Name} = {fieldValue}");
                                                
                                                // Store any interesting state fields
                                                if (field.Name.ToLower().Contains("charges") && field.FieldType == typeof(int))
                                                {
                                                    abilityInfo.Charges = (int)fieldValue;
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    
                                    // Check for combo information
                                    if (abilityGroupEntity.Has<AbilityGroupComboState>())
                                    {
                                        try
                                        {
                                            var comboState = abilityGroupEntity.Read<AbilityGroupComboState>();
                                            var comboFields = typeof(AbilityGroupComboState).GetFields(BindingFlags.Public | BindingFlags.Instance);
                                            
                                            abilityInfo.IsCombo = true;
                                            
                                            foreach (var field in comboFields)
                                            {
                                                var fieldValue = field.GetValue(comboState);
                                                Plugin.BLogger.Debug(LogCategory.System, $"Combo field: {field.Name} = {fieldValue}");
                                                
                                                if (field.Name.ToLower().Contains("length") && field.FieldType == typeof(int))
                                                {
                                                    abilityInfo.ComboLength = (int)fieldValue;
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    
                                    // Check for ability group info
                                    if (abilityGroupEntity.Has<AbilityGroupInfo>())
                                    {
                                        try
                                        {
                                            var groupInfo = abilityGroupEntity.Read<AbilityGroupInfo>();
                                            var infoFields = typeof(AbilityGroupInfo).GetFields(BindingFlags.Public | BindingFlags.Instance);
                                            
                                            foreach (var field in infoFields)
                                            {
                                                var fieldValue = field.GetValue(groupInfo);
                                                Plugin.BLogger.Debug(LogCategory.System, $"AbilityGroupInfo field: {field.Name} = {fieldValue}");
                                                
                                                // Extract specific fields we care about
                                                if (field.Name.ToLower().Contains("minrange") && field.FieldType == typeof(float))
                                                {
                                                    abilityInfo.ExtraData["MinRange"] = (float)fieldValue;
                                                }
                                                else if (field.Name.ToLower().Contains("maxrange") && field.FieldType == typeof(float))
                                                {
                                                    abilityInfo.ExtraData["MaxRange"] = (float)fieldValue;
                                                }
                                                else if (field.Name.ToLower().Contains("behaviortype"))
                                                {
                                                    abilityInfo.ExtraData["BehaviorType"] = fieldValue?.ToString() ?? "None";
                                                }
                                                else if (field.Name.ToLower().Contains("inputtype"))
                                                {
                                                    abilityInfo.ExtraData["InputType"] = fieldValue?.ToString() ?? "None";
                                                }
                                                else if (field.Name.ToLower().Contains("target"))
                                                {
                                                    abilityInfo.ExtraData["Target"] = fieldValue?.ToString() ?? "None";
                                                }
                                                else if (field.Name.ToLower().Contains("cooldown") && field.FieldType == typeof(float))
                                                {
                                                    abilityInfo.Cooldown = (float)fieldValue;
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    
                                    // Check for additional ability components
                                    if (abilityGroupEntity.Has<AbilityChargesData>())
                                    {
                                        try
                                        {
                                            var chargesData = abilityGroupEntity.Read<AbilityChargesData>();
                                            var chargeFields = typeof(AbilityChargesData).GetFields(BindingFlags.Public | BindingFlags.Instance);
                                            
                                            foreach (var field in chargeFields)
                                            {
                                                var fieldValue = field.GetValue(chargesData);
                                                if (field.Name.ToLower().Contains("maxcharges") && field.FieldType == typeof(int))
                                                {
                                                    abilityInfo.Charges = (int)fieldValue;
                                                }
                                                else if (field.Name.ToLower().Contains("chargetime") && field.FieldType == typeof(float))
                                                {
                                                    abilityInfo.ExtraData["ChargeTime"] = (float)fieldValue;
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    
                                    // Check for ability cooldown data
                                    if (abilityGroupEntity.Has<AbilityCooldownData>())
                                    {
                                        try
                                        {
                                            var cooldownData = abilityGroupEntity.Read<AbilityCooldownData>();
                                            var cooldownFields = typeof(AbilityCooldownData).GetFields(BindingFlags.Public | BindingFlags.Instance);
                                            
                                            foreach (var field in cooldownFields)
                                            {
                                                var fieldValue = field.GetValue(cooldownData);
                                                if (field.Name.ToLower().Contains("cooldown") && field.FieldType == typeof(float))
                                                {
                                                    abilityInfo.Cooldown = (float)fieldValue;
                                                    Plugin.BLogger.Debug(LogCategory.System, $"Found Cooldown field '{field.Name}' with value: {fieldValue}");
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    
                                    // Check for ability target data
                                    // TODO: Find correct component name through VBloodComponentDebugger
                                    /*
                                    if (abilityGroupEntity.Has<AbilityTargetArcInfo>())
                                    {
                                        try
                                        {
                                            var targetArc = abilityGroupEntity.Read<AbilityTargetArcInfo>();
                                            var arcFields = typeof(AbilityTargetArcInfo).GetFields(BindingFlags.Public | BindingFlags.Instance);
                                            
                                            foreach (var field in arcFields)
                                            {
                                                var fieldValue = field.GetValue(targetArc);
                                                if (field.Name.ToLower().Contains("angle") && field.FieldType == typeof(float))
                                                {
                                                    abilityInfo.ExtraData["TargetAngle"] = (float)fieldValue;
                                                }
                                                else if (field.Name.ToLower().Contains("radius") && field.FieldType == typeof(float))
                                                {
                                                    abilityInfo.ExtraData["TargetRadius"] = (float)fieldValue;
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    */
                                    
                                    // Additional checks based on name patterns for flight
                                    if (nameLower.Contains("bat_form") || nameLower.Contains("fly") || nameLower.Contains("wing"))
                                    {
                                        abilityInfo.RequiresFlight = true;
                                    }
                                    
                                    // Check for projectile component
                                    if (abilityGroupEntity.Has<ProjectileComponent>())
                                    {
                                        if (abilityInfo.Category == AbilityCategory.Unknown || 
                                            abilityInfo.Category == AbilityCategory.BasicAttack)
                                            abilityInfo.Category = AbilityCategory.Special;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Plugin.BLogger.Debug(LogCategory.System, $"Could not check components for ability {abilityPrefabGuid.GuidHash}: {ex.Message}");
                                }
                            }
                        }

                        vbloodInfo.Abilities[i] = abilityInfo;
                    }
                    catch (Exception ex)
                    {
                        Plugin.BLogger.Debug(LogCategory.System, $"Error extracting ability at slot {i}: {ex.Message}");
                    }
                }
                
                // Log summary of extracted abilities with detailed info
                Plugin.BLogger.Debug(LogCategory.System, $"Extracted {vbloodInfo.Abilities.Count} abilities for {vbloodInfo.Name}");
                foreach (var ability in vbloodInfo.Abilities.Values)
                {
                    var details = new List<string>();
                    if (ability.CastTime > 0) details.Add($"Cast:{ability.CastTime}s");
                    if (ability.Cooldown > 0) details.Add($"CD:{ability.Cooldown}s");
                    if (ability.IsChanneled) details.Add("Channeled");
                    if (ability.IsCombo) details.Add($"Combo:{ability.ComboLength}");
                    if (ability.RequiresAnimation) details.Add("Animated");
                    if (ability.RequiresFlight) details.Add("Flight");
                    if (ability.ExtraData.ContainsKey("MinRange")) details.Add($"MinRange:{ability.ExtraData["MinRange"]}");
                    if (ability.ExtraData.ContainsKey("MaxRange")) details.Add($"MaxRange:{ability.ExtraData["MaxRange"]}");
                    if (ability.ExtraData.ContainsKey("BehaviorType")) details.Add($"Behavior:{ability.ExtraData["BehaviorType"]}");
                    
                    var detailsStr = details.Count > 0 ? $" [{string.Join(", ", details)}]" : "";
                    Plugin.BLogger.Debug(LogCategory.System, $"  - Slot {ability.SlotIndex}: {ability.Name} ({ability.Category}){detailsStr}");
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Error(LogCategory.System, $"Failed to extract abilities: {ex.Message}");
            }
        }

        private static void ExtractCastTimeData(AbilityCastTimeData castTimeData, VBloodAbilityInfo abilityInfo)
        {
            var fieldInfos = typeof(AbilityCastTimeData).GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var field in fieldInfos)
            {
                try
                {
                    // Los campos son ModifiableFloat, no float
                    if (field.FieldType == typeof(ModifiableFloat))
                    {
                        var modFloat = (ModifiableFloat)field.GetValue(castTimeData);
                        
                        if (field.Name.ToLower().Contains("maxcasttime") || field.Name == "MaxCastTime")
                        {
                            abilityInfo.CastTime = modFloat.Value;
                            Plugin.BLogger.Info(LogCategory.System, $"Found MaxCastTime: {modFloat.Value}");
                        }
                        else if (field.Name.ToLower().Contains("postcast") || field.Name == "PostCastTime")
                        {
                            abilityInfo.PostCastTime = modFloat.Value;
                            Plugin.BLogger.Info(LogCategory.System, $"Found PostCastTime: {modFloat.Value}");
                        }
                    }
                    else if (field.Name == "HideCastBar" && field.FieldType == typeof(bool))
                    {
                        abilityInfo.HideCastBar = (bool)field.GetValue(castTimeData);
                        Plugin.BLogger.Info(LogCategory.System, $"Found HideCastBar: {abilityInfo.HideCastBar}");
                    }
                }
                catch { }
            }
        }
        
        private static void AnalyzeIndividualAbility(PrefabGUID individualAbilityGuid, VBloodAbilityInfo abilityInfo)
        {
            try
            {
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                if (prefabSystem == null) return;
                
                if (!prefabSystem._PrefabGuidToEntityMap.TryGetValue(individualAbilityGuid, out Entity individualAbilityEntity))
                {
                    Plugin.BLogger.Debug(LogCategory.System, $"Individual ability entity not found for {individualAbilityGuid.GuidHash}");
                    return;
                }
                
                var entityManager = Core.World.EntityManager;
                
                // Check for AbilityCastTimeData in individual ability
                if (entityManager.HasComponent<AbilityCastTimeData>(individualAbilityEntity))
                {
                    abilityInfo.IsChanneled = true;
                    try
                    {
                        var castTimeData = entityManager.GetComponentData<AbilityCastTimeData>(individualAbilityEntity);
                        ExtractCastTimeData(castTimeData, abilityInfo);
                    }
                    catch (Exception ex)
                    {
                        Plugin.BLogger.Debug(LogCategory.System, $"Could not read AbilityCastTimeData from individual ability: {ex.Message}");
                    }
                }
                
                // Check for spawn prefabs
                if (entityManager.HasBuffer<AbilitySpawnPrefabOnCast>(individualAbilityEntity))
                {
                    try
                    {
                        var spawnBuffer = entityManager.GetBuffer<AbilitySpawnPrefabOnCast>(individualAbilityEntity);
                        for (int i = 0; i < spawnBuffer.Length; i++)
                        {
                            var spawn = spawnBuffer[i];
                            var spawnType = spawn.GetType();
                            var fields = spawnType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                            
                            var spawnInfo = new SpawnInfo();
                            
                            foreach (var field in fields)
                            {
                                try
                                {
                                    var value = field.GetValue(spawn);
                                    if (field.Name == "SpawnPrefab" && value is PrefabGUID spawnPrefab)
                                    {
                                        spawnInfo.SpawnPrefab = spawnPrefab;
                                        spawnInfo.SpawnName = spawnPrefab.LookupName() ?? $"Unknown_{spawnPrefab.GuidHash}";
                                    }
                                    else if (field.Name == "Target")
                                    {
                                        spawnInfo.Target = value?.ToString() ?? "Unknown";
                                    }
                                    else if (field.Name == "HoverDistance" && value is float hoverDist)
                                    {
                                        spawnInfo.HoverDistance = hoverDist;
                                    }
                                }
                                catch { }
                            }
                            
                            if (spawnInfo.SpawnPrefab.GuidHash != 0)
                            {
                                abilityInfo.SpawnedPrefabs.Add(spawnInfo);
                            }
                        }
                    }
                    catch { }
                }
                
                // Check for buffs applied - using correct component name from log
                if (entityManager.HasBuffer<ApplyBuffOnGameplayEvent>(individualAbilityEntity))
                {
                    try
                    {
                        var buffBuffer = entityManager.GetBuffer<ApplyBuffOnGameplayEvent>(individualAbilityEntity);
                        for (int i = 0; i < buffBuffer.Length; i++)
                        {
                            var buff = buffBuffer[i];
                            var buffType = buff.GetType();
                            var fields = buffType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                            
                            var buffInfo = new BuffInfo();
                            
                            foreach (var field in fields)
                            {
                                try
                                {
                                    var value = field.GetValue(buff);
                                    if (field.Name.Contains("Buff") && value is PrefabGUID buffPrefab)
                                    {
                                        buffInfo.BuffPrefab = buffPrefab;
                                        buffInfo.BuffName = buffPrefab.LookupName() ?? $"Unknown_{buffPrefab.GuidHash}";
                                    }
                                    else if (field.Name.Contains("Target"))
                                    {
                                        buffInfo.BuffTarget = value?.ToString() ?? "Unknown";
                                    }
                                }
                                catch { }
                            }
                            
                            if (buffInfo.BuffPrefab.GuidHash != 0)
                            {
                                abilityInfo.AppliedBuffs.Add(buffInfo);
                            }
                        }
                    }
                    catch { }
                }
                
                // Check for movement/rotation during cast
                // Components are in ProjectM.Network namespace
                var componentTypes = entityManager.GetComponentTypes(individualAbilityEntity, Allocator.Temp);
                foreach (var componentType in componentTypes)
                {
                    var typeName = componentType.ToString();
                    
                    // Movement during cast
                    if (typeName.Contains("ModifyMovementDuringCast"))
                    {
                        abilityInfo.CanMoveWhileCasting = true;
                        Plugin.BLogger.Info(LogCategory.System, $"Found movement during cast for ability");
                    }
                    
                    // Rotation during cast
                    if (typeName.Contains("ModifyRotationDuringCast"))
                    {
                        abilityInfo.CanRotateWhileCasting = true;
                        Plugin.BLogger.Info(LogCategory.System, $"Found rotation during cast for ability");
                    }
                    
                    // Dash ability
                    if (typeName.Contains("Dash"))
                    {
                        abilityInfo.ExtraData["IsDash"] = true;
                        abilityInfo.ExtraData["DashComponent"] = typeName;
                        if (abilityInfo.Category == AbilityCategory.Unknown)
                            abilityInfo.Category = AbilityCategory.Movement;
                    }
                    
                    // Projectile ability
                    if (typeName.Contains("Projectile") && !typeName.Contains("Network"))
                    {
                        abilityInfo.ExtraData["IsProjectile"] = true;
                        abilityInfo.ExtraData["ProjectileComponent"] = typeName;
                    }
                    
                    // AoE ability
                    if (typeName.Contains("AoE") || typeName.Contains("AreaOfEffect"))
                    {
                        abilityInfo.ExtraData["IsAoE"] = true;
                        abilityInfo.ExtraData["AoEComponent"] = typeName;
                    }
                    
                    // Channeling
                    if (typeName.Contains("Channel"))
                    {
                        abilityInfo.IsChanneled = true;
                        abilityInfo.ExtraData["ChannelComponent"] = typeName;
                    }
                }
                componentTypes.Dispose();
                
                // Extract gameplay event data if exists
                if (entityManager.HasComponent<ForceCastOnGameplayEvent>(individualAbilityEntity))
                {
                    try
                    {
                        var forceCast = entityManager.GetComponentData<ForceCastOnGameplayEvent>(individualAbilityEntity);
                        var forceCastType = forceCast.GetType();
                        var fields = forceCastType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                        
                        foreach (var field in fields)
                        {
                            try
                            {
                                var value = field.GetValue(forceCast);
                                if (value != null)
                                {
                                    abilityInfo.ExtraData[$"ForceCast_{field.Name}"] = value.ToString();
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
                
                // Check for cast conditions
                if (entityManager.HasBuffer<AbilityCastCondition>(individualAbilityEntity))
                {
                    try
                    {
                        var conditionBuffer = entityManager.GetBuffer<AbilityCastCondition>(individualAbilityEntity);
                        if (conditionBuffer.Length > 0)
                        {
                            for (int i = 0; i < conditionBuffer.Length; i++)
                            {
                                var condition = conditionBuffer[i];
                                var conditionType = condition.GetType();
                                var fields = conditionType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                                
                                foreach (var field in fields)
                                {
                                    try
                                    {
                                        var value = field.GetValue(condition);
                                        if (value != null)
                                        {
                                            abilityInfo.CastConditions.Add($"{field.Name}: {value}");
                                        }
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Plugin.BLogger.Debug(LogCategory.System, $"Error analyzing individual ability: {ex.Message}");
            }
        }

        private static string GetVBloodName(Entity entity, PrefabGUID prefabGuid)
        {
            try
            {
                // ManagedCharacterHUD no es accesible directamente, usar el PrefabDataLookup

                // Buscar en PrefabDataLookup
                var prefabSystem = Plugin.SystemsCore?.PrefabCollectionSystem;
                if (prefabSystem != null && prefabSystem._PrefabDataLookup.TryGetValue(prefabGuid, out var prefabData))
                {
                    var assetName = prefabData.AssetName.ToString();
                    return assetName
                        .Replace("CHAR_", "")
                        .Replace("_VBlood", "")
                        .Replace("_Vblood", "")
                        .Replace("_", " ");
                }
            }
            catch { }

            return $"VBlood_{prefabGuid.GuidHash}";
        }

        public static Dictionary<int, VBloodInfo> GetAllVBloods()
        {
            return new Dictionary<int, VBloodInfo>(_vbloodCache);
        }

        public static VBloodInfo GetVBloodInfo(int guidHash)
        {
            return _vbloodCache.TryGetValue(guidHash, out var info) ? info : null;
        }

        public static bool IsAbilityCompatible(int bossPrefab, int sourcePrefab, int abilityIndex)
        {
            var boss = GetVBloodInfo(bossPrefab);
            var source = GetVBloodInfo(sourcePrefab);
            
            if (boss == null || source == null)
            {
                Plugin.BLogger.Warning(LogCategory.System, $"Cannot check compatibility: Boss or source VBlood not found");
                return false;
            }
            
            // Verificar si el source tiene esa habilidad
            if (!source.Abilities.ContainsKey(abilityIndex))
            {
                Plugin.BLogger.Warning(LogCategory.System, $"Source VBlood {source.Name} doesn't have ability at index {abilityIndex}");
                return false;
            }
            
            var sourceAbility = source.Abilities[abilityIndex];
            
            // Verificar compatibilidad de vuelo específica de la habilidad
            if (sourceAbility.RequiresFlight && !boss.CanFly)
            {
                Plugin.BLogger.Warning(LogCategory.System, $"Incompatible: Ability {sourceAbility.Name} requires flight but {boss.Name} cannot fly");
                return false;
            }
            
            // Verificar compatibilidad basada en el comportamiento
            if (sourceAbility.ExtraData.ContainsKey("BehaviorType"))
            {
                var behaviorType = sourceAbility.ExtraData["BehaviorType"].ToString();
                
                // Las habilidades de tipo Travel/Dash pueden requerir características específicas
                if ((behaviorType == "Travel" || behaviorType == "Dash") && sourceAbility.RequiresFlight && !boss.CanFly)
                {
                    Plugin.BLogger.Warning(LogCategory.System, $"Incompatible: {behaviorType} ability requires flight capability");
                    return false;
                }
                
                // Las habilidades de canalización pueden no funcionar bien en bosses que no las soporten
                if (behaviorType == "Channeling")
                {
                    Plugin.BLogger.Info(LogCategory.System, $"Info: Channeling ability {sourceAbility.Name} - ensure boss supports channeling animations");
                }
            }
            
            // Verificar compatibilidad de transformación
            if (source.Features.Contains("Werewolf") && !boss.Features.Contains("Werewolf"))
            {
                Plugin.BLogger.Warning(LogCategory.System, $"Incompatible: {source.Name} has werewolf transformation but {boss.Name} is not a werewolf");
                return false;
            }
            
            // Verificar compatibilidad de tipo de criatura
            bool sourceIsBeast = source.Features.Any(f => f.Contains("Beast") || f.Contains("Wolf") || f.Contains("Bear"));
            bool bossIsBeast = boss.Features.Any(f => f.Contains("Beast") || f.Contains("Wolf") || f.Contains("Bear"));
            
            if (sourceIsBeast && !bossIsBeast)
            {
                Plugin.BLogger.Warning(LogCategory.System, $"Warning: {source.Name} has beast abilities but {boss.Name} is not a beast - animations may not work properly");
                // No lo bloqueamos completamente, solo advertimos
            }
            
            // Verificar compatibilidad de humanoide vs no-humanoide
            bool sourceIsHumanoid = source.Features.Any(f => f.Contains("Human") || f.Contains("Vampire") || f.Contains("Bandit") || f.Contains("Militia") || f.Contains("Church"));
            bool bossIsHumanoid = boss.Features.Any(f => f.Contains("Human") || f.Contains("Vampire") || f.Contains("Bandit") || f.Contains("Militia") || f.Contains("Church"));
            
            if (sourceIsHumanoid != bossIsHumanoid)
            {
                Plugin.BLogger.Warning(LogCategory.System, $"Warning: Model type mismatch between {source.Name} and {boss.Name} - some abilities may not work properly");
            }
            
            // Verificar si es un GateBoss (tienen restricciones especiales)
            if (boss.Features.Contains("GateBoss"))
            {
                Plugin.BLogger.Warning(LogCategory.System, $"Warning: {boss.Name} is a GateBoss - some abilities may be restricted");
            }
            
            // Información adicional sobre la habilidad
            Plugin.BLogger.Info(LogCategory.System, $"Ability compatibility check: {sourceAbility.Name} (Index: {abilityIndex})");
            if (sourceAbility.ExtraData.ContainsKey("MinRange") && sourceAbility.ExtraData.ContainsKey("MaxRange"))
            {
                Plugin.BLogger.Info(LogCategory.System, $"  Range: {sourceAbility.ExtraData["MinRange"]}-{sourceAbility.ExtraData["MaxRange"]}");
            }
            if (sourceAbility.IsCombo)
            {
                Plugin.BLogger.Info(LogCategory.System, $"  Combo ability with {sourceAbility.ComboLength} hits");
            }
            
            return true; // Permitimos la mayoría de combinaciones con advertencias
        }
    }
}