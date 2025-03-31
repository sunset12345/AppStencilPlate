using System;
using System.Collections;
using System.Collections.Generic;
using App.Config;
using App.DataCache;
using App.UI.Common;
using DFDev.EventSystem;
using DFDev.UI;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;


namespace App.UI.PersonCenterPanel
{
    public class PersonCenterPanel : LayerContent
    {
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
            _nicknameChangeBtn.AddClick(OnClickChangeNickname);
            _avatarChangeBtn.AddClick(OnClickChangeAvatar);
        }

        private void OnClickChangeAvatar()
        {
            var avatarId = DataCache.DataCache.Load<int>(DataEnum.AvatarId.ToString());
            avatarId = avatarId + 1;
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

        void OnDestroy()
        {
            _eventSubscriber.Dispose();
        }

    }
}