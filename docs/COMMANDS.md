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
.bb create "Ancient Dracula" -1905691330 90 2 1800
.bb create "Shadow Beast" -1905691330 75 1.5 3600
.bb create "Test Boss" -1905691330 50 1 900
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

### `.bb set hour`
Set automatic spawn time for a boss.

**Syntax:** `.bb set hour <BossName> <Time>`

**Time Format:** 24-hour format `HH:MM`

**Examples:**
```bash
.bb set hour "Ancient Dracula" 20:00    # 8:00 PM
.bb set hour "Shadow Beast" 14:30       # 2:30 PM
.bb set hour "Test Boss" 09:15          # 9:15 AM
```

## 🎁 Reward System

### `.bb items add`
Add items to boss drop table.

**Syntax:** `.bb items add <BossName> <ItemName> <ItemPrefabID> <Stack> <Chance>`

**Parameters:**
- `ItemName` - Display name for the item
- `ItemPrefabID` - V Rising item PrefabGUID
- `Stack` - Quantity to drop
- `Chance` - Drop chance percentage (1-100)

**Examples:**
```bash
.bb items add "Ancient Dracula" "Blood Essence" 1055853475 50 100
.bb items add "Ancient Dracula" "Legendary Gem" 429052660 5 25
.bb items add "Shadow Beast" "Iron Ingot" -257494203 20 75
```

### `.bb items remove`
Remove items from boss drop table.

**Syntax:** `.bb items remove <BossName> <ItemName>`

**Examples:**
```bash
.bb items remove "Ancient Dracula" "Blood Essence"
.bb items remove "Shadow Beast" "Iron Ingot"
```

### `.bb items list`
Display all items for a specific boss.

**Syntax:** `.bb items list <BossName>`

**Output:**
```
Items for Boss: Ancient Dracula
- Blood Essence (50x, 100% chance)
- Legendary Gem (5x, 25% chance)
```

## 🎮 Boss Control

### `.bb start`
Manually spawn a boss at its configured location.

**Syntax:** `.bb start <BossName>`

**Examples:**
```bash
.bb start "Ancient Dracula"
.bb start "Test Boss"
```

### `.bb test`
Spawn a boss at your current location with 1-minute timer.

**Syntax:** `.bb test <BossName>`

**Examples:**
```bash
.bb test "Ancient Dracula"
.bb test "Shadow Beast"
```

**Use cases:**
- Testing boss configurations
- Checking server time synchronization
- Quick boss encounters

## 🗺️ Map & Icons

### `.bb clearicon`
Remove map icon for a specific boss.

**Syntax:** `.bb clearicon <BossName>`

**Examples:**
```bash
.bb clearicon "Ancient Dracula"
.bb clearicon "Shadow Beast"
```

### `.bb clearallicons`
Remove all BloodyBoss icons from the map.

**Syntax:** `.bb clearallicons`

**Use cases:**
- Fix stuck icons after boss despawn
- Clean up after configuration changes
- Reset map state

## 🔄 System Management

### `.bb reload`
Reload boss database from file.

**Syntax:** `.bb reload`

**Use cases:**
- Apply manual JSON edits
- Refresh configuration after changes
- Restore from backup

**Note:** Only reloads boss data, not mod configuration.

## ✨ Advanced Commands (v2.1.0)

### `.bb despawn`
**Admin only** - Immediately despawn an active boss.

**Syntax:** `.bb despawn <BossName>`

**Examples:**
```bash
.bb despawn "Ancient Dracula"
.bb despawn "Shadow Beast"
```

**Features:**
- Sends announcement to all players
- Clears boss entity and map icon
- Resets boss state in database
- Logs action for administrators

### `.bb pause`
**Admin only** - Pause boss spawn timer.

**Syntax:** `.bb pause <BossName>`

**Examples:**
```bash
.bb pause "Ancient Dracula"
.bb pause "Shadow Beast"
```

**Use cases:**
- Server maintenance during boss time
- Event coordination
- Temporary boss disabling

### `.bb resume`
**Admin only** - Resume paused boss timer.

**Syntax:** `.bb resume <BossName>`

**Examples:**
```bash
.bb resume "Ancient Dracula" 
.bb resume "Shadow Beast"
```

**Shows:** Duration boss was paused

### `.bb status`
**Admin only** - Display detailed boss information.

**Syntax:** `.bb status <BossName>`

**Example output:**
```
📊 Status for Boss 'Ancient Dracula':
├─ Currently Spawned: ✅ Yes
├─ Timer Status: ▶️ Running
├─ Spawn Time: 20:00
├─ Despawn Time: 20:30:00
├─ Level: 90 | Multiplier: 2x
├─ Lifetime: 1800s (30min)
├─ Position: (100.5, 50.2, -200.1)
├─ Current Killers: 3
├─ Consecutive Spawns: 2
├─ Difficulty Multiplier: 1.20x
├─ Health: 85000/120000 (70.8%)
├─ Items: 5 configured
└─ Last Spawn: 2024-01-15 20:00:15
```

### `.bb debug`
**Admin only** - Display technical debug information.

**Syntax:** `.bb debug <BossName>`

**Example output:**
```
🔧 Debug Info for 'Ancient Dracula':
├─ Asset Name: CHAR_VBlood_Dracula_VBlood
├─ PrefabGUID: -1905691330
├─ Name Hash: AncientDracula123
├─ VBlood First Kill: false
├─ Entity ID: 12345.1
├─ Has VBloodUnit: true
├─ Has Health: true
├─ Has UnitStats: true
├─ Physical Power: 150.5
├─ Spell Power: 200.0
├─ Physical Resistance: 75.2
└─ Database Index: 0
```

### `.bb simulate`
**Admin only** - Simulate boss death for testing.

**Syntax:** `.bb simulate <BossName> [KillerName]`

**Examples:**
```bash
.bb simulate "Ancient Dracula"
.bb simulate "Ancient Dracula" "TestPlayer"
```

**Features:**
- Tests death mechanics without killing boss
- Triggers item drops and announcements
- Shows what would happen on real death
- Doesn't affect boss spawn state

### `.bb resetkills`
**Admin only** - Clear boss kill tracking.

**Syntax:** `.bb resetkills <BossName>`

**Examples:**
```bash
.bb resetkills "Ancient Dracula"
.bb resetkills "Shadow Beast"
```

**Clears:**
- Player kill list
- VBlood first kill flag
- Kill participation tracking

### `.bb forcedrop`
**Admin only** - Force boss to drop items.

**Syntax:** `.bb forcedrop <BossName> [PlayerName]`

**Examples:**
```bash
.bb forcedrop "Ancient Dracula"
.bb forcedrop "Ancient Dracula" "SpecificPlayer"
```

**Features:**
- Forces item drop calculation
- Uses configured drop rates
- Shows drop results
- Adds player to killers if needed

### `.bb teleport`
**Configurable** - Teleport to boss location.

**Syntax:** `.bb teleport <BossName>`

**Examples:**
```bash
.bb teleport "Ancient Dracula"
.bb teleport "Shadow Beast"
```

**Configuration dependent:**
- May require admin permissions
- May have cooldown restrictions
- May require item costs
- May require boss to be alive/active

**Error messages:**
- `🚫 Teleport command is restricted to administrators only`
- `⏰ Teleport on cooldown for 25 seconds`
- `💰 Requires 5x Blood Essence (you have 2)`
- `💀 Boss 'Ancient Dracula' is dead`

## 🧪 Modular Ability System Commands (v2.1.0)

**NEW!** Mix and match abilities from different VBloods while preserving original appearance.

### `.bb ability-slot-set`
**Admin only** - Configure individual ability slot.

**Syntax:** `.bb ability-slot-set <BossName> <SlotName> <SourcePrefabGUID> <AbilityIndex> [Enabled] [Description]`

**Parameters:**
- `SlotName` - Custom name for this ability slot
- `SourcePrefabGUID` - VBlood to copy ability from
- `AbilityIndex` - Which ability index to use (0-36)
- `Enabled` - true/false (default: true)
- `Description` - Optional description

**Examples:**
```bash
.bb ability-slot-set "HybridBoss" "primary" -327335305 0 true "Dracula shockwave"
.bb ability-slot-set "HybridBoss" "spell1" -99012450 2 true "Christina AoE"
.bb ability-slot-set "HybridBoss" "mobility" -1905691330 6 true "Wolf pounce"
```

### `.bb ability-slot-remove`
**Admin only** - Remove specific ability slot.

**Syntax:** `.bb ability-slot-remove <BossName> <SlotName>`

**Examples:**
```bash
.bb ability-slot-remove "HybridBoss" "primary"
.bb ability-slot-remove "HybridBoss" "spell1"
```

### `.bb ability-slot-toggle`
**Admin only** - Enable/disable specific ability slot.

**Syntax:** `.bb ability-slot-toggle <BossName> <SlotName>`

**Examples:**
```bash
.bb ability-slot-toggle "HybridBoss" "primary"    # Disables if enabled
.bb ability-slot-toggle "HybridBoss" "spell1"     # Enables if disabled
```

### `.bb ability-slot-clear`
**Admin only** - Remove all custom ability slots from boss.

**Syntax:** `.bb ability-slot-clear <BossName>`

**Examples:**
```bash
.bb ability-slot-clear "HybridBoss"
.bb ability-slot-clear "TestBoss"
```

### `.bb ability-slot-list`
**Admin only** - List all configured ability slots for boss.

**Syntax:** `.bb ability-slot-list <BossName>`

**Example output:**
```
📋 Custom ability slots for boss 'HybridBoss':
────────────────────────────────────────────────
├─ Slot 'primary': ✅
│  ├─ Source: Dracula (-327335305)
│  ├─ Ability Index: 0
│  └─ Description: Dracula shockwave slash
├─ Slot 'spell1': ✅
│  ├─ Source: Christina (-99012450)
│  ├─ Ability Index: 2
│  └─ Description: Christina AoE magic
└─ Total: 2 slots (2 enabled)
```

### `.bb ability-preset`
**Admin only** - Apply predefined ability combination.

**Syntax:** `.bb ability-preset <BossName> <PresetName>`

**Available presets:**
- `dracula-mix` - Dracula melee + Vincent frost
- `frost-warrior` - Tristan knight + Vincent ice
- `spell-caster` - Christina holy + Dracula dark

**Examples:**
```bash
.bb ability-preset "HybridBoss" "dracula-mix"
.bb ability-preset "MageBoss" "spell-caster"
.bb ability-preset "KnightBoss" "frost-warrior"
```

### `.bb ability-discover`
**Admin only** - Discover all VBloods currently in the game world.

**Syntax:** `.bb ability-discover`

**Example output:**
```
🔍 Discovering all VBloods in the game world...
Found 25 VBlood entities in game world:
├─ CHAR_VBlood_Dracula_VBlood
│  ├─ PrefabGUID: -327335305
│  ├─ AbilityBar: ✅
│  └─ Abilities: 37 slots
├─ CHAR_Undead_BishopOfShadows_VBlood
│  ├─ PrefabGUID: 939467639
│  ├─ AbilityBar: ✅
│  └─ Abilities: 6 slots
└─ Total unique VBloods discovered: 25
```

**Generates file:** `Discovered_VBloods_[timestamp].txt`

### `.bb ability-inspect`
**Admin only** - Inspect detailed abilities of specific VBlood.

**Syntax:** `.bb ability-inspect <SourcePrefabGUID>`

**Examples:**
```bash
.bb ability-inspect -327335305    # Dracula abilities
.bb ability-inspect 939467639     # Vincent abilities
.bb ability-inspect -99012450     # Christina abilities
```

**Example output:**
```
🔍 Detailed abilities for Dracula (PrefabGUID: -327335305):
────────────────────────────────────────────────
✅ Has AbilityBar_Shared component
✅ Found 37 ability groups:

├─ Index 0: Primary Attack - 'Vampire Dracula ShockwaveSlash Group'
├─ Index 1: Secondary Attack - 'Vampire Dracula DownSwing group'
├─ Index 2: Special/Spell - 'Vampire Dracula SwordThrow group'
├─ Index 3: Ultimate/Spell - 'Vampire Dracula WolfAtk group'
├─ Index 4: Combat Ability - 'Vampire Dracula WolfLeap EnterTravel Group'
└─ ... and 32 more abilities

💡 Usage examples:
   .bb ability-slot-set "YourBoss" "slot1" -327335305 0 true "Dracula ability 1"
   .bb ability-slot-set "YourBoss" "slot2" -327335305 1 true "Dracula ability 2"
```

### `.bb ability-export-all`
**Admin only** - Generate comprehensive abilities documentation.

**Syntax:** `.bb ability-export-all`

**Features:**
- Scans all available VBloods
- Documents each ability with names and classifications
- Generates markdown documentation file
- Includes usage examples for each VBlood

**Generates file:** `VBlood_Abilities_Documentation_[timestamp].md`

**Example output:**
```
📄 Documentation exported successfully!
└─ File: VBlood_Abilities_Documentation_20241215_143052.md
└─ Path: /BepInEx/config/BloodyBoss/VBlood_Abilities_Documentation_20241215_143052.md
└─ Total VBloods documented: 25
```

## 🧪 Modular Ability Examples

### Basic Hybrid Setup
```bash
# Create boss with Vincent appearance
.bb create "HybridTest" 939467639 350 6 3000

# Set location
.bb set location "HybridTest"

# Mix abilities from different VBloods
.bb ability-slot-set "HybridTest" "primary" -327335305 0 true "Dracula shockwave"
.bb ability-slot-set "HybridTest" "secondary" 1112948824 1 true "Tristan charge"
.bb ability-slot-set "HybridTest" "spell1" -99012450 2 true "Christina AoE"
.bb ability-slot-set "HybridTest" "spell2" -99012450 3 true "Christina minions"
.bb ability-slot-set "HybridTest" "mobility" -1905691330 6 true "Wolf pounce"

# Start the encounter
.bb start "HybridTest"
```

### Using Presets for Quick Setup
```bash
# Create multiple themed bosses quickly
.bb create "DarkMage" 939467639 400 8 2400
.bb ability-preset "DarkMage" "dracula-mix"

.bb create "HolyKnight" 1112948824 350 6 2400  
.bb ability-preset "HolyKnight" "spell-caster"

.bb create "FrostWarrior" -327335305 375 7 2400
.bb ability-preset "FrostWarrior" "frost-warrior"
```

### Discovery and Documentation Workflow
```bash
# Step 1: Discover what's available
.bb ability-discover

# Step 2: Inspect specific VBloods for abilities
.bb ability-inspect -327335305    # Check Dracula's 37 abilities
.bb ability-inspect 939467639     # Check Vincent's 6 abilities

# Step 3: Generate complete documentation
.bb ability-export-all

# Step 4: Configure based on findings
.bb ability-slot-set "CustomBoss" "ultimate" -327335305 21 true "Blood shower"
```

### Troubleshooting Modular Abilities
```bash
# Check current ability configuration
.bb ability-slot-list "ProblematicBoss"

# Disable problematic abilities temporarily  
.bb ability-slot-toggle "ProblematicBoss" "broken_ability"

# Remove and reconfigure
.bb ability-slot-remove "ProblematicBoss" "bad_slot"
.bb ability-slot-set "ProblematicBoss" "fixed_slot" -99012450 1 true "Working ability"

# Clear all and start fresh if needed
.bb ability-slot-clear "ProblematicBoss"

# Force respawn to apply changes
.bb despawn "ProblematicBoss"
.bb start "ProblematicBoss"
```

## 📊 Command Usage Examples

### Setting Up a Weekly Boss Event
```bash
# Create the boss
.bb create "Weekly Titan" -1905691330 100 3 7200

# Set location (go to spawn point first)
.bb set location "Weekly Titan"

# Schedule for Saturday 8 PM
.bb set hour "Weekly Titan" 20:00

# Add premium rewards
.bb items add "Weekly Titan" "Legendary Essence" 1055853475 100 100
.bb items add "Weekly Titan" "Rare Material Box" 429052660 10 75
.bb items add "Weekly Titan" "Epic Weapon" -257494203 1 25

# Test the setup
.bb test "Weekly Titan"
```

### Emergency Boss Management
```bash
# Check what's happening
.bb status "Problem Boss"

# Get technical details
.bb debug "Problem Boss"

# Force cleanup if needed
.bb despawn "Problem Boss"
.bb clearicon "Problem Boss"

# Reset for fresh start
.bb resetkills "Problem Boss"
```

### Daily Maintenance Routine
```bash
# List all bosses
.bb list

# Clear any stuck icons
.bb clearallicons

# Check status of each boss
.bb status "Boss1"
.bb status "Boss2"

# Reload configuration if needed
.bb reload
```

## 🚫 Error Messages & Solutions

### Common Errors

**`Boss with name 'BossName' does not exist`**
- Solution: Check boss name spelling and quotes
- Use `.bb list` to see exact names

**`The PrefabGUID entered does not correspond to a VBlood Unit`**
- Solution: Verify PrefabGUID is correct
- Check [V Rising Prefabs Database](https://wiki.vrisingmods.com/prefabs/)

**`Boss 'BossName' is not currently spawned`**
- Solution: Use `.bb start` to spawn boss first
- Check spawn time hasn't passed

**`Boss 'BossName' is already paused`**
- Solution: Use `.bb resume` instead of `.bb pause`

**`Teleport command is disabled`**
- Solution: Admin needs to enable in configuration
- Check `[Teleport] Enable = true`

## 🔐 Permission Levels

### Admin Only Commands
```bash
.bb create, .bb remove, .bb set location, .bb set hour
.bb items add, .bb items remove, .bb start, .bb test
.bb clearicon, .bb clearallicons, .bb reload
.bb despawn, .bb pause, .bb resume, .bb status
.bb debug, .bb simulate, .bb resetkills, .bb forcedrop
```

### Player Commands (if enabled)
```bash
.bb list          # Always available
.bb items list    # Always available  
.bb teleport      # If configured for players
```

## 📚 Next Steps

- ⚙️ [Configuration Guide](CONFIGURATION.md) - Configure command permissions
- 🚀 [Advanced Features](ADVANCED-FEATURES.md) - Learn about scaling and phases
- 📝 [Examples](EXAMPLES.md) - Real-world command sequences
- 🛠️ [Troubleshooting](TROUBLESHOOTING.md) - Solve command issues

---

*Command help needed? Join the [V Rising Mod Community Discord](https://discord.gg/vrisingmods)!*