# BloodyBoss v2.1.5

**BloodyBoss** is an advanced mod for V Rising that allows you to create dynamic VBlood world bosses with intelligent scaling, progressive difficulty, and extensive customization options. Create epic encounters that adapt to your player base and provide engaging challenges for solo players and large groups alike.

## 🆕 What's New in v2.1.3

### 🔧 **Critical Fixes**
- **Boss persistence on server restart** - Bosses now properly retain all custom configurations after server restarts
- **TeamReference synchronization** - Fixed Entity.Null errors with smart team synchronization system
- **Cleanup command improved** - Now properly clears drop tables to prevent vanilla drops

### ✨ **New Features**
- **Smart boss recovery** - Automatic entity detection and re-linking on server startup
- **Deferred reconfiguration** - Boss settings are reapplied when players connect after restart
- **Mechanic reset system** - Boss mechanics automatically reset when boss regenerates to full health
- **Minion tracking system** - Clones and summons are now properly tracked and destroyed when boss dies

## 📜 Previous Updates - v2.1.2

### ✨ **New Features**
- **Item reward notifications** - Players now receive private chat messages showing each item received from boss kills
- **Customizable item colors** - Each reward item can have its own color in chat messages using the `Color` configuration

### 🐛 **Critical Bug Fix**
- **Fixed vanilla boss rewards** - Vanilla bosses no longer give mod-configured rewards when killed. Only bosses spawned by BloodyBoss will drop the configured items

## 📜 Previous Updates - v2.1.1

### 🔧 **Bug Fixes & Improvements**
- **Boss persistence fix** - Bosses are now properly validated on server restart, preventing invalid entities
- **Hash collision prevention** - Using GUID instead of GetHashCode for unique boss identifiers
- **VBlood reward prevention** - Critical components removed to prevent unintended VBlood unlocks from world bosses
- **Server cleanup command** - New `.bb cleanup` command for server maintenance and entity cleanup
- **Enhanced clone mechanics** - Clones can no longer be consumed for VBlood rewards

## 📜 Previous Updates - v2.1.0

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

### 🎯 **Boss Mechanics System**
Add special combat mechanics to create challenging encounters:
- **Stun Mechanic** - Stun players with visual warnings
- **AoE Attacks** - Create damaging areas with different elements
- **Slow Mechanic** - Drain energy and slow players
- **Absorb Mechanic** - Boss drains health or shields
- **Phase-based triggers** - Mechanics activate at health thresholds

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

### 🖥️ **NEW: BloodyBoss Configuration Tool**
- **Visual Boss Editor** - Modern desktop app for configuring BloodyBoss
- **Cross-Platform** - Native app for Windows, Linux, and macOS
- **Advanced Item Selector** - Browse and filter from 1082+ items with tags
- **VBlood Integration** - Select from 459+ VBlood prefabs with ability recommendations
- **Mechanics Editor** - Configure boss mechanics with visual editor and real-time validation
- **Import/Export** - Full compatibility with existing BloodyBoss configurations

<div align="center">
  <a href="https://thunderstore.io/c/v-rising/p/Trodi/BloodyBossConfig/" target="_blank">
    <img src="https://img.shields.io/badge/Download%20on-Thunderstore-blue?style=for-the-badge" alt="Download on Thunderstore">
  </a>
  <a href="https://github.com/oscarpedrero/BloodyBossConfig/" target="_blank">
    <img src="https://img.shields.io/badge/Source%20Code-GitHub-181717?style=for-the-badge&logo=github" alt="GitHub Repository">
  </a>
</div>

💡 **Highly Recommended**: Use the visual configuration tool for an easier and error-free setup experience!

## ✨ Key Features

- 🎯 **Boss Mechanics** - Add stun, AoE, slow and absorb mechanics to encounters
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
5. **Set location** with `.bb location set "My Boss"`
6. **Set spawn time** with `.bb schedule set "My Boss" 20:00`
7. **Add rewards** with `.bb item add "My Boss" "Blood Essence" 862477668 50 1.0`
8. **Start** the encounter with `.bb start "My Boss"`

## 📚 Documentation

| Topic | Description |
|-------|-------------|
| [📦 Installation](doc/INSTALLATION.md) | Complete installation guide and troubleshooting |
| [⚙️ Configuration](doc/CONFIGURATION.md) | Detailed configuration options and examples |
| [🎮 Commands](doc/commands/index.md) | Complete command reference with examples |
| [🧪 Boss Mechanics](doc/BOSS-MECHANICS.md) | Available combat mechanics (stun, aoe, slow, absorb) |

## 🎯 Example Boss Encounter

```bash
# Create a challenging boss that scales with players
.bb create "Ancient Dracula" -1905691330 100 2 2400

# Set location (stand where you want the boss to spawn)
.bb location set "Ancient Dracula"

# Schedule for 8 PM server time
.bb schedule set "Ancient Dracula" 20:00

# Add epic rewards
.bb item add "Ancient Dracula" "Greater Blood Essence" 862477668 100 0.75
.bb item add "Ancient Dracula" "Legendary Weapon Box" -257494203 1 0.25
.bb item add "Ancient Dracula" "Ancient Relic" 429052660 5 0.50

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
.bb location set "Shadow Lord"
.bb schedule set "Shadow Lord" 21:30
.bb start "Shadow Lord"
```

**Advanced Commands (v2.1.0):**
```bash
.bb status "Shadow Lord"           # Detailed boss information
.bb schedule pause "Shadow Lord"   # Pause boss timer
.bb schedule resume "Shadow Lord"  # Resume boss timer
.bb despawn "Shadow Lord"          # Force despawn boss (deprecated)
.bb location teleport "Shadow Lord" # Teleport to boss location
.bb debug info "Shadow Lord"       # Technical debug information
```

**Boss Mechanics Commands:**
```bash
# Add mechanics with quotes around options!
.bb mechanic-add "Boss" stun "--hp 90 --target random --duration 3"
.bb mechanic-add "Boss" aoe "--hp 75 --aoe_type fire --radius 10"
.bb mechanic-add "Boss" slow "--hp 50 --radius 15 --min_players 3"
.bb mechanic-add "Boss" absorb "--hp 25 --type health --amount 100"

# Manage mechanics
.bb mechanic-list "Boss"              # List all configured mechanics
.bb mechanic-toggle "Boss" 0          # Enable/disable specific mechanic
.bb mechanic-remove "Boss" 0          # Remove specific mechanic
.bb mechanic-clear "Boss"             # Clear all mechanics

# Test and debug
.bb mechanic-test "Boss" 0            # Manually trigger a mechanic
.bb mechanic-help                     # Show mechanic examples
```

## 🎯 Boss Mechanics Examples

Create challenging encounters with combat mechanics:

```bash
# Create a boss with multiple mechanics
.bb create "VincentMix" 939467639 350 6 3000
.bb location set "VincentMix"

# Add combat mechanics (remember quotes!)
.bb mechanic-add "VincentMix" stun "--hp 90 --target random --duration 3"
.bb mechanic-add "VincentMix" aoe "--hp 75 --aoe_type frost --radius 10 --damage 500"
.bb mechanic-add "VincentMix" slow "--hp 50 --radius 15 --min_players 3"
.bb mechanic-add "VincentMix" absorb "--hp 25 --type health --amount 100"

# Start the encounter
.bb start "VincentMix"
```

**Result:** A boss with escalating difficulty phases that stuns players, creates frost damage zones, slows the raid, and absorbs health to heal itself!

### 🎯 Available Mechanics

| Mechanic | Description | Key Parameters |
|----------|-------------|----------------|
| **stun** | Stuns players with warning indicator | target, duration, mark_duration |
| **aoe** | Creates damaging areas | aoe_type, radius, damage, pattern |
| **slow** | Slows and drains energy | radius, min_players, global_radius |
| **absorb** | Drains health or shields | type (health/shield/all), amount |

### 📋 **Mechanic Patterns**

Create classic boss encounters:

```bash
# Raid-style boss with phases
.bb mechanic-add "RaidBoss" stun "--hp 80 --target closest --duration 5"  # Tank swap
.bb mechanic-add "RaidBoss" aoe "--hp 60 --pattern players --count 5"     # Raid damage
.bb mechanic-add "RaidBoss" absorb "--hp 40 --type shield"                # Shield phase
.bb mechanic-add "RaidBoss" slow "--hp 20 --radius 15 --min_players 4"   # Burn phase
```

**🔗 For complete mechanics reference:** See [doc/BOSS-MECHANICS.md](doc/BOSS-MECHANICS.md)

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
- 🌟 **@erza_belserion** - For invaluable testing of the mod and configuration tool
- 👑 **Imperivm Draconis** - For their incredible support and the countless hours spent testing every single new feature and for the promotional video ( :P ), Your dedication and feedback have been absolutely invaluable in making BloodyBoss what it is today!
- 🔧 **Contributors** - Everyone who reported issues and suggested improvements

---

## 📋 Changelog

<details>
<summary>Version History</summary>

### `2.1.3` - Boss Persistence & Team System Improvements
- 🐛 **FIXED**: Boss configuration loss on server restart - bosses now properly retain all custom settings
- 🐛 **FIXED**: TeamReference synchronization issues causing Entity.Null errors
- 🐛 **FIXED**: Cleanup command now clears drop tables to prevent vanilla drops
- 🐛 **FIXED**: Clone and summon mechanics now properly despawn minions when boss dies
- ✨ **NEW**: Smart boss entity detection and re-linking on server startup
- ✨ **NEW**: Deferred boss reconfiguration when players connect after restart
- ✨ **NEW**: Minion tracking system ensures clones/summons are cleaned up properly
- 🔧 **IMPROVED**: Boss mechanic reset system when boss regenerates to full health
- 🔧 **IMPROVED**: Team system now uses smart synchronization for TeamReference
- 🔧 **IMPROVED**: More robust boss tracking during server lifecycle events
- 🔧 **IMPROVED**: Clone mechanic LifeTime properly applied with EndAction.Destroy

### `2.1.2` - Reward Improvements & Bug Fixes
- 🐛 **FIXED**: Vanilla bosses no longer grant mod rewards when killed - only mod-spawned bosses drop configured items
- ✨ **NEW**: Players now receive private chat notifications for each item received, with customizable color formatting
- 🔧 **IMPROVED**: Boss identification system now properly validates boss entities before processing rewards
- 🔧 **IMPROVED**: Item reward messages respect the configured color for each item in the boss configuration

### `2.1.1` - Bug Fixes & Stability Improvements
- 🐛 **FIXED**: Boss persistence validation on server restart
- 🐛 **FIXED**: Hash collision prevention using GUID instead of GetHashCode
- 🐛 **FIXED**: VBlood reward prevention by removing critical components
- ✨ **NEW**: Server cleanup command `.bb cleanup` for maintenance
- 🔧 **IMPROVED**: Clone mechanics - clones can no longer be consumed for VBlood

### `2.1.0` - Performance & Stability Update
- ✨ **NEW**: **Boss Mechanics System** - Add stun, AoE, slow and absorb mechanics to create challenging encounters
- ✨ **NEW**: **Event-driven damage system** - Replaced polling with efficient event-based damage detection
- ✨ **NEW**: **Optimized boss tracking** - O(1) performance instead of O(n²) for better server performance
- ✨ **NEW**: **Automatic lifetime system** - Bosses despawn automatically using Unity's LifeTime component
- 🔧 **IMPROVED**: **Memory safety** - Fixed memory leaks with proper cache management and entity cleanup
- 🔧 **IMPROVED**: **Thread safety** - All ECS operations now run on main thread preventing crashes
- 🔧 **IMPROVED**: **Damage correlation** - Better tracking of who damaged bosses for accurate rewards
- 🔧 **IMPROVED**: **Error handling** - Comprehensive null checks and graceful error recovery
- 🗑️ **REMOVED**: **Manual despawn system** - No longer needed with automatic LifeTime
- 🗑️ **REMOVED**: **HourDespawn configuration** - Simplified to just spawn time + lifetime
- 🐛 **FIXED**: Boss spawning multiple times per minute
- 🐛 **FIXED**: Server crashes when accessing ECS from timer threads
- 🐛 **FIXED**: Memory leaks from uncleaned boss data
- 🐛 **FIXED**: Damage tracking correlation issues

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

*BloodyBoss v2.1.5 - Creating legendary encounters for V Rising servers worldwide* 🧛‍♂️⚔️