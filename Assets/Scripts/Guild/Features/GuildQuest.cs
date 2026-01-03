using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild quest system with daily, weekly and special quests
    /// Hệ thống nhiệm vụ guild với nhiệm vụ hàng ngày, hàng tuần và đặc biệt
    /// </summary>
    public class GuildQuest : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        [Header("Quest Configuration")]
        [SerializeField] private List<GuildQuestConfig> availableQuests;
        
        // Active quests per guild / Nhiệm vụ đang hoạt động cho mỗi guild
        private Dictionary<string, List<ActiveGuildQuest>> activeQuests = new Dictionary<string, List<ActiveGuildQuest>>();
        
        /// <summary>
        /// Guild quest configuration
        /// Cấu hình nhiệm vụ guild
        /// </summary>
        [Serializable]
        public class GuildQuestConfig
        {
            public string QuestId;
            public string QuestName;
            public QuestType Type;
            public string Description;
            public int MinGuildLevel;
            public List<QuestObjective> Objectives;
            public GuildQuestReward Rewards;
        }
        
        /// <summary>
        /// Quest types
        /// Loại nhiệm vụ
        /// </summary>
        public enum QuestType
        {
            Daily,      // Hàng ngày
            Weekly,     // Hàng tuần
            Special     // Đặc biệt
        }
        
        /// <summary>
        /// Quest objective types
        /// Loại mục tiêu nhiệm vụ
        /// </summary>
        public enum ObjectiveType
        {
            KillMonsters,       // Giết quái vật
            CompleteDungeons,   // Hoàn thành던전
            WinGuildWars,       // Thắng Guild War
            CollectItems,       // Thu thập vật phẩm
            ReachArea,          // Đến khu vực
            EarnGold,           // Kiếm tiền
            CompleteEvents      // Hoàn thành sự kiện
        }
        
        /// <summary>
        /// Quest objective
        /// Mục tiêu nhiệm vụ
        /// </summary>
        [Serializable]
        public class QuestObjective
        {
            public string ObjectiveId;
            public ObjectiveType Type;
            public string TargetName;       // Monster name, dungeon name, etc.
            public int RequiredCount;
            public string Description;
        }
        
        /// <summary>
        /// Quest rewards
        /// Phần thưởng nhiệm vụ
        /// </summary>
        [Serializable]
        public class GuildQuestReward
        {
            public int GuildEXP;
            public int GuildPoints;
            public int Zen;
            public List<string> ItemRewards;  // Item IDs
        }
        
        /// <summary>
        /// Active guild quest
        /// Nhiệm vụ guild đang hoạt động
        /// </summary>
        [Serializable]
        public class ActiveGuildQuest
        {
            public string QuestId;
            public string QuestName;
            public QuestType Type;
            public string Description;
            public DateTime StartTime;
            public DateTime ExpirationTime;
            public List<QuestProgress> Progress;
            public GuildQuestReward Rewards;
            public bool IsCompleted;
            
            public bool IsExpired => DateTime.Now > ExpirationTime;
            public float CompletionPercentage
            {
                get
                {
                    if (Progress.Count == 0) return 0f;
                    return Progress.Average(p => p.CompletionPercentage);
                }
            }
        }
        
        /// <summary>
        /// Progress for a quest objective
        /// Tiến độ cho mục tiêu nhiệm vụ
        /// </summary>
        [Serializable]
        public class QuestProgress
        {
            public string ObjectiveId;
            public int CurrentCount;
            public int RequiredCount;
            public bool IsCompleted => CurrentCount >= RequiredCount;
            public float CompletionPercentage => RequiredCount > 0 ? (float)CurrentCount / RequiredCount : 0f;
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
            
            InitializeDefaultQuests();
        }
        
        private void Update()
        {
            CleanupExpiredQuests();
        }
        
        /// <summary>
        /// Initialize default guild quests
        /// Khởi tạo nhiệm vụ guild mặc định
        /// </summary>
        private void InitializeDefaultQuests()
        {
            if (availableQuests == null || availableQuests.Count == 0)
            {
                availableQuests = new List<GuildQuestConfig>
                {
                    // Daily Quests
                    new GuildQuestConfig
                    {
                        QuestId = "daily_kill_100",
                        QuestName = "Monster Slayer",
                        Type = QuestType.Daily,
                        Description = "Kill 100 monsters as a guild",
                        MinGuildLevel = 1,
                        Objectives = new List<QuestObjective>
                        {
                            new QuestObjective
                            {
                                ObjectiveId = "obj_1",
                                Type = ObjectiveType.KillMonsters,
                                TargetName = "Any",
                                RequiredCount = 100,
                                Description = "Kill 100 monsters"
                            }
                        },
                        Rewards = new GuildQuestReward
                        {
                            GuildEXP = 500,
                            GuildPoints = 50,
                            Zen = 10000
                        }
                    },
                    new GuildQuestConfig
                    {
                        QuestId = "daily_dungeon_5",
                        QuestName = "Dungeon Explorers",
                        Type = QuestType.Daily,
                        Description = "Complete 5 dungeons as a guild",
                        MinGuildLevel = 5,
                        Objectives = new List<QuestObjective>
                        {
                            new QuestObjective
                            {
                                ObjectiveId = "obj_1",
                                Type = ObjectiveType.CompleteDungeons,
                                TargetName = "Any",
                                RequiredCount = 5,
                                Description = "Complete 5 dungeons"
                            }
                        },
                        Rewards = new GuildQuestReward
                        {
                            GuildEXP = 1000,
                            GuildPoints = 100,
                            Zen = 25000
                        }
                    },
                    // Weekly Quests
                    new GuildQuestConfig
                    {
                        QuestId = "weekly_kill_1000",
                        QuestName = "Elite Hunters",
                        Type = QuestType.Weekly,
                        Description = "Kill 1000 monsters as a guild",
                        MinGuildLevel = 1,
                        Objectives = new List<QuestObjective>
                        {
                            new QuestObjective
                            {
                                ObjectiveId = "obj_1",
                                Type = ObjectiveType.KillMonsters,
                                TargetName = "Any",
                                RequiredCount = 1000,
                                Description = "Kill 1000 monsters"
                            }
                        },
                        Rewards = new GuildQuestReward
                        {
                            GuildEXP = 5000,
                            GuildPoints = 500,
                            Zen = 100000
                        }
                    },
                    new GuildQuestConfig
                    {
                        QuestId = "weekly_war_3",
                        QuestName = "War Champions",
                        Type = QuestType.Weekly,
                        Description = "Win 3 Guild Wars",
                        MinGuildLevel = 10,
                        Objectives = new List<QuestObjective>
                        {
                            new QuestObjective
                            {
                                ObjectiveId = "obj_1",
                                Type = ObjectiveType.WinGuildWars,
                                TargetName = "Any",
                                RequiredCount = 3,
                                Description = "Win 3 Guild Wars"
                            }
                        },
                        Rewards = new GuildQuestReward
                        {
                            GuildEXP = 10000,
                            GuildPoints = 1000,
                            Zen = 500000
                        }
                    }
                };
            }
        }
        
        /// <summary>
        /// Start a guild quest
        /// Bắt đầu nhiệm vụ guild
        /// </summary>
        public bool StartQuest(string guildId, string playerId, string questId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null)
            {
                Debug.LogError("Member not found.");
                return false;
            }
            
            GuildRankPermissions permissions = member.GetPermissions();
            if (!permissions.CanStartGuildQuest)
            {
                Debug.LogError("You don't have permission to start guild quests.");
                return false;
            }
            
            GuildQuestConfig config = availableQuests.FirstOrDefault(q => q.QuestId == questId);
            if (config == null)
            {
                Debug.LogError("Quest not found.");
                return false;
            }
            
            // Check guild level requirement
            if (guild.Level < config.MinGuildLevel)
            {
                Debug.LogError($"Guild level {config.MinGuildLevel} required.");
                return false;
            }
            
            // Check if quest is already active
            if (!activeQuests.ContainsKey(guildId))
            {
                activeQuests[guildId] = new List<ActiveGuildQuest>();
            }
            
            if (activeQuests[guildId].Any(q => q.QuestId == questId && !q.IsCompleted && !q.IsExpired))
            {
                Debug.LogError("This quest is already active.");
                return false;
            }
            
            // Create active quest
            ActiveGuildQuest activeQuest = new ActiveGuildQuest
            {
                QuestId = config.QuestId,
                QuestName = config.QuestName,
                Type = config.Type,
                Description = config.Description,
                StartTime = DateTime.Now,
                ExpirationTime = GetExpirationTime(config.Type),
                Progress = config.Objectives.Select(obj => new QuestProgress
                {
                    ObjectiveId = obj.ObjectiveId,
                    CurrentCount = 0,
                    RequiredCount = obj.RequiredCount
                }).ToList(),
                Rewards = config.Rewards,
                IsCompleted = false
            };
            
            activeQuests[guildId].Add(activeQuest);
            
            Debug.Log($"Guild quest '{config.QuestName}' started!");
            return true;
        }
        
        /// <summary>
        /// Update quest progress
        /// Cập nhật tiến độ nhiệm vụ
        /// </summary>
        public bool UpdateProgress(string guildId, string questId, string objectiveId, int amount = 1)
        {
            if (!activeQuests.ContainsKey(guildId))
            {
                return false;
            }
            
            ActiveGuildQuest quest = activeQuests[guildId]
                .FirstOrDefault(q => q.QuestId == questId && !q.IsCompleted && !q.IsExpired);
            
            if (quest == null)
            {
                return false;
            }
            
            QuestProgress progress = quest.Progress.FirstOrDefault(p => p.ObjectiveId == objectiveId);
            if (progress == null)
            {
                return false;
            }
            
            progress.CurrentCount += amount;
            
            // Check if quest is completed
            if (quest.Progress.All(p => p.IsCompleted))
            {
                CompleteQuest(guildId, questId);
            }
            
            return true;
        }
        
        /// <summary>
        /// Complete guild quest and grant rewards
        /// Hoàn thành nhiệm vụ guild và trao phần thưởng
        /// </summary>
        private void CompleteQuest(string guildId, string questId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null) return;
            
            if (!activeQuests.ContainsKey(guildId))
            {
                return;
            }
            
            ActiveGuildQuest quest = activeQuests[guildId]
                .FirstOrDefault(q => q.QuestId == questId);
            
            if (quest == null || quest.IsCompleted)
            {
                return;
            }
            
            quest.IsCompleted = true;
            
            // Grant rewards
            GuildData data = guildManager.GetGuildData();
            guild.AddExperience(quest.Rewards.GuildEXP, data);
            guild.Points += quest.Rewards.GuildPoints;
            
            Debug.Log($"Guild quest '{quest.QuestName}' completed!");
            Debug.Log($"Rewards: {quest.Rewards.GuildEXP} EXP, {quest.Rewards.GuildPoints} Points, {quest.Rewards.Zen} Zen");
            
            OnQuestCompleted(guild, quest);
        }
        
        /// <summary>
        /// Get active quests for guild
        /// Lấy nhiệm vụ đang hoạt động cho guild
        /// </summary>
        public List<ActiveGuildQuest> GetActiveQuests(string guildId)
        {
            if (!activeQuests.ContainsKey(guildId))
            {
                return new List<ActiveGuildQuest>();
            }
            
            return activeQuests[guildId]
                .Where(q => !q.IsCompleted && !q.IsExpired)
                .ToList();
        }
        
        /// <summary>
        /// Get available quests for guild
        /// Lấy nhiệm vụ có thể nhận cho guild
        /// </summary>
        public List<GuildQuestConfig> GetAvailableQuests(string guildId, QuestType? type = null)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return new List<GuildQuestConfig>();
            }
            
            var quests = availableQuests.Where(q => guild.Level >= q.MinGuildLevel);
            
            if (type.HasValue)
            {
                quests = quests.Where(q => q.Type == type.Value);
            }
            
            return quests.ToList();
        }
        
        /// <summary>
        /// Get expiration time based on quest type
        /// Lấy thời gian hết hạn dựa trên loại nhiệm vụ
        /// </summary>
        private DateTime GetExpirationTime(QuestType type)
        {
            switch (type)
            {
                case QuestType.Daily:
                    return DateTime.Now.Date.AddDays(1).AddHours(23).AddMinutes(59);
                case QuestType.Weekly:
                    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)DateTime.Now.DayOfWeek + 7) % 7;
                    if (daysUntilMonday == 0) daysUntilMonday = 7;
                    return DateTime.Now.Date.AddDays(daysUntilMonday).AddHours(23).AddMinutes(59);
                case QuestType.Special:
                    return DateTime.Now.AddDays(7); // 7 days for special quests
                default:
                    return DateTime.Now.AddDays(1);
            }
        }
        
        /// <summary>
        /// Clean up expired quests
        /// Dọn dẹp nhiệm vụ hết hạn
        /// </summary>
        private void CleanupExpiredQuests()
        {
            foreach (var guildQuests in activeQuests.Values)
            {
                guildQuests.RemoveAll(q => q.IsExpired && !q.IsCompleted);
            }
        }
        
        /// <summary>
        /// Called when quest is completed
        /// Được gọi khi nhiệm vụ hoàn thành
        /// </summary>
        private void OnQuestCompleted(Guild guild, ActiveGuildQuest quest)
        {
            // Notify all guild members
            // Update UI
            // Send announcements
        }
    }
}
