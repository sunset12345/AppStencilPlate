using App.DataCache;
using App.UI.Chat;
using DFDev.EventSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;


namespace App.UI.AivanaPlanet
{
    public class AiItem : MonoBehaviour
    {
        [SerializeField] private Image _aiImage;
        [SerializeField] private Button _chatBtn;
        [SerializeField] private Button _unlockBtn;
        [SerializeField] private TextMeshProUGUI _aiName;
        [SerializeField] private TextMeshProUGUI _height;
        [SerializeField] private TextMeshProUGUI _weight;

        private AiConfig _aiConfig;
        private EventSubscriber _eventSubscriber;
        void Awake()
        {
            _eventSubscriber = new EventSubscriber();
            _eventSubscriber.Subscribe<DataCahceUpdateEvent>(OnDataCahceUpdateEvent);
            _chatBtn.AddClick(OnClickChatBtn);
            _unlockBtn.AddClick(OnClickUnlockBtn);
        }

        private void OnDataCahceUpdateEvent(DataCahceUpdateEvent @event)
        {
            if (@event.Key == DataEnum.AiUnlock.ToString())
            {
                UpdateButtonShow();
            }
        }

        private void OnClickUnlockBtn()
        {
            AivanaUnlockTip.Create(_aiConfig, () =>
            {
                var unlockList = DataCacheManager.Instance.GetUnlockList();
                unlockList.Add(_aiConfig.Id);
                DataCache.DataCache.Save(DataEnum.AiUnlock.ToString(), unlockList);
                UpdateButtonShow();
            });
        }

        private void OnClickChatBtn()
        {
            ChatLayer.Create(_aiConfig.Id);
        }

        public void SetInfo(AiConfig aiConfig)
        {
            _aiConfig = aiConfig;
            _aiImage.sprite = aiConfig.IconRes.Load<Sprite>();
            _aiName.text = aiConfig.Name;
            _height.text = $"Height: {aiConfig.Height} cm";
            _weight.text = $"Weight: {aiConfig.Weight} kg";
            UpdateButtonShow();
        }

        private void UpdateButtonShow()
        {
                var unlockList = DataCacheManager.Instance.GetUnlockList();
            if (unlockList.Contains(_aiConfig.Id) || _aiConfig.UnlockCost == 0)
            {
                _unlockBtn.gameObject.SetActive(false);
                _chatBtn.gameObject.SetActive(true);
            }
            else
            {
                _unlockBtn.gameObject.SetActive(true);
                _chatBtn.gameObject.SetActive(false);
            }
        }

        void OnDestroy()
        {
            _eventSubscriber.Dispose();
        }
    }
}
