using System.Collections.Generic;

namespace ProjectBS.Data
{
    public class SaveData
    {
        public string PlayerName;
        public PartyData Party;
        public List<OwningCharacterData> Characters;
        public List<OwningEquipmentData> Equipments;
        public List<OwningSkillData> Skills;
        public int OwnExp;
    }

    public class PartyData
    {
        public string MemberUDID_0;
        public string MemberUDID_1;
        public string MemberUDID_2;
        public string MemberUDID_3;
    }

    public class OwningCharacterData
    {
        public string UDID;
        public int CharacterNameID;
        public int CharacterSpriteID;
        public int Level;
        public int Exp;
        public int HP;
        public int HPAbilityID;
        public int SP;
        public int Attack;
        public int AttackAbilityID;
        public int Defense;
        public int DefenseAbilityID;
        public int Speed;
        public int SpeedAbilityID;
        public string Equipment_UDID_Head;
        public string Equipment_UDID_Body;
        public string Equipment_UDID_Hand;
        public string Equipment_UDID_Foot;
        public int SkillSlot_0;
        public int SkillSlot_1;
        public int SkillSlot_2;
        public int SkillSlot_3;
    }

    public class OwningEquipmentData
    {
        public string UDID;
        public int EquipmentSourceID;
        public int Level;
        public int Exp;
        public int HP;
        public int SP;
        public int Attack;
        public int Defense;
        public int Speed;
        public string EffectIDs;
    }

    public class OwningSkillData
    {
        public int SkillSourceID;
        public int Amount;
    }
}