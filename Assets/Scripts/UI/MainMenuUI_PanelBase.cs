﻿using UnityEngine;

namespace ProjectBS.UI
{
    public abstract class MainMenuUI_PanelBase : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_root = null;

        private float m_targetValue = 0f;

        public void Show()
        {
            m_targetValue = 1f;
            m_root.gameObject.SetActive(true);
            m_root.blocksRaycasts = true;

            OnShown();
        }

        public void Hide()
        {
            m_targetValue = 0f;
            m_root.blocksRaycasts = false;

            OnHidden();
        }

        protected abstract void OnShown();
        protected abstract void OnHidden();

        private void Update()
        {
            m_root.alpha = Mathf.Lerp(m_root.alpha, m_targetValue, 0.2f);
            if (Mathf.Approximately(m_targetValue, 0f)
                && m_root.alpha <= 0.1)
            {
                m_root.gameObject.SetActive(false);
            }
        }
    }
}
