# Boss Mechanics System

## Overview

The Boss Mechanics System in BloodyBoss allows you to create dynamic, challenging encounters by adding special behaviors that trigger during combat. These mechanics transform simple boss fights into memorable experiences with multiple phases, special attacks, and cooperative challenges.

## Table of Contents
- [What Are Boss Mechanics?](#what-are-boss-mechanics)
- [How the System Works](#how-the-system-works)
- [Types of Triggers](#types-of-triggers)
- [Available Mechanics Explained](#available-mechanics-explained)
- [Creating Dynamic Encounters](#creating-dynamic-encounters)
- [Examples and Templates](#examples-and-templates)
- [Tips for Server Admins](#tips-for-server-admins)

---

## What Are Boss Mechanics?

Boss mechanics are special actions or behaviors that activate during combat to create more engaging fights. Think of them as the "special moves" that make each boss unique and challenging.

### Examples from Popular Games
- **WoW Raids**: Bosses that summon adds, create fire on the ground, or enrage at low health
- **Dark Souls**: Bosses that change attack patterns or gain new abilities in different phases
- **Final Fantasy**: Bosses with ultimate attacks that require specific positioning or actions

BloodyBoss brings these concepts to V Rising, allowing you to create similar experiences.

---

## How the System Works

### The Flow of Combat

1. **Normal Combat**: Boss uses its regular abilities (configured via the [Ability System](VBLOOD-ABILITIES.md))
2. **Trigger Conditions**: When certain conditions are met (health %, time, etc.)
3. **Mechanic Activation**: Special mechanics activate with warnings
4. **Player Response**: Players must adapt to survive
5. **Phase Progression**: Multiple mechanics create distinct combat phases

### Visual Indicators

Most mechanics include visual warnings:
- **Floating Eyes**: Mark targeted players before mechanics
- **Ground Effects**: Show where damage will occur
- **Buff Icons**: Indicate active effects on boss/players
- **Chat Announcements**: Warn players of incoming mechanics

---

## Types of Triggers

### Health-Based Triggers
Most common trigger type - activates at specific health percentages.

```bash
# Example: Stun at 75% health
.bb mechanic-add "Dragon" stun "--hp 75 --target random --duration 3"
```

**Best Practices:**
- Use descending thresholds: 90% → 75% → 50% → 25%
- Space mechanics to avoid overlap
- Save most dangerous mechanics for low health

### Time-Based Triggers
Activates after a certain duration or repeatedly.

```bash
# Example: Periodic AoE attacks every 45 seconds
.bb mechanic-add "Mage" aoe "--time 45 --repeat 45 --aoe_type fire --radius 10"
```

**Use Cases:**
- Periodic pressure mechanics
- Escalating difficulty over time
- Rhythm-based encounters

### Conditional Triggers
Based on player count or other conditions.

```bash
# Example: Requires 3 players nearby or punishes everyone
.bb mechanic-add "Ancient One" slow "--hp 50 --min_players 3 --global_radius 100"
```

**Benefits:**
- Encourages teamwork
- Scales with group size
- Creates positioning challenges

---

## Available Mechanics Explained

Currently, BloodyBoss features four fully functional and tested mechanics that can transform your boss encounters:

### Stun Mechanic
**What it does**: Temporarily disables targeted players, preventing all actions.

**Player Experience**:
1. Floating eye appears above target (warning)
2. After delay, player is stunned
3. Cannot move, attack, or use abilities
4. Other players must protect stunned ally

**Strategic Uses**:
- Force tank swaps
- Create vulnerability windows
- Test group coordination

[→ Detailed Stun Mechanic Documentation](mechanics/STUN.md)

### AoE (Area of Effect) Mechanic
**What it does**: Creates damaging zones on the battlefield with telegraphed warnings.

**Player Experience**:
1. Ground indicators show where damage will occur
2. Players have time to move away
3. Damage applies to anyone remaining in zones
4. Different patterns create different challenges

**Damage Types & Effects**:
- **Fire**: Burning damage over time
- **Frost**: Slows movement
- **Shadow**: Reduces healing received
- **Holy**: Extra damage to undead
- **Electric**: Chains between nearby players
- **Poison**: Stacking damage over time
- **Blood**: Life drain effect

[→ Detailed AoE Mechanic Documentation](mechanics/AoeMechanic.md)

### Slow Mechanic
**What it does**: Reduces player movement speed and drains energy.

**Player Experience**:
1. Area effect that slows players
2. Energy/stamina drains while affected
3. Makes dodging other mechanics harder
4. Can require group coordination

**Cooperative Element**:
- Can require minimum players in range
- Punishes spread groups
- Encourages stacking strategies

[→ Detailed Slow Mechanic Documentation](mechanics/SlowMechanic.md)

### Absorb Mechanic
**What it does**: Boss drains health or shields from players to heal itself.

**Player Experience**:
1. Boss channels drain effect
2. Players lose health/shields
3. Boss gains health/shields
4. Can be continuous or burst

**Types**:
- **Health**: Direct life steal
- **Shield**: Removes all shield buffs
- **All**: Combines both effects

[→ Detailed Absorb Mechanic Documentation](mechanics/AbsorbMechanic.md)

---

## Creating Dynamic Encounters

### Phase Design Philosophy

Good boss fights tell a story through mechanics:

#### Phase 1: Introduction (100-75% HP)
- Simple mechanics
- Teach players the basics
- Low pressure

```bash
.bb mechanic-add "Dragon" stun "--hp 90 --target random --duration 2"
```

#### Phase 2: Development (75-50% HP)
- Add complexity
- Combine mechanics
- Moderate pressure

```bash
.bb mechanic-add "Dragon" aoe "--hp 75 --aoe_type fire --radius 10"
.bb mechanic-add "Dragon" slow "--hp 60 --radius 15 --min_players 2"
```

#### Phase 3: Climax (50-25% HP)
- Force adaptation
- Cooperative mechanics
- High pressure

```bash
.bb mechanic-add "Dragon" absorb "--hp 50 --type shield"
.bb mechanic-add "Dragon" slow "--hp 40 --radius 20 --min_players 3"
```

#### Phase 4: Finale (25-0% HP)
- All-out assault
- Maximum pressure
- Combined mechanics

```bash
.bb mechanic-add "Dragon" absorb "--hp 25 --type health --amount 100"
.bb mechanic-add "Dragon" aoe "--hp 20 --pattern spiral --count 8"
```

### Combining Mechanics

Create memorable moments by combining mechanics:

#### The "Area Denial" Combo
```bash
# AoE patterns force movement while slowed
.bb mechanic-add "Controller" slow "--hp 50 --radius 30 --target all"
.bb mechanic-add "Controller" aoe "--hp 49 --pattern players --radius 8 --delay 3"
```

#### The "Coordination Check"
```bash
# Requires players to stack for slow mechanic
.bb mechanic-add "Guardian" slow "--hp 60 --radius 10 --min_players 4"
.bb mechanic-add "Guardian" stun "--hp 59 --target furthest --duration 4"
```

#### The "Drain Phase"
```bash
# Progressive resource drain
.bb mechanic-add "Vampire" absorb "--hp 75 --type shield"
.bb mechanic-add "Vampire" absorb "--hp 50 --type health --amount 50"
.bb mechanic-add "Vampire" absorb "--hp 25 --type all --continuous true"
```

---

## Examples and Templates

### Classic Tank and Spank
Simple boss for learning mechanics:
```bash
# Basic threat mechanics
.bb mechanic-add "Guardian" stun "--hp 80 --target closest --duration 4"
.bb mechanic-add "Guardian" aoe "--hp 50 --pattern boss --radius 15"
.bb mechanic-add "Guardian" absorb "--hp 25 --type health --amount 100"
```

### Raid-Style Encounter
Complex boss requiring coordination:
```bash
# Phase 1: Learning
.bb mechanic-add "Raid Boss" aoe "--hp 90 --pattern random --count 3"

# Phase 2: Coordination required
.bb mechanic-add "Raid Boss" stun "--hp 75 --target random --duration 3"
.bb mechanic-add "Raid Boss" slow "--hp 70 --min_players 3"

# Phase 3: Survival
.bb mechanic-add "Raid Boss" absorb "--hp 50 --type shield"
.bb mechanic-add "Raid Boss" aoe "--hp 45 --pattern players --count 5"

# Phase 4: Burn
.bb mechanic-add "Raid Boss" slow "--hp 25 --radius 40 --target all"
.bb mechanic-add "Raid Boss" absorb "--hp 20 --type health --continuous true"
```

### Solo-Friendly Boss
Designed for single players or small groups:
```bash
# Predictable patterns
.bb mechanic-add "Solo Boss" aoe "--hp 75 --pattern cross --delay 3"
.bb mechanic-add "Solo Boss" stun "--hp 50 --duration 2 --mark_duration 3"
.bb mechanic-add "Solo Boss" absorb "--hp 25 --type health --amount 50"
```

### PvP-Zone Boss
Encourages player conflict:
```bash
# Mechanics that affect large areas
.bb mechanic-add "PvP Boss" aoe "--hp 90 --pattern spiral --radius 30"
.bb mechanic-add "PvP Boss" slow "--hp 60 --radius 50 --target all"
.bb mechanic-add "PvP Boss" stun "--hp 40 --target random --duration 2"
```

---

## Tips for Server Admins

### Balancing for Your Community

#### Casual Servers
- Longer warning times on mechanics
- Lower damage values
- Fewer simultaneous mechanics
- More forgiving positioning

#### Hardcore Servers
- Shorter warnings
- Higher damage
- Complex mechanic combinations
- Tight DPS checks via enrage timers

### Performance Considerations

#### Optimize for Server Health
- Limit particle-heavy effects
- Space out AoE spawns
- Reasonable add counts
- Test with expected player counts

#### Common Performance Settings
```bash
# Good performance
.bb mechanic-add "Boss" aoe "--hp 50 --count 5 --radius 8"

# Potential lag
.bb mechanic-add "Boss" aoe "--hp 50 --count 20 --radius 15"
```

### Communication with Players

#### Announce New Bosses
- Explain unique mechanics
- Suggest group sizes
- Share location/schedule
- Provide strategy hints

#### Gather Feedback
- Watch fights personally
- Ask about difficulty
- Monitor kill rates
- Adjust as needed

### Testing Methodology

1. **Solo Test**: Can one skilled player progress?
2. **Small Group**: Is it fun with 2-3 players?
3. **Full Raid**: Does it scale to 10+ players?
4. **Undergeared**: Is it possible with minimum gear?
5. **Overgeared**: Does it remain challenging?

---

## Integration with Other Systems

### Ability System
Mechanics complement base abilities:
- Base abilities = consistent threat
- Mechanics = special moments
- Together = complete encounter

### Reward Scaling
Match rewards to difficulty:
- More mechanics = better loot
- Longer fights = more rewards
- First kills = special items

### Scheduling
Consider mechanic complexity when scheduling:
- Simple bosses = frequent spawns
- Complex bosses = rare spawns
- Event bosses = limited time

---

## Conclusion

The Boss Mechanics System transforms V Rising bosses from simple stat checks into engaging encounters that challenge players to think, adapt, and cooperate. By carefully combining different mechanics and triggers, you can create unique experiences that keep your server's content fresh and exciting.

Remember: Start simple, test thoroughly, and gradually increase complexity based on your community's skill level and preferences.

---

[← Back to Commands](commands/index.md)