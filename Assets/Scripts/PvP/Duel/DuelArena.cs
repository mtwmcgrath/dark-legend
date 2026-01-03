using UnityEngine;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Duel Arena - Arena cho đấu tay đôi
    /// Physical space where duels take place
    /// </summary>
    public class DuelArena : MonoBehaviour
    {
        [Header("Arena Settings")]
        public string arenaName = "Duel Arena";
        public Transform[] spawnPoints = new Transform[2];
        public Collider arenaBounds;
        
        [Header("Visual Effects")]
        public GameObject arenaBarrierEffect;
        public GameObject arenaFloorEffect;
        
        private bool isOccupied = false;
        
        private void Awake()
        {
            if (arenaBounds == null)
            {
                arenaBounds = GetComponent<Collider>();
            }
        }
        
        /// <summary>
        /// Check if arena is available
        /// Kiểm tra arena có trống không
        /// </summary>
        public bool IsAvailable()
        {
            return !isOccupied;
        }
        
        /// <summary>
        /// Occupy arena for a duel
        /// Chiếm arena cho trận đấu
        /// </summary>
        public void Occupy()
        {
            isOccupied = true;
            ActivateArena();
        }
        
        /// <summary>
        /// Release arena after duel
        /// Giải phóng arena sau đấu
        /// </summary>
        public void Release()
        {
            isOccupied = false;
            DeactivateArena();
        }
        
        /// <summary>
        /// Get spawn point for player
        /// Lấy vị trí spawn cho người chơi
        /// </summary>
        public Vector3 GetSpawnPoint(int playerIndex)
        {
            if (spawnPoints.Length > playerIndex && spawnPoints[playerIndex] != null)
            {
                return spawnPoints[playerIndex].position;
            }
            return transform.position;
        }
        
        private void ActivateArena()
        {
            if (arenaBarrierEffect != null)
            {
                arenaBarrierEffect.SetActive(true);
            }
            if (arenaFloorEffect != null)
            {
                arenaFloorEffect.SetActive(true);
            }
        }
        
        private void DeactivateArena()
        {
            if (arenaBarrierEffect != null)
            {
                arenaBarrierEffect.SetActive(false);
            }
            if (arenaFloorEffect != null)
            {
                arenaFloorEffect.SetActive(false);
            }
        }
    }
}
