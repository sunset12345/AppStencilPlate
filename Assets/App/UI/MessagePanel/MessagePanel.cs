using App.UI.Chat;
using DFDev.UI.Layer;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace App.UI.MessagePanel
{
    public class MessagePanel : LayerContent
    {
        [SerializeField] private Transform _messageRoot;
        [SerializeField] private MessageItem _messageItemPrefab;

        void Awake()
        {
            InitMessage();
            ChatManager.Instance.ChatInfoList.ObserveAdd()
                .Subscribe(_ =>
                {
                    var messageItem = Instantiate(_messageItemPrefab, _messageRoot);
                    messageItem.SetInfo(_.Key);
                }).AddTo(this);
        }

        public void InitMessage()
        {
            var messageList = ChatManager.Instance.ChatInfoList;
            foreach (var pair in messageList)
            {
                var messageItem = Instantiate(_messageItemPrefab, _messageRoot);
                messageItem.SetInfo(pair.Key);
            }
        }
    }
}
