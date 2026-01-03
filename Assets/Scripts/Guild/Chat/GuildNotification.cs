using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild notification system for real-time updates
    /// Hệ thống thông báo guild cho cập nhật thời gian thực
    /// </summary>
    public class GuildNotification : MonoBehaviour
    {
        /// <summary>
        /// Guild notification
        /// Thông báo guild
        /// </summary>
        [Serializable]
        public class Notification
        {
            public string NotificationId;
            public string GuildId;
            public string Title;
            public string Message;
            public DateTime Timestamp;
            public NotificationType Type;
            public bool IsRead;
        }
        
        /// <summary>
        /// Notification types
        /// Loại thông báo
        /// </summary>
        public enum NotificationType
        {
            MemberJoined,       // Thành viên mới gia nhập
            MemberLeft,         // Thành viên rời đi
            MemberPromoted,     // Thành viên được thăng cấp
            MemberDemoted,      // Thành viên bị giáng cấp
            GuildLevelUp,       // Guild lên cấp
            WarDeclared,        // Tuyên chiến
            WarStarted,         // Bắt đầu chiến tranh
            WarEnded,           // Kết thúc chiến tranh
            BuffActivated,      // Buff được kích hoạt
            QuestCompleted,     // Nhiệm vụ hoàn thành
            Achievement,        // Thành tựu
            System              // Hệ thống
        }
        
        // Notifications per guild / Thông báo cho mỗi guild
        private Dictionary<string, List<Notification>> notifications = 
            new Dictionary<string, List<Notification>>();
        
        // Events / Sự kiện
        public event Action<Notification> OnNotificationReceived;
        
        [Header("Settings")]
        [SerializeField] private int maxNotifications = 50;
        
        /// <summary>
        /// Send notification to guild
        /// Gửi thông báo tới guild
        /// </summary>
        public void SendNotification(string guildId, string title, string message, NotificationType type)
        {
            Notification notification = new Notification
            {
                NotificationId = Guid.NewGuid().ToString(),
                GuildId = guildId,
                Title = title,
                Message = message,
                Timestamp = DateTime.Now,
                Type = type,
                IsRead = false
            };
            
            if (!notifications.ContainsKey(guildId))
            {
                notifications[guildId] = new List<Notification>();
            }
            
            notifications[guildId].Add(notification);
            
            // Keep only recent notifications
            if (notifications[guildId].Count > maxNotifications)
            {
                notifications[guildId].RemoveAt(0);
            }
            
            Debug.Log($"[{type}] {title}: {message}");
            OnNotificationReceived?.Invoke(notification);
        }
        
        /// <summary>
        /// Get notifications for guild
        /// Lấy thông báo cho guild
        /// </summary>
        public List<Notification> GetNotifications(string guildId, bool unreadOnly = false)
        {
            if (!notifications.ContainsKey(guildId))
            {
                return new List<Notification>();
            }
            
            if (unreadOnly)
            {
                return notifications[guildId].FindAll(n => !n.IsRead);
            }
            
            return new List<Notification>(notifications[guildId]);
        }
        
        /// <summary>
        /// Mark notification as read
        /// Đánh dấu thông báo đã đọc
        /// </summary>
        public bool MarkAsRead(string guildId, string notificationId)
        {
            if (!notifications.ContainsKey(guildId))
            {
                return false;
            }
            
            Notification notification = notifications[guildId]
                .Find(n => n.NotificationId == notificationId);
            
            if (notification != null)
            {
                notification.IsRead = true;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Mark all notifications as read
        /// Đánh dấu tất cả thông báo đã đọc
        /// </summary>
        public void MarkAllAsRead(string guildId)
        {
            if (!notifications.ContainsKey(guildId))
            {
                return;
            }
            
            foreach (var notification in notifications[guildId])
            {
                notification.IsRead = true;
            }
        }
        
        /// <summary>
        /// Clear notifications
        /// Xóa thông báo
        /// </summary>
        public void ClearNotifications(string guildId)
        {
            if (notifications.ContainsKey(guildId))
            {
                notifications[guildId].Clear();
            }
        }
        
        /// <summary>
        /// Get unread notification count
        /// Lấy số lượng thông báo chưa đọc
        /// </summary>
        public int GetUnreadCount(string guildId)
        {
            if (!notifications.ContainsKey(guildId))
            {
                return 0;
            }
            
            return notifications[guildId].FindAll(n => !n.IsRead).Count;
        }
    }
}
