using ProjectBS.Data;
using KahaGameCore.Static;
using System.Collections.Generic;
using System;

namespace ProjectBS
{
    /// <summary>
    /// Must be VARY careful while editing Player
    /// </summary>
    public class PlayerManager
    {
        public static PlayerManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new PlayerManager();
                }
                return m_instance;
            }
        }
        private static PlayerManager m_instance = null;

        private PlayerManager() { }

        public SaveData Player 
        { 
            get
            {
                if(!m_isInited)
                {
                    throw new Exception("[PlayerManager][Player get] Need to init Player first");
                }
                return m_player;
            }
        }
        private SaveData m_player = null;
        private int m_passedGameTime = 0;

        private bool m_isInited = false;

        public void Init()
        {
            if (m_isInited)
            {
                throw new System.Exception("[PlayerManager][Init] Player is already inited");
            }

            m_player = KahaGameCore.Static.GameDataManager.LoadJsonData<SaveData>();
            if (m_player == null)
            {
                m_player = CreateNewPlayer();
                SavePlayer();
            }

            GameTimeCounter.Instance.OnOneSecPassed += OnOneSecPassed;
            m_isInited = true;
        }

        public void SavePlayer()
        {
            KahaGameCore.Static.GameDataManager.SaveData(new SaveData[] { m_player });
        }

        public OwningCharacterData GetCharacterByUDID(string UDID)
        {
            if (!m_isInited)
            {
                throw new System.Exception("[PlayerManager][GetEquipmentByUDID] Need to init Player first");
            }

            return Player.Characters.Find(x => x.UDID == UDID);
        }

        public OwningEquipmentData GetEquipmentByUDID(string UDID)
        {
            if (!m_isInited)
            {
                throw new System.Exception("[PlayerManager][GetEquipmentByUDID] Need to init Player first");
            }

            return Player.Equipments.Find(x => x.UDID == UDID);
        }

        public List<OwningEquipmentData> GetEquipmentsByType(string type)
        {
            List<OwningEquipmentData> _equipments = new List<OwningEquipmentData>();
            for (int i = 0; i < m_player.Equipments.Count; i++)
            {
                RawEquipmentData _source = GameDataManager.GetGameData<RawEquipmentData>(m_player.Equipments[i].EquipmentSourceID);
                if (_source.EquipmentType == type)
                {
                    _equipments.Add(m_player.Equipments[i]);
                }
            }

            return _equipments;
        }

        public OwningCharacterData GetEquipedCharacter(string equipmentUDID)
        {
            for(int i = 0; i < m_player.Characters.Count; i++)
            {
                if(m_player.Characters[i].Equipment_UDID_Body == equipmentUDID
                    || m_player.Characters[i].Equipment_UDID_Foot == equipmentUDID
                    || m_player.Characters[i].Equipment_UDID_Hand == equipmentUDID
                    || m_player.Characters[i].Equipment_UDID_Head == equipmentUDID)
                {
                    return m_player.Characters[i];
                }
            }

            return null;
        }

        public void SetSkill(OwningCharacterData character, int targetSlotIndex, int skillID)
        {
            OwningSkillData _owingSkillData = m_player.Skills.Find(x => x.SkillSourceID == skillID);
            if (_owingSkillData != null)
            {
                if(character == null)
                    throw new System.Exception("[PlayerManager][SetSkill] Target character is null");

                character.SetSkill(targetSlotIndex, skillID);

                _owingSkillData.Amount--;
                if (_owingSkillData.Amount <= 0)
                {
                    m_player.Skills.Remove(_owingSkillData);
                }
                SavePlayer();
            }
            else
                throw new System.Exception("[PlayerManager][SetSkill] Is not having Skill " + skillID + " but still trying to set it");
        }

        public void EquipmentTo(OwningCharacterData characterData, string UDID)
        {
            RemoveEquipmentFromAllCharacter(UDID);
            string _equipmentType = GetEquipmentByUDID(UDID).GetEquipmentType();
            switch(_equipmentType)
            {
                case Keyword.Body:
                    {
                        characterData.Equipment_UDID_Body = UDID;
                        break;
                    }
                case Keyword.Foot:
                    {
                        characterData.Equipment_UDID_Foot = UDID;
                        break;
                    }
                case Keyword.Hand:
                    {
                        characterData.Equipment_UDID_Hand = UDID;
                        break;
                    }
                case Keyword.Head:
                    {
                        characterData.Equipment_UDID_Head = UDID;
                        break;
                    }
                default:
                    throw new System.Exception("[PlayerManager][EquipmentTo] Invaild EquipmentType=" + _equipmentType);
            }
        }

        public bool RemoveEquipmentFromAllCharacter(string equipmentUDID)
        {
            OwningCharacterData _equipingCharacter = GetEquipedCharacter(equipmentUDID);
            if(_equipingCharacter == null)
            {
                return false;
            }
            else
            {
                if(_equipingCharacter.Equipment_UDID_Body == equipmentUDID)
                {
                    _equipingCharacter.Equipment_UDID_Body = "";
                    return true;
                }

                if (_equipingCharacter.Equipment_UDID_Foot == equipmentUDID)
                {
                    _equipingCharacter.Equipment_UDID_Foot = "";
                    return true;
                }

                if (_equipingCharacter.Equipment_UDID_Hand == equipmentUDID)
                {
                    _equipingCharacter.Equipment_UDID_Hand = "";
                    return true;
                }

                if (_equipingCharacter.Equipment_UDID_Head == equipmentUDID)
                {
                    _equipingCharacter.Equipment_UDID_Head = "";
                    return true;
                }

                return false;
            }
        }

        public int GetPartyIndex(string UDID)
        {
            if (Player.Party.MemberUDID_0 == UDID)
            {
                return 0;
            }
            else if (Player.Party.MemberUDID_1 == UDID)
            {
                return 1;
            }
            else if (Player.Party.MemberUDID_2 == UDID)
            {
                return 2;
            }
            else if (Player.Party.MemberUDID_3 == UDID)
            {
                return 3;
            }
            else
            {
                return -1;
            }
        }

        public int GetPartyIndex(OwningCharacterData characterData)
        {
            return GetPartyIndex(characterData.UDID);
        }

        public void SetToParty(int index, OwningCharacterData characterData)
        {
            if(characterData == null)
                throw new System.Exception("[PlayerManager][SetToParty] Target character is null");

            switch (index)
            {
                case 0:
                    Player.Party.MemberUDID_0 = characterData.UDID;
                    break;
                case 1:
                    Player.Party.MemberUDID_1 = characterData.UDID;
                    break;
                case 2:
                    Player.Party.MemberUDID_2 = characterData.UDID;
                    break;
                case 3:
                    Player.Party.MemberUDID_3 = characterData.UDID;
                    break;
                default:
                    throw new System.Exception("[PlayerManager][SetToParty] Invaild party index=" + index);
            }
        }

        public OwningCharacterData GetCharacterByPartyIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return Player.Characters.Find(x => x.UDID == Player.Party.MemberUDID_0);
                case 1:
                    return Player.Characters.Find(x => x.UDID == Player.Party.MemberUDID_1);
                case 2:
                    return Player.Characters.Find(x => x.UDID == Player.Party.MemberUDID_2);
                case 3:
                    return Player.Characters.Find(x => x.UDID == Player.Party.MemberUDID_3);
                default:
                    throw new System.Exception("[PlayerManager][GetCharacterByPartyIndex] Invaild party index=" + index);
            }
        }

        public void AddSkill(int skillID)
        {
            OwningSkillData _owningData = m_player.Skills.Find(x => x.SkillSourceID == skillID);
            if (_owningData != null)
            {
                _owningData.Amount++;
            }
            else
            {
                m_player.Skills.Add(new OwningSkillData { SkillSourceID = skillID, Amount = 1 });
            }
        }

        private SaveData CreateNewPlayer()
        {
            SaveData _newPlayer = new SaveData();

            // tank
            _newPlayer.Characters.Add(new OwningCharacterData
            {
                AttackAbilityID = 15,
                CharacterNameID = CharacterUtility.GetRandomName(),
                CharacterSpriteID = 0,
                DefenseAbilityID = 24,
                HPAbilityID = 4,
                Exp = 0,
                Level = 0,
                SkillSlot_0 = 1,
                SkillSlot_1 = 2,
                SkillSlot_2 = 101,
                SkillSlot_3 = 108,
                SpeedAbilityID = 36,
                SP = 100,
                UDID = System.Guid.NewGuid().ToString()
            });
            CharacterUtility.SetLevel(_newPlayer.Characters[_newPlayer.Characters.Count - 1], 1);

            // healer
            _newPlayer.Characters.Add(new OwningCharacterData
            {
                AttackAbilityID = 16,
                CharacterNameID = CharacterUtility.GetRandomName(),
                CharacterSpriteID = 0,
                DefenseAbilityID = 24,
                HPAbilityID = 4,
                Exp = 0,
                Level = 0,
                SkillSlot_0 = 1,
                SkillSlot_1 = 2,
                SkillSlot_2 = 127,
                SkillSlot_3 = 119,
                SpeedAbilityID = 35,
                SP = 100,
                UDID = System.Guid.NewGuid().ToString()
            });
            CharacterUtility.SetLevel(_newPlayer.Characters[_newPlayer.Characters.Count - 1], 1);

            // DPS 0
            _newPlayer.Characters.Add(new OwningCharacterData
            {
                AttackAbilityID = 14,
                CharacterNameID = CharacterUtility.GetRandomName(),
                CharacterSpriteID = 0,
                DefenseAbilityID = 26,
                HPAbilityID = 5,
                Exp = 0,
                Level = 0,
                SkillSlot_0 = 1,
                SkillSlot_1 = 2,
                SkillSlot_2 = 129,
                SkillSlot_3 = 106,
                SpeedAbilityID = 34,
                SP = 100,
                UDID = System.Guid.NewGuid().ToString()
            });
            CharacterUtility.SetLevel(_newPlayer.Characters[_newPlayer.Characters.Count - 1], 1);

            // DPS 1
            _newPlayer.Characters.Add(new OwningCharacterData
            {
                AttackAbilityID = 14,
                CharacterNameID = CharacterUtility.GetRandomName(),
                CharacterSpriteID = 0,
                DefenseAbilityID = 24,
                HPAbilityID = 6,
                Exp = 0,
                Level = 0,
                SkillSlot_0 = 1,
                SkillSlot_1 = 2,
                SkillSlot_2 = 104,
                SkillSlot_3 = 125,
                SpeedAbilityID = 35,
                SP = 100,
                UDID = System.Guid.NewGuid().ToString()
            });
            CharacterUtility.SetLevel(_newPlayer.Characters[_newPlayer.Characters.Count - 1], 1);

            _newPlayer.Party.MemberUDID_0 = _newPlayer.Characters[0].UDID;
            _newPlayer.Party.MemberUDID_1 = _newPlayer.Characters[1].UDID;
            _newPlayer.Party.MemberUDID_2 = _newPlayer.Characters[2].UDID;
            _newPlayer.Party.MemberUDID_3 = _newPlayer.Characters[3].UDID;

            return _newPlayer;
        }

        private void OnOneSecPassed()
        {
            m_passedGameTime += 1;
            if(m_passedGameTime >= GameDataManager.GameProperties.AddStaminaPerSec)
            {
                m_passedGameTime = 0;
                m_player.Stamina += GameDataManager.GameProperties.AddStaminaPerTime;
                if(m_player.Stamina > GameDataManager.GameProperties.MaxStamina)
                {
                    m_player.Stamina = GameDataManager.GameProperties.MaxStamina;
                }
                SavePlayer();
            }
        }
    }
}

