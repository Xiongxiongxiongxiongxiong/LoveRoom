#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using UnityEditorInternal;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Text;

namespace AssetBundleTools.Editor
{
    public class VersionHandlePostBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private static bool fileExit;
        private static string abDir;
        private static string outputPath = "AssetBundleOutput";
        public int callbackOrder => -1;
        public void OnPreprocessBuild(BuildReport report)
        {
            BuildTarget target = report.summary.platform;
            VersionInfo info = AssetBundleVersionSetting.Instance.GetVersionInfo(target);
            fileExit = Directory.Exists(Application.streamingAssetsPath);
            if (!fileExit)
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(info.LastBuildedVersion);
            var versionInfo = AssetBundleVersionSetting.Instance.GetVersionInfo(target);
            var includeList = versionInfo.buildIncludeBundles;
            abDir = Path.Combine(Application.streamingAssetsPath, "ABPKG");
            if (Directory.Exists(abDir))
            {
                Directory.Delete(abDir, true);
                File.Delete(abDir + ".meta");
            }
            Directory.CreateDirectory(abDir);
            for (int i = 0; i < includeList.Count; i++)
            {
                if (includeList[i].Include)
                {
                    stringBuilder.AppendLine(includeList[i].FileName);
                    File.Copy(Path.Combine(Directory.GetCurrentDirectory(), $"{outputPath}/{target.ToString()}/{versionInfo.LastBuildedVersion}/{includeList[i].FileName}"), Path.Combine(abDir, includeList[i].FileName));
                }

            }
            File.WriteAllText(Path.Combine(abDir, "Catalog.hash"), stringBuilder.ToString());
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log("====>" + fileExit);
            if (Directory.Exists(abDir))
            {
                Directory.Delete(abDir, true);
                File.Delete(abDir + ".meta");
            }
            AssetDatabase.Refresh();
            if (fileExit == false)
            {
                if (Directory.Exists(Application.streamingAssetsPath))
                {
                    AssetDatabase.DeleteAsset("Assets/StreamingAssets");
                }
            }
            else
            {
                AssetDatabase.DeleteAsset("Assets/StreamingAssets/Catalog.hash");
            }

        }
    }

}
#endif