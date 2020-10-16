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
        public int Stamina;
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

        public string GetEquipmentUDID(string equipmentType)
        {
            switch(equipmentType)
            {
                case Keyword.Body:
                    return Equipment_UDID_Body;
                case Keyword.Foot:
                    return Equipment_UDID_Foot;
                case Keyword.Hand:
                    return Equipment_UDID_Hand;
                case Keyword.Head:
                    return Equipment_UDID_Head;
                default:
                    throw new System.Exception("[OwningCharacterData][GetEquipmentUDID] Invaild equipmentType=" + equipmentType);
            }
        }

        public int GetTotal(string statusType)
        {
            int _value;
            switch (statusType)
            {
                case Keyword.Attack:
                    {
                        _value = Attack;
                        break;
                    }
                case Keyword.Defense:
                    {
                        _value = Defense;
                        break;
                    }
                case Keyword.Speed:
                    {
                        _value = Speed;
                        break;
                    }
                case Keyword.HP:
                    {
                        _value = HP;
                        break;
                    }
                case Keyword.SP:
                    {
                        _value = SP;
                        break;
                    }
                default:
                    throw new System.Exception("[OwningCharacterData][GetTotal] Invaild statusType=" + statusType);
            }

            _value += GetEquipmentValue(Equipment_UDID_Body, statusType);
            _value += GetEquipmentValue(Equipment_UDID_Foot, statusType);
            _value += GetEquipmentValue(Equipment_UDID_Hand, statusType);
            _value += GetEquipmentValue(Equipment_UDID_Head, statusType);

            return _value;
        }

        public int GetSkill(int slotIndex)
        {
            switch(slotIndex)
            {
                case 0:
                    return SkillSlot_0;
                case 1:
                    return SkillSlot_1;
                case 2:
                    return SkillSlot_2;
                case 3:
                    return SkillSlot_3;
                default:
                    throw new System.Exception("[OwningCharacterData][GetSkill] Invaild Index=" + slotIndex);
            }
        }

        public void SetSkill(int slotIndex, int skillID)
        {
            switch (slotIndex)
            {
                case 0:
                    SkillSlot_0 = skillID;
                    break;
                case 1:
                    SkillSlot_1 = skillID;
                    break;
                case 2:
                    SkillSlot_2 = skillID;
                    break;
                case 3:
                    SkillSlot_3 = skillID;
                    break;
                default:
                    throw new System.Exception("[OwningCharacterData][GetSkill] Invaild Index=" + slotIndex);
            }
        }

        private int GetEquipmentValue(string UDID, string statusType)
        {
            OwningEquipmentData _equipment = PlayerManager.Instance.GetEquipmentByUDID(UDID);

            if (_equipment == null)
                return 0;

            switch (statusType)
            {
                case Keyword.Attack:
                    {
                        return _equipment.Attack;
                    }
                case Keyword.Defense:
                    {
                        return _equipment.Defense;

                    }
                case Keyword.Speed:
                    {
                        return _equipment.Speed;
                    }
                case Keyword.HP:
                    {
                        return _equipment.HP;
                    }
                case Keyword.SP:
                    {
                        return _equipment.SP;
                    }
                default:
                    throw new System.Exception("[OwningCharacterData][GetTotal] Invaild statusType=" + statusType);
            }
        }
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

        public string GetEquipmentType()
        {
            RawEquipmentData _source = KahaGameCore.Static.GameDataManager.GetGameData<RawEquipmentData>(EquipmentSourceID);
            return _source.EquipmentType;
        }
    }

    public class OwningSkillData
    {
        public int SkillSourceID;
        public int Amount;
    }
}