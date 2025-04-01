using App.Config;
using App.LoadingFunction;
using App.UI.Common;
using Cysharp.Threading.Tasks;
using DFDev.UI.Layer;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.Chat
{
    public class ChatLayer : LayerContent
    {
        [SerializeField] private Transform _chatItemRoot;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _sendBtn;
        [SerializeField] private InputField _inputField;
        [SerializeField] private Image _roleIcon;
        [SerializeField] private TextMeshProUGUI _roleNameText;
        [SerializeField] private ChatInfoItem _selfChatItem;
        [SerializeField] private ChatInfoItem _otherChatItem;
        [SerializeField] private ScrollRect _chatScrollView;

        private AiConfig _aiConfig;

        void Awake()
        {
            _closeBtn.AddClick(Close);
            _sendBtn.AddClick(OnClickSend);

            //             // 监听整个字典结构变化
            //     ChatManager.Instance.ChatInfoList.ObserveAdd()
            //         .Subscribe(addEvent => Debug.Log($"Key {addEvent.Key} added"))
            //         .AddTo(this);

            // reactiveChatInfo.ObserveRemove()
            //     .Subscribe(removeEvent => Debug.Log($"Key {removeEvent.Key} removed"))
            //     .AddTo(disposables);

            // 监听具体某个列表的内容变化
        }

        private void OnClickSend()
        {
            if (string.IsNullOrEmpty(_inputField.text))
            {
                CommonMessageTip.Create("Please enter a message");
                return;
            }
            StartCoroutine(ChatManager.Instance.SendChat(_inputField.text, _aiConfig.Id));
            _inputField.text = string.Empty;
        }

        public void SetInfo(int roleId)
        {
            _aiConfig = ConfigManager.Instance.GetConfig<AiConfigTable>().GetRowData(roleId);
            _roleIcon.sprite = _aiConfig.IconRes.Load<Sprite>();
            _roleNameText.text = _aiConfig.Name;
            ChatManager.Instance.ChatInfoList.ObserveEveryValueChanged(dict => dict.Values)
               .Subscribe(_ =>
               {
                   foreach (var pair in ChatManager.Instance.ChatInfoList)
                   {
                       if (pair.Key == _aiConfig.Id)
                       {
                           // 只处理当前AI的聊天记录
                           pair.Value.ObserveAdd()
                               .Subscribe(add => AddItem(add.Value))
                               .AddTo(this);

                           pair.Value.ObserveRemove()
                               .Subscribe(remove => Debug.Log($"Removed from {pair.Key}: {remove.Value}"))
                               .AddTo(this);
                       }
                   }
               });
            UpdateChatList();
        }

        public void UpdateChatList()
        {
            _chatItemRoot.DestroysInChildren();
            var chatList = ChatManager.Instance.GetChatListById(_aiConfig.Id);
            foreach (var chat in chatList)
            {
                var itemPrefab = chat.name == "assistant" ? _otherChatItem : _selfChatItem;
                var chatItem = Instantiate(
                    itemPrefab,
                    _chatItemRoot);
                chatItem.SetInfo(chat.content);
            }
            SetScrollViewToBottom().Forget();
        }

        private void AddItem(ChatInfo chatInfo)
        {
            var itemPrefab = chatInfo.name == "assistant" ? _otherChatItem : _selfChatItem;
            var chatItem = Instantiate(
                itemPrefab,
                _chatItemRoot);
            chatItem.SetInfo(chatInfo.content);
            SetScrollViewToBottom().Forget();
        }

        private async UniTaskVoid SetScrollViewToBottom()
        {
            await UniTask.DelayFrame(1);
            _chatScrollView.verticalNormalizedPosition = 0;
        }

        public static void Create(int roleId)
        {
            var chatLayer = LayerManager.Instance.LoadContent(
                LayerTag.Chat,
                "ui/chat/ChatLayer") as ChatLayer;
            chatLayer.SetInfo(roleId);
        }
    }
}
