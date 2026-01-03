using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild contribution tracking system
    /// Hệ thống theo dõi đóng góp guild
    /// </summary>
    public class GuildContribution : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        [SerializeField] private GuildLevel guildLevel;
        
        /// <summary>
        /// Contribution types
        /// Loại đóng góp
        /// </summary>
        public enum ContributionType
        {
            ZenDonation,        // Quyên góp Zen
            ItemDonation,       // Quyên góp vật phẩm
            QuestCompletion,    // Hoàn thành nhiệm vụ
            BossKill,           // Giết boss
            DungeonClear,       // Hoàn thành던전
            GuildWarKill,       // Giết trong Guild War
            EventParticipation  // Tham gia sự kiện
        }
        
        /// <summary>
        /// Contribution record
        /// Bản ghi đóng góp
        /// </summary>
        [Serializable]
        public class ContributionRecord
        {
            public string RecordId;
            public string PlayerId;
            public string PlayerName;
            public ContributionType Type;
            public int Amount;
            public DateTime Timestamp;
            public string Description;
        }
        
        // Contribution records per guild / Bản ghi đóng góp cho mỗi guild
        private Dictionary<string, List<ContributionRecord>> contributionHistory = 
            new Dictionary<string, List<ContributionRecord>>();
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
            
            if (guildLevel == null)
            {
                guildLevel = GetComponent<GuildLevel>();
            }
        }
        
        /// <summary>
        /// Add contribution for member
        /// Thêm đóng góp cho thành viên
        /// </summary>
        public bool AddContribution(string guildId, string playerId, ContributionType type, int amount, string description = "")
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
            
            // Calculate contribution points
            int contributionPoints = CalculateContributionPoints(type, amount);
            
            // Add contribution to member
            member.AddContribution(contributionPoints);
            
            // Add guild EXP based on contribution
            if (guildLevel != null)
            {
                int guildExp = contributionPoints * 10; // 10 EXP per contribution point
                guildLevel.AddGuildExperience(guildId, guildExp, $"{member.PlayerName}'s contribution");
            }
            
            // Record contribution
            if (!contributionHistory.ContainsKey(guildId))
            {
                contributionHistory[guildId] = new List<ContributionRecord>();
            }
            
            ContributionRecord record = new ContributionRecord
            {
                RecordId = Guid.NewGuid().ToString(),
                PlayerId = playerId,
                PlayerName = member.PlayerName,
                Type = type,
                Amount = contributionPoints,
                Timestamp = DateTime.Now,
                Description = description
            };
            
            contributionHistory[guildId].Add(record);
            
            // Keep only last 10000 records
            if (contributionHistory[guildId].Count > 10000)
            {
                contributionHistory[guildId].RemoveAt(0);
            }
            
            Debug.Log($"{member.PlayerName} contributed {contributionPoints} points ({type})");
            
            return true;
        }
        
        /// <summary>
        /// Calculate contribution points based on type and amount
        /// Tính điểm đóng góp dựa trên loại và số lượng
        /// </summary>
        private int CalculateContributionPoints(ContributionType type, int amount)
        {
            return type switch
            {
                ContributionType.ZenDonation => amount / 1000,        // 1 point per 1000 zen
                ContributionType.ItemDonation => amount * 10,         // 10 points per item
                ContributionType.QuestCompletion => amount * 5,       // 5 points per quest
                ContributionType.BossKill => amount * 20,             // 20 points per boss
                ContributionType.DungeonClear => amount * 15,         // 15 points per dungeon
                ContributionType.GuildWarKill => amount * 10,         // 10 points per kill
                ContributionType.EventParticipation => amount * 5,    // 5 points per event
                _ => amount
            };
        }
        
        /// <summary>
        /// Get member contribution stats
        /// Lấy thống kê đóng góp của thành viên
        /// </summary>
        public ContributionStats GetMemberStats(string guildId, string playerId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return null;
            }
            
            GuildMember member = guild.GetMember(playerId);
            if (member == null)
            {
                return null;
            }
            
            if (!contributionHistory.ContainsKey(guildId))
            {
                return new ContributionStats
                {
                    PlayerId = playerId,
                    PlayerName = member.PlayerName,
                    TotalContribution = member.TotalContribution,
                    WeeklyContribution = member.WeeklyContribution,
                    ContributionsByType = new Dictionary<ContributionType, int>()
                };
            }
            
            var memberRecords = contributionHistory[guildId]
                .Where(r => r.PlayerId == playerId)
                .ToList();
            
            var contributionsByType = memberRecords
                .GroupBy(r => r.Type)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Amount));
            
            return new ContributionStats
            {
                PlayerId = playerId,
                PlayerName = member.PlayerName,
                TotalContribution = member.TotalContribution,
                WeeklyContribution = member.WeeklyContribution,
                ContributionsByType = contributionsByType,
                LastContribution = memberRecords.OrderByDescending(r => r.Timestamp).FirstOrDefault()?.Timestamp
            };
        }
        
        /// <summary>
        /// Get top contributors for guild
        /// Lấy những người đóng góp nhiều nhất cho guild
        /// </summary>
        public List<ContributionStats> GetTopContributors(string guildId, int count = 10, ContributionPeriod period = ContributionPeriod.AllTime)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return new List<ContributionStats>();
            }
            
            var contributors = guild.Members.Select(m => GetMemberStats(guildId, m.PlayerId));
            
            switch (period)
            {
                case ContributionPeriod.Weekly:
                    return contributors
                        .OrderByDescending(s => s.WeeklyContribution)
                        .Take(count)
                        .ToList();
                case ContributionPeriod.AllTime:
                default:
                    return contributors
                        .OrderByDescending(s => s.TotalContribution)
                        .Take(count)
                        .ToList();
            }
        }
        
        /// <summary>
        /// Get recent contribution history
        /// Lấy lịch sử đóng góp gần đây
        /// </summary>
        public List<ContributionRecord> GetRecentContributions(string guildId, int limit = 50)
        {
            if (!contributionHistory.ContainsKey(guildId))
            {
                return new List<ContributionRecord>();
            }
            
            return contributionHistory[guildId]
                .OrderByDescending(r => r.Timestamp)
                .Take(limit)
                .ToList();
        }
        
        /// <summary>
        /// Reset weekly contributions (should be called weekly)
        /// Reset đóng góp tuần (nên được gọi hàng tuần)
        /// </summary>
        public void ResetWeeklyContributions(string guildId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return;
            }
            
            foreach (var member in guild.Members)
            {
                member.ResetWeeklyContribution();
            }
            
            Debug.Log($"Weekly contributions reset for guild '{guild.GuildName}'");
        }
        
        /// <summary>
        /// Contribution period for rankings
        /// Khoảng thời gian đóng góp cho xếp hạng
        /// </summary>
        public enum ContributionPeriod
        {
            Weekly,
            AllTime
        }
        
        /// <summary>
        /// Contribution statistics
        /// Thống kê đóng góp
        /// </summary>
        [Serializable]
        public class ContributionStats
        {
            public string PlayerId;
            public string PlayerName;
            public int TotalContribution;
            public int WeeklyContribution;
            public Dictionary<ContributionType, int> ContributionsByType;
            public DateTime? LastContribution;
        }
    }
}
