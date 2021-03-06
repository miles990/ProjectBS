using KahaGameCore.Static;
using System.Collections.Generic;

namespace ProjectBS
{
    public class GameManager : KahaGameCore.Interface.Manager
    {
        private const string PLAYER_PREFS_KEY_Tutorial = "GameManager_Tutorial";

        public static GameManager Instance
        {
            get
            {
                if (m_instance == null)
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

        private bool m_isSweaping = false;

        public void StartGame()
        {
            if (m_currentState != State.None)
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
                    ShowAddStamina);
                return;
            }

            m_currentPlayingStage = bossStageData;
            m_currentState = State.Combat;

            if (PlayerManager.Instance.Player.ClearedBossStage.Contains(bossStageData.ID))
            {
                m_isSweaping = true;
                EndCombat(true);
                return;
            }

            m_isSweaping = false;
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

        private void ShowAddStamina()
        {
            TimerManager.Schedule(UnityEngine.Time.fixedDeltaTime, GetPage<UI.MainMenuUIView>().Button_Ad_AddStamina);
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

            if (isWin)
            {
                if (m_isSweaping) UnityEngine.Time.timeScale = 4f;

                DropUtility.DropInfo _drop = DropUtility.Drop(m_currentPlayingStage);

                PlayerManager.Instance.Player.OwnExp += _drop.exp;

                if (!PlayerManager.Instance.Player.ClearedBossStage.Contains(m_currentPlayingStage.ID))
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
            if (!m_isSweaping) ShowMainMenu();
            if (m_isSweaping) UnityEngine.Time.timeScale = 1f;
        }

        private void StartInitData()
        {
            m_currentState = State.InitData;
            InitGameSetting();
            GameDataManager.StartLoad(OnAllDataLoaded);
        }

        private void OnAllDataLoaded()
        {
            CreateGameTimeCounter();
            PlayerManager.Instance.Init();
            IAP.ProductManager.Instance.Initialize();
            ShowMainMenu();
        }

        private void CreateGameTimeCounter()
        {
            UnityEngine.GameObject _gameTimeCounter = new UnityEngine.GameObject("[GameTimeCounter]");
            _gameTimeCounter.AddComponent<GameTimeCounter>();
        }

        private void InitGameSetting()
        {
            string _setting = UnityEngine.Resources.Load<UnityEngine.TextAsset>("GameSetting").text;
            string[] _settingPart = _setting.Split('|');

            GameSetting.dataSource = _settingPart[0];
            GameSetting.version = _settingPart[1];
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
