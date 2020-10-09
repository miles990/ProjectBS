using ProjectBS.Data;
using KahaGameCore.Static;

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

        private SaveData CreateNewPlayer()
        {
            SaveData _newPlayer = new SaveData
            {
                Characters = new System.Collections.Generic.List<OwningCharacterData>(),
                Equipments = new System.Collections.Generic.List<OwningEquipmentData>(),
                Party = new PartyData(),
                PlayerName = "New Player",
                Skills = new System.Collections.Generic.List<OwningSkillData>()
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

