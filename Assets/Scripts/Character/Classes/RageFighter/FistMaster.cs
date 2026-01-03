using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Fist Master - Evolution of Rage Fighter
    /// Đại Võ Sĩ - Tiến hóa của Chiến Binh Cuồng Nộ
    /// </summary>
    public class FistMaster : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.FistMaster;
            ClassName = "Fist Master";
            Description = "Ultimate martial artist with godlike speed and power. / Võ sĩ tối thượng với tốc độ và sức mạnh thần thánh.";
            
            BaseStrength = 32;
            BaseAgility = 27;
            BaseVitality = 25;
            BaseEnergy = 20;
            BaseCommand = 0;
            
            // Stat Growth Per Level / Tăng chỉ số mỗi level
            StrengthPerLevel = 6;
            AgilityPerLevel = 3;
            VitalityPerLevel = 4;
            EnergyPerLevel = 2;
            CommandPerLevel = 0;
            
            // Equipment / Trang bị
            AllowedWeapons = new WeaponType[] 
            { 
                WeaponType.Fist,
                WeaponType.Claw
            };
            AllowedArmor = ArmorType.Light;
            
            // Role / Vai trò
            Roles = new CharacterRole[] 
            { 
                CharacterRole.MeleeDPS
            };
            
            // Evolution / Tiến hóa
            EvolutionLevel = 1;
            NextEvolution = null; // Final evolution / Tiến hóa cuối cùng
            
            // Requirements / Yêu cầu
            UnlockLevel = 400;
            RequiresQuest = true;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Fist Master initialized - One with the fist!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Master Combos / Combo bậc thầy
- Ultimate Speed / Tốc độ tối thượng
- Dragon Rage / Cuồng nộ rồng
- Fist Barrage / Mưa đấm
- Unstoppable Force / Sức mạnh không thể chặn";
        }
    }
}
