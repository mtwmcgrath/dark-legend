namespace DarkLegend.Utils
{
    /// <summary>
    /// Game constants and configuration values
    /// Các hằng số và giá trị cấu hình của game
    /// </summary>
    public static class Constants
    {
        // Player Settings
        public const float DEFAULT_MOVE_SPEED = 5f;
        public const float DEFAULT_ROTATION_SPEED = 10f;
        public const float DEFAULT_JUMP_FORCE = 5f;

        // Camera Settings
        public const float CAMERA_MIN_DISTANCE = 3f;
        public const float CAMERA_MAX_DISTANCE = 10f;
        public const float CAMERA_ROTATION_SPEED = 5f;
        public const float CAMERA_ZOOM_SPEED = 2f;

        // Combat Settings
        public const float DEFAULT_ATTACK_RANGE = 2f;
        public const float DEFAULT_ATTACK_COOLDOWN = 1f;
        public const float CRITICAL_HIT_CHANCE = 0.1f; // 10%
        public const float CRITICAL_HIT_MULTIPLIER = 1.5f;

        // Stats Settings
        public const int STARTING_STAT_POINTS = 50;
        public const int STAT_POINTS_PER_LEVEL = 5;
        public const float HP_PER_VITALITY = 15f;
        public const float MP_PER_ENERGY = 10f;
        public const float DAMAGE_PER_STRENGTH = 1.2f;

        // Level System
        public const int MAX_LEVEL = 400;
        public const int BASE_EXP_REQUIREMENT = 100;
        public const float EXP_MULTIPLIER = 1.2f;

        // UI Settings
        public const int INVENTORY_SIZE = 64; // 8x8 grid
        public const int SKILL_BAR_SIZE = 6;
        public const int EQUIPMENT_SLOTS = 7; // Weapon, Helmet, Armor, Gloves, Pants, Boots, Wings

        // Input Keys
        public const string HORIZONTAL_AXIS = "Horizontal";
        public const string VERTICAL_AXIS = "Vertical";
        public const string MOUSE_X = "Mouse X";
        public const string MOUSE_Y = "Mouse Y";

        // Layer Names
        public const string LAYER_PLAYER = "Player";
        public const string LAYER_ENEMY = "Enemy";
        public const string LAYER_GROUND = "Ground";
        public const string LAYER_UI = "UI";

        // Tags
        public const string TAG_PLAYER = "Player";
        public const string TAG_ENEMY = "Enemy";
        public const string TAG_GROUND = "Ground";

        // Animation Parameters
        public const string ANIM_MOVE_SPEED = "MoveSpeed";
        public const string ANIM_IS_ATTACKING = "IsAttacking";
        public const string ANIM_IS_DEAD = "IsDead";
        public const string ANIM_ATTACK_TRIGGER = "Attack";
        public const string ANIM_SKILL_TRIGGER = "Skill";
        public const string ANIM_JUMP_TRIGGER = "Jump";
        public const string ANIM_HIT_TRIGGER = "Hit";

        // Save/Load
        public const string SAVE_FILE_NAME = "darklegend_save.json";
        public const string SAVE_FOLDER = "Saves";

        // Audio
        public const float DEFAULT_MUSIC_VOLUME = 0.7f;
        public const float DEFAULT_SFX_VOLUME = 0.8f;

        // Object Pool
        public const int DEFAULT_POOL_SIZE = 20;
        public const int MAX_POOL_SIZE = 100;
    }
}
