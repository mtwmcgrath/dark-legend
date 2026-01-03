using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild buff system - Provides buffs to guild members
    /// Hệ thống buff guild - Cung cấp buff cho thành viên guild
    /// </summary>
    public class GuildBuff : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        [Header("Buff Configuration")]
        [SerializeField] private List<GuildBuffConfig> availableBuffs;
        
        // Active buffs per guild / Buff đang hoạt động cho mỗi guild
        private Dictionary<string, List<ActiveGuildBuff>> activeBuffs = new Dictionary<string, List<ActiveGuildBuff>>();
        
        /// <summary>
        /// Guild buff configuration
        /// Cấu hình buff guild
        /// </summary>
        [Serializable]
        public class GuildBuffConfig
        {
            public string BuffId;
            public string BuffName;
            public BuffType Type;
            public float Value;
            public int DurationHours;
            public int PointsCost;
            public int MinGuildLevel;
            public string Description;
        }
        
        /// <summary>
        /// Buff types available
        /// Các loại buff có sẵn
        /// </summary>
        public enum BuffType
        {
            ExpBoost,       // Tăng EXP
            AttackBoost,    // Tăng sát thương
            DefenseBoost,   // Tăng phòng thủ
            DropRate,       // Tăng tỷ lệ rơi đồ
            HPBoost,        // Tăng HP tối đa
            MPBoost,        // Tăng MP tối đa
            MoveSpeed,      // Tăng tốc độ di chuyển
            CritRate        // Tăng tỷ lệ chí mạng
        }
        
        /// <summary>
        /// Active buff instance
        /// Buff đang hoạt động
        /// </summary>
        [Serializable]
        public class ActiveGuildBuff
        {
            public string BuffId;
            public string BuffName;
            public BuffType Type;
            public float Value;
            public DateTime ActivationTime;
            public DateTime ExpirationTime;
            public string ActivatedBy;
            
            public bool IsActive => DateTime.Now < ExpirationTime;
            public TimeSpan RemainingTime => ExpirationTime - DateTime.Now;
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
            
            InitializeDefaultBuffs();
        }
        
        private void Update()
        {
            // Clean up expired buffs
            CleanupExpiredBuffs();
        }
        
        /// <summary>
        /// Initialize default guild buffs
        /// Khởi tạo buff guild mặc định
        /// </summary>
        private void InitializeDefaultBuffs()
        {
            if (availableBuffs == null || availableBuffs.Count == 0)
            {
                availableBuffs = new List<GuildBuffConfig>
                {
                    new GuildBuffConfig
                    {
                        BuffId = "exp_boost_1",
                        BuffName = "EXP Boost I",
                        Type = BuffType.ExpBoost,
                        Value = 0.10f,
                        DurationHours = 1,
                        PointsCost = 100,
                        MinGuildLevel = 1,
                        Description = "+10% EXP for 1 hour"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "exp_boost_2",
                        BuffName = "EXP Boost II",
                        Type = BuffType.ExpBoost,
                        Value = 0.20f,
                        DurationHours = 2,
                        PointsCost = 300,
                        MinGuildLevel = 5,
                        Description = "+20% EXP for 2 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "exp_boost_3",
                        BuffName = "EXP Boost III",
                        Type = BuffType.ExpBoost,
                        Value = 0.50f,
                        DurationHours = 3,
                        PointsCost = 1000,
                        MinGuildLevel = 10,
                        Description = "+50% EXP for 3 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "attack_boost_1",
                        BuffName = "Attack Boost I",
                        Type = BuffType.AttackBoost,
                        Value = 0.05f,
                        DurationHours = 1,
                        PointsCost = 150,
                        MinGuildLevel = 1,
                        Description = "+5% Attack for 1 hour"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "attack_boost_2",
                        BuffName = "Attack Boost II",
                        Type = BuffType.AttackBoost,
                        Value = 0.10f,
                        DurationHours = 2,
                        PointsCost = 400,
                        MinGuildLevel = 8,
                        Description = "+10% Attack for 2 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "attack_boost_3",
                        BuffName = "Attack Boost III",
                        Type = BuffType.AttackBoost,
                        Value = 0.20f,
                        DurationHours = 3,
                        PointsCost = 1200,
                        MinGuildLevel = 15,
                        Description = "+20% Attack for 3 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "defense_boost_1",
                        BuffName = "Defense Boost I",
                        Type = BuffType.DefenseBoost,
                        Value = 0.05f,
                        DurationHours = 1,
                        PointsCost = 150,
                        MinGuildLevel = 1,
                        Description = "+5% Defense for 1 hour"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "defense_boost_2",
                        BuffName = "Defense Boost II",
                        Type = BuffType.DefenseBoost,
                        Value = 0.10f,
                        DurationHours = 2,
                        PointsCost = 400,
                        MinGuildLevel = 8,
                        Description = "+10% Defense for 2 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "defense_boost_3",
                        BuffName = "Defense Boost III",
                        Type = BuffType.DefenseBoost,
                        Value = 0.20f,
                        DurationHours = 3,
                        PointsCost = 1200,
                        MinGuildLevel = 15,
                        Description = "+20% Defense for 3 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "drop_rate_boost",
                        BuffName = "Drop Rate Boost",
                        Type = BuffType.DropRate,
                        Value = 0.30f,
                        DurationHours = 2,
                        PointsCost = 500,
                        MinGuildLevel = 10,
                        Description = "+30% Drop Rate for 2 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "hp_boost",
                        BuffName = "HP Boost",
                        Type = BuffType.HPBoost,
                        Value = 0.30f,
                        DurationHours = 3,
                        PointsCost = 600,
                        MinGuildLevel = 12,
                        Description = "+30% Max HP for 3 hours"
                    },
                    new GuildBuffConfig
                    {
                        BuffId = "mp_boost",
                        BuffName = "MP Boost",
                        Type = BuffType.MPBoost,
                        Value = 0.30f,
                        DurationHours = 3,
                        PointsCost = 600,
                        MinGuildLevel = 12,
                        Description = "+30% Max MP for 3 hours"
                    }
                };
            }
        }
        
        /// <summary>
        /// Activate guild buff
        /// Kích hoạt buff guild
        /// </summary>
        public bool ActivateBuff(string guildId, string playerId, string buffId)
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
            if (!permissions.CanActivateBuffs)
            {
                Debug.LogError("You don't have permission to activate buffs.");
                return false;
            }
            
            GuildBuffConfig config = availableBuffs.FirstOrDefault(b => b.BuffId == buffId);
            if (config == null)
            {
                Debug.LogError("Buff configuration not found.");
                return false;
            }
            
            // Check guild level requirement
            if (guild.Level < config.MinGuildLevel)
            {
                Debug.LogError($"Guild level {config.MinGuildLevel} required.");
                return false;
            }
            
            // Check guild points
            if (guild.Points < config.PointsCost)
            {
                Debug.LogError($"Insufficient guild points. Need {config.PointsCost}, have {guild.Points}.");
                return false;
            }
            
            // Check buff slot limit
            GuildData data = guildManager.GetGuildData();
            int maxBuffSlots = data.GetBuffSlots(guild.Level);
            
            List<ActiveGuildBuff> guildBuffs = GetActiveBuffs(guildId);
            if (guildBuffs.Count >= maxBuffSlots)
            {
                Debug.LogError($"Maximum {maxBuffSlots} buff(s) can be active at once.");
                return false;
            }
            
            // Check if same type buff is already active
            if (guildBuffs.Any(b => b.Type == config.Type && b.IsActive))
            {
                Debug.LogError($"A {config.Type} buff is already active.");
                return false;
            }
            
            // Deduct points
            guild.Points -= config.PointsCost;
            
            // Activate buff
            ActiveGuildBuff activeBuff = new ActiveGuildBuff
            {
                BuffId = config.BuffId,
                BuffName = config.BuffName,
                Type = config.Type,
                Value = config.Value,
                ActivationTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddHours(config.DurationHours),
                ActivatedBy = member.PlayerName
            };
            
            if (!activeBuffs.ContainsKey(guildId))
            {
                activeBuffs[guildId] = new List<ActiveGuildBuff>();
            }
            activeBuffs[guildId].Add(activeBuff);
            
            Debug.Log($"{member.PlayerName} activated {config.BuffName} for the guild!");
            OnBuffActivated(guild, activeBuff);
            
            return true;
        }
        
        /// <summary>
        /// Get active buffs for guild
        /// Lấy buff đang hoạt động cho guild
        /// </summary>
        public List<ActiveGuildBuff> GetActiveBuffs(string guildId)
        {
            if (!activeBuffs.ContainsKey(guildId))
            {
                return new List<ActiveGuildBuff>();
            }
            
            return activeBuffs[guildId].Where(b => b.IsActive).ToList();
        }
        
        /// <summary>
        /// Get available buffs for guild
        /// Lấy buff có thể sử dụng cho guild
        /// </summary>
        public List<GuildBuffConfig> GetAvailableBuffs(string guildId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return new List<GuildBuffConfig>();
            }
            
            return availableBuffs
                .Where(b => guild.Level >= b.MinGuildLevel)
                .ToList();
        }
        
        /// <summary>
        /// Get total buff value for a member
        /// Lấy tổng giá trị buff cho thành viên
        /// </summary>
        public float GetBuffValue(string guildId, BuffType type)
        {
            List<ActiveGuildBuff> buffs = GetActiveBuffs(guildId);
            return buffs
                .Where(b => b.Type == type && b.IsActive)
                .Sum(b => b.Value);
        }
        
        /// <summary>
        /// Clean up expired buffs
        /// Dọn dẹp buff hết hạn
        /// </summary>
        private void CleanupExpiredBuffs()
        {
            foreach (var guildBuffs in activeBuffs.Values)
            {
                guildBuffs.RemoveAll(b => !b.IsActive);
            }
        }
        
        /// <summary>
        /// Called when buff is activated
        /// Được gọi khi buff được kích hoạt
        /// </summary>
        private void OnBuffActivated(Guild guild, ActiveGuildBuff buff)
        {
            // Notify all guild members
            // Update UI
            // Send announcements
        }
    }
}
