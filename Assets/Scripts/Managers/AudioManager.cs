using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.Managers
{
    /// <summary>
    /// Audio manager for music and sound effects
    /// Quản lý âm thanh cho nhạc và hiệu ứng âm thanh
    /// </summary>
    public class AudioManager : Utils.Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        
        [Header("Volume Settings")]
        [Range(0f, 1f)]
        public float musicVolume = 0.7f;
        [Range(0f, 1f)]
        public float sfxVolume = 0.8f;
        
        [Header("Music Tracks")]
        public AudioClip mainMenuMusic;
        public AudioClip gameplayMusic;
        public AudioClip combatMusic;
        
        [Header("Sound Effects")]
        public AudioClip buttonClick;
        public AudioClip itemPickup;
        public AudioClip levelUp;
        
        // Object pooling for SFX
        private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
        private const int SFX_POOL_SIZE = 10;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            // Initialize SFX pool
            InitializeSFXPool();
            
            // Set initial volumes
            SetMusicVolume(musicVolume);
            SetSFXVolume(sfxVolume);
        }
        
        /// <summary>
        /// Initialize sound effect audio source pool
        /// Khởi tạo pool audio source cho hiệu ứng âm thanh
        /// </summary>
        private void InitializeSFXPool()
        {
            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                GameObject sfxObj = new GameObject($"SFX_{i}");
                sfxObj.transform.SetParent(transform);
                AudioSource source = sfxObj.AddComponent<AudioSource>();
                source.loop = false;
                source.playOnAwake = false;
                sfxPool.Enqueue(source);
            }
        }
        
        /// <summary>
        /// Play music track
        /// Phát nhạc
        /// </summary>
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (musicSource == null || clip == null) return;
            
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        
        /// <summary>
        /// Stop music
        /// Dừng nhạc
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        /// <summary>
        /// Pause music
        /// Tạm dừng nhạc
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }
        
        /// <summary>
        /// Resume music
        /// Tiếp tục nhạc
        /// </summary>
        public void ResumeMusic()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
        }
        
        /// <summary>
        /// Play sound effect
        /// Phát hiệu ứng âm thanh
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null) return;
            
            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
            }
        }
        
        /// <summary>
        /// Play sound effect at position (3D sound)
        /// Phát hiệu ứng âm thanh tại vị trí (âm thanh 3D)
        /// </summary>
        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
        {
            if (clip == null) return;
            
            // Get audio source from pool
            AudioSource source = GetPooledSFXSource();
            if (source != null)
            {
                source.transform.position = position;
                source.clip = clip;
                source.volume = sfxVolume * volumeMultiplier;
                source.spatialBlend = 1f; // 3D sound
                source.Play();
                
                // Return to pool after playing
                StartCoroutine(ReturnToPoolAfterPlay(source, clip.length));
            }
            else
            {
                // Fallback to PlayOneShot
                AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volumeMultiplier);
            }
        }
        
        /// <summary>
        /// Get pooled SFX audio source
        /// Lấy audio source SFX từ pool
        /// </summary>
        private AudioSource GetPooledSFXSource()
        {
            if (sfxPool.Count > 0)
            {
                return sfxPool.Dequeue();
            }
            return null;
        }
        
        /// <summary>
        /// Return audio source to pool after playing
        /// Trả audio source về pool sau khi phát
        /// </summary>
        private System.Collections.IEnumerator ReturnToPoolAfterPlay(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            source.Stop();
            sfxPool.Enqueue(source);
        }
        
        /// <summary>
        /// Set music volume
        /// Đặt âm lượng nhạc
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
            {
                musicSource.volume = musicVolume;
            }
        }
        
        /// <summary>
        /// Set SFX volume
        /// Đặt âm lượng hiệu ứng âm thanh
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
            {
                sfxSource.volume = sfxVolume;
            }
        }
        
        /// <summary>
        /// Mute/unmute all audio
        /// Tắt/bật tất cả âm thanh
        /// </summary>
        public void SetMuted(bool muted)
        {
            AudioListener.volume = muted ? 0f : 1f;
        }
        
        /// <summary>
        /// Play button click sound
        /// Phát âm thanh click nút
        /// </summary>
        public void PlayButtonClick()
        {
            PlaySFX(buttonClick);
        }
        
        /// <summary>
        /// Play item pickup sound
        /// Phát âm thanh nhặt vật phẩm
        /// </summary>
        public void PlayItemPickup()
        {
            PlaySFX(itemPickup);
        }
        
        /// <summary>
        /// Play level up sound
        /// Phát âm thanh lên cấp
        /// </summary>
        public void PlayLevelUp()
        {
            PlaySFX(levelUp);
        }
    }
}
