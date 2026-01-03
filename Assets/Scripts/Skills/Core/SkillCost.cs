using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Chi phí sử dụng skill (MP, HP, vật phẩm)
    /// Skill usage cost (MP, HP, items)
    /// </summary>
    [CreateAssetMenu(fileName = "New Cost", menuName = "Dark Legend/Skills/Cost")]
    public class SkillCost : ScriptableObject
    {
        [Header("MP Cost")]
        [Tooltip("MP cost cơ bản / Base MP cost")]
        public float baseMPCost = 10f;
        
        [Tooltip("MP cost tăng mỗi level / MP cost increase per level")]
        public float mpCostPerLevel = 2f;
        
        [Tooltip("MP cost tối đa / Maximum MP cost")]
        public float maxMPCost = 200f;
        
        [Header("HP Cost")]
        [Tooltip("HP cost (nếu có) / HP cost (if any)")]
        public float baseHPCost = 0f;
        
        [Tooltip("HP cost tăng mỗi level / HP cost increase per level")]
        public float hpCostPerLevel = 0f;
        
        [Header("Other Costs")]
        [Tooltip("Có cần item không / Requires items")]
        public bool requiresItem = false;
        
        [Tooltip("Tên item cần / Required item name")]
        public string requiredItemName = "";
        
        [Tooltip("Số lượng item / Item quantity")]
        public int requiredItemCount = 1;
        
        /// <summary>
        /// Tính MP cost ở level cụ thể / Calculate MP cost at specific level
        /// </summary>
        public float GetMPCost(int level)
        {
            float cost = baseMPCost + (mpCostPerLevel * (level - 1));
            return Mathf.Min(cost, maxMPCost);
        }
        
        /// <summary>
        /// Tính MP cost với % giảm từ stats/equipment / Calculate MP cost with reduction
        /// </summary>
        public float GetMPCost(int level, float mpReduction)
        {
            float baseCost = GetMPCost(level);
            return baseCost * (1f - mpReduction);
        }
        
        /// <summary>
        /// Tính HP cost ở level cụ thể / Calculate HP cost at specific level
        /// </summary>
        public float GetHPCost(int level)
        {
            return baseHPCost + (hpCostPerLevel * (level - 1));
        }
        
        /// <summary>
        /// Kiểm tra có đủ điều kiện trả cost không / Check if can pay cost
        /// </summary>
        public bool CanPay(GameObject owner, int level)
        {
            // Kiểm tra MP
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats != null)
            {
                float mpCost = GetMPCost(level);
                if (stats.currentMP < mpCost)
                {
                    return false;
                }
                
                float hpCost = GetHPCost(level);
                if (hpCost > 0 && stats.currentHP <= hpCost)
                {
                    return false; // Không cho phép giết bản thân
                }
            }
            
            // Kiểm tra item (nếu cần)
            if (requiresItem)
            {
                // TODO: Implement inventory check
                // Cần có inventory system để check item
            }
            
            return true;
        }
        
        /// <summary>
        /// Trừ cost từ player / Deduct cost from player
        /// </summary>
        public void Pay(GameObject owner, int level)
        {
            CharacterStats stats = owner.GetComponent<CharacterStats>();
            if (stats != null)
            {
                // Trừ MP
                float mpCost = GetMPCost(level);
                stats.currentMP = Mathf.Max(0, stats.currentMP - mpCost);
                
                // Trừ HP (nếu có)
                float hpCost = GetHPCost(level);
                if (hpCost > 0)
                {
                    stats.currentHP = Mathf.Max(1, stats.currentHP - hpCost);
                }
            }
            
            // Trừ item (nếu cần)
            if (requiresItem)
            {
                // TODO: Implement inventory item removal
                // Cần có inventory system để remove item
            }
        }
    }
    
    /// <summary>
    /// Component giả định cho character stats
    /// Placeholder component for character stats
    /// </summary>
    public class CharacterStats : MonoBehaviour
    {
        [Header("Basic Stats")]
        public float maxHP = 1000f;
        public float currentHP = 1000f;
        public float maxMP = 500f;
        public float currentMP = 500f;
        
        [Header("Attributes")]
        public int STR = 10;
        public int AGI = 10;
        public int VIT = 10;
        public int ENE = 10;
        
        [Header("Combat Stats")]
        public int level = 1;
        public float attackPower = 50f;
        public float defense = 20f;
        public float magicPower = 30f;
        public float critRate = 0.05f;
        
        // Các methods khác sẽ được implement sau
    }
}
