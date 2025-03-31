using System.Collections;
using System.Collections.Generic;
using App.LoadingFunction;
using DFDev.UI.Layer;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace App.UI.Common
{
    public class WaitingTip : DFDev.UI.Layer.LayerContent
    {
        private static WaitingTip _instance;
        private const string WaitingTipAniName = "WaitingTip_ShowClose";
        private float _showDelay, _destroyDelay;
        [SerializeField] private Image _maskImg;
        [SerializeField] private GameObject _waitObj;

        private float _fMaskAlpha;


        private void Awake()
        {
            _fMaskAlpha = _maskImg.color.a;
        }

        public static WaitingTip Open(float showDelay = -1f, float destroyDelay = -1f)
        {
            if (!_instance)
                _instance = Create();
            _instance.Initialize(showDelay, destroyDelay);
            return _instance;
        }

        public new static void Close()
        {
            if (!_instance) return;
            _instance.gameObject.SetActive(false);
            _instance.OnDestroy();
        }

        private void OnDestroy()
        {
            _maskImg.DOKill();
            DOTween.Kill(WaitingTipAniName);
        }


        private static WaitingTip Create()
        {
            var content = LayerManager.Instance.LoadContent((int)LayerTag.Waiting, "ui/common/WaitingTip");
            return content.GetComponent<WaitingTip>();
        }

        private void Initialize(float showDelay, float destroyDelay)
        {
            _showDelay = showDelay;
            _destroyDelay = destroyDelay;
            _maskImg.SetAlpha(0);
            gameObject.SetActive(true);
            _waitObj.SetActive(false);
            DOTween.Kill(WaitingTipAniName);
            var sequence = DOTween.Sequence(_maskImg).SetId(WaitingTipAniName);
            if (_showDelay >= 0)
            {
                sequence.AppendInterval(_showDelay);
                sequence.AppendCallback(() =>
                {
                    _waitObj.gameObject.SetActive(true);
                    _maskImg.DOFade(_fMaskAlpha, 0.3f);
                });
            }

            if (_destroyDelay >= 0)
            {
                sequence.AppendInterval(_destroyDelay); ;
                sequence.AppendCallback(Close);
            }
        }
    }
}