using System;
using System.Collections.Generic;
using System.Linq;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Main Guild class
    /// Class Guild chính
    /// </summary>
    [Serializable]
    public class Guild
    {
        public string GuildId;
        public string GuildName;
        public byte[] GuildMark;  // 8x8 pixels logo data
        
        // Guild type / Loại guild
        public GuildType Type;
        
        // Guild info / Thông tin guild
        public string Notice;
        public string Description;
        public DateTime CreationDate;
        
        // Guild Master
        public string GuildMasterId;
        
        // Level and progression / Cấp độ và tiến trình
        public int Level;
        public int CurrentExp;
        public int Points;
        
        // Members / Thành viên
        public List<GuildMember> Members;
        
        // Alliance / Liên minh
        public string AllianceId;
        
        // War history / Lịch sử chiến tranh
        public List<GuildWarRecord> WarHistory;
        
        // Stats / Thống kê
        public int TotalWins;
        public int TotalLosses;
        public int CastleSiegeWins;
        
        public Guild(string guildId, string guildName, string guildMasterId, GuildType type)
        {
            GuildId = guildId;
            GuildName = guildName;
            GuildMasterId = guildMasterId;
            Type = type;
            
            Level = 1;
            CurrentExp = 0;
            Points = 0;
            
            Members = new List<GuildMember>();
            WarHistory = new List<GuildWarRecord>();
            
            CreationDate = DateTime.Now;
            Notice = "Welcome to our guild!";
            Description = "";
            
            TotalWins = 0;
            TotalLosses = 0;
            CastleSiegeWins = 0;
        }
        
        #region Member Management / Quản lý thành viên
        
        /// <summary>
        /// Add member to guild
        /// Thêm thành viên vào guild
        /// </summary>
        public bool AddMember(GuildMember member, GuildData guildData)
        {
            if (Members.Count >= guildData.GetMaxMembers(Level))
            {
                return false;
            }
            
            if (Members.Any(m => m.PlayerId == member.PlayerId))
            {
                return false;
            }
            
            Members.Add(member);
            return true;
        }
        
        /// <summary>
        /// Remove member from guild
        /// Xóa thành viên khỏi guild
        /// </summary>
        public bool RemoveMember(string playerId)
        {
            var member = Members.FirstOrDefault(m => m.PlayerId == playerId);
            if (member == null || member.PlayerId == GuildMasterId)
            {
                return false;
            }
            
            Members.Remove(member);
            return true;
        }
        
        /// <summary>
        /// Get member by player ID
        /// Lấy thành viên theo ID người chơi
        /// </summary>
        public GuildMember GetMember(string playerId)
        {
            return Members.FirstOrDefault(m => m.PlayerId == playerId);
        }
        
        /// <summary>
        /// Promote member to higher rank
        /// Thăng cấp thành viên
        /// </summary>
        public bool PromoteMember(string playerId, GuildData guildData)
        {
            var member = GetMember(playerId);
            if (member == null || member.Rank >= GuildRank.ViceMaster)
            {
                return false;
            }
            
            // Check rank limits
            if (member.Rank == GuildRank.SeniorMember)
            {
                int battleMasterCount = Members.Count(m => m.Rank == GuildRank.BattleMaster);
                if (battleMasterCount >= guildData.MaxBattleMasters)
                {
                    return false;
                }
                member.Rank = GuildRank.BattleMaster;
            }
            else if (member.Rank == GuildRank.BattleMaster)
            {
                int viceMasterCount = Members.Count(m => m.Rank == GuildRank.ViceMaster);
                if (viceMasterCount >= guildData.MaxViceMasters)
                {
                    return false;
                }
                member.Rank = GuildRank.ViceMaster;
            }
            else
            {
                member.Rank = (GuildRank)((int)member.Rank + 1);
            }
            
            return true;
        }
        
        /// <summary>
        /// Demote member to lower rank
        /// Giáng cấp thành viên
        /// </summary>
        public bool DemoteMember(string playerId)
        {
            var member = GetMember(playerId);
            if (member == null || member.Rank <= GuildRank.Newbie || member.PlayerId == GuildMasterId)
            {
                return false;
            }
            
            member.Rank = (GuildRank)((int)member.Rank - 1);
            return true;
        }
        
        /// <summary>
        /// Transfer guild master to another member
        /// Chuyển quyền chủ guild cho thành viên khác
        /// </summary>
        public bool TransferMaster(string newMasterId)
        {
            var newMaster = GetMember(newMasterId);
            if (newMaster == null)
            {
                return false;
            }
            
            // Demote old master to Vice Master
            var oldMaster = GetMember(GuildMasterId);
            if (oldMaster != null)
            {
                oldMaster.Rank = GuildRank.ViceMaster;
            }
            
            // Promote new master
            newMaster.Rank = GuildRank.GuildMaster;
            GuildMasterId = newMasterId;
            
            return true;
        }
        
        #endregion
        
        #region Level and Experience / Cấp độ và Kinh nghiệm
        
        /// <summary>
        /// Add experience to guild
        /// Thêm kinh nghiệm cho guild
        /// </summary>
        public bool AddExperience(int exp, GuildData guildData)
        {
            if (Level >= guildData.MaxGuildLevel)
            {
                return false;
            }
            
            CurrentExp += exp;
            
            // Check for level up
            int requiredExp = guildData.GetRequiredExp(Level);
            while (CurrentExp >= requiredExp && Level < guildData.MaxGuildLevel)
            {
                CurrentExp -= requiredExp;
                Level++;
                requiredExp = guildData.GetRequiredExp(Level);
                
                // Guild level up! Trigger event here
                OnGuildLevelUp();
            }
            
            return true;
        }
        
        /// <summary>
        /// Called when guild levels up
        /// Được gọi khi guild lên cấp
        /// </summary>
        private void OnGuildLevelUp()
        {
            // Trigger level up event
            // Can be used for notifications, rewards, etc.
        }
        
        #endregion
        
        #region Statistics / Thống kê
        
        /// <summary>
        /// Get online member count
        /// Lấy số thành viên online
        /// </summary>
        public int GetOnlineMemberCount()
        {
            return Members.Count(m => m.IsOnline);
        }
        
        /// <summary>
        /// Get members by rank
        /// Lấy thành viên theo cấp bậc
        /// </summary>
        public List<GuildMember> GetMembersByRank(GuildRank rank)
        {
            return Members.Where(m => m.Rank == rank).ToList();
        }
        
        /// <summary>
        /// Get top contributors
        /// Lấy những người đóng góp nhiều nhất
        /// </summary>
        public List<GuildMember> GetTopContributors(int count = 10)
        {
            return Members
                .OrderByDescending(m => m.TotalContribution)
                .Take(count)
                .ToList();
        }
        
        /// <summary>
        /// Calculate guild win rate
        /// Tính tỷ lệ thắng của guild
        /// </summary>
        public float GetWinRate()
        {
            int totalBattles = TotalWins + TotalLosses;
            if (totalBattles == 0)
                return 0f;
            
            return (float)TotalWins / totalBattles;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Guild type enumeration
    /// Kiểu guild
    /// </summary>
    [Serializable]
    public enum GuildType
    {
        PvE,      // Tập trung vào PvE content
        PvP,      // Tập trung vào PvP và Guild War
        Mixed     // Cả PvE và PvP
    }
    
    /// <summary>
    /// Guild war record
    /// Bản ghi chiến tranh guild
    /// </summary>
    [Serializable]
    public class GuildWarRecord
    {
        public string WarId;
        public string OpponentGuildId;
        public string OpponentGuildName;
        public DateTime WarDate;
        public bool IsVictory;
        public int FinalScore;
        public int OpponentScore;
        public int Rewards;
    }
}
