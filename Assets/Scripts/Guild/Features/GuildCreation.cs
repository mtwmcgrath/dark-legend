using System;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild creation system with requirements validation
    /// Hệ thống tạo guild với kiểm tra yêu cầu
    /// </summary>
    public class GuildCreation : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        /// <summary>
        /// Requirements for creating a guild
        /// Yêu cầu để tạo guild
        /// </summary>
        [Serializable]
        public class GuildCreationRequirement
        {
            public int RequiredLevel = 100;
            public int RequiredZen = 1000000;
            public string RequiredItemId;  // Optional special item
            public int MinimumMembers = 1;
        }
        
        /// <summary>
        /// Request to create a guild
        /// Yêu cầu tạo guild
        /// </summary>
        [Serializable]
        public class GuildCreationRequest
        {
            public string PlayerId;
            public string PlayerName;
            public int PlayerLevel;
            public int PlayerZen;
            public string CharacterClass;
            
            public string GuildName;
            public GuildType GuildType;
            public byte[] GuildMark;  // 8x8 pixels
            public string Description;
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        /// <summary>
        /// Validate guild name
        /// Kiểm tra tên guild hợp lệ
        /// </summary>
        public bool ValidateGuildName(string guildName, out string error)
        {
            error = null;
            GuildData data = guildManager.GetGuildData();
            
            if (string.IsNullOrEmpty(guildName))
            {
                error = "Guild name cannot be empty.";
                return false;
            }
            
            if (guildName.Length < data.MinNameLength)
            {
                error = $"Guild name must be at least {data.MinNameLength} characters.";
                return false;
            }
            
            if (guildName.Length > data.MaxNameLength)
            {
                error = $"Guild name must be at most {data.MaxNameLength} characters.";
                return false;
            }
            
            // Check for invalid characters
            if (!System.Text.RegularExpressions.Regex.IsMatch(guildName, @"^[a-zA-Z0-9_\s]+$"))
            {
                error = "Guild name can only contain letters, numbers, spaces and underscores.";
                return false;
            }
            
            // Check if name is already taken
            if (guildManager.GetGuildByName(guildName) != null)
            {
                error = "Guild name is already taken.";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Validate guild mark/logo
        /// Kiểm tra logo guild hợp lệ
        /// </summary>
        public bool ValidateGuildMark(byte[] guildMark, out string error)
        {
            error = null;
            
            if (guildMark == null)
            {
                // Guild mark is optional
                return true;
            }
            
            // Guild mark should be 8x8 pixels (64 bytes for RGBA or 192 for RGB)
            if (guildMark.Length != 64 && guildMark.Length != 192 && guildMark.Length != 256)
            {
                error = "Invalid guild mark size. Must be 8x8 pixels.";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Check if player meets guild creation requirements
        /// Kiểm tra người chơi đáp ứng yêu cầu tạo guild
        /// </summary>
        public bool CheckRequirements(GuildCreationRequest request, out string error)
        {
            error = null;
            GuildData data = guildManager.GetGuildData();
            
            // Check if player is already in a guild
            if (guildManager.GetPlayerGuild(request.PlayerId) != null)
            {
                error = "You are already in a guild.";
                return false;
            }
            
            // Check level requirement
            if (request.PlayerLevel < data.RequiredLevel)
            {
                error = $"You must be at least level {data.RequiredLevel} to create a guild.";
                return false;
            }
            
            // Check zen requirement
            if (request.PlayerZen < data.RequiredZen)
            {
                error = $"You need at least {data.RequiredZen:N0} Zen to create a guild.";
                return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Create guild with validation
        /// Tạo guild với kiểm tra
        /// </summary>
        public Guild CreateGuild(GuildCreationRequest request, out string error)
        {
            error = null;
            
            // Validate guild name
            if (!ValidateGuildName(request.GuildName, out error))
            {
                return null;
            }
            
            // Validate guild mark
            if (!ValidateGuildMark(request.GuildMark, out error))
            {
                return null;
            }
            
            // Check requirements
            if (!CheckRequirements(request, out error))
            {
                return null;
            }
            
            // Create the guild
            Guild guild = guildManager.CreateGuild(
                request.GuildName,
                request.PlayerId,
                request.GuildType,
                request.GuildMark
            );
            
            if (guild == null)
            {
                error = "Failed to create guild. Please try again.";
                return null;
            }
            
            // Set description
            guild.Description = request.Description ?? "";
            
            // Update guild master member info
            GuildMember master = guild.GetMember(request.PlayerId);
            if (master != null)
            {
                master.PlayerName = request.PlayerName;
                master.PlayerLevel = request.PlayerLevel;
                master.CharacterClass = request.CharacterClass;
            }
            
            // Deduct zen cost (this should be done in the calling code)
            // DeductPlayerZen(request.PlayerId, data.RequiredZen);
            
            Debug.Log($"Guild '{guild.GuildName}' created successfully by {request.PlayerName}!");
            OnGuildCreated(guild);
            
            return guild;
        }
        
        /// <summary>
        /// Called when guild is successfully created
        /// Được gọi khi guild được tạo thành công
        /// </summary>
        private void OnGuildCreated(Guild guild)
        {
            // Trigger events, achievements, notifications
            // Send notification to player
            // Log to database
        }
        
        /// <summary>
        /// Generate default guild mark
        /// Tạo logo guild mặc định
        /// </summary>
        public byte[] GenerateDefaultGuildMark()
        {
            // Generate a simple 8x8 default pattern
            byte[] mark = new byte[192]; // 8x8 RGB
            
            // Simple pattern: border
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int index = (y * 8 + x) * 3;
                    
                    // Border pixels
                    if (x == 0 || x == 7 || y == 0 || y == 7)
                    {
                        mark[index] = 255;     // R
                        mark[index + 1] = 255; // G
                        mark[index + 2] = 255; // B
                    }
                }
            }
            
            return mark;
        }
    }
}
