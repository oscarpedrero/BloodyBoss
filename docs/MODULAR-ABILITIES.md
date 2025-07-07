# 🎯 VBlood Ability System

The VBlood Ability System in BloodyBoss v2.1.0 allows you to mix and match abilities from different VBloods while preserving the boss's original appearance. Create unique boss encounters by combining abilities from any of the 92 VBloods available in V Rising.

## 📚 Complete VBlood List

For a comprehensive list of all 92 VBloods and their abilities, see: **[VBlood Abilities Documentation](VBLOOD-ABILITIES.md)**

## 🚀 Quick Start

### Basic Usage

```bash
# 1. Create a boss (appearance)
.bb create "CustomBoss" -327335305 100 5 1800  # Dracula appearance

# 2. Add abilities from different VBloods
.bb ability-slot "CustomBoss" "Solarus" 0 true "Holy projectile"
.bb ability-slot "CustomBoss" "Beatrice" 2 true "Shadow dash"
.bb ability-slot "CustomBoss" "Gorecrusher" 3 true "Whirlwind attack"

# 3. Enable and test
.bb on "CustomBoss"
.bb test "CustomBoss"
```

## 🛠️ Commands

### `.bb ability-slot`
Configure individual ability slots for a boss.

**Syntax:** `.bb ability-slot <BossName> <SourceVBlood> <SlotIndex> <Enable> [Description]`

**Parameters:**
- `BossName` - Your configured boss name
- `SourceVBlood` - VBlood to copy ability from (partial names work)
- `SlotIndex` - Which ability slot (0-36+)
- `Enable` - `true` to apply, `false` to remove
- `Description` - Optional description

**Examples:**
```bash
# Add abilities
.bb ability-slot "CustomBoss" "Dracula" 0 true "Blood projectile"
.bb ability-slot "CustomBoss" "Christina" 14 true "Holy protection"

# Remove abilities (restore original)
.bb ability-slot "CustomBoss" "Dracula" 0 false
```

### `.bb ability-info`
Display detailed information about a VBlood's abilities.

**Syntax:** `.bb ability-info <VBloodName> [SlotIndex]`

**Examples:**
```bash
.bb ability-info "Dracula"        # Show all 37 abilities
.bb ability-info "Solarus" 0      # Show specific slot
.bb ability-info "Forest Wolf"    # Show all wolf abilities
```

### `.bb ability-suggest`
Get compatible ability suggestions for your boss.

**Syntax:** `.bb ability-suggest <BossName> [Category]`

**Examples:**
```bash
.bb ability-suggest "CustomBoss"              # All suggestions
.bb ability-suggest "CustomBoss" "Movement"   # Movement abilities only
.bb ability-suggest "CustomBoss" "Special"    # Special abilities only
```

### `.bb ability-test`
Test ability compatibility before applying.

**Syntax:** `.bb ability-test <BossName> <SourceVBlood> <SlotIndex>`

**Examples:**
```bash
.bb ability-test "CustomBoss" "Gorecrusher" 3
.bb ability-test "CustomBoss" "Morian" 2     # Test flying ability
```

### `.bb vblood-docs`
Export comprehensive VBlood documentation.

**Syntax:** `.bb vblood-docs`

**Output:**
- Creates markdown file with all 92 VBloods
- Saves to `/BepInEx/config/BloodyBoss/`
- Includes all abilities and slots

## 🎨 Compatibility System

The system automatically checks ability compatibility and provides feedback:

### Compatibility Levels

- **[PERFECT]** - Abilities work flawlessly together
- **[GOOD]** - Minor visual issues possible
- **[WARNING]** - Noticeable visual glitches may occur
- **[INCOMPATIBLE]** - Significant issues expected

### Compatibility Factors

1. **Model Type** - Humanoid vs Beast vs Flying
2. **Animation Sets** - Compatible animation requirements
3. **Special Features** - Flight capability, size differences
4. **Ability Type** - Melee vs Ranged vs Magic

### Example Output
```
🧪 Testing ability compatibility...
├─ Boss: Vampire Dracula
├─ Source: Gorecrusher the Behemoth
├─ Slot: 3
└─ Result: [WARNING] Warning

⚠️ Warnings:
├─ Model type mismatch: Vampire using Beast ability
└─ Animations may glitch during execution
```

## 💡 Best Practices

### 1. Start Simple
Begin with 2-3 ability swaps and test thoroughly before adding more.

### 2. Match Categories
Abilities work best when matching similar categories:
- Humanoid → Humanoid
- Beast → Beast
- Flying → Flying

### 3. Test Combinations
Always use `.bb ability-test` before applying abilities to check compatibility.

### 4. Document Your Builds
Use descriptive names when adding abilities:
```bash
.bb ability-slot "Boss" "Solarus" 0 true "AoE holy burst for phase 2"
```

### 5. Consider Visual Coherence
Some combinations may work mechanically but look strange visually.

## 🔥 Example Builds

### The Hybrid Vampire
```bash
# Dracula base with mixed abilities
.bb create "HybridVampire" -327335305 100 5 1800

# Mix of vampire and holy abilities
.bb ability-slot "HybridVampire" "Beatrice" 0 true "Shadow melee"
.bb ability-slot "HybridVampire" "Christina" 2 true "Holy AoE"
.bb ability-slot "HybridVampire" "Vincent" 10 true "Frost storm"
.bb ability-slot "HybridVampire" "Adam" 20 true "Chaos teleport"
```

### The Beast Master
```bash
# Alpha Wolf base with beast abilities
.bb create "BeastMaster" -1269164423 80 3 1500

# Mix beast abilities
.bb ability-slot "BeastMaster" "Forest Wolf" 2 true "Pack dash"
.bb ability-slot "BeastMaster" "Gorecrusher" 3 true "Rage charge"
.bb ability-slot "BeastMaster" "Frostmaw" 4 true "Frost breath"
```

### The Spell Weaver
```bash
# Christina base with magic focus
.bb create "SpellWeaver" -99214611 90 4 2000

# Pure magic build
.bb ability-slot "SpellWeaver" "Mairwyn" 0 true "Elemental burst"
.bb ability-slot "SpellWeaver" "Leandra" 2 true "Shadow magic"
.bb ability-slot "SpellWeaver" "Dracula" 10 true "Blood swarm"
```

## ⚠️ Limitations

1. **Visual Glitches** - Some ability combinations may cause animation issues
2. **Flying Abilities** - Only work on bosses with flight capability
3. **Size Mismatches** - Large creature abilities on small bosses may look odd
4. **Performance** - Too many complex abilities can impact server performance

## 🔧 Troubleshooting

### Abilities Not Working
1. Check boss is spawned: `.bb status "BossName"`
2. Verify slot configuration: `.bb status "BossName"`
3. Test compatibility: `.bb ability-test "BossName" "VBlood" slot`
4. Respawn boss: `.bb despawn "BossName"` then `.bb test "BossName"`

### Visual Issues
- Some combinations naturally have visual quirks
- Try abilities from similar model types
- Use `.bb ability-suggest` for better matches

### Performance Problems
- Limit to 6-8 modified abilities per boss
- Avoid multiple summon/AoE abilities
- Test on smaller player counts first

## 📖 Resources

- **[VBlood Abilities List](VBLOOD-ABILITIES.md)** - Complete list of all 92 VBloods
- **[Commands Reference](COMMANDS.md)** - All BloodyBoss commands
- **[Examples](EXAMPLES.md)** - More build examples
- **[Configuration](CONFIGURATION.md)** - Mod settings

---

*The VBlood Ability System opens unlimited possibilities for creating unique boss encounters. Experiment with different combinations to create memorable battles!*