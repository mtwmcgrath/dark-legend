using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild War Manager - Coordinates all guild wars
    /// Quản lý Guild War - Điều phối tất cả các cuộc chiến guild
    /// </summary>
    public class GuildWarManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        [SerializeField] private GuildWar guildWar;
        
        // All war declarations / Tất cả tuyên chiến
        private Dictionary<string, GuildWar.GuildWarDeclaration> warDeclarations = 
            new Dictionary<string, GuildWar.GuildWarDeclaration>();
        
        // Active wars / Chiến tranh đang diễn ra
        private Dictionary<string, GuildWar.ActiveGuildWar> activeWars = 
            new Dictionary<string, GuildWar.ActiveGuildWar>();
        
        // War cooldowns (guild can't declare war for X hours after losing)
        // Thời gian chờ chiến tranh (guild không thể tuyên chiến trong X giờ sau khi thua)
        private Dictionary<string, DateTime> warCooldowns = new Dictionary<string, DateTime>();
        
        [Header("Settings")]
        [SerializeField] private int warCooldownHours = 24;
        [SerializeField] private int maxSimultaneousWars = 3;
        [SerializeField] private int declarationExpirationHours = 48;
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
            
            if (guildWar == null)
            {
                guildWar = GetComponent<GuildWar>();
            }
        }
        
        private void Update()
        {
            CheckActiveWars();
            CleanupExpiredDeclarations();
        }
        
        /// <summary>
        /// Declare war on another guild
        /// Tuyên chiến với guild khác
        /// </summary>
        public GuildWar.GuildWarDeclaration DeclareWar(
            string attackingGuildId,
            string defendingGuildId,
            string declarerId,
            GuildWar.GuildWarType type,
            DateTime scheduledTime,
            int betAmount = 0)
        {
            // Check if guild is on cooldown
            if (IsOnCooldown(attackingGuildId))
            {
                Debug.LogError($"Guild is on war cooldown. Cannot declare war.");
                return null;
            }
            
            // Check if guild already has too many active wars
            int activeWarCount = GetGuildActiveWarCount(attackingGuildId);
            if (activeWarCount >= maxSimultaneousWars)
            {
                Debug.LogError($"Guild already has {maxSimultaneousWars} active wars.");
                return null;
            }
            
            // Check if there's already a pending declaration between these guilds
            if (HasPendingDeclaration(attackingGuildId, defendingGuildId))
            {
                Debug.LogError("There's already a pending war declaration between these guilds.");
                return null;
            }
            
            // Declare war
            GuildWar.GuildWarDeclaration declaration = guildWar.DeclareWar(
                attackingGuildId,
                defendingGuildId,
                declarerId,
                type,
                scheduledTime,
                betAmount
            );
            
            if (declaration != null)
            {
                warDeclarations[declaration.WarId] = declaration;
            }
            
            return declaration;
        }
        
        /// <summary>
        /// Accept war declaration
        /// Chấp nhận tuyên chiến
        /// </summary>
        public bool AcceptWarDeclaration(string warId, string defendingGuildId, string accepterId)
        {
            if (!warDeclarations.TryGetValue(warId, out GuildWar.GuildWarDeclaration declaration))
            {
                Debug.LogError("War declaration not found.");
                return false;
            }
            
            if (declaration.DefendingGuildId != defendingGuildId)
            {
                Debug.LogError("This war declaration is not for your guild.");
                return false;
            }
            
            if (declaration.Status != GuildWar.GuildWarStatus.Pending)
            {
                Debug.LogError("War declaration is no longer pending.");
                return false;
            }
            
            if (guildWar.AcceptWar(warId, defendingGuildId, accepterId))
            {
                declaration.Status = GuildWar.GuildWarStatus.Accepted;
                
                // Schedule the war to start
                ScheduleWarStart(declaration);
                
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Decline war declaration
        /// Từ chối tuyên chiến
        /// </summary>
        public bool DeclineWarDeclaration(string warId, string defendingGuildId, string declinerId)
        {
            if (!warDeclarations.TryGetValue(warId, out GuildWar.GuildWarDeclaration declaration))
            {
                Debug.LogError("War declaration not found.");
                return false;
            }
            
            if (declaration.DefendingGuildId != defendingGuildId)
            {
                Debug.LogError("This war declaration is not for your guild.");
                return false;
            }
            
            Guild defendingGuild = guildManager.GetGuild(defendingGuildId);
            if (defendingGuild == null)
            {
                return false;
            }
            
            GuildMember decliner = defendingGuild.GetMember(declinerId);
            if (decliner == null || !decliner.GetPermissions().CanDeclareWar)
            {
                Debug.LogError("You don't have permission to decline wars.");
                return false;
            }
            
            declaration.Status = GuildWar.GuildWarStatus.Cancelled;
            Debug.Log($"War declaration declined by {defendingGuild.GuildName}");
            
            return true;
        }
        
        /// <summary>
        /// Start war at scheduled time
        /// Bắt đầu chiến tranh vào thời gian đã lên lịch
        /// </summary>
        private void ScheduleWarStart(GuildWar.GuildWarDeclaration declaration)
        {
            // In a real implementation, this would use a scheduler
            // For now, start immediately if scheduled time has passed
            if (DateTime.Now >= declaration.ScheduledTime)
            {
                StartWar(declaration.WarId);
            }
        }
        
        /// <summary>
        /// Start a guild war
        /// Bắt đầu chiến tranh guild
        /// </summary>
        public bool StartWar(string warId)
        {
            if (!warDeclarations.TryGetValue(warId, out GuildWar.GuildWarDeclaration declaration))
            {
                Debug.LogError("War declaration not found.");
                return false;
            }
            
            if (declaration.Status != GuildWar.GuildWarStatus.Accepted)
            {
                Debug.LogError("War must be accepted before starting.");
                return false;
            }
            
            GuildWar.ActiveGuildWar war = guildWar.StartWar(declaration);
            if (war != null)
            {
                activeWars[warId] = war;
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Register a kill in active war
        /// Đăng ký giết trong chiến tranh đang diễn ra
        /// </summary>
        public bool RegisterWarKill(string warId, string killerId, string victimId)
        {
            if (!activeWars.TryGetValue(warId, out GuildWar.ActiveGuildWar war))
            {
                return false;
            }
            
            return guildWar.RegisterKill(war, killerId, victimId);
        }
        
        /// <summary>
        /// Check and end active wars that have expired
        /// Kiểm tra và kết thúc chiến tranh đang diễn ra đã hết hạn
        /// </summary>
        private void CheckActiveWars()
        {
            List<string> warsToEnd = new List<string>();
            
            foreach (var kvp in activeWars)
            {
                if (!kvp.Value.IsActive)
                {
                    warsToEnd.Add(kvp.Key);
                }
            }
            
            foreach (string warId in warsToEnd)
            {
                EndWar(warId);
            }
        }
        
        /// <summary>
        /// End a guild war
        /// Kết thúc chiến tranh guild
        /// </summary>
        public void EndWar(string warId)
        {
            if (!activeWars.TryGetValue(warId, out GuildWar.ActiveGuildWar war))
            {
                return;
            }
            
            guildWar.EndWar(war);
            
            // Set cooldown for loser
            string loserGuildId = war.WinningGuildId == war.Declaration.AttackingGuildId ?
                war.Declaration.DefendingGuildId : war.Declaration.AttackingGuildId;
            
            if (!string.IsNullOrEmpty(loserGuildId))
            {
                warCooldowns[loserGuildId] = DateTime.Now.AddHours(warCooldownHours);
            }
            
            activeWars.Remove(warId);
        }
        
        /// <summary>
        /// Get all war declarations for a guild
        /// Lấy tất cả tuyên chiến cho một guild
        /// </summary>
        public List<GuildWar.GuildWarDeclaration> GetGuildWarDeclarations(string guildId)
        {
            return warDeclarations.Values
                .Where(d => d.AttackingGuildId == guildId || d.DefendingGuildId == guildId)
                .Where(d => d.Status == GuildWar.GuildWarStatus.Pending || 
                           d.Status == GuildWar.GuildWarStatus.Accepted)
                .ToList();
        }
        
        /// <summary>
        /// Get active war for guild
        /// Lấy chiến tranh đang diễn ra cho guild
        /// </summary>
        public GuildWar.ActiveGuildWar GetGuildActiveWar(string guildId)
        {
            return activeWars.Values
                .FirstOrDefault(w => w.Declaration.AttackingGuildId == guildId || 
                                    w.Declaration.DefendingGuildId == guildId);
        }
        
        /// <summary>
        /// Get count of active wars for guild
        /// Lấy số lượng chiến tranh đang diễn ra cho guild
        /// </summary>
        private int GetGuildActiveWarCount(string guildId)
        {
            return activeWars.Values
                .Count(w => w.Declaration.AttackingGuildId == guildId || 
                           w.Declaration.DefendingGuildId == guildId);
        }
        
        /// <summary>
        /// Check if guild is on war cooldown
        /// Kiểm tra guild có đang trong thời gian chờ chiến tranh không
        /// </summary>
        public bool IsOnCooldown(string guildId)
        {
            if (warCooldowns.TryGetValue(guildId, out DateTime cooldownEnd))
            {
                if (DateTime.Now < cooldownEnd)
                {
                    return true;
                }
                warCooldowns.Remove(guildId);
            }
            return false;
        }
        
        /// <summary>
        /// Get remaining cooldown time
        /// Lấy thời gian chờ còn lại
        /// </summary>
        public TimeSpan GetRemainingCooldown(string guildId)
        {
            if (warCooldowns.TryGetValue(guildId, out DateTime cooldownEnd))
            {
                TimeSpan remaining = cooldownEnd - DateTime.Now;
                return remaining.TotalSeconds > 0 ? remaining : TimeSpan.Zero;
            }
            return TimeSpan.Zero;
        }
        
        /// <summary>
        /// Check if there's a pending declaration between guilds
        /// Kiểm tra có tuyên chiến đang chờ giữa các guild không
        /// </summary>
        private bool HasPendingDeclaration(string guildId1, string guildId2)
        {
            return warDeclarations.Values.Any(d =>
                d.Status == GuildWar.GuildWarStatus.Pending &&
                ((d.AttackingGuildId == guildId1 && d.DefendingGuildId == guildId2) ||
                 (d.AttackingGuildId == guildId2 && d.DefendingGuildId == guildId1))
            );
        }
        
        /// <summary>
        /// Clean up expired declarations
        /// Dọn dẹp tuyên chiến hết hạn
        /// </summary>
        private void CleanupExpiredDeclarations()
        {
            List<string> expiredDeclarations = new List<string>();
            
            foreach (var kvp in warDeclarations)
            {
                if (kvp.Value.Status == GuildWar.GuildWarStatus.Pending)
                {
                    TimeSpan age = DateTime.Now - kvp.Value.DeclarationTime;
                    if (age.TotalHours > declarationExpirationHours)
                    {
                        expiredDeclarations.Add(kvp.Key);
                    }
                }
            }
            
            foreach (string warId in expiredDeclarations)
            {
                warDeclarations[warId].Status = GuildWar.GuildWarStatus.Cancelled;
                Debug.Log($"War declaration {warId} expired");
            }
        }
        
        /// <summary>
        /// Get war statistics
        /// Lấy thống kê chiến tranh
        /// </summary>
        public WarStatistics GetWarStatistics()
        {
            return new WarStatistics
            {
                TotalDeclarations = warDeclarations.Count,
                PendingDeclarations = warDeclarations.Values.Count(d => d.Status == GuildWar.GuildWarStatus.Pending),
                ActiveWars = activeWars.Count,
                CompletedWars = warDeclarations.Values.Count(d => d.Status == GuildWar.GuildWarStatus.Completed)
            };
        }
        
        [Serializable]
        public class WarStatistics
        {
            public int TotalDeclarations;
            public int PendingDeclarations;
            public int ActiveWars;
            public int CompletedWars;
        }
    }
}
