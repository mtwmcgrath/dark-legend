using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Muse Elf - 2nd evolution of Fairy Elf
    /// Nữ Thần Âm Nhạc - Tiến hóa bậc 2 của Tiên Nữ
    /// </summary>
    public class MuseElf : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.MuseElf;
            ClassName = "Muse Elf";
            Description = "Enhanced archer with powerful support magic. / Cung thủ nâng cao với ma thuật hỗ trợ mạnh mẽ.";
            
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
            EvolutionLevel = 1;
            NextEvolution = CharacterClassType.HighElf;
            
            // Requirements / Yêu cầu
            UnlockLevel = 150;
            RequiresQuest = true;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Muse Elf initialized - Harmony in battle!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Enhanced Party Buffs / Buff nhóm nâng cao
- Greater Healing / Hồi máu mạnh hơn
- Penetrating Shot / Bắn xuyên thấu
- Triple Shot / Bắn ba mũi tên";
        }
    }
}
