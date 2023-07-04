
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System;

[Serializable]
public class Manifest
{
    public struct AddressToAssetName
    {
        public string Address;
        public string AssetName;
    }
    public string BundleName;
    public string BundleHashName;
    public string[] Deps;
    public bool Preload;
    public long size;

    public AddressToAssetName[] Addresses;
}

namespace AssetBundleTools.Editor
{
    public class BuildAssetBundle
    {

        public static string outputPath = "AssetBundleOutput";
        private static Dictionary<string, string> keyvalue = new Dictionary<string, string>();

        static string version = string.Empty;

        static string versionFileName = "versionlist.txt";

        //static string splitLine = "===================================";
        static Action<string, string> OnBuildFinish;
        // Start is called before the first frame update
        public static void Build(BuildTarget target)
        {
            Caching.ClearCache();
            keyvalue.Clear();
            AssetBundleVersionSetting versionSetting = AssetBundleVersionSetting.Instance;
            VersionInfo targetVersionInfo = versionSetting.GetVersionInfo(target);
            version = targetVersionInfo.BuildingVersion;

            string fullOutput = Path.Combine(Path.Combine(Application.dataPath, outputPath), target.ToString());
            string relativeOutput = Path.Combine("Assets/" + outputPath, target.ToString());
            if (Directory.Exists(fullOutput))
            {
                AssetDatabase.DeleteAsset(relativeOutput);
            }
            Directory.CreateDirectory(fullOutput);
            AssetBundleBuild[] builds = CollectAssetBundle.Collect();
            if (builds == null || builds.Length == 0) return;

            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(relativeOutput, builds, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.IgnoreTypeTreeChanges, target);
            PostHandle(manifest, fullOutput, target, targetVersionInfo);
            AssetDatabase.Refresh();
        }
        private static void PostHandle(AssetBundleManifest manifest, string fullOutput, BuildTarget target, VersionInfo targetVersionInfo)
        {

            if (manifest == null)
            {
                Debug.Log("AssetBundle Build failed");
                return;
            }
            var lastVersion = targetVersionInfo.LastBuildedVersion;
            string[] allFiles = Directory.GetFiles(fullOutput, "*.*", SearchOption.AllDirectories);
            string curPath = Directory.GetCurrentDirectory();
            string fullRealOutput = Path.Combine(curPath, Path.Combine(outputPath, $"{target.ToString()}/{version}"));
            if (Directory.Exists(fullRealOutput))
            {
                Directory.Delete(fullRealOutput, true);
            }
            Directory.CreateDirectory(fullRealOutput);
            List<IncludeAssetBundles> includeList = new List<IncludeAssetBundles>();
            foreach (var item in allFiles)
            {
                if (item.EndsWith(".bundle.manifest") || item.EndsWith(".meta") || Path.GetFileNameWithoutExtension(item) == target.ToString() || item.EndsWith(".DS_Store"))
                {
                }
                else
                {
                    FileStream stream = File.OpenRead(item);
                    string md5 = GenerateMD5(stream);
                    string newPath = item.Replace(".bundle", "_" + md5 + ".bundle");
                    string newName = Path.GetFileName(newPath);
                    string oldName = Path.GetFileName(item);
                    if (!includeList.Exists((item) => { return item.BundleName == oldName; }))
                        includeList.Add(new IncludeAssetBundles(oldName, newName));
                    newPath = Path.Combine(fullRealOutput, newName);
                    File.Copy(item, newPath);
                    if (!keyvalue.ContainsKey(oldName))
                    {
                        keyvalue.Add(oldName, newName);
                    }
                }
            }
            if (targetVersionInfo.AutoUpVersion)
            {
                List<string> changeList = new List<string>();
                if (string.IsNullOrEmpty(lastVersion))
                {
                    changeList.Add("");
                }
                else
                {

                    string lastVersionfullRealOutput = Path.Combine(curPath, Path.Combine(outputPath, $"{target.ToString()}/{lastVersion}"));
                    if (Directory.Exists(lastVersionfullRealOutput))
                    {
                        string[] LastVersionFileList = Directory.GetFiles(lastVersionfullRealOutput, "*.bundle");
                        if (LastVersionFileList.Length != keyvalue.Count) changeList.Add("fix");
                        foreach (var item in LastVersionFileList)
                        {
                            string fileName = Path.GetFileName(item);
                            if (keyvalue.ContainsValue(fileName) == false) changeList.Add(fileName);
                        }

                    }
                }
                if (changeList.Count == 0)
                {
                    Debug.Log("和上一个版本相比没有变化，不提升版本");
                    if (Directory.Exists(fullRealOutput))
                    {
                        Directory.Delete(fullRealOutput, true);
                    }
                    AssetDatabase.DeleteAsset("Assets/" + outputPath);
                    AssetDatabase.Refresh();
                    Debug.Log($"build assetbundle is finish, but content is not change, version is {version}");
                    return;
                }
            }


            var removingList = new List<IncludeAssetBundles>();
            foreach (var item in includeList)
            {
                if (!keyvalue.ContainsKey(item.BundleName))
                {
                    removingList.Add(item);
                }
            }
            foreach (var item in removingList)
            {
                includeList.Remove(item);
            }
            AssetBundleVersionSetting.Instance.GetVersionInfo(target).buildIncludeBundles = includeList;
            AssetBundleVersionSetting.Save();
            AssetDatabase.DeleteAsset("Assets/" + outputPath);
            StringBuilder stringBuilder = new StringBuilder(1024 * 1024);
            stringBuilder.AppendLine(version);
            string[] allBundles = manifest.GetAllAssetBundles();
            List<string> allBundleList = new List<string>(allBundles);
            List<string> preloadFileList = new List<string>();
            foreach (var item in allBundles)
            {
                string id = GenerateMD5(item);
                if (item.Length <= 12) id = item;
                string preloadStr = CollectAssetBundle.IsPreLoad(item) ? "|1" : "|0";
                stringBuilder.AppendLine(keyvalue[item] + preloadStr);
                if (preloadStr == "|1") preloadFileList.Add(keyvalue[item]);
                string[] allDependencies = manifest.GetAllDependencies(item);
                // mf.Deps = allDependencies;
                for (int i = 0; i < allDependencies.Length; i++)
                {
                    id = GenerateMD5(allDependencies[i]);
                    if (allDependencies[i].Length <= 12) id = allDependencies[i];
                    stringBuilder.AppendLine("&" + allDependencies[i]);
                }
                List<BundleConfig> bundleConfigs = BundleConfigAsset.Instance.configs;
                for (int i = 0; i < bundleConfigs.Count; i++)
                {
                    BundleConfig config = bundleConfigs[i];
                    string bundleName = $"{config.bundleName.ToLower()}.bundle";
                    if (bundleName == item)
                    {

                        if (config.isFold)
                        {
                            for (int j = 0; j < config.SubAssetPath.Count; j++)
                            {
                                string subAssetName = Path.GetFileName(config.SubAssetPath[j]);
                                stringBuilder.AppendLine($"#{config.bundleName}/{subAssetName}|{subAssetName}");

                            }
                        }
                        else
                        {

                            string subAssetName = Path.GetFileName(config.AssetPath);
                            stringBuilder.AppendLine($"#{config.AssetAddress}|{subAssetName}");
                        }
                        break;
                    }

                }
            }
            File.WriteAllText(Path.Combine(fullRealOutput, $"Catalog.hash"), stringBuilder.ToString());
            if (preloadFileList.Count > 0)
            {
                stringBuilder.Clear();
                stringBuilder.AppendLine(version);
                for (int i = 0; i < preloadFileList.Count; i++)
                {
                    stringBuilder.AppendLine(preloadFileList[i]);
                }
                File.WriteAllText(Path.Combine(fullRealOutput, $"PreLoad.hash"), stringBuilder.ToString());
            }

            AssetBundleVersionSetting.Instance.GetVersionInfo(target).LastBuildedVersion = version;
            if (AssetBundleVersionSetting.Instance.GetVersionInfo(target).OldVersionList == null)
                AssetBundleVersionSetting.Instance.GetVersionInfo(target).OldVersionList = new List<string>();
            if (!AssetBundleVersionSetting.Instance.GetVersionInfo(target).OldVersionList.Contains(version))
                AssetBundleVersionSetting.Instance.GetVersionInfo(target).OldVersionList.Add(version);
            AssetBundleVersionSetting.Instance.GetVersionInfo(target).BuildingVersion = targetVersionInfo.AutoUpVersion == true ? UpVersion(version) : version;
            AssetBundleVersionSetting.Save();
            stringBuilder.Clear();
            var list = AssetBundleVersionSetting.Instance.GetVersionInfo(target).OldVersionList;
            for (int i = 0; i < list.Count; i++)
            {
                stringBuilder.AppendLine(list[i]);
            }

            File.WriteAllText(Path.Combine(curPath, Path.Combine(outputPath, $"{target.ToString()}/{versionFileName}")), stringBuilder.ToString());



            AssetDatabase.Refresh();
            if (OnBuildFinish != null) OnBuildFinish(target.ToString(), version);
            Debug.Log($"build assetbundle is sucess target is {target.ToString()} version is {version}");
        }

        private static void PostBinaryHandle(string fullRealOutput, string version, Dictionary<string, Manifest> map)
        {
            FileStream fileStream = new FileStream(Path.Combine(fullRealOutput, $"Catalog.txt"), FileMode.CreateNew);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(version);
            foreach (var item in map)
            {
                binaryWriter.Write(item.Value.BundleName);
                binaryWriter.Write(item.Value.BundleHashName);
                binaryWriter.Write(item.Value.size);
                binaryWriter.Write(item.Value.Preload);
                if (item.Value.Addresses != null)
                {
                    binaryWriter.Write(item.Value.Addresses.Length);
                    for (int i = 0; i < item.Value.Addresses.Length; i++)
                    {
                        binaryWriter.Write(item.Value.Addresses[i].Address);
                        binaryWriter.Write(item.Value.Addresses[i].AssetName);
                    }
                }
                else
                {
                    binaryWriter.Write(0);
                }

                if (item.Value.Deps != null)
                {
                    binaryWriter.Write(item.Value.Deps.Length);
                    for (int i = 0; i < item.Value.Deps.Length; i++)
                    {
                        binaryWriter.Write(item.Value.Deps[i]);
                    }
                }
                else
                {
                    binaryWriter.Write(0);
                }
            }
            // binaryWriter.Seek(0,SeekOrigin.Begin);
            // binaryWriter.Write(count);
            binaryWriter.Flush();
            binaryWriter.Close();
        }
        static List<Manifest> manifests;

        // [MenuItem("Tools/Read")]
        private static void ReadBinaryFile()
        {
            FileStream fileStream = new FileStream("/Users/mac/Desktop/UnityProgram/unity-main/Source/AssetBundleOutput/iOS/1.001/Catalog.txt", FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            manifests = new List<Manifest>();
            string version = null;
            try
            {
                while (fileStream.Position < fileStream.Length)
                {
                    if (version == null) version = binaryReader.ReadString();
                    Manifest manifest = new Manifest();
                    manifest.BundleName = binaryReader.ReadString();
                    manifest.BundleHashName = binaryReader.ReadString();
                    manifest.size = binaryReader.ReadInt64();
                    manifest.Preload = binaryReader.ReadBoolean();
                    int length = binaryReader.ReadInt32();
                    if (length != 0)
                    {
                        manifest.Addresses = new Manifest.AddressToAssetName[length];
                        for (int i = 0; i < length; i++)
                        {
                            manifest.Addresses[i] = new Manifest.AddressToAssetName();
                            manifest.Addresses[i].Address = binaryReader.ReadString();
                            manifest.Addresses[i].AssetName = binaryReader.ReadString();
                        }
                    }
                    length = binaryReader.ReadInt32();
                    if (length != 0)
                    {
                        manifest.Deps = new string[length];
                        for (int i = 0; i < length; i++)
                        {
                            manifest.Deps[i] = binaryReader.ReadString();
                        }
                    }
                    manifests.Add(manifest);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                fileStream.Close();
                binaryReader.Close();
            }



        }
        private static string UpVersion(string oldVersion)
        {
            string newVersion = string.Empty;
            int lastIndex = oldVersion.LastIndexOf('.');
            string lastnum = oldVersion.Substring(lastIndex + 1, oldVersion.Length - lastIndex - 1);
            int subVersion = 1;
            if (!int.TryParse(lastnum, out subVersion))
            {
                subVersion = 1;
            }
            subVersion++;
            newVersion = oldVersion.Substring(0, lastIndex) + string.Format(".{0:D3}", subVersion);
            return newVersion;
        }


        public static void BuildAuto()
        {
            Build(EditorUserBuildSettings.activeBuildTarget);
        }


        public static void BuildiOS()
        {
            Build(BuildTarget.iOS);
        }


        public static void BuildStandaloneOSX()
        {
            Build(BuildTarget.StandaloneOSX);
        }


        public static void BuildAndroid()
        {
            Build(BuildTarget.Android);
        }

        public static void BuildWindow()
        {
            Build(BuildTarget.StandaloneWindows);
        }


        public static string GenerateMD5(Stream stream)
        {
            using (var md = MD5.Create())
            {
                byte[] hash = md.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string GenerateMD5(string content)
        {
            using (var md = MD5.Create())
            {
                byte[] hash = md.ComputeHash(Encoding.UTF8.GetBytes(content));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < 6; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }

}
#endif
