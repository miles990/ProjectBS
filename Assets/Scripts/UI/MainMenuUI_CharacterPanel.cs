using System;
using System.Collections.Generic;
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

        [SerializeField] private Michsky.UI.ModernUIPack.WindowManager m_panelManger = null;
        [SerializeField] private RectTransform m_characterButtonContainer = null;
        [SerializeField] private MainMenuUI_CharacterButton[] m_partyCharacterButtons = null;
        [SerializeField] private MainMenuUI_CharacterButton m_characterButtonPrefab = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel m_characterInfoPanel = null;

        private List<MainMenuUI_CharacterButton> m_allClonedCharacterButtons = new List<MainMenuUI_CharacterButton>();

        private PanelType m_currentPanelType = PanelType.Party;

        private void Start()
        {
            for(int i = 0; i < m_partyCharacterButtons.Length; i++)
            {
                m_partyCharacterButtons[i].OnButtonPressed += OnCharacterButtonPressed;
            }
            m_characterInfoPanel.OnEditEnded += RefreshCharacterPageButtonState;

            //for(int i = 0; i < m_equipmentButtons.Length; i++)
            //{
            //    m_equipmentButtons[i].OnEdited += RefrshEquipmentPageButtonState;
            //}

            m_panelManger.OnWindowChanged += OnWindowChanged;
        }

        private void OnWindowChanged(int windowIndex)
        {
            switch(windowIndex)
            {
                case 0:
                    {
                        m_currentPanelType = PanelType.Party;
                        break;
                    }
                case 1:
                    {
                        m_currentPanelType = PanelType.AllCharacter;
                        break;
                    }
                case 2:
                    {
                        m_currentPanelType = PanelType.Equipment;
                        break;
                    }
                case 3:
                    {
                        m_currentPanelType = PanelType.Skill;
                        break;
                    }
            }

            UpdateAllButtonData();
        }

        private void OnEnable()
        {
            m_characterInfoPanel.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            m_characterInfoPanel.Hide();
        }

        private void OnCharacterButtonPressed(OwningCharacterData obj)
        {
            m_characterInfoPanel.Enable(obj);
        }

        protected override void OnHidden()
        {
        }

        protected override void OnShown()
        {
        }

        private void UpdateAllButtonData()
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
                        RefreshButtonState();
                        break;
                    }
                case PanelType.Skill:
                    {
                        RefreshSkillPageButtonState();
                        break;
                    }
            }
        }

        private void RefreshButtonState()
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
            throw new NotImplementedException();
        }

        private void RefreshCharacterPageButtonState()
        {
            if(m_currentPanelType == PanelType.Party)
            {
                for (int i = 0; i < m_partyCharacterButtons.Length; i++)
                {
                    m_partyCharacterButtons[i].SetUp(PlayerManager.Instance.GetCharacterByPartyIndex(i));
                }
            }
            else
            {
                List<OwningCharacterData> _allCharacter = PlayerManager.Instance.Player.Characters;
                
                for(int i = 0; i < m_allClonedCharacterButtons.Count; i++)
                {
                    m_allClonedCharacterButtons[i].gameObject.SetActive(false);
                }

                for(int i = 0; i < _allCharacter.Count; i++)
                {
                    if(i < m_allClonedCharacterButtons.Count)
                    {
                        m_allClonedCharacterButtons[i].SetUp(_allCharacter[i]);
                        m_allClonedCharacterButtons[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        MainMenuUI_CharacterButton _cloneButton = Instantiate(m_characterButtonPrefab);
                        _cloneButton.transform.SetParent(m_characterButtonContainer);
                        _cloneButton.SetUp(_allCharacter[i]);
                        m_allClonedCharacterButtons.Add(_cloneButton);
                    }
                }
            }
        }

        private void RefreshSkillPageButtonState()
        {
            throw new NotImplementedException();
        }
    }
}
