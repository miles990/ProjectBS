using KahaGameCore.Static;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterInfoPanel : MonoBehaviour
    {
        private const string HP_FORMAT = "HP: {0}({1})\nSP: 100";
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

        private Data.OwningCharacterData m_refCharacter = null;

        public void Enable(Data.OwningCharacterData characterData)
        {
            m_refCharacter = characterData;
            RefreshInfo();
            gameObject.SetActive(true);
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
                                GameDataManager.GetGameData<Data.AbilityData>(m_refCharacter.HPAbilityID).RankString);
        }
    }
}
