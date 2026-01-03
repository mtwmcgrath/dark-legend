using UnityEngine;
using System.Collections.Generic;

namespace DarkLegend.InputSystem
{
    /// <summary>
    /// Key binding configuration
    /// Cấu hình phím bấm
    /// </summary>
    [System.Serializable]
    public class KeyBinding
    {
        public string actionName;
        public KeyCode primaryKey;
        public KeyCode alternateKey;
        
        public bool IsPressed()
        {
            return Input.GetKey(primaryKey) || (alternateKey != KeyCode.None && Input.GetKey(alternateKey));
        }
        
        public bool WasPressed()
        {
            return Input.GetKeyDown(primaryKey) || (alternateKey != KeyCode.None && Input.GetKeyDown(alternateKey));
        }
        
        public bool WasReleased()
        {
            return Input.GetKeyUp(primaryKey) || (alternateKey != KeyCode.None && Input.GetKeyUp(alternateKey));
        }
    }
    
    /// <summary>
    /// Key bindings manager
    /// Quản lý cấu hình phím
    /// </summary>
    [CreateAssetMenu(fileName = "KeyBindings", menuName = "Dark Legend/Key Bindings")]
    public class KeyBindings : ScriptableObject
    {
        [Header("Movement")]
        public KeyBinding moveForward = new KeyBinding { actionName = "MoveForward", primaryKey = KeyCode.W, alternateKey = KeyCode.UpArrow };
        public KeyBinding moveBackward = new KeyBinding { actionName = "MoveBackward", primaryKey = KeyCode.S, alternateKey = KeyCode.DownArrow };
        public KeyBinding moveLeft = new KeyBinding { actionName = "MoveLeft", primaryKey = KeyCode.A, alternateKey = KeyCode.LeftArrow };
        public KeyBinding moveRight = new KeyBinding { actionName = "MoveRight", primaryKey = KeyCode.D, alternateKey = KeyCode.RightArrow };
        public KeyBinding jump = new KeyBinding { actionName = "Jump", primaryKey = KeyCode.Space, alternateKey = KeyCode.None };
        
        [Header("Combat")]
        public KeyBinding attack = new KeyBinding { actionName = "Attack", primaryKey = KeyCode.Mouse0, alternateKey = KeyCode.None };
        public KeyBinding skill1 = new KeyBinding { actionName = "Skill1", primaryKey = KeyCode.Alpha1, alternateKey = KeyCode.None };
        public KeyBinding skill2 = new KeyBinding { actionName = "Skill2", primaryKey = KeyCode.Alpha2, alternateKey = KeyCode.None };
        public KeyBinding skill3 = new KeyBinding { actionName = "Skill3", primaryKey = KeyCode.Alpha3, alternateKey = KeyCode.None };
        public KeyBinding skill4 = new KeyBinding { actionName = "Skill4", primaryKey = KeyCode.Alpha4, alternateKey = KeyCode.None };
        public KeyBinding skill5 = new KeyBinding { actionName = "Skill5", primaryKey = KeyCode.Alpha5, alternateKey = KeyCode.None };
        public KeyBinding skill6 = new KeyBinding { actionName = "Skill6", primaryKey = KeyCode.Alpha6, alternateKey = KeyCode.None };
        
        [Header("UI")]
        public KeyBinding inventory = new KeyBinding { actionName = "Inventory", primaryKey = KeyCode.Tab, alternateKey = KeyCode.I };
        public KeyBinding characterInfo = new KeyBinding { actionName = "CharacterInfo", primaryKey = KeyCode.C, alternateKey = KeyCode.None };
        public KeyBinding map = new KeyBinding { actionName = "Map", primaryKey = KeyCode.M, alternateKey = KeyCode.None };
        public KeyBinding pause = new KeyBinding { actionName = "Pause", primaryKey = KeyCode.Escape, alternateKey = KeyCode.P };
        
        /// <summary>
        /// Get key binding by action name
        /// Lấy key binding theo tên hành động
        /// </summary>
        public KeyBinding GetBinding(string actionName)
        {
            // Use reflection to find the binding
            var field = GetType().GetField(actionName.ToLower());
            if (field != null)
            {
                return field.GetValue(this) as KeyBinding;
            }
            return null;
        }
        
        /// <summary>
        /// Set key binding
        /// Đặt key binding
        /// </summary>
        public void SetBinding(string actionName, KeyCode primaryKey, KeyCode alternateKey = KeyCode.None)
        {
            var field = GetType().GetField(actionName.ToLower());
            if (field != null)
            {
                KeyBinding binding = field.GetValue(this) as KeyBinding;
                if (binding != null)
                {
                    binding.primaryKey = primaryKey;
                    binding.alternateKey = alternateKey;
                }
            }
        }
    }
}
