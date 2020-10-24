using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_CharacterInfoPanel_SkilltButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private Text m_nameText = null;

        private Data.SkillData m_referenceSkill = null;
        private float m_showInfoTimer = 0f;

        public void SetUp(Data.SkillData data)
        {
            m_referenceSkill = data;
            m_nameText.text = ContextConverter.Instance.GetContext(data.NameContextID);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataLoader.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }

        private void Update()
        {
            if (m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if (m_showInfoTimer <= 0f)
                {
                    m_showInfoTimer = 0f;
                    string _name = ContextConverter.Instance.GetContext(m_referenceSkill.NameContextID);

                    GameManager.Instance.MessageManager.ShowCommonMessage(
                        m_referenceSkill.GetAllDescriptionContext(),
                        _name, null);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }
    }
}
