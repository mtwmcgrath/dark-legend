using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Castle Siege event - Weekly guild war for castle control
    /// Công thành chiến - Chiến tranh guild hàng tuần để kiểm soát thành
    /// </summary>
    public class CastleSiege : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        
        [Header("Settings")]
        [SerializeField] private int siegeDurationHours = 2;
        [SerializeField] private float taxRate = 0.1f; // 10% tax
        [SerializeField] private DayOfWeek siegeDay = DayOfWeek.Saturday;
        [SerializeField] private int siegeHour = 20; // 8 PM
        
        // Current castle owner / Chủ thành hiện tại
        private string currentOwnerId;
        
        // Active siege / Công thành đang diễn ra
        private ActiveCastleSiege activeSiege;
        
        /// <summary>
        /// Active castle siege session
        /// Phiên công thành đang diễn ra
        /// </summary>
        [Serializable]
        public class ActiveCastleSiege
        {
            public string SiegeId;
            public DateTime StartTime;
            public DateTime EndTime;
            public string DefendingGuildId;
            public List<AttackingGuild> Attackers;
            public CastleObjectives Objectives;
            public SiegeStatus Status;
            
            public bool IsActive => DateTime.Now < EndTime && Status == SiegeStatus.InProgress;
        }
        
        /// <summary>
        /// Attacking guild in siege
        /// Guild tấn công trong công thành
        /// </summary>
        [Serializable]
        public class AttackingGuild
        {
            public string GuildId;
            public string GuildName;
            public int Score;
            public List<string> Participants;
            public bool HasReachedThrone;
        }
        
        /// <summary>
        /// Castle siege objectives
        /// Mục tiêu công thành
        /// </summary>
        [Serializable]
        public class CastleObjectives
        {
            public bool OuterGateDestroyed;
            public bool InnerGateDestroyed;
            public bool ThroneCaptured;
            public int DefenderKills;
            public int AttackerKills;
        }
        
        /// <summary>
        /// Siege status
        /// Trạng thái công thành
        /// </summary>
        public enum SiegeStatus
        {
            Scheduled,
            InProgress,
            Completed,
            Cancelled
        }
        
        /// <summary>
        /// Castle benefits for owner
        /// Lợi ích thành cho chủ sở hữu
        /// </summary>
        [Serializable]
        public class CastleBenefits
        {
            public float TaxRevenue;
            public List<string> SpecialBuffs;
            public List<string> ExclusiveItems;
            public string Title; // "Lords of [Castle Name]"
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
        }
        
        private void Update()
        {
            CheckSiegeSchedule();
            CheckActiveSiege();
        }
        
        /// <summary>
        /// Register guild for castle siege
        /// Đăng ký guild cho công thành
        /// </summary>
        public bool RegisterForSiege(string guildId, string requesterId)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                Debug.LogError("Guild not found.");
                return false;
            }
            
            GuildMember requester = guild.GetMember(requesterId);
            if (requester == null || !requester.GetPermissions().CanDeclareWar)
            {
                Debug.LogError("You don't have permission to register for castle siege.");
                return false;
            }
            
            // Check guild level requirement (e.g., level 20+)
            if (guild.Level < 20)
            {
                Debug.LogError("Guild must be at least level 20 to participate in Castle Siege.");
                return false;
            }
            
            if (activeSiege == null)
            {
                Debug.LogError("No castle siege scheduled.");
                return false;
            }
            
            // Check if already registered
            if (activeSiege.Attackers.Exists(a => a.GuildId == guildId))
            {
                Debug.LogError("Guild is already registered.");
                return false;
            }
            
            // Add as attacker
            AttackingGuild attacker = new AttackingGuild
            {
                GuildId = guildId,
                GuildName = guild.GuildName,
                Score = 0,
                Participants = new List<string>(),
                HasReachedThrone = false
            };
            
            activeSiege.Attackers.Add(attacker);
            
            Debug.Log($"Guild '{guild.GuildName}' registered for Castle Siege!");
            
            return true;
        }
        
        /// <summary>
        /// Start castle siege
        /// Bắt đầu công thành
        /// </summary>
        public bool StartSiege()
        {
            if (string.IsNullOrEmpty(currentOwnerId))
            {
                Debug.LogError("No castle owner to defend against.");
                return false;
            }
            
            activeSiege = new ActiveCastleSiege
            {
                SiegeId = Guid.NewGuid().ToString(),
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddHours(siegeDurationHours),
                DefendingGuildId = currentOwnerId,
                Attackers = new List<AttackingGuild>(),
                Objectives = new CastleObjectives(),
                Status = SiegeStatus.InProgress
            };
            
            Debug.Log($"Castle Siege started! Defenders: {guildManager.GetGuild(currentOwnerId)?.GuildName}");
            OnSiegeStarted();
            
            return true;
        }
        
        /// <summary>
        /// Update siege objective
        /// Cập nhật mục tiêu công thành
        /// </summary>
        public void UpdateObjective(string objectiveName, bool completed)
        {
            if (activeSiege == null || !activeSiege.IsActive)
            {
                return;
            }
            
            switch (objectiveName)
            {
                case "OuterGate":
                    activeSiege.Objectives.OuterGateDestroyed = completed;
                    Debug.Log("Outer gate destroyed!");
                    break;
                case "InnerGate":
                    activeSiege.Objectives.InnerGateDestroyed = completed;
                    Debug.Log("Inner gate destroyed!");
                    break;
                case "Throne":
                    activeSiege.Objectives.ThroneCaptured = completed;
                    Debug.Log("Throne captured!");
                    // End siege if throne is captured
                    if (completed)
                    {
                        EndSiege();
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Register kill in castle siege
        /// Đăng ký giết trong công thành
        /// </summary>
        public void RegisterSiegeKill(bool isDefender)
        {
            if (activeSiege == null || !activeSiege.IsActive)
            {
                return;
            }
            
            if (isDefender)
            {
                activeSiege.Objectives.DefenderKills++;
            }
            else
            {
                activeSiege.Objectives.AttackerKills++;
            }
        }
        
        /// <summary>
        /// End castle siege and determine winner
        /// Kết thúc công thành và xác định người thắng
        /// </summary>
        public void EndSiege()
        {
            if (activeSiege == null)
            {
                return;
            }
            
            activeSiege.Status = SiegeStatus.Completed;
            
            // Determine winner
            string winnerId = currentOwnerId; // Default to defender
            
            if (activeSiege.Objectives.ThroneCaptured)
            {
                // Find guild that captured throne
                AttackingGuild winner = activeSiege.Attackers
                    .OrderByDescending(a => a.Score)
                    .FirstOrDefault();
                
                if (winner != null)
                {
                    winnerId = winner.GuildId;
                    TransferCastleOwnership(winnerId);
                }
            }
            
            Guild winningGuild = guildManager.GetGuild(winnerId);
            Debug.Log($"Castle Siege ended! Winner: {winningGuild?.GuildName}");
            
            // Award rewards
            AwardSiegeRewards(winnerId);
            
            // Update stats
            if (winningGuild != null)
            {
                winningGuild.CastleSiegeWins++;
            }
            
            OnSiegeEnded(winningGuild);
            
            activeSiege = null;
        }
        
        /// <summary>
        /// Transfer castle ownership
        /// Chuyển quyền sở hữu thành
        /// </summary>
        private void TransferCastleOwnership(string newOwnerId)
        {
            currentOwnerId = newOwnerId;
            Guild newOwner = guildManager.GetGuild(newOwnerId);
            
            if (newOwner != null)
            {
                Debug.Log($"Castle ownership transferred to {newOwner.GuildName}!");
            }
        }
        
        /// <summary>
        /// Award castle siege rewards
        /// Trao phần thưởng công thành
        /// </summary>
        private void AwardSiegeRewards(string winnerId)
        {
            Guild winner = guildManager.GetGuild(winnerId);
            if (winner == null)
            {
                return;
            }
            
            // Award massive rewards
            GuildData data = guildManager.GetGuildData();
            winner.AddExperience(50000, data); // 50k guild EXP
            winner.Points += 10000; // 10k guild points
            
            Debug.Log($"Castle Siege rewards awarded to {winner.GuildName}:");
            Debug.Log("- 50,000 Guild EXP");
            Debug.Log("- 10,000 Guild Points");
            Debug.Log("- Castle ownership and tax benefits");
        }
        
        /// <summary>
        /// Get castle benefits for owner
        /// Lấy lợi ích thành cho chủ sở hữu
        /// </summary>
        public CastleBenefits GetCastleBenefits()
        {
            if (string.IsNullOrEmpty(currentOwnerId))
            {
                return null;
            }
            
            return new CastleBenefits
            {
                TaxRevenue = taxRate,
                SpecialBuffs = new List<string> { "Castle Lord Buff", "Territory Buff" },
                ExclusiveItems = new List<string> { "Castle Crown", "Lord's Sword" },
                Title = "Lords of Dark Castle"
            };
        }
        
        /// <summary>
        /// Get current castle owner
        /// Lấy chủ thành hiện tại
        /// </summary>
        public Guild GetCurrentOwner()
        {
            if (string.IsNullOrEmpty(currentOwnerId))
            {
                return null;
            }
            
            return guildManager.GetGuild(currentOwnerId);
        }
        
        /// <summary>
        /// Check if it's time to schedule siege
        /// Kiểm tra có phải thời gian lên lịch công thành không
        /// </summary>
        private void CheckSiegeSchedule()
        {
            DateTime now = DateTime.Now;
            
            if (now.DayOfWeek == siegeDay && now.Hour == siegeHour && activeSiege == null)
            {
                StartSiege();
            }
        }
        
        /// <summary>
        /// Check active siege status
        /// Kiểm tra trạng thái công thành đang diễn ra
        /// </summary>
        private void CheckActiveSiege()
        {
            if (activeSiege != null && !activeSiege.IsActive)
            {
                EndSiege();
            }
        }
        
        #region Events / Sự kiện
        
        private void OnSiegeStarted()
        {
            // Notify all guilds
            // Teleport participants
            // Start timers
        }
        
        private void OnSiegeEnded(Guild winner)
        {
            // Distribute rewards
            // Send notifications
            // Update rankings
        }
        
        #endregion
    }
}
