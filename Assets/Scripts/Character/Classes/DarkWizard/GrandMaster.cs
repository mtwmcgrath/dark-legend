using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Grand Master - 3rd evolution of Dark Wizard
    /// Đại Pháp Sư - Tiến hóa bậc 3 của Pháp Sư Bóng Tối
    /// </summary>
    public class GrandMaster : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.GrandMaster;
            ClassName = "Grand Master";
            Description = "Ultimate mage with god-like magical power. / Pháp sư tối thượng với sức mạnh ma thuật thần thánh.";
            
            BaseStrength = 18;
            BaseAgility = 18;
            BaseVitality = 15;
            BaseEnergy = 30;
            BaseCommand = 0;
            
            // Stat Growth Per Level / Tăng chỉ số mỗi level
            StrengthPerLevel = 1;
            AgilityPerLevel = 2;
            VitalityPerLevel = 1;
            EnergyPerLevel = 6;
            CommandPerLevel = 0;
            
            // Equipment / Trang bị
            AllowedWeapons = new WeaponType[] 
            { 
                WeaponType.Staff, 
                WeaponType.Wand 
            };
            AllowedArmor = ArmorType.Robe;
            
            // Role / Vai trò
            Roles = new CharacterRole[] 
            { 
                CharacterRole.MagicDPS 
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
            Debug.Log("Grand Master initialized - Arcane supremacy!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Master AoE Magic / Ma thuật AoE bậc thầy
- Meteor Storm / Mưa thiên thạch
- Nova / Bùng nổ năng lượng
- Instant Teleport / Dịch chuyển tức thời
- Infinite Mana Mastery / Tinh thông mana vô hạn";
        }
    }
}
