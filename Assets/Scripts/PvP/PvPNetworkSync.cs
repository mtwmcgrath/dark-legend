using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PvP Network Sync - Đồng bộ mạng PvP
    /// Photon PUN 2 integration for multiplayer PvP
    /// </summary>
    public class PvPNetworkSync : MonoBehaviourPunCallbacks
    {
        private PvPManager pvpManager;
        private DuelSystem duelSystem;
        private ArenaManager arenaManager;
        private PKSystem pkSystem;
        
        private void Start()
        {
            pvpManager = PvPManager.Instance;
            if (pvpManager != null)
            {
                duelSystem = pvpManager.GetDuelSystem();
                arenaManager = pvpManager.GetArenaManager();
                
                var openWorldPvP = pvpManager.GetComponent<OpenWorldPvP>();
                if (openWorldPvP != null)
                {
                    pkSystem = openWorldPvP.GetPKSystem();
                }
            }
        }
        
        #region Duel RPCs
        
        /// <summary>
        /// Send duel request over network
        /// Gửi yêu cầu đấu qua mạng
        /// </summary>
        public void SendDuelRequest(int targetViewID, DuelSettings settings)
        {
            photonView.RPC("RPC_DuelRequest", RpcTarget.Others, targetViewID, 
                settings.timeLimit, settings.allowPotions, settings.allowSkills, 
                settings.betAmount, (int)settings.type);
        }
        
        [PunRPC]
        private void RPC_DuelRequest(int targetViewID, int timeLimit, bool allowPotions, 
            bool allowSkills, int betAmount, int duelTypeInt)
        {
            PhotonView targetView = PhotonView.Find(targetViewID);
            if (targetView == null) return;
            
            GameObject challenger = photonView.gameObject;
            GameObject target = targetView.gameObject;
            
            DuelSettings settings = new DuelSettings
            {
                timeLimit = timeLimit,
                allowPotions = allowPotions,
                allowSkills = allowSkills,
                betAmount = betAmount,
                type = (DuelType)duelTypeInt
            };
            
            if (duelSystem != null)
            {
                duelSystem.SendDuelRequest(challenger, target, settings);
            }
        }
        
        /// <summary>
        /// Accept duel over network
        /// Chấp nhận đấu qua mạng
        /// </summary>
        public void AcceptDuel(int challengerViewID)
        {
            photonView.RPC("RPC_DuelAccept", RpcTarget.All, challengerViewID, photonView.ViewID);
        }
        
        [PunRPC]
        private void RPC_DuelAccept(int challengerViewID, int targetViewID)
        {
            PhotonView challengerView = PhotonView.Find(challengerViewID);
            PhotonView targetView = PhotonView.Find(targetViewID);
            
            if (challengerView == null || targetView == null) return;
            
            if (duelSystem != null)
            {
                duelSystem.AcceptDuel(targetView.gameObject, challengerView.gameObject);
            }
        }
        
        /// <summary>
        /// Decline duel over network
        /// Từ chối đấu qua mạng
        /// </summary>
        public void DeclineDuel(int challengerViewID)
        {
            photonView.RPC("RPC_DuelDecline", RpcTarget.All, challengerViewID, photonView.ViewID);
        }
        
        [PunRPC]
        private void RPC_DuelDecline(int challengerViewID, int targetViewID)
        {
            PhotonView challengerView = PhotonView.Find(challengerViewID);
            PhotonView targetView = PhotonView.Find(targetViewID);
            
            if (challengerView == null || targetView == null) return;
            
            if (duelSystem != null)
            {
                duelSystem.DeclineDuel(targetView.gameObject, challengerView.gameObject);
            }
        }
        
        #endregion
        
        #region PvP Damage RPCs
        
        /// <summary>
        /// Send PvP damage over network
        /// Gửi sát thương PvP qua mạng
        /// </summary>
        public void SendPvPDamage(int targetViewID, int damage, int skillID)
        {
            photonView.RPC("RPC_PvPDamage", RpcTarget.All, photonView.ViewID, targetViewID, damage, skillID);
        }
        
        [PunRPC]
        private void RPC_PvPDamage(int attackerViewID, int targetViewID, int damage, int skillID)
        {
            PhotonView attackerView = PhotonView.Find(attackerViewID);
            PhotonView targetView = PhotonView.Find(targetViewID);
            
            if (attackerView == null || targetView == null) return;
            
            if (pvpManager != null)
            {
                pvpManager.ProcessPvPDamage(attackerView.gameObject, targetView.gameObject, damage);
            }
        }
        
        #endregion
        
        #region PK System RPCs
        
        /// <summary>
        /// Update PK status over network
        /// Cập nhật trạng thái PK qua mạng
        /// </summary>
        public void UpdatePKStatus(int pkStatusInt, int pkCount)
        {
            photonView.RPC("RPC_UpdatePKStatus", RpcTarget.All, photonView.ViewID, pkStatusInt, pkCount);
        }
        
        [PunRPC]
        private void RPC_UpdatePKStatus(int viewID, int pkStatusInt, int pkCount)
        {
            PhotonView view = PhotonView.Find(viewID);
            if (view == null) return;
            
            if (pkSystem != null)
            {
                string playerId = view.gameObject.GetInstanceID().ToString();
                var pkData = pkSystem.GetPKData(playerId);
                pkData.pkCount = pkCount;
                pkData.status = (PKStatus)pkStatusInt;
            }
        }
        
        /// <summary>
        /// Broadcast PvP kill over network
        /// Phát sóng kill PvP qua mạng
        /// </summary>
        public void BroadcastPvPKill(int victimViewID)
        {
            photonView.RPC("RPC_PvPKill", RpcTarget.All, photonView.ViewID, victimViewID);
        }
        
        [PunRPC]
        private void RPC_PvPKill(int killerViewID, int victimViewID)
        {
            PhotonView killerView = PhotonView.Find(killerViewID);
            PhotonView victimView = PhotonView.Find(victimViewID);
            
            if (killerView == null || victimView == null) return;
            
            if (pvpManager != null)
            {
                pvpManager.ProcessPvPKill(killerView.gameObject, victimView.gameObject);
            }
        }
        
        #endregion
        
        #region Arena RPCs
        
        /// <summary>
        /// Notify players that arena match is found
        /// Thông báo người chơi đã tìm thấy trận đấu
        /// </summary>
        public void NotifyArenaMatchFound(int[] team1ViewIDs, int[] team2ViewIDs, int modeInt)
        {
            photonView.RPC("RPC_ArenaMatchFound", RpcTarget.All, team1ViewIDs, team2ViewIDs, modeInt);
        }
        
        [PunRPC]
        private void RPC_ArenaMatchFound(int[] team1ViewIDs, int[] team2ViewIDs, int modeInt)
        {
            List<GameObject> team1 = new List<GameObject>();
            List<GameObject> team2 = new List<GameObject>();
            
            foreach (int viewID in team1ViewIDs)
            {
                PhotonView view = PhotonView.Find(viewID);
                if (view != null) team1.Add(view.gameObject);
            }
            
            foreach (int viewID in team2ViewIDs)
            {
                PhotonView view = PhotonView.Find(viewID);
                if (view != null) team2.Add(view.gameObject);
            }
            
            // Create arena match
            ArenaMode mode = (ArenaMode)modeInt;
            ArenaMatch match = new ArenaMatch(mode);
            match.team1 = team1;
            match.team2 = team2;
            match.StartMatch();
            
            Debug.Log($"Arena match started via network: {mode}");
        }
        
        /// <summary>
        /// Update arena score over network
        /// Cập nhật điểm đấu trường qua mạng
        /// </summary>
        public void UpdateArenaScore(string matchId, int team1Score, int team2Score)
        {
            photonView.RPC("RPC_ArenaScoreUpdate", RpcTarget.All, matchId, team1Score, team2Score);
        }
        
        [PunRPC]
        private void RPC_ArenaScoreUpdate(string matchId, int team1Score, int team2Score)
        {
            // TODO: Update arena match score
            Debug.Log($"Arena match {matchId} score: {team1Score} - {team2Score}");
        }
        
        #endregion
        
        #region Battleground RPCs
        
        /// <summary>
        /// Sync battleground objective state
        /// Đồng bộ trạng thái mục tiêu battleground
        /// </summary>
        public void SyncObjectiveState(string objectiveId, int state, int controllingTeam)
        {
            photonView.RPC("RPC_ObjectiveStateSync", RpcTarget.All, objectiveId, state, controllingTeam);
        }
        
        [PunRPC]
        private void RPC_ObjectiveStateSync(string objectiveId, int state, int controllingTeam)
        {
            // TODO: Update battleground objective state
            Debug.Log($"Objective {objectiveId} state: {state}, controlled by team {controllingTeam}");
        }
        
        /// <summary>
        /// Sync flag carrier in CTF
        /// Đồng bộ người mang cờ trong CTF
        /// </summary>
        public void SyncFlagCarrier(int flagTeam, int carrierViewID)
        {
            photonView.RPC("RPC_FlagCarrierSync", RpcTarget.All, flagTeam, carrierViewID);
        }
        
        [PunRPC]
        private void RPC_FlagCarrierSync(int flagTeam, int carrierViewID)
        {
            PhotonView carrierView = carrierViewID > 0 ? PhotonView.Find(carrierViewID) : null;
            
            // TODO: Update CTF flag carrier
            if (carrierView != null)
            {
                Debug.Log($"Team {flagTeam} flag picked up by {carrierView.gameObject.name}");
            }
            else
            {
                Debug.Log($"Team {flagTeam} flag dropped");
            }
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Get PhotonView ID of GameObject
        /// Lấy ID PhotonView của GameObject
        /// </summary>
        public static int GetViewID(GameObject obj)
        {
            PhotonView view = obj.GetComponent<PhotonView>();
            return view != null ? view.ViewID : -1;
        }
        
        /// <summary>
        /// Check if local player owns this object
        /// Kiểm tra người chơi local có sở hữu object này không
        /// </summary>
        public bool IsMine()
        {
            return photonView.IsMine;
        }
        
        #endregion
    }
}
