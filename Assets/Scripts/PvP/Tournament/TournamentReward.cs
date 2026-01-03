using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Tournament Reward - Phần thưởng tournament
    /// </summary>
    [System.Serializable]
    public class TournamentRewardTier
    {
        public int placement;                     // 1st, 2nd, 3rd, etc.
        public PvPReward reward;
    }

    /// <summary>
    /// Tournament Reward System
    /// </summary>
    public class TournamentRewardSystem : MonoBehaviour
    {
        [Header("Reward Tiers")]
        public List<TournamentRewardTier> rewardTiers = new List<TournamentRewardTier>();
        
        /// <summary>
        /// Get reward for placement
        /// Lấy phần thưởng cho thứ hạng
        /// </summary>
        public PvPReward GetRewardForPlacement(int placement)
        {
            foreach (var tier in rewardTiers)
            {
                if (tier.placement == placement)
                {
                    return tier.reward;
                }
            }
            return null;
        }
        
        /// <summary>
        /// Distribute rewards to participants
        /// Phân phối phần thưởng cho người tham gia
        /// </summary>
        public void DistributeRewards(List<GameObject> participants, Dictionary<GameObject, int> placements)
        {
            foreach (var participant in participants)
            {
                if (placements.TryGetValue(participant, out int placement))
                {
                    var reward = GetRewardForPlacement(placement);
                    if (reward != null)
                    {
                        GiveReward(participant, reward);
                    }
                }
            }
        }
        
        /// <summary>
        /// Give reward to player
        /// Trao phần thưởng cho người chơi
        /// </summary>
        private void GiveReward(GameObject player, PvPReward reward)
        {
            // TODO: Integrate with player inventory/currency system
            Debug.Log($"{player.name} received tournament reward: {reward.zen} Zen");
        }
    }
}
