using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Arena UI - Giao diện đấu trường
    /// </summary>
    public class ArenaUI : MonoBehaviour
    {
        [Header("Queue Panel")]
        public GameObject queuePanel;
        public TMP_Dropdown modeDropdown;
        public Button joinQueueButton;
        public Button leaveQueueButton;
        public TextMeshProUGUI queueStatusText;
        public TextMeshProUGUI queueTimeText;
        public TextMeshProUGUI playersInQueueText;
        
        [Header("Player Info")]
        public TextMeshProUGUI ratingText;
        public TextMeshProUGUI rankText;
        public TextMeshProUGUI recordText;
        
        [Header("Match Info")]
        public GameObject matchPanel;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        public GameObject team1Panel;
        public GameObject team2Panel;
        
        private ArenaManager arenaManager;
        private ArenaMode currentMode = ArenaMode.Solo1v1;
        private bool inQueue = false;
        private float queueStartTime = 0f;
        
        private void Start()
        {
            arenaManager = PvPManager.Instance?.GetArenaManager();
            
            // Setup buttons
            if (joinQueueButton != null)
                joinQueueButton.onClick.AddListener(OnJoinQueueClicked);
            if (leaveQueueButton != null)
                leaveQueueButton.onClick.AddListener(OnLeaveQueueClicked);
            
            // Setup dropdown
            if (modeDropdown != null)
            {
                modeDropdown.ClearOptions();
                modeDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Solo 1v1", "Solo 2v2", "Solo 3v3", "Team 5v5", "Free For All"
                });
                modeDropdown.onValueChanged.AddListener(OnModeChanged);
            }
            
            HideMatchPanel();
            UpdateUI();
        }
        
        private void Update()
        {
            if (inQueue)
            {
                UpdateQueueInfo();
            }
        }
        
        /// <summary>
        /// Join queue button clicked
        /// Nhấn nút tham gia hàng đợi
        /// </summary>
        private void OnJoinQueueClicked()
        {
            if (arenaManager == null) return;
            
            // TODO: Get local player
            GameObject player = null; // Replace with actual player
            if (player != null)
            {
                arenaManager.JoinQueue(player, currentMode);
                inQueue = true;
                queueStartTime = Time.time;
                UpdateQueueStatus("Searching for match...");
            }
        }
        
        /// <summary>
        /// Leave queue button clicked
        /// Nhấn nút rời khỏi hàng đợi
        /// </summary>
        private void OnLeaveQueueClicked()
        {
            if (arenaManager == null) return;
            
            // TODO: Get local player
            GameObject player = null; // Replace with actual player
            if (player != null)
            {
                arenaManager.LeaveQueue(player);
                inQueue = false;
                UpdateQueueStatus("Not in queue");
            }
        }
        
        /// <summary>
        /// Mode dropdown changed
        /// Thay đổi chế độ
        /// </summary>
        private void OnModeChanged(int index)
        {
            currentMode = (ArenaMode)index;
            UpdateUI();
        }
        
        /// <summary>
        /// Update queue information
        /// Cập nhật thông tin hàng đợi
        /// </summary>
        private void UpdateQueueInfo()
        {
            float queueTime = Time.time - queueStartTime;
            int minutes = Mathf.FloorToInt(queueTime / 60f);
            int seconds = Mathf.FloorToInt(queueTime % 60f);
            
            if (queueTimeText != null)
            {
                queueTimeText.text = $"Time: {minutes}:{seconds:00}";
            }
            
            // TODO: Get actual players in queue count
            if (playersInQueueText != null)
            {
                playersInQueueText.text = "Players in queue: --";
            }
        }
        
        /// <summary>
        /// Update queue status text
        /// Cập nhật text trạng thái hàng đợi
        /// </summary>
        private void UpdateQueueStatus(string status)
        {
            if (queueStatusText != null)
            {
                queueStatusText.text = status;
            }
        }
        
        /// <summary>
        /// Update UI
        /// Cập nhật giao diện
        /// </summary>
        private void UpdateUI()
        {
            // TODO: Update rating, rank, record from player data
            if (ratingText != null)
                ratingText.text = "Rating: --";
            if (rankText != null)
                rankText.text = "Rank: --";
            if (recordText != null)
                recordText.text = "Record: 0W - 0L";
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
    }
}
