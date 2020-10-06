using KahaGameCore.Static;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterButton : MonoBehaviour
    {
        private const string ABILITY_FORMAT = "HP: {0} ({1})\nAttack: {2} ({3})\nDefense: {4} ({5})\nSpeed: {6} ({7})";

        [SerializeField] private Text m_nameAndLevelText = null;
        [SerializeField] private Text m_abilityText = null;
        [SerializeField] private Text m_equipmentText = null;
        [SerializeField] private Text m_skillText = null;
        [SerializeField] private GameObject m_partyHintRoot = null;
        [SerializeField] private Text m_partyHintText = null;

        private Data.OwningCharacterData m_refCharacter = null;

        public void SetUp(Data.OwningCharacterData characterData)
        {
            m_refCharacter = characterData;
            RefreshInfo();
        }

        public void RefreshInfo()
        {
            m_nameAndLevelText.text = ContextConverter.Instance.GetContext(m_refCharacter.CharacterNameID) + "\nLevel " + m_refCharacter.Level;
            m_abilityText.text = string.Format(ABILITY_FORMAT,
                                        m_refCharacter.GetTotal(Keyword.HP),
                                        GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.HPAbilityID).RankString,
                                        m_refCharacter.GetTotal(Keyword.Attack),
                                        GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.AttackAbilityID).RankString,
                                        m_refCharacter.GetTotal(Keyword.Defense),
                                        GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.DefenseAbilityID).RankString,
                                        m_refCharacter.GetTotal(Keyword.Speed),
                                        GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.SpeedAbilityID).RankString);

            Data.OwningEquipmentData _head = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Head);
            int _headNameID = _head != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_head.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _body = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Body);
            int _bodyNameID = _body != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_body.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _hand = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Hand);
            int _handNameID = _head != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_hand.EquipmentSourceID).NameContextID : 0;
            Data.OwningEquipmentData _foot = PlayerManager.Instance.GetEquipmentByUDID(m_refCharacter.Equipment_UDID_Foot);
            int _footNameID = _foot != null ? GameDataManager.GetGameData<Data.RawEquipmentData>(_foot.EquipmentSourceID).NameContextID : 0;

            m_equipmentText.text = ContextConverter.Instance.GetContext(_headNameID) + "\n" +
                                   ContextConverter.Instance.GetContext(_bodyNameID) + "\n" +
                                   ContextConverter.Instance.GetContext(_handNameID) + "\n" +
                                   ContextConverter.Instance.GetContext(_footNameID);

            m_skillText.text = ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_0).NameContextID) + "\n" +
                               ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_1).NameContextID) + "\n" +
                               ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_2).NameContextID) + "\n" +
                               ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_refCharacter.SkillSlot_3).NameContextID);

            int _partyIndex = PlayerManager.Instance.GetPartyIndex(m_refCharacter);
            if (_partyIndex != -1)
            {
                m_partyHintText.text = (_partyIndex+1).ToString();
                m_partyHintRoot.SetActive(true);
            }
            else
            {
                m_partyHintRoot.SetActive(false);
            }
        }
    }
}
