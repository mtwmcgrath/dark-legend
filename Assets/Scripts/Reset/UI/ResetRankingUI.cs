using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.Reset
{
    /// <summary>
    /// Reset ranking UI - UI bảng xếp hạng reset
    /// Displays reset rankings for all players
    /// </summary>
    public class ResetRankingUI : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("Ranking panel - Panel bảng xếp hạng")]
        public GameObject rankingPanel;

        [Tooltip("Title text - Text tiêu đề")]
        public Text titleText;

        [Tooltip("Ranking list content - Nội dung danh sách xếp hạng")]
        public Transform rankingListContent;

        [Tooltip("Ranking entry prefab - Prefab mục xếp hạng")]
        public GameObject rankingEntryPrefab;

        [Tooltip("Filter dropdown - Dropdown lọc")]
        public Dropdown filterDropdown;

        [Tooltip("Close button - Nút đóng")]
        public Button closeButton;

        [Header("Settings")]
        [Tooltip("Max rankings to display - Số xếp hạng hiển thị tối đa")]
        public int maxRankingsToDisplay = 100;

        [Tooltip("Highlight player - Làm nổi bật người chơi")]
        public bool highlightCurrentPlayer = true;

        private static ResetRankingUI _instance;
        public static ResetRankingUI Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ResetRankingUI>();
                }
                return _instance;
            }
        }

        private CharacterStats currentPlayer;
        private List<GameObject> entryObjects = new List<GameObject>();
        private RankingFilter currentFilter = RankingFilter.TotalResets;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeUI();
        }

        private void InitializeUI()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            if (filterDropdown != null)
            {
                filterDropdown.ClearOptions();
                filterDropdown.AddOptions(new List<string>
                {
                    "Total Resets",
                    "Normal Resets",
                    "Grand Resets",
                    "Master Resets"
                });
                filterDropdown.onValueChanged.AddListener(OnFilterChanged);
            }

            Hide();
        }

        /// <summary>
        /// Show ranking
        /// Hiển thị bảng xếp hạng
        /// </summary>
        public void Show(CharacterStats player = null)
        {
            currentPlayer = player;

            if (rankingPanel != null)
                rankingPanel.SetActive(true);

            UpdateUI();
        }

        /// <summary>
        /// Hide ranking panel
        /// Ẩn panel bảng xếp hạng
        /// </summary>
        public void Hide()
        {
            if (rankingPanel != null)
                rankingPanel.SetActive(false);

            ClearRankingList();
            currentPlayer = null;
        }

        /// <summary>
        /// Update UI content
        /// Cập nhật nội dung UI
        /// </summary>
        private void UpdateUI()
        {
            UpdateTitle();
            UpdateRankingList();
        }

        /// <summary>
        /// Update title
        /// Cập nhật tiêu đề
        /// </summary>
        private void UpdateTitle()
        {
            if (titleText == null)
                return;

            string title = $"Reset Rankings - {currentFilter}";
            titleText.text = title;
        }

        /// <summary>
        /// Update ranking list
        /// Cập nhật danh sách xếp hạng
        /// </summary>
        private void UpdateRankingList()
        {
            ClearRankingList();

            // Get rankings based on filter
            List<ResetRankEntry> rankings = GetRankings(currentFilter);

            if (rankings.Count == 0)
            {
                CreateNoRankingsEntry();
                return;
            }

            // Display top rankings
            int displayCount = Mathf.Min(maxRankingsToDisplay, rankings.Count);
            for (int i = 0; i < displayCount; i++)
            {
                CreateRankingEntry(rankings[i], i + 1);
            }
        }

        /// <summary>
        /// Get rankings based on filter
        /// Lấy bảng xếp hạng dựa trên bộ lọc
        /// </summary>
        private List<ResetRankEntry> GetRankings(RankingFilter filter)
        {
            // This is a placeholder implementation
            // In a real game, you would fetch this from a server or database
            // Đây là implementation tạm thời
            // Trong game thực tế, bạn sẽ lấy từ server hoặc database

            List<ResetRankEntry> rankings = new List<ResetRankEntry>();

            // For demonstration, create some sample data
            // Để demo, tạo một số dữ liệu mẫu
            if (currentPlayer != null)
            {
                rankings.Add(new ResetRankEntry
                {
                    playerName = currentPlayer.name,
                    characterClass = "Dark Knight",
                    normalResets = currentPlayer.normalResetCount,
                    grandResets = currentPlayer.grandResetCount,
                    hasMasterReset = currentPlayer.hasMasterReset,
                    totalResetPower = currentPlayer.resetHistory?.GetTotalResetPower() ?? 0
                });
            }

            // Sort based on filter
            switch (filter)
            {
                case RankingFilter.TotalResets:
                    rankings = rankings.OrderByDescending(r => r.totalResetPower).ToList();
                    break;
                case RankingFilter.NormalResets:
                    rankings = rankings.OrderByDescending(r => r.normalResets).ToList();
                    break;
                case RankingFilter.GrandResets:
                    rankings = rankings.OrderByDescending(r => r.grandResets).ToList();
                    break;
                case RankingFilter.MasterResets:
                    rankings = rankings.OrderByDescending(r => r.hasMasterReset ? 1 : 0).ToList();
                    break;
            }

            return rankings;
        }

        /// <summary>
        /// Create a ranking entry
        /// Tạo một mục xếp hạng
        /// </summary>
        private void CreateRankingEntry(ResetRankEntry entry, int rank)
        {
            if (rankingListContent == null)
                return;

            GameObject entryObj;

            if (rankingEntryPrefab != null)
            {
                entryObj = Instantiate(rankingEntryPrefab, rankingListContent);
            }
            else
            {
                // Create simple text entry if no prefab
                entryObj = new GameObject("RankingEntry");
                entryObj.transform.SetParent(rankingListContent);
                Text text = entryObj.AddComponent<Text>();
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 14;
            }

            // Set entry text
            Text entryText = entryObj.GetComponentInChildren<Text>();
            if (entryText != null)
            {
                entryText.text = FormatRankingEntry(entry, rank);

                // Highlight current player
                if (highlightCurrentPlayer && currentPlayer != null && entry.playerName == currentPlayer.name)
                {
                    entryText.color = Color.yellow;
                    entryText.fontStyle = FontStyle.Bold;
                }
            }

            entryObjects.Add(entryObj);
        }

        /// <summary>
        /// Format ranking entry for display
        /// Định dạng mục xếp hạng để hiển thị
        /// </summary>
        private string FormatRankingEntry(ResetRankEntry entry, int rank)
        {
            string rankIcon = GetRankIcon(rank);
            
            string formatted = $"{rankIcon} #{rank} {entry.playerName} ({entry.characterClass})\n";
            formatted += $"   Normal: {entry.normalResets} | Grand: {entry.grandResets} | Master: {(entry.hasMasterReset ? "Yes" : "No")}\n";
            formatted += $"   Reset Power: {entry.totalResetPower:N0}\n";

            return formatted;
        }

        /// <summary>
        /// Get rank icon
        /// Lấy icon xếp hạng
        /// </summary>
        private string GetRankIcon(int rank)
        {
            switch (rank)
            {
                case 1:
                    return "🥇";
                case 2:
                    return "🥈";
                case 3:
                    return "🥉";
                default:
                    return "  ";
            }
        }

        /// <summary>
        /// Create entry when no rankings exist
        /// Tạo mục khi không có xếp hạng
        /// </summary>
        private void CreateNoRankingsEntry()
        {
            if (rankingListContent == null)
                return;

            GameObject entryObj = new GameObject("NoRankingsEntry");
            entryObj.transform.SetParent(rankingListContent);
            Text text = entryObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = "No rankings available yet.\nBe the first to reset!";

            entryObjects.Add(entryObj);
        }

        /// <summary>
        /// Clear ranking list
        /// Xóa danh sách xếp hạng
        /// </summary>
        private void ClearRankingList()
        {
            foreach (GameObject obj in entryObjects)
            {
                if (obj != null)
                    Destroy(obj);
            }
            entryObjects.Clear();
        }

        /// <summary>
        /// Handle filter dropdown change
        /// Xử lý thay đổi dropdown lọc
        /// </summary>
        private void OnFilterChanged(int value)
        {
            currentFilter = (RankingFilter)value;
            UpdateUI();
        }
    }

    /// <summary>
    /// Reset rank entry - Mục xếp hạng reset
    /// </summary>
    public class ResetRankEntry
    {
        public string playerName;
        public string characterClass;
        public int normalResets;
        public int grandResets;
        public bool hasMasterReset;
        public int totalResetPower;
    }

    /// <summary>
    /// Ranking filter type - Loại bộ lọc xếp hạng
    /// </summary>
    public enum RankingFilter
    {
        TotalResets,
        NormalResets,
        GrandResets,
        MasterResets
    }
}
