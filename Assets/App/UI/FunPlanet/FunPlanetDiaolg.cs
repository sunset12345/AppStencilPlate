using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using App.Config;
using App.DataCache;
using App.LoadingFunction;
using App.UI.Common;
using App.UI.SnapPlanet;
using DFDev.UI.Layer;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;
using Button = DFDev.UI.Button;

namespace App.UI.FunPlanet
{
    public class FunPlanetDialog : LayerContent
    {
        [SerializeField] private VideoPlayer _player;
        [SerializeField] private RawImage _videoDisplay;
        [SerializeField] private GestureDetector _gestureDetector;
        [SerializeField] private Button _playBtn;

        [SerializeField] private Toggle _followToggle;
        [SerializeField] private GameObject _followSelected;
        [SerializeField] private GameObject _followUnSelected;

        [SerializeField] private Toggle _recmmendToggle;
        [SerializeField] private GameObject _recmmendSelected;
        [SerializeField] private GameObject _recmmendUnSelected;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _likeBtn;
        [SerializeField] private Image _likeImage;
        [SerializeField] private Sprite[] _likeSprite;
        [SerializeField] private TextMeshProUGUI _likeCount;
        [SerializeField] private Button _commentaryBtn;
        [SerializeField] private TextMeshProUGUI _commentCount;
        [SerializeField] private Button _publishBtn;

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _aiName;
        [SerializeField] private Button _aiInfo;
        [SerializeField] private TextMeshProUGUI _vidoeDes;

        private bool _isPlaying = false;
        private AiConfig _aiConfig;
        public DefaultVideoConfig _videoConfig;
        private int _videoId = 1;

        void Awake()
        {
            _aiInfo.AddClick(OnClickAiInfo);
            _publishBtn.AddClick(OnClickPublish);
            _likeBtn.AddClick(OnClickLike);
            _followSelected.SetActive(false);
            _followUnSelected.SetActive(true);
            _recmmendSelected.SetActive(true);
            _recmmendUnSelected.SetActive(false);
            _closeButton.AddClick(() =>
            {
                Close();
            });
            _gestureDetector.OnSwipeUp.AddListener(OnSwipeUp);
            _gestureDetector.OnSwipeDown.AddListener(OnSwipeDown);

            _playBtn.AddClick(PlayVideo);
            _player.playOnAwake = true;
            _player.renderMode = VideoRenderMode.RenderTexture;
            _player.prepareCompleted += Prepared;

            RenderTexture renderTexture = new RenderTexture(
                (int)_player.width, (int)_player.height, 24, RenderTextureFormat.ARGB32);
            _player.targetTexture = renderTexture;
            _videoDisplay.texture = renderTexture;
        }

        private void OnClickLike()
        {
            DataCacheManager.Instance.ChangeLikeStage(_videoConfig.Id);
            UpdateLikeStage();
        }

        private void OnClickAiInfo()
        {
            AiInfoDialog.Create(_aiConfig.Id, false);
            _player.Pause();
        }

        private void UpdateInfo()
        {
            _videoConfig = ConfigManager.Instance.GetConfig<DefaultVideoConfigTable>().GetRowData(_videoId);
            _aiConfig = ConfigManager.Instance.GetConfig<AiConfigTable>().GetRowData(_videoConfig.AiId);
            _aiName.text = _aiConfig.Name;
            _vidoeDes.text = _videoConfig.Description;
            _icon.sprite = _aiConfig.IconRes.Load<Sprite>();

            var clip = Resources.Load<VideoClip>(_videoConfig.Asset);
            _player.clip = clip;
            _player.isLooping = true;
            _player.Prepare();
            // _commentCount.text = DataCacheManager.Instance.GetCommentary(_videoConfig.Id).Count.ToString();
            UpdateLikeStage();
        }

        private void UpdateLikeStage()
        {
            var isLike = DataCacheManager.Instance.VidoeIsLike(_videoId);
            _likeImage.sprite = isLike ? _likeSprite[0] : _likeSprite[1];
            _likeCount.text = isLike ? "1" : "0";
        }

        private void PlayVideo()
        {
            if (!_player.isPrepared)
            {
                CommonMessageTip.Create("Please wait for the video to be prepared");
                return;
            }

            if (_isPlaying)
            {
                _player.Pause();
                _isPlaying = false;
            }
            else
            {
                _player.Play();
                _isPlaying = true;
            }
        }

        private void OnSwipeDown(GestureDetector.GestureData arg0)
        {

        }

        private void OnSwipeUp(GestureDetector.GestureData arg0)
        {

        }

        private void UpdateView()
        {

        }

        private void Prepared(VideoPlayer source)
        {
            _videoDisplay.texture = _player.texture;
            _player.Play();
            _isPlaying = true;
        }

        void OnDestroy()
        {
            _player.prepareCompleted -= Prepared;
        }

        private void OnClickPublish()
        {
#if UNITY_IOS && !UNITY_EDITOR
        OpenVideoPicker();
#endif
        }


        #region Publish Vidoe

        [DllImport("__Internal")]
        private static extern void OpenVideoPicker();

        [DllImport("__Internal")]
        private static extern void DeleteCachedVideo(string filePath);

        // 缓存目录路径
        private string cacheDir;

        void Start()
        {
            cacheDir = DataCacheManager.Instance.GetCacheDir();
        }

        // 接收iOS传回的视频路径（通过UnitySendMessage调用）
        public void OnVideoSelected(string videoPath)
        {
            Debug.Log($"iOS传回原始路径: {videoPath}");
            StartCoroutine(CopyVideoToCache(videoPath));
        }

        // 将视频复制到缓存目录
        private IEnumerator CopyVideoToCache(string sourcePath)
        {
            Debug.Log($"CopyVideoToCache :{sourcePath} -> {cacheDir}");
            string fileName = Path.GetFileName(sourcePath);
            string destPath = Path.Combine(cacheDir, fileName);

            if (File.Exists(destPath))
            {
                Debug.Log("视频已存在，跳过复制");
                CommonMessageTip.Create("Video Published successfully");
                yield break;
            }
            string fileURL = "file://" + sourcePath;

            using (UnityWebRequest www = UnityWebRequest.Get(uri: fileURL))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError($"下载失败: {www.error}");
                    CommonMessageTip.Create("Video Published failed");
                    yield break;
                }

                File.WriteAllBytes(destPath, www.downloadHandler.data);
                Debug.Log($"视频已缓存到: {destPath}");
                CommonMessageTip.Create("Video Published successfully");
            }
        }

        // 删除缓存视频
        public void DeleteVideo(string fileName)
        {
            string filePath = Path.Combine(cacheDir, fileName);
#if UNITY_IOS && !UNITY_EDITOR
            DeleteCachedVideo(filePath);
#else
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
#endif
        }


        #endregion


        public static void Create()
        {
            LayerManager.Instance.LoadContent(
                LayerTag.Dialog,
                "ui/funplanet/FunPlanetDialog"
                );
        }
    }
}