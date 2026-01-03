# 🗺️ Dark Legend - Maps System

Complete maps system for Dark Legend, inspired by MU Online, featuring 15+ maps with NPCs, monsters, portals, and dynamic environments.

## 📁 Structure

```
Assets/Scripts/Maps/
├── Core/           # Core map management systems
├── Zones/          # Different zone types (Town, Hunting, PvP, etc.)
├── Spawning/       # Monster and boss spawning systems
├── NPCs/           # NPC system (Shops, Quests, Storage, Guild, Teleport)
├── Portals/        # Portal and teleportation systems
├── Environment/    # Day/Night, Weather, Ambient sounds
├── Minimap/        # Minimap and World Map systems
└── UI/             # Map-related UI components
```

## 🎯 Features

### Core Systems

#### MapManager
Central manager for all maps in the game:
- Load/Unload maps asynchronously
- Transition between maps
- Track current and previous maps
- Support for multiple map types (Town, Hunting Ground, Dungeon, Boss Area, PvP, Event, Arena)

#### MapData (ScriptableObject)
Configurable map data including:
- Basic info (name, level range, type)
- Environment settings (weather, day/night cycle, ambient sound)
- Spawn configurations (monsters, bosses, NPCs)
- Portal definitions
- Special features for event maps

#### MapLoader
Handles async loading with:
- Scene loading and unloading
- Resource management
- NPC spawning
- Monster spawner setup
- Environment initialization

### Zone Types

#### TownZone
Safe zones with NPCs and services:
- Fountain for HP/MP regeneration
- Multiple NPC services
- Safe from PvP and combat

#### HuntingZone
Monster spawning and hunting areas:
- Dynamic monster spawning
- Day/night spawn variations
- Difficulty scaling
- Boss spawns

#### PvPZone
Free-for-all PvP zones:
- Forced PvP mode
- Death penalties
- Item dropping
- PvP points and ranking

#### DungeonZone
Multi-floor dungeons:
- Multiple floors with increasing difficulty
- Time limits
- Party requirements
- Instance support

#### BossZone
Special boss encounter areas:
- Scheduled boss spawns
- Server-wide announcements
- Minion spawns
- High rewards

#### EventZone
Special event maps:
- Wave-based encounters
- Time-limited events
- Entry tickets required
- Special rewards

### NPC System

#### NPCBase
Base class for all NPCs with:
- Interaction system
- Dialog system
- Range detection
- UI integration

#### ShopNPC
Vendor NPCs:
- Multiple shop types (Weapon, Armor, Potion, etc.)
- Buy and sell items
- Repair system
- Shop hours support

#### QuestNPC
Quest givers:
- Multiple quests per NPC
- Quest tracking
- Rewards system
- Prerequisites

#### StorageNPC
Warehouse management:
- Personal storage
- Expandable slots
- Item sorting
- Shared storage option

#### GuildNPC
Guild master:
- Guild creation
- Member management
- Alliances
- Guild wars

#### TeleportNPC
Teleportation services:
- Multiple destinations
- Dynamic pricing
- Level requirements
- Cooldown system

### Spawning System

#### MonsterSpawner
Dynamic monster spawning:
- Configurable spawn areas
- Respawn timers
- Day/night spawn rules
- Player proximity detection
- Random stats

#### SpawnPoint
Individual spawn locations:
- Position and rotation
- Spawn radius
- One-time or repeating
- Active/inactive state

#### SpawnWave
Wave-based spawning for events:
- Multiple monster types per wave
- Wave delays
- Spawn intervals
- Progress tracking

#### BossSpawner
Boss management:
- Scheduled spawns
- Warning announcements
- Minion support
- Stat modifiers
- Reward multipliers

### Portal System

#### Portal
Basic portal functionality:
- Map transitions
- Entry requirements (level, item, Zen)
- Party portals
- Cooldown system
- Visual and audio effects

#### TownPortal
Return to town mechanic:
- Cast time
- Interruptible
- Combat restrictions
- Item support

#### DungeonPortal
Dungeon entry:
- Party requirements
- Daily entry limits
- Cooldown tracking
- Instance creation

### Environment System

#### DayNightCycle
Dynamic time system:
- Configurable day length (default 30 real minutes)
- Lighting changes
- Ambient color transitions
- Time events (Dawn, Dusk, Noon, Midnight)

#### Weather
Dynamic weather system:
- Multiple weather types (Clear, Rain, Snow, Fog, Sandstorm)
- Visual effects
- Audio effects
- Gameplay effects (visibility, combat modifiers)
- Auto-changing weather

#### AmbientSound
Environmental audio:
- Day/night ambient sounds
- Zone-specific audio
- Smooth transitions
- Volume fading

#### EnvironmentEffect
Environmental hazards:
- Damage over time zones
- Buff/Debuff areas
- Movement effects
- Visual indicators

### Minimap System

#### MinimapSystem
Real-time minimap:
- Player position tracking
- Icon system
- Zoom support
- Rotation options
- Configurable display

#### MinimapIcon
Object markers:
- Multiple icon types (Player, NPC, Monster, Boss, Portal, etc.)
- Sorting layers
- Distance fading
- Rotation support

#### WorldMap
Full server map:
- All map locations
- Discovery system
- Map markers
- Player position
- Fast travel

## 🗺️ 15+ Maps

### Towns (Safe Zones)

1. **Lorencia** (Level 1+)
   - Starting point
   - Basic shops and NPCs
   - Fountain
   - Arena entrance

2. **Devias** (Level 60+)
   - Advanced shops
   - Snow weather
   - Higher level quests
   - Evolution NPC

3. **Noria** (Level 40+)
   - Elf-specific NPCs
   - Forest environment
   - Bow/Crossbow shops

### Hunting Zones

4. **Lorencia Outskirts** (Level 1-30)
   - Beginner monsters
   - Giant Bull boss (Level 30, 2h spawn)

5. **Dungeon** (Level 15-80)
   - 3 floors
   - Increasing difficulty
   - Kundun Fragment boss

6. **Lost Tower** (Level 80-160)
   - 7 floors
   - Socket items
   - Tower Guardian boss

7. **Atlans** (Level 60-120)
   - Underwater theme
   - Water breathing required
   - Hydra King boss

8. **Tarkan** (Level 120-200)
   - Desert zone
   - Sandstorm weather
   - Zaikan boss (4h spawn)

9. **Aida** (Level 180-260)
   - Demonic forest
   - Dark, foggy
   - Demon Lord boss

10. **Karutan** (Level 260-350)
    - Wasteland
    - Kundun World Boss
    - Wings materials

### Special/Event Maps

11. **Devil Square** (Level 100+)
    - 20-minute duration
    - 5 waves + Boss
    - Every 2 hours

12. **Blood Castle** (Level 80+)
    - 15-minute duration
    - Gate destruction objective
    - Every 3 hours

13. **Chaos Castle** (Level 150+)
    - 10-minute PvP
    - Last player standing
    - Every 4 hours

14. **Kalima** (Level 200+)
    - 7 levels
    - Boss summoning
    - Ancient items

15. **Arena** (Any level)
    - 1v1, 3v3, 5v5
    - Ranking system
    - Arena points

## 🎮 Usage Examples

### Creating a Map

```csharp
// Create MapData ScriptableObject in Unity
// Configure in Inspector:
// - Basic Info (name, level range, type)
// - Scene name
// - Spawn position
// - Environment settings
// - Monster spawns
// - NPC spawns
// - Portals
```

### Transitioning to a Map

```csharp
// Get map by name
MapData targetMap = MapManager.Instance.GetMapByName("Lorencia");

// Transition to map
MapManager.Instance.TransitionToMap(targetMap);

// Transition with custom spawn position
MapManager.Instance.TransitionToMap(targetMap, new Vector3(100, 0, 100));
```

### Creating a Portal

```csharp
// Add Portal component to GameObject
// Configure in Inspector:
// - Destination map
// - Spawn position
// - Requirements (level, item, Zen)
// - Portal settings
// - Effects

// Or create programmatically
Portal portal = gameObject.AddComponent<Portal>();
portal.SetDestination(targetMap, spawnPos);
portal.SetRequirements(minLevel: 50, zenCost: 1000);
```

### Spawning Monsters

```csharp
// Add MonsterSpawner component
// Configure:
// - Monster prefab
// - Max count
// - Respawn time
// - Spawn area or spawn points
// - Spawn rules (day/night, player proximity)

// Initialize spawner
MonsterSpawner spawner = GetComponent<MonsterSpawner>();
spawner.InitializeSpawner();
```

### Day/Night Cycle

```csharp
// Add DayNightCycle component
// Configure lighting and colors

// Control time
DayNightCycle cycle = FindObjectOfType<DayNightCycle>();
cycle.SetTime(12f); // Noon
cycle.SetTimeSpeed(2f); // 2x speed
cycle.PauseTime();

// Check time
bool isNight = cycle.IsNightTime();
float currentTime = cycle.GetCurrentTime();
```

### Weather System

```csharp
// Add Weather component
// Configure effects and sounds

// Change weather
Weather weather = FindObjectOfType<Weather>();
weather.SetWeather(WeatherType.Rain);
weather.SetWeather(WeatherType.Clear, instant: true);

// Get combat modifier
float modifier = weather.GetCombatModifier("Fire");
```

## 📝 Notes

- All systems use the `DarkLegend.Maps` namespace
- Comments are in both Vietnamese and English
- Systems are designed to be modular and extensible
- Uses Unity's ScriptableObject for data configuration
- Event-driven architecture for map transitions
- Object pooling recommended for monsters
- LOD system recommended for performance

## 🔮 Future Enhancements

- Addressable Assets for map loading
- Advanced object pooling
- LOD system implementation
- Network synchronization for multiplayer
- Save/Load system integration
- Achievement system integration
- Map editor tools

## 📚 Dependencies

- Unity 2022.3 LTS or higher
- TextMeshPro (optional, for better text rendering)
- Post-processing (optional, for environmental effects)

---

Created for Dark Legend - MU Online Inspired 3D RPG
