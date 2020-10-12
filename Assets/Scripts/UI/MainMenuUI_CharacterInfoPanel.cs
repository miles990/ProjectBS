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
            ChangingEquipment
        }

        private const string HP_FORMAT = "HP: {0}({1})\nSP: {2}";
        private const string ABILITY_FORMAT = "Attack: {0} ({1})\nDefense: {2} ({3})\nSpeed: {4} ({5})";

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
        [SerializeField] private Button m_changeEquipmentPanel_nextPageButton = null;
        [SerializeField] private Button m_changeEquipmentPanel_previousPageButton = null;
        [SerializeField] private Text m_changeEquipmentPanel_beforeText = null;
        [SerializeField] private Text m_changeEquipmentPanel_afterText = null;

        private Data.OwningCharacterData m_refCharacter = null;
        private Dictionary<Text, Data.OwningEquipmentData> m_changeEquipmentPanelEquipmentListToEquipment = new Dictionary<Text, Data.OwningEquipmentData>();

        private State m_currentState = State.None;
        private int m_currentPage = 0;
        private string m_currentEquipmentType = Keyword.Head;
        private Data.OwningEquipmentData m_currrentSelectEquipment = null;

        public void Enable(Data.OwningCharacterData characterData)
        {
            m_refCharacter = characterData;
            RefreshInfo();
            m_changeEquipmentPanelRoot.SetActive(false);
            m_currentState = State.None;
            gameObject.SetActive(true);
        }

        public void Button_StartChangeEquipment(string equipmentType)
        {
            m_currentPage = 0;
            m_currentState = State.ChangingEquipment;
            m_currrentSelectEquipment = null;
            RefreshChangeEquipmentPanel(equipmentType);
            m_changeEquipmentPanelRoot.SetActive(true);
        }

        public void Button_ChangeEquipment_GoNextPage()
        {
            m_currentPage++;
            RefreshChangeEquipmentPanel(m_currentEquipmentType);
        }

        public void Button_ChangeEquipment_GoPreviousPage()
        {
            m_currentPage--;
            m_currentPage = m_currentPage < 0 ? 0 : m_currentPage;
            RefreshChangeEquipmentPanel(m_currentEquipmentType);
        }

        public void Button_ChangeEquipment_Select(Text key)
        {
            if(m_changeEquipmentPanelEquipmentListToEquipment.ContainsKey(key))
            {
                m_currrentSelectEquipment = m_changeEquipmentPanelEquipmentListToEquipment[key];
            }

            RefreshChangeEquipmentPanel(m_currentEquipmentType);
        }

        public void Button_Back()
        {
            switch(m_currentState)
            {
                case State.None:
                    {
                        gameObject.SetActive(false);
                        break;
                    }
                case State.ChangingEquipment:
                    {
                        m_changeEquipmentPanelRoot.SetActive(false);
                        m_currentState = State.None;
                        break;
                    }
            }
        }

        private void RefreshChangeEquipmentPanel(string equipmentType)
        {
            m_currentEquipmentType = equipmentType;

            List<Data.OwningEquipmentData> _equipments = PlayerManager.Instance.GetEquipmentsByType(m_currentEquipmentType);
            m_changeEquipmentPanel_previousPageButton.interactable = m_currentPage != 0;
            m_changeEquipmentPanel_nextPageButton.interactable = m_changeEquipmentPanel_equipmentList.Length * (m_currentPage + 1) < PlayerManager.Instance.Player.Equipments.Count;
            for (int i = 0; i < m_changeEquipmentPanel_equipmentList.Length; i++)
            {
                if (i + m_changeEquipmentPanel_equipmentList.Length * m_currentPage >= _equipments.Count)
                {
                    m_changeEquipmentPanel_equipmentList[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    m_changeEquipmentPanel_equipmentList[i].text = string.Format("{0}\nHP{1}\nAttack{2}\nDefense{3}\nSpeed{4}",
                                                             ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.RawEquipmentData>(_equipments[i].EquipmentSourceID).NameContextID),
                                                             _equipments[i].HP >= 0 ? "+" + _equipments[i].HP : _equipments[i].HP.ToString(),
                                                             _equipments[i].Attack >= 0 ? "+" + _equipments[i].Attack : _equipments[i].Attack.ToString(),
                                                             _equipments[i].Defense >= 0 ? "+" + _equipments[i].Defense : _equipments[i].Defense.ToString(),
                                                             _equipments[i].Speed >= 0 ? "+" + _equipments[i].Speed : _equipments[i].Speed.ToString());

                    if (m_changeEquipmentPanelEquipmentListToEquipment.ContainsKey(m_changeEquipmentPanel_equipmentList[i]))
                    {
                        m_changeEquipmentPanelEquipmentListToEquipment[m_changeEquipmentPanel_equipmentList[i]] = _equipments[i];
                    }
                    else
                    {
                        m_changeEquipmentPanelEquipmentListToEquipment.Add(m_changeEquipmentPanel_equipmentList[i], _equipments[i]);
                    }

                    m_changeEquipmentPanel_equipmentList[i].transform.parent.gameObject.SetActive(true);
                }
            }

            m_changeEquipmentPanel_beforeText.text = string.Format("HP: {0}\nSP: {1}\nAttack: {2}\nDefense: {3}\nSpeed: {4}",
                                                        m_refCharacter.GetTotal(Keyword.HP),
                                                        m_refCharacter.GetTotal(Keyword.SP),
                                                        m_refCharacter.GetTotal(Keyword.Attack),
                                                        m_refCharacter.GetTotal(Keyword.Defense),
                                                        m_refCharacter.GetTotal(Keyword.Speed));

            if(m_currrentSelectEquipment == null)
            {
                m_changeEquipmentPanel_afterText.text = m_changeEquipmentPanel_beforeText.text;
            }
            else
            {
                m_changeEquipmentPanel_afterText.text = string.Format("HP: {0}\nSP: {1}\nAttack: {2}\nDefense: {3}\nSpeed: {4}",
                                            m_refCharacter.HP + m_currrentSelectEquipment.HP,
                                            m_refCharacter.SP + m_currrentSelectEquipment.SP,
                                            m_refCharacter.Attack + m_currrentSelectEquipment.Attack,
                                            m_refCharacter.Defense + m_currrentSelectEquipment.Defense,
                                            m_refCharacter.Speed + m_currrentSelectEquipment.Speed);
            }
        }

        private void RefreshInfo()
        {
            Data.OwningEquipmentData _head = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Head);
            int _headNameID = _head != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_head.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _body = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Body);
            int _bodyNameID = _body != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_body.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _hand = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Hand);
            int _handNameID = _head != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_hand.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _foot = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Foot);
            int _footNameID = _foot != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_foot.EquipmentSourceID).NameContextID : 0;

            m_headEquipmentText.text = ContextConverter.Instance.GetContext(_headNameID);
            m_bodyEquipmentText.text = ContextConverter.Instance.GetContext(_bodyNameID);
            m_handEquipmentText.text = ContextConverter.Instance.GetContext(_headNameID);
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
