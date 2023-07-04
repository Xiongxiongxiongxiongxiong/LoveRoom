using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;




public class ModificationModel : EditorWindow
{
  //  private int cc=1;
    Object folder;
    [MenuItem("Tool/ModificationModel")]
    public static void ShowWindow()
    {
        ModificationModel window = GetWindow<ModificationModel>("ModificationModel");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Folder Path:", EditorStyles.boldLabel);
        folder = EditorGUILayout.ObjectField(folder, typeof(Object), false);
        GUILayout.Label(" Ŀ¼: " + AssetDatabase.GetAssetPath(folder));

        if (GUILayout.Button("ModificationModel"))
        {
            CompressTextures();
        }
    }

    private void CompressTextures()
    {

        var obj = AssetDatabase.FindAssets("t:Model", new string[] { AssetDatabase.GetAssetPath(folder) });
      //  Debug.Log(obj.Length);
        for (int i =0;i<obj.Length;i++)
        {
         //   cc++;
            var path = AssetDatabase.GUIDToAssetPath(obj[i]);
         //   var model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //    string fileName = Path.GetFileNameWithoutExtension(path);
            // model.name = $"Plant_tree_{cc}_models_Lod0";
          //  string newFileName = $"Plant_tree_{cc}_models_Lod0".ToString();

            ModelImporter mesh = AssetImporter.GetAtPath(path) as ModelImporter;
            mesh.importBlendShapes = false;
            mesh.importVisibility = false;
            mesh.importCameras = false;
            mesh.importLights = false;
            mesh.isReadable = false;
            mesh.indexFormat = ModelImporterIndexFormat.UInt16;
             mesh.importNormals = ModelImporterNormals.Import;
             mesh.animationType = ModelImporterAnimationType.None;
            // mesh.skinWeights = ModelImporterSkinWeights.Standard;
         //   MeshFilter[] meshes = model.GetComponentsInChildren<MeshFilter>();
            mesh.SaveAndReimport();
           // string newFilePath = Path.Combine(Path.GetDirectoryName(path), newFileName + ".fbx"); // �޸���չ������Ӧ�������
          //  string newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newFilePath);
           // AssetDatabase.RenameAsset(path, newFileName);
          //  AssetDatabase.ImportAsset(newFilePath);
        }

     //   cc = 0;
        AssetDatabase.SaveAssets();
        //刷新资源
        AssetDatabase.Refresh();

    }









}
