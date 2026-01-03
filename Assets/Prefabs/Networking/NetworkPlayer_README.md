# Network Player Prefab Configuration

## NetworkPlayer.prefab

This document describes how to create the NetworkPlayer prefab in Unity Editor.

### Required Components

#### 1. GameObject Structure
```
NetworkPlayer (root)
├── PlayerModel (3D model/sprite)
├── Camera (Main Camera)
├── UI
│   └── NameTag
│       ├── Canvas
│       ├── NameText (TextMeshPro)
│       └── HealthBar (Image)
└── Audio
    └── AudioListener
```

### 2. Required Scripts/Components on Root

#### PhotonView
- **View ID**: 0 (auto-assigned)
- **Ownership**: Takeover
- **Observed Components**:
  - PlayerNetworkSync (for IPunObservable)
  - PhotonTransformView (for Transform sync)

#### PhotonTransformView
- **Synchronize Position**: ✓
- **Synchronize Rotation**: ✓
- **Synchronize Scale**: ✗
- **Use Local Values**: ✓

#### PhotonAnimatorView (if using Animator)
- **Synchronize Parameters**: ✓
- **Synchronize Layers**: ✓

#### PlayerNetworkSync
- **Sync Position**: ✓
- **Sync Rotation**: ✓
- **Sync Animation**: ✓
- **Sync Stats**: ✓
- **Position Lerp Speed**: 10
- **Rotation Lerp Speed**: 10
- **Enable Lag Compensation**: ✓

#### CombatNetworkSync
- **Attack Cooldown**: 1.0
- **Skill Cooldown**: 2.0

#### PlayerPhotonView
- **Network Sync**: (reference)
- **Combat Sync**: (reference)
- **Name Tag**: (reference)
- **Player Camera**: (reference)
- **Audio Listener**: (reference)

#### Character Controller / Rigidbody
- Use your preferred movement controller
- For network play, consider using Character Controller for better control

### 3. NameTag Setup

#### Canvas (on NameTag GameObject)
- **Render Mode**: World Space
- **Dynamic Pixels Per Unit**: 10
- **Sort Order**: 10

#### NameText (TextMeshProUGUI)
- **Font Size**: 24
- **Alignment**: Center
- **Color**: White
- **Auto Size**: ✓

#### HealthBar (UI Image)
- **Image Type**: Filled
- **Fill Method**: Horizontal
- **Fill Origin**: Left
- **Color**: Green

#### PlayerNameTag Script
- **Name Text**: (reference)
- **Health Bar**: (reference)
- **Offset**: (0, 2.5, 0)
- **Show Health Bar**: ✓
- **Max Visible Distance**: 50

### 4. Camera Setup
- **Clear Flags**: Skybox
- **Culling Mask**: Everything
- **Depth**: 0
- **Field of View**: 60
- **Clipping Planes**: Near 0.3, Far 1000

> **Note**: Camera and AudioListener will be automatically disabled for remote players by PlayerPhotonView script

### 5. Tags and Layers
- **Tag**: Player
- **Layer**: Player (create if doesn't exist)

### 6. Prefab Location
- Must be placed in: `Assets/Resources/NetworkPlayer.prefab`
- This allows Photon to find and instantiate it via `PhotonNetwork.Instantiate()`

### 7. PhotonServerSettings Configuration
After creating the prefab:
1. Open `PhotonServerSettings` (usually in `Assets/Photon/PhotonUnityNetworking/Resources/`)
2. Go to **PUN** section
3. Add `NetworkPlayer` to **Prefab List**

### 8. Testing the Prefab
```csharp
// Test spawn code
void SpawnPlayer()
{
    Vector3 spawnPos = Vector3.zero;
    Quaternion spawnRot = Quaternion.identity;
    GameObject player = PhotonNetwork.Instantiate("NetworkPlayer", spawnPos, spawnRot);
}
```

### 9. Optional Components

#### Animator
- **Controller**: Your character animation controller
- **Avatar**: Your character avatar
- **Apply Root Motion**: ✓ (if using root motion)

#### Audio Source
- For footsteps, attacks, etc.
- **Spatial Blend**: 3D
- **Min Distance**: 1
- **Max Distance**: 50

#### Collider
- **Capsule Collider** (recommended for players)
- **Radius**: 0.5
- **Height**: 2
- **Center**: (0, 1, 0)

### 10. Advanced Settings

#### LOD Group (optional for performance)
- LOD 0: Full detail (0-15m)
- LOD 1: Medium detail (15-30m)
- LOD 2: Low detail (30-50m)

#### Occlusion Culling
Enable if using large scenes with many players

### 11. Character Classes Support

If supporting multiple character classes (Dark Knight, Dark Wizard, Elf):

**Option A**: Multiple Prefabs
- Create `NetworkPlayer_DarkKnight.prefab`
- Create `NetworkPlayer_DarkWizard.prefab`
- Create `NetworkPlayer_Elf.prefab`

**Option B**: Single Prefab with Runtime Changes
- Use single `NetworkPlayer.prefab`
- Change model/materials at runtime based on player properties
- More flexible but requires more setup code

### 12. Network Optimization

For better performance:
- **PhotonView Observation Option**: Unreliable On Change (for transform)
- **Send Rate**: 20 (default) or lower for less important objects
- **Serialize Rate**: 10 (default) or adjust based on needs

### Checklist
- [ ] All required components added
- [ ] PhotonView configured with observed components
- [ ] Scripts attached and configured
- [ ] NameTag UI setup correctly
- [ ] Camera and AudioListener added
- [ ] Placed in Assets/Resources/ folder
- [ ] Added to PhotonServerSettings prefab list
- [ ] Tested spawning in network game
- [ ] Verified sync between clients
- [ ] Checked that remote players' cameras are disabled

---

**Next**: Create NetworkEnemy.prefab using similar steps
