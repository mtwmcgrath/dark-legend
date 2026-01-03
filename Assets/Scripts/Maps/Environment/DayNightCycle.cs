using UnityEngine;

namespace DarkLegend.Maps.Environment
{
    /// <summary>
    /// Hệ thống ngày/đêm / Day/Night cycle system
    /// Controls time of day and lighting
    /// </summary>
    public class DayNightCycle : MonoBehaviour
    {
        [Header("Time Settings")]
        [Tooltip("Độ dài 1 ngày (phút thực) / Day length in real minutes")]
        [SerializeField] private float dayLengthMinutes = 30f;
        
        [Tooltip("Giờ bắt đầu / Starting hour (0-24)")]
        [SerializeField] private float startingHour = 6f;
        
        [Tooltip("Tốc độ thời gian / Time speed multiplier")]
        [SerializeField] private float timeSpeed = 1f;
        
        [Header("Day/Night Timing")]
        [Tooltip("Giờ bình minh / Dawn hour")]
        [SerializeField] private float dawnHour = 6f;
        
        [Tooltip("Giờ hoàng hôn / Dusk hour")]
        [SerializeField] private float duskHour = 18f;
        
        [Header("Lighting")]
        [Tooltip("Directional light / Sun light")]
        [SerializeField] private Light directionalLight;
        
        [Tooltip("Màu ngày / Day color")]
        [SerializeField] private Color dayColor = new Color(1f, 0.95f, 0.8f);
        
        [Tooltip("Màu đêm / Night color")]
        [SerializeField] private Color nightColor = new Color(0.5f, 0.5f, 0.7f);
        
        [Tooltip("Độ sáng ngày / Day intensity")]
        [SerializeField] private float dayIntensity = 1f;
        
        [Tooltip("Độ sáng đêm / Night intensity")]
        [SerializeField] private float nightIntensity = 0.3f;
        
        [Header("Sky")]
        [Tooltip("Material trời / Sky material")]
        [SerializeField] private Material skyMaterial;
        
        [Tooltip("Màu trời ngày / Day sky color")]
        [SerializeField] private Color daySkyColor = new Color(0.5f, 0.7f, 1f);
        
        [Tooltip("Màu trời đêm / Night sky color")]
        [SerializeField] private Color nightSkyColor = new Color(0.1f, 0.1f, 0.2f);
        
        [Header("Ambient")]
        [Tooltip("Màu ambient ngày / Day ambient color")]
        [SerializeField] private Color dayAmbientColor = new Color(0.5f, 0.5f, 0.5f);
        
        [Tooltip("Màu ambient đêm / Night ambient color")]
        [SerializeField] private Color nightAmbientColor = new Color(0.2f, 0.2f, 0.3f);
        
        private float currentTimeOfDay = 0f; // 0-24 hours
        private bool isNight = false;
        
        // Events
        public delegate void TimeEventHandler();
        public event TimeEventHandler OnDawn;
        public event TimeEventHandler OnDusk;
        public event TimeEventHandler OnNoon;
        public event TimeEventHandler OnMidnight;
        
        private void Start()
        {
            currentTimeOfDay = startingHour;
            
            if (directionalLight == null)
            {
                directionalLight = FindObjectOfType<Light>();
            }
            
            UpdateLighting();
        }
        
        private void Update()
        {
            // Update time
            UpdateTime();
            
            // Update lighting
            UpdateLighting();
            
            // Check for time events
            CheckTimeEvents();
        }
        
        /// <summary>
        /// Cập nhật thời gian / Update time
        /// </summary>
        private void UpdateTime()
        {
            // Convert day length to hours per second
            float hoursPerSecond = 24f / (dayLengthMinutes * 60f);
            
            // Increment time
            currentTimeOfDay += hoursPerSecond * Time.deltaTime * timeSpeed;
            
            // Wrap around 24 hours
            if (currentTimeOfDay >= 24f)
            {
                currentTimeOfDay -= 24f;
            }
        }
        
        /// <summary>
        /// Cập nhật ánh sáng / Update lighting
        /// </summary>
        private void UpdateLighting()
        {
            // Calculate time factor (0 = midnight, 0.5 = noon, 1 = midnight)
            float timeFactor = Mathf.Abs((currentTimeOfDay - 12f) / 12f);
            
            // Update sun rotation
            if (directionalLight != null)
            {
                float rotation = (currentTimeOfDay / 24f) * 360f - 90f;
                directionalLight.transform.rotation = Quaternion.Euler(rotation, 0, 0);
                
                // Lerp light color and intensity
                directionalLight.color = Color.Lerp(dayColor, nightColor, timeFactor);
                directionalLight.intensity = Mathf.Lerp(dayIntensity, nightIntensity, timeFactor);
            }
            
            // Update ambient lighting
            RenderSettings.ambientLight = Color.Lerp(dayAmbientColor, nightAmbientColor, timeFactor);
            
            // Update sky
            if (skyMaterial != null)
            {
                Color skyColor = Color.Lerp(daySkyColor, nightSkyColor, timeFactor);
                RenderSettings.ambientSkyColor = skyColor;
            }
            
            // Update night status
            bool wasNight = isNight;
            isNight = currentTimeOfDay < dawnHour || currentTimeOfDay > duskHour;
            
            if (isNight != wasNight)
            {
                OnTimeOfDayChanged();
            }
        }
        
        /// <summary>
        /// Kiểm tra events thời gian / Check time events
        /// </summary>
        private void CheckTimeEvents()
        {
            float prevTime = currentTimeOfDay - (24f / (dayLengthMinutes * 60f)) * Time.deltaTime;
            
            // Check dawn
            if (prevTime < dawnHour && currentTimeOfDay >= dawnHour)
            {
                OnDawn?.Invoke();
                Debug.Log("[DayNightCycle] Dawn");
            }
            
            // Check dusk
            if (prevTime < duskHour && currentTimeOfDay >= duskHour)
            {
                OnDusk?.Invoke();
                Debug.Log("[DayNightCycle] Dusk");
            }
            
            // Check noon
            if (prevTime < 12f && currentTimeOfDay >= 12f)
            {
                OnNoon?.Invoke();
                Debug.Log("[DayNightCycle] Noon");
            }
            
            // Check midnight
            if (prevTime < 24f && currentTimeOfDay < prevTime)
            {
                OnMidnight?.Invoke();
                Debug.Log("[DayNightCycle] Midnight");
            }
        }
        
        /// <summary>
        /// Khi thay đổi ngày/đêm / When time of day changes
        /// </summary>
        private void OnTimeOfDayChanged()
        {
            if (isNight)
            {
                Debug.Log("[DayNightCycle] Night time started");
                // TODO: Trigger night-specific events
            }
            else
            {
                Debug.Log("[DayNightCycle] Day time started");
                // TODO: Trigger day-specific events
            }
        }
        
        /// <summary>
        /// Lấy giờ hiện tại / Get current time
        /// </summary>
        public float GetCurrentTime()
        {
            return currentTimeOfDay;
        }
        
        /// <summary>
        /// Lấy giờ và phút / Get hours and minutes
        /// </summary>
        public void GetTime(out int hours, out int minutes)
        {
            hours = Mathf.FloorToInt(currentTimeOfDay);
            minutes = Mathf.FloorToInt((currentTimeOfDay - hours) * 60f);
        }
        
        /// <summary>
        /// Kiểm tra có phải ban đêm / Check if night time
        /// </summary>
        public bool IsNightTime()
        {
            return isNight;
        }
        
        /// <summary>
        /// Kiểm tra có phải ban ngày / Check if day time
        /// </summary>
        public bool IsDayTime()
        {
            return !isNight;
        }
        
        /// <summary>
        /// Set thời gian / Set time
        /// </summary>
        public void SetTime(float hour)
        {
            currentTimeOfDay = Mathf.Clamp(hour, 0f, 24f);
            UpdateLighting();
        }
        
        /// <summary>
        /// Set tốc độ thời gian / Set time speed
        /// </summary>
        public void SetTimeSpeed(float speed)
        {
            timeSpeed = Mathf.Max(0f, speed);
        }
        
        /// <summary>
        /// Tạm dừng thời gian / Pause time
        /// </summary>
        public void PauseTime()
        {
            timeSpeed = 0f;
        }
        
        /// <summary>
        /// Tiếp tục thời gian / Resume time
        /// </summary>
        public void ResumeTime()
        {
            timeSpeed = 1f;
        }
    }
}
