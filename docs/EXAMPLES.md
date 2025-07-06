# 📝 Examples & Use Cases

This guide provides real-world examples of BloodyBoss v2.1.0 configurations and use cases. Copy these examples and adapt them to your server's needs.

## 🎯 Quick Start Examples

### Basic Boss Setup
Perfect for getting started with BloodyBoss.

```bash
# 1. Create a simple boss
.bb create "Forest Guardian" -1905691330 75 2 1800

# 2. Set spawn location (go to desired location first)
.bb set location "Forest Guardian"

# 3. Schedule for evening
.bb set hour "Forest Guardian" 19:00

# 4. Add basic rewards
.bb items add "Forest Guardian" "Blood Essence" 1055853475 25 100
.bb items add "Forest Guardian" "Iron Ingot" -257494203 15 75

# 5. Test the setup
.bb test "Forest Guardian"
```

### Weekend Event Boss
A special boss for weekend events with premium rewards.

```bash
# Create powerful weekend boss
.bb create "Weekend Titan" -1905691330 95 3 3600

# Set location at a special event area
.bb set location "Weekend Titan"

# Schedule for Saturday prime time
.bb set hour "Weekend Titan" 20:00

# Add rare weekend rewards
.bb items add "Weekend Titan" "Legendary Blood Essence" 1055853475 100 100
.bb items add "Weekend Titan" "Rare Gem Cache" 429052660 5 50
.bb items add "Weekend Titan" "Weekend Trophy" -1464869978 1 100

# Start immediately for testing
.bb start "Weekend Titan"
```

## ⚙️ Configuration Examples

### Casual Server Configuration
Balanced for relaxed gameplay with moderate scaling.

```ini
[Main]
Enabled = true
PlayersOnlineMultiplier = false
MinionDamage = false
BuffAfterKillingEnabled = true

[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.2
HealthPerPlayer = 0.2
DamagePerPlayer = 0.15
MaxPlayersForScaling = 8

[Progressive Difficulty]
Enable = true
DifficultyIncrease = 0.1
MaxDifficultyMultiplier = 1.8
ResetDifficultyOnKill = true

[Teleport]
Enable = true
AdminOnly = false
CooldownSeconds = 60
OnlyToActiveBosses = true
RequireBossAlive = true
CostItemGUID = 1055853475
CostAmount = 5

[Phase Announcements]
Enable = true
AnnounceEveryPhase = false
AnnounceMilestoneSpawns = true
```

### Hardcore PvP Server Configuration
Designed for competitive gameplay with aggressive scaling.

```ini
[Main]
Enabled = true
PlayersOnlineMultiplier = false
MinionDamage = true
TeamBossEnable = false
BuffAfterKillingEnabled = false

[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 2.0
HealthPerPlayer = 0.4
DamagePerPlayer = 0.3
MaxPlayersForScaling = 15

[Progressive Difficulty]
Enable = true
DifficultyIncrease = 0.25
MaxDifficultyMultiplier = 4.0
ResetDifficultyOnKill = false

[Teleport]
Enable = true
AdminOnly = true
CooldownSeconds = 300
OnlyToActiveBosses = true
RequireBossAlive = true
CostItemGUID = 0

[Phase Announcements]
Enable = true
AnnounceEveryPhase = true
AnnounceMilestoneSpawns = true
```

### RP Server Configuration
Focused on immersive storytelling with custom messages.

```ini
[Phase Messages]
NormalTemplate = 🌙 The ancient #bossname# stirs in the darkness...
HardTemplate = ⚡ #bossname# awakens with terrible fury! #players# brave souls face the challenge
EpicTemplate = 🔥 The very earth trembles as #bossname# enters its #phasename# form!
LegendaryTemplate = 💀 BEWARE! #bossname# has transcended to #phasename# power! The end times draw near!

[Phase Names]
Normal = Slumbering
Hard = Awakened
Epic = Enraged
Legendary = Transcendent

[Main]
SpawnMessageBossTemplate = 🌙 Dark energies stir... The ancient #worldbossname# has awakened! You have #time# minutes before it returns to slumber.
KillMessageBossTemplate = ⚔️ The #vblood# has fallen! Honor to the heroes who vanquished this ancient evil:
DespawnMessageBossTemplate = 🌫️ The #worldbossname# fades back into the shadows... The opportunity is lost.
```

## 🎮 Real-World Scenarios

### Scenario 1: Small Guild Server (5-10 players)

**Goal:** Balanced encounters that scale appropriately for small groups.

**Configuration:**
```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.0
HealthPerPlayer = 0.25
DamagePerPlayer = 0.15
MaxPlayersForScaling = 6
```

**Boss Setup:**
```bash
# Daily morning boss for EU players
.bb create "Dawn Sentinel" -1905691330 80 2 2400
.bb set hour "Dawn Sentinel" 08:00
.bb items add "Dawn Sentinel" "Morning Essence" 1055853475 30 100

# Evening boss for US players
.bb create "Twilight Hunter" -1905691330 85 2.5 2400
.bb set hour "Twilight Hunter" 21:00
.bb items add "Twilight Hunter" "Twilight Essence" 1055853475 35 100
```

**Expected Scaling:**
- 1 player: 1.25x health, 1.15x damage
- 3 players: 1.75x health, 1.45x damage
- 6 players: 2.5x health, 1.9x damage

### Scenario 2: Large Public Server (20-50 players)

**Goal:** Challenging encounters that require coordination.

**Configuration:**
```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.5
HealthPerPlayer = 0.3
DamagePerPlayer = 0.2
MaxPlayersForScaling = 12

[Progressive Difficulty]
Enable = true
DifficultyIncrease = 0.15
MaxDifficultyMultiplier = 3.0
```

**Boss Setup:**
```bash
# Prime time raid boss
.bb create "Crimson Overlord" -1905691330 100 4 4800
.bb set hour "Crimson Overlord" 20:00
.bb items add "Crimson Overlord" "Overlord's Crown" 429052660 1 15
.bb items add "Crimson Overlord" "Crimson Essence" 1055853475 75 100
.bb items add "Crimson Overlord" "Rare Materials" -257494203 25 60
```

**Expected Scaling (12 players):**
- Base: 5.1x health (1.5 + 12×0.3), 3.4x damage (1.0 + 12×0.2)
- After 3 consecutive spawns: 6.9x health, 4.6x damage
- Maximum difficulty: 15.3x health, 10.2x damage

### Scenario 3: Event Server Setup

**Goal:** Special themed events with progression over multiple days.

**Day 1: The Awakening**
```bash
.bb create "Sleeping Ancient" -1905691330 70 1.5 1800
.bb set hour "Sleeping Ancient" 19:00
.bb items add "Sleeping Ancient" "Ancient Fragment" 429052660 3 100
```

**Day 2: The Rising**
```bash
.bb create "Risen Ancient" -1905691330 85 2.5 2400
.bb set hour "Risen Ancient" 19:00
.bb items add "Risen Ancient" "Ancient Fragment" 429052660 5 100
.bb items add "Risen Ancient" "Power Crystal" 1055853475 50 75
```

**Day 3: The Ascension**
```bash
.bb create "Ascended Ancient" -1905691330 100 4 3600
.bb set hour "Ascended Ancient" 19:00
.bb items add "Ascended Ancient" "Ancient Fragment" 429052660 10 100
.bb items add "Ascended Ancient" "Ascension Relic" -1464869978 1 50
.bb items add "Ascended Ancient" "Legendary Cache" -257494203 1 25
```

## 🌍 Multi-Language Server Examples

### Spanish Server Setup

```ini
[Phase Names]
Normal = Normal
Hard = Difícil
Epic = Épico
Legendary = Legendario

[Phase Messages]
NormalTemplate = ⚔️ #bossname# [#phasename#] - Fase #phase# | #players# jugadores | Daño x#damage#
EpicTemplate = ⚡ ¡ENCUENTRO ÉPICO! #bossname# [#phasename#] - Fase #phase# | #players# jugadores | Daño x#damage##consecutive_info#
LegendaryTemplate = 💀 ¡AMENAZA LEGENDARIA! #bossname# [#phasename#] - Fase #phase# | #players# jugadores | Daño x#damage##consecutive_info#
ConsecutiveInfoTemplate =  | Consecutivos: #consecutive#
VeteranSuffix =  Veterano
EnragedSuffix =  Enfurecido

[Main]
SpawnMessageBossTemplate = ¡Un Jefe #worldbossname# ha aparecido! ¡Tienes #time# minutos para derrotarlo!
KillMessageBossTemplate = ¡El jefe #vblood# ha sido derrotado por los siguientes valientes guerreros:
DespawnMessageBossTemplate = Fallaste en matar al Jefe #worldbossname# a tiempo.
```

**Boss Commands in Spanish Context:**
```bash
.bb create "Señor Sombra" -1905691330 90 2 1800
.bb set hour "Señor Sombra" 21:00
.bb items add "Señor Sombra" "Esencia de Sangre" 1055853475 50 100
.bb items add "Señor Sombra" "Gema Legendaria" 429052660 3 25
```

### German Server Setup

```ini
[Phase Names]
Normal = Normal
Hard = Schwer
Epic = Episch
Legendary = Legendär

[Phase Messages]
NormalTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase# | #players# Spieler | Schaden x#damage#
EpicTemplate = ⚡ EPISCHE BEGEGNUNG! #bossname# [#phasename#] - Phase #phase# | #players# Spieler | Schaden x#damage##consecutive_info#
LegendaryTemplate = 💀 LEGENDÄRE BEDROHUNG! #bossname# [#phasename#] - Phase #phase# | #players# Spieler | Schaden x#damage##consecutive_info#
ConsecutiveInfoTemplate =  | Aufeinanderfolgend: #consecutive#
VeteranSuffix =  Veteran
EnragedSuffix =  Wütend

[Main]
SpawnMessageBossTemplate = Ein Boss #worldbossname# ist erschienen! Du hast #time# Minuten um ihn zu besiegen!
KillMessageBossTemplate = Der #vblood# Boss wurde von den folgenden tapferen Kriegern besiegt:
DespawnMessageBossTemplate = Ihr habt versagt, den Boss #worldbossname# rechtzeitig zu töten.
```

## 🎯 Advanced Use Cases

### Progressive Raid Series

Create a series of bosses that unlock based on previous victories.

**Week 1: The Lieutenants**
```bash
.bb create "Shadow Lieutenant" -1905691330 75 2 1800
.bb create "Blood Lieutenant" -1905691330 75 2 1800
.bb create "Frost Lieutenant" -1905691330 75 2 1800

# Schedule them on different days
.bb set hour "Shadow Lieutenant" 20:00  # Monday
.bb set hour "Blood Lieutenant" 20:00   # Wednesday  
.bb set hour "Frost Lieutenant" 20:00   # Friday
```

**Week 2: The Generals** (After all lieutenants defeated)
```bash
.bb create "Shadow General" -1905691330 90 3 2400
.bb create "Blood General" -1905691330 90 3 2400
.bb set hour "Shadow General" 20:00     # Tuesday
.bb set hour "Blood General" 20:00      # Thursday
```

**Week 3: The Commander** (Final boss)
```bash
.bb create "Supreme Commander" -1905691330 100 5 3600
.bb set hour "Supreme Commander" 20:00  # Saturday
```

### Dynamic Difficulty Tournament

Set up bosses with increasing difficulty throughout the week.

```bash
# Monday - Warm-up
.bb create "Training Dummy" -1905691330 60 1 1200
.bb items add "Training Dummy" "Practice Token" 429052660 1 100

# Tuesday - Getting Serious  
.bb create "Novice Champion" -1905691330 70 1.5 1500
.bb items add "Novice Champion" "Novice Medal" 429052660 1 100

# Wednesday - Mid-week Challenge
.bb create "Veteran Warrior" -1905691330 80 2 1800
.bb items add "Veteran Warrior" "Veteran Medal" 429052660 1 100

# Thursday - Elite Encounter
.bb create "Elite Guardian" -1905691330 90 3 2400
.bb items add "Elite Guardian" "Elite Medal" 429052660 1 100

# Friday - Legendary Battle
.bb create "Legendary Hero" -1905691330 95 4 3000
.bb items add "Legendary Hero" "Legendary Medal" 429052660 1 100

# Weekend - Championship
.bb create "Grand Champion" -1905691330 100 5 3600
.bb items add "Grand Champion" "Championship Trophy" -1464869978 1 100
```

### Seasonal Event Configuration

**Halloween Event Example:**
```ini
[Phase Messages]
NormalTemplate = 🎃 The cursed #bossname# haunts the realm... #players# souls dare to face it
EpicTemplate = 👻 SUPERNATURAL ENCOUNTER! The spirit of #bossname# grows more malevolent!
LegendaryTemplate = 💀 NIGHTMARE INCARNATE! #bossname# has become a creature of pure terror!

[Phase Names]
Normal = Restless
Hard = Haunting
Epic = Malevolent
Legendary = Nightmare
```

**Halloween Bosses:**
```bash
.bb create "Pumpkin King" -1905691330 85 2.5 2400
.bb items add "Pumpkin King" "Cursed Pumpkin" 429052660 5 100
.bb items add "Pumpkin King" "Halloween Candy" 1055853475 25 75

.bb create "Phantom Lord" -1905691330 90 3 2700
.bb items add "Phantom Lord" "Ectoplasm" 429052660 10 100
.bb items add "Phantom Lord" "Spectral Essence" 1055853475 50 50

.bb create "Death's Shadow" -1905691330 100 4 3600
.bb items add "Death's Shadow" "Soul Fragment" -1464869978 1 25
.bb items add "Death's Shadow" "Death's Gift" 429052660 3 50
```

## 🔧 Maintenance Examples

### Daily Server Maintenance Routine

```bash
#!/bin/bash
# Daily BloodyBoss maintenance script

echo "Starting BloodyBoss maintenance..."

# Clean up any stuck icons
.bb clearallicons

# Check status of all bosses
.bb list | while read boss; do
    if [[ $boss == *"Boss"* ]]; then
        boss_name=$(echo $boss | cut -d' ' -f2-)
        .bb status "$boss_name"
    fi
done

# Reload configuration to ensure consistency
.bb reload

echo "BloodyBoss maintenance complete!"
```

### Weekly Reset Routine

```bash
#!/bin/bash
# Weekly reset for progressive difficulty

echo "Starting weekly reset..."

# Reset all progressive difficulty counters
.bb resetkills "Weekly Boss 1"
.bb resetkills "Weekly Boss 2" 
.bb resetkills "Weekly Boss 3"

# Clear any stuck icons
.bb clearallicons

# Reload database
.bb reload

echo "Weekly reset complete!"
```

### Backup and Restore

```bash
#!/bin/bash
# Backup BloodyBoss configuration

BACKUP_DIR="/path/to/backups/$(date +%Y%m%d_%H%M%S)"
mkdir -p "$BACKUP_DIR"

# Backup configuration files
cp BepInEx/config/BloodyBoss.cfg "$BACKUP_DIR/"
cp -r BepInEx/config/BloodyBoss/ "$BACKUP_DIR/"

echo "Backup created at: $BACKUP_DIR"

# To restore:
# cp "$BACKUP_DIR/BloodyBoss.cfg" BepInEx/config/
# cp -r "$BACKUP_DIR/BloodyBoss/" BepInEx/config/
# .bb reload
```

## 📊 Performance Optimization Examples

### High-Population Server Optimization

For servers with 30+ players:

```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 2.0
HealthPerPlayer = 0.2
DamagePerPlayer = 0.15
MaxPlayersForScaling = 15  # Cap to prevent excessive scaling

[Phase Announcements]
Enable = true
AnnounceEveryPhase = false      # Reduce message spam
AnnounceMilestoneSpawns = false # Disable milestone announcements

[Phase Messages]
# Use shorter, simpler templates
NormalTemplate = #bossname# - Phase #phase#
EpicTemplate = EPIC! #bossname# - Phase #phase#
LegendaryTemplate = LEGENDARY! #bossname# - Phase #phase#
```

### Low-Resource Server Optimization

For servers with limited resources:

```ini
[Dynamic Scaling]
Enable = false  # Disable to reduce calculations

[Progressive Difficulty]
Enable = false  # Disable to reduce tracking

[Phase Announcements]
Enable = false  # Disable to reduce message processing

[Teleport]
Enable = false  # Disable to reduce complexity
```

## 🎮 Player Engagement Examples

### Daily Login Incentive

```bash
# Easy daily boss for casual rewards
.bb create "Daily Reward" -1905691330 60 1 900
.bb set hour "Daily Reward" 12:00  # Lunch time spawn
.bb items add "Daily Reward" "Daily Token" 429052660 1 100
.bb items add "Daily Reward" "Login Bonus" 1055853475 10 100
```

### Competitive Leaderboard System

Create bosses that track and reward top performers:

```bash
# Weekly competitive boss
.bb create "Arena Champion" -1905691330 95 3 1800
.bb set hour "Arena Champion" 19:00
.bb items add "Arena Champion" "Champion Points" 429052660 10 100
.bb items add "Arena Champion" "Leaderboard Token" -1464869978 1 25

# Monthly tournament boss
.bb create "Tournament Master" -1905691330 100 5 3600
.bb set hour "Tournament Master" 20:00  # First Saturday of month
.bb items add "Tournament Master" "Tournament Trophy" -1464869978 1 100
.bb items add "Tournament Master" "Grand Prize" 429052660 50 100
```

## 📚 Next Steps

Now that you've seen these examples, explore more:

- ⚙️ [Configuration Guide](CONFIGURATION.md) - Customize these examples
- 🎮 [Commands Reference](COMMANDS.md) - Master all commands
- 🚀 [Advanced Features](ADVANCED-FEATURES.md) - Understand the systems
- 🛠️ [Troubleshooting](TROUBLESHOOTING.md) - Fix any issues

---

*Need more examples? Ask the community in [V Rising Mod Discord](https://discord.gg/vrisingmods)!*