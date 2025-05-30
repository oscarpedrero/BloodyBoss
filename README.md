# BloodyBoss

**BloodyBoss** is a mod for V Rising that allows you to create VBlood world bosses with random rewards that can be set for each of the world bosses.

## [BepInEx 1.733.2 (RC2)](https://github.com/decaprime/VRising-Modding/releases/tag/1.733.2)

# BloodyConfig

**BloodyConfig** It is a Windows application that will help you configure the bosses and their drop table.

https://github.com/oscarpedrero/BloodyConfig/releases/tag/v0.0.2

## IMPORTANT NOTE

You must have version 2.0.0 of Bloody.Core installed to be able to use version 2.0.0 or higher of this mod

# Sponsor this project

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/K3K8ENRQY)

## Known bugs

- When spawning two bosses at the same time based on the same PrefabGUID, if they die at the same time, only one rewards.


<details>
<summary>Changelog</summary>

`2.0.0`
- Updated to Oakveil
- Removed the following UnitStats: PhysicalCriticalStrikeDamage, SpellCriticalStrikeChance, SpellCriticalStrikeDamage, ResourceYieldModifier, ReducedResourceDurabilityLoss, SilverResistance, SilverCoinResistance, 
GarlicResistance, PassiveHealthRegen, HealthRecovery, DamageReduction, HealingReceived and ShieldAbsorbModifier
- Removed Bloodstone dependency
- Fixed when the boss disappears due to time, it used to drop loot, now it doesn't anymore ;)
- Fixed an issue with lifetime and despawn time

`1.1.14`
- Fixed bug when bosses did not have a spawn time they would sometimes stop the spawn timer.
- Fixed a bug that caused enemies to not be on the same team and attack each other.

`1.1.13`
- Added functionality for random bosses to appear at the spawn location of the original boss

`1.1.12`
- Added `clearallicons` command to remove any icons that are stuck on the BloodyBoss map
- The VBlood works again in the original way. Only those who bite the VBlood receive the reward
- Removed the comma from the end of each player's name.
- Removed the WorldBossFinalConcatCharacters parameter from the configuration.

`1.1.11`
- Fixed the error that occurred when the Vblood/NPC spawned by Bloodyboss appears in the game, if you defeat the original Vblood/NPC, you are considered to have killed the Vblood/NPC spawned by Bloodyboss and you will receive the items set by Bloodyboss.

`1.1.10`
- Fixed error when spawning two bosses based on the same prefabGUID that only gave a reward to one of them.
- Fixed a bug where some NPCs did not spawn correctly due to the BloodConsumeSource component

`1.1.9`
- Fixed error that did not recognize some VBLOOD as such.

`1.1.8`
- Fixed bug when sending the message multiple times when more than one vampire kills the world boss.

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

1. [BepInEx 1.733.2 (RC2)](https://github.com/decaprime/VRising-Modding/releases/tag/1.733.2)
2. [VampireCommandFramework](https://thunderstore.io/c/v-rising/p/deca/VampireCommandFramework/)
3. [Bloody.Core](https://thunderstore.io/c/v-rising/p/Trodi/BloodyCore/)

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

```ansi
.bb clearallicons
```
- Forces the removal of all icons that exist on the map generated by BloodyBoss without having to specify the boss's name
  - Example: `.bb clearallicons`

# Resources

[Complete items list of prefabs/GUID](https://wiki.vrisingmods.com/prefabs/)

# Credits

[V Rising Mod Community](https://discord.gg/vrisingmods) is the premier community of mods for V Rising.

[@Deca](https://github.com/decaprime), thank you for the exceptional frameworks [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework)

**Special thanks to the testers and supporters of the project:**

- @Bromelda & Wolfyowns, owners & founders of [BloodCraft- Modded Xprising server](https://discord.gg/aDh98KtEWZ) server, for their great work giving me feedback and thoroughly testing the mod.
