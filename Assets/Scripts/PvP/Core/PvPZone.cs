using UnityEngine;
using System;

namespace DarkLegend.PvP
{
    /// <summary>
    /// PvP Zone definition - Vùng PvP
    /// </summary>
    public class PvPZone : MonoBehaviour
    {
        [Header("Zone Settings")]
        public string zoneName;
        public PvPRules rules;
        
        [Header("Zone Boundaries")]
        public Collider zoneCollider;
        
        // Events
        public event Action<GameObject> OnPlayerEnter;
        public event Action<GameObject> OnPlayerExit;
        
        private void Awake()
        {
            if (zoneCollider == null)
            {
                zoneCollider = GetComponent<Collider>();
            }
            
            if (zoneCollider != null)
            {
                zoneCollider.isTrigger = true;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerEnter?.Invoke(other.gameObject);
                NotifyPlayerEntered(other.gameObject);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPlayerExit?.Invoke(other.gameObject);
                NotifyPlayerExited(other.gameObject);
            }
        }
        
        private void NotifyPlayerEntered(GameObject player)
        {
            // TODO: Integrate with player controller to apply zone rules
            Debug.Log($"Player entered PvP zone: {zoneName}");
        }
        
        private void NotifyPlayerExited(GameObject player)
        {
            // TODO: Integrate with player controller to remove zone rules
            Debug.Log($"Player exited PvP zone: {zoneName}");
        }
    }
}
