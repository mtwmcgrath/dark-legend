using UnityEngine;

namespace DarkLegend.Skills
{
    /// <summary>
    /// Base class cho tất cả skill effects
    /// Base class for all skill effects
    /// </summary>
    public abstract class SkillEffect : MonoBehaviour
    {
        [Header("Effect Settings")]
        public float duration = 0f;          // Thời gian hiệu lực (0 = instant)
        public bool stackable = false;       // Có thể stack nhiều lần không
        public int maxStacks = 1;            // Số stack tối đa
        public Sprite effectIcon;            // Icon hiển thị trên UI
        
        [Header("Visual")]
        public GameObject visualEffectPrefab;
        public bool attachToTarget = true;
        
        protected GameObject target;
        protected GameObject source;
        protected float remainingDuration;
        protected int currentStacks = 1;
        protected GameObject visualEffect;
        
        /// <summary>
        /// Khởi tạo effect / Initialize effect
        /// </summary>
        public virtual void Initialize(GameObject target, GameObject source, float duration = 0f)
        {
            this.target = target;
            this.source = source;
            this.remainingDuration = duration > 0 ? duration : this.duration;
            
            ApplyEffect();
            SpawnVisualEffect();
        }
        
        /// <summary>
        /// Áp dụng hiệu ứng / Apply effect
        /// </summary>
        protected abstract void ApplyEffect();
        
        /// <summary>
        /// Loại bỏ hiệu ứng / Remove effect
        /// </summary>
        protected abstract void RemoveEffect();
        
        /// <summary>
        /// Update mỗi frame / Update each frame
        /// </summary>
        protected virtual void Update()
        {
            if (remainingDuration > 0f)
            {
                remainingDuration -= Time.deltaTime;
                
                if (remainingDuration <= 0f)
                {
                    EndEffect();
                }
            }
        }
        
        /// <summary>
        /// Kết thúc effect / End effect
        /// </summary>
        public virtual void EndEffect()
        {
            RemoveEffect();
            RemoveVisualEffect();
            Destroy(this);
        }
        
        /// <summary>
        /// Stack thêm effect / Stack additional effect
        /// </summary>
        public virtual bool AddStack()
        {
            if (!stackable) return false;
            if (currentStacks >= maxStacks) return false;
            
            currentStacks++;
            RefreshDuration();
            return true;
        }
        
        /// <summary>
        /// Refresh duration / Làm mới thời gian
        /// </summary>
        public virtual void RefreshDuration()
        {
            remainingDuration = duration;
        }
        
        /// <summary>
        /// Spawn visual effect / Tạo hiệu ứng visual
        /// </summary>
        protected virtual void SpawnVisualEffect()
        {
            if (visualEffectPrefab == null) return;
            
            if (attachToTarget && target != null)
            {
                visualEffect = Instantiate(visualEffectPrefab, target.transform);
            }
            else
            {
                visualEffect = Instantiate(visualEffectPrefab, target.transform.position, Quaternion.identity);
            }
        }
        
        /// <summary>
        /// Remove visual effect / Xóa hiệu ứng visual
        /// </summary>
        protected virtual void RemoveVisualEffect()
        {
            if (visualEffect != null)
            {
                Destroy(visualEffect);
            }
        }
        
        /// <summary>
        /// Lấy thông tin effect / Get effect info
        /// </summary>
        public virtual string GetDescription()
        {
            return $"{GetType().Name} (Stacks: {currentStacks}, Duration: {remainingDuration:F1}s)";
        }
    }
}
