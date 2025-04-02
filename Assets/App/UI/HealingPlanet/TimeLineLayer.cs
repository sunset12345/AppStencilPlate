using System.Collections.Generic;
using App.DataCache;
using App.LoadingFunction;
using DFDev.UI;
using DFDev.UI.Layer;
using UnityEngine;

namespace App.UI.HealingPlanet
{
    public class TimeLineLayer : LayerContent
    {
        [SerializeField] private Transform _timeLineRoot;
        [SerializeField] private TimeLineItem _prefab;

        [SerializeField] private Button _closeBtn;

        void Awake()
        {
            _closeBtn.AddClick(Close);
        }

        void Start()
        {
            var tiemLineList = DataCache.DataCache.Load<List<TimeLineData>>(DataEnum.TimeLine.ToString(), new List<TimeLineData>());
            foreach(var timeLine in tiemLineList)
            {
                var item = Instantiate(_prefab, _timeLineRoot);
                item.SetInfo(timeLine);
            }
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/healingplanet/TimeLineLayer"
            );
        }
    }
}
