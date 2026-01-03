using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Soul Master - 2nd evolution of Dark Wizard
    /// Linh Hồn Sư - Tiến hóa bậc 2 của Pháp Sư Bóng Tối
    /// </summary>
    public class SoulMaster : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.SoulMaster;
            ClassName = "Soul Master";
            Description = "Enhanced mage with soul-based magic. / Pháp sư nâng cao với ma thuật linh hồn.";
            
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
            EvolutionLevel = 1;
            NextEvolution = CharacterClassType.GrandMaster;
            
            // Requirements / Yêu cầu
            UnlockLevel = 150;
            RequiresQuest = true;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Soul Master initialized - Command the souls!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Enhanced AoE Magic / Ma thuật AoE nâng cao
- Soul Barrier / Rào cản linh hồn
- Advanced Teleport / Dịch chuyển nâng cao
- Greater Mana Pool / Mana pool lớn hơn";
        }
    }
}
