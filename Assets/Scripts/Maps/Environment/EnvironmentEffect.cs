using UnityEngine;

namespace DarkLegend.Maps.Environment
{
    /// <summary>
    /// Hiệu ứng môi trường / Environmental effects
    /// Handles special environmental gameplay effects
    /// </summary>
    public class EnvironmentEffect : MonoBehaviour
    {
        [Header("Effect Type")]
        [Tooltip("Loại hiệu ứng / Effect type")]
        [SerializeField] private EffectType effectType = EffectType.None;
        
        [Tooltip("Kích hoạt / Is active")]
        [SerializeField] private bool isActive = true;
        
        [Header("Damage Over Time")]
        [Tooltip("Damage mỗi giây / Damage per second")]
        [SerializeField] private float damagePerSecond = 5f;
        
        [Tooltip("Loại damage / Damage type")]
        [SerializeField] private string damageType = "Environmental";
        
        [Header("Movement Effects")]
        [Tooltip("Giảm tốc độ / Speed reduction")]
        [Range(0f, 1f)]
        [SerializeField] private float speedReduction = 0.3f;
        
        [Header("Buff/Debuff")]
        [Tooltip("Stat được ảnh hưởng / Affected stat")]
        [SerializeField] private string affectedStat = "";
        
        [Tooltip("Modifier / Stat modifier")]
        [SerializeField] private float statModifier = 1f;
        
        [Header("Visual")]
        [Tooltip("Hiệu ứng particle / Particle effect")]
        [SerializeField] private GameObject particleEffect;
        
        [Tooltip("Khu vực ảnh hưởng / Effect area")]
        [SerializeField] private Vector3 effectArea = new Vector3(10, 5, 10);
        
        private GameObject spawnedEffect;
        
        private void Start()
        {
            if (isActive && particleEffect != null)
            {
                spawnedEffect = Instantiate(particleEffect, transform.position, Quaternion.identity, transform);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;
            
            if (other.CompareTag("Player"))
            {
                ApplyEffect(other.gameObject);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                RemoveEffect(other.gameObject);
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!isActive) return;
            
            if (other.CompareTag("Player"))
            {
                ApplyContinuousEffect(other.gameObject);
            }
        }
        
        /// <summary>
        /// Áp dụng hiệu ứng / Apply effect
        /// </summary>
        private void ApplyEffect(GameObject target)
        {
            switch (effectType)
            {
                case EffectType.Poison:
                    ApplyPoisonEffect(target);
                    break;
                case EffectType.Fire:
                    ApplyFireEffect(target);
                    break;
                case EffectType.Ice:
                    ApplyIceEffect(target);
                    break;
                case EffectType.Healing:
                    ApplyHealingEffect(target);
                    break;
                case EffectType.Buff:
                    ApplyBuffEffect(target);
                    break;
                case EffectType.Debuff:
                    ApplyDebuffEffect(target);
                    break;
            }
            
            Debug.Log($"[EnvironmentEffect] Applied {effectType} effect to {target.name}");
        }
        
        /// <summary>
        /// Xóa hiệu ứng / Remove effect
        /// </summary>
        private void RemoveEffect(GameObject target)
        {
            Debug.Log($"[EnvironmentEffect] Removed {effectType} effect from {target.name}");
            // TODO: Remove effects from target
        }
        
        /// <summary>
        /// Áp dụng hiệu ứng liên tục / Apply continuous effect
        /// </summary>
        private void ApplyContinuousEffect(GameObject target)
        {
            if (effectType == EffectType.Poison || effectType == EffectType.Fire)
            {
                ApplyDamageOverTime(target);
            }
            else if (effectType == EffectType.Healing)
            {
                ApplyHealingOverTime(target);
            }
        }
        
        /// <summary>
        /// Áp dụng độc / Apply poison effect
        /// </summary>
        private void ApplyPoisonEffect(GameObject target)
        {
            // TODO: Apply poison DOT
            Debug.Log($"[EnvironmentEffect] Poison applied");
        }
        
        /// <summary>
        /// Áp dụng lửa / Apply fire effect
        /// </summary>
        private void ApplyFireEffect(GameObject target)
        {
            // TODO: Apply fire DOT
            Debug.Log($"[EnvironmentEffect] Fire applied");
        }
        
        /// <summary>
        /// Áp dụng băng / Apply ice effect
        /// </summary>
        private void ApplyIceEffect(GameObject target)
        {
            // TODO: Apply slow effect
            Debug.Log($"[EnvironmentEffect] Ice applied - speed reduced by {speedReduction * 100}%");
        }
        
        /// <summary>
        /// Áp dụng hồi máu / Apply healing effect
        /// </summary>
        private void ApplyHealingEffect(GameObject target)
        {
            // TODO: Apply healing over time
            Debug.Log($"[EnvironmentEffect] Healing applied");
        }
        
        /// <summary>
        /// Áp dụng buff / Apply buff
        /// </summary>
        private void ApplyBuffEffect(GameObject target)
        {
            // TODO: Apply stat buff
            Debug.Log($"[EnvironmentEffect] Buff applied: {affectedStat} x{statModifier}");
        }
        
        /// <summary>
        /// Áp dụng debuff / Apply debuff
        /// </summary>
        private void ApplyDebuffEffect(GameObject target)
        {
            // TODO: Apply stat debuff
            Debug.Log($"[EnvironmentEffect] Debuff applied: {affectedStat} x{statModifier}");
        }
        
        /// <summary>
        /// Áp dụng damage theo thời gian / Apply damage over time
        /// </summary>
        private void ApplyDamageOverTime(GameObject target)
        {
            float damage = damagePerSecond * Time.deltaTime;
            // TODO: Apply damage to target
        }
        
        /// <summary>
        /// Áp dụng healing theo thời gian / Apply healing over time
        /// </summary>
        private void ApplyHealingOverTime(GameObject target)
        {
            float healing = damagePerSecond * Time.deltaTime;
            // TODO: Apply healing to target
        }
        
        /// <summary>
        /// Kích hoạt/vô hiệu hóa / Enable/disable effect
        /// </summary>
        public void SetActive(bool active)
        {
            isActive = active;
            
            if (spawnedEffect != null)
            {
                spawnedEffect.SetActive(active);
            }
        }
        
        private void OnDrawGizmos()
        {
            // Draw effect area
            Color gizmoColor = Color.green;
            
            switch (effectType)
            {
                case EffectType.Poison:
                    gizmoColor = Color.green;
                    break;
                case EffectType.Fire:
                    gizmoColor = Color.red;
                    break;
                case EffectType.Ice:
                    gizmoColor = Color.cyan;
                    break;
                case EffectType.Healing:
                    gizmoColor = Color.yellow;
                    break;
                case EffectType.Buff:
                    gizmoColor = Color.blue;
                    break;
                case EffectType.Debuff:
                    gizmoColor = Color.magenta;
                    break;
            }
            
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawCube(transform.position, effectArea);
            
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(transform.position, effectArea);
        }
    }
    
    /// <summary>
    /// Loại hiệu ứng / Effect types
    /// </summary>
    public enum EffectType
    {
        None,       // Không có
        Poison,     // Độc
        Fire,       // Lửa
        Ice,        // Băng
        Healing,    // Hồi máu
        Buff,       // Buff
        Debuff      // Debuff
    }
}
