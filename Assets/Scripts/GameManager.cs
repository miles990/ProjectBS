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

        private State m_currentState = State.None;

        private Combat.CombatManager m_combatManager = null;

        public void StartGame()
        {
            if(m_currentState != State.None)
            {
                throw new System.Exception("[GameManager][StartGame] Game is already started");
            }

            StartInitData();
        }

        public void StartCombat()
        {
            if (m_currentState != State.MainMenu)
                throw new System.Exception("[GameManager][StartGame] Can't start comabt now");

            m_currentState = State.Combat;
            GetPage<UI.MainMenuUIView>().Show(this, false, null);

            m_combatManager = new Combat.CombatManager();
            m_combatManager.StartCombat(
                PlayerManager.Instance.Player.Party,
                new List<Data.BossData> { KahaGameCore.Static.GameDataManager.GetGameData<Data.BossData>(1) });
        }

        public void EndCombat()
        {
            if (m_currentState != State.Combat)
                throw new System.Exception("[GameManager][EndCombat] Can't end comabt since it didn't start");

            m_currentState = State.MainMenu;
            GetPage<UI.MainMenuUIView>().Show(this, true, null);
        }

        private void StartInitData()
        {
            m_currentState = State.InitData;
            GameDataLoader.StartLoad();
            PlayerManager.Instance.Init();
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            m_currentState = State.MainMenu;
            GetPage<UI.MainMenuUIView>().Show(this, true, null);
        }
    }
}
