using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild announcement system for important messages
    /// Hệ thống thông báo guild cho tin nhắn quan trọng
    /// </summary>
    public class GuildAnnouncement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        /// <summary>
        /// Guild announcement
        /// Thông báo guild
        /// </summary>
        [Serializable]
        public class Announcement
        {
            public string AnnouncementId;
            public string GuildId;
            public string Title;
            public string Message;
            public string AuthorId;
            public string AuthorName;
            public DateTime PostTime;
            public AnnouncementPriority Priority;
        }
        
        /// <summary>
        /// Announcement priority levels
        /// Mức độ ưu tiên thông báo
        /// </summary>
        public enum AnnouncementPriority
        {
            Low,        // Thông tin chung
            Normal,     // Thông báo thường
            High,       // Quan trọng
            Critical    // Khẩn cấp
        }
        
        // Announcements per guild / Thông báo cho mỗi guild
        private Dictionary<string, List<Announcement>> announcements = 
            new Dictionary<string, List<Announcement>>();
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        /// <summary>
        /// Post guild announcement
        /// Đăng thông báo guild
        /// </summary>
        public bool PostAnnouncement(string guildId, string authorId, string title, string message, AnnouncementPriority priority = AnnouncementPriority.Normal)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            GuildMember author = guild.GetMember(authorId);
            if (author == null)
            {
                Debug.LogError("Author is not a member of the guild.");
                return false;
            }
            
            // Check permissions
            GuildRankPermissions permissions = author.GetPermissions();
            if (!permissions.CanEditGuildNotice)
            {
                Debug.LogError("You don't have permission to post announcements.");
                return false;
            }
            
            Announcement announcement = new Announcement
            {
                AnnouncementId = Guid.NewGuid().ToString(),
                GuildId = guildId,
                Title = title,
                Message = message,
                AuthorId = authorId,
                AuthorName = author.PlayerName,
                PostTime = DateTime.Now,
                Priority = priority
            };
            
            if (!announcements.ContainsKey(guildId))
            {
                announcements[guildId] = new List<Announcement>();
            }
            
            announcements[guildId].Add(announcement);
            
            Debug.Log($"Guild announcement posted: {title}");
            OnAnnouncementPosted(announcement);
            
            return true;
        }
        
        /// <summary>
        /// Get guild announcements
        /// Lấy thông báo guild
        /// </summary>
        public List<Announcement> GetAnnouncements(string guildId, int limit = 10)
        {
            if (!announcements.ContainsKey(guildId))
            {
                return new List<Announcement>();
            }
            
            return announcements[guildId]
                .OrderByDescending(a => a.PostTime)
                .Take(limit)
                .ToList();
        }
        
        /// <summary>
        /// Delete announcement
        /// Xóa thông báo
        /// </summary>
        public bool DeleteAnnouncement(string guildId, string announcementId, string requesterId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return false;
            }
            
            GuildMember requester = guild.GetMember(requesterId);
            if (requester == null || !requester.GetPermissions().CanEditGuildNotice)
            {
                Debug.LogError("You don't have permission to delete announcements.");
                return false;
            }
            
            if (!announcements.ContainsKey(guildId))
            {
                return false;
            }
            
            Announcement announcement = announcements[guildId]
                .Find(a => a.AnnouncementId == announcementId);
            
            if (announcement != null)
            {
                announcements[guildId].Remove(announcement);
                Debug.Log("Announcement deleted.");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Called when announcement is posted
        /// Được gọi khi thông báo được đăng
        /// </summary>
        private void OnAnnouncementPosted(Announcement announcement)
        {
            // Send notification to all guild members
            // Update UI
            // Log to chat if high priority
        }
    }
}
