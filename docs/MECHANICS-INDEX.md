# 🎯 Boss Mechanics Documentation

Complete documentation for all boss mechanics available in BloodyBoss v2.1.0.

## 📚 Table of Contents

### Core Combat Mechanics
1. [Enrage](mechanics/ENRAGE.md) - Boss becomes more powerful
2. [Shield](mechanics/SHIELD.md) - Boss gains damage immunity/resistance
3. [Heal](mechanics/HEAL.md) - Boss heals itself or allies
4. [Summon](mechanics/SUMMON.md) - Boss spawns additional enemies
5. [Teleport](mechanics/TELEPORT.md) - Boss changes position instantly

### Control Mechanics
6. [Stun](mechanics/STUN.md) - Stuns targeted players ⭐ NEW
7. [Fear](mechanics/FEAR.md) - Forces players to flee
8. [Slow](mechanics/SlowMechanic.md) - Reduces player movement speed
9. [Root](mechanics/ROOT.md) - Immobilizes players
10. [Silence](mechanics/SILENCE.md) - Prevents ability usage

### Area Effects
11. [AoE](mechanics/AoeMechanic.md) - Area damage effects
12. [Pull](mechanics/PULL.md) - Pulls players toward boss
13. [Knockback](mechanics/KNOCKBACK.md) - Pushes players away
14. [Trap](mechanics/TRAP.md) - Creates hazardous zones

### Advanced Mechanics
15. [Phase](mechanics/PHASE.md) - Boss changes behavior phases
16. [Clone](mechanics/CLONE.md) - Creates boss copies
17. [DoT](mechanics/DOT.md) - Damage over time effects
18. [Buff Steal](mechanics/BUFF_STEAL.md) - Steals player buffs
19. [Reflect](mechanics/REFLECT.md) - Reflects damage back
20. [Mind Control](mechanics/MIND_CONTROL.md) - Controls player actions

### Utility Mechanics
21. [Vision](mechanics/VISION.md) - Affects player vision
22. [Weaken](mechanics/WEAKEN.md) - Reduces player stats
23. [Curse](mechanics/CURSE.md) - Applies various curses
24. [Dispel](mechanics/DISPEL.md) - Removes player buffs
25. [Absorb](mechanics/AbsorbMechanic.md) - Absorbs damage or resources

## 🎮 Quick Start

### Basic Syntax
```bash
.bb mechanic-add "<BossName>" "<MechanicType>" "[options]"
```

### Examples
```bash
# Add stun at 80% HP
.bb mechanic-add "MyBoss" "stun" "--hp 80"

# Configure stun parameters
.bb mechanic-config "MyBoss" 0 "target=nearest duration=3 mark_duration=2.5"

# List all mechanics
.bb mechanic-list "MyBoss"
```

## ⚙️ Common Parameters

### Trigger Types
- **HP Threshold**: `--hp <percentage>` (triggers at specific health %)
- **Time Based**: `--time <seconds>` (triggers after X seconds)
- **Player Count**: `--players <count>` (triggers with X players nearby)

### Comparisons
- `less_than` (default for HP)
- `greater_than` 
- `equals`

### Targeting
- `nearest` - Closest player
- `farthest` - Farthest player
- `random` - Random player(s)
- `all` - All players in range

## 📝 Configuration Commands

```bash
# View mechanic configuration
.bb mechanic-config "<BossName>" <index>

# Update mechanic parameters
.bb mechanic-config "<BossName>" <index> "param1=value1 param2=value2"

# Enable/disable mechanic
.bb mechanic-toggle "<BossName>" <index>

# Remove mechanic
.bb mechanic-remove "<BossName>" <index>
```

## 🔍 Testing

```bash
# Test trigger a mechanic (boss must be spawned)
.bb mechanic-test "<BossName>" <index>

# Export mechanics to file
.bb mechanic-export "<BossName>"
```

## 💡 Tips

1. **Start Simple**: Begin with basic mechanics like enrage or shield
2. **Test Incrementally**: Add one mechanic at a time and test
3. **Use Logs**: Check server logs for mechanic execution details
4. **Balance Carefully**: Consider player count and difficulty
5. **Mix Mechanics**: Combine different mechanics for interesting fights

## 🚫 Common Issues

- **Mechanic not triggering**: Check trigger conditions and boss HP
- **Effects not visible**: Some effects require specific conditions
- **Performance impact**: Too many mechanics can cause lag
- **Conflicts**: Some mechanics may conflict with each other

---

*For detailed information about each mechanic, click on the mechanic name above.*