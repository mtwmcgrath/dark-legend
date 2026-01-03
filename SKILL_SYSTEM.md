# 🗡️ Dark Legend - Comprehensive Skill System Documentation

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Core Components](#core-components)
- [Skill Types](#skill-types)
- [Skill Effects](#skill-effects)
- [Skill Tree System](#skill-tree-system)
- [Combo System](#combo-system)
- [UI System](#ui-system)
- [Integration Guide](#integration-guide)
- [Skill Formulas](#skill-formulas)
- [Creating Custom Skills](#creating-custom-skills)
- [Example Skill Data](#example-skill-data)

---

## 🎯 Overview

The Dark Legend Skill System is a comprehensive MU Online-inspired skill framework featuring:

- **20+ Skills** per character class (Dark Knight, Dark Wizard, Elf)
- **10 Skill Types**: Active, Passive, Buff, Debuff, AoE, Projectile, Melee, Heal, Summon, Ultimate
- **10 Effect Types**: Damage, Heal, Buff, Debuff, Stun, Slow, Poison, Burn, Knockback
- **Skill Tree System**: Tiered progression with prerequisites
- **Combo System**: Skill chains with damage multipliers
- **Complete UI**: Skill bars (1-9, F1-F12), tooltips, and upgrade interface

All code uses the `DarkLegend.Skills` namespace and includes bilingual comments (Vietnamese/English).

---

## 🏗️ Architecture

### Directory Structure

```
Assets/Scripts/Skills/
├── Core/                    # Core skill system
│   ├── SkillBase.cs
│   ├── SkillData.cs
│   ├── SkillManager.cs
│   ├── SkillCooldown.cs
│   ├── SkillCost.cs
│   └── SkillRequirement.cs
│
├── Types/                   # Skill implementations
│   ├── ActiveSkill.cs
│   ├── PassiveSkill.cs
│   ├── BuffSkill.cs
│   ├── DebuffSkill.cs
│   ├── AoESkill.cs
│   ├── ProjectileSkill.cs
│   ├── MeleeSkill.cs
│   ├── HealSkill.cs
│   ├── SummonSkill.cs
│   └── UltimateSkill.cs
│
├── Effects/                 # Skill effect components
│   ├── SkillEffect.cs
│   ├── DamageEffect.cs
│   ├── HealEffect.cs
│   ├── BuffEffect.cs
│   ├── DebuffEffect.cs
│   ├── StunEffect.cs
│   ├── SlowEffect.cs
│   ├── PoisonEffect.cs
│   ├── BurnEffect.cs
│   └── KnockbackEffect.cs
│
├── SkillTree/              # Skill tree system
│   ├── SkillTree.cs
│   ├── SkillNode.cs
│   ├── SkillUnlock.cs
│   └── SkillTreeUI.cs
│
├── Combo/                  # Combo system
│   ├── ComboSystem.cs
│   ├── ComboData.cs
│   └── ComboUI.cs
│
└── UI/                     # User interface
    ├── SkillBarUI.cs
    ├── SkillSlotUI.cs
    ├── SkillTooltipUI.cs
    └── SkillLevelUpUI.cs
```

---

## 🎮 Core Components

### SkillBase

Abstract base class for all skills.

**Key Features:**
- Skill initialization and management
- Cost checking and consumption
- Cooldown tracking
- Level progression (1-20)
- Requirement validation

**Usage:**
```csharp
public abstract class SkillBase : MonoBehaviour
{
    public virtual void Initialize(GameObject owner, SkillManager manager);
    public virtual bool CanUse();
    public virtual bool Use(Vector3 targetPosition, GameObject targetObject = null);
    public virtual bool LevelUp();
}
```

### SkillData (ScriptableObject)

Configuration data for skills.

**Key Properties:**
- Basic info (name, description, icon)
- Type classification (Active, Passive, Buff, etc.)
- Damage and stat ratios (STR, AGI, VIT, ENE)
- Range and area settings
- Visual and audio effects
- References to cooldown, cost, and requirements

**Create via:** `Assets > Create > Dark Legend > Skills > Skill Data`

### SkillManager

Manages all skills for a character.

**Responsibilities:**
- Learning and forgetting skills
- Skill bar management (main + secondary)
- Skill point allocation
- Skill upgrading

**API:**
```csharp
public bool LearnSkill(SkillData skillData);
public bool UseSkill(string skillName, Vector3 targetPosition, GameObject targetObject = null);
public bool LevelUpSkill(string skillName);
public void AddSkillPoints(int amount);
```

### SkillCooldown (ScriptableObject)

Manages skill cooldowns.

**Formula:**
```csharp
cooldown = baseCooldown - (cooldownReductionPerLevel * (level - 1))
cooldown = Max(cooldown, minCooldown)
```

### SkillCost (ScriptableObject)

Manages MP/HP costs.

**Formula:**
```csharp
mpCost = baseMPCost + (mpCostPerLevel * (level - 1))
mpCost = Min(mpCost, maxMPCost)
```

### SkillRequirement (ScriptableObject)

Defines skill prerequisites.

**Requirements:**
- Minimum level
- Stat requirements (STR, AGI, VIT, ENE)
- Class restrictions
- Prerequisite skills
- Weapon requirements (optional)

---

## 💥 Skill Types

### 1. ActiveSkill

Skills that require manual activation with cast time support.

**Features:**
- Cast time handling
- Cast cancellation
- Animation integration
- Target validation

### 2. PassiveSkill

Auto-active skills providing constant bonuses.

**Features:**
- Automatic activation on learn
- Stat bonuses (damage, defense, crit rate)
- HP/MP regeneration
- Scales with skill level

### 3. BuffSkill

Enhances self or allies.

**Buff Types:**
- Attack Power
- Defense
- Attack Speed
- Movement Speed
- Crit Rate
- Max HP/MP
- HP/MP Regen

### 4. DebuffSkill

Weakens enemies.

**Debuff Types:**
- Attack/Defense reduction
- Speed reduction
- Stun
- Silence
- Blind
- Poison
- Burn

### 5. AoESkill

Area of effect attacks.

**Features:**
- Sphere collision detection
- Max target limiting
- Damage falloff (optional)
- AoE indicator

### 6. ProjectileSkill

Projectile-based attacks.

**Features:**
- Projectile physics
- Pierce capability
- Homing behavior
- Chain/bounce effects

### 7. MeleeSkill

Close-range attacks.

**Features:**
- Cone-based hit detection
- Chain attacks
- Knockback
- Animation integration

### 8. HealSkill

Healing abilities.

**Features:**
- HP/MP restoration
- AoE healing
- Heal over Time (HoT)
- Percentage or flat healing

### 9. SummonSkill

Summons creatures/pets.

**Features:**
- Summon management
- AI behaviors (Follow, Attack, Patrol)
- Summon stats scaling
- Duration control

### 10. UltimateSkill

Powerful abilities requiring gauge.

**Features:**
- Ultimate gauge requirement
- Transformation effects
- Stat multipliers
- Spectacular visuals

---

## ✨ Skill Effects

### Base: SkillEffect

All effects inherit from `SkillEffect` with:
- Duration management
- Stack support
- Visual effects
- Apply/Remove logic

### Effect Types

1. **DamageEffect**: Instant or DoT damage
2. **HealEffect**: HP/MP restoration
3. **BuffEffect**: Temporary stat increases
4. **DebuffEffect**: Temporary stat decreases
5. **StunEffect**: Prevents actions
6. **SlowEffect**: Reduces movement/attack speed
7. **PoisonEffect**: Poison DoT
8. **BurnEffect**: Fire DoT + defense reduction
9. **KnockbackEffect**: Pushes targets

---

## 🌳 Skill Tree System

### SkillTree (ScriptableObject)

Defines class skill progression.

**Structure:**
- 4 Tiers (Level gates: 1, 50, 150, 250)
- 5-7 nodes per tier
- Visual layout data

**Create via:** `Assets > Create > Dark Legend > Skills > Skill Tree`

### SkillNode

Individual skill in tree.

**Properties:**
- Position in tree (tier, index)
- Prerequisites
- Unlock state
- Connection visualization

### SkillTreeUI

Interactive UI for learning skills.

**Features:**
- Visual tree layout
- Node state indicators (locked/available/learned)
- Skill details panel
- Learn/upgrade buttons
- Connection lines

---

## 🔗 Combo System

### ComboSystem

Tracks and rewards skill chains.

**Features:**
- Combo timer (default 2s)
- Combo counter (max 10)
- Damage multiplier (+10% per hit)
- Combo finishers

### ComboData (ScriptableObject)

Defines combo sequences.

**Types:**
- Exact order required
- Any order
- Allow intermediate skills

**Rewards:**
- Damage multiplier
- Crit rate bonus
- Special finisher skill

**Create via:** `Assets > Create > Dark Legend > Skills > Combo Data`

### ComboUI

Displays combo status.

**Elements:**
- Combo counter
- Timer bar
- Color coding by hits
- Combo list with progress

---

## 🖥️ UI System

### SkillBarUI

Main skill interface.

**Features:**
- Main bar: 1-9, 0, -, = keys
- Secondary bar: F1-F12 keys
- Tab to toggle bars
- Drag & drop support

### SkillSlotUI

Individual skill slots.

**Features:**
- Skill icon display
- Cooldown overlay
- Level indicator
- Key binding label
- Hover tooltips

### SkillTooltipUI

Detailed skill information.

**Content:**
- Skill name and icon
- Level and description
- Damage/healing values
- Cost and cooldown
- Range and AoE
- Requirements

### SkillLevelUpUI

Skill upgrade interface.

**Features:**
- Current vs next level stats
- Skill point cost
- Stat comparison
- Level up animation

---

## 🔧 Integration Guide

### Step 1: Setup Character

```csharp
// Add to your player GameObject
SkillManager skillManager = gameObject.AddComponent<SkillManager>();
CharacterStats stats = gameObject.AddComponent<CharacterStats>();
CharacterClass charClass = gameObject.AddComponent<CharacterClass>();
ComboSystem comboSystem = gameObject.AddComponent<ComboSystem>();
```

### Step 2: Create Skill Data

1. Right-click in Project: `Create > Dark Legend > Skills > Skill Data`
2. Configure properties
3. Create Cooldown, Cost, and Requirement ScriptableObjects
4. Assign to SkillData

### Step 3: Setup Skill Tree

1. Create SkillTree: `Create > Dark Legend > Skills > Skill Tree`
2. Define nodes with positions and prerequisites
3. Assign to SkillTreeUI

### Step 4: Setup UI

1. Add SkillBarUI to Canvas
2. Assign prefabs and references
3. Add SkillTooltipUI
4. Add ComboUI

### Step 5: Learn Skills

```csharp
// In game code
skillManager.LearnSkill(skillData);
skillManager.AssignToMainBar(skillName, slotIndex);
```

---

## 📐 Skill Formulas

### Damage Calculation

```csharp
baseDamage = skillData.BaseDamage + (skillData.DamagePerLevel * (level - 1))
statBonus = (STR * strRatio) + (AGI * agiRatio) + (ENE * eneRatio)
totalDamage = baseDamage + statBonus + attackPower

// Critical hit
if (Random.value < critRate && canCrit)
    totalDamage *= 2

// Defense
if (!pierceArmor)
    damageReduction = defense / (defense + 100)
    totalDamage *= (1 - damageReduction)

// Combo
totalDamage *= comboMultiplier

return Max(1, totalDamage)
```

### Combo Multiplier

```csharp
multiplier = 1 + ((comboCount - 1) * 0.1f)  // +10% per hit
if (activeCombo)
    multiplier *= activeCombo.damageMultiplier
```

### Ultimate Gauge

```csharp
// Gain gauge on:
OnHit: +5 per hit
OnKill: +20 per kill
OnDamageReceived: +(damage / 10) * 2

// Max gauge: 100
```

---

## 🎨 Creating Custom Skills

### Example: Fire Slash (Melee)

```csharp
// Create SkillData
Name: "Fire Slash"
Type: Active
Element: Fire
TargetType: SingleEnemy
BaseDamage: 150
DamagePerLevel: 15
StrRatio: 1.2
CastRange: 3
CastTime: 0.5

// Create SkillCooldown
BaseCooldown: 5
CooldownReductionPerLevel: 0.1
MinCooldown: 2

// Create SkillCost
BaseMPCost: 20
MPCostPerLevel: 3
MaxMPCost: 80

// Create SkillRequirement
RequiredLevel: 50
RequiredSTR: 100
AllowedClasses: [DarkKnight]

// The system will automatically create a MeleeSkill component
// when this skill is learned
```

---

## 📝 Example Skill Data

### Dark Knight: Slash (Tier 1)

```
Skill Name: Slash
Level Unlock: 1
Max Level: 20

Stats:
- Base Damage: 100
- Damage/Level: 10
- STR Ratio: 1.0
- Cast Range: 2m
- Type: Melee

Cost:
- MP Cost: 10 + 2/level
- Cooldown: 3s - 0.05/level (min 1s)

Requirements:
- None (starter skill)
```

### Dark Wizard: Fireball (Tier 1)

```
Skill Name: Fireball
Level Unlock: 1
Max Level: 20

Stats:
- Base Damage: 120
- Damage/Level: 12
- ENE Ratio: 1.5
- Cast Range: 15m
- Type: Projectile

Cost:
- MP Cost: 15 + 3/level
- Cooldown: 4s - 0.08/level (min 1.5s)

Requirements:
- Class: Dark Wizard
```

### Elf: Heal (Tier 1)

```
Skill Name: Heal
Level Unlock: 1
Max Level: 20

Stats:
- Heal Amount: 100
- Heal/Level: 15
- ENE Ratio: 0.8
- Cast Range: 10m
- Target: Self/Ally

Cost:
- MP Cost: 20 + 4/level
- Cooldown: 8s - 0.15/level (min 3s)

Requirements:
- Class: Elf
```

---

## 🎯 Best Practices

1. **Balance Testing**: Adjust damage ratios and costs based on gameplay
2. **Visual Feedback**: Always add effects for skill casts and impacts
3. **Sound Design**: Include audio for better player feedback
4. **Tooltips**: Provide clear descriptions with numbers
5. **Prerequisites**: Design logical skill tree progressions
6. **Combos**: Create intuitive skill chains for each class

---

## 🐛 Troubleshooting

### Skills Not Appearing in Bar
- Check SkillManager is attached to player
- Verify skill is learned: `skillManager.HasSkill(skillName)`
- Check skill assignment: `skillManager.mainSkillBar[index]`

### Skills Not Dealing Damage
- Verify target has CharacterStats component
- Check tags: "Enemy" or "Monster"
- Enable Debug.Log in skill execution

### Cooldowns Not Working
- Ensure SkillCooldown ScriptableObject is assigned
- Check Time.deltaTime in Update loop
- Verify cooldown values are reasonable

### UI Not Updating
- Check references in SkillBarUI inspector
- Verify Canvas is set up correctly
- Check slot prefab has SkillSlotUI component

---

## 📞 Support

For questions or issues with the skill system:
1. Check this documentation
2. Review example skills in the project
3. Examine Debug.Log output during gameplay
4. Check Unity Console for errors

---

## 📄 License

MIT License - See project LICENSE file

---

**Created for Dark Legend - A MU Online Inspired 3D RPG**

**Version:** 1.0  
**Last Updated:** 2026-01-03
