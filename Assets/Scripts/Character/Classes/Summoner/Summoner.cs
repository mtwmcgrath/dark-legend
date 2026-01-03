using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Summoner base class - Summoner/Magic DPS
    /// Triệu Hồi Sư - Triệu hồi/Sát thương ma thuật
    /// </summary>
    public class Summoner : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.Summoner;
            ClassName = "Summoner";
            Description = "Mage who summons creatures and uses curse magic. / Pháp sư triệu hồi sinh vật và dùng ma thuật nguyền rủa.";
            
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
            EvolutionLevel = 0;
            NextEvolution = CharacterClassType.BloodySummoner;
            
            // Requirements / Yêu cầu
            UnlockLevel = 220; // Requires level 220 on any character / Cần level 220 trên nhân vật bất kỳ
            RequiresQuest = false;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Summoner initialized - Call forth the darkness!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Summon Creatures / Triệu hồi sinh vật
- Curse Magic / Ma thuật nguyền rủa
- Sleep Spell / Phép ngủ
- Drain Life / Hút sinh lực
- Debuff Mastery / Tinh thông debuff";
        }
    }
}
