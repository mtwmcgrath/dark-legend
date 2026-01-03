using UnityEngine;

namespace DarkLegend.Reset
{
    /// <summary>
    /// ScriptableObject for reset configuration - Cấu hình reset
    /// Allows designers to configure reset system through Unity Inspector
    /// </summary>
    [CreateAssetMenu(fileName = "ResetData", menuName = "Dark Legend/Reset/Reset Data", order = 1)]
    public class ResetData : ScriptableObject
    {
        [Header("Normal Reset Configuration")]
        [Tooltip("Requirements for normal reset - Yêu cầu cho reset thường")]
        public ResetRequirement normalResetRequirement;

        [Tooltip("Maximum normal resets allowed - Số reset thường tối đa")]
        public int maxNormalResets = 100;

        [Header("Grand Reset Configuration")]
        [Tooltip("Requirements for grand reset - Yêu cầu cho Grand Reset")]
        public ResetRequirement grandResetRequirement;

        [Tooltip("Maximum grand resets allowed - Số Grand Reset tối đa")]
        public int maxGrandResets = 10;

        [Header("Master Reset Configuration")]
        [Tooltip("Requirements for master reset - Yêu cầu cho Master Reset")]
        public ResetRequirement masterResetRequirement;

        [Tooltip("Master reset can only be done once - Master Reset chỉ làm 1 lần")]
        public bool allowOnlyOneMasterReset = true;

        [Header("Reset Effects")]
        [Tooltip("Level after reset - Level sau reset")]
        public int levelAfterReset = 1;

        [Tooltip("Keep stats points after reset - Giữ điểm stats sau reset")]
        public bool keepStats = false;

        [Tooltip("Keep items after reset - Giữ đồ sau reset")]
        public bool keepItems = true;

        [Tooltip("Keep skills after reset - Giữ skills sau reset")]
        public bool keepSkills = true;

        [Tooltip("Keep zen after reset - Giữ tiền sau reset")]
        public bool keepZen = true;

        [Header("Grand Reset Bonuses")]
        [Tooltip("Bonus stats for grand reset - Điểm stats bonus Grand Reset")]
        public int grandResetBonusStats = 5000;

        [Tooltip("Damage bonus for grand reset - % damage bonus Grand Reset")]
        public float grandDamageBonus = 0.10f; // 10%

        [Tooltip("Defense bonus for grand reset - % defense bonus Grand Reset")]
        public float grandDefenseBonus = 0.10f; // 10%

        [Tooltip("HP bonus for grand reset - % HP bonus Grand Reset")]
        public float grandHPBonus = 0.05f; // 5%

        [Tooltip("Grand reset title - Danh hiệu Grand Reset")]
        public string grandResetTitle = "Grand Master";

        [Header("Master Reset Bonuses")]
        [Tooltip("Bonus stats for master reset - Điểm stats bonus Master Reset")]
        public int masterBonusStats = 50000;

        [Tooltip("Damage bonus for master reset - % damage bonus Master Reset")]
        public float masterDamageBonus = 0.50f; // 50%

        [Tooltip("Defense bonus for master reset - % defense bonus Master Reset")]
        public float masterDefenseBonus = 0.50f; // 50%

        [Tooltip("Master reset title - Danh hiệu Master Reset")]
        public string masterTitle = "Master";

        [Tooltip("Master name color - Màu tên Master")]
        public Color masterNameColor = Color.yellow;

        [Tooltip("Unlock master skills - Mở khóa skills Master")]
        public bool unlockMasterSkills = true;

        [Tooltip("Unlock master wings - Mở khóa wings Master")]
        public bool unlockMasterWings = true;

        private void OnValidate()
        {
            // Initialize default requirements if null
            if (normalResetRequirement == null)
            {
                normalResetRequirement = new ResetRequirement
                {
                    MinLevel = 400,
                    ZenCost = 10000000,
                    MinResetCount = 0
                };
            }

            if (grandResetRequirement == null)
            {
                grandResetRequirement = new ResetRequirement
                {
                    MinLevel = 400,
                    ZenCost = 1000000000, // 1 billion
                    MinResetCount = 100
                };
            }

            if (masterResetRequirement == null)
            {
                masterResetRequirement = new ResetRequirement
                {
                    MinLevel = 400,
                    ZenCost = 10000000000, // 10 billion
                    MinResetCount = 10
                };
            }
        }
    }
}
