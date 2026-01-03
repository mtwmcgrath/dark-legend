# Unity Setup Guide for Character Classes System

## Quick Start

### Step 1: Import Scripts
All scripts are located in `Assets/Scripts/Character/`. Unity will automatically compile them.

### Step 2: Create Class Data ScriptableObjects

#### For Dark Knight:
1. In Unity, right-click in Project window → Create → Dark Legend → Character → Class Data
2. Name it "DarkKnight_Data"
3. In Inspector, set:
   - Class Type: DarkKnight
   - Class Name: "Dark Knight"
   - Description: "Melee warrior with high physical damage..."
   - Base Strength: 28
   - Base Agility: 20
   - Base Vitality: 25
   - Base Energy: 10
   - Strength Per Level: 5
   - Agility Per Level: 2
   - Vitality Per Level: 3
   - Energy Per Level: 1
   - Allowed Weapons: Sword, Blade, TwoHandedSword
   - Allowed Armor: Heavy
   - Roles: MeleeDPS, Tank

#### Repeat for all 7 base classes:
- Dark Knight
- Dark Wizard
- Fairy Elf
- Magic Gladiator
- Dark Lord
- Summoner
- Rage Fighter

#### And all evolution classes:
- Blade Knight, Blade Master
- Soul Master, Grand Master
- Muse Elf, High Elf
- Duel Master
- Lord Emperor
- Bloody Summoner, Dimension Master
- Fist Master

### Step 3: Create ClassManager GameObject
1. Create empty GameObject in scene
2. Name it "ClassManager"
3. Add Component → ClassManager
4. Drag all ClassData assets into the "All Classes" list
5. Mark "Don't Destroy On Load"

### Step 4: Create Character Creation Scene

#### Scene Hierarchy:
```
CharacterCreationScene
├── Canvas
│   ├── ClassSelectionPanel
│   │   ├── ClassGrid
│   │   │   └── (Class buttons will be created here)
│   │   └── ClassDescription
│   ├── AppearancePanel
│   │   ├── FaceSlider
│   │   ├── HairStyleSlider
│   │   ├── HairColorSlider
│   │   └── SkinToneSlider
│   ├── NameInputPanel
│   │   ├── NameInputField
│   │   └── ConfirmButton
│   ├── StatAllocationPanel
│   │   └── StatsUI (component)
│   └── ConfirmationPanel
│       └── CreateButton
├── CharacterPreview
│   └── Character (with appearance components)
├── CharacterCreation (GameObject with component)
└── ClassManager (from Step 3)
```

#### Setup CharacterCreationUI:
1. Add CharacterCreationUI component to Canvas
2. Assign all panel references
3. Assign button references
4. Assign slider references
5. Assign StatsUI component

#### Setup CharacterCreation:
1. Create empty GameObject "CharacterCreation"
2. Add CharacterCreation component
3. Set Starting Level: 1
4. Set Bonus Stat Points: 10

#### Setup CharacterPreview:
1. Create GameObject "CharacterPreview"
2. Add your character model
3. Add CharacterAppearance component
4. Assign face meshes array (10 options)
5. Assign hair meshes array (15 options)
6. Assign hair renderer
7. Assign skin renderer

### Step 5: Create Character Slot Manager
1. Create empty GameObject "CharacterSlotManager"
2. Add CharacterSlot component
3. Set Max Slots: 5
4. Mark "Don't Destroy On Load"

### Step 6: Test Character Creation

#### In Play Mode:
1. Run the Character Creation scene
2. Select a class
3. Customize appearance
4. Enter character name
5. Allocate stat points
6. Confirm creation

### Step 7: Setup Stats UI (In-Game)

#### For in-game stats display:
```
GameplayScene
└── Canvas
    └── StatsPanel
        ├── LevelText
        ├── StrengthText
        ├── AgilityText
        ├── VitalityText
        ├── EnergyText
        ├── CommandText
        ├── FreePointsText
        ├── HPText
        ├── MPText
        └── (other stat texts)
```

1. Create UI panel for stats
2. Add StatsUI component
3. Assign all TextMeshProUGUI references
4. Add buttons for stat allocation
5. Call `statsUI.SetStats(character.GetComponent<CharacterStats>())`

### Step 8: Setup Evolution System

#### Create EvolutionSystem GameObject:
1. Create empty GameObject "EvolutionSystem"
2. Add EvolutionSystem component
3. Create evolution data for each class
4. Register evolution paths programmatically or via Inspector

#### Example Evolution Registration:
```csharp
void Start()
{
    var evolutionSystem = FindObjectOfType<EvolutionSystem>();
    
    // Register Dark Knight -> Blade Knight evolution
    var evolutionData = new EvolutionSystem.EvolutionData
    {
        FromClass = CharacterClassType.DarkKnight,
        ToClass = CharacterClassType.BladeKnight,
        Requirements = new EvolutionRequirement
        {
            RequiredLevel = 150,
            EvolutionQuestId = "BK_Evolution_Quest"
        },
        Bonuses = new EvolutionBonus
        {
            BonusStatPoints = 10,
            NewTitle = "Blade Knight"
        }
    };
    
    evolutionSystem.RegisterEvolution(evolutionData);
}
```

## Testing Checklist

- [ ] Class selection works
- [ ] All 7 classes can be created
- [ ] Appearance customization changes visible
- [ ] Character name validation works
- [ ] Stat allocation functions correctly
- [ ] Character creation saves to slot
- [ ] Stats display updates properly
- [ ] Evolution requirements check correctly
- [ ] Evolution applies bonuses
- [ ] Multiple characters can be created

## Common Issues

### Issue: ClassData not found
**Solution**: Make sure ClassData assets are created and assigned to ClassManager

### Issue: UI elements not displaying
**Solution**: Check TextMeshProUGUI references are assigned in Inspector

### Issue: Stats not calculating
**Solution**: Ensure CharacterStats component is attached and initialized

### Issue: Evolution not working
**Solution**: Verify evolution data is registered in EvolutionSystem

### Issue: Character model not showing
**Solution**: Check character prefab has renderers and materials assigned

## Performance Tips

1. Use object pooling for UI elements
2. Cache frequently accessed components
3. Use ScriptableObjects for static data
4. Batch stat updates to avoid multiple recalculations
5. Lazy load character models only when needed

## Next Steps

After setup:
1. Create skill system
2. Add equipment system
3. Implement combat mechanics
4. Add quest system
5. Create monster AI
6. Build inventory system

## Support

For issues or questions:
- Check DEVELOPER_DOCS.md for API reference
- Review code comments in individual scripts
- Verify Unity version compatibility (2022.3 LTS)
