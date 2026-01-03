using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Ranking UI - Giao diện xếp hạng
    /// </summary>
    public class RankingUI : MonoBehaviour
    {
        [Header("Mode Tabs")]
        public Button tab1v1;
        public Button tab2v2;
        public Button tab3v3;
        public Button tab5v5;
        public Button tabOverall;
        
        [Header("Leaderboard")]
        public Transform leaderboardContent;
        public GameObject leaderboardEntryPrefab;
        
        [Header("Player Info")]
        public TextMeshProUGUI playerRankText;
        public TextMeshProUGUI playerRatingText;
        public TextMeshProUGUI playerRecordText;
        public TextMeshProUGUI playerTierText;
        
        [Header("Pagination")]
        public Button prevPageButton;
        public Button nextPageButton;
        public TextMeshProUGUI pageText;
        
        private PvPRankingSystem rankingSystem;
        private Leaderboard leaderboard;
        private ArenaMode currentMode = ArenaMode.Solo1v1;
        private int currentPage = 1;
        
        private void Start()
        {
            rankingSystem = PvPManager.Instance?.GetRankingSystem();
            if (rankingSystem != null)
            {
                leaderboard = rankingSystem.GetLeaderboard();
            }
            
            // Setup tab buttons
            if (tab1v1 != null)
                tab1v1.onClick.AddListener(() => SwitchMode(ArenaMode.Solo1v1));
            if (tab2v2 != null)
                tab2v2.onClick.AddListener(() => SwitchMode(ArenaMode.Solo2v2));
            if (tab3v3 != null)
                tab3v3.onClick.AddListener(() => SwitchMode(ArenaMode.Solo3v3));
            if (tab5v5 != null)
                tab5v5.onClick.AddListener(() => SwitchMode(ArenaMode.Team5v5));
            
            // Setup pagination
            if (prevPageButton != null)
                prevPageButton.onClick.AddListener(PreviousPage);
            if (nextPageButton != null)
                nextPageButton.onClick.AddListener(NextPage);
            
            RefreshLeaderboard();
        }
        
        /// <summary>
        /// Switch arena mode
        /// Chuyển chế độ đấu trường
        /// </summary>
        private void SwitchMode(ArenaMode mode)
        {
            currentMode = mode;
            currentPage = 1;
            RefreshLeaderboard();
        }
        
        /// <summary>
        /// Refresh leaderboard display
        /// Làm mới hiển thị bảng xếp hạng
        /// </summary>
        private void RefreshLeaderboard()
        {
            if (leaderboard == null || leaderboardContent == null) return;
            
            // Clear existing entries
            foreach (Transform child in leaderboardContent)
            {
                Destroy(child.gameObject);
            }
            
            // Get entries for current page
            var entries = leaderboard.GetEntriesByPage(currentMode, currentPage);
            
            // Create UI entries
            foreach (var entry in entries)
            {
                CreateLeaderboardEntry(entry);
            }
            
            // Update pagination
            UpdatePagination();
            
            // Update player info
            UpdatePlayerInfo();
        }
        
        /// <summary>
        /// Create leaderboard entry UI
        /// Tạo UI mục xếp hạng
        /// </summary>
        private void CreateLeaderboardEntry(LeaderboardEntry entry)
        {
            if (leaderboardEntryPrefab == null || leaderboardContent == null) return;
            
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            
            // Set entry data (assuming entry prefab has these components)
            var rankText = entryObj.transform.Find("RankText")?.GetComponent<TextMeshProUGUI>();
            var nameText = entryObj.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            var ratingText = entryObj.transform.Find("RatingText")?.GetComponent<TextMeshProUGUI>();
            var recordText = entryObj.transform.Find("RecordText")?.GetComponent<TextMeshProUGUI>();
            
            if (rankText != null)
                rankText.text = $"#{entry.rank}";
            if (nameText != null)
                nameText.text = entry.playerName;
            if (ratingText != null)
                ratingText.text = entry.rating.ToString();
            if (recordText != null)
                recordText.text = $"{entry.wins}W - {entry.losses}L";
        }
        
        /// <summary>
        /// Update pagination buttons
        /// Cập nhật nút phân trang
        /// </summary>
        private void UpdatePagination()
        {
            if (leaderboard == null) return;
            
            int totalPages = leaderboard.GetTotalPages(currentMode);
            
            if (prevPageButton != null)
                prevPageButton.interactable = currentPage > 1;
            if (nextPageButton != null)
                nextPageButton.interactable = currentPage < totalPages;
            if (pageText != null)
                pageText.text = $"Page {currentPage} / {totalPages}";
        }
        
        /// <summary>
        /// Go to previous page
        /// Chuyển trang trước
        /// </summary>
        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                RefreshLeaderboard();
            }
        }
        
        /// <summary>
        /// Go to next page
        /// Chuyển trang sau
        /// </summary>
        private void NextPage()
        {
            if (leaderboard != null)
            {
                int totalPages = leaderboard.GetTotalPages(currentMode);
                if (currentPage < totalPages)
                {
                    currentPage++;
                    RefreshLeaderboard();
                }
            }
        }
        
        /// <summary>
        /// Update player info panel
        /// Cập nhật panel thông tin người chơi
        /// </summary>
        private void UpdatePlayerInfo()
        {
            // TODO: Get local player ID and update info
            string playerId = ""; // Replace with actual player ID
            
            if (leaderboard != null && !string.IsNullOrEmpty(playerId))
            {
                var entry = leaderboard.GetPlayerEntry(currentMode, playerId);
                if (entry != null)
                {
                    if (playerRankText != null)
                        playerRankText.text = $"Your Rank: #{entry.rank}";
                    if (playerRatingText != null)
                        playerRatingText.text = $"Rating: {entry.rating}";
                    if (playerRecordText != null)
                        playerRecordText.text = $"Record: {entry.wins}W - {entry.losses}L";
                    if (playerTierText != null)
                        playerTierText.text = PvPRankingSystem.GetRankTierName(entry.tier);
                }
            }
        }
    }
}
