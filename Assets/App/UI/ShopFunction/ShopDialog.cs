using App.Config;
using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using UnityEngine;

namespace App.UI.ShopFunction
{
    public class ShopDialog : LayerContent
    {
        [SerializeField] private Transform _shopItemRoot;
        [SerializeField] private ShopItem _shopItemPrefab;

        [SerializeField] private Button _closeBtn;

        void Awake()
        {
            var InappConfigTable = ConfigManager.Instance.GetConfig<InappConfigTable>();
            foreach (var inappConfig in InappConfigTable.Rows)
            {
                var shopItem = Instantiate(_shopItemPrefab, _shopItemRoot);
                shopItem.SetInfo(inappConfig.Value);
            }
            _closeBtn.AddClick(Close);
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(LayerTag.Popup, "ui/shop/ShopDialog");
        }
    }
}
