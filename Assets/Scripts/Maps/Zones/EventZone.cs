using UnityEngine;

namespace DarkLegend.Maps.Zones
{
    /// <summary>
    /// Event zone - Map sự kiện đặc biệt
    /// Special event map zone
    /// </summary>
    public class EventZone : ZoneBase
    {
        [Header("Event Configuration")]
        [Tooltip("Tên sự kiện / Event name")]
        [SerializeField] private string eventName;
        
        [Tooltip("Loại sự kiện / Event type")]
        [SerializeField] private EventType eventType;
        
        [Tooltip("Thời gian sự kiện (phút) / Event duration in minutes")]
        [SerializeField] private int eventDuration = 20;
        
        [Header("Entry Requirements")]
        [Tooltip("Item vào cửa / Entry ticket")]
        [SerializeField] private string entryTicket;
        
        [Tooltip("Level tối thiểu / Minimum level")]
        [SerializeField] private int minEventLevel = 100;
        
        [Tooltip("Số người tối đa / Maximum participants")]
        [SerializeField] private int maxParticipants = 10;
        
        [Header("Event Mechanics")]
        [Tooltip("Số waves / Wave count")]
        [SerializeField] private int waveCount = 5;
        
        [Tooltip("Thời gian giữa waves (giây) / Time between waves")]
        [SerializeField] private float waveCooldown = 30f;
        
        [Tooltip("Boss cuối / Final boss")]
        [SerializeField] private GameObject finalBossPrefab;
        
        [Header("Rewards")]
        [Tooltip("Bonus EXP / Bonus experience")]
        [SerializeField] private int bonusExp = 10000;
        
        [Tooltip("Phần thưởng đặc biệt / Special rewards")]
        [SerializeField] private string[] specialRewards;
        
        private float eventStartTime;
        private int currentWave = 0;
        private bool eventActive = false;
        private int participantCount = 0;
        
        public override void InitializeZone()
        {
            base.InitializeZone();
            
            Debug.Log($"[EventZone] Event zone initialized: {eventName}");
        }
        
        /// <summary>
        /// Bắt đầu sự kiện / Start event
        /// </summary>
        public void StartEvent()
        {
            if (eventActive)
            {
                Debug.Log($"[EventZone] Event already active!");
                return;
            }
            
            eventActive = true;
            eventStartTime = Time.time;
            currentWave = 0;
            
            // Announce event start
            AnnounceEventStart();
            
            // Start first wave
            StartNextWave();
            
            Debug.Log($"[EventZone] Event started: {eventName}");
        }
        
        /// <summary>
        /// Kết thúc sự kiện / End event
        /// </summary>
        public void EndEvent(bool success)
        {
            if (!eventActive)
            {
                return;
            }
            
            eventActive = false;
            
            if (success)
            {
                OnEventSuccess();
            }
            else
            {
                OnEventFailed();
            }
            
            Debug.Log($"[EventZone] Event ended: {eventName} - Success: {success}");
        }
        
        protected override void UpdateZone()
        {
            base.UpdateZone();
            
            if (!eventActive) return;
            
            // Check time limit
            float elapsed = Time.time - eventStartTime;
            float remaining = (eventDuration * 60) - elapsed;
            
            if (remaining <= 0)
            {
                EndEvent(false);
            }
        }
        
        /// <summary>
        /// Bắt đầu wave tiếp theo / Start next wave
        /// </summary>
        private void StartNextWave()
        {
            currentWave++;
            
            if (currentWave > waveCount)
            {
                // Spawn final boss
                SpawnFinalBoss();
                return;
            }
            
            // Announce wave
            AnnounceWave();
            
            // Spawn wave monsters
            SpawnWaveMonsters();
            
            Debug.Log($"[EventZone] Starting wave {currentWave}/{waveCount}");
        }
        
        /// <summary>
        /// Spawn monsters cho wave / Spawn wave monsters
        /// </summary>
        private void SpawnWaveMonsters()
        {
            // TODO: Spawn monsters based on wave number
            int monsterCount = 10 + (currentWave * 5);
            Debug.Log($"[EventZone] Spawning {monsterCount} monsters for wave {currentWave}");
        }
        
        /// <summary>
        /// Spawn boss cuối / Spawn final boss
        /// </summary>
        private void SpawnFinalBoss()
        {
            if (finalBossPrefab != null)
            {
                Vector3 bossPos = GetRandomPositionInZone();
                GameObject boss = Instantiate(finalBossPrefab, bossPos, Quaternion.identity, transform);
                
                Debug.Log($"[EventZone] Final boss spawned!");
                AnnounceFinalBoss();
            }
        }
        
        /// <summary>
        /// Khi wave hoàn thành / When wave completed
        /// </summary>
        public void OnWaveCompleted()
        {
            Debug.Log($"[EventZone] Wave {currentWave} completed!");
            
            // Wait before starting next wave
            Invoke(nameof(StartNextWave), waveCooldown);
        }
        
        /// <summary>
        /// Khi sự kiện thành công / When event succeeds
        /// </summary>
        private void OnEventSuccess()
        {
            // Award rewards
            AwardRewards();
            
            // Announce success
            string announcement = $"🎉 Sự kiện {eventName} hoàn thành thành công! 🎉";
            Debug.Log($"[EventZone] {announcement}");
            // TODO: Server announcement
        }
        
        /// <summary>
        /// Khi sự kiện thất bại / When event fails
        /// </summary>
        private void OnEventFailed()
        {
            string announcement = $"❌ Sự kiện {eventName} thất bại!";
            Debug.Log($"[EventZone] {announcement}");
            // TODO: Server announcement
        }
        
        /// <summary>
        /// Trao thưởng / Award rewards
        /// </summary>
        private void AwardRewards()
        {
            // TODO: Give rewards to participants
            Debug.Log($"[EventZone] Awarding {bonusExp} bonus EXP and special items");
        }
        
        /// <summary>
        /// Thông báo bắt đầu sự kiện / Announce event start
        /// </summary>
        private void AnnounceEventStart()
        {
            string announcement = $"🎮 Sự kiện {eventName} bắt đầu! Thời gian: {eventDuration} phút";
            Debug.Log($"[EventZone] {announcement}");
        }
        
        /// <summary>
        /// Thông báo wave / Announce wave
        /// </summary>
        private void AnnounceWave()
        {
            string announcement = $"⚔️ Wave {currentWave}/{waveCount} đang bắt đầu!";
            Debug.Log($"[EventZone] {announcement}");
        }
        
        /// <summary>
        /// Thông báo boss cuối / Announce final boss
        /// </summary>
        private void AnnounceFinalBoss()
        {
            string announcement = $"🔥 Boss cuối xuất hiện! Đánh bại nó để hoàn thành sự kiện!";
            Debug.Log($"[EventZone] {announcement}");
        }
        
        public override bool CanPlayerEnter(int playerLevel)
        {
            if (playerLevel < minEventLevel)
            {
                Debug.Log($"[EventZone] Level too low: {playerLevel} < {minEventLevel}");
                return false;
            }
            
            if (participantCount >= maxParticipants)
            {
                Debug.Log($"[EventZone] Event full: {participantCount}/{maxParticipants}");
                return false;
            }
            
            // TODO: Check for entry ticket
            
            return base.CanPlayerEnter(playerLevel);
        }
        
        public override void OnPlayerEnter(GameObject player)
        {
            base.OnPlayerEnter(player);
            
            participantCount++;
            Debug.Log($"[EventZone] Participants: {participantCount}/{maxParticipants}");
        }
        
        public override void OnPlayerExit(GameObject player)
        {
            base.OnPlayerExit(player);
            
            participantCount--;
        }
        
        /// <summary>
        /// Lấy thời gian còn lại / Get remaining time
        /// </summary>
        public float GetRemainingTime()
        {
            if (!eventActive) return 0;
            
            float elapsed = Time.time - eventStartTime;
            float remaining = (eventDuration * 60) - elapsed;
            
            return Mathf.Max(0, remaining);
        }
    }
    
    /// <summary>
    /// Loại sự kiện / Event types
    /// </summary>
    public enum EventType
    {
        DevilSquare,    // Quảng trường quỷ
        BloodCastle,    // Lâu đài máu
        ChaosCastle,    // Lâu đài hỗn loạn
        Kalima,         // Chiều không gian
        CustomEvent     // Sự kiện tùy chỉnh
    }
}
