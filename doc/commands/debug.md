# Debug Commands

Debug commands help you test boss configurations, troubleshoot issues, and simulate various scenarios without waiting for normal spawn conditions.

## Table of Contents
- [Quick Test](#quick-test)
- [Boss Information](#boss-information)
- [Simulate Death](#simulate-death)
- [Force Item Drop](#force-item-drop)
- [Reset Kill Tracking](#reset-kill-tracking)
- [Ability Debugging](#ability-debugging)
- [Additional Debug Tools](#additional-debug-tools)

---

## Quick Test

Instantly set up and spawn a boss for testing.

### Syntax
```
.bb debug test <BossName>
```

### What It Does
1. Sets spawn location to your current position
2. Schedules spawn in 1 minute
3. Enables the boss if disabled
4. Provides countdown feedback

### Examples
```bash
.bb debug test "Fire Dragon"
.bb debug test TestBoss
```

### Output Example
```
🧪 Test setup for boss 'Fire Dragon':
├─ Location: 1250.5, 50.2, -890.3
└─ Spawn time: 15:31 (in 1 minute)
```

### Use Cases
- Quick testing after configuration changes
- Demonstrating bosses to players
- Debugging combat mechanics
- Performance testing

---

## Boss Information

Shows detailed technical information about a boss.

### Syntax
```
.bb debug info <BossName>
```

### Examples
```bash
.bb debug info "Fire Dragon"
.bb debug info TestBoss
```

### Output Example
```
🔧 Debug Info for 'Fire Dragon':
├─ Asset Name: CHAR_Wildlife_Wolf_VBlood
├─ PrefabGUID: -1905691330
├─ Name Hash: 847951234
├─ VBlood First Kill: false
├─ Entity ID: 532.15
├─ Has VBloodUnit: true
├─ Has Health: true
├─ Has UnitStats: true
├─ Physical Power: 150.5
├─ Spell Power: 100.0
├─ Physical Resistance: 0.25
└─ Spell Resistance: 0.15
```

### Information Provided
- **Asset Name**: Internal game asset reference
- **PrefabGUID**: Unique identifier for the unit type
- **Entity ID**: Current game entity reference
- **Components**: Which ECS components are attached
- **Stats**: Current combat statistics

---

## Simulate Death

Simulates a boss death without actually killing it.

### Syntax
```
.bb debug simulate-death <BossName> [KillerName]
```

### Parameters
- `BossName` - Boss to simulate death for
- `KillerName` - Optional, defaults to your character name

### Examples
```bash
# Simulate death with your name as killer
.bb debug simulate-death "Fire Dragon"

# Simulate death with specific killer
.bb debug simulate-death "Fire Dragon" "PlayerName"
```

### What Gets Simulated
1. Killer is added to boss records
2. Buff rewards are applied (if configured)
3. Death announcement is sent
4. Items are distributed
5. Boss is marked as defeated

### Output Example
```
🎭 Simulating death of boss 'Fire Dragon' killed by 'PlayerName'...
✅ Death simulation completed:
├─ Killer added: PlayerName
├─ Buffs applied: Yes
├─ Items dropped: 5 configured
└─ Boss despawned: Yes
```

---

## Force Item Drop

Forces a boss to drop its configured items without killing it.

### Syntax
```
.bb debug force-drop <BossName> [PlayerName]
```

### Parameters
- `BossName` - Boss to force drops from
- `PlayerName` - Optional recipient, defaults to command user

### Examples
```bash
# Drop items to yourself
.bb debug force-drop "Fire Dragon"

# Drop items to specific player
.bb debug force-drop "Fire Dragon" "PlayerName"
```

### Output Example
```
💰 Forcing item drop for boss 'Fire Dragon'...
✅ Items dropped successfully to:
└─ PlayerName
```

### Notes
- Boss doesn't need to be spawned
- Items follow normal drop chances
- Useful for testing loot tables

---

## Reset Kill Tracking

Clears the list of players who have killed a boss.

### Syntax
```
.bb debug reset-kills <BossName>
```

### Examples
```bash
.bb debug reset-kills "Fire Dragon"
.bb debug reset-kills TestBoss
```

### What Gets Reset
- Killer list cleared
- VBlood first kill flag reset
- Participation records removed
- Buff eligibility restored

### Output Example
```
🧹 Cleared 5 killers from boss 'Fire Dragon'
└─ VBlood first kill flag reset
```

### Use Cases
- Testing first-kill rewards
- Resetting after events
- Fixing bugged kill records

---

## Ability Debugging

### Debug Ability Search
Tests the VBlood search functionality.

#### Syntax
```
.bb ability-debug <SearchTerm>
```

#### Examples
```bash
.bb ability-debug alpha
.bb ability-debug wolf
.bb ability-debug frost
```

#### Output Example
```
🔍 Debugging search for: 'alpha'
Total VBloods loaded: 92
Sample VBloods:
  - 'Alpha the White Wolf'
  - 'Azariel the Sunbringer'
  - 'Beatrice the Tailor'
  ...
Exact match: None
Contains matches: 1
  - 'Alpha the White Wolf'
```

### Debug Boss Abilities
Shows current ability configuration for a spawned boss.

#### Syntax
```
.bb debug ability-info <BossName>
```

#### Requirements
- Boss must be currently spawned
- Shows real-time ability data

#### Output Example
```
🔍 Ability Debug Info for 'Fire Dragon':
Current Ability Configuration:
├─ Slot 0: Fireball (GUID: 123456)
├─ Slot 2: Fire Dash (GUID: 789012)
├─ Slot 4: Flame Burst (GUID: 345678)
└─ Total Custom Abilities: 3
```

---

## Additional Debug Tools

### Manual Boss Spawn
Force spawn a boss immediately at its configured location.

```bash
.bb start <BossName>
```

### Manual Boss Despawn (Deprecated)
Bosses now automatically despawn using the LifeTime system. Manual despawn is no longer necessary.

```bash
.bb despawn <BossName>  # This command may be removed in future versions
```

**Note**: Bosses will automatically disappear after their configured lifetime expires.

### Clear Map Icons
Remove all boss icons from the map.

```bash
.bb clearallicons
```

### Test Command
Generic test command for development.

```bash
.bb test
```

---

## Debug Workflows

### New Boss Testing
1. **Create and configure**
   ```bash
   .bb create TestBoss -1905691330
   ```

2. **Quick test spawn**
   ```bash
   .bb debug test TestBoss
   ```

3. **Check information**
   ```bash
   .bb debug info TestBoss
   ```

4. **Test mechanics**
   - Fight the boss
   - Observe behaviors
   - Check logs

5. **Simulate scenarios**
   ```bash
   .bb debug simulate-death TestBoss
   .bb debug force-drop TestBoss
   ```

### Troubleshooting Issues

#### Boss Not Spawning
```bash
# Check configuration
.bb status <BossName>
.bb debug info <BossName>

# Force spawn
.bb start <BossName>
```

#### Abilities Not Working
```bash
# Check ability configuration
.bb ability-slot-list <BossName>
.bb debug ability-info <BossName>

# Test compatibility
.bb ability-test <BossName> <VBlood> <Slot>
```

#### Items Not Dropping
```bash
# Check item configuration
.bb item list <BossName>

# Force drop test
.bb debug force-drop <BossName>
```

### Performance Testing

1. **Spawn multiple bosses**
   ```bash
   .bb debug test Boss1
   .bb debug test Boss2
   .bb debug test Boss3
   ```

2. **Monitor server performance**
   - Check CPU usage
   - Monitor memory
   - Watch for lag spikes

3. **Test mechanics under load**
   ```bash
   .bb mechanic-test Boss1 0
   .bb mechanic-test Boss2 0
   ```

---

## Best Practices

### Testing Checklist
- [ ] Boss spawns at correct location
- [ ] Health and stats are appropriate
- [ ] All abilities function correctly
- [ ] Mechanics trigger at right thresholds
- [ ] Items drop as configured
- [ ] Death behaviors work properly
- [ ] No performance issues

### Documentation
When testing, document:
- Unexpected behaviors
- Performance metrics
- Player feedback
- Balance observations

### Safety Measures
- Test on development server first
- Backup configurations before major changes
- Have rollback plan ready
- Monitor server logs during tests

---

## Common Issues

### "Boss entity not found"
- Boss isn't currently spawned
- Use `.bb debug test` to spawn first

### "Cannot simulate death - boss not spawned"
- Some debug commands require active boss
- Spawn boss before testing

### "Debug command failed"
- Check server logs for details
- Verify boss configuration is valid
- Ensure proper permissions

---

[← Back to Index](index.md)