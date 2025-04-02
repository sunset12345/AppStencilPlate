using App.LoadingFunction;
using DFDev.UI.Layer;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.SnapPlanet
{
    public class SnapPlanet : LayerContent
    {
        [SerializeField] private Toggle _followToggle;
        [SerializeField] private GameObject _followSelected;
        [SerializeField] private GameObject _followUnSelected;

        [SerializeField] private Toggle _recmmendToggle;
        [SerializeField] private GameObject _recmmendSelected;
        [SerializeField] private GameObject _recmmendUnSelected;

        [SerializeField] private Transform _photoItemRoot;
        [SerializeField] private PhotoInfoItem _photoInfoItem;
        [SerializeField] private Button _publishBtn;

        void Awake()
        {
            _publishBtn.AddClick(OnClickPublish);
        }

        private void OnClickPublish()
        {
            PublishPhotoDialog.Create();
        }

        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/snapplanet/SnapPlanet"
            );
        }
    }
}
