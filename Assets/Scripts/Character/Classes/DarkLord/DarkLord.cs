using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Dark Lord base class - Commander/Summoner/Support
    /// Chúa Tể Bóng Tối - Chỉ huy/Triệu hồi/Hỗ trợ
    /// </summary>
    public class DarkLord : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.DarkLord;
            ClassName = "Dark Lord";
            Description = "Commander with mount and pet, focuses on leadership. / Chỉ huy với ngựa và pet, tập trung vào lãnh đạo.";
            
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
            EvolutionLevel = 0;
            NextEvolution = CharacterClassType.LordEmperor;
            
            // Requirements / Yêu cầu
            UnlockLevel = 250; // Requires level 250 on any character / Cần level 250 trên nhân vật bất kỳ
            RequiresQuest = false;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Dark Lord initialized - Command your army!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Command Stat for Pet/Summon / Chỉ số Command cho pet/triệu hồi
- Dark Horse Mount / Ngựa bóng tối (+tốc độ, +tấn công)
- Dark Raven Pet / Quạ bóng tối (tự động tấn công)
- Party Leadership Buffs / Buff lãnh đạo nhóm
- Pet Management / Quản lý pet";
        }
    }
}
