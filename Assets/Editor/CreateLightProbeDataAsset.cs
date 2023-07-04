using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



/// <summary>
/// 
/// </summary>
public class CreateLightProbeDataAsset : EditorWindow
{

    static CreateLightProbeDataAsset window;

    UnityEngine.Object save_path;
   // string save_path;
    string name;
    [MenuItem("Tool / CreateLightProbeDataAsset")]
  
    static void Init()
    {
        window = CreateLightProbeDataAsset.CreateInstance<CreateLightProbeDataAsset>();
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Folder Path",EditorStyles.boldLabel);
        save_path = EditorGUILayout.ObjectField(save_path, typeof(UnityEngine.Object), false);
        GUILayout.Label(" Path : " + AssetDatabase.GetAssetPath(save_path));
        
        GUILayout.Label("Name", EditorStyles.boldLabel);
        name = EditorGUILayout.TextField(name);
        if (GUILayout.Button("save"))
        {
            var path = AssetDatabase.GetAssetPath(save_path);

            CreateLightProbeAsset(path+$"/{name}.asset");
        }
    }

    static void CreateLightProbeAsset(string path)
    {
        var instance = Instantiate(LightmapSettings.lightProbes);

        AssetDatabase.CreateAsset(instance, path);

        AssetDatabase.Refresh();

    }





}


