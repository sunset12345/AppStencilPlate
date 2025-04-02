using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.SnapPlanet
{
    public class PhotoInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _aiIcon;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _photoInfo;
        [SerializeField] private Image _photo;
        [SerializeField] private Sprite[] _likeSprites;

        [SerializeField] private Image _like;
        [SerializeField] private Button _likeButton;
        [SerializeField] private Button _downLoadBtn;

        public void SetInfo(int photoId)
        {

        }
    }
}
