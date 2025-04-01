using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using UnityEngine;

namespace App.UI.AivanaPlanet
{
    public class HealingPlanet : LayerContent
    {
        [SerializeField] private Button _entranceBtn;

        void Awake()
        {
            _entranceBtn.AddClick(OnClickHealingPlanet);
        }

        private void OnClickHealingPlanet()
        {

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
