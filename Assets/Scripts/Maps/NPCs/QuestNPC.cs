using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.NPCs
{
    /// <summary>
    /// NPC nhiệm vụ / Quest giver NPC
    /// </summary>
    public class QuestNPC : NPCBase
    {
        [Header("Quest Configuration")]
        [Tooltip("Danh sách quests có sẵn / Available quests")]
        [SerializeField] private List<QuestData> availableQuests = new List<QuestData>();
        
        [Tooltip("Icon quest / Quest icon")]
        [SerializeField] private Sprite questIcon;
        
        [Tooltip("Icon quest hoàn thành / Quest complete icon")]
        [SerializeField] private Sprite questCompleteIcon;
        
        [Header("Quest State")]
        [Tooltip("Có quest available / Has available quest")]
        [SerializeField] private bool hasAvailableQuest = true;
        
        private Dictionary<int, QuestState> playerQuestStates = new Dictionary<int, QuestState>();
        
        protected override void InitializeNPC()
        {
            base.InitializeNPC();
            
            Debug.Log($"[QuestNPC] Quest NPC initialized: {npcName} with {availableQuests.Count} quests");
        }
        
        protected override void OpenNPCUI(GameObject player)
        {
            Debug.Log($"[QuestNPC] Opening quest UI");
            
            // Check quest status for player
            QuestState state = GetPlayerQuestState(player);
            
            switch (state)
            {
                case QuestState.Available:
                    ShowAvailableQuests(player);
                    break;
                case QuestState.InProgress:
                    ShowQuestProgress(player);
                    break;
                case QuestState.ReadyToComplete:
                    ShowQuestCompletion(player);
                    break;
                case QuestState.Completed:
                    ShowDialog("Bạn đã hoàn thành tất cả nhiệm vụ của tôi. Cảm ơn bạn!");
                    break;
            }
        }
        
        /// <summary>
        /// Hiển thị quests có sẵn / Show available quests
        /// </summary>
        private void ShowAvailableQuests(GameObject player)
        {
            Debug.Log($"[QuestNPC] Showing {availableQuests.Count} available quests");
            // TODO: Show quest list UI
        }
        
        /// <summary>
        /// Hiển thị tiến độ quest / Show quest progress
        /// </summary>
        private void ShowQuestProgress(GameObject player)
        {
            Debug.Log($"[QuestNPC] Showing quest progress");
            // TODO: Show progress UI
        }
        
        /// <summary>
        /// Hiển thị hoàn thành quest / Show quest completion
        /// </summary>
        private void ShowQuestCompletion(GameObject player)
        {
            Debug.Log($"[QuestNPC] Quest ready to complete!");
            // TODO: Show completion UI with rewards
        }
        
        /// <summary>
        /// Nhận quest / Accept quest
        /// </summary>
        public bool AcceptQuest(GameObject player, QuestData quest)
        {
            // Check if player meets requirements
            if (!CanAcceptQuest(player, quest))
            {
                ShowDialog($"Bạn cần level {quest.requiredLevel} để nhận quest này!");
                return false;
            }
            
            // TODO: Add quest to player's quest log
            Debug.Log($"[QuestNPC] Player accepted quest: {quest.questName}");
            
            // Update state
            int playerId = player.GetInstanceID();
            playerQuestStates[playerId] = QuestState.InProgress;
            
            ShowDialog(quest.acceptMessage);
            return true;
        }
        
        /// <summary>
        /// Hoàn thành quest / Complete quest
        /// </summary>
        public bool CompleteQuest(GameObject player, QuestData quest)
        {
            // Check if quest objectives are met
            if (!AreObjectivesMet(player, quest))
            {
                ShowDialog("Bạn chưa hoàn thành tất cả mục tiêu!");
                return false;
            }
            
            // Give rewards
            GiveRewards(player, quest);
            
            // Update state
            int playerId = player.GetInstanceID();
            playerQuestStates[playerId] = QuestState.Completed;
            
            ShowDialog(quest.completeMessage);
            Debug.Log($"[QuestNPC] Quest completed: {quest.questName}");
            
            return true;
        }
        
        /// <summary>
        /// Kiểm tra có thể nhận quest / Check if can accept quest
        /// </summary>
        private bool CanAcceptQuest(GameObject player, QuestData quest)
        {
            // TODO: Check player level
            // TODO: Check prerequisite quests
            return true;
        }
        
        /// <summary>
        /// Kiểm tra mục tiêu đã hoàn thành / Check if objectives met
        /// </summary>
        private bool AreObjectivesMet(GameObject player, QuestData quest)
        {
            // TODO: Check quest objectives in player's quest log
            return true;
        }
        
        /// <summary>
        /// Trao thưởng / Give rewards
        /// </summary>
        private void GiveRewards(GameObject player, QuestData quest)
        {
            // TODO: Give EXP
            Debug.Log($"[QuestNPC] Rewarding {quest.expReward} EXP");
            
            // TODO: Give Zen
            if (quest.zenReward > 0)
            {
                Debug.Log($"[QuestNPC] Rewarding {quest.zenReward} Zen");
            }
            
            // TODO: Give items
            foreach (var item in quest.itemRewards)
            {
                Debug.Log($"[QuestNPC] Rewarding item: {item}");
            }
        }
        
        /// <summary>
        /// Lấy trạng thái quest của player / Get player quest state
        /// </summary>
        private QuestState GetPlayerQuestState(GameObject player)
        {
            int playerId = player.GetInstanceID();
            
            if (playerQuestStates.ContainsKey(playerId))
            {
                return playerQuestStates[playerId];
            }
            
            return QuestState.Available;
        }
        
        /// <summary>
        /// Lấy danh sách quests / Get available quests
        /// </summary>
        public List<QuestData> GetAvailableQuests()
        {
            return new List<QuestData>(availableQuests);
        }
    }
    
    /// <summary>
    /// Trạng thái quest / Quest state
    /// </summary>
    public enum QuestState
    {
        Available,          // Có thể nhận
        InProgress,         // Đang làm
        ReadyToComplete,    // Sẵn sàng hoàn thành
        Completed           // Đã hoàn thành
    }
    
    /// <summary>
    /// Dữ liệu quest / Quest data
    /// </summary>
    [System.Serializable]
    public class QuestData
    {
        [Tooltip("Tên quest / Quest name")]
        public string questName;
        
        [Tooltip("Mô tả / Description")]
        [TextArea(3, 5)]
        public string description;
        
        [Tooltip("Level yêu cầu / Required level")]
        public int requiredLevel = 1;
        
        [Tooltip("Lời nhận quest / Accept message")]
        public string acceptMessage;
        
        [Tooltip("Lời hoàn thành / Complete message")]
        public string completeMessage;
        
        [Header("Objectives")]
        [Tooltip("Mục tiêu / Quest objectives")]
        public List<QuestObjective> objectives = new List<QuestObjective>();
        
        [Header("Rewards")]
        [Tooltip("EXP thưởng / EXP reward")]
        public int expReward;
        
        [Tooltip("Zen thưởng / Zen reward")]
        public int zenReward;
        
        [Tooltip("Items thưởng / Item rewards")]
        public List<string> itemRewards = new List<string>();
    }
    
    /// <summary>
    /// Mục tiêu quest / Quest objective
    /// </summary>
    [System.Serializable]
    public class QuestObjective
    {
        [Tooltip("Loại mục tiêu / Objective type")]
        public ObjectiveType type;
        
        [Tooltip("Mô tả / Description")]
        public string description;
        
        [Tooltip("Mục tiêu (tên quái, item, vị trí) / Target")]
        public string target;
        
        [Tooltip("Số lượng cần / Required count")]
        public int requiredCount = 1;
        
        [Tooltip("Đã hoàn thành / Current count")]
        public int currentCount = 0;
    }
    
    /// <summary>
    /// Loại mục tiêu / Objective types
    /// </summary>
    public enum ObjectiveType
    {
        KillMonster,    // Giết quái
        CollectItem,    // Thu thập item
        TalkToNPC,      // Nói chuyện với NPC
        ReachLocation,  // Đến vị trí
        UseItem,        // Dùng item
        Escort          // Hộ tống
    }
}
