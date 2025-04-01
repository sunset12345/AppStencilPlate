using System;
using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;
namespace App.UI.AivanaPlanet
{
    public class AivanaUnlockTip : LayerContent
    {
        [SerializeField] private Button _unlockBtn;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private TextMeshProUGUI _unlockText;

private Action _unlockAction;
        void Awake()
        {
            _unlockBtn.AddClick(OnClickUnlockBtn);
        }

        private void OnClickUnlockBtn()
        {
            Close();
            _unlockAction?.Invoke();
        }

        public void SetInfo(AiConfig aiConfig, Action unlockAction)
        {
            _unlockAction = unlockAction;
            // _unlockText.text = string.Format(
            //     LanguageManager.Instance.GetString("AivanaUnlockTip"),
            //     aiConfig.Name);
        }

        public static void Create(AiConfig aiConfig, Action unlockAction)
        {
            var unlockTip = LayerManager.Instance.LoadContent(
                LayerTag.Tip,
                 "ui/aivanaPlanet/AivanaUnlockTip") as AivanaUnlockTip;
            unlockTip.SetInfo(aiConfig, unlockAction);
        }
    }
}
