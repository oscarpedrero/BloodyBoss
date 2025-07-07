# 🎮 Commands Reference

Complete reference for all BloodyBoss v2.1.0 commands. All commands use the prefix `.bb` and most require admin permissions.

## 📝 Command Syntax Notes

- **Names with spaces** must be enclosed in quotes: `"Alpha Wolf"`
- **Case sensitive** - use exact boss/item names
- **Admin only** unless specified otherwise
- **Server console** - all commands work from server console

## 🏗️ Boss Creation & Management

### `.bb create`
Create a new boss configuration.

**Syntax:** `.bb create <BossName> <PrefabGUID> <Level> <Multiplier> <LifetimeSeconds>`

**Parameters:**
- `BossName` - Display name for the boss
- `PrefabGUID` - V Rising entity PrefabGUID 
- `Level` - Boss level (1-100)
- `Multiplier` - Base health multiplier
- `LifetimeSeconds` - How long boss stays spawned

**Examples:**
```bash
.bb create "Ancient Dracula" -327335305 90 2 1800
.bb create "Shadow Beast" -1905691330 75 1.5 3600
.bb create "Test Boss" 24378719 50 1 900
```

### `.bb remove`
Remove a boss from the configuration.

**Syntax:** `.bb remove <BossName>`

**Examples:**
```bash
.bb remove "Ancient Dracula"
.bb remove "Test Boss"
```

### `.bb list`
Display all configured bosses.

**Syntax:** `.bb list`

**Output:**
```
Boss List
----------------------------
--
Boss Ancient Dracula
--
Boss Shadow Beast
--
----------------------------
```

## 📍 Location & Timing

### `.bb set location`
Set boss spawn location to your current position.

**Syntax:** `.bb set location <BossName>`

**Examples:**
```bash
.bb set location "Ancient Dracula"
.bb set location "Shadow Beast"
```

**Tips:**
- Stand exactly where you want the boss to spawn
- Consider terrain and player accessibility
- Avoid spawning inside walls or unreachable areas

### `.bb hour`
Set automatic spawn times for a boss (multiple hours supported).

**Syntax:** `.bb hour <BossName> <Hour1> [Hour2] [Hour3] ...`

**Hour Format:** 0-23 (24-hour format)

**Examples:**
```bash
.bb hour "Ancient Dracula" 20              # Spawns at 8:00 PM
.bb hour "Shadow Beast" 9 15 21            # Spawns at 9:00 AM, 3:00 PM, 9:00 PM
.bb hour "Test Boss" 0 6 12 18             # Every 6 hours
```

## 🎁 Reward System

### `.bb items`
Add items to boss drop table.

**Syntax:** `.bb items <BossName> <ItemPrefabID> <Stack> <Chance>`

**Parameters:**
- `ItemPrefabID` - V Rising item PrefabGUID
- `Stack` - Quantity to drop
- `Chance` - Drop chance percentage (1-100)

**Examples:**
```bash
.bb items "Ancient Dracula" 862477668 50 100      # Pristine Leather x50, 100% chance
.bb items "Ancient Dracula" -257494203 10 50      # Greater Blood Essence x10, 50% chance
.bb items "Shadow Beast" -1531666018 5 25          # Scourgestone x5, 25% chance
```

### `.bb remove-item`
Remove specific item from boss drop table.

**Syntax:** `.bb remove-item <BossName> <ItemPrefabID>`

**Examples:**
```bash
.bb remove-item "Ancient Dracula" 862477668
.bb remove-item "Shadow Beast" -257494203
```

## 🎮 Boss Control

### `.bb on`
Enable automatic spawning for a boss.

**Syntax:** `.bb on <BossName>`

**Examples:**
```bash
.bb on "Ancient Dracula"
.bb on "Test Boss"
```

### `.bb off`
Disable automatic spawning for a boss.

**Syntax:** `.bb off <BossName>`

**Examples:**
```bash
.bb off "Ancient Dracula"
.bb off "Test Boss"
```

### `.bb test`
Spawn a boss at configured location for testing.

**Syntax:** `.bb test <BossName>`

**Examples:**
```bash
.bb test "Ancient Dracula"
.bb test "Shadow Beast"
```

**Features:**
- Spawns at configured location
- Full boss duration
- Tests all mechanics

### `.bb despawn`
Immediately despawn an active boss.

**Syntax:** `.bb despawn <BossName>`

**Examples:**
```bash
.bb despawn "Ancient Dracula"
.bb despawn "Shadow Beast"
```

## ⚡ VBlood Ability System (NEW!)

The ability system allows you to mix and match abilities from different VBloods while preserving the boss's original appearance. See [VBlood Abilities List](VBLOOD-ABILITIES.md) for all available VBloods and their abilities.

### `.bb ability-slot`
Configure individual ability slots for a boss.

**Syntax:** `.bb ability-slot <BossName> <SourceVBlood> <SlotIndex> <Enable> [Description]`

**Parameters:**
- `BossName` - Name of your configured boss
- `SourceVBlood` - Name of the VBlood to copy ability from (partial names work)
- `SlotIndex` - Which ability slot to configure (0-36+)
- `Enable` - `true` to apply ability, `false` to remove it
- `Description` - Optional description for the ability

**Examples:**
```bash
# Apply abilities
.bb ability-slot "Ancient Dracula" "Solarus" 0 true "Holy projectile attack"
.bb ability-slot "Ancient Dracula" "Beatrice" 2 true "Shadow slash"
.bb ability-slot "Ancient Dracula" "Gorecrusher" 3 true "Whirlwind attack"
.bb ability-slot "Ancient Dracula" "Frostmaw" 4 true "Frost barrage"

# Remove abilities (restore original)
.bb ability-slot "Ancient Dracula" "Solarus" 0 false
```

**Compatibility Notes:**
- Messages show compatibility level: [PERFECT], [GOOD], [WARNING], [INCOMPATIBLE]
- Warnings are logged but don't prevent ability usage
- Some visual glitches may occur with incompatible combinations

### `.bb ability-info`
Display detailed information about a VBlood's abilities.

**Syntax:** `.bb ability-info <VBloodName> [SlotIndex]`

**Examples:**
```bash
.bb ability-info "Dracula"              # Show all abilities
.bb ability-info "Solarus" 0            # Show specific ability slot
.bb ability-info "Frostmaw"             # Show all Frostmaw abilities
```

**Output Example:**
```
🩸 Solarus the Immaculate (Level 55)
├─ Category: Militia, Human, Humanoid, Paladin
├─ Can Fly: No
└─ Abilities: 4

🎯 Slot 0: BasicAttack
├─ GUID: 348884445
├─ Cast: 1.4s (post: 0.5s)
├─ Combo: 2 hits
├─ Spawns: 1 projectile(s)
└─ Behavior: None
```

### `.bb ability-suggest`
Get compatible ability suggestions for a boss.

**Syntax:** `.bb ability-suggest <BossName> [Category]`

**Examples:**
```bash
.bb ability-suggest "Ancient Dracula"              # All suggestions
.bb ability-suggest "Ancient Dracula" "Movement"   # Only movement abilities
.bb ability-suggest "Ancient Dracula" "Special"    # Only special abilities
```

**Output Example:**
```
🎯 Compatible abilities for Vampire Dracula:
├─ Boss Category: Vampire
├─ Can Fly: No
└─ Searching for compatible abilities...

Found 245 compatible abilities:
[PERFECT] Beatrice the Tailor - Slot 0 (BasicAttack)
   └─ Cast: 0.5s
[PERFECT] Vincent the Frostbringer - Slot 2 (Movement)
   └─ Cast: 0.3s
[GOOD] Gorecrusher the Behemoth - Slot 1 (Movement)
   └─ Cast: 0.3s
... and 242 more
```

### `.bb ability-test`
Test ability compatibility before applying.

**Syntax:** `.bb ability-test <BossName> <SourceVBlood> <SlotIndex>`

**Examples:**
```bash
.bb ability-test "Ancient Dracula" "Gorecrusher" 3
.bb ability-test "Shadow Beast" "Morian" 2
```

**Output Example:**
```
🧪 Compatibility Test:
├─ Boss: Vampire Dracula
├─ Source: Gorecrusher the Behemoth
├─ Slot: 3
└─ Result: [WARNING] Warning

⚠️ Warnings:
├─ Model type mismatch: Vampire using Beast ability - animations may glitch

✅ This ability can be used with command:
.bb ability-slot "Ancient Dracula" "custom_slot" -1936575244 3 true "Test ability"
```

### `.bb vblood-docs`
Export comprehensive VBlood abilities documentation.

**Syntax:** `.bb vblood-docs`

**Features:**
- Exports all 92 VBloods with their abilities
- Creates markdown file with examples
- Includes slot information and descriptions
- Generated in config folder

**Output:**
```
[PERFECT] VBlood documentation exported successfully!
File location: /BepInEx/config/BloodyBoss/VBlood_Abilities_Documentation.md
Total VBloods documented: 92
```

## 🔄 System Management

### `.bb status`
Display detailed boss information.

**Syntax:** `.bb status <BossName>`

**Example output:**
```
📊 Status for Boss 'Ancient Dracula':
├─ Currently Spawned: ✅ Yes
├─ Timer Status: ▶️ Running
├─ Spawn Time: 20
├─ Despawn Time: 20:30:00
├─ Level: 90 | Multiplier: 2x
├─ Lifetime: 1800s (30min)
├─ Position: (100.5, 50.2, -200.1)
├─ Current Killers: 3
├─ Consecutive Spawns: 2
├─ Difficulty Multiplier: 1.20x
```

### `.bb pause`
Pause boss spawn timer.

**Syntax:** `.bb pause <BossName>`

**Examples:**
```bash
.bb pause "Ancient Dracula"
.bb pause "Shadow Beast"
```

### `.bb resume`
Resume paused boss timer.

**Syntax:** `.bb resume <BossName>`

**Examples:**
```bash
.bb resume "Ancient Dracula" 
.bb resume "Shadow Beast"
```

## 📊 Complete Boss Setup Example

### Creating a Custom Dracula Boss
```bash
# 1. Create the boss using Vampire Dracula's GUID
.bb create "Epic Dracula" -327335305 100 5 1800

# 2. Configure spawn location (stand where you want it)
.bb set location "Epic Dracula"

# 3. Mix abilities from different VBloods
.bb ability-slot "Epic Dracula" "Solarus" 0 true "Holy projectile barrage"
.bb ability-slot "Epic Dracula" "Beatrice" 2 true "Shadow dash attack"
.bb ability-slot "Epic Dracula" "Gorecrusher" 3 true "Whirlwind devastation"
.bb ability-slot "Epic Dracula" "Frostmaw" 4 true "Frost projectile barrage"
.bb ability-slot "Epic Dracula" "Styx" 7 true "Shadow movement"
.bb ability-slot "Epic Dracula" "Vincent" 10 true "Ice storm channeling"
.bb ability-slot "Epic Dracula" "Christina" 14 true "Holy ring of protection"
.bb ability-slot "Epic Dracula" "Adam" 20 true "Chaos teleportation"

# 4. Configure rewards
.bb items "Epic Dracula" -257494203 10 100    # Greater Blood Essence x10, 100%
.bb items "Epic Dracula" 862477668 5 80       # Pristine Leather x5, 80%
.bb items "Epic Dracula" -1531666018 3 50     # Scourgestone x3, 50%
.bb items "Epic Dracula" -810805974 2 25      # Soul Shard x2, 25%

# 5. Set spawn schedule (every 3 hours)
.bb hour "Epic Dracula" 0 3 6 9 12 15 18 21

# 6. Enable automatic spawning
.bb on "Epic Dracula"

# 7. Test the setup
.bb test "Epic Dracula"
```

### Removing Abilities
```bash
# Remove specific abilities to restore originals
.bb ability-slot "Epic Dracula" "Solarus" 0 false
.bb ability-slot "Epic Dracula" "Beatrice" 2 false

# The boss will now use original abilities for those slots
```

## 🚫 Common Errors & Solutions

### Ability System Errors

**`VBlood 'Name' not found`**
- Use partial names: "Frostmaw" instead of "Frostmaw the Mountain Terror"
- Check spelling and capitalization
- Use `.bb vblood-docs` to see all available names

**`Ability is incompatible!`**
- Some abilities require specific features (flight, model type)
- Check compatibility with `.bb ability-test` first
- Read warnings in chat/log for details

**`Bandit Frostarrow doesn't have ability at slot X`**
- Not all VBloods have abilities in every slot
- Use `.bb ability-info "VBloodName"` to see available slots
- Try different slot numbers

### Boss Creation Errors

**`Boss with name 'BossName' does not exist`**
- Check exact spelling and quotes
- Use `.bb list` to see all boss names

**`The PrefabGUID entered does not correspond to a VBlood Unit`**
- Verify PrefabGUID is correct
- Use negative values for some VBloods (e.g., -327335305 for Dracula)
- Check [VBlood Abilities List](VBLOOD-ABILITIES.md) for correct GUIDs

## 📚 Additional Resources

- 📋 [VBlood Abilities List](VBLOOD-ABILITIES.md) - Complete list of all VBloods and their abilities
- ⚙️ [Configuration Guide](CONFIGURATION.md) - Configure mod settings
- 🚀 [Advanced Features](ADVANCED-FEATURES.md) - Dynamic scaling and phases
- 📝 [Examples](EXAMPLES.md) - More setup examples
- 🛠️ [Troubleshooting](TROUBLESHOOTING.md) - Solve common issues

---

*Need help? Join the [V Rising Mod Community Discord](https://discord.gg/vrisingmods)!*