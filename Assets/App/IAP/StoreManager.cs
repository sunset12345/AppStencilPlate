using App.DataCache;
using App.UI.Common;
using DFDev.EventSystem;
using DFDev.Singleton;
using UnityEngine.Purchasing;

namespace App.IAP
{
    public class StoreManager : Singleton<StoreManager>, IEventSender
    {
        EventDispatcher IEventSender.Dispatcher => EventDispatcher.Global;

        public void PurchaseCallBack(
            bool result,
            Product product,
            int code)
        {
            if (result)
            {
                var config = IAPManager.GetStoreConfig(product.definition.id);
                DataCacheManager.Instance.AddDiamond(config.DiamondCount);
                // LoginManager.Instance.ApplyPay(product);
                CommonMessageTip.Create("Purchase Success");
            }
            else
                CommonMessageTip.Create("Purchase failed");
        }
    }
}

