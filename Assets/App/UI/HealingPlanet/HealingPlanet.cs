using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using UnityEngine;

namespace App.UI.HealingPlanet
{
    public class HealingPlanet : LayerContent
    {
        [SerializeField] private Button _entranceBtn;
        [SerializeField] private Button _closeBtn;

        void Awake()
        {
            _entranceBtn.AddClick(OnClickHealingPlanet);
            _closeBtn.AddClick(Close);
        }

        private void OnClickHealingPlanet()
        {
            Close();
            LetterLayer.Create();
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/healingplanet/HealingPlanet"
            );
        }
    }
}
