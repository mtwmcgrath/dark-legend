using UnityEngine;

namespace DarkLegend.Character
{
    /// <summary>
    /// Magic Gladiator base class - Hybrid Melee/Magic
    /// Võ Sĩ Ma Thuật - Lai giữa cận chiến và ma thuật
    /// </summary>
    public class MagicGladiator : CharacterClass
    {
        private void Awake()
        {
            // Base Stats / Chỉ số cơ bản
            ClassType = CharacterClassType.MagicGladiator;
            ClassName = "Magic Gladiator";
            Description = "Hybrid warrior who combines sword and magic. / Chiến binh lai kết hợp kiếm và ma thuật.";
            
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
            EvolutionLevel = 0;
            NextEvolution = CharacterClassType.DuelMaster;
            
            // Requirements / Yêu cầu
            UnlockLevel = 220; // Requires level 220 on any character / Cần level 220 trên nhân vật bất kỳ
            RequiresQuest = false;
        }
        
        public override void Initialize()
        {
            base.Initialize();
            Debug.Log("Magic Gladiator initialized - Sword and sorcery!");
        }
        
        public override string GetSpecialAbilities()
        {
            return @"Special Abilities / Kỹ năng đặc biệt:
- Use Both Sword and Magic / Dùng cả kiếm và ma thuật
- No Shield Required / Không cần khiên
- Power Slash / Chém sức mạnh
- Magic Combo / Combo ma thuật
- Hybrid Damage / Sát thương lai";
        }
    }
}
