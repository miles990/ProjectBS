using UnityEngine.UI;
using UnityEngine;
using KahaGameCore.Static;

namespace ProjectBS.UI
{
    public class MainMenuUI_SkillButton : MonoBehaviour
    {
        [SerializeField] private Text m_nameText = null;
        [SerializeField] private Text m_descriptionText = null;
        [SerializeField] private Text m_amoumtText = null;

        public void SetUp(Data.OwningSkillData skillData)
        {
            Data.SkillData _source = GameDataManager.GetGameData<Data.SkillData>(skillData.SkillSourceID);
            m_nameText.text = ContextConverter.Instance.GetContext(_source.NameContextID);
            m_descriptionText.text = ContextConverter.Instance.GetContext(_source.DescriptionContextID);
            m_amoumtText.text = "x" + skillData.Amount;
        }
    }
}

