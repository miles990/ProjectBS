﻿using ProjectBS.Data;
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

            _newPlayer.Characters.Add(CharacterManager.Instance.CreateNewCharacter());
            _newPlayer.Characters.Add(CharacterManager.Instance.CreateNewCharacter());
            _newPlayer.Characters.Add(CharacterManager.Instance.CreateNewCharacter());
            _newPlayer.Characters.Add(CharacterManager.Instance.CreateNewCharacter());

            _newPlayer.Party.MemberUDID_0 = _newPlayer.Characters[0].UDID;
            _newPlayer.Party.MemberUDID_1 = _newPlayer.Characters[1].UDID;
            _newPlayer.Party.MemberUDID_2 = _newPlayer.Characters[2].UDID;
            _newPlayer.Party.MemberUDID_3 = _newPlayer.Characters[3].UDID;

            // TEST
            CharacterManager.Instance.LevelUp(_newPlayer.Characters[0], 100);
            CharacterManager.Instance.LevelUp(_newPlayer.Characters[1], 100);
            CharacterManager.Instance.LevelUp(_newPlayer.Characters[2], 100);
            CharacterManager.Instance.LevelUp(_newPlayer.Characters[3], 100);

            return _newPlayer;
        }
    }
}

