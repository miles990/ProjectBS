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

        private bool m_defaultCancel = false;

        public override bool IsShowing 
        {
            get
            {
                return m_modernUI.isOn;
            }
        }

        public override void SetMessage(string content, string title, Action onConfirmed)
        {
            m_modernUI.descriptionText = content;
            m_modernUI.titleText = title;
            m_onConfirmed = onConfirmed;
            m_cancelButtonObj.SetActive(false);
            m_defaultCancel = false;
        }

        public void SetCancelableMessage(string content, string title, Action onConfirmed, Action onCanceled)
        {
            m_modernUI.descriptionText = content;
            m_modernUI.titleText = title;
            m_onConfirmed = onConfirmed;
            m_onCanceled = onCanceled;
            m_cancelButtonObj.SetActive(true);
            m_defaultCancel = true;
        }

        public void Confirm()
        {
            m_onConfirmed?.Invoke();
        }

        public void Cancel()
        {
            m_onCanceled?.Invoke();
        }

        public void ClickBackground()
        {
            if (m_defaultCancel)
                Cancel();
            else
                Confirm();
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
