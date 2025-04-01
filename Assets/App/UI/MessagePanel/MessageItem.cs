using App.Config;
using App.UI.Chat;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.MessagePanel
{
    public class MessageItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _messageLabel;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Button _chatBtn;
        private AiConfig _aiConfig;
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Awake()
        {
            _chatBtn.AddClick(OnClickChatBtn);
        }
        private void OnClickChatBtn()
        {
            ChatLayer.Create(_aiConfig.Id);
        }

        public void SetInfo(int roleId)
        {
            _aiConfig = ConfigManager.Instance.GetConfig<AiConfigTable>().GetRowData(roleId);
            _icon.sprite = _aiConfig.IconRes.Load<Sprite>();
            UpdateInfo();
            _name.text = _aiConfig.Name;

            ChatManager.Instance.ChatInfoList.ObserveEveryValueChanged(dict => dict.Values)
               .Subscribe(_ =>
               {
                   foreach (var pair in ChatManager.Instance.ChatInfoList)
                   {
                       if (pair.Key == _aiConfig.Id)
                       {
                           // 只处理当前AI的聊天记录
                           pair.Value.ObserveAdd()
                               .Subscribe(add => UpdateInfo())
                               .AddTo(_disposables);
                       }
                   }
               });
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }

        private void UpdateInfo()
        {
            _messageLabel.text = ChatManager.Instance.GetLastChatInfoById(_aiConfig.Id);
        }
    }
}
