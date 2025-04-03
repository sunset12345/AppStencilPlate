using DFDev.UI.Layer;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;
using App.LoadingFunction;
using TMPro;

namespace App.UI.SnapPlanet
{
    public class PublishPhotoDialog : LayerContent
    {
       [SerializeField] private Button _publishBtn;
       [SerializeField] private Button _choseBtn;
       [SerializeField] private GameObject _photoMask;
       [SerializeField] private Image _photo;
       [SerializeField] private TMP_InputField _input;

        void Awake()
        {
            _photoMask.SetActive(false);
            _publishBtn.AddClick(OnClickPublish);
            _choseBtn.AddClick(OnClickChose);
        }

        private void OnClickPublish()
        {
            
        }

        private void OnClickChose()
        {

        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/snapplanet/PublishPhotoDialog"
            );
        }
    }
}
