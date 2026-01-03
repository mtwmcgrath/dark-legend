using System;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild member ranks with associated permissions
    /// Các cấp bậc thành viên guild với quyền hạn tương ứng
    /// </summary>
    [Serializable]
    public enum GuildRank
    {
        Newbie = 0,           // Tân binh - Thử việc 7 ngày
        Member = 1,           // Thành viên - Quyền cơ bản
        SeniorMember = 2,     // Thành viên cao cấp - Rút đồ bank giới hạn
        BattleMaster = 3,     // Đội trưởng - Max 5, quản lý war teams
        ViceMaster = 4,       // Phó guild - Max 3, quản lý members
        GuildMaster = 5       // Chủ guild - Toàn quyền
    }

    /// <summary>
    /// Permissions associated with each guild rank
    /// Quyền hạn tương ứng với mỗi cấp bậc guild
    /// </summary>
    [Serializable]
    public class GuildRankPermissions
    {
        public GuildRank Rank;
        
        // Member management / Quản lý thành viên
        public bool CanInviteMembers;
        public bool CanKickMembers;
        public bool CanPromoteMembers;
        public bool CanDemoteMembers;
        
        // Guild Bank / Kho guild
        public bool CanDepositBank;
        public bool CanWithdrawBank;
        public int DailyWithdrawLimit;
        
        // Guild War / Chiến tranh guild
        public bool CanDeclareWar;
        public bool CanParticipateWar;
        public bool CanManageWarTeams;
        
        // Guild Settings / Cài đặt guild
        public bool CanEditGuildInfo;
        public bool CanEditGuildNotice;
        public bool CanManageAlliance;
        public bool CanDisbandGuild;
        public bool CanTransferMaster;
        
        // Guild Features / Tính năng guild
        public bool CanActivateBuffs;
        public bool CanAccessGuildShop;
        public bool CanStartGuildQuest;
        
        public static GuildRankPermissions GetPermissions(GuildRank rank)
        {
            switch (rank)
            {
                case GuildRank.GuildMaster:
                    return new GuildRankPermissions
                    {
                        Rank = rank,
                        CanInviteMembers = true,
                        CanKickMembers = true,
                        CanPromoteMembers = true,
                        CanDemoteMembers = true,
                        CanDepositBank = true,
                        CanWithdrawBank = true,
                        DailyWithdrawLimit = -1, // Unlimited
                        CanDeclareWar = true,
                        CanParticipateWar = true,
                        CanManageWarTeams = true,
                        CanEditGuildInfo = true,
                        CanEditGuildNotice = true,
                        CanManageAlliance = true,
                        CanDisbandGuild = true,
                        CanTransferMaster = true,
                        CanActivateBuffs = true,
                        CanAccessGuildShop = true,
                        CanStartGuildQuest = true
                    };
                    
                case GuildRank.ViceMaster:
                    return new GuildRankPermissions
                    {
                        Rank = rank,
                        CanInviteMembers = true,
                        CanKickMembers = true,
                        CanPromoteMembers = true,
                        CanDemoteMembers = true,
                        CanDepositBank = true,
                        CanWithdrawBank = true,
                        DailyWithdrawLimit = 1000000,
                        CanDeclareWar = false,
                        CanParticipateWar = true,
                        CanManageWarTeams = true,
                        CanEditGuildInfo = true,
                        CanEditGuildNotice = true,
                        CanManageAlliance = false,
                        CanDisbandGuild = false,
                        CanTransferMaster = false,
                        CanActivateBuffs = true,
                        CanAccessGuildShop = true,
                        CanStartGuildQuest = true
                    };
                    
                case GuildRank.BattleMaster:
                    return new GuildRankPermissions
                    {
                        Rank = rank,
                        CanInviteMembers = true,
                        CanKickMembers = false,
                        CanPromoteMembers = false,
                        CanDemoteMembers = false,
                        CanDepositBank = true,
                        CanWithdrawBank = true,
                        DailyWithdrawLimit = 500000,
                        CanDeclareWar = false,
                        CanParticipateWar = true,
                        CanManageWarTeams = true,
                        CanEditGuildInfo = false,
                        CanEditGuildNotice = true,
                        CanManageAlliance = false,
                        CanDisbandGuild = false,
                        CanTransferMaster = false,
                        CanActivateBuffs = false,
                        CanAccessGuildShop = true,
                        CanStartGuildQuest = false
                    };
                    
                case GuildRank.SeniorMember:
                    return new GuildRankPermissions
                    {
                        Rank = rank,
                        CanInviteMembers = false,
                        CanKickMembers = false,
                        CanPromoteMembers = false,
                        CanDemoteMembers = false,
                        CanDepositBank = true,
                        CanWithdrawBank = true,
                        DailyWithdrawLimit = 100000,
                        CanDeclareWar = false,
                        CanParticipateWar = true,
                        CanManageWarTeams = false,
                        CanEditGuildInfo = false,
                        CanEditGuildNotice = false,
                        CanManageAlliance = false,
                        CanDisbandGuild = false,
                        CanTransferMaster = false,
                        CanActivateBuffs = false,
                        CanAccessGuildShop = true,
                        CanStartGuildQuest = false
                    };
                    
                case GuildRank.Member:
                    return new GuildRankPermissions
                    {
                        Rank = rank,
                        CanInviteMembers = false,
                        CanKickMembers = false,
                        CanPromoteMembers = false,
                        CanDemoteMembers = false,
                        CanDepositBank = true,
                        CanWithdrawBank = false,
                        DailyWithdrawLimit = 0,
                        CanDeclareWar = false,
                        CanParticipateWar = true,
                        CanManageWarTeams = false,
                        CanEditGuildInfo = false,
                        CanEditGuildNotice = false,
                        CanManageAlliance = false,
                        CanDisbandGuild = false,
                        CanTransferMaster = false,
                        CanActivateBuffs = false,
                        CanAccessGuildShop = true,
                        CanStartGuildQuest = false
                    };
                    
                case GuildRank.Newbie:
                default:
                    return new GuildRankPermissions
                    {
                        Rank = rank,
                        CanInviteMembers = false,
                        CanKickMembers = false,
                        CanPromoteMembers = false,
                        CanDemoteMembers = false,
                        CanDepositBank = true,
                        CanWithdrawBank = false,
                        DailyWithdrawLimit = 0,
                        CanDeclareWar = false,
                        CanParticipateWar = false,
                        CanManageWarTeams = false,
                        CanEditGuildInfo = false,
                        CanEditGuildNotice = false,
                        CanManageAlliance = false,
                        CanDisbandGuild = false,
                        CanTransferMaster = false,
                        CanActivateBuffs = false,
                        CanAccessGuildShop = false,
                        CanStartGuildQuest = false
                    };
            }
        }
    }
}
