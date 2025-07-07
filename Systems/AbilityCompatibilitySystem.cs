using System;
using System.Collections.Generic;
using System.Linq;
using BloodyBoss.Data;
using BloodyBoss.Models;
using ProjectM;
using Unity.Entities;
using Bloody.Core;

namespace BloodyBoss.Systems
{
    public static class AbilityCompatibilitySystem
    {
        public enum CompatibilityLevel
        {
            Perfect,      // Totalmente compatible
            Good,         // Compatible con advertencias menores
            Warning,      // Puede funcionar pero con limitaciones
            Incompatible  // No funcionará correctamente
        }

        public class CompatibilityResult
        {
            public CompatibilityLevel Level { get; set; }
            public List<string> Warnings { get; set; } = new();
            public List<string> Errors { get; set; } = new();
            public bool IsCompatible => Level != CompatibilityLevel.Incompatible;
        }

        public static CompatibilityResult CheckAbilityCompatibility(int bossPrefabGuid, int sourcePrefabGuid, int abilitySlot)
        {
            var result = new CompatibilityResult { Level = CompatibilityLevel.Perfect };

            // Obtener información de la base de datos
            var bossInfo = VBloodDatabase.GetVBlood(bossPrefabGuid);
            var sourceInfo = VBloodDatabase.GetVBlood(sourcePrefabGuid);

            if (bossInfo == null || sourceInfo == null)
            {
                result.Level = CompatibilityLevel.Incompatible;
                result.Errors.Add($"VBlood information not found in database");
                return result;
            }

            // Verificar que el source tenga la habilidad
            if (!sourceInfo.Abilities.ContainsKey(abilitySlot))
            {
                result.Level = CompatibilityLevel.Incompatible;
                result.Errors.Add($"{sourceInfo.Name} doesn't have ability at slot {abilitySlot}");
                return result;
            }

            var sourceAbility = sourceInfo.Abilities[abilitySlot];

            // 1. Verificar requisitos de vuelo
            if (sourceAbility.RequiresFlight && !bossInfo.CanFly)
            {
                result.Level = CompatibilityLevel.Incompatible;
                result.Errors.Add($"Ability requires flight but {bossInfo.Name} cannot fly");
                return result;
            }

            // 2. Verificar tipo de criatura
            var bossCategory = GetPrimaryCategory(bossInfo);
            var sourceCategory = GetPrimaryCategory(sourceInfo);

            if (bossCategory != sourceCategory)
            {
                // Humanoide con Beast es problemático
                if ((bossCategory == "Humanoid" && sourceCategory == "Beast") ||
                    (bossCategory == "Beast" && sourceCategory == "Humanoid"))
                {
                    result.Level = CompatibilityLevel.Warning;
                    result.Warnings.Add($"Model type mismatch: {bossCategory} using {sourceCategory} ability - animations may glitch");
                }
                // Mechanical con organic puede tener problemas
                else if (bossCategory == "Mechanical" && sourceCategory != "Mechanical")
                {
                    if (result.Level == CompatibilityLevel.Perfect) result.Level = CompatibilityLevel.Good;
                    result.Warnings.Add($"Mechanical boss using organic ability - visual effects may look odd");
                }
            }

            // 3. Verificar transformaciones específicas
            if (HasTransformationAbility(sourceAbility))
            {
                if (sourceInfo.Features.Contains("Werewolf") && !bossInfo.Features.Contains("Werewolf"))
                {
                    result.Level = CompatibilityLevel.Incompatible;
                    result.Errors.Add($"Werewolf transformation requires werewolf model");
                    return result;
                }
                if (sourceInfo.Features.Contains("Vampire") && !bossInfo.Features.Contains("Vampire"))
                {
                    if (result.Level == CompatibilityLevel.Perfect) result.Level = CompatibilityLevel.Warning;
                    result.Warnings.Add($"Vampire transformation on non-vampire may have visual issues");
                }
            }

            // 4. Verificar requisitos de movimiento
            if (sourceAbility.CanMoveWhileCasting || sourceAbility.ExtraData.ContainsKey("IsDash"))
            {
                // Los GateBoss tienen movimiento limitado
                if (bossInfo.Features.Contains("GateBoss"))
                {
                    if (result.Level == CompatibilityLevel.Perfect) result.Level = CompatibilityLevel.Warning;
                    result.Warnings.Add($"GateBoss has limited movement - dash abilities may be restricted");
                }
            }

            // 5. Verificar spawns de proyectiles
            if (sourceAbility.SpawnedPrefabs.Count > 0)
            {
                foreach (var spawn in sourceAbility.SpawnedPrefabs)
                {
                    // Verificar si el spawn requiere características específicas
                    if (spawn.SpawnName.Contains("Web") && !sourceCategory.Contains("Spider"))
                    {
                        if (result.Level == CompatibilityLevel.Perfect) result.Level = CompatibilityLevel.Good;
                        result.Warnings.Add($"Web projectile on non-spider may look unusual");
                    }
                    else if (spawn.SpawnName.Contains("Holy") && bossInfo.Features.Any(f => f.Contains("Undead") || f.Contains("Demon")))
                    {
                        if (result.Level == CompatibilityLevel.Perfect) result.Level = CompatibilityLevel.Good;
                        result.Warnings.Add($"Holy projectile on {bossCategory} is thematically inconsistent");
                    }
                }
            }

            // 6. Verificar requisitos de animación
            if (sourceAbility.RequiresAnimation)
            {
                // Los combos largos pueden no funcionar bien en todos los modelos
                if (sourceAbility.IsCombo && sourceAbility.ComboLength > 5)
                {
                    if (bossCategory != sourceCategory)
                    {
                        if (result.Level == CompatibilityLevel.Perfect) result.Level = CompatibilityLevel.Good;
                        result.Warnings.Add($"Long combo ({sourceAbility.ComboLength} hits) may not animate properly on different model type");
                    }
                }
            }

            // 7. Verificar tiempos de casteo extremos
            if (sourceAbility.CastTime > 3.0f)
            {
                if (bossInfo.Features.Contains("GateBoss_Minor"))
                {
                    if (result.Level == CompatibilityLevel.Perfect) result.Level = CompatibilityLevel.Good;
                    result.Warnings.Add($"Long cast time ({sourceAbility.CastTime}s) on minor boss may make it too vulnerable");
                }
            }

            // 8. Información adicional útil
            if (result.IsCompatible)
            {
                // Agregar información sobre la habilidad
                Plugin.Logger.LogInfo($"Ability transfer: {sourceAbility.Category} ability from {sourceInfo.Name} to {bossInfo.Name}");
                if (sourceAbility.CastTime > 0)
                    Plugin.Logger.LogInfo($"  Cast time: {sourceAbility.CastTime}s (post: {sourceAbility.PostCastTime}s)");
                if (sourceAbility.IsCombo)
                    Plugin.Logger.LogInfo($"  Combo: {sourceAbility.ComboLength} hits");
                if (sourceAbility.SpawnedPrefabs.Count > 0)
                    Plugin.Logger.LogInfo($"  Spawns: {string.Join(", ", sourceAbility.SpawnedPrefabs.Select(s => s.SpawnName))}");
            }

            return result;
        }

        private static string GetPrimaryCategory(VBloodStaticInfo vblood)
        {
            // Priorizar categorías
            if (vblood.Features.Any(f => f.Contains("Beast"))) return "Beast";
            if (vblood.Features.Any(f => f.Contains("Undead"))) return "Undead";
            if (vblood.Features.Any(f => f.Contains("Demon"))) return "Demon";
            if (vblood.Features.Any(f => f.Contains("Mechanical"))) return "Mechanical";
            if (vblood.Features.Any(f => f.Contains("Human") || f.Contains("Vampire"))) return "Humanoid";
            return "Unknown";
        }

        private static bool HasTransformationAbility(AbilityStaticInfo ability)
        {
            return ability.Category == AbilityCategory.Transformation ||
                   ability.Category == AbilityCategory.Ultimate ||
                   (ability.ExtraData.ContainsKey("BehaviorType") && 
                    ability.ExtraData["BehaviorType"].ToString().Contains("Transform"));
        }

        // Método para obtener sugerencias de habilidades compatibles
        public static List<(int sourceGuid, int slot, string abilityName, CompatibilityLevel level)> 
            GetCompatibleAbilities(int bossPrefabGuid, AbilityCategory? filterCategory = null)
        {
            var results = new List<(int, int, string, CompatibilityLevel)>();
            var bossInfo = VBloodDatabase.GetVBlood(bossPrefabGuid);
            
            if (bossInfo == null) return results;

            // Revisar todos los VBloods en la base de datos
            for (int i = 0; i < VBloodDatabase.Count; i++)
            {
                // Necesitaríamos un método para iterar, por ahora asumimos que tenemos los GUIDs
                // Este es un ejemplo conceptual, necesitarías implementar la iteración real
            }

            return results.OrderBy(r => r.Item4).ThenBy(r => r.Item3).ToList();
        }
    }
}