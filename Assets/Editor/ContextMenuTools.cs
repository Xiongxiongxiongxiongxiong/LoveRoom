using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContextMenuTools : Editor
{
    [MenuItem("Assets/ CreateLightProbeDataAsset", validate = false)]
    static void CreateLightProbeDataAsset()
    {
        var da = Selection.activeObject as LightingDataAsset;
        if (da)
        {
            Lightmapping.lightingDataAsset = da;
            var probes = Instantiate(LightmapSettings.lightProbes);
            var path = AssetDatabase.GetAssetPath(da);
            Debug.Log(path);
            path = path.Substring(0, path.LastIndexOf('/'));
            var name = SceneManager.GetActiveScene().name;
            AssetDatabase.CreateAsset(probes, $"{path}/{name}_lightprobes.asset");
        }
        else
        {
            Debug.LogError(" Selected Object  is not a LightingDataAsset ");
        }
    }

}
