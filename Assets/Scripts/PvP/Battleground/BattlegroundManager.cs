using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Battleground Manager - Quản lý các chế độ battleground
    /// </summary>
    public class BattlegroundManager : MonoBehaviour
    {
        [Header("Available Modes")]
        public TeamDeathmatch teamDeathmatchPrefab;
        public CaptureTheFlag captureFlagPrefab;
        public KingOfTheHill kingOfHillPrefab;
        public BattleRoyale battleRoyalePrefab;
        
        // Active battlegrounds
        private List<BattlegroundMode> activeBattlegrounds = new List<BattlegroundMode>();
        
        // Queues for each mode
        private Dictionary<string, List<GameObject>> modeQueues = new Dictionary<string, List<GameObject>>();
        
        // Events
        public event Action<BattlegroundMode> OnBattlegroundStart;
        public event Action<BattlegroundMode, int> OnBattlegroundEnd;
        
        private void Awake()
        {
            InitializeQueues();
        }
        
        private void InitializeQueues()
        {
            modeQueues["TeamDeathmatch"] = new List<GameObject>();
            modeQueues["CaptureTheFlag"] = new List<GameObject>();
            modeQueues["KingOfTheHill"] = new List<GameObject>();
            modeQueues["BattleRoyale"] = new List<GameObject>();
        }
        
        /// <summary>
        /// Join battleground queue
        /// Tham gia hàng đợi battleground
        /// </summary>
        public void JoinQueue(GameObject player, string modeName)
        {
            if (!modeQueues.ContainsKey(modeName))
            {
                Debug.LogWarning($"Unknown battleground mode: {modeName}");
                return;
            }
            
            var queue = modeQueues[modeName];
            if (!queue.Contains(player))
            {
                queue.Add(player);
                Debug.Log($"{player.name} joined {modeName} queue");
                
                // Try to start match if enough players
                TryStartBattleground(modeName);
            }
        }
        
        /// <summary>
        /// Leave battleground queue
        /// Rời khỏi hàng đợi battleground
        /// </summary>
        public void LeaveQueue(GameObject player, string modeName)
        {
            if (!modeQueues.ContainsKey(modeName)) return;
            
            var queue = modeQueues[modeName];
            if (queue.Remove(player))
            {
                Debug.Log($"{player.name} left {modeName} queue");
            }
        }
        
        /// <summary>
        /// Try to start battleground if enough players
        /// Thử bắt đầu battleground nếu đủ người
        /// </summary>
        private void TryStartBattleground(string modeName)
        {
            var queue = modeQueues[modeName];
            int requiredPlayers = GetRequiredPlayers(modeName);
            
            if (queue.Count >= requiredPlayers)
            {
                StartBattleground(modeName, queue);
            }
        }
        
        /// <summary>
        /// Start a battleground match
        /// Bắt đầu trận battleground
        /// </summary>
        private void StartBattleground(string modeName, List<GameObject> players)
        {
            BattlegroundMode mode = CreateBattlegroundMode(modeName);
            if (mode == null) return;
            
            // Split players into teams
            List<GameObject> team1 = new List<GameObject>();
            List<GameObject> team2 = new List<GameObject>();
            
            for (int i = 0; i < players.Count; i++)
            {
                if (i % 2 == 0)
                    team1.Add(players[i]);
                else
                    team2.Add(players[i]);
            }
            
            // Initialize and start match
            mode.InitializeMatch(team1, team2);
            mode.StartMatch();
            
            activeBattlegrounds.Add(mode);
            
            // Subscribe to events
            mode.OnMatchEnd += (winningTeam) => OnBattlegroundComplete(mode, winningTeam);
            
            OnBattlegroundStart?.Invoke(mode);
            
            // Clear queue
            modeQueues[modeName].Clear();
        }
        
        /// <summary>
        /// Create battleground mode instance
        /// Tạo instance chế độ battleground
        /// </summary>
        private BattlegroundMode CreateBattlegroundMode(string modeName)
        {
            GameObject modeObject = new GameObject($"Battleground_{modeName}");
            
            switch (modeName)
            {
                case "TeamDeathmatch":
                    return modeObject.AddComponent<TeamDeathmatch>();
                case "CaptureTheFlag":
                    return modeObject.AddComponent<CaptureTheFlag>();
                case "KingOfTheHill":
                    return modeObject.AddComponent<KingOfTheHill>();
                case "BattleRoyale":
                    return modeObject.AddComponent<BattleRoyale>();
                default:
                    Destroy(modeObject);
                    return null;
            }
        }
        
        /// <summary>
        /// Handle battleground completion
        /// Xử lý khi battleground kết thúc
        /// </summary>
        private void OnBattlegroundComplete(BattlegroundMode mode, int winningTeam)
        {
            activeBattlegrounds.Remove(mode);
            OnBattlegroundEnd?.Invoke(mode, winningTeam);
            
            // Clean up mode object
            Destroy(mode.gameObject, 5f);
        }
        
        /// <summary>
        /// Get required players for mode
        /// Lấy số người chơi cần cho chế độ
        /// </summary>
        private int GetRequiredPlayers(string modeName)
        {
            switch (modeName)
            {
                case "TeamDeathmatch":
                    return 20; // 10v10
                case "CaptureTheFlag":
                    return 16; // 8v8
                case "KingOfTheHill":
                    return 12; // 6v6
                case "BattleRoyale":
                    return 10; // Minimum for BR
                default:
                    return 2;
            }
        }
        
        /// <summary>
        /// Get queue count for mode
        /// Lấy số người trong hàng đợi
        /// </summary>
        public int GetQueueCount(string modeName)
        {
            return modeQueues.ContainsKey(modeName) ? modeQueues[modeName].Count : 0;
        }
        
        /// <summary>
        /// Get active battleground count
        /// Lấy số battleground đang hoạt động
        /// </summary>
        public int GetActiveBattlegroundCount()
        {
            return activeBattlegrounds.Count;
        }
    }
}
