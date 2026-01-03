using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Guild
{
    /// <summary>
    /// Guild War reward system
    /// Hệ thống phần thưởng Guild War
    /// </summary>
    public class GuildWarReward : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GuildManager guildManager;
        [SerializeField] private GuildLevel guildLevel;
        
        [Header("Reward Configuration")]
        [SerializeField] private WarRewardConfig skirmishReward;
        [SerializeField] private WarRewardConfig battleReward;
        [SerializeField] private WarRewardConfig fullWarReward;
        [SerializeField] private WarRewardConfig territoryReward;
        
        /// <summary>
        /// War reward configuration
        /// Cấu hình phần thưởng chiến tranh
        /// </summary>
        [Serializable]
        public class WarRewardConfig
        {
            public int BaseGuildEXP;
            public int BaseGuildPoints;
            public int BaseZen;
            public float WinnerMultiplier = 2f;      // Winner gets 2x rewards
            public float LoserMultiplier = 0.5f;     // Loser gets 50% rewards
            public List<string> WinnerItemRewards;
        }
        
        /// <summary>
        /// Calculate and distribute rewards
        /// Tính toán và phân phối phần thưởng
        /// </summary>
        [Serializable]
        public class WarRewards
        {
            public int GuildEXP;
            public int GuildPoints;
            public int Zen;
            public List<string> ItemRewards;
            public int IndividualContribution;       // Per participant
        }
        
        private void Awake()
        {
            if (guildManager == null)
            {
                guildManager = GuildManager.Instance;
            }
            
            if (guildLevel == null)
            {
                guildLevel = FindObjectOfType<GuildLevel>();
            }
            
            InitializeDefaultRewards();
        }
        
        /// <summary>
        /// Initialize default reward configurations
        /// Khởi tạo cấu hình phần thưởng mặc định
        /// </summary>
        private void InitializeDefaultRewards()
        {
            if (skirmishReward == null)
            {
                skirmishReward = new WarRewardConfig
                {
                    BaseGuildEXP = 500,
                    BaseGuildPoints = 100,
                    BaseZen = 50000,
                    WinnerMultiplier = 2f,
                    LoserMultiplier = 0.5f,
                    WinnerItemRewards = new List<string>()
                };
            }
            
            if (battleReward == null)
            {
                battleReward = new WarRewardConfig
                {
                    BaseGuildEXP = 1500,
                    BaseGuildPoints = 300,
                    BaseZen = 150000,
                    WinnerMultiplier = 2f,
                    LoserMultiplier = 0.5f,
                    WinnerItemRewards = new List<string>()
                };
            }
            
            if (fullWarReward == null)
            {
                fullWarReward = new WarRewardConfig
                {
                    BaseGuildEXP = 5000,
                    BaseGuildPoints = 1000,
                    BaseZen = 500000,
                    WinnerMultiplier = 2f,
                    LoserMultiplier = 0.5f,
                    WinnerItemRewards = new List<string>()
                };
            }
            
            if (territoryReward == null)
            {
                territoryReward = new WarRewardConfig
                {
                    BaseGuildEXP = 10000,
                    BaseGuildPoints = 2000,
                    BaseZen = 1000000,
                    WinnerMultiplier = 2f,
                    LoserMultiplier = 0.5f,
                    WinnerItemRewards = new List<string>()
                };
            }
        }
        
        /// <summary>
        /// Calculate rewards for guild based on war result
        /// Tính phần thưởng cho guild dựa trên kết quả chiến tranh
        /// </summary>
        public WarRewards CalculateRewards(
            GuildWar.ActiveGuildWar war,
            string guildId,
            bool isWinner)
        {
            WarRewardConfig config = GetRewardConfig(war.Declaration.Type);
            float multiplier = isWinner ? config.WinnerMultiplier : config.LoserMultiplier;
            
            // Base rewards with multiplier
            WarRewards rewards = new WarRewards
            {
                GuildEXP = Mathf.RoundToInt(config.BaseGuildEXP * multiplier),
                GuildPoints = Mathf.RoundToInt(config.BaseGuildPoints * multiplier),
                Zen = Mathf.RoundToInt(config.BaseZen * multiplier),
                ItemRewards = isWinner ? new List<string>(config.WinnerItemRewards) : new List<string>(),
                IndividualContribution = Mathf.RoundToInt(50 * multiplier)
            };
            
            // Add bet rewards if there was a bet
            if (isWinner && war.Declaration.BetAmount > 0)
            {
                rewards.GuildPoints += war.Declaration.BetAmount * 2; // Winner takes all
            }
            
            // Bonus based on score difference
            int scoreDifference = isWinner ? 
                Mathf.Abs(war.AttackingScore - war.DefendingScore) : 0;
            
            if (scoreDifference > 0)
            {
                float bonusMultiplier = 1f + (scoreDifference * 0.01f); // 1% bonus per point difference
                rewards.GuildEXP = Mathf.RoundToInt(rewards.GuildEXP * bonusMultiplier);
                rewards.GuildPoints = Mathf.RoundToInt(rewards.GuildPoints * bonusMultiplier);
            }
            
            return rewards;
        }
        
        /// <summary>
        /// Distribute rewards to guild
        /// Phân phối phần thưởng cho guild
        /// </summary>
        public void DistributeRewards(
            GuildWar.ActiveGuildWar war,
            string guildId,
            bool isWinner)
        {
            Guild guild = guildManager.GetGuild(guildId);
            if (guild == null)
            {
                return;
            }
            
            WarRewards rewards = CalculateRewards(war, guildId, isWinner);
            
            // Add guild EXP
            if (guildLevel != null)
            {
                guildLevel.AddGuildExperience(guildId, rewards.GuildEXP, "Guild War");
            }
            
            // Add guild points
            guild.Points += rewards.GuildPoints;
            
            // Distribute individual contribution to participants
            List<string> participants = guildId == war.Declaration.AttackingGuildId ?
                war.AttackingParticipants : war.DefendingParticipants;
            
            foreach (string participantId in participants)
            {
                GuildMember member = guild.GetMember(participantId);
                if (member != null)
                {
                    member.AddContribution(rewards.IndividualContribution);
                    member.GuildWarsParticipated++;
                }
            }
            
            Debug.Log($"Guild '{guild.GuildName}' received war rewards:");
            Debug.Log($"- Guild EXP: {rewards.GuildEXP}");
            Debug.Log($"- Guild Points: {rewards.GuildPoints}");
            Debug.Log($"- Zen: {rewards.Zen}");
            Debug.Log($"- Individual Contribution: {rewards.IndividualContribution} per participant");
            
            // Give items to guild bank (would integrate with GuildBank here)
            // Award achievements
            // Send notifications
        }
        
        /// <summary>
        /// Get reward configuration for war type
        /// Lấy cấu hình phần thưởng cho loại chiến tranh
        /// </summary>
        private WarRewardConfig GetRewardConfig(GuildWar.GuildWarType type)
        {
            return type switch
            {
                GuildWar.GuildWarType.Skirmish => skirmishReward,
                GuildWar.GuildWarType.Battle => battleReward,
                GuildWar.GuildWarType.FullWar => fullWarReward,
                GuildWar.GuildWarType.Territory => territoryReward,
                _ => skirmishReward
            };
        }
        
        /// <summary>
        /// Calculate MVP rewards for top performers
        /// Tính phần thưởng MVP cho người thể hiện tốt nhất
        /// </summary>
        public Dictionary<string, int> CalculateMVPRewards(GuildWar.ActiveGuildWar war)
        {
            Dictionary<string, int> playerKills = new Dictionary<string, int>();
            
            // Count kills per player
            foreach (var kill in war.KillLog)
            {
                if (!playerKills.ContainsKey(kill.KillerId))
                {
                    playerKills[kill.KillerId] = 0;
                }
                playerKills[kill.KillerId]++;
            }
            
            // Get top 3 players
            var topPlayers = playerKills
                .OrderByDescending(kvp => kvp.Value)
                .Take(3)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value * 100); // 100 contribution per kill
            
            return topPlayers;
        }
    }
}
