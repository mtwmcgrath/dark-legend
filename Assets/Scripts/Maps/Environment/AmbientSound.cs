using UnityEngine;

namespace DarkLegend.Maps.Environment
{
    /// <summary>
    /// Âm thanh môi trường / Ambient sound system
    /// Plays environmental audio based on map and time
    /// </summary>
    public class AmbientSound : MonoBehaviour
    {
        [Header("Audio Configuration")]
        [Tooltip("Âm thanh ban ngày / Day ambient sound")]
        [SerializeField] private AudioClip dayAmbient;
        
        [Tooltip("Âm thanh ban đêm / Night ambient sound")]
        [SerializeField] private AudioClip nightAmbient;
        
        [Tooltip("Volume / Audio volume")]
        [Range(0f, 1f)]
        [SerializeField] private float volume = 0.5f;
        
        [Tooltip("Fade time (giây) / Fade time")]
        [SerializeField] private float fadeTime = 2f;
        
        [Header("Special Ambients")]
        [Tooltip("Âm thanh trong dungeon / Dungeon ambient")]
        [SerializeField] private AudioClip dungeonAmbient;
        
        [Tooltip("Âm thanh trong boss zone / Boss zone ambient")]
        [SerializeField] private AudioClip bossZoneAmbient;
        
        [Tooltip("Âm thanh trong town / Town ambient")]
        [SerializeField] private AudioClip townAmbient;
        
        private AudioSource audioSource;
        private AudioClip currentClip;
        private bool isFading = false;
        private float targetVolume;
        
        private void Start()
        {
            // Create audio source
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.spatialBlend = 0f; // 2D sound
            audioSource.volume = volume;
            
            // Start playing
            PlayDayAmbient();
        }
        
        private void Update()
        {
            // Handle volume fading
            if (isFading)
            {
                float step = Time.deltaTime / fadeTime;
                audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, step);
                
                if (Mathf.Approximately(audioSource.volume, targetVolume))
                {
                    isFading = false;
                }
            }
        }
        
        /// <summary>
        /// Play ambient ban ngày / Play day ambient
        /// </summary>
        public void PlayDayAmbient()
        {
            if (dayAmbient != null)
            {
                ChangeAmbient(dayAmbient);
            }
        }
        
        /// <summary>
        /// Play ambient ban đêm / Play night ambient
        /// </summary>
        public void PlayNightAmbient()
        {
            if (nightAmbient != null)
            {
                ChangeAmbient(nightAmbient);
            }
        }
        
        /// <summary>
        /// Play ambient dungeon / Play dungeon ambient
        /// </summary>
        public void PlayDungeonAmbient()
        {
            if (dungeonAmbient != null)
            {
                ChangeAmbient(dungeonAmbient);
            }
        }
        
        /// <summary>
        /// Play ambient boss zone / Play boss zone ambient
        /// </summary>
        public void PlayBossZoneAmbient()
        {
            if (bossZoneAmbient != null)
            {
                ChangeAmbient(bossZoneAmbient);
            }
        }
        
        /// <summary>
        /// Play ambient town / Play town ambient
        /// </summary>
        public void PlayTownAmbient()
        {
            if (townAmbient != null)
            {
                ChangeAmbient(townAmbient);
            }
        }
        
        /// <summary>
        /// Thay đổi ambient / Change ambient sound
        /// </summary>
        private void ChangeAmbient(AudioClip newClip)
        {
            if (newClip == currentClip)
            {
                return;
            }
            
            currentClip = newClip;
            
            // Fade out current
            FadeOut(() =>
            {
                // Change clip
                audioSource.clip = newClip;
                audioSource.Play();
                
                // Fade in new
                FadeIn();
            });
        }
        
        /// <summary>
        /// Fade out / Fade out volume
        /// </summary>
        private void FadeOut(System.Action onComplete = null)
        {
            targetVolume = 0f;
            isFading = true;
            
            if (onComplete != null)
            {
                Invoke(nameof(ExecuteCallback), fadeTime);
                callbackAction = onComplete;
            }
        }
        
        /// <summary>
        /// Fade in / Fade in volume
        /// </summary>
        private void FadeIn()
        {
            targetVolume = volume;
            isFading = true;
        }
        
        private System.Action callbackAction;
        
        private void ExecuteCallback()
        {
            callbackAction?.Invoke();
            callbackAction = null;
        }
        
        /// <summary>
        /// Set volume / Set volume
        /// </summary>
        public void SetVolume(float newVolume)
        {
            volume = Mathf.Clamp01(newVolume);
            if (!isFading)
            {
                audioSource.volume = volume;
            }
        }
        
        /// <summary>
        /// Stop ambient / Stop ambient sound
        /// </summary>
        public void StopAmbient()
        {
            FadeOut(() =>
            {
                audioSource.Stop();
            });
        }
    }
}
