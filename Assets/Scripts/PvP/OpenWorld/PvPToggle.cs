using UnityEngine;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PvP Toggle Component - Bật/tắt PvP
    /// Allows players to enable/disable PvP mode
    /// </summary>
    public class PvPToggle : MonoBehaviour
    {
        [Header("PvP State")]
        public bool pvpEnabled = false;
        
        [Header("Toggle Settings")]
        public float toggleCooldown = 60f;        // 1 minute cooldown
        public bool allowToggleInCombat = false;
        
        private float lastToggleTime = -999f;
        private bool isInCombat = false;
        
        /// <summary>
        /// Enable PvP mode
        /// Bật chế độ PvP
        /// </summary>
        public bool EnablePvP()
        {
            if (!CanToggle())
            {
                Debug.LogWarning("Cannot toggle PvP right now");
                return false;
            }
            
            pvpEnabled = true;
            lastToggleTime = Time.time;
            
            Debug.Log($"{gameObject.name} enabled PvP mode");
            return true;
        }
        
        /// <summary>
        /// Disable PvP mode
        /// Tắt chế độ PvP
        /// </summary>
        public bool DisablePvP()
        {
            if (!CanToggle())
            {
                Debug.LogWarning("Cannot toggle PvP right now");
                return false;
            }
            
            pvpEnabled = false;
            lastToggleTime = Time.time;
            
            Debug.Log($"{gameObject.name} disabled PvP mode");
            return true;
        }
        
        /// <summary>
        /// Toggle PvP mode
        /// Chuyển đổi chế độ PvP
        /// </summary>
        public bool TogglePvP()
        {
            if (pvpEnabled)
            {
                return DisablePvP();
            }
            else
            {
                return EnablePvP();
            }
        }
        
        /// <summary>
        /// Check if can toggle PvP
        /// Kiểm tra có thể chuyển đổi PvP không
        /// </summary>
        private bool CanToggle()
        {
            // Check cooldown
            if (Time.time - lastToggleTime < toggleCooldown)
            {
                return false;
            }
            
            // Check if in combat
            if (isInCombat && !allowToggleInCombat)
            {
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Set combat state
        /// Đặt trạng thái chiến đấu
        /// </summary>
        public void SetInCombat(bool inCombat)
        {
            isInCombat = inCombat;
        }
        
        /// <summary>
        /// Get remaining cooldown time
        /// Lấy thời gian cooldown còn lại
        /// </summary>
        public float GetRemainingCooldown()
        {
            float elapsed = Time.time - lastToggleTime;
            return Mathf.Max(0, toggleCooldown - elapsed);
        }
    }
}
