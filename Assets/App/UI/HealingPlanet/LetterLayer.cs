using App.LoadingFunction;
using App.UI.Common;
using DFDev.UI;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;

namespace App.UI.HealingPlanet
{
    public class LetterLayer : LayerContent
    {
        [SerializeField] private Button _sendButton;

        [SerializeField] private TMP_InputField _InputField;

        [SerializeField] private Button _closeBtn;

        void Awake()
        {
            _sendButton.AddClick(OnClickSend);
            _closeBtn.AddClick(Close);
        }

        private void OnClickSend()
        {
            if(string.IsNullOrEmpty(_InputField.text))
            {
                CommonMessageTip.Create("The input cannot be Empty");
                return;
            }
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/healingplanet/LetterLayer"
            );
        }
    }
}
