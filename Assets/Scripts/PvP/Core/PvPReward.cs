using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PvP reward system - Hệ thống phần thưởng PvP
    /// </summary>
    [Serializable]
    public class PvPReward
    {
        public int zen;
        public int experience;
        public int arenaPoints;
        public List<RewardItem> items = new List<RewardItem>();
        public string title;
        
        [Serializable]
        public class RewardItem
        {
            public string itemId;
            public int quantity;
            public float dropChance; // 0.0 to 1.0
        }
    }

    /// <summary>
    /// Season reward - Phần thưởng mùa giải
    /// </summary>
    [Serializable]
    public class SeasonReward
    {
        public RankTier requiredRank;
        public int zen;
        public List<PvPReward.RewardItem> items = new List<PvPReward.RewardItem>();
        public string title;
        public string mountSkinId;
        public string auraSkinId;
        public int arenaPoints;
    }
}
