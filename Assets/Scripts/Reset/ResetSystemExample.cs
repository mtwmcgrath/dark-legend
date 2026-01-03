using UnityEngine;
using DarkLegend.Reset;

/// <summary>
/// Example integration script demonstrating how to use the Reset System
/// Script ví dụ tích hợp hệ thống Reset
/// </summary>
public class ResetSystemExample : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStats playerCharacter;
    
    private void Start()
    {
        // Subscribe to reset events
        // Đăng ký sự kiện reset
        SubscribeToEvents();
        
        // Load saved reset data
        // Tải dữ liệu reset đã lưu
        LoadResetData();
    }

    private void SubscribeToEvents()
    {
        // Listen for successful resets
        // Lắng nghe reset thành công
        ResetSystem.Instance.OnResetPerformed += HandleResetPerformed;
        
        // Listen for failed resets
        // Lắng nghe reset thất bại
        ResetSystem.Instance.OnResetFailed += HandleResetFailed;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        // Hủy đăng ký sự kiện
        if (ResetSystem.Instance != null)
        {
            ResetSystem.Instance.OnResetPerformed -= HandleResetPerformed;
            ResetSystem.Instance.OnResetFailed -= HandleResetFailed;
        }
    }

    /// <summary>
    /// Example: Check if player can perform any type of reset
    /// Ví dụ: Kiểm tra người chơi có thể reset loại nào
    /// </summary>
    public void CheckResetAvailability()
    {
        if (playerCharacter == null)
        {
            Debug.LogWarning("No player character assigned!");
            return;
        }

        Debug.Log("=== Reset Availability Check ===");

        // Check Normal Reset
        bool canNormalReset = ResetSystem.Instance.CanPerformNormalReset(playerCharacter, out string normalReason);
        Debug.Log($"Normal Reset: {(canNormalReset ? "✓ Available" : $"✗ {normalReason}")}");

        // Check Grand Reset
        bool canGrandReset = ResetSystem.Instance.CanPerformGrandReset(playerCharacter, out string grandReason);
        Debug.Log($"Grand Reset: {(canGrandReset ? "✓ Available" : $"✗ {grandReason}")}");

        // Check Master Reset
        bool canMasterReset = ResetSystem.Instance.CanPerformMasterReset(playerCharacter, out string masterReason);
        Debug.Log($"Master Reset: {(canMasterReset ? "✓ Available" : $"✗ {masterReason}")}");
    }

    /// <summary>
    /// Example: Perform a normal reset with UI
    /// Ví dụ: Thực hiện normal reset với UI
    /// </summary>
    public void PerformNormalResetWithUI()
    {
        if (playerCharacter == null)
            return;

        // Check if can reset
        if (ResetSystem.Instance.CanPerformNormalReset(playerCharacter, out string reason))
        {
            // Show reset UI
            ResetUI.Instance.Show(playerCharacter);
        }
        else
        {
            Debug.LogWarning($"Cannot perform normal reset: {reason}");
        }
    }

    /// <summary>
    /// Example: Perform reset with confirmation
    /// Ví dụ: Thực hiện reset với xác nhận
    /// </summary>
    public void PerformResetWithConfirmation(ResetType resetType)
    {
        if (playerCharacter == null)
            return;

        // Show confirmation dialog
        ResetConfirmUI.Instance.Show(playerCharacter, resetType, () =>
        {
            // This callback is executed when user confirms
            // Callback này được thực thi khi người dùng xác nhận
            PerformReset(resetType);
        });
    }

    /// <summary>
    /// Example: Direct reset without UI
    /// Ví dụ: Reset trực tiếp không qua UI
    /// </summary>
    public void PerformReset(ResetType resetType)
    {
        if (playerCharacter == null)
            return;

        bool success = false;

        switch (resetType)
        {
            case ResetType.Normal:
                success = ResetSystem.Instance.PerformNormalReset(playerCharacter);
                break;
            case ResetType.Grand:
                success = ResetSystem.Instance.PerformGrandReset(playerCharacter);
                break;
            case ResetType.Master:
                success = ResetSystem.Instance.PerformMasterReset(playerCharacter);
                break;
        }

        if (success)
        {
            Debug.Log($"✓ {resetType} reset successful!");
            
            // Save after successful reset
            SaveResetData();
        }
    }

    /// <summary>
    /// Example: Display reset information
    /// Ví dụ: Hiển thị thông tin reset
    /// </summary>
    public void DisplayResetInfo(ResetType resetType)
    {
        if (playerCharacter == null)
            return;

        string info = ResetSystem.Instance.GetResetInfo(playerCharacter, resetType);
        Debug.Log(info);

        // Or show in UI
        ResetInfoUI.Instance.Show(playerCharacter);
    }

    /// <summary>
    /// Example: View reset history
    /// Ví dụ: Xem lịch sử reset
    /// </summary>
    public void ViewResetHistory()
    {
        if (playerCharacter == null)
            return;

        if (playerCharacter.resetHistory == null || playerCharacter.resetHistory.Entries.Count == 0)
        {
            Debug.Log("No reset history yet.");
            return;
        }

        Debug.Log("=== Reset History ===");
        var recentResets = playerCharacter.resetHistory.GetRecentResets(10);
        foreach (var entry in recentResets)
        {
            Debug.Log(entry.GetFormattedString());
        }

        // Or show in UI
        ResetHistoryUI.Instance.Show(playerCharacter);
    }

    /// <summary>
    /// Example: Display reset rankings
    /// Ví dụ: Hiển thị bảng xếp hạng reset
    /// </summary>
    public void ViewResetRankings()
    {
        ResetRankingUI.Instance.Show(playerCharacter);
    }

    /// <summary>
    /// Example: Calculate total bonuses
    /// Ví dụ: Tính tổng bonus
    /// </summary>
    public void DisplayTotalBonuses()
    {
        if (playerCharacter == null)
            return;

        Debug.Log("=== Total Reset Bonuses ===");
        Debug.Log($"Bonus Stats: +{playerCharacter.resetBonusStats:N0}");
        Debug.Log($"Damage: +{(playerCharacter.resetDamageMultiplier - 1f) * 100:F1}%");
        Debug.Log($"Defense: +{(playerCharacter.resetDefenseMultiplier - 1f) * 100:F1}%");
        Debug.Log($"HP: +{(playerCharacter.resetHPMultiplier - 1f) * 100:F1}%");
        Debug.Log($"MP: +{(playerCharacter.resetMPMultiplier - 1f) * 100:F1}%");

        if (playerCharacter.resetHistory != null)
        {
            Debug.Log($"Reset Power: {playerCharacter.resetHistory.GetTotalResetPower():N0}");
        }
    }

    /// <summary>
    /// Example: Apply combat bonuses
    /// Ví dụ: Áp dụng bonus chiến đấu
    /// </summary>
    public void ApplyCombatBonuses()
    {
        if (playerCharacter == null)
            return;

        // Example base values
        int baseDamage = 1000;
        int baseDefense = 500;
        int baseHP = 5000;
        int baseMP = 3000;

        // Calculate final values with reset bonuses
        int finalDamage = playerCharacter.CalculateFinalDamage(baseDamage);
        int finalDefense = playerCharacter.CalculateFinalDefense(baseDefense);
        int finalHP = playerCharacter.CalculateMaxHP(baseHP);
        int finalMP = playerCharacter.CalculateMaxMP(baseMP);

        Debug.Log("=== Combat Stats with Reset Bonuses ===");
        Debug.Log($"Damage: {baseDamage} → {finalDamage} (+{finalDamage - baseDamage})");
        Debug.Log($"Defense: {baseDefense} → {finalDefense} (+{finalDefense - baseDefense})");
        Debug.Log($"HP: {baseHP} → {finalHP} (+{finalHP - baseHP})");
        Debug.Log($"MP: {baseMP} → {finalMP} (+{finalMP - baseMP})");
    }

    /// <summary>
    /// Save reset data
    /// Lưu dữ liệu reset
    /// </summary>
    private void SaveResetData()
    {
        if (playerCharacter == null)
            return;

        string saveKey = ResetSaveManager.Instance.GetDefaultSaveKey(playerCharacter);
        bool success = ResetSaveManager.Instance.SaveResetData(playerCharacter, saveKey);

        if (success)
        {
            Debug.Log("✓ Reset data saved successfully");
        }
        else
        {
            Debug.LogError("✗ Failed to save reset data");
        }
    }

    /// <summary>
    /// Load reset data
    /// Tải dữ liệu reset
    /// </summary>
    private void LoadResetData()
    {
        if (playerCharacter == null)
            return;

        string saveKey = ResetSaveManager.Instance.GetDefaultSaveKey(playerCharacter);
        
        if (ResetSaveManager.Instance.HasResetData(saveKey))
        {
            bool success = ResetSaveManager.Instance.LoadResetData(playerCharacter, saveKey);
            
            if (success)
            {
                Debug.Log("✓ Reset data loaded successfully");
            }
            else
            {
                Debug.LogError("✗ Failed to load reset data");
            }
        }
        else
        {
            Debug.Log("No saved reset data found. Starting fresh.");
        }
    }

    /// <summary>
    /// Handle successful reset event
    /// Xử lý sự kiện reset thành công
    /// </summary>
    private void HandleResetPerformed(ResetType resetType, CharacterStats character)
    {
        Debug.Log($"🎉 Reset performed: {resetType} for {character.name}");
        
        // Play success effects
        PlayResetSuccessEffects(resetType);
        
        // Show success message
        ShowSuccessMessage(resetType);
        
        // Save game state
        SaveResetData();
    }

    /// <summary>
    /// Handle failed reset event
    /// Xử lý sự kiện reset thất bại
    /// </summary>
    private void HandleResetFailed(string reason)
    {
        Debug.LogWarning($"⚠️ Reset failed: {reason}");
        
        // Show error message to player
        ShowErrorMessage(reason);
    }

    /// <summary>
    /// Play visual/audio effects for successful reset
    /// Phát hiệu ứng hình ảnh/âm thanh cho reset thành công
    /// </summary>
    private void PlayResetSuccessEffects(ResetType resetType)
    {
        // Implement your VFX/SFX here
        // Implement VFX/SFX của bạn ở đây
        
        switch (resetType)
        {
            case ResetType.Normal:
                Debug.Log("✨ Playing normal reset effects");
                break;
            case ResetType.Grand:
                Debug.Log("⚡ Playing grand reset effects");
                break;
            case ResetType.Master:
                Debug.Log("🌟 Playing master reset effects");
                break;
        }
    }

    /// <summary>
    /// Show success message to player
    /// Hiển thị thông báo thành công cho người chơi
    /// </summary>
    private void ShowSuccessMessage(ResetType resetType)
    {
        string message = resetType switch
        {
            ResetType.Normal => "Normal Reset Complete! You've become stronger!",
            ResetType.Grand => "Grand Reset Complete! You've achieved greatness!",
            ResetType.Master => "Master Reset Complete! You are now a legend!",
            _ => "Reset Complete!"
        };

        Debug.Log($"💬 {message}");
        
        // Display in your UI system
        // Hiển thị trong UI system của bạn
    }

    /// <summary>
    /// Show error message to player
    /// Hiển thị thông báo lỗi cho người chơi
    /// </summary>
    private void ShowErrorMessage(string reason)
    {
        Debug.Log($"💬 Cannot reset: {reason}");
        
        // Display in your UI system
        // Hiển thị trong UI system của bạn
    }

    // ============================================
    // Unity Editor Helper Methods (for testing)
    // Các phương thức hỗ trợ Unity Editor (để test)
    // ============================================

    #if UNITY_EDITOR
    [ContextMenu("Test: Check Reset Availability")]
    private void TestCheckResetAvailability()
    {
        CheckResetAvailability();
    }

    [ContextMenu("Test: Display Total Bonuses")]
    private void TestDisplayTotalBonuses()
    {
        DisplayTotalBonuses();
    }

    [ContextMenu("Test: Apply Combat Bonuses")]
    private void TestApplyCombatBonuses()
    {
        ApplyCombatBonuses();
    }

    [ContextMenu("Test: Save Reset Data")]
    private void TestSaveResetData()
    {
        SaveResetData();
    }

    [ContextMenu("Test: Load Reset Data")]
    private void TestLoadResetData()
    {
        LoadResetData();
    }

    [ContextMenu("Test: Simulate Level 400")]
    private void TestSimulateLevel400()
    {
        if (playerCharacter != null)
        {
            playerCharacter.level = 400;
            playerCharacter.zen = 100000000; // 100M Zen
            Debug.Log("✓ Character set to Level 400 with 100M Zen");
        }
    }

    [ContextMenu("Test: Simulate 100 Normal Resets")]
    private void TestSimulate100Resets()
    {
        if (playerCharacter != null)
        {
            playerCharacter.normalResetCount = 100;
            playerCharacter.level = 400;
            playerCharacter.zen = 2000000000; // 2 billion Zen
            Debug.Log("✓ Character set with 100 resets, Level 400, 2B Zen");
        }
    }

    [ContextMenu("Test: Simulate 10 Grand Resets")]
    private void TestSimulate10GrandResets()
    {
        if (playerCharacter != null)
        {
            playerCharacter.grandResetCount = 10;
            playerCharacter.level = 400;
            playerCharacter.zen = 20000000000; // 20 billion Zen
            Debug.Log("✓ Character set with 10 grand resets, Level 400, 20B Zen");
        }
    }
    #endif
}
