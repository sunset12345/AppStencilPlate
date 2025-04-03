using System.IO;
using App.Config;
using App.DataCache;
using App.UI.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.SnapPlanet
{
    public class PhotoInfoItem : MonoBehaviour
    {

        [SerializeField] private Image _aiIcon;
        [SerializeField] private Button _aiInfoBtn;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _photoInfo;
        [SerializeField] private Image _photo;
        [SerializeField] private Sprite[] _likeSprites;

        [SerializeField] private Image _like;
        [SerializeField] private Button _likeButton;
        [SerializeField] private Button _downLoadBtn;

        private PhotoConfig _photoConfig;
        private AiConfig _aiconfig;

        private bool _isLike;

        void Awake()
        {
            _likeButton.AddClick(OnClickLike);
            _downLoadBtn.AddClick(OnClickDown);
            _aiInfoBtn.AddClick(OnClickAiInfo);
        }

        private void OnClickAiInfo()
        {
            AiInfoDialog.Create(_photoConfig.AiId);
        }

        private void OnClickDown()
        {
            Texture2D texture = GetReadableTexture(_photo.sprite);
            byte[] bytes = texture.EncodeToPNG();
            string filePath = Path.Combine(Application.persistentDataPath, "tempImage.png");
            File.WriteAllBytes(filePath, bytes);

            PhotoManager.Instance.SaveImage(filePath, (result) =>
            {
                if (result == "success")
                {
                    CommonMessageTip.Create("Save successfully");
                }
                // 清理临时 Texture2D
                if (texture != _photo.sprite.texture)
                {
                    Destroy(texture);
                }
            });
        }

        private void OnClickLike()
        {
            DataCacheManager.Instance.ChangeLikeStage(_photoConfig.Id);
            UpdateLikeStage();
        }

        public void SetInfo(PhotoConfig _config)
        {
            _photoConfig = _config;
            _aiconfig = ConfigManager.Instance.GetConfig<AiConfigTable>().GetRowData(_photoConfig.AiId);
            _aiIcon.sprite = _aiconfig.IconRes.Load<Sprite>();
            _name.text = _aiconfig.Name;
            _photo.sprite = _photoConfig.Asset.Load<Sprite>();
            UpdateLikeStage();
        }

        private void UpdateLikeStage()
        {
            _isLike = DataCacheManager.Instance.IsPhotoLiked(_photoConfig.Id);
            _like.sprite = _isLike ? _likeSprites[1] : _likeSprites[0];
        }

        private Texture2D GetReadableTexture(Sprite sprite)
        {
            if (sprite.texture.isReadable)
            {
                return sprite.texture;
            }
            else
            {
                // 复制像素到可读 Texture2D
                Texture2D readableTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
                Color[] pixels = sprite.texture.GetPixels(
                    (int)sprite.rect.x,
                    (int)sprite.rect.y,
                    (int)sprite.rect.width,
                    (int)sprite.rect.height
                );
                readableTexture.SetPixels(pixels);
                readableTexture.Apply();
                return readableTexture;
            }
        }
    }
}
