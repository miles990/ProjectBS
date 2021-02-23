using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterInfoPanel : MainMenuUI_PanelBase
    {
        private enum State
        {
            None,
            ChangingEquipment,
            SetToParty,
            ChangingSkill
        }

        public event System.Action OnEditEnded = null;

        [Header("Equipmet")]
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_headEquipment = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_bodyEquipment = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_footEquipment = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_EquipmentButton m_handEquipment = null;
        [Header("Status")]
        [SerializeField] private GameObject m_partyHint = null;
        [SerializeField] private TextMeshProUGUI m_partyHintText = null;
        [SerializeField] private TextMeshProUGUI m_levelText = null;
        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private RawImage m_iconImage = null;
        [SerializeField] private TextMeshProUGUI m_expText = null;
        [SerializeField] private TextMeshProUGUI m_hpRankText = null;
        [SerializeField] private TextMeshProUGUI m_hpText = null;
        [SerializeField] private TextMeshProUGUI m_attackRankText = null;
        [SerializeField] private TextMeshProUGUI m_attackText = null;
        [SerializeField] private TextMeshProUGUI m_defenseRankText = null;
        [SerializeField] private TextMeshProUGUI m_defenseText = null;
        [SerializeField] private TextMeshProUGUI m_speedRankText = null;
        [SerializeField] private TextMeshProUGUI m_speedText = null;
        [Header("Status Bar")]
        [SerializeField] private Image m_hpProgressBarImage = null;
        [SerializeField] private TextMeshProUGUI m_hpProgressText = null;
        [SerializeField] private Image m_attackProgressBarImage = null;
        [SerializeField] private TextMeshProUGUI m_attackProgressText = null;
        [SerializeField] private Image m_defenseProgressBarImage = null;
        [SerializeField] private TextMeshProUGUI m_defenseProgressText = null;
        [SerializeField] private Image m_speedProgressBarImage = null;
        [SerializeField] private TextMeshProUGUI m_speedProgressText = null;
        [Header("Skill")]
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill0 = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill1 = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill2 = null;
        [SerializeField] private MainMenuUI_CharacterInfoPanel_SkilltButton m_skill3 = null;
        [Header("Select Object Panel")]
        [SerializeField] private GameObject m_selectObjectPanelRoot = null;
        [SerializeField] private RectTransform m_selectObjectContainer = null;
        [Header("Change Equipment Panel")]
        [SerializeField] private MainMenuUI_EquipmentButton m_equipmentButtonPrefab = null;
        [SerializeField] private GameObject m_compareEquipmentPanelRoot = null;
        [SerializeField] private MainMenuUI_EquipmentButton m_changeEquipment_before = null;
        [SerializeField] private GameObject m_changeEquipment_before_noContentRoot = null;
        [SerializeField] private MainMenuUI_EquipmentButton m_changeEquipment_after = null;
        [SerializeField] private GameObject m_changeEquipment_after_noContentRoot = null;
        [SerializeField] private TextMeshProUGUI m_changeEquipment_defferentValue_hp = null;
        [SerializeField] private TextMeshProUGUI m_changeEquipment_defferentValue_attack = null;
        [SerializeField] private TextMeshProUGUI m_changeEquipment_defferentValue_defense = null;
        [SerializeField] private TextMeshProUGUI m_changeEquipment_defferentValue_speed = null;
        [Header("Set To Party Panel")]
        [SerializeField] private GameObject m_setToPartyPanelRoot = null;
        [SerializeField] private MainMenuUI_CharacterButton[] m_partyButtons = null;
        [Header("Change Skill Panel")]
        [SerializeField] private MainMenuUI_SkillButton m_skillButtonPrefab = null;
        [SerializeField] private GameObject m_compareSkillPanelRoot = null;
        [SerializeField] private MainMenuUI_SkillButton m_changeSkill_beforeSkill = null;
        [SerializeField] private GameObject m_changeSkill_before_noContentRoot = null;
        [SerializeField] private MainMenuUI_SkillButton m_changeSkill_afterSkill = null;
        [SerializeField] private GameObject m_changeSkill_after_noContentRoot = null;

        private Data.OwningCharacterData m_refCharacter = null;

        private State m_currentState = State.None;
        private string m_currentEquipmentType = Keyword.Head;
        private Data.OwningEquipmentData m_equipingEquipment = null;
        private Data.OwningEquipmentData m_currrentSelectEquipment = null;
        private Data.OwningSkillData m_currentSelectSkill = null;
        private int m_targetSkillSlotIndex = 0;

        private List<MainMenuUI_EquipmentButton> m_clonedEquipmentButtons = new List<MainMenuUI_EquipmentButton>();
        private List<MainMenuUI_SkillButton> m_clonedSkillButtons = new List<MainMenuUI_SkillButton>();

        private void Start()
        {
            m_headEquipment.OnSelected += StartChangeEquipment;
            m_bodyEquipment.OnSelected += StartChangeEquipment;
            m_handEquipment.OnSelected += StartChangeEquipment;
            m_footEquipment.OnSelected += StartChangeEquipment;
            m_skill0.OnSelected += StartChangeSkill;
            m_skill1.OnSelected += StartChangeSkill;
            m_skill2.OnSelected += StartChangeSkill;
            m_skill3.OnSelected += StartChangeSkill;
        }

        public void Enable(Data.OwningCharacterData characterData)
        {
            m_refCharacter = characterData;
            DisableAllSubPanel();
            RefreshInfo();
            Show();
        }

        private void StartChangeEquipment(MainMenuUI_CharacterInfoPanel_EquipmentButton button)
        {
            m_currentState = State.ChangingEquipment;

            if(button == m_headEquipment)
            {
                ShowEquipmentList(Keyword.Head);
            }
            else if (button == m_handEquipment)
            {
                ShowEquipmentList(Keyword.Hand);
            }
            else if (button == m_bodyEquipment)
            {
                ShowEquipmentList(Keyword.Body);
            }
            else if (button == m_footEquipment)
            {
                ShowEquipmentList(Keyword.Foot);
            }
        }

        private void ShowEquipmentList(string type)
        {
            string _equipingUDID = m_refCharacter.GetEquipmentUDID(type);
            if (string.IsNullOrEmpty(_equipingUDID))
            {
                m_equipingEquipment = m_currrentSelectEquipment = null;
            }
            else
            {
                m_equipingEquipment = m_currrentSelectEquipment = PlayerManager.Instance.GetEquipmentByUDID(_equipingUDID);
            }
            RefreshChangeEquipmentPanel(type);
            m_selectObjectPanelRoot.SetActive(true);
        }

        private void StartChangeSkill(MainMenuUI_CharacterInfoPanel_SkilltButton skill)
        {
            m_targetSkillSlotIndex = -1;
            if (skill == m_skill0) m_targetSkillSlotIndex = 0;
            if (skill == m_skill1) m_targetSkillSlotIndex = 1;
            if (skill == m_skill2) m_targetSkillSlotIndex = 2;
            if (skill == m_skill3) m_targetSkillSlotIndex = 3;

            if (m_targetSkillSlotIndex == -1) 
                throw new System.Exception("[MainMenuUI_CharacterInfoPanel][StartChangeSkill] Invaild skill button");

            m_currentState = State.ChangingSkill;
            m_currentSelectSkill = null;
            RefreshChangeSkillPanel();
            m_selectObjectPanelRoot.SetActive(true);
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
            RefreshInfo();
            DisableAllSubPanel();
        }

        public void Button_ChangeSkill_Change()
        {
            if(m_currentSelectSkill != null)
            {
                PlayerManager.Instance.SetSkill(m_refCharacter, m_targetSkillSlotIndex, m_currentSelectSkill.SkillSourceID);
            }
            RefreshInfo();
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

        public void Button_LevelUp(int addLevel)
        {
            for(int i = 0; i < addLevel; i++)
            {
                CharacterUtility.TryAddOneLevel(m_refCharacter);
            }

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
            m_selectObjectPanelRoot.SetActive(false);
            m_compareSkillPanelRoot.SetActive(false);
            m_compareEquipmentPanelRoot.SetActive(false);
            m_setToPartyPanelRoot.SetActive(false);
        }

        private void RefreshChangeEquipmentPanel(string equipmentType)
        {
            m_currentEquipmentType = equipmentType;

            for(int i = 0; i < m_clonedEquipmentButtons.Count; i++)
            {
                m_clonedEquipmentButtons[i].gameObject.SetActive(false);
            }
            for(int i = 0; i < m_clonedSkillButtons.Count; i++)
            {
                m_clonedSkillButtons[i].gameObject.SetActive(false);
            }

            List<Data.OwningEquipmentData> _equipments = PlayerManager.Instance.GetEquipmentsByType(m_currentEquipmentType);
            
            for (int i = 0; i < _equipments.Count; i++)
            {
                if (i < m_clonedEquipmentButtons.Count)
                {
                    m_clonedEquipmentButtons[i].SetUp(_equipments[i]);
                    m_clonedEquipmentButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    MainMenuUI_EquipmentButton _cloneButton = Instantiate(m_equipmentButtonPrefab);
                    _cloneButton.transform.SetParent(m_selectObjectContainer);
                    _cloneButton.transform.localScale = Vector3.one;
                    _cloneButton.SetUp(_equipments[i]);
                    _cloneButton.OnButtonPressed += ShowEquipmentComparePanel;
                    _cloneButton.OnEdited += delegate { RefreshChangeEquipmentPanel(m_currentEquipmentType); };
                    m_clonedEquipmentButtons.Add(_cloneButton);
                }
            }
        }

        private void ShowEquipmentComparePanel(Data.OwningEquipmentData selected)
        {
            m_changeEquipment_before_noContentRoot.SetActive(m_equipingEquipment == null);
            if (m_equipingEquipment != null) m_changeEquipment_before.SetUp(m_equipingEquipment);
            m_changeEquipment_before.gameObject.SetActive(m_equipingEquipment != null);

            m_changeEquipment_after_noContentRoot.SetActive(selected == null);
            if (selected != null) m_changeEquipment_after.SetUp(selected);
            m_changeEquipment_after.gameObject.SetActive(selected != null);

            m_currrentSelectEquipment = selected;

            int _beforeHP = m_refCharacter.GetTotal(Keyword.HP);
            int _beforeAtk = m_refCharacter.GetTotal(Keyword.Attack);
            int _beforeDef = m_refCharacter.GetTotal(Keyword.Defense);
            int _beforeSpd = m_refCharacter.GetTotal(Keyword.Speed);

            int _afterHP = _beforeHP - (m_equipingEquipment == null ? 0 : m_equipingEquipment.HP) + m_currrentSelectEquipment.HP;
            int _afterAtk = _beforeAtk - (m_equipingEquipment == null ? 0 : m_equipingEquipment.Attack) + m_currrentSelectEquipment.Attack;
            int _afterDef = _beforeDef - (m_equipingEquipment == null ? 0 : m_equipingEquipment.Defense) + m_currrentSelectEquipment.Defense;
            int _afterSpd = _beforeSpd - (m_equipingEquipment == null ? 0 : m_equipingEquipment.Speed) + m_currrentSelectEquipment.Speed;

            m_changeEquipment_defferentValue_hp.text = GetChangeStatusString(_beforeHP, _afterHP);
            m_changeEquipment_defferentValue_attack.text = GetChangeStatusString(_beforeAtk, _afterAtk);
            m_changeEquipment_defferentValue_defense.text = GetChangeStatusString(_beforeDef, _afterDef);
            m_changeEquipment_defferentValue_speed.text = GetChangeStatusString(_beforeSpd, _afterSpd);

            m_compareEquipmentPanelRoot.SetActive(true);
        }

        private void RefreshChangeSkillPanel()
        {
            for (int i = 0; i < m_clonedSkillButtons.Count; i++)
            {
                m_clonedSkillButtons[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < m_clonedEquipmentButtons.Count; i++)
            {
                m_clonedEquipmentButtons[i].gameObject.SetActive(false);
            }

            List<Data.OwningSkillData> _skills = PlayerManager.Instance.Player.Skills;

            for (int i = 0; i < _skills.Count; i++)
            {
                if (i < m_clonedSkillButtons.Count)
                {
                    m_clonedSkillButtons[i].SetUp(_skills[i]);
                    m_clonedSkillButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    MainMenuUI_SkillButton _cloneButton = Instantiate(m_skillButtonPrefab);
                    _cloneButton.transform.SetParent(m_selectObjectContainer);
                    _cloneButton.transform.localScale = Vector3.one;
                    _cloneButton.SetUp(_skills[i]);
                    _cloneButton.OnButtonPressed += ShowSkillComparePanel;
                    m_clonedSkillButtons.Add(_cloneButton);
                }
            }
        }

        private void ShowSkillComparePanel(Data.OwningSkillData skillData)
        {
            Data.SkillData _beforeSkillData = m_refCharacter.GetSkill(m_targetSkillSlotIndex);
            m_changeSkill_before_noContentRoot.SetActive(_beforeSkillData == null);
            m_changeSkill_beforeSkill.gameObject.SetActive(_beforeSkillData != null);
            if (_beforeSkillData != null) m_changeSkill_beforeSkill.SetUp(_beforeSkillData);

            m_currentSelectSkill = skillData;

            m_changeSkill_after_noContentRoot.SetActive(m_currentSelectSkill == null);
            m_changeSkill_afterSkill.gameObject.SetActive(m_currentSelectSkill != null);
            if (m_currentSelectSkill != null) m_changeSkill_afterSkill.SetUp(m_currentSelectSkill);

            m_compareSkillPanelRoot.SetActive(true);
        }

        private string GetChangeStatusString(int before, int after)
        {
            string _string;
            if (after > before)
            {
                _string = "<#FFAA00FF>(+" + (after - before) + ")</color>";
            }
            else if (after < before)
            {
                _string = "<#00C0FFFF>(" + (after - before) + ")</color>";
            }
            else
                _string = "";

            return _string;
        }

        private void RefreshInfo()
        {
            int _partyIndex = PlayerManager.Instance.GetPartyIndex(m_refCharacter);
            if(_partyIndex == -1)
            {
                m_partyHint.SetActive(false);
            }
            else
            {
                m_partyHint.SetActive(true);
                m_partyHintText.text = "#" + (_partyIndex + 1);
            }

            m_headEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Head));
            m_bodyEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Body));
            m_handEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Hand));
            m_footEquipment.SetUp(PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Foot));

            m_skill0.SetUp(m_refCharacter.GetSkill(0));
            m_skill1.SetUp(m_refCharacter.GetSkill(1));
            m_skill2.SetUp(m_refCharacter.GetSkill(2));
            m_skill3.SetUp(m_refCharacter.GetSkill(3));

            m_nameText.text = m_refCharacter.GetName();
            m_iconImage.texture = m_refCharacter.GetIcon();
            m_levelText.text = "Level. " + m_refCharacter.Level;
            m_expText.text = m_refCharacter.Exp + " / " + m_refCharacter.GetRequireExp();
            m_hpRankText.text = m_refCharacter.GetAbilityRankString("HP");
            m_hpRankText.color = UITextColorStorer.GetRankStringColor(m_hpRankText.text);
            m_hpText.text = m_refCharacter.GetTotal(Keyword.HP).ToString();
            m_attackRankText.text = m_refCharacter.GetAbilityRankString("Attack");
            m_attackRankText.color = UITextColorStorer.GetRankStringColor(m_attackRankText.text);
            m_attackText.text = m_refCharacter.GetTotal(Keyword.Attack).ToString();
            m_defenseRankText.text = m_refCharacter.GetAbilityRankString("Defense");
            m_defenseRankText.color = UITextColorStorer.GetRankStringColor(m_defenseRankText.text);
            m_defenseText.text = m_refCharacter.GetTotal(Keyword.Defense).ToString();
            m_speedRankText.text = m_refCharacter.GetAbilityRankString("Speed");
            m_speedRankText.color = UITextColorStorer.GetRankStringColor(m_speedRankText.text);
            m_speedText.text = m_refCharacter.GetTotal(Keyword.Speed).ToString();

            // const value from designer
            int _maxHPValue = GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.HPAbilityID).MaxValue * 100 + 100000;
            int _maxAttackValue = GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.AttackAbilityID).MaxValue * 100 + 10000;
            int _maxDefenseValue = GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.DefenseAbilityID).MaxValue * 100 + 10000;
            int _maxSpeedValue = GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.SpeedAbilityID).MaxValue * 100 + 10000;

            m_hpProgressBarImage.fillAmount = (float)m_refCharacter.HP / (float)_maxHPValue;
            m_hpProgressText.text = m_refCharacter.HP + " / " + _maxHPValue;
            m_attackProgressBarImage.fillAmount = (float)m_refCharacter.Attack / (float)_maxAttackValue;
            m_attackProgressText.text = m_refCharacter.Attack + " / " + _maxAttackValue;
            m_defenseProgressBarImage.fillAmount = (float)m_refCharacter.Defense / (float)_maxDefenseValue;
            m_defenseProgressText.text = m_refCharacter.Defense + " / " + _maxDefenseValue;
            m_speedProgressBarImage.fillAmount = (float)m_refCharacter.Speed / (float)_maxSpeedValue;
            m_speedProgressText.text = m_refCharacter.Speed + " / " + _maxSpeedValue;
        }

        protected override void OnShown()
        {
        }

        protected override void OnHidden()
        {
        }
    }
}
