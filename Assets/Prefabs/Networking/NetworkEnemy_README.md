# Network Enemy Prefab Configuration

## NetworkEnemy.prefab

This document describes how to create the NetworkEnemy prefab in Unity Editor.

### Required Components

#### 1. GameObject Structure
```
NetworkEnemy (root)
├── EnemyModel (3D model/sprite)
├── UI
│   └── NameTag
│       ├── Canvas
│       ├── NameText (TextMeshPro)
│       └── HealthBar (Image)
└── AI
    └── NavMeshAgent (if using navigation)
```

### 2. Required Scripts/Components on Root

#### PhotonView
- **View ID**: 0 (auto-assigned)
- **Ownership**: Fixed
- **Owner**: Master Client (automatically assigned)
- **Observed Components**:
  - EnemyNetworkSync (for IPunObservable)

> **Important**: Enemies are always controlled by Master Client. Other clients receive position/state updates.

#### EnemyNetworkSync
- **Sync Position**: ✓
- **Sync Rotation**: ✓
- **Sync Health**: ✓
- **Sync State**: ✓
- **Position Lerp Speed**: 5
- **Rotation Lerp Speed**: 5

#### AI Controller (Your Custom Script)
- Only runs on Master Client
- Example:
```csharp
void Update()
{
    if (!PhotonNetwork.IsMasterClient) return;
    
    // AI logic here
    // Movement, attack decisions, etc.
}
```

#### Character Controller / NavMeshAgent
- **NavMeshAgent** (recommended for AI)
  - Speed: 3.5
  - Angular Speed: 120
  - Acceleration: 8
  - Stopping Distance: 0.5

### 3. NameTag Setup (Same as Player)

#### Canvas
- **Render Mode**: World Space
- **Dynamic Pixels Per Unit**: 10

#### NameText
- **Text**: Enemy name (e.g., "Goblin Lv.5")
- **Color**: Red (to distinguish from players)

#### HealthBar
- **Fill Amount**: 1.0 (full health)
- **Color**: Red

#### PlayerNameTag Script (reusable)
- **Offset**: (0, 2, 0)
- **Show Health Bar**: ✓

### 4. Enemy States

```csharp
// State enum in EnemyNetworkSync
public enum EnemyState
{
    Idle = 0,      // Standing still
    Patrol = 1,    // Patrolling area
    Chase = 2,     // Chasing player
    Attack = 3,    // Attacking player
    Dead = 4       // Dead (waiting respawn)
}
```

### 5. Enemy Properties

#### Stats
- **Max Health**: 100 (configurable)
- **Attack Damage**: 10
- **Attack Range**: 2
- **Detection Range**: 10
- **Patrol Range**: 5
- **Move Speed**: 3.5

#### Rewards
- **EXP Reward**: 50
- **Gold Drop**: 10-20
- **Item Drop Chance**: 0.1 (10%)

### 6. Network Behavior

#### Master Client (Host)
- Runs AI logic
- Detects and attacks players
- Sends position/rotation/state to clients
- Handles damage calculation
- Manages respawning

#### Other Clients
- Receive position/rotation/state
- Interpolate movement
- Display health bar
- Show animations
- **Do NOT run AI logic**

### 7. Combat Integration

```csharp
// Example: Player attacks enemy
public void OnPlayerAttack(int playerViewID, int damage)
{
    if (!PhotonNetwork.IsMasterClient) return;
    
    PhotonView enemyView = GetComponent<PhotonView>();
    EnemyNetworkSync enemySync = GetComponent<EnemyNetworkSync>();
    
    enemySync.TakeDamage(damage, playerViewID);
}
```

### 8. Spawning Enemies

#### Option A: Pre-placed in Scene
```csharp
// Add PhotonView to enemy already in scene
// Master Client will control it automatically
```

#### Option B: Runtime Spawning (Master Client Only)
```csharp
// Only Master Client spawns enemies
if (PhotonNetwork.IsMasterClient)
{
    Vector3 spawnPos = GetRandomSpawnPoint();
    GameObject enemy = PhotonNetwork.Instantiate(
        "NetworkEnemy", 
        spawnPos, 
        Quaternion.identity
    );
}
```

### 9. Enemy Types

Create variants for different enemy types:

#### Goblin (NetworkEnemy_Goblin.prefab)
- Health: 50
- Damage: 5
- Speed: 3.5

#### Orc (NetworkEnemy_Orc.prefab)
- Health: 150
- Damage: 15
- Speed: 2.5

#### Dragon (NetworkEnemy_Dragon.prefab)
- Health: 1000
- Damage: 50
- Speed: 5
- Boss enemy

### 10. Prefab Location
- Must be in: `Assets/Resources/NetworkEnemy.prefab`
- Variants: `Assets/Resources/NetworkEnemy_[Type].prefab`

### 11. Animation Sync

```csharp
// In EnemyNetworkSync
public void PlayAnimation(string animationTrigger)
{
    if (!PhotonNetwork.IsMasterClient) return;
    
    // Send animation trigger to all clients
    photonView.RPC("RPC_PlayAnimation", RpcTarget.All, animationTrigger);
}

[PunRPC]
void RPC_PlayAnimation(string trigger)
{
    animator.SetTrigger(trigger);
}
```

### 12. Loot System

```csharp
// In EnemyNetworkSync
private void DropLoot(int killerViewID)
{
    if (!PhotonNetwork.IsMasterClient) return;
    
    // Spawn loot items
    PhotonNetwork.Instantiate("LootItem", transform.position, Quaternion.identity);
    
    // Give EXP to killer
    PhotonView killerView = PhotonView.Find(killerViewID);
    if (killerView != null)
    {
        // killerView.RPC("RPC_GainEXP", killerView.Owner, expReward);
    }
}
```

### 13. Respawn System

```csharp
// Automatic respawn after 30 seconds
IEnumerator RespawnCoroutine()
{
    yield return new WaitForSeconds(30f);
    
    if (!PhotonNetwork.IsMasterClient) yield break;
    
    // Reset enemy
    currentHealth = maxHealth;
    transform.position = spawnPosition;
    gameObject.SetActive(true);
    
    // Notify all clients
    photonView.RPC("RPC_Respawn", RpcTarget.All, spawnPosition);
}
```

### 14. Performance Optimization

#### Send Rate
- **Position**: Every 0.1s (10 Hz)
- **Health**: Only when changed
- **State**: Only when changed

#### Culling
- Disable AI updates for enemies far from all players
- Reduce update frequency for distant enemies

#### Object Pooling
```csharp
// Reuse enemy objects instead of destroying/creating
public class EnemyPool
{
    List<GameObject> pooledEnemies;
    
    public GameObject GetEnemy()
    {
        // Return inactive enemy from pool
        // Or create new if pool empty
    }
    
    public void ReturnEnemy(GameObject enemy)
    {
        // Deactivate and return to pool
    }
}
```

### 15. Testing

#### Single Player Test
1. Start as Master Client
2. Enemies should spawn and run AI
3. Attack enemies and verify damage

#### Multiplayer Test
1. Player 1 (Master Client): Sees AI running
2. Player 2 (Client): Sees enemies move but doesn't run AI
3. Both can attack enemies
4. Damage syncs to both clients
5. Death/respawn works correctly

### Checklist
- [ ] PhotonView added and configured
- [ ] EnemyNetworkSync script attached
- [ ] AI controller only runs on Master Client
- [ ] NameTag displays correctly
- [ ] Health bar updates
- [ ] Combat integration works
- [ ] Loot drops correctly
- [ ] Respawn system works
- [ ] Placed in Assets/Resources/
- [ ] Added to PhotonServerSettings
- [ ] Tested in multiplayer
- [ ] Performance optimized

---

**Note**: Always test enemy spawning and combat with at least 2 clients to verify Master Client authority and proper synchronization.
