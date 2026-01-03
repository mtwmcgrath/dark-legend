using System;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild level system with bonuses
    /// Hệ thống cấp độ guild với phần thưởng
    /// </summary>
    public class GuildLevel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        /// <summary>
        /// Add experience to guild
        /// Thêm kinh nghiệm cho guild
        /// </summary>
        public bool AddGuildExperience(string guildId, int exp, string source = "")
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            GuildData data = guildManager.GetGuildData();
            int oldLevel = guild.Level;
            
            bool leveledUp = guild.AddExperience(exp, data);
            
            Debug.Log($"Guild '{guild.GuildName}' gained {exp} EXP from {source}");
            
            if (guild.Level > oldLevel)
            {
                OnGuildLevelUp(guild, oldLevel, guild.Level);
            }
            
            return leveledUp;
        }
        
        /// <summary>
        /// Get guild level info
        /// Lấy thông tin cấp độ guild
        /// </summary>
        public GuildLevelInfo GetLevelInfo(string guildId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return null;
            }
            
            GuildData data = guildManager.GetGuildData();
            
            return new GuildLevelInfo
            {
                CurrentLevel = guild.Level,
                CurrentExp = guild.CurrentExp,
                RequiredExp = data.GetRequiredExp(guild.Level),
                MaxMembers = data.GetMaxMembers(guild.Level),
                BankSlots = data.GetMaxBankSlots(guild.Level),
                ExpBonus = data.GetExpBonus(guild.Level),
                BuffSlots = data.GetBuffSlots(guild.Level),
                IsMaxLevel = guild.Level >= data.MaxGuildLevel
            };
        }
        
        /// <summary>
        /// Calculate EXP gain from activity
        /// Tính EXP nhận được từ hoạt động
        /// </summary>
        public int CalculateExpFromActivity(ActivityType activity, int amount)
        {
            return activity switch
            {
                ActivityType.MemberQuestComplete => amount * 10,
                ActivityType.MemberBossKill => amount * 50,
                ActivityType.MemberDungeonClear => amount * 30,
                ActivityType.GuildWarVictory => amount * 500,
                ActivityType.MemberContribution => amount / 100, // 1 EXP per 100 contribution
                _ => 0
            };
        }
        
        /// <summary>
        /// Called when guild levels up
        /// Được gọi khi guild lên cấp
        /// </summary>
        private void OnGuildLevelUp(Guild guild, int oldLevel, int newLevel)
        {
            Debug.Log($"🎉 Guild '{guild.GuildName}' leveled up! {oldLevel} → {newLevel}");
            
            GuildData data = guildManager.GetGuildData();
            
            // Log bonuses
            Debug.Log($"New bonuses:");
            Debug.Log($"- Max Members: {data.GetMaxMembers(newLevel)}");
            Debug.Log($"- Bank Slots: {data.GetMaxBankSlots(newLevel)}");
            Debug.Log($"- EXP Bonus: {data.GetExpBonus(newLevel):P0}");
            Debug.Log($"- Buff Slots: {data.GetBuffSlots(newLevel)}");
            
            // Notify all guild members
            // Update UI
            // Grant level up rewards
        }
        
        /// <summary>
        /// Activity types that grant guild EXP
        /// Loại hoạt động trao EXP cho guild
        /// </summary>
        public enum ActivityType
        {
            MemberQuestComplete,    // Thành viên hoàn thành nhiệm vụ
            MemberBossKill,         // Thành viên giết boss
            MemberDungeonClear,     // Thành viên hoàn thành던전
            GuildWarVictory,        // Thắng Guild War
            MemberContribution      // Đóng góp của thành viên
        }
        
        /// <summary>
        /// Guild level information
        /// Thông tin cấp độ guild
        /// </summary>
        [Serializable]
        public class GuildLevelInfo
        {
            public int CurrentLevel;
            public int CurrentExp;
            public int RequiredExp;
            public int MaxMembers;
            public int BankSlots;
            public float ExpBonus;
            public int BuffSlots;
            public bool IsMaxLevel;
            
            public float ExpProgress => RequiredExp > 0 ? (float)CurrentExp / RequiredExp : 1f;
        }
    }
}
