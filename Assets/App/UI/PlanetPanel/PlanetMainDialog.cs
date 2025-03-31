using System;
using App.Config;
using App.LoadingFunction;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.PlanetPanel
{
    public class PlanetMainDialog : LayerContent
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _leftBtn;
        [SerializeField] private Button _rightBtn;
        [SerializeField] private Button _goBtn;
        [SerializeField] private Image _planetImage;
        [SerializeField] private TextMeshProUGUI _planetName;
        [SerializeField] private TextMeshProUGUI _planetDesc;

        private PlanetConfig _planetConfig;
        private PlanetConfigTable _planetConfigTable;

        private void Awake()
        {
            _planetConfigTable = ConfigManager.Instance.GetConfig<PlanetConfigTable>();
            _closeBtn.AddClick(Close);
            _leftBtn.AddClick(OnClickLeft);
            _rightBtn.AddClick(OnClickRight);
            _goBtn.AddClick(OnClickGo);
        }

        private void OnClickGo()
        {
            // switch (_planetId)
            // {
            //     case 1:
            //         LoadingLayer.Create(LoadingType.Start);
            //         break;
            //     case 2:
            //         LoadingLayer.Create(LoadingType.Planet2);
            //         break;
            //     case 3:
            //         LoadingLayer.Create(LoadingType.Planet3);
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
        }

        private int _planetId;

        public void Init(int id)
        {
            _planetId = id;
            UpdateInfo();
        }

        private void OnClickRight()
        {
            _planetId++;
            if (_planetId > _planetConfigTable.GetRowCount())
                _planetId = 1;
            UpdateInfo();
        }

        private void OnClickLeft()
        {
            _planetId--;
            if (_planetId < 1)
                _planetId = _planetConfigTable.GetRowCount();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            _planetConfig = ConfigManager.Instance.GetConfig<PlanetConfigTable>().GetRowData(_planetId);
            _planetImage.sprite = _planetConfig.IconRes.Load<Sprite>();
            _planetName.text = _planetConfig.Name;
            _planetDesc.text = _planetConfig.Desc;
        }

        public static void Create(int id)
        {
            var dialog = LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/planet/PlanetMainDialog"
            ) as PlanetMainDialog;
            dialog.Init(id);
        }
    }
}