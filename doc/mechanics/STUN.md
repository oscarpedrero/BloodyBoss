# 👁️ Stun Mechanic

The Stun mechanic marks players with a visual indicator before applying a stun effect, giving them time to react or prepare.

## Overview

When triggered, this mechanic:
1. Marks targeted players with a floating eye effect
2. Displays a warning message in chat
3. After the mark duration, applies a stun effect
4. Shows impact effects and screen shake

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `target` | string | "nearest" | Target selection: "nearest", "farthest", "random", "all" |
| `duration` | float | 3.0 | Stun duration in seconds |
| `mark_duration` | float | 2.5 | Warning mark duration before stun |
| `radius` | float | 0 | Effect radius (0 = single target) |
| `max_targets` | int | 1 | Maximum number of targets |
| `announcement` | string | "👁️ The boss's psychic gaze..." | Chat announcement |
| `flash_before_stun` | bool | true | Flash effect before stun (not implemented) |
| `can_be_cleansed` | bool | true | If mark can be removed (future feature) |
| `mark_effect` | string/int | "auto" | Visual effect GUID or "auto" for floating eye |

## Visual Effects

### Default Mark Effect
- **Floating Eye** (1520432556): A mystical eye hovers above the marked player
- Clearly visible and thematic
- Follows player movement

### Alternative Effects (use with `mark_effect` parameter)
- `1491093272` - Full Red Aura
- `-1807398295` - Holy Yellow Glow
- `662242066` - Hound Mark
- `-2118254056` - Lightning Shield
- See [CLAUDE.md](../../CLAUDE.md) for full list

## Usage Examples

### Basic Stun (80% HP)
```bash
.bb mechanic-add "BossName" "stun" "--hp 80"
```

### Configured Stun
```bash
# Add mechanic
.bb mechanic-add "BossName" "stun" "--hp 75 --comparison less_than"

# Configure parameters
.bb mechanic-config "BossName" 0 "target=nearest duration=3 mark_duration=2 max_targets=1"
```

### Multi-Target Stun
```bash
# Stun 3 nearest players
.bb mechanic-config "BossName" 0 "target=nearest duration=2 mark_duration=3 max_targets=3"
```

### Area Stun
```bash
# Stun all players within 20 units
.bb mechanic-config "BossName" 0 "target=all duration=2.5 radius=20"
```

### Custom Visual Effect
```bash
# Use red aura instead of floating eye
.bb mechanic-config "BossName" 0 "target=nearest duration=3 mark_effect=1491093272"
```

## Trigger Examples

### HP-Based Triggers
```bash
# At 50% HP
.bb mechanic-add "BossName" "stun" "--hp 50"

# Below 30% HP
.bb mechanic-add "BossName" "stun" "--hp 30 --comparison less_than"
```

### Time-Based Triggers
```bash
# Every 45 seconds
.bb mechanic-add "BossName" "stun" "--time 45 --repeat 45"

# Once after 60 seconds
.bb mechanic-add "BossName" "stun" "--time 60"
```

### Player Count Triggers
```bash
# When 3+ players nearby
.bb mechanic-add "BossName" "stun" "--players 3 --comparison greater_than"
```

## Strategy Tips

### For Players
- **Watch for the floating eye** - You have ~2.5 seconds to react
- **Spread out** - Avoid multiple players being stunned together
- **Save defensive cooldowns** - Use them when marked
- **Coordinate cleanses** - Help stunned allies (future feature)

### For Admins
- **Balance mark duration** - Longer = easier, shorter = harder
- **Mix with other mechanics** - Stun + AoE can be deadly
- **Consider player count** - Scale max_targets with group size
- **Test thoroughly** - Ensure visual effects work on your server

## Technical Notes

- Uses `Buff_General_Stun` (355774169) for the actual stun
- Mark effect is a buff that gets removed before stun application
- Screen shake affects players within 15 units of stunned target
- Chat messages are sent to all players

## Known Issues

- Some visual effects may not work depending on server/client versions
- Mark effect doesn't follow player perfectly in all situations
- Screen shake implementation is basic

## Examples for Testing

### Quick Test Setup
```bash
# Create test boss
.bb create "StunTest" -1905691330 20 0.5 600
.bb set location "StunTest"

# Add stun at 90% HP (easy to trigger)
.bb mechanic-add "StunTest" "stun" "--hp 90"

# Configure for quick testing
.bb mechanic-config "StunTest" 0 "duration=2 mark_duration=3"

# Spawn and test
.bb test "StunTest"
```

### Advanced Multi-Phase Example
```bash
# Phase 1: Single target stun at 75%
.bb mechanic-add "BossName" "stun" "--hp 75"
.bb mechanic-config "BossName" 0 "target=nearest duration=3 mark_duration=2.5"

# Phase 2: Dual target stun at 50%
.bb mechanic-add "BossName" "stun" "--hp 50"
.bb mechanic-config "BossName" 1 "target=nearest duration=2.5 mark_duration=2 max_targets=2"

# Phase 3: Area stun at 25%
.bb mechanic-add "BossName" "stun" "--hp 25"
.bb mechanic-config "BossName" 2 "target=all duration=2 mark_duration=1.5 radius=30"
```

---

[← Back to Mechanics Index](../MECHANICS-INDEX.md)