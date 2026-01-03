using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Ultimate Skill - Skill tối thượng cần tích gauge
    /// Ultimate Skill - Ultimate abilities requiring gauge
    /// </summary>
    public class UltimateSkill : ActiveSkill
    {
        [Header("Ultimate Settings")]
        public float requiredGauge = 100f;      // Gauge cần để sử dụng
        public bool consumeAllGauge = true;     // Có tiêu hết gauge không
        public float gaugeCostPercentage = 1f;  // % gauge tiêu hao (nếu không consume all)
        
        [Header("Ultimate Effects")]
        public bool hasTransformation = false;  // Có transform character không
        public GameObject transformationPrefab;
        public float transformationDuration = 10f;
        
        private UltimateGaugeManager gaugeManager;
        
        /// <summary>
        /// Override Initialize để lấy gauge manager / Override Initialize to get gauge manager
        /// </summary>
        public override void Initialize(GameObject owner, SkillManager manager)
        {
            base.Initialize(owner, manager);
            
            gaugeManager = owner.GetComponent<UltimateGaugeManager>();
            if (gaugeManager == null)
            {
                gaugeManager = owner.AddComponent<UltimateGaugeManager>();
            }
        }
        
        /// <summary>
        /// Kiểm tra có thể dùng ultimate không / Check if can use ultimate
        /// </summary>
        public override bool CanUse()
        {
            if (!base.CanUse()) return false;
            
            if (gaugeManager == null) return false;
            
            float requiredAmount = consumeAllGauge ? requiredGauge : (requiredGauge * gaugeCostPercentage);
            
            return gaugeManager.currentGauge >= requiredAmount;
        }
        
        /// <summary>
        /// Override consume cost để trừ gauge / Override consume cost to deduct gauge
        /// </summary>
        protected override void ConsumeCost()
        {
            base.ConsumeCost();
            
            if (gaugeManager != null)
            {
                if (consumeAllGauge)
                {
                    gaugeManager.ConsumeGauge(gaugeManager.currentGauge);
                }
                else
                {
                    float gaugeCost = requiredGauge * gaugeCostPercentage;
                    gaugeManager.ConsumeGauge(gaugeCost);
                }
            }
        }
        
        /// <summary>
        /// Execute ultimate skill / Thực hiện ultimate skill
        /// </summary>
        protected override void ExecuteSkill(Vector3 targetPosition, GameObject targetObject)
        {
            base.ExecuteSkill(targetPosition, targetObject);
            
            // Play ultimate animation
            PlayUltimateAnimation();
            
            // Spawn ultimate effect
            SpawnUltimateEffect();
            
            // Apply transformation nếu có
            if (hasTransformation && transformationPrefab != null)
            {
                ApplyTransformation();
            }
            
            Debug.Log($"ULTIMATE SKILL ACTIVATED: {skillData.skillName}");
        }
        
        /// <summary>
        /// Play ultimate animation / Phát animation ultimate
        /// </summary>
        protected virtual void PlayUltimateAnimation()
        {
            Animator animator = owner.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Ultimate");
            }
        }
        
        /// <summary>
        /// Spawn ultimate effect / Tạo hiệu ứng ultimate
        /// </summary>
        protected virtual void SpawnUltimateEffect()
        {
            if (skillData.castEffect != null)
            {
                GameObject effect = Instantiate(skillData.castEffect, owner.transform.position, Quaternion.identity);
                effect.transform.SetParent(owner.transform);
                
                // Scale effect lớn hơn
                effect.transform.localScale = Vector3.one * 2f;
                
                Destroy(effect, 5f);
            }
        }
        
        /// <summary>
        /// Áp dụng transformation / Apply transformation
        /// </summary>
        protected virtual void ApplyTransformation()
        {
            TransformationEffect transformEffect = owner.GetComponent<TransformationEffect>();
            if (transformEffect == null)
            {
                transformEffect = owner.AddComponent<TransformationEffect>();
            }
            
            transformEffect.StartTransformation(
                transformationPrefab,
                transformationDuration,
                GetTransformationBonuses()
            );
        }
        
        /// <summary>
        /// Lấy stat bonuses cho transformation / Get stat bonuses for transformation
        /// </summary>
        protected virtual TransformationBonuses GetTransformationBonuses()
        {
            return new TransformationBonuses
            {
                damageMultiplier = 1.5f + (currentLevel * 0.1f),
                defenseMultiplier = 1.3f + (currentLevel * 0.05f),
                speedMultiplier = 1.2f,
                critRateBonus = 0.2f
            };
        }
    }
    
    /// <summary>
    /// Ultimate Gauge Manager - Quản lý gauge cho ultimate skills
    /// Ultimate Gauge Manager - Manages gauge for ultimate skills
    /// </summary>
    public class UltimateGaugeManager : MonoBehaviour
    {
        [Header("Gauge Settings")]
        public float maxGauge = 100f;
        public float currentGauge = 0f;
        
        [Header("Gauge Gain")]
        public float gaugePerHit = 5f;          // Gauge khi đánh trúng
        public float gaugePerKill = 20f;        // Gauge khi giết enemy
        public float gaugePerDamageReceived = 2f; // Gauge khi nhận damage
        public float gaugeDecayRate = 0f;       // Gauge tự giảm (0 = không giảm)
        
        /// <summary>
        /// Thêm gauge / Add gauge
        /// </summary>
        public void AddGauge(float amount)
        {
            currentGauge = Mathf.Min(maxGauge, currentGauge + amount);
            Debug.Log($"Gauge: {currentGauge}/{maxGauge}");
        }
        
        /// <summary>
        /// Tiêu hao gauge / Consume gauge
        /// </summary>
        public void ConsumeGauge(float amount)
        {
            currentGauge = Mathf.Max(0f, currentGauge - amount);
            Debug.Log($"Gauge consumed: {amount}. Remaining: {currentGauge}/{maxGauge}");
        }
        
        /// <summary>
        /// Gọi khi đánh trúng / Called when hitting enemy
        /// </summary>
        public void OnHit()
        {
            AddGauge(gaugePerHit);
        }
        
        /// <summary>
        /// Gọi khi giết enemy / Called when killing enemy
        /// </summary>
        public void OnKill()
        {
            AddGauge(gaugePerKill);
        }
        
        /// <summary>
        /// Gọi khi nhận damage / Called when taking damage
        /// </summary>
        public void OnDamageReceived(float damage)
        {
            float gaugeGain = (damage / 10f) * gaugePerDamageReceived;
            AddGauge(gaugeGain);
        }
        
        /// <summary>
        /// Update để xử lý gauge decay / Update to handle gauge decay
        /// </summary>
        private void Update()
        {
            if (gaugeDecayRate > 0f && currentGauge > 0f)
            {
                currentGauge = Mathf.Max(0f, currentGauge - (gaugeDecayRate * Time.deltaTime));
            }
        }
    }
    
    /// <summary>
    /// Stat bonuses cho transformation / Stat bonuses for transformation
    /// </summary>
    public class TransformationBonuses
    {
        public float damageMultiplier = 1.5f;
        public float defenseMultiplier = 1.3f;
        public float speedMultiplier = 1.2f;
        public float critRateBonus = 0.2f;
    }
    
    /// <summary>
    /// Transformation Effect - Hiệu ứng biến hình
    /// Transformation Effect
    /// </summary>
    public class TransformationEffect : MonoBehaviour
    {
        private bool isTransformed = false;
        private float remainingDuration = 0f;
        private GameObject transformVisual;
        private TransformationBonuses bonuses;
        
        private CharacterStats stats;
        private float originalDamage;
        private float originalDefense;
        private float originalCritRate;
        
        public void StartTransformation(GameObject visualPrefab, float duration, TransformationBonuses bonuses)
        {
            if (isTransformed)
            {
                EndTransformation();
            }
            
            this.bonuses = bonuses;
            remainingDuration = duration;
            isTransformed = true;
            
            // Spawn visual effect
            if (visualPrefab != null)
            {
                transformVisual = Instantiate(visualPrefab, transform);
            }
            
            // Apply stat bonuses
            ApplyBonuses();
            
            Debug.Log($"Transformation started for {duration}s");
        }
        
        private void ApplyBonuses()
        {
            stats = GetComponent<CharacterStats>();
            if (stats == null) return;
            
            // Lưu stats gốc
            originalDamage = stats.attackPower;
            originalDefense = stats.defense;
            originalCritRate = stats.critRate;
            
            // Apply bonuses
            stats.attackPower *= bonuses.damageMultiplier;
            stats.defense *= bonuses.defenseMultiplier;
            stats.critRate += bonuses.critRateBonus;
            
            // TODO: Apply speed multiplier
        }
        
        private void RemoveBonuses()
        {
            if (stats == null) return;
            
            stats.attackPower = originalDamage;
            stats.defense = originalDefense;
            stats.critRate = originalCritRate;
        }
        
        private void Update()
        {
            if (!isTransformed) return;
            
            remainingDuration -= Time.deltaTime;
            
            if (remainingDuration <= 0f)
            {
                EndTransformation();
            }
        }
        
        private void EndTransformation()
        {
            if (!isTransformed) return;
            
            isTransformed = false;
            remainingDuration = 0f;
            
            // Remove visual
            if (transformVisual != null)
            {
                Destroy(transformVisual);
            }
            
            // Remove bonuses
            RemoveBonuses();
            
            Debug.Log("Transformation ended");
        }
    }
}
