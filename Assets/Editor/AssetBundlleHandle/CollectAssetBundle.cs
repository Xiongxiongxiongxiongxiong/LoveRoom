#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using System;
using UnityEditor.U2D;
using UnityEngine.U2D;
using Jagat.DressAssetBundle;
using Jagat.DressAssetBundle.Editors;

namespace AssetBundleTools.Editor
{
    [XmlRoot("linker")]
    public class AssemblyList
    {
        [XmlElement("assembly")]
        public List<AssemblyNode> assemblyNodes;

        public AssemblyNode GetAssemblyNode(string assemblyName)
        {
            foreach (var item in assemblyNodes)
            {
                if (item.fullname == assemblyName) return item;
            }
            return null;
        }
    }
    public class AssemblyNode
    {
        [XmlAttribute("fullname")]
        public string fullname { get; set; }
        [XmlAttribute("preserve")]
        public string preserve { get; set; }
        [XmlElement("type")]
        public List<TypeNode> typeNodes;

        public TypeNode GetTypeNode(string name)
        {
            foreach (var item in typeNodes)
            {
                if (item.fullname == name) return item;
            }
            return null;
        }
    }

    public class TypeNode
    {
        [XmlAttribute("fullname")]
        public string fullname { get; set; }
        [XmlAttribute("preserve")]
        public string preserve { get; set; }
    }
    public class CollectAssetBundle
    {
        static HashSet<string> buildAssetMap = new HashSet<string>();
        static HashSet<string> allDependencies = new HashSet<string>();
        static HashSet<string> allClassNames = new HashSet<string>();

        static HashSet<string> AllPreLoadBundles = new HashSet<string>();

        static DepMode mode;

        static HashSet<string> shaderNames = new HashSet<string>();
        [MenuItem("Tools/AssetbundleTest")]
        public static AssetBundleBuild[] Collect()
        {
            shaderNames.Clear();
            buildAssetMap.Clear();
            allDependencies.Clear();
            AllPreLoadBundles.Clear();
            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            ReadAssetList();
            GetAddressableBundleInfos(list);
            return list.ToArray();
        }

        /// <summary>
        /// 是否是预加载设置
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static bool IsPreLoad(string bundleName)
        {
            return AllPreLoadBundles.Contains(bundleName);
        }

        /// <summary>
        /// shader的名字是否在bundle包内
        /// </summary>
        /// <param name="shaderName"></param>
        /// <returns></returns>
        public static bool IsExitShaderName(string shaderName)
        {
            bool exit = shaderNames.Contains(shaderName);
            return exit;
        }

        /// <summary>
        /// 收集由文本配置的key和资源路径
        /// </summary>
        private static void ReadAssetList()
        {
            string curDir = Directory.GetCurrentDirectory();
            string filePath = $"{curDir}/Assets/asset_list.txt";
            bool exit = File.Exists(filePath);
            if (exit == false) return;
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            StreamReader streamReader = new StreamReader(fileStream);
            var inst = AddressDefineObjectSetting.Instance.activeAddressDefineObject;
            if (inst == null || inst.addressList == null)
            {
                Debug.Log("AssetBundleConfigSettings 请先设置AssetBundle");
                return;
            }
            List<AddressInfo> configs = inst.addressList;
            string line = "";
            while (fileStream.Position <= fileStream.Length && line != null)
            {
                line = streamReader.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                string[] lineArray = line.Split('\t');
                if (lineArray.Length != 2) continue;
                string key = lineArray[0];
                string path = Path.Combine(curDir, lineArray[1]);
                string guid = AssetDatabase.AssetPathToGUID(lineArray[1]);
                if (File.Exists(path))
                {
                    exit = false;
                    foreach (var item in configs)
                    {
                        if (/*item.AssetPath == lineArray[1] || */item.guid == guid)
                        {
                            exit = true;
                            break;
                        }
                        //if (item.SubAssetPath != null)
                        //{
                        //    foreach (var it in item.SubAssetPath)
                        //    {
                        //        if (it == lineArray[1])
                        //        {
                        //            exit = true;
                        //            break;
                        //        }
                        //    }
                        //    if (exit == true) break;
                        //}
                    }
                    if (exit)
                    {
                        Debug.Log("已经存在相同的资源，资源路径为：" + path);
                    }
                    else
                    {
                        AddressInfo bundleConfig = new AddressInfo();
                        bundleConfig.guid = guid;
                        bundleConfig.address = key;
                        bundleConfig.active = true;
                        //bundleConfig.AssetAddress = key;
                        //bundleConfig.bundleName = GetBundleName(lineArray[1]);
                        //bundleConfig.AssetPath = lineArray[1];
                        //bundleConfig.GUID = guid;
                        //bundleConfig.isFold = false;
                        //bundleConfig.PreLoad = false;
                        configs.Add(bundleConfig);
                    }
                }

            }
            fileStream.Close();
            inst.addressList = configs;
            //BundleConfigAsset.Save();
            EditorUtility.SetDirty(inst);
        }

        /// <summary>
        /// 从配置中得到需要生成的bundle和资源
        /// </summary>
        /// <param name="list"></param>
        private static void GetAddressableBundleInfos(List<AssetBundleBuild> list)
        {
            BundleConfigAsset inst = BundleConfigAsset.Instance;
            if (inst == null || inst.configs == null)
            {
                Debug.Log("AssetBundleConfigSettings 请先设置AssetBundle");
                return;
            }
            allClassNames.Clear();
            mode = inst.model;
            string curDir = Directory.GetCurrentDirectory().Replace('\\', '/');
            for (int i = 0; i < inst.configs.Count; i++)
            {
                var config = inst.configs[i];
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = config.bundleName;
                build.assetBundleVariant = "bundle";
                if (config.PreLoad)
                    AllPreLoadBundles.Add(config.bundleName.ToLower() + ".bundle");
                if (config.isFold)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(config.GUID);
                    var files = Directory.GetFiles(Path.Combine(curDir, assetPath), "*.*", SearchOption.TopDirectoryOnly);
                    if (files != null)
                    {
                        if (config.SubAssetPath == null) config.SubAssetPath = new List<string>();
                        config.SubAssetPath.Clear();
                        for (int j = 0; j < files.Length; j++)
                        {
                            if (files[j].EndsWith(".meta") || files[i].EndsWith(".DS_Store"))
                            {
                            }
                            else
                            {
                                string localPath = files[j].Replace("\\", "/").Replace(curDir + "/", "");
                                config.SubAssetPath.Add(localPath);
                            }
                        }
                    }
                    if (config.SubAssetPath == null) continue;
                    string[] subAssets = config.SubAssetPath.ToArray();
                    for (int j = 0; j < subAssets.Length; j++)
                    {
                        buildAssetMap.Add(subAssets[j]);
                        if (subAssets[j].EndsWith(".spriteatlas") || subAssets[j].EndsWith(".spriteatlasv2"))
                        {
                            SpriteAtlasHandle(config.AssetPath);
                        }

                        CollectClass(config.AssetPath);
                    }
                    build.assetNames = subAssets;
                }
                else if (string.IsNullOrEmpty(config.GUID) == false)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(config.GUID);
                    if (string.IsNullOrEmpty(assetPath) == false)
                    {
                        config.AssetPath = assetPath;
                        build.assetNames = new string[] { assetPath };
                        buildAssetMap.Add(assetPath);
                        if (config.AssetPath.EndsWith(".spriteatlas") || config.AssetPath.EndsWith(".spriteatlasv2"))
                        {
                            SpriteAtlasHandle(config.AssetPath);
                        }
                        CollectClass(assetPath);
                    }

                }
                else if (string.IsNullOrEmpty(config.AssetPath) == false)
                {
                    bool exit = File.Exists(Path.Combine(Directory.GetCurrentDirectory(), config.AssetPath));
                    if (exit == true)
                    {
                        build.assetNames = new string[] { config.AssetPath };
                        buildAssetMap.Add(config.AssetPath);
                        if (config.AssetPath.EndsWith(".spriteatlas") || config.AssetPath.EndsWith(".spriteatlasv2"))
                        {
                            SpriteAtlasHandle(config.AssetPath);
                        }
                        CollectClass(config.AssetPath);
                    }
                }
                if (build.assetNames != null && build.assetNames.Length > 0)
                    list.Add(build);
            }
            WriteToLink();
            AssetDepHandle(list);
        }

        private static void AssetDepHandle(List<AssetBundleBuild> list)
        {
            Dictionary<string, HashSet<string>> autoCollectBundleName = new Dictionary<string, HashSet<string>>();
            foreach (var item in buildAssetMap)
            {
                HashSet<string> hashSet = new HashSet<string>();
                GetAllDep(hashSet, item);
                foreach (var it in hashSet)
                {
                    if (it.EndsWith(".shader") || it.EndsWith(".shadergraph"))
                    {
                        Shader shader = AssetDatabase.LoadAssetAtPath(it, typeof(Shader)) as Shader;
                        if (shader != null)
                        {
                            shaderNames.Add(shader.name);
                        }

                    }
                    if (buildAssetMap.Contains(it))
                    {
                    }
                    else
                    {
                        string bundleDic = Path.GetDirectoryName(it);
                        if (autoCollectBundleName.TryGetValue(bundleDic, out var resList))
                        {
                            resList.Add(it);
                        }
                        else
                        {
                            autoCollectBundleName.Add(bundleDic, new HashSet<string>() { it });
                        }

                    }
                }
            }
            string[] assetList = null;
            int maxCount = 10;
            foreach (var item in autoCollectBundleName)
            {
                if (item.Value != null && item.Value.Count > 0)
                {
                    int totalCount = item.Value.Count;
                    int index = 1;
                    int assetIndex = 0;
                    assetList = new string[totalCount];
                    item.Value.CopyTo(assetList);
                    while (totalCount > 0)
                    {
                        string bundleName = GetBundleName(item.Key) + index.ToString();
                        AssetBundleBuild build = new AssetBundleBuild();
                        build.assetBundleName = bundleName;
                        build.assetBundleVariant = "bundle";
                        int count = totalCount > maxCount ? maxCount : totalCount;
                        totalCount = totalCount - count;
                        build.assetNames = new string[count];
                        for (int i = 0; i < count; i++)
                        {
                            build.assetNames[i] = assetList[assetIndex];
                            assetIndex++;
                        }

                        list.Add(build);
                        index++;
                    }

                }

                // foreach (var it in item.Value)
                // {
                //     Debug.Log($"新增的包名为===》{item.Key}  ===>资源为{it}");
                // }

            }
        }

        private static string GetBundleName(string path)
        {
            bool isdir = isDic(Path.Combine(Directory.GetCurrentDirectory(), path));
            string bundleName = path.Replace("\\", "/");
            if (bundleName.StartsWith("Packages/"))
            {
                bundleName = bundleName.Substring(9);
            }
            else if (bundleName.StartsWith("Assets/"))
            {
                bundleName = bundleName.Substring(7);
            }
            bundleName = bundleName.Replace('/', '_');
            if (isdir == false)
            {
                int index = bundleName.LastIndexOf(".");
                if (index != -1)
                {
                    bundleName = bundleName.Substring(0, index);
                }
            }
            return bundleName;
        }
        private static void GetAllDep(HashSet<string> list, string resPath)
        {
            if (list.Contains(resPath)) return;
            switch (mode)
            {
                case DepMode.IgnoreAll:
                    return;
                case DepMode.IgnorePackage:
                    if (resPath.StartsWith("Packages/"))
                    {
                        return;
                    }
                    break;
                case DepMode.IgnoreAssets:
                    if (resPath.StartsWith("Assets/"))
                    {
                        return;
                    }
                    break;
                default:
                    break;
            }
            list.Add(resPath);
            string[] dep = Dependencies(resPath);
            if (dep == null || dep.Length == 0) return;
            for (int i = 0; i < dep.Length; i++)
            {
                GetAllDep(list, dep[i]);
            }
        }

        private static void CollectClass(string path)
        {
            var allDependencies = AssetDatabase.GetDependencies(path);
            foreach (var item in allDependencies)
            {
                if (item.EndsWith(".cs"))
                {
                    allClassNames.Add(item);
                }
            }
        }

        private static void WriteToLink()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Scripts/link.xml");
            if (File.Exists(path) == false) return;
            FileStream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            if (stream != null)
            {
                var xmlser = new XmlSerializer(typeof(AssemblyList));
                AssemblyList obj = xmlser.Deserialize(stream) as AssemblyList;
                foreach (var item in allClassNames)
                {
                    UnityEditor.MonoScript monoScript = AssetDatabase.LoadAssetAtPath(item, typeof(UnityEditor.MonoScript)) as UnityEditor.MonoScript;
                    if (obj != null && monoScript.GetClass() != null)
                    {
                        string className = monoScript.GetClass().FullName;
                        string assemblyName = monoScript.GetClass().Assembly.GetName().Name;
                        var assemblyNode = obj.GetAssemblyNode(assemblyName);
                        if (assemblyNode == null)
                        {
                            assemblyNode = new AssemblyNode();
                            assemblyNode.fullname = assemblyName;
                            assemblyNode.typeNodes = new List<TypeNode>();
                            TypeNode typeNode = new TypeNode();
                            typeNode.fullname = className;
                            typeNode.preserve = "all";
                            assemblyNode.typeNodes.Add(typeNode);
                            obj.assemblyNodes.Add(assemblyNode);
                        }
                        else
                        {
                            var typeNode = assemblyNode.GetTypeNode(className);
                            if (typeNode == null)
                            {
                                typeNode = new TypeNode();
                                typeNode.fullname = className;
                                typeNode.preserve = "all";
                                assemblyNode.typeNodes.Add(typeNode);
                            }
                        }

                        // Debug.Log($"类的名字{className},类的程序集{assemblyName}");
                    }
                }
                stream.Dispose();
                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.OmitXmlDeclaration = true;
                xws.Encoding = Encoding.UTF8;
                if (File.Exists(path)) File.Delete(path);
                FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite);
                XmlWriter xmlWriter = XmlWriter.Create(fs, xws);

                XmlSerializerNamespaces xsns = new XmlSerializerNamespaces(new XmlQualifiedName[] { new XmlQualifiedName(string.Empty, "aa") });
                xmlser.Serialize(xmlWriter, obj, xsns);
                xmlWriter.Flush();
                xmlWriter.Dispose();
                fs.Close();

                XmlDocument xd = new XmlDocument();
                xd.Load(path);
                xd.InsertBefore(xd.CreateXmlDeclaration("1.0", "utf-8", null), xd.DocumentElement);
                xd.Save(path);

            }


        }

        private static string[] Dependencies(string assetPath)
        {
            bool recursive = false;
            string[] dependencies = AssetDatabase.GetDependencies(assetPath, recursive);
            List<string> result = new List<string>();
            result.Add(assetPath);
            foreach (var path in dependencies)
            {
                if (path.EndsWith(".cs") == false)
                {
                    // Debug.Log("=" + path);
                    result.Add(path);
                }

            }
            return result.ToArray();
        }

        private static bool isDic(string path)
        {
            if (File.Exists(path)) return false;
            else if (Directory.Exists(path)) return true;
            return false;
        }

        private static void SpriteAtlasHandle(string assetPath)
        {
            SpriteAtlas spriteAtlas = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SpriteAtlas)) as SpriteAtlas;
            var dd = SpriteAtlasExtensions.GetPackables(spriteAtlas);
            string curDir = Directory.GetCurrentDirectory().Replace("\\", "/");
            if (dd != null && dd.Length != 0)
            {
                foreach (var item in dd)
                {
                    string path = AssetDatabase.GetAssetPath(item);
                    string fullPath = Path.Combine(curDir, path).Replace("\\", "/");
                    bool isdic = isDic(fullPath);
                    if (isdic)
                    {
                        string[] files = Directory.GetFiles(fullPath, "*.*", SearchOption.TopDirectoryOnly);
                        for (int j = 0; j < files.Length; j++)
                        {
                            if (files[j].EndsWith(".png") || files[j].EndsWith("*.jpg") || files[j].EndsWith(".jpeg"))
                            {
                                string localPath = files[j].Replace("\\", "/").Replace(curDir + "/", "");
                                buildAssetMap.Add(localPath);
                            }

                        }
                    }
                    else
                    {
                        if (AssetDatabase.IsValidFolder(path))
                        {
                            buildAssetMap.Add(path);
                        }
                    }
                }
            }
        }

        // [MenuItem("Tools/AssetBundle/AddressableToBundleTool")]
        // private static void AddressableToBundleTool()
        // {
        //     string[] guids = AssetDatabase.FindAssets("t:AddressableAssetGroup");
        //     if (guids == null)
        //     {
        //         Debug.Log("没有找到相关的类型：AddressableAssetGroup");
        //         return;
        //     }
        //     BundleConfigAsset inst = BundleConfigAsset.Instance;
        //     if (inst == null)
        //     {
        //         Debug.Log("AssetBundleConfigSettings 请先设置AssetBundle");
        //         return;
        //     }
        //     inst.configs = new List<BundleConfig>();
        //     List<BundleConfig> list = new List<BundleConfig>();
        //     for (int i = 0; i < guids.Length; i++)
        //     {
        //         string path = AssetDatabase.GUIDToAssetPath(guids[i]);
        //         AddressableAssetGroup group = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(path);
        //         foreach (var item in group.entries)
        //         {
        //             BundleConfig config = new BundleConfig();
        //             config.AssetPath = item.AssetPath;
        //             config.bundleName = item.address.Replace("/", "_");
        //             config.AssetAddress = item.address;
        //             config.GUID = item.guid;
        //             if (item.IsFolder)
        //             {
        //                 if (item.SubAssets == null) continue;
        //                 if (config.SubAssetPath == null) config.SubAssetPath = new List<string>();
        //                 string[] subAssets = new string[item.SubAssets.Count];
        //                 for (int j = 0; j < subAssets.Length; j++)
        //                 {
        //                     config.SubAssetPath.Add(item.SubAssets[j].AssetPath);
        //                 }
        //             }
        //             else if (item.IsSubAsset == false)
        //             {

        //                 // if(config.SubAssetPath == null)config.SubAssetPath = new List<string>();
        //                 // config.SubAssetPath.Add(item.AssetPath);
        //             }
        //             inst.configs.Add(config);
        //             //    Debug.Log($"group is {group.name} =={item.address};;;;;path is {item.AssetPath}");
        //         }
        //         // Debug.Log($"guid is {guids[i]} path is {path}");
        //     }
        //     BundleConfigAsset.Save();
        //     Debug.Log("转换完成");
        // }
    }

}
#endif