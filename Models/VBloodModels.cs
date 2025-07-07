using System.Collections.Generic;
using Stunlock.Core;

namespace BloodyBoss.Models
{
    public enum AbilityCategory
    {
        Unknown,
        BasicAttack,
        Special,
        Movement,
        Defensive,
        Ultimate,
        Transformation,
        Summon,
        Buff
    }

    public class VBloodAbilityInfo
    {
        public int SlotIndex { get; set; }
        public PrefabGUID AbilityPrefabGUID { get; set; }
        public string Name { get; set; }
        public AbilityCategory Category { get; set; }
        public bool RequiresAnimation { get; set; }
        public bool IsChanneled { get; set; }
        public bool RequiresFlight { get; set; }
        
        // 1. Tiempos de casteo
        public float CastTime { get; set; }
        public float PostCastTime { get; set; }
        public bool HideCastBar { get; set; }
        
        // 2. Información de combo
        public bool IsCombo { get; set; }
        public int ComboLength { get; set; }
        
        // 3. Información de cooldown y cargas
        public float Cooldown { get; set; }
        public int Charges { get; set; }
        
        // 4. Animación y movimiento durante casteo
        public string AnimationSequence { get; set; }
        public bool CanMoveWhileCasting { get; set; }
        public bool CanRotateWhileCasting { get; set; }
        
        // 5. Información de spawn (proyectiles, efectos, etc.)
        public List<SpawnInfo> SpawnedPrefabs { get; set; } = new List<SpawnInfo>();
        
        // 6. Condiciones y buffs
        public List<string> CastConditions { get; set; } = new List<string>();
        public List<BuffInfo> AppliedBuffs { get; set; } = new List<BuffInfo>();
        
        // Datos adicionales
        public Dictionary<string, object> ExtraData { get; set; } = new Dictionary<string, object>();
    }
    
    // Información de spawn de proyectiles/efectos
    public class SpawnInfo
    {
        public PrefabGUID SpawnPrefab { get; set; }
        public string SpawnName { get; set; } // Nombre del prefab spawneado
        public string Target { get; set; } // Owner, Target, etc.
        public float HoverDistance { get; set; }
        public float HoverMaxDistance { get; set; }
    }
    
    // Información de buffs aplicados
    public class BuffInfo
    {
        public PrefabGUID BuffPrefab { get; set; }
        public string BuffName { get; set; } // Nombre del buff
        public string BuffTarget { get; set; } // EventTarget, Owner, etc.
        public string SpellTarget { get; set; }
    }

    public class VBloodInfo
    {
        public PrefabGUID PrefabGUID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool CanFly { get; set; }
        public List<string> Features { get; set; } = new();
        public Dictionary<int, VBloodAbilityInfo> Abilities { get; set; } = new();
        
        // Sistema de escalado dinámico
        public DynamicScalingInfo DynamicScaling { get; set; }
        
        // Resistencias a buffs
        public PrefabGUID BuffResistanceSettings { get; set; }
        
        // Tabla de drop
        public PrefabGUID DropTableGuid { get; set; }
        
        // Eventos al morir
        public List<PrefabGUID> DeathEvents { get; set; } = new();
    }
    
    // Información de escalado dinámico
    public class DynamicScalingInfo
    {
        public float Multiplier { get; set; } = 1.0f;
        public float MultiplierReductionFactorPerCrowdedness { get; set; } = 1.0f;
        public int MaxPlayers { get; set; } = 1;
        public bool HasDynamicScaling { get; set; } = false;
    }
}