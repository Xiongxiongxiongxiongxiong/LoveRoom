using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateAB : Editor
{
    [MenuItem("Tool /OpenCacheFolder")]
    static void OpenCacheFolder()
    {
        Application.OpenURL(new System.Uri(Application.persistentDataPath).AbsoluteUri);
    }
    [MenuItem("Tool / Create AB")]
   static void Creator()
    {
        //创建一个List的数组，用于保存数据
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        // 获取unity文件夹里的asset_list文本文件里的字段数据。
        string[] lines= System.IO.File.ReadAllLines(Application.dataPath + "/asset_list.txt");
        //将文本里面的数据循环遍历
        foreach (var line in lines)
        {
            Debug.Log(line);
            //将每一行都分割成两个部分，分别保存为名字和路径。
            string[] content = line.Split(new char[] { ' ','\t'}, System.StringSplitOptions.RemoveEmptyEntries);
            string name = content[0];
            string path = content[1];
            //创建一个Bundle包的实例，并将包的名字设置为上面文本里面保存的名字。路径为保存的路径。让后将这个包加入包体的数组中。
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = name;
            build.assetNames = new[] { path };
            builds.Add(build);

            Debug.Log(line);
        }

        // ============================shaders
        //查找路径下的shader文件
        var shaders = AssetDatabase.FindAssets("t:Shader", new[] { "Packages/io.jagat.artlogic/Shaders/Object_Painter" });
        //创建一个Bundle包的实例
        AssetBundleBuild shaderBuild = new AssetBundleBuild();
        //
        List<string> paths = new List<string>();
        foreach (var sh in shaders)
        {
            var path = AssetDatabase.GUIDToAssetPath(sh);
            paths.Add(path);


        }
        shaderBuild.assetBundleName = "object_shaders";
        shaderBuild.assetNames = paths.ToArray();
        builds.Add(shaderBuild);

        if(!System.IO.Directory.Exists(Application.streamingAssetsPath))
        {
            System.IO.Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath ,builds.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
