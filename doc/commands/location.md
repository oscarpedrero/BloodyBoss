# Location Commands

Location commands allow you to set where bosses spawn in the world and provide teleportation utilities.

## Table of Contents
- [Set Location](#set-location)
- [Show Location](#show-location)
- [Teleport to Boss](#teleport-to-boss)
- [Location Best Practices](#location-best-practices)

---

## Set Location

Sets the boss spawn point to your current position.

### Syntax
```
.bb location set <BossName>
```

### Examples
```bash
# Stand where you want the boss to spawn, then:
.bb location set "Fire Dragon"
.bb location set TestBoss
```

### What Gets Saved
- X, Y, Z coordinates
- Your current rotation (boss will face the same direction)
- The location persists through server restarts

### Tips
- Test the area for accessibility before setting
- Ensure there's enough space for the boss model
- Consider player approach routes
- Avoid water or uneven terrain that might cause issues

### Visual Feedback
```
✅ Position 1250.5, 50.2, -890.3 set for boss 'Fire Dragon'
```

---

## Show Location

Displays the configured spawn location for a boss.

### Syntax
```
.bb location show <BossName>
```

### Examples
```bash
.bb location show "Fire Dragon"
.bb location show TestBoss
```

### Output Examples

**Location Set:**
```
📍 Boss 'Fire Dragon' location: 1250.5, 50.2, -890.3
```

**No Location:**
```
⚠️ Boss 'Fire Dragon' does not have a location set
```

### Uses
- Verify location before spawning
- Share coordinates with other admins
- Debug spawning issues

---

## Teleport to Boss

Instantly teleports you to the boss's configured location.

### Syntax
```
.bb location teleport <BossName>
```

### Examples
```bash
.bb location teleport "Fire Dragon"
.bb location teleport TestBoss
```

### Requirements
- Boss must have a location set
- Teleportation must be enabled in config
- Admin permissions (or configured otherwise)

### Configuration Options
The following can be set in the mod configuration:
- `EnableTeleportCommand` - Enable/disable the command
- `TeleportAdminOnly` - Restrict to admins only
- `TeleportOnlyToActiveBosses` - Only allow teleport when boss is spawned

### Visual Feedback
```
📍 Teleporting to boss 'Fire Dragon' at 1250.5, 50.2, -890.3
```

### Common Errors
```
🚫 Teleport command is disabled
🚫 Teleport command is restricted to administrators only
Boss 'Fire Dragon' does not have a location set
Boss 'Fire Dragon' is not currently active
```

---

## Location Best Practices

### Choosing Good Spawn Locations

#### ✅ Good Locations
- Open areas with clear ground
- Away from player spawn points
- Accessible from multiple directions
- Flat or gently sloped terrain
- Clear of obstacles and structures

#### ❌ Avoid These
- Inside or too close to buildings
- Narrow passages or corridors
- Steep cliffs or deep water
- Near important NPCs or merchants
- Player base locations

### Testing Your Location

1. **Set the location**
   ```
   .bb location set "Fire Dragon"
   ```

2. **Move away and teleport back**
   ```
   .bb location teleport "Fire Dragon"
   ```

3. **Check the area**
   - Is there enough space?
   - Can players approach safely?
   - Are there escape routes?

4. **Spawn test**
   ```
   .bb debug test "Fire Dragon"
   ```

### Multi-Boss Considerations

When setting up multiple bosses:
- Space them adequately apart (recommended: 100+ units)
- Consider patrol routes if bosses move
- Avoid overlapping combat areas
- Think about simultaneous spawns

### Location Examples by Biome

**Forest Areas**
- Clearings work best
- Avoid dense tree clusters
- Watch for uneven ground

**Desert/Plains**
- Most flexible for boss placement
- Good visibility for players
- Easy to navigate

**Mountain/Cave**
- Ensure adequate ceiling height
- Test for pathfinding issues
- Consider lighting

**Near Water**
- Set back from water's edge
- Test if boss can path properly
- Some bosses may get stuck in water

### Coordinate System

V Rising uses a 3D coordinate system:
- **X**: East (+) / West (-)
- **Y**: Up (+) / Down (-)
- **Z**: North (+) / South (-)

Example coordinates:
- `0, 0, 0` - World center
- `1250.5, 50.2, -890.3` - Specific location

### Advanced Tips

#### Marking Locations
Before setting a boss location, you can:
1. Drop an item as a marker
2. Build a small structure (if building is enabled)
3. Note nearby landmarks

#### Multiple Spawn Points
For variety, consider:
- Setting up day/night locations
- Seasonal spawn points
- Rotating between locations weekly

#### Safety Zones
Always ensure:
- 50+ unit clearance for large bosses
- No spawn-killing positions
- Fair engagement distances

---

## Troubleshooting

### Boss Not Spawning at Location

1. **Verify location is set**
   ```
   .bb location show <BossName>
   ```

2. **Check boss is enabled**
   ```
   .bb status <BossName>
   ```

3. **Test spawn manually**
   ```
   .bb debug test <BossName>
   ```

### Boss Spawning in Wrong Place

- Ensure you saved after setting location
- Check if boss has multiple spawn configurations
- Verify no other mods are interfering

### Teleport Not Working

Check these settings:
- `EnableTeleportCommand` in config
- Your admin permissions
- Boss has location set
- Boss exists in database

---

[← Back to Index](index.md) | [Next: Schedule Commands →](schedule.md)