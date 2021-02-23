using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectBS.UI
{
    public class CombatUI_ButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public event Action<int> OnSelected = null;
        protected int Index { get { return m_index; } }

        [SerializeField] private Image m_buttonImage = null;
        [SerializeField] private Color m_enableColor = Color.white;
        [SerializeField] private Color m_disableColor = Color.gray;
        [SerializeField] private int m_index = 0;

        private float m_showInfoTimer = 0f;
        private bool m_isEnable = false;

        public void EnableButton(bool enable)
        {
            m_buttonImage.color = enable ? m_enableColor : m_disableColor;
            m_isEnable = enable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_showInfoTimer = GameDataManager.GameProperties.PressDownShowInfoTime * Time.timeScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (m_showInfoTimer > 0f)
            {
                if (m_isEnable) OnSelected?.Invoke(m_index);
            }

            m_showInfoTimer = 0f;
        }

        protected virtual void Update()
        {
            if (m_showInfoTimer > 0f)
            {
                m_showInfoTimer -= Time.deltaTime;
                if (m_showInfoTimer <= 0f)
                {
                    m_showInfoTimer = 0f;
                    OnNeedToShowDetail();
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_showInfoTimer = 0f;
        }

        protected virtual void OnNeedToShowDetail() { } 
    }
}

