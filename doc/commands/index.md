# BloodyBoss Commands Documentation

Welcome to the comprehensive command reference for BloodyBoss. This documentation is organized by command categories to help you quickly find what you need.

## 📚 Command Categories

### Core Functions
- [**Boss Management**](boss-management.md) - Create, remove, and manage bosses
- [**Location Commands**](location.md) - Set spawn points and teleport
- [**Schedule Commands**](schedule.md) - Control when bosses appear
- [**Item Configuration**](items.md) - Configure loot drops and rewards

### Advanced Features
- [**Ability System**](abilities.md) - Copy and mix abilities from VBloods
- [**Mechanic System**](mechanics.md) - Add combat mechanics and phases
- [**Debug Commands**](debug.md) - Testing and troubleshooting tools
- [**Logging System**](logging.md) - Control log verbosity and categories

### Server Maintenance
- [**Server Cleanup**](boss-management.md#server-cleanup) - Clean up all BloodyBoss entities and icons

### Configuration
- [**Configuration Guide**](../CONFIGURATION.md) - Complete BloodyBoss.cfg documentation

## 🚀 Quick Start

New to BloodyBoss? Start here:

1. **[Boss Management](boss-management.md)** - Learn how to create your first boss
2. **[Location Commands](location.md)** - Set where your boss spawns
3. **[Schedule Commands](schedule.md)** - Configure when it appears
4. **[Item Configuration](items.md)** - Add rewards for defeating the boss

## 📖 Command Syntax

All commands follow this pattern:
```
.bb <category> <action> [parameters]
```

### Common Rules:
- All commands start with `.bb`
- Names with spaces need quotes: `"Boss Name"`
- Most commands require admin permissions
- Parameters in `<brackets>` are required
- Parameters in `[brackets]` are optional

## 🎯 Example Workflow

Here's a complete example of creating a boss:

```bash
# Create the boss
.bb create "Fire Dragon" -1905691330

# Set spawn location
.bb location set "Fire Dragon"

# Schedule spawn time
.bb schedule set "Fire Dragon" 20:00

# Add loot
.bb item add "Fire Dragon" "Dragon Blood" -77477508 10 1.0

# Add abilities from other VBloods
.bb ability-slot "Fire Dragon" "Alpha the White Wolf" 0 true

# Add mechanics
.bb mechanic-add "Fire Dragon" enrage --hp 25 --damage 1.5

# Test it
.bb debug test "Fire Dragon"
```

## 💡 Tips

- Start simple and add complexity gradually
- Use `.bb debug test` to quickly test your configurations
- Check logs for detailed error messages
- Join our Discord for community support

---

*Select a category above to view detailed command documentation.*