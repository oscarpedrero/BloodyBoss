# BloodyBoss v2.1.0 - Complete User Guide

Welcome to BloodyBoss, the ultimate boss management system for V Rising servers! This guide will help you create epic boss encounters with dynamic scaling, custom mechanics, and exciting rewards.

## 🚀 Quick Start

### Your First Boss in 3 Steps

```bash
# 1. Create a boss
.bb create "Alpha Wolf King" -1905691330

# 2. Set spawn location (stand where you want the boss)
.bb location set "Alpha Wolf King"

# 3. Schedule spawn time
.bb schedule set "Alpha Wolf King" 20:00
```

That's it! Your boss will spawn automatically at 8 PM server time.

## 📚 Complete Documentation

### Core Systems

- **[Boss Management](commands/boss-management.md)** - Create, remove, and configure bosses
- **[Location Commands](commands/location.md)** - Set spawn points and territories
- **[Schedule Commands](commands/schedule.md)** - Control when bosses spawn
- **[Item Rewards](commands/items.md)** - Configure loot drops and rewards

### Advanced Features

- **[Custom Abilities](commands/abilities.md)** - Swap and customize boss abilities
- **[Boss Mechanics](commands/mechanics.md)** - Add special combat mechanics
- **[Debug Tools](commands/debug.md)** - Testing and troubleshooting

### Reference Guides

- **[Configuration Guide](CONFIGURATION.md)** - Complete BloodyBoss.cfg documentation
- **[Boss Mechanics List](BOSS-MECHANICS.md)** - All available mechanics explained
- **[VBlood Abilities Database](VBLOOD-ABILITIES.md)** - Complete ability reference
- **[Boss Abilities Guide](BOSS-ABILITIES.md)** - Understanding ability slots

## 🎮 How It Works

### Automatic Boss Lifecycle

1. **Spawn**: Boss appears at scheduled time
2. **Combat**: Players engage with dynamic mechanics
3. **Rewards**: Victorious players receive configured items
4. **Despawn**: Boss automatically disappears after lifetime expires

### Key Features

- ✅ **Dynamic Scaling** - Bosses adapt to player count and performance
- ✅ **Progressive Difficulty** - Encounters get harder if players fail
- ✅ **Phase Announcements** - Clear combat feedback at health thresholds
- ✅ **Custom Mechanics** - Stuns, AoE attacks, shields, and more
- ✅ **Flexible Rewards** - Items with configurable drop chances
- ✅ **VBlood Integration** - Bosses can appear on the map

## 🔧 Configuration Examples

### Basic Combat Boss
```bash
# Create a straightforward combat encounter
.bb create "Dire Wolf" -1905691330 50 1.5 300
.bb location set "Dire Wolf"
.bb item add "Dire Wolf" "Blood Essence" 862477668 10 0.5  # 50% chance for 10 blood essence
.bb schedule set "Dire Wolf" 19:00
```

### Raid Boss with Mechanics
```bash
# Create a complex raid encounter
.bb create "Ancient Dragon" -393555055 90 2.0 1800
.bb location set "Ancient Dragon"

# Add mechanics (remember quotes!)
.bb mechanic-add "Ancient Dragon" stun "--hp 75 --target random --duration 3"
.bb mechanic-add "Ancient Dragon" aoe "--hp 50 --aoe_type fire --radius 10 --damage 500"
.bb mechanic-add "Ancient Dragon" absorb "--hp 25 --type health --amount 100"

# Add valuable rewards
.bb item add "Ancient Dragon" "Legendary Weapon" -257494203 1 0.1   # 10% for legendary weapon
.bb item add "Ancient Dragon" "Blood Essence" 862477668 100 1.0 # Guaranteed blood essence

# Weekend spawn only
.bb schedule set "Ancient Dragon" 20:00
```

### Event Boss
```bash
# Limited-time event boss
.bb create "Frost Spirit" 1007496851 60 1.5 300
.bb location set "Frost Spirit"

# Special mechanics (quotes required!)
.bb mechanic-add "Frost Spirit" slow "--hp 90 --radius 15"
.bb mechanic-add "Frost Spirit" aoe "--hp 50 --aoe_type frost --pattern spiral --count 5"

# Event rewards
.bb item add "Frost Spirit" "Event Token" -2128746226 5 0.25  # 25% for special event item

# Frequent spawns during event
.bb schedule set "Frost Spirit" 10:00
.bb schedule set "Frost Spirit" 14:00
.bb schedule set "Frost Spirit" 18:00
.bb schedule set "Frost Spirit" 22:00
```

## 📊 Understanding Boss Configuration

### Lifetime System
Bosses now use an automatic lifetime system. Once spawned, they remain active for the configured duration then disappear automatically. No manual despawn needed!

- **Default**: 300 seconds (5 minutes)
- **Standard Boss**: 1800 seconds (30 minutes)
- **Raid Boss**: 3600 seconds (1 hour)
- **Event Boss**: 300-600 seconds (5-10 minutes)

### Scaling Options

#### Player Count Scaling
```bash
# Multiplier is set during boss creation
.bb create "BossName" -1905691330 50 2.0 300  # 2x health multiplier
```

#### Dynamic Scaling (Recommended)
Automatically adjusts based on:
- Active player count
- Player average level
- Previous encounter results
- Boss consecutive spawns

Enable with: `.bb config dynamic_scaling true`

### Reward System

Items drop directly to player inventory when:
1. Player dealt damage to the boss
2. Player was alive when boss died
3. Drop chance roll succeeds

If inventory is full, items drop on the ground near the player.

## 🛠️ Troubleshooting

### Boss Not Spawning
1. Check boss is enabled: `.bb status "BossName"`
2. Verify location is set: `.bb location show "BossName"`
3. Confirm schedule: `.bb schedule list`
4. Test manually: `.bb debug test "BossName"`

### Players Not Getting Rewards
1. Ensure players damaged the boss
2. Check item configuration: `.bb item list "BossName"`
3. Verify drop chances aren't too low
4. Look for items on ground if inventory was full

### Performance Issues
1. Reduce number of simultaneous bosses
2. Lower mechanic frequency
3. Decrease particle effects in mechanics
4. Stagger spawn times

## 🎯 Best Practices

### Spawn Locations
- Choose open areas for better combat
- Avoid player bases and important paths
- Consider multiple locations for variety
- Set location with `.bb location set` at desired position

### Difficulty Balance
- Start with lower multipliers (1.5-2x)
- Test with different player counts
- Adjust based on player feedback
- Use progressive difficulty for learning curve

### Reward Design
- Mix guaranteed and chance-based rewards
- Include both common and rare items
- Scale rewards with difficulty
- Consider server economy impact

### Schedule Planning
- Peak hours: More frequent spawns
- Off-hours: Longer lasting bosses
- Weekends: Special encounters
- Events: Limited-time bosses

## 📝 Command Quick Reference

### Essential Commands
```bash
.bb help                    # Show all commands
.bb create <name> <prefab>  # Create boss
.bb list                    # List all bosses
.bb status <name>           # Boss details
.bb reload                  # Reload configs
```

### During Combat
```bash
.bb debug info <name>       # Show boss technical info
.bb debug test <name>       # Quick test spawn
.bb debug force-drop <name> # Force item drops
```

## 🔄 Recent Updates (v2.1.0)

### What's New
- ✅ Automatic lifetime system (no manual despawn needed)
- ✅ Improved damage detection and reward distribution
- ✅ Enhanced performance with optimized tracking
- ✅ Better error handling and stability
- ✅ Simplified configuration process

### What Changed
- ❌ HourDespawn removed (use lifetime instead)
- ❌ Manual despawn deprecated
- ❌ SetHourDespawn() no longer exists
- ✅ All bosses now use automatic cleanup

## 💡 Tips & Tricks

1. **Test First**: Always test new bosses with `.bb debug test` before scheduling
2. **Start Small**: Begin with simple bosses, add complexity gradually
3. **Monitor Logs**: Check server logs for detailed spawn/combat information
4. **Player Feedback**: Adjust difficulty based on community input
5. **Backup Configs**: Keep copies of your Bosses.json and Items.json

## 🆘 Getting Help

- Use `.bb help <command>` for detailed command info
- Check server logs for error messages
- Review this documentation for examples
- Test in isolation with debug commands

---

**Happy Boss Hunting!** 🗡️

*BloodyBoss v2.1.0 - Making V Rising servers legendary, one boss at a time*