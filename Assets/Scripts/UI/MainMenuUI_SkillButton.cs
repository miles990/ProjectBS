using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ProjectBS.UI
{
    public class MainMenuUI_SkillButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private TextMeshProUGUI m_descriptionText = null;
        [SerializeField] private TextMeshProUGUI m_amoumtText = null;

        public void SetUp(Data.OwningSkillData skillData)
        {
            Data.SkillData _source = skillData.GetSourceData();

            m_nameText.text = ContextConverter.Instance.GetContext(_source.NameContextID);
            m_descriptionText.text = _source.GetAllDescriptionContext();
            m_amoumtText.text = "x " + skillData.Amount;
        }
    }
}

