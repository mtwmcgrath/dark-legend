using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Dark Wizard base class - Magic DPS/AoE
    /// Pháp Sư Bóng Tối - Sát thương ma thuật/AoE
    /// </summary>
    public class DarkWizard : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.DarkWizard;
            ClassName = "Dark Wizard";
            Description = "Powerful mage with devastating AoE magic. / Pháp sư mạnh mẽ với ma thuật AoE tàn phá.";
            
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
            EvolutionLevel = 0;
            NextEvolution = CharacterClassType.SoulMaster;
            
            // Requirements / Yêu cầu
            UnlockLevel = 0; // Available from start / Có sẵn từ đầu
            RequiresQuest = false;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Dark Wizard initialized - Harness the dark magic!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Powerful AoE Magic / Ma thuật AoE mạnh mẽ
- Teleport / Dịch chuyển tức thời
- Mana Shield / Khiên mana
- Elemental Mastery / Tinh thông nguyên tố";
        }
    }
}
