using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild chat system
    /// Hệ thống chat guild
    /// </summary>
    public class GuildChat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        [Header("Settings")]
        [SerializeField] private int maxChatHistory = 100;
        
        // Chat history per guild / Lịch sử chat cho mỗi guild
        private Dictionary<string, List<ChatMessage>> chatHistory = 
            new Dictionary<string, List<ChatMessage>>();
        
        /// <summary>
        /// Chat message
        /// Tin nhắn chat
        /// </summary>
        [Serializable]
        public class ChatMessage
        {
            public string MessageId;
            public string SenderId;
            public string SenderName;
            public GuildRank SenderRank;
            public string Message;
            public DateTime Timestamp;
            public MessageType Type;
        }
        
        /// <summary>
        /// Message types
        /// Loại tin nhắn
        /// </summary>
        public enum MessageType
        {
            Normal,         // Chat thường
            System,         // Thông báo hệ thống
            Announcement,   // Thông báo guild
            Warning         // Cảnh báo
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        /// <summary>
        /// Send message to guild chat
        /// Gửi tin nhắn vào chat guild
        /// </summary>
        public bool SendMessage(string guildId, string senderId, string message, MessageType type = MessageType.Normal)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            GuildMember sender = guild.GetMember(senderId);
            if (sender == null)
            {
                Debug.LogError("Sender is not a member of the guild.");
                return false;
            }
            
            // Create message
            ChatMessage chatMessage = new ChatMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                SenderId = senderId,
                SenderName = sender.PlayerName,
                SenderRank = sender.Rank,
                Message = message,
                Timestamp = DateTime.Now,
                Type = type
            };
            
            // Add to history
            if (!chatHistory.ContainsKey(guildId))
            {
                chatHistory[guildId] = new List<ChatMessage>();
            }
            
            chatHistory[guildId].Add(chatMessage);
            
            // Keep only recent messages
            if (chatHistory[guildId].Count > maxChatHistory)
            {
                chatHistory[guildId].RemoveAt(0);
            }
            
            OnMessageSent(guildId, chatMessage);
            
            return true;
        }
        
        /// <summary>
        /// Send system message
        /// Gửi tin nhắn hệ thống
        /// </summary>
        public void SendSystemMessage(string guildId, string message)
        {
            ChatMessage chatMessage = new ChatMessage
            {
                MessageId = Guid.NewGuid().ToString(),
                SenderId = "SYSTEM",
                SenderName = "System",
                SenderRank = GuildRank.GuildMaster,
                Message = message,
                Timestamp = DateTime.Now,
                Type = MessageType.System
            };
            
            if (!chatHistory.ContainsKey(guildId))
            {
                chatHistory[guildId] = new List<ChatMessage>();
            }
            
            chatHistory[guildId].Add(chatMessage);
            
            if (chatHistory[guildId].Count > maxChatHistory)
            {
                chatHistory[guildId].RemoveAt(0);
            }
            
            OnMessageSent(guildId, chatMessage);
        }
        
        /// <summary>
        /// Get chat history for guild
        /// Lấy lịch sử chat cho guild
        /// </summary>
        public List<ChatMessage> GetChatHistory(string guildId, int limit = 50)
        {
            if (!chatHistory.ContainsKey(guildId))
            {
                return new List<ChatMessage>();
            }
            
            return chatHistory[guildId]
                .OrderByDescending(m => m.Timestamp)
                .Take(limit)
                .Reverse()
                .ToList();
        }
        
        /// <summary>
        /// Clear chat history
        /// Xóa lịch sử chat
        /// </summary>
        public void ClearChatHistory(string guildId)
        {
            if (chatHistory.ContainsKey(guildId))
            {
                chatHistory[guildId].Clear();
            }
        }
        
        /// <summary>
        /// Called when message is sent
        /// Được gọi khi tin nhắn được gửi
        /// </summary>
        private void OnMessageSent(string guildId, ChatMessage message)
        {
            // Broadcast to all online guild members
            // Update UI
        }
    }
}
