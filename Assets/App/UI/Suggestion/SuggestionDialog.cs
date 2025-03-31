using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using UnityEngine;

namespace App.UI.Suggestion
{
    public class SuggestionDialog : LayerContent
    {
        [SerializeField] private Button _closeBtn1;
        [SerializeField] private Button _closeBtn2;

        void Awake()
        {
            _closeBtn1.AddClick(OnClickCloseBtn);
            _closeBtn2.AddClick(OnClickCloseBtn);
        }

        private void OnClickCloseBtn()
        {
            Close();
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(LayerTag.Popup, "ui/suggestion/SuggestionDialog");
        }
    }
}
