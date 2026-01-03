using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Fairy Elf base class - Ranged DPS/Support/Healer
    /// Tiên Nữ - Sát thương tầm xa/Hỗ trợ/Hồi máu
    /// </summary>
    public class FairyElf : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.FairyElf;
            ClassName = "Fairy Elf";
            Description = "Agile archer with healing and support abilities. / Cung thủ nhanh nhẹn với khả năng hồi máu và hỗ trợ.";
            
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
            EvolutionLevel = 0;
            NextEvolution = CharacterClassType.MuseElf;
            
            // Requirements / Yêu cầu
            UnlockLevel = 0; // Available from start / Có sẵn từ đầu
            RequiresQuest = false;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Fairy Elf initialized - Grace and power combined!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Party Buffs / Buff nhóm
- Healing / Hồi máu
- High Attack Speed / Tốc độ tấn công cao
- Multi-Shot / Bắn đa mục tiêu";
        }
    }
}
