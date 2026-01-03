using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Capture The Flag Mode - Chế độ cướp cờ
    /// </summary>
    public class CaptureTheFlag : BattlegroundMode
    {
        [Header("CTF Settings")]
        public int capturesToWin = 3;
        public float flagCarrierSpeedReduction = 0.3f;  // 30% slower
        public float flagReturnTime = 30f;              // Auto return after 30s
        public int pointsPerCapture = 1;
        public int pointsPerReturn = 0;                 // Personal score
        public int pointsPerFlagKill = 0;               // Personal score
        
        [Header("Flag Bases")]
        public Transform team1FlagBase;
        public Transform team2FlagBase;
        
        [Header("Flag Objects")]
        public GameObject team1Flag;
        public GameObject team2Flag;
        
        // Flag states
        private GameObject team1FlagCarrier;
        private GameObject team2FlagCarrier;
        private float team1FlagDropTime;
        private float team2FlagDropTime;
        private bool team1FlagAtBase = true;
        private bool team2FlagAtBase = true;
        
        private void Awake()
        {
            modeName = "Capture The Flag";
            teamSize = 8;
            timeLimit = 1200; // 20 minutes
        }
        
        public override void StartMatch()
        {
            base.StartMatch();
            
            // Reset flags
            ResetFlag(1);
            ResetFlag(2);
        }
        
        /// <summary>
        /// Player picks up flag
        /// Người chơi nhặt cờ
        /// </summary>
        public void PickupFlag(GameObject player, int flagTeam)
        {
            if (state != MatchState.InProgress) return;
            
            int playerTeam = team1.Contains(player) ? 1 : 2;
            
            // Can't pick up own flag
            if (playerTeam == flagTeam) return;
            
            // Set carrier
            if (flagTeam == 1)
            {
                team1FlagCarrier = player;
                team1FlagAtBase = false;
                Debug.Log($"{player.name} picked up Team 1's flag!");
            }
            else
            {
                team2FlagCarrier = player;
                team2FlagAtBase = false;
                Debug.Log($"{player.name} picked up Team 2's flag!");
            }
            
            // TODO: Apply speed reduction to carrier
            // TODO: Attach flag to player model
        }
        
        /// <summary>
        /// Player drops flag
        /// Người chơi thả cờ
        /// </summary>
        public void DropFlag(GameObject player, int flagTeam)
        {
            if (flagTeam == 1 && team1FlagCarrier == player)
            {
                team1FlagCarrier = null;
                team1FlagDropTime = Time.time;
                // TODO: Drop flag at player position
                Debug.Log($"{player.name} dropped Team 1's flag!");
            }
            else if (flagTeam == 2 && team2FlagCarrier == player)
            {
                team2FlagCarrier = null;
                team2FlagDropTime = Time.time;
                // TODO: Drop flag at player position
                Debug.Log($"{player.name} dropped Team 2's flag!");
            }
        }
        
        /// <summary>
        /// Attempt to capture flag
        /// Thử bắt cờ
        /// </summary>
        public void AttemptCapture(GameObject player)
        {
            if (state != MatchState.InProgress) return;
            
            int playerTeam = team1.Contains(player) ? 1 : 2;
            
            // Check if player is carrying enemy flag and at own base
            if (playerTeam == 1)
            {
                // Team 1 trying to capture
                if (team2FlagCarrier == player && team1FlagAtBase)
                {
                    CaptureFlag(1);
                    team2FlagCarrier = null;
                    ResetFlag(2);
                }
            }
            else
            {
                // Team 2 trying to capture
                if (team1FlagCarrier == player && team2FlagAtBase)
                {
                    CaptureFlag(2);
                    team1FlagCarrier = null;
                    ResetFlag(1);
                }
            }
        }
        
        /// <summary>
        /// Capture flag for team
        /// Bắt cờ cho đội
        /// </summary>
        private void CaptureFlag(int capturingTeam)
        {
            UpdateScore(capturingTeam, pointsPerCapture);
            Debug.Log($"Team {capturingTeam} captured the flag! Score: {team1Score}-{team2Score}");
        }
        
        /// <summary>
        /// Return flag to base
        /// Trả cờ về base
        /// </summary>
        public void ReturnFlag(GameObject player, int flagTeam)
        {
            int playerTeam = team1.Contains(player) ? 1 : 2;
            
            // Can only return own flag
            if (playerTeam != flagTeam) return;
            
            ResetFlag(flagTeam);
            Debug.Log($"{player.name} returned the flag!");
        }
        
        /// <summary>
        /// Reset flag to base
        /// Đặt lại cờ về base
        /// </summary>
        private void ResetFlag(int flagTeam)
        {
            if (flagTeam == 1)
            {
                team1FlagCarrier = null;
                team1FlagAtBase = true;
                if (team1Flag != null && team1FlagBase != null)
                {
                    team1Flag.transform.position = team1FlagBase.position;
                }
            }
            else
            {
                team2FlagCarrier = null;
                team2FlagAtBase = true;
                if (team2Flag != null && team2FlagBase != null)
                {
                    team2Flag.transform.position = team2FlagBase.position;
                }
            }
        }
        
        protected override bool CheckWinCondition()
        {
            return team1Score >= capturesToWin || team2Score >= capturesToWin;
        }
        
        protected override void Update()
        {
            base.Update();
            
            // Auto-return dropped flags
            if (!team1FlagAtBase && team1FlagCarrier == null)
            {
                if (Time.time - team1FlagDropTime >= flagReturnTime)
                {
                    ResetFlag(1);
                    Debug.Log("Team 1's flag auto-returned");
                }
            }
            
            if (!team2FlagAtBase && team2FlagCarrier == null)
            {
                if (Time.time - team2FlagDropTime >= flagReturnTime)
                {
                    ResetFlag(2);
                    Debug.Log("Team 2's flag auto-returned");
                }
            }
        }
    }
}
