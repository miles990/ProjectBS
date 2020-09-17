using ProjectBS.Data;
using KahaGameCore.Static;

namespace ProjectBS
{
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

        public SaveData Player { get; private set; }

        public void Init()
        {
            Player = GameDataManager.LoadJsonData<SaveData>();
            if(Player == null)
            {
                Player = CreateNewPlayer();
                GameDataManager.SaveData(new SaveData[] { Player });
            }
        }

        public OwningEquipmentData GetEquipmentByUDID(string UDID)
        {
            return Player.Equipments.Find(x => x.UDID == UDID);
        }

        private SaveData CreateNewPlayer()
        {
            SaveData _newPlayer = new SaveData
            {
                Characters = new System.Collections.Generic.List<OwningCharacterData>(),
                Equipments = new System.Collections.Generic.List<OwningEquipmentData>(),
                Items = new System.Collections.Generic.List<OwningItemData>(),
                Party = new PartyData(),
                PlayerName = "New Player",
                Skills = new System.Collections.Generic.List<OwningSkillData>()
            };

            for(int i = 0; i < 4; i++)
            {
                OwningCharacterData _newCharacter = CharacterUtility.CreateNewCharacter();
                CharacterUtility.LevelUp(_newCharacter, 100);
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

