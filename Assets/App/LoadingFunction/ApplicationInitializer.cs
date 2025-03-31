using App.UI.LoginFunction;
using App.UI.Main;
using Cysharp.Threading.Tasks;
using DFDev.AssetBundles;
using DFDev.UI.Layer;
using PushInformationFunction;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.LoadingFunction
{
    public class ApplicationInitializer : MonoBehaviour
    {
        [SerializeField] private string _assetBundleCode;
        [SerializeField] private string _enterSceneName;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");

#if ENABLE_GM
            DG.Tweening.DOTween.Init(false, false, DG.Tweening.LogBehaviour.Default);
            Debug.unityLogger.logEnabled = true;
            OSDev.CSVConfig.ConfigBase.ErrorLogger = error => Debug.unityLogger.Log(LogType.Error, error);
#endif
            AssetBundleManager.Instance.Init(
                AssetBundleUpdateProcessor.VersionFileRelativePath,
                _assetBundleCode);

            LayerManager.Instance.CreateLayer(
                (int)LayerTag.Loading,
                new DefaultLayerController());
            PushInformationManager.Instance.TriggerTokenRequest();
            LayerManager.Instance.AddLayerLoader((path, layer) =>
            {
                path.ResolveAssetPath(out var bundle, out var asset);
                var prefab = AssetBundleManager.Instance.LoadAsset<GameObject>(bundle, asset);
                Debug.Assert(prefab, $"add layer error:{path}");
                var go = GameObject.Instantiate(
                    prefab,
                    layer.Root,
                    false);
                go.name = prefab.name;
                return go;
            });

            LayerManager.Instance.LoadContent(
                LayerTag.Loading,
                "ui/loading/LoadingLayer");
            ApplicationLoader.StartLoading(FinishLoading);
        }

        private async void FinishLoading()
        {
            // var userToken = UserDataManager.UserToken;
            // if (string.IsNullOrEmpty(userToken))
            //     UserDataManager.Instance.SetUserToken();
            // AudioManager.PlayMusic(AudioName.MainBGM);
            await SceneManager.LoadSceneAsync(_enterSceneName);
            // MainLayer.Create();
            LoginLayer.Create();
            LayerManager.Instance.GetLayerController((int)LayerTag.Loading)?.CloseAll();
        }
    }

}
