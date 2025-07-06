# 🚀 Advanced Features

BloodyBoss v2.1.0 introduces powerful new systems that create dynamic, engaging boss encounters. This guide covers the advanced features that set BloodyBoss apart from basic boss spawning mods.

## 🎯 Dynamic Scaling System

The Dynamic Scaling System automatically adjusts boss difficulty based on server population, ensuring balanced encounters for any group size.

### How It Works

1. **Real-time monitoring** of online players
2. **Automatic calculation** of scaling multipliers
3. **Separate scaling** for health and damage
4. **Configurable caps** to prevent overwhelming difficulty

### Scaling Formula

```
Final Health = Base Health × Base Multiplier × (1 + Players × Health Per Player)
Final Damage = Base Damage × (1 + Players × Damage Per Player)
```

### Configuration Examples

#### Casual Server Setup
```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.2
HealthPerPlayer = 0.15      # +15% health per player
DamagePerPlayer = 0.1       # +10% damage per player
MaxPlayersForScaling = 6    # Cap at 6 players
```

**Results:**
- 1 player: 1.2x health, 1.0x damage
- 3 players: 1.65x health, 1.3x damage
- 6 players: 2.1x health, 1.6x damage

#### Hardcore Server Setup
```ini
[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.8
HealthPerPlayer = 0.35      # +35% health per player
DamagePerPlayer = 0.25      # +25% damage per player
MaxPlayersForScaling = 12   # Cap at 12 players
```

**Results:**
- 1 player: 1.8x health, 1.0x damage
- 6 players: 3.9x health, 2.5x damage
- 12 players: 6.0x health, 4.0x damage

### Real-world Scenarios

**Scenario 1: Growing Guild**
- Boss spawns with 2 players online → Moderate difficulty
- 4 more players join during fight → Boss maintains current stats
- Next spawn recalculates for 6 players → Higher difficulty

**Scenario 2: Prime Time Rush**
- Boss spawns during server peak (10 players) → Maximum difficulty
- Players log off during fight → Boss keeps current difficulty
- Late night respawn (2 players) → Much easier encounter

## 📈 Progressive Difficulty System

Progressive Difficulty makes bosses increasingly challenging when they spawn repeatedly without being defeated, creating escalating stakes and rewards.

### How It Works

1. **Spawn tracking** - Counts consecutive spawns without kills
2. **Difficulty escalation** - Each spawn increases multipliers
3. **Success reset** - Killing the boss resets progression
4. **Configurable caps** - Prevents impossibly hard encounters

### Progression Examples

With `DifficultyIncrease = 0.15` (15% per spawn):

```
Spawn 1: Normal difficulty (1.0x multiplier)
Spawn 2: 15% harder (1.15x multiplier)
Spawn 3: 30% harder (1.30x multiplier)
Spawn 4: 45% harder (1.45x multiplier)
Spawn 5: 60% harder (1.60x multiplier)
...
Max Cap: Depends on MaxDifficultyMultiplier setting
```

### Combined with Dynamic Scaling

Progressive Difficulty **multiplies** with Dynamic Scaling:

```
Example with 4 players online:
- Dynamic scaling: 2.0x health
- Progressive (3rd spawn): 1.30x multiplier
- Final result: 2.6x health (2.0 × 1.30)
```

### Strategic Implications

**For Players:**
- **Early attempts** are easier but offer standard rewards
- **Later attempts** are much harder but more rewarding
- **Success** immediately resets difficulty
- **Failure** means next attempt is even harder

**For Server Admins:**
- **Encourages coordination** - players organize to tackle hard bosses
- **Creates urgency** - don't let the boss get too strong
- **Balances populations** - harder bosses during peak times
- **Rewards persistence** - dedicated groups get better rewards

## 🎭 Phase Announcement System

The Phase Announcement System provides dynamic, real-time feedback about boss encounters, keeping all players informed and engaged.

### Phase Determination

Phases are calculated based on:

1. **Player count** (2-3 = Normal, 4-5 = Hard, 6-7 = Epic, 8+ = Legendary)
2. **Damage multiplier** (2.0x+ = Hard, 2.5x+ = Epic, 3.0x+ = Legendary)
3. **Progressive spawns** (3+ = Veteran, 5+ = Enraged)

### Phase Examples

#### Normal Encounters
```
⚔️ Shadow Lord [Normal] - Phase 1 | 2 players | Damage x1.3
```

#### Escalating Difficulty
```
⚔️ Shadow Lord [Hard] - Phase 2 | 4 players | Damage x1.8
⚡ EPIC ENCOUNTER! Shadow Lord [Epic Veteran] - Phase 3 | 6 players | Damage x2.8 | Consecutive: 4
💀 LEGENDARY THREAT! Shadow Lord [Legendary Enraged] - Phase 5 | 8 players | Damage x3.5 | Consecutive: 6 💀
```

### Customization

**Message Templates:**
```ini
[Phase Messages]
NormalTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase# | #players# players
EpicTemplate = ⚡ EPIC! #bossname# [#phasename#] | Damage x#damage# | Players: #players#
LegendaryTemplate = 💀 LEGENDARY THREAT! #bossname# has reached #phasename# power!
```

**Placeholders Available:**
- `#bossname#` - Boss name (colored)
- `#phasename#` - Phase name (colored by difficulty)
- `#phase#` - Phase number (1, 2, 3, 4+)
- `#players#` - Online player count
- `#damage#` - Damage multiplier (e.g., 2.5)
- `#consecutive#` - Consecutive spawn count
- `#consecutive_info#` - Formatted consecutive info

### Multi-language Support

**Spanish Example:**
```ini
[Phase Names]
Normal = Normal
Hard = Difícil
Epic = Épico
Legendary = Legendario

[Phase Messages]
EpicPrefix = ⚡ ¡ENCUENTRO ÉPICO! 
LegendaryPrefix = 💀 ¡AMENAZA LEGENDARIA! 
ConsecutiveInfoTemplate =  | Consecutivos: #consecutive#
```

## 🌐 Teleportation System

The advanced teleportation system provides configurable player access to boss encounters with built-in balance mechanisms.

### Features

1. **Permission control** - Admin-only or all players
2. **Cooldown system** - Prevents teleport spam
3. **Cost system** - Require items for teleportation
4. **State restrictions** - Only to active/alive bosses
5. **Safety checks** - Validates boss existence and location

### Configuration Scenarios

#### Free Admin Teleport
```ini
[Teleport]
Enable = true
AdminOnly = true
CooldownSeconds = 0
CostItemGUID = 0
```

#### Player Teleport with Cost
```ini
[Teleport]
Enable = true
AdminOnly = false
CooldownSeconds = 60
OnlyToActiveBosses = true
RequireBossAlive = true
CostItemGUID = 1055853475    # Blood Essence
CostAmount = 10
```

#### Emergency Escape System
```ini
[Teleport]
Enable = true
AdminOnly = false
RequireBossAlive = false     # Can teleport to dead boss location
OnlyToActiveBosses = false   # Can teleport to any configured boss
CostItemGUID = 429052660     # Expensive teleport scroll
CostAmount = 1
```

### Teleport Behavior

**Smart Location Detection:**
1. **Active boss** - Teleports to current boss position
2. **Dead boss** - Teleports to death location (if enabled)
3. **Inactive boss** - Teleports to configured spawn location

**Cost Consumption:**
- Checked before teleport
- Consumed only on successful teleport
- Admin teleports always free

**Cooldown Management:**
- Applied after successful teleport
- Shared across all boss teleports
- Displays remaining time on failure

## 🔄 System Integration

All advanced systems work together seamlessly:

### Example: Epic Encounter Progression

1. **Initial Spawn** (4 players online):
   ```
   ⚔️ Ancient Dracula [Hard] - Phase 2 | 4 players | Damage x1.8
   ```

2. **Players Fail, Boss Despawns**

3. **Second Spawn** (6 players online, progressive +15%):
   ```
   ⚡ EPIC ENCOUNTER! Ancient Dracula [Epic Veteran] - Phase 3 | 6 players | Damage x2.6 | Consecutive: 2
   ```

4. **Players Use Teleport**:
   ```
   Player: .bb teleport "Ancient Dracula"
   System: 🌀 Teleporting to alive boss 'Ancient Dracula'...
   System: ✅ Teleported to -150.2, 75.0, 200.5
   System: ⏰ Next teleport available in 60 seconds
   ```

5. **Boss Defeated**:
   - Progressive difficulty resets
   - Next spawn returns to normal scaling
   - Players receive enhanced rewards

### Performance Optimizations

**Efficient Calculations:**
- Scaling computed only on spawn
- Cached values reduce repeated calculations
- Phase checks only on significant changes

**Smart Announcements:**
- Only announces meaningful phase changes
- Configurable frequency to prevent spam
- Batched calculations for multiple players

**Memory Management:**
- Proper entity cleanup on despawn
- Disposed query results prevent leaks
- Optimized timer comparisons

## 🎮 Advanced Usage Tips

### For Server Administrators

**Balancing Guidelines:**
- Start with conservative scaling values
- Monitor player feedback and adjust
- Use test commands to verify difficulty
- Consider your server's typical population

**Event Planning:**
- Schedule harder bosses during peak hours
- Use progressive difficulty for recurring events
- Coordinate with teleport costs for special events
- Customize messages for themed encounters

**Performance Monitoring:**
- Watch for lag during phase announcements
- Monitor memory usage with many consecutive spawns
- Adjust announcement frequency if needed

### For Players

**Strategy Tips:**
- Check phase announcements to gauge difficulty
- Coordinate with others for epic/legendary phases
- Use teleportation strategically (mind the costs)
- Don't let bosses get too many consecutive spawns

**Resource Management:**
- Save teleportation items for important fights
- Consider cooldowns when planning boss attempts
- Use status commands to check boss difficulty

## 🛠️ Troubleshooting Advanced Features

### Dynamic Scaling Issues

**Problem**: Scaling seems incorrect
- **Check**: Player count vs. expected scaling
- **Debug**: Use `.bb debug "BossName"` for technical details
- **Verify**: Configuration values are reasonable

**Problem**: No scaling occurring
- **Check**: `EnableDynamicScaling = true` in config
- **Verify**: Boss has correct multiplier values
- **Test**: Create new boss to verify system works

### Progressive Difficulty Problems

**Problem**: Difficulty not increasing
- **Check**: `EnableProgressiveDifficulty = true`
- **Verify**: Boss is being tracked correctly
- **Debug**: Use `.bb status` to see consecutive count

**Problem**: Difficulty not resetting
- **Check**: `ResetDifficultyOnKill = true` setting
- **Manual**: Use `.bb resetkills` to clear manually
- **Verify**: Boss death is being detected properly

### Phase Announcement Issues

**Problem**: No announcements showing
- **Check**: `EnablePhaseAnnouncements = true`
- **Test**: Use `.bb simulate` to trigger manually
- **Verify**: Message templates are valid

**Problem**: Wrong language/formatting
- **Check**: Phase names and templates in config
- **Verify**: Placeholder syntax is correct
- **Test**: Use simple templates first

### Teleportation Problems

**Problem**: Teleport command not working
- **Check**: `EnableTeleportCommand = true`
- **Verify**: Player has admin permissions (if required)
- **Test**: Check cooldown and cost requirements

**Problem**: Wrong teleport location
- **Check**: Boss location is set correctly
- **Verify**: Boss entity exists and is valid
- **Debug**: Use `.bb status` to see coordinates

## 📚 Next Steps

- 📝 [Examples](EXAMPLES.md) - Real-world usage scenarios
- 🛠️ [Troubleshooting](TROUBLESHOOTING.md) - Solve specific issues
- ⚙️ [Configuration](CONFIGURATION.md) - Fine-tune settings
- 🎮 [Commands](COMMANDS.md) - Master all commands

---

*Advanced features questions? Join the [V Rising Mod Community Discord](https://discord.gg/vrisingmods)!*