using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Stat modifier types / Loại bổ trợ chỉ số
    /// </summary>
    public enum ModifierType
    {
        Flat,        // +50 HP
        Percentage   // +10% HP
    }
    
    /// <summary>
    /// Stat modifier / Bổ trợ chỉ số
    /// </summary>
    [System.Serializable]
    public class StatModifier
    {
        public string StatName;          // Stat to modify / Chỉ số cần bổ trợ
        public ModifierType Type;        // Type of modifier / Loại bổ trợ
        public float Value;              // Modifier value / Giá trị bổ trợ
        public float Duration;           // Duration in seconds (0 = permanent) / Thời gian tính bằng giây (0 = vĩnh viễn)
        public string Source;            // Source of modifier (e.g., "Potion", "Buff") / Nguồn bổ trợ (vd: "Thuốc", "Buff")
        
        private float startTime;
        
        public StatModifier(string statName, ModifierType type, float value, float duration = 0f, string source = "")
        {
            StatName = statName;
            Type = type;
            Value = value;
            Duration = duration;
            Source = source;
            startTime = Time.time;
        }
        
        /// <summary>
        /// Apply modifier to base value / Áp dụng bổ trợ vào giá trị cơ bản
        /// </summary>
        public float ApplyModifier(float baseValue)
        {
            if (Type == ModifierType.Flat)
            {
                return baseValue + Value;
            }
            else // Percentage
            {
                return baseValue * (1 + Value / 100f);
            }
        }
        
        /// <summary>
        /// Check if modifier has expired / Kiểm tra bổ trợ đã hết hạn
        /// </summary>
        public bool IsExpired()
        {
            if (Duration <= 0)
                return false;
                
            return Time.time - startTime >= Duration;
        }
        
        /// <summary>
        /// Get remaining time / Lấy thời gian còn lại
        /// </summary>
        public float GetRemainingTime()
        {
            if (Duration <= 0)
                return -1;
                
            float remaining = Duration - (Time.time - startTime);
            return Mathf.Max(0, remaining);
        }
    }
}
