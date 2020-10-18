﻿using ProjectBS.Data;
using KahaGameCore.Static;
using System.Collections.Generic;

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
                    throw new System.Exception("[PlayerManager][Player get] Need to init Player first");
                }
                return m_player;
            }
        }
        private SaveData m_player = null;

        private bool m_isInited = false;

        public void Init()
        {
            if (m_isInited)
            {
                throw new System.Exception("[PlayerManager][Init] Player is already inited");
            }

            m_player = GameDataManager.LoadJsonData<SaveData>();
            if(m_player == null)
            {
                m_player = CreateNewPlayer();
                SavePlayer();
            }

            m_isInited = true;
        }

        public void SavePlayer()
        {
            GameDataManager.SaveData(new SaveData[] { m_player });
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

        public OwningCharacterData GetEquipedCharacter(string UDID)
        {
            for(int i = 0; i < m_player.Characters.Count; i++)
            {
                if(m_player.Characters[i].Equipment_UDID_Body == UDID
                    || m_player.Characters[i].Equipment_UDID_Foot == UDID
                    || m_player.Characters[i].Equipment_UDID_Hand == UDID
                    || m_player.Characters[i].Equipment_UDID_Head == UDID)
                {
                    return m_player.Characters[i];
                }
            }

            return null;
        }

        public void EquipmentTo(OwningCharacterData characterData, string UDID)
        {
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

        public int GetPartyIndex(OwningCharacterData characterData)
        {
            if (Player.Party.MemberUDID_0 == characterData.UDID)
            {
                return 0;
            }
            else if (Player.Party.MemberUDID_1 == characterData.UDID)
            {
                return 1;
            }
            else if (Player.Party.MemberUDID_2 == characterData.UDID)
            {
                return 2;
            }
            else if (Player.Party.MemberUDID_3 == characterData.UDID)
            {
                return 3;
            }
            else
            {
                return -1;
            }
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
            SaveData _newPlayer = new SaveData
            {
                Characters = new List<OwningCharacterData>(),
                Equipments = new List<OwningEquipmentData>(),
                Party = new PartyData(),
                PlayerName = "New Player",
                Stamina = 100,
                OwnExp = 0,
                ClearedBossStage = new List<int>(),
                Skills = new List<OwningSkillData>()
            };

            for(int i = 0; i < 4; i++)
            {
                OwningCharacterData _newCharacter = CharacterUtility.CreateNewCharacter();
                _newPlayer.Characters.Add(_newCharacter);
            }

            _newPlayer.Party.MemberUDID_0 = _newPlayer.Characters[0].UDID;
            _newPlayer.Party.MemberUDID_1 = _newPlayer.Characters[1].UDID;
            _newPlayer.Party.MemberUDID_2 = _newPlayer.Characters[2].UDID;
            _newPlayer.Party.MemberUDID_3 = _newPlayer.Characters[3].UDID;

            return _newPlayer;
        }
    }
}

