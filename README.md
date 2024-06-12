# BloodyBoss

**BloodyBoss** is a mod for V Rising that allows you to create VBlood world bosses with random rewards that can be set for each of the world bosses.

## IMPORTANT NOTE

You must have version 1.2.4 of Bloody.Core installed to be able to use version 1.1.4 or higher of this mod

# Sponsor this project

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/K3K8ENRQY)

## NEW IN 1.1.7

- Added a configuration that makes the two bosses the same team and although they hit each other they do not take life and their preference is to attack the player and not each other
- Added the ability to modify boss statistics.

When you start the server the mod itself generates the statistics of the original boss if you don't have them filled out in the configuration file.
Then just modify the ones you want in your Bosses.json configuration file and execute chatcommand `.bb reload` to reload Bosses.json file.


```json
[
  {
    "name": "Test Boss",
    "nameHash": "292216611",
    "AssetName": "CHAR_ChurchOfLight_Cardinal_VBlood",
    "Hour": "00:30",
    "HourDespawn": "00:55:00",
    "PrefabGUID": 114912615,
    "level": 105,
    "multiplier": 1,
    "unitStats": {
      "PhysicalCriticalStrikeChance": 0,
      "PhysicalCriticalStrikeDamage": 2,
      "SpellCriticalStrikeChance": 0,
      "SpellCriticalStrikeDamage": 2,
      "PhysicalPower": 90.64623,
      "SpellPower": 90.64623,
      "ResourcePower": 48.96,
      "SiegePower": 19.4,
      "ResourceYieldModifier": 1,
      "ReducedResourceDurabilityLoss": 1,
      "PhysicalResistance": 0,
      "SpellResistance": 0,
      "SunResistance": 0,
      "FireResistance": 0,
      "HolyResistance": 0,
      "SilverResistance": 0,
      "SilverCoinResistance": 0,
      "GarlicResistance": 0,
      "PassiveHealthRegen": 1,
      "CCReduction": 0,
      "HealthRecovery": 1,
      "DamageReduction": 0,
      "HealingReceived": 0,
      "ShieldAbsorbModifier": 1,
      "BloodEfficiency": 1
    },
    "items": [
      {
        "name": "Brew of Ferocity",
        "ItemID": -269326085,
        "Stack": 12,
        "Chance": 100,
        "Color": "#daa520"
      }
    ],
    "Lifetime": 1500,
    "x": -2012.1184,
    "y": 5.000004,
    "z": -2792.9685,
  }
]
```

## NEW IN 1.1.4

- Minions summoned by players do not harm the NPC.
- Removed the ability to bite a non-vBlood NPC boss

**If with this version you have problems with messages or drops, I recommend that you make a copy of your boss configuration and create them again. Some problems have been reported with previous versions.**


<details>
<summary>Changelog</summary>

`1.1.7`
- Fixed an error that occurred when you turned off the server with a boss in the world that did not change its statistics once you restarted the server.
- Fixed the bug that did not eliminate a world boss if you turned off the server before its spawn and turned it on after its spawn.
- Fixed a bug that did not spawn the boss when there was no one online on the server.
- Added animation when you kill a boss for all players who participated in the battle.
- Added a configuration that makes the two bosses the same team and although they hit each other they do not take life and their preference is to attack the player and not each other

`1.1.6`
- Added option to enable or disable damage from players' minions to the boss

`1.1.5`
- Fixed doble icon

`1.1.4`
- Updated the timer system through CoroutineHandler.
- Minions summoned by players do not harm the NPC.
- Removed the ability to bite a non-vBlood NPC boss.
- Fixed errors that existed with messages from VBlood and non-VBlood NPCS.
- Fixed reward item duplication bug.
- Fixed the error that did not deliver the items in the inventory and always threw them on the ground.

`1.1.3`
- Fixed error with duplicate messages when killing an NPCS boss

`1.1.2`
- Performance improvements

`1.1.1`
- Added that any NPC can be used to act as a boss
- Updated to the latest version of Bloody.Core

`1.1.0`
- Fixed bug that occurred when too many players consumed the blood of a world boss.
- The location of the configuration file has been changed to the root config folder.

`1.0.8`
- Added protection against being able to include a drive other than VBlood.
- Added command to reload the vblood database in case the json is changed by hand.
- Added option in the BloodyBoss.cfg configuration file a section to prevent the boss from doing its original drop

`1.0.7`
- Fixed bug that caused the BloodyBoss reward system and death message to also affect the game's default Vblood if the VBlood Prefab was set to BloodyBoss and BloodyBoss was active at that time.

`1.0.6`
- Bloody.Core dependency removed as dll and added as framework
- The drop calculation formula has been solved, now it is not 100% as it used to be

`1.0.5`
- Addeed clearicon command

`1.0.4`
- Added the PlayersOnlineMultiplier option in the general configuration of the mod to activate or deactivate the online player multiplier.

`1.0.3`
- Fixed the error where the Boss's life multiplier used the total number of users registered on the server and not the number of users online on the server.
- Fixed the bug that the boss autospawn had. A new spawn system has been generated to avoid incompatibilities with other mods
- Fixed bug that caused the BloodyBoss reward system and death message to also affect the game's default Vblood if the VBlood Prefab was configured as BloodyBoss.

`1.0.0`
- Initial public release of the mod
</details>

## Requirements
Ensure the following mods are installed for seamless integration:

1. [BepInEx](https://thunderstore.io/c/v-rising/p/BepInEx/BepInExPack_V_Rising/)
2. [Bloodstone](https://thunderstore.io/c/v-rising/p/deca/Bloodstone/)
3. [VampireCommandFramework](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/)
4. [Bloody.Core](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/)

## Installation
1. Copy `BloodyBoss.dll` to your `BepInEx/Plugins` directory.
2. Launch the server once to generate the config file; configurations will be located in the `BepInEx/Config` directory.

## Configuration

**BloodyBoss.cfg** In the configuration file **BloodyBoss.cfg** you have several options to configure the mod to your liking

```
[Main]

## Determines whether the boss spawn timer is enabled or not.
# Setting type: Boolean
# Default value: true
Enabled = true

## The message that will appear globally once the boss gets killed.
# Setting type: String
# Default value: The #vblood# boss has been defeated by the following brave warriors:
KillMessageBossTemplate = The #vblood# boss has been defeated by the following brave warriors:

## The message that will appear globally one the boss gets spawned.
# Setting type: String
# Default value: A Boss #worldbossname# has been summon you got #time# minutes to defeat it!.
SpawnMessageBossTemplate = A Boss #worldbossname# has been summon you got #time# minutes to defeat it!.

## The message that will appear globally if the players failed to kill the boss.
# Setting type: String
# Default value: You failed to kill the Boss #worldbossname# in time.
DespawnMessageBossTemplate = You failed to kill the Boss #worldbossname# in time.

## Buff that applies to each of the Bosses that we create with our mod.
# Setting type: Int32
# Default value: 1163490655
BuffForWorldBoss = 1163490655

## Final string for concat two or more players kill a WorldBoss Boss.
# Setting type: String
# Default value: and
WorldBossFinalConcatCharacters = and

## If you activate this option, the boss life formula changes from "bosslife * multiplier" to "bosslife * multiplier * numberofonlineplayers".
# Setting type: Boolean
# Default value: false
PlayersOnlineMultiplier = false

## If you activate this option it will remove the original vblood droptable.
# Setting type: Boolean
# Default value: false
ClearDropTable = false

## Disable minion damage to bosses.
# Setting type: Boolean
# Default value: true
MinionDamage = true

## If you activate this option instead of spawning a specific boss at a specific time, the system will search for a random boss and spawn the random boss instead of the original boss at the original boss's specific time..
# Setting type: Boolean
# Default value: false
RandomBoss = true

## Deactivates the buff animation received by players who have participated in the battle for three seconds.
# Setting type: Boolean
# Default value: true
BuffAfterKillingEnabled = true

## PrefabGUID of the buff received by players who have participated in the battle for three seconds.
# Setting type: Int32
# Default value: -2061047741
BuffAfterKillingPrefabGUID = -2061047741

## If you activate this option, the bosses will not attack each other and will team up if two bosses are summoned together.
# Setting type: Boolean
# Default value: false
TeamBossEnable = true
```

## Commands
It's crucial to note that for any command containing a name argument such as `<NameOfBoss>` or `<ItemName>`, if your name consists of more than one word, include it inside `""` to ensure proper functionality (e.g., "Alpha Wolf" or "Blood Rose Potion").

prefix: `.bb`.

```ansi
.bb test <NameOfBoss>
```
- This command spawns the boss you want at your position and sets it to spawn within one minute. This is useful to see what time your server has set and to know what time to add to each boss.
  - Example: `.bb test "Alpha Wolf"`

```ansi
.bb reload
```
- Reload boss database in case the json is changed by hand (It only reloads bosses, it does not reload mod settings)
  - Example: `.bb reload`

```ansi
.bb create <NameOfBoss> <PrefabGUIDOfBOSS> <Level> <Multiplier> <LifeTimeSeconds>
```
- Create your desired Boss to include in the Boss list.
  - **NameOfBoss**: The Boss name that will appear in the chat when the Boss spawn.
  - **PrefabGUIDOfBOSS**: The GUID of the Boss you prefer to use. 
  - **Level**: Specify the level you want the Boss  to be.
  - **Multiplier**: Specify the HP multiplier based on how many players are online. For example, if the multiplier is 2 and there are 2 players online then the Boss HP will be x4 (2 for `multiplier value` x 2 `players online`) 
  - **LifeTimeSeconds**: The duration the player has to kill the Boss in seconds.
  - Example: `.bb create "Alpha Wolf" -1905691330 90 1 1800`

```ansi
.bb remove (bossName)
```
- Remove a Boss from the Boss list.
  - **bossName**: The Boss name that you want to remove from the list.
  - Example: `.bb remove "Alpha Wolf"`

```ansi
.bb list
```
- List all the available Bosses to spawn from the Boss list.
  - Example: `.bb list`

```ansi
.bb set location <NameOfBoss>
```
- Specify the location at which a specific Boss will spawn based on where you currently at in the game, meaning that where you stand is where the boss will spawn.
  - **NameOfBoss**: The Boss name you want to specify the spawn location of.
  - Example: `.bbs set location "Alpha Wolf"`

```ansi
.bb set hour <NameOfBoss> <Hour>
```
- Specifies the time when a specific boss will appear.
  - **NameOfBoss**: The Boss name you want to specify the spawn location of.
  - **Hour**:  The format must be in 24 hours and HH:MM. 01:00 for 1 AM and 13:00 for 1 PM.
  - Example: `.bbs set hour "Alpha Wolf" 18:30`

```ansi
.bb items add <NameOfBoss> <ItemName> <ItemPrefabID> <Stack> <Chance>
```
- Adds items/rewards to the randomized pool that the player will receive from defeating a particular Boss.
  - **NameOfBoss**: The Boss name to which you want to add items.
  - **ItemName**: The name of the item/reward appearing in the chat once the player defeats the Boss.
  - **ItemPrefabID**: The GUID for the item you want to add.
  - **Stack**: The quantity of items the player will gain upon winning the encounter (e.g., x25 Blood Potions).
  - **Chance**: The chance of that item to get upon defeating the Boss from 1 to 100.
  - Example: `.bb items add "Alpha Wolf" "Blood Rose Potion" 429052660 25 10`

```ansi
.bb items remove <NameOfBoss> <ItemName>
```
- Removes items/rewards from the randomized pool that the player will receive from defeating a particular Boss.
  - **NameOfBoss**: The Boss name to which you want to remove items.
  - **ItemName**: The name of the item/reward you want to remove.
  - Example: `.bb items remove "Alpha Wolf" "Blood Rose"`

```ansi
.bb items list <NameOfBoss>
```
- Display all items/rewards from the randomized pool that the player will receive from defeating a particular Boss.
  - **NameOfBoss**: The Boss name to which you want to display items/rewards.
  - Example: `.bb items list "Alpha Wolf"`

```ansi
.bb start <NameOfBoss>
```
- Manually spawn the Boss in its specified location.
  - **NameOfBoss**: The Boss name you want to start.
  - Example: `.bb start "Alpha Wolf"`

```ansi
.bb clearicon <NameOfBoss>
```
- If at any time you have an icon left on the minimap due to some error, with this command you can delete it.
  - **NameOfBoss**: The Boss name you want to start.
  - Example: `.bb clearicon "Alpha Wolf"`

# Resources

[Complete items list of prefabs/GUID](https://discord.com/channels/978094827830915092/1117273637024714862/1117273642817044571)

# Credits

[V Rising Mod Community](https://discord.gg/vrisingmods) is the premier community of mods for V Rising.

[@Deca](https://github.com/decaprime), thank you for the exceptional frameworks [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework) and [BloodStone](https://github.com/decaprime/Bloodstone), based on [WetStone](https://github.com/molenzwiebel/Wetstone) by [@Molenzwiebel](https://github.com/molenzwiebel).

**Special thanks to the testers and supporters of the project:**

- @Vex, owner & founder of [Vexor RPG](https://discord.gg/JpVsKVvKNR) server, a tester and great supporter who provided his server as a test platform and took care of all the graphics and documentation.
- @Bromelda & Wolfyowns, owners & founders of [BloodCraft- Modded Xprising server](https://discord.gg/aDh98KtEWZ) server, for their great work giving me feedback and thoroughly testing the mod.
