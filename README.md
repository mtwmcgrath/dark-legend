# 🗡️ Dark Legend

A 3D RPG Game inspired by MU Online - Built with Unity

## Features

### ✅ Implemented
- 👑 **Complete Guild System** (MU Online Style)
  - Guild creation with requirements (Level 100, 1M Zen)
  - 6-tier rank system (Guild Master → Newbie)
  - Guild levels 1-50 with scaling bonuses
  - Guild Bank with permissions & transaction logs
  - 12 different guild buffs (EXP, Attack, Defense, etc.)
  - Daily/Weekly/Special quest system
  - Contribution tracking & rankings
  
- ⚔️ **Guild War System**
  - 4 war types: Skirmish (5v5), Battle (10v10), Full War, Territory
  - War declaration, acceptance, and scheduling
  - Kill scoring system with rank bonuses
  - War rewards and MVP system
  - Weekly Castle Siege events
  
- 🤝 **Alliance System**
  - Max 5 guilds per alliance
  - Alliance management (create, invite, join, leave)
  - Shared alliance features
  
- 💬 **Communication Systems**
  - Guild chat with message history
  - Guild announcements (priority levels)
  - Real-time notifications
  
- 👥 **Enhanced Party System**
  - Max 5 members with EXP sharing (10% bonus)
  - 5 loot distribution modes
  - Party Finder with auto-match
  - Looking For Party (LFP) registration
  
- 💾 **Data Persistence**
  - JSON-based save/load system
  - Auto-save functionality
  - Complete guild state preservation

### 🚧 Coming Soon
- ⚔️ 3 Character Classes: Dark Knight, Dark Wizard, Elf
- 📊 Stats System: STR, AGI, VIT, ENE
- 🎯 Skill System with cooldowns
- 👹 Monster AI & Combat
- 🎒 Inventory & Equipment System
- 🖥️ PC Controls (Keyboard + Mouse)
- 🎨 Guild & Party UI Components

## 📁 Project Structure

```
Assets/Scripts/
├── Guild/                     # Complete Guild System
│   ├── Core/                 # Core functionality (5 files)
│   ├── Features/             # Guild features (8 files)
│   ├── War/                  # War systems (4 files)
│   ├── Alliance/             # Alliance systems (2 files)
│   ├── Chat/                 # Communication (3 files)
│   └── Data/                 # Persistence (1 file)
└── Party/                     # Enhanced Party System (3 files)
```

**Total: 26 C# files + comprehensive documentation**

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or higher
- Basic knowledge of C# and Unity

### Setup
1. Clone the repository
2. Open project in Unity
3. See `Assets/Scripts/Guild/README.md` for detailed Guild System documentation
4. Create GuildData ScriptableObject: `Right-click → Create → DarkLegend → Guild → Guild Data`
5. Add manager components to a GameObject in your scene

### Quick Example
```csharp
using DarkLegend.Guild;

// Create a guild
GuildManager guildManager = GuildManager.Instance;
Guild guild = guildManager.CreateGuild("MyGuild", playerId, GuildType.Mixed);

// Activate guild buff
GuildBuff guildBuff = FindObjectOfType<GuildBuff>();
guildBuff.ActivateBuff(guild.GuildId, playerId, "exp_boost_2");
```

## 📚 Documentation

- **Guild System**: See `Assets/Scripts/Guild/README.md`
- **API Reference**: All classes include XML documentation
- **Examples**: Check README for usage examples

## 🎮 Features Highlights

### Guild System
- **Dynamic Scaling**: Guild grows from 20 to 120 members
- **Smart Permissions**: 6 ranks with customizable permissions
- **Bank System**: Shared storage with withdrawal limits
- **Buff System**: 12 buffs with duration and effects
- **Quest System**: Daily, weekly, and special quests
- **War System**: Multiple war types with scoring
- **Castle Siege**: Weekly events for castle control

### Party System
- **EXP Sharing**: 10% bonus when in party
- **Loot Modes**: 5 different distribution systems
- **Party Finder**: Auto-match and search
- **Level Balancing**: EXP penalty for level differences

## 🔧 Tech Stack
- Unity 2022.3 LTS
- C# (.NET Standard 2.1)
- Modular architecture
- Event-driven design
- ScriptableObject configuration
- JSON serialization
- Ready for Photon PUN 2 integration

## 🌟 Key Design Principles
- **Modular**: Each system is independent and reusable
- **Extensible**: Easy to add new features
- **Scalable**: Designed for multiplayer
- **Documented**: Bilingual comments (EN/VN)
- **Clean Code**: Follows Unity best practices

## 🤝 Contributing
Contributions are welcome! Feel free to:
- Report bugs
- Suggest features
- Submit pull requests

## 📄 License
MIT License - See LICENSE file for details

## 🎯 Roadmap
- [ ] UI implementation for all systems
- [ ] Photon PUN 2 multiplayer integration
- [ ] Character classes and combat
- [ ] Inventory and equipment
- [ ] Monster AI
- [ ] Map and world design
- [ ] Audio and VFX

## 📞 Contact
For questions or support, please open an issue on GitHub.

---

**Status**: Guild System Complete ✅ | Ready for UI & Multiplayer Integration