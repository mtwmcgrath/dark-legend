using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild configuration data using ScriptableObject
    /// Dữ liệu cấu hình guild sử dụng ScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "GuildData", menuName = "DarkLegend/Guild/Guild Data")]
    public class GuildData : ScriptableObject
    {
        [Header("Guild Creation Requirements / Yêu cầu tạo Guild")]
        [Tooltip("Minimum level required to create a guild")]
        public int RequiredLevel = 100;
        
        [Tooltip("Zen cost to create a guild")]
        public int RequiredZen = 1000000;
        
        [Tooltip("Minimum number of initial members")]
        public int MinimumMembers = 1;
        
        [Header("Guild Name Settings / Cài đặt tên Guild")]
        [Tooltip("Minimum guild name length")]
        public int MinNameLength = 3;
        
        [Tooltip("Maximum guild name length")]
        public int MaxNameLength = 16;
        
        [Header("Guild Level System / Hệ thống cấp độ Guild")]
        [Tooltip("Maximum guild level")]
        public int MaxGuildLevel = 50;
        
        [Tooltip("Base members for level 1 guild")]
        public int BaseMemberSlots = 20;
        
        [Tooltip("Additional member slots per level")]
        public int MemberSlotsPerLevel = 2;
        
        [Tooltip("Base bank slots for level 1 guild")]
        public int BaseBankSlots = 50;
        
        [Tooltip("Additional bank slots per level")]
        public int BankSlotsPerLevel = 5;
        
        [Tooltip("Base EXP bonus percentage at level 1")]
        public float BaseExpBonus = 1f;
        
        [Tooltip("Additional EXP bonus per level (%)")]
        public float ExpBonusPerLevel = 0.01f;
        
        [Tooltip("Buff slots available every X levels")]
        public int BuffSlotLevelInterval = 10;
        
        [Header("Guild Ranks Limits / Giới hạn cấp bậc")]
        [Tooltip("Maximum number of Vice Masters")]
        public int MaxViceMasters = 3;
        
        [Tooltip("Maximum number of Battle Masters")]
        public int MaxBattleMasters = 5;
        
        [Header("Guild Bank / Kho Guild")]
        [Tooltip("Maximum daily withdraw limit for Senior Members")]
        public int SeniorMemberWithdrawLimit = 100000;
        
        [Tooltip("Maximum daily withdraw limit for Battle Masters")]
        public int BattleMasterWithdrawLimit = 500000;
        
        [Tooltip("Maximum daily withdraw limit for Vice Masters")]
        public int ViceMasterWithdrawLimit = 1000000;
        
        [Header("Newbie Probation / Thử việc tân binh")]
        [Tooltip("Days for newbie probation period")]
        public int NewbieProbationDays = 7;
        
        [Header("Guild War Settings / Cài đặt chiến tranh Guild")]
        [Tooltip("Points for killing regular member")]
        public int WarPointsKillMember = 1;
        
        [Tooltip("Points for killing Vice Master")]
        public int WarPointsKillViceMaster = 3;
        
        [Tooltip("Points for killing Guild Master")]
        public int WarPointsKillGuildMaster = 5;
        
        [Header("Castle Siege / Công thành chiến")]
        [Tooltip("Duration of castle siege in hours")]
        public int CastleSiegeDuration = 2;
        
        [Tooltip("Tax percentage for castle owner")]
        public float CastleOwnerTaxRate = 0.1f;
        
        [Header("Alliance Settings / Cài đặt liên minh")]
        [Tooltip("Maximum guilds in an alliance")]
        public int MaxAllianceGuilds = 5;
        
        /// <summary>
        /// Calculate max members for guild level
        /// Tính số thành viên tối đa cho cấp độ guild
        /// </summary>
        public int GetMaxMembers(int level)
        {
            return BaseMemberSlots + (level * MemberSlotsPerLevel);
        }
        
        /// <summary>
        /// Calculate max bank slots for guild level
        /// Tính số ô kho tối đa cho cấp độ guild
        /// </summary>
        public int GetMaxBankSlots(int level)
        {
            return BaseBankSlots + (level * BankSlotsPerLevel);
        }
        
        /// <summary>
        /// Calculate EXP bonus multiplier for guild level
        /// Tính hệ số bonus EXP cho cấp độ guild
        /// </summary>
        public float GetExpBonus(int level)
        {
            return BaseExpBonus + (level * ExpBonusPerLevel);
        }
        
        /// <summary>
        /// Calculate buff slots for guild level
        /// Tính số ô buff cho cấp độ guild
        /// </summary>
        public int GetBuffSlots(int level)
        {
            return level / BuffSlotLevelInterval;
        }
        
        /// <summary>
        /// Calculate required EXP for next guild level
        /// Tính EXP cần thiết cho cấp độ guild tiếp theo
        /// </summary>
        public int GetRequiredExp(int currentLevel)
        {
            if (currentLevel >= MaxGuildLevel)
                return 0;
                
            // Formula: level^2 * 1000 + level * 5000
            return (currentLevel * currentLevel * 1000) + (currentLevel * 5000);
        }
    }
}
