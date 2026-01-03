# Changelog

All notable changes to the Dark Legend project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.0] - 2026-01-03

### Added
- Initial project structure for Unity 3D RPG game
- Complete character system with stats (STR, AGI, VIT, ENE)
- Three character classes: Dark Knight, Dark Wizard, Elf
- Character class configuration via ScriptableObjects
- Level and experience system (max level 400)
- Stat points allocation system

#### Combat System
- Real-time combat with mouse targeting
- Physical and magic damage calculation
- Critical hit system
- Skill system with cooldowns
- 6 skill slots with hotkeys (1-6)
- Skill data configuration via ScriptableObjects
- Attack animations and cooldowns

#### Enemy System
- Enemy AI with patrol, chase, and attack behaviors
- Dynamic aggro system
- Wave-based enemy spawning
- Enemy stats scaling with level
- Experience and loot rewards
- Enemy configuration via ScriptableObjects

#### Inventory System
- 64-slot inventory (8x8 grid)
- Item stacking system
- Equipment system with 9 slots
- Item rarity system (Common to Legendary)
- Gold/currency system
- Equipment stat bonuses
- Item data configuration via ScriptableObjects

#### UI System
- HUD with HP, MP, and EXP bars
- Skill bar with cooldown indicators
- Inventory interface
- Character stats panel with stat allocation
- Minimap system
- Pause menu and settings

#### Camera System
- Third-person camera with mouse control
- Camera rotation and zoom
- Smooth camera follow
- Collision detection
- Adjustable camera settings

#### Input System
- Keyboard and mouse input handling
- Configurable key bindings
- WASD movement controls
- Mouse camera control
- Skill hotkeys (1-6)
- UI toggle keys (Tab, C, M, Esc)

#### Manager Systems
- GameManager with singleton pattern
- Game state management (Playing, Paused, GameOver)
- AudioManager for music and SFX
- Object pooling for performance optimization
- SaveManager with JSON serialization
- Multiple save slots support

#### Utilities
- Generic Singleton base class
- Game constants and configuration
- Extension methods for common operations
- Optimized for PC performance

#### Documentation
- Comprehensive README with features and setup
- Detailed setup guide (SETUP.md)
- Code comments in English and Vietnamese
- MIT License

### Technical Details
- Unity 2022.3 LTS compatible
- C# namespace: DarkLegend.*
- ScriptableObject-based data design
- Event-driven architecture
- Component-based design
- Clean code practices

### Project Structure
- Organized folder hierarchy
- Separated concerns (Character, Combat, Enemy, Inventory, UI, etc.)
- ScriptableObjects for data configuration
- Prefab system for reusable objects
- Resource management

## [Unreleased]

### Planned Features
- Character models and animations
- Skill visual effects
- More enemy types and bosses
- Quest system
- NPC dialogue system
- Crafting system
- Party system
- More character classes
- Additional maps and zones
- Sound effects and music tracks
- Gamepad support
- Settings menu implementation
- Localization support
- Achievement system
- Leaderboards

### Known Issues
- No 3D models yet (using primitives)
- UI requires Unity UI setup
- No skill effects implemented
- Audio clips need to be added
- Scenes need to be created
- NavMesh not configured

## Notes

This is the initial foundation release. The game is fully functional with code systems but requires Unity setup, asset creation, and scene configuration to be playable.

All core systems are implemented and ready for content creation.
