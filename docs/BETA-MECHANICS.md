# 🚧 BETA Mechanics System

**⚠️ WARNING: This feature is in BETA for v2.2.0 and not yet fully implemented. Use at your own risk!**

The Boss Mechanics System allows you to add dynamic behaviors to bosses that trigger under specific conditions, creating more complex and challenging encounters.

## 📋 Overview

The mechanics system is designed to make boss fights more engaging by adding triggered abilities, phase transitions, and environmental effects. We have implemented 25 different mechanics with more planned for the future.

## 🎮 Available Mechanics (BETA)

### Combat Mechanics

#### 1. **Enrage**
Boss increases damage and speed when reaching health thresholds.

**Parameters:**
- `damage_multiplier` - Damage increase (e.g., 1.5 = +50%)
- `movement_speed` - Movement speed multiplier
- `attack_speed` - Attack speed multiplier
- `duration` - Duration in seconds (0 = permanent)

**Command:**
```bash
.bb mechanic add <BossName> enrage --hp 25 --damage 1.5 --movement 1.3 --attack-speed 1.4
```

#### 2. **Shield**
Boss gains temporary damage immunity or absorption.

**Parameters:**
- `shield_type` - "immune" | "absorb" | "reflect"
- `shield_amount` - HP to absorb (for absorb type)
- `duration` - Shield duration

**Command:**
```bash
.bb mechanic add <BossName> shield --hp 50 --type immune --duration 10
```

#### 3. **Absorb**
Boss absorbs incoming damage and converts it to health or energy.

**Parameters:**
- `absorb_percent` - Percentage of damage absorbed
- `convert_to_health` - Convert absorbed damage to healing
- `max_absorb` - Maximum damage that can be absorbed
- `duration` - Absorb duration

**Command:**
```bash
.bb mechanic add <BossName> absorb --hp 30 --percent 50 --convert true --duration 15
```

#### 4. **Reflect**
Boss reflects damage back to attackers.

**Parameters:**
- `reflect_percent` - Percentage of damage reflected
- `reflect_type` - "physical" | "spell" | "all"
- `duration` - Reflect duration

**Command:**
```bash
.bb mechanic add <BossName> reflect --hp 40 --percent 30 --type all --duration 10
```

### Summon & Minion Mechanics

#### 5. **Summon**
Boss summons additional enemies to assist.

**Parameters:**
- `prefab_guid` - NPC to summon
- `count` - Number of adds
- `pattern` - "circle" | "line" | "random"
- `despawn_on_death` - Remove adds when boss dies

**Command:**
```bash
.bb mechanic add <BossName> summon --hp 75 --prefab -1905691330 --count 3 --pattern circle
```

#### 6. **Clone**
Boss creates copies of itself with reduced stats.

**Parameters:**
- `clone_count` - Number of clones
- `clone_health_percent` - Clone health percentage
- `clone_damage_percent` - Clone damage percentage
- `shuffle_positions` - Randomly swap positions

**Command:**
```bash
.bb mechanic add <BossName> clone --hp 50 --count 2 --health 25 --damage 50
```

### Healing & Recovery

#### 7. **Heal**
Boss heals itself or allies.

**Parameters:**
- `heal_amount` - Fixed amount or percentage
- `heal_type` - "instant" | "channel" | "overtime"
- `target` - "self" | "allies" | "all"
- `duration` - For channel/overtime healing

**Command:**
```bash
.bb mechanic add <BossName> heal --hp 25 --amount 20% --type channel --duration 5
```

### Crowd Control Mechanics

#### 8. **Stun**
Stuns all players in range.

**Parameters:**
- `radius` - Effect radius
- `duration` - Stun duration
- `delay` - Delay before stun applies

**Command:**
```bash
.bb mechanic add <BossName> stun --hp 60 --radius 15 --duration 2
```

#### 9. **Fear**
Causes players to flee in terror.

**Parameters:**
- `radius` - Effect radius
- `duration` - Fear duration
- `speed_multiplier` - Flee movement speed

**Command:**
```bash
.bb mechanic add <BossName> fear --hp 40 --radius 20 --duration 3
```

#### 10. **Root**
Roots players in place.

**Parameters:**
- `radius` - Effect radius
- `duration` - Root duration
- `damage_breaks` - If damage breaks the root

**Command:**
```bash
.bb mechanic add <BossName> root --hp 70 --radius 10 --duration 4
```

#### 11. **Silence**
Prevents players from casting abilities.

**Parameters:**
- `radius` - Effect radius
- `duration` - Silence duration

**Command:**
```bash
.bb mechanic add <BossName> silence --hp 55 --radius 15 --duration 3
```

#### 12. **Slow**
Reduces player movement and attack speed.

**Parameters:**
- `radius` - Effect radius
- `slow_percent` - Speed reduction percentage
- `duration` - Slow duration

**Command:**
```bash
.bb mechanic add <BossName> slow --hp 80 --radius 25 --percent 50 --duration 5
```

### Movement & Positioning

#### 13. **Teleport**
Boss teleports to different locations.

**Parameters:**
- `teleport_type` - "random" | "to_player" | "to_farthest" | "pattern"
- `range` - Teleport range
- `delay` - Delay before teleport

**Command:**
```bash
.bb mechanic add <BossName> teleport --hp 35 --type to_farthest --range 30
```

#### 14. **Pull**
Pulls players toward the boss.

**Parameters:**
- `radius` - Pull radius
- `force` - Pull strength
- `duration` - Pull duration

**Command:**
```bash
.bb mechanic add <BossName> pull --hp 65 --radius 30 --force 10
```

#### 15. **Knockback**
Knocks players away from the boss.

**Parameters:**
- `radius` - Knockback radius
- `force` - Knockback strength
- `damage` - Damage on impact

**Command:**
```bash
.bb mechanic add <BossName> knockback --hp 45 --radius 15 --force 20
```

### Area Control

#### 16. **AoE (Area of Effect)**
Creates damaging areas on the battlefield.

**Parameters:**
- `aoe_type` - "fire" | "poison" | "frost" | "blood" | "holy"
- `radius` - Area radius
- `damage_per_second` - DPS in area
- `duration` - Area duration
- `count` - Number of areas

**Command:**
```bash
.bb mechanic add <BossName> aoe --hp 70 --type fire --radius 5 --dps 50 --duration 20
```

#### 17. **Trap**
Places traps that trigger when players approach.

**Parameters:**
- `trap_type` - "explosive" | "root" | "slow" | "damage"
- `count` - Number of traps
- `trigger_radius` - Activation radius
- `damage` - Trap damage

**Command:**
```bash
.bb mechanic add <BossName> trap --hp 55 --type explosive --count 5 --damage 500
```

### Debuff Mechanics

#### 18. **Curse**
Applies stacking curses to players.

**Parameters:**
- `curse_type` - "weakness" | "vulnerability" | "drain"
- `stack_max` - Maximum curse stacks
- `effect_per_stack` - Effect strength per stack
- `duration` - Curse duration

**Command:**
```bash
.bb mechanic add <BossName> curse --hp 40 --type weakness --stacks 5 --effect 10
```

#### 19. **DoT (Damage over Time)**
Applies damage over time effects.

**Parameters:**
- `dot_type` - "bleed" | "burn" | "poison" | "corruption"
- `damage_per_tick` - Damage per tick
- `tick_rate` - Seconds between ticks
- `duration` - Total duration

**Command:**
```bash
.bb mechanic add <BossName> dot --hp 60 --type bleed --damage 25 --rate 1 --duration 10
```

#### 20. **Weaken**
Reduces player stats.

**Parameters:**
- `stat_type` - "damage" | "defense" | "speed" | "all"
- `reduction_percent` - Stat reduction percentage
- `duration` - Debuff duration

**Command:**
```bash
.bb mechanic add <BossName> weaken --hp 50 --stat damage --percent 30 --duration 8
```

### Special Mechanics

#### 21. **Mind Control**
Takes control of a player temporarily.

**Parameters:**
- `duration` - Control duration
- `target_count` - Number of players affected
- `force_attack_allies` - Make controlled players attack allies

**Command:**
```bash
.bb mechanic add <BossName> mindcontrol --hp 30 --duration 5 --count 1
```

#### 22. **Phase**
Transitions boss to a different combat phase.

**Parameters:**
- `phase_number` - Target phase (1-5)
- `reset_abilities` - Reset ability cooldowns
- `heal_percent` - Heal boss on phase change

**Command:**
```bash
.bb mechanic add <BossName> phase --hp 50 --number 2 --reset true
```

#### 23. **Vision**
Reduces player vision/increases darkness.

**Parameters:**
- `vision_reduction` - Vision reduction percentage
- `duration` - Effect duration
- `flicker` - Add flickering effect

**Command:**
```bash
.bb mechanic add <BossName> vision --hp 35 --reduction 80 --duration 10
```

#### 24. **Buff Steal**
Steals buffs from players and applies them to boss.

**Parameters:**
- `steal_count` - Number of buffs to steal
- `from_all_players` - Steal from all or nearest
- `duration_multiplier` - Multiplier for stolen buff duration

**Command:**
```bash
.bb mechanic add <BossName> buffsteal --hp 45 --count 3 --all true
```

#### 25. **Dispel**
Removes buffs from players or debuffs from boss.

**Parameters:**
- `target` - "players" | "self" | "allies"
- `dispel_count` - Number of effects to remove
- `dispel_type` - "buffs" | "debuffs" | "all"

**Command:**
```bash
.bb mechanic add <BossName> dispel --hp 65 --target players --count 5 --type buffs
```

## 🛠️ Command Reference

### Basic Commands

#### Add Mechanic
```bash
.bb mechanic add <BossName> <MechanicType> [options]
```

#### List Mechanics
```bash
.bb mechanic list <BossName>
```

#### Remove Mechanic
```bash
.bb mechanic remove <BossName> <MechanicID>
```

#### Test Mechanic
```bash
.bb mechanic test <BossName> <MechanicType>
```

### Advanced Commands

#### Debug Mechanics
```bash
.bb mechanic debug <BossName>
```

#### Reset Mechanics
```bash
.bb mechanic reset <BossName>
```

#### Copy Mechanics
```bash
.bb mechanic copy <SourceBoss> <TargetBoss>
```

## 🔧 Trigger Conditions

### Health-Based Triggers
```bash
--hp <percentage>        # Trigger at health percentage
--hp-below <percentage>  # Trigger below health
--hp-above <percentage>  # Trigger above health
```

### Time-Based Triggers
```bash
--time <seconds>         # Trigger after X seconds
--repeat <seconds>       # Repeat every X seconds
```

### Player-Based Triggers
```bash
--players <count>        # Trigger with X players nearby
--players-min <count>    # Minimum players required
```

### Combined Triggers
```bash
--hp 50 --players 3      # Trigger at 50% HP AND 3+ players
```

## 📝 Example Boss Configurations

### Basic Enrage Boss
```bash
.bb create "EnrageBoss" -327335305 100 5 1800
.bb mechanic add "EnrageBoss" enrage --hp 25 --damage 2.0 --movement 1.5
.bb mechanic add "EnrageBoss" aoe --hp 50 --type fire --radius 10 --dps 100
```

### Summoner Boss
```bash
.bb create "SummonerBoss" -99214611 90 4 2400
.bb mechanic add "SummonerBoss" summon --time 60 --repeat 60 --prefab -1905691330 --count 3
.bb mechanic add "SummonerBoss" shield --hp 50 --type absorb --amount 5000
.bb mechanic add "SummonerBoss" heal --hp 25 --amount 30% --type channel
```

### Control Boss
```bash
.bb create "ControlBoss" 1112948824 95 6 3000
.bb mechanic add "ControlBoss" stun --hp 80 --radius 20 --duration 2
.bb mechanic add "ControlBoss" fear --hp 60 --radius 25 --duration 3
.bb mechanic add "ControlBoss" mindcontrol --hp 40 --duration 5
.bb mechanic add "ControlBoss" pull --hp 20 --radius 30 --force 15
```

## ⚠️ Current Limitations

1. **Visual Effects** - Many mechanics lack proper visual feedback
2. **Performance** - Multiple complex mechanics may impact server performance
3. **Save State** - Mechanic states not persisted across server restarts
4. **Conflicts** - Some mechanics may conflict with each other
5. **Balance** - Mechanics not yet balanced for fair gameplay

## 🐛 Known Issues

1. **Summon** - Summoned adds may not always despawn properly
2. **Teleport** - Boss can sometimes teleport outside valid areas
3. **Mind Control** - May cause client disconnections
4. **Phase** - Phase transitions can reset some mechanics incorrectly
5. **DoT** - Damage ticks may continue after boss death

## 💡 Best Practices

### DO:
- ✅ Test each mechanic individually first
- ✅ Use reasonable values for parameters
- ✅ Monitor server performance
- ✅ Provide feedback on mechanic behavior
- ✅ Back up configurations before major changes

### DON'T:
- ❌ Stack too many mechanics on one boss
- ❌ Use extreme parameter values
- ❌ Combine conflicting mechanics
- ❌ Use untested mechanics on live servers
- ❌ Expect perfect functionality in BETA

## 📚 Related Documentation

- [Commands Reference](COMMANDS.md) - All BloodyBoss commands
- [Boss Mechanics Design](BOSS-MECHANICS.md) - Full design document
- [Examples](EXAMPLES.md) - More usage examples
- [Configuration](CONFIGURATION.md) - Mod settings

---

*⚠️ Remember: This is a BETA feature. Always backup your server before testing new mechanics!*