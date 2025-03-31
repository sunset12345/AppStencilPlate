using System.Collections.Generic;
using System.Linq;
using App.LoadingFunction;
using DFDev.EventSystem;
using DFDev.UI.Layer;
using UnityEngine;

namespace App.UI.Main
{
    public class MainLayer : LayerContent
    {
        [SerializeField] private Transform navigatorRoot;
        [SerializeField] private Transform _panelRoot;
        [SerializeField] private MainPanelType _defaultPanel = MainPanelType.Planet;
    
        private MainNavigateItem _currentNavigator;
        public MainNavigateItem CurrentNavigator => _currentNavigator;
        private EventSubscriber _eventSubscriber;
        private readonly Dictionary<MainNavigateItem, GameObject> _panelCache = new();

        void Awake()
        {
            _eventSubscriber = new EventSubscriber();
            _eventSubscriber.Subscribe<MainNavigatorItemSelectedEvent>(OnMainNavigatorSelected);
        }

        void Start()
        {
            var navigators = navigatorRoot.GetComponentsInChildren<MainNavigateItem>();
            var navigator = navigators.FirstOrDefault(item => item.Type == _defaultPanel);
            if (navigator != null)
            {
                var toggle = navigator.Toggle;
                if (toggle.isOn)
                {
                    toggle.onValueChanged.Invoke(true);
                }
                else
                {
                    toggle.isOn = true;
                }
            }
        }

        private void OnMainNavigatorSelected(MainNavigatorItemSelectedEvent evt)
        {
            if (_currentNavigator != null)
            {
                if (evt.Item == _currentNavigator) return;
                var currentPanel = _panelCache[_currentNavigator];
                currentPanel.SetActive(false);
                _currentNavigator = null;
            }

            _currentNavigator = evt.Item;
            if (!_panelCache.TryGetValue(_currentNavigator, out var panel))
            {
                panel = GameObject.Instantiate(
                    _currentNavigator.PanelPrefab,
                    _panelRoot,
                    false);
                _panelCache[_currentNavigator] = panel;
            }

            panel.SetActive(true);
        }

        void OnDestroy()
        {
            _eventSubscriber.Dispose();
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Main,
                "ui/main/MainLayer");
        }
    }

    public enum MainPanelType
    {
        None = 0,
        Planet = 1,
        Message = 2,
        UserInfo = 3,
    }
}
