using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// ELO Rating System - Hệ thống xếp hạng ELO
    /// </summary>
    [Serializable]
    public class EloRating
    {
        public int rating = 1200;             // Default starting rating
        public int wins = 0;
        public int losses = 0;
        public int streak = 0;                // Win/Loss streak (positive = wins, negative = losses)
        
        /// <summary>
        /// K-factor determines how much rating changes per match
        /// K-factor xác định rating thay đổi bao nhiêu mỗi trận
        /// </summary>
        public int KFactor
        {
            get
            {
                if (rating < 1600) return 40;     // New players
                if (rating < 2000) return 32;     // Mid-level
                if (rating < 2400) return 24;     // High-level
                return 16;                         // Top players
            }
        }
        
        /// <summary>
        /// Calculate new ratings after a match
        /// Tính toán rating mới sau trận đấu
        /// </summary>
        public static (int, int) CalculateNewRatings(EloRating playerA, EloRating playerB, float result)
        {
            // result: 1.0 = A wins, 0.0 = B wins, 0.5 = draw
            float expectedA = 1f / (1f + Mathf.Pow(10f, (playerB.rating - playerA.rating) / 400f));
            float expectedB = 1f - expectedA;
            
            int newRatingA = playerA.rating + Mathf.RoundToInt(playerA.KFactor * (result - expectedA));
            int newRatingB = playerB.rating + Mathf.RoundToInt(playerB.KFactor * ((1f - result) - expectedB));
            
            return (newRatingA, newRatingB);
        }
        
        /// <summary>
        /// Get rank tier based on rating
        /// Lấy hạng dựa trên rating
        /// </summary>
        public RankTier GetRankTier()
        {
            if (rating >= 2900) return RankTier.Challenger;
            if (rating >= 2700) return RankTier.GrandMaster;
            if (rating >= 2500) return RankTier.Master;
            if (rating >= 2400) return RankTier.Diamond_I;
            if (rating >= 2300) return RankTier.Diamond_II;
            if (rating >= 2200) return RankTier.Diamond_III;
            if (rating >= 2100) return RankTier.Platinum_I;
            if (rating >= 2000) return RankTier.Platinum_II;
            if (rating >= 1900) return RankTier.Platinum_III;
            if (rating >= 1800) return RankTier.Gold_I;
            if (rating >= 1700) return RankTier.Gold_II;
            if (rating >= 1600) return RankTier.Gold_III;
            if (rating >= 1500) return RankTier.Silver_I;
            if (rating >= 1400) return RankTier.Silver_II;
            if (rating >= 1300) return RankTier.Silver_III;
            if (rating >= 1200) return RankTier.Bronze_I;
            if (rating >= 1100) return RankTier.Bronze_II;
            return RankTier.Bronze_III;
        }
        
        /// <summary>
        /// Update rating after a match
        /// Cập nhật rating sau trận
        /// </summary>
        public void UpdateAfterMatch(bool won, int ratingChange)
        {
            rating += ratingChange;
            
            if (won)
            {
                wins++;
                streak = streak > 0 ? streak + 1 : 1;
            }
            else
            {
                losses++;
                streak = streak < 0 ? streak - 1 : -1;
            }
        }
        
        /// <summary>
        /// Get win rate percentage
        /// Lấy tỷ lệ thắng
        /// </summary>
        public float GetWinRate()
        {
            int totalMatches = wins + losses;
            if (totalMatches == 0) return 0f;
            return (float)wins / totalMatches * 100f;
        }
    }
}
