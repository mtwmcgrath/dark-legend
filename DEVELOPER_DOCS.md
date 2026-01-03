# Character Classes System - Developer Documentation

## Overview
This document provides technical details about the Character Classes System implementation for Dark Legend.

## Architecture

### Namespace
All character-related code is organized under the `DarkLegend.Character` namespace.

### Core Components

#### 1. Class System (`Assets/Scripts/Character/Classes/`)
- **CharacterClass.cs**: Abstract base class for all character classes
- **ClassData.cs**: ScriptableObject for storing class configurations
- **ClassManager.cs**: Singleton manager for class selection and management

#### 2. Stats System (`Assets/Scripts/Character/Stats/`)
- **CharacterStats.cs**: Main stats container with base and derived stats
- **StatModifier.cs**: Represents stat modifiers (buffs/debuffs)
- **StatCalculator.cs**: Static utility class for stat calculations
- **StatsUI.cs**: UI component for displaying character stats

#### 3. Evolution System (`Assets/Scripts/Character/Evolution/`)
- **EvolutionSystem.cs**: Manages class evolution logic and requirements
- **EvolutionQuest.cs**: Quest system for evolution requirements
- **EvolutionUI.cs**: UI component for evolution interface

#### 4. Character Creation (`Assets/Scripts/Character/Creation/`)
- **CharacterCreation.cs**: Main character creation logic
- **CharacterSlot.cs**: Manages 5 character slots per account
- **CharacterCreationUI.cs**: Multi-step character creation interface

#### 5. Appearance System (`Assets/Scripts/Character/Appearance/`)
- **CharacterAppearance.cs**: Character customization (face, hair, skin)
- **ArmorVisual.cs**: Armor visual display system
- **WeaponVisual.cs**: Weapon visual display system

## Character Classes

### Base Classes (Available from Start)
1. **Dark Knight** - Melee DPS/Tank
   - Base Stats: STR 28, AGI 20, VIT 25, ENE 10
   - Growth: STR +5, AGI +2, VIT +3, ENE +1
   - Weapons: Swords, Blades, Two-Handed Swords
   - Armor: Heavy
   
2. **Dark Wizard** - Magic DPS/AoE
   - Base Stats: STR 18, AGI 18, VIT 15, ENE 30
   - Growth: STR +1, AGI +2, VIT +1, ENE +6
   - Weapons: Staffs, Wands
   - Armor: Robe
   
3. **Fairy Elf** - Ranged DPS/Support/Healer
   - Base Stats: STR 22, AGI 25, VIT 15, ENE 20
   - Growth: STR +1, AGI +4, VIT +1, ENE +3
   - Weapons: Bows, Crossbows
   - Armor: Light

### Advanced Classes (Unlockable)
4. **Magic Gladiator** (Unlock: Lv220)
   - Base Stats: STR 26, AGI 26, VIT 26, ENE 16
   - Growth: STR +5, AGI +3, VIT +3, ENE +2
   - Weapons: Swords, Blades, Two-Handed Swords
   - Armor: Medium
   
5. **Dark Lord** (Unlock: Lv250)
   - Base Stats: STR 26, AGI 20, VIT 20, ENE 15, CMD 25
   - Growth: STR +4, AGI +2, VIT +2, ENE +2, CMD +4
   - Weapons: Scepters, Shields
   - Armor: Heavy
   - Special: Command stat for pet/summon control
   
6. **Summoner** (Unlock: Lv220)
   - Base Stats: STR 21, AGI 21, VIT 18, ENE 23
   - Growth: STR +2, AGI +2, VIT +1, ENE +4
   - Weapons: Books, Sticks
   - Armor: Robe
   
7. **Rage Fighter** (Unlock: Lv220)
   - Base Stats: STR 32, AGI 27, VIT 25, ENE 20
   - Growth: STR +6, AGI +3, VIT +4, ENE +2
   - Weapons: Fists, Claws
   - Armor: Light

## Evolution Paths

### 3-Tier Evolution (Base Classes)
- Dark Knight → Blade Knight (Lv150) → Blade Master (Lv400)
- Dark Wizard → Soul Master (Lv150) → Grand Master (Lv400)
- Fairy Elf → Muse Elf (Lv150) → High Elf (Lv400)
- Summoner → Bloody Summoner (Lv150) → Dimension Master (Lv400)

### 2-Tier Evolution (Advanced Classes)
- Magic Gladiator → Duel Master (Lv400)
- Dark Lord → Lord Emperor (Lv400)
- Rage Fighter → Fist Master (Lv400)

## Stats System

### Base Stats
- **Strength (STR)**: Physical damage, carry capacity
- **Agility (AGI)**: Attack speed, defense rate, movement speed
- **Vitality (VIT)**: HP, HP recovery
- **Energy (ENE)**: MP, magic damage, MP recovery
- **Command (CMD)**: Pet/summon power (Dark Lord only)

### Derived Stats
- **MaxHP**: Calculated from Vitality and Level
- **MaxMP**: Calculated from Energy and Level
- **PhysicalDamage**: Based on Strength
- **MagicDamage**: Based on Energy
- **Defense**: Based on Agility and Vitality
- **DefenseRate**: Based on Agility
- **AttackSpeed**: Based on Agility
- **MovementSpeed**: Based on Agility
- **CriticalRate**: Based on Agility and Strength
- **CriticalDamage**: Based on Strength

### Stat Formulas (from StatCalculator.cs)
```csharp
MaxHP = 100 + (Vitality * 5) + (Level * 2)
MaxMP = 50 + (Energy * 3) + (Level * 1)
PhysicalDamage = Strength / 4
MagicDamage = Energy / 3
Defense = (Agility / 3) + (Vitality / 5)
DefenseRate = Agility / 3
AttackSpeed = 1.0 + (Agility / 100)
MovementSpeed = 5.0 + (Agility / 50)
CriticalRate = (Agility / 10) + (Strength / 20)
CriticalDamage = 150 + (Strength / 10)
```

## Usage Examples

### Creating a Character
```csharp
// Get character creation system
var creation = GetComponent<CharacterCreation>();

// Select class
var darkKnightClass = ClassManager.Instance.GetClassData(CharacterClassType.DarkKnight);
creation.SelectClass(darkKnightClass);

// Set appearance
var appearance = new CharacterAppearanceData
{
    FaceIndex = 0,
    HairStyleIndex = 2,
    HairColor = Color.black,
    SkinToneIndex = 3
};
creation.SetAppearance(appearance);

// Set name
creation.SetCharacterName("MyHero");

// Allocate stats
creation.AllocateStatPoint("Strength", 5);
creation.AllocateStatPoint("Vitality", 5);

// Create character
var characterData = creation.CreateCharacter();
```

### Managing Stats
```csharp
var stats = new CharacterStats();

// Add stat points
stats.AddStatPoint("Strength", 10);
stats.AddStatPoint("Agility", 5);

// Level up
stats.LevelUp(); // Adds 5 free points

// Add modifiers
var modifier = new StatModifier("MaxHP", ModifierType.Percentage, 10f, 60f, "Potion");
stats.AddModifier(modifier);

// Get calculated stats
Debug.Log($"HP: {stats.MaxHP}");
Debug.Log($"Attack Speed: {stats.AttackSpeed}");
```

### Evolution
```csharp
var evolutionSystem = GetComponent<EvolutionSystem>();
var currentClass = CharacterClassType.DarkKnight;
var stats = GetComponent<CharacterStats>();

// Check if can evolve
if (evolutionSystem.CanEvolve(currentClass, stats, questCompleted, hasItems, zen))
{
    // Perform evolution
    evolutionSystem.Evolve(ref currentClass, stats, questCompleted, hasItems, zen);
    Debug.Log($"Evolved to: {currentClass}");
}
```

## Integration with Unity

### ScriptableObjects
Create ClassData ScriptableObjects in Unity:
1. Right-click in Project → Create → Dark Legend → Character → Class Data
2. Configure class properties
3. Assign to ClassManager

### UI Setup
1. Create Canvas with CharacterCreationUI component
2. Assign UI elements in Inspector
3. Connect to CharacterCreation and CharacterAppearance systems

### Character Prefab
1. Add CharacterClass component (specific class)
2. Add CharacterStats component
3. Add CharacterAppearance component
4. Add ArmorVisual and WeaponVisual components
5. Configure mesh references

## Extension Points

### Adding New Classes
1. Create new class script inheriting from `CharacterClass`
2. Add enum value to `CharacterClassType`
3. Create corresponding ClassData ScriptableObject
4. Implement evolution path if needed

### Custom Stat Calculations
Modify `StatCalculator.cs` to adjust formulas or add new derived stats.

### Custom Modifiers
Create new `StatModifier` types for buffs, debuffs, equipment bonuses, etc.

## Notes

- All classes follow MU Online conventions for stats and progression
- Comments are bilingual (English/Vietnamese) for international team
- System is designed to be data-driven via ScriptableObjects
- UI components use TextMeshPro for better text rendering
- Evolution system supports quest and item requirements
- Character slots support up to 5 characters per account

## File Count: 34 C# Scripts
## Total Lines of Code: ~2,846 lines
