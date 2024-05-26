# BloodyBoss

**BloodyBoss** is a mod for V Rising that allows you to create VBlood world bosses with random rewards that can be set for each of the world bosses.

![BloodyBoss](https://github.com/oscarpedrero/BloodyBoss/blob/master/Images/BloodyBoss.png?raw=true)

<details>
<summary>Changelog</summary>

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

1. [BepInEx](https://github.com/BepInEx/BepInEx)
2. [VampireCommandFramework](https://github.com/decaprime/VampireCommandFramework)
3. [Bloodstone](https://github.com/decaprime/Bloodstone)
3. [Bloody.Core](https://github.com/oscarpedrero/BloodyCore)

## Installation
1. Copy `BloodyBoss.dll` to your `BepInEx/Plugins` directory.
2. Launch the server once to generate the config file; configurations will be located in the `BepInEx/Config` directory.

## Configuration

**BloodyBoss.cfg** In the configuration file **BloodyBoss.cfg** you have several options to configure the mod to your liking

```
[Main]

## Determines whether the random encounter timer is enabled or not.
# Setting type: Boolean
# Default value: true
Enabled = true

## The message that will appear globally once the boss gets killed.
# Setting type: String
# Default value: The Boss has been defeated. Congratulations to #user# for beating #vblood#!
KillMessageBossTemplate = The Boss has been defeated. Congratulations to #user# for beating #vblood#!

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
```

## Commands
It's crucial to note that for any command containing a name argument such as `<NameOfBoss>` or `<ItemName>`, if your name consists of more than one word, include it inside `""` to ensure proper functionality (e.g., "Alpha Wolf" or "Blood Rose Potion").

prefix: `.bb`.

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

![Bloody](https://github.com/oscarpedrero/BloodyBoss/blob/master/Images/Bloody.png?raw=true)
