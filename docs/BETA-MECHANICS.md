# 🚧 BETA Mechanics System

**⚠️ WARNING: This feature is in BETA for v2.2.0 and not yet fully implemented. Use at your own risk!**

The Boss Mechanics System allows you to add dynamic behaviors to bosses that trigger under specific conditions, creating more complex and challenging encounters.

## 📋 Overview

The mechanics system is designed to make boss fights more engaging by adding triggered abilities, phase transitions, and environmental effects. While the foundation is implemented, full integration is planned for v2.2.0.

## 🎮 Available Mechanics (BETA)

### Currently Implemented

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

#### 3. **Summon**
Boss summons additional enemies to assist.

**Parameters:**
- `prefab_guid` - NPC to summon
- `count` - Number of adds
- `pattern` - "circle" | "line" | "random"

**Command:**
```bash
.bb mechanic add <BossName> summon --hp 75 --prefab -1905691330 --count 3 --pattern circle
```

### Planned Mechanics (v2.2.0)

- **Heal** - Boss heals itself or allies
- **Teleport** - Boss teleports around arena
- **Phase Transition** - Complete behavior change
- **Area Denial** - Creates hazardous zones
- **Buff/Debuff** - Applies effects to players/allies
- **And 20+ more mechanics...**

## 🛠️ Command Reference

### Basic Commands

#### Add Mechanic
```bash
.bb mechanic add <BossName> <MechanicType> [options]
```

**Examples:**
```bash
# Add enrage at 25% HP
.bb mechanic add "TestBoss" enrage --hp 25 --damage 1.5

# Add shield phase at 50% HP
.bb mechanic add "TestBoss" shield --hp 50 --type absorb --amount 10000

# Add summons every 60 seconds
.bb mechanic add "TestBoss" summon --time 60 --prefab -1905691330 --count 5
```

#### List Mechanics
```bash
.bb mechanic list <BossName>
```

Shows all configured mechanics for a boss.

#### Remove Mechanic
```bash
.bb mechanic remove <BossName> <MechanicID>
```

#### Test Mechanic
```bash
.bb mechanic test <BossName> <MechanicType>
```

Manually triggers a mechanic for testing.

### Advanced Commands

#### Debug Mechanics
```bash
.bb mechanic debug <BossName>
```

Shows detailed mechanic information and trigger states.

#### Reset Mechanics
```bash
.bb mechanic reset <BossName>
```

Resets all mechanic triggers for a boss.

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

## 📝 Configuration

Mechanics are stored in the boss configuration:

```json
{
  "name": "TestBoss",
  "mechanics": [
    {
      "id": "enrage_low_health",
      "type": "enrage",
      "trigger": {
        "type": "health",
        "value": 25,
        "comparison": "less_than"
      },
      "parameters": {
        "damage_multiplier": 1.5,
        "movement_speed": 1.3,
        "duration": 0
      },
      "enabled": true
    }
  ]
}
```

## ⚠️ Current Limitations

1. **Limited Mechanics** - Only 3 mechanics fully implemented
2. **Basic Triggers** - Complex condition combinations not yet supported
3. **No Visual Effects** - Mechanics lack visual feedback
4. **Performance** - Not optimized for many simultaneous mechanics
5. **Save State** - Mechanic states not persisted across restarts

## 🔮 Future Features (v2.2.0)

### Planned Improvements

1. **25+ New Mechanics**
   - Environmental hazards
   - Mind control
   - Time manipulation
   - Reality tears
   - And many more...

2. **Advanced Triggers**
   - Compound conditions (AND/OR)
   - Player action triggers
   - Environmental triggers
   - Chain triggers

3. **Visual System**
   - Mechanic animations
   - Warning indicators
   - Phase transition effects

4. **Mechanic Designer**
   - In-game mechanic builder
   - Visual scripting
   - Preset library

## 🧪 Testing Mechanics

### Safe Testing Environment
```bash
# Create test boss
.bb create "MechanicTest" -327335305 100 5 3600

# Add test mechanics
.bb mechanic add "MechanicTest" enrage --hp 50 --damage 2.0
.bb mechanic add "MechanicTest" shield --hp 25 --type immune --duration 5

# Spawn for testing
.bb test "MechanicTest"

# Manually trigger mechanics
.bb mechanic test "MechanicTest" enrage
.bb mechanic test "MechanicTest" shield
```

### Debug Information
```bash
# View mechanic states
.bb mechanic debug "MechanicTest"

# Check trigger conditions
.bb status "MechanicTest"
```

## 🐛 Known Issues

1. **Enrage** - May not properly reset on boss death
2. **Shield** - Absorb shields don't track damage correctly
3. **Summon** - Summoned adds may not despawn properly
4. **Triggers** - Time-based triggers can desync after server restart

## 💡 Best Practices

### DO:
- ✅ Test mechanics thoroughly before using on live server
- ✅ Start with simple mechanics and single triggers
- ✅ Monitor server performance with multiple mechanics
- ✅ Report bugs to help improve the system

### DON'T:
- ❌ Stack too many mechanics on one boss
- ❌ Use conflicting mechanics (e.g., immune + heal)
- ❌ Rely on mechanics for critical gameplay
- ❌ Use in production without extensive testing

## 🔄 Migration Path

When v2.2.0 releases:
1. Existing mechanics will be automatically migrated
2. New visual effects will be applied
3. Additional configuration options will be available
4. Full documentation will be provided

## 📚 Related Documentation

- [Commands Reference](COMMANDS.md) - All BloodyBoss commands
- [Boss Mechanics Design](BOSS-MECHANICS.md) - Full design document
- [Examples](EXAMPLES.md) - Usage examples
- [Troubleshooting](TROUBLESHOOTING.md) - Common issues

## 🤝 Feedback

The mechanics system is in active development. Your feedback helps shape the final implementation:

- Report bugs: [GitHub Issues](https://github.com/oscarpedrero/BloodyBoss/issues)
- Suggest mechanics: [Discord Community](https://discord.gg/vrisingmods)
- Share configurations: [Community Forums](https://vrisingmods.com)

---

*⚠️ Remember: This is a BETA feature. Always backup your server before testing new mechanics!*