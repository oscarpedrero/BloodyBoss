# BloodyBoss - 25 Mechanics Overview

## Current Mechanics System

El sistema de mecánicas de BloodyBoss permite crear comportamientos complejos para los jefes mediante triggers basados en:
- **HP Threshold**: Se activan cuando el jefe alcanza cierto % de vida
- **Time**: Se activan después de X segundos de combate
- **Player Count**: Se activan según el número de jugadores cercanos
- **Add Death**: Se activan cuando mueren adds específicos
- **Damage Taken**: Se activan al recibir cierta cantidad de daño

## Lista de las 25 Mecánicas Actuales

### 1. **Stun Mechanic** ✅ MEJORADO
- **Archivo**: `StunMechanic.cs`
- **Descripción**: Sistema de stun con marcado psíquico previo
- **Nuevo Sistema**:
  - **Fase 1**: Aparece un ojo psíquico sobre el objetivo (2.5s)
  - **Fase 2**: El ojo parpadea rápidamente (0.5s advertencia)
  - **Fase 3**: Explosión psíquica y stun aplicado
- **Parámetros**:
  - `target`: "nearest", "farthest", "all", "random"
  - `duration`: Duración del stun (default: 3s)
  - `mark_duration`: Duración de la marca previa (default: 2.5s)
  - `radius`: Radio de efecto (default: 0)
  - `max_targets`: Número máximo de objetivos (default: 1)
  - `announcement`: Mensaje de anuncio (default: "👁️ A psychic eye marks its target!")
- **Efectos Visuales**:
  - Marca: `AB_Illusion_PsychicEye_Float` (-461356643)
  - Impacto: `AB_Illusion_PsychicBurst_Purple` (-1460585349)
  - Buff: `Buff_General_Stun` (355774169)
- **Características**:
  - El ojo psíquico sigue al jugador
  - Advertencia visual antes del stun
  - Efecto de pantalla temblorosa al aplicar
  - Sonido psíquico incluido en el burst

### 2. **AoE Mechanic**
- **Archivo**: `AoeMechanic.cs`
- **Descripción**: Crea zonas de daño en área
- **Parámetros**:
  - `aoe_type`: "fire", "frost", "holy", "blood", "explosion"
  - `radius`: Radio del AoE (default: 10f)
  - `damage`: Daño infligido (default: 50f)
  - `count`: Número de zonas (default: 1)
  - `pattern`: "center", "ring", "random", "cross"
  - `delay`: Retraso antes del daño (default: 1.5s)
- **Efectos visuales**:
  - Fire: `AB_Shared_FireArea_VegetationSpread` (-2134900616)
  - Frost: `AB_Frost_IceBlockVortex_Buff_AreaDamageTrigger` (-1766133599)
  - Holy: `AB_Militia_EyeOfGod_AreaInitBuff` (933561205)
  - Blood: `AB_FeedDraculaBloodSoul_03_Complete_AreaDamage` (250152500)
- **Mejoras posibles**: Usar habilidades de AoE específicas de VBloods

### 3. **Enrage Mechanic**
- **Archivo**: `EnrageMechanic.cs`
- **Descripción**: Aumenta el daño y velocidad del jefe
- **Parámetros**:
  - `damage_multiplier`: Multiplicador de daño (default: 1.5f)
  - `movement_speed_multiplier`: Multiplicador de velocidad (default: 1.0f)
  - `attack_speed_multiplier`: Multiplicador de velocidad de ataque (default: 1.0f)
  - `cast_speed_multiplier`: Multiplicador de velocidad de casteo (default: 1.0f)
  - `cooldown_reduction`: Reducción de cooldowns (default: 0f)
  - `duration`: Duración del enrage (0 = permanente)
  - `visual_effect`: "blood_rage", "fire_aura", "speed_lines"
- **Buffs visuales**:
  - Blood Rage: `AB_Blood_BloodRage_Buff_MagicSource` (2085766220)
  - Fire Aura: (-1576893213)
  - Speed Lines: (788443104)
- **Mejoras posibles**: Aplicar transformaciones visuales de VBloods enfurecidos

### 4. **Shield Mechanic**
- **Descripción**: Crea un escudo que absorbe daño
- **Parámetros**: 
  - `shield_amount`: Cantidad de absorción
  - `duration`: Duración del escudo
  - `visual_effect`: Efecto visual del escudo
- **Mejoras posibles**: Usar mecánicas de escudo de Gorecrusher o Solarus

### 5. **Summon Mechanic**
- **Descripción**: Invoca adds/minions para ayudar al jefe
- **Parámetros**:
  - `summon_type`: Tipo de criatura a invocar
  - `count`: Número de invocaciones
  - `pattern`: Patrón de spawn
- **Mejoras posibles**: Invocar VBloods menores o usar summons específicos

### 6. **Heal Mechanic**
- **Descripción**: Cura al jefe o a sus aliados
- **Parámetros**:
  - `heal_amount`: Cantidad de curación
  - `target`: "self", "allies", "lowest_health"
- **Mejoras posibles**: Usar animaciones de curación de VBloods sanadores

### 7. **Phase Mechanic**
- **Descripción**: Cambia de fase con nuevas habilidades
- **Parámetros**:
  - `phase_number`: Número de fase
  - `new_abilities`: Lista de nuevas habilidades
- **Mejoras posibles**: Intercambiar habilidades completas de diferentes VBloods

### 8. **Fear Mechanic**
- **Descripción**: Causa miedo haciendo huir a los jugadores
- **Parámetros**:
  - `duration`: Duración del miedo
  - `radius`: Radio de efecto
- **Mejoras posibles**: Usar fear de Styx the Sunderer

### 9. **Slow Mechanic**
- **Descripción**: Ralentiza el movimiento de los jugadores
- **Parámetros**:
  - `slow_percentage`: Porcentaje de ralentización
  - `duration`: Duración
- **Mejoras posibles**: Usar slows de frost VBloods

### 10. **Clone Mechanic**
- **Descripción**: Crea clones del jefe
- **Parámetros**:
  - `clone_count`: Número de clones
  - `clone_health_percentage`: % de vida de los clones
- **Mejoras posibles**: Usar mecánica de ilusiones de Azariel

### 11. **Root Mechanic**
- **Descripción**: Inmoviliza a los jugadores
- **Parámetros**:
  - `duration`: Duración del root
  - `break_on_damage`: Si se rompe al recibir daño
- **Mejoras posibles**: Usar roots de nature VBloods

### 12. **Silence Mechanic**
- **Descripción**: Silencia impidiendo el uso de habilidades
- **Parámetros**:
  - `duration`: Duración del silencio
  - `radius`: Radio de efecto
- **Mejoras posibles**: Usar silence de Azariel

### 13. **Knockback Mechanic**
- **Descripción**: Empuja a los jugadores
- **Parámetros**:
  - `force`: Fuerza del empuje
  - `direction`: Dirección del knockback
- **Mejoras posibles**: Usar knockbacks de Gorecrusher

### 14. **DoT Mechanic**
- **Descripción**: Aplica daño en el tiempo
- **Parámetros**:
  - `damage_per_tick`: Daño por tick
  - `tick_interval`: Intervalo entre ticks
  - `duration`: Duración total
- **Mejoras posibles**: Usar DoTs específicos (veneno, fuego, etc.)

### 15. **Vision Mechanic**
- **Descripción**: Reduce la visión de los jugadores
- **Parámetros**:
  - `vision_radius`: Radio de visión reducida
  - `duration`: Duración
- **Mejoras posibles**: Usar efectos de oscuridad de shadow VBloods

### 16. **Mind Control Mechanic**
- **Descripción**: Controla temporalmente a un jugador
- **Parámetros**:
  - `duration`: Duración del control
  - `target_allies`: Si ataca a aliados
- **Mejoras posibles**: Usar charm de Azariel

### 17. **Reflect Mechanic**
- **Descripción**: Refleja daño a los atacantes
- **Parámetros**:
  - `reflect_percentage`: % de daño reflejado
  - `duration`: Duración
- **Mejoras posibles**: Usar mecánicas de reflejo de Solarus

### 18. **Weaken Mechanic**
- **Descripción**: Debilita reduciendo stats
- **Parámetros**:
  - `stat_reduction`: % de reducción de stats
  - `affected_stats`: Stats afectados
- **Mejoras posibles**: Usar debuffs de curse VBloods

### 19. **Curse Mechanic**
- **Descripción**: Aplica maldiciones con efectos variados
- **Parámetros**:
  - `curse_type`: Tipo de maldición
  - `duration`: Duración
- **Mejoras posibles**: Usar curses de Ungora

### 20. **Absorb Mechanic**
- **Descripción**: Absorbe buffs o vida de los jugadores
- **Parámetros**:
  - `absorb_type`: "health", "buffs", "blood"
  - `amount`: Cantidad absorbida
- **Mejoras posibles**: Usar mecánicas de blood drain

### 21. **Trap Mechanic**
- **Descripción**: Coloca trampas en el campo
- **Parámetros**:
  - `trap_type`: Tipo de trampa
  - `trigger_radius`: Radio de activación
- **Mejoras posibles**: Usar trampas de Jade the Vampire Hunter

### 22. **Pull Mechanic**
- **Descripción**: Atrae a los jugadores hacia el jefe
- **Parámetros**:
  - `pull_force`: Fuerza de atracción
  - `radius`: Radio de efecto
- **Mejoras posibles**: Usar pulls de Behemoth

### 23. **Teleport Mechanic**
- **Descripción**: Teletransporta al jefe o jugadores
- **Parámetros**:
  - `teleport_type`: "boss", "players", "swap"
  - `destination`: Destino del teleport
- **Mejoras posibles**: Usar blinks de Styx

### 24. **Buff Steal Mechanic**
- **Descripción**: Roba buffs de los jugadores
- **Parámetros**:
  - `max_buffs_stolen`: Número máximo de buffs
  - `buff_filter`: Filtro de tipos de buff
- **Mejoras posibles**: Usar mecánicas de dispel de Azariel

### 25. **Dispel Mechanic**
- **Descripción**: Remueve buffs o debuffs
- **Parámetros**:
  - `dispel_type`: "buffs", "debuffs", "all"
  - `target`: Objetivo del dispel
- **Mejoras posibles**: Usar cleanse abilities de holy VBloods

## Configuración de Mecánicas

Cada mecánica se configura mediante un `BossMechanicModel`:

```json
{
  "Type": "stun",
  "Enabled": true,
  "Trigger": {
    "Type": "hp_threshold",
    "Value": 50,
    "Comparison": "less_than",
    "OneTime": true
  },
  "Parameters": {
    "target": "nearest",
    "duration": 3,
    "announcement": "Boss stuns the nearest player!"
  }
}
```

## Sistema de Triggers

### HP Threshold Trigger
```json
{
  "Type": "hp_threshold",
  "Value": 75,  // Se activa al 75% HP
  "Comparison": "less_than",
  "OneTime": true
}
```

### Time Trigger
```json
{
  "Type": "time",
  "Value": 30,  // Se activa a los 30 segundos
  "OneTime": false,
  "RepeatInterval": 20  // Se repite cada 20 segundos
}
```

### Player Count Trigger
```json
{
  "Type": "player_count",
  "Value": 3,
  "Comparison": "greater_than",  // Cuando hay más de 3 jugadores
  "OneTime": false
}
```

## Próximos Pasos

1. **Mapear habilidades de VBlood**: Identificar qué habilidades específicas de cada VBlood podemos usar
2. **Mejorar efectos visuales**: Usar las animaciones y efectos propios de cada VBlood
3. **Combinar mecánicas**: Crear mecánicas compuestas más complejas
4. **Optimizar rendimiento**: Mejorar el sistema de triggers y ejecución
