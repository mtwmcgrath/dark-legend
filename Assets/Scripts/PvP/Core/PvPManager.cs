using UnityEngine;
using System;
using System.Collections.Generic;

namespace DarkLegend.PvP
{
    /// <summary>
    /// Main PvP Manager - Quản lý tất cả hệ thống PvP
    /// Singleton pattern for global access
    /// </summary>
    public class PvPManager : MonoBehaviour
    {
        private static PvPManager instance;
        public static PvPManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PvPManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("PvPManager");
                        instance = go.AddComponent<PvPManager>();
                    }
                }
                return instance;
            }
        }
        
        [Header("Configuration")]
        public PvPData pvpData;
        
        [Header("Systems")]
        private DuelSystem duelSystem;
        private ArenaManager arenaManager;
        private BattlegroundManager battlegroundManager;
        private PKSystem pkSystem;
        private PvPRankingSystem rankingSystem;
        private TournamentManager tournamentManager;
        private BountySystem bountySystem;
        
        // Active PvP zones
        private List<PvPZone> activePvPZones = new List<PvPZone>();
        
        // Events
        public event Action<GameObject, GameObject> OnPvPKill;
        public event Action<GameObject, int> OnPvPDamage;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeSystems();
        }
        
        private void InitializeSystems()
        {
            // Initialize all PvP subsystems
            duelSystem = gameObject.AddComponent<DuelSystem>();
            arenaManager = gameObject.AddComponent<ArenaManager>();
            battlegroundManager = gameObject.AddComponent<BattlegroundManager>();
            pkSystem = gameObject.AddComponent<PKSystem>();
            rankingSystem = gameObject.AddComponent<PvPRankingSystem>();
            tournamentManager = gameObject.AddComponent<TournamentManager>();
            bountySystem = gameObject.AddComponent<BountySystem>();
            
            Debug.Log("PvP Manager initialized");
        }
        
        // Getters for subsystems
        public DuelSystem GetDuelSystem() => duelSystem;
        public ArenaManager GetArenaManager() => arenaManager;
        public BattlegroundManager GetBattlegroundManager() => battlegroundManager;
        public PKSystem GetPKSystem() => pkSystem;
        public PvPRankingSystem GetRankingSystem() => rankingSystem;
        public TournamentManager GetTournamentManager() => tournamentManager;
        public BountySystem GetBountySystem() => bountySystem;
        
        // Register/Unregister PvP zones
        public void RegisterPvPZone(PvPZone zone)
        {
            if (!activePvPZones.Contains(zone))
            {
                activePvPZones.Add(zone);
            }
        }
        
        public void UnregisterPvPZone(PvPZone zone)
        {
            activePvPZones.Remove(zone);
        }
        
        // Check if PvP is allowed between two players
        public bool CanPvP(GameObject player1, GameObject player2)
        {
            if (!pvpData.pvpEnabled) return false;
            if (player1 == player2) return false;
            
            // TODO: Add additional checks (same team, guild, party, etc.)
            return true;
        }
        
        // Handle PvP damage
        public void ProcessPvPDamage(GameObject attacker, GameObject target, int damage)
        {
            if (!CanPvP(attacker, target)) return;
            
            // Apply damage multiplier
            int finalDamage = Mathf.RoundToInt(damage * pvpData.globalPvPDamageMultiplier);
            
            OnPvPDamage?.Invoke(target, finalDamage);
            
            // TODO: Apply damage to target
        }
        
        // Handle PvP kill
        public void ProcessPvPKill(GameObject killer, GameObject victim)
        {
            OnPvPKill?.Invoke(killer, victim);
            
            // Update PK system
            pkSystem.OnPlayerKill(killer, victim);
            
            // Update rankings if in ranked mode
            // This will be handled by specific systems (Arena, Duel, etc.)
        }
    }
}
