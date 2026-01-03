# 🚀 Dark Legend Skill System - Quick Start Guide

## ⚡ 5-Minute Setup

### 1. Add Components to Player

```csharp
// Required components for skill system
GameObject player = GameObject.FindGameObjectWithTag("Player");

// Core components
player.AddComponent<SkillManager>();
player.AddComponent<CharacterStats>();
player.AddComponent<CharacterClass>();
player.AddComponent<CharacterMovement>();
player.AddComponent<ComboSystem>();
player.AddComponent<UltimateGaugeManager>();
```

### 2. Create Your First Skill

**Via Unity Editor:**
1. Right-click in Project window
2. `Create > Dark Legend > Skills > Skill Data`
3. Name it "BasicAttack"
4. Configure:
   - Skill Name: "Basic Attack"
   - Skill Type: Active
   - Base Damage: 50
   - Cast Range: 3

**Create Supporting Data:**
```
Create > Dark Legend > Skills > Cooldown
- Base Cooldown: 1.5s
- Min Cooldown: 0.5s

Create > Dark Legend > Skills > Cost
- Base MP Cost: 10
- MP Cost Per Level: 2

Create > Dark Legend > Skills > Requirement
- Required Level: 1
```

**Assign to Skill Data:**
- Drag & drop Cooldown, Cost, and Requirement into SkillData

### 3. Learn the Skill

```csharp
// In your game initialization code
void Start()
{
    SkillManager skillManager = GetComponent<SkillManager>();
    
    // Load your skill data (assign in inspector or load from Resources)
    SkillData basicAttack = Resources.Load<SkillData>("Skills/BasicAttack");
    
    // Learn the skill
    skillManager.LearnSkill(basicAttack);
    
    // Assign to skill bar slot 1
    skillManager.AssignToMainBar("Basic Attack", 0);
    
    // Give initial skill points
    skillManager.AddSkillPoints(10);
}
```

### 4. Setup UI (Optional but Recommended)

**Create Canvas:**
1. Right-click in Hierarchy: `UI > Canvas`
2. Add SkillBarUI component to Canvas
3. Create skill slot prefab with Image and Text components
4. Assign prefab to SkillBarUI

**Minimal Slot Prefab Structure:**
```
SkillSlot (GameObject)
├── SkillSlotUI (Component)
├── Icon (Image)
├── Cooldown (Image - filled)
├── KeyText (Text)
└── LevelText (Text)
```

---

## 🎯 Testing Your Setup

### Test Script

```csharp
using UnityEngine;
using DarkLegend.Skills;

public class SkillTester : MonoBehaviour
{
    public SkillData testSkill;
    private SkillManager skillManager;
    
    void Start()
    {
        // Setup
        skillManager = gameObject.AddComponent<SkillManager>();
        gameObject.AddComponent<CharacterStats>();
        
        // Learn skill
        if (testSkill != null)
        {
            skillManager.LearnSkill(testSkill);
            skillManager.AssignToMainBar(testSkill.skillName, 0);
        }
    }
    
    void Update()
    {
        // Test with number key 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SkillBase skill = skillManager.mainSkillBar[0];
            if (skill != null)
            {
                Vector3 target = transform.position + transform.forward * 5f;
                bool success = skill.Use(target);
                Debug.Log($"Skill used: {success}");
            }
        }
    }
}
```

---

## 📋 Common Patterns

### Pattern 1: Fireball Projectile

```csharp
// SkillData configuration:
Skill Type: Active
Element: Fire
Target Type: SingleEnemy
Base Damage: 100
ENE Ratio: 1.5
Cast Range: 15
Cast Time: 0.5
Projectile Prefab: [Assign sphere with trail renderer]
Impact Effect: [Assign particle system]
```

### Pattern 2: AoE Explosion

```csharp
// SkillData configuration:
Skill Type: Active
Target Type: Area
Base Damage: 150
Cast Range: 10
AoE Radius: 5
Max Targets: 10
Impact Effect: [Assign explosion particles]
```

### Pattern 3: Self Buff

```csharp
// SkillData configuration:
Skill Type: Buff
Target Type: Self
Duration: 30
Buff Value: 50 (attack power increase)
```

### Pattern 4: Passive Bonus

```csharp
// SkillData configuration:
Skill Type: Passive
// In PassiveSkill component settings:
damageBonus: 20
critRateBonus: 0.05
```

---

## 🎨 Visual Setup

### Particle Effects

**Cast Effect:**
- Create particle system prefab
- Attach to character position
- Duration: 0.5-1s

**Projectile:**
- Sphere mesh + Trail Renderer
- Add Rigidbody (kinematic)
- Add Sphere Collider (trigger)

**Impact Effect:**
- Particle system with burst emission
- Scale based on skill power
- Auto-destroy after 2s

### Animation Integration

```csharp
// Animator triggers used by skill system:
animator.SetTrigger("Cast");     // For casting
animator.SetTrigger("Attack");   // For melee
animator.SetTrigger("Ultimate"); // For ultimates
animator.SetBool("Stunned", true); // For stun effect
```

---

## ⚙️ Configuration Examples

### Dark Knight Starter Skills

```csharp
// Slash - Basic melee
Base Damage: 100
STR Ratio: 1.0
Cast Range: 2m
Cooldown: 3s
MP Cost: 10

// Twisting Slash - AoE spin
Base Damage: 150
STR Ratio: 1.2
AoE Radius: 3m
Cooldown: 8s
MP Cost: 25
Required Level: 10

// Death Stab - Strong single target
Base Damage: 200
STR Ratio: 1.5
Pierce Armor: true
Cooldown: 12s
MP Cost: 35
Required Level: 20
```

### Dark Wizard Starter Skills

```csharp
// Fireball - Basic projectile
Base Damage: 120
ENE Ratio: 1.5
Cast Range: 15m
Projectile Speed: 20
Cooldown: 4s
MP Cost: 15

// Lightning - Chain attack
Base Damage: 150
ENE Ratio: 1.3
Max Chain: 3
Cooldown: 10s
MP Cost: 30
Required Level: 15

// Ice Storm - AoE slow
Base Damage: 200
ENE Ratio: 1.4
AoE Radius: 6m
Slow: 50%
Duration: 5s
Cooldown: 15s
MP Cost: 50
Required Level: 25
```

### Elf Starter Skills

```csharp
// Triple Shot - Multi-projectile
Damage Per Arrow: 90
AGI Ratio: 1.0
Arrow Count: 3
Cast Range: 12m
Cooldown: 5s
MP Cost: 20

// Heal - Self/ally heal
Heal Amount: 100
ENE Ratio: 0.8
Cast Range: 10m
Target: Ally
Cooldown: 8s
MP Cost: 25

// Poison Arrow - DoT
Base Damage: 100
AGI Ratio: 0.8
Poison Damage: 20/tick
Duration: 10s
Cooldown: 12s
MP Cost: 30
Required Level: 15
```

---

## 🔍 Debugging Tips

### Enable Debug Logs

```csharp
// In SkillBase.cs, uncomment Debug.Log statements:
Debug.Log($"Skill used: {skillData.skillName}");
Debug.Log($"Can use: {CanUse()}, On cooldown: {isOnCooldown}");
Debug.Log($"Current MP: {stats.currentMP}, Required: {mpCost}");
```

### Visual Debugging

```csharp
// Show skill ranges in Scene view
void OnDrawGizmosSelected()
{
    if (skillData != null)
    {
        // Cast range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, skillData.castRange);
        
        // AoE radius
        if (skillData.aoeRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, skillData.aoeRadius);
        }
    }
}
```

---

## 🎓 Advanced Topics

### Custom Skill Type

```csharp
using DarkLegend.Skills;

public class CustomTeleportSkill : ActiveSkill
{
    public float teleportDistance = 10f;
    
    protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
    {
        base.ExecuteSkill(targetPosition, targetObject);
        
        // Teleport logic
        Vector3 direction = (targetPosition - owner.transform.position).normalized;
        Vector3 newPosition = owner.transform.position + direction * teleportDistance;
        
        owner.transform.position = newPosition;
        
        // Spawn effects
        if (skillData.impactEffect != null)
        {
            Instantiate(skillData.impactEffect, newPosition, Quaternion.identity);
        }
    }
}
```

### Custom Combo

```csharp
// Create ComboData: Create > Dark Legend > Skills > Combo Data
Combo Name: "Fire Combo"
Skill Sequence: ["Fireball", "Meteor", "Hellfire"]
Requires Exact Order: true
Damage Multiplier: 2.5
Finisher Skill: "Inferno"
```

### Skill Tree Setup

```csharp
// Create SkillTree: Create > Dark Legend > Skills > Skill Tree
Tree Name: "Dark Knight Skills"
Character Class: DarkKnight
Max Tiers: 4

// Add nodes:
Tier 1: Slash, Defense Boost
Tier 2: Twisting Slash, Death Stab (requires Slash level 5)
Tier 3: Cyclone (requires Twisting Slash level 10)
Tier 4: Ultimate - Berserk Mode
```

---

## 📦 Project Organization

### Recommended Folder Structure

```
Assets/
├── Prefabs/
│   ├── Skills/
│   │   ├── Effects/
│   │   ├── Projectiles/
│   │   └── UI/
│   └── Characters/
│
├── Resources/
│   └── Skills/
│       ├── DarkKnight/
│       ├── DarkWizard/
│       └── Elf/
│
├── ScriptableObjects/
│   └── Skills/
│       ├── SkillData/
│       ├── SkillTrees/
│       ├── Combos/
│       └── Requirements/
│
└── Scripts/
    └── Skills/
        └── [Skill System Code]
```

---

## ✅ Checklist

- [ ] Core components added to player
- [ ] CharacterStats configured (HP, MP, stats)
- [ ] At least one skill created and tested
- [ ] Skill bar UI setup
- [ ] Key bindings tested (1-9, F1-F12)
- [ ] Visual effects assigned
- [ ] Audio clips assigned
- [ ] Tooltips showing correctly
- [ ] Cooldowns working
- [ ] MP costs deducting
- [ ] Damage calculation verified

---

## 🎉 Next Steps

1. **Create More Skills**: Build out your class skill sets
2. **Balance Testing**: Adjust damage, costs, and cooldowns
3. **Visual Polish**: Add better effects and animations
4. **Skill Trees**: Design progression paths
5. **Combos**: Create skill chain sequences
6. **AI Integration**: Make enemies use skills
7. **Multiplayer**: Add network synchronization (if needed)

---

## 📚 Additional Resources

- See `SKILL_SYSTEM.md` for complete documentation
- Check example skills in `Assets/Resources/Skills/`
- Review skill type implementations in `Assets/Scripts/Skills/Types/`
- Study effect system in `Assets/Scripts/Skills/Effects/`

---

**Happy Skill Crafting! 🗡️✨**
