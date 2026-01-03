# 🗡️ Dark Legend

A 3D MMORPG Game inspired by MU Online - Built with Unity & Photon PUN 2

## 🌟 Features

### 🎮 Core Gameplay
- ⚔️ 3 Character Classes: Dark Knight, Dark Wizard, Elf
- 📊 Stats System: STR, AGI, VIT, ENE
- 🎯 Skill System with cooldowns
- 👹 Monster AI & Combat
- 🎒 Inventory & Equipment System
- 🖥️ PC Controls (Keyboard + Mouse)

### 🌐 Multiplayer Online System (NEW!)
- 🔌 **Photon PUN 2 Integration** - Real-time networking for up to 20 CCU (free tier)
- 👥 **Room System** - Create/join rooms with custom settings (map, difficulty, PvP)
- 🎯 **Player Synchronization** - Position, rotation, animation, stats
- ⚔️ **Combat Sync** - Damage, skills, buffs/debuffs across clients
- 👾 **Enemy Sync** - Master Client controlled AI with client interpolation
- 💬 **Chat System** - Global, room, party, and whisper chat
- 👫 **Party System** - Create parties (max 4), invite/kick, EXP sharing
- 🛡️ **PvP System** - Duel requests, PvP zones, PK tracking
- 🏷️ **Name Tags** - Display player names and health bars above heads
- 📡 **Lag Compensation** - Smooth movement with interpolation and extrapolation
- 🔄 **Auto-Reconnect** - Automatic reconnection on network issues

### 🎮 Game Modes
- **Co-op PvE** - Team up to fight monsters and bosses
- **PvP Battles** - Free PvP zones and organized duels
- **Party Dungeons** - Instanced dungeons requiring teamwork
- **Arena Battles** - Competitive PvP combat

## 📋 Tech Stack
- Unity 2022.3 LTS
- C# (.NET Standard 2.1)
- Photon PUN 2 (Networking)
- TextMeshPro (UI)

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or later
- Photon PUN 2 (free or paid)
- Internet connection for multiplayer

### Installation
1. Clone this repository
   ```bash
   git clone https://github.com/mtwmcgrath/dark-legend.git
   ```

2. Open project in Unity 2022.3 LTS or later

3. Install Photon PUN 2 from Unity Asset Store

4. Configure Photon settings (see [Multiplayer Setup](#multiplayer-setup))

### Multiplayer Setup
For detailed multiplayer setup instructions, see [README_MULTIPLAYER.md](README_MULTIPLAYER.md)

Quick steps:
1. Create free Photon account at [photonengine.com](https://www.photonengine.com/)
2. Create new PUN app and copy App ID
3. In Unity: Window > Photon Unity Networking > PUN Wizard
4. Paste App ID and setup project
5. Configure region (asia for Vietnam/Asia)

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Networking/           # Multiplayer networking scripts
│   │   ├── NetworkManager.cs
│   │   ├── PhotonLauncher.cs
│   │   ├── RoomManager.cs
│   │   ├── PlayerNetworkSync.cs
│   │   ├── CombatNetworkSync.cs
│   │   ├── EnemyNetworkSync.cs
│   │   ├── ChatSystem.cs
│   │   ├── PartySystem.cs
│   │   ├── PvPSystem.cs
│   │   ├── PlayerNameTag.cs
│   │   ├── NetworkPrefabs.cs
│   │   └── NetworkUI/        # Networking UI scripts
│   │       ├── LoginUI.cs
│   │       ├── LobbyUI.cs
│   │       ├── RoomUI.cs
│   │       ├── ChatUI.cs
│   │       └── PartyUI.cs
│   └── Player/               # Player-specific scripts
│       ├── PlayerPhotonView.cs
│       └── PlayerSpawnManager.cs
├── Prefabs/
│   └── Networking/           # Network prefabs
│       ├── NetworkPlayer_README.md
│       └── NetworkEnemy_README.md
└── Resources/                # Photon instantiable prefabs
    ├── NetworkPlayer.prefab
    └── NetworkEnemy.prefab
```

## 🎯 Usage

### Testing Locally
1. Build the game (File > Build Settings > Build)
2. Run the executable
3. Open Unity Editor in Play mode
4. Both instances will connect and you can test multiplayer

### Chat Commands
- `/g <message>` or `/global <message>` - Send to global chat
- `/r <message>` or `/room <message>` - Send to room chat
- `/p <message>` or `/party <message>` - Send to party chat
- `/w <player> <message>` or `/whisper <player> <message>` - Send private message
- `/help` - Show help

## 🛠️ Development

### Namespace Convention
All networking scripts use the `DarkLegend.Networking` namespace.

### Code Style
- Comments in both Vietnamese and English
- XML documentation for public methods
- Error handling for network issues
- Graceful disconnection handling

## 🐛 Troubleshooting

### Common Issues
- **Can't connect to Photon**: Check App ID in PhotonServerSettings
- **Players not syncing**: Verify PhotonView components are properly configured
- **High lag**: Switch to closer region or check network connection

See [README_MULTIPLAYER.md](README_MULTIPLAYER.md) for detailed troubleshooting.

## 📝 Documentation
- [Multiplayer Setup Guide](README_MULTIPLAYER.md) - Complete guide for Photon PUN 2 setup
- [Network Player Prefab](Assets/Prefabs/Networking/NetworkPlayer_README.md) - Player prefab configuration
- [Network Enemy Prefab](Assets/Prefabs/Networking/NetworkEnemy_README.md) - Enemy prefab configuration

## 🤝 Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License
MIT

## 🙏 Acknowledgments
- Inspired by MU Online
- Built with Unity Engine
- Networking powered by Photon PUN 2

---

**Made with ❤️ for MMORPG fans**