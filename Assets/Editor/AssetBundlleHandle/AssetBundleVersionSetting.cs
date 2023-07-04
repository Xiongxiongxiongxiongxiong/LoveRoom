#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System;

using UnityEditor;

namespace AssetBundleTools.Editor
{
    public class AssetBundleVersionSetting : ScriptableObject
    {

        public static string path = "ProjectSettings/AssetBundleVersionSettings.asset";
        private static AssetBundleVersionSetting instance;
        public static AssetBundleVersionSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    LoadOrCreate();
                }
                return instance;
            }
        }


        public static void Save()
        {
            if (!Instance)
            {
                Debug.LogErrorFormat("Cannot save ScriptableSingleton: no instance!");
                return;
            }
            UnityEngine.Object[] obj = new AssetBundleVersionSetting[1] { Instance };
            InternalEditorUtility.SaveToSerializedFileAndForget(obj, path, true);
        }
        private static void LoadOrCreate()
        {
            UnityEngine.Object[] obj = InternalEditorUtility.LoadSerializedFileAndForget(path);
            if (obj != null && obj.Length > 0)
                instance = obj[0] as AssetBundleVersionSetting;
            else
            {
                instance = CreateInstance<AssetBundleVersionSetting>();
                obj = new UnityEngine.Object[1] { instance };
                InternalEditorUtility.SaveToSerializedFileAndForget(obj, path, true);
            }
        }
        public VersionInfo iOS;
        public VersionInfo Android;
        public VersionInfo WebGL;

        public VersionInfo StandaloneOSX;

        public VersionInfo StandaloneWindows;

        public VersionInfo GetVersionInfo(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.iOS:
                    return iOS;
                case BuildTarget.Android:
                    return Android;
                case BuildTarget.WebGL:
                    return WebGL;
                case BuildTarget.StandaloneOSX:
                    return StandaloneOSX;
                case BuildTarget.StandaloneWindows:
                    return StandaloneWindows;
                default:
                    return StandaloneWindows;
            }
        }
    }

    [Serializable]
    public class VersionInfo
    {
        [Header("勾线之后小版本自动更新，不勾选则是手动填写")]
        public bool AutoUpVersion = false;
        [Header("当前将要构建版本")]
        public string BuildingVersion = "1.001";
        [Header("将需要构建进应用包内的Bundle打勾")]
        public List<IncludeAssetBundles> buildIncludeBundles;

        [Header("已经构建的最新版本")]
        public string LastBuildedVersion = string.Empty;

        [Header("老版本列表")]
        public List<string> OldVersionList;
    }

    [Serializable]
    public class IncludeAssetBundles
    {
        public bool Include;
        public string BundleName;

        public string FileName;
        public IncludeAssetBundles(string name, string fileName, bool include = false)
        {
            BundleName = name;
            Include = include;
            FileName = fileName;
        }
    }

}
#endif