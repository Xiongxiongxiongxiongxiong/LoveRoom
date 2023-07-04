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
        //����һ��List�����飬���ڱ�������
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        // ��ȡunity�ļ������asset_list�ı��ļ�����ֶ����ݡ�
        string[] lines= System.IO.File.ReadAllLines(Application.dataPath + "/asset_list.txt");
        //���ı����������ѭ������
        foreach (var line in lines)
        {
            Debug.Log(line);
            //��ÿһ�ж��ָ���������֣��ֱ𱣴�Ϊ���ֺ�·����
            string[] content = line.Split(new char[] { ' ','\t'}, System.StringSplitOptions.RemoveEmptyEntries);
            string name = content[0];
            string path = content[1];
            //����һ��Bundle����ʵ��������������������Ϊ�����ı����汣������֡�·��Ϊ�����·�����ú�������������������С�
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = name;
            build.assetNames = new[] { path };
            builds.Add(build);

            Debug.Log(line);
        }

        // ============================shaders
        //����·���µ�shader�ļ�
        var shaders = AssetDatabase.FindAssets("t:Shader", new[] { "Packages/io.jagat.artlogic/Shaders/Object_Painter" });
        //����һ��Bundle����ʵ��
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
