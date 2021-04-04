using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectBS.UI
{
    public abstract class MainMenuUI_CustomButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private float m_longPressTimer;
        private Vector2 m_touchDownPos;

        public void OnPointerDown(PointerEventData eventData)
        {
            m_touchDownPos = eventData.position;
            m_longPressTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Vector2.Distance(m_touchDownPos, eventData.position) <= 1f && m_longPressTimer > 0f)
            {
                OnPressed();
            }

            m_longPressTimer = -1f;
        }

        protected abstract void OnPressed();

        private void Update()
        {
            if(m_longPressTimer > 0f)
            {
                m_longPressTimer -= Time.deltaTime;
                if(m_longPressTimer <= 0f)
                {
                    OnLongPressed();
                }
            }
        }

        protected abstract void OnLongPressed();
    }
}
