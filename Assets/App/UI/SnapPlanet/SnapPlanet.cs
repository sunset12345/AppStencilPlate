using System.Collections.Generic;
using App.Config;
using App.LoadingFunction;
using DFDev.UI.Layer;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.SnapPlanet
{
    public class SnapPlanet : LayerContent
    {
        [SerializeField] private Toggle _followToggle;
        [SerializeField] private GameObject _followSelected;
        [SerializeField] private GameObject _followUnSelected;

        [SerializeField] private Toggle _recmmendToggle;
        [SerializeField] private GameObject _recmmendSelected;
        [SerializeField] private GameObject _recmmendUnSelected;

        [SerializeField] private Transform _photoItemRoot;
        [SerializeField] private PhotoInfoItem _photoInfoItem;
        [SerializeField] private Button _publishBtn;
        private Dictionary<int, PhotoInfoItem> _createCache = new();

        void Awake()
        {
            _publishBtn.AddClick(OnClickPublish);
            _recmmendToggle.onValueChanged.AddListener(OnRecmmendToggle);
            _followToggle.onValueChanged.AddListener(OnFollowToggle);
        }

        private void OnFollowToggle(bool arg0)
        {
            if (_followToggle.isOn && arg0)
                return;
            _followSelected.SetActive(arg0);
            _followUnSelected.SetActive(!arg0);
            _createCache.Clear();
            _photoItemRoot.DetachChildren();
        }

        private void OnRecmmendToggle(bool arg0)
        {
            if (_recmmendToggle.isOn && arg0)
                return;
            _recmmendSelected.SetActive(arg0);
            _recmmendUnSelected.SetActive(!arg0);
            _createCache.Clear();
            _photoItemRoot.DetachChildren();
        }

        void Start()
        {
            Refresh();
            _recmmendToggle.isOn = true;
        }

        public void Refresh()
        {
            _createCache.Clear();
            _photoItemRoot.DetachChildren();

            var photoList = ConfigManager.Instance.GetConfig<PhotoConfigTable>().Rows;
            foreach (var photoConfig in photoList)
            {
                var photoItem = Instantiate(_photoInfoItem, _photoItemRoot);
                photoItem.SetInfo(photoConfig.Value);
                _createCache.Add(photoConfig.Key, photoItem);
            }
        }

        private void OnClickPublish()
        {
            PublishPhotoDialog.Create();
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/snapplanet/SnapPlanet"
            );
        }
    }
}
