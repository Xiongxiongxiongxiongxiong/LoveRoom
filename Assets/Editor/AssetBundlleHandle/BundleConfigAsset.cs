#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.IO;
using System.Text;
using AssetBundleTools.Editor;
// namespace AssetBundleTools.Editor
// {

public enum DepMode
{
    IncludeAll,
    IgnorePackage,
    IgnoreAssets,
    IgnoreAll,
}
public class BundleConfigAsset : ScriptableObject
{

    private static string path = "Assets/Editor/AssetBundlleHandle/AssetBundleConfigSettings.asset";
    public List<BundleConfig> configs;
    public DepMode model = DepMode.IncludeAll;
    private static BundleConfigAsset instance;
    public static BundleConfigAsset Instance
    {
        get
        {
            LoadOrCreate();
            return instance;
        }
    }[ContextMenu("Copy2AddressDefineObject")]
        public void Copy2AddressDefineObject()
        {
           var obj =  Jagat.DressAssetBundle.AddressDefineObjectSetting.Instance.activeAddressDefineObject;
            obj.addressList = new List<Jagat.DressAssetBundle.AddressInfo>();
            foreach (var item in configs)
            {
                obj.addressList.Add(new Jagat.DressAssetBundle.AddressInfo() { 
                    active = true,
                    address = item.AssetAddress,
                    guid = item.GUID
                });
            }
            EditorUtility.SetDirty(obj);
        }

    public static void Save()
    {
        if (!Instance)
        {
            Debug.LogErrorFormat("Cannot save ScriptableSingleton: no instance!");
            return;
        }
        EditorUtility.SetDirty(instance);
        // UnityEngine.Object[] obj = new BundleConfigAsset[1] { Instance };
        AssetDatabase.SaveAssetIfDirty(instance);
        // InternalEditorUtility.SaveToSerializedFileAndForget(obj, path, true);
    }

    private static void LoadOrCreate()
    {
        var objj = AssetDatabase.LoadAssetAtPath<BundleConfigAsset>(path);
        if (objj != null)
            instance = objj;
        else
        {
            if (AssetDatabase.IsValidFolder("Assets/Editor"))
            {
                AssetDatabase.CreateFolder("Assets/Editor", "AssetBundlleHandle");
            }
            else
            {
                AssetDatabase.CreateFolder("Assets", "Editor");
                AssetDatabase.CreateFolder("Assets/Editor", "AssetBundlleHandle");
            }
            instance = CreateInstance<BundleConfigAsset>();
            AssetDatabase.CreateAsset(instance, path);

        }
        if (instance.configs == null) instance.configs = new List<BundleConfig>();
    }
}

// }
#endif