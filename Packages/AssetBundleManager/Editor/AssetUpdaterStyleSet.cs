using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace YYDev.AssetBundles.Editor
{
    internal class AssetUpdaterStyleSet : ScriptableObject
    {
        public List<Texture2D> Icons = new List<Texture2D>();
        public List<GUIStyle> Styles = new List<GUIStyle>();
    }
}


