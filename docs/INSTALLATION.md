# 📦 Installation Guide

This guide will walk you through installing BloodyBoss v2.1.0 and all its dependencies for V Rising servers.

## 📋 Prerequisites

Before installing BloodyBoss, ensure your V Rising server meets these requirements:

- ✅ **V Rising Dedicated Server** (Oakveil compatible)
- ✅ **Windows or Linux** server environment
- ✅ **Administrator access** to server files
- ✅ **Basic command line knowledge** (recommended)

## 🔧 Required Dependencies

BloodyBoss requires the following mods to function properly. Install them in this exact order:

### 1. BepInEx 1.733.2
**Essential framework for V Rising modding**

1. Download [BepInEx 1.733.2](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/)
2. Extract to your V Rising server root directory
3. The structure should look like:
   ```
   VRising_Server/
   ├── BepInEx/
   │   ├── core/
   │   ├── plugins/
   │   └── config/
   ├── VRisingServer_Data/
   └── VRisingServer.exe
   ```

### 2. VampireCommandFramework 0.10.4
**Command system framework**

1. Download [VampireCommandFramework 0.10.4](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/)
2. Place `VampireCommandFramework.dll` in `BepInEx/plugins/`

### 3. Bloody.Core 2.0.0
**Core framework for Bloody mods**

1. Download [Bloody.Core 2.0.0](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/)
2. Place `Bloody.Core.dll` in `BepInEx/plugins/`

## 🎯 Installing BloodyBoss

### Method 1: Thunderstore (Recommended)

1. Visit [BloodyBoss on Thunderstore](https://thunderstore.io/c/v-rising/p/Trodi/BloodyBoss/)
2. Click **"Manual Download"**
3. Extract `BloodyBoss.dll` to `BepInEx/plugins/`

### Method 2: Manual Installation

1. Download the latest release from GitHub
2. Copy `BloodyBoss.dll` to `BepInEx/plugins/`

### Final Directory Structure

After installation, your plugins folder should contain:
```
BepInEx/plugins/
├── VampireCommandFramework.dll
├── Bloody.Core.dll
└── BloodyBoss.dll
```

## 🚀 First Launch

### 1. Start Your Server
Launch your V Rising server normally. On first run, BloodyBoss will:
- ✅ Generate configuration files
- ✅ Create database files
- ✅ Initialize all systems

### 2. Verify Installation
Check the server console for these messages:
```
[Info   :   BloodyBoss] Plugin BloodyBoss is loaded!
[Info   :   BloodyBoss] Start Timer for BloodyBoss
[Info   :   BloodyBoss] BloodyBoss v2.1.0 initialized successfully
```

### 3. Check Generated Files
Verify these files were created:
```
BepInEx/config/
├── BloodyBoss.cfg          # Main configuration
└── BloodyBoss/
    └── Database.json       # Boss database
```

## ⚙️ Basic Configuration

### Enable Dynamic Features
Edit `BepInEx/config/BloodyBoss.cfg`:

```ini
[Main]
Enabled = true

[Dynamic Scaling]
Enable = true
BaseHealthMultiplier = 1.5
HealthPerPlayer = 0.25
DamagePerPlayer = 0.15

[Progressive Difficulty]
Enable = true
DifficultyIncrease = 0.1
MaxDifficultyMultiplier = 2.0

[Phase Announcements]
Enable = true
```

### Test Installation
Connect to your server and run:
```bash
.bb list
```

If you see "There are no boss created", the installation was successful!

## 🛠️ Troubleshooting

### Common Issues

#### ❌ "Command not found" error
**Cause**: VampireCommandFramework not installed correctly
**Solution**: 
1. Verify `VampireCommandFramework.dll` is in `BepInEx/plugins/`
2. Check server console for VCF loading errors
3. Restart server completely

#### ❌ BloodyBoss not loading
**Cause**: Missing dependencies or wrong load order
**Solution**:
1. Verify all dependencies are installed
2. Check file versions match requirements
3. Remove any conflicting mods

#### ❌ Configuration not generating
**Cause**: Insufficient permissions or folder lock
**Solution**:
1. Run server as administrator
2. Check folder permissions
3. Manually create `BepInEx/config/BloodyBoss/` folder

#### ❌ Database corruption
**Cause**: Server crash during save or manual edit error
**Solution**:
```bash
# Backup and reset database
mv BepInEx/config/BloodyBoss/Database.json BepInEx/config/BloodyBoss/Database.json.backup
# Restart server to regenerate
```

### Getting Help

If you encounter issues:

1. **Check server console** for error messages
2. **Verify dependencies** are correct versions  
3. **Review configuration** for syntax errors
4. **Search existing issues** on GitHub
5. **Ask for help** in the [V Rising Mod Community Discord](https://discord.gg/vrisingmods)

### Debug Information

To get detailed debug info for support:
```bash
.bb debug "YourBossName"  # Replace with actual boss name
```

## 🔄 Updating BloodyBoss

### From v2.0.x to v2.1.0

1. **Backup your configuration**:
   ```bash
   cp -r BepInEx/config/BloodyBoss/ BepInEx/config/BloodyBoss_backup/
   ```

2. **Replace the DLL**:
   - Remove old `BloodyBoss.dll`
   - Install new version

3. **Start server** - configuration will auto-migrate

4. **Review new settings** in `BloodyBoss.cfg`

### From v1.x to v2.1.0

⚠️ **Major version upgrade** - follow these steps carefully:

1. **Complete backup** of server and configs
2. **Update all dependencies** to required versions
3. **Clean install** BloodyBoss v2.1.0
4. **Manually recreate bosses** (database format changed)
5. **Configure new features** as desired

## 🎮 Quick Setup Example

After installation, create your first boss:

```bash
# Create a boss
.bb create "Test Boss" -1905691330 90 2 1800

# Set location (stand where you want it to spawn)
.bb set location "Test Boss"

# Set spawn time  
.bb set hour "Test Boss" 20:00

# Add some rewards
.bb items add "Test Boss" "Blood Essence" 1055853475 50 100

# Start immediately for testing
.bb start "Test Boss"
```

## 📚 Next Steps

- 📖 [Configuration Guide](CONFIGURATION.md) - Configure all settings
- 🎮 [Commands Reference](COMMANDS.md) - Learn all commands
- 🚀 [Advanced Features](ADVANCED-FEATURES.md) - Dynamic scaling and phases
- 📝 [Examples](EXAMPLES.md) - Step-by-step boss creation guides

---

*Need help? Join the [V Rising Mod Community Discord](https://discord.gg/vrisingmods) for support!*