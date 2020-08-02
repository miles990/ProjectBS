using System.Collections.Generic;

namespace ProjectBS.Data
{
    public class SaveData
    {
        public string PlayerName;
        public PartyData Party;
        public List<OwningCharacterData> Characters;
        public List<OwningEquipmentData> Equipments;
        public List<OwningItemData> Items;
        public List<OwningSkillData> Skills;
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
        public int Defence;
        public int DefenceAbilityID;
        public int Speed;
        public int SpeedAbilityID;
        public string UpgradeAbilityIDs;
        public string Equipment_UDID_Head;
        public string Equipment_UDID_Body;
        public string Equipment_UDID_Hand;
        public string Equipment_UDID_Foot;
        public int SkillSlot_0;
        public int SkillSlot_1;
        public int SKillSlot_2;
        public int SKillSlot_3;
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
        public int Defence;
        public int Speed;
        public string EffectIDs;
    }

    public class OwningItemData
    {
        public int ItemSourceID;
        public int Amount;
    }

    public class OwningSkillData
    {
        public int SkillSourceID;
        public int Amount;
    }
}