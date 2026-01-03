# 🌐 Dark Legend - Multiplayer Setup Guide

## Overview
Dark Legend multiplayer system is powered by **Photon PUN 2** (Photon Unity Networking 2), providing real-time networking capabilities for up to 20 concurrent users on the free tier.

## 📋 Table of Contents
- [Prerequisites](#prerequisites)
- [Photon Account Setup](#photon-account-setup)
- [Unity Configuration](#unity-configuration)
- [Network Architecture](#network-architecture)
- [Features](#features)
- [Testing](#testing)
- [Troubleshooting](#troubleshooting)

## Prerequisites

### Software Requirements
- Unity 2022.3 LTS or later
- Photon PUN 2 (Free or Plus)
- Internet connection

### Unity Packages Required
1. **Photon PUN 2** - Install from Unity Asset Store or Package Manager
2. **TextMeshPro** - For UI text rendering

## Photon Account Setup

### Step 1: Create Photon Account
1. Go to [https://www.photonengine.com/](https://www.photonengine.com/)
2. Click **Sign Up** and create a free account
3. Verify your email address

### Step 2: Create New Photon Application
1. Log in to [Photon Dashboard](https://dashboard.photonengine.com/)
2. Click **Create a New App**
3. Fill in the details:
   - **Photon Type**: `Photon PUN`
   - **Name**: `Dark Legend` (or your preferred name)
   - **Description**: Optional
4. Click **Create**
5. Copy your **App ID** (you'll need this for Unity configuration)

### Step 3: Choose Your Region
For best performance, select the region closest to your players:
- **asia** - Asia (Singapore, Tokyo, Seoul)
- **us** - United States
- **eu** - Europe
- **jp** - Japan
- **au** - Australia

## Unity Configuration

### Step 1: Import Photon PUN 2
1. Open **Unity Package Manager** (Window > Package Manager)
2. Search for **"Photon PUN 2"** in Asset Store
3. Download and import into your project
4. Or install via Asset Store: [Photon PUN 2](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922)

### Step 2: Configure Photon Settings
1. In Unity, go to **Window > Photon Unity Networking > PUN Wizard**
2. Click **Setup Project**
3. Paste your **App ID** from Photon Dashboard
4. Click **Setup Project**

### Step 3: Configure PhotonServerSettings
1. Open **Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings**
2. Verify settings:
   ```
   App Id PUN: [YOUR_APP_ID]
   Fixed Region: asia (or your preferred region)
   Protocol: UDP
   ```

### Step 4: Setup Network Prefabs
1. Create your player prefab with required components:
   - `PhotonView`
   - `PhotonTransformView`
   - `PlayerNetworkSync`
   - `CombatNetworkSync`
   - `PlayerPhotonView`
2. Place the prefab in `Assets/Resources/` folder
3. Add prefab to **Photon Server Settings > PUN > Prefab List**

## Network Architecture

### Component Overview

```
NetworkManager.cs          → Connection management, auto-reconnect
PhotonLauncher.cs          → Login, connect, join lobby
RoomManager.cs             → Room creation, joining, properties
PlayerNetworkSync.cs       → Player position, rotation, stats sync
CombatNetworkSync.cs       → Combat, damage, skills sync
EnemyNetworkSync.cs        → Enemy AI sync (Master Client only)
ChatSystem.cs              → Global, room, party, whisper chat
PartySystem.cs             → Party management, EXP sharing
PvPSystem.cs               → PvP, duel, PK system
PlayerNameTag.cs           → Name display above players
```

### Network Flow

```
Player Opens Game
    ↓
LoginUI → Connect to Photon
    ↓
PhotonLauncher → Connect to Master Server
    ↓
Join Lobby
    ↓
LobbyUI → Show available rooms
    ↓
Create/Join Room
    ↓
RoomUI → Wait for players
    ↓
Start Game → Load game scene
    ↓
PlayerSpawnManager → Spawn network players
    ↓
Game Running (with sync)
```

## Features

### 🎮 Core Networking
- ✅ Player synchronization (position, rotation, animation)
- ✅ Combat synchronization (damage, skills, buffs)
- ✅ Enemy synchronization (Master Client authority)
- ✅ Lag compensation and interpolation
- ✅ Auto-reconnect on disconnection

### 💬 Social Systems
- ✅ **Chat System**
  - Global chat (all players)
  - Room chat (players in same room)
  - Party chat (party members only)
  - Whisper (private messages)
  - Chat commands: `/g`, `/r`, `/p`, `/w`

- ✅ **Party System**
  - Create/join party (max 4 players)
  - Party leader management
  - Invite/kick members
  - EXP sharing with bonus
  - Party chat

- ✅ **PvP System**
  - Toggle PvP mode
  - Duel requests (1v1)
  - PvP zones
  - PK (Player Kill) tracking
  - PK penalties

### 🎯 Game Modes
1. **Co-op PvE** - Fight monsters together
2. **PvP** - Player vs Player combat
3. **Duel** - Organized 1v1 matches
4. **Party Dungeons** - Instanced group content

## Testing

### Local Testing (Single Machine)
1. Build your game (File > Build Settings > Build)
2. Run the built executable
3. Open Unity Editor (Play mode)
4. Both instances will connect to the same Photon server

### Multi-Machine Testing
1. Build the game for your target platform
2. Copy the build to different computers
3. Run on each machine
4. Players will see each other in the lobby

### Testing Checklist
- [ ] Connection to Photon succeeds
- [ ] Can create and join rooms
- [ ] Players spawn correctly
- [ ] Movement syncs between clients
- [ ] Combat damage syncs
- [ ] Chat messages work
- [ ] Party invites work
- [ ] Duel requests work

## Troubleshooting

### Common Issues

#### "Cannot connect to Photon"
**Solutions:**
1. Check your internet connection
2. Verify App ID in PhotonServerSettings
3. Check Photon server status: [https://status.photonengine.com/](https://status.photonengine.com/)
4. Try different region in settings

#### "Player prefab not found"
**Solutions:**
1. Ensure prefab is in `Assets/Resources/` folder
2. Check prefab name matches in code: `PhotonNetwork.Instantiate("NetworkPlayer", ...)`
3. Add prefab to Photon Server Settings prefab list

#### "Players not syncing"
**Solutions:**
1. Check `PhotonView` component is on player prefab
2. Verify `PhotonTransformView` is configured correctly
3. Check `PlayerNetworkSync` is attached and enabled
4. Ensure `OnPhotonSerializeView` is implemented

#### "High latency/lag"
**Solutions:**
1. Switch to closer region (asia for Vietnam)
2. Check network connection quality
3. Reduce send rate in PhotonServerSettings
4. Enable lag compensation in PlayerNetworkSync

#### "Room doesn't appear in lobby"
**Solutions:**
1. Ensure room is created with `IsVisible = true`
2. Check `MaxPlayers > 0`
3. Verify in lobby before creating room
4. Room may be full or closed

### Debug Tips

#### Enable Photon Logs
```csharp
PhotonNetwork.LogLevel = PunLogLevel.Full;
```

#### Check Connection State
```csharp
Debug.Log($"Connected: {PhotonNetwork.IsConnected}");
Debug.Log($"In Lobby: {PhotonNetwork.InLobby}");
Debug.Log($"In Room: {PhotonNetwork.InRoom}");
Debug.Log($"Ping: {PhotonNetwork.GetPing()}ms");
```

#### Monitor Network Traffic
- Open **Window > Photon Unity Networking > PUN Wizard > Stats**
- View real-time network statistics

## Performance Tips

### Optimization
1. **Reduce Send Rate**: Lower `SendRate` in PhotonView (default: 20)
2. **Use Unreliable**: For position updates, use `SendOptions.SendUnreliable`
3. **Compress Data**: Send only changed values
4. **Limit Updates**: Don't send updates every frame
5. **Use RPCs Wisely**: Only for important events

### Best Practices
- Master Client handles AI and game logic
- Clients only send player input
- Validate important actions server-side
- Use object pooling for network objects
- Limit number of PhotonViews per scene

## Photon Free Tier Limits
- **20 CCU** (Concurrent Users)
- **Unlimited** monthly active users
- **10GB** monthly traffic
- **All regions** available

To upgrade: [Photon Pricing](https://www.photonengine.com/en-US/PUN/Pricing)

## Support & Resources

### Official Resources
- [Photon PUN 2 Documentation](https://doc.photonengine.com/pun/v2/getting-started/pun-intro)
- [Photon Forum](https://forum.photonengine.com/)
- [Unity Networking Manual](https://docs.unity3d.com/Manual/UNet.html)

### Dark Legend Resources
- GitHub Repository: [mtwmcgrath/dark-legend](https://github.com/mtwmcgrath/dark-legend)
- Issue Tracker: Report bugs and request features

## Next Steps

After setting up multiplayer:
1. ✅ Create player prefabs with PhotonView
2. ✅ Setup spawn points in game scenes
3. ✅ Configure room settings (max players, maps)
4. ✅ Test with friends
5. ✅ Add game-specific features (skills, items, quests)
6. ✅ Optimize network performance
7. ✅ Add anti-cheat measures
8. ✅ Deploy and enjoy!

---

**Happy Gaming! 🎮**

For questions or support, please open an issue on GitHub or contact the development team.
