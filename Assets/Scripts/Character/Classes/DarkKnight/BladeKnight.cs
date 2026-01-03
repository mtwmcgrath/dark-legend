using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Blade Knight - 2nd evolution of Dark Knight
    /// Hiệp Sĩ Kiếm - Tiến hóa bậc 2 của Hiệp Sĩ Bóng Tối
    /// </summary>
    public class BladeKnight : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.BladeKnight;
            ClassName = "Blade Knight";
            Description = "Enhanced melee warrior with stronger combos. / Chiến binh cận chiến nâng cao với combo mạnh hơn.";
            
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
            EvolutionLevel = 1;
            NextEvolution = CharacterClassType.BladeMaster;
            
            // Requirements / Yêu cầu
            UnlockLevel = 150;
            RequiresQuest = true;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Blade Knight initialized - Master of the blade!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Enhanced Combo System / Hệ thống combo nâng cao
- Cyclone Strike / Tấn công xoáy
- Whirlwind Slash / Chém lốc xoáy
- Greater Defense / Phòng thủ mạnh hơn";
        }
    }
}
