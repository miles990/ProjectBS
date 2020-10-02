using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KahaGameCore.Interface;

namespace ProjectBS.UI
{
    public class MainMenuUIManager : Manager
    {
        public enum UIPage
        {
            None,
            EditPatyUI
        }

        private Dictionary<UIPage, UIView> m_pageToUI = new Dictionary<UIPage, UIView>();

        public MainMenuUIManager()
        {
        }

        public void Show(UIPage page)
        {
            DeshowAll();
            if(page != UIPage.None)
                m_pageToUI[page].Show(this, true, null);
        }

        public void DeshowAll()
        {
            foreach(KeyValuePair<UIPage, UIView> keyValuePair in m_pageToUI)
            {
                keyValuePair.Value.Show(this, false, null);
            }
        }
    }
}

