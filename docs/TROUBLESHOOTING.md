# 🛠️ Troubleshooting Guide

This guide helps you diagnose and fix common issues with BloodyBoss v2.1.0. Follow the step-by-step solutions to get your boss encounters working perfectly.

## 🚨 Quick Diagnostic Steps

Before diving into specific issues, run these quick checks:

1. **Check server console** for error messages
2. **Verify mod loading** - look for "BloodyBoss is loaded!" message
3. **Test basic functionality** - run `.bb list` command
4. **Check configuration** - ensure files exist and are valid

## 🔧 Installation Issues

### Mod Not Loading

**Symptoms:**
- No BloodyBoss messages in console
- `.bb` commands don't work
- Missing configuration files

**Solutions:**

1. **Verify file placement:**
   ```
   BepInEx/plugins/
   ├── VampireCommandFramework.dll ✓
   ├── Bloody.Core.dll ✓
   └── BloodyBoss.dll ✓
   ```

2. **Check dependencies:**
   ```bash
   # Look for these messages in console:
   [Info] VampireCommandFramework loaded
   [Info] Bloody.Core loaded  
   [Info] BloodyBoss is loaded!
   ```

3. **Verify versions:**
   - BepInEx: 1.733.2
   - VampireCommandFramework: 0.10.4
   - Bloody.Core: 2.0.0

4. **Check permissions:**
   - Ensure server has write access to `BepInEx/config/`
   - Run server as administrator if needed

### Commands Not Working

**Symptoms:**
- "Command not found" errors
- `.bb` prefix not recognized

**Solutions:**

1. **Check VampireCommandFramework:**
   ```bash
   # Server console should show:
   [Info] VampireCommandFramework: Loaded X commands
   ```

2. **Verify admin permissions:**
   ```bash
   # In-game, check if you have admin status
   # Most .bb commands require admin privileges
   ```

3. **Test with basic command:**
   ```bash
   .bb list
   # Should show boss list or "no bosses created"
   ```

## ⚙️ Configuration Problems

### Configuration File Missing

**Symptoms:**
- `BloodyBoss.cfg` doesn't exist
- Default settings not working

**Solutions:**

1. **Manual file creation:**
   ```bash
   # Create the config directory
   mkdir -p BepInEx/config/BloodyBoss/
   
   # Restart server to generate config
   ```

2. **Check file permissions:**
   ```bash
   # Ensure server can write to config folder
   chmod 755 BepInEx/config/
   ```

3. **Force regeneration:**
   ```bash
   # Delete config and restart server
   rm BepInEx/config/BloodyBoss.cfg
   # Server will recreate with defaults
   ```

### Invalid Configuration Values

**Symptoms:**
- Features not working as expected
- Error messages about invalid config

**Solutions:**

1. **Check value ranges:**
   ```ini
   # Valid ranges:
   BaseHealthMultiplier = 0.1 to 10.0
   HealthPerPlayer = 0.0 to 2.0
   DamagePerPlayer = 0.0 to 2.0
   MaxPlayersForScaling = 1 to 50
   ```

2. **Fix boolean values:**
   ```ini
   # Correct:
   Enable = true
   Enable = false
   
   # Incorrect:
   Enable = yes
   Enable = 1
   ```

3. **Validate message templates:**
   ```ini
   # Check for valid placeholders:
   NormalTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase#
   # Avoid special characters that might break parsing
   ```

## 🎯 Boss Creation Issues

### Boss Not Creating

**Symptoms:**
- "Boss creation failed" error
- Boss doesn't appear in `.bb list`

**Solutions:**

1. **Check PrefabGUID validity:**
   ```bash
   # Use a known VBlood PrefabGUID:
   .bb create "Test Boss" -1905691330 50 1 1800
   
   # Reference: https://wiki.vrisingmods.com/prefabs/
   ```

2. **Verify name uniqueness:**
   ```bash
   # Each boss must have unique name
   .bb remove "Existing Boss"  # Remove duplicate if needed
   .bb create "New Unique Name" -1905691330 50 1 1800
   ```

3. **Check parameter ranges:**
   ```bash
   # Valid parameter ranges:
   Level: 1-100
   Multiplier: 0.1-50.0
   Lifetime: 60-86400 seconds
   ```

### Boss Not Spawning

**Symptoms:**
- Boss created but doesn't spawn at scheduled time
- Manual spawn fails

**Solutions:**

1. **Verify location is set:**
   ```bash
   .bb set location "Boss Name"
   .bb status "Boss Name"  # Check coordinates
   ```

2. **Check spawn time format:**
   ```bash
   # Correct format (24-hour):
   .bb set hour "Boss Name" 20:00
   .bb set hour "Boss Name" 09:30
   
   # Incorrect:
   .bb set hour "Boss Name" 8:00 PM
   ```

3. **Test manual spawn:**
   ```bash
   .bb start "Boss Name"
   # If this works, timing is the issue
   ```

4. **Check server time:**
   ```bash
   .bb test "Boss Name"
   # Shows current server time + 1 minute
   ```

## 🎮 Gameplay Issues

### Scaling Not Working

**Symptoms:**
- Boss difficulty doesn't change with player count
- Dynamic scaling announcements missing

**Solutions:**

1. **Enable dynamic scaling:**
   ```ini
   [Dynamic Scaling]
   Enable = true
   ```

2. **Check scaling values:**
   ```ini
   BaseHealthMultiplier = 1.5    # Must be > 0
   HealthPerPlayer = 0.25        # Must be >= 0
   DamagePerPlayer = 0.15        # Must be >= 0
   ```

3. **Verify player count:**
   ```bash
   .bb debug "Boss Name"
   # Shows actual player count used for scaling
   ```

4. **Test with multiple players:**
   - Have different numbers of players online
   - Create new boss to see scaling differences

### Progressive Difficulty Not Working

**Symptoms:**
- Boss difficulty doesn't increase with consecutive spawns
- Consecutive count not tracking

**Solutions:**

1. **Enable progressive difficulty:**
   ```ini
   [Progressive Difficulty]
   Enable = true
   DifficultyIncrease = 0.15
   ```

2. **Check consecutive tracking:**
   ```bash
   .bb status "Boss Name"
   # Shows "Consecutive Spawns" count
   ```

3. **Reset if stuck:**
   ```bash
   .bb resetkills "Boss Name"
   # Clears consecutive counter
   ```

### Phase Announcements Missing

**Symptoms:**
- No phase change messages
- Incorrect phase information

**Solutions:**

1. **Enable announcements:**
   ```ini
   [Phase Announcements]
   Enable = true
   ```

2. **Check message templates:**
   ```ini
   # Ensure templates are not empty:
   NormalTemplate = ⚔️ #bossname# [#phasename#] - Phase #phase#
   ```

3. **Test manually:**
   ```bash
   .bb simulate "Boss Name"
   # Should trigger phase announcement
   ```

4. **Check placeholder syntax:**
   ```ini
   # Correct placeholders:
   #bossname# #phasename# #phase# #players# #damage#
   
   # Incorrect:
   {bossname} [phasename] $phase$ %players%
   ```

## 🗺️ Map and Icon Issues

### Icons Stuck on Map

**Symptoms:**
- Boss icons remain after boss death
- Multiple icons for same boss

**Solutions:**

1. **Clear specific icon:**
   ```bash
   .bb clearicon "Boss Name"
   ```

2. **Clear all icons:**
   ```bash
   .bb clearallicons
   ```

3. **Prevent future issues:**
   - Don't force-kill server during boss fights
   - Use `.bb despawn` instead of manual entity removal

### Teleportation Problems

**Symptoms:**
- Teleport command fails
- Wrong teleport location
- Cooldown/cost issues

**Solutions:**

1. **Check teleport configuration:**
   ```ini
   [Teleport]
   Enable = true
   AdminOnly = false  # For player access
   ```

2. **Verify permissions:**
   ```bash
   # If AdminOnly = true, only admins can teleport
   # Check admin status in-game
   ```

3. **Check cooldown:**
   ```bash
   .bb teleport "Boss Name"
   # Error message shows remaining cooldown time
   ```

4. **Verify item costs:**
   ```bash
   # Check inventory for required items
   # Cost items consumed on successful teleport only
   ```

5. **Test location validity:**
   ```bash
   .bb status "Boss Name"  # Shows coordinates
   .bb debug "Boss Name"   # Shows entity information
   ```

## 🎁 Reward and Drop Issues

### Items Not Dropping

**Symptoms:**
- Boss defeated but no items received
- Partial item drops

**Solutions:**

1. **Check boss killers:**
   ```bash
   .bb status "Boss Name"
   # Shows current killers count
   ```

2. **Verify item configuration:**
   ```bash
   .bb items list "Boss Name"
   # Shows all configured items and chances
   ```

3. **Test drop chances:**
   ```bash
   .bb forcedrop "Boss Name"
   # Forces item calculation and drop
   ```

4. **Check inventory space:**
   - Ensure players have inventory space
   - Items drop on ground if inventory full

### Incorrect Drop Chances

**Symptoms:**
- Items drop too frequently/rarely
- Drop rates don't match configuration

**Solutions:**

1. **Verify chance values:**
   ```bash
   # Valid range: 1-100
   .bb items add "Boss" "Item" 12345 10 75  # 75% chance
   ```

2. **Test with simulation:**
   ```bash
   .bb simulate "Boss Name"
   # Shows what items would drop
   ```

3. **Understanding RNG:**
   - Drop chances are per item, not guaranteed
   - Multiple runs needed to see true rates
   - Each player rolls separately

## 🔄 Database and Save Issues

### Database Corruption

**Symptoms:**
- Bosses disappear from list
- Configuration changes lost
- JSON parse errors

**Solutions:**

1. **Backup and restore:**
   ```bash
   # Create backup
   cp BepInEx/config/BloodyBoss/Database.json Database.backup
   
   # Restore from backup
   cp Database.backup BepInEx/config/BloodyBoss/Database.json
   .bb reload
   ```

2. **Validate JSON:**
   ```bash
   # Use online JSON validator
   # Check for missing commas, brackets, quotes
   ```

3. **Reset database:**
   ```bash
   # Remove corrupted database
   rm BepInEx/config/BloodyBoss/Database.json
   # Restart server to recreate empty database
   ```

### Save Issues

**Symptoms:**
- Changes not persisting
- Configuration resets on restart

**Solutions:**

1. **Check file permissions:**
   ```bash
   # Ensure server can write to files
   chmod 644 BepInEx/config/BloodyBoss/Database.json
   ```

2. **Verify disk space:**
   ```bash
   df -h  # Check available disk space
   ```

3. **Manual save trigger:**
   ```bash
   .bb reload  # Forces save of current state
   ```

## 📊 Performance Issues

### Server Lag During Boss Events

**Symptoms:**
- Server FPS drops during boss spawns
- Lag during phase announcements

**Solutions:**

1. **Reduce announcement frequency:**
   ```ini
   [Phase Announcements]
   AnnounceEveryPhase = false
   AnnounceMilestoneSpawns = false
   ```

2. **Optimize scaling calculations:**
   ```ini
   [Dynamic Scaling]
   MaxPlayersForScaling = 10  # Lower cap
   ```

3. **Simplify messages:**
   ```ini
   # Use shorter, simpler templates
   NormalTemplate = #bossname# - Phase #phase#
   ```

### Memory Usage Issues

**Symptoms:**
- Server memory usage increases over time
- Eventual server crashes

**Solutions:**

1. **Regular icon cleanup:**
   ```bash
   # Schedule regular cleanup
   .bb clearallicons
   ```

2. **Reset long-running bosses:**
   ```bash
   # Reset bosses with many consecutive spawns
   .bb resetkills "Boss Name"
   ```

3. **Monitor entity count:**
   ```bash
   .bb debug "Boss Name"  # Check entity validity
   ```

## 🆘 Getting Help

### Diagnostic Information to Collect

When seeking help, provide:

1. **Server information:**
   - Operating system
   - V Rising server version
   - BloodyBoss version

2. **Error messages:**
   - Full console output
   - Exact error text
   - When the error occurs

3. **Configuration:**
   - Relevant sections of `BloodyBoss.cfg`
   - Boss configuration (from `.bb list` and `.bb status`)

4. **Steps to reproduce:**
   - What you were trying to do
   - Commands used
   - Expected vs. actual behavior

### Useful Debug Commands

```bash
# System information
.bb list                    # All configured bosses
.bb status "Boss Name"      # Detailed boss info
.bb debug "Boss Name"       # Technical details

# Test functionality
.bb test "Boss Name"        # Quick spawn test
.bb simulate "Boss Name"    # Test death mechanics
.bb forcedrop "Boss Name"   # Test item drops

# Cleanup commands
.bb clearallicons          # Fix icon issues
.bb resetkills "Boss Name" # Reset tracking
.bb reload                 # Reload configuration
```

### Community Resources

- 🎮 [V Rising Mod Community Discord](https://discord.gg/vrisingmods)
- 📖 [V Rising Modding Wiki](https://wiki.vrisingmods.com/)
- 🔧 [BloodyBoss GitHub Issues](https://github.com/your-repo/issues)
- 💬 [Mod Support Forums](https://thunderstore.io/c/v-rising/)

### When to File a Bug Report

File a bug report if:
- ✅ You've followed all troubleshooting steps
- ✅ Issue persists with default configuration
- ✅ Error occurs with minimal mod setup
- ✅ You can reproduce the issue consistently

Include in your bug report:
- **Detailed description** of the issue
- **Steps to reproduce** the problem
- **Expected behavior** vs actual behavior
- **System information** and mod versions
- **Console logs** showing errors
- **Configuration files** if relevant

---

*Still having issues? The [V Rising Mod Community](https://discord.gg/vrisingmods) is here to help!*