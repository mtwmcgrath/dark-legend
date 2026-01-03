using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Dark Knight base class - Melee DPS/Tank
    /// Hiệp Sĩ Bóng Tối - Sát thương cận chiến/Tank
    /// </summary>
    public class DarkKnight : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.DarkKnight;
            ClassName = "Dark Knight";
            Description = "Melee warrior with high physical damage and defense. / Chiến binh cận chiến với sát thương vật lý và phòng thủ cao.";
            
            BaseStrength = 28;
            BaseAgility = 20;
            BaseVitality = 25;
            BaseEnergy = 10;
            BaseCommand = 0;
            
            // Stat Growth Per Level / Tăng chỉ số mỗi level
            StrengthPerLevel = 5;
            AgilityPerLevel = 2;
            VitalityPerLevel = 3;
            EnergyPerLevel = 1;
            CommandPerLevel = 0;
            
            // Equipment / Trang bị
            AllowedWeapons = new WeaponType[] 
            { 
                WeaponType.Sword, 
                WeaponType.Blade, 
                WeaponType.TwoHandedSword 
            };
            AllowedArmor = ArmorType.Heavy;
            
            // Role / Vai trò
            Roles = new CharacterRole[] 
            { 
                CharacterRole.MeleeDPS, 
                CharacterRole.Tank 
            };
            
            // Evolution / Tiến hóa
            EvolutionLevel = 0;
            NextEvolution = CharacterClassType.BladeKnight;
            
            // Requirements / Yêu cầu
            UnlockLevel = 0; // Available from start / Có sẵn từ đầu
            RequiresQuest = false;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Dark Knight initialized - Ready for battle!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Combo Attack System / Hệ thống tấn công combo
- High Physical Damage / Sát thương vật lý cao
- Tank with Heavy Armor / Tank với giáp nặng
- Shield Mastery / Tinh thông khiên";
        }
    }
}
