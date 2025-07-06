# 📋 Changelog

Complete version history for BloodyBoss with detailed changes, improvements, and fixes.

## 🚀 v2.1.0 - Dynamic Scaling & Advanced Features
*Released: 2024-01-XX*

### ✨ Major New Features

**🎯 Dynamic Scaling System**
- Automatic boss scaling based on online player count
- Separate multipliers for health and damage
- Configurable maximum player caps
- Real-time difficulty adjustment

**📈 Progressive Difficulty System**
- Consecutive spawn tracking for escalating challenges
- Configurable difficulty increase per spawn
- Automatic reset on boss defeat
- Maximum difficulty caps to prevent impossible encounters

**🎭 Phase Announcement System**
- Dynamic phase notifications with customizable messages
- Color-coded difficulty levels (Normal, Hard, Epic, Legendary)
- Multi-language support with translatable templates
- Configurable announcement frequency and triggers

**🎮 Advanced Command System**
- `.bb despawn` - Force despawn bosses immediately
- `.bb pause` / `.bb resume` - Control boss timers
- `.bb status` - Detailed boss information display
- `.bb debug` - Technical debugging information
- `.bb simulate` - Test boss mechanics safely
- `.bb resetkills` - Clear boss kill tracking
- `.bb forcedrop` - Manually trigger item drops
- `.bb teleport` - Configurable teleportation system

**🌐 Teleportation System**
- Player/admin configurable access
- Cooldown management system
- Item cost requirements
- Safety restrictions (alive bosses, active encounters)

### 🔧 Core Improvements

**⚡ Performance Optimizations**
- Timer system now uses numeric comparisons instead of string parsing
- Reduced memory footprint with proper entity disposal
- Optimized scaling calculations with value caching
- Smart announcement system prevents unnecessary messages

**🛡️ Enhanced Error Handling**
- Comprehensive try-catch blocks throughout codebase
- Detailed error logging for easier debugging
- Graceful degradation when systems fail
- Better validation of user inputs and configuration

**📊 Improved Logging**
- More detailed console output for admin monitoring
- Debug information for troubleshooting
- Performance metrics for system monitoring
- Clear error messages with suggested solutions

### ⚙️ Configuration Enhancements

**New Configuration Sections:**
- `[Dynamic Scaling]` - Control automatic difficulty adjustment
- `[Progressive Difficulty]` - Configure escalating challenges
- `[Teleport]` - Manage player transportation system
- `[Phase Announcements]` - Control notification system
- `[Phase Messages]` - Customize announcement templates
- `[Phase Names]` - Translatable phase names

**Enhanced Customization:**
- Over 20 new configuration options
- Placeholder system for dynamic messages
- Multi-language template support
- Per-feature enable/disable controls

### 🌍 Internationalization

**Full Multi-language Support:**
- Translatable phase names and messages
- Configurable message templates with placeholders
- Support for custom prefixes and suffixes
- Easy adaptation to any language

**Included Examples:**
- English (default)
- Spanish translations
- German translations
- French translations

### 🔄 Dependency Updates

**Updated Requirements:**
- BepInEx: Updated to 1.733.2
- VampireCommandFramework: Updated to 0.10.4
- Bloody.Core: Updated to 2.0.0

### 🐛 Bug Fixes

- Fixed accessibility modifier inconsistencies in new systems
- Resolved VWorld reference issues in announcement system
- Corrected color method fallbacks for unsupported colors
- Fixed using statement organization and dependencies

---

## 🔧 v2.0.0 - Oakveil Compatibility Update
*Released: 2024-XX-XX*

### 🔄 Major Updates

**Oakveil Compatibility**
- Updated for V Rising Oakveil patch compatibility
- Verified functionality with latest game version
- Updated entity handling for new ECS systems

**🗑️ Removed Deprecated Features**
- Removed deprecated UnitStats properties:
  - PhysicalCriticalStrikeDamage
  - SpellCriticalStrikeChance  
  - SpellCriticalStrikeDamage
  - ResourceYieldModifier
  - ReducedResourceDurabilityLoss
  - SilverResistance
  - SilverCoinResistance
  - GarlicResistance
  - PassiveHealthRegen
  - HealthRecovery
  - DamageReduction
  - HealingReceived
  - ShieldAbsorbModifier

**📦 Dependency Changes**
- Removed Bloodstone dependency (no longer needed)
- Updated to use BloodyCore 2.0.0 framework
- Streamlined dependency requirements

### 🐛 Critical Bug Fixes

**Boss Despawn Issues**
- Fixed bosses dropping loot when despawning due to timeout
- Improved lifetime and despawn time calculations
- Better handling of boss entity cleanup

**Performance Improvements**
- Optimized entity queries for better performance
- Reduced memory usage with proper disposal patterns
- Improved timer accuracy and reliability

---

## 🔄 v1.1.14 - Stability Improvements
*Released: 2023-XX-XX*

### 🐛 Bug Fixes
- Fixed spawn timer stopping when bosses lacked spawn times
- Resolved team assignment issues causing bosses to attack each other
- Improved error handling for edge cases

---

## ✨ v1.1.13 - Random Boss Features  
*Released: 2023-XX-XX*

### 🎲 New Features
- Added random boss spawning at original boss locations
- Bosses can now spawn randomly instead of specific scheduled spawns
- Configurable random boss selection system

---

## 🗺️ v1.1.12 - Map Icon Management
*Released: 2023-XX-XX*

### ✨ New Features
- Added `.bb clearallicons` command for removing stuck map icons
- Improved icon cleanup and management system

### 🔧 Improvements  
- VBlood reward system restored to original behavior
- Removed trailing commas from player names in messages
- Cleaned up WorldBossFinalConcatCharacters parameter

---

## 🛡️ v1.1.11 - VBlood Detection Fix
*Released: 2023-XX-XX*

### 🐛 Critical Fixes
- Fixed error when Vblood/NPC spawned by BloodyBoss appears in game
- Resolved reward conflicts with original VBlood encounters
- Improved VBlood detection and handling system

---

## 🎁 v1.1.10 - Reward System Fixes
*Released: 2023-XX-XX*

### 🐛 Bug Fixes
- Fixed reward distribution when two bosses with same PrefabGUID die simultaneously
- Resolved BloodConsumeSource component issues affecting some NPCs
- Improved spawn success rate for problematic entities

---

## 🩸 v1.1.9 - VBlood Recognition
*Released: 2023-XX-XX*

### 🐛 Fixes
- Fixed VBlood recognition issues
- Improved VBlood detection system reliability

---

## 📢 v1.1.8 - Message System Fix
*Released: 2023-XX-XX*

### 🐛 Fixes
- Fixed duplicate messages when multiple vampires kill world boss
- Improved message broadcasting system

---

## 🔄 v1.1.7 - Persistence & Team Features
*Released: 2023-XX-XX*

### 🛡️ Major Fixes
- Fixed boss statistics not persisting through server restarts
- Resolved boss spawning issues with server downtime
- Fixed boss spawning when no players online

### ✨ New Features
- Added buff animation for participating players
- Team boss system - bosses can work together instead of fighting
- Configurable team behavior for multiple boss encounters

---

## ⚔️ v1.1.6 - Minion Damage Control
*Released: 2023-XX-XX*

### ✨ New Features
- Added option to enable/disable minion damage to bosses
- Configurable minion interaction system

---

## 🗺️ v1.1.5 - Icon Bug Fix
*Released: 2023-XX-XX*

### 🐛 Fixes
- Fixed double icon display issue
- Improved map icon management

---

## ⚡ v1.1.4 - Core System Improvements
*Released: 2023-XX-XX*

### 🔧 Major Improvements
- Updated timer system to use CoroutineHandler
- Improved minion interaction system
- Removed VBlood biting from non-VBlood NPCs

### 🐛 Bug Fixes
- Fixed VBlood vs non-VBlood message inconsistencies  
- Resolved item duplication bugs
- Fixed inventory vs ground drop issues

---

## 📢 v1.1.3 - Message System Fix
*Released: 2023-XX-XX*

### 🐛 Fixes
- Fixed duplicate messages when killing NPC bosses
- Improved message broadcasting reliability

---

## ⚡ v1.1.2 - Performance Update
*Released: 2023-XX-XX*

### 🔧 Improvements
- Performance optimizations across the codebase
- Reduced server load during boss encounters
- Improved memory management

---

## 🎯 v1.1.1 - NPC Boss Support
*Released: 2023-XX-XX*

### ✨ Major Features
- Added support for any NPC as boss (not just VBloods)
- Updated to latest Bloody.Core version
- Expanded boss creation possibilities

---

## 🛡️ v1.1.0 - Core Stability & Config Changes
*Released: 2023-XX-XX*

### 🐛 Critical Fixes
- Fixed blood consumption bugs with multiple players
- Resolved server crashes during mass VBlood consumption

### 🔧 Configuration Changes
- Moved configuration files to root config folder
- Improved configuration organization and accessibility

---

## 🔧 v1.0.8 - Admin Tools & Drop Control
*Released: 2023-XX-XX*

### ✨ New Features
- Added VBlood validation protection
- Database reload command for manual JSON edits
- Option to clear original boss drop tables

### 🛡️ Security Improvements
- Protection against invalid VBlood PrefabGUIDs
- Better validation of boss creation parameters

---

## 🎯 v1.0.7 - Conflict Resolution
*Released: 2023-XX-XX*

### 🐛 Critical Fixes
- Fixed reward conflicts between BloodyBoss and default VBloods
- Resolved message conflicts with original game VBloods
- Improved boss identification system

---

## 📦 v1.0.6 - Framework Integration
*Released: 2023-XX-XX*

### 🔧 Architecture Changes
- Integrated Bloody.Core as framework dependency
- Removed direct DLL dependency
- Improved drop calculation formula accuracy

---

## 🗺️ v1.0.5 - Icon Management
*Released: 2023-XX-XX*

### ✨ New Features
- Added `.bb clearicon` command
- Basic map icon management system

---

## 👥 v1.0.4 - Player Multiplier
*Released: 2023-XX-XX*

### ✨ New Features
- Added PlayersOnlineMultiplier configuration option
- Toggle between fixed and player-based multipliers
- Enhanced scaling options for different server sizes

---

## 🔢 v1.0.3 - Multiplier Fixes
*Released: 2023-XX-XX*

### 🐛 Critical Fixes
- Fixed boss HP multiplier using total registered users instead of online users
- Resolved autospawn compatibility issues with other mods
- Improved boss spawning system reliability
- Fixed VBlood reward conflicts with default game systems

---

## 🎉 v1.0.0 - Initial Release
*Released: 2023-XX-XX*

### ✨ Initial Features
- Basic boss creation and management system
- Scheduled boss spawning
- Item reward system with configurable drop chances
- Map icon integration
- Admin command system
- Configuration file support
- VBlood integration and detection

### 🎮 Core Commands
- Boss creation, removal, and listing
- Location and timing configuration
- Item management system
- Manual boss spawning
- Basic administrative controls

---

## 🔄 Migration Notes

### Upgrading from v1.x to v2.1.0

**⚠️ Important:** This is a major version upgrade with significant changes.

**Required Actions:**
1. **Backup** your current configuration and database
2. **Update dependencies** to required versions
3. **Review new configuration options** in BloodyBoss.cfg
4. **Test thoroughly** before deploying to production

**New Configuration Sections:**
- Enable dynamic scaling: `[Dynamic Scaling] Enable = true`
- Enable progressive difficulty: `[Progressive Difficulty] Enable = true`
- Configure phase announcements: `[Phase Announcements] Enable = true`
- Set up teleportation: `[Teleport] Enable = true`

**Compatibility:**
- ✅ Existing boss configurations will be preserved
- ✅ All old commands continue to work
- ✅ Database format is backward compatible
- ⚠️ New features require configuration to enable

### Upgrading from v2.0.x to v2.1.0

**Recommended Actions:**
1. **Backup** configuration files
2. **Review new settings** in config file
3. **Configure new features** as desired
4. **Test advanced commands** in development environment

**New Features Available:**
- All v2.1.0 features are additions, no breaking changes
- Existing configurations will continue to work
- New settings have sensible defaults

---

*For technical support and questions about any version, visit the [V Rising Mod Community Discord](https://discord.gg/vrisingmods).*