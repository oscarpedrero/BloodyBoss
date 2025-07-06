# Modular Ability System Guide

The **Modular Ability System** is a revolutionary feature in BloodyBoss v2.1.0 that allows you to mix and match abilities from different VBloods while preserving their original visual appearance. Create unique boss encounters by combining the best abilities from across the V Rising universe.

## 🎯 Quick Start

### Basic Setup
```bash
# 1. Create a boss with desired appearance
.bb create "HybridBoss" 939467639 350 6 3000  # Vincent appearance

# 2. Mix abilities from different VBloods
.bb ability-slot-set "HybridBoss" "melee1" -327335305 0 true "Dracula shockwave"
.bb ability-slot-set "HybridBoss" "spell1" -99012450 2 true "Christina AoE"
.bb ability-slot-set "HybridBoss" "special" 1112948824 1 true "Tristan charge"

# 3. Start the encounter
.bb start "HybridBoss"
```

**Result:** A boss that looks like Vincent but fights with Dracula's shockwave attacks, Christina's holy magic, and Tristan's knight charges!

### Using Presets
```bash
# Apply pre-configured ability combinations
.bb ability-preset "YourBoss" "dracula-mix"    # Dracula + Vincent combo
.bb ability-preset "YourBoss" "frost-warrior"  # Tristan + Vincent combo
.bb ability-preset "YourBoss" "spell-caster"   # Christina + Dracula combo
```

## 🛠️ Command Reference

### Configuration Commands
| Command | Description | Example |
|---------|-------------|---------|
| `ability-slot-set` | Configure individual ability slot | `.bb ability-slot-set "Boss" "melee1" -327335305 0 true "Description"` |
| `ability-slot-remove` | Remove specific slot | `.bb ability-slot-remove "Boss" "melee1"` |
| `ability-slot-toggle` | Enable/disable slot | `.bb ability-slot-toggle "Boss" "melee1"` |
| `ability-slot-clear` | Clear all slots | `.bb ability-slot-clear "Boss"` |
| `ability-slot-list` | List configured slots | `.bb ability-slot-list "Boss"` |

### Discovery Commands
| Command | Description | Example |
|---------|-------------|---------|
| `ability-discover` | Discover all VBloods in game | `.bb ability-discover` |
| `ability-inspect` | Inspect specific VBlood abilities | `.bb ability-inspect -327335305` |
| `ability-export-all` | Generate complete documentation | `.bb ability-export-all` |

### Preset Commands
| Command | Description | Example |
|---------|-------------|---------|
| `ability-preset` | Apply predefined combination | `.bb ability-preset "Boss" "dracula-mix"` |

## 🏆 VBlood Abilities Database

### Top-Tier VBloods (Most Abilities)

#### **Dracula (PrefabGUID: -327335305)** - 37 abilities ⭐⭐⭐
The ultimate ability source with the most diverse skill set.

**Key Abilities:**
- **Index 0**: Primary Attack - `Vampire Dracula ShockwaveSlash Group` - Devastating area shockwave
- **Index 1**: Secondary Attack - `Vampire Dracula DownSwing group` - Powerful downward strike
- **Index 2**: Special/Spell - `Vampire Dracula SwordThrow group` - Ranged sword projectile
- **Index 3**: Ultimate/Spell - `Vampire Dracula WolfAtk group` - Transform and leap attack
- **Index 4**: Combat Ability - `Vampire Dracula WolfLeap EnterTravel Group` - Mobility skill
- **Index 5**: Combat Ability - `Vampire Dracula Feed Group` - Life drain ability
- **Index 10**: System Ability - `Vampire Dracula BloodBoltSwarm Group` - Blood projectile swarm
- **Index 15**: System Ability - `Vampire Dracula Evolve Group` - Boss transformation
- **Index 21**: System Ability - `Vampire Dracula BloodShower Group` - Area blood magic
- **Index 26**: System Ability - `Vampire Dracula SummonBats group` - Summon bat swarm

**Best for:** High-tier bosses, dark-themed encounters, complex multi-phase fights

#### **The Winged Horror (PrefabGUID: 1233988687)** - 21 abilities ⭐⭐
Perfect for aerial combat and horror-themed encounters.

**Best for:** Flying bosses, horror themes, mid-to-high tier encounters

#### **Cyril the Cursed Smith (PrefabGUID: -1347412392)** - 16 abilities ⭐⭐
Excellent for melee-focused bosses with crafting/smith themes.

**Best for:** Melee-heavy encounters, smith/forge themes, sustained combat

### Mid-Tier VBloods (Balanced Options)

#### **Christina the Sun Priestess (PrefabGUID: -99012450)** - 10 abilities ⭐
Holy magic specialist with healing and support abilities.

**Key Abilities:**
- **Index 0**: Primary Attack - `Nun Vblood MeleeAtk Group` - Holy melee strike
- **Index 1**: Secondary Attack - `Nun HARD HolyBoltSpray Group` - Holy projectile burst
- **Index 2**: Special/Spell - `Nun AoE Group` - Area of effect holy damage
- **Index 3**: Ultimate/Spell - `Nun SpawnMinions Group` - Summon holy minions
- **Index 4**: Combat Ability - `Nun Counter Group` - Defensive counter-attack
- **Index 5**: Combat Ability - `Nun HolyProj Group` - Holy projectile
- **Index 6**: Advanced Ability - `Nun HealingChannel Group Short` - Quick heal
- **Index 7**: Advanced Ability - `Nun HealingChannel Group` - Standard heal
- **Index 8**: Advanced Ability - `Nun HealingChannel Group Long` - Extended heal
- **Index 9**: Advanced Ability - `Nun HealCommand Group` - Command healing

**Best for:** Healing bosses, holy-themed encounters, support-style fights

#### **Alpha Wolf (PrefabGUID: -1905691330)** - 10 abilities ⭐
Beast-type abilities with mobility and pack tactics.

**Key Abilities:**
- **Index 2**: Special/Spell - `Wolf Boss DashAtk Single Group` - Single dash attack
- **Index 3**: Ultimate/Spell - `Wolf Boss DashAtk Double Group` - Double dash combo
- **Index 4**: Combat Ability - `Wolf Boss DashAtk Tripple Group` - Triple dash sequence
- **Index 5**: Combat Ability - `Wolf Boss Maul Hard Group` - Devastating maul attack
- **Index 6**: Advanced Ability - `Wolf Boss Pounce Group` - Leap attack
- **Index 7**: Advanced Ability - `Wolf Boss StepBack Group` - Evasive maneuver
- **Index 8**: Advanced Ability - `Wolf Boss Howl Group` - Pack buff/summon
- **Index 9**: Advanced Ability - `Wolf Boss SpeedBuff Group` - Speed enhancement

**Best for:** Beast-themed bosses, mobility-focused encounters, pack tactics

#### **Tristan the Vampire Knight (PrefabGUID: 1112948824)** - 10 abilities ⭐
Knight combat with heavy armor and weapon skills.

**Best for:** Knight-themed bosses, melee combat, chivalric encounters

### Specialized VBloods (Focused Kits)

#### **Vincent the Frostbringer (PrefabGUID: 939467639)** - 6 abilities
Ice magic specialist with frost-based combat abilities.

**Best for:** Ice-themed bosses, elemental magic, frost effects

#### **Beatrice the Tailor (PrefabGUID: -1942352521)** - 11 abilities
Shapeshifting gargoyle with wing shields and flying attacks.

**Key Abilities:**
- **Index 0**: Primary Attack - `Tailor Shapeshift Group` - Transformation ability
- **Index 1**: Secondary Attack - `Legion Gargoyle ForwardSwipe Group` - Wing swipe
- **Index 4**: Combat Ability - `Gargoyle WingShield Group` - Defensive wing shield
- **Index 5**: Combat Ability - `Gargoyle FlyStart Group` - Flight initiation
- **Index 6**: Advanced Ability - `Gargoyle FlyEnd Group` - Flight landing attack

**Best for:** Flying bosses, transformation themes, defensive encounters

#### **Bane the Shadowblade (PrefabGUID: 1896428751)** - 3 abilities
Explosive specialist with bombs and stealth.

**Key Abilities:**
- **Index 0**: Primary Attack - `Bandit ClusterBombThrow Group` - Cluster bomb attack
- **Index 2**: Special/Spell - `Bandit StickyBomb Group` - Sticky explosive

**Best for:** Explosive-themed bosses, stealth encounters, quick burst damage

## 🎨 Advanced Configuration Examples

### Example 1: Ultimate Hybrid Boss
```bash
# Create the ultimate boss mixing the best from each tier
.bb create "UltimateBoss" -327335305 500 10 3600  # Dracula appearance

# Primary combat from Dracula
.bb ability-slot-set "UltimateBoss" "primary" -327335305 0 true "Dracula shockwave slash"
.bb ability-slot-set "UltimateBoss" "secondary" -327335305 1 true "Dracula down swing"

# Magic from Christina  
.bb ability-slot-set "UltimateBoss" "holy1" -99012450 2 true "Christina AoE"
.bb ability-slot-set "UltimateBoss" "holy2" -99012450 3 true "Christina minions"

# Mobility from Alpha Wolf
.bb ability-slot-set "UltimateBoss" "mobility" -1905691330 6 true "Wolf pounce"

# Explosives from Bane
.bb ability-slot-set "UltimateBoss" "explosive" 1896428751 0 true "Cluster bombs"
```

### Example 2: Themed Encounters

#### Ice & Fire Boss
```bash
.bb create "ElementalBoss" 939467639 400 8 2400  # Vincent (ice) appearance

# Ice abilities from Vincent
.bb ability-slot-set "ElementalBoss" "ice1" 939467639 0 true "Frost primary"
.bb ability-slot-set "ElementalBoss" "ice2" 939467639 2 true "Frost spell"

# Fire/Dark abilities from Dracula  
.bb ability-slot-set "ElementalBoss" "fire1" -327335305 2 true "Dracula sword throw"
.bb ability-slot-set "ElementalBoss" "fire2" -327335305 10 true "Blood bolt swarm"
```

#### Holy Warrior Boss
```bash
.bb create "PaladinBoss" 1112948824 350 6 2400  # Tristan (knight) appearance

# Knight melee from Tristan
.bb ability-slot-set "PaladinBoss" "melee1" 1112948824 0 true "Knight primary"
.bb ability-slot-set "PaladinBoss" "melee2" 1112948824 1 true "Knight charge"

# Holy magic from Christina
.bb ability-slot-set "PaladinBoss" "holy1" -99012450 1 true "Holy bolt spray"
.bb ability-slot-set "PaladinBoss" "holy2" -99012450 6 true "Healing channel"
```

## 🔧 Configuration Management

### Viewing Current Configuration
```bash
# List all configured slots for a boss
.bb ability-slot-list "YourBoss"

# Check specific boss status including abilities
.bb status "YourBoss"

# Debug ability information
.bb debug "YourBoss"
```

### Modifying Configuration
```bash
# Enable/disable specific slots without removing them
.bb ability-slot-toggle "YourBoss" "melee1"

# Remove unwanted slots
.bb ability-slot-remove "YourBoss" "unwanted_slot"

# Clear all and start fresh
.bb ability-slot-clear "YourBoss"
```

### Troubleshooting
```bash
# If abilities aren't working, try:
.bb despawn "YourBoss"  # Despawn current instance
.bb start "YourBoss"    # Respawn with new abilities

# Inspect source VBlood for ability details
.bb ability-inspect -327335305  # Check Dracula's abilities
```

## 📋 Best Practices

### 🎯 **Slot Naming Convention**
Use descriptive slot names for easier management:
- `primary`, `secondary` - Main attacks
- `spell1`, `spell2` - Magic abilities  
- `utility1`, `utility2` - Support/mobility
- `ultimate` - Powerful finishing moves

### ⚡ **Performance Considerations**
- Limit to 6 active slots per boss for optimal performance
- Use abilities from the same "tier" when possible for balance
- Test ability combinations before deploying to live server

### 🎮 **Balance Guidelines**
- **Primary/Secondary slots**: Use for main combat abilities
- **Special/Ultimate slots**: Reserve for powerful spells or unique mechanics
- **Mix mobility and damage**: Include both offensive and utility abilities
- **Consider visual coherence**: Some ability combinations may look strange

### 🔄 **Update Workflow**
1. **Design** your boss concept and desired abilities
2. **Test** with a single player first
3. **Adjust** slot configurations based on performance
4. **Deploy** to live server
5. **Monitor** player feedback and balance

## 🚀 Advanced Features

### Real-time Discovery
The system can discover new VBloods automatically:
```bash
# Discover all VBloods currently in the game world
.bb ability-discover

# This generates a file with all found VBloods and their PrefabGUIDs
# Useful after game updates that add new VBloods
```

### Auto-Documentation Generation
```bash
# Generate comprehensive documentation with all current abilities
.bb ability-export-all

# Files are saved to: /BepInEx/config/BloodyBoss/
# - VBlood_Abilities_Documentation_[timestamp].md
# - Discovered_VBloods_[timestamp].txt
```

### JSON Configuration
Boss ability configurations are stored in JSON format:
```json
{
  "CustomAbilities": {
    "melee1": {
      "SourcePrefabGUID": -327335305,
      "AbilityIndex": 0,
      "Enabled": true,
      "Description": "Dracula shockwave slash"
    },
    "spell1": {
      "SourcePrefabGUID": -99012450,
      "AbilityIndex": 2,
      "Enabled": true,
      "Description": "Christina AoE"
    }
  }
}
```

## 🎪 Predefined Presets

### Available Presets

#### **dracula-mix**
Dark melee combat with frost magic support.
```json
{
  "melee1": { "SourcePrefabGUID": -327335305, "AbilityIndex": 0, "Description": "Dracula melee attack" },
  "spell1": { "SourcePrefabGUID": -327335305, "AbilityIndex": 2, "Description": "Dracula spell" },
  "special": { "SourcePrefabGUID": 939467639, "AbilityIndex": 1, "Description": "Vincent frost ability" }
}
```

#### **frost-warrior**
Knight combat enhanced with ice powers.
```json
{
  "melee1": { "SourcePrefabGUID": 1112948824, "AbilityIndex": 0, "Description": "Tristan melee" },
  "melee2": { "SourcePrefabGUID": 1112948824, "AbilityIndex": 1, "Description": "Tristan charge" },
  "frost": { "SourcePrefabGUID": 939467639, "AbilityIndex": 2, "Description": "Vincent frost blast" }
}
```

#### **spell-caster**
Pure magic combination focusing on spells and support.
```json
{
  "spell1": { "SourcePrefabGUID": -99012450, "AbilityIndex": 1, "Description": "Christina heal" },
  "spell2": { "SourcePrefabGUID": -99012450, "AbilityIndex": 2, "Description": "Christina light" },
  "spell3": { "SourcePrefabGUID": -327335305, "AbilityIndex": 3, "Description": "Dracula dark spell" }
}
```

## 🎯 Tips for Epic Encounters

### Theme-Based Combinations
- **Elemental Master**: Mix fire (Dracula), ice (Vincent), and earth abilities
- **Undead Legion**: Combine necromancy, vampiric, and unholy abilities  
- **Divine Warrior**: Holy magic (Christina) with knight combat (Tristan)
- **Beast Lord**: Wolf abilities with various creature powers
- **Arcane Destroyer**: Pure magic focus with devastating spell combinations

### Progressive Difficulty
Use the modular system with dynamic scaling:
1. **Phase 1**: Basic abilities only
2. **Phase 2**: Add special abilities at 75% health
3. **Phase 3**: Activate ultimate abilities at 25% health

### Surprise Elements
- Start with familiar abilities, then switch mid-fight
- Use `.bb ability-slot-toggle` during combat to change boss behavior
- Combine unexpected ability sources for unique encounters

---

*This modular ability system opens infinite possibilities for creating unique boss encounters. Experiment, test, and create the ultimate challenges for your V Rising server!*