using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Player controller for 3D movement with WASD and mouse
    /// Điều khiển người chơi 3D với WASD và chuột
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float baseMoveSpeed = 5f;
        public float rotationSpeed = 10f;
        public float jumpForce = 5f;
        public float gravity = -9.81f;
        
        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;
        
        [Header("References")]
        public CharacterStats characterStats;
        public Animator animator;
        
        // Components
        private CharacterController controller;
        private Vector3 velocity;
        private bool isGrounded;
        private Vector3 moveDirection;
        
        // Animation hashes
        private static readonly int MoveSpeedHash = Animator.StringToHash(Utils.Constants.ANIM_MOVE_SPEED);
        private static readonly int JumpHash = Animator.StringToHash(Utils.Constants.ANIM_JUMP_TRIGGER);
        private static readonly int IsDeadHash = Animator.StringToHash(Utils.Constants.ANIM_IS_DEAD);
        
        private void Start()
        {
            controller = GetComponent<CharacterController>();
            
            if (characterStats == null)
            {
                characterStats = GetComponent<CharacterStats>();
            }
            
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }
            
            // Create ground check if not assigned
            if (groundCheck == null)
            {
                GameObject groundCheckObj = new GameObject("GroundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = new Vector3(0, -1f, 0);
                groundCheck = groundCheckObj.transform;
            }
            
            // Subscribe to death event
            if (characterStats != null)
            {
                characterStats.OnDeath += OnPlayerDeath;
            }
        }
        
        private void Update()
        {
            if (characterStats != null && characterStats.IsDead)
            {
                return;
            }
            
            HandleMovement();
            HandleJump();
        }
        
        /// <summary>
        /// Handle WASD movement and rotation
        /// Xử lý di chuyển WASD và xoay hướng
        /// </summary>
        private void HandleMovement()
        {
            // Check if grounded
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            
            // Get input
            float horizontal = Input.GetAxisRaw(Utils.Constants.HORIZONTAL_AXIS);
            float vertical = Input.GetAxisRaw(Utils.Constants.VERTICAL_AXIS);
            
            // Calculate move direction relative to camera
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            
            moveDirection = forward * vertical + right * horizontal;
            moveDirection.Normalize();
            
            // Apply movement
            if (moveDirection.magnitude >= 0.1f)
            {
                float currentSpeed = characterStats != null ? characterStats.moveSpeed : baseMoveSpeed;
                controller.Move(moveDirection * currentSpeed * Time.deltaTime);
                
                // Rotate character to face movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                
                // Update animator
                if (animator != null)
                {
                    animator.SetFloat(MoveSpeedHash, moveDirection.magnitude);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetFloat(MoveSpeedHash, 0f);
                }
            }
            
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        
        /// <summary>
        /// Handle jump input
        /// Xử lý input nhảy
        /// </summary>
        private void HandleJump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                
                if (animator != null)
                {
                    animator.SetTrigger(JumpHash);
                }
            }
        }
        
        /// <summary>
        /// Move to specific position (for click-to-move)
        /// Di chuyển đến vị trí cụ thể (cho click-to-move)
        /// </summary>
        public void MoveToPosition(Vector3 targetPosition)
        {
            if (characterStats != null && characterStats.IsDead)
            {
                return;
            }
            
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0f;
            
            if (direction.magnitude >= 0.1f)
            {
                float currentSpeed = characterStats != null ? characterStats.moveSpeed : baseMoveSpeed;
                controller.Move(direction * currentSpeed * Time.deltaTime);
                
                // Rotate to face target
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Enable/disable player control
        /// Bật/tắt điều khiển người chơi
        /// </summary>
        public void SetControlEnabled(bool enabled)
        {
            this.enabled = enabled;
        }
        
        /// <summary>
        /// Handle player death
        /// Xử lý khi người chơi chết
        /// </summary>
        private void OnPlayerDeath()
        {
            if (animator != null)
            {
                animator.SetBool(IsDeadHash, true);
            }
            
            SetControlEnabled(false);
        }
        
        private void OnDestroy()
        {
            if (characterStats != null)
            {
                characterStats.OnDeath -= OnPlayerDeath;
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
            }
        }
    }
}
