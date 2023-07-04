using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class BuildResources
{
    [MenuItem("BuildAB/Android")]
    public static void BuildAddressableAndroid()
    {
        UnityEngine.Debug.Assert(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android);
        UpdateBuildAddressableToTarget();
    }
    [MenuItem("BuildAB/IOS")]
    public static void BuildAddressableiOS()
    {
        UnityEngine.Debug.Assert(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS);
        UpdateBuildAddressableToTarget();
    }
    [MenuItem("BuildAB/WebGL")]
    public static void BuildAddressableWebGL()
    {
        UnityEngine.Debug.Assert(EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL);
        UpdateBuildAddressableToTarget();
    }
    [MenuItem("BuildAB/Win64")]
    public static void BuildAddressableWin64()
    {
        UnityEngine.Debug.Assert(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64);
        UpdateBuildAddressableToTarget();
    }

    [MenuItem("BuildAB/OSX")]
    public static void BuildAddressableOSXUniversal()
    {
        UnityEngine.Debug.Assert(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX);
        UpdateBuildAddressableToTarget();
    }
    public static void UpdateBuildAddressableToTarget()
    {
        //MaterialOnPlatforChanged.ChangeMaterials(EditorUserBuildSettings.activeBuildTarget);
        var defineObj = AssetDatabase.LoadAssetAtPath<Jagat.DressAssetBundle.AddressDefineObject>("Assets/AddressDefineObject.asset");
        if (defineObj)
        {
            defineObj = ScriptableObject.Instantiate(defineObj);
            var assetLists = AssetDatabase.FindAssets("asset_list");
            foreach (var guid in assetLists)
            {
                var filePath = AssetDatabase.GUIDToAssetPath(guid);
                if (filePath.EndsWith(".txt"))
                {
                    Jagat.DressAssetBundle.Editors.AddressABBuilder.ImportAssetList(filePath, defineObj);
                }
            }
            Jagat.DressAssetBundle.Editors.AddressABBuilder.AutoBuildAddressDefine(defineObj);
        }
        else
        {
            Debug.LogError("AddressDefineObjectSetting.activeAddressDefineObject empty!");
        }
        UnityEngine.Debug.Log("Addressable Created Finished!");
    }

}