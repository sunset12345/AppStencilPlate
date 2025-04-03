using App.Config;
using App.DataCache;
using App.LoadingFunction;
using App.UI.Chat;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.SnapPlanet
{
    public class AiInfoDialog : LayerContent
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _des;
        [SerializeField] private Button _followBtn;
        [SerializeField] private Button _cancelFollowBtn;
        [SerializeField] private Button _chatBtn;
        [SerializeField] private Button _closeBtn;

        [SerializeField] private GameObject[] mask;
        [SerializeField] private Image[] iamge;

        private AiConfig _aiConfig;

        void Awake()
        {
            _chatBtn.AddClick(OnClickChatBtn);
            _followBtn.AddClick(OnClickFollow);
            _cancelFollowBtn.AddClick(OnClickCancelFollow);
        }

        private void OnClickCancelFollow()
        {
            DataCacheManager.Instance.ChangeFollowList(_aiConfig.Id, false);
            UpdateFollowShow();
        }

        private void OnClickFollow()
        {
            DataCacheManager.Instance.ChangeFollowList(_aiConfig.Id, true);
            UpdateFollowShow();
        }

        private void OnClickChatBtn()
        {
            ChatLayer.CreateUp(_aiConfig.Id);
        }

        public void Initialized(int aiId, bool showChat)
        {
            _aiConfig = ConfigManager.Instance.GetConfig<AiConfigTable>().GetRowData(aiId);
            _icon.sprite = _aiConfig.IconRes.Load<Sprite>();
            _name.text = _aiConfig.Name;
            _des.text = _aiConfig.PersonalSignature;
            if (!showChat)
                _chatBtn.gameObject.SetActive(false);
            else
            {
                var unlockList = DataCacheManager.Instance.GetUnlockList();
                if (unlockList.Contains(_aiConfig.Id) || _aiConfig.UnlockCost == 0)
                    _chatBtn.gameObject.SetActive(true);
                else
                    _chatBtn.gameObject.SetActive(false);
            }
            UpdateFollowShow();
            ShowPhotoInfo();
        }

        private void ShowPhotoInfo()
        {
            var photoList = ConfigManager.Instance.GetRef<PhotoConfigRef>().GetPhotoList(_aiConfig.Id);
            for (int i = 0; i < mask.Length; i++)
            {
                mask[i].SetActive(photoList.Count >= i + 1);
                if (photoList.Count >= i + 1)
                    iamge[i].sprite = ConfigManager.Instance.GetConfig<PhotoConfigTable>().GetRowData(photoList[i]).Asset.Load<Sprite>();
            }
        }

        private void UpdateFollowShow()
        {
            var followList = DataCacheManager.Instance.GetFollowList();
            if (followList.Contains(_aiConfig.Id))
            {
                _followBtn.gameObject.SetActive(false);
                _cancelFollowBtn.gameObject.SetActive(true);
            }
            else
            {
                _followBtn.gameObject.SetActive(true);
                _cancelFollowBtn.gameObject.SetActive(false);
            }
        }

        public static void Create(int aiId, bool showChat = true)
        {
            var dialog = LayerManager.Instance.LoadContent(
                LayerTag.Popup,
                "ui/snapplanet/AiInfoDialog"
            ) as AiInfoDialog;
            dialog.Initialized(aiId, showChat);
        }
    }
}
