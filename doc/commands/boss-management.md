# Boss Management Commands

This section covers the core commands for creating and managing bosses in BloodyBoss.

## Table of Contents
- [Create Boss](#create-boss)
- [Remove Boss](#remove-boss)
- [List Bosses](#list-bosses)
- [Enable/Disable Boss](#enabledisable-boss)
- [VBlood Configuration](#vblood-configuration)
- [Reload Database](#reload-database)
- [Boss Status](#boss-status)

---

## Create Boss

Creates a new boss configuration.

### Syntax
```
.bb create <BossName> <PrefabGUID>
```

### Parameters
- `BossName` - Unique name for your boss (use quotes if it contains spaces)
- `PrefabGUID` - The game's prefab ID for the unit you want to use

### Examples
```bash
# Create a boss using Alpha Wolf's model
.bb create "Dire Wolf King" -1905691330

# Create a boss using Dracula's model
.bb create "Ancient Vampire Lord" -327335305

# Create a simple test boss
.bb create TestBoss -1905691330
```

### Common PrefabGUIDs
- `-1905691330` - Alpha the White Wolf
- `-327335305` - Dracula
- `1106149033` - Goreswine the Ravager
- `-2013903325` - Azariel the Sunbringer
- `153390636` - Voltatia the Power Master

### Notes
- Boss names must be unique
- The boss won't spawn until you configure location and enable it
- Default stats are based on the original unit's level

---

## Remove Boss

Permanently deletes a boss configuration.

### Syntax
```
.bb remove <BossName>
```

### Examples
```bash
.bb remove "Dire Wolf King"
.bb remove TestBoss
```

### Warning
- This action cannot be undone
- All associated data (items, mechanics, abilities) will be lost
- If the boss is currently spawned, it will be despawned

---

## List Bosses

Shows all configured bosses and their basic information.

### Syntax
```
.bb list
```

### Output Example
```
📋 Configured Bosses (3):
----------------------------
🐺 Dire Wolf King
   Status: ✅ Enabled
   Spawned: No
   Location: Set
   Schedule: 20:00

🦇 Ancient Vampire Lord
   Status: ❌ Disabled
   Spawned: Yes
   Location: Set
   Schedule: Not set

🧪 TestBoss
   Status: ✅ Enabled
   Spawned: No
   Location: Not set
   Schedule: Not set
----------------------------
```

---

## Enable/Disable Boss

Controls whether a boss can spawn automatically.

### Enable Syntax
```
.bb enable <BossName>
```

### Disable Syntax
```
.bb disable <BossName>
```

### Examples
```bash
# Enable automatic spawning
.bb enable "Dire Wolf King"

# Disable to prevent spawning
.bb disable "Dire Wolf King"
```

### Notes
- Disabled bosses won't spawn on schedule
- You can still manually spawn disabled bosses with `.bb debug test`
- Useful for temporarily removing bosses without deleting them

---

## VBlood Configuration

Configure a boss to appear as a VBlood on the map.

### Add VBlood Status
```
.bb addvblood <BossName>
```

### Remove VBlood Status
```
.bb removevblood <BossName>
```

### Examples
```bash
# Make boss appear on map with VBlood icon
.bb addvblood "Ancient Vampire Lord"

# Remove VBlood status
.bb removevblood "Ancient Vampire Lord"
```

### Benefits of VBlood Status
- Boss appears on the map with a skull icon
- Players can track the boss location
- Boss name appears when hovering over the icon
- Adds prestige to the encounter

---

## Reload Database

Reloads the boss configuration from disk.

### Syntax
```
.bb reload
```

### When to Use
- After manually editing configuration files
- To sync changes between server restarts
- If bosses aren't behaving as expected

### Notes
- This won't affect currently spawned bosses
- Schedule timers will be recalculated
- Use sparingly as it may cause brief lag

---

## Boss Status

Shows detailed information about a specific boss.

### Syntax
```
.bb status <BossName>
```

### Output Example
```
📊 Status for 'Ancient Vampire Lord':
━━━━━━━━━━━━━━━━━━━━━━━━━━━━
General Information:
├─ PrefabGUID: -327335305
├─ Enabled: ✅ Yes
├─ VBlood: ✅ Yes
└─ Created: 2025-01-10 15:30

Spawn Information:
├─ Currently Spawned: ❌ No
├─ Location Set: ✅ Yes (1250.5, 50.0, -890.3)
├─ Schedule: 20:00, 02:00
└─ Last Spawn: 2025-01-10 20:00

Combat Stats:
├─ Level: 90
├─ Health Multiplier: 2.5x
├─ Lifetime: 1800 seconds (30 minutes)
└─ Killers: 0

Configured Features:
├─ Items: 5 configured
├─ Abilities: 8 custom slots
├─ Mechanics: 3 active
└─ Phases: Dynamic scaling enabled
━━━━━━━━━━━━━━━━━━━━━━━━━━━━
```

---

## Server Cleanup

Completely removes all BloodyBoss entities and icons from the server.

### Syntax
```
.bb cleanup
```

### What it does
- **Removes all active bosses** - Destroys all currently spawned boss entities
- **Removes all boss icons** - Clears map markers and icons
- **Cleans orphaned entities** - Removes leftover boss/icon entities not in database
- **Updates database** - Marks all bosses as not spawned
- **Clears tracking systems** - Resets all internal tracking and caches

### Output Example
```
🧹 Starting server cleanup...
✅ Server cleanup completed:
├─ Bosses removed: 3
├─ Icons removed: 5
├─ Database entries updated: 3
└─ Tracking systems cleared: Yes
```

### When to Use
- **Server maintenance** - Clean up after testing or development
- **Before major updates** - Clear state before mod updates
- **Troubleshooting** - Reset everything to a known clean state
- **Performance issues** - Remove potentially corrupted entities

### Safety Notes
- ⚠️ **Admin only command** - Requires administrator privileges
- ⚠️ **Destructive operation** - Cannot be undone
- ⚠️ **All active bosses will be removed** - Even if players are fighting them
- ✅ **Database preserved** - Boss configurations remain intact, only spawn states are reset

### Related Commands
- Use `.bb list` to see which bosses are configured after cleanup
- Use `.bb start <BossName>` to manually spawn bosses after cleanup
- Use `.bb status <BossName>` to verify boss states after cleanup

---

## Best Practices

### Naming Conventions
- Use descriptive names that indicate the boss type
- Include difficulty indicators if helpful: "Elite", "Ancient", "Greater"
- Avoid special characters that might cause issues

### PrefabGUID Selection
- Choose models that match your boss theme
- Consider the base unit's animations and abilities
- Test different models to find the best fit

### Organization Tips
- Group similar bosses with prefixes: "Frost Giant Alpha", "Frost Giant Beta"
- Document your configurations outside the game
- Test thoroughly before enabling automatic spawns

### Common Issues

**"Boss already exists"**
- Each boss needs a unique name
- Use `.bb list` to see existing names

**"Invalid PrefabGUID"**
- Ensure you're using a valid unit prefab
- Check the PrefabGUID list or wiki for correct values

**"Boss not spawning"**
- Verify location is set: `.bb location show <BossName>`
- Check if enabled: `.bb status <BossName>`
- Ensure schedule is configured if using timed spawns

---

[← Back to Index](index.md) | [Next: Location Commands →](location.md)