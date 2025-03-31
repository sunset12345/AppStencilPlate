using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using OSDev.UI.Layer;
using TMPro;
using UnityEngine;

namespace App.UI.Common
{
    public class CommonPop : PopupLayerContent
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _contentText;
        [SerializeField] private Button _confirmBtn;
        [SerializeField] private Button _cancelBtn;

        public void SetInfo(
            string title,
            string content,
            System.Action onConfirm = null,
            System.Action onCancel = null)
        {
            _titleText.text = title;
            _contentText.text = content;
            _confirmBtn.AddClick(() =>
            {
                onConfirm?.Invoke();
                Close();
            });
            _cancelBtn.AddClick(() =>
            {
                onCancel?.Invoke();
                Close();
            });
        }

        public static CommonPop Create(
            string title,
            string content,
            System.Action onConfirm = null,
            System.Action onCancel = null)
        {
            var popup = LayerManager.Instance.LoadContent(LayerTag.Popup, "ui/common/CommonPop") as CommonPop;
            popup.SetInfo(
                title,
                content,
                onConfirm,
                onCancel);
            return popup;
        }
    }
}
