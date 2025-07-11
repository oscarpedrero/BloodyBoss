# BloodyBoss v2.1.0 - Important Changes

## 🔄 Major System Changes

### Automatic Boss Despawn System
The biggest change in v2.1.0 is the complete overhaul of how bosses despawn:

#### Before (v2.0.0 and earlier)
- Bosses had both spawn time (`Hour`) and despawn time (`HourDespawn`)
- Manual despawn system checked every second
- Complex scheduling required
- Potential for timing conflicts

#### Now (v2.1.0)
- Bosses only have spawn time (`Hour`) 
- Automatic despawn using Unity's LifeTime component
- Set lifetime in seconds: `.bb stats "BossName" lifetime 1800`
- Boss disappears automatically when lifetime expires
- No manual intervention needed

### Example Configuration Change

**Old way (no longer works):**
```json
{
  "name": "TestBoss",
  "Hour": "20:00",
  "HourDespawn": "20:30",  // This field is removed!
  ...
}
```

**New way:**
```json
{
  "name": "TestBoss", 
  "Hour": "20:00",
  "Lifetime": 1800,  // Boss lasts 30 minutes (1800 seconds)
  ...
}
```

## 🚫 Removed Features

### Commands/Methods No Longer Available
- `.bb despawn <BossName>` - Deprecated, bosses despawn automatically
- `SetHourDespawn()` method - Removed from codebase
- `HourDespawn` field - No longer exists in configuration

### Internal Systems Removed
- `EntityDestroyHook` - Replaced with BossTrackingSystem
- `VWorld.cs` utility class - Use `Plugin.SystemsCore.EntityManager`
- Manual despawn checking - Now handled by LifeTime component
- `BuffAnalyzer` - No longer needed

## ✅ Improvements

### Performance
- O(1) boss tracking instead of O(n²)
- Reduced server load with event-driven system
- Eliminated redundant timer checks
- Optimized memory usage with proper cleanup

### Stability
- Fixed server crashes during boss spawn
- Improved null safety throughout codebase
- Better error handling and recovery
- Thread-safe ECS operations

### Simplicity
- Cleaner configuration files
- Less manual management required
- Automatic cleanup on disconnect
- Simplified damage tracking

## 📝 Migration Guide

### For Server Admins

1. **Update Your Boss Configurations**
   - Remove any `HourDespawn` entries from Bosses.json
   - Ensure all bosses have appropriate `Lifetime` values
   - Default lifetime is 300 seconds (5 minutes) if not specified

2. **Update Any Scripts**
   - Remove calls to `.bb despawn`
   - Remove any automation around despawn times
   - Focus on spawn scheduling only

3. **Test Your Bosses**
   ```bash
   .bb debug test "YourBossName"
   ```
   Watch the lifetime countdown and ensure proper despawn

### For Players
- No action needed! Bosses will behave more reliably
- Boss timers are now shown in UI during combat
- Despawn is predictable based on lifetime

## 🎯 Best Practices Going Forward

### Lifetime Recommendations
- **Test/Event Boss**: 300-600 seconds (5-10 minutes)
- **Standard Boss**: 1800 seconds (30 minutes)  
- **Raid Boss**: 3600-7200 seconds (1-2 hours)
- **World Boss**: 7200+ seconds (2+ hours)

### Scheduling Tips
- Don't worry about despawn conflicts anymore
- Can schedule bosses closer together
- Lifetime ensures clean transitions
- Focus on player experience, not technical timing

## 🐛 Known Issues Fixed

- ✅ Boss spawning multiple times per minute
- ✅ Memory leaks from uncleaned boss data
- ✅ Server crashes from thread-unsafe operations
- ✅ Damage tracking correlation problems
- ✅ Boss cleanup on server restart

## 💡 Tips for Success

1. **Start Fresh**: Clear your boss database and recreate if having issues
2. **Set Reasonable Lifetimes**: Too short frustrates players, too long blocks respawns
3. **Monitor First Spawn**: Watch logs to ensure proper behavior
4. **Use Debug Commands**: `.bb debug range` shows active boss info

## 🆘 Troubleshooting

### "Boss disappeared too quickly"
- Check lifetime setting: `.bb status "BossName"`
- Increase lifetime: `.bb stats "BossName" lifetime 3600`

### "Boss won't despawn"
- This should not happen in v2.1.0
- Check if boss entity exists: `.bb debug test "BossName"`
- Restart server if necessary (very rare)

### "Old commands not working"
- `.bb despawn` is deprecated
- `HourDespawn` no longer exists
- Focus on lifetime-based management

---

*Thank you for using BloodyBoss! These changes make the mod more stable, performant, and easier to use.*