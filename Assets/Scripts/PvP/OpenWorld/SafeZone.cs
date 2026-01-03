using UnityEngine;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Safe Zone - Vùng an toàn (không PvP)
    /// </summary>
    public class SafeZone : MonoBehaviour
    {
        [Header("Safe Zone Settings")]
        public string zoneName = "Safe Zone";
        public bool blockAllPvP = true;
        public bool healPlayers = false;
        public float healRate = 5f; // HP per second
        
        private Collider zoneCollider;
        
        private void Awake()
        {
            zoneCollider = GetComponent<Collider>();
            if (zoneCollider != null)
            {
                zoneCollider.isTrigger = true;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerEnterSafeZone(other.gameObject);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerExitSafeZone(other.gameObject);
            }
        }
        
        private void OnPlayerEnterSafeZone(GameObject player)
        {
            // TODO: Disable PvP for player
            // TODO: Apply safe zone buffs
            Debug.Log($"{player.name} entered safe zone: {zoneName}");
        }
        
        private void OnPlayerExitSafeZone(GameObject player)
        {
            // TODO: Re-enable PvP for player
            // TODO: Remove safe zone buffs
            Debug.Log($"{player.name} exited safe zone: {zoneName}");
        }
    }
}
