using App.DataCache;
using TMPro;
using UnityEngine;

namespace App.UI.HealingPlanet
{
    public class TimeLineItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _des;
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _time;

        public void SetInfo(TimeLineData timeLine)
        {
            _des.text = timeLine.Description;
            _name.text = timeLine.Name;
            _time.text = timeLine.TimeInfo;
        }

    }
}
