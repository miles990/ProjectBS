﻿using System;
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
        [SerializeField] private RectTransform m_equipmentButtonContainer = null;
        [SerializeField] private RectTransform m_skillButtonContainer = null;
        [SerializeField] private MainMenuUI_CharacterButton[] m_partyCharacterButtons = null;
        [SerializeField] private MainMenuUI_CharacterButton m_characterButtonPrefab = null;
        [SerializeField] private MainMenuUI_EquipmentButton m_equipmentButtonPrefab = null;
        [SerializeField] private MainMenuUI_SkillButton m_skillButtonPrefab = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel m_characterInfoPanel = null;

        private List<MainMenuUI_CharacterButton> m_allClonedCharacterButtons = new List<MainMenuUI_CharacterButton>();
        private List<MainMenuUI_EquipmentButton> m_allClonedEquipmentButtons = new List<MainMenuUI_EquipmentButton>();
        private List<MainMenuUI_SkillButton> m_allClonedSkillButtons = new List<MainMenuUI_SkillButton>();

        private PanelType m_currentPanelType = PanelType.Party;

        private void Start()
        {
            for(int i = 0; i < m_partyCharacterButtons.Length; i++)
            {
                m_partyCharacterButtons[i].OnButtonPressed += OnCharacterButtonPressed;
            }
            m_characterInfoPanel.OnEditEnded += RefreshCharacterPageButtonState;

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

            RefreshButtonState();
        }

        private void OnCharacterButtonPressed(OwningCharacterData obj)
        {
            m_characterInfoPanel.Enable(obj);
        }

        protected override void OnHidden()
        {
            m_characterInfoPanel.Hide();
        }

        protected override void OnShown()
        {
            RefreshCharacterPageButtonState();
            RefreshSkillPageButtonState();
            RefreshEquipmentPageButtonState();
            m_characterInfoPanel.Hide();
            OnWindowChanged(0);
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
                        RefreshEquipmentPageButtonState();
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

        private void RefreshEquipmentPageButtonState()
        {
            List<OwningEquipmentData> _allEquipment = PlayerManager.Instance.Player.Equipments;

            for (int i = 0; i < m_allClonedEquipmentButtons.Count; i++)
            {
                m_allClonedEquipmentButtons[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _allEquipment.Count; i++)
            {
                if (i < m_allClonedEquipmentButtons.Count)
                {
                    m_allClonedEquipmentButtons[i].SetUp(_allEquipment[i]);
                    m_allClonedEquipmentButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    MainMenuUI_EquipmentButton _cloneButton = Instantiate(m_equipmentButtonPrefab);
                    _cloneButton.transform.SetParent(m_equipmentButtonContainer);
                    _cloneButton.transform.localScale = Vector3.one;
                    _cloneButton.SetUp(_allEquipment[i]);
                    _cloneButton.OnEdited += RefreshEquipmentPageButtonState;
                    m_allClonedEquipmentButtons.Add(_cloneButton);
                }
            }
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
                        _cloneButton.transform.localScale = Vector3.one;
                        _cloneButton.SetUp(_allCharacter[i]);
                        _cloneButton.OnButtonPressed += OnCharacterButtonPressed;
                        _cloneButton.OnEdited += RefreshCharacterPageButtonState;
                        m_allClonedCharacterButtons.Add(_cloneButton);
                    }
                }
            }
        }

        private void RefreshSkillPageButtonState()
        {
            List<OwningSkillData> _allSkills = PlayerManager.Instance.Player.Skills;

            for (int i = 0; i < m_allClonedSkillButtons.Count; i++)
            {
                m_allClonedSkillButtons[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _allSkills.Count; i++)
            {
                if (i < m_allClonedSkillButtons.Count)
                {
                    m_allClonedSkillButtons[i].SetUp(_allSkills[i]);
                    m_allClonedSkillButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    MainMenuUI_SkillButton _cloneButton = Instantiate(m_skillButtonPrefab);
                    _cloneButton.transform.SetParent(m_skillButtonContainer);
                    _cloneButton.transform.localScale = Vector3.one;
                    _cloneButton.OnButtonPressed += OnSkillButtonPressed;
                    _cloneButton.SetUp(_allSkills[i]);
                    m_allClonedSkillButtons.Add(_cloneButton);
                }
            }
        }

        private void OnSkillButtonPressed(OwningSkillData owningSkillData)
        {
            SkillData _skill = GameDataManager.GetGameData<SkillData>(owningSkillData.SkillSourceID);
            string _name = ContextConverter.Instance.GetContext(_skill.NameContextID);

            GameManager.Instance.MessageManager.ShowCommonMessage(
                "Cost SP:" + _skill.SP + "\n\n" + _skill.GetAllDescriptionContext(),
                _name, null);
        }
    }
}
