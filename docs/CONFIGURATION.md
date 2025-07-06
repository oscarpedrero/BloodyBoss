# ⚙️ Configuration Guide

BloodyBoss v2.1.0 offers extensive configuration options to customize your boss encounters. This guide covers all available settings with detailed explanations and examples.

## 📁 Configuration Files

BloodyBoss uses these configuration files:

- **`BepInEx/config/BloodyBoss.cfg`** - Main configuration file
- **`BepInEx/config/BloodyBoss/Database.json`** - Boss database (auto-generated)

## 🎯 Main Configuration Sections

### [Main] - Core Settings

Controls basic mod functionality and global behavior.

```ini
[Main]
## Determines whether the boss spawn timer is enabled or not
# Setting type: Boolean
# Default value: true
Enabled = true

## The message that will appear globally once the boss gets killed
# Setting type: String  
# Default value: The #vblood# boss has been defeated by the following brave warriors:
KillMessageBossTemplate = The #vblood# boss has been defeated by the following brave warriors:

## The message that will appear globally when the boss gets spawned
# Setting type: String
# Default value: A Boss #worldbossname# has been summon you got #time# minutes to defeat it!
SpawnMessageBossTemplate = A Boss #worldbossname# has been summon you got #time# minutes to defeat it!

## The message that will appear globally if players fail to kill the boss
# Setting type: String
# Default value: You failed to kill the Boss #worldbossname# in time
DespawnMessageBossTemplate = You failed to kill the Boss #worldbossname# in time

## Buff that applies to each boss created with this mod
# Setting type: Int32
# Default value: 1163490655
BuffForWorldBoss = 1163490655

## Multiplier formula: "bosslife * multiplier" vs "bosslife * multiplier * players"  
# Setting type: Boolean
# Default value: false
PlayersOnlineMultiplier = false

## Remove original vblood drop table
# Setting type: Boolean
# Default value: false
ClearDropTable = false

## Disable minion damage to bosses
# Setting type: Boolean
# Default value: true
MinionDamage = true

## Spawn random boss instead of specific boss at spawn time
# Setting type: Boolean
# Default value: false
RandomBoss = false

## Enable buff animation for players who participated in battle
# Setting type: Boolean
# Default value: true
BuffAfterKillingEnabled = true

## PrefabGUID of buff received by participating players
# Setting type: Int32
# Default value: -2061047741
BuffAfterKillingPrefabGUID = -2061047741

## Bosses team up instead of attacking each other
# Setting type: Boolean
# Default value: false
TeamBossEnable = false
```

**Message Template Placeholders:**
- `#vblood#` - Boss name (colored)
- `#worldbossname#` - Boss name (colored)
- `#time#` - Lifetime in minutes

### [Dynamic Scaling] - Smart Difficulty Adjustment

**✨ New in v2.1.0** - Automatically scale boss difficulty based on online players.

```ini
[Dynamic Scaling]
## Enable dynamic scaling based on online players
# Setting type: Boolean
# Default value: false
Enable = true

## Base health multiplier for all bosses
# Setting type: Single
# Default value: 1.0
BaseHealthMultiplier = 1.5

## Additional health percentage per online player (0.25 = +25%)
# Setting type: Single
# Default value: 0.25
HealthPerPlayer = 0.3

## Additional damage percentage per online player (0.15 = +15%)
# Setting type: Single
# Default value: 0.15
DamagePerPlayer = 0.2

## Maximum players considered for scaling
# Setting type: Int32
# Default value: 10
MaxPlayersForScaling = 8
```

**Example Scaling:**
- **Base**: 1 player = 1.5x health, 1.0x damage
- **4 players**: 2.7x health (1.5 + 4×0.3), 1.8x damage (1.0 + 4×0.2)
- **8 players**: 3.9x health (1.5 + 8×0.3), 2.6x damage (1.0 + 8×0.2)

### [Progressive Difficulty] - Escalating Challenges

**✨ New in v2.1.0** - Bosses become harder with consecutive spawns.

```ini
[Progressive Difficulty]
## Enable progressive difficulty increase
# Setting type: Boolean
# Default value: false
Enable = true

## Difficulty increase per consecutive spawn (0.1 = +10%)
# Setting type: Single
# Default value: 0.1
DifficultyIncrease = 0.15

## Maximum difficulty multiplier (2.0 = 200%)
# Setting type: Single
# Default value: 2.0
MaxDifficultyMultiplier = 3.0

## Reset difficulty when boss is killed
# Setting type: Boolean
# Default value: true
ResetDifficultyOnKill = true
```

**Progressive Example:**
- **Spawn 1**: Normal difficulty (1.0x)
- **Spawn 2**: +15% harder (1.15x)
- **Spawn 3**: +30% harder (1.30x)
- **Kill**: Resets to normal (if ResetDifficultyOnKill = true)

### [Teleport] - Player Transportation

**✨ New in v2.1.0** - Configurable teleportation to boss locations.

```ini
[Teleport]
## Enable teleport to boss command
# Setting type: Boolean
# Default value: true
Enable = true

## Restrict teleport command to admins only
# Setting type: Boolean
# Default value: true
AdminOnly = false

## Cooldown between teleports in seconds (0 = no cooldown)
# Setting type: Single
# Default value: 60.0
CooldownSeconds = 30.0

## Only allow teleport to currently spawned bosses
# Setting type: Boolean
# Default value: true
OnlyToActiveBosses = true

## Require boss to be alive (not dead) for teleport
# Setting type: Boolean
# Default value: true
RequireBossAlive = false

## PrefabGUID of item required for teleport (0 = no cost)
# Setting type: String
# Default value: 0
CostItemGUID = 1055853475

## Amount of cost item required
# Setting type: Int32
# Default value: 1
CostAmount = 5
```

**Teleport Scenarios:**
- **Free admin teleport**: `AdminOnly = true`, `CostItemGUID = 0`
- **Player teleport with cost**: `AdminOnly = false`, `CostItemGUID = 1055853475`
- **Emergency escape**: `RequireBossAlive = false` (teleport to dead boss location)

### [Phase Announcements] - Dynamic Notifications

**✨ New in v2.1.0** - Customizable phase change announcements.

```ini
[Phase Announcements]
## Enable boss phase change announcements
# Setting type: Boolean
# Default value: true
Enable = true

## Announce every phase change (not just increases)
# Setting type: Boolean
# Default value: false
AnnounceEveryPhase = false

## Announce milestone consecutive spawns (every 3)
# Setting type: Boolean
# Default value: true
AnnounceMilestoneSpawns = true
```

### [Phase Messages] - Customizable Templates

**✨ New in v2.1.0** - Fully customizable phase announcement messages.

```ini
[Phase Messages]
## Template for normal phase messages
# Available placeholders: #bossname#, #phasename#, #phase#, #players#, #damage#, #consecutive#
# Setting type: String
# Default value: ⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage#
NormalTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage#

## Template for hard phase messages
# Setting type: String
HardTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage#

## Template for epic phase messages
# Setting type: String
EpicTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage##consecutive_info#

## Template for legendary phase messages
# Setting type: String
LegendaryTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage##consecutive_info#

## Suffix added to phase names for veteran bosses (3+ consecutive spawns)
# Setting type: String
# Default value:  Veteran
VeteranSuffix =  Veteran

## Suffix added to phase names for enraged bosses (5+ consecutive spawns)
# Setting type: String
# Default value:  Enraged
EnragedSuffix =  Enraged

## Prefix added to epic phase messages
# Setting type: String
# Default value: ⚡ EPIC ENCOUNTER! 
EpicPrefix = ⚡ EPIC ENCOUNTER! 

## Prefix added to legendary phase messages
# Setting type: String
# Default value: 💀 LEGENDARY THREAT! 
LegendaryPrefix = 💀 LEGENDARY THREAT! 

## Template for consecutive spawn information
# Setting type: String
# Default value:  | Consecutive: #consecutive#
ConsecutiveInfoTemplate =  | Consecutive: #consecutive#
```

### [Phase Names] - Translatable Phase Names

**✨ New in v2.1.0** - Customize phase names for any language.

```ini
[Phase Names]
## Name for normal difficulty phase
# Setting type: String
# Default value: Normal
Normal = Normal

## Name for hard difficulty phase  
# Setting type: String
# Default value: Hard
Hard = Hard

## Name for epic difficulty phase
# Setting type: String
# Default value: Epic
Epic = Epic

## Name for legendary difficulty phase
# Setting type: String
# Default value: Legendary
Legendary = Legendary
```

## 🌍 Multi-Language Examples

### Spanish Configuration
```ini
[Phase Names]
Normal = Normal
Hard = Difícil
Epic = Épico
Legendary = Legendario

[Phase Messages]
EpicPrefix = ⚡ ¡ENCUENTRO ÉPICO! 
LegendaryPrefix = 💀 ¡AMENAZA LEGENDARIA! 
VeteranSuffix =  Veterano
EnragedSuffix =  Enfurecido
ConsecutiveInfoTemplate =  | Consecutivos: #consecutive#
```

### French Configuration
```ini
[Phase Names]
Normal = Normal
Hard = Difficile
Epic = Épique
Legendary = Légendaire

[Phase Messages]
EpicPrefix = ⚡ RENCONTRE ÉPIQUE! 
LegendaryPrefix = 💀 MENACE LÉGENDAIRE! 
VeteranSuffix =  Vétéran
EnragedSuffix =  Enragé
ConsecutiveInfoTemplate =  | Consécutifs: #consecutive#
```

### German Configuration
```ini
[Phase Names]
Normal = Normal
Hard = Schwer
Epic = Episch
Legendary = Legendär

[Phase Messages]
EpicPrefix = ⚡ EPISCHE BEGEGNUNG! 
LegendaryPrefix = 💀 LEGENDÄRE BEDROHUNG! 
VeteranSuffix =  Veteran
EnragedSuffix =  Wütend
ConsecutiveInfoTemplate =  | Aufeinanderfolgend: #consecutive#
```

## 🎛️ Advanced Configuration Scenarios

### Scenario 1: Casual Server (Easy Mode)
```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.0
HealthPerPlayer = 0.15
DamagePerPlayer = 0.1
MaxPlayersForScaling = 6

[Progressive Difficulty]
Enable = false

[Phase Announcements]
Enable = true
```

### Scenario 2: Hardcore Server (Hard Mode)
```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 2.0
HealthPerPlayer = 0.4
DamagePerPlayer = 0.3
MaxPlayersForScaling = 15

[Progressive Difficulty]
Enable = true
DifficultyIncrease = 0.2
MaxDifficultyMultiplier = 4.0
ResetDifficultyOnKill = false
```

### Scenario 3: RP Server (Immersive)
```ini
[Phase Announcements]
Enable = true

[Phase Messages]
NormalTemplate = 🌙 The ancient #bossname# stirs... #players# souls dare to challenge it
EpicTemplate = ⚡ The very air crackles as #bossname# enters its #phasename# form!
LegendaryTemplate = 💀 BEWARE! #bossname# has ascended to #phasename# power!

[Phase Names]
Normal = Dormant
Hard = Awakened  
Epic = Enraged
Legendary = Ascended
```

## 🔄 Configuration Hot-Reload

Some settings can be reloaded without server restart:

**Hot-reloadable settings:**
- Phase message templates
- Phase names
- Announcement settings
- Teleport settings

**Requires restart:**
- Dynamic scaling enable/disable
- Progressive difficulty enable/disable
- Core mod settings

**To reload configuration:**
```bash
.bb reload
```

## 🛠️ Troubleshooting Configuration

### Invalid Configuration Values
If you set invalid values, BloodyBoss will:
1. Log an error message
2. Use default values
3. Continue functioning normally

### Configuration Corruption
If your configuration becomes corrupted:
1. **Backup current config**
2. **Delete** `BloodyBoss.cfg`
3. **Restart server** to regenerate defaults
4. **Reapply** your custom settings

### Performance Considerations

**High player count servers:**
- Set reasonable `MaxPlayersForScaling` limits
- Consider disabling phase announcements for very large servers
- Use shorter message templates to reduce chat spam

**Low-end servers:**
- Disable progressive difficulty if not needed
- Reduce announcement frequency
- Use simpler message templates

## 📊 Configuration Testing

Test your configuration with these commands:

```bash
# Test scaling calculations
.bb debug "YourBoss"

# Test phase announcements
.bb simulate "YourBoss"

# Test teleportation
.bb teleport "YourBoss"

# View current status
.bb status "YourBoss"
```

## 📚 Next Steps

- 🎮 [Commands Reference](COMMANDS.md) - Learn all available commands
- 🚀 [Advanced Features](ADVANCED-FEATURES.md) - Deep dive into new systems
- 📝 [Examples](EXAMPLES.md) - Real-world configuration examples
- 🛠️ [Troubleshooting](TROUBLESHOOTING.md) - Solve common issues

---

*Configuration questions? Ask in the [V Rising Mod Community Discord](https://discord.gg/vrisingmods)!*