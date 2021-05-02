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

            MessageManager = new KahaGameCore.Common.ConfirmWindowManager(GetPage<UI.ConfirmWindowUIView>());
            UnityEngine.Application.logMessageReceived += HandleLog;

            StartInitData();
        }

        public void StartLocalCombat(Data.BossStageData bossStageData)
        {
            if (m_currentState != State.MainMenu)
                throw new System.Exception("[GameManager][StartGame] Can't start comabt now");

            if (PlayerManager.Instance.Player.Stamina < bossStageData.Stamina)
            {
                MessageManager.ShowCommonMessage(
                    ContextConverter.Instance.GetContext(1000033),
                    "Warning",
                    null);
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

            m_localGameCombatManager = new Combat.CombatManager();
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
                _bossParty.Add(Combat.CombatUtility.CreateUnit(GameDataManager.GetGameData<Data.BossData>(int.Parse(_bossIDs[i]))));
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

            if(isWin)
            {
                DropUtility.DropInfo _drop = DropUtility.Drop(m_currentPlayingStage);

                PlayerManager.Instance.Player.OwnExp += _drop.exp;

                if(!PlayerManager.Instance.Player.ClearedBossStage.Contains(m_currentPlayingStage.ID))
                {
                    PlayerManager.Instance.Player.ClearedBossStage.Add(m_currentPlayingStage.ID);
                }
                PlayerManager.Instance.Player.Stamina -= m_currentPlayingStage.Stamina;
                for (int i = 0; i < _drop.skillIDs.Count; i++)
                {
                    PlayerManager.Instance.AddSkill(_drop.skillIDs[i]);
                }

                GetPage<UI.EndUIView>().ShowGameWin(new UI.EndUIView.GameWinInfo
                {
                    exp = _drop.exp,
                    skills = _drop.skillIDs
                }, EndLoaclCombat);
            }
            else
            {
                GetPage<UI.EndUIView>().ShowGameLose(EndLoaclCombat);
            }

            m_currentPlayingStage = null;
            PlayerManager.Instance.SavePlayer();
        }

        private void EndLoaclCombat()
        {
            Combat.CombatUtility.EndCombatProcess(m_localGameCombatManager);
            ShowMainMenu();
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
            IAP.ProductManager.Instance.Initialize();

            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            m_currentState = State.MainMenu;
            GetPage<UI.MainMenuUIView>().Show(this, true, null);
            GetPage<UI.CombatUIView>().Show(this, false, null);
        }

        private void HandleLog(string condition, string stackTrace, UnityEngine.LogType type)
        {
            if (type == UnityEngine.LogType.Log) return;

            MessageManager.ShowCommonMessage(condition, type.ToString(), null);
        }
    }
}
