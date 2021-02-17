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

        private Action m_onConfirmed = null;

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
        }

        public override void SetMessage(string content, string title, Action onConfirmed)
        {
            m_modernUI.descriptionText = content;
            m_modernUI.titleText = title;
            m_onConfirmed = onConfirmed;
        }

        private void OnConfirmed()
        {
            m_onConfirmed?.Invoke();
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
