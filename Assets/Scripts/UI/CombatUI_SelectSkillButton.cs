using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProjectBS.Combat
{
    public class CombatUI_SelectSkillButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event Action<int> OnSelected = null;
        public event Action<int> OnShownDetailCommanded = null;

        [SerializeField] private Text m_skillText = null;
        [SerializeField] private Image m_buttonImage = null;
        [SerializeField] private Color m_enableColor = Color.white;
        [SerializeField] private Color m_disableColor = Color.gray;
        [SerializeField] private int m_index = 0;

        private float m_showInfoTimer = 0f;
        private bool m_isEnable = false;

        public void SetUp(string skillName, int sp)
        {
            m_skillText.text = skillName + "\nSP: " + sp;
        }

        public void EnableButton(bool enable)
        {
            m_buttonImage.color = enable ? m_enableColor : m_disableColor;
            m_isEnable = enable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataLoader.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(m_showInfoTimer > 0f)
            {
                if(m_isEnable) OnSelected?.Invoke(m_index);
            }

            m_showInfoTimer = 0f;
        }

        private void Update()
        {
            if(m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if(m_showInfoTimer <= 0f)
                {
                    m_showInfoTimer = 0f;
                    OnShownDetailCommanded?.Invoke(m_index);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }
    }
}
