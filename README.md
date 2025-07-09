# BloodyBoss v2.1.0

**BloodyBoss** is an advanced mod for V Rising that allows you to create dynamic VBlood world bosses with intelligent scaling, progressive difficulty, and extensive customization options. Create epic encounters that adapt to your player base and provide engaging challenges for solo players and large groups alike.

## 🆕 What's New in v2.1.0

### 🎯 **Dynamic Scaling System**
- **Automatic scaling** based on online players - bosses become stronger with more players
- **Separate multipliers** for health and damage scaling
- **Configurable caps** to prevent overwhelming difficulty
- **Real-time adjustment** as players join or leave the server

### 📈 **Progressive Difficulty**
- **Consecutive spawn tracking** - bosses become more challenging if they keep spawning
- **Escalating rewards** for defeating harder iterations
- **Automatic reset** on successful kills or configurable persistence
- **Visual indicators** showing difficulty progression

### 🎭 **Phase Announcement System**
- **Dynamic phase notifications** when bosses scale up or down
- **Fully customizable messages** supporting multiple languages
- **Color-coded difficulty levels** (Normal, Hard, Epic, Legendary)
- **Special effects** for high-tier encounters
- **Consecutive spawn milestones** with unique announcements

### 🎮 **Advanced Command System**
New administrative and player commands for enhanced control:
- **Boss management** - despawn, pause, resume boss timers
- **Debugging tools** - detailed status and technical information
- **Simulation commands** - test boss mechanics safely
- **Teleportation system** - configurable player/admin access to boss locations
- **Force mechanics** - manually trigger drops, reset kill counters

### 🧪 **Modular Ability System**
Revolutionary new system for mixing and matching VBlood abilities:
- **Slot-based configuration** - Configure up to 6 individual ability slots per boss
- **Cross-VBlood mixing** - Combine abilities from different VBloods
- **Visual preservation** - Bosses maintain their original appearance while using new abilities
- **Predefined presets** - Quick-apply popular ability combinations
- **Real-time discovery** - Automatically discover all available VBlood abilities

### ⚙️ **Enhanced Configuration**
- **Modular settings** for all new systems
- **Per-server customization** of messages and behavior
- **Granular control** over scaling parameters
- **Backward compatibility** with existing configurations

### 🏰 **Smart Territory System**
- **Castle detection** - automatically detects player-built territories
- **Intelligent relocation** - finds alternative spawn positions within 100 units
- **Conflict prevention** - prevents boss spawns inside player bases
- **Seamless operation** - works transparently without user intervention

### ⚡ **Reliable Timer System**
- **Independent operation** - functions regardless of player connection status
- **System.Threading.Timer** - more reliable than game-dependent coroutines
- **Consistent scheduling** - bosses spawn on time even on empty servers
- **Performance optimized** - minimal server resource usage

### 🌍 **Internationalization Support**
- **Translatable phase names** and messages
- **Configurable templates** with placeholder system
- **Multi-language ready** - easily adapt to any language
- **Server-specific branding** with custom prefixes and suffixes

## ✨ Key Features

- 🧪 **Modular Ability System** - Mix and match abilities from different VBloods while preserving appearance
- 🎯 **Smart Boss Scaling** - Automatically adjusts difficulty based on server population
- 📊 **Progressive Challenges** - Bosses become stronger over multiple spawns
- 🎪 **Epic Encounters** - Special phases with dramatic announcements
- 🎮 **Advanced Commands** - Complete administrative control over boss systems
- 🔍 **Real-time Discovery** - Automatically discover all VBloods and their abilities
- 🌐 **Multi-language** - Fully customizable in any language
- ⚡ **Performance Optimized** - Efficient timer system with minimal server impact
- 🏰 **Territory Aware** - Intelligently avoids spawning bosses inside player castles
- ⏰ **Always-On Timers** - Reliable spawning even when no players are online
- 🔇 **Silent Operations** - Optional quiet mode for administrative actions
- 🔧 **Highly Configurable** - Extensive options for server customization

## 📋 Requirements

Ensure the following mods are installed for seamless integration:

1. [BepInEx 1.733.2](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/) 
2. [VampireCommandFramework 0.10.4](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/)
3. [Bloody.Core 2.0.0](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/)

## 🚀 Quick Start

1. **Install** the mod and dependencies
2. **Launch** your server to generate config files
3. **Configure** your settings in `BepInEx/Config/BloodyBoss.cfg`
4. **Create** your first boss with `.bb create "My Boss" -1905691330 90 2 1800`
5. **Set location** with `.bb set location "My Boss"`
6. **Set spawn time** with `.bb set hour "My Boss" 20:00`
7. **Add rewards** with `.bb items add "My Boss" "Blood Essence" 1055853475 50 100`
8. **Start** the encounter with `.bb start "My Boss"`

## 📚 Documentation

| Topic | Description |
|-------|-------------|
| [📦 Installation](docs/INSTALLATION.md) | Complete installation guide and troubleshooting |
| [⚙️ Configuration](docs/CONFIGURATION.md) | Detailed configuration options and examples |
| [🎮 Commands](docs/COMMANDS.md) | Complete command reference with examples |
| [🧪 Modular Abilities](docs/MODULAR-ABILITIES.md) | **NEW!** Complete guide to mixing VBlood abilities |
| [🚀 Advanced Features](docs/ADVANCED-FEATURES.md) | Dynamic scaling, progressive difficulty, and phase systems |
| [🛠️ Troubleshooting](docs/TROUBLESHOOTING.md) | Common issues and solutions |
| [📝 Examples](docs/EXAMPLES.md) | Step-by-step setup guides and use cases |

## 🎯 Example Boss Encounter

```bash
# Create a challenging boss that scales with players
.bb create "Ancient Dracula" -1905691330 100 2 2400

# Set location (stand where you want the boss to spawn)
.bb set location "Ancient Dracula"

# Schedule for 8 PM server time
.bb set hour "Ancient Dracula" 20:00

# Add epic rewards
.bb items add "Ancient Dracula" "Greater Blood Essence" 1055853475 100 75
.bb items add "Ancient Dracula" "Legendary Weapon Box" -257494203 1 25
.bb items add "Ancient Dracula" "Ancient Relic" 429052660 5 50

# Enable dynamic scaling for this encounter
# Configure in BloodyBoss.cfg:
# EnableDynamicScaling = true
# HealthPerPlayer = 0.3    (30% more health per player)
# DamagePerPlayer = 0.2    (20% more damage per player)
# EnableProgressiveDifficulty = true
```

This will create a boss that:
- ⚖️ **Scales automatically** with 2-10 players online
- 📈 **Gets progressively harder** if not defeated
- 📢 **Announces phase changes** to all players
- 🎁 **Drops rare rewards** with configured chances
- ⏰ **Spawns reliably** at 8 PM server time

## 🔧 Configuration Preview

**Dynamic Scaling:**
```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.5
HealthPerPlayer = 0.25
DamagePerPlayer = 0.15
MaxPlayersForScaling = 10
```

**Phase Messages (Customizable):**
```ini
[Phase Messages]
EpicTemplate = ⚡ EPIC ENCOUNTER! #bossname# [#phasename#] - Phase #phase#
LegendaryPrefix = 💀 LEGENDARY THREAT! 
```

## 🎮 Command Examples

**Basic Commands:**
```bash
.bb create "Shadow Lord" -1905691330 95 3 3000
.bb set location "Shadow Lord"
.bb set hour "Shadow Lord" 21:30
.bb start "Shadow Lord"
```

**Advanced Commands (v2.1.0):**
```bash
.bb status "Shadow Lord"           # Detailed boss information
.bb pause "Shadow Lord"            # Pause boss timer
.bb resume "Shadow Lord"           # Resume boss timer
.bb despawn "Shadow Lord"          # Force despawn boss
.bb teleport "Shadow Lord"         # Teleport to boss (if enabled)
.bb debug "Shadow Lord"            # Technical debug information
```

**Modular Ability Commands:**
```bash
# Configure individual ability slots
.bb ability-slot-set "Boss" "melee1" -327335305 0 true "Dracula melee"
.bb ability-slot-set "Boss" "spell1" 939467639 2 true "Vincent frost"

# Apply predefined presets
.bb ability-preset "Boss" "dracula-mix"    # Dracula + Vincent combo
.bb ability-preset "Boss" "frost-warrior"  # Tristan + Vincent combo
.bb ability-preset "Boss" "spell-caster"   # Christina + Dracula combo

# Manage ability slots
.bb ability-slot-list "Boss"              # List all configured slots
.bb ability-slot-toggle "Boss" "melee1"   # Enable/disable specific slot
.bb ability-slot-remove "Boss" "spell1"   # Remove specific slot
.bb ability-slot-clear "Boss"             # Clear all slots

# Discovery and documentation
.bb ability-discover                       # Discover all VBloods in game
.bb ability-export-all                    # Generate complete documentation
.bb ability-inspect -327335305            # Inspect specific VBlood abilities
```

## 🧪 Modular Ability System Examples

Create bosses with unique ability combinations:

```bash
# Create a Vincent that looks like Vincent but fights like Dracula + others
.bb create "VincentMix" 939467639 350 6 3000
.bb set location "VincentMix"

# Configure mixed abilities from different VBloods
.bb ability-slot-set "VincentMix" "melee1" -327335305 0 true "Dracula dark melee"
.bb ability-slot-set "VincentMix" "melee2" 1112948824 1 true "Tristan knight charge"
.bb ability-slot-set "VincentMix" "spell1" -99012450 1 true "Christina holy light"
.bb ability-slot-set "VincentMix" "spell2" -99012450 2 true "Christina divine blast"
.bb ability-slot-set "VincentMix" "special1" 1666186131 0 true "Ungora web attack"
.bb ability-slot-set "VincentMix" "special2" -1347412392 1 true "Octavian militia strike"

# Start the encounter
.bb start "VincentMix"
```

**Result:** A boss that appears as Vincent visually but uses a deadly combination of melee attacks from Dracula and Tristan, holy magic from Christina, web attacks from Ungora, and militia strikes from Octavian!

### 📋 Available VBlood Abilities

Based on game discovery, here are the confirmed VBloods with their available ability slots:

| VBlood Name | PrefabGUID | Ability Slots | Compatible |
|-------------|------------|---------------|------------|
| **Dracula** | -327335305 | 37 slots | ✅ Excellent |
| **The Winged Horror** | 1233988687 | 21 slots | ✅ Excellent |
| **Cyril the Cursed Smith** | -1347412392 | 16 slots | ✅ Very Good |
| **Frostmaw the Mountain Terror** | 24378719 | 12 slots | ✅ Very Good |
| **Meredith the Bright Archer** | -1065970933 | 12 slots | ✅ Very Good |
| **Beatrice the Tailor** | -1942352521 | 11 slots | ✅ Good |
| **Alpha Wolf** | -1905691330 | 10 slots | ✅ Good |
| **Christina the Sun Priestess** | -99012450 | 10 slots | ✅ Good |
| **Tristan the Vampire Knight** | 1112948824 | 10 slots | ✅ Good |
| **Errol the Stonebreaker** | 1106149033 | 9 slots | ✅ Good |
| **Putrid Rat** | -1391546313 | 9 slots | ✅ Good |
| **Quincy the Bandit King** | -1659822956 | 9 slots | ✅ Good |
| **Cursed Wanderer** | 577478542 | 8 slots | ✅ Good |
| **Jade the Vampire Hunter** | -2025101517 | 8 slots | ✅ Good |
| **Manticore** | 1945956671 | 8 slots | ✅ Good |
| **Raziel the Shepherd** | -680831417 | 8 slots | ✅ Good |
| **Terah the Geomancer** | -203043163 | 8 slots | ✅ Good |
| **Terrorclaw the Ogre** | 577478542 | 8 slots | ✅ Good |
| **Vincent the Frostbringer** | 939467639 | 6 slots | ✅ Good |
| **Polora the Feywalker** | -1208888966 | 6 slots | ✅ Good |
| **Rufus the Foreman** | -2039908510 | 5 slots | ✅ Moderate |
| **Bane the Shadowblade** | 1896428751 | 3 slots | ✅ Limited |
| **Clive the Firestarter** | -700632469 | 3 slots | ✅ Limited |
| **Nicholaus the Fallen** | 1896428751 | 3 slots | ✅ Limited |

### 🎯 Predefined Presets

Quick-apply popular combinations:

| Preset Name | Description | Abilities |
|-------------|-------------|-----------|
| **dracula-mix** | Dark melee + frost magic | Dracula melee + spell, Vincent frost |
| **frost-warrior** | Knight combat + ice powers | Tristan melee + charge, Vincent frost blast |
| **spell-caster** | Pure magic combination | Christina heal + light, Dracula dark spell |

```bash
# Apply any preset instantly
.bb ability-preset "YourBoss" "dracula-mix"
```

### 📋 **Modular Ability System Overview**

Mix and match abilities from different VBloods while preserving original appearance:

```bash
# Quick example: Vincent with Dracula's powers
.bb create "HybridBoss" 939467639 400 8 2400
.bb ability-slot-set "HybridBoss" "primary" -327335305 0 true "Dracula shockwave"
.bb ability-slot-set "HybridBoss" "spell" -99012450 2 true "Christina AoE"
.bb start "HybridBoss"
```

**🔗 For complete abilities reference:** See [docs/MODULAR-ABILITIES.md](docs/MODULAR-ABILITIES.md)

## 🎭 Phase System Examples

When dynamic scaling is enabled, players will see announcements like:

- `⚔️ Ancient Dracula [Normal] - Phase 1 | 2 players | Damage x1.3`
- `⚡ EPIC ENCOUNTER! Ancient Dracula [Epic Veteran] - Phase 3 | 6 players | Damage x2.8 | Consecutive: 4`
- `💀 LEGENDARY THREAT! Ancient Dracula [Legendary Enraged] - Phase 5 | 8 players | Damage x3.5 | Consecutive: 6 💀`

## 🐛 Known Issues

- When two bosses with identical PrefabGUIDs die simultaneously, only one may grant rewards
- Boss icons may occasionally persist on the map after boss death (use `.bb clearallicons` to fix)

## 🔧 Fixed Issues

- **Boss Y-coordinate (height) spawning** - Fixed in v2.1.0 with BloodyCore v2.0.0
  - Bosses now correctly spawn at the specified Y coordinate when structures are present
  - V Rising has a natural height limit (~5 units) for NPCs on terrain
  - Bosses will spawn at higher elevations when placed on castle floors or elevated structures

## 📊 Performance Notes

- ✅ **Optimized timer system** - Uses numeric comparisons instead of string parsing
- ✅ **Efficient scaling calculations** - Cached values reduce computation overhead  
- ✅ **Smart announcement system** - Only triggers on meaningful phase changes
- ✅ **Memory optimized** - Proper entity cleanup and disposal

## 🤝 Community & Support

- 🎮 [V Rising Mod Community Discord](https://discord.gg/vrisingmods)
- 🐛 [Report Issues](https://github.com/your-repo/issues)
- 💡 [Request Features](https://github.com/your-repo/discussions)
- 📖 [Modding Wiki](https://wiki.vrisingmods.com/)

## 🎁 Support the Project

If you enjoy BloodyBoss and want to support continued development:

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/K3K8ENRQY)

## 🏆 Credits

**Special thanks to:**

- 👨‍💻 **[@Deca](https://github.com/decaprime)** - VampireCommandFramework creator
- 🎮 **[V Rising Mod Community](https://discord.gg/vrisingmods)** - Premier modding community
- 🧪 **Beta Testers** - @Bromelda, @Wolfyowns, and the [BloodCraft server](https://discord.gg/aDh98KtEWZ) community
- 🔧 **Contributors** - Everyone who reported issues and suggested improvements

---

## 📋 Changelog

<details>
<summary>Version History</summary>

### `2.1.0` - Dynamic Scaling & Modular Abilities
- ✨ **NEW**: **Modular Ability System** - Mix and match abilities from different VBloods
- ✨ **NEW**: **Slot-based configuration** - Configure up to 6 individual ability slots per boss
- ✨ **NEW**: **Cross-VBlood mixing** - Combine abilities while preserving original appearance
- ✨ **NEW**: **Predefined presets** - Quick-apply popular ability combinations (dracula-mix, frost-warrior, spell-caster)
- ✨ **NEW**: **Real-time VBlood discovery** - Automatically discover all VBloods and their abilities
- ✨ **NEW**: **Comprehensive ability documentation** - Auto-generate complete ability guides
- ✨ **NEW**: Dynamic scaling system based on online players
- ✨ **NEW**: Progressive difficulty with consecutive spawn tracking  
- ✨ **NEW**: Phase announcement system with customizable messages
- ✨ **NEW**: Advanced admin commands (despawn, pause, resume, debug, etc.)
- ✨ **NEW**: Teleportation system with configurable permissions
- ✨ **NEW**: Multi-language support for all messages
- ✨ **NEW**: Castle territory detection - prevents boss spawns inside player castles
- ✨ **NEW**: Automatic spawn position relocation - finds valid positions within 100 units if original is in castle
- ✨ **NEW**: Independent timer system - bosses spawn on schedule regardless of player connection status
- 🔧 **IMPROVED**: Timer system performance with numeric comparisons
- 🔧 **IMPROVED**: Error handling and logging throughout
- 🔧 **IMPROVED**: Silent despawn command - no more spam messages when admins manually despawn bosses
- 🔧 **UPDATED**: Dependencies to latest versions (BepInEx 1.733.2, VCF 0.10.4)

### `2.0.0` - Oakveil Compatibility Update
- 🔧 **UPDATED**: To Oakveil compatibility
- 🗑️ **REMOVED**: Deprecated UnitStats properties for Oakveil compatibility
- 🗑️ **REMOVED**: Bloodstone dependency
- 🐛 **FIXED**: Boss despawn loot drop issue
- 🐛 **FIXED**: Lifetime and despawn time calculation issues

### `1.1.14`
- 🐛 **FIXED**: Spawn timer stopping when bosses lack spawn times
- 🐛 **FIXED**: Boss team assignment causing friendly fire

### `1.1.13`
- ✨ **NEW**: Random boss spawning at original locations

### `1.1.12`  
- ✨ **NEW**: `clearallicons` command for stuck map icons
- 🔧 **IMPROVED**: VBlood reward system restored to original behavior
- 🔧 **CLEANED**: Removed trailing commas from player names

[View full changelog...](docs/CHANGELOG.md)

</details>

---

*BloodyBoss v2.1.0 - Creating legendary encounters for V Rising servers worldwide* 🧛‍♂️⚔️