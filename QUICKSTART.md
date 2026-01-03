# 🚀 Quick Start Guide - Dark Legend Multiplayer

## For Developers

This guide helps you quickly understand and use the multiplayer system in Dark Legend.

## 📁 File Overview

### Core Managers (Singleton Pattern)
These are persistent managers that handle core networking functionality:

```csharp
NetworkManager.Instance      // Connection management, auto-reconnect
RoomManager.Instance         // Room operations, properties
ChatSystem.Instance          // All chat functionality
PartySystem.Instance         // Party management
PvPSystem.Instance          // PvP/duel system
PlayerSpawnManager.Instance // Player spawning
```

### Component Scripts
These are attached to GameObjects:

```csharp
PlayerNetworkSync      // On player prefab - syncs transform, animation, stats
CombatNetworkSync      // On player/enemy - syncs combat actions
EnemyNetworkSync       // On enemy prefab - syncs enemy state
PlayerPhotonView       // On player prefab - manages PhotonView setup
PlayerNameTag          // On player - displays name/health above head
```

### UI Scripts
These manage UI panels:

```csharp
LoginUI    // Login screen
LobbyUI    // Room list and creation
RoomUI     // In-room UI
ChatUI     // Chat interface
PartyUI    // Party management
```

## 🎮 Common Usage Patterns

### 1. Connect to Photon

```csharp
// Simple connection
NetworkManager.Instance.ConnectToPhoton();

// Or with PhotonLauncher
PhotonLauncher launcher = GetComponent<PhotonLauncher>();
launcher.Login("PlayerName");
launcher.ConnectToRegion("asia");
```

### 2. Create/Join Room

```csharp
// Create room
RoomManager.Instance.CreateRoom(
    roomName: "My Room",
    mapName: "MainMap", 
    difficulty: 2,
    pvpEnabled: false
);

// Join room by name
RoomManager.Instance.JoinRoom("My Room");

// Join random room
RoomManager.Instance.JoinRandomRoom();

// Join or create if doesn't exist
RoomManager.Instance.JoinOrCreateRoom("My Room", "MainMap", 1, false);
```

### 3. Spawn Player

```csharp
// Automatic spawn (in PlayerSpawnManager.Start())
GameObject player = PlayerSpawnManager.Instance.SpawnLocalPlayer();

// Manual spawn at specific position
Vector3 position = new Vector3(0, 0, 0);
GameObject player = PlayerSpawnManager.Instance.SpawnPlayerAt(
    position, 
    Quaternion.identity
);
```

### 4. Send Chat Messages

```csharp
// Room chat (default)
ChatSystem.Instance.SendRoomMessage("Hello everyone!");

// Global chat
ChatSystem.Instance.SendGlobalMessage("Hello world!");

// Party chat
ChatSystem.Instance.SendPartyMessage("Let's go!");

// Whisper (private message)
ChatSystem.Instance.SendWhisper("PlayerName", "Hi there!");

// Or use commands
ChatSystem.Instance.ProcessChatCommand("/g Hello world!");
ChatSystem.Instance.ProcessChatCommand("/w PlayerName Hi!");
```

### 5. Party Management

```csharp
// Create party
PartySystem.Instance.CreateParty();

// Invite player
PartySystem.Instance.InvitePlayer("PlayerName");

// Accept invite
PartySystem.Instance.AcceptPartyInvite("LeaderName");

// Leave party
PartySystem.Instance.LeaveParty();

// Kick member (leader only)
PartySystem.Instance.KickMember("PlayerName");

// Check if in party
bool inParty = PartySystem.Instance.IsInParty();

// Check if party leader
bool isLeader = PartySystem.Instance.IsPartyLeader();
```

### 6. PvP System

```csharp
// Toggle PvP mode
PvPSystem.Instance.TogglePvPMode();

// Request duel
PvPSystem.Instance.RequestDuel("PlayerName");

// Accept duel
PvPSystem.Instance.AcceptDuel("RequesterName");

// Enter PvP zone
PvPSystem.Instance.EnterPvPZone("Arena");

// Check if can attack player
Player target = GetTargetPlayer();
bool canAttack = PvPSystem.Instance.CanAttackPlayer(target);
```

### 7. Combat Sync

```csharp
// On local player's combat script
CombatNetworkSync combatSync = GetComponent<CombatNetworkSync>();

// Attack target
int targetViewID = targetPhotonView.ViewID;
combatSync.Attack(targetViewID, damage: 50);

// Cast skill at position
combatSync.CastSkill(skillID: 1, targetPosition);

// Cast skill on target
combatSync.CastSkillOnTarget(skillID: 1, targetViewID);

// Apply buff
combatSync.ApplyBuff(buffID: 1, duration: 10f);

// Handle death
combatSync.Die(killerViewID);

// Respawn
combatSync.Respawn(spawnPosition);
```

### 8. Player Sync

```csharp
PlayerNetworkSync networkSync = GetComponent<PlayerNetworkSync>();

// Update stats for sync
networkSync.UpdateStats(hp: 100, mp: 50, level: 5);

// Update animation state
int animStateHash = Animator.StringToHash("Attack");
networkSync.UpdateAnimationState(animStateHash);

// Teleport player
networkSync.Teleport(new Vector3(10, 0, 10));

// Get synced data (on any player)
float currentHP = networkSync.GetCurrentHP();
float currentMP = networkSync.GetCurrentMP();
int currentLevel = networkSync.GetCurrentLevel();
```

### 9. Enemy Spawning (Master Client Only)

```csharp
// Only works if you're Master Client
if (PhotonNetwork.IsMasterClient)
{
    GameObject enemy = EnemyNetworkSync.SpawnEnemy(
        prefabName: "NetworkEnemy",
        position: spawnPosition,
        rotation: Quaternion.identity
    );
}

// On enemy script
EnemyNetworkSync enemySync = GetComponent<EnemyNetworkSync>();

// Take damage (Master Client only)
if (PhotonNetwork.IsMasterClient)
{
    enemySync.TakeDamage(damage: 50, attackerViewID);
}

// Attack player (Master Client only)
if (PhotonNetwork.IsMasterClient)
{
    enemySync.AttackPlayer(targetViewID, damage: 30);
}

// Change state (Master Client only)
if (PhotonNetwork.IsMasterClient)
{
    enemySync.ChangeState(newState: 2); // 2 = Chase
}
```

### 10. Name Tag

```csharp
PlayerNameTag nameTag = GetComponentInChildren<PlayerNameTag>();

// Update health bar
nameTag.UpdateHealthBar(currentHealth: 80, maxHealth: 100);

// Show/hide health bar
nameTag.ShowHealthBar(true);

// Update name with level
nameTag.SetPlayerNameWithLevel("PlayerName", level: 10);

// Update with class and level
nameTag.SetPlayerInfo("PlayerName", "DarkKnight", level: 10);
```

## 🎯 Event Handling

### Subscribe to Events

```csharp
void Start()
{
    // Network events
    if (NetworkManager.Instance != null)
    {
        // No events exposed yet, but connection callbacks work via MonoBehaviourPunCallbacks
    }
    
    // Room events
    if (RoomManager.Instance != null)
    {
        RoomManager.Instance.RoomJoinedEvent += OnRoomJoined;
        RoomManager.Instance.RoomLeftEvent += OnRoomLeft;
        RoomManager.Instance.RoomListUpdated += OnRoomListUpdated;
    }
    
    // Chat events
    if (ChatSystem.Instance != null)
    {
        ChatSystem.Instance.MessageReceived += OnChatMessage;
    }
    
    // Party events
    if (PartySystem.Instance != null)
    {
        PartySystem.Instance.PartyCreated += OnPartyCreated;
        PartySystem.Instance.PartyJoined += OnPartyJoined;
        PartySystem.Instance.PartyMemberJoined += OnPartyMemberJoined;
        PartySystem.Instance.PartyInviteReceived += OnPartyInvite;
    }
    
    // PvP events
    if (PvPSystem.Instance != null)
    {
        PvPSystem.Instance.PvPModeChanged += OnPvPModeChanged;
        PvPSystem.Instance.DuelRequested += OnDuelRequest;
        PvPSystem.Instance.DuelStarted += OnDuelStarted;
        PvPSystem.Instance.PlayerKilled += OnPlayerKilled;
    }
    
    // Combat events
    CombatNetworkSync combatSync = GetComponent<CombatNetworkSync>();
    if (combatSync != null)
    {
        combatSync.DamageReceivedEvent += OnDamageReceived;
        combatSync.DeathEvent += OnDeath;
        combatSync.RespawnEvent += OnRespawn;
    }
}

void OnDestroy()
{
    // Unsubscribe from all events to prevent memory leaks
    if (RoomManager.Instance != null)
    {
        RoomManager.Instance.RoomJoinedEvent -= OnRoomJoined;
        // ... etc
    }
}

// Event handlers
void OnRoomJoined(Photon.Realtime.Room room)
{
    Debug.Log($"Joined room: {room.Name}");
}

void OnChatMessage(ChatSystem.ChatMessage message)
{
    Debug.Log($"{message.senderName}: {message.message}");
}

void OnPartyInvite(string senderName)
{
    Debug.Log($"{senderName} invited you to party!");
}

void OnDamageReceived(int damage, int attackerId)
{
    Debug.Log($"Took {damage} damage from {attackerId}");
}
```

## 🔍 Debugging

### Check Connection State

```csharp
void Update()
{
    if (Input.GetKeyDown(KeyCode.F1))
    {
        Debug.Log($"Connected: {PhotonNetwork.IsConnected}");
        Debug.Log($"In Lobby: {PhotonNetwork.InLobby}");
        Debug.Log($"In Room: {PhotonNetwork.InRoom}");
        Debug.Log($"Is Master Client: {PhotonNetwork.IsMasterClient}");
        Debug.Log($"Player Count: {PhotonNetwork.CountOfPlayers}");
        Debug.Log($"Ping: {PhotonNetwork.GetPing()}ms");
        
        if (NetworkManager.Instance != null)
        {
            Debug.Log($"Online Players: {NetworkManager.Instance.GetOnlinePlayerCount()}");
            Debug.Log($"Room Count: {NetworkManager.Instance.GetRoomCount()}");
        }
    }
}
```

### Enable Photon Logs

```csharp
void Awake()
{
    PhotonNetwork.LogLevel = PunLogLevel.Full; // Show all logs
    // PhotonNetwork.LogLevel = PunLogLevel.Informational; // Normal logs
    // PhotonNetwork.LogLevel = PunLogLevel.ErrorsOnly; // Errors only
}
```

## 🎨 UI Integration Example

```csharp
public class GameMenuController : MonoBehaviour
{
    void Start()
    {
        // Subscribe to network events for UI updates
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.RoomJoinedEvent += ShowRoomUI;
            RoomManager.Instance.RoomLeftEvent += ShowLobbyUI;
        }
    }
    
    void ShowLobbyUI()
    {
        // Activate lobby panel
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);
    }
    
    void ShowRoomUI(Photon.Realtime.Room room)
    {
        // Activate room panel
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
    }
    
    public void OnCreateRoomButton()
    {
        string roomName = roomNameInput.text;
        RoomManager.Instance.CreateRoom(roomName, "MainMap", 1, false);
    }
    
    public void OnQuickJoinButton()
    {
        RoomManager.Instance.JoinRandomRoom();
    }
}
```

## 📝 Best Practices

### 1. Always Check PhotonView.IsMine
```csharp
void Update()
{
    PhotonView pv = GetComponent<PhotonView>();
    if (pv.IsMine)
    {
        // Only control your own player
        HandleInput();
    }
}
```

### 2. Master Client for AI
```csharp
void Update()
{
    if (!PhotonNetwork.IsMasterClient) return;
    
    // Only Master Client runs AI logic
    RunEnemyAI();
}
```

### 3. Use RPCs for Important Events
```csharp
// For critical actions that must reach all clients
photonView.RPC("RPC_ImportantAction", RpcTarget.All, data);

[PunRPC]
void RPC_ImportantAction(object data)
{
    // Handle on all clients
}
```

### 4. Use OnPhotonSerializeView for Continuous Data
```csharp
// For data that changes frequently (position, rotation)
public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(transform.position);
    }
    else
    {
        networkPosition = (Vector3)stream.ReceiveNext();
    }
}
```

## 🆘 Common Issues

### "Player not spawning"
- Check player prefab is in Resources folder
- Verify PhotonView component exists
- Ensure PhotonNetwork.IsConnectedAndReady returns true

### "Other players not visible"
- Check PhotonView is in the same room
- Verify PhotonTransformView is configured
- Check if prefab is instantiated correctly

### "Chat not working"
- Ensure PhotonNetwork.IsConnected
- Check if ChatSystem.Instance exists
- Verify you're in a room for room chat

### "Combat not syncing"
- Check CombatNetworkSync has PhotonView
- Verify RPC methods have [PunRPC] attribute
- Ensure ViewIDs are correct

## 📚 Additional Resources

- [README_MULTIPLAYER.md](README_MULTIPLAYER.md) - Full setup guide
- [Photon PUN 2 Docs](https://doc.photonengine.com/pun/v2) - Official documentation
- [Unity Networking Best Practices](https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity6.html)

---

**Happy Coding! 🎮**
