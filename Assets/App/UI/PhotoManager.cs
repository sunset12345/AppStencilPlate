using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using DFDev.Singleton;
using UnityEngine;
using UnityEngine.UI;

public class PhotoManager : MonoSingleton<PhotoManager>
{
    // 定义回调委托
    public delegate void SaveCallback(string result);
    public delegate void PickCallback(string base64Image);

    // 注册原生方法
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _NativeGallery_SaveImage(string filePath, SaveCallback callback);

    [DllImport("__Internal")]
    private static extern void _NativeGallery_PickImage(bool allowEditing, PickCallback callback);
#endif

    //---------------- 从相册选择图片 -----------------

    // 按钮点击触发选择
    public void OnSelectAvatarButtonClick()
    {
        PickImage(true, (base64Image) =>
        {
            if (!string.IsNullOrEmpty(base64Image))
            {
                OnImagePicked(base64Image);
            }
        });
    }

    private void PickImage(bool allowEditing, PickCallback callback)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _NativeGallery_PickImage(allowEditing, callback);
#else
        Debug.LogWarning("PickImage is only supported on iOS.");
#endif
    }

    #region 从手机读取头像
    public static event Action<Texture2D> OnAvatarUpdated;

    // 接收图片数据的回调方法
    private void OnImagePicked(string base64Image)
    {
        byte[] imageData = Convert.FromBase64String(base64Image);
        StartCoroutine(ProcessImage(imageData));
    }

    private IEnumerator ProcessImage(byte[] data)
    {
        // 创建Texture
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(data);

        // 等比缩放至最大512px
        int maxSize = 512;
        if (texture.width > maxSize || texture.height > maxSize)
        {
            float scale = Mathf.Min(
                (float)maxSize / texture.width,
                (float)maxSize / texture.height
            );

            ScaleWithRenderTexture(texture,
                Mathf.FloorToInt(texture.width * scale),
                Mathf.FloorToInt(texture.height * scale)
            );
        }

        // 保存到本地
        string savePath = Path.Combine(Application.persistentDataPath, "avatar.png");
        File.WriteAllBytes(savePath, texture.EncodeToPNG());

        // 更新显示
        OnAvatarUpdated?.Invoke(texture);

        yield return null;
    }

    Texture2D ScaleWithRenderTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);

        Texture2D result = new Texture2D(width, height);
        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();

        RenderTexture.ReleaseTemporary(rt);
        return result;
    }

    public void SetTextureToImage(Texture2D texture, Image targetImage)
    {
        // Step 1: 创建Sprite
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height), // 纹理范围
            new Vector2(0.5f, 0.5f), // 轴心点（居中）
            pixelsPerUnit: 100 // 每单位像素数
        );

        // Step 2: 设置给Image组件
        targetImage.sprite = sprite;

        // Step 3: 调整Image显示（可选）
        targetImage.preserveAspect = true; // 保持宽高比
    }

    // 加载本地保存的头像
    public static Texture2D LoadSavedAvatar()
    {
        string path = Path.Combine(Application.persistentDataPath, "avatar.png");
        if (File.Exists(path))
        {
            byte[] data = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return tex;
        }
        return null;
    }

    #endregion


    //---------------- 保存图片到相册 -----------------
    public void SaveImage(string filePath, SaveCallback callback)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _NativeGallery_SaveImage(filePath, callback);
#else
        Debug.LogWarning("SaveImage is only supported on iOS.");
#endif
    }
}
