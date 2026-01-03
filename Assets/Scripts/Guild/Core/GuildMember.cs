using System;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild member information
    /// Thông tin thành viên guild
    /// </summary>
    [Serializable]
    public class GuildMember
    {
        public string PlayerId;
        public string PlayerName;
        public int PlayerLevel;
        public string CharacterClass;
        
        public GuildRank Rank;
        public DateTime JoinDate;
        public DateTime LastOnline;
        
        // Contribution / Đóng góp
        public int TotalContribution;
        public int WeeklyContribution;
        
        // Activity / Hoạt động
        public int GuildWarsParticipated;
        public int GuildQuestsCompleted;
        public int MonstersKilledForGuild;
        
        // Status
        public bool IsOnline;
        public bool IsProbation => (DateTime.Now - JoinDate).Days < 7 && Rank == GuildRank.Newbie;
        
        public GuildMember(string playerId, string playerName, int playerLevel, string characterClass)
        {
            PlayerId = playerId;
            PlayerName = playerName;
            PlayerLevel = playerLevel;
            CharacterClass = characterClass;
            
            Rank = GuildRank.Newbie;
            JoinDate = DateTime.Now;
            LastOnline = DateTime.Now;
            
            TotalContribution = 0;
            WeeklyContribution = 0;
            GuildWarsParticipated = 0;
            GuildQuestsCompleted = 0;
            MonstersKilledForGuild = 0;
            IsOnline = true;
        }
        
        /// <summary>
        /// Add contribution to member
        /// Thêm đóng góp cho thành viên
        /// </summary>
        public void AddContribution(int amount)
        {
            TotalContribution += amount;
            WeeklyContribution += amount;
        }
        
        /// <summary>
        /// Reset weekly contribution (called weekly)
        /// Reset đóng góp tuần (gọi hàng tuần)
        /// </summary>
        public void ResetWeeklyContribution()
        {
            WeeklyContribution = 0;
        }
        
        /// <summary>
        /// Get member's permissions based on rank
        /// Lấy quyền hạn của thành viên dựa trên cấp bậc
        /// </summary>
        public GuildRankPermissions GetPermissions()
        {
            return GuildRankPermissions.GetPermissions(Rank);
        }
        
        /// <summary>
        /// Update online status
        /// Cập nhật trạng thái online
        /// </summary>
        public void UpdateOnlineStatus(bool online)
        {
            IsOnline = online;
            if (online)
            {
                LastOnline = DateTime.Now;
            }
        }
    }
}
