using System.Collections;
using System.Collections.Generic;
using DFDev.Singleton;
using UnityEngine;

namespace PushInformationFunction
{
    public class PushInformationManager : MonoSingleton<PushInformationManager>
    {
#if UNITY_IOS && !UNITY_EDITOR

    // 链接原生代码
    [DllImport("__Internal")]
    private static extern void RequestPushToken();
#endif

        public void TriggerTokenRequest()
        {
#if UNITY_IOS && !UNITY_EDITOR
        RequestPushToken();
#endif
        }

        // 由 iOS 原生代码调用（成功时）
        public void OniOSPushTokenReceived(string token)
        {
            Debug.Log("iOS Push Token: " + token);
            _deviceToken = token;
            // 将 token 发送到你的服务器
        }

        // 由 iOS 原生代码调用（失败时）
        public void OniOSPushTokenFailed(string error)
        {
            Debug.LogError("Failed to get token: " + error);
        }

        private string _deviceToken;

        public string DeviceToken => _deviceToken;
    }
}
