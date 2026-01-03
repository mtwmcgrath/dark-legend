# 🔄 Dark Legend - Character Reset System

## Overview | Tổng quan

The Character Reset System is a comprehensive MU Online-style reset system that allows players to reset their character level to gain permanent bonuses and become progressively stronger.

Hệ thống Reset nhân vật là một hệ thống reset theo phong cách MU Online hoàn chỉnh, cho phép người chơi reset level về 1 để nhận bonus vĩnh viễn và trở nên mạnh hơn theo thời gian.

## 📁 File Structure | Cấu trúc Files

```
Assets/Scripts/Reset/
├── Core/                          # Core system components
│   ├── ResetSystem.cs            # Main reset system
│   ├── ResetData.cs              # ScriptableObject configuration
│   ├── ResetRequirement.cs       # Reset requirements
│   ├── ResetReward.cs            # Reset rewards
│   ├── ResetHistory.cs           # Character reset history
│   └── ResetSaveData.cs          # Save/Load system
│
├── Types/                         # Reset type implementations
│   ├── NormalReset.cs            # Normal reset (1-100)
│   ├── GrandReset.cs             # Grand reset (1-10)
│   └── MasterReset.cs            # Master reset (1 time only)
│
├── Bonuses/                       # Bonus system
│   ├── ResetBonus.cs             # Base bonus class
│   ├── StatPointBonus.cs         # Stat point bonuses
│   ├── DamageBonus.cs            # Damage bonuses
│   ├── DefenseBonus.cs           # Defense bonuses
│   ├── HPMPBonus.cs              # HP/MP bonuses
│   └── DropRateBonus.cs          # Drop rate bonuses
│
├── NPC/                           # NPC interaction
│   ├── ResetNPC.cs               # Reset NPC
│   └── ResetNPCDialog.cs         # Dialog system
│
└── UI/                            # User interface
    ├── ResetUI.cs                # Main reset UI
    ├── ResetConfirmUI.cs         # Confirmation dialog
    ├── ResetInfoUI.cs            # Information display
    ├── ResetHistoryUI.cs         # History viewer
    └── ResetRankingUI.cs         # Ranking board
```

## 🎯 Features | Tính năng

### 1. Three Reset Types | Ba loại Reset

#### Normal Reset (Reset Thường)
- **Requirements**: Level 400, 10M-210M Zen (increases per reset)
- **Rewards**: +200-400 stat points, +1-2.5% damage/defense
- **Limit**: 100 resets maximum
- **Effects**: Level reset to 1, keep items & skills

#### Grand Reset (Đại Reset)  
- **Requirements**: 100 Normal Resets, Level 400, 1 billion Zen
- **Rewards**: +5,000 stat points, +10% damage/defense, special title
- **Limit**: 10 grand resets maximum
- **Effects**: Level reset to 1, normal reset count reset to 0

#### Master Reset (Tối Thượng Reset)
- **Requirements**: 10 Grand Resets, Level 400, 10 billion Zen
- **Rewards**: +50,000 stat points, +50% damage/defense, Master title, golden name, special skills & wings
- **Limit**: Once per character
- **Effects**: Ultimate power, special abilities unlocked

### 2. Tiered Reward System | Hệ thống phần thưởng phân cấp

Normal Reset rewards scale based on reset count:
- **Reset 1-10**: +200 stats, +1% damage/defense per reset
- **Reset 11-30**: +250 stats, +1.5% damage/defense per reset  
- **Reset 31-50**: +300 stats, +2% damage/defense per reset
- **Reset 51-100**: +400 stats, +2.5% damage/defense per reset

### 3. Comprehensive History System | Hệ thống lịch sử đầy đủ

- Track every reset with timestamp, level, and rewards
- View recent reset history
- Calculate total reset power
- Export history data

### 4. Ranking System | Hệ thống xếp hạng

- Rankings by total resets
- Rankings by normal resets
- Rankings by grand resets  
- Master reset hall of fame
- Player highlighting

### 5. Save/Load System | Hệ thống lưu/tải

- Complete save data serialization
- JSON-based storage
- PlayerPrefs integration
- Safe error handling

## 🚀 Quick Start | Bắt đầu nhanh

### 1. Setup in Unity

1. Import all scripts into your Unity project
2. Create a ResetData ScriptableObject:
   ```
   Assets > Create > Dark Legend > Reset > Reset Data
   ```
3. Add ResetSystem component to a GameObject in your scene
4. Assign the ResetData to ResetSystem

### 2. Basic Usage | Sử dụng cơ bản

```csharp
using DarkLegend.Reset;

// Check if character can reset
bool canReset = ResetSystem.Instance.CanPerformNormalReset(character, out string reason);

if (canReset)
{
    // Perform reset
    bool success = ResetSystem.Instance.PerformNormalReset(character);
    
    if (success)
    {
        Debug.Log("Reset successful!");
    }
}
else
{
    Debug.Log($"Cannot reset: {reason}");
}
```

### 3. Subscribe to Events | Đăng ký sự kiện

```csharp
ResetSystem.Instance.OnResetPerformed += (resetType, character) =>
{
    Debug.Log($"{character.name} performed {resetType} reset!");
};

ResetSystem.Instance.OnResetFailed += (reason) =>
{
    Debug.Log($"Reset failed: {reason}");
};
```

### 4. Save and Load | Lưu và tải

```csharp
// Save reset data
string saveKey = ResetSaveManager.Instance.GetDefaultSaveKey(character);
ResetSaveManager.Instance.SaveResetData(character, saveKey);

// Load reset data
ResetSaveManager.Instance.LoadResetData(character, saveKey);
```

## 📊 Reset Overview Table | Bảng tổng hợp Reset

```
╔═══════════════════════════════════════════════════════════════════╗
║                    RESET SYSTEM OVERVIEW                          ║
╠═══════════════════════════════════════════════════════════════════╣
║ TYPE          │ REQUIREMENT      │ REWARD                         ║
╠═══════════════════════════════════════════════════════════════════╣
║ Normal Reset  │ Level 400        │ +200-400 Stats                 ║
║ (1-100 times) │ 10M-210M Zen     │ +1-2.5% Damage/Defense         ║
╠═══════════════════════════════════════════════════════════════════╣
║ Grand Reset   │ 100 Normal Reset │ +5,000 Stats                   ║
║ (1-10 times)  │ Level 400        │ +10% Damage/Defense            ║
║               │ 1 billion Zen    │ Special Title                  ║
╠═══════════════════════════════════════════════════════════════════╣
║ Master Reset  │ 10 Grand Reset   │ +50,000 Stats                  ║
║ (1 time)      │ Level 400        │ +50% Damage/Defense            ║
║               │ 10 billion Zen   │ Master Title + Golden Name     ║
║               │ Special Item     │ Master Skills & Wings          ║
╚═══════════════════════════════════════════════════════════════════╝
```

## 🔧 Configuration | Cấu hình

The ResetData ScriptableObject allows complete customization:

- **Requirements**: Adjust level, zen cost, and prerequisites
- **Rewards**: Configure stat bonuses and multipliers
- **Effects**: Control what is kept/reset
- **Limits**: Set maximum reset counts
- **Special Features**: Enable/disable master skills, wings, etc.

## 🎨 UI Components | Thành phần giao diện

All UI components are designed to be modular and customizable:

- **ResetUI**: Main interface for performing resets
- **ResetConfirmUI**: Confirmation dialog with detailed info
- **ResetInfoUI**: Display reset system overview
- **ResetHistoryUI**: View character's reset history
- **ResetRankingUI**: Browse server rankings

## 🔌 Integration | Tích hợp

### With CharacterStats

The system extends CharacterStats with:
```csharp
public partial class CharacterStats : MonoBehaviour
{
    public int normalResetCount;
    public int grandResetCount;
    public bool hasMasterReset;
    public int resetBonusStats;
    public float resetDamageMultiplier;
    public float resetDefenseMultiplier;
    public float resetHPMultiplier;
    public float resetMPMultiplier;
    public ResetHistory resetHistory;
}
```

### With Combat System

Use the calculation methods:
```csharp
int finalDamage = character.CalculateFinalDamage(baseDamage);
int finalDefense = character.CalculateFinalDefense(baseDefense);
int maxHP = character.CalculateMaxHP(baseHP);
int maxMP = character.CalculateMaxMP(baseMP);
```

## 📝 Example Scenarios | Ví dụ tình huống

### Scenario 1: First Normal Reset
```csharp
// Player reaches level 400 for the first time
CharacterStats player = GetPlayer();

if (player.level >= 400 && player.zen >= 10000000)
{
    ResetSystem.Instance.PerformNormalReset(player);
    // Player receives +200 stats, +1% damage, +1% defense
    // Level reset to 1, but keeps all items and skills
}
```

### Scenario 2: Grand Reset Journey
```csharp
// Player has completed 100 normal resets
CharacterStats veteran = GetPlayer();

if (veteran.normalResetCount == 100 && veteran.level == 400)
{
    ResetSystem.Instance.PerformGrandReset(veteran);
    // Player receives +5000 stats, +10% damage/defense
    // Normal reset count reset to 0
    // Receives "Grand Master" title
}
```

### Scenario 3: Achieving Master Status
```csharp
// Player has completed 10 grand resets
CharacterStats legend = GetPlayer();

if (legend.grandResetCount == 10)
{
    ResetSystem.Instance.PerformMasterReset(legend);
    // Player becomes a Master!
    // +50000 stats, +50% damage/defense
    // Golden name, special skills unlocked
}
```

## 🐛 Troubleshooting | Xử lý sự cố

### Common Issues | Vấn đề thường gặp

1. **Reset button disabled**: Check if character meets all requirements
2. **Reset fails silently**: Subscribe to OnResetFailed event for error messages
3. **Bonuses not applying**: Ensure CharacterStats properly implements calculation methods
4. **Save data not persisting**: Verify PlayerPrefs permissions on target platform

## 📚 API Reference | Tài liệu API

### ResetSystem

```csharp
// Check reset eligibility
bool CanPerformNormalReset(CharacterStats character, out string reason)
bool CanPerformGrandReset(CharacterStats character, out string reason)
bool CanPerformMasterReset(CharacterStats character, out string reason)

// Perform resets
bool PerformNormalReset(CharacterStats character)
bool PerformGrandReset(CharacterStats character)
bool PerformMasterReset(CharacterStats character)

// Get information
string GetResetInfo(CharacterStats character, ResetType type)
```

### ResetSaveManager

```csharp
// Save/Load
bool SaveResetData(CharacterStats character, string saveKey)
bool LoadResetData(CharacterStats character, string saveKey)
bool DeleteResetData(string saveKey)

// Utilities
bool HasResetData(string saveKey)
string GetDefaultSaveKey(CharacterStats character)
```

## 🎯 Best Practices | Thực hành tốt nhất

1. **Always validate** before performing resets
2. **Subscribe to events** for proper feedback
3. **Save frequently** after successful resets
4. **Use confirmation dialogs** to prevent accidents
5. **Display clear information** to players about requirements and rewards

## 📄 License

This reset system is part of the Dark Legend project and follows the MIT license.

## 🤝 Contributing

Contributions are welcome! Please ensure all code includes both English and Vietnamese comments.

---

Created with ❤️ for Dark Legend RPG
