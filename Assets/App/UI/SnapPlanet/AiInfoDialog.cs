using App.LoadingFunction;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.SnapPlanet
{
    public class AiInfoDialog : LayerContent
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _des;
        [SerializeField] private Button _followBtn;
        [SerializeField] private Button _chatBtn;
        [SerializeField] private Button _closeBtn;

        [SerializeField] private GameObject[] mask;
        [SerializeField] private Image[] iamge; 

        public void Initialized(int aiId)
        {

        }

        public static void Create(int aiId)
        {
            var dialog = LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/snapplanet/AiInfoDialog"
            ) as AiInfoDialog;
            dialog.Initialized(aiId);
        }
    }
}
