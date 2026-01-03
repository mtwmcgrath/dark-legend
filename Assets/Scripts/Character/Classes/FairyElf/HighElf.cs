using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// High Elf - 3rd evolution of Fairy Elf
    /// Tiên Nữ Cấp Cao - Tiến hóa bậc 3 của Tiên Nữ
    /// </summary>
    public class HighElf : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.HighElf;
            ClassName = "High Elf";
            Description = "Ultimate archer with divine support powers. / Cung thủ tối thượng với sức mạnh hỗ trợ thần thánh.";
            
            BaseStrength = 22;
            BaseAgility = 25;
            BaseVitality = 15;
            BaseEnergy = 20;
            BaseCommand = 0;
            
            // Stat Growth Per Level / Tăng chỉ số mỗi level
            StrengthPerLevel = 1;
            AgilityPerLevel = 4;
            VitalityPerLevel = 1;
            EnergyPerLevel = 3;
            CommandPerLevel = 0;
            
            // Equipment / Trang bị
            AllowedWeapons = new WeaponType[] 
            { 
                WeaponType.Bow, 
                WeaponType.Crossbow 
            };
            AllowedArmor = ArmorType.Light;
            
            // Role / Vai trò
            Roles = new CharacterRole[] 
            { 
                CharacterRole.RangedDPS, 
                CharacterRole.Support, 
                CharacterRole.Healer 
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
            Debug.Log("High Elf initialized - Celestial archer supreme!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Master Party Buffs / Buff nhóm bậc thầy
- Divine Healing / Hồi máu thần thánh
- Multi Arrow / Mưa tên
- Summon Fairy / Triệu hồi tiên
- Ultimate Support / Hỗ trợ tối thượng";
        }
    }
}
