using UnityEngine;

namespace DarkLegend.Maps.Zones
{
    /// <summary>
    /// Vùng PvP tự do / Free PvP zone
    /// Players can attack each other freely
    /// </summary>
    public class PvPZone : ZoneBase
    {
        [Header("PvP Settings")]
        [Tooltip("Cưỡng bức PvP / Force PvP mode")]
        [SerializeField] private bool forcePvP = true;
        
        [Tooltip("Penalty khi chết / Death penalty multiplier")]
        [SerializeField] private float deathPenaltyMultiplier = 2f;
        
        [Tooltip("Drop item khi chết / Drop items on death")]
        [SerializeField] private bool dropItemsOnDeath = true;
        
        [Tooltip("Tỷ lệ drop / Drop rate (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float dropRate = 0.5f;
        
        [Header("Rewards")]
        [Tooltip("Bonus EXP khi giết player / Bonus EXP for kills")]
        [SerializeField] private int killBonusExp = 100;
        
        [Tooltip("PvP points cho ranking / PvP points")]
        [SerializeField] private int pvpPointsPerKill = 10;
        
        [Header("Restrictions")]
        [Tooltip("Không thể dùng town portal / Cannot use town portal")]
        [SerializeField] private bool blockTownPortal = true;
        
        [Tooltip("Thời gian combat (giây) / Combat time in seconds")]
        [SerializeField] private float combatTime = 10f;
        
        public override void InitializeZone()
        {
            base.InitializeZone();
            
            Debug.Log($"[PvPZone] PvP zone initialized: {zoneName}");
            Debug.Log($"[PvPZone] Force PvP: {forcePvP}, Death penalty: {deathPenaltyMultiplier}x");
        }
        
        public override void OnPlayerEnter(GameObject player)
        {
            base.OnPlayerEnter(player);
            
            // Enable PvP mode
            EnablePvPMode(player);
            
            // Show warning
            ShowPvPWarning(player);
        }
        
        public override void OnPlayerExit(GameObject player)
        {
            base.OnPlayerExit(player);
            
            // Disable PvP mode if not forced
            if (!forcePvP)
            {
                DisablePvPMode(player);
            }
        }
        
        /// <summary>
        /// Bật chế độ PvP / Enable PvP mode
        /// </summary>
        private void EnablePvPMode(GameObject player)
        {
            // TODO: Implement PvP system integration
            Debug.Log($"[PvPZone] Enabled PvP mode for player");
            
            // Block town portal
            if (blockTownPortal)
            {
                // TODO: Block town portal usage
            }
        }
        
        /// <summary>
        /// Tắt chế độ PvP / Disable PvP mode
        /// </summary>
        private void DisablePvPMode(GameObject player)
        {
            // TODO: Implement PvP system integration
            Debug.Log($"[PvPZone] Disabled PvP mode for player");
        }
        
        /// <summary>
        /// Hiển thị cảnh báo PvP / Show PvP warning
        /// </summary>
        private void ShowPvPWarning(GameObject player)
        {
            string warning = $"⚠️ BẠN ĐÃ VÀO VÙNG PVP! ⚠️\n" +
                           $"• Players có thể tấn công bạn\n" +
                           $"• Penalty khi chết: {deathPenaltyMultiplier}x\n";
            
            if (dropItemsOnDeath)
            {
                warning += $"• Drop items: {dropRate * 100}%\n";
            }
            
            if (blockTownPortal)
            {
                warning += "• Không thể dùng town portal";
            }
            
            Debug.Log($"[PvPZone] {warning}");
            // TODO: Show UI warning
        }
        
        /// <summary>
        /// Xử lý khi player giết player khác / Handle player kill
        /// </summary>
        public void OnPlayerKilledPlayer(GameObject killer, GameObject victim)
        {
            Debug.Log($"[PvPZone] Player kill recorded");
            
            // Award EXP bonus
            AwardKillBonus(killer);
            
            // Award PvP points
            AwardPvPPoints(killer);
            
            // Apply death penalty to victim
            ApplyDeathPenalty(victim);
            
            // Handle item drops
            if (dropItemsOnDeath)
            {
                HandleItemDrop(victim);
            }
        }
        
        /// <summary>
        /// Trao bonus cho killer / Award kill bonus
        /// </summary>
        private void AwardKillBonus(GameObject killer)
        {
            // TODO: Add EXP to player
            Debug.Log($"[PvPZone] Awarded {killBonusExp} bonus EXP");
        }
        
        /// <summary>
        /// Trao PvP points / Award PvP points
        /// </summary>
        private void AwardPvPPoints(GameObject killer)
        {
            // TODO: Add PvP points to player
            Debug.Log($"[PvPZone] Awarded {pvpPointsPerKill} PvP points");
        }
        
        /// <summary>
        /// Áp dụng penalty khi chết / Apply death penalty
        /// </summary>
        private void ApplyDeathPenalty(GameObject victim)
        {
            // TODO: Apply penalty (EXP loss, etc.)
            Debug.Log($"[PvPZone] Applied death penalty: {deathPenaltyMultiplier}x");
        }
        
        /// <summary>
        /// Xử lý drop item / Handle item dropping
        /// </summary>
        private void HandleItemDrop(GameObject victim)
        {
            // TODO: Drop items based on drop rate
            Debug.Log($"[PvPZone] Handling item drop with rate: {dropRate}");
        }
        
        /// <summary>
        /// Kiểm tra có thể dùng town portal không / Check if can use town portal
        /// </summary>
        public bool CanUseTownPortal()
        {
            return !blockTownPortal;
        }
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            // Draw PvP zone with red color
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawCube(zoneCenter, zoneSize);
        }
    }
}
