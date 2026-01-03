using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Arena Reward - Phần thưởng đấu trường
    /// </summary>
    [System.Serializable]
    public class ArenaReward
    {
        public RankTier minRank;
        public RankTier maxRank;
        public PvPReward reward;
        
        public bool IsEligible(RankTier playerRank)
        {
            return playerRank >= minRank && playerRank <= maxRank;
        }
    }

    /// <summary>
    /// Arena Reward System - Hệ thống phần thưởng đấu trường
    /// </summary>
    public class ArenaRewardSystem : MonoBehaviour
    {
        [Header("Match Rewards")]
        public int winRewardZen = 1000;
        public int lossRewardZen = 100;
        public int winRewardArenaPoints = 10;
        public int lossRewardArenaPoints = 3;
        
        [Header("Season Rewards")]
        public List<ArenaReward> seasonRewards = new List<ArenaReward>();
        
        /// <summary>
        /// Give match rewards to player
        /// Trao phần thưởng trận đấu
        /// </summary>
        public PvPReward GetMatchReward(bool won, int ratingGain, int killCount)
        {
            PvPReward reward = new PvPReward();
            
            if (won)
            {
                reward.zen = winRewardZen + (ratingGain * 10);
                reward.arenaPoints = winRewardArenaPoints + (killCount * 2);
            }
            else
            {
                reward.zen = lossRewardZen;
                reward.arenaPoints = lossRewardArenaPoints + killCount;
            }
            
            return reward;
        }
        
        /// <summary>
        /// Get season end rewards
        /// Lấy phần thưởng cuối mùa
        /// </summary>
        public PvPReward GetSeasonReward(RankTier playerRank)
        {
            foreach (var arenaReward in seasonRewards)
            {
                if (arenaReward.IsEligible(playerRank))
                {
                    return arenaReward.reward;
                }
            }
            return null;
        }
    }
}
