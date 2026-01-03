using UnityEngine;

namespace DarkLegend.Maps.NPCs
{
    /// <summary>
    /// Base class cho tất cả NPCs
    /// Base class for all NPCs in the game
    /// </summary>
    public abstract class NPCBase : MonoBehaviour
    {
        [Header("NPC Info")]
        [Tooltip("Tên NPC / NPC name")]
        [SerializeField] protected string npcName = "NPC";
        
        [Tooltip("Tiêu đề / Title")]
        [SerializeField] protected string npcTitle = "";
        
        [Tooltip("Mô tả / Description")]
        [TextArea(2, 4)]
        [SerializeField] protected string description = "";
        
        [Header("Interaction")]
        [Tooltip("Khoảng cách tương tác / Interaction distance")]
        [SerializeField] protected float interactionDistance = 3f;
        
        [Tooltip("Có thể tương tác / Is interactable")]
        [SerializeField] protected bool isInteractable = true;
        
        [Tooltip("Icon trên minimap / Minimap icon")]
        [SerializeField] protected Sprite minimapIcon;
        
        [Header("Dialog")]
        [Tooltip("Lời chào / Greeting message")]
        [TextArea(2, 3)]
        [SerializeField] protected string greetingMessage = "Xin chào!";
        
        [Tooltip("Lời tạm biệt / Farewell message")]
        [SerializeField] protected string farewellMessage = "Hẹn gặp lại!";
        
        [Header("Visual")]
        [Tooltip("Hiển thị tên trên đầu / Show name above")]
        [SerializeField] protected bool showNameAbove = true;
        
        [Tooltip("Màu tên / Name color")]
        [SerializeField] protected Color nameColor = Color.yellow;
        
        protected GameObject currentInteractingPlayer;
        protected bool isInInteraction = false;
        
        protected virtual void Start()
        {
            InitializeNPC();
        }
        
        protected virtual void Update()
        {
            if (!isInteractable) return;
            
            // Check for nearby players
            CheckPlayerProximity();
        }
        
        /// <summary>
        /// Khởi tạo NPC / Initialize NPC
        /// </summary>
        protected virtual void InitializeNPC()
        {
            Debug.Log($"[NPCBase] Initializing NPC: {npcName}");
            
            // Setup name display
            if (showNameAbove)
            {
                CreateNameDisplay();
            }
        }
        
        /// <summary>
        /// Tạo hiển thị tên / Create name display
        /// </summary>
        protected virtual void CreateNameDisplay()
        {
            // TODO: Create UI element above NPC showing name
            Debug.Log($"[NPCBase] Creating name display for {npcName}");
        }
        
        /// <summary>
        /// Kiểm tra player gần / Check player proximity
        /// </summary>
        protected virtual void CheckPlayerProximity()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                
                if (distance <= interactionDistance && !isInInteraction)
                {
                    ShowInteractionPrompt(player);
                }
                else if (distance > interactionDistance && currentInteractingPlayer == player)
                {
                    HideInteractionPrompt();
                }
            }
        }
        
        /// <summary>
        /// Hiển thị prompt tương tác / Show interaction prompt
        /// </summary>
        protected virtual void ShowInteractionPrompt(GameObject player)
        {
            // TODO: Show "Press E to interact" prompt
            Debug.Log($"[NPCBase] Player near {npcName}");
        }
        
        /// <summary>
        /// Ẩn prompt tương tác / Hide interaction prompt
        /// </summary>
        protected virtual void HideInteractionPrompt()
        {
            // TODO: Hide interaction prompt
        }
        
        /// <summary>
        /// Bắt đầu tương tác / Start interaction
        /// </summary>
        public virtual void Interact(GameObject player)
        {
            if (!isInteractable)
            {
                Debug.Log($"[NPCBase] {npcName} is not interactable");
                return;
            }
            
            currentInteractingPlayer = player;
            isInInteraction = true;
            
            Debug.Log($"[NPCBase] Player interacting with {npcName}");
            
            // Show greeting
            ShowDialog(greetingMessage);
            
            // Open NPC UI
            OpenNPCUI(player);
        }
        
        /// <summary>
        /// Kết thúc tương tác / End interaction
        /// </summary>
        public virtual void EndInteraction()
        {
            if (isInInteraction)
            {
                ShowDialog(farewellMessage);
                CloseNPCUI();
                
                currentInteractingPlayer = null;
                isInInteraction = false;
                
                Debug.Log($"[NPCBase] Ended interaction with {npcName}");
            }
        }
        
        /// <summary>
        /// Hiển thị dialog / Show dialog
        /// </summary>
        protected virtual void ShowDialog(string message)
        {
            Debug.Log($"[NPCBase] {npcName}: {message}");
            // TODO: Show dialog UI
        }
        
        /// <summary>
        /// Mở UI của NPC / Open NPC UI
        /// Override in derived classes
        /// </summary>
        protected abstract void OpenNPCUI(GameObject player);
        
        /// <summary>
        /// Đóng UI của NPC / Close NPC UI
        /// </summary>
        protected virtual void CloseNPCUI()
        {
            // TODO: Close NPC UI
            Debug.Log($"[NPCBase] Closing UI for {npcName}");
        }
        
        /// <summary>
        /// Lấy tên NPC / Get NPC name
        /// </summary>
        public string GetNPCName()
        {
            return npcName;
        }
        
        /// <summary>
        /// Lấy mô tả NPC / Get NPC description
        /// </summary>
        public string GetDescription()
        {
            return description;
        }
        
        /// <summary>
        /// Bật/tắt tương tác / Enable/disable interaction
        /// </summary>
        public void SetInteractable(bool value)
        {
            isInteractable = value;
        }
        
        protected virtual void OnDrawGizmos()
        {
            // Draw interaction radius
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
        
        protected virtual void OnDrawGizmosSelected()
        {
            // Draw interaction area when selected
            Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
            Gizmos.DrawSphere(transform.position, interactionDistance);
        }
    }
}
