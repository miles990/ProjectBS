using KahaGameCore.Static;
using System.Collections.Generic;

namespace ProjectBS
{
    public class GameManager : KahaGameCore.Interface.Manager
    {
        public static GameManager Instance 
        {
            get
            { 
                if(m_instance == null)
                {
                    m_instance = new GameManager();
                }

                return m_instance;
            }
        }
        private static GameManager m_instance = null;
        private GameManager() { }

        private enum State
        {
            None,
            InitData,
            MainMenu,
            Combat
        }

        public KahaGameCore.Common.ConfirmWindowManager MessageManager { get; private set; }

        public bool IsCombating { get { return m_currentState == State.Combat; } }
        private State m_currentState = State.None;

        private Combat.CombatManager m_localGameCombatManager = null;
        private Data.BossStageData m_currentPlayingStage = null;

        public void StartGame()
        {
            if(m_currentState != State.None)
            {
                throw new System.Exception("[GameManager][StartGame] Game is already started");
            }

            StartInitData();
        }

        public void StartLocalCombat(Data.BossStageData bossStageData)
        {
            if (m_currentState != State.MainMenu)
                throw new System.Exception("[GameManager][StartGame] Can't start comabt now");

            if (PlayerManager.Instance.Player.Stamina < bossStageData.Stamina)
            {
                return;
            }

            m_currentPlayingStage = bossStageData;
            m_currentState = State.Combat;

            if (PlayerManager.Instance.Player.ClearedBossStage.Contains(bossStageData.ID))
            {
                EndCombat(true);
                return;
            }

            GetPage<UI.MainMenuUIView>().Show(this, false, null);

            if(m_localGameCombatManager == null) m_localGameCombatManager = new Combat.CombatManager();
            Combat.CombatUtility.SetCombatManager(m_localGameCombatManager);

            List<Combat.CombatUnit> _playerParty = new List<Combat.CombatUnit>
            {
                Combat.CombatUtility.CreateUnit(PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_0), 0),
                Combat.CombatUtility.CreateUnit(PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_1), 0),
                Combat.CombatUtility.CreateUnit(PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_2), 0),
                Combat.CombatUtility.CreateUnit(PlayerManager.Instance.GetCharacterByUDID(PlayerManager.Instance.Player.Party.MemberUDID_3), 0)
            };
            List<Combat.CombatUnit> _bossParty = new List<Combat.CombatUnit>();
            string[] _bossIDs = bossStageData.BossIDs.RemoveBlankCharacters().Split(';');
            for (int i = 0; i < _bossIDs.Length; i++)
            {
                if (string.IsNullOrEmpty(_bossIDs[i]))
                {
                    continue;
                }
                _bossParty.Add(Combat.CombatUtility.GetUnit(GameDataManager.GetGameData<Data.BossData>(int.Parse(_bossIDs[i]))));
            }

            m_localGameCombatManager.StartCombat(_playerParty, _bossParty);
        }

        public void StartOnlineCombat()
        {
            GetPage<UI.MainMenuUIView>().Show(this, false, null);
            Network.PhotonManager.Instance.ConnectToLobby();
        }

        public void EndCombat(bool isWin)
        {
            if (m_currentState != State.Combat)
                throw new System.Exception("[GameManager][EndCombat] Can't end comabt since it didn't start");

            m_currentState = State.MainMenu;
            GetPage<UI.MainMenuUIView>().Show(this, true, null);

            if(isWin)
            {
                DropUtility.DropInfo _drop = DropUtility.Drop(m_currentPlayingStage);

                // TODO: should set UI here, use _resultString for testing now
                string _resultString = "Add Exp: " + _drop.exp + "\n\nAdd Equipments:\n";

                PlayerManager.Instance.Player.OwnExp += _drop.exp;
                for (int i = 0; i < _drop.equipments.Count; i++)
                {
                    Data.RawEquipmentData _source = _drop.equipments[i].GetSourceData();
                    _resultString += ContextConverter.Instance.GetContext(_source.NameContextID);
                    PlayerManager.Instance.Player.Equipments.Add(_drop.equipments[i]);

                    if (i != _drop.equipments.Count - 1)
                        _resultString += ", ";
                    else
                        _resultString += "\n\nAdd Skills:\n";
                }

                if(!PlayerManager.Instance.Player.ClearedBossStage.Contains(m_currentPlayingStage.ID))
                {
                    PlayerManager.Instance.Player.ClearedBossStage.Add(m_currentPlayingStage.ID);
                }
                PlayerManager.Instance.Player.Stamina -= m_currentPlayingStage.Stamina;
                for (int i = 0; i < _drop.skillIDs.Count; i++)
                {
                    Data.SkillData _source = GameDataManager.GetGameData<Data.SkillData>(_drop.skillIDs[i]);
                    _resultString += ContextConverter.Instance.GetContext(_source.NameContextID);

                    PlayerManager.Instance.AddSkill(_drop.skillIDs[i]);

                    if (i != _drop.skillIDs.Count - 1)
                        _resultString += ", ";
                }
                MessageManager.ShowCommonMessage(_resultString, "Victory", null);
            }
            else
            {
                MessageManager.ShowCommonMessage("", "Lose.....", null);
            }

            m_currentPlayingStage = null;
            Combat.CombatUtility.EndCombatProcess(m_localGameCombatManager);

            PlayerManager.Instance.SavePlayer();
        }

        private void StartInitData()
        {
            m_currentState = State.InitData;
            GameDataManager.StartLoad(OnAllDataLoaded);
        }

        private void OnAllDataLoaded()
        {
            UnityEngine.GameObject _gameTimeCounter = new UnityEngine.GameObject("[GameTimeCounter]");
            _gameTimeCounter.AddComponent<GameTimeCounter>();

            PlayerManager.Instance.Init();
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            m_currentState = State.MainMenu;
            MessageManager = new KahaGameCore.Common.ConfirmWindowManager(GetPage<UI.ConfirmWindowUIView>());
            GetPage<UI.MainMenuUIView>().Show(this, true, null);
        }
    }
}
