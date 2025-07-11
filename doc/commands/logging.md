# Logging Commands

## Overview
BloodyBoss includes a sophisticated logging system that allows server administrators to control the verbosity and categories of logs in real-time without restarting the server.

## Log Levels
The logging system supports the following levels (from least to most verbose):
- **None** - No logging
- **Error** - Only critical errors
- **Warning** - Errors and warnings
- **Info** - Errors, warnings, and informational messages
- **Debug** - All above plus debug information
- **Trace** - Everything including detailed trace logs

## Log Categories
Logs are organized into the following categories:
- **System** - Core system operations
- **Boss** - Boss spawning, tracking, and management
- **Damage** - Damage detection and processing
- **Mechanic** - Boss mechanic execution
- **Command** - Command processing
- **Database** - Database operations
- **Hook** - Hook system events
- **Timer** - Timer and scheduling
- **Spawn** - Entity spawning
- **Death** - Death events
- **Reward** - Reward distribution
- **Debug** - Debug information

## Commands

### Basic Commands

#### `.bb log level <level>`
Sets the global log level for all categories.
- **Admin only**: Yes
- **Example**: `.bb log level Warning`

#### `.bb log category <category> <level>`
Sets the log level for a specific category.
- **Admin only**: Yes
- **Example**: `.bb log category Damage Error`

#### `.bb log disable <category>`
Completely disables logging for a specific category.
- **Admin only**: Yes
- **Example**: `.bb log disable Timer`

#### `.bb log enable <category>`
Re-enables logging for a previously disabled category.
- **Admin only**: Yes
- **Example**: `.bb log enable Timer`

#### `.bb log list`
Lists all available log categories.
- **Admin only**: Yes

### Preset Modes

#### `.bb log quiet`
Enables quiet mode - only errors will be logged.
- **Admin only**: Yes
- **Sets**: Global level to Error

#### `.bb log normal`
Enables normal mode - standard logging level.
- **Admin only**: Yes
- **Sets**: Global level to Info

#### `.bb log verbose`
Enables verbose mode - all logs will be shown.
- **Admin only**: Yes
- **Sets**: Global level to Trace

#### `.bb log performance`
Optimizes logging for best performance with minimal output.
- **Admin only**: Yes
- **Effects**:
  - Disables: Damage, Hook, Timer, Debug categories
  - Sets to Warning: Mechanic, Spawn
  - Keeps at Info: Boss, Death, Reward
  - Sets to Warning: System

## Configuration File
Logging can also be configured in the `BloodyBoss.cfg` file:

```ini
[Logging]
## Global log level: None, Error, Warning, Info, Debug, Trace
GlobalLogLevel = Info

## Specific log levels per category (format: Category1:Level,Category2:Level)
CategoryLogLevels = Damage:Warning,Hook:Warning,Timer:Warning,Mechanic:Info,Debug:None

## Comma-separated list of disabled log categories
DisabledCategories = Debug

## Enable logging to a separate file
LogToFile = false

## Path for the log file (relative to game root)
LogFilePath = BepInEx/logs/BloodyBoss.log
```

## Usage Examples

### Debugging a Specific Issue
```
.bb log category Boss Debug
.bb log category Mechanic Trace
```

### Production Server Setup
```
.bb log performance
```

### Troubleshooting Damage Detection
```
.bb log category Damage Debug
.bb log category Hook Debug
```

### Silent Mode for Events
```
.bb log quiet
```

## Best Practices

1. **Default Configuration**: The default settings are optimized for a good balance between information and performance.

2. **Performance Impact**: More verbose logging (Debug/Trace) can impact server performance, especially in categories like Damage and Hook which fire frequently.

3. **Troubleshooting**: When troubleshooting, enable specific categories rather than global verbose mode.

4. **Production Servers**: Use `.bb log performance` or `.bb log quiet` for production servers with many players.

5. **Log Rotation**: If using file logging, implement log rotation to prevent disk space issues.

## Tips

- Changes take effect immediately without server restart
- Settings persist until server restart or config change
- Use `.bb log list` to see available categories if you forget
- Combine category-specific settings for targeted debugging