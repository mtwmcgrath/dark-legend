# Character Reset System - Architecture Diagram

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────┐
│                        PLAYER INTERACTION                           │
│                                                                     │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐   │
│  │ ResetNPC │───▶│  Dialog  │───▶│ ResetUI  │───▶│ Confirm  │   │
│  └──────────┘    └──────────┘    └──────────┘    └──────────┘   │
│                                         │                          │
└─────────────────────────────────────────┼──────────────────────────┘
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────┐
│                         CORE SYSTEM                                 │
│                                                                     │
│  ┌───────────────────────────────────────────────────────────┐    │
│  │                    ResetSystem                            │    │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │    │
│  │  │ NormalReset  │  │ GrandReset   │  │ MasterReset  │  │    │
│  │  └──────────────┘  └──────────────┘  └──────────────┘  │    │
│  └───────────────────────────────────────────────────────────┘    │
│                           │                                        │
│              ┌────────────┼────────────┐                          │
│              ▼            ▼            ▼                          │
│    ┌──────────────┐ ┌──────────┐ ┌──────────┐                   │
│    │ Requirement  │ │  Reward  │ │ History  │                   │
│    └──────────────┘ └──────────┘ └──────────┘                   │
└─────────────────────────────────────────────────────────────────────┘
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────┐
│                        BONUS SYSTEM                                 │
│                                                                     │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐         │
│  │StatPoints│  │  Damage  │  │ Defense  │  │  HP/MP   │         │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘         │
│                                                                     │
│  ┌──────────┐  ┌──────────┐                                       │
│  │DropRate  │  │   Exp    │                                       │
│  └──────────┘  └──────────┘                                       │
└─────────────────────────────────────────────────────────────────────┘
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────┐
│                    CHARACTER STATS                                  │
│                                                                     │
│  ┌─────────────────────────────────────────────────────────────┐  │
│  │                   CharacterStats                            │  │
│  │  • level                    • resetBonusStats              │  │
│  │  • zen                      • resetDamageMultiplier         │  │
│  │  • normalResetCount         • resetDefenseMultiplier        │  │
│  │  • grandResetCount          • resetHPMultiplier             │  │
│  │  • hasMasterReset           • resetMPMultiplier             │  │
│  │  • resetHistory                                             │  │
│  └─────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      DATA PERSISTENCE                               │
│                                                                     │
│  ┌──────────────┐              ┌──────────────┐                   │
│  │ ResetSaveData│─────JSON────▶│ PlayerPrefs  │                   │
│  └──────────────┘              └──────────────┘                   │
└─────────────────────────────────────────────────────────────────────┘
```

## Data Flow Diagram

```
START: Player Interaction
    │
    ├─▶ Player talks to ResetNPC
    │       │
    │       ├─▶ ResetNPC checks proximity
    │       │       │
    │       │       └─▶ Opens ResetNPCDialog
    │       │               │
    │       └───────────────┘
    │
    ├─▶ ResetUI displays options
    │       │
    │       ├─▶ Player selects reset type
    │       │       │
    │       │       └─▶ Shows requirements & rewards
    │       │
    │       └─▶ Player clicks Reset button
    │               │
    │               └─▶ ResetConfirmUI appears
    │                       │
    │                       ├─▶ Player confirms
    │                       │       │
    │                       │       └─▶ Proceed to validation
    │                       │
    │                       └─▶ Player cancels
    │                               │
    │                               └─▶ Return to ResetUI
    │
    ├─▶ VALIDATION PHASE
    │       │
    │       ├─▶ ResetSystem.CanPerform[Type]Reset()
    │       │       │
    │       │       ├─▶ Check level requirement
    │       │       ├─▶ Check zen requirement
    │       │       ├─▶ Check reset count requirement
    │       │       └─▶ Check special requirements
    │       │
    │       ├─▶ Validation SUCCESS
    │       │       │
    │       │       └─▶ Proceed to execution
    │       │
    │       └─▶ Validation FAILED
    │               │
    │               └─▶ Fire OnResetFailed event
    │                       │
    │                       └─▶ Show error message
    │                               │
    │                               └─▶ END
    │
    ├─▶ EXECUTION PHASE
    │       │
    │       ├─▶ Deduct zen cost
    │       │
    │       ├─▶ Calculate rewards based on reset count
    │       │       │
    │       │       └─▶ ResetReward.CalculateReward()
    │       │
    │       ├─▶ Apply bonuses to character
    │       │       │
    │       │       ├─▶ StatPointBonus.Apply()
    │       │       ├─▶ DamageBonus.Apply()
    │       │       ├─▶ DefenseBonus.Apply()
    │       │       ├─▶ HPMPBonus.Apply()
    │       │       └─▶ DropRateBonus.Apply()
    │       │
    │       ├─▶ Reset character state
    │       │       │
    │       │       ├─▶ Level → 1
    │       │       ├─▶ Reset stat points (optional)
    │       │       └─▶ Keep items & skills
    │       │
    │       ├─▶ Update reset counts
    │       │       │
    │       │       ├─▶ normalResetCount++
    │       │       ├─▶ grandResetCount++ (if Grand)
    │       │       └─▶ hasMasterReset = true (if Master)
    │       │
    │       ├─▶ Add entry to history
    │       │       │
    │       │       └─▶ ResetHistory.AddEntry()
    │       │
    │       └─▶ Fire OnResetPerformed event
    │               │
    │               └─▶ Trigger effects & feedback
    │
    ├─▶ POST-RESET PHASE
    │       │
    │       ├─▶ Show success message
    │       │
    │       ├─▶ Play VFX/SFX effects
    │       │
    │       ├─▶ Update UI displays
    │       │       │
    │       │       ├─▶ ResetUI refresh
    │       │       ├─▶ ResetInfoUI update
    │       │       └─▶ ResetHistoryUI refresh
    │       │
    │       └─▶ Save game state
    │               │
    │               └─▶ ResetSaveManager.SaveResetData()
    │                       │
    │                       └─▶ Serialize to JSON
    │                               │
    │                               └─▶ Store in PlayerPrefs
    │
    └─▶ END: Reset Complete
```

## Class Relationship Diagram

```
┌──────────────────┐
│  ResetSystem     │◄────────────┐
│  (Singleton)     │             │
└────────┬─────────┘             │
         │ uses                  │ triggers
         │                       │
    ┌────▼────┐             ┌────┴────┐
    │ResetData│             │  Events │
    │(ScriptO)│             └─────────┘
    └─────────┘                   │
         │                        │
         │ configures       ┌─────▼──────┐
         │                  │ UI System  │
    ┌────▼──────────┐      │            │
    │ Requirements  │      │ • ResetUI  │
    │ • Level       │      │ • Confirm  │
    │ • Zen         │      │ • Info     │
    │ • Count       │      │ • History  │
    └───────────────┘      │ • Ranking  │
                           └────────────┘
    ┌───────────────┐
    │   Rewards     │
    │ • Stats       │
    │ • Multipliers │
    └───────────────┘
         │
         │ applies to
         │
    ┌────▼─────────────┐
    │ CharacterStats   │◄──────┐
    │ • level          │       │
    │ • zen            │       │
    │ • resetCounts    │       │ modifies
    │ • multipliers    │       │
    │ • history        │  ┌────┴────┐
    └──────────────────┘  │ Bonuses │
         │                │         │
         │ persisted by   │ • Stat  │
         │                │ • DMG   │
    ┌────▼──────────┐    │ • DEF   │
    │ ResetSaveData │    │ • HP/MP │
    │ • JSON format │    │ • Drop  │
    │ • PlayerPrefs │    └─────────┘
    └───────────────┘
```

## Reset Type Progression Flow

```
┌──────────────────────────────────────────────────────────────────┐
│                      NORMAL RESET TIER                           │
│                                                                  │
│  Reset 1-10:   Level 400 → Reset → +200 stats, +1% bonuses     │
│       ↓                                                          │
│  Reset 11-30:  Level 400 → Reset → +250 stats, +1.5% bonuses   │
│       ↓                                                          │
│  Reset 31-50:  Level 400 → Reset → +300 stats, +2% bonuses     │
│       ↓                                                          │
│  Reset 51-100: Level 400 → Reset → +400 stats, +2.5% bonuses   │
│       ↓                                                          │
│  UNLOCK: Grand Reset Available                                  │
└────────────────────────┬─────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────────┐
│                      GRAND RESET TIER                            │
│                                                                  │
│  Grand Reset 1-10:                                              │
│    Requires: 100 Normal Resets, Level 400, 1B Zen              │
│    Rewards:  +5,000 stats, +10% DMG/DEF                        │
│    Effect:   Normal reset count → 0                             │
│       ↓                                                          │
│  UNLOCK: Master Reset Available (after 10 Grand)                │
└────────────────────────┬─────────────────────────────────────────┘
                         │
                         ▼
┌──────────────────────────────────────────────────────────────────┐
│                      MASTER RESET TIER                           │
│                                                                  │
│  Master Reset (ONE TIME ONLY):                                  │
│    Requires: 10 Grand Resets, Level 400, 10B Zen               │
│    Rewards:  +50,000 stats, +50% DMG/DEF                       │
│    Special:  Master title, golden name, exclusive skills       │
│    Status:   LEGENDARY ACHIEVEMENT                              │
└──────────────────────────────────────────────────────────────────┘
```

## Event System Flow

```
User Action
    │
    ├─▶ Performs Reset
    │       │
    │       ├─▶ [SUCCESS]
    │       │       │
    │       │       └─▶ OnResetPerformed(ResetType, CharacterStats)
    │       │               │
    │       │               ├─▶ UI updates
    │       │               ├─▶ VFX/SFX plays
    │       │               ├─▶ Analytics logs
    │       │               └─▶ Auto-save triggers
    │       │
    │       └─▶ [FAILURE]
    │               │
    │               └─▶ OnResetFailed(string reason)
    │                       │
    │                       ├─▶ Error message displayed
    │                       └─▶ Log for debugging
    │
    ├─▶ Opens UI
    │       │
    │       └─▶ OnDialogOpened()
    │               │
    │               └─▶ Load character data
    │
    └─▶ Closes UI
            │
            └─▶ OnDialogClosed()
                    │
                    └─▶ Cleanup & save state
```

## Integration Points Summary

```
┌────────────────────────────────────────────────────────────┐
│                  EXTERNAL SYSTEMS                          │
│                                                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │
│  │   Combat     │  │  Inventory   │  │  Experience  │   │
│  │   System     │  │   System     │  │   System     │   │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘   │
│         │                  │                  │           │
│         └──────────────────┼──────────────────┘           │
│                            │                              │
└────────────────────────────┼──────────────────────────────┘
                             │
                    Uses multipliers from
                             │
                    ┌────────▼────────┐
                    │ CharacterStats  │
                    │ with Reset Data │
                    └─────────────────┘
```

---

**Legend:**
- `│ ─ ┌ ┐ └ ┘ ├ ┤ ┬ ┴ ┼` : Box drawing characters
- `▶ ▼` : Flow direction
- `◄` : Dependency direction
- `(Singleton)` : Singleton pattern
- `(ScriptO)` : ScriptableObject
