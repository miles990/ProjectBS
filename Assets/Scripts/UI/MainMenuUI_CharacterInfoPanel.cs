using KahaGameCore.Static;
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
        [SerializeField] private Text m_headEquipmentText = null;
        [SerializeField] private Text m_bodyEquipmentText = null;
        [SerializeField] private Text m_footEquipmentText = null;
        [SerializeField] private Text m_handEquipmentText = null;
        [Header("Status")]
        [SerializeField] private Text m_levelText = null;
        [SerializeField] private Text m_hpspText = null;
        [SerializeField] private Text m_statusText = null;
        [Header("Skill")]
        [SerializeField] private Text m_skill0Text = null;
        [SerializeField] private Text m_skill1Text = null;
        [SerializeField] private Text m_skill2Text = null;
        [SerializeField] private Text m_skill3Text = null;
        [Header("Change Equipment Panel")]
        [SerializeField] private GameObject m_changeEquipmentPanelRoot = null;
        [SerializeField] private Text[] m_changeEquipmentPanel_equipmentList = null;
        [SerializeField] private Image[] m_changeEquipmentPanel_equipmentListImgae = null;
        [SerializeField] private Text m_changeEquipmentPanel_beforeText = null;
        [SerializeField] private Text m_changeEquipmentPanel_afterText = null;
        [Header("Set To Party Panel")]
        [SerializeField] private GameObject m_setToPartyPanelRoot = null;
        [SerializeField] private MainMenuUI_CharacterButton[] m_partyButtons = null;
        [Header("Change Skill Panel")]
        [SerializeField] private GameObject m_changeSkillPanelRoot = null;
        [SerializeField] private Text[] m_changeSkillPanel_skillList = null;
        [SerializeField] private Text m_changeSkillPanel_beforeText = null;
        [SerializeField] private Text m_changeSkillPanel_afterText = null;
        [Header("General")]
        [SerializeField] private Button m_nextPageButton = null;
        [SerializeField] private Button m_previousPageButton = null;

        private Data.OwningCharacterData m_refCharacter = null;
        private Dictionary<Text, Data.OwningEquipmentData> m_changeEquipmentPanelEquipmentListToEquipment = new Dictionary<Text, Data.OwningEquipmentData>();
        private Dictionary<Text, Data.OwningSkillData> m_changeSkillPanelSkillListToSkill = new Dictionary<Text, Data.OwningSkillData>();

        private State m_currentState = State.None;
        private int m_currentPage = 0;
        private string m_currentEquipmentType = Keyword.Head;
        private Data.OwningEquipmentData m_equipingEquipment = null;
        private Data.OwningEquipmentData m_currrentSelectEquipment = null;
        private Data.OwningSkillData m_currentSelectSkill = null;
        private int m_targetSkillSlotIndex = 0;

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
            m_currentState = State.ChangingEquipment;
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

        public void Button_ChangeEquipment_Select(Text key)
        {
            if(m_changeEquipmentPanelEquipmentListToEquipment.ContainsKey(key))
            {
                m_currrentSelectEquipment = m_changeEquipmentPanelEquipmentListToEquipment[key];
            }

            RefreshChangeEquipmentPanel(m_currentEquipmentType);
        }

        public void Button_ChangeSkill_Select(Text key)
        {
            if(m_changeSkillPanelSkillListToSkill.ContainsKey(key))
            {
                m_currentSelectSkill = m_changeSkillPanelSkillListToSkill[key];
            }

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
            m_refCharacter.SetSkill(m_targetSkillSlotIndex, m_currentSelectSkill.SkillSourceID);
            PlayerManager.Instance.SavePlayer();
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
                    m_changeEquipmentPanel_equipmentList[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    m_changeEquipmentPanel_equipmentList[i].text = string.Format("{0}\nHP{1}\nAttack{2}\nDefense{3}\nSpeed{4}",
                                                             ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.RawEquipmentData>(_equipments[_currentDisplayEquipmentIndex].EquipmentSourceID).NameContextID),
                                                             _equipments[_currentDisplayEquipmentIndex].HP >= 0 ? "+" + _equipments[_currentDisplayEquipmentIndex].HP : _equipments[_currentDisplayEquipmentIndex].HP.ToString(),
                                                             _equipments[_currentDisplayEquipmentIndex].Attack >= 0 ? "+" + _equipments[_currentDisplayEquipmentIndex].Attack : _equipments[_currentDisplayEquipmentIndex].Attack.ToString(),
                                                             _equipments[_currentDisplayEquipmentIndex].Defense >= 0 ? "+" + _equipments[_currentDisplayEquipmentIndex].Defense : _equipments[_currentDisplayEquipmentIndex].Defense.ToString(),
                                                             _equipments[_currentDisplayEquipmentIndex].Speed >= 0 ? "+" + _equipments[_currentDisplayEquipmentIndex].Speed : _equipments[_currentDisplayEquipmentIndex].Speed.ToString());

                    if (m_changeEquipmentPanelEquipmentListToEquipment.ContainsKey(m_changeEquipmentPanel_equipmentList[i]))
                    {
                        m_changeEquipmentPanelEquipmentListToEquipment[m_changeEquipmentPanel_equipmentList[i]] = _equipments[_currentDisplayEquipmentIndex];
                    }
                    else
                    {
                        m_changeEquipmentPanelEquipmentListToEquipment.Add(m_changeEquipmentPanel_equipmentList[i], _equipments[_currentDisplayEquipmentIndex]);
                    }

                    m_changeEquipmentPanel_equipmentListImgae[i].color = _equipments[_currentDisplayEquipmentIndex] == m_currrentSelectEquipment ? Color.gray : Color.white;
                    m_changeEquipmentPanel_equipmentList[i].transform.parent.gameObject.SetActive(true);
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
                    m_changeSkillPanel_skillList[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    Data.SkillData _skillData = GameDataManager.GetGameData<Data.SkillData>(PlayerManager.Instance.Player.Skills[_currentDisplaySkillIndex].SkillSourceID);

                    m_changeSkillPanel_skillList[i].text = string.Format("{0} (x{1})\n{2}",
                        ContextConverter.Instance.GetContext(_skillData.NameContextID),
                        PlayerManager.Instance.Player.Skills[_currentDisplaySkillIndex].Amount,
                        ContextConverter.Instance.GetContext(_skillData.DescriptionContextID));

                    if (m_changeSkillPanelSkillListToSkill.ContainsKey(m_changeSkillPanel_skillList[i]))
                    {
                        m_changeSkillPanelSkillListToSkill[m_changeSkillPanel_skillList[i]] = PlayerManager.Instance.Player.Skills[_currentDisplaySkillIndex];
                    }
                    else
                    {
                        m_changeSkillPanelSkillListToSkill.Add(m_changeSkillPanel_skillList[i], PlayerManager.Instance.Player.Skills[_currentDisplaySkillIndex]);
                    }

                    m_changeSkillPanel_skillList[i].transform.parent.gameObject.SetActive(true);
                }
            }

            Data.SkillData _beforeSkillData = GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.GetSkill(m_targetSkillSlotIndex));
            
            m_changeSkillPanel_beforeText.text = string.Format("{0}\n{1}",
            ContextConverter.Instance.GetContext(_beforeSkillData.NameContextID),
            ContextConverter.Instance.GetContext(_beforeSkillData.DescriptionContextID));

            if (m_currentSelectSkill == null)
            {
                m_changeSkillPanel_afterText.text = m_changeSkillPanel_beforeText.text;
            }
            else
            {
                Data.SkillData _selectingSkill = GameDataManager.GetGameData<Data.SkillData>(m_currentSelectSkill.SkillSourceID);

                m_changeSkillPanel_afterText.text = string.Format("{0}\n{1}",
                    ContextConverter.Instance.GetContext(_selectingSkill.NameContextID),
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
            Data.OwningEquipmentData _head = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Head);
            int _headNameID = _head != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_head.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _body = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Body);
            int _bodyNameID = _body != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_body.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _hand = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Hand);
            int _handNameID = _hand != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_hand.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _foot = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Foot);
            int _footNameID = _foot != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_foot.EquipmentSourceID).NameContextID : 0;

            m_headEquipmentText.text = ContextConverter.Instance.GetContext(_headNameID);
            m_bodyEquipmentText.text = ContextConverter.Instance.GetContext(_bodyNameID);
            m_handEquipmentText.text = ContextConverter.Instance.GetContext(_handNameID);
            m_footEquipmentText.text = ContextConverter.Instance.GetContext(_footNameID);

            m_skill0Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_0).NameContextID);
            m_skill1Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_1).NameContextID);
            m_skill2Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_2).NameContextID);
            m_skill3Text.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_3).NameContextID);

            m_levelText.text = "Level" + m_refCharacter.Level;
            m_statusText.text = string.Format(ABILITY_FORMAT,
                            m_refCharacter.GetTotal(Keyword.Attack),
                            GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.AttackAbilityID).RankString,
                            m_refCharacter.GetTotal(Keyword.Defense),
                            GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.DefenseAbilityID).RankString,
                            m_refCharacter.GetTotal(Keyword.Speed),
                            GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.SpeedAbilityID).RankString);

            m_hpspText.text = string.Format(HP_FORMAT,
                                m_refCharacter.GetTotal(Keyword.HP),
                                GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.HPAbilityID).RankString,
                                m_refCharacter.SP);
        }
    }
}
