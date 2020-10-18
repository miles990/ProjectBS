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

        [Serializable]
        private class PanelData
        {
            public PanelType panelType = PanelType.Party;
            public Button topButton = null;
            public GameObject[] panelObjs = null;
        }

        [SerializeField] private GameObject m_characterPanelRoot = null;
        [SerializeField] private PanelData[] m_panelDatas = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel m_characterInfoPanel = null;
        [Header("Buttons")]
        [SerializeField] private Button m_nextPageButton = null;
        [SerializeField] private Button m_previousButton = null;
        [SerializeField] private MainMenuUI_CharacterButton[] m_characterButtons = null;
        [SerializeField] private MainMenuUI_EquipmentButton[] m_equipmentButtons = null;
        [SerializeField] private MainMenuUI_SkillButton[] m_skillButtons = null;

        private PanelType m_currentPanelType = PanelType.Party;
        private int m_currentPage = 0;

        private void Start()
        {
            for(int i = 0; i < m_characterButtons.Length; i++)
            {
                m_characterButtons[i].OnButtonPressed += OnCharacterButtonPressed;
            }
            m_characterInfoPanel.OnEditEnded += RefreshCharacterPageButtonState;
            for(int i = 0; i < m_equipmentButtons.Length; i++)
            {
                m_equipmentButtons[i].OnEdited += RefrshEquipmentPageButtonState;
            }
        }

        private void OnEnable()
        {
            m_characterInfoPanel.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            DisableAllPanel();
            m_characterInfoPanel.gameObject.SetActive(false);
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
            Button_Select(m_panelDatas[0].topButton);
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
            m_currentPanelType = panelType;
            switch (panelType)
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
                        RefreshCharacterPageButtonState();
                        break;
                    }
                case PanelType.Equipment:
                    {
                        m_currentPage = 0;
                        RefreshPageButtonState();
                        break;
                    }
                case PanelType.Skill:
                    {
                        m_currentPage = 0;
                        RefreshSkillPageButtonState();
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
            switch (m_currentPanelType)
            {
                case PanelType.Party:
                case PanelType.AllCharacter:
                    {
                        RefreshCharacterPageButtonState();
                        break;
                    }
                case PanelType.Equipment:
                    {
                        RefrshEquipmentPageButtonState();
                        break;
                    }
                case PanelType.Skill:
                    {
                        RefreshSkillPageButtonState();
                        break;
                    }
                default:
                    throw new Exception("[MainMenuUI_CharacterPanel][RefreshPageButtonState] Invaild m_currentPanelType=" + m_currentPanelType);
            }
        }

        private void RefrshEquipmentPageButtonState()
        {
            m_previousButton.interactable = m_currentPage != 0;
            m_nextPageButton.interactable = m_equipmentButtons.Length * (m_currentPage + 1) < PlayerManager.Instance.Player.Equipments.Count;
            for (int i = 0; i < m_equipmentButtons.Length; i++)
            {
                if (i + m_equipmentButtons.Length * m_currentPage >= PlayerManager.Instance.Player.Equipments.Count)
                {
                    m_equipmentButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    m_equipmentButtons[i].SetUp(PlayerManager.Instance.Player.Equipments[i + m_equipmentButtons.Length * m_currentPage]);
                    m_equipmentButtons[i].gameObject.SetActive(true);
                }
            }
        }

        private void RefreshCharacterPageButtonState()
        {
            if(m_currentPanelType == PanelType.Party)
            {
                for (int i = 0; i < m_characterButtons.Length; i++)
                {
                    m_characterButtons[i].SetUp(PlayerManager.Instance.GetCharacterByPartyIndex(i));
                    m_characterButtons[i].gameObject.SetActive(true);
                }
            }
            else
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
        }

        private void RefreshSkillPageButtonState()
        {
            m_previousButton.interactable = m_currentPage != 0;
            m_nextPageButton.interactable = m_skillButtons.Length * (m_currentPage + 1) < PlayerManager.Instance.Player.Skills.Count;
            for (int i = 0; i < m_skillButtons.Length; i++)
            {
                if (i + m_skillButtons.Length * m_currentPage >= PlayerManager.Instance.Player.Skills.Count)
                {
                    m_skillButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    m_skillButtons[i].SetUp(PlayerManager.Instance.Player.Skills[i + m_skillButtons.Length * m_currentPage]);
                    m_skillButtons[i].gameObject.SetActive(true);
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
