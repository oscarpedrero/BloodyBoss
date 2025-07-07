# 🎯 Sistema de Mecánicas de Boss - Diseño Detallado

## 📋 Visión General

El Sistema de Mecánicas permite añadir comportamientos dinámicos a los bosses que se activan bajo condiciones específicas, creando encuentros más complejos y desafiantes.

## 🎮 Mecánicas Disponibles

### 1. **Enrage (Enfurecer)**
- **Descripción**: El boss aumenta su daño, velocidad de movimiento y velocidad de ataque
- **Parámetros**:
  - `damage_multiplier`: Multiplicador de daño (ej: 1.5 = 50% más daño)
  - `movement_speed_multiplier`: Multiplicador de velocidad de movimiento (ej: 1.3 = 30% más rápido)
  - `attack_speed_multiplier`: Multiplicador de velocidad de ataque (ej: 1.5 = 50% más rápido)
  - `cast_speed_multiplier`: Multiplicador de velocidad de casteo (ej: 1.4 = 40% más rápido)
  - `cooldown_reduction`: Reducción de cooldowns en % (ej: 30 = 30% menos cooldown)
  - `duration`: Duración en segundos (0 = permanente)
  - `visual_effect`: Efecto visual (ej: "blood_rage", "fire_aura", "speed_lines")
- **Ejemplo**: Boss se enfurece al 25% HP: +50% daño, +30% velocidad movimiento, +40% velocidad ataque

### 2. **Shield Phase (Fase de Escudo)**
- **Descripción**: Boss se vuelve inmune/resistente al daño
- **Parámetros**:
  - `shield_type`: "immune" | "absorb" | "reflect"
  - `shield_amount`: Cantidad de daño a absorber (solo para "absorb")
  - `duration`: Duración en segundos
  - `can_move`: Si el boss puede moverse durante el escudo
- **Ejemplo**: Escudo de absorción de 10000 HP al 50% vida

### 3. **Summon Adds (Invocar Súbditos)**
- **Descripción**: Invoca NPCs aliados para ayudar en combate
- **Parámetros**:
  - `add_prefab`: PrefabGUID del NPC a invocar
  - `count`: Cantidad de adds
  - `pattern`: "circle" | "line" | "random"
  - `despawn_on_boss_death`: true/false
- **Ejemplo**: Invoca 5 esqueletos en círculo al 75% HP

### 4. **Heal (Curación)**
- **Descripción**: Boss se cura a sí mismo o a sus aliados
- **Parámetros**:
  - `heal_amount`: Cantidad fija o porcentaje ("5000" o "10%")
  - `heal_type`: "instant" | "channel" | "overtime"
  - `duration`: Duración del channeling o HoT
  - `interruptible`: Si puede ser interrumpido
- **Ejemplo**: Canaliza curación del 20% HP durante 5 segundos

### 5. **Teleport (Teletransporte)**
- **Descripción**: Boss se teletransporta a ubicaciones específicas
- **Parámetros**:
  - `teleport_type`: "random" | "to_player" | "to_center" | "fixed_positions"
  - `range`: Rango de teletransporte
  - `positions`: Array de posiciones fijas (si aplica)
  - `after_effect`: Efecto después del teleport (ej: "aoe_damage")
- **Ejemplo**: Teleport aleatorio en 50 unidades cada 30 segundos

### 6. **Phase Transition (Transición de Fase)**
- **Descripción**: Cambia completamente el comportamiento del boss
- **Parámetros**:
  - `new_ability_preset`: Preset de habilidades a aplicar
  - `transform_visual`: Cambio visual (si es posible)
  - `announcement`: Mensaje de anuncio
  - `reset_threat`: Resetea la tabla de amenaza
- **Ejemplo**: Al 50% HP cambia a modo "berserker"

### 7. **Area Denial (Negación de Área)**
- **Descripción**: Crea zonas peligrosas en el área de combate
- **Parámetros**:
  - `zone_type`: "fire" | "poison" | "frost" | "blood"
  - `damage_per_second`: Daño por segundo en la zona
  - `radius`: Radio de la zona
  - `duration`: Duración de la zona
  - `count`: Número de zonas a crear
- **Ejemplo**: Crea 3 charcos de fuego de 10m que duran 30s

### 8. **Buff Allies (Potenciar Aliados)**
- **Descripción**: Aplica buffs a todos los aliados cercanos
- **Parámetros**:
  - `buff_type`: "damage" | "defense" | "speed" | "heal"
  - `buff_power`: Poder del buff (porcentaje)
  - `radius`: Radio de efecto
  - `duration`: Duración del buff
- **Ejemplo**: +30% velocidad a todos los aliados en 50m

### 9. **Debuff Players (Debilitar Jugadores)**
- **Descripción**: Aplica debuffs a jugadores cercanos
- **Parámetros**:
  - `debuff_type`: "slow" | "weaken" | "blind" | "silence"
  - `debuff_power`: Poder del debuff
  - `radius`: Radio de efecto
  - `duration`: Duración del debuff
- **Ejemplo**: Reduce 50% velocidad de movimiento por 10s

### 10. **Rage Mode (Modo Furia)**
- **Descripción**: Combinación de múltiples efectos agresivos
- **Parámetros**:
  - `damage_multiplier`: Multiplicador de daño
  - `speed_multiplier`: Multiplicador de velocidad
  - `lifesteal`: Porcentaje de robo de vida
  - `cc_immunity`: Inmunidad a control de masas
- **Ejemplo**: +100% daño, +50% velocidad, 20% lifesteal

### 11. **Blood Drain (Drenaje de Sangre)**
- **Descripción**: El boss drena sangre de todos los jugadores cercanos
- **Parámetros**:
  - `drain_amount`: Sangre por segundo
  - `radius`: Radio de efecto
  - `convert_to_health`: Si convierte la sangre en vida para el boss
  - `blood_pool_on_death`: Si deja charcos de sangre al drenar
- **Ejemplo**: Drena 5 de sangre/seg en 30m y se cura 2% por jugador afectado

### 12. **Mirror Image (Imágenes Espejo)**
- **Descripción**: Crea copias ilusorias del boss que confunden a los jugadores
- **Parámetros**:
  - `copies_count`: Número de copias (1-5)
  - `copy_health_percent`: % de vida de las copias
  - `copy_damage_percent`: % de daño de las copias
  - `shuffle_positions`: Si intercambian posiciones aleatoriamente
- **Ejemplo**: 3 copias con 25% HP que hacen 50% daño

### 13. **Vampiric Evolution (Evolución Vampírica)**
- **Descripción**: El boss evoluciona y cambia de forma visual/mecánicamente
- **Parámetros**:
  - `evolution_stage`: Etapa de evolución (1-3)
  - `size_increase`: Aumento de tamaño (1.2 = 20% más grande)
  - `new_model`: PrefabGUID del nuevo modelo (si es posible)
  - `inherit_abilities`: Si mantiene habilidades anteriores
- **Ejemplo**: Evoluciona a forma draconiana al 40% HP

### 14. **Time Warp (Distorsión Temporal)**
- **Descripción**: Ralentiza el tiempo para los jugadores mientras el boss mantiene velocidad normal
- **Parámetros**:
  - `player_slow_percent`: % de ralentización de jugadores
  - `duration`: Duración del efecto
  - `affects_projectiles`: Si afecta proyectiles en el aire
  - `visual_effect`: Efecto de distorsión temporal
- **Ejemplo**: Jugadores al 50% velocidad por 8 segundos

### 15. **Possession (Posesión)**
- **Descripción**: El boss posee temporalmente a un jugador aleatorio
- **Parámetros**:
  - `duration`: Duración de la posesión
  - `force_attack_allies`: Si fuerza ataques a aliados
  - `damage_multiplier`: Multiplicador de daño durante posesión
  - `break_on_damage`: Si X daño rompe la posesión
- **Ejemplo**: Posee por 5 seg, jugador ataca aliados con +50% daño

### 16. **Environmental Hazards (Peligros Ambientales)**
- **Descripción**: Modifica el entorno de batalla
- **Parámetros**:
  - `hazard_type`: "lightning_strikes" | "blood_rain" | "shadow_pools" | "ice_patches"
  - `frequency`: Cada cuántos segundos ocurre
  - `damage`: Daño por impacto
  - `special_effect`: Efectos adicionales (stun, slow, burn)
- **Ejemplo**: Rayos caen cada 3 seg causando 500 daño + stun

### 17. **Soul Link (Vínculo de Almas)**
- **Descripción**: Vincula el boss con jugadores o adds compartiendo daño/efectos
- **Parámetros**:
  - `link_targets`: "players" | "adds" | "random"
  - `shared_damage_percent`: % de daño compartido
  - `share_healing`: Si comparte curación
  - `break_distance`: Distancia para romper vínculo
- **Ejemplo**: Comparte 30% del daño con 2 jugadores aleatorios

### 18. **Curse Stacking (Acumulación de Maldiciones)**
- **Descripción**: Aplica maldiciones que se acumulan con el tiempo
- **Parámetros**:
  - `curse_type`: Tipo de maldición
  - `stack_interval`: Cada cuánto se añade un stack
  - `max_stacks`: Máximo de stacks
  - `effect_per_stack`: Efecto por stack (ej: -5% velocidad)
- **Ejemplo**: -10% velocidad por stack, max 10 stacks

### 19. **Berserk Adds (Súbditos Furiosos)**
- **Descripción**: Los adds del boss se vuelven más fuertes cuando él tiene poca vida
- **Parámetros**:
  - `trigger_hp`: HP del boss para activar
  - `add_enrage_multiplier`: Multiplicador para adds
  - `add_size_increase`: Aumento de tamaño de adds
  - `chain_explosion_on_death`: Si explotan en cadena al morir
- **Ejemplo**: Adds +100% daño y explotan cuando boss < 25% HP

### 20. **Phase Skip Prevention (Anti-Skip de Fases)**
- **Descripción**: Evita que los jugadores se salten fases con burst damage
- **Parámetros**:
  - `damage_cap_per_second`: Límite de DPS
  - `overflow_to_shield`: Si el exceso se convierte en escudo
  - `punish_overflow`: Castiga el exceso de daño
  - `reset_threat_on_cap`: Resetea amenaza al alcanzar cap
- **Ejemplo**: Max 10k DPS, exceso se refleja a atacantes

### 21. **Adaptive Resistance (Resistencia Adaptativa)**
- **Descripción**: El boss desarrolla resistencia al tipo de daño más recibido
- **Parámetros**:
  - `adaptation_rate`: Velocidad de adaptación
  - `max_resistance`: Resistencia máxima (%)
  - `decay_rate`: Velocidad de pérdida si no recibe ese daño
  - `types_tracked`: "physical" | "spell" | "holy" | "unholy" | "all"
- **Ejemplo**: +5% resistencia/seg al tipo más dañino, max 75%

### 22. **Revenge Counter (Contador de Venganza)**
- **Descripción**: Tras recibir X golpes, contraataca devastadoramente
- **Parámetros**:
  - `hits_to_trigger`: Golpes necesarios
  - `counter_type`: "aoe_blast" | "targeted_strike" | "reflect_damage"
  - `counter_damage`: Daño del contraataque
  - `reset_on_trigger`: Si resetea el contador
- **Ejemplo**: Cada 10 golpes, AoE de 2000 daño

### 23. **Minion Sacrifice (Sacrificio de Súbditos)**
- **Descripción**: El boss sacrifica sus propios adds para efectos poderosos
- **Parámetros**:
  - `sacrifice_effect`: "heal" | "damage_boost" | "explosion" | "shield"
  - `effect_per_minion`: Efecto por cada add sacrificado
  - `voluntary`: Si el boss lo hace automáticamente
  - `player_can_prevent`: Si matar adds antes previene el sacrificio
- **Ejemplo**: Sacrifica adds para curarse 10% por cada uno

### 24. **Reality Tear (Desgarro de Realidad)**
- **Descripción**: Crea portales que teletransportan jugadores/proyectiles
- **Parámetros**:
  - `portal_pairs`: Número de pares de portales
  - `teleport_players`: Si afecta jugadores
  - `teleport_projectiles`: Si afecta proyectiles
  - `redirect_to_players`: Si redirige proyectiles a jugadores
- **Ejemplo**: 3 portales que redirigen hechizos hacia jugadores aleatorios

### 25. **Last Stand (Última Resistencia)**
- **Descripción**: Modo desesperado cuando está a punto de morir
- **Parámetros**:
  - `trigger_hp`: HP para activar (ej: 5%)
  - `immunity_duration`: Segundos de inmunidad inicial
  - `final_enrage_multiplier`: Multiplicador extremo
  - `death_explosion`: Si explota al morir de verdad
- **Ejemplo**: A 5% HP: inmune 3 seg, luego +300% todo

## 🛠️ Condiciones de Activación

### 1. **HP Threshold (Umbral de Vida)**
```json
{
  "trigger_type": "hp_threshold",
  "value": 25,
  "comparison": "less_than"
}
```

### 2. **Time Based (Basado en Tiempo)**
```json
{
  "trigger_type": "time",
  "value": 120,
  "repeat": true,
  "repeat_interval": 60
}
```

### 3. **Player Count (Cantidad de Jugadores)**
```json
{
  "trigger_type": "player_count",
  "value": 5,
  "comparison": "greater_than",
  "radius": 100
}
```

### 4. **On Death (Al Morir Adds)**
```json
{
  "trigger_type": "add_death",
  "add_name": "summoned_skeleton",
  "count": 3
}
```

### 5. **Damage Taken (Daño Recibido)**
```json
{
  "trigger_type": "damage_taken",
  "value": 50000,
  "time_window": 10
}
```

## 💬 Comandos de Chat

### Comandos Básicos
```bash
# Añadir enrage simple (solo daño)
.bb mechanic-add "BossName" "enrage" --hp 25 --damage 1.5

# Añadir enrage completo con todas las velocidades
.bb mechanic-add "BossName" "enrage" --hp 25 --damage 1.5 --movement 1.3 --attack-speed 1.4 --cast-speed 1.5 --cooldown-reduction 30

# Añadir mecánica con duración
.bb mechanic-add "BossName" "shield" --hp 50 --duration 10 --type immune

# Añadir invocación compleja
.bb mechanic-add "BossName" "summon" --hp 75 --prefab -1905691330 --count 3 --pattern circle

# Mecánica basada en tiempo
.bb mechanic-add "BossName" "teleport" --time 30 --repeat 30 --type random --range 50

# Mecánica con múltiples condiciones
.bb mechanic-add "BossName" "rage" --hp 25 --players 3+ --duration 0
```

### Comandos de Gestión
```bash
# Listar todas las mecánicas
.bb mechanic-list "BossName"

# Remover mecánica específica
.bb mechanic-remove "BossName" "enrage"

# Deshabilitar temporalmente
.bb mechanic-toggle "BossName" "shield"

# Testear mecánica manualmente
.bb mechanic-test "BossName" "summon"

# Resetear todas las mecánicas
.bb mechanic-clear "BossName"
```

### Comandos Avanzados
```bash
# Crear preset de mecánicas
.bb mechanic-preset-create "hardcore" 
.bb mechanic-preset-add "hardcore" "enrage" --hp 50 --multiplier 1.3
.bb mechanic-preset-add "hardcore" "shield" --hp 25 --duration 5
.bb mechanic-preset-apply "BossName" "hardcore"

# Copiar mecánicas entre bosses
.bb mechanic-copy "SourceBoss" "TargetBoss"

# Exportar/Importar configuración
.bb mechanic-export "BossName" 
.bb mechanic-import "BossName" "mechanics_config.json"
```

## 📁 Estructura JSON

### En el archivo Bosses.json
```json
{
  "name": "EpicBoss",
  "PrefabGUID": -1905691330,
  "level": 100,
  "Mechanics": [
    {
      "id": "enrage_phase",
      "type": "enrage",
      "enabled": true,
      "trigger": {
        "type": "hp_threshold",
        "value": 25,
        "comparison": "less_than"
      },
      "parameters": {
        "damage_multiplier": 1.5,
        "movement_speed_multiplier": 1.3,
        "attack_speed_multiplier": 1.4,
        "cast_speed_multiplier": 1.5,
        "cooldown_reduction": 30,
        "duration": 0,
        "visual_effect": "blood_rage",
        "announcement": "The boss enters a blood rage!"
      }
    },
    {
      "id": "shield_phase",
      "type": "shield",
      "enabled": true,
      "trigger": {
        "type": "hp_threshold",
        "value": 50,
        "comparison": "less_than",
        "one_time": true
      },
      "parameters": {
        "shield_type": "absorb",
        "shield_amount": 10000,
        "duration": 15,
        "can_move": false,
        "visual_effect": "holy_shield",
        "announcement": "A divine shield protects the boss!"
      }
    },
    {
      "id": "summon_adds",
      "type": "summon",
      "enabled": true,
      "trigger": {
        "type": "time",
        "value": 60,
        "repeat": true,
        "repeat_interval": 90
      },
      "parameters": {
        "add_prefab": -1905691330,
        "count": 3,
        "pattern": "circle",
        "despawn_on_boss_death": true,
        "announcement": "Minions answer the call!"
      }
    },
    {
      "id": "area_denial",
      "type": "area_denial",
      "enabled": true,
      "trigger": {
        "type": "hp_threshold",
        "value": 75,
        "comparison": "less_than"
      },
      "parameters": {
        "zone_type": "fire",
        "damage_per_second": 50,
        "radius": 5,
        "duration": 30,
        "count": 3,
        "announcement": "The ground erupts in flames!"
      }
    },
    {
      "id": "desperation",
      "type": "rage_mode",
      "enabled": true,
      "trigger": {
        "type": "compound",
        "conditions": [
          {
            "type": "hp_threshold",
            "value": 10,
            "comparison": "less_than"
          },
          {
            "type": "player_count",
            "value": 2,
            "comparison": "greater_than"
          }
        ],
        "operator": "AND"
      },
      "parameters": {
        "damage_multiplier": 2.0,
        "speed_multiplier": 1.5,
        "lifesteal": 25,
        "cc_immunity": true,
        "visual_effect": "dark_aura",
        "announcement": "WITNESS TRUE POWER!"
      }
    }
  ]
}
```

## 🔄 Flujo de Implementación

### 1. **Modelos de Datos**
```csharp
public class BossMechanicModel
{
    public string Id { get; set; }
    public string Type { get; set; }
    public bool Enabled { get; set; }
    public MechanicTrigger Trigger { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public bool HasTriggered { get; set; }
    public DateTime? LastTriggered { get; set; }
}

public class MechanicTrigger
{
    public string Type { get; set; }
    public float Value { get; set; }
    public string Comparison { get; set; }
    public bool OneTime { get; set; }
    public float RepeatInterval { get; set; }
}
```

### 2. **Sistema de Detección**
- Hook en `DealDamageHook` para detectar umbrales de HP
- Timer independiente para mecánicas basadas en tiempo
- Sistema de eventos para condiciones complejas

### 3. **Ejecución de Mecánicas**
- Factory pattern para crear ejecutores de mecánicas
- Sistema de cola para mecánicas simultáneas
- Validación de condiciones antes de ejecutar

## 🎯 Ejemplos de Uso

### Boss Básico con Enrage
```bash
.bb create "RageBoss" -1905691330 80 2 1800
.bb mechanic-add "RageBoss" "enrage" --hp 30 --multiplier 1.5
```

### Boss Complejo Multi-Fase
```bash
.bb create "PhaseBoss" -327335305 100 5 2400
.bb mechanic-add "PhaseBoss" "shield" --hp 75 --duration 10 --type immune
.bb mechanic-add "PhaseBoss" "summon" --hp 50 --prefab -1905691330 --count 5
.bb mechanic-add "PhaseBoss" "enrage" --hp 25 --multiplier 2.0
.bb mechanic-add "PhaseBoss" "area_denial" --time 60 --repeat 45 --type fire
```

### Boss de Raid
```bash
.bb create "RaidBoss" 939467639 120 10 3600
.bb mechanic-preset-apply "RaidBoss" "ultimate_challenge"
```

## 🔧 Configuración Global

En `BloodyBoss.cfg`:
```ini
[Boss Mechanics]
Enable = true
MaxMechanicsPerBoss = 10
EnableVisualEffects = true
EnableAnnouncements = true
AnnouncementPrefix = "⚔️ BOSS EVENT: "
DebugMode = false
```

---

*Sistema de Mecánicas de Boss - Diseñado para BloodyBoss v2.2.0*