# Known TODOs and Future Enhancements

This document tracks incomplete features and planned enhancements for the Character Classes System.

## Critical TODOs (Required for Production)

### 1. Class Unlock System (ClassManager.cs, line 62)
**Status**: Hardcoded to always return true
**Description**: The unlock check for advanced classes (Magic Gladiator, Dark Lord, Summoner, Rage Fighter) needs proper implementation.
**Requirements**:
- Check player's account for any character that meets level requirements
- Magic Gladiator: Requires any character at level 220+
- Dark Lord: Requires any character at level 250+
- Summoner: Requires any character at level 220+
- Rage Fighter: Requires any character at level 220+

**Implementation Plan**:
```csharp
public bool IsClassUnlocked(ClassData classData)
{
    if (!classData.IsUnlockableClass())
        return true;
    
    // Check account character database
    var accountChars = AccountManager.GetAllCharacters();
    foreach (var character in accountChars)
    {
        if (character.Level >= classData.UnlockLevel)
            return true;
    }
    return false;
}
```

### 2. Evolution Requirements Check (EvolutionUI.cs, line 126)
**Status**: Hardcoded to false/0
**Description**: Evolution requirements are not properly checked before allowing evolution.
**Requirements**:
- Check quest completion status
- Verify required items in inventory
- Verify sufficient Zen currency

**Implementation Plan**:
```csharp
private void OnEvolveClicked()
{
    if (evolutionSystem != null)
    {
        var questCompleted = QuestManager.IsQuestCompleted(requirements.EvolutionQuestId);
        var hasItems = InventoryManager.HasRequiredItems(requirements.RequiredItemIds);
        var zen = PlayerManager.GetCurrentZen();
        
        evolutionSystem.Evolve(ref currentClass, currentStats, questCompleted, hasItems, zen);
    }
    Hide();
}
```

### 3. Evolution Bonuses (ClassManager.cs, line 142)
**Status**: TODO comment, bonuses not applied
**Description**: Evolution bonuses need to be properly applied when character evolves.

**Implementation Plan**:
- Define evolution bonuses in EvolutionSystem
- Apply stat bonuses on evolution
- Unlock new skills
- Add title/achievements

### 4. Armor Visual Updates (ArmorVisual.cs, line 44)
**Status**: TODO comment, visual updates not implemented
**Description**: Armor appearance changes based on type and tier need implementation.

**Implementation Plan**:
- Load armor meshes from Resources or Addressables
- Apply materials based on armor tier
- Handle armor set effects
- Update character model appearance

### 5. Weapon Visual Updates (WeaponVisual.cs, line 123)
**Status**: TODO comment, visual updates not implemented
**Description**: Weapon appearance changes based on type and tier need implementation.

**Implementation Plan**:
- Load weapon models from Resources or Addressables
- Apply materials based on weapon tier
- Handle weapon glow effects for high-tier weapons
- Update weapon trails and particles

## Non-Critical TODOs (Nice to Have)

### 6. Character Data Persistence
**Description**: Save/load character data to disk or server
**Priority**: Medium
**Dependencies**: Database or serialization system

### 7. Animation Integration
**Description**: Connect class abilities to animation system
**Priority**: Medium
**Dependencies**: Animation controller and animator setup

### 8. Skill System Integration
**Description**: Link special abilities to actual skill implementations
**Priority**: High
**Dependencies**: Skill system implementation

### 9. Equipment System Integration
**Description**: Connect armor/weapon visuals to actual equipment system
**Priority**: High
**Dependencies**: Equipment and inventory system

### 10. Quest System Integration
**Description**: Implement evolution quests with actual quest mechanics
**Priority**: High
**Dependencies**: Quest system implementation

## Future Enhancements

### Character Classes
- [ ] Add more evolution tiers (4th, 5th class advancement)
- [ ] Add class-specific mount systems (beyond Dark Lord)
- [ ] Add awakening system for max level characters
- [ ] Add class change system (reset to different class)

### Stats System
- [ ] Add more derived stats (penetration, block, dodge)
- [ ] Add stat soft/hard caps
- [ ] Add stat reset potions
- [ ] Add stat presets/templates

### Evolution System
- [ ] Add multiple evolution paths per class
- [ ] Add evolution preview system
- [ ] Add evolution cinematics
- [ ] Add failed evolution mechanics

### Character Creation
- [ ] Add more appearance options
- [ ] Add voice selection
- [ ] Add background story selection
- [ ] Add starting gift selection

### Visual Systems
- [ ] Add equipment dyeing system
- [ ] Add transmog/costume system
- [ ] Add wings/auras system
- [ ] Add title display system

## Integration Requirements

### Required Systems for Full Functionality
1. **Quest System**: For evolution quests
2. **Inventory System**: For evolution item requirements
3. **Currency System**: For Zen requirements
4. **Database System**: For character persistence
5. **Account System**: For multi-character unlock tracking
6. **Skill System**: For class abilities
7. **Equipment System**: For armor/weapon management
8. **Resource Loading**: For dynamic model loading

### Optional Systems
1. **Achievement System**: For evolution achievements
2. **Title System**: For evolution titles
3. **Social System**: For class-specific features
4. **Guild System**: For class roles in guilds

## Testing Checklist

### Before Production
- [ ] Implement critical TODOs (#1-5)
- [ ] Test all 7 classes creation
- [ ] Test all evolution paths
- [ ] Test stat calculations accuracy
- [ ] Test UI responsiveness
- [ ] Test character persistence
- [ ] Test class unlock conditions
- [ ] Test evolution requirements
- [ ] Performance test with multiple characters
- [ ] Memory leak testing

### After Integration
- [ ] Test with quest system
- [ ] Test with inventory system
- [ ] Test with equipment system
- [ ] Test with skill system
- [ ] End-to-end player journey testing

## Notes

- All TODO items are documented in code with clear markers
- Critical items affect core functionality
- Non-critical items enhance user experience
- Future enhancements are long-term improvements
- Integration requirements should be planned early

## Timeline Estimates

- **Critical TODOs**: 2-3 weeks
- **Non-Critical TODOs**: 4-6 weeks
- **Future Enhancements**: 3-6 months
- **Full Integration**: 2-4 months

Last Updated: 2026-01-03
