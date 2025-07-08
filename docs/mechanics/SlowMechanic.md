# Slow Mechanic Documentation

## Overview
The Slow mechanic drains energy from players within range, drastically reducing their movement speed. This mechanic includes cooperative gameplay elements that punish teams who don't stay together during critical moments.

## How It Works

### Energy Drain Effect
When activated, the boss drains energy from nearby players, applying:
- **Movement Speed Reduction**: Significant slow debuff that makes dodging difficult
- **Visual Effect**: Vampiric energy drain animation on affected players
- **Fixed Duration**: The slow effect has a predetermined duration based on the buff

### Cooperative Mechanic - Minimum Players

⚠️ **IMPORTANT**: This mechanic encourages team coordination!

When `min_players` is set, the mechanic requires a minimum number of players within the normal radius:
- **Enough players in range**: Only those players are slowed
- **NOT enough players in range**: **GLOBAL PUNISHMENT** activates!

#### Global Punishment
- Affects ALL players within a much larger radius (`global_radius`)
- Same slow effect applied to everyone in the extended range
- Warning message alerts all players about failed cooperation
- Forces teams to group up during boss encounters

## Command Usage

```
.bb mechanic-config "<boss_name>" <phase> "radius=<radius> [options]"
```

### Parameters

- **radius**: Effect radius in meters (default: 15, max: 50)
- **min_players**: Minimum players required in range (default: 0 = disabled)
- **global_radius**: Punishment radius if min_players not met (default: 50)
- **announcement**: Custom warning message (default: "🐌 Time slows down!")

### Examples

**Basic slow in 10m radius:**
```
.bb mechanic-config "1" 0 "radius=10"
```

**Cooperative mechanic requiring 2 players:**
```
.bb mechanic-config "1" 0 "radius=12 min_players=2"
```

**Full cooperative setup with large punishment radius:**
```
.bb mechanic-config "1" 0 "radius=15 min_players=3 global_radius=45"
```

**Custom announcement in Spanish:**
```
.bb mechanic-config "1" 0 "radius=15 announcement='⚠️ ¡El tiempo se ralentiza!'"
```

**Maximum challenge - small radius, high minimum:**
```
.bb mechanic-config "1" 0 "radius=8 min_players=4 global_radius=50 announcement='⏰ GATHER OR SUFFER!'"
```

## Mechanic Strategy

### For Players
- **Watch for the announcement** - You have moments to position correctly
- **Stay grouped** - If you see teammates getting slowed, move closer!
- **Plan movement** - While slowed, you're vulnerable to other attacks
- **Communication is key** - Call out positions when mechanic approaches

### For Server Admins
- **Combo potential**: Use before AoE attacks for devastating combinations
- **Difficulty scaling**: Increase min_players for larger groups
- **Phase transitions**: Great for forcing players to regroup
- **Balance carefully**: Too small radius + high min_players = very hard

## Visual Indicators

- **Slowed players**: Purple vampiric curse effect showing energy drain
- **Movement impaired**: Visibly slower movement animation
- **No boss speed change**: Boss maintains normal speed

## Technical Notes

- Slow duration is fixed by the buff (cannot be configured)
- The actual slow percentage is determined by the game buff
- Visual effects are synchronized across all clients
- Distance calculations use 3D space (height differences matter)

## Common Combinations

1. **Slow → AoE**: Players can't escape damage zones
2. **Slow → Summon**: Adds become more dangerous
3. **Slow → Pull**: Guaranteed to bring players close
4. **Phase transition**: Force regroup between phases

## Troubleshooting

**Players not getting slowed:**
- Check if they're actually within radius
- Verify boss entity exists and has position
- Some buffs may provide slow immunity

**Global punishment not triggering:**
- Ensure min_players > 0
- Check global_radius is larger than radius
- Verify player count calculation

**Visual effects missing:**
- Effects are cosmetic and may not show in all conditions
- Check server logs for application confirmation