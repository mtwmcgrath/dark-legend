# ⚔️ Dark Legend - Full PvP System

A complete MU Online-style PvP system for Unity with duels, arenas, battlegrounds, rankings, and tournaments.

## 📁 File Structure

```
Assets/Scripts/PvP/
├── Core/
│   ├── PvPManager.cs              # Main PvP system manager
│   ├── PvPData.cs                 # ScriptableObject for PvP configuration
│   ├── PvPZone.cs                 # PvP zone definition
│   ├── PvPRules.cs                # PvP rules configuration
│   ├── PvPReward.cs               # Reward system
│   └── PvPEnums.cs                # All PvP enumerations
│
├── Duel/
│   ├── DuelSystem.cs              # 1v1 duel system
│   ├── DuelRequest.cs             # Duel request/response handling
│   ├── DuelArena.cs               # Physical duel arena
│   └── DuelUI.cs                  # Duel user interface
│
├── Arena/
│   ├── ArenaManager.cs            # Arena matchmaking manager
│   ├── ArenaMatch.cs              # Arena match logic
│   ├── ArenaQueue.cs              # ELO-based queue system
│   ├── ArenaRanking.cs            # Arena ranking tracking
│   ├── ArenaSeason.cs             # Seasonal system
│   └── ArenaReward.cs             # Arena rewards
│
├── Battleground/
│   ├── BattlegroundManager.cs     # Battleground mode manager
│   ├── BattlegroundMode.cs        # Base class for modes
│   ├── TeamDeathmatch.cs          # Team Deathmatch mode
│   ├── CaptureTheFlag.cs          # CTF mode
│   ├── KingOfTheHill.cs           # KOTH mode
│   └── BattleRoyale.cs            # Battle Royale mode
│
├── OpenWorld/
│   ├── OpenWorldPvP.cs            # Open world PvP manager
│   ├── PKSystem.cs                # Player Kill tracking
│   ├── PKPenalty.cs               # PK penalties
│   ├── BountySystem.cs            # Bounty/wanted system
│   ├── SafeZone.cs                # Safe zones (no PvP)
│   └── PvPToggle.cs               # PvP mode toggle
│
├── Ranking/
│   ├── PvPRankingSystem.cs        # Overall ranking system
│   ├── EloRating.cs               # ELO rating algorithm
│   ├── Leaderboard.cs             # Leaderboard display
│   └── PvPTitle.cs                # PvP titles and achievements
│
├── Tournament/
│   ├── TournamentManager.cs       # Tournament scheduler
│   ├── TournamentBracket.cs       # Bracket generation
│   ├── TournamentMatch.cs         # Tournament matches
│   └── TournamentReward.cs        # Tournament prizes
│
├── UI/
│   ├── PvPUI.cs                   # Main PvP menu
│   ├── DuelUI.cs                  # Duel interface (inherited)
│   ├── ArenaUI.cs                 # Arena interface
│   ├── RankingUI.cs               # Ranking/leaderboard UI
│   ├── BattlegroundUI.cs          # Battleground interface
│   ├── PKStatusUI.cs              # PK status display
│   └── TournamentUI.cs            # Tournament interface
│
└── PvPNetworkSync.cs              # Photon PUN 2 network sync
```

## 🎮 Features

### 1. Duel System (1v1)
- **Request/Accept/Decline** duel challenges
- **Multiple duel types**: Normal, Ranked, Bet, Tournament
- **Custom settings**: Time limit, allow potions/skills, bet amount
- **Safe dueling**: No EXP loss, no item drops
- **Arena teleportation**: Players teleported to duel arena and back

### 2. Arena System
- **Multiple modes**: 1v1, 2v2, 3v3, 5v5, Free-for-All
- **ELO-based matchmaking**: Fair matches based on skill rating
- **Seasonal rankings**: 3-month seasons with rewards
- **Rank tiers**: Bronze → Silver → Gold → Platinum → Diamond → Master → GrandMaster → Challenger
- **Score system**: Kills, assists, first blood, multi-kills, streaks
- **Win conditions**: First to X kills or most kills when time ends

### 3. Battleground Modes

#### Team Deathmatch (10v10)
- First to 100 kills wins
- 15-minute time limit
- Respawn system with 5-second timer

#### Capture The Flag (8v8)
- First to 3 captures wins
- Flag carriers move 30% slower
- Auto-return after 30 seconds

#### King of the Hill (6v6)
- First to 1000 points wins
- 1 point per second while holding hill
- Hill rotates every 2 minutes
- Contested hill awards no points

#### Battle Royale (50 players)
- Last player/team standing wins
- Shrinking circle mechanic
- Random loot spawns
- 30-minute time limit

### 4. Open World PvP

#### PK System
- **PK Status Tracking**: Normal → Self Defense → Murderer → Outlaw → Hero
- **Name Colors**: White → Yellow → Orange → Red → Blue
- **PK Count Decay**: -1 per hour online

#### PK Penalties
- **Murderer (Orange, 1-10 kills)**
  - 5% EXP loss on death
  - 3% item drop chance
  - Can enter towns
  
- **Outlaw (Red, 10+ kills)**
  - 10% EXP loss on death
  - 10% item drop chance
  - Cannot enter towns (guards attack)
  - -10% all stats penalty
  - 5x teleport costs

#### Bounty System
- **Automatic bounties**: 10,000 Zen per PK count for outlaws
- **Manual bounties**: Players can place bounties on others
- **24-hour duration**: Bounties expire after 1 day
- **Claim rewards**: Kill bounty target to collect

#### Safe Zones
- No PvP allowed in safe zones
- Optional healing in safe zones
- Town guards attack outlaws

### 5. Ranking System

#### ELO Rating
- **Starting rating**: 1200
- **K-factor scaling**: 40 → 32 → 24 → 16 (adjusts by skill level)
- **Win/loss tracking**: Full statistics
- **Streak system**: Tracks consecutive wins/losses

#### Rank Tiers
```
Bronze III:    0-1099
Bronze II:     1100-1199
Bronze I:      1200-1299
Silver III:    1300-1399
Silver II:     1400-1499
Silver I:      1500-1599
Gold III:      1600-1699
Gold II:       1700-1799
Gold I:        1800-1899
Platinum III:  1900-1999
Platinum II:   2000-2099
Platinum I:    2100-2199
Diamond III:   2200-2299
Diamond II:    2300-2399
Diamond I:      2400-2499
Master:        2500-2699
GrandMaster:   2700-2899
Challenger:    2900+ (Top 100)
```

#### PvP Titles
- **Duelist**: Win 100 duels (+5% Damage in duels)
- **Gladiator**: Reach Gold rank (+3% All Stats in Arena)
- **Warlord**: Win 50 Guild Wars (+10% Damage to players)
- **Assassin**: 500 PvP kills (+5% Crit in PvP)
- **Defender**: 200 Arena wins (+5% Defense in PvP)
- **Champion**: Reach Diamond rank (+5% All PvP Stats)
- **Legend**: Reach Challenger (Special aura effect)

### 6. Tournament System

#### Tournament Types
- **Weekly 1v1**: Every Sunday, 1M Zen prize
- **Monthly 2v2**: First Saturday of month, 5M Zen prize
- **Seasonal 5v5**: End of season, 10M Zen prize
- **Guild Tournament**: Guild vs Guild, 20M Zen prize
- **World Championship**: Annual event, 100M Zen prize

#### Bracket System
- **Single Elimination**: One loss = out
- **Double Elimination**: Two losses = out (future)
- **Automatic seeding**: Random or by ranking
- **Prize distribution**: 50%, 25%, 12.5%, 12.5%

### 7. Season System
- **Duration**: 3 months (90 days)
- **Season rewards**: Based on highest rank achieved
- **Rewards include**:
  - Zen currency
  - Exclusive items
  - Season titles
  - Mount/Aura skins (Diamond+)
  - Arena Points for shop

## 🔧 Setup Instructions

### 1. Add PvPManager to Scene
```csharp
// Create empty GameObject
GameObject pvpObj = new GameObject("PvPManager");
PvPManager pvpManager = pvpObj.AddComponent<PvPManager>();

// Assign PvP data ScriptableObject
pvpManager.pvpData = yourPvPDataAsset;
```

### 2. Create PvP Data Asset
```
Right-click in Project → Create → Dark Legend → PvP → PvP Data
Configure settings in Inspector
```

### 3. Setup PvP Zone
```csharp
// Add PvPZone component to area GameObject
PvPZone zone = GameObject.AddComponent<PvPZone>();
zone.zoneName = "Arena District";
zone.rules.isPvPZone = true;
```

### 4. Setup Safe Zone
```csharp
// Add SafeZone component to town area
SafeZone safeZone = GameObject.AddComponent<SafeZone>();
safeZone.zoneName = "Safe Town";
safeZone.blockAllPvP = true;
```

### 5. Integrate with Player
```csharp
// Add to player GameObject
PvPToggle pvpToggle = player.AddComponent<PvPToggle>();
PvPNetworkSync networkSync = player.AddComponent<PvPNetworkSync>();

// Handle player death
pvpManager.ProcessPvPKill(killer, victim);
```

## 📡 Network Integration (Photon PUN 2)

### Required Setup
1. Import Photon PUN 2 from Asset Store
2. Setup Photon AppId in settings
3. Add PhotonView component to players
4. Add PvPNetworkSync component

### Network Methods
```csharp
// Send duel request
networkSync.SendDuelRequest(targetViewID, settings);

// Send PvP damage
networkSync.SendPvPDamage(targetViewID, damage, skillID);

// Broadcast kill
networkSync.BroadcastPvPKill(victimViewID);

// Update PK status
networkSync.UpdatePKStatus((int)pkStatus, pkCount);
```

## 🎨 UI Integration

### Main PvP Menu
```csharp
// Show PvP menu
PvPUI pvpUI = FindObjectOfType<PvPUI>();
pvpUI.Show();
```

### Arena Queue
```csharp
// Join arena queue
ArenaUI arenaUI = FindObjectOfType<ArenaUI>();
arenaManager.JoinQueue(player, ArenaMode.Solo1v1);
```

### Display Rankings
```csharp
// Show leaderboard
RankingUI rankingUI = FindObjectOfType<RankingUI>();
// Will automatically display current rankings
```

## 🔐 Anti-Cheat Considerations

### Server-Side Validation
- Validate all damage values server-side
- Check PvP rules before allowing attacks
- Verify player positions and distances
- Rate limit requests to prevent spam

### Client-Side Protection
- Use PhotonView ownership for authority
- Validate match state before actions
- Check cooldowns and resources
- Prevent exploits in matchmaking

## 📊 Events System

### Subscribe to PvP Events
```csharp
// PvP Manager events
pvpManager.OnPvPKill += (killer, victim) => {
    Debug.Log($"{killer.name} killed {victim.name}");
};

// Duel events
duelSystem.OnDuelAccepted += (challenger, target) => {
    Debug.Log("Duel accepted!");
};

// Arena events
arenaManager.OnMatchStart += (match) => {
    Debug.Log("Arena match started!");
};

// PK events
pkSystem.OnPKStatusChanged += (player, status) => {
    Debug.Log($"{player.name} is now {status}");
};
```

## 🎯 Usage Examples

### Example 1: Challenge Player to Duel
```csharp
DuelSettings settings = new DuelSettings {
    timeLimit = 300,
    allowPotions = true,
    allowSkills = true,
    betAmount = 100000,
    type = DuelType.Bet
};

duelSystem.SendDuelRequest(localPlayer, targetPlayer, settings);
```

### Example 2: Join Arena Queue
```csharp
arenaManager.JoinQueue(localPlayer, ArenaMode.Solo1v1);
```

### Example 3: Place Bounty
```csharp
bountySystem.PlaceBounty(targetPlayer, 50000, localPlayerId, localPlayerName);
```

### Example 4: Check PK Status
```csharp
string playerId = player.GetInstanceID().ToString();
PKStatus status = pkSystem.GetPKStatus(playerId);
Color nameColor = pkSystem.GetNameColor(status);
```

## 🚀 Performance Tips

1. **Object Pooling**: Pool match objects and UI elements
2. **LOD System**: Use LOD for arena spectators
3. **Network Optimization**: Send only essential data
4. **Update Optimization**: Cache component references
5. **UI Optimization**: Virtual scrolling for leaderboards

## 📝 Notes

- All scripts use namespace `DarkLegend.PvP`
- Comments are bilingual (English/Vietnamese)
- Compatible with Unity 2022.3 LTS
- Requires Photon PUN 2 for multiplayer
- TextMeshPro required for UI

## 🔄 Future Enhancements

- [ ] Guild Wars system
- [ ] Siege battles
- [ ] Custom game modes
- [ ] Replay system
- [ ] Spectator mode
- [ ] Advanced statistics
- [ ] Cross-server tournaments
- [ ] Mobile controls support

## 📄 License

MIT License - See main project LICENSE file

## 👥 Credits

Developed for Dark Legend - MU Online inspired 3D RPG
