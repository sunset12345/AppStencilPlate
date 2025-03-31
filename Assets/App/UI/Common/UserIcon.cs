using App.Config;
using App.DataCache;
using DFDev.EventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Common
{
    public class UserIcon : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        private EventSubscriber _eventSubscriber;

        void Start()
        {
            SetInfo();
        }

        void Awake()
        {
            _eventSubscriber = new EventSubscriber();
            _eventSubscriber.Subscribe<DataCahceUpdateEvent>(OnDataCahceUpdateEvent);
        }

        private void OnDataCahceUpdateEvent(DataCahceUpdateEvent @event)
        {
            if (@event.Key == DataEnum.AvatarId.ToString())
                SetInfo();
        }

        private void SetInfo()
        {
            var avatarId = DataCache.DataCache.Load<int>(DataEnum.AvatarId.ToString());
            _icon.sprite = ConfigManager.Instance.GetConfig<AvatarConfigTable>()
                .GetRowData(avatarId).IconRes.Load<Sprite>();
        }

        void OnDestroy()
        {
            _eventSubscriber.Dispose();
        }
    }
}
