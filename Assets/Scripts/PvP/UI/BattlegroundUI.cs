using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Battleground UI - Giao diện battleground
    /// </summary>
    public class BattlegroundUI : MonoBehaviour
    {
        [Header("Mode Selection")]
        public Button tdmButton;
        public Button ctfButton;
        public Button kothButton;
        public Button brButton;
        public TextMeshProUGUI queueCountText;
        
        [Header("Match Info")]
        public GameObject matchPanel;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI objectiveText;
        
        private BattlegroundManager battlegroundManager;
        
        private void Start()
        {
            battlegroundManager = PvPManager.Instance?.GetBattlegroundManager();
            
            // Setup buttons
            if (tdmButton != null)
                tdmButton.onClick.AddListener(() => JoinQueue("TeamDeathmatch"));
            if (ctfButton != null)
                ctfButton.onClick.AddListener(() => JoinQueue("CaptureTheFlag"));
            if (kothButton != null)
                kothButton.onClick.AddListener(() => JoinQueue("KingOfTheHill"));
            if (brButton != null)
                brButton.onClick.AddListener(() => JoinQueue("BattleRoyale"));
            
            HideMatchPanel();
        }
        
        private void Update()
        {
            UpdateQueueCounts();
        }
        
        /// <summary>
        /// Join battleground queue
        /// Tham gia hàng đợi battleground
        /// </summary>
        private void JoinQueue(string modeName)
        {
            if (battlegroundManager == null) return;
            
            // TODO: Get local player
            GameObject player = null; // Replace with actual player
            if (player != null)
            {
                battlegroundManager.JoinQueue(player, modeName);
                Debug.Log($"Joined {modeName} queue");
            }
        }
        
        /// <summary>
        /// Update queue counts
        /// Cập nhật số người trong hàng đợi
        /// </summary>
        private void UpdateQueueCounts()
        {
            if (battlegroundManager == null || queueCountText == null) return;
            
            int tdmCount = battlegroundManager.GetQueueCount("TeamDeathmatch");
            int ctfCount = battlegroundManager.GetQueueCount("CaptureTheFlag");
            int kothCount = battlegroundManager.GetQueueCount("KingOfTheHill");
            int brCount = battlegroundManager.GetQueueCount("BattleRoyale");
            
            queueCountText.text = $"TDM: {tdmCount} | CTF: {ctfCount} | KOTH: {kothCount} | BR: {brCount}";
        }
        
        /// <summary>
        /// Show match panel
        /// Hiện panel trận đấu
        /// </summary>
        public void ShowMatchPanel()
        {
            if (matchPanel != null)
                matchPanel.SetActive(true);
        }
        
        /// <summary>
        /// Hide match panel
        /// Ẩn panel trận đấu
        /// </summary>
        public void HideMatchPanel()
        {
            if (matchPanel != null)
                matchPanel.SetActive(false);
        }
        
        /// <summary>
        /// Update match info
        /// Cập nhật thông tin trận đấu
        /// </summary>
        public void UpdateMatchInfo(float timeRemaining, string score, string objective)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(timeRemaining / 60f);
                int seconds = Mathf.FloorToInt(timeRemaining % 60f);
                timerText.text = $"{minutes}:{seconds:00}";
            }
            
            if (scoreText != null)
                scoreText.text = score;
            
            if (objectiveText != null)
                objectiveText.text = objective;
        }
    }
}
