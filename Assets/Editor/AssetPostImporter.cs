using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetPostImporter : AssetPostprocessor
{
    static void ImportToAssetListObject(string filePath)
    {
        var assetListObjs = AssetDatabase.FindAssets("asset_list_show");
        if (assetListObjs != null && assetListObjs.Length > 0)
        {
            var assetsObj = AssetDatabase.LoadAssetAtPath<Jagat.DressAssetBundle.AddressDefineObject>(AssetDatabase.GUIDToAssetPath(assetListObjs[0]));
            assetsObj.addressList.Clear();
            assetsObj.refBundleList.Clear();
            Jagat.DressAssetBundle.Editors.AddressABBuilder.ImportAssetList(filePath, assetsObj);
        }
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        foreach (var item in importedAssets)
        {
            if (item.EndsWith("asset_list.txt"))
            {
                ImportToAssetListObject(item);
            }
        }
    }
}
