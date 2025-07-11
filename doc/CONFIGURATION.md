# BloodyBoss Configuration Guide

This guide explains all configuration options available in the `BloodyBoss.cfg` file. The configuration file is automatically created when you first run the mod and can be found in your `BepInEx/config/` folder.

## Table of Contents
- [Main Settings](#main-settings)
- [Dynamic Scaling](#dynamic-scaling)
- [Progressive Difficulty](#progressive-difficulty)
- [Teleport System](#teleport-system)
- [Phase Announcements](#phase-announcements)
- [Castle Detection](#castle-detection)
- [Ability Compatibility](#ability-compatibility)
- [Logging System](#logging-system)

## Main Settings

### Basic Configuration
```ini
[Main]
## Enable or disable the boss spawn timer system
Enabled = true

## Buff applied to all spawned bosses (default: Buff_General_Vampiric)
BuffForWorldBoss = 1163490655

## Multiply boss health by number of online players
PlayersOnlineMultiplier = false

## Remove original VBlood drop tables (bosses will only drop configured items)
ClearDropTable = false

## Allow minions/summons to damage bosses
MinionDamage = true

## Spawn random boss instead of scheduled one
RandomBoss = false

## Enable buff animation for players who participated in boss kill
BuffAfterKillingEnabled = true

## Buff given to players after boss kill (default: AB_Interact_TombCoffinSpawn_Buff)
BuffAfterKillingPrefabGUID = -2061047741

## Make multiple bosses work together instead of fighting each other
TeamBossEnable = false
```

### Message Templates
```ini
## Message shown when boss is killed
KillMessageBossTemplate = "The #vblood# boss has been defeated by the following brave warriors:"

## Message shown when boss spawns
SpawnMessageBossTemplate = "A Boss #worldbossname# has been summon you got #time# minutes to defeat it!."

## Message shown when boss despawns (timeout)
DespawnMessageBossTemplate = "You failed to kill the Boss #worldbossname# in time."
```

**Placeholders:**
- `#vblood#` - VBlood type name
- `#worldbossname#` - Boss custom name
- `#time#` - Time limit in minutes

## Dynamic Scaling

Automatically adjusts boss stats based on online players.

```ini
[Dynamic Scaling]
## Enable dynamic scaling system
Enable = false

## Base health multiplier for all bosses
BaseHealthMultiplier = 1.0

## Additional health per online player (0.25 = +25% per player)
HealthPerPlayer = 0.25

## Additional damage per online player (0.15 = +15% per player)
DamagePerPlayer = 0.15

## Maximum players considered for scaling
MaxPlayersForScaling = 10
```

**Example:** With 4 players online:
- Health = BaseHealth × 1.0 × (1 + 0.25 × 4) = 2x health
- Damage = BaseDamage × (1 + 0.15 × 4) = 1.6x damage

## Progressive Difficulty

Increases difficulty for consecutive boss spawns.

```ini
[Progressive Difficulty]
## Enable progressive difficulty system
Enable = false

## Difficulty increase per consecutive spawn (0.1 = +10%)
DifficultyIncrease = 0.1

## Maximum difficulty multiplier (2.0 = 200% max)
MaxDifficultyMultiplier = 2.0

## Reset difficulty when boss is killed
ResetDifficultyOnKill = true
```

**Example:** Boss spawns 3 times without being killed:
- 1st spawn: 100% difficulty
- 2nd spawn: 110% difficulty
- 3rd spawn: 120% difficulty

## Teleport System

Configure the `.bb tp` command for teleporting to bosses.

```ini
[Teleport]
## Enable teleport command
Enable = true

## Restrict teleport to admins only
AdminOnly = true

## Cooldown between teleports in seconds
CooldownSeconds = 60.0

## Only allow teleport to currently spawned bosses
OnlyToActiveBosses = true

## Require boss to be alive for teleport
RequireBossAlive = true

## Item required for teleport (0 = no cost)
CostItemGUID = 0

## Amount of cost item required
CostAmount = 1
```

## Phase Announcements

Configure boss phase change notifications.

```ini
[Phase Announcements]
## Enable phase change announcements
Enable = true

## Announce every phase change (not just difficulty increases)
AnnounceEveryPhase = false

## Announce milestone spawns (every 3 consecutive)
AnnounceMilestoneSpawns = true
```

### Phase Message Templates
```ini
[Phase Messages]
## Templates for different difficulty phases
NormalTemplate = "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage#"
HardTemplate = "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage#"
EpicTemplate = "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage##consecutive_info#"
LegendaryTemplate = "⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players | Damage x#damage##consecutive_info#"

## Suffixes for special states
VeteranSuffix = " Veteran"
EnragedSuffix = " Enraged"

## Prefixes for high difficulty
EpicPrefix = "⚡ EPIC ENCOUNTER! "
LegendaryPrefix = "💀 LEGENDARY THREAT! "

## Consecutive spawn info
ConsecutiveInfoTemplate = " | Consecutive: #consecutive#"
```

### Phase Names
```ini
[Phase Names]
## Translatable phase names
Normal = "Normal"
Hard = "Hard"
Epic = "Epic"
Legendary = "Legendary"
```

## Castle Detection

Prevents bosses from spawning inside player territories.

```ini
[Castle Detection]
## Enable castle territory detection
Enable = true
```

## Ability Compatibility

Controls VBlood ability swapping validation.

```ini
[Ability Compatibility]
## Enable ability compatibility checking
Enable = true

## Block incompatible swaps (false = allow with warnings)
StrictMode = false

## Log compatibility warnings to console
LogWarnings = true

## Allow abilities across creature types (e.g., vampire abilities on beasts)
AllowCrossType = false
```

## Logging System

Control mod logging verbosity and performance.

```ini
[Logging]
## Global log level: None, Error, Warning, Info, Debug, Trace
GlobalLogLevel = "Warning"

## Per-category log levels (Category:Level format)
## Categories: System, Boss, Damage, Mechanic, Command, Database, Hook, Timer, Spawn, Death, Reward, Debug
CategoryLogLevels = "Boss:Warning,Damage:Warning,Hook:Warning,Timer:Warning,Mechanic:Warning,Spawn:Info,Death:Info,Reward:Info,System:Warning"

## Disabled log categories (comma-separated)
DisabledCategories = "Debug,Timer,Hook"

## Enable logging to file
LogToFile = false

## Log file path (relative to game root)
LogFilePath = "BepInEx/logs/BloodyBoss.log"
```

### Logging Presets
Use these commands in-game to quickly change logging:
- `.bb log quiet` - Only errors
- `.bb log essential` - Only spawns, deaths, rewards
- `.bb log performance` - Minimal logging
- `.bb log combat` - Combat-related logs
- `.bb log normal` - Standard logging
- `.bb log verbose` - All logs

## Default Configuration

The default configuration is optimized for:
- **Performance**: Minimal logging, no file output
- **Gameplay**: Standard boss behavior without scaling
- **Compatibility**: Works with most server setups

## Best Practices

1. **For Production Servers:**
   - Keep logging at "Warning" or use `.bb log performance`
   - Enable `TeamBossEnable` if spawning multiple bosses
   - Set appropriate teleport restrictions

2. **For Testing:**
   - Use `.bb log combat` to see mechanic executions
   - Enable phase announcements to track difficulty
   - Disable teleport cooldowns

3. **For Large Servers:**
   - Enable Dynamic Scaling
   - Set reasonable `MaxPlayersForScaling`
   - Consider enabling Progressive Difficulty

## Troubleshooting

### Bosses Too Easy/Hard
- Adjust `BaseHealthMultiplier` and damage settings
- Enable/disable Dynamic Scaling
- Check if Progressive Difficulty is accumulating

### Too Many Logs
- Use `.bb log performance` command
- Adjust `CategoryLogLevels` in config
- Disable unnecessary categories

### Bosses Spawning in Bases
- Ensure `EnableCastleDetection = true`
- Check boss spawn locations with `.bb location list`

### Ability Swaps Not Working
- Check `EnableAbilityCompatibilityCheck`
- Set `StrictMode = false` for more permissive swaps
- Enable `LogWarnings` to see issues

## Configuration Reload

Most settings require a server restart to take effect. Logging settings can be changed in real-time using the `.bb log` commands.