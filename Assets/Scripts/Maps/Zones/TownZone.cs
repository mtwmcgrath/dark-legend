using System.Collections.Generic;
using UnityEngine;

namespace DarkLegend.Maps.Zones
{
    /// <summary>
    /// Town zone - Thành phố, safe zone với NPCs
    /// Town zone - Safe area with NPCs and shops
    /// </summary>
    public class TownZone : ZoneBase
    {
        [Header("Town Features")]
        [Tooltip("Danh sách NPCs / NPC list")]
        [SerializeField] private List<GameObject> npcs = new List<GameObject>();
        
        [Tooltip("Vị trí fountain / Fountain position")]
        [SerializeField] private Vector3 fountainPosition;
        
        [Tooltip("Bán kính hồi phục / Recovery radius")]
        [SerializeField] private float recoveryRadius = 5f;
        
        [Tooltip("Tốc độ hồi HP/MP / HP/MP regen rate")]
        [SerializeField] private float regenRate = 10f;
        
        [Header("Town Services")]
        [Tooltip("Có shop không? / Has shop?")]
        [SerializeField] private bool hasShop = true;
        
        [Tooltip("Có storage không? / Has storage?")]
        [SerializeField] private bool hasStorage = true;
        
        [Tooltip("Có guild NPC không? / Has guild NPC?")]
        [SerializeField] private bool hasGuildNPC = true;
        
        [Tooltip("Có teleporter không? / Has teleporter?")]
        [SerializeField] private bool hasTeleporter = true;
        
        private List<GameObject> spawnedNPCs = new List<GameObject>();
        
        public override void InitializeZone()
        {
            base.InitializeZone();
            
            // Spawn NPCs
            SpawnTownNPCs();
            
            // Setup fountain
            SetupFountain();
            
            Debug.Log($"[TownZone] Town initialized: {zoneName}");
        }
        
        public override void CleanupZone()
        {
            base.CleanupZone();
            
            // Cleanup NPCs
            foreach (var npc in spawnedNPCs)
            {
                if (npc != null)
                {
                    Destroy(npc);
                }
            }
            spawnedNPCs.Clear();
        }
        
        /// <summary>
        /// Spawn NPCs trong town / Spawn town NPCs
        /// </summary>
        private void SpawnTownNPCs()
        {
            foreach (var npcPrefab in npcs)
            {
                if (npcPrefab != null)
                {
                    Vector3 spawnPos = GetRandomPositionInZone();
                    GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity, transform);
                    spawnedNPCs.Add(npc);
                }
            }
            
            Debug.Log($"[TownZone] Spawned {spawnedNPCs.Count} NPCs");
        }
        
        /// <summary>
        /// Setup fountain hồi máu / Setup recovery fountain
        /// </summary>
        private void SetupFountain()
        {
            // TODO: Create fountain object with recovery effect
            Debug.Log($"[TownZone] Fountain setup at {fountainPosition}");
        }
        
        public override void OnPlayerEnter(GameObject player)
        {
            base.OnPlayerEnter(player);
            
            // Show town welcome message
            ShowWelcomeMessage(player);
            
            // Apply town buffs (safe zone, regen)
            ApplyTownBuffs(player);
        }
        
        public override void OnPlayerExit(GameObject player)
        {
            base.OnPlayerExit(player);
            
            // Remove town buffs
            RemoveTownBuffs(player);
        }
        
        /// <summary>
        /// Hiển thị lời chào / Show welcome message
        /// </summary>
        private void ShowWelcomeMessage(GameObject player)
        {
            string message = $"Chào mừng đến {zoneName}!";
            Debug.Log($"[TownZone] {message}");
            // TODO: Show UI message
        }
        
        /// <summary>
        /// Áp dụng buffs của town / Apply town buffs
        /// </summary>
        private void ApplyTownBuffs(GameObject player)
        {
            // TODO: Implement buff system
            Debug.Log($"[TownZone] Applying town buffs to player");
        }
        
        /// <summary>
        /// Xóa buffs của town / Remove town buffs
        /// </summary>
        private void RemoveTownBuffs(GameObject player)
        {
            // TODO: Remove buff system
            Debug.Log($"[TownZone] Removing town buffs from player");
        }
        
        /// <summary>
        /// Kiểm tra player có gần fountain không / Check if near fountain
        /// </summary>
        public bool IsNearFountain(Vector3 position)
        {
            return Vector3.Distance(position, fountainPosition) <= recoveryRadius;
        }
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            // Draw fountain radius
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(fountainPosition, recoveryRadius);
        }
    }
}
