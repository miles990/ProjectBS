using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterInfoPanel : MonoBehaviour
    {
        private enum State
        {
            None,
            ChangingEquipment,
            SetToParty,
            ChangingSkill
        }

        private const string HP_FORMAT = "HP: {0}({1})\nSP: {2}";
        private const string ABILITY_FORMAT = "Attack: {0} ({1})\nDefense: {2} ({3})\nSpeed: {4} ({5})";

        public event System.Action OnEditEnded = null;

        [Header("Equipmet")]
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_headEquipment = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_bodyEquipment = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_footEquipment = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_handEquipment = null;
        [Header("Status")]
        [SerializeField] private Text m_levelText = null;
        [SerializeField] private Text m_hpspText = null;
        [SerializeField] private Text m_statusText = null;
        [Header("Skill")]
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill0 = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill1 = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill2 = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill3 = null;
        [Header("Change Equipment Panel")]
        [SerializeField] private GameObject m_changeEquipmentPanelRoot = null;
        [SerializeField] private MainMenuUI_ChangeWeaponPanel_EquipmentButton[] m_changeEquipmentPanel_equipmentList = null;
        [SerializeField] private Text m_changeEquipmentPanel_beforeText = null;
        [SerializeField] private Text m_changeEquipmentPanel_afterText = null;
        [Header("Set To Party Panel")]
        [SerializeField] private GameObject m_setToPartyPanelRoot = null;
        [SerializeField] private MainMenuUI_CharacterButton[] m_partyButtons = null;
        [Header("Change Skill Panel")]
        [SerializeField] private GameObject m_changeSkillPanelRoot = null;
        [SerializeField] private MainMenuUI_ChangeSkillPanel_SkilltButton[] m_changeSkillPanel_skillList = null;
        [SerializeField] private Text m_changeSkillPanel_beforeText = null;
        [SerializeField] private Text m_changeSkillPanel_afterText = null;
        [Header("General")]
        [SerializeField] private Button m_nextPageButton = null;
        [SerializeField] private Button m_previousPageButton = null;

        private Data.OwningCharacterData m_refCharacter = null;

        private State m_currentState = State.None;
        private int m_currentPage = 0;
        private string m_currentEquipmentType = Keyword.Head;
        private Data.OwningEquipmentData m_equipingEquipment = null;
        private Data.OwningEquipmentData m_currrentSelectEquipment = null;
        private Data.OwningSkillData m_currentSelectSkill = null;
        private int m_targetSkillSlotIndex = 0;

        private void Start()
        {
            for(int i = 0; i < m_changeEquipmentPanel_equipmentList.Length; i++)
            {
                m_changeEquipmentPanel_equipmentList[i].OnSelected += Button_ChangeEquipment_Select;
            }
            for (int i = 0; i < m_changeSkillPanel_skillList.Length; i++)
            {
                m_changeSkillPanel_skillList[i].OnSelected += Button_ChangeSkill_Select;
            }
        }

        public void Enable(Data.OwningCharacterData characterData)
        {
            m_refCharacter = characterData;
            DisableAllSubPanel();
            gameObject.SetActive(true);
        }

        public void Button_StartChangeEquipment(string equipmentType)
        {
            m_currentPage = 0;
            m_currentState = State.ChangingEquipment;
            string _equipingUDID = m_refCharacter.GetEquipmentUDID(equipmentType);
            if (string.IsNullOrEmpty(_equipingUDID))
            {
                m_equipingEquipment = m_currrentSelectEquipment = null;
            }
            else
            {
                m_equipingEquipment = m_currrentSelectEquipment = PlayerManager.Instance.GetEquipmentByUDID(_equipingUDID);
            }
            RefreshChangeEquipmentPanel(equipmentType);
            m_changeEquipmentPanelRoot.SetActive(true);
            m_nextPageButton.gameObject.SetActive(true);
            m_previousPageButton.gameObject.SetActive(true);
        }

        public void Button_StartChangeSkill(int index)
        {
            m_targetSkillSlotIndex = index;
            m_currentPage = 0;
            m_currentState = State.ChangingSkill;
            m_currentSelectSkill = null;
            RefreshChangeSkillPanel();
            m_changeSkillPanelRoot.SetActive(true);
            m_nextPageButton.gameObject.SetActive(true);
            m_previousPageButton.gameObject.SetActive(true);
        }

        public void Button_ChangePanels_GoNextPage()
        {
            m_currentPage++;
            RefreshSelectPanel();
        }

        public void Button_ChangePanels_GoPreviousPage()
        {
            m_currentPage--;
            m_currentPage = m_currentPage < 0 ? 0 : m_currentPage;
            RefreshSelectPanel();
        }

        public void Button_ChangeEquipment_Select(Data.OwningEquipmentData data)
        {
            m_currrentSelectEquipment = data;
            RefreshChangeEquipmentPanel(m_currentEquipmentType);
        }

        public void Button_ChangeSkill_Select(Data.OwningSkillData data)
        {
            m_currentSelectSkill = data;
            RefreshChangeSkillPanel();
        }

        public void Button_ChangeEquipment_Change()
        {
            if(m_currrentSelectEquipment != null)
            {
                PlayerManager.Instance.EquipmentTo(m_refCharacter, m_currrentSelectEquipment.UDID);
            }
            PlayerManager.Instance.SavePlayer();
            DisableAllSubPanel();
        }

        public void Button_ChangeSkill_Change()
        {
            if(m_currentSelectSkill != null)
            {
                PlayerManager.Instance.SetSkill(m_refCharacter, m_targetSkillSlotIndex, m_currentSelectSkill.SkillSourceID);
            }
            DisableAllSubPanel();
        }

        public void Button_Back()
        {
            switch(m_currentState)
            {
                case State.None:
                    {
                        OnEditEnded?.Invoke();
                        gameObject.SetActive(false);
                        break;
                    }
                case State.ChangingEquipment:
                case State.SetToParty:
                case State.ChangingSkill:
                    {
                        DisableAllSubPanel();
                        break;
                    }
                default:
                    throw new System.Exception("[MainMenuUI_CharacterInfoPanel][Button_Back] Invaild state=" + m_currentState.ToString());
            }
        }

        public void Button_Depart()
        {
            CharacterUtility.Depart(m_refCharacter.UDID);
            Button_Save();
        }

        public void Button_LevelUp()
        {
            CharacterUtility.TryAddOneLevel(m_refCharacter);
            RefreshInfo();
            PlayerManager.Instance.SavePlayer();
        }

        public void Button_Save()
        {
            PlayerManager.Instance.SavePlayer();
            Button_Back();
        }

        public void Button_StartSetToParty()
        {
            m_currentState = State.SetToParty;
            for(int i = 0; i < m_partyButtons.Length; i++)
            {
                Data.OwningCharacterData _character = PlayerManager.Instance.GetCharacterByPartyIndex(i);
                m_partyButtons[i].SetUp(_character);
                m_partyButtons[i].OnButtonPressed += OnPartyCharacterButtonSelected;
            }
            m_setToPartyPanelRoot.SetActive(true);
        }

        private void RefreshSelectPanel()
        {
            switch(m_currentState)
            {
                case State.ChangingEquipment:
                    RefreshChangeEquipmentPanel(m_currentEquipmentType);
                    break;
                case State.ChangingSkill:
                    RefreshChangeSkillPanel();
                    break;
                default:
                    throw new System.Exception("[MainMenuUI_CharacterInfoPanel][RefreshSelectPanel] Invaild State=" + m_currentState.ToString());
            }
        }

        private void OnPartyCharacterButtonSelected(Data.OwningCharacterData characterData)
        {
            for (int i = 0; i < m_partyButtons.Length; i++)
            {
                m_partyButtons[i].OnButtonPressed -= OnPartyCharacterButtonSelected;
            }
            int _currentCharacterPartyIndex = PlayerManager.Instance.GetPartyIndex(m_refCharacter);
            int _selectPartyIndex = PlayerManager.Instance.GetPartyIndex(characterData);
            if(_currentCharacterPartyIndex == -1)
            {
                PlayerManager.Instance.SetToParty(_selectPartyIndex, m_refCharacter);
            }
            else
            {
                PlayerManager.Instance.SetToParty(_selectPartyIndex, m_refCharacter);
                PlayerManager.Instance.SetToParty(_currentCharacterPartyIndex, characterData);
            }
            PlayerManager.Instance.SavePlayer();
            DisableAllSubPanel();
        }

        private void DisableAllSubPanel()
        {
            m_currentState = State.None;
            m_changeEquipmentPanelRoot.SetActive(false);
            m_changeSkillPanelRoot.SetActive(false);
            m_setToPartyPanelRoot.SetActive(false);
            m_nextPageButton.gameObject.SetActive(false);
            m_previousPageButton.gameObject.SetActive(false);
            RefreshInfo();
        }

        private void RefreshChangeEquipmentPanel(string equipmentType)
        {
            m_currentEquipmentType = equipmentType;

            List<Data.OwningEquipmentData> _equipments = PlayerManager.Instance.GetEquipmentsByType(m_currentEquipmentType);
            m_previousPageButton.interactable = m_currentPage != 0;
            m_nextPageButton.interactable = m_changeEquipmentPanel_equipmentList.Length * (m_currentPage + 1) < _equipments.Count;
            
            for (int i = 0; i < m_changeEquipmentPanel_equipmentList.Length; i++)
            {
                int _currentDisplayEquipmentIndex = i + m_changeEquipmentPanel_equipmentList.Length * m_currentPage;

                if (_currentDisplayEquipmentIndex >= _equipments.Count)
                {
                    m_changeEquipmentPanel_equipmentList[i].gameObject.SetActive(false);
                }
                else
                {
                    m_changeEquipmentPanel_equipmentList[i].SetUp(_equipments[_currentDisplayEquipmentIndex]);
                    m_changeEquipmentPanel_equipmentList[i].EnableButton(_equipments[_currentDisplayEquipmentIndex] != m_currrentSelectEquipment);
                    m_changeEquipmentPanel_equipmentList[i].gameObject.SetActive(true);
                }
            }

            int _beforeHP = m_refCharacter.GetTotal(Keyword.HP);
            int _beforeSP = m_refCharacter.GetTotal(Keyword.SP);
            int _beforeAtk = m_refCharacter.GetTotal(Keyword.Attack);
            int _beforeDef = m_refCharacter.GetTotal(Keyword.Defense);
            int _beforeSpd = m_refCharacter.GetTotal(Keyword.Speed);

            m_changeEquipmentPanel_beforeText.text = string.Format("HP: {0}\nSP: {1}\nAttack: {2}\nDefense: {3}\nSpeed: {4}",
                                                        _beforeHP.ToString(),
                                                        _beforeSP.ToString(),
                                                        _beforeAtk.ToString(),
                                                        _beforeDef.ToString(),
                                                        _beforeSpd.ToString());

            if(m_currrentSelectEquipment == null)
            {
                m_changeEquipmentPanel_afterText.text = m_changeEquipmentPanel_beforeText.text;
            }
            else
            {
                int _afterHP = _beforeHP - (m_equipingEquipment == null ? 0 : m_equipingEquipment.HP) + m_currrentSelectEquipment.HP;
                int _afterSP = _beforeSP - (m_equipingEquipment == null ? 0 : m_equipingEquipment.SP) + m_currrentSelectEquipment.SP;
                int _afterAtk = _beforeAtk - (m_equipingEquipment == null ? 0 : m_equipingEquipment.Attack) + m_currrentSelectEquipment.Attack;
                int _afterDef = _beforeDef - (m_equipingEquipment == null ? 0 : m_equipingEquipment.Defense) + m_currrentSelectEquipment.Defense;
                int _afterSpd = _beforeSpd - (m_equipingEquipment == null ? 0 : m_equipingEquipment.Speed) + m_currrentSelectEquipment.Speed;

                m_changeEquipmentPanel_afterText.text = string.Format("HP: {0}\nSP: {1}\nAttack: {2}\nDefense: {3}\nSpeed: {4}",
                                            GetChangeStatusString(_beforeHP, _afterHP),
                                            GetChangeStatusString(_beforeSP, _afterSP),
                                            GetChangeStatusString(_beforeAtk, _afterAtk),
                                            GetChangeStatusString(_beforeDef, _afterDef),
                                            GetChangeStatusString(_beforeSpd, _afterSpd));
            }
        }

        private void RefreshChangeSkillPanel()
        {
            m_previousPageButton.interactable = m_currentPage != 0;
            m_nextPageButton.interactable = m_changeSkillPanel_skillList.Length * (m_currentPage + 1) < PlayerManager.Instance.Player.Skills.Count;
            for (int i = 0; i < m_changeSkillPanel_skillList.Length; i++)
            {
                int _currentDisplaySkillIndex = i + m_changeSkillPanel_skillList.Length * m_currentPage;

                if (i + m_changeSkillPanel_skillList.Length * m_currentPage >= PlayerManager.Instance.Player.Skills.Count)
                {
                    m_changeSkillPanel_skillList[i].transform.gameObject.SetActive(false);
                }
                else
                {
                    m_changeSkillPanel_skillList[i].SetUp(PlayerManager.Instance.Player.Skills[_currentDisplaySkillIndex]);
                    m_changeSkillPanel_skillList[i].transform.gameObject.SetActive(true);
                }
            }

            Data.SkillData _beforeSkillData = m_refCharacter.GetSkill(m_targetSkillSlotIndex);
            
            m_changeSkillPanel_beforeText.text = string.Format("{0}\n(Cost SP:{1})\n{2}",
            ContextConverter.Instance.GetContext(_beforeSkillData.NameContextID),
            _beforeSkillData.SP,
            ContextConverter.Instance.GetContext(_beforeSkillData.DescriptionContextID));

            if (m_currentSelectSkill == null)
            {
                m_changeSkillPanel_afterText.text = m_changeSkillPanel_beforeText.text;
            }
            else
            {
                Data.SkillData _selectingSkill = m_currentSelectSkill.GetSourceData();

                m_changeSkillPanel_afterText.text = string.Format("{0}\n(Cost SP:{1})\n{2}",
                    ContextConverter.Instance.GetContext(_selectingSkill.NameContextID),
                    _selectingSkill.SP,
                    ContextConverter.Instance.GetContext(_selectingSkill.DescriptionContextID));
            }
        }

        private string GetChangeStatusString(int before, int after)
        {
            string _string;
            if (after > before)
                _string = "<Color=blue>" + after + "</color>";
            else if (after < before)
                _string = "<Color=red>" + after + "</color>";
            else
                _string = after.ToString();

            return _string;
        }

        private void RefreshInfo()
        {
            m_headEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Head));
            m_bodyEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Body));
            m_handEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Hand));
            m_footEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Foot));

            m_skill0.SetUp(m_refCharacter.GetSkill(0));
            m_skill1.SetUp(m_refCharacter.GetSkill(1));
            m_skill2.SetUp(m_refCharacter.GetSkill(2));
            m_skill3.SetUp(m_refCharacter.GetSkill(3));

            m_levelText.text = "Level" + m_refCharacter.Level + "\n\nExp\n" + m_refCharacter.Exp + "/" + m_refCharacter.GetRequireExp();
            m_statusText.text = string.Format(ABILITY_FORMAT,
                            m_refCharacter.GetTotal(Keyword.Attack),
                            m_refCharacter.GetAbilityRankString(Keyword.Attack),
                            m_refCharacter.GetTotal(Keyword.Defense),
                            m_refCharacter.GetAbilityRankString(Keyword.Defense),
                            m_refCharacter.GetTotal(Keyword.Speed),
                            m_refCharacter.GetAbilityRankString(Keyword.Speed),

            m_hpspText.text = string.Format(HP_FORMAT,
                                m_refCharacter.GetTotal(Keyword.HP),
                                m_refCharacter.GetAbilityRankString(Keyword.HP),
                                m_refCharacter.SP));
        }
    }
}
