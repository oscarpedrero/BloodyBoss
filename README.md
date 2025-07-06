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

### ⚙️ **Enhanced Configuration**
- **Modular settings** for all new systems
- **Per-server customization** of messages and behavior
- **Granular control** over scaling parameters
- **Backward compatibility** with existing configurations

### 🌍 **Internationalization Support**
- **Translatable phase names** and messages
- **Configurable templates** with placeholder system
- **Multi-language ready** - easily adapt to any language
- **Server-specific branding** with custom prefixes and suffixes

## ✨ Key Features

- 🎯 **Smart Boss Scaling** - Automatically adjusts difficulty based on server population
- 📊 **Progressive Challenges** - Bosses become stronger over multiple spawns
- 🎪 **Epic Encounters** - Special phases with dramatic announcements
- 🎮 **Advanced Commands** - Complete administrative control over boss systems
- 🌐 **Multi-language** - Fully customizable in any language
- ⚡ **Performance Optimized** - Efficient timer system with minimal server impact
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

## 🎭 Phase System Examples

When dynamic scaling is enabled, players will see announcements like:

- `⚔️ Ancient Dracula [Normal] - Phase 1 | 2 players | Damage x1.3`
- `⚡ EPIC ENCOUNTER! Ancient Dracula [Epic Veteran] - Phase 3 | 6 players | Damage x2.8 | Consecutive: 4`
- `💀 LEGENDARY THREAT! Ancient Dracula [Legendary Enraged] - Phase 5 | 8 players | Damage x3.5 | Consecutive: 6 💀`

## 🐛 Known Issues

- When two bosses with identical PrefabGUIDs die simultaneously, only one may grant rewards
- Boss icons may occasionally persist on the map after boss death (use `.bb clearallicons` to fix)

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

### `2.1.0` - Dynamic Scaling & Advanced Features
- ✨ **NEW**: Dynamic scaling system based on online players
- ✨ **NEW**: Progressive difficulty with consecutive spawn tracking  
- ✨ **NEW**: Phase announcement system with customizable messages
- ✨ **NEW**: Advanced admin commands (despawn, pause, resume, debug, etc.)
- ✨ **NEW**: Teleportation system with configurable permissions
- ✨ **NEW**: Multi-language support for all messages
- 🔧 **IMPROVED**: Timer system performance with numeric comparisons
- 🔧 **IMPROVED**: Error handling and logging throughout
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