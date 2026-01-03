using UnityEngine;

namespace DarkLegend.Maps.Environment
{
    /// <summary>
    /// Hệ thống thời tiết / Weather system
    /// Controls weather effects and conditions
    /// </summary>
    public class Weather : MonoBehaviour
    {
        [Header("Weather Configuration")]
        [Tooltip("Thời tiết hiện tại / Current weather")]
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        
        [Tooltip("Thời gian chuyển đổi (giây) / Transition time")]
        [SerializeField] private float transitionTime = 5f;
        
        [Tooltip("Tự động thay đổi thời tiết / Auto change weather")]
        [SerializeField] private bool autoChangeWeather = true;
        
        [Tooltip("Thời gian giữa thay đổi (phút) / Time between changes")]
        [SerializeField] private float changeIntervalMinutes = 30f;
        
        [Header("Weather Effects")]
        [Tooltip("Hiệu ứng mưa / Rain effect")]
        [SerializeField] private GameObject rainEffect;
        
        [Tooltip("Hiệu ứng tuyết / Snow effect")]
        [SerializeField] private GameObject snowEffect;
        
        [Tooltip("Hiệu ứng sương mù / Fog effect")]
        [SerializeField] private GameObject fogEffect;
        
        [Tooltip("Hiệu ứng bão cát / Sandstorm effect")]
        [SerializeField] private GameObject sandstormEffect;
        
        [Header("Audio")]
        [Tooltip("Âm thanh mưa / Rain sound")]
        [SerializeField] private AudioClip rainSound;
        
        [Tooltip("Âm thanh gió / Wind sound")]
        [SerializeField] private AudioClip windSound;
        
        [Tooltip("Âm thanh sấm / Thunder sound")]
        [SerializeField] private AudioClip thunderSound;
        
        [Header("Gameplay Effects")]
        [Tooltip("Ảnh hưởng tầm nhìn / Affects visibility")]
        [SerializeField] private bool affectsVisibility = true;
        
        [Tooltip("Ảnh hưởng combat / Affects combat")]
        [SerializeField] private bool affectsCombat = true;
        
        [Tooltip("Giảm tầm nhìn (%) / Visibility reduction")]
        [Range(0f, 1f)]
        [SerializeField] private float visibilityReduction = 0.3f;
        
        private GameObject currentWeatherEffect;
        private AudioSource weatherAudioSource;
        private float nextWeatherChange;
        private WeatherType targetWeather;
        private float transitionProgress = 0f;
        
        // Events
        public delegate void WeatherEventHandler(WeatherType newWeather);
        public event WeatherEventHandler OnWeatherChanged;
        
        private void Start()
        {
            // Create audio source
            weatherAudioSource = gameObject.AddComponent<AudioSource>();
            weatherAudioSource.loop = true;
            weatherAudioSource.spatialBlend = 0f; // 2D sound
            weatherAudioSource.volume = 0.5f;
            
            // Set initial weather
            SetWeather(currentWeather, true);
            
            // Schedule next change
            if (autoChangeWeather)
            {
                nextWeatherChange = Time.time + (changeIntervalMinutes * 60f);
            }
        }
        
        private void Update()
        {
            // Auto change weather
            if (autoChangeWeather && Time.time >= nextWeatherChange)
            {
                ChangeToRandomWeather();
                nextWeatherChange = Time.time + (changeIntervalMinutes * 60f);
            }
            
            // Update transition
            if (transitionProgress < 1f)
            {
                transitionProgress += Time.deltaTime / transitionTime;
                UpdateWeatherTransition();
            }
        }
        
        /// <summary>
        /// Set thời tiết / Set weather
        /// </summary>
        public void SetWeather(WeatherType weather, bool instant = false)
        {
            if (weather == currentWeather && instant)
            {
                return;
            }
            
            targetWeather = weather;
            
            if (instant)
            {
                currentWeather = weather;
                transitionProgress = 1f;
                ApplyWeather(weather);
            }
            else
            {
                transitionProgress = 0f;
            }
            
            Debug.Log($"[Weather] Changing to {weather}");
        }
        
        /// <summary>
        /// Áp dụng thời tiết / Apply weather effects
        /// </summary>
        private void ApplyWeather(WeatherType weather)
        {
            // Remove current effect
            if (currentWeatherEffect != null)
            {
                Destroy(currentWeatherEffect);
                currentWeatherEffect = null;
            }
            
            // Stop audio
            if (weatherAudioSource.isPlaying)
            {
                weatherAudioSource.Stop();
            }
            
            // Apply new weather
            switch (weather)
            {
                case WeatherType.Clear:
                    ApplyClearWeather();
                    break;
                case WeatherType.Rain:
                    ApplyRainWeather();
                    break;
                case WeatherType.Snow:
                    ApplySnowWeather();
                    break;
                case WeatherType.Fog:
                    ApplyFogWeather();
                    break;
                case WeatherType.Sandstorm:
                    ApplySandstormWeather();
                    break;
            }
            
            // Trigger event
            OnWeatherChanged?.Invoke(weather);
            
            Debug.Log($"[Weather] Applied {weather} weather");
        }
        
        /// <summary>
        /// Cập nhật chuyển đổi / Update weather transition
        /// </summary>
        private void UpdateWeatherTransition()
        {
            if (transitionProgress >= 1f)
            {
                currentWeather = targetWeather;
                ApplyWeather(currentWeather);
            }
        }
        
        /// <summary>
        /// Áp dụng thời tiết quang / Apply clear weather
        /// </summary>
        private void ApplyClearWeather()
        {
            RenderSettings.fogDensity = 0f;
            RenderSettings.fog = false;
        }
        
        /// <summary>
        /// Áp dụng mưa / Apply rain weather
        /// </summary>
        private void ApplyRainWeather()
        {
            if (rainEffect != null)
            {
                currentWeatherEffect = Instantiate(rainEffect, transform.position, Quaternion.identity, transform);
            }
            
            if (rainSound != null)
            {
                weatherAudioSource.clip = rainSound;
                weatherAudioSource.Play();
            }
            
            // Light fog
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.01f;
            RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.5f);
            
            // TODO: Apply rain gameplay effects
        }
        
        /// <summary>
        /// Áp dụng tuyết / Apply snow weather
        /// </summary>
        private void ApplySnowWeather()
        {
            if (snowEffect != null)
            {
                currentWeatherEffect = Instantiate(snowEffect, transform.position, Quaternion.identity, transform);
            }
            
            if (windSound != null)
            {
                weatherAudioSource.clip = windSound;
                weatherAudioSource.Play();
            }
            
            // Light fog
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.015f;
            RenderSettings.fogColor = new Color(0.8f, 0.8f, 0.9f);
        }
        
        /// <summary>
        /// Áp dụng sương mù / Apply fog weather
        /// </summary>
        private void ApplyFogWeather()
        {
            if (fogEffect != null)
            {
                currentWeatherEffect = Instantiate(fogEffect, transform.position, Quaternion.identity, transform);
            }
            
            // Heavy fog
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.05f;
            RenderSettings.fogColor = new Color(0.7f, 0.7f, 0.7f);
            
            // Reduce visibility
            if (affectsVisibility)
            {
                Camera.main.farClipPlane *= (1f - visibilityReduction);
            }
        }
        
        /// <summary>
        /// Áp dụng bão cát / Apply sandstorm weather
        /// </summary>
        private void ApplySandstormWeather()
        {
            if (sandstormEffect != null)
            {
                currentWeatherEffect = Instantiate(sandstormEffect, transform.position, Quaternion.identity, transform);
            }
            
            if (windSound != null)
            {
                weatherAudioSource.clip = windSound;
                weatherAudioSource.volume = 0.7f;
                weatherAudioSource.Play();
            }
            
            // Heavy fog with yellow/brown tint
            RenderSettings.fog = true;
            RenderSettings.fogDensity = 0.08f;
            RenderSettings.fogColor = new Color(0.8f, 0.7f, 0.5f);
            
            // Severe visibility reduction
            if (affectsVisibility)
            {
                Camera.main.farClipPlane *= (1f - visibilityReduction * 1.5f);
            }
        }
        
        /// <summary>
        /// Thay đổi thời tiết random / Change to random weather
        /// </summary>
        private void ChangeToRandomWeather()
        {
            WeatherType[] allWeathers = System.Enum.GetValues(typeof(WeatherType)) as WeatherType[];
            WeatherType newWeather = allWeathers[Random.Range(0, allWeathers.Length)];
            
            // Don't change to same weather
            if (newWeather == currentWeather)
            {
                newWeather = (WeatherType)(((int)currentWeather + 1) % allWeathers.Length);
            }
            
            SetWeather(newWeather, false);
        }
        
        /// <summary>
        /// Lấy thời tiết hiện tại / Get current weather
        /// </summary>
        public WeatherType GetCurrentWeather()
        {
            return currentWeather;
        }
        
        /// <summary>
        /// Lấy combat modifier / Get combat modifier
        /// </summary>
        public float GetCombatModifier(string damageType)
        {
            if (!affectsCombat)
            {
                return 1f;
            }
            
            // Rain reduces fire damage
            if (currentWeather == WeatherType.Rain && damageType == "Fire")
            {
                return 0.9f;
            }
            
            // Snow reduces ice damage
            if (currentWeather == WeatherType.Snow && damageType == "Ice")
            {
                return 1.1f;
            }
            
            return 1f;
        }
    }
    
    /// <summary>
    /// Loại thời tiết / Weather types
    /// </summary>
    public enum WeatherType
    {
        Clear,      // Quang đãng
        Rain,       // Mưa
        Snow,       // Tuyết
        Fog,        // Sương mù
        Sandstorm   // Bão cát
    }
}
