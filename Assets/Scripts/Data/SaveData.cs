using KahaGameCore.Static;
using System.Collections.Generic;

namespace ProjectBS.Data
{
    public class SaveData
    {
        public string PlayerName = "New Player";
        public PartyData Party = new PartyData();
        public List<OwningCharacterData> Characters = new List<OwningCharacterData>();
        public List<OwningEquipmentData> Equipments = new List<OwningEquipmentData>();
        public List<string> LockedEquipmentUDIDs = new List<string>();
        public List<OwningSkillData> Skills = new List<OwningSkillData>();
        public List<int> ClearedBossStage = new List<int>();
        public int Stamina = 100;
        public int OwnExp = 0;
    }

    public class PartyData
    {
        public string MemberUDID_0 = "";
        public string MemberUDID_1 = "";
        public string MemberUDID_2 = "";
        public string MemberUDID_3 = "";
    }

    public class OwningCharacterData
    {
        public string UDID = "";
        public int AppearanceDataID = 0;
        public string Name = "";
        public int Level = 1;
        public int Exp = 0;
        public int HP = 0;
        public int HPAbilityID = 0;
        public int SP = 100;
        public int Attack = 0;
        public int AttackAbilityID = 0;
        public int Defense = 0;
        public int DefenseAbilityID = 0;
        public int Speed = 0;
        public int SpeedAbilityID = 0;
        public string Equipment_UDID_Head = "";
        public string Equipment_UDID_Body = "";
        public string Equipment_UDID_Hand = "";
        public string Equipment_UDID_Foot = "";
        public int[] Skills = new int[8];

        public string GetOriginName()
        {
            return ContextConverter.Instance.GetContext(GameDataManager.GetGameData<AppearanceData>(AppearanceDataID).NameContextID);
        }

        public UnityEngine.Texture2D GetIcon()
        {
            return UnityEngine.Resources.Load<UnityEngine.Texture2D>(GameDataManager.GetGameData<AppearanceData>(AppearanceDataID).SpriteAssetPath);
        }

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

        public string GetAbilityRankString(string statusType)
        {
            switch (statusType)
            {
                case Keyword.Attack:
                    {
                        return GameDataManager.GetGameData<AbilityData>(AttackAbilityID).RankString;
                    }
                case Keyword.Defense:
                    {
                        return GameDataManager.GetGameData<AbilityData>(DefenseAbilityID).RankString;
                    }
                case Keyword.Speed:
                    {
                        return GameDataManager.GetGameData<AbilityData>(SpeedAbilityID).RankString;

                    }
                case Keyword.HP:
                    {
                        return GameDataManager.GetGameData<AbilityData>(HPAbilityID).RankString;

                    }
                default:
                    throw new System.Exception("[OwningCharacterData][GetAbilityRankString] Invaild statusType=" + statusType);
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

        public int GetRequireExp()
        {
            return GameDataManager.GetGameData<ExpData>(Level).Require;
        }

        public SkillData GetSkill(int slotIndex)
        {
            if(slotIndex >= Skills.Length || slotIndex < 0)
            {
                throw new System.Exception("[OwningCharacterData][GetSkill] Invaild Index=" + slotIndex);
            }

            return GameDataManager.GetGameData<SkillData>(Skills[slotIndex]);
        }

        public void SetSkill(int slotIndex, int skillID)
        {
            if (slotIndex >= Skills.Length || slotIndex < 0)
            {
                throw new System.Exception("[OwningCharacterData][SetSkill] Invaild Index=" + slotIndex);
            }

            Skills[slotIndex] = skillID;
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
        public string UDID = "";
        public int EquipmentSourceID = 0;
        public int Level = 1;
        public int Exp = 0;
        public int HP = 0;
        public int SP = 0;
        public int Attack = 0;
        public int Defense = 0;
        public int Speed = 0;
        public string EffectIDs = "";

        public string GetEquipmentType()
        {
            RawEquipmentData _source = GameDataManager.GetGameData<RawEquipmentData>(EquipmentSourceID);
            return _source.EquipmentType;
        }

        public RawEquipmentData GetSourceData()
        {
            return GameDataManager.GetGameData<RawEquipmentData>(EquipmentSourceID);
        }

        public int GetRequireExp()
        {
            return GameDataManager.GetGameData<ExpData>(Level).Require;
        }
    }

    public class OwningSkillData
    {
        public int SkillSourceID = 0;
        public int Amount = 0;

        public SkillData GetSourceData()
        {
            return GameDataManager.GetGameData<SkillData>(SkillSourceID);
        }
    }
}