# 👑 Dark Legend - Guild System Documentation

## Overview
Complete Guild System implementation inspired by MU Online, featuring guild creation, ranks, wars, alliances, and comprehensive social features.

## 📁 Structure

```
Assets/Scripts/
├── Guild/
│   ├── Core/                  # Core guild functionality
│   │   ├── GuildManager.cs    # Central guild management
│   │   ├── Guild.cs           # Main guild class
│   │   ├── GuildData.cs       # Configuration ScriptableObject
│   │   ├── GuildMember.cs     # Member information
│   │   └── GuildRank.cs       # Rank system & permissions
│   │
│   ├── Features/              # Guild features
│   │   ├── GuildCreation.cs   # Guild creation system
│   │   ├── GuildInvite.cs     # Member invitation
│   │   ├── GuildBank.cs       # Shared bank system
│   │   ├── GuildShop.cs       # Guild shop
│   │   ├── GuildBuff.cs       # Guild buffs
│   │   ├── GuildQuest.cs      # Guild quests
│   │   ├── GuildLevel.cs      # Leveling system
│   │   └── GuildContribution.cs # Contribution tracking
│   │
│   ├── War/                   # Guild war systems
│   │   ├── GuildWar.cs        # Core war mechanics
│   │   ├── GuildWarManager.cs # War coordination
│   │   ├── GuildWarReward.cs  # War rewards
│   │   └── CastleSiege.cs     # Castle siege events
│   │
│   ├── Alliance/              # Alliance systems
│   │   ├── GuildAlliance.cs   # Alliance structure
│   │   └── AllianceManager.cs # Alliance management
│   │
│   ├── Chat/                  # Communication
│   │   ├── GuildChat.cs       # Guild chat
│   │   ├── GuildAnnouncement.cs # Announcements
│   │   └── GuildNotification.cs # Notifications
│   │
│   └── Data/                  # Data persistence
│       └── GuildDataPersistence.cs # Save/load system
│
└── Party/                     # Enhanced party system
    ├── Party.cs              # Party structure
    ├── PartyManager.cs       # Party management
    └── PartyFinder.cs        # Party finder & auto-match
```

## 🎮 Key Features

### Guild Ranks (6 Levels)
1. **Guild Master** - Full control
2. **Vice Master** (Max 3) - Management & bank access
3. **Battle Master** (Max 5) - War team management
4. **Senior Member** - Limited bank access
5. **Member** - Basic features
6. **Newbie** - 7-day probation

### Guild Level System
- **Levels**: 1-50
- **Max Members**: 20-120 (scales with level)
- **Bank Slots**: 50-300 (scales with level)
- **EXP Bonus**: 1%-50% (scales with level)
- **Buff Slots**: 0-5 (every 10 levels)

### Guild Features

#### Guild Bank
- Shared storage for items and Zen
- Permission-based access control
- Daily withdrawal limits per rank
- Complete transaction logging

#### Guild Buffs
- EXP Boost: +10-50%
- Attack Boost: +5-20%
- Defense Boost: +5-20%
- Drop Rate: +5-30%
- HP/MP Boost: +10-30%

#### Guild Quests
- **Daily Quests**: Reset every 24 hours
- **Weekly Quests**: Reset weekly
- **Special Quests**: Limited time events
- Guild-wide progression tracking

### Guild War System

#### War Types
- **Skirmish**: 5v5, 15 minutes
- **Battle**: 10v10, 30 minutes
- **Full War**: All members, 1 hour
- **Territory**: Land control, 90 minutes

#### Scoring
- Regular kill: +1 point
- Vice Master kill: +3 points
- Guild Master kill: +5 points

#### Castle Siege
- Weekly event (configurable day/time)
- Multiple guilds can attack
- Objectives: Destroy gates, capture throne
- Benefits: 10% tax revenue, special buffs, exclusive items

### Alliance System
- Max 5 guilds per alliance
- Shared chat channel
- Alliance wars
- Cannot attack allied guilds

### Party System

#### Features
- Max 5 members per party
- EXP sharing with 10% bonus
- Loot distribution modes:
  - Free For All
  - Round Robin
  - Leader decides
  - Random
  - Need Before Greed

#### Party Finder
- Search by activity, level, class
- Auto-match system
- Looking For Party (LFP) registration
- Public party listing

## 🔧 Setup

### 1. Create Guild Data Asset
```
Right-click in Project → Create → DarkLegend → Guild → Guild Data
```

Configure:
- Required level/zen for creation
- Guild name length limits
- Level system parameters
- War settings
- Alliance limits

### 2. Add Managers to Scene
```csharp
// Create empty GameObject "GameManagers"
// Add these components:
- GuildManager
- GuildCreation
- GuildInvite
- GuildBank
- GuildBuff
- GuildQuest
- GuildLevel
- GuildContribution
- GuildWarManager
- GuildWar
- GuildWarReward
- CastleSiege
- AllianceManager
- GuildChat
- GuildAnnouncement
- GuildNotification
- GuildDataPersistence
- PartyManager
- PartyFinder
```

### 3. Assign References
Link the GuildData ScriptableObject to GuildManager and other components that need it.

## 💻 Usage Examples

### Create a Guild
```csharp
using DarkLegend.Guild;

GuildManager guildManager = GuildManager.Instance;
GuildCreation guildCreation = FindObjectOfType<GuildCreation>();

var request = new GuildCreation.GuildCreationRequest
{
    PlayerId = "player123",
    PlayerName = "PlayerName",
    PlayerLevel = 100,
    PlayerZen = 1000000,
    CharacterClass = "DarkKnight",
    GuildName = "DarkLegends",
    GuildType = GuildType.Mixed,
    Description = "Best guild ever!"
};

Guild newGuild = guildCreation.CreateGuild(request, out string error);
if (newGuild != null)
{
    Debug.Log($"Guild created: {newGuild.GuildName}");
}
```

### Invite Player to Guild
```csharp
GuildInvite guildInvite = FindObjectOfType<GuildInvite>();

bool invited = guildInvite.SendInvitation(
    guildId: "guild123",
    inviterId: "player123",
    targetPlayerId: "player456",
    targetPlayerName: "NewPlayer"
);
```

### Activate Guild Buff
```csharp
GuildBuff guildBuff = FindObjectOfType<GuildBuff>();

bool activated = guildBuff.ActivateBuff(
    guildId: "guild123",
    playerId: "player123",
    buffId: "exp_boost_2"
);
```

### Declare Guild War
```csharp
GuildWarManager warManager = FindObjectOfType<GuildWarManager>();

var declaration = warManager.DeclareWar(
    attackingGuildId: "guild123",
    defendingGuildId: "guild456",
    declarerId: "player123",
    type: GuildWar.GuildWarType.Battle,
    scheduledTime: DateTime.Now.AddHours(2),
    betAmount: 1000
);
```

### Create Party
```csharp
using DarkLegend.Party;

PartyManager partyManager = PartyManager.Instance;

Party party = partyManager.CreateParty(
    leaderId: "player123",
    partyName: "Dungeon Crawlers"
);

party.Settings.PreferredActivity = "Dungeon";
party.Settings.MinLevel = 50;
party.LootMode = LootMode.RoundRobin;
```

### Use Party Finder
```csharp
PartyFinder partyFinder = FindObjectOfType<PartyFinder>();

// Register looking for party
var lfpSettings = new PartyFinder.LFPSettings
{
    PreferredActivity = "Boss",
    MinLevel = 80,
    MaxLevel = 120,
    AutoJoin = true
};

partyFinder.RegisterLFP(
    playerId: "player123",
    playerName: "PlayerName",
    level: 100,
    characterClass: "DarkWizard",
    settings: lfpSettings
);
```

## 📊 Events

The system provides various events you can subscribe to:

```csharp
GuildManager.OnGuildCreated += (guild) => { /* handle */ };
GuildManager.OnMemberJoined += (guild, member) => { /* handle */ };
GuildManager.OnGuildLevelUp += (guild, level) => { /* handle */ };

GuildNotification notification = FindObjectOfType<GuildNotification>();
notification.OnNotificationReceived += (notif) => { /* handle */ };
```

## 💾 Data Persistence

### Auto-Save
```csharp
GuildDataPersistence persistence = FindObjectOfType<GuildDataPersistence>();
persistence.autoSave = true;
persistence.autoSaveInterval = 300f; // 5 minutes
```

### Manual Save/Load
```csharp
// Save all guilds
persistence.SaveAllGuilds();

// Load all guilds
persistence.LoadAllGuilds();

// Save specific guild
Guild guild = guildManager.GetGuild("guild123");
persistence.SaveGuild(guild);
```

## 🔐 Permissions System

Each rank has specific permissions defined in `GuildRankPermissions`:
- Member management (invite, kick, promote, demote)
- Bank access (deposit, withdraw, limits)
- Guild war participation
- Settings modification
- Buff activation
- Quest management

## 🌐 Multiplayer Integration

The system is designed for Photon PUN 2 integration:
- All state changes should be synchronized
- Use PhotonView for networked operations
- Implement RPC calls for guild actions
- Sync guild data across clients

## ⚙️ Customization

### Modify Guild Data
Edit the `GuildData` ScriptableObject to adjust:
- Creation requirements
- Level progression
- Member limits
- Bank capacity
- Buff configurations
- War point values

### Add Custom Features
The system is modular - extend functionality by:
1. Creating new classes in appropriate directories
2. Adding references to managers
3. Implementing additional events
4. Extending save data structures

## 📝 Notes

- All times use `DateTime` for consistency
- Guild IDs use GUIDs for uniqueness
- Comments are bilingual (English/Vietnamese)
- Follows Unity best practices
- Ready for multiplayer integration
- Extensible architecture

## 🎯 Future Enhancements

Potential additions:
- Guild housing/bases
- Territory control maps
- Guild achievements
- Alliance wars
- Guild ranking system
- Voice chat integration
- Mobile UI adaptation

## 📄 License

MIT License - See repository LICENSE file
