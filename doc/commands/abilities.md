# Ability System Commands

The ability system allows you to customize boss abilities by copying them from existing VBloods. Mix and match abilities to create unique encounters while keeping the boss's original appearance.

## Table of Contents
- [Understanding the System](#understanding-the-system)
- [List Available VBloods](#list-available-vbloods)
- [View Ability Information](#view-ability-information)
- [Configure Boss Abilities](#configure-boss-abilities)
- [Test Compatibility](#test-compatibility)
- [Suggest Compatible Abilities](#suggest-compatible-abilities)
- [Advanced Slot Management](#advanced-slot-management)
- [Best Practices](#best-practices)

---

## Understanding the System

### How It Works
1. **Original Appearance**: Boss keeps its original model and animations
2. **Borrowed Abilities**: Abilities are copied from other VBloods
3. **Slot System**: Each ability occupies a numbered slot (0-36+)
4. **Compatibility**: Some combinations work better than others

### Compatibility Levels
- `[PERFECT]` - Ideal match, no issues expected
- `[GOOD]` - Works well, minor visual glitches possible
- `[WARNING]` - Functional but may have animation issues
- `[INCOMPATIBLE]` - Won't work (e.g., flight abilities on non-flying boss)

---

## List Available VBloods

Shows all VBloods you can copy abilities from.

### Syntax
```
.bb ability-list [filter]
```

### Examples
```bash
# Show all VBloods
.bb ability-list

# Filter by name
.bb ability-list wolf
.bb ability-list frost
.bb ability-list vampire
```

### Output Example
```
🩸 Known VBloods (92 of 92):
────────────────────────────
├─ Alpha the White Wolf: -1905691330
├─ Azariel the Sunbringer: -2013903325
├─ Beatrice the Tailor: -1449631170
├─ Christina the Sun Priestess: -1968372384
├─ Clive the Firestarter: -1391546313
└─ ... and 87 more

📝 Examples:
   .bb ability-info "alpha"
   .bb ability-list wolf
```

---

## View Ability Information

Shows detailed information about a VBlood's abilities.

### Syntax
```
.bb ability-info <VBloodName> [SlotIndex]
```

### Parameters
- `VBloodName` - Name of the VBlood (partial names work)
- `SlotIndex` - Optional, show specific ability slot only

### Examples
```bash
# Show all abilities
.bb ability-info "Alpha the White Wolf"
.bb ability-info "Solarus"

# Show specific slot
.bb ability-info "Dracula" 0
.bb ability-info "Gorecrusher" 3
```

### Output Example
```
🩸 Alpha the White Wolf (Level 16)
├─ Category: Beast, Wolf
├─ Can Fly: No
└─ Abilities: 8

🎯 Slot 0: BasicAttack
├─ GUID: 1120243216
├─ Cast: 0.8s (post: 0.5s)
├─ Cooldown: 0s
├─ Combo: 2 hits
├─ Spawns: 1 projectile(s)
└─ Behavior: None

🎯 Slot 2: Movement
├─ GUID: -587451309
├─ Cast: 0.35s (post: 0.1s)
├─ Cooldown: 8s
├─ Spawns: 2 projectile(s)
└─ Behavior: Dash
```

---

## Configure Boss Abilities

Copy abilities from VBloods to your boss.

### Syntax
```
.bb ability-slot <BossName> <SourceVBlood> <SlotIndex> <Enable> [Description]
```

### Parameters
- `BossName` - Your configured boss
- `SourceVBlood` - VBlood to copy from (partial names work)
- `SlotIndex` - Which ability slot to use (0-36+)
- `Enable` - `true` to apply, `false` to remove
- `Description` - Optional note about the ability

### Examples
```bash
# Add abilities
.bb ability-slot "Fire Dragon" "Solarus" 0 true "Holy projectile"
.bb ability-slot "Fire Dragon" "Gorecrusher" 3 true "Whirlwind attack"
.bb ability-slot "Fire Dragon" "Alpha" 2 true "Dash attack"

# Remove abilities (restore original)
.bb ability-slot "Fire Dragon" "Solarus" 0 false
```

### Visual Feedback
```
[GOOD] Configured slot 0 with ability from Solarus the Immaculate

Warnings:
  - Model type mismatch: Beast using Human ability
```

---

## Test Compatibility

Test if an ability will work before applying it.

### Syntax
```
.bb ability-test <BossName> <SourceVBlood> <SlotIndex>
```

### Examples
```bash
.bb ability-test "Fire Dragon" "Morian" 2
.bb ability-test "Shadow Beast" "Dracula" 0
```

### Output Example
```
🧪 Compatibility Test:
├─ Boss: Ancient Dragon
├─ Source: Morian the Stormwing Matriarch
├─ Slot: 2
└─ Result: [WARNING] Warning

⚠️ Warnings:
├─ Flight ability on non-flying boss - may cause hovering
├─ Model size mismatch - animations may scale oddly

✅ This ability can be used with command:
.bb ability-slot "Ancient Dragon" "Morian" 2 true "Test ability"
```

---

## Suggest Compatible Abilities

Get AI-assisted suggestions for compatible abilities.

### Syntax
```
.bb ability-suggest <BossName> [Category]
```

### Parameters
- `BossName` - Your configured boss
- `Category` - Optional filter: BasicAttack, Movement, Special, Ultimate

### Examples
```bash
# All suggestions
.bb ability-suggest "Fire Dragon"

# Only movement abilities
.bb ability-suggest "Fire Dragon" Movement

# Only special abilities
.bb ability-suggest "Shadow Beast" Special
```

### Output Example
```
🎯 Compatible abilities for Fire Dragon:
├─ Boss Category: Beast
├─ Can Fly: No
└─ Searching for compatible abilities...

Found 156 compatible abilities:
[PERFECT] Alpha the White Wolf - Slot 0 (BasicAttack)
   └─ Cast: 0.8s - Bite attack combo
[PERFECT] Goreswine the Ravager - Slot 2 (Movement)
   └─ Cast: 0.3s - Charge forward
[GOOD] Solarus the Immaculate - Slot 0 (BasicAttack)
   └─ Cast: 1.4s - Holy projectile barrage
... and 153 more
```

---

## Advanced Slot Management

### Set Specific Slot
```
.bb ability-slot-set <BossName> <SlotIndex> <Enabled> [Description]
```

Modify an existing ability swap configuration.

### Remove Slot
```
.bb ability-slot-remove <BossName> <SlotIndex>
```

### List All Slots
```
.bb ability-slot-list <BossName>
```

### Clear All Slots
```
.bb ability-slot-clear <BossName>
```

---

## Best Practices

### Ability Selection

#### Match Categories
Abilities work best when categories align:
- Beast boss + Beast abilities = Perfect
- Humanoid boss + Humanoid abilities = Good
- Mixed categories = May have issues

#### Consider Animation Types
- **Melee attacks**: Work on most bosses
- **Projectiles**: Generally compatible
- **Movement**: Check if dash/teleport fits model
- **Channeled**: May lock boss in place

### Building a Boss Kit

#### Balanced Loadout Example
```bash
# Slot 0-1: Basic attacks (fast, low cooldown)
.bb ability-slot "Boss" "Alpha" 0 true "Quick bite"
.bb ability-slot "Boss" "Alpha" 1 true "Claw swipe"

# Slot 2-3: Movement/gap closers
.bb ability-slot "Boss" "Goreswine" 2 true "Charge"
.bb ability-slot "Boss" "Jade" 3 true "Backstep"

# Slot 4-6: Main abilities
.bb ability-slot "Boss" "Vincent" 4 true "Frost nova"
.bb ability-slot "Boss" "Christina" 5 true "Holy beam"
.bb ability-slot "Boss" "Gorecrusher" 6 true "Whirlwind"

# Slot 7+: Ultimate/special
.bb ability-slot "Boss" "Solarus" 7 true "Divine shield"
```

### Common Combinations

#### Tank Boss
```bash
# Defensive abilities
.bb ability-slot "Tank" "Gorecrusher" 0 true "Heavy swing"
.bb ability-slot "Tank" "Errol" 3 true "Stone skin"
.bb ability-slot "Tank" "Quincey" 4 true "Shield wall"
```

#### Caster Boss
```bash
# Ranged magical abilities
.bb ability-slot "Caster" "Vincent" 0 true "Frost bolt"
.bb ability-slot "Caster" "Christina" 2 true "Holy projectile"
.bb ability-slot "Caster" "Azariel" 4 true "Solar beam"
```

#### Assassin Boss
```bash
# High mobility and burst
.bb ability-slot "Assassin" "Jade" 0 true "Quick strikes"
.bb ability-slot "Assassin" "Octavian" 2 true "Shadowstep"
.bb ability-slot "Assassin" "Styx" 3 true "Execute"
```

### Testing Your Configuration

1. **Start Simple**
   ```bash
   # Add one ability at a time
   .bb ability-slot "Boss" "Alpha" 0 true
   .bb debug test "Boss"
   ```

2. **Check Visual Issues**
   - Watch for animation glitches
   - Note any floating/clipping
   - Test all ability combinations

3. **Balance Testing**
   - Ensure cooldowns make sense
   - Check damage scaling
   - Verify movement abilities work

### Troubleshooting

#### "VBlood not found"
- Use partial names: "alpha" instead of full name
- Check spelling with `.bb ability-list`
- Names are case-insensitive

#### "Ability is incompatible"
- Some abilities require specific features
- Flight abilities need flying boss
- Check compatibility first with `.bb ability-test`

#### Visual Glitches
- Model mismatches can cause issues
- Try abilities from similar-sized VBloods
- Some glitches are cosmetic only

### Performance Tips

#### Limit Ability Count
- Too many abilities can overwhelm players
- 6-10 active abilities is usually enough
- Leave some slots empty for pacing

#### Cooldown Management
- Mix short and long cooldowns
- Avoid all abilities triggering at once
- Consider boss phases for ability activation

---

## Popular VBlood Sources

### For Basic Attacks
- **Alpha the White Wolf** - Fast melee combos
- **Goreswine the Ravager** - Heavy hits
- **Rufus the Foreman** - Balanced attacks

### For Movement
- **Jade the Vampire Hunter** - Agile movement
- **Octavian the Militia Captain** - Tactical positioning
- **Beatrice the Tailor** - Teleportation

### For AoE Damage
- **Vincent the Frostbringer** - Frost explosions
- **Azariel the Sunbringer** - Solar bombardment
- **Gorecrusher the Behemoth** - Ground slams

### For Defensive
- **Solarus the Immaculate** - Divine shields
- **Errol the Stonebreaker** - Stone armor
- **Quincey the Bandit King** - Evasion

---

[← Back to Index](index.md) | [Next: Mechanic Commands →](mechanics.md)