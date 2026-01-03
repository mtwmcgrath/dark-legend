using UnityEngine;
using UnityEngine.UI;

namespace DarkLegend.Mobile.UI
{
    /// <summary>
    /// Mobile HUD (Health, Mana, EXP bars)
    /// HUD cho mobile (thanh HP, MP, EXP)
    /// </summary>
    public class MobileHUD : MonoBehaviour
    {
        [Header("Health Bar")]
        public Image healthFillImage;
        public Text healthText;
        public Color healthColor = Color.red;
        public Color lowHealthColor = new Color(1f, 0.3f, 0f);
        public float lowHealthThreshold = 0.3f;

        [Header("Mana Bar")]
        public Image manaFillImage;
        public Text manaText;
        public Color manaColor = Color.blue;

        [Header("Experience Bar")]
        public Image expFillImage;
        public Text levelText;
        public Color expColor = Color.yellow;

        [Header("Mini Stats")]
        public Text damageText;
        public Text defenseText;
        public Text goldText;

        [Header("Buffs/Debuffs")]
        public Transform buffContainer;
        public GameObject buffIconPrefab;

        // Current values
        private float currentHP = 100f;
        private float maxHP = 100f;
        private float currentMP = 100f;
        private float maxMP = 100f;
        private float currentEXP = 0f;
        private float maxEXP = 100f;
        private int currentLevel = 1;

        private void Start()
        {
            UpdateAllBars();
        }

        /// <summary>
        /// Update health bar
        /// Cập nhật thanh máu
        /// </summary>
        public void UpdateHealthBar(float current, float max)
        {
            currentHP = current;
            maxHP = max;

            if (healthFillImage != null)
            {
                float fillAmount = Mathf.Clamp01(currentHP / maxHP);
                healthFillImage.fillAmount = fillAmount;

                // Change color if low health
                if (fillAmount < lowHealthThreshold)
                {
                    healthFillImage.color = lowHealthColor;
                }
                else
                {
                    healthFillImage.color = healthColor;
                }
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.CeilToInt(currentHP)} / {Mathf.CeilToInt(maxHP)}";
            }
        }

        /// <summary>
        /// Update mana bar
        /// Cập nhật thanh mana
        /// </summary>
        public void UpdateManaBar(float current, float max)
        {
            currentMP = current;
            maxMP = max;

            if (manaFillImage != null)
            {
                manaFillImage.fillAmount = Mathf.Clamp01(currentMP / maxMP);
            }

            if (manaText != null)
            {
                manaText.text = $"{Mathf.CeilToInt(currentMP)} / {Mathf.CeilToInt(maxMP)}";
            }
        }

        /// <summary>
        /// Update experience bar
        /// Cập nhật thanh kinh nghiệm
        /// </summary>
        public void UpdateExpBar(float current, float max, int level)
        {
            currentEXP = current;
            maxEXP = max;
            currentLevel = level;

            if (expFillImage != null)
            {
                expFillImage.fillAmount = Mathf.Clamp01(currentEXP / maxEXP);
            }

            if (levelText != null)
            {
                levelText.text = $"Lv.{currentLevel}";
            }
        }

        /// <summary>
        /// Update all bars
        /// Cập nhật tất cả thanh
        /// </summary>
        public void UpdateAllBars()
        {
            UpdateHealthBar(currentHP, maxHP);
            UpdateManaBar(currentMP, maxMP);
            UpdateExpBar(currentEXP, maxEXP, currentLevel);
        }

        /// <summary>
        /// Update damage stat
        /// Cập nhật chỉ số damage
        /// </summary>
        public void UpdateDamage(int damage)
        {
            if (damageText != null)
            {
                damageText.text = $"ATK: {damage}";
            }
        }

        /// <summary>
        /// Update defense stat
        /// Cập nhật chỉ số defense
        /// </summary>
        public void UpdateDefense(int defense)
        {
            if (defenseText != null)
            {
                defenseText.text = $"DEF: {defense}";
            }
        }

        /// <summary>
        /// Update gold
        /// Cập nhật vàng
        /// </summary>
        public void UpdateGold(int gold)
        {
            if (goldText != null)
            {
                goldText.text = $"Gold: {gold}";
            }
        }

        /// <summary>
        /// Add buff icon
        /// Thêm icon buff
        /// </summary>
        public void AddBuff(Sprite icon, float duration)
        {
            if (buffContainer == null || buffIconPrefab == null)
                return;

            GameObject buffIcon = Instantiate(buffIconPrefab, buffContainer);
            Image iconImage = buffIcon.GetComponent<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = icon;
            }

            // TODO: Add duration countdown
            Destroy(buffIcon, duration);
        }

        /// <summary>
        /// Clear all buffs
        /// Xóa tất cả buff
        /// </summary>
        public void ClearBuffs()
        {
            if (buffContainer == null)
                return;

            foreach (Transform child in buffContainer)
            {
                Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Show damage text (floating)
        /// Hiển thị text damage (nổi)
        /// </summary>
        public void ShowDamageText(int damage, bool isCritical = false)
        {
            // TODO: Implement floating damage text
            Debug.Log($"[MobileHUD] Damage: {damage}{(isCritical ? " CRIT!" : "")}");
        }
    }
}
