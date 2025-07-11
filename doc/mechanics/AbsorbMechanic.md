# Absorb Mechanic Documentation

## Overview
The Absorb mechanic allows bosses to drain life force or steal protective shields from players within range. This mechanic includes cooperative gameplay elements where players must work together to minimize the impact.

## How It Works

### Absorption Types

#### 1. Health Absorption
- **Effect**: Drains health directly from players (never lethal - leaves at least 1 HP)
- **Boss gains**: 
  - Direct healing equal to health drained
  - Temporary health boost buff for 15 seconds
- **Visual**: Vampiric blood leech effect on players, healing aura on boss
- **Configurable**: Amount of health to drain per player

#### 2. Shield Absorption
- **Effect**: Removes ALL shield buffs from affected players
- **Boss gains**: 
  - Real damage-absorbing shield (AB_Blood_BloodRage_SpellMod_Buff_Shield)
  - Shield lasts for 20 seconds (fixed duration)
  - Absorbs a fixed amount of damage (determined by the buff)
- **Visual**: Holy shield break effect on players, protective barrier on boss
- **Note**: Shield strength and duration are NOT configurable - they're determined by the buff

#### 3. All (Combined)
- **Effect**: Combines both absorption types
- **Distribution**: 70% health drain, 30% shield steal
- **Boss gains**: Both healing and shield protection

### Absorption Modes

#### Instant Absorption
- Single burst of absorption when mechanic triggers
- Immediate transfer of resources
- Visual effects show the drain happening

#### Continuous Absorption  
- Drains resources over time (per second)
- Persistent visual aura on boss
- Players must escape the radius to stop draining
- Total drain = amount × duration

### Cooperative Mechanic - Minimum Players

⚠️ **IMPORTANT**: This mechanic punishes teams that don't cooperate!

When `min_players` is set, the mechanic requires a minimum number of players within the normal radius:
- **Enough players in range**: Normal absorption occurs
- **NOT enough players in range**: **GLOBAL PUNISHMENT** activates!

#### Global Punishment
- Affects ALL players within a much larger radius (`global_radius`)
- Damage/drain is multiplied by `global_multiplier` (default: 1.5x)
- Warning message alerts all players to the failed cooperation
- Encourages teams to stay together during boss fights

## Command Usage

```
.bb mechanic-config "<boss_name>" <phase> "absorb_type=<type> [parameters]"
```

### Parameters

#### Common Parameters
- **absorb_type**: Type of absorption (health, shield, all)
- **radius**: Absorption radius in meters (default: 20)
- **continuous**: Whether to drain over time (default: false)
- **announcement**: Custom warning message (optional)

#### Health-Specific Parameters  
- **amount**: Amount of health to absorb per player (default: 20)
- **duration**: Duration for continuous absorption in seconds (default: 5)

#### Shield-Specific Parameters
- **No additional parameters** - Shield strength and duration are fixed by the buff

#### Cooperative Parameters
- **min_players**: Minimum players required in range (default: 0 = disabled)
- **global_radius**: Punishment radius if min_players not met (default: 50)
- **global_multiplier**: Damage multiplier for global punishment (default: 1.5)

### Examples

**Basic health absorption:**
```
.bb mechanic-config "BossName" 0 "absorb_type=health amount=30 radius=15"
```

**Continuous health drain over 10 seconds:**
```
.bb mechanic-config "BossName" 0 "absorb_type=health amount=10 radius=20 duration=10 continuous=true"
```

**Shield steal (no amount needed):**
```
.bb mechanic-config "BossName" 0 "absorb_type=shield radius=15"
```

**Combined absorption:**
```
.bb mechanic-config "BossName" 0 "absorb_type=all amount=25 radius=15"
```

**Cooperative shield steal requiring 3 players:**
```
.bb mechanic-config "BossName" 0 "absorb_type=shield radius=12 min_players=3 global_radius=40"
```

**Full cooperative health drain with custom announcement:**
```
.bb mechanic-config "BossName" 0 "absorb_type=health amount=20 radius=10 min_players=2 global_radius=35 global_multiplier=2 announcement='⚠️ GATHER CLOSE OR SUFFER!'"
```

## Mechanic Strategy

### For Players
- **Health Absorption**: 
  - Keep your health topped up before the fight
  - Being at max range reduces damage taken
  - Healers should be ready to counter the drain
  
- **Shield Absorption**: 
  - Save your shield abilities for after this mechanic
  - The boss gains a real damage shield - switch to high DPS
  - Shield buffs will be completely removed, plan accordingly
  
- **Cooperative Mode**: 
  - Count your team! Make sure enough players are in range
  - Communicate positioning to avoid global punishment
  - Stack together when you see the warning

### For Server Admins
- Health drain amount should scale with server difficulty
- Shield absorption is powerful - use sparingly or at low HP thresholds
- Combine with other mechanics for complex encounters:
  - Shield steal → High damage phase (players vulnerable)
  - Health drain → Healing phase (sustain check)
- Min_players forces coordination - great for raid bosses

## Visual Indicators

- **Health drain**: Red vampiric leech streams from players to boss
- **Shield steal**: Golden holy break effect as shields shatter
- **Boss healing**: Green healing aura pulses around boss
- **Boss shield**: Blue-purple barrier surrounds the boss
- **Global punishment**: Extended range effects with warning text

## Technical Notes

### Health Absorption
- Non-lethal (minimum 1 HP retained)
- Healing is instant, not affected by healing reduction
- Boss can exceed max HP temporarily with boost buff

### Shield Absorption  
- Removes these known shield buffs:
  - AB_Blood_BloodRage_SpellMod_Buff_Shield (-1605515615)
  - AB_Chaos_PowerSurge_SpellMod_Buff_Shield (-1763296393)
  - Buff_BloodBuff_Scholar_Tier2_Shield (-231593873)
  - Buff_ChurchOfLight_Cleric_Intervene_Shield (514720473)
  - AB_Illusion_PhantomAegis_Buff_MagicSource (1433921398)
  - AB_Legion_Guardian_BlockBuff_Buff (-365991522)
- Boss gains AB_Blood_BloodRage_SpellMod_Buff_Shield
- Shield has fixed absorption amount and 20s duration

### Continuous Mode
- Ticks every second
- Each tick is a separate absorption event
- Total absorption = amount × duration × affected players

## Balance Considerations

- **Health absorption** is reliable sustain for the boss
- **Shield absorption** provides temporary but powerful defense
- **Combined mode** gives versatility but splits the effect
- Global punishment can quickly turn the tide of battle

## Common Issues

**No shield removed from player:**
- Player may not have any of the known shield buffs
- Add more shield buff GUIDs to the removal list if needed

**Boss shield seems weak:**
- The shield buff has a fixed absorption amount
- Consider using at higher HP thresholds for better impact

**Continuous drain too strong:**
- Reduce amount per second or duration
- Give players more time to react with announcements