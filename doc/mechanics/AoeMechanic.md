# AoE Mechanic Documentation

## Overview
The AoE (Area of Effect) mechanic allows bosses to perform devastating area attacks with various elemental types. Each attack type has unique visual effects and may include additional gameplay effects.

## How It Works

### Attack Phases

1. **Telegraph Phase (Warning)**
   - When the boss prepares an AoE attack, it displays a casting animation with the corresponding elemental effect
   - Players within the danger radius receive a floating eye marker (👁️) above their heads
   - This warning phase lasts for the configured delay time (default: 2 seconds)
   - During this phase, players can move out of the danger zone to avoid damage

2. **Damage Phase**
   - After the delay, the AoE damage is applied to all players still within the radius
   - Each player hit receives the corresponding elemental visual effect
   - Damage is calculated with distance falloff (30% reduction at the edge of the radius)

### Visual Effects

⚠️ **WARNING**: Visual effects may include additional gameplay mechanics beyond just visuals!

Each damage type has unique visual effects that may affect gameplay:

- **🔥 Fire**: Ignites players with burning flames
- **❄️ Frost**: Wendigo freeze effect (may slow movement)
- **🩸 Blood**: Blood rage fury effect
- **✨ Holy**: Golden holy aura
- **👁️ Shadow**: Dracula's shadow bat swarm
- **☠️ Poison**: Toxic sludge area effect
- **⚡ Electric**: Lightning shield with projectiles (may knockback)

## Command Usage

```
.bb mechanic-config "<boss_name>" <phase> "aoe_type=<type> radius=<radius> damage=<damage> delay=<delay>"
```

### Parameters

- **aoe_type**: Type of elemental damage (fire, frost, blood, holy, shadow, poison, electric)
- **radius**: Area of effect radius in meters (default: 10)
- **damage**: Base damage amount (default: 50)
- **delay**: Warning time before damage in seconds (default: 2)
- **pattern**: AoE positioning pattern (default: "boss")
  - `boss`/`center`: Single AoE at boss position
  - `ring`: Multiple AoEs in a ring around boss
  - `random`: Random positions around boss
  - `cross`: Cross pattern centered on boss
  - `line`: Line of AoEs
  - `players`: Target player positions
  - `spiral`: Spiral pattern
- **count**: Number of AoE zones for multi-patterns (default: 1)
- **announcement**: Custom warning message (optional)
- **target_players**: If true, directly targets player positions (default: false)

### Examples

**Basic fire AoE at boss position:**
```
.bb mechanic-config "BossName" 0 "aoe_type=fire radius=10 damage=50 delay=2"
```

**Electric ring pattern with 8 zones:**
```
.bb mechanic-config "BossName" 0 "aoe_type=electric pattern=ring count=8 radius=15 damage=75 delay=3"
```

**Frost AoE targeting players directly:**
```
.bb mechanic-config "BossName" 0 "aoe_type=frost pattern=players target_players=true damage=100"
```

**Poison spiral with custom announcement:**
```
.bb mechanic-config "BossName" 0 "aoe_type=poison pattern=spiral count=5 announcement='💀 Toxic spiral incoming!'"
```

## Mechanic Strategy

### For Players
- Watch for the boss's casting animation - this signals an incoming AoE attack
- Look for the floating eye marker (👁️) - if you have it, you're in the danger zone!
- You have a brief window (delay time) to escape before damage hits
- Different elements have different side effects - plan accordingly:
  - Electric attacks may knock you back
  - Frost attacks may slow your movement
  - Fire will burn over time

### For Server Admins
- Balance damage values based on your server's difficulty
- Shorter delays make attacks harder to dodge
- Larger radius values create more dangerous encounters
- Combine different patterns for varied boss phases
- Use HP thresholds to trigger AoEs at specific boss health percentages

## Technical Notes

- Visual effects are game buffs that may include inherent mechanics
- Damage calculation includes distance-based falloff for fairness
- The telegraph system ensures players have a chance to react
- All effects are synchronized across clients for consistent gameplay