using System;
using KahaGameCore.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUIView : UIView
    {
        private const string PLAYER_INFO_FORMAT = "{0}\nStamina: {1} /  Exp: {2}";
        public override bool IsShowing { get { return m_root.activeSelf; } }

        [Serializable]
        private class PanelData
        {
            public Button downButton = null;
            public MainMenuUI_PanelBase panel = null;
        }

        [SerializeField] private GameObject m_root = null; 
        [Header("Top")]
        [SerializeField] private Text m_playerInfoText = null;
        [Header("Panels")]
        [SerializeField] private PanelData[] m_panelDatas = null;

        private void Update()
        {
            if(m_root.activeSelf)
            {
                m_playerInfoText.text = string.Format(PLAYER_INFO_FORMAT,
                    PlayerManager.Instance.Player.PlayerName,
                    PlayerManager.Instance.Player.Stamina,
                    PlayerManager.Instance.Player.OwnExp);
            }
        }

        public override void ForceShow(Manager manager, bool show)
        {
            throw new NotImplementedException();
        }

        public override void Show(Manager manager, bool show, Action onCompleted)
        {
            if (show)
                DisableAllPanel();
            m_root.SetActive(show);
            onCompleted?.Invoke();
        }

        public void Button_SwitchTo(Button button)
        {
            DisableAllPanel();
            for(int i = 0; i < m_panelDatas.Length; i++)
            {
                if(m_panelDatas[i].downButton == button)
                {
                    m_panelDatas[i].downButton.interactable = false;
                    m_panelDatas[i].panel.Show();
                    return;
                }
            }
        }

        public void Test_Mall()
        {
            DisableAllPanel();
            for (int i = 0; i < 11; i++)
            {
                PlayerManager.Instance.Player.Characters.Add(CharacterUtility.CreateNewCharacter());
            }
            GameManager.Instance.MessageManager.ShowCommonMessage("新增了11隻角色，快去確認看看!", "", null);
            PlayerManager.Instance.SavePlayer();
        }

        private void DisableAllPanel()
        {
            for (int i = 0; i < m_panelDatas.Length; i++)
            {
                m_panelDatas[i].downButton.interactable = true;
                m_panelDatas[i].panel.Hide();
            }
        }
    }
}
