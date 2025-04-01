using App.Config;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Common
{
    public class UserIcon : MonoBehaviour
    {
        [SerializeField] private Image _icon;

        void Start()
        {
            SetInfo();
        }

        void Awake()
        {
            PhotoManager.OnAvatarUpdated += OnAvatarUpdated;
        }

        private void OnAvatarUpdated(Texture2D d)
        {
            PhotoManager.Instance.SetTextureToImage(d, _icon);
        }

        private void SetInfo()
        {
            var texture2D = PhotoManager.LoadSavedAvatar();
            if (texture2D == null)
                _icon.sprite = ConfigManager.Instance.GetConfig<Const>().DefaultIcon.Load<Sprite>();
            else
                PhotoManager.Instance.SetTextureToImage(texture2D, _icon);
        }

        void OnDestroy()
        {
            PhotoManager.OnAvatarUpdated -= OnAvatarUpdated;
        }
    }
}
