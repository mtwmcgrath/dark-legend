using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Bloody Summoner - 2nd evolution of Summoner
    /// Triệu Hồi Sư Máu - Tiến hóa bậc 2 của Triệu Hồi Sư
    /// </summary>
    public class BloodySummoner : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.BloodySummoner;
            ClassName = "Bloody Summoner";
            Description = "Enhanced summoner with blood magic. / Triệu hồi sư nâng cao với ma thuật máu.";
            
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
            EvolutionLevel = 1;
            NextEvolution = CharacterClassType.DimensionMaster;
            
            // Requirements / Yêu cầu
            UnlockLevel = 150;
            RequiresQuest = true;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Bloody Summoner initialized - Blood and darkness!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Enhanced Summons / Triệu hồi nâng cao
- Blood Magic / Ma thuật máu
- Stronger Curses / Nguyền rủa mạnh hơn
- Life Absorption / Hấp thụ sinh lực";
        }
    }
}
