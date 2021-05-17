using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectBS.UI
{
    public abstract class MainMenuUI_CustomButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEngine.UI.ScrollRect refScrollRect;

        public bool IsHide { get { return m_hidePartRoot != null && !m_hidePartRoot.activeSelf; } }

        [SerializeField] private GameObject m_hidePartRoot = null;
        [SerializeField] private RectTransform m_rectTransform = null;

        private float m_longPressTimer;
        private Vector2 m_touchDownPos;

        public void OnPointerDown(PointerEventData eventData)
        {
            m_touchDownPos = eventData.position;
            m_longPressTimer = GameDataManager.GameProperties.PressDownShowInfoTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Vector2.Distance(m_touchDownPos, eventData.position) <= 2f && m_longPressTimer > 0f)
            {
                OnPressed();
            }

            m_longPressTimer = -1f;
        }

        protected abstract void OnPressed();

        public float GetStartNormalizeHeight()
        {
            if (refScrollRect == null)
                return 1f;

            return 1f - ((m_rectTransform.anchoredPosition.y + 15f) / -refScrollRect.content.sizeDelta.y);
        }

        public float GetEndNormalizeHeight()
        {
            if (refScrollRect == null)
                return 0f;

            return 1f - ((m_rectTransform.anchoredPosition.y - m_rectTransform.sizeDelta.y) / -refScrollRect.content.sizeDelta.y);
        }

        public bool IsOverMiddle { get; private set; }

        private void Update()
        {
            if(refScrollRect != null)
            {
                float _totalHeight = 0f - (refScrollRect.content.rect.height - refScrollRect.viewport.rect.height);
                float _topPos = _totalHeight * (1f - refScrollRect.normalizedPosition.y);
                float _downPos = _topPos - refScrollRect.viewport.rect.height;

                if (m_rectTransform.anchoredPosition.y < _downPos)
                {
                    m_hidePartRoot.SetActive(false);
                }
                else if(m_rectTransform.anchoredPosition.y - m_rectTransform.sizeDelta.y > _topPos)
                {
                    m_hidePartRoot.SetActive(false);
                }
                else
                {
                    m_hidePartRoot.SetActive(true);
                }

                IsOverMiddle = (m_rectTransform.anchoredPosition.y - m_rectTransform.sizeDelta.y / 2f) > _topPos;
            }

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
