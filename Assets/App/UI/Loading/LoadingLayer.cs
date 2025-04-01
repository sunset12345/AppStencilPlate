using App.LoadingFunction;
using DFDev.UI.Layer;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Loading
{

    public class LoadingLayer : LayerContent
    {
        [SerializeField] private Image _progress;
        [SerializeField] private TextMeshProUGUI _value;
        [SerializeField] private TextMeshProUGUI _info;

        void Start()
        {
            LoadingManager.Instance.Progress
               .Subscribe(OnLoadingProgressUpdated)
               .AddTo(this);
            LoadingManager.Instance.Info
                .Subscribe(OnLoadingInfoUpdated)
                .AddTo(this);
        }

        void OnEnable()
        {
            _info.text = "Loading...";
            _value.text = "0.0%";
        }

        private void OnLoadingProgressUpdated(float progress)
        {
            _progress.transform.SetWidth(progress * 375f);
            _value.text = $"{progress:P2}";
        }

        private void OnLoadingInfoUpdated(string info)
        {
            _info.text = info;
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Loading,
                "ui/loading/LoadingLayer");
        }
    }
}
