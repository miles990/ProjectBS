using System;
using UnityEngine;
using Michsky.UI.ModernUIPack;

namespace ProjectBS.UI
{
    public class ConfirmWindowUIView : KahaGameCore.Common.ConfirmWindowBase
    {
        [Space(50)]
        [Header("=============Do not use above=============")]
        [SerializeField] private ModalWindowManager m_modernUI = null;
        [SerializeField] private GameObject m_cancelButtonObj = null;

        private Action m_onConfirmed = null;
        private Action m_onCanceled = null;

        public override bool IsShowing 
        {
            get
            {
                return m_modernUI.isOn;
            }
        }

        private void Start()
        {
            m_modernUI.onConfirm.AddListener(OnConfirmed);
            m_modernUI.onCancel.AddListener(OnCanceled);
        }

        public override void SetMessage(string content, string title, Action onConfirmed)
        {
            m_modernUI.descriptionText = content;
            m_modernUI.titleText = title;
            m_onConfirmed = onConfirmed;
            m_cancelButtonObj.SetActive(false);
        }

        public void SetCancelableMessage(string content, string title, Action onConfirmed, Action onCanceled)
        {
            m_modernUI.descriptionText = content;
            m_modernUI.titleText = title;
            m_onConfirmed = onConfirmed;
            m_onCanceled = onCanceled;
            m_cancelButtonObj.SetActive(true);
        }

        private void OnConfirmed()
        {
            m_onConfirmed?.Invoke();
        }

        private void OnCanceled()
        {
            m_onCanceled?.Invoke();
        }

        protected override void OnStartToShow(bool show, Action onShown)
        {
            if(show)
            {
                m_modernUI.UpdateUI();
                m_modernUI.OpenWindow();
            }
            else
            {
                m_modernUI.CloseWindow();
            }
            onShown?.Invoke();
        }
    }
}
