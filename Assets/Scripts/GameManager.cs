using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public static void StartGame()
        {
            if(m_currentState != State.None)
            {
                throw new System.Exception("[GameManager][StartGame] Game is already started");
            }

            StartInitData();
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
            m_uiManager = new UI.MainMenuUIManager();
            m_uiManager.Show(UI.MainMenuUIManager.UIPage.EditPatyUI);
        }
    }
}
