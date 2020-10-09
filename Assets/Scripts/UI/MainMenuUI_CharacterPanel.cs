using System;
using ProjectBS.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterPanel : MainMenuUI_PanelBase
    {
        private enum PanelType
        {
            Party,
            AllCharacter,
            Equipment,
            Skill
        }

        [System.Serializable]
        private class PanelData
        {
            public PanelType panelType = PanelType.Party;
            public Button topButton = null;
            public GameObject[] panelObjs = null;
        }

        [SerializeField] private GameObject m_characterPanelRoot = null;
        [SerializeField] private PanelData[] m_panelDatas = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel m_characterInfoPanel = null;
        [SerializeField] private Text m_characterInfoText = null;
        [Header("Buttons")]
        [SerializeField] private Button m_nextPageButton = null;
        [SerializeField] private Button m_previousButton = null;
        [SerializeField] private MainMenuUI_CharacterButton[] m_characterButtons = null;
        [SerializeField] private MainMenuUI_EquipmentButton[] m_equipmentButtons = null;
        [SerializeField] private MainMenuUI_SkillButton[] m_skillButtons = null;

        private int m_currentPage = 0;

        private void Start()
        {
            for(int i = 0; i < m_characterButtons.Length; i++)
            {
                m_characterButtons[i].OnButtonPressed += OnCharacterButtonPressed;
            }
        }

        private void OnCharacterButtonPressed(OwningCharacterData obj)
        {
            m_characterInfoPanel.Enable(obj);
        }

        public override void Hide()
        {
            m_characterPanelRoot.SetActive(false);
        }

        public override void Show()
        {
            DisableAllPanel();
            m_characterPanelRoot.SetActive(true);
        }

        public void Button_Select(Button button)
        {
            DisableAllPanel();
            for (int i = 0; i < m_panelDatas.Length; i++)
            {
                if(m_panelDatas[i].topButton == button)
                {
                    UpdateAllButtonData(m_panelDatas[i].panelType);
                    m_panelDatas[i].topButton.interactable = false;
                    for (int j = 0; j < m_panelDatas[i].panelObjs.Length; j++)
                    {
                        m_panelDatas[i].panelObjs[j].SetActive(true);
                    }
                    return;
                }
            }
        }

        private void UpdateAllButtonData(PanelType panelType)
        {
            switch(panelType)
            {
                case PanelType.Party:
                    {
                        for(int i = 0; i < m_characterButtons.Length; i++)
                        {
                            m_characterButtons[i].SetUp(PlayerManager.Instance.GetCharacterByPartyIndex(i));
                            m_characterButtons[i].gameObject.SetActive(true);
                        }
                        break;
                    }
                case PanelType.AllCharacter:
                    {
                        m_currentPage = 0;
                        RefreshPageButtonState();
                        break;
                    }
            }
        }

        public void Button_GoNextPage()
        {
            m_currentPage++;
            RefreshPageButtonState();
        }

        public void Button_GoPreviousPage()
        {
            m_currentPage--;
            if (m_currentPage < 0) m_currentPage = 0;
            RefreshPageButtonState();
        }

        private void RefreshPageButtonState()
        {
            m_previousButton.interactable = m_currentPage != 0;
            m_nextPageButton.interactable = m_characterButtons.Length * (m_currentPage + 1) < PlayerManager.Instance.Player.Characters.Count;
            for (int i = 0; i < m_characterButtons.Length; i++)
            {
                if (i + m_characterButtons.Length * m_currentPage >= PlayerManager.Instance.Player.Characters.Count)
                {
                    m_characterButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    m_characterButtons[i].SetUp(PlayerManager.Instance.Player.Characters[i + m_characterButtons.Length * m_currentPage]);
                    m_characterButtons[i].gameObject.SetActive(true);
                }
            }
        }

        private void DisableAllPanel()
        {
            for (int i = 0; i < m_panelDatas.Length; i++)
            {
                m_panelDatas[i].topButton.interactable = true;
                for(int j = 0; j < m_panelDatas[i].panelObjs.Length; j++)
                {
                    m_panelDatas[i].panelObjs[j].SetActive(false);
                }
            }
        }
    }
}
