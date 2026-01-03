using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild War system - Core mechanics for guild vs guild combat
    /// Hệ thống Guild War - Cơ chế cốt lõi cho chiến đấu guild vs guild
    /// </summary>
    public class GuildWar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        /// <summary>
        /// Guild war types
        /// Loại chiến tranh guild
        /// </summary>
        public enum GuildWarType
        {
            Skirmish,       // 5v5, 15 minutes
            Battle,         // 10v10, 30 minutes
            FullWar,        // All members, 1 hour
            Territory       // Giành lãnh thổ
        }
        
        /// <summary>
        /// Guild war status
        /// Trạng thái chiến tranh guild
        /// </summary>
        public enum GuildWarStatus
        {
            Pending,        // Chờ chấp nhận
            Accepted,       // Đã chấp nhận
            InProgress,     // Đang diễn ra
            Completed,      // Hoàn thành
            Cancelled       // Bị hủy
        }
        
        /// <summary>
        /// Guild war declaration
        /// Tuyên chiến Guild War
        /// </summary>
        [Serializable]
        public class GuildWarDeclaration
        {
            public string WarId;
            public string AttackingGuildId;
            public string AttackingGuildName;
            public string DefendingGuildId;
            public string DefendingGuildName;
            public GuildWarType Type;
            public DateTime DeclarationTime;
            public DateTime ScheduledTime;
            public int BetAmount;           // Optional bet
            public GuildWarStatus Status;
        }
        
        /// <summary>
        /// Active guild war session
        /// Phiên chiến tranh guild đang diễn ra
        /// </summary>
        [Serializable]
        public class ActiveGuildWar
        {
            public string WarId;
            public GuildWarDeclaration Declaration;
            public DateTime StartTime;
            public DateTime EndTime;
            public int Duration;            // Minutes
            
            // Scores / Điểm số
            public int AttackingScore;
            public int DefendingScore;
            
            // Participants / Người tham gia
            public List<string> AttackingParticipants;
            public List<string> DefendingParticipants;
            
            // Kill records / Bản ghi giết
            public List<GuildWarKill> KillLog;
            
            public bool IsActive => DateTime.Now < EndTime;
            public string WinningGuildId
            {
                get
                {
                    if (AttackingScore > DefendingScore)
                        return Declaration.AttackingGuildId;
                    else if (DefendingScore > AttackingScore)
                        return Declaration.DefendingGuildId;
                    return null; // Tie
                }
            }
        }
        
        /// <summary>
        /// Kill record in guild war
        /// Bản ghi giết trong Guild War
        /// </summary>
        [Serializable]
        public class GuildWarKill
        {
            public string KillerId;
            public string KillerName;
            public string KillerGuildId;
            public string VictimId;
            public string VictimName;
            public string VictimGuildId;
            public GuildRank VictimRank;
            public int PointsAwarded;
            public DateTime Timestamp;
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        /// <summary>
        /// Declare guild war
        /// Tuyên chiến Guild War
        /// </summary>
        public GuildWarDeclaration DeclareWar(
            string attackingGuildId, 
            string defendingGuildId, 
            string declarerId,
            GuildWarType type, 
            DateTime scheduledTime, 
            int betAmount = 0)
        {
            Guild attackingGuild = guildManager.GetGuild(attackingGuildId);
            Guild defendingGuild = guildManager.GetGuild(defendingGuildId);
            
            if (attackingGuild == null || defendingGuild == null)
            {
                Debug.LogError("One or both guilds not found.");
                return null;
            }
            
            // Check if declarer is guild master
            GuildMember declarer = attackingGuild.GetMember(declarerId);
            if (declarer == null || !declarer.GetPermissions().CanDeclareWar)
            {
                Debug.LogError("You don't have permission to declare war.");
                return null;
            }
            
            // Check bet amount
            if (betAmount > 0 && attackingGuild.Points < betAmount)
            {
                Debug.LogError("Insufficient guild points for bet.");
                return null;
            }
            
            // Create war declaration
            GuildWarDeclaration declaration = new GuildWarDeclaration
            {
                WarId = Guid.NewGuid().ToString(),
                AttackingGuildId = attackingGuildId,
                AttackingGuildName = attackingGuild.GuildName,
                DefendingGuildId = defendingGuildId,
                DefendingGuildName = defendingGuild.GuildName,
                Type = type,
                DeclarationTime = DateTime.Now,
                ScheduledTime = scheduledTime,
                BetAmount = betAmount,
                Status = GuildWarStatus.Pending
            };
            
            Debug.Log($"Guild '{attackingGuild.GuildName}' declared war on '{defendingGuild.GuildName}'!");
            OnWarDeclared(declaration);
            
            return declaration;
        }
        
        /// <summary>
        /// Accept guild war
        /// Chấp nhận Guild War
        /// </summary>
        public bool AcceptWar(string warId, string defendingGuildId, string accepterId)
        {
            Guild defendingGuild = guildManager.GetGuild(defendingGuildId);
            if (defendingGuild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            GuildMember accepter = defendingGuild.GetMember(accepterId);
            if (accepter == null || !accepter.GetPermissions().CanDeclareWar)
            {
                Debug.LogError("You don't have permission to accept wars.");
                return false;
            }
            
            Debug.Log($"Guild '{defendingGuild.GuildName}' accepted the war!");
            return true;
        }
        
        /// <summary>
        /// Start guild war
        /// Bắt đầu Guild War
        /// </summary>
        public ActiveGuildWar StartWar(GuildWarDeclaration declaration)
        {
            if (declaration.Status != GuildWarStatus.Accepted)
            {
                Debug.LogError("War must be accepted before starting.");
                return null;
            }
            
            int duration = GetWarDuration(declaration.Type);
            
            ActiveGuildWar war = new ActiveGuildWar
            {
                WarId = declaration.WarId,
                Declaration = declaration,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(duration),
                Duration = duration,
                AttackingScore = 0,
                DefendingScore = 0,
                AttackingParticipants = new List<string>(),
                DefendingParticipants = new List<string>(),
                KillLog = new List<GuildWarKill>()
            };
            
            declaration.Status = GuildWarStatus.InProgress;
            
            Debug.Log($"Guild War started! {declaration.AttackingGuildName} vs {declaration.DefendingGuildName}");
            OnWarStarted(war);
            
            return war;
        }
        
        /// <summary>
        /// Register kill in guild war
        /// Đăng ký giết trong Guild War
        /// </summary>
        public bool RegisterKill(ActiveGuildWar war, string killerId, string victimId)
        {
            if (!war.IsActive)
            {
                Debug.LogError("War is not active.");
                return false;
            }
            
            Guild killerGuild = war.AttackingParticipants.Contains(killerId) ?
                guildManager.GetGuild(war.Declaration.AttackingGuildId) :
                guildManager.GetGuild(war.Declaration.DefendingGuildId);
            
            Guild victimGuild = war.AttackingParticipants.Contains(victimId) ?
                guildManager.GetGuild(war.Declaration.AttackingGuildId) :
                guildManager.GetGuild(war.Declaration.DefendingGuildId);
            
            if (killerGuild == null || victimGuild == null)
            {
                return false;
            }
            
            GuildMember killer = killerGuild.GetMember(killerId);
            GuildMember victim = victimGuild.GetMember(victimId);
            
            if (killer == null || victim == null)
            {
                return false;
            }
            
            // Calculate points based on victim rank
            GuildData data = guildManager.GetGuildData();
            int points = victim.Rank switch
            {
                GuildRank.GuildMaster => data.WarPointsKillGuildMaster,
                GuildRank.ViceMaster => data.WarPointsKillViceMaster,
                _ => data.WarPointsKillMember
            };
            
            // Add points to killer's guild
            if (killerGuild.GuildId == war.Declaration.AttackingGuildId)
            {
                war.AttackingScore += points;
            }
            else
            {
                war.DefendingScore += points;
            }
            
            // Record kill
            GuildWarKill kill = new GuildWarKill
            {
                KillerId = killerId,
                KillerName = killer.PlayerName,
                KillerGuildId = killerGuild.GuildId,
                VictimId = victimId,
                VictimName = victim.PlayerName,
                VictimGuildId = victimGuild.GuildId,
                VictimRank = victim.Rank,
                PointsAwarded = points,
                Timestamp = DateTime.Now
            };
            
            war.KillLog.Add(kill);
            
            Debug.Log($"{killer.PlayerName} killed {victim.PlayerName} (+{points} points)");
            
            return true;
        }
        
        /// <summary>
        /// End guild war and determine winner
        /// Kết thúc Guild War và xác định người thắng
        /// </summary>
        public void EndWar(ActiveGuildWar war)
        {
            war.Declaration.Status = GuildWarStatus.Completed;
            
            Guild attackingGuild = guildManager.GetGuild(war.Declaration.AttackingGuildId);
            Guild defendingGuild = guildManager.GetGuild(war.Declaration.DefendingGuildId);
            
            if (attackingGuild == null || defendingGuild == null)
            {
                return;
            }
            
            string winnerGuildId = war.WinningGuildId;
            Guild winner = winnerGuildId == attackingGuild.GuildId ? attackingGuild : defendingGuild;
            Guild loser = winnerGuildId == attackingGuild.GuildId ? defendingGuild : attackingGuild;
            
            if (winnerGuildId != null)
            {
                winner.TotalWins++;
                loser.TotalLosses++;
                
                Debug.Log($"Guild War ended! Winner: {winner.GuildName}");
                Debug.Log($"Final Score: {attackingGuild.GuildName} {war.AttackingScore} - {war.DefendingScore} {defendingGuild.GuildName}");
            }
            else
            {
                Debug.Log("Guild War ended in a tie!");
            }
            
            // Record war in history
            GuildWarRecord attackingRecord = new GuildWarRecord
            {
                WarId = war.WarId,
                OpponentGuildId = defendingGuild.GuildId,
                OpponentGuildName = defendingGuild.GuildName,
                WarDate = war.StartTime,
                IsVictory = winnerGuildId == attackingGuild.GuildId,
                FinalScore = war.AttackingScore,
                OpponentScore = war.DefendingScore,
                Rewards = 0
            };
            attackingGuild.WarHistory.Add(attackingRecord);
            
            GuildWarRecord defendingRecord = new GuildWarRecord
            {
                WarId = war.WarId,
                OpponentGuildId = attackingGuild.GuildId,
                OpponentGuildName = attackingGuild.GuildName,
                WarDate = war.StartTime,
                IsVictory = winnerGuildId == defendingGuild.GuildId,
                FinalScore = war.DefendingScore,
                OpponentScore = war.AttackingScore,
                Rewards = 0
            };
            defendingGuild.WarHistory.Add(defendingRecord);
            
            OnWarEnded(war, winner, loser);
        }
        
        /// <summary>
        /// Get war duration based on type
        /// Lấy thời lượng chiến tranh dựa trên loại
        /// </summary>
        private int GetWarDuration(GuildWarType type)
        {
            return type switch
            {
                GuildWarType.Skirmish => 15,
                GuildWarType.Battle => 30,
                GuildWarType.FullWar => 60,
                GuildWarType.Territory => 90,
                _ => 30
            };
        }
        
        #region Events / Sự kiện
        
        private void OnWarDeclared(GuildWarDeclaration declaration)
        {
            // Notify defending guild
            // Send announcements
            // Update UI
        }
        
        private void OnWarStarted(ActiveGuildWar war)
        {
            // Notify all participants
            // Teleport to war zone
            // Start timers
        }
        
        private void OnWarEnded(ActiveGuildWar war, Guild winner, Guild loser)
        {
            // Distribute rewards
            // Send notifications
            // Update rankings
        }
        
        #endregion
    }
}
