using UnityEngine.Purchasing;
using System;

namespace ProjectBS.IAP
{
    public class ProductManager : IStoreListener
    {
        public static ProductManager Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new ProductManager();
                }
                return m_instance;
            }
        }
        private static ProductManager m_instance = null;

        private bool m_isInited = false;

        private IStoreController m_storeController = null;
        private IExtensionProvider m_extensionProvider = null;

        private ProductManager() { }

        public void Initialize()
        {
            return;

            if (m_isInited)
                return;

            ConfigurationBuilder _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            _builder.AddProduct("com.MarsImapct.ProjectBS.Exp0", ProductType.Consumable);
            _builder.AddProduct("com.MarsImapct.ProjectBS.Exp1", ProductType.Consumable);
            _builder.AddProduct("com.MarsImapct.ProjectBS.Exp2", ProductType.Consumable);
            _builder.AddProduct("com.MarsImapct.ProjectBS.Exp3", ProductType.Consumable);
            _builder.AddProduct("com.MarsImapct.ProjectBS.Exp4", ProductType.Consumable);
            _builder.AddProduct("com.MarsImapct.ProjectBS.Exp5", ProductType.Consumable);

            UnityPurchasing.Initialize(this, _builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            m_storeController = controller;
            m_extensionProvider = extensions;
            m_isInited = true;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            throw new Exception("[ProductManager][Initialize] OnInitializeFailed: " + error);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            throw new Exception("[ProductManager][Purchase] OnPurchaseFailed: " + product.definition.id + " failureReason=" + failureReason);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            UnityEngine.Debug.Log("Bought " + purchaseEvent.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }

        public void BuyProductID(string productId)
        {
            if (m_isInited)
            {
                Product product = m_storeController.products.WithID(productId);

                if (product != null && product.availableToPurchase)
                {
                    UnityEngine.Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    m_storeController.InitiatePurchase(product);
                }
                else
                {
                    UnityEngine.Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                UnityEngine.Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        public void RestorePurchases()
        {
            if (!m_isInited)
            {
                UnityEngine.Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer ||
                UnityEngine.Application.platform == UnityEngine.RuntimePlatform.OSXPlayer)
            {
                UnityEngine.Debug.Log("RestorePurchases started ...");

                var apple = m_extensionProvider.GetExtension<IAppleExtensions>();
                apple.RestoreTransactions((result) => {
                    UnityEngine.Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            else
            {
                UnityEngine.Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + UnityEngine.Application.platform);
            }
        }
    }
}
