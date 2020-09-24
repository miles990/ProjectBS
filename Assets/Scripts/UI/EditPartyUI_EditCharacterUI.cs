using KahaGameCore.Static;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ProjectBS.UI
{
    public class EditPartyUI_EditCharacterUI : MonoBehaviour
    {
        public event Action OnEditEnded = null;
        public event Action<Data.OwningCharacterData> OnStartEditParty = null;

        [SerializeField] private Text m_nameText = null;
        [SerializeField] private Text m_statusText = null;
        [SerializeField] private InputField m_skill0Field = null;
        [SerializeField] private Text m_skill0NameText = null;
        [SerializeField] private InputField m_skill1Field = null;
        [SerializeField] private Text m_skill1NameText = null;
        [SerializeField] private InputField m_skill2Field = null;
        [SerializeField] private Text m_skill2NameText = null;
        [SerializeField] private InputField m_skill3Field = null;
        [SerializeField] private Text m_skill3NameText = null;

        private Data.OwningCharacterData m_referencingCharacterData = null;

        private void Awake()
        {
            m_skill0Field.onEndEdit.AddListener(Skill0_OnEdited);
            m_skill1Field.onEndEdit.AddListener(Skill1_OnEdited);
            m_skill2Field.onEndEdit.AddListener(Skill2_OnEdited);
            m_skill3Field.onEndEdit.AddListener(Skill3_OnEdited);
        }

        private void OnEnable()
        {
            if (m_referencingCharacterData == null)
                throw new System.Exception("[EditPartyUI_EditCharacterUI][OnEnable] Need to set up referencingCharacterData first");
        }

        private void OnDisable()
        {
            m_referencingCharacterData = null;
        }

        public void SetUp(Data.OwningCharacterData characterData)
        {
            m_referencingCharacterData = characterData;

            m_nameText.text = ContextConverter.Instance.GetContext(characterData.CharacterNameID);
            m_statusText.text = string.Format("HP:{0}({1})\nSP:{2}\nAttack:{3}({4})\nDefence:{5}({6})\nSpeed:{7}({8})",
                characterData.HP,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.HPAbilityID).RankString,
                characterData.SP,
                characterData.Attack,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.AttackAbilityID).RankString,
                characterData.Defence,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.DefenceAbilityID).RankString,
                characterData.Speed,
                GameDataManager.GetGameData<Data.AbilityData>(characterData.SpeedAbilityID).RankString);

            m_skill0Field.text = characterData.SkillSlot_0 == 0 ? "" : characterData.SkillSlot_0.ToString();
            m_skill0NameText.text = characterData.SkillSlot_0 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_0).NameContextID);
            m_skill1Field.text = characterData.SkillSlot_1 == 0 ? "" : characterData.SkillSlot_1.ToString();
            m_skill1NameText.text = characterData.SkillSlot_1 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_1).NameContextID);
            m_skill2Field.text = characterData.SkillSlot_2 == 0 ? "" : characterData.SkillSlot_2.ToString();
            m_skill2NameText.text = characterData.SkillSlot_2 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_2).NameContextID);
            m_skill3Field.text = characterData.SkillSlot_3 == 0 ? "" : characterData.SkillSlot_3.ToString();
            m_skill3NameText.text = characterData.SkillSlot_3 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(characterData.SkillSlot_3).NameContextID);
        }

        private void Skill0_OnEdited(string value)
        {
            if(int.TryParse(value, out int intValue))
            {
                m_referencingCharacterData.SkillSlot_0 = intValue;
                m_skill0NameText.text = m_referencingCharacterData.SkillSlot_0 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_referencingCharacterData.SkillSlot_0).NameContextID);
            }
        }

        private void Skill1_OnEdited(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                m_referencingCharacterData.SkillSlot_1 = intValue;
                m_skill1NameText.text = m_referencingCharacterData.SkillSlot_1 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_referencingCharacterData.SkillSlot_1).NameContextID);
            }
        }

        private void Skill2_OnEdited(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                m_referencingCharacterData.SkillSlot_2 = intValue; 
                m_skill2NameText.text = m_referencingCharacterData.SkillSlot_2 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_referencingCharacterData.SkillSlot_2).NameContextID);
            }
        }

        private void Skill3_OnEdited(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                m_referencingCharacterData.SkillSlot_3 = intValue;
                m_skill3NameText.text = m_referencingCharacterData.SkillSlot_3 == 0 ? "" : ContextConverter.Instance.GetContext(GameDataManager.GetGameData<Data.SkillData>(m_referencingCharacterData.SkillSlot_3).NameContextID);
            }
        }

        public void Button_EndEdit()
        {
            PlayerManager.Instance.SavePlayer();
            OnEditEnded?.Invoke();
            gameObject.SetActive(false);
        }

        public void Button_StartEditParty()
        {
            PlayerManager.Instance.SavePlayer();
            OnStartEditParty?.Invoke(m_referencingCharacterData);
            gameObject.SetActive(false);
        }
    }
}
