# Mechanic System Commands

Add special combat mechanics to your bosses to create challenging and dynamic encounters. Mechanics trigger based on health thresholds, time, or other conditions.

## Table of Contents
- [Understanding Mechanics](#understanding-mechanics)
- [View Mechanic Help](#view-mechanic-help)
- [Add Mechanics](#add-mechanics)
- [Manage Mechanics](#manage-mechanics)
- [Available Mechanics](#available-mechanics)
- [Best Practices](#best-practices)

---

## Understanding Mechanics

### What Are Mechanics?
Mechanics are special behaviors that trigger during combat:
- **Health-based**: Activate at specific HP percentages
- **Time-based**: Trigger after certain duration
- **Conditional**: Require specific conditions (player count, etc.)

### Important Note About Parameters
**Commands with `--` parameters MUST be enclosed in quotes:**
```bash
# CORRECT ✅
.bb mechanic-add "Fire Dragon" aoe "--hp 75 --aoe_type fire --radius 10"

# WRONG ❌
.bb mechanic-add "Fire Dragon" aoe --hp 75 --aoe_type fire --radius 10
```

---

## View Mechanic Help

Get examples and syntax for all available mechanics.

### Syntax
```
.bb mechanic-help
```

### Output Example
```
📋 Mechanic Command Examples:
─────────────────────────────
Add stun at 90% HP:
  .bb mechanic-add "Boss" stun "--hp 90 --target random --duration 3"

Add AoE damage zones at 75% HP:
  .bb mechanic-add "Boss" aoe "--hp 75 --aoe_type fire --radius 10 --damage 500"

Add slow effect at 50% HP:
  .bb mechanic-add "Boss" slow "--hp 50 --radius 15 --min_players 3"

Add life/shield absorption at 25% HP:
  .bb mechanic-add "Boss" absorb "--hp 25 --type health --amount 100"
─────────────────────────────
Use .bb mechanic-list "BossName" to see configured mechanics
```

---

## Add Mechanics

Adds a new mechanic to a boss.

### Syntax
```
.bb mechanic-add <BossName> <MechanicType> "<options>"
```

### Important: Quote Usage
Always enclose options in quotes when using `--` parameters:
```bash
.bb mechanic-add "MyBoss" stun "--hp 90 --target random --duration 3"
```

---

## Manage Mechanics

### List Mechanics
```
.bb mechanic-list <BossName>
```
Shows all configured mechanics for a boss.

### Remove Mechanic
```
.bb mechanic-remove <BossName> <MechanicID>
```
Remove a specific mechanic by ID.

### Toggle Mechanic
```
.bb mechanic-toggle <BossName> <MechanicID>
```
Enable or disable a mechanic without removing it.

### Test Mechanic
```
.bb mechanic-test <BossName> <MechanicID>
```
Manually trigger a mechanic for testing.

### Clear All Mechanics
```
.bb mechanic-clear <BossName>
```
Remove all mechanics from a boss.

---

## Available Mechanics

BloodyBoss currently features four fully functional and tested mechanics. Additional mechanics may be added in future updates.

### Stun Mechanic
[→ Full Documentation](../mechanics/STUN.md)
Stuns target players with a warning indicator.

#### Syntax
```
.bb mechanic-add <BossName> stun "<options>"
```

#### Options
- `--hp <percent>` - Trigger at health percentage
- `--target <type>` - Target selection: random, closest, furthest, all
- `--duration <seconds>` - Stun duration
- `--mark_duration <seconds>` - Warning indicator duration
- `--announcement <text>` - Custom announcement message

#### Examples
```bash
# Stun random player at 90% HP
.bb mechanic-add "Dragon" stun "--hp 90 --target random --duration 3 --mark_duration 2"

# Stun all players at 50% HP
.bb mechanic-add "Dragon" stun "--hp 50 --target all --duration 2"

# Custom announcement
.bb mechanic-add "Dragon" stun "--hp 75 --target closest --duration 4 --announcement 'The dragon fixes its gaze!'"
```

---

### AoE (Area of Effect) Mechanic
[→ Full Documentation](../mechanics/AoeMechanic.md)
Creates damaging areas with telegraphed warnings.

#### Syntax
```
.bb mechanic-add <BossName> aoe "<options>"
```

#### Options
- `--hp <percent>` - Trigger at health percentage
- `--aoe_type <type>` - Damage type: fire, frost, blood, holy, shadow, poison, electric
- `--radius <units>` - Area size
- `--damage <amount>` - Damage per tick
- `--delay <seconds>` - Warning time before damage
- `--pattern <type>` - Placement pattern: boss, ring, random, cross, line, players, spiral
- `--count <number>` - Number of AoEs to create
- `--duration <seconds>` - How long AoEs last

#### Examples
```bash
# Fire explosions around boss at 75% HP
.bb mechanic-add "Dragon" aoe "--hp 75 --aoe_type fire --radius 10 --damage 500 --delay 2 --pattern ring --count 5"

# Frost zones on players at 50% HP
.bb mechanic-add "Frost Giant" aoe "--hp 50 --aoe_type frost --radius 8 --damage 300 --pattern players --count 3"

# Shadow cross pattern at 25% HP
.bb mechanic-add "Shadow Lord" aoe "--hp 25 --aoe_type shadow --radius 12 --damage 600 --pattern cross"
```


---

### Slow Mechanic
[→ Full Documentation](../mechanics/SlowMechanic.md)
Slows nearby players and drains energy.

#### Syntax
```
.bb mechanic-add <BossName> slow "<options>"
```

#### Options
- `--hp <percent>` - Trigger at health percentage
- `--target <type>` - Target selection: all, random, closest
- `--radius <units>` - Effect radius
- `--min_players <number>` - Minimum players required
- `--global_radius <units>` - Punishment radius if min not met
- `--announcement <text>` - Custom message

#### Examples
```bash
# Slow all players in radius at 50% HP
.bb mechanic-add "Frost Mage" slow "--hp 50 --target all --radius 15"

# Cooperative mechanic - need 3 players close
.bb mechanic-add "Ancient One" slow "--hp 60 --radius 10 --min_players 3 --global_radius 100"
```

---

### Absorb Mechanic
[→ Full Documentation](../mechanics/AbsorbMechanic.md)
Boss drains health or shields from players.

#### Syntax
```
.bb mechanic-add <BossName> absorb "<options>"
```

#### Options
- `--hp <percent>` - Trigger at health percentage
- `--type <type>` - Absorb type: health, shield, all
- `--amount <value>` - Amount to absorb (health type only)
- `--continuous <true/false>` - One-time or continuous
- `--min_players <number>` - Minimum players for cooperative

#### Examples
```bash
# Drain health at 25% HP
.bb mechanic-add "Vampire" absorb "--hp 25 --type health --amount 100"

# Steal all shields at 50% HP
.bb mechanic-add "Mage Slayer" absorb "--hp 50 --type shield"

# Combined absorption (70% health, 30% shield)
.bb mechanic-add "Void Lord" absorb "--hp 40 --type all --amount 50"

# Continuous health drain
.bb mechanic-add "Life Stealer" absorb "--hp 30 --type health --amount 50 --continuous true"
```

---

## Best Practices

### Mechanic Progression

#### Health Threshold Planning
```bash
# Phase 1: 100-75% HP
.bb mechanic-add "Boss" stun "--hp 90 --target random --duration 2"

# Phase 2: 75-50% HP  
.bb mechanic-add "Boss" aoe "--hp 75 --aoe_type fire --radius 10 --damage 400"
.bb mechanic-add "Boss" slow "--hp 60 --radius 15"

# Phase 3: 50-25% HP
.bb mechanic-add "Boss" absorb "--hp 50 --type shield"
.bb mechanic-add "Boss" aoe "--hp 40 --pattern players --count 5"

# Phase 4: 25-0% HP
.bb mechanic-add "Boss" absorb "--hp 25 --type health --amount 100"
.bb mechanic-add "Boss" slow "--hp 20 --radius 30 --min_players 3"
```

### Combining Mechanics

#### Combo Example 1: Stun + AoE
```bash
# Stun players then create danger zones
.bb mechanic-add "Controller" stun "--hp 70 --target random --duration 3"
.bb mechanic-add "Controller" aoe "--hp 69 --aoe_type fire --pattern random --count 5"
```

#### Combo Example 2: Absorb + Slow
```bash
# Drain shields then slow everyone
.bb mechanic-add "Drainer" absorb "--hp 40 --type shield"
.bb mechanic-add "Drainer" slow "--hp 39 --radius 25 --target all"
```

### Testing Configuration

1. **Test Individual Mechanics**
   ```bash
   .bb mechanic-add "Test" stun "--hp 90 --target random --duration 3"
   .bb debug test "Test"
   .bb mechanic-test "Test" 0
   ```

2. **Check Timing**
   - Ensure mechanics don't overlap too much
   - Leave gaps for normal combat
   - Test full progression

3. **Balance Difficulty**
   - Start with fewer mechanics
   - Add more for experienced players
   - Consider group size

### Common Patterns

#### Classic MMO Boss
```bash
# Tank swap mechanic
.bb mechanic-add "Raid Boss" stun "--hp 80 --target closest --duration 5"

# Raid-wide damage
.bb mechanic-add "Raid Boss" aoe "--hp 60 --pattern players --count 5"

# Shield phase
.bb mechanic-add "Raid Boss" absorb "--hp 40 --type shield"

# Burn phase with cooperative element
.bb mechanic-add "Raid Boss" slow "--hp 20 --radius 15 --min_players 4"
```

#### Action Boss
```bash
# Fast-paced combat
.bb mechanic-add "Agile Boss" aoe "--hp 90 --pattern spiral --delay 1"
.bb mechanic-add "Agile Boss" slow "--hp 70 --radius 20"
.bb mechanic-add "Agile Boss" aoe "--hp 50 --pattern random --count 10"
.bb mechanic-add "Agile Boss" absorb "--hp 30 --type all --amount 75"
```

---

## Troubleshooting

### "Invalid mechanic type"
- Check spelling of mechanic name
- Use `.bb mechanic-help` for valid types

### "Mechanic not triggering"
- Verify HP threshold makes sense
- Check if mechanic is enabled
- Ensure boss health actually reaches threshold

### "Options not working"
- **Remember to use quotes around options**
- Check parameter names (use `--` prefix)
- Verify values are in correct format

### Performance Issues
- Too many simultaneous mechanics can lag
- Limit AoE count and particle effects
- Space out mechanic triggers

---

[← Back to Index](index.md) | [Next: Debug Commands →](debug.md)