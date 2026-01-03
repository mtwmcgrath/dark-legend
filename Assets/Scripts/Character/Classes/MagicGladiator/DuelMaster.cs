using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Duel Master - Evolution of Magic Gladiator
    /// Đại Võ Sĩ - Tiến hóa của Võ Sĩ Ma Thuật
    /// </summary>
    public class DuelMaster : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.DuelMaster;
            ClassName = "Duel Master";
            Description = "Ultimate hybrid warrior with mastery over blade and magic. / Chiến binh lai tối thượng tinh thông kiếm và ma thuật.";
            
            BaseStrength = 26;
            BaseAgility = 26;
            BaseVitality = 26;
            BaseEnergy = 16;
            BaseCommand = 0;
            
            // Stat Growth Per Level / Tăng chỉ số mỗi level
            StrengthPerLevel = 5;
            AgilityPerLevel = 3;
            VitalityPerLevel = 3;
            EnergyPerLevel = 2;
            CommandPerLevel = 0;
            
            // Equipment / Trang bị
            AllowedWeapons = new WeaponType[] 
            { 
                WeaponType.Sword,
                WeaponType.Blade,
                WeaponType.TwoHandedSword
            };
            AllowedArmor = ArmorType.Medium;
            
            // Role / Vai trò
            Roles = new CharacterRole[] 
            { 
                CharacterRole.Hybrid,
                CharacterRole.MeleeDPS,
                CharacterRole.MagicDPS
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
            Debug.Log("Duel Master initialized - Master of both worlds!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Enhanced Hybrid Combat / Chiến đấu lai nâng cao
- Chain Lightning / Sét dây chuyền
- Spiral Slash / Chém xoắn ốc
- Magic Mastery / Tinh thông ma thuật
- Perfect Duality / Song hành hoàn hảo";
        }
    }
}
