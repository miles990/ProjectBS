using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_SkillButton : MainMenuUI_CustomButtonBase
    {
        public event System.Action<Data.OwningSkillData> OnButtonPressed = null;

        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private TextMeshProUGUI m_descriptionText = null;
        [SerializeField] private TextMeshProUGUI m_amoumtText = null;

        private Data.OwningSkillData m_referenceOwingSkill = null;

        public void SetUp(Data.OwningSkillData skillData)
        {
            Data.SkillData _source = skillData.GetSourceData();

            m_nameText.text = ContextConverter.Instance.GetContext(_source.NameContextID);
            m_descriptionText.text = _source.GetAllDescriptionContext();
            m_amoumtText.text = "x " + skillData.Amount;

            m_referenceOwingSkill = skillData;
        }

        public void SetUp(Data.SkillData skillData)
        {
            m_nameText.text = ContextConverter.Instance.GetContext(skillData.NameContextID);
            m_descriptionText.text = skillData.GetAllDescriptionContext();
            m_amoumtText.text = "";

            m_referenceOwingSkill = null;
        }

        protected override void OnPressed()
        {
            if (m_referenceOwingSkill == null) return;

            OnButtonPressed?.Invoke(m_referenceOwingSkill);
        }
    }
}

