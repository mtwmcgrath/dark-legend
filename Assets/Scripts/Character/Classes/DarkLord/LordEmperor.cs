using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Lord Emperor - Evolution of Dark Lord
    /// Hoàng Đế - Tiến hóa của Chúa Tể Bóng Tối
    /// </summary>
    public class LordEmperor : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.LordEmperor;
            ClassName = "Lord Emperor";
            Description = "Ultimate commander with supreme leadership powers. / Chỉ huy tối thượng với sức mạnh lãnh đạo tuyệt đối.";
            
            BaseStrength = 26;
            BaseAgility = 20;
            BaseVitality = 20;
            BaseEnergy = 15;
            BaseCommand = 25; // Unique stat / Chỉ số độc quyền
            
            // Stat Growth Per Level / Tăng chỉ số mỗi level
            StrengthPerLevel = 4;
            AgilityPerLevel = 2;
            VitalityPerLevel = 2;
            EnergyPerLevel = 2;
            CommandPerLevel = 4; // Unique stat growth / Tăng chỉ số độc quyền
            
            // Equipment / Trang bị
            AllowedWeapons = new WeaponType[] 
            { 
                WeaponType.Scepter,
                WeaponType.Shield
            };
            AllowedArmor = ArmorType.Heavy;
            
            // Role / Vai trò
            Roles = new CharacterRole[] 
            { 
                CharacterRole.Commander,
                CharacterRole.Summoner,
                CharacterRole.Support
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
            Debug.Log("Lord Emperor initialized - Rule the battlefield!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Enhanced Command Power / Sức mạnh chỉ huy nâng cao
- Imperial Mount / Ngựa hoàng gia
- Multiple Pet Control / Điều khiển nhiều pet
- Ultimate Leadership / Lãnh đạo tối thượng
- Army Buffs / Buff toàn quân";
        }
    }
}
