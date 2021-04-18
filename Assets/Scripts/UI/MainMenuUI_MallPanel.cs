using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectBS.UI
{
    public class MainMenuUI_MallPanel : MainMenuUI_PanelBase
    {
        protected override void OnHidden()
        {
        }

        protected override void OnShown()
        {
        }

        public void Purchase(string id)
        {
            IAP.ProductManager.Instance.BuyProductID(id);
        }
    }
}
