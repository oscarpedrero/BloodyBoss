# Boss Mechanics Testing Guide

## Quick Test Commands for All 25 Mechanics

Copy and paste these commands to test each mechanic:

### Basic Combat Mechanics (1-5)
```bash
.bb mechanic-add TestBoss enrage --hp 50 --damage_multiplier 2
.bb mechanic-add TestBoss shield --hp 30 --shield_type immune --duration 5
.bb mechanic-add TestBoss summon --hp 70 --count 3
.bb mechanic-add TestBoss heal --hp 40 --heal_amount 25
.bb mechanic-add TestBoss teleport --time 20 --teleport_type random
```

### Control Mechanics (6-10)
```bash
.bb mechanic-add TestBoss phase --hp 50 --phase_number 2
.bb mechanic-add TestBoss aoe --hp 40 --aoe_type explosion --damage 500 --radius 15
.bb mechanic-add TestBoss stun --hp 60 --duration 3 --radius 20
.bb mechanic-add TestBoss fear --hp 35 --duration 4 --radius 25
.bb mechanic-add TestBoss slow --time 45 --slow_amount 50 --duration 6
```

### Force Mechanics (11-15)
```bash
.bb mechanic-add TestBoss pull --hp 45 --force 50 --radius 30
.bb mechanic-add TestBoss clone --hp 55 --count 2 --health_percent 30
.bb mechanic-add TestBoss root --hp 70 --duration 4 --radius 15
.bb mechanic-add TestBoss silence --hp 65 --duration 5 --radius 20
.bb mechanic-add TestBoss knockback --hp 80 --force 30 --damage 200
```

### Advanced Mechanics (16-20)
```bash
.bb mechanic-add TestBoss dot --hp 35 --damage_per_tick 50 --duration 10 --dot_type poison
.bb mechanic-add TestBoss buff_steal --hp 40 --steal_count 3 --target strongest
.bb mechanic-add TestBoss vision --hp 55 --vision_type darkness --intensity 80
.bb mechanic-add TestBoss mind_control --hp 20 --target random --duration 5
.bb mechanic-add TestBoss reflect --hp 45 --reflect_type all --reflect_percent 100
```

### Ultimate Mechanics (21-25)
```bash
.bb mechanic-add TestBoss absorb --hp 30 --absorb_type health --amount 100
.bb mechanic-add TestBoss dispel --hp 60 --dispel_type all --max_dispels 5
.bb mechanic-add TestBoss weaken --hp 70 --weaken_type all --amount 30
.bb mechanic-add TestBoss curse --hp 25 --curse_type doom --duration 15
.bb mechanic-add TestBoss trap --hp 50 --trap_type spike --count 10 --pattern circle
```

## Create Test Boss
```bash
.bb create TestBoss -1905691330 80 2 1800
```

## List All Mechanics
```bash
.bb mechanic-list TestBoss
```

## Clear All Mechanics
```bash
.bb mechanic-clear TestBoss
```