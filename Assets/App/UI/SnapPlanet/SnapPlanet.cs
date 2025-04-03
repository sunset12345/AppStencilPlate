using System.Collections.Generic;
using App.Config;
using App.DataCache;
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

        private bool _isFollow = false;

        void Awake()
        {
            _publishBtn.AddClick(OnClickPublish);
            _recmmendToggle.onValueChanged.AddListener(OnRecmmendToggle);
            _followToggle.onValueChanged.AddListener(OnFollowToggle);
        }

        private void OnFollowToggle(bool arg0)
        {
            if (_isFollow && arg0)
                return;
            _followSelected.SetActive(arg0);
            _followUnSelected.SetActive(!arg0);
            if (!arg0)
                return;
            _createCache.Clear();
            _photoItemRoot.DestroysInChildren();

            var follows = DataCacheManager.Instance.GetFollowList();
            foreach (var girlId in follows)
            {
                var photos = ConfigManager.Instance.GetRef<PhotoConfigRef>().GetPhotoList(girlId);
                if (photos != null && photos.Count > 0)
                {
                    foreach (var photo in photos)
                    {
                        var photoConfig = ConfigManager.Instance.GetConfig<PhotoConfigTable>().GetRowData(photo);
                        var photoItem = Instantiate(_photoInfoItem, _photoItemRoot);
                        photoItem.SetInfo(photoConfig);
                        _createCache.Add(photoConfig.Id, photoItem);
                    }
                }
            }
            _isFollow = true;
        }

        private void OnRecmmendToggle(bool arg0)
        {
            if (!_isFollow && arg0)
                return;
            _recmmendSelected.SetActive(arg0);
            _recmmendUnSelected.SetActive(!arg0);
            _createCache.Clear();
            _photoItemRoot.DestroysInChildren();
            if (!arg0)
                return;
            Refresh();
            _isFollow = false;
        }

        void Start()
        {
            Refresh();
            _recmmendToggle.isOn = true;
            _followSelected.SetActive(false);
            _followUnSelected.SetActive(true);
            _recmmendSelected.SetActive(true);
            _recmmendUnSelected.SetActive(false);
        }

        public void Refresh()
        {
            _createCache.Clear();
            _photoItemRoot.DestroysInChildren();

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
