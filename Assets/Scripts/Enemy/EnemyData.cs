using UnityEngine;

namespace DarkLegend.Enemy
{
    /// <summary>
    /// ScriptableObject for enemy configuration
    /// ScriptableObject cho cấu hình quái vật
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy", menuName = "Dark Legend/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Basic Info")]
        public string enemyName = "Monster";
        public int level = 1;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;
        
        [Header("Stats")]
        public int strength = 10;
        public int agility = 10;
        public int vitality = 10;
        public int energy = 10;
        
        [Header("Movement")]
        public float moveSpeed = 3f;
        public float patrolRange = 10f;
        public float chaseRange = 15f;
        public float attackRange = 2f;
        
        [Header("Combat")]
        public float attackCooldown = 2f;
        public float returnToPatrolDistance = 20f;
        
        [Header("Rewards")]
        public long baseExpReward = 50;
        public int baseGoldReward = 10;
        // Note: Item drops will be implemented when Inventory system is ready
        
        [Header("Visual")]
        public GameObject enemyPrefab;
        public RuntimeAnimatorController animatorController;
        
        [Header("AI Behavior")]
        public float patrolWaitTime = 2f;
        public bool isAggressive = true; // Automatically attack nearby players
        public float aggroRange = 10f;
    }
}
