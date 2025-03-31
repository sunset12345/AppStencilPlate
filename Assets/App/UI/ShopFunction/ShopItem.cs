using App.IAP;
using DFDev.UI;
using TMPro;
using UnityEngine;
namespace App.UI.ShopFunction
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private Button _buyBtn;
        [SerializeField] private TextMeshProUGUI _count;
        [SerializeField] private TextMeshProUGUI _price;

        private InappConfig _inappConfig;

        void Awake()
        {
            _buyBtn.AddClick(OnClickBuyBtn);
        }

        private void OnClickBuyBtn()
        {
            IAPManager.Instance.Purchase(_inappConfig);
        }

        public void SetInfo(InappConfig inappConfig)
        {
            _inappConfig = inappConfig;
            _count.text = inappConfig.DiamondCount.ToString();
            _price.text = IAPManager.Instance.GetLocalizedPrice(inappConfig);
        }
    }
}
