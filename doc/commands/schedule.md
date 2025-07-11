# Schedule Commands

Schedule commands control when bosses automatically spawn in your world. You can set specific times, pause spawning, and manage complex schedules.

## Table of Contents
- [Set Schedule](#set-schedule)
- [Pause Timer](#pause-timer)
- [Resume Timer](#resume-timer)
- [List Schedules](#list-schedules)
- [Clear Schedule](#clear-schedule)
- [Schedule Best Practices](#schedule-best-practices)

---

## Set Schedule

Programs a boss to spawn at specific times during the day.

### Syntax
```
.bb schedule set <BossName> <HH:mm>
```

### Parameters
- `BossName` - Name of the boss to schedule
- `HH:mm` - Time in 24-hour format (00:00 to 23:59)

### Examples
```bash
# Spawn at 8 PM (20:00)
.bb schedule set "Fire Dragon" 20:00

# Spawn at midnight
.bb schedule set "Shadow Beast" 00:00

# Morning spawn at 6:30 AM
.bb schedule set "Dawn Stalker" 06:30
```

### Multiple Spawn Times
You can set multiple spawn times by running the command multiple times:
```bash
.bb schedule set "Fire Dragon" 20:00
.bb schedule set "Fire Dragon" 02:00
.bb schedule set "Fire Dragon" 14:00
```

### Important Notes
- Times are in server time, not client time
- Boss must have a location set to spawn
- Boss must be enabled for automatic spawning
- If a boss is already spawned, the next schedule is skipped

---

## Pause Timer

Temporarily stops a boss's spawn timer without removing the schedule.

### Syntax
```
.bb schedule pause <BossName>
```

### Examples
```bash
.bb schedule pause "Fire Dragon"
.bb schedule pause "Shadow Beast"
```

### What Happens
- Current timer stops
- Schedule is preserved
- Pause time is recorded
- Boss won't spawn until resumed

### Visual Feedback
```
⏸️ Boss 'Fire Dragon' timer paused at 15:45:30
```

### Use Cases
- Server maintenance periods
- Special events
- Debugging spawn issues
- Temporary content disable

---

## Resume Timer

Resumes a paused boss timer from where it left off.

### Syntax
```
.bb schedule resume <BossName>
```

### Examples
```bash
.bb schedule resume "Fire Dragon"
.bb schedule resume "Shadow Beast"
```

### Timer Calculation
When resumed, the timer accounts for:
- Time already elapsed before pause
- Duration of the pause
- Next scheduled spawn time

### Visual Feedback
```
▶️ Boss 'Fire Dragon' timer resumed (was paused for 45.5 minutes)
```

### Common Errors
```
Boss 'Fire Dragon' is not paused
```

---

## List Schedules

Shows all bosses with configured spawn schedules.

### Syntax
```
.bb schedule list
```

### Output Example
```
📅 Scheduled Bosses (4):
----------------------------
06:00 - Dawn Stalker ⭕ SCHEDULED
14:00 - Fire Dragon 🟢 ACTIVE
20:00 - Shadow Beast ⏸️ PAUSED
22:00 - Night Terror ⭕ SCHEDULED
----------------------------
```

### Status Indicators
- `🟢 ACTIVE` - Boss is currently spawned
- `⭕ SCHEDULED` - Waiting for spawn time
- `⏸️ PAUSED` - Timer is paused

---

## Clear Schedule

Removes all spawn schedules for a boss.

### Syntax
```
.bb schedule clear <BossName>
```

### Examples
```bash
.bb schedule clear "Fire Dragon"
.bb schedule clear "Shadow Beast"
```

### What Gets Cleared
- All scheduled spawn times
- Current timer state
- Pause status

### Visual Feedback
```
🗓️ Schedule cleared for boss 'Fire Dragon'
```

---

## Schedule Best Practices

### Planning Your Schedule

#### Peak Hours Strategy
```bash
# Lunch time boss
.bb schedule set "Noon Demon" 12:00

# Evening bosses for peak hours
.bb schedule set "Twilight Horror" 18:00
.bb schedule set "Night Stalker" 20:00
.bb schedule set "Midnight Terror" 00:00
```

#### Regular Intervals
```bash
# Every 4 hours
.bb schedule set "Elemental Lord" 00:00
.bb schedule set "Elemental Lord" 04:00
.bb schedule set "Elemental Lord" 08:00
.bb schedule set "Elemental Lord" 12:00
.bb schedule set "Elemental Lord" 16:00
.bb schedule set "Elemental Lord" 20:00
```

#### Weekend Special
```bash
# More spawns on weekends
# (Requires manual management or external scripts)
```

### Lifetime Considerations

Bosses now use an automatic LifeTime system for despawning. The boss will automatically disappear after the configured lifetime expires:

- **30-second boss**: Testing and events
- **30-minute boss**: Standard encounters
- **1-hour boss**: Raid-level content
- **Overlap Prevention**: Bosses automatically despawn via LifeTime, no manual scheduling needed

### Example Schedule Setup

```bash
# Short-lived mini-boss (15 min lifetime)
.bb schedule set "Scout Captain" 09:00
.bb schedule set "Scout Captain" 09:30
.bb schedule set "Scout Captain" 10:00

# Medium boss (30 min lifetime)
.bb schedule set "Elite Guard" 12:00
.bb schedule set "Elite Guard" 15:00
.bb schedule set "Elite Guard" 18:00

# Raid boss (2 hour lifetime)
.bb schedule set "Dragon Lord" 20:00  # Only once per day
```

### Time Zone Management

#### Server Time Display
The mod uses server time. To help players:
1. Display server time in MOTD
2. Use consistent time zone references
3. Consider international player base

#### Conversion Tips
- UTC example: 20:00 UTC = 3:00 PM EST
- Document your server's time zone
- Provide conversion tools/references

### Dynamic Scheduling

#### Weekday vs Weekend
Consider different schedules:
```bash
# Weekday - Less frequent
Monday-Friday: 18:00, 21:00

# Weekend - More spawns
Saturday-Sunday: 10:00, 14:00, 18:00, 21:00, 00:00
```

#### Event-Based Scheduling
- Special bosses during holidays
- Increased spawns during events
- Temporary bosses for limited time

### Troubleshooting Schedules

#### Boss Not Spawning on Schedule

1. **Check Prerequisites**
   ```bash
   .bb status <BossName>           # Is enabled?
   .bb location show <BossName>    # Has location?
   .bb schedule list               # Is scheduled?
   ```

2. **Verify Timer State**
   - Not paused
   - Not currently spawned
   - Server time is correct

3. **Test Manually**
   ```bash
   .bb debug test <BossName>
   ```

#### Multiple Bosses Same Time

Be careful with overlapping schedules:
```bash
# BAD - All spawn at once
.bb schedule set "Boss1" 20:00
.bb schedule set "Boss2" 20:00
.bb schedule set "Boss3" 20:00

# GOOD - Staggered spawns
.bb schedule set "Boss1" 20:00
.bb schedule set "Boss2" 20:15
.bb schedule set "Boss3" 20:30
```

### Advanced Patterns

#### Prime Time Rotation
```bash
# Boss A: Monday, Wednesday, Friday
.bb schedule set "Ancient Golem" 20:00

# Boss B: Tuesday, Thursday
.bb schedule set "Frost Wyrm" 20:00

# Boss C: Weekends
.bb schedule set "Shadow Lord" 20:00
```

#### Difficulty Progression
```bash
# Easy - Morning/Afternoon
.bb schedule set "Young Dragon" 10:00
.bb schedule set "Young Dragon" 14:00

# Medium - Evening
.bb schedule set "Adult Dragon" 18:00

# Hard - Night only
.bb schedule set "Ancient Dragon" 22:00
```

---

## Common Issues

### "Timer already paused"
- Boss is already in paused state
- Use `.bb schedule resume` first

### "No schedule to clear"
- Boss has no configured times
- Check with `.bb schedule list`

### "Invalid time format"
- Use HH:mm format (24-hour)
- Examples: 09:00, 14:30, 23:59

---

[← Back to Index](index.md) | [Next: Item Commands →](items.md)