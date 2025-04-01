using App.Config;
using App.DataCache;
using App.UI.Common;
using App.UI.LoginFunction;
using App.UI.Main;
using App.UI.ShopFunction;
using App.UI.Suggestion;
using DFDev.EventSystem;
using DFDev.UI;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;


namespace App.UI.PersonCenterPanel
{
    public class PersonCenterPanel : LayerContent, IEventSender
    {
        public EventDispatcher Dispatcher => EventDispatcher.Global;
        [SerializeField] private Button _avatarChangeBtn;
        [SerializeField] private Button _nicknameChangeBtn;
        [SerializeField] private TextMeshProUGUI _nicknameText;
        [SerializeField] private TextMeshProUGUI _idText;

        [SerializeField] private TextMeshProUGUI _diamondText;
        [SerializeField] private Button _storeBtn;

        [SerializeField] private Button _suggestionBtn;
        [SerializeField] private Button _useArgeementBtn;
        [SerializeField] private Button _privacyPolicyBtn;
        [SerializeField] private Button _logoutBtn;
        [SerializeField] private Button _deleteAccountBtn;

        private EventSubscriber _eventSubscriber;

        private AvatarConfigTable _avatarConfigTable;
        void Awake()
        {
            _avatarConfigTable = ConfigManager.Instance.GetConfig<AvatarConfigTable>();
            _eventSubscriber = new EventSubscriber();
            _eventSubscriber.Subscribe<DataCahceUpdateEvent>(OnDataCahceUpdateEvent);
            _eventSubscriber.Subscribe<DataCacheClearAllEvent>(OnDataCacheClearAllEvent);
            _nicknameChangeBtn.AddClick(OnClickChangeNickname);
            _avatarChangeBtn.AddClick(OnClickChangeAvatar);

            _idText.text = $"id:{DataCache.DataCache.Load<string>(DataEnum.UserToken.ToString())}";
            _nicknameText.text = DataCache.DataCache.Load<string>(DataEnum.UserName.ToString());
            _diamondText.text = DataCache.DataCache.Load<int>(DataEnum.Diamond.ToString()).ToString();

            _suggestionBtn.AddClick(OnClickSuggestionBtn);
            _deleteAccountBtn.AddClick(OnClickDeleteAccount);
            _logoutBtn.AddClick(OnClickLogout);
            _storeBtn.AddClick(OnClickStore);
        }

        private void OnClickStore()
        {
            ShopDialog.Create();
        }

        private void OnDataCacheClearAllEvent(DataCacheClearAllEvent @event)
        {
            _diamondText.text = "0";
        }

        private void OnClickLogout()
        {
            LoginLayer.Create();
            this.DispatchEvent(Witness<MainNavigatorItemSwitchEvent>._, MainPanelType.Planet);
        }

        private void OnClickDeleteAccount()
        {
            CommonPop.Create(
                "Delete Account",
                "Are you sure you want to delete your account? This action cannot be undone.",
                () =>
                {
                    DataCacheManager.Instance.DeleteAccount();
                    LoginLayer.Create();
                    this.DispatchEvent(Witness<MainNavigatorItemSwitchEvent>._, MainPanelType.Planet);
                }
            );
        }

        private void OnClickChangeAvatar()
        {
            var avatarId = DataCache.DataCache.Load<int>(DataEnum.AvatarId.ToString());
            avatarId++;
            if (avatarId > _avatarConfigTable.GetRowCount())
                avatarId = 1;
            DataCache.DataCache.Save(DataEnum.AvatarId.ToString(), avatarId);
        }

        private void OnClickChangeNickname()
        {
            RenamePop.Create();
        }

        private void OnDataCahceUpdateEvent(DataCahceUpdateEvent @event)
        {
            if (@event.Key == DataEnum.UserName.ToString())
            {
                _nicknameText.text = DataCache.DataCache.Load<string>(DataEnum.UserName.ToString());
            }
            else if (@event.Key == DataEnum.Diamond.ToString())
            {
                _diamondText.text = DataCache.DataCache.Load<int>(DataEnum.Diamond.ToString()).ToString();
            }
        }

        private void OnClickSuggestionBtn()
        {
            SuggestionDialog.Create();
        }

        void OnDestroy()
        {
            _eventSubscriber.Dispose();
        }

    }
}