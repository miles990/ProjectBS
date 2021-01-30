using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectBS.UI
{
    public abstract class MainMenuUI_CustomButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Vector2 m_touchDownPos;

        public void OnPointerDown(PointerEventData eventData)
        {
            m_touchDownPos = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Vector2.Distance(m_touchDownPos, eventData.position) <= 2f)
            {
                OnPressed();
            }
        }

        protected abstract void OnPressed();
    }
}
