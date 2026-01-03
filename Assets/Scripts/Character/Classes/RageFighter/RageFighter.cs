using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Rage Fighter base class - Melee DPS/Fast Attacker
    /// Chiến Binh Cuồng Nộ - Sát thương cận chiến/Tấn công nhanh
    /// </summary>
    public class RageFighter : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.RageFighter;
            ClassName = "Rage Fighter";
            Description = "Martial artist with devastating fast attacks. / Võ sĩ với tấn công nhanh tàn phá.";
            
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
            EvolutionLevel = 0;
            NextEvolution = CharacterClassType.FistMaster;
            
            // Requirements / Yêu cầu
            UnlockLevel = 220; // Requires level 220 on any character / Cần level 220 trên nhân vật bất kỳ
            RequiresQuest = false;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Rage Fighter initialized - Feel the fury!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Martial Arts Combos / Combo võ thuật
- Very Fast Attacks / Tấn công cực nhanh
- Chain Skills / Kỹ năng liên hoàn
- Rage Mode / Chế độ cuồng nộ
- Critical Strike Mastery / Tinh thông đòn chí mạng";
        }
    }
}
