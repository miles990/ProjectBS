using System;

namespace ProjectBS.UI
{
    public class ConfirmWindowUIView : KahaGameCore.Common.ConfirmWindowBase
    {
        protected override void OnStartToShow(bool show, Action onShown)
        {
            m_root.SetActive(show);
            onShown?.Invoke();
        }
    }
}
