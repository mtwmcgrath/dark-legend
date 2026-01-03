using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Blade Master - 3rd evolution of Dark Knight
    /// Đại Hiệp Sĩ Kiếm - Tiến hóa bậc 3 của Hiệp Sĩ Bóng Tối
    /// </summary>
    public class BladeMaster : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.BladeMaster;
            ClassName = "Blade Master";
            Description = "Ultimate melee warrior with devastating power. / Chiến binh cận chiến tối thượng với sức mạnh tàn phá.";
            
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
            EvolutionLevel = 2;
            NextEvolution = null; // Final evolution / Tiến hóa cuối cùng
            
            // Requirements / Yêu cầu
            UnlockLevel = 400;
            RequiresQuest = true;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Blade Master initialized - The legend awakens!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Master Combo System / Hệ thống combo bậc thầy
- Rageful Blow / Đòn giận dữ
- Death Stab / Đâm chết chóc
- Impale / Đâm xuyên
- Ultimate Defense / Phòng thủ tối thượng";
        }
    }
}
