# Item Configuration Commands

Configure what items bosses drop when defeated. Create exciting rewards and manage loot tables.

## Table of Contents
- [Add Item](#add-item)
- [List Items](#list-items)
- [Remove Item](#remove-item)
- [Item Best Practices](#item-best-practices)
- [Common Item PrefabIDs](#common-item-prefabids)

---

## Add Item

Adds an item to a boss's loot table.

### Syntax
```
.bb item add <BossName> <ItemName> <PrefabID> <Stack> <Chance>
```

### Parameters
- `BossName` - Name of the boss
- `ItemName` - Friendly name for the item (for your reference)
- `PrefabID` - The game's item PrefabGUID
- `Stack` - How many items to drop
- `Chance` - Drop probability (0.0 to 1.0, where 1.0 = 100%)

### Examples
```bash
# Guaranteed drops (100% chance)
.bb item add "Fire Dragon" "Dragon Blood" -77477508 10 1.0
.bb item add "Fire Dragon" "Dragon Scale" 862477668 5 1.0

# Rare drops (25% chance)
.bb item add "Fire Dragon" "Dragon Heart" -810805974 1 0.25
.bb item add "Fire Dragon" "Ancient Rune" -1531666018 2 0.25

# Common drops (75% chance)
.bb item add "Shadow Beast" "Dark Essence" -257494203 15 0.75
```

### Chance System
- `1.0` = 100% (always drops)
- `0.75` = 75% (drops 3 out of 4 times)
- `0.5` = 50% (drops half the time)
- `0.25` = 25% (drops 1 out of 4 times)
- `0.1` = 10% (rare drop)
- `0.01` = 1% (very rare drop)

### Important Notes
- Item names are for display only
- The actual item is determined by PrefabID
- Multiple items can have the same PrefabID with different chances
- Stack size should be reasonable for the item type

---

## List Items

Shows all items configured for a boss.

### Syntax
```
.bb item list <BossName>
```

### Examples
```bash
.bb item list "Fire Dragon"
.bb item list "Shadow Beast"
```

### Output Example
```
Fire Dragon Items List
----------------------------
--
Item Dragon Blood
Stack 10
Chance 1.0
--
Item Dragon Scale
Stack 5
Chance 1.0
--
Item Dragon Heart
Stack 1
Chance 0.25
--
Item Ancient Rune
Stack 2
Chance 0.25
--
----------------------------
```

---

## Remove Item

Removes an item from a boss's loot table.

### Syntax
```
.bb item remove <BossName> <ItemName>
```

### Parameters
- `BossName` - Name of the boss
- `ItemName` - The friendly name you gave the item when adding it

### Examples
```bash
.bb item remove "Fire Dragon" "Dragon Blood"
.bb item remove "Shadow Beast" "Dark Essence"
```

### Notes
- Must use the exact item name you specified when adding
- This removes the entire item entry (all stacks)
- To change quantity or chance, remove and re-add

---

## Item Best Practices

### Loot Table Design

#### Tiered Rewards
```bash
# Guaranteed basic rewards
.bb item add "Boss" "Basic Material" -77477508 20 1.0

# Common rewards (75%)
.bb item add "Boss" "Good Material" -257494203 10 0.75

# Uncommon rewards (40%)
.bb item add "Boss" "Rare Material" 862477668 5 0.4

# Rare rewards (10%)
.bb item add "Boss" "Epic Material" -810805974 2 0.1

# Legendary rewards (1%)
.bb item add "Boss" "Legendary Item" -1531666018 1 0.01
```

#### Themed Loot
```bash
# Fire Boss Example
.bb item add "Fire Lord" "Sulfur" -1763813322 50 1.0
.bb item add "Fire Lord" "Fire Blossom" -1557883468 10 0.75
.bb item add "Fire Lord" "Gem Dust" -929428510 20 0.5
.bb item add "Fire Lord" "Hell's Clarion" 2027629097 1 0.05

# Frost Boss Example
.bb item add "Frost Giant" "Snow Flower" -1848322903 10 1.0
.bb item add "Frost Giant" "Pristine Leather" 862477668 20 0.75
.bb item add "Frost Giant" "Frost Core" 581527578 1 0.1
```

### Balanced Progression

#### Early Game Boss
```bash
.bb item add "Young Wolf" "Animal Hide" -632179142 20 1.0
.bb item add "Young Wolf" "Bones" 873914934 10 1.0
.bb item add "Young Wolf" "Blood Essence" -77477508 5 0.5
```

#### Mid Game Boss
```bash
.bb item add "Elite Guard" "Iron Ore" -668962842 30 1.0
.bb item add "Elite Guard" "Leather" 862477668 15 0.75
.bb item add "Elite Guard" "Greater Blood Essence" -257494203 10 0.5
.bb item add "Elite Guard" "Merciless Iron Weapon" -1576952285 1 0.1
```

#### End Game Boss
```bash
.bb item add "Ancient Dragon" "Dark Silver Ingot" -982817303 20 1.0
.bb item add "Ancient Dragon" "Pristine Leather" 862477668 50 1.0
.bb item add "Ancient Dragon" "Soul Shard" -810805974 5 0.5
.bb item add "Ancient Dragon" "Legendary Weapon Recipe" 1821405057 1 0.05
```

---

## Common Item PrefabIDs

### Blood Essences
- `-77477508` - Blood Essence
- `-257494203` - Greater Blood Essence
- `-1647526126` - Primal Blood Essence

### Basic Materials
- `-668962842` - Iron Ore
- `-2095346305` - Copper Ore
- `-870640474` - Silver Ore
- `-889478620` - Gold Ore
- `873914934` - Bones
- `-632179142` - Animal Hide
- `1124739990` - Plant Fibre
- `-1763813322` - Sulfur

### Refined Materials
- `946737117` - Iron Ingot
- `-21483617` - Copper Ingot
- `-982817303` - Dark Silver Ingot
- `1821405057` - Gold Ingot
- `862477668` - Pristine Leather
- `-929428510` - Gem Dust
- `-162403520` - Brick

### Gems
- `911871970` - Regular Emerald
- `-1726966959` - Regular Ruby
- `1452307469` - Regular Sapphire
- `-1185009621` - Regular Amethyst

### Special Items
- `-810805974` - Soul Shard
- `-1531666018` - Scourgestone
- `581527578` - Power Core
- `-1557883468` - Fire Blossom
- `-1848322903` - Snow Flower

### Consumables
- `429052660` - Blood Rose Potion
- `-1382865424` - Minor Healing Potion
- `1919691013` - Healing Potion
- `2132095202` - Greater Healing Potion

### Recipe Books
- Various weapon and armor recipe books
- Check the game's item database for specific IDs

### Tips for Finding PrefabIDs
1. Use the V Rising Wiki
2. Check community databases
3. Use admin commands to spawn items and note their IDs
4. Reference other boss configurations

---

## Advanced Configurations

### Multiple Drop Tables
Create different loot pools:
```bash
# Common drops (always something)
.bb item add "Boss" "Common1" -77477508 10 0.5
.bb item add "Boss" "Common2" -257494203 5 0.5

# Rare table (only one drops)
.bb item add "Boss" "Rare1" -810805974 1 0.1
.bb item add "Boss" "Rare2" -1531666018 1 0.1
.bb item add "Boss" "Rare3" 581527578 1 0.1
```

### Quantity Scaling
Scale rewards based on difficulty:
```bash
# Normal Boss
.bb item add "Wolf" "Hide" -632179142 10 1.0

# Elite Version
.bb item add "Elite Wolf" "Hide" -632179142 25 1.0

# Legendary Version
.bb item add "Legendary Wolf" "Hide" -632179142 50 1.0
```

### Event Rewards
Special drops for events:
```bash
# Halloween Event
.bb item add "Spooky Boss" "Candy" 123456 10 1.0
.bb item add "Spooky Boss" "Pumpkin Seeds" 789012 5 0.5

# Christmas Event  
.bb item add "Frost Boss" "Snowflake" 345678 20 1.0
.bb item add "Frost Boss" "Gift Box" 901234 1 0.25
```

---

## Troubleshooting

### Items Not Dropping
1. Verify boss was killed (not despawned)
2. Check item configuration: `.bb item list <BossName>`
3. Ensure PrefabID is correct
4. Check server logs for errors

### Wrong Items Dropping
- Double-check PrefabID
- Some items share similar IDs
- Use exact values from database

### Drop Rates Feel Wrong
- RNG can create streaks
- Test with larger sample size
- Consider adjusting chances

---

[← Back to Index](index.md) | [Next: Ability Commands →](abilities.md)