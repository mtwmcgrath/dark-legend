using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Central manager for all guild operations
    /// Quản lý trung tâm cho tất cả hoạt động guild
    /// </summary>
    public class GuildManager : MonoBehaviour
    {
        private static GuildManager _instance;
        public static GuildManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GuildManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GuildManager");
                        _instance = go.AddComponent<GuildManager>();
                    }
                }
                return _instance;
            }
        }
        
        [Header("Configuration")]
        [SerializeField] private GuildData guildData;
        
        // All guilds in the game / Tất cả guilds trong game
        private Dictionary<string, Guild> guilds = new Dictionary<string, Guild>();
        
        // Player to guild mapping / Ánh xạ người chơi tới guild
        private Dictionary<string, string> playerToGuild = new Dictionary<string, string>();
        
        #region Events / Sự kiện
        
        public event Action<Guild> OnGuildCreated;
        public event Action<string> OnGuildDisbanded;
        public event Action<Guild, GuildMember> OnMemberJoined;
        public event Action<Guild, string> OnMemberLeft;
        public event Action<Guild, int> OnGuildLevelUp;
        
        #endregion
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        #region Guild CRUD / Tạo Đọc Cập nhật Xóa Guild
        
        /// <summary>
        /// Create a new guild
        /// Tạo guild mới
        /// </summary>
        public Guild CreateGuild(string guildName, string guildMasterId, GuildType type, byte[] guildMark = null)
        {
            // Validate guild name
            if (string.IsNullOrEmpty(guildName) || 
                guildName.Length < guildData.MinNameLength || 
                guildName.Length > guildData.MaxNameLength)
            {
                Debug.LogError($"Invalid guild name. Must be between {guildData.MinNameLength} and {guildData.MaxNameLength} characters.");
                return null;
            }
            
            // Check if guild name already exists
            if (guilds.Values.Any(g => g.GuildName.Equals(guildName, StringComparison.OrdinalIgnoreCase)))
            {
                Debug.LogError("Guild name already exists.");
                return null;
            }
            
            // Check if player is already in a guild
            if (playerToGuild.ContainsKey(guildMasterId))
            {
                Debug.LogError("Player is already in a guild.");
                return null;
            }
            
            // Create guild
            string guildId = Guid.NewGuid().ToString();
            Guild guild = new Guild(guildId, guildName, guildMasterId, type)
            {
                GuildMark = guildMark
            };
            
            guilds[guildId] = guild;
            
            // Add guild master as first member
            GuildMember guildMaster = new GuildMember(guildMasterId, "GuildMaster", 0, "")
            {
                Rank = GuildRank.GuildMaster
            };
            guild.AddMember(guildMaster, guildData);
            playerToGuild[guildMasterId] = guildId;
            
            OnGuildCreated?.Invoke(guild);
            Debug.Log($"Guild '{guildName}' created successfully!");
            
            return guild;
        }
        
        /// <summary>
        /// Get guild by ID
        /// Lấy guild theo ID
        /// </summary>
        public Guild GetGuild(string guildId)
        {
            return guilds.TryGetValue(guildId, out Guild guild) ? guild : null;
        }
        
        /// <summary>
        /// Get guild by name
        /// Lấy guild theo tên
        /// </summary>
        public Guild GetGuildByName(string guildName)
        {
            return guilds.Values.FirstOrDefault(g => 
                g.GuildName.Equals(guildName, StringComparison.OrdinalIgnoreCase));
        }
        
        /// <summary>
        /// Get player's guild
        /// Lấy guild của người chơi
        /// </summary>
        public Guild GetPlayerGuild(string playerId)
        {
            if (playerToGuild.TryGetValue(playerId, out string guildId))
            {
                return GetGuild(guildId);
            }
            return null;
        }
        
        /// <summary>
        /// Disband guild
        /// Giải tán guild
        /// </summary>
        public bool DisbandGuild(string guildId, string requesterId)
        {
            Guild guild = GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            // Only guild master can disband
            if (guild.GuildMasterId != requesterId)
            {
                Debug.LogError("Only Guild Master can disband the guild.");
                return false;
            }
            
            // Remove all members from mapping
            foreach (var member in guild.Members)
            {
                playerToGuild.Remove(member.PlayerId);
            }
            
            // Remove guild
            guilds.Remove(guildId);
            
            OnGuildDisbanded?.Invoke(guildId);
            Debug.Log($"Guild '{guild.GuildName}' disbanded.");
            
            return true;
        }
        
        #endregion
        
        #region Member Management / Quản lý thành viên
        
        /// <summary>
        /// Add member to guild
        /// Thêm thành viên vào guild
        /// </summary>
        public bool AddMemberToGuild(string guildId, GuildMember member)
        {
            Guild guild = GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            // Check if player is already in a guild
            if (playerToGuild.ContainsKey(member.PlayerId))
            {
                Debug.LogError("Player is already in a guild.");
                return false;
            }
            
            if (guild.AddMember(member, guildData))
            {
                playerToGuild[member.PlayerId] = guildId;
                OnMemberJoined?.Invoke(guild, member);
                Debug.Log($"Player '{member.PlayerName}' joined guild '{guild.GuildName}'.");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Remove member from guild
        /// Xóa thành viên khỏi guild
        /// </summary>
        public bool RemoveMemberFromGuild(string guildId, string playerId)
        {
            Guild guild = GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            if (guild.RemoveMember(playerId))
            {
                playerToGuild.Remove(playerId);
                OnMemberLeft?.Invoke(guild, playerId);
                Debug.Log($"Player removed from guild '{guild.GuildName}'.");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Player leaves guild
        /// Người chơi rời guild
        /// </summary>
        public bool LeaveGuild(string playerId)
        {
            if (!playerToGuild.TryGetValue(playerId, out string guildId))
            {
                Debug.LogError("Player is not in a guild.");
                return false;
            }
            
            Guild guild = GetGuild(guildId);
            if (guild == null)
            {
                return false;
            }
            
            // Guild master cannot leave, must transfer or disband
            if (guild.GuildMasterId == playerId)
            {
                Debug.LogError("Guild Master must transfer leadership or disband the guild.");
                return false;
            }
            
            return RemoveMemberFromGuild(guildId, playerId);
        }
        
        #endregion
        
        #region Guild Search / Tìm kiếm Guild
        
        /// <summary>
        /// Get all guilds
        /// Lấy tất cả guilds
        /// </summary>
        public List<Guild> GetAllGuilds()
        {
            return guilds.Values.ToList();
        }
        
        /// <summary>
        /// Search guilds by criteria
        /// Tìm kiếm guilds theo tiêu chí
        /// </summary>
        public List<Guild> SearchGuilds(string searchTerm = "", GuildType? type = null, int minLevel = 0)
        {
            var results = guilds.Values.AsEnumerable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                results = results.Where(g => 
                    g.GuildName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }
            
            if (type.HasValue)
            {
                results = results.Where(g => g.Type == type.Value);
            }
            
            if (minLevel > 0)
            {
                results = results.Where(g => g.Level >= minLevel);
            }
            
            return results.OrderByDescending(g => g.Level)
                         .ThenByDescending(g => g.Members.Count)
                         .ToList();
        }
        
        /// <summary>
        /// Get top guilds by level
        /// Lấy top guilds theo cấp độ
        /// </summary>
        public List<Guild> GetTopGuilds(int count = 10)
        {
            return guilds.Values
                .OrderByDescending(g => g.Level)
                .ThenByDescending(g => g.Points)
                .Take(count)
                .ToList();
        }
        
        #endregion
        
        #region Utility / Tiện ích
        
        /// <summary>
        /// Check if player can create guild
        /// Kiểm tra người chơi có thể tạo guild không
        /// </summary>
        public bool CanCreateGuild(string playerId, int playerLevel, int playerZen)
        {
            if (playerToGuild.ContainsKey(playerId))
            {
                Debug.LogError("Player is already in a guild.");
                return false;
            }
            
            if (playerLevel < guildData.RequiredLevel)
            {
                Debug.LogError($"Minimum level {guildData.RequiredLevel} required.");
                return false;
            }
            
            if (playerZen < guildData.RequiredZen)
            {
                Debug.LogError($"Minimum {guildData.RequiredZen} Zen required.");
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Get guild data configuration
        /// Lấy cấu hình dữ liệu guild
        /// </summary>
        public GuildData GetGuildData()
        {
            return guildData;
        }
        
        #endregion
    }
}
