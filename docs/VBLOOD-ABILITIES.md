# V Rising VBlood Abilities Documentation
Generated on: 2025-07-07 23:45:03

This document lists all available VBlood bosses and their abilities that can be used with the BloodyBoss mod.

## Command Usage
```
.bb ability-slot <BossName> <VBloodName> <SlotIndex> true <Description>
```

## Available VBloods and Their Abilities

### Forest Wolf (Level 16)
- **GUID**: -1905691330
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 10

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,3s | Channeled, effect |
| 1 | BasicAttack | 0,6s | Channeled, effect |
| 2 | Movement | 1,7s | Channeled, effect, Dash |
| 3 | Movement | 1,7s | 2-hit combo, Channeled, effect, Dash |
| 4 | Movement | 1,7s | 3-hit combo, Channeled, effect, Dash |
| 5 | Special | 1,4s | Channeled, effect, Dash |
| 6 | Movement | 0,7s | Channeled, effect, Travel |
| 7 | Movement | 0,8s | Channeled |
| 8 | Movement | 1,3s | Channeled, effect |
| 9 | Buff | 2s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Forest Wolf" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Forest Wolf" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Forest Wolf" 2 true "Channeled, effect, Dash"
```

---

### Bandit Foreman (Level 20)
- **GUID**: 2122229952
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 13

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 2s | Channeled, projectile |
| 1 | BasicAttack | 1s | Channeled, effect, TargetAOE |
| 2 | BasicAttack | 0,5s | Channeled |
| 3 | Special | 1,5s | Channeled, effect |
| 4 | Special | 0,5s | Channeled, AoE |
| 5 | Special | 1,3s | Channeled, projectile |
| 6 | Movement | 0,4s | Channeled, projectile |
| 7 | Movement | 2,5s | Channeled, effect, TargetAOE |
| 8 | Movement | 3s | Channeled, buff |
| 9 | Movement | 0,25s | Channeled, projectile |
| 10 | Unknown | 2,4s | Channeled, buff |
| 11 | Unknown | 1,4s | Channeled, projectile |
| 12 | Special | 1,4s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Foreman" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Bandit Foreman" 1 true "Channeled, effect, TargetAOE"
```
```
.bb ability-slot "YourBoss" "Bandit Foreman" 2 true "Channeled"
```

---

### Bandit Frostarrow (Level 20)
- **GUID**: 1124739990
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 2,3s | Channeled |
| 1 | BasicAttack | 0,1s | Channeled, buff |
| 2 | BasicAttack | 0,35s | Channeled |
| 3 | Special | 1,5s | Channeled, effect |
| 4 | Special | 1,6s | Channeled |
| 5 | Special | 1,5s | Channeled, effect |
| 6 | Special | 0,1s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Frostarrow" 0 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Bandit Frostarrow" 1 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Bandit Frostarrow" 2 true "Channeled"
```

---

### Bandit StoneBreaker (Level 20)
- **GUID**: -2025101517
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | Channeled, Requires flight, effect |
| 1 | BasicAttack | 1s | Channeled, Requires flight, effect |
| 2 | Special | 1,2s | Channeled, projectile/effect |
| 3 | Special | 1,2s | Channeled, projectile/effect |
| 4 | Special | 1s | Channeled, effect, Channeling |
| 5 | Special | 0,3s | Channeled, effect |
| 6 | Special | 2s | Channeled, effect |
| 7 | Movement | 2s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit StoneBreaker" 0 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit StoneBreaker" 1 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit StoneBreaker" 2 true "Channeled, projectile/effect"
```

---

### Bandit Stalker (Level 27)
- **GUID**: 1106149033
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,5s | Channeled, effect |
| 1 | Special | 2,2s | Channeled, effect |
| 2 | Special | 2s | Channeled |
| 3 | Special | 1,2s | Channeled, effect, Channeling |
| 4 | Special | 0,2s | Channeled, effect |
| 5 | Special | 1,2s | Channeled, buff, Channeling |
| 6 | Special | 0,1s | Channeled, buff |
| 7 | Special | 0,5s | Channeled, effect, Channeling |
| 8 | Special | 1,4s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Stalker" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Bandit Stalker" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Bandit Stalker" 2 true "Channeled"
```

---

### Undead BishopOfDeath (Level 27)
- **GUID**: 577478542
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,4s | Channeled, projectile |
| 1 | BasicAttack | 1,4s | Channeled, projectile |
| 2 | BasicAttack | 0,9s | Channeled, effect, TargetAOE |
| 3 | Special | 0,9s | Channeled, effect, TargetAOE |
| 4 | Movement | 0,4s | Channeled, movement, Travel |
| 5 | Special | 1,2s | Channeled, buff |
| 6 | Movement | 1,2s | Channeled, buff |
| 7 | Movement | 0,9s | 3-hit combo, Channeled, effect, TargetAOE |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead BishopOfDeath" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfDeath" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfDeath" 2 true "Channeled, effect, TargetAOE"
```

---

### Bandit Bomber (Level 30)
- **GUID**: 1896428751
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 3

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, effect, TargetAOE |
| 1 | BasicAttack | 0,5s | Channeled |
| 2 | BasicAttack | 0,7s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Bomber" 0 true "Channeled, effect, TargetAOE"
```
```
.bb ability-slot "YourBoss" "Bandit Bomber" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Bandit Bomber" 2 true "Channeled, projectile"
```

---

### Bandit Chaosarrow (Level 30)
- **GUID**: 763273073
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,5s | Channeled, buff |
| 1 | BasicAttack | 0,1s | Channeled, buff |
| 2 | BasicAttack | 0,35s | Channeled |
| 3 | Buff | 2s | Channeled, buff |
| 4 | Special | 1,2s | Channeled, buff |
| 5 | Buff | 2s | Channeled, buff |
| 6 | Movement | 0,9s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Chaosarrow" 0 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Bandit Chaosarrow" 1 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Bandit Chaosarrow" 2 true "Channeled"
```

---

### Bandit Leader UNUSED (Level 30)
- **GUID**: -175381832
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 2,5s | Channeled |
| 1 | BasicAttack | 0,5s | Channeled |
| 2 | BasicAttack | 2s | Channeled, effect |
| 3 | Special | 1s | Channeled, effect, TargetAOE |
| 4 | Special | 0,4s | Channeled |
| 5 | Special | 0,25s | Channeled, projectile |
| 6 | Movement | 3s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Leader UNUSED" 0 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Bandit Leader UNUSED" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Bandit Leader UNUSED" 2 true "Channeled, effect"
```

---

### Vermin DireRat (Level 30)
- **GUID**: -2039908510
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 5

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, effect |
| 1 | Buff | 1,2s | Channeled |
| 2 | BasicAttack | 0,2s | Channeled, effect |
| 3 | Special | 2,4s | Channeled, AoE |
| 4 | Special | 1,5s | Channeled, buff, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Vermin DireRat" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Vermin DireRat" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Vermin DireRat" 2 true "Channeled, effect"
```

---

### Bandit Fisherman (Level 32)
- **GUID**: -2122682556
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 13

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,4s | 2-hit combo, Channeled, Requires flight, effect |
| 1 | BasicAttack | 0,6s | Channeled, effect/buff, TargetAOE |
| 2 | BasicAttack | 2,2s | Channeled, effect, Channeling |
| 3 | Special | 1,6s | Channeled, effect, Hover |
| 4 | Special | Instant | Channeled, buff, Hover |
| 5 | Special | Instant | Channeled, buff, Hover |
| 6 | Movement | 0,1s | Channeled, buff, Hover |
| 7 | Movement | Instant | Channeled, buff, Hover |
| 8 | Movement | Instant | Channeled, buff, Hover |
| 9 | Movement | Instant | Channeled, buff, Hover |
| 10 | Unknown | Instant | Channeled, buff, Hover |
| 11 | Unknown | Instant | Channeled, buff, Hover |
| 12 | Unknown | 1,6s | Channeled, effect, Hover |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Fisherman" 0 true "2-hit combo, Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit Fisherman" 1 true "Channeled, effect/buff, TargetAOE"
```
```
.bb ability-slot "YourBoss" "Bandit Fisherman" 2 true "Channeled, effect, Channeling"
```

---

### Forest Bear Dire (Level 35)
- **GUID**: -1391546313
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,25s | Channeled, effect |
| 1 | BasicAttack | 2,2s | Channeled, AoE/buff |
| 2 | Movement | 1,2s | Channeled, effect, Dash |
| 3 | Special | 1,7s | Channeled, AoE |
| 4 | Special | 0,1s | Channeled, buff |
| 5 | Special | 2s | Channeled |
| 6 | BasicAttack | 0,9s | Channeled, effect |
| 7 | Movement | Instant | Channeled, buff, Hover |
| 8 | Movement | Instant | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Forest Bear Dire" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Forest Bear Dire" 1 true "Channeled, AoE/buff"
```
```
.bb ability-slot "YourBoss" "Forest Bear Dire" 2 true "Channeled, effect, Dash"
```

---

### Poloma (Level 35)
- **GUID**: -484556888
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,5s | Channeled |
| 1 | BasicAttack | 1,7s | Channeled, projectile |
| 2 | BasicAttack | 0,2s | Channeled, buff |
| 3 | Special | 1,6s | Channeled, effect, TargetAOE |
| 4 | Special | 0,3s | Channeled, effect |
| 5 | Special | 0,2s | Channeled, buff, Hover |
| 6 | Special | 1,5s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Poloma" 0 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Poloma" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Poloma" 2 true "Channeled, buff"
```

---

### Undead Priest (Level 35)
- **GUID**: 153390636
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 2s | Channeled, projectile |
| 1 | Movement | 1s | Channeled, movement, Travel |
| 2 | Special | 3s | Channeled, projectile, Channeling |
| 3 | Special | 1,5s | Channeled, buff, Channeling |
| 4 | Special | 1,5s | Channeled, buff, Channeling |
| 5 | Special | 0,1s | Channeled, buff |
| 6 | Movement | 1,5s | Channeled, buff |
| 7 | Special | 3s | Channeled, projectile, Channeling |
| 8 | Special | 1,3s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead Priest" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead Priest" 1 true "Channeled, movement, Travel"
```
```
.bb ability-slot "YourBoss" "Undead Priest" 2 true "Channeled, projectile, Channeling"
```

---

### Bandit Tourok (Level 37)
- **GUID**: -1659822956
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Bandit
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, Requires flight, effect |
| 1 | BasicAttack | 0,4s | Channeled, Requires flight, effect |
| 2 | Special | 1,6s | Channeled, effect |
| 3 | Special | 1,4s | Channeled, effect, Dash |
| 4 | Special | 0,4s | Channeled, buff |
| 5 | Special | 0,6s | Channeled, effect, Dash |
| 6 | Special | 0,5s | Channeled, buff, Channeling |
| 7 | Movement | 1,2s | Channeled, projectile/buff |
| 8 | Special | 1,4s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Tourok" 0 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit Tourok" 1 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit Tourok" 2 true "Channeled, effect"
```

---

### Villager Tailor (Level 40)
- **GUID**: -1942352521
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Ultimate | 5,5s | Channeled |
| 1 | BasicAttack | 1,2s | 2-hit combo, Channeled, effect |
| 2 | BasicAttack | Instant | Channeled, projectile |
| 3 | Movement | 0,8s | Channeled, movement/buff, Travel |
| 4 | Defensive | 1,53s | Channeled, Requires flight, buff, Channeling |
| 5 | Special | 0,9s | Channeled, Requires flight, movement, Travel |
| 6 | Movement | 0,1s | Channeled, Requires flight, movement, Travel |
| 7 | Defensive | 0,1s | Channeled, Requires flight, effect |
| 8 | Buff | 0,1s | Channeled, buff |
| 9 | Movement | 0,9s | Channeled, Requires flight, movement, Travel |
| 10 | Unknown | 0,1s | Channeled, Requires flight, movement, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Villager Tailor" 0 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Villager Tailor" 1 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Villager Tailor" 2 true "Channeled, projectile"
```

---

### Militia Guard (Level 44)
- **GUID**: -29797003
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,2s | 2-hit combo, Channeled, effect |
| 1 | Special | 2s | Channeled |
| 2 | Special | 0,4s | Channeled, buff |
| 3 | Special | 0,8s | Channeled, effect/buff |
| 4 | Special | 3,5s | Channeled, AoE |
| 5 | Special | 1s | Channeled, buff |
| 6 | Special | 1s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Guard" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Militia Guard" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Militia Guard" 2 true "Channeled, buff"
```

---

### Militia Nun (Level 44)
- **GUID**: -99012450
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 10

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,4s | Channeled, effect |
| 1 | Special | 0,8s | Channeled, effect |
| 2 | BasicAttack | 1,8s | Channeled, effect, TargetAOE |
| 3 | Special | 0,5s | Channeled, effect |
| 4 | Defensive | 0,6s | Channeled, buff, Channeling |
| 5 | Special | 2s | Channeled, projectile |
| 6 | Movement | 1s | Channeled, buff, Channeling |
| 7 | Movement | 1s | Channeled, buff, Channeling |
| 8 | Movement | 1s | Channeled, buff, Channeling |
| 9 | Special | Instant | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Nun" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Militia Nun" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Militia Nun" 2 true "Channeled, effect, TargetAOE"
```

---

### VHunter Leader (Level 44)
- **GUID**: -1449631170
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,3s | Channeled, effect |
| 1 | BasicAttack | 0,7s | Channeled, effect |
| 2 | BasicAttack | 1s | Channeled, effect |
| 3 | BasicAttack | 1s | Channeled, effect |
| 4 | Movement | 0,1s | Channeled, movement, Travel |
| 5 | Special | 0,5s | Channeled, effect |
| 6 | Movement | 1,5s | Channeled, projectile |
| 7 | Movement | 1,5s | Channeled, projectile |
| 8 | Special | 1,2s | Channeled, effect, TargetAOE |
| 9 | Movement | 8s | Channeled |
| 10 | Unknown | 1s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "VHunter Leader" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "VHunter Leader" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "VHunter Leader" 2 true "Channeled, effect"
```

---

### Militia Fabian (Level 46)
- **GUID**: 619948378
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 5

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,1s | Channeled, Requires flight, effect |
| 1 | BasicAttack | 1,1s | Channeled, Requires flight, effect |
| 2 | BasicAttack | 0,1s | Channeled, movement, Travel |
| 3 | Special | 1,4s | Channeled, effect |
| 4 | Special | 1s | 2-hit combo, Channeled, projectile/buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Fabian" 0 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Militia Fabian" 1 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Militia Fabian" 2 true "Channeled, movement, Travel"
```

---

### Militia Scribe (Level 47)
- **GUID**: 1945956671
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,7s | Channeled, projectile |
| 1 | BasicAttack | 0,3s | 2-hit combo, Channeled, projectile |
| 2 | BasicAttack | 2,5s | Channeled, effect |
| 3 | Special | 0,3s | Channeled, buff, Hover |
| 4 | Movement | 2,5s | Channeled, movement |
| 5 | Special | 1,4s | Channeled, projectile |
| 6 | Movement | 1,7s | 2-hit combo, Channeled, projectile |
| 7 | Movement | Instant | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Scribe" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia Scribe" 1 true "2-hit combo, Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia Scribe" 2 true "Channeled, effect"
```

---

### Undead BishopOfShadows (Level 47)
- **GUID**: 939467639
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 6

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,2s | Channeled, projectile |
| 1 | Special | 1,2s | Channeled, projectile |
| 2 | Special | 1,6s | Channeled, effect |
| 3 | Special | 0,8s | Channeled, effect, Travel |
| 4 | Special | 1,2s | Channeled, effect |
| 5 | Special | 1,5s | Channeled, effect, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead BishopOfShadows" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfShadows" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfShadows" 2 true "Channeled, effect"
```

---

### Undead Leader (Level 47)
- **GUID**: -1365931036
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 5

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,3s | 2-hit combo, Channeled, effect |
| 1 | Special | 1,6s | Channeled |
| 2 | Movement | 1,6s | Channeled, effect, Dash |
| 3 | Special | 2s | Channeled, AoE |
| 4 | Special | 0,5s | Channeled, buff, Channeling |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead Leader" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead Leader" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Undead Leader" 2 true "Channeled, effect, Dash"
```

---

### Militia Hound (Level 48)
- **GUID**: -1373413273
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast, Militia
- **Total Abilities**: 6

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | Channeled, effect |
| 1 | BasicAttack | 1,8s | Channeled, effect, Channeling |
| 2 | Movement | 1,45s | 5-hit combo, Channeled, effect, Dash |
| 3 | Movement | 0,75s | Channeled, effect, Dash |
| 4 | Special | Instant | Channeled, effect |
| 5 | Special | 1,2s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Hound" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Militia Hound" 1 true "Channeled, effect, Channeling"
```
```
.bb ability-slot "YourBoss" "Militia Hound" 2 true "5-hit combo, Channeled, effect, Dash"
```

---

### Militia HoundMaster (Level 48)
- **GUID**: -784265984
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,5s | 10-hit combo, Channeled, projectile |
| 1 | BasicAttack | 0,4s | Channeled, effect |
| 2 | BasicAttack | 0,7s | Channeled, effect, Dash |
| 3 | Special | 0,5s | Channeled |
| 4 | Special | 2s | Channeled |
| 5 | Special | 0,6s | Channeled, effect |
| 6 | Movement | 0,2s | Channeled, effect, Dash |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia HoundMaster" 0 true "10-hit combo, Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia HoundMaster" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Militia HoundMaster" 2 true "Channeled, effect, Dash"
```

---

### Militia Glassblower (Level 50)
- **GUID**: 910988233
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,5s | Channeled |
| 1 | BasicAttack | 0,8s | Channeled |
| 2 | BasicAttack | 1,5s | Channeled, effect |
| 3 | Movement | 1,8s | Channeled, effect, Dash |
| 4 | Special | Instant | Channeled, buff |
| 5 | Special | 1,5s | Channeled, buff |
| 6 | Movement | 1,5s | Channeled, buff |
| 7 | Defensive | 0,7s | Channeled, buff |
| 8 | Movement | 0,1s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Glassblower" 0 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Militia Glassblower" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Militia Glassblower" 2 true "Channeled, effect"
```

---

### Militia Longbowman LightArrow (Level 50)
- **GUID**: 850622034
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,6s | Channeled, projectile |
| 1 | BasicAttack | 2,5s | Channeled, projectile |
| 2 | BasicAttack | 1,2s | Channeled |
| 3 | Special | 2,5s | Channeled, projectile |
| 4 | Movement | 0,1s | Channeled, effect, Dash |
| 5 | Special | 1,4s | Channeled, effect, TargetAOE |
| 6 | Movement | 0,6s | Channeled, projectile |
| 7 | Summon | 0,5s | Channeled, effect |
| 8 | Movement | 0,6s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Longbowman LightArrow" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia Longbowman LightArrow" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia Longbowman LightArrow" 2 true "Channeled"
```

---

### Undead Infiltrator (Level 50)
- **GUID**: 613251918
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 3-hit combo, Channeled, effect, Channeling |
| 1 | BasicAttack | 0,7s | Channeled, effect |
| 2 | Special | 0,3s | Channeled, movement/buff, Travel |
| 3 | Special | 0,3s | Channeled, effect/buff, Dash |
| 4 | Special | 0,5s | 3-hit combo, Channeled, projectile |
| 5 | Special | 2s | Channeled, effect |
| 6 | Special | 2,5s | Channeled, buff |
| 7 | Movement | 0,3s | 3-hit combo, Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead Infiltrator" 0 true "3-hit combo, Channeled, effect, Channeling"
```
```
.bb ability-slot "YourBoss" "Undead Infiltrator" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead Infiltrator" 2 true "Channeled, movement/buff, Travel"
```

---

### Geomancer Human (Level 53)
- **GUID**: -1065970933
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,7s | Channeled, projectile |
| 1 | Ultimate | 4s | Channeled, buff |
| 2 | BasicAttack | 1,5s | Channeled, effect |
| 3 | Special | 1,2s | Channeled, AoE |
| 4 | Special | 2s | Channeled, effect |
| 5 | Special | 2s | Channeled, effect |
| 6 | Buff | 0,6s | Channeled, projectile/effect |
| 7 | Buff | 2s | Channeled, buff |
| 8 | Movement | 2,5s | Channeled |
| 9 | Ultimate | 2s | Channeled, movement, Travel |
| 10 | Unknown | 0,75s | Channeled, effect |
| 11 | Ultimate | 4s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Geomancer Human" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Geomancer Human" 1 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Geomancer Human" 2 true "Channeled, effect"
```

---

### Vampire IceRanger (Level 53)
- **GUID**: 795262842
- **Can Fly**: No
- **Features**: Vampire
- **Total Abilities**: 14

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,5s | Channeled, projectile/effect |
| 1 | BasicAttack | 0,6s | Channeled, effect |
| 2 | Special | 0,45s | Channeled |
| 3 | Buff | 1s | Channeled, effect |
| 4 | Special | 0,05s | Channeled, buff, Channeling |
| 5 | Special | 0,7s | Channeled, effect, TargetAOE |
| 6 | Special | 2,1s | Channeled, effect/AoE, TargetAOE |
| 7 | Movement | 0,9s | Channeled, projectile |
| 8 | Special | 2,1s | Channeled, effect/AoE, TargetAOE |
| 9 | Movement | 0,7s | Channeled, effect, TargetAOE |
| 10 | Unknown | 1,6s | Channeled |
| 11 | Special | 1s | Channeled, buff |
| 12 | Movement | 0,1s | Channeled, movement/buff, Travel |
| 13 | Special | 0,8s | Channeled, projectile/effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Vampire IceRanger" 0 true "Channeled, projectile/effect"
```
```
.bb ability-slot "YourBoss" "Vampire IceRanger" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Vampire IceRanger" 2 true "Channeled"
```

---

### Wendigo (Level 53)
- **GUID**: 24378719
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,7s | 3-hit combo, Channeled, effect |
| 1 | BasicAttack | 0,7s | 3-hit combo, Channeled, projectile/buff |
| 2 | Special | 2s | Channeled, AoE |
| 3 | Movement | 1,1s | Channeled |
| 4 | BasicAttack | 0,5s | Channeled, effect/buff |
| 5 | Special | 1,2s | Channeled, effect/buff, Channeling |
| 6 | Movement | 2s | Channeled, buff |
| 7 | Movement | Instant | Channeled, buff |
| 8 | Movement | 1,2s | Channeled, effect/buff, Channeling |
| 9 | Movement | 0,7s | Channeled |
| 10 | BasicAttack | 0,4s | Channeled, effect/buff |
| 11 | Movement | 0,5s | Channeled, movement, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Wendigo" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Wendigo" 1 true "3-hit combo, Channeled, projectile/buff"
```
```
.bb ability-slot "YourBoss" "Wendigo" 2 true "Channeled, AoE"
```

---

### Undead ArenaChampion (Level 55)
- **GUID**: -753453016
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 16

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | 4-hit combo, Channeled, effect |
| 1 | BasicAttack | 0,5s | Channeled, movement |
| 2 | BasicAttack | 0,5s | Channeled |
| 3 | Special | 0,5s | Channeled |
| 4 | BasicAttack | 1,4s | 2-hit combo, Channeled, projectile |
| 5 | Special | 0,1s | Channeled, effect, Dash |
| 6 | BasicAttack | 0,5s | Channeled, effect, TargetAOE |
| 7 | Movement | 1,2s | Channeled, effect/buff, Dash |
| 8 | Movement | 0,6s | Channeled, effect |
| 9 | Movement | Instant | Channeled, buff, Hover |
| 10 | Buff | Instant | Channeled, buff |
| 11 | Unknown | 0,8s | Channeled, effect |
| 12 | Unknown | 0,6s | Channeled, effect |
| 13 | BasicAttack | 0,7s | Channeled, buff, Channeling |
| 14 | Unknown | 0,4s | Channeled, AoE |
| 15 | Unknown | Instant | Channeled, buff, Hover |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead ArenaChampion" 0 true "4-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead ArenaChampion" 1 true "Channeled, movement"
```
```
.bb ability-slot "YourBoss" "Undead ArenaChampion" 2 true "Channeled"
```

---

### Bandit Chaosarrow GateBoss Minor (Level 57)
- **GUID**: 1854211210
- **Can Fly**: No
- **Features**: Bandit, GateBoss, GateBoss_Minor
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,5s | Channeled, buff |
| 1 | BasicAttack | 0,1s | Channeled, buff |
| 2 | BasicAttack | 0,35s | Channeled |
| 3 | Buff | 2s | Channeled, buff |
| 4 | Special | 1,2s | Channeled, buff |
| 5 | Buff | 2s | Channeled, buff |
| 6 | Movement | 0,9s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Chaosarrow GateBoss Minor" 0 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Bandit Chaosarrow GateBoss Minor" 1 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Bandit Chaosarrow GateBoss Minor" 2 true "Channeled"
```

---

### Bandit Foreman GateBoss Minor (Level 57)
- **GUID**: 17609984
- **Can Fly**: No
- **Features**: Bandit, GateBoss, GateBoss_Minor
- **Total Abilities**: 13

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 2s | Channeled, projectile |
| 1 | BasicAttack | 1s | Channeled, effect, TargetAOE |
| 2 | BasicAttack | 0,5s | Channeled |
| 3 | Special | 1,5s | Channeled, effect |
| 4 | Special | 0,5s | Channeled, AoE |
| 5 | Special | 1,3s | Channeled, projectile |
| 6 | Movement | 0,4s | Channeled, projectile |
| 7 | Movement | 2,5s | Channeled, effect, TargetAOE |
| 8 | Movement | 3s | Channeled, buff |
| 9 | Movement | 0,25s | Channeled, projectile |
| 10 | Unknown | 2,4s | Channeled, buff |
| 11 | Unknown | 1,4s | Channeled, projectile |
| 12 | Special | 1,4s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Foreman GateBoss Minor" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Bandit Foreman GateBoss Minor" 1 true "Channeled, effect, TargetAOE"
```
```
.bb ability-slot "YourBoss" "Bandit Foreman GateBoss Minor" 2 true "Channeled"
```

---

### Bandit StoneBreaker GateBoss Minor (Level 57)
- **GUID**: -1420322422
- **Can Fly**: No
- **Features**: Bandit, GateBoss, GateBoss_Minor
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | Channeled, Requires flight, effect |
| 1 | BasicAttack | 1s | Channeled, Requires flight, effect |
| 2 | Special | 1,2s | Channeled, projectile/effect |
| 3 | Special | 1,2s | Channeled, projectile/effect |
| 4 | Special | 1s | Channeled, effect, Channeling |
| 5 | Special | 0,3s | Channeled, effect |
| 6 | Special | 2s | Channeled, effect |
| 7 | Movement | 2s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit StoneBreaker GateBoss Minor" 0 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit StoneBreaker GateBoss Minor" 1 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit StoneBreaker GateBoss Minor" 2 true "Channeled, projectile/effect"
```

---

### Bandit Tourok GateBoss Minor (Level 57)
- **GUID**: 478580792
- **Can Fly**: No
- **Features**: Bandit, GateBoss, GateBoss_Minor
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, Requires flight, effect |
| 1 | BasicAttack | 0,4s | Channeled, Requires flight, effect |
| 2 | Special | 1,6s | Channeled, effect |
| 3 | Special | 1,4s | Channeled, effect, Dash |
| 4 | Special | 0,4s | Channeled, buff |
| 5 | Special | 0,6s | Channeled, effect, Dash |
| 6 | Special | 0,5s | Channeled, buff, Channeling |
| 7 | Movement | 1,2s | Channeled, projectile/buff |
| 8 | Special | 1,4s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Bandit Tourok GateBoss Minor" 0 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit Tourok GateBoss Minor" 1 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Bandit Tourok GateBoss Minor" 2 true "Channeled, effect"
```

---

### Frostarrow GateBoss Minor (Level 57)
- **GUID**: 1318855899
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Minor
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 2,3s | Channeled |
| 1 | BasicAttack | 0,1s | Channeled, buff |
| 2 | BasicAttack | 0,35s | Channeled |
| 3 | Special | 1,5s | Channeled, effect |
| 4 | Special | 1,6s | Channeled |
| 5 | Special | 1,5s | Channeled, effect |
| 6 | Special | 0,1s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Frostarrow GateBoss Minor" 0 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Frostarrow GateBoss Minor" 1 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Frostarrow GateBoss Minor" 2 true "Channeled"
```

---

### GateBossComponentsTemplate Minor (Level 57)
- **GUID**: -222706317
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Minor
- **Total Abilities**: 0


**Example Commands:**

---

### Militia BishopOfDunley (Level 57)
- **GUID**: -680831417
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,5s | Channeled, projectile |
| 1 | Special | 1,2s | Channeled |
| 2 | Summon | 0,3s | Channeled, effect |
| 3 | Special | 0,7s | Channeled, effect |
| 4 | Special | 0,7s | Channeled, effect, Channeling |
| 5 | Special | 0,05s | Channeled, buff |
| 6 | Special | 0,05s | Channeled, buff, Hover |
| 7 | Summon | 0,3s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia BishopOfDunley" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia BishopOfDunley" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Militia BishopOfDunley" 2 true "Channeled, effect"
```

---

### Militia Guard GateBoss Minor (Level 57)
- **GUID**: 1494126678
- **Can Fly**: No
- **Features**: Militia, GateBoss, GateBoss_Minor
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,2s | 2-hit combo, Channeled, effect |
| 1 | Special | 2s | Channeled |
| 2 | Special | 0,4s | Channeled, buff |
| 3 | Special | 0,8s | Channeled, effect/buff |
| 4 | Special | 3,5s | Channeled, AoE |
| 5 | Special | 1s | Channeled, buff |
| 6 | Special | 1s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Guard GateBoss Minor" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Militia Guard GateBoss Minor" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Militia Guard GateBoss Minor" 2 true "Channeled, buff"
```

---

### Poloma GateBoss Minor (Level 57)
- **GUID**: -1381375644
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Minor
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,5s | Channeled |
| 1 | BasicAttack | 1,7s | Channeled, projectile |
| 2 | BasicAttack | 0,2s | Channeled, buff |
| 3 | Special | 1,6s | Channeled, effect, TargetAOE |
| 4 | Special | 0,3s | Channeled, effect |
| 5 | Special | 0,2s | Channeled, buff, Hover |
| 6 | Special | 1,5s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Poloma GateBoss Minor" 0 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Poloma GateBoss Minor" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Poloma GateBoss Minor" 2 true "Channeled, buff"
```

---

### Undead BishopOfDeath GateBoss Minor (Level 57)
- **GUID**: -1822337177
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Minor
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,4s | Channeled, projectile |
| 1 | BasicAttack | 1,4s | Channeled, projectile |
| 2 | BasicAttack | 0,9s | Channeled, effect, TargetAOE |
| 3 | Special | 0,9s | Channeled, effect, TargetAOE |
| 4 | Movement | 0,4s | Channeled, movement, Travel |
| 5 | Special | 1,2s | Channeled, buff |
| 6 | Movement | 1,2s | Channeled, buff |
| 7 | Movement | 0,9s | 3-hit combo, Channeled, effect, TargetAOE |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead BishopOfDeath GateBoss Minor" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfDeath GateBoss Minor" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfDeath GateBoss Minor" 2 true "Channeled, effect, TargetAOE"
```

---

### Undead Leader GateBoss Minor (Level 57)
- **GUID**: -989493184
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Minor
- **Total Abilities**: 5

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,3s | 2-hit combo, Channeled, effect |
| 1 | Special | 1,6s | Channeled |
| 2 | Movement | 1,6s | Channeled, effect, Dash |
| 3 | Special | 2s | Channeled, AoE |
| 4 | Special | 0,5s | Channeled, buff, Channeling |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead Leader GateBoss Minor" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead Leader GateBoss Minor" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Undead Leader GateBoss Minor" 2 true "Channeled, effect, Dash"
```

---

### Vampire HighLord (Level 57)
- **GUID**: -496360395
- **Can Fly**: No
- **Features**: Vampire
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 2-hit combo, Channeled, effect |
| 1 | BasicAttack | 1s | Channeled, buff, Travel |
| 2 | BasicAttack | 1s | Channeled, effect |
| 3 | Movement | 0,8s | Channeled, effect, Dash |
| 4 | BasicAttack | 0,15s | Channeled, effect, TargetAOE |
| 5 | Special | 0,6s | Channeled, effect |
| 6 | Special | 0,6s | Channeled, effect, Travel |
| 7 | Movement | 1s | Channeled, effect |
| 8 | Special | 0,7s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Vampire HighLord" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Vampire HighLord" 1 true "Channeled, buff, Travel"
```
```
.bb ability-slot "YourBoss" "Vampire HighLord" 2 true "Channeled, effect"
```

---

### VHunter Jade (Level 57)
- **GUID**: -1968372384
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,75s | 12-hit combo, Channeled, projectile |
| 1 | BasicAttack | 0,75s | 2-hit combo, Channeled, projectile |
| 2 | BasicAttack | 1,6s | Channeled, projectile |
| 3 | Special | 0,5s | Channeled |
| 4 | Special | 0,5s | Channeled |
| 5 | Special | 0,8s | Channeled, projectile |
| 6 | Movement | 0,5s | Channeled, projectile |
| 7 | Movement | 0,3s | Channeled, effect, Travel |
| 8 | Movement | Instant | Channeled, buff |
| 9 | Movement | 1,1s | Channeled, buff, Channeling |
| 10 | Unknown | 0,75s | 4-hit combo, Channeled, projectile |
| 11 | Unknown | 0,75s | 6-hit combo, Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "VHunter Jade" 0 true "12-hit combo, Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "VHunter Jade" 1 true "2-hit combo, Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "VHunter Jade" 2 true "Channeled, projectile"
```

---

### VHunter Leader GateBoss Minor (Level 57)
- **GUID**: 2009018555
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Minor
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,3s | Channeled, effect |
| 1 | BasicAttack | 0,7s | Channeled, effect |
| 2 | BasicAttack | 1s | Channeled, effect |
| 3 | BasicAttack | 1s | Channeled, effect |
| 4 | Movement | 0,1s | Channeled, movement, Travel |
| 5 | Special | 0,5s | Channeled, effect |
| 6 | Movement | 1,5s | Channeled, projectile |
| 7 | Movement | 1,5s | Channeled, projectile |
| 8 | Special | 1,2s | Channeled, effect, TargetAOE |
| 9 | Movement | 8s | Channeled |
| 10 | Unknown | 1s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "VHunter Leader GateBoss Minor" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "VHunter Leader GateBoss Minor" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "VHunter Leader GateBoss Minor" 2 true "Channeled, effect"
```

---

### Militia Leader (Level 58)
- **GUID**: 1688478381
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Militia
- **Total Abilities**: 14

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,2s | Channeled, effect, Hover |
| 1 | BasicAttack | 1,2s | Channeled, projectile |
| 2 | Movement | 0,5s | Channeled, movement/buff, Travel |
| 3 | Special | 10s | Channeled |
| 4 | Special | 2s | Channeled, effect |
| 5 | Special | 1s | Channeled, effect |
| 6 | Movement | 0,7s | Channeled, buff/effect |
| 7 | BasicAttack | 1,2s | Channeled, effect, Hover |
| 8 | BasicAttack | 0,7s | Channeled, effect, Hover |
| 9 | BasicAttack | 1,2s | Channeled, projectile |
| 10 | BasicAttack | 0,7s | Channeled, projectile |
| 11 | Movement | 0,4s | Channeled, movement/buff, Travel |
| 12 | Unknown | 0,1s | Channeled, effect, Dash |
| 13 | Unknown | 0,1s | Channeled, effect, Dash |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Leader" 0 true "Channeled, effect, Hover"
```
```
.bb ability-slot "YourBoss" "Militia Leader" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia Leader" 2 true "Channeled, movement/buff, Travel"
```

---

### Gloomrot Iva (Level 60)
- **GUID**: 172235178
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Gloomrot
- **Total Abilities**: 21

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,2s | 3-hit combo, Channeled |
| 1 | Special | 1,4s | Channeled, effect/buff, Channeling |
| 2 | BasicAttack | 1,5s | Channeled, effect, Channeling |
| 3 | Special | 1,5s | Channeled, effect |
| 4 | Special | 2,5s | Channeled, effect |
| 5 | Special | 1,4s | Channeled, effect/buff, Channeling |
| 6 | Movement | 1,5s | Channeled, effect |
| 7 | Movement | 2s | Channeled, projectile/effect |
| 8 | Movement | 0,6s | Channeled, movement, Travel |
| 9 | Movement | 2,5s | Channeled, buff |
| 10 | Unknown | 2,5s | Channeled, buff |
| 11 | Unknown | 2,5s | Channeled, buff |
| 12 | Unknown | 2,5s | Channeled, buff |
| 13 | Unknown | 1,5s | Channeled, effect |
| 14 | Unknown | 1,2s | Channeled, effect |
| 15 | Unknown | 2s | Channeled, effect |
| 16 | Unknown | 2s | Channeled, effect |
| 17 | Unknown | Instant | Channeled, buff, Hover |
| 18 | Unknown | 0,6s | Channeled, movement, Travel |
| 19 | Unknown | 2s | Channeled, effect, Travel |
| 20 | Unknown | Instant | Channeled, effect, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot Iva" 0 true "3-hit combo, Channeled"
```
```
.bb ability-slot "YourBoss" "Gloomrot Iva" 1 true "Channeled, effect/buff, Channeling"
```
```
.bb ability-slot "YourBoss" "Gloomrot Iva" 2 true "Channeled, effect, Channeling"
```

---

### Gloomrot Voltage (Level 60)
- **GUID**: -1101874342
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Gloomrot
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | 3-hit combo, Channeled, effect |
| 1 | Movement | 0,4s | Channeled, movement, Travel |
| 2 | BasicAttack | 0,8s | Channeled, effect |
| 3 | Special | 0,4s | Channeled, effect/buff |
| 4 | Movement | 0,6s | Channeled, movement, Travel |
| 5 | Special | 0,9s | Channeled |
| 6 | Movement | 2s | Channeled, effect, Channeling |
| 7 | Movement | 2s | Channeled, buff |
| 8 | Movement | 0,4s | Channeled, effect |
| 9 | Movement | 1s | Channeled |
| 10 | Unknown | 0,8s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot Voltage" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Gloomrot Voltage" 1 true "Channeled, movement, Travel"
```
```
.bb ability-slot "YourBoss" "Gloomrot Voltage" 2 true "Channeled, effect"
```

---

### Gloomrot Purifier (Level 61)
- **GUID**: 106480588
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Gloomrot
- **Total Abilities**: 15

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | Channeled, effect |
| 1 | Defensive | 0,4s | Channeled, buff, Channeling |
| 2 | BasicAttack | 1,2s | Channeled, effect/buff |
| 3 | Special | 0,3s | Channeled |
| 4 | Defensive | 0,4s | Channeled, buff, Channeling |
| 5 | Special | 1,4s | Channeled, effect |
| 6 | Movement | 0,4s | Channeled, effect/buff, Dash |
| 7 | Movement | 0,2s | Channeled, effect, Travel |
| 8 | Movement | 1,5s | Channeled, effect |
| 9 | Movement | 0,6s | Channeled, effect |
| 10 | Unknown | 0,2s | Channeled, effect, Travel |
| 11 | BasicAttack | 0,6s | Channeled, effect |
| 12 | BasicAttack | 0,6s | Channeled, effect |
| 13 | Unknown | 0,1s | Channeled, buff/effect |
| 14 | Buff | Instant | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot Purifier" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Gloomrot Purifier" 1 true "Channeled, buff, Channeling"
```
```
.bb ability-slot "YourBoss" "Gloomrot Purifier" 2 true "Channeled, effect/buff"
```

---

### Spider Queen (Level 63)
- **GUID**: -548489519
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,4s | Channeled, effect |
| 1 | BasicAttack | 1s | Channeled, effect |
| 2 | Special | 2,2s | Channeled, projectile |
| 3 | Special | 1,8s | Channeled, effect, TargetAOE |
| 4 | Special | 2s | Channeled, effect |
| 5 | Special | 0,8s | Channeled, effect, Projectile |
| 6 | Special | 1,2s | Channeled, projectile |
| 7 | Summon | 1s | 4-hit combo, Channeled, effect |
| 8 | Movement | 1s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Spider Queen" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Spider Queen" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Spider Queen" 2 true "Channeled, projectile"
```

---

### Undead ZealousCultist (Level 63)
- **GUID**: -1208888966
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 6

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | Channeled, effect |
| 1 | BasicAttack | 0,3s | Channeled, effect |
| 2 | Summon | 1,5s | Channeled, buff |
| 3 | Special | 0,4s | Channeled, buff, Channeling |
| 4 | Special | 0,1s | Channeled, buff |
| 5 | BasicAttack | 0,9s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead ZealousCultist" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead ZealousCultist" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead ZealousCultist" 2 true "Channeled, buff"
```

---

### Villager CursedWanderer (Level 63)
- **GUID**: 109969450
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Cursed
- **Total Abilities**: 3

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, effect |
| 1 | BasicAttack | 1s | Channeled |
| 2 | BasicAttack | 2s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Villager CursedWanderer" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Villager CursedWanderer" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Villager CursedWanderer" 2 true "Channeled"
```

---

### Cursed ToadKing (Level 64)
- **GUID**: -203043163
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast, Cursed
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | 2-hit combo, Channeled, effect |
| 1 | BasicAttack | 1,2s | Channeled, effect |
| 2 | Movement | 0,6s | Channeled, movement, Travel |
| 3 | Movement | 0,8s | Channeled, movement, Travel |
| 4 | Special | 2s | Channeled, effect |
| 5 | Special | 1,2s | Channeled, effect |
| 6 | Movement | 0,7s | Channeled |
| 7 | Movement | 0,6s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Cursed ToadKing" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Cursed ToadKing" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Cursed ToadKing" 2 true "Channeled, movement, Travel"
```

---

### WerewolfChieftain Human (Level 64)
- **GUID**: -1505705712
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Werewolf
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,5s | 2-hit combo, Channeled, effect |
| 1 | BasicAttack | 0,8s | 2-hit combo, Channeled, effect |
| 2 | Special | 1s | Channeled, effect, Dash |
| 3 | Special | 1,2s | Channeled, effect, Dash |
| 4 | Special | 0,1s | Channeled, buff/effect |
| 5 | BasicAttack | 0,3s | 3-hit combo, Channeled, effect |
| 6 | BasicAttack | 1,2s | Channeled, buff |
| 7 | Movement | 1,2s | Channeled, effect |
| 8 | Buff | 1s | Channeled, buff, Hover |
| 9 | Movement | 0,3s | Channeled, effect |
| 10 | BasicAttack | 1,2s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "WerewolfChieftain Human" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "WerewolfChieftain Human" 1 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "WerewolfChieftain Human" 2 true "Channeled, effect, Dash"
```

---

### WerewolfChieftain ShadowClone (Level 64)
- **GUID**: -1699898875
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast, Werewolf
- **Total Abilities**: 1

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1s | Channeled, effect, Dash |

**Example Commands:**
```
.bb ability-slot "YourBoss" "WerewolfChieftain ShadowClone" 0 true "Channeled, effect, Dash"
```

---

### Undead CursedSmith (Level 65)
- **GUID**: 326378955
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Cursed
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | 4-hit combo, Channeled, effect |
| 1 | BasicAttack | 0,3s | Channeled, effect/buff, Travel |
| 2 | BasicAttack | 1,5s | Channeled, projectile |
| 3 | Movement | 1,5s | Channeled, effect |
| 4 | Movement | 1,5s | Channeled, effect |
| 5 | Movement | 0,6s | Channeled, effect, Dash |
| 6 | BasicAttack | 1,75s | Channeled, effect |
| 7 | Summon | 1,75s | Channeled, effect |
| 8 | Summon | 1,75s | Channeled, effect |
| 9 | Summon | 1,75s | Channeled, effect |
| 10 | Summon | 1,75s | Channeled, effect |
| 11 | Summon | 0,8s | Channeled, effect, Channeling |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead CursedSmith" 0 true "4-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead CursedSmith" 1 true "Channeled, effect/buff, Travel"
```
```
.bb ability-slot "YourBoss" "Undead CursedSmith" 2 true "Channeled, projectile"
```

---

### ChurchOfLight Overseer (Level 66)
- **GUID**: -26105228
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Church
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 2-hit combo, Channeled, Requires flight, effect |
| 1 | BasicAttack | 1,8s | Channeled, effect |
| 2 | BasicAttack | 2,2s | Channeled, effect/buff |
| 3 | Movement | 1s | Channeled, effect, Dash |
| 4 | Movement | 0,7s | Channeled |
| 5 | Special | Instant | Channeled, effect, Hover |
| 6 | Movement | 1,2s | Channeled, projectile/effect |
| 7 | Movement | 0,5s | Channeled, effect, TargetAOE |

**Example Commands:**
```
.bb ability-slot "YourBoss" "ChurchOfLight Overseer" 0 true "2-hit combo, Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Overseer" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Overseer" 2 true "Channeled, effect/buff"
```

---

### ArchMage (Level 70)
- **GUID**: -2013903325
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 10

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | 3-hit combo, Channeled, projectile/buff |
| 1 | Movement | 1,5s | Channeled, projectile |
| 2 | BasicAttack | 3,5s | Channeled, AoE |
| 3 | Special | 0,2s | Channeled, effect, Travel |
| 4 | Special | 1,8s | Channeled, effect, Projectile |
| 5 | Special | 3s | Channeled, effect/AoE |
| 6 | Movement | 0,2s | Channeled, effect, Travel |
| 7 | Movement | 0,8s | Channeled, effect |
| 8 | Movement | 1,2s | Channeled |
| 9 | Movement | 0,1s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "ArchMage" 0 true "3-hit combo, Channeled, projectile/buff"
```
```
.bb ability-slot "YourBoss" "ArchMage" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "ArchMage" 2 true "Channeled, AoE"
```

---

### ChurchOfLight Sommelier (Level 70)
- **GUID**: 192051202
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Church
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, effect, Hover |
| 1 | BasicAttack | 0,5s | Channeled, effect |
| 2 | Special | 2s | Channeled, projectile |
| 3 | Special | 0,2s | Channeled, buff |
| 4 | Special | 0,5s | Channeled, buff |
| 5 | Special | 0,5s | Channeled, buff |
| 6 | BasicAttack | 0,9s | Channeled, Requires flight, effect, Hover |
| 7 | Movement | 2,4s | Channeled, effect, Hover |
| 8 | Movement | 0,5s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "ChurchOfLight Sommelier" 0 true "Channeled, effect, Hover"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Sommelier" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Sommelier" 2 true "Channeled, projectile"
```

---

### Harpy Matriarch (Level 70)
- **GUID**: 685266977
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,1s | Channeled, effect |
| 1 | BasicAttack | 1,45s | Channeled, effect |
| 2 | Movement | 1s | Channeled, effect, Dash |
| 3 | Special | 2s | Channeled, effect, Channeling |
| 4 | Special | 1,1s | Channeled, projectile, Channeling |
| 5 | Special | 1,2s | Channeled, effect, TargetAOE |
| 6 | Movement | 1,2s | Channeled, movement, Travel |
| 7 | Movement | 0,5s | Channeled, effect, Dash |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Harpy Matriarch" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Harpy Matriarch" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Harpy Matriarch" 2 true "Channeled, effect, Dash"
```

---

### Gloomrot TheProfessor (Level 74)
- **GUID**: 814083983
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Gloomrot
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,5s | Channeled, projectile, Hover |
| 1 | BasicAttack | 2s | Channeled, projectile |
| 2 | BasicAttack | 3s | Channeled, buff, Channeling |
| 3 | Movement | 0,5s | Channeled, buff, Channeling |
| 4 | Movement | 0,05s | Channeled, movement |
| 5 | Special | 0,01s | Channeled, effect |
| 6 | Movement | 2s | Channeled, effect |
| 7 | Movement | 0,05s | Channeled, effect, Channeling |
| 8 | Movement | 0,01s | Channeled, effect |
| 9 | Movement | 0,01s | Channeled, effect |
| 10 | Unknown | 2s | Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot TheProfessor" 0 true "Channeled, projectile, Hover"
```
```
.bb ability-slot "YourBoss" "Gloomrot TheProfessor" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Gloomrot TheProfessor" 2 true "Channeled, buff, Channeling"
```

---

### Blackfang CarverBoss (Level 75)
- **GUID**: -1669199769
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,6s | 2-hit combo, Channeled, Requires flight, effect |
| 1 | Movement | 0,3s | Channeled, movement/buff, Travel |
| 2 | BasicAttack | 1,3s | Channeled, effect, Dash |
| 3 | Special | 1,4s | Channeled, buff/effect |
| 4 | Special | 1s | 2-hit combo, Channeled, effect/buff/projectile |
| 5 | Movement | 0,3s | Channeled, movement/buff, Travel |
| 6 | Movement | 0,8s | Channeled, effect |
| 7 | Summon | 0,7s | Channeled, effect/buff |
| 8 | Movement | 0,6s | Channeled, effect |
| 9 | Movement | 1,2s | Channeled, projectile |
| 10 | Unknown | 2s | Channeled, effect/buff |
| 11 | Movement | 0,3s | Channeled, movement/buff, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Blackfang CarverBoss" 0 true "2-hit combo, Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Blackfang CarverBoss" 1 true "Channeled, movement/buff, Travel"
```
```
.bb ability-slot "YourBoss" "Blackfang CarverBoss" 2 true "Channeled, effect, Dash"
```

---

### Blackfang Livith (Level 75)
- **GUID**: -1383529374
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 15

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | 3-hit combo, Channeled, effect |
| 1 | Movement | 1,5s | Channeled, effect, Dash |
| 2 | BasicAttack | Instant | Channeled, buff |
| 3 | Special | 0,7s | Channeled, effect |
| 4 | Special | 0,6s | Channeled, effect |
| 5 | Special | 1,8s | Channeled, effect |
| 6 | Movement | 0,9s | 2-hit combo, Channeled, projectile/buff |
| 7 | Movement | 0,5s | Channeled |
| 8 | Movement | 2s | Channeled, buff |
| 9 | Movement | Instant | Channeled, buff |
| 10 | Movement | 1,5s | Channeled, effect/buff, Dash |
| 11 | Unknown | Instant | Channeled, buff |
| 12 | Unknown | Instant | Channeled, buff |
| 13 | Unknown | Instant | Channeled, buff, Hover |
| 14 | Special | 0,1s | Channeled, buff, Channeling |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Blackfang Livith" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Blackfang Livith" 1 true "Channeled, effect, Dash"
```
```
.bb ability-slot "YourBoss" "Blackfang Livith" 2 true "Channeled, buff"
```

---

### Blackfang Lucie (Level 76)
- **GUID**: 1295855316
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 13

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | 3-hit combo, Channeled, projectile/buff |
| 1 | BasicAttack | 0,6s | Channeled, projectile |
| 2 | BasicAttack | 0,3s | Channeled, effect, TargetAOE |
| 3 | Special | 0,8s | Channeled, effect, TargetAOE |
| 4 | Defensive | 1,2s | Channeled, buff |
| 5 | Special | 0,5s | Channeled, effect, TargetAOE |
| 6 | Movement | 1,2s | Channeled, projectile |
| 7 | Movement | 0,1s | Channeled, buff, Hover |
| 8 | Movement | 0,4s | Channeled, buff, Hover |
| 9 | Movement | 0,5s | Channeled |
| 10 | Unknown | 0,1s | Channeled, buff, Hover |
| 11 | Unknown | 1,5s | Channeled, buff, Hover |
| 12 | Unknown | 0,3s | Channeled, effect, TargetAOE |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Blackfang Lucie" 0 true "3-hit combo, Channeled, projectile/buff"
```
```
.bb ability-slot "YourBoss" "Blackfang Lucie" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Blackfang Lucie" 2 true "Channeled, effect, TargetAOE"
```

---

### Cursed Witch (Level 76)
- **GUID**: -910296704
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Cursed
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, projectile |
| 1 | BasicAttack | 1s | Channeled |
| 2 | Special | 4s | Channeled, effect |
| 3 | Special | 0,5s | Channeled, buff |
| 4 | Special | 1,4s | Channeled, effect |
| 5 | Special | 0,3s | Channeled, buff, Hover |
| 6 | Movement | 2,5s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Cursed Witch" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Cursed Witch" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Cursed Witch" 2 true "Channeled, effect"
```

---

### Winter Yeti (Level 76)
- **GUID**: -1347412392
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 16

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 2-hit combo, Channeled, effect |
| 1 | Movement | 0,3s | Channeled, effect, Travel |
| 2 | BasicAttack | 1,8s | Channeled |
| 3 | Special | 1s | Channeled |
| 4 | Special | 0,6s | Channeled |
| 5 | Special | 1,2s | Channeled, buff |
| 6 | Movement | 1,2s | Channeled, effect |
| 7 | Movement | 0,5s | Channeled, effect, Travel |
| 8 | Movement | 0,5s | Channeled, effect |
| 9 | BasicAttack | 1,4s | Channeled, effect, Dash |
| 10 | Unknown | Instant | Channeled, buff, Hover |
| 11 | Unknown | 0,7s | Channeled, projectile |
| 12 | Movement | 0,3s | Channeled, movement, Travel |
| 13 | Unknown | 0,5s | Channeled, effect |
| 14 | Buff | Instant | Channeled, effect |
| 15 | Unknown | 0,3s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Winter Yeti" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Winter Yeti" 1 true "Channeled, effect, Travel"
```
```
.bb ability-slot "YourBoss" "Winter Yeti" 2 true "Channeled"
```

---

### ChurchOfLight Cardinal (Level 79)
- **GUID**: 114912615
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Church
- **Total Abilities**: 7

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,6s | Channeled, buff |
| 1 | BasicAttack | 1s | Channeled, buff, Channeling |
| 2 | BasicAttack | 1,2s | Channeled, effect, Travel |
| 3 | Special | 0,3s | Channeled, buff, Hover |
| 4 | Special | 1,7s | Channeled, projectile |
| 5 | Summon | 1s | Channeled, effect |
| 6 | Summon | 1s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "ChurchOfLight Cardinal" 0 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Cardinal" 1 true "Channeled, buff, Channeling"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Cardinal" 2 true "Channeled, effect, Travel"
```

---

### Gloomrot RailgunSergeant (Level 79)
- **GUID**: 2054432370
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Gloomrot
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,7s | Channeled, effect, Channeling |
| 1 | BasicAttack | 2s | Channeled, effect |
| 2 | BasicAttack | 1,4s | Channeled, effect |
| 3 | Special | 0,3s | Channeled, effect, Travel |
| 4 | Special | 3s | Channeled, effect, Travel |
| 5 | Special | 0,3s | Channeled, projectile |
| 6 | Movement | 2s | Channeled, effect |
| 7 | Movement | 0,5s | Channeled, effect/buff, TargetAOE |
| 8 | Movement | 2s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot RailgunSergeant" 0 true "Channeled, effect, Channeling"
```
```
.bb ability-slot "YourBoss" "Gloomrot RailgunSergeant" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Gloomrot RailgunSergeant" 2 true "Channeled, effect"
```

---

### VHunter CastleMan (Level 80)
- **GUID**: 336560131
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 25

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,1s | 3-hit combo, Channeled, effect |
| 1 | Special | 0,8s | 3-hit combo, Channeled, effect |
| 2 | Special | 0,8s | 3-hit combo, Channeled, effect |
| 3 | Special | 1s | Channeled, effect |
| 4 | Special | 0,8s | Channeled, effect, TargetAOE |
| 5 | Special | 0,2s | Channeled, buff |
| 6 | Special | 2s | Channeled, AoE |
| 7 | Special | 1,2s | 3-hit combo, Channeled, effect |
| 8 | Special | 1,4s | Channeled, projectile |
| 9 | Special | 0,6s | Channeled, effect, TargetAOE |
| 10 | Special | 0,3s | Channeled, effect, Travel |
| 11 | Special | 0,1s | Channeled, effect, Dash |
| 12 | Special | 1,8s | 3-hit combo, Channeled, projectile |
| 13 | Special | 0,7s | Channeled |
| 14 | Special | 0,4s | Channeled, projectile |
| 15 | Special | 0,9s | Channeled, buff |
| 16 | Special | 0,6s | Channeled, buff |
| 17 | Special | 0,1s | Channeled, buff |
| 18 | Special | 2,5s | Channeled, AoE |
| 19 | Special | Instant | Channeled, projectile |
| 20 | Special | 5s | Channeled, AoE |
| 21 | Special | 1,2s | Channeled, effect |
| 22 | Special | 1,8s | Channeled, effect, Projectile |
| 23 | Special | 0,9s | Channeled |
| 24 | Special | 1,8s | Channeled, effect, Projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "VHunter CastleMan" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "VHunter CastleMan" 1 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "VHunter CastleMan" 2 true "3-hit combo, Channeled, effect"
```

---

### Blackfang Valyr (Level 82)
- **GUID**: 173259239
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,6s | 2-hit combo, Channeled, effect |
| 1 | BasicAttack | 0,1s | Channeled, effect, Dash |
| 2 | Buff | 0,8s | Channeled |
| 3 | Movement | 1,25s | 2-hit combo, Channeled, projectile/effect |
| 4 | Special | 0,01s | Channeled, buff, TargetAOE |
| 5 | BasicAttack | 1s | 2-hit combo, Channeled, effect |
| 6 | Movement | 1,2s | Channeled, effect, Dash |
| 7 | Movement | 1s | Channeled, effect, TargetAOE |
| 8 | Movement | 4,5s | Channeled, buff |
| 9 | Movement | 1,2s | Channeled, effect |
| 10 | Unknown | 0,01s | Channeled, buff, TargetAOE |
| 11 | Unknown | 0,1s | Channeled, buff, Hover |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Blackfang Valyr" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Blackfang Valyr" 1 true "Channeled, effect, Dash"
```
```
.bb ability-slot "YourBoss" "Blackfang Valyr" 2 true "Channeled"
```

---

### BatVampire (Level 84)
- **GUID**: 1112948824
- **Can Fly**: No
- **Features**: Vampire
- **Total Abilities**: 10

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,5s | 3-hit combo, Channeled, effect |
| 1 | BasicAttack | 0,15s | Channeled, effect, TargetAOE |
| 2 | BasicAttack | 0,15s | Channeled, effect, TargetAOE |
| 3 | Special | 1,5s | Channeled, effect/projectile, Hover |
| 4 | Movement | 0,7s | Channeled, movement/buff, Travel |
| 5 | Special | 2,5s | Channeled, projectile/effect |
| 6 | Movement | 0,7s | Channeled, effect, Dash |
| 7 | Summon | 1,8s | Channeled, effect, Travel |
| 8 | Movement | 1,7s | Channeled, effect, Channeling |
| 9 | Movement | 0,3s | Channeled, effect, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "BatVampire" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "BatVampire" 1 true "Channeled, effect, TargetAOE"
```
```
.bb ability-slot "YourBoss" "BatVampire" 2 true "Channeled, effect, TargetAOE"
```

---

### Cursed MountainBeast (Level 84)
- **GUID**: -1936575244
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast, Cursed
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 3-hit combo, Channeled, effect |
| 1 | Buff | Instant | Channeled, buff |
| 2 | BasicAttack | 1,8s | Channeled, effect |
| 3 | Movement | 0,3s | Channeled, movement/buff, Travel |
| 4 | Movement | 1,4s | Channeled, effect/buff, Dash |
| 5 | Special | 0,4s | 3-hit combo, Channeled, effect |
| 6 | Movement | 0,9s | Channeled, effect, Dash |
| 7 | Movement | 0,3s | Channeled, effect/buff |
| 8 | Movement | 0,3s | Channeled, movement, Travel |
| 9 | BasicAttack | 0,4s | Channeled, effect/buff |
| 10 | Unknown | 1s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Cursed MountainBeast" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Cursed MountainBeast" 1 true "Channeled, buff"
```
```
.bb ability-slot "YourBoss" "Cursed MountainBeast" 2 true "Channeled, effect"
```

---

### Vampire BloodKnight (Level 84)
- **GUID**: 495971434
- **Can Fly**: No
- **Features**: Vampire
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 0,7s | 2-hit combo, Channeled, effect |
| 1 | Special | 0,5s | Channeled, buff, Hover |
| 2 | Special | 0,6s | Channeled, projectile/buff |
| 3 | Special | 0,1s | Channeled, buff, Hover |
| 4 | Special | 0,7s | Channeled |
| 5 | Special | 1,8s | Channeled, effect, Channeling |
| 6 | Special | 0,1s | Channeled, buff |
| 7 | Special | 1,2s | Channeled, projectile/buff |
| 8 | Special | 0,5s | Channeled, effect, TargetAOE |
| 9 | Special | 0,5s | Channeled, effect, Travel |
| 10 | Special | 0,5s | Channeled, effect, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Vampire BloodKnight" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Vampire BloodKnight" 1 true "Channeled, buff, Hover"
```
```
.bb ability-slot "YourBoss" "Vampire BloodKnight" 2 true "Channeled, projectile/buff"
```

---

### GateBossComponentsTemplate Major (Level 85)
- **GUID**: -1695577577
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 0


**Example Commands:**

---

### Gloomrot Purifier GateBoss Major (Level 85)
- **GUID**: -440174408
- **Can Fly**: No
- **Features**: Gloomrot, GateBoss, GateBoss_Major
- **Total Abilities**: 15

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | Channeled, effect |
| 1 | Defensive | 0,4s | Channeled, buff, Channeling |
| 2 | BasicAttack | 1,2s | Channeled, effect/buff |
| 3 | Special | 0,3s | Channeled |
| 4 | Defensive | 0,4s | Channeled, buff, Channeling |
| 5 | Special | 1,4s | Channeled, effect |
| 6 | Movement | 0,4s | Channeled, effect/buff, Dash |
| 7 | Movement | 0,2s | Channeled, effect, Travel |
| 8 | Movement | 1,5s | Channeled, effect |
| 9 | Movement | 0,6s | Channeled, effect |
| 10 | Unknown | 0,2s | Channeled, effect, Travel |
| 11 | BasicAttack | 0,6s | Channeled, effect |
| 12 | BasicAttack | 0,6s | Channeled, effect |
| 13 | Unknown | 0,1s | Channeled, buff/effect |
| 14 | Buff | Instant | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot Purifier GateBoss Major" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Gloomrot Purifier GateBoss Major" 1 true "Channeled, buff, Channeling"
```
```
.bb ability-slot "YourBoss" "Gloomrot Purifier GateBoss Major" 2 true "Channeled, effect/buff"
```

---

### Gloomrot Voltage GateBoss Major (Level 85)
- **GUID**: -427888732
- **Can Fly**: No
- **Features**: Gloomrot, GateBoss, GateBoss_Major
- **Total Abilities**: 11

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | 3-hit combo, Channeled, effect |
| 1 | Movement | 0,4s | Channeled, movement, Travel |
| 2 | BasicAttack | 0,8s | Channeled, effect |
| 3 | Special | 0,4s | Channeled, effect/buff |
| 4 | Movement | 0,6s | Channeled, movement, Travel |
| 5 | Special | 0,9s | Channeled |
| 6 | Movement | 2s | Channeled, effect, Channeling |
| 7 | Movement | 2s | Channeled, buff |
| 8 | Movement | 0,4s | Channeled, effect |
| 9 | Movement | 1s | Channeled |
| 10 | Unknown | 0,8s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot Voltage GateBoss Major" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Gloomrot Voltage GateBoss Major" 1 true "Channeled, movement, Travel"
```
```
.bb ability-slot "YourBoss" "Gloomrot Voltage GateBoss Major" 2 true "Channeled, effect"
```

---

### Militia Leader GateBoss Major (Level 85)
- **GUID**: 1990744594
- **Can Fly**: No
- **Features**: Militia, GateBoss, GateBoss_Major
- **Total Abilities**: 14

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,2s | Channeled, effect, Hover |
| 1 | BasicAttack | 1,2s | Channeled, projectile |
| 2 | Movement | 0,5s | Channeled, movement/buff, Travel |
| 3 | Special | 10s | Channeled |
| 4 | Special | 2s | Channeled, effect |
| 5 | Special | 1s | Channeled, effect |
| 6 | Movement | 0,7s | Channeled, buff/effect |
| 7 | BasicAttack | 1,2s | Channeled, effect, Hover |
| 8 | BasicAttack | 0,7s | Channeled, effect, Hover |
| 9 | BasicAttack | 1,2s | Channeled, projectile |
| 10 | BasicAttack | 0,7s | Channeled, projectile |
| 11 | Movement | 0,4s | Channeled, movement/buff, Travel |
| 12 | Unknown | 0,1s | Channeled, effect, Dash |
| 13 | Unknown | 0,1s | Channeled, effect, Dash |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Militia Leader GateBoss Major" 0 true "Channeled, effect, Hover"
```
```
.bb ability-slot "YourBoss" "Militia Leader GateBoss Major" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Militia Leader GateBoss Major" 2 true "Channeled, movement/buff, Travel"
```

---

### Spider Queen GateBoss Major (Level 85)
- **GUID**: -943858353
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 9

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,4s | Channeled, effect |
| 1 | BasicAttack | 1s | Channeled, effect |
| 2 | Special | 2,2s | Channeled, projectile |
| 3 | Special | 1,8s | Channeled, effect, TargetAOE |
| 4 | Special | 2s | Channeled, effect |
| 5 | Special | 0,8s | Channeled, effect, Projectile |
| 6 | Special | 1,2s | Channeled, projectile |
| 7 | Summon | 1s | Channeled, effect |
| 8 | Movement | 1s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Spider Queen GateBoss Major" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Spider Queen GateBoss Major" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Spider Queen GateBoss Major" 2 true "Channeled, projectile"
```

---

### Undead BishopOfShadows GateBoss Major (Level 85)
- **GUID**: -1805216630
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 6

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | Special | 1,2s | Channeled, projectile |
| 1 | Special | 1,2s | Channeled, projectile |
| 2 | Special | 1,6s | Channeled, effect |
| 3 | Special | 0,8s | Channeled, effect, Travel |
| 4 | Special | 1,2s | Channeled, effect |
| 5 | Special | 1,5s | Channeled, effect, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead BishopOfShadows GateBoss Major" 0 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfShadows GateBoss Major" 1 true "Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Undead BishopOfShadows GateBoss Major" 2 true "Channeled, effect"
```

---

### Undead Infiltrator GateBoss Major (Level 85)
- **GUID**: -982850914
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 8

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 3-hit combo, Channeled, effect, Channeling |
| 1 | BasicAttack | 0,7s | Channeled, effect |
| 2 | Special | 0,3s | Channeled, movement/buff, Travel |
| 3 | Special | 0,3s | Channeled, effect/buff, Dash |
| 4 | Special | 0,5s | 3-hit combo, Channeled, projectile |
| 5 | Special | 2s | Channeled, effect |
| 6 | Special | 2,5s | Channeled, buff |
| 7 | Movement | 0,3s | 3-hit combo, Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead Infiltrator GateBoss Major" 0 true "3-hit combo, Channeled, effect, Channeling"
```
```
.bb ability-slot "YourBoss" "Undead Infiltrator GateBoss Major" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead Infiltrator GateBoss Major" 2 true "Channeled, movement/buff, Travel"
```

---

### Undead ZealousCultist GateBoss Major (Level 85)
- **GUID**: -1189707552
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 6

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,9s | Channeled, effect |
| 1 | BasicAttack | 0,3s | Channeled, effect |
| 2 | Summon | 1,5s | Channeled, buff |
| 3 | Special | 0,4s | Channeled, buff, Channeling |
| 4 | Special | 0,1s | Channeled, buff |
| 5 | BasicAttack | 0,9s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Undead ZealousCultist GateBoss Major" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead ZealousCultist GateBoss Major" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Undead ZealousCultist GateBoss Major" 2 true "Channeled, buff"
```

---

### VHunter Jade GateBoss Major (Level 85)
- **GUID**: 282791819
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,75s | 12-hit combo, Channeled, projectile |
| 1 | BasicAttack | 0,75s | 2-hit combo, Channeled, projectile |
| 2 | BasicAttack | 1,6s | Channeled, projectile |
| 3 | Special | 0,5s | Channeled |
| 4 | Special | 0,5s | Channeled |
| 5 | Special | 0,8s | Channeled, projectile |
| 6 | Movement | 0,5s | Channeled, projectile |
| 7 | Movement | 0,3s | Channeled, effect, Travel |
| 8 | Movement | Instant | Channeled, buff |
| 9 | Movement | 1,1s | Channeled, buff, Channeling |
| 10 | Unknown | 0,75s | 4-hit combo, Channeled, projectile |
| 11 | Unknown | 0,75s | 6-hit combo, Channeled, projectile |

**Example Commands:**
```
.bb ability-slot "YourBoss" "VHunter Jade GateBoss Major" 0 true "12-hit combo, Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "VHunter Jade GateBoss Major" 1 true "2-hit combo, Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "VHunter Jade GateBoss Major" 2 true "Channeled, projectile"
```

---

### Villager CursedWanderer GateBoss Major (Level 85)
- **GUID**: -1160778038
- **Can Fly**: No
- **Features**: Cursed, GateBoss, GateBoss_Major
- **Total Abilities**: 3

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | Channeled, effect |
| 1 | BasicAttack | 1s | Channeled |
| 2 | BasicAttack | 2s | Channeled |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Villager CursedWanderer GateBoss Major" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Villager CursedWanderer GateBoss Major" 1 true "Channeled"
```
```
.bb ability-slot "YourBoss" "Villager CursedWanderer GateBoss Major" 2 true "Channeled"
```

---

### Wendigo GateBoss Major (Level 85)
- **GUID**: 468179469
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 12

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,7s | 3-hit combo, Channeled, effect |
| 1 | BasicAttack | 0,7s | 3-hit combo, Channeled, projectile/buff |
| 2 | Special | 2s | Channeled, AoE |
| 3 | Movement | 1,1s | Channeled |
| 4 | BasicAttack | 0,5s | Channeled, effect/buff |
| 5 | Special | 1,2s | Channeled, effect/buff, Channeling |
| 6 | Movement | 2s | Channeled, buff |
| 7 | Movement | Instant | Channeled, buff |
| 8 | Movement | 1,2s | Channeled, effect/buff, Channeling |
| 9 | Movement | 0,7s | Channeled |
| 10 | BasicAttack | 0,4s | Channeled, effect/buff |
| 11 | Movement | 0,5s | Channeled, movement, Travel |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Wendigo GateBoss Major" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Wendigo GateBoss Major" 1 true "3-hit combo, Channeled, projectile/buff"
```
```
.bb ability-slot "YourBoss" "Wendigo GateBoss Major" 2 true "Channeled, AoE"
```

---

### WerewolfChieftain GateBoss Major (Level 85)
- **GUID**: 2079933370
- **Can Fly**: No
- **Features**: Werewolf, GateBoss, GateBoss_Major
- **Total Abilities**: 6

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,8s | 2-hit combo, Channeled, effect |
| 1 | Special | 1s | Channeled, effect, Dash |
| 2 | BasicAttack | 1,2s | Channeled, effect, Dash |
| 3 | Special | 0,1s | Channeled, buff/effect |
| 4 | BasicAttack | 0,3s | 3-hit combo, Channeled, effect |
| 5 | BasicAttack | 1,2s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "WerewolfChieftain GateBoss Major" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "WerewolfChieftain GateBoss Major" 1 true "Channeled, effect, Dash"
```
```
.bb ability-slot "YourBoss" "WerewolfChieftain GateBoss Major" 2 true "Channeled, effect, Dash"
```

---

### Winter Yeti GateBoss Major (Level 85)
- **GUID**: 666177656
- **Can Fly**: No
- **Features**: GateBoss, GateBoss_Major
- **Total Abilities**: 16

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 2-hit combo, Channeled, effect |
| 1 | Movement | 0,3s | Channeled, effect, Travel |
| 2 | BasicAttack | 1,8s | Channeled |
| 3 | Special | 1s | Channeled |
| 4 | Special | 0,6s | Channeled |
| 5 | Special | 1,2s | Channeled, buff |
| 6 | Movement | 1,2s | Channeled, effect |
| 7 | Movement | 0,5s | Channeled, effect, Travel |
| 8 | Movement | 0,5s | Channeled, effect |
| 9 | BasicAttack | 1,4s | Channeled, effect, Dash |
| 10 | Unknown | Instant | Channeled, buff, Hover |
| 11 | Unknown | 0,7s | Channeled, projectile |
| 12 | Movement | 0,3s | Channeled, movement, Travel |
| 13 | Unknown | 0,5s | Channeled, effect |
| 14 | Buff | Instant | Channeled, effect |
| 15 | Unknown | 0,3s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Winter Yeti GateBoss Major" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Winter Yeti GateBoss Major" 1 true "Channeled, effect, Travel"
```
```
.bb ability-slot "YourBoss" "Winter Yeti GateBoss Major" 2 true "Channeled"
```

---

### ChurchOfLight Paladin (Level 86)
- **GUID**: -740796338
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Church
- **Total Abilities**: 13

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,4s | 2-hit combo, Channeled, effect |
| 1 | Movement | 1,6s | Channeled, effect, Dash |
| 2 | Special | Instant | Channeled, buff |
| 3 | Special | 0,5s | Channeled, movement, Travel |
| 4 | Special | 1,8s | Channeled, buff |
| 5 | Special | 1,2s | Channeled, effect/buff |
| 6 | Summon | 2,2s | Channeled, effect/AoE |
| 7 | BasicAttack | 1,3s | Channeled, Requires flight, effect |
| 8 | Movement | 0,1s | Channeled, movement, Travel |
| 9 | Movement | 1,2s | Channeled, buff, Hover |
| 10 | Unknown | 1,2s | Channeled, buff |
| 11 | Movement | 1s | Channeled, effect, Dash |
| 12 | Special | 1,2s | Channeled, effect/buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "ChurchOfLight Paladin" 0 true "2-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Paladin" 1 true "Channeled, effect, Dash"
```
```
.bb ability-slot "YourBoss" "ChurchOfLight Paladin" 2 true "Channeled, buff"
```

---

### Manticore (Level 86)
- **GUID**: -393555055
- **Can Fly**: Yes
- **Features**: UnitCategory:Beast, Beast, CanFly
- **Total Abilities**: 19

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,27s | Channeled, effect |
| 1 | Special | 1,5s | Channeled, effect, Channeling |
| 2 | BasicAttack | 1,4s | Channeled, Requires flight, effect |
| 3 | Special | 0,9s | Channeled, Requires flight, movement, Travel |
| 4 | Movement | 1,5s | Channeled, effect, Dash |
| 5 | Movement | 0,05s | Channeled, effect, Dash |
| 6 | Movement | 0,1s | Channeled, Requires flight, movement, Travel |
| 7 | Movement | 0,5s | Channeled, movement, Travel |
| 8 | Movement | 1,27s | Channeled |
| 9 | Special | 1,2s | Channeled, effect |
| 10 | BasicAttack | 0,8s | Channeled, projectile/effect |
| 11 | Unknown | 0,83s | Channeled, projectile/effect |
| 12 | Unknown | 0,83s | Channeled, projectile/effect |
| 13 | Unknown | 1,2s | Channeled, projectile/effect |
| 14 | Unknown | 2s | Channeled, projectile/effect |
| 15 | Unknown | 1,2s | Channeled, projectile/effect |
| 16 | Movement | 1,5s | Channeled, effect, Dash |
| 17 | Unknown | 4s | Channeled |
| 18 | Unknown | Instant | Channeled, buff, Hover |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Manticore" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Manticore" 1 true "Channeled, effect, Channeling"
```
```
.bb ability-slot "YourBoss" "Manticore" 2 true "Channeled, Requires flight, effect"
```

---

### Blackfang Morgana (Level 88)
- **GUID**: 591725925
- **Can Fly**: No
- **Features**: UnitCategory:Beast, Beast
- **Total Abilities**: 30

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1,9s | Channeled, effect |
| 1 | BasicAttack | 2,3s | Channeled, effect |
| 2 | Movement | 1s | Channeled, movement/buff, Hover |
| 3 | Special | 0,2s | Channeled, buff |
| 4 | Special | 0,1s | Channeled, buff, Hover |
| 5 | Summon | 0,1s | Channeled, effect |
| 6 | BasicAttack | 1,8s | Channeled, effect/buff |
| 7 | BasicAttack | 1,8s | Channeled, effect/buff |
| 8 | Movement | 0,3s | Channeled, projectile/buff |
| 9 | Movement | 1,4s | Channeled, effect, TargetAOE |
| 10 | Unknown | 0,6s | 2-hit combo, Channeled, effect/buff, TargetAOE |
| 11 | Movement | 1,6s | Channeled |
| 12 | Buff | 1s | Channeled, effect |
| 13 | BasicAttack | 1,2s | Channeled, effect/buff |
| 14 | Unknown | 0,9s | 2-hit combo, Channeled, effect/buff, TargetAOE |
| 15 | Movement | 2s | Channeled, movement/buff, Hover |
| 16 | Unknown | 0,3s | Channeled, projectile/buff |
| 17 | Unknown | 0,7s | Channeled, effect |
| 18 | Ultimate | 2s | Channeled, effect/movement |
| 19 | Unknown | 0,8s | Channeled, effect, Hover |
| 20 | Unknown | 1s | Channeled, buff |
| 21 | Unknown | 1,2s | Channeled, effect/buff, TargetAOE |
| 22 | Unknown | 1,2s | Channeled, buff, Hover |
| 23 | Movement | 0,6s | Channeled, movement, Travel |
| 24 | Unknown | 1,2s | Channeled, buff, Hover |
| 25 | Unknown | 0,8s | Channeled, effect, Hover |
| 26 | Unknown | 1,4s | Channeled, effect |
| 27 | Unknown | 2s | Channeled, effect, Hover |
| 28 | Unknown | 0,7s | Channeled, effect |
| 29 | Unknown | 0,7s | Channeled, effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Blackfang Morgana" 0 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Blackfang Morgana" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Blackfang Morgana" 2 true "Channeled, movement/buff, Hover"
```

---

### Gloomrot Monster (Level 88)
- **GUID**: 1233988687
- **Can Fly**: No
- **Features**: UnitCategory:Human, Human, Humanoid, Gloomrot
- **Total Abilities**: 21

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 0,6s | 3-hit combo, Channeled, effect |
| 1 | BasicAttack | 1,7s | Channeled, effect |
| 2 | BasicAttack | 0,6s | Channeled, effect, Travel |
| 3 | Special | 0,6s | Channeled, effect/buff, Dash |
| 4 | Special | 0,7s | Channeled |
| 5 | BasicAttack | 1s | 3-hit combo, Channeled, effect |
| 6 | Movement | 0,95s | Channeled, effect |
| 7 | Movement | 0,7s | Channeled |
| 8 | Defensive | 0,2s | Channeled, buff, Channeling |
| 9 | Movement | 0,5s | Channeled, effect/AoE, Travel |
| 10 | Unknown | 0,5s | Channeled, effect, Travel |
| 11 | Summon | 0,7s | Channeled, effect |
| 12 | Special | 0,8s | Channeled, projectile |
| 13 | Unknown | 2s | Channeled, buff |
| 14 | Unknown | Instant | Channeled, projectile |
| 15 | Unknown | Instant | Channeled, effect, Travel |
| 16 | Unknown | Instant | Channeled, effect, Travel |
| 17 | Unknown | 1,8s | Channeled, effect/AoE |
| 18 | Unknown | 0,6s | Channeled, effect, Hover |
| 19 | Unknown | Instant | Channeled, effect, Travel |
| 20 | Unknown | 1,4s | Channeled, buff |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Gloomrot Monster" 0 true "3-hit combo, Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Gloomrot Monster" 1 true "Channeled, effect"
```
```
.bb ability-slot "YourBoss" "Gloomrot Monster" 2 true "Channeled, effect, Travel"
```

---

### Vampire Dracula (Level 91)
- **GUID**: -327335305
- **Can Fly**: No
- **Features**: Vampire
- **Total Abilities**: 37

#### Available Ability Slots:
| Slot | Category | Cast Time | Description |
|------|----------|-----------|-------------|
| 0 | BasicAttack | 1s | 2-hit combo, Channeled, projectile |
| 1 | BasicAttack | 1,3s | Channeled, Requires flight, effect |
| 2 | BasicAttack | 1,2s | Channeled, buff, Hover |
| 3 | Special | 0,5s | Channeled, effect |
| 4 | Movement | 0,3s | Channeled, movement/buff, Travel |
| 5 | Special | 1,2s | Channeled, buff/effect |
| 6 | Movement | 0,8s | Channeled, effect |
| 7 | Movement | 0,4s | Channeled, effect/buff, Dash |
| 8 | Movement | 0,4s | Channeled, effect/buff, Dash |
| 9 | Movement | 0,5s | Channeled, movement, Hover |
| 10 | Special | 3s | Channeled, buff |
| 11 | Unknown | 0,4s | Channeled, effect |
| 12 | Unknown | 0,2s | Channeled, effect, Dash |
| 13 | Unknown | 0,2s | Channeled, effect, Dash |
| 14 | Special | 2s | Channeled, AoE/buff/effect |
| 15 | Unknown | 1s | Channeled, effect, Hover |
| 16 | Unknown | 2s | Channeled, effect |
| 17 | Movement | 0,3s | Channeled, effect, Travel |
| 18 | Unknown | 1,4s | 2-hit combo, Channeled, effect |
| 19 | Movement | 0,7s | Channeled, effect |
| 20 | Movement | 0,3s | Channeled, effect |
| 21 | Special | 0,1s | Channeled, projectile |
| 22 | BasicAttack | 1,8s | Channeled, Requires flight, AoE |
| 23 | Unknown | 0,2s | Channeled, effect, Dash |
| 24 | Unknown | 0,2s | Channeled, effect, Dash |
| 25 | Movement | 0,3s | Channeled, effect, Travel |
| 26 | Summon | 1,5s | Channeled, effect |
| 27 | Special | 0,1s | Channeled, effect, Hover |
| 28 | Special | 1,5s | Channeled, effect, Hover |
| 29 | Unknown | 0,7s | Channeled, effect |
| 30 | BasicAttack | 1s | 4-hit combo, Channeled, projectile |
| 31 | Ultimate | 1,8s | Channeled, effect |
| 32 | Ultimate | 0,4s | Channeled, buff/effect, Hover |
| 33 | Unknown | 0,7s | Channeled, buff, Hover |
| 34 | Movement | 0,1s | Channeled, movement, Travel |
| 35 | Unknown | 0,7s | Channeled, effect |
| 36 | Movement | 0,6s | Channeled, buff/effect |

**Example Commands:**
```
.bb ability-slot "YourBoss" "Vampire Dracula" 0 true "2-hit combo, Channeled, projectile"
```
```
.bb ability-slot "YourBoss" "Vampire Dracula" 1 true "Channeled, Requires flight, effect"
```
```
.bb ability-slot "YourBoss" "Vampire Dracula" 2 true "Channeled, buff, Hover"
```

---

## Compatibility Notes

- **Beast** abilities work best on beast-type bosses
- **Humanoid** abilities work best on humanoid/vampire bosses
- **Flight** abilities require bosses that can fly
- **Transformation** abilities may have visual issues on incompatible models

## Tips

1. Use `.bb ability-suggest <BossName>` to get compatible ability suggestions
2. Use `.bb ability-test <BossName> <VBloodName> <Slot>` to test compatibility before applying
3. Some abilities may have warnings but still work with minor visual issues
4. Avoid mixing abilities from very different creature types (e.g., spider abilities on humanoids)
