# 🗡️ Dark Legend

A 3D PC RPG Game inspired by MU Online - Built with Unity & C#

## 📋 Overview

Dark Legend is a PC-first 3D action RPG inspired by the classic MU Online. Features real-time hack & slash combat, character progression, and a comprehensive inventory system optimized for keyboard and mouse controls.

## ✨ Features

### 🎮 Character System
- **3 Unique Classes:**
  - **Dark Knight** (Hiệp Sĩ Bóng Tối) - High STR/VIT, Melee combat specialist
  - **Dark Wizard** (Pháp Sư Bóng Tối) - High ENE, Powerful magic attacks
  - **Elf** (Tiên Nữ) - High AGI, Ranged attacks and support skills

### 📊 Stats & Progression
- **Core Stats:** Strength (STR), Agility (AGI), Vitality (VIT), Energy (ENE)
- **Derived Stats:** HP, MP, Physical Damage, Magic Damage, Defense, Attack Speed, Move Speed
- Level system with experience points (max level 400)
- Stat points allocation on level up

### ⚔️ Combat System
- Real-time hack & slash combat
- Mouse-targeted attacks
- 6 skill slots with cooldown system (hotkeys 1-6)
- Critical hit system
- Damage calculation based on stats
- Attack animations and effects

### 🤖 AI & Enemies
- Enemy AI with patrol, chase, and attack behaviors
- Wave-based enemy spawning system
- Dynamic difficulty scaling with level
- Enemy stats and rewards system
- Aggro and return-to-patrol mechanics

### 🎒 Inventory & Equipment
- 64-slot inventory system (8x8 grid)
- Equipment system with 9 slots: Weapon, Helmet, Armor, Gloves, Pants, Boots, Wings, 2x Accessories
- Item rarity system: Common, Uncommon, Rare, Epic, Legendary
- Stackable items
- Equipment stat bonuses
- Gold/currency system

### 🖥️ PC Controls
- **WASD / Arrow Keys** - Character movement
- **Mouse** - Camera control and targeting
- **Left Click** - Basic attack
- **Right Click** - Camera rotation / Move to position
- **1-6** - Use skills
- **Tab** - Toggle inventory
- **C** - Character info panel
- **M** - Toggle map
- **Esc** - Pause menu
- **Space** - Jump/Dodge

### 📺 UI System
- HUD with HP, MP, and EXP bars
- Skill bar with cooldown indicators
- Inventory interface with drag & drop support
- Character stats panel
- Minimap with player tracking
- Pause and settings menus

### 🎥 Camera System
- Third-person camera with mouse control
- Smooth camera follow
- Adjustable distance and rotation
- Collision detection
- Zoom functionality

### 💾 Save System
- JSON-based save/load system
- Multiple save slots (3 slots)
- Saves character progress, stats, inventory, and position
- Auto-save functionality

### 🎵 Audio System
- Music and SFX management
- Object pooling for sound effects
- 3D spatial audio support
- Volume controls
- Background music system

### ⚡ Performance Optimization
- Object pooling system for frequently spawned objects
- Efficient enemy spawning
- Optimized UI updates
- Singleton pattern for managers

## 🏗️ Project Structure

```
Assets/
├── Scripts/
│   ├── Character/
│   │   ├── PlayerController.cs      # WASD movement & controls
│   │   ├── CharacterStats.cs        # Stats system
│   │   ├── CharacterClass.cs        # Class definitions
│   │   ├── CharacterClassData.cs    # ScriptableObject config
│   │   └── LevelSystem.cs           # EXP & leveling
│   │
│   ├── Combat/
│   │   ├── CombatSystem.cs          # Main combat handler
│   │   ├── DamageCalculator.cs      # Damage formulas
│   │   ├── Skill.cs                 # Skill base class
│   │   ├── SkillData.cs             # ScriptableObject
│   │   └── SkillManager.cs          # Skill casting
│   │
│   ├── Enemy/
│   │   ├── EnemyBase.cs             # Base enemy class
│   │   ├── EnemyAI.cs               # AI behavior
│   │   ├── EnemyStats.cs            # Monster stats
│   │   ├── EnemySpawner.cs          # Wave spawning
│   │   └── EnemyData.cs             # ScriptableObject
│   │
│   ├── Inventory/
│   │   ├── InventorySystem.cs       # Inventory management
│   │   ├── Item.cs                  # Base item class
│   │   ├── ItemData.cs              # ScriptableObject
│   │   ├── Equipment.cs             # Equipment items
│   │   └── EquipmentSlot.cs         # Equipment slots
│   │
│   ├── UI/
│   │   ├── UIManager.cs             # Central UI controller
│   │   ├── HUDController.cs         # HP/MP/EXP bars
│   │   ├── SkillBarUI.cs            # Skill hotkeys
│   │   ├── InventoryUI.cs           # Inventory panel
│   │   ├── CharacterInfoUI.cs       # Stats panel
│   │   └── MinimapUI.cs             # Minimap
│   │
│   ├── Camera/
│   │   ├── CameraController.cs      # Third-person camera
│   │   └── CameraFollow.cs          # Smooth follow
│   │
│   ├── Input/
│   │   ├── InputManager.cs          # Input handling
│   │   └── KeyBindings.cs           # Key configuration
│   │
│   ├── Managers/
│   │   ├── GameManager.cs           # Game state
│   │   ├── AudioManager.cs          # Sound/Music
│   │   ├── SaveManager.cs           # Save/Load
│   │   └── ObjectPoolManager.cs     # Performance
│   │
│   └── Utils/
│       ├── Singleton.cs             # Singleton pattern
│       ├── Constants.cs             # Game constants
│       └── Extensions.cs            # Utility methods
│
├── ScriptableObjects/
│   ├── Classes/                     # Character classes
│   ├── Skills/                      # Skill configs
│   ├── Items/                       # Item configs
│   └── Enemies/                     # Enemy configs
│
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Effects/
│   └── UI/
│
├── Scenes/
│   ├── MainMenu.unity
│   ├── GameScene.unity
│   └── LoadingScene.unity
│
├── Resources/
│   └── Data/
│
└── Materials/
```

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- C# development environment (Visual Studio or VS Code)
- Basic understanding of Unity and C#

### Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone https://github.com/mtwmcgrath/dark-legend.git
   cd dark-legend
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Click "Add" and select the project folder
   - Open the project with Unity 2022.3 LTS or newer

3. **Create ScriptableObjects**
   - Right-click in Project window
   - Create → Dark Legend → Character Class Data
   - Create → Dark Legend → Skill Data
   - Create → Dark Legend → Enemy Data
   - Create → Dark Legend → Item Data
   - Configure your character classes, skills, enemies, and items

4. **Setup Scenes**
   - Create MainMenu scene
   - Create GameScene scene
   - Add GameManager to scene
   - Add UIManager to scene
   - Setup player spawn point

5. **Configure Player**
   - Create player prefab with required components:
     - CharacterStats
     - LevelSystem
     - PlayerController
     - CombatSystem
     - SkillManager
     - InventorySystem
     - EquipmentSlotManager
   - Add CharacterController component
   - Assign character class data

6. **Build and Run**
   - File → Build Settings
   - Add scenes to build
   - Build and play!

## 🎮 How to Play

### Basic Controls
- **W/A/S/D** or **Arrow Keys** - Move your character
- **Mouse Movement** - Rotate camera (hold right click)
- **Mouse Scroll** - Zoom in/out
- **Space** - Jump
- **Left Click** - Attack enemy
- **1-6** - Cast skills

### UI Controls
- **Tab** - Open/Close inventory
- **C** - View character stats
- **M** - Toggle map
- **Esc** - Pause menu

### Character Progression
1. Defeat enemies to gain EXP
2. Level up to gain stat points
3. Allocate points to STR, AGI, VIT, or ENE
4. Learn new skills as you level up
5. Find or craft better equipment

### Combat Tips
- Use skills strategically - they have cooldowns
- Watch your MP when using skills
- Manage your HP with potions
- Critical hits deal extra damage
- Different classes excel at different ranges

## 🛠️ Technical Details

### Architecture
- **Singleton Pattern** - Used for managers (GameManager, AudioManager, etc.)
- **ScriptableObjects** - Data-driven design for characters, skills, items, enemies
- **Event System** - Decoupled communication between systems
- **Object Pooling** - Performance optimization for frequently spawned objects
- **Component-based** - Modular design for easy extension

### Code Style
- Namespace: `DarkLegend.*`
- Bilingual comments (English / Vietnamese)
- Clear method documentation
- Consistent naming conventions

### Performance Considerations
- Object pooling for projectiles and effects
- Efficient enemy spawning
- Optimized UI updates with events
- NavMesh for enemy pathfinding (optional)

## 📝 Extending the Game

### Adding a New Character Class
1. Create a new CharacterClassData ScriptableObject
2. Set base stats and growth rates
3. Define starting skills
4. Create character prefab
5. Add to character selection

### Adding a New Skill
1. Create a SkillData ScriptableObject
2. Define skill properties (damage, cooldown, cost, etc.)
3. Create visual effects prefab
4. Add skill animation
5. Assign to character class

### Adding a New Enemy
1. Create an EnemyData ScriptableObject
2. Set stats, behavior, and rewards
3. Create enemy prefab with EnemyBase component
4. Add to enemy spawner

### Adding a New Item
1. Create an ItemData ScriptableObject
2. Define item properties
3. Create world prefab (optional)
4. Add item icon
5. Configure drop rates

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Inspired by MU Online
- Built with Unity Engine
- Community feedback and support

## 📧 Contact

For questions or support, please open an issue on GitHub.

---

Made with ❤️ by the Dark Legend Team