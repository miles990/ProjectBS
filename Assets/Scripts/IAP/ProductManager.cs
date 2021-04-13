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
            if (m_isInited)
                return;

            ConfigurationBuilder _builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            _builder.AddProduct("com.MarsImapct.ProjectBS.Test0", ProductType.Consumable);
            _builder.AddProduct("com.MarsImapct.ProjectBS.Test1", ProductType.NonConsumable);
            _builder.AddProduct("com.MarsImapct.ProjectBS.Test2", ProductType.Subscription);

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
            throw new NotImplementedException();
        }

        void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (m_isInited)
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_storeController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    UnityEngine.Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_storeController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    UnityEngine.Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                UnityEngine.Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!m_isInited)
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                UnityEngine.Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer ||
                UnityEngine.Application.platform == UnityEngine.RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                UnityEngine.Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_extensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    UnityEngine.Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                UnityEngine.Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + UnityEngine.Application.platform);
            }
        }
    }
}
