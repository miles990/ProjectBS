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
                // GameDataManager.SaveData(new SaveData[] { Player });
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

            _newPlayer.Characters.Add(
                new OwningCharacterData
                {
                    Attack = 100,
                    CharacterNameID = 0,
                    CharacterSpriteID = 0,
                    Defence = 100,
                    Equipment_UDID_Body = null,
                    Equipment_UDID_Foot = null,
                    Equipment_UDID_Hand = null,
                    Equipment_UDID_Head = null,
                    HP = 1000,
                    Level = 1,
                    SkillSlot_0 = 0,
                    SkillSlot_1 = 0,
                    SKillSlot_2 = 0,
                    SKillSlot_3 = 0,
                    SP = 100,
                    Speed = 10,
                    UDID = "0",
                    UpgradeAbilityIDs = null
                });

            _newPlayer.Characters.Add(
                new OwningCharacterData
                {
                    Attack = 100,
                    CharacterNameID = 0,
                    CharacterSpriteID = 0,
                    Defence = 100,
                    Equipment_UDID_Body = null,
                    Equipment_UDID_Foot = null,
                    Equipment_UDID_Hand = null,
                    Equipment_UDID_Head = null,
                    HP = 1000,
                    Level = 1,
                    SkillSlot_0 = 0,
                    SkillSlot_1 = 0,
                    SKillSlot_2 = 0,
                    SKillSlot_3 = 0,
                    SP = 100,
                    Speed = 10,
                    UDID = "1",
                    UpgradeAbilityIDs = null
                });

            _newPlayer.Characters.Add(
                new OwningCharacterData
                {
                    Attack = 100,
                    CharacterNameID = 0,
                    CharacterSpriteID = 0,
                    Defence = 100,
                    Equipment_UDID_Body = null,
                    Equipment_UDID_Foot = null,
                    Equipment_UDID_Hand = null,
                    Equipment_UDID_Head = null,
                    HP = 1000,
                    Level = 1,
                    SkillSlot_0 = 0,
                    SkillSlot_1 = 0,
                    SKillSlot_2 = 0,
                    SKillSlot_3 = 0,
                    SP = 100,
                    Speed = 10,
                    UDID = "2",
                    UpgradeAbilityIDs = null
                });

            _newPlayer.Characters.Add(
                new OwningCharacterData
                {
                    Attack = 100,
                    CharacterNameID = 0,
                    CharacterSpriteID = 0,
                    Defence = 100,
                    Equipment_UDID_Body = null,
                    Equipment_UDID_Foot = null,
                    Equipment_UDID_Hand = null,
                    Equipment_UDID_Head = null,
                    HP = 1000,
                    Level = 1,
                    SkillSlot_0 = 0,
                    SkillSlot_1 = 0,
                    SKillSlot_2 = 0,
                    SKillSlot_3 = 0,
                    SP = 100,
                    Speed = 10,
                    UDID = "3",
                    UpgradeAbilityIDs = null
                });

            _newPlayer.Party.MemberUDID_0 = "0";
            _newPlayer.Party.MemberUDID_1 = "1";
            _newPlayer.Party.MemberUDID_2 = "2";
            _newPlayer.Party.MemberUDID_3 = "3";

            return _newPlayer;
        }
    }
}

