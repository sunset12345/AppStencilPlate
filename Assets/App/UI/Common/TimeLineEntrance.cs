using App.UI.HealingPlanet;
using DFDev.UI;
using UnityEngine;

namespace App.Common
{
    public class TimeLineEntrance : MonoBehaviour
    {
        [SerializeField]
        private Button _entranceBtn;

        void Awake()
        {
            _entranceBtn.AddClick(()=>{
                TimeLineLayer.Create();
            });
        }
    }
}