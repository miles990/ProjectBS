using UnityEngine.UI;
using UnityEngine;

namespace ProjectBS.UI
{
    public class MainMenuUI_SkillButton : MonoBehaviour
    {
        [SerializeField] private Text m_nameText = null;
        [SerializeField] private Text m_descriptionText = null;
        [SerializeField] private Text m_amoumtText = null;

        public void SetUp(Data.OwningSkillData skillData)
        {
            Data.SkillData _source = skillData.GetSourceData();

            m_nameText.text = ContextConverter.Instance.GetContext(_source.NameContextID);
            m_descriptionText.text = _source.GetAllDescriptionContext();
            m_amoumtText.text = "x" + skillData.Amount;
        }
    }
}

