using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class MainMenuUI_ChangeSkillPanel_SkilltButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event Action<Data.OwningSkillData> OnSelected = null;

        [SerializeField] private Text m_nameText = null;

        private Data.OwningSkillData m_referenceSkill = null;
        private float m_showInfoTimer = 0f;

        public void SetUp(Data.OwningSkillData data)
        {
            m_referenceSkill = data;
            m_nameText.text = string.Format("{0} (x{1})\n{2}",
                        ContextConverter.Instance.GetContext(m_referenceSkill.GetSourceData().NameContextID),
                        m_referenceSkill.Amount,
                        ContextConverter.Instance.GetContext(m_referenceSkill.GetSourceData().DescriptionContextID)); ;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataLoader.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_showInfoTimer > 0f)
            {
                OnSelected?.Invoke(m_referenceSkill);
            }
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
                    string _name = ContextConverter.Instance.GetContext(m_referenceSkill.GetSourceData().NameContextID);

                    GameManager.Instance.MessageManager.ShowCommonMessage(
                        m_referenceSkill.GetSourceData().GetAllDescriptionContext(),
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
