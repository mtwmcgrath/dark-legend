using UnityEngine;

namespace DarkLegend.InputSystem
{
    /// <summary>
    /// Input manager for keyboard and mouse
    /// Quản lý input cho bàn phím và chuột
    /// </summary>
    public class InputManager : Utils.Singleton<InputManager>
    {
        [Header("Key Bindings")]
        public KeyBindings keyBindings;
        
        [Header("Mouse Settings")]
        public float mouseSensitivity = 1f;
        public bool invertYAxis = false;
        
        [Header("Input State")]
        public bool inputEnabled = true;
        
        // Movement input
        private Vector2 movementInput;
        private bool jumpPressed;
        
        // Mouse input
        private Vector2 mouseInput;
        private bool leftMousePressed;
        private bool rightMousePressed;
        
        // UI input
        private bool inventoryPressed;
        private bool characterInfoPressed;
        private bool mapPressed;
        private bool pausePressed;
        
        protected override void Awake()
        {
            base.Awake();
            
            // Load default key bindings if not assigned
            if (keyBindings == null)
            {
                keyBindings = Resources.Load<KeyBindings>("KeyBindings");
            }
        }
        
        private void Update()
        {
            if (!inputEnabled) return;
            
            UpdateMovementInput();
            UpdateMouseInput();
            UpdateUIInput();
        }
        
        /// <summary>
        /// Update movement input
        /// Cập nhật input di chuyển
        /// </summary>
        private void UpdateMovementInput()
        {
            float horizontal = 0f;
            float vertical = 0f;
            
            if (keyBindings != null)
            {
                if (keyBindings.moveForward.IsPressed()) vertical += 1f;
                if (keyBindings.moveBackward.IsPressed()) vertical -= 1f;
                if (keyBindings.moveLeft.IsPressed()) horizontal -= 1f;
                if (keyBindings.moveRight.IsPressed()) horizontal += 1f;
                
                jumpPressed = keyBindings.jump.WasPressed();
            }
            else
            {
                // Fallback to default input
                horizontal = Input.GetAxisRaw(Utils.Constants.HORIZONTAL_AXIS);
                vertical = Input.GetAxisRaw(Utils.Constants.VERTICAL_AXIS);
                jumpPressed = Input.GetButtonDown("Jump");
            }
            
            movementInput = new Vector2(horizontal, vertical).normalized;
        }
        
        /// <summary>
        /// Update mouse input
        /// Cập nhật input chuột
        /// </summary>
        private void UpdateMouseInput()
        {
            float mouseX = Input.GetAxis(Utils.Constants.MOUSE_X) * mouseSensitivity;
            float mouseY = Input.GetAxis(Utils.Constants.MOUSE_Y) * mouseSensitivity;
            
            if (invertYAxis)
            {
                mouseY = -mouseY;
            }
            
            mouseInput = new Vector2(mouseX, mouseY);
            
            leftMousePressed = Input.GetMouseButtonDown(0);
            rightMousePressed = Input.GetMouseButtonDown(1);
        }
        
        /// <summary>
        /// Update UI input
        /// Cập nhật input UI
        /// </summary>
        private void UpdateUIInput()
        {
            if (keyBindings != null)
            {
                inventoryPressed = keyBindings.inventory.WasPressed();
                characterInfoPressed = keyBindings.characterInfo.WasPressed();
                mapPressed = keyBindings.map.WasPressed();
                pausePressed = keyBindings.pause.WasPressed();
            }
            else
            {
                // Fallback
                inventoryPressed = Input.GetKeyDown(KeyCode.Tab);
                characterInfoPressed = Input.GetKeyDown(KeyCode.C);
                mapPressed = Input.GetKeyDown(KeyCode.M);
                pausePressed = Input.GetKeyDown(KeyCode.Escape);
            }
        }
        
        // Public getters
        public Vector2 GetMovementInput() => inputEnabled ? movementInput : Vector2.zero;
        public bool GetJumpPressed() => inputEnabled && jumpPressed;
        public Vector2 GetMouseInput() => inputEnabled ? mouseInput : Vector2.zero;
        public bool GetLeftMousePressed() => inputEnabled && leftMousePressed;
        public bool GetRightMousePressed() => inputEnabled && rightMousePressed;
        public bool GetInventoryPressed() => inputEnabled && inventoryPressed;
        public bool GetCharacterInfoPressed() => inputEnabled && characterInfoPressed;
        public bool GetMapPressed() => inputEnabled && mapPressed;
        public bool GetPausePressed() => inputEnabled && pausePressed;
        
        /// <summary>
        /// Enable/disable input
        /// Bật/tắt input
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
        }
        
        /// <summary>
        /// Check if skill key was pressed
        /// Kiểm tra phím skill có được nhấn không
        /// </summary>
        public bool GetSkillPressed(int skillIndex)
        {
            if (!inputEnabled || keyBindings == null) return false;
            
            switch (skillIndex)
            {
                case 0: return keyBindings.skill1.WasPressed();
                case 1: return keyBindings.skill2.WasPressed();
                case 2: return keyBindings.skill3.WasPressed();
                case 3: return keyBindings.skill4.WasPressed();
                case 4: return keyBindings.skill5.WasPressed();
                case 5: return keyBindings.skill6.WasPressed();
                default: return false;
            }
        }
        
        /// <summary>
        /// Get mouse position in world
        /// Lấy vị trí chuột trong thế giới
        /// </summary>
        public Vector3 GetMouseWorldPosition(LayerMask layerMask)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                return hit.point;
            }
            
            return Vector3.zero;
        }
    }
}
