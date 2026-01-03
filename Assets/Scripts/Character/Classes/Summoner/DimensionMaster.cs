using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Dimension Master - 3rd evolution of Summoner
    /// Chủ Nhân Chiều Không - Tiến hóa bậc 3 của Triệu Hồi Sư
    /// </summary>
    public class DimensionMaster : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.DimensionMaster;
            ClassName = "Dimension Master";
            Description = "Ultimate summoner with dimensional magic. / Triệu hồi sư tối thượng với ma thuật chiều không.";
            
            BaseStrength = 21;
            BaseAgility = 21;
            BaseVitality = 18;
            BaseEnergy = 23;
            BaseCommand = 0;
            
            // Stat Growth Per Level / Tăng chỉ số mỗi level
            StrengthPerLevel = 2;
            AgilityPerLevel = 2;
            VitalityPerLevel = 1;
            EnergyPerLevel = 4;
            CommandPerLevel = 0;
            
            // Equipment / Trang bị
            AllowedWeapons = new WeaponType[] 
            { 
                WeaponType.Book,
                WeaponType.Stick
            };
            AllowedArmor = ArmorType.Robe;
            
            // Role / Vai trò
            Roles = new CharacterRole[] 
            { 
                CharacterRole.Summoner,
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
            Debug.Log("Dimension Master initialized - Bend reality itself!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Master Summons / Triệu hồi bậc thầy
- Dimensional Rift / Khe nứt không gian
- Multiple Summons / Triệu hồi đa thể
- Reality Warp / Bóp méo thực tại
- Ultimate Curse / Nguyền rủa tối thượng";
        }
    }
}
