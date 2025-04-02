using App.DataCache;
using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using OSDev.UI.Layer;
using TMPro;
using UnityEngine;


namespace App.UI.Common
{
    public class RenamePop : PopupLayerContent
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private Button _confirmBtn;
        [SerializeField] private Button _cancelBtn;

        void Awake()
        {
            _confirmBtn.AddClick(OnClickConfirm);
            _cancelBtn.AddClick(OnClickCancel);
        }

        private void OnClickCancel()
        {
            Close();
        }

        private void OnClickConfirm()
        {
            if (string.IsNullOrEmpty(_inputField.text))
            {
                CommonMessageTip.Create("Please enter a nickname");
                return;
            }

            DataCacheManager.Instance.SetData(DataEnum.UserName.ToString(), _inputField.text);
            Close();
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(LayerTag.Popup, "ui/common/RenamePop");
        }
    }
}
