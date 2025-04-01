using System.Collections.Generic;
using App.Config;
using App.DataCache;
using App.LoadingFunction;
using DFDev.EventSystem;
using DFDev.UI.Layer;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.AivanaPlanet
{
    public class AivanaPlanetDialog : LayerContent
    {
        [SerializeField] private AiItem _aiItemPrefab;
        [SerializeField] private Transform _aiItemRoot;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Toggle _unLockToggle;
        [SerializeField] private GameObject _unlockSelected;
        [SerializeField] private GameObject _unlockUnSelected;

        [SerializeField] private Toggle _recmmendToggle;
        [SerializeField] private GameObject _recmmendSelected;
        [SerializeField] private GameObject _recmmendUnSelected;

        private EventSubscriber _eventSubscriber;
        void Awake()
        {
            _unLockToggle.onValueChanged.AddListener(OnUnlockToggle);
            _recmmendToggle.onValueChanged.AddListener(OnRecmmendToggle);
            _eventSubscriber = new EventSubscriber();
            _eventSubscriber.Subscribe<DataCahceUpdateEvent>(OnDataCahceUpdateEvent);
            _eventSubscriber.Subscribe<DataCacheClearAllEvent>(OnDataCacheClearAllEvent);
            _closeBtn.AddClick(Close);
        }

        void Start()
        {
            UpdateSelected();
            UpdateList();
        }

        private void UpdateSelected()
        {
            _recmmendSelected.SetActive(_recmmendToggle.isOn);
            _recmmendUnSelected.SetActive(!_recmmendToggle.isOn);
            _unlockSelected.SetActive(_unLockToggle.isOn);
            _unlockUnSelected.SetActive(!_unLockToggle.isOn);
        }

        private void UpdateList()
        {
            _aiItemRoot.DestroysInChildren();
            var aiConfigTable = ConfigManager.Instance.GetConfig<AiConfigTable>();
            var unlockList = DataCacheManager.Instance.GetUnlockList();
            foreach (var aiConfig in aiConfigTable.Rows)
            {
                if (_unLockToggle.isOn)
                {
                    if (aiConfig.Value.UnlockCost == 0 || unlockList.Contains(aiConfig.Value.Id))
                    {
                        var aiItem = Instantiate(_aiItemPrefab, _aiItemRoot);
                        aiItem.SetInfo(aiConfig.Value);
                    }
                }
                else
                {
                    if (aiConfig.Value.UnlockCost != 0 && !unlockList.Contains(aiConfig.Value.Id))
                    {
                        var aiItem = Instantiate(_aiItemPrefab, _aiItemRoot);
                        aiItem.SetInfo(aiConfig.Value);
                    }
                }
            }
        }

        private void OnDataCacheClearAllEvent(DataCacheClearAllEvent @event)
        {
            UpdateList();
        }

        private void OnDataCahceUpdateEvent(DataCahceUpdateEvent @event)
        {
            if (@event.Key == DataEnum.AiUnlock.ToString())
            {
                UpdateList();
            }
        }

        private void OnRecmmendToggle(bool arg0)
        {
            _recmmendSelected.SetActive(arg0);
            _recmmendUnSelected.SetActive(!arg0);
            UpdateList();
        }

        private void OnUnlockToggle(bool arg0)
        {
            _unlockSelected.SetActive(arg0);
            _unlockUnSelected.SetActive(!arg0);
            UpdateList();
        }

        void OnDestroy()
        {
            _eventSubscriber.Dispose();
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(LayerTag.Dialog, "ui/aivanaplanet/AivanaPlanetDialog");
        }
    }
}
