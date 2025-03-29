using UnityEngine;

namespace YYDev.UI
{
    public class ConversionShowOrHide : ConversionBase
    {
        [SerializeField] private GameObject _node;

        protected override void OnAwake()
        {
            if (!_node) _node = gameObject;
        }

        protected override void OnSwitch(bool conversion)
        {
            _node.SetActive(conversion);
        }
    }
}