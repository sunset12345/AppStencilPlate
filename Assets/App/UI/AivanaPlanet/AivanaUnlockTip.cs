using System;
using App.Config;
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
            _closeBtn.AddClick(Close);
        }

        private void OnClickUnlockBtn()
        {
            Close();
            _unlockAction?.Invoke();
        }

        public void SetInfo(AiConfig aiConfig, Action unlockAction)
        {
            _unlockAction = unlockAction;
            _unlockText.text = string.Format(
                ConfigManager.Instance.GetConfig<Const>().UnlockAiTitle,
                aiConfig.UnlockCost);
        }

        public static void Create(AiConfig aiConfig, Action unlockAction)
        {
            var unlockTip = LayerManager.Instance.LoadContent(
                LayerTag.Tip,
                 "ui/aivanaplanet/AivanaUnlockTip") as AivanaUnlockTip;
            unlockTip.SetInfo(aiConfig, unlockAction);
        }
    }
}
