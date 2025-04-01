using System;
using App.Config;
using App.LoadingFunction;
using App.UI.AivanaPlanet;
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
        [SerializeField] private Image _planetName;
        [SerializeField] private Image _planetDesc;

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
            switch (_planetId)
            {
                case 1:
                    HealingPlanet.HealingPlanet.Create();
                    break;
                case 2:
                    AivanaPlanetDialog.Create();
                    break;
                case 3:
                    AivanaPlanetDialog.Create();
                    break;
                case 4:
                    AivanaPlanetDialog.Create();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            _planetName.sprite = _planetConfig.Name.Load<Sprite>();
            _planetDesc.sprite = _planetConfig.Desc.Load<Sprite>();
            _planetName.SetNativeSize();
            _planetDesc.SetNativeSize();
        }

        public static void Create(int id)
        {
            var dialog = LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/planetpanel/PlanetMainDialog"
            ) as PlanetMainDialog;
            dialog.Init(id);
        }
    }
}