# 📋 Plan de Implementación - Sistema de Mecánicas de Boss

## 🎯 Objetivo
Implementar un sistema modular de mecánicas para bosses en BloodyBoss v2.2.0 que permita crear encuentros dinámicos y desafiantes.

## 🔧 Arquitectura del Sistema

### 1. **Estructura de Carpetas**
```
BloodyBoss/
├── Systems/
│   ├── BossMechanicSystem.cs         # Sistema principal
│   └── Mechanics/                    # Implementaciones específicas
│       ├── EnrageMechanic.cs
│       ├── ShieldMechanic.cs
│       ├── SummonMechanic.cs
│       └── ...
├── DB/Models/
│   ├── BossMechanicModel.cs          # Modelo de datos
│   └── MechanicTriggerModel.cs      # Modelo de activadores
├── Command/
│   └── MechanicCommand.cs            # Comandos de chat
└── Configuration/
    └── MechanicConfig.cs             # Configuración

```

### 2. **Modelos de Datos**

#### BossMechanicModel.cs
```csharp
public class BossMechanicModel
{
    public string Id { get; set; }
    public string Type { get; set; }
    public bool Enabled { get; set; }
    public MechanicTriggerModel Trigger { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public bool HasTriggered { get; set; }
    public DateTime? LastTriggered { get; set; }
    public int TriggerCount { get; set; }
}
```

#### MechanicTriggerModel.cs
```csharp
public class MechanicTriggerModel
{
    public string Type { get; set; } // "hp_threshold", "time", "player_count", etc.
    public float Value { get; set; }
    public string Comparison { get; set; } // "less_than", "greater_than", "equals"
    public bool OneTime { get; set; }
    public float RepeatInterval { get; set; }
    public List<MechanicTriggerModel> CompoundConditions { get; set; } // Para AND/OR
    public string CompoundOperator { get; set; } // "AND", "OR"
}
```

### 3. **Interfaz Base para Mecánicas**
```csharp
public interface IMechanic
{
    string Type { get; }
    void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world);
    bool Validate(Dictionary<string, object> parameters);
    string GetDescription();
}
```

## 📝 Orden de Implementación

### Fase 1: Infraestructura Base (Semana 1)
1. ✅ Crear modelos de datos
2. ✅ Implementar BossMechanicSystem
3. ✅ Integrar con BossEncounterModel
4. ✅ Sistema de detección de triggers
5. ✅ Comandos básicos (add, remove, list)

### Fase 2: Mecánicas Core (Semana 2)
1. ⬜ **Enrage** - Multiplicadores de stats
2. ⬜ **Shield Phase** - Inmunidad/absorción
3. ⬜ **Summon Adds** - Invocación de NPCs
4. ⬜ **Heal** - Curación
5. ⬜ **Teleport** - Movimiento instantáneo

### Fase 3: Mecánicas Avanzadas (Semana 3)
6. ⬜ **Phase Transition** - Cambio de comportamiento
7. ⬜ **Area Denial** - Zonas peligrosas
8. ⬜ **Buff/Debuff** - Modificadores de grupo
9. ⬜ **Rage Mode** - Modo furia combinado
10. ⬜ **Blood Drain** - Mecánica vampírica

### Fase 4: Mecánicas Especiales (Semana 4)
11. ⬜ **Mirror Image** - Copias ilusorias
12. ⬜ **Time Warp** - Distorsión temporal
13. ⬜ **Environmental Hazards** - Peligros ambientales
14. ⬜ **Soul Link** - Vínculos de daño
15. ⬜ **Last Stand** - Última resistencia

### Fase 5: Mecánicas Complejas (Semana 5)
16. ⬜ **Vampiric Evolution** - Transformaciones
17. ⬜ **Possession** - Control de jugadores
18. ⬜ **Adaptive Resistance** - Resistencias dinámicas
19. ⬜ **Reality Tear** - Portales
20. ⬜ Resto de mecánicas...

## 🛠️ Implementación Detallada

### 1. Sistema de Detección de HP
```csharp
// En DealDamageHook.cs
private void CheckMechanicTriggers(Entity bossEntity, float currentHpPercent)
{
    var mechanics = GetBossMechanics(bossEntity);
    foreach (var mechanic in mechanics.Where(m => m.Enabled))
    {
        if (ShouldTrigger(mechanic, currentHpPercent))
        {
            TriggerMechanic(bossEntity, mechanic);
        }
    }
}
```

### 2. Factory de Mecánicas
```csharp
public class MechanicFactory
{
    private readonly Dictionary<string, Func<IMechanic>> _mechanics = new()
    {
        ["enrage"] = () => new EnrageMechanic(),
        ["shield"] = () => new ShieldMechanic(),
        ["summon"] = () => new SummonMechanic(),
        // etc...
    };
    
    public IMechanic Create(string type)
    {
        return _mechanics.TryGetValue(type, out var factory) 
            ? factory() 
            : throw new ArgumentException($"Unknown mechanic type: {type}");
    }
}
```

### 3. Integración con JSON
```json
{
  "name": "MechanicBoss",
  "Mechanics": [
    {
      "id": "enrage_25",
      "type": "enrage",
      "enabled": true,
      "trigger": {
        "type": "hp_threshold",
        "value": 25,
        "comparison": "less_than",
        "oneTime": true
      },
      "parameters": {
        "damage_multiplier": 1.5,
        "movement_speed_multiplier": 1.3,
        "attack_speed_multiplier": 1.4,
        "duration": 0,
        "announcement": "¡El boss entra en modo furia!"
      }
    }
  ]
}
```

## 🚀 Primeros Pasos de Implementación

### Paso 1: Crear BossMechanicModel
```csharp
// BloodyBoss/DB/Models/BossMechanicModel.cs
using System;
using System.Collections.Generic;

namespace BloodyBoss.DB.Models
{
    public class BossMechanicModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Type { get; set; }
        public bool Enabled { get; set; } = true;
        public MechanicTriggerModel Trigger { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
        public bool HasTriggered { get; set; }
        public DateTime? LastTriggered { get; set; }
        public int TriggerCount { get; set; }
        
        public void Reset()
        {
            HasTriggered = false;
            LastTriggered = null;
            TriggerCount = 0;
        }
    }
}
```

### Paso 2: Modificar BossEncounterModel
```csharp
// Añadir a BossEncounterModel.cs
public List<BossMechanicModel> Mechanics { get; set; } = new List<BossMechanicModel>();
```

### Paso 3: Crear EnrageMechanic
```csharp
// BloodyBoss/Systems/Mechanics/EnrageMechanic.cs
public class EnrageMechanic : IMechanic
{
    public string Type => "enrage";
    
    public void Execute(Entity bossEntity, Dictionary<string, object> parameters, World world)
    {
        var damageMultiplier = GetParameter<float>(parameters, "damage_multiplier", 1.5f);
        var movementMultiplier = GetParameter<float>(parameters, "movement_speed_multiplier", 1.0f);
        var attackSpeedMultiplier = GetParameter<float>(parameters, "attack_speed_multiplier", 1.0f);
        
        // Aplicar modificadores
        var unitStats = bossEntity.Read<UnitStats>();
        unitStats.PhysicalPower._Value *= damageMultiplier;
        unitStats.SpellPower._Value *= damageMultiplier;
        unitStats.MovementSpeed._Value *= movementMultiplier;
        unitStats.PrimaryAttackSpeed._Value *= attackSpeedMultiplier;
        bossEntity.Write(unitStats);
        
        // Anunciar
        var announcement = GetParameter<string>(parameters, "announcement", "¡El boss se enfurece!");
        ServerChatUtils.SendSystemMessageToAllClients(EntityManager, announcement);
    }
}
```

## 📊 Métricas de Éxito

1. **Funcionalidad**: Todas las mecánicas funcionan correctamente
2. **Rendimiento**: < 1ms de overhead por mecánica
3. **Configurabilidad**: 100% configurable via comandos/JSON
4. **Compatibilidad**: Compatible con sistemas existentes
5. **Documentación**: Completa y con ejemplos

## 🎮 Casos de Uso de Ejemplo

### Boss Simple con Enrage
```bash
.bb create "RageBoss" -1905691330 80 2 1800
.bb mechanic-add "RageBoss" "enrage" --hp 30 --damage 1.5 --movement 1.3
```

### Boss Complejo Multi-Fase
```bash
.bb create "ComplexBoss" -327335305 100 5 2400
.bb mechanic-add "ComplexBoss" "shield" --hp 75 --duration 10
.bb mechanic-add "ComplexBoss" "summon" --hp 50 --count 5 --prefab -1905691330
.bb mechanic-add "ComplexBoss" "enrage" --hp 25 --damage 2.0 --movement 1.5
.bb mechanic-add "ComplexBoss" "last_stand" --hp 5 --immunity 3
```

## 🔄 Próximos Pasos

1. **Crear branch**: `feature/boss-mechanics-system`
2. **Implementar modelos**: BossMechanicModel y MechanicTriggerModel
3. **Crear sistema base**: BossMechanicSystem
4. **Implementar primera mecánica**: EnrageMechanic
5. **Añadir comandos**: mechanic-add, mechanic-list
6. **Testing**: Probar con boss simple
7. **Iterar**: Añadir más mecánicas

---

*Documento creado: 2025-01-06*
*Versión objetivo: BloodyBoss v2.2.0*