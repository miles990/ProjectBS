using UnityEngine;
using TMPro;
using ProjectBS.Data;

namespace ProjectBS.UI
{
    public class MainMenuUI_SkillButton : MainMenuUI_CustomButtonBase
    {
        public event System.Action<OwningSkillData> OnButtonPressed = null;

        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private TextMeshProUGUI m_descriptionText = null;
        [SerializeField] private TextMeshProUGUI m_amoumtText = null;

        private OwningSkillData m_referenceOwingSkill = null;

        public void SetUp(OwningSkillData skillData)
        {
            SkillData _source = skillData.GetSourceData();

            m_nameText.text = ContextConverter.Instance.GetContext(_source.NameContextID);
            m_descriptionText.text = _source.GetAllDescriptionContext();
            m_amoumtText.text = "x " + skillData.Amount;

            m_referenceOwingSkill = skillData;
        }

        public void SetUp(SkillData skillData)
        {
            m_nameText.text = ContextConverter.Instance.GetContext(skillData.NameContextID);
            m_descriptionText.text = skillData.GetAllDescriptionContext();
            m_amoumtText.text = "";

            m_referenceOwingSkill = new OwningSkillData
            {
                SkillSourceID = skillData.ID,
                Amount = 0
            };
        }

        protected override void OnPressed()
        {
            if (m_referenceOwingSkill == null) return;

            OnButtonPressed?.Invoke(m_referenceOwingSkill);
        }

        protected override void OnLongPressed()
        {
            SkillData _skill = GameDataManager.GetGameData<SkillData>(m_referenceOwingSkill.SkillSourceID);
            string _name = ContextConverter.Instance.GetContext(_skill.NameContextID);

            GameManager.Instance.MessageManager.ShowCommonMessage(
                _skill.GetAllDescriptionContext(),
                _name, null);
        }
    }
}

