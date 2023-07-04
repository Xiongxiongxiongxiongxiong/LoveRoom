using System;
using UnityEngine;
using UnityEditor;


public class ModificationTexture : EditorWindow
{
    
    UnityEngine.Object tex;
    [MenuItem("Tool/ModificationTexture")]
    public static void ShowWindow()
    {
        ModificationTexture window = GetWindow<ModificationTexture>("ModificationTexture");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Folder Path",EditorStyles.boldLabel);
        tex = EditorGUILayout.ObjectField(tex, typeof(UnityEngine.Object), false);
        if (GUILayout.Button("ModificationTexture"))
        {
            CompressTextures();
        }
    }
    private void CompressTextures()
    {
        var textures = AssetDatabase.FindAssets("t:Texture", new string[] { AssetDatabase.GetAssetPath(tex) });
        foreach (var texture in textures)
        {
            var path = AssetDatabase.GUIDToAssetPath(texture);
           // var t = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
         //  var t = AssetDatabase.LoadAllAssetsAtPath(path);
           TextureImporter Tex = AssetImporter.GetAtPath(path) as TextureImporter;
           Tex.mipmapEnabled = false;
           
           TextureImporterPlatformSettings androidSettings = Tex.GetPlatformTextureSettings("Android");
           if (androidSettings.overridden==false)
           {
               androidSettings.overridden = true; 
           }
           androidSettings.maxTextureSize = 1024;
           if (Tex.textureType == TextureImporterType.NormalMap)
           {
               androidSettings.format = TextureImporterFormat.ETC2_RGB4;
           }
           else
           {
               androidSettings.format = TextureImporterFormat.ETC_RGB4Crunched; 
           }
           
           Tex.SetPlatformTextureSettings(androidSettings);
           // 修改iOS平台的Max Size属性
           TextureImporterPlatformSettings iOSSettings = Tex.GetPlatformTextureSettings("iPhone");
           if (iOSSettings.overridden==false)
           {
               iOSSettings.overridden = true;
           }
           iOSSettings.maxTextureSize = 1024;
           Tex.SetPlatformTextureSettings(iOSSettings);
           // 应用修改
           AssetDatabase.ImportAsset(texture, ImportAssetOptions.ForceUpdate);
        }
        // 刷新资源
        AssetDatabase.Refresh();
    }
    
    
    
    
    
    
}
