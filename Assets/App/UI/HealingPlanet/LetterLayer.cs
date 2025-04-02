using System;
using App.DataCache;
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

        /// <summary>
        /// Called when the send button is clicked.
        /// </summary>
        private void OnClickSend()
        {
            if (string.IsNullOrEmpty(_InputField.text))
            {
                // Show an error message if the input is empty
                CommonMessageTip.Create("The input cannot be Empty");
                return;
            }
            var timeLineData = new TimeLineData(
                DataCache.DataCache.Load<string>(DataEnum.UserName.ToString(), "user"),
                DateTime.Now.ToString("yyyy-MM-dd"),
                _InputField.text
            );
            DataCacheManager.Instance.SaveTimeLine(timeLineData);

            CommonMessageTip.Create("Published successfully");
            _InputField.text = "";
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
