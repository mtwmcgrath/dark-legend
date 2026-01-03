using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PK Status UI - Giao diện trạng thái PK
    /// Displays player's PK status and name color
    /// </summary>
    public class PKStatusUI : MonoBehaviour
    {
        [Header("UI Elements")]
        public TextMeshProUGUI statusText;
        public TextMeshProUGUI pkCountText;
        public Image statusIndicator;
        public GameObject bountyPanel;
        public TextMeshProUGUI bountyAmountText;
        
        [Header("Status Colors")]
        public Color normalColor = Color.white;
        public Color selfDefenseColor = Color.yellow;
        public Color murdererColor = new Color(1f, 0.5f, 0f); // Orange
        public Color outlawColor = Color.red;
        public Color heroColor = Color.cyan;
        
        private PKSystem pkSystem;
        private BountySystem bountySystem;
        private string playerId;
        
        private void Start()
        {
            var openWorldPvP = PvPManager.Instance?.GetComponent<OpenWorldPvP>();
            if (openWorldPvP != null)
            {
                pkSystem = openWorldPvP.GetPKSystem();
                bountySystem = openWorldPvP.GetBountySystem();
                
                if (pkSystem != null)
                {
                    pkSystem.OnPKStatusChanged += OnPKStatusChanged;
                }
            }
            
            // TODO: Get local player ID
            playerId = ""; // Replace with actual player ID
            
            UpdateUI();
        }
        
        private void OnDestroy()
        {
            if (pkSystem != null)
            {
                pkSystem.OnPKStatusChanged -= OnPKStatusChanged;
            }
        }
        
        private void Update()
        {
            UpdateUI();
        }
        
        /// <summary>
        /// Handle PK status change event
        /// Xử lý sự kiện thay đổi trạng thái PK
        /// </summary>
        private void OnPKStatusChanged(GameObject player, PKStatus status)
        {
            // Check if it's the local player
            // TODO: Compare with actual player
            UpdateUI();
        }
        
        /// <summary>
        /// Update UI display
        /// Cập nhật hiển thị UI
        /// </summary>
        private void UpdateUI()
        {
            if (pkSystem == null || string.IsNullOrEmpty(playerId)) return;
            
            var pkData = pkSystem.GetPKData(playerId);
            
            // Update status text
            if (statusText != null)
            {
                statusText.text = GetStatusText(pkData.status);
                statusText.color = GetStatusColor(pkData.status);
            }
            
            // Update PK count
            if (pkCountText != null)
            {
                pkCountText.text = $"PK Count: {pkData.pkCount}";
            }
            
            // Update status indicator
            if (statusIndicator != null)
            {
                statusIndicator.color = GetStatusColor(pkData.status);
            }
            
            // Update bounty display
            UpdateBountyDisplay();
        }
        
        /// <summary>
        /// Update bounty display
        /// Cập nhật hiển thị truy nã
        /// </summary>
        private void UpdateBountyDisplay()
        {
            if (bountySystem == null || bountyPanel == null) return;
            
            // TODO: Get local player GameObject
            GameObject player = null; // Replace with actual player
            if (player != null)
            {
                int bountyAmount = bountySystem.GetBountyAmount(player);
                
                if (bountyAmount > 0)
                {
                    bountyPanel.SetActive(true);
                    if (bountyAmountText != null)
                    {
                        bountyAmountText.text = $"Bounty: {bountyAmount:N0} Zen";
                    }
                }
                else
                {
                    bountyPanel.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Get status text
        /// Lấy text trạng thái
        /// </summary>
        private string GetStatusText(PKStatus status)
        {
            switch (status)
            {
                case PKStatus.Normal:
                    return "Normal";
                case PKStatus.SelfDefense:
                    return "Self Defense";
                case PKStatus.Murderer:
                    return "Murderer";
                case PKStatus.Outlaw:
                    return "Outlaw";
                case PKStatus.Hero:
                    return "Hero";
                default:
                    return "Unknown";
            }
        }
        
        /// <summary>
        /// Get status color
        /// Lấy màu trạng thái
        /// </summary>
        private Color GetStatusColor(PKStatus status)
        {
            switch (status)
            {
                case PKStatus.Normal:
                    return normalColor;
                case PKStatus.SelfDefense:
                    return selfDefenseColor;
                case PKStatus.Murderer:
                    return murdererColor;
                case PKStatus.Outlaw:
                    return outlawColor;
                case PKStatus.Hero:
                    return heroColor;
                default:
                    return normalColor;
            }
        }
    }
}
