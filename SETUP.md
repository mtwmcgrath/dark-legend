# Dark Legend - Setup Guide

## Quick Start Checklist

### 1. Unity Setup ✅
- Install Unity 2022.3 LTS or newer
- Install Visual Studio or VS Code with C# support
- Open project in Unity Hub

### 2. Create Character Classes 🧙

Create three character class ScriptableObjects:

**Dark Knight**
- Right-click in `Assets/ScriptableObjects/Classes/`
- Create → Dark Legend → Character Class Data
- Name: `DarkKnightClass`
- Settings:
  - Base STR: 30
  - Base AGI: 20
  - Base VIT: 25
  - Base ENE: 15
  - Physical Damage Multiplier: 1.5
  - Magic Damage Multiplier: 0.5

**Dark Wizard**
- Name: `DarkWizardClass`
- Settings:
  - Base STR: 15
  - Base AGI: 20
  - Base VIT: 20
  - Base ENE: 35
  - Physical Damage Multiplier: 0.5
  - Magic Damage Multiplier: 2.0

**Elf**
- Name: `ElfClass`
- Settings:
  - Base STR: 20
  - Base AGI: 35
  - Base VIT: 20
  - Base ENE: 20
  - Physical Damage Multiplier: 1.2
  - Attack Speed Multiplier: 1.5

### 3. Create Skills ⚔️

Example skills for each class:

**Dark Knight Skills**
- Slash (1-2): Basic melee attack, 1.5x damage
- Cyclone (3): AOE spin attack, 2.0x damage
- Rageful Blow (4): Heavy single target, 3.0x damage
- Death Stab (5): Critical strike skill, 2.5x damage

**Dark Wizard Skills**
- Fireball (1): Magic projectile, 2.0x damage
- Lightning (2): Fast cast, 1.8x damage
- Meteor (3): AOE damage, 2.5x damage
- Ice Storm (4): AOE with slow, 2.2x damage

**Elf Skills**
- Triple Arrow (1): 3 arrows, 1.5x damage each
- Heal (2): Restore HP
- Penetration (3): Armor piercing shot, 2.5x damage
- Multi-Shot (4): Hit multiple enemies, 1.8x damage

### 4. Create Enemies 👹

Basic enemy types:

**Goblin** (Level 1-5)
- HP: Low
- Damage: Low
- Speed: Fast
- Aggro Range: Medium

**Orc** (Level 5-10)
- HP: Medium
- Damage: Medium
- Speed: Medium
- Aggro Range: High

**Demon** (Level 10+)
- HP: High
- Damage: High
- Speed: Slow
- Aggro Range: Very High

### 5. Create Items 🎒

**Consumables**
- Health Potion (Small/Medium/Large)
- Mana Potion (Small/Medium/Large)

**Equipment**
- Weapons: Sword, Staff, Bow
- Armor: Helmet, Chest, Gloves, Pants, Boots
- Accessories: Rings, Necklaces

### 6. Setup Player Prefab 🎮

1. Create empty GameObject named "Player"
2. Add components:
   - CharacterController
   - CharacterStats
   - LevelSystem
   - PlayerController
   - CombatSystem
   - SkillManager
   - InventorySystem
   - EquipmentSlotManager
3. Add 3D model (capsule for testing)
4. Set tag to "Player"
5. Configure ground check
6. Save as prefab in `Assets/Prefabs/Player/`

### 7. Setup Game Scene 🎬

1. Create new scene: GameScene
2. Add GameManager (Create empty GameObject)
3. Add UIManager (Create UI Canvas)
4. Add Main Camera
5. Add CameraController to camera
6. Create ground plane
7. Add spawn point
8. Add enemy spawner

### 8. Setup UI 📺

Create UI hierarchy:
```
Canvas
├── HUD
│   ├── HPBar
│   ├── MPBar
│   ├── EXPBar
│   └── LevelText
├── SkillBar
│   ├── Skill1-6 Slots
│   └── Cooldown Overlays
├── InventoryPanel (hidden)
│   ├── ItemGrid
│   └── ItemInfo
├── CharacterPanel (hidden)
│   ├── Stats Display
│   └── Stat Buttons
└── Minimap
```

### 9. Configure Input ⌨️

The input system is already set up with default keybindings:
- Movement: WASD / Arrows
- Skills: 1-6
- UI: Tab (Inventory), C (Character), M (Map), Esc (Pause)

### 10. Testing 🧪

1. Press Play in Unity
2. Character should spawn
3. Test movement with WASD
4. Test camera with mouse
5. Test skills with 1-6 keys
6. Test inventory with Tab
7. Test character panel with C
8. Spawn enemies and test combat

## Common Issues & Solutions

### Player not moving
- Check CharacterController is added
- Verify ground check setup
- Check Input Manager is in scene

### Skills not working
- Verify SkillData ScriptableObjects are created
- Check skill manager has skills assigned
- Ensure MP is sufficient

### UI not showing
- Check Canvas is active
- Verify UIManager is in scene
- Check event system exists

### Camera not following
- Verify target is set to player
- Check camera controller settings
- Ensure player has correct tag

## Next Steps

1. Create your own character models
2. Design custom skills and effects
3. Add more enemy types
4. Create unique items and equipment
5. Build levels and maps
6. Add sound effects and music
7. Polish UI and effects
8. Add multiplayer (future)

## Resources

- Unity Documentation: https://docs.unity3d.com
- C# Documentation: https://docs.microsoft.com/dotnet/csharp/
- Game Design Patterns: https://gameprogrammingpatterns.com/

## Support

If you encounter issues:
1. Check Unity console for errors
2. Review setup steps
3. Check GitHub issues
4. Create new issue with details

Happy developing! 🎮
