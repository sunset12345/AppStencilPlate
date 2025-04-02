using System;
using App.Config;
using App.DataCache;
using DFDev.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.SnapPlanet
{
    public class PhotoInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _aiIcon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _photoInfo;
        [SerializeField] private Image _photo;
        [SerializeField] private Sprite[] _likeSprites;

        [SerializeField] private Image _like;
        [SerializeField] private Button _likeButton;
        [SerializeField] private Button _downLoadBtn;

        private PhotoConfig _photoConfig;
        private AiConfig _aiconfig;
        private EventSubscriber _eventSubscriber;

        void Awake()
        {
            _likeButton.AddClick(OnClickLike);
            _downLoadBtn.AddClick(OnClickDown);
            _eventSubscriber = new EventSubscriber();
        }

        private void OnClickDown()
        {
            
        }

        private void OnClickLike()
        {
            
        }

        public void SetInfo(PhotoConfig _config)
        {
            _photoConfig = _config;
            _aiconfig = ConfigManager.Instance.GetConfig<AiConfigTable>().GetRowData(_photoConfig.AiId);
            _aiIcon.sprite = _aiconfig.IconRes.Load<Sprite>();
            _name.text = _aiconfig.Name;
            _photo.sprite = _photoConfig.Asset.Load<Sprite>();
        }

        void OnDestroy()
        {
            _eventSubscriber.Dispose();
        }
    }
}
