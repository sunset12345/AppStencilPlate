using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace App.UI.Chat
{
    public class ChatInfoItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _chatLabel;
        [SerializeField] private RectTransform _chatBubble;

          public void SetInfo(string chatContent)
        {
            _chatLabel.text = chatContent;
            SetChatBubbleSize().Forget();
        }

        private async UniTaskVoid SetChatBubbleSize()
        {
            await UniTask.DelayFrame(1);
            transform.SetSize(transform.AsRT().rect.width, _chatBubble.rect.height);
        }
    }
}
