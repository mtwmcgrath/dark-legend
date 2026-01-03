using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Tournament Match - Trận đấu tournament
    /// </summary>
    [Serializable]
    public class TournamentMatch
    {
        public string matchId;
        public int roundNumber;
        public int matchNumber;
        public List<GameObject> team1;
        public List<GameObject> team2;
        public int winnerId;                      // 1 or 2
        public bool isComplete = false;
        public DateTime scheduledTime;
        
        public TournamentMatch(int round, int matchNum)
        {
            this.matchId = Guid.NewGuid().ToString();
            this.roundNumber = round;
            this.matchNumber = matchNum;
            this.team1 = new List<GameObject>();
            this.team2 = new List<GameObject>();
        }
    }
}
