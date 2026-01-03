# Dark Legend - Developer Quick Reference

## 🎯 System Overview

### Character System
```csharp
// Access character stats
CharacterStats stats = GetComponent<CharacterStats>();
stats.strength += 10;
stats.CalculateDerivedStats();

// Level up
LevelSystem levelSys = GetComponent<LevelSystem>();
levelSys.AddExp(500);
```

### Combat System
```csharp
// Perform attack
CombatSystem combat = GetComponent<CombatSystem>();
combat.TryAttack(targetEnemy);

// Cast skill
SkillManager skillMgr = GetComponent<SkillManager>();
skillMgr.TryCastSkill(0, targetEnemy); // Skill slot 0

// Calculate damage
int damage = DamageCalculator.CalculatePhysicalDamage(
    attackerDamage, 
    targetDefense, 
    isCritical
);
```

### Inventory System
```csharp
// Add item to inventory
InventorySystem inv = GetComponent<InventorySystem>();
Item item = itemData.CreateItemInstance();
inv.AddItem(item);

// Equip item
Equipment equipment = item as Equipment;
EquipmentSlotManager equipMgr = GetComponent<EquipmentSlotManager>();
equipMgr.EquipItem(equipment);

// Add gold
inv.AddGold(100);
```

### Enemy System
```csharp
// Spawn enemy
EnemySpawner spawner = FindObjectOfType<EnemySpawner>();
GameObject enemy = spawner.SpawnSpecificEnemy(enemyData, position);

// Enemy AI states
EnemyAI ai = enemy.GetComponent<EnemyAI>();
// States: Idle, Patrol, Chase, Attack, Return
```

### UI System
```csharp
// Access UI Manager
UIManager ui = UIManager.Instance;
ui.ShowNotification("Level Up!");
ui.TogglePauseMenu();

// Show damage number
ui.ShowDamageNumber(worldPos, damage, isCritical);
```

### Audio System
```csharp
// Play music
AudioManager audio = AudioManager.Instance;
audio.PlayMusic(gameplayMusic);

// Play sound effect
audio.PlaySFX(attackSound);

// Play 3D sound at position
audio.PlaySFXAtPosition(hitSound, hitPosition);
```

### Save/Load System
```csharp
// Save game
SaveManager saveManager = SaveManager.Instance;
saveManager.SaveGame(slotIndex: 0);

// Load game
saveManager.LoadGame(slotIndex: 0);

// Check if save exists
bool exists = saveManager.SaveExists(0);
```

### Game Manager
```csharp
// Access game manager
GameManager gm = GameManager.Instance;

// Change game state
gm.SetGameState(GameState.Playing);
gm.PauseGame();
gm.ResumeGame();

// Get player
GameObject player = gm.GetPlayer();
```

### Input System
```csharp
// Get input
InputManager input = InputManager.Instance;
Vector2 movement = input.GetMovementInput();
bool jump = input.GetJumpPressed();
bool skill = input.GetSkillPressed(0);

// Mouse position in world
Vector3 worldPos = input.GetMouseWorldPosition(groundLayer);
```

### Object Pooling
```csharp
// Create pool
ObjectPoolManager pool = ObjectPoolManager.Instance;
pool.CreatePool("Projectiles", projectilePrefab, 50);

// Spawn from pool
GameObject obj = pool.Spawn("Projectiles", position);

// Return to pool
pool.Despawn("Projectiles", obj);

// Return after delay
pool.DespawnAfterDelay("Projectiles", obj, 3f);
```

## 🎨 ScriptableObject Creation

### Character Class
```
Right-click → Create → Dark Legend → Character Class Data
- Set base stats
- Set growth rates
- Set multipliers
- Assign starting skills
```

### Skill
```
Right-click → Create → Dark Legend → Skill Data
- Set damage multiplier
- Set mana cost
- Set cooldown
- Assign effects
```

### Enemy
```
Right-click → Create → Dark Legend → Enemy Data
- Set level and stats
- Set AI behavior
- Set rewards
- Assign prefab
```

### Item
```
Right-click → Create → Dark Legend → Item Data
- Set item type and rarity
- Set stack size
- Set price
- Configure effects
```

## 📊 Common Stats Formulas

### HP Calculation
```csharp
maxHP = 100 + (vitality * 15)
```

### MP Calculation
```csharp
maxMP = 50 + (energy * 10)
```

### Physical Damage
```csharp
physicalDamage = strength * 1.2 * classMultiplier
```

### Magic Damage
```csharp
magicDamage = energy * 1.5 * classMultiplier
```

### Defense
```csharp
defense = (vitality * 0.5 + strength * 0.3) * classMultiplier
```

### Critical Chance
```csharp
criticalChance = 0.1 + (agility * 0.001)
```

### Move Speed
```csharp
moveSpeed = 5.0 + (agility * 0.01)
```

## 🎮 Component Setup

### Player Setup
```
GameObject: Player
├── CharacterController
├── CharacterStats
├── LevelSystem
├── PlayerController
├── CombatSystem
├── SkillManager
├── InventorySystem
└── EquipmentSlotManager
```

### Enemy Setup
```
GameObject: Enemy
├── EnemyBase
├── EnemyStats
├── EnemyAI
├── Rigidbody (optional)
├── Collider
└── NavMeshAgent (optional)
```

### Scene Setup
```
Scene: GameScene
├── GameManager (empty GameObject)
├── UIManager (Canvas)
├── Main Camera + CameraController
├── Directional Light
├── Ground Plane
├── PlayerSpawnPoint (empty GameObject)
└── EnemySpawner (empty GameObject)
```

## 🔧 Common Tasks

### Add New Character Class
1. Create CharacterClassData
2. Configure stats and multipliers
3. Assign starting skills
4. Create character prefab
5. Add to character selection

### Add New Skill
1. Create SkillData
2. Set properties (damage, cost, cooldown)
3. Create effect prefab
4. Add to class's available skills
5. Test in game

### Add New Enemy
1. Create EnemyData
2. Set stats and AI behavior
3. Create enemy prefab
4. Add EnemyBase component
5. Add to spawner's enemy list

### Add New Item
1. Create ItemData
2. Configure properties
3. Create world prefab (optional)
4. Add to loot tables
5. Test pickup and use

## 🐛 Debugging Tips

### Character Not Moving
- Check CharacterController exists
- Verify Input Manager in scene
- Check ground layer mask

### Skills Not Working
- Verify SkillManager has skills
- Check MP availability
- Confirm skill cooldown

### UI Not Updating
- Check event subscriptions
- Verify references assigned
- Confirm Canvas active

### Combat Issues
- Check attack range
- Verify damage calculation
- Confirm target assignment

## 📝 Useful Constants

```csharp
// From Utils.Constants
DEFAULT_MOVE_SPEED = 5f
CRITICAL_HIT_CHANCE = 0.1f
CRITICAL_HIT_MULTIPLIER = 1.5f
MAX_LEVEL = 400
INVENTORY_SIZE = 64
SKILL_BAR_SIZE = 6
```

## 🎯 Event System

### Subscribe to Events
```csharp
// Character events
stats.OnHPChanged += (current, max) => { /* handle */ };
stats.OnDeath += () => { /* handle */ };

// Level events
levelSys.OnLevelUp += (level) => { /* handle */ };
levelSys.OnExpChanged += (current, required) => { /* handle */ };

// Inventory events
inventory.OnItemAdded += (item) => { /* handle */ };
inventory.OnInventoryChanged += () => { /* handle */ };
```

### Unsubscribe (Important!)
```csharp
void OnDestroy()
{
    stats.OnHPChanged -= HandleHPChanged;
    levelSys.OnLevelUp -= HandleLevelUp;
}
```

## 🚀 Performance Tips

1. Use Object Pooling for frequent spawns
2. Avoid GetComponent in Update
3. Cache component references
4. Use events instead of polling
5. Disable unused UI elements
6. Optimize enemy AI update frequency
7. Use NavMesh for pathfinding
8. Batch draw calls where possible

## 📚 Further Reading

- Unity Manual: https://docs.unity3d.com/Manual/
- C# Guide: https://docs.microsoft.com/dotnet/csharp/
- Game Programming Patterns: https://gameprogrammingpatterns.com/
- Unity Best Practices: https://unity.com/how-to/programming-unity

---

Quick Reference v0.1.0 | Dark Legend Team
