using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild Alliance system - Multiple guilds forming alliances
    /// Hệ thống liên minh Guild - Nhiều guild tạo thành liên minh
    /// </summary>
    [Serializable]
    public class GuildAlliance
    {
        public string AllianceId;
        public string AllianceName;
        public string LeaderGuildId;
        public List<string> MemberGuildIds;
        public DateTime CreationDate;
        public int MaxGuilds = 5;
        
        // Alliance stats / Thống kê liên minh
        public int TotalMembers;
        public int AverageLevel;
        public int TotalWars;
        public int TotalWins;
        
        public GuildAlliance(string allianceId, string allianceName, string leaderGuildId)
        {
            AllianceId = allianceId;
            AllianceName = allianceName;
            LeaderGuildId = leaderGuildId;
            MemberGuildIds = new List<string> { leaderGuildId };
            CreationDate = DateTime.Now;
            MaxGuilds = 5;
            
            TotalMembers = 0;
            AverageLevel = 0;
            TotalWars = 0;
            TotalWins = 0;
        }
        
        /// <summary>
        /// Add guild to alliance
        /// Thêm guild vào liên minh
        /// </summary>
        public bool AddGuild(string guildId)
        {
            if (MemberGuildIds.Count >= MaxGuilds)
            {
                return false;
            }
            
            if (MemberGuildIds.Contains(guildId))
            {
                return false;
            }
            
            MemberGuildIds.Add(guildId);
            return true;
        }
        
        /// <summary>
        /// Remove guild from alliance
        /// Xóa guild khỏi liên minh
        /// </summary>
        public bool RemoveGuild(string guildId)
        {
            if (guildId == LeaderGuildId)
            {
                return false; // Leader cannot be removed
            }
            
            return MemberGuildIds.Remove(guildId);
        }
        
        /// <summary>
        /// Transfer alliance leadership
        /// Chuyển quyền lãnh đạo liên minh
        /// </summary>
        public bool TransferLeadership(string newLeaderGuildId)
        {
            if (!MemberGuildIds.Contains(newLeaderGuildId))
            {
                return false;
            }
            
            LeaderGuildId = newLeaderGuildId;
            return true;
        }
        
        /// <summary>
        /// Check if alliance is full
        /// Kiểm tra liên minh đã đầy chưa
        /// </summary>
        public bool IsFull => MemberGuildIds.Count >= MaxGuilds;
        
        /// <summary>
        /// Get alliance power (total members)
        /// Lấy sức mạnh liên minh (tổng thành viên)
        /// </summary>
        public int GetAlliancePower()
        {
            return TotalMembers;
        }
    }
}
