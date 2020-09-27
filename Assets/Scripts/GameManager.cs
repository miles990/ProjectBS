using System.Collections.Generic;

namespace ProjectBS
{
    public static class GameManager
    {
        private enum State
        {
            None,
            InitData,
            MainMenu,
            Combat
        }

        private static State m_currentState = State.None;

        private static UI.MainMenuUIManager m_uiManager = null;
        private static Combat.CombatManager m_combatManager = null;

        public static void StartGame()
        {
            if(m_currentState != State.None)
            {
                throw new System.Exception("[GameManager][StartGame] Game is already started");
            }

            StartInitData();
        }

        public static void StartCombat()
        {
            if (m_currentState != State.MainMenu)
                throw new System.Exception("[GameManager][StartGame] Can't start comabt now");

            m_uiManager.Show(UI.MainMenuUIManager.UIPage.None);

            m_combatManager = new Combat.CombatManager();
            m_combatManager.StartCombat(
                PlayerManager.Instance.Player.Party,
                new List<Data.BossData> { KahaGameCore.Static.GameDataManager.GetGameData<Data.BossData>(1) });
        }

        public static void EndCombat()
        {
            if (m_currentState != State.Combat)
                throw new System.Exception("[GameManager][EndCombat] Can't end comabt since it didn't start");

            m_uiManager.Show(UI.MainMenuUIManager.UIPage.EditPatyUI);
        }

        private static void StartInitData()
        {
            m_currentState = State.InitData;
            GameDataLoader.StartLoad();
            PlayerManager.Instance.Init();
            ShowMainMenu();
        }

        private static void ShowMainMenu()
        {
            m_currentState = State.MainMenu;

            m_uiManager = new UI.MainMenuUIManager();
            m_uiManager.Show(UI.MainMenuUIManager.UIPage.EditPatyUI);
        }
    }
}
