using App.Config;
using App.UI.PlanetPanel;
using UnityEngine;
using UnityEngine.UI;
using Button = DFDev.UI.Button;

namespace App.UI.StartMain
{
    public class PlaneteItem : MonoBehaviour
    {
       [SerializeField] private Image _planetImage;
       [SerializeField] private Button _goBtn;
       [SerializeField] private int _planetId;

        private PlanetConfig _planetConfig;
        void Awake()
        {
            _planetConfig = ConfigManager.Instance.GetConfig<PlanetConfigTable>().GetRowData(_planetId);
            _goBtn.AddClick(OnClickGoBtn);
            _planetImage.sprite = _planetConfig.IconRes.Load<Sprite>();
        }

        private void OnClickGoBtn()
        {
            PlanetMainDialog.Create(_planetId);
        }
    }
}