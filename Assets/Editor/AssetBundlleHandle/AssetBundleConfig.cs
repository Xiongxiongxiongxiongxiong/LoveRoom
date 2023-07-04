#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.IO;
using System.Text;

namespace AssetBundleTools.Editor
{
    [Serializable]
    public class BundleConfig
    {
        [Header("包名")]
        public string bundleName;
        [Header("资源路径")]
        public string AssetPath;

        [Header("资源地址")]
        public string AssetAddress;
        [Header("Guid")]
        public string GUID;

        [Header("资源路径")]
        public List<string> SubAssetPath;

        public bool isFold;

        public bool PreLoad;
    }


    public class AssetBundleConfig : EditorWindow
    {
        private Dictionary<int, bool> subAssetFaldout = new Dictionary<int, bool>();
        private Dictionary<int, bool> bundleFaldout = new Dictionary<int, bool>();
        private Dictionary<int, float> elementHeights = new Dictionary<int, float>();
        private List<int> selects = new List<int>();

        private bool removeSelect = false;
        private int curItemSelect = -1;
        private static int change = 0;
        List<BundleConfig> configs;
        DepMode model = DepMode.IncludeAll;
        //int modeSelect = 0;
        [MenuItem("Tools/AssetBundle/AssetBundleTool")]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetBundleConfig>();
            window.titleContent = new GUIContent("AssetBundleConfig");
            window.Show();
        }
	    [ContextMenu("Copy2AddressDefineObject")]
        public void Copy2AddressDefineObject()
        {
           var obj =  Jagat.DressAssetBundle.AddressDefineObjectSetting.Instance.activeAddressDefineObject;
            obj.addressList = new List<Jagat.DressAssetBundle.AddressInfo>();
            foreach (var item in configs)
            {
                obj.addressList.Add(new Jagat.DressAssetBundle.AddressInfo() { 
                    active = true,
                    address = item.AssetAddress,
                    guid = item.GUID
                });
            }
            EditorUtility.SetDirty(obj);
        }
        // [MenuItem("Tools/AssetBundle/AssetBundleGraph")]
        // private static void ShowGraphWindow()
        // {
        //     var graph = AssetDatabase.LoadAssetAtPath<Jagat.ABGraph>("Assets/Def/AssetBundleGraph.asset");
        //     if (graph != null)
        //     {
        //         var window = GetWindow<UTown.NodeGraph.NodeGraphWindow>();
        //         window.OpenGraph(graph);
        //     }
        // }
        private void OnEnable()
        {
            model = BundleConfigAsset.Instance.model;
            subAssetFaldout.Clear();
            bundleFaldout.Clear();
            var name = BundleConfigAsset.Instance.name; ;
            curSelect = GetIndex(EditorUserBuildSettings.activeBuildTarget);
        }

        private void RunBuild(int index)
        {
            switch (index)
            {
                case 0:
                    BuildAssetBundle.Build(BuildTarget.iOS);
                    return;
                case 1:
                    BuildAssetBundle.Build(BuildTarget.Android);
                    return;
                case 2:
                    BuildAssetBundle.Build(BuildTarget.WebGL);
                    return;
                case 3:
                    BuildAssetBundle.Build(BuildTarget.StandaloneOSX);
                    return;
                case 4:
                    BuildAssetBundle.Build(BuildTarget.StandaloneWindows64);
                    return;
            }
            BuildAssetBundle.BuildAuto();
        }

        private int GetIndex(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.iOS:
                    return 0;
                case BuildTarget.Android:
                    return 1;
                case BuildTarget.WebGL:
                    return 2;
                case BuildTarget.StandaloneOSX:
                    return 3;
                case BuildTarget.StandaloneWindows64:
                    return 4;
            }
            return 5;
        }

        private void onSelect(ReorderableList list)
        {
            int Length = list.count;
            selects.Clear();
            for (int i = 0; i < Length; i++)
            {
                if (list.IsSelected(i))
                {
                    Debug.Log("this selecet is " + i.ToString());
                    selects.Add(i);
                }
            }
        }

        private void onMouseDrag(){

        }
        Vector2 vector2 = Vector2.zero;
        //bool mainFaldout = false;
        //int curSelectIndex = -1;

        ReorderableList _orderableList;

        string[] tabBar = { "iOS", "Android", "WebGL", "StandaloneOSX", "StandaloneWindows64", "Auto" };
        int curSelect = 0;

        private void OnGUI()
        {
            configs = BundleConfigAsset.Instance.configs;
            BundleConfigAsset.Instance.model = model;
            EditorGUI.BeginChangeCheck();
            if (_orderableList == null)
            {
                _orderableList = new ReorderableList(configs, typeof(BundleConfig));
                _orderableList.drawHeaderCallback += DrawHead;
                _orderableList.drawElementCallback += DrawElement;
                _orderableList.elementHeightCallback += GetHeight;
                _orderableList.multiSelect = true;
                _orderableList.onSelectCallback += onSelect;
            }
            vector2 = GUILayout.BeginScrollView(vector2);
            _orderableList.DoLayoutList();
            GUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck() || change == 2)
            {
                BundleConfigAsset.Instance.configs = configs;
                BundleConfigAsset.Save();
                change = 0;
            }

            if (GUILayout.Button("Build"))
            {
                RunBuild(curSelect);
            }
            curSelect = GUILayout.Toolbar(curSelect, tabBar);
            if(position.Contains(Event.current.mousePosition)&& Event.current.type == EventType.MouseDrag&&Selection.objects != null && Selection.objects.Length !=0)
            {
                Debug.Log("==============");
            }
            if (removeSelect)
            {
                if (selects.Count != 0)
                {
                    for (int i = selects.Count - 1; i >= 0; i--)
                    {
                        configs.RemoveAt(selects[i]);
                    }
                }
                _orderableList.ClearSelection();
                change = 1;
                removeSelect = false;
            }
            if (change == 1) change++;


        }

        private float GetHeight(int index)
        {
            if (elementHeights.ContainsKey(index) == false) elementHeights.Add(index, EditorGUIUtility.singleLineHeight);
            return elementHeights[index];
        }

        private void DrawHead(Rect rect)
        {
            rect.x = 25;
            EditorGUI.LabelField(rect, "AssetBundleName");
            rect.x = EditorGUIUtility.currentViewWidth/2-120;
            EditorGUI.LabelField(rect, "DependenceMode");
            rect.x = EditorGUIUtility.currentViewWidth/2;
            rect.width = 120;
            model = (DepMode) EditorGUI.EnumPopup(rect,model);
            rect.x = EditorGUIUtility.currentViewWidth - 80;
            EditorGUI.LabelField(rect, "PreLoad");
        }
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {

            BundleConfig config = configs[index];
            if (!bundleFaldout.ContainsKey(index))
            {
                bundleFaldout.Add(index, false);
            }
            if (string.IsNullOrEmpty(config.bundleName))
                bundleFaldout[index] = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), bundleFaldout[index], "Element " + index);
            else
                bundleFaldout[index] = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), bundleFaldout[index], config.bundleName);
            config.PreLoad = EditorGUI.Toggle(new Rect(EditorGUIUtility.currentViewWidth - 65, rect.y, 80, EditorGUIUtility.singleLineHeight), config.PreLoad);
            if (bundleFaldout[index])
            {
                Rect rect1 = new Rect(rect.x + 10, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight);
                config.bundleName = EditorGUI.TextField(rect1, "AssetBundleName", config.bundleName);

                rect1 = new Rect(rect.x + 10, rect.y + EditorGUIUtility.singleLineHeight * 2, rect.width, EditorGUIUtility.singleLineHeight);
                config.AssetPath = EditorGUI.TextField(rect1, "AssetPath", config.AssetPath);
                string guid = OnDragAndDrop(rect1);

                config.AssetPath = AssetDatabase.GUIDToAssetPath(config.GUID);
                config.GUID = guid == string.Empty ? config.GUID : guid;
                // config.GUID = AssetDatabase.AssetPathToGUID(config.AssetPath);
                rect1 = new Rect(rect.x + 10, rect.y + EditorGUIUtility.singleLineHeight * 3, rect.width, EditorGUIUtility.singleLineHeight);
                config.AssetAddress = EditorGUI.TextField(rect1, "AssetAddress", config.AssetAddress);
                if (AssetDatabase.IsValidFolder(config.AssetPath))
                {
                    config.isFold = true;
                    string curDir = Directory.GetCurrentDirectory().Replace('\\', '/');
                    if (string.IsNullOrEmpty(config.bundleName))
                        config.bundleName = config.AssetPath.Replace('/', '_');
                    var files = Directory.GetFiles(Path.Combine(curDir, config.AssetPath), "*.*", SearchOption.TopDirectoryOnly);
                    if (files != null)
                    {
                        if (config.SubAssetPath == null) config.SubAssetPath = new List<string>();
                        config.SubAssetPath.Clear();
                        for (int i = 0; i < files.Length; i++)
                        {
                            if (files[i].EndsWith(".meta") || files[i].EndsWith(".DS_Store"))
                            {
                            }
                            else
                            {
                                string localPath = files[i].Replace("\\", "/").Replace(curDir + "/", "");
                                config.SubAssetPath.Add(localPath);
                            }
                        }
                    }
                }
                else
                {
                    config.isFold = false;
                    if (string.IsNullOrEmpty(config.AssetPath) == false)
                    {
                        if (string.IsNullOrEmpty(config.bundleName))
                        {
                            config.bundleName = config.AssetPath.Substring(0, config.AssetPath.LastIndexOf(".")).Replace('/', '_');
                            config.AssetAddress = $"{config.bundleName}/{Path.GetFileName(config.AssetPath)}";
                        }

                    }
                }
                if (config.SubAssetPath != null)
                {
                    if (!subAssetFaldout.ContainsKey(index))
                    {
                        subAssetFaldout.Add(index, false);
                    }
                    subAssetFaldout[index] = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y + EditorGUIUtility.singleLineHeight * 4, rect.width, EditorGUIUtility.singleLineHeight), subAssetFaldout[index], "SubAssetPaths");
                    if (subAssetFaldout[index])
                    {
                        elementHeights[index] = EditorGUIUtility.singleLineHeight * (5 + config.SubAssetPath.Count);
                        GUI.enabled = false;
                        for (int i = 0; i < config.SubAssetPath.Count; i++)
                        {
                            rect1 = new Rect(rect.x + 20, rect.y + EditorGUIUtility.singleLineHeight * (5 + i), rect.width, EditorGUIUtility.singleLineHeight);
                            config.SubAssetPath[i] = EditorGUI.TextField(rect1, "AssetName", config.SubAssetPath[i]);
                        }
                        GUI.enabled = true;
                    }
                    else
                    {
                        elementHeights[index] = EditorGUIUtility.singleLineHeight * 5;
                    }

                }
                else
                {
                    elementHeights[index] = EditorGUIUtility.singleLineHeight * 4;
                }
                if (string.IsNullOrEmpty(guid) == false)
                {
                    change = 1;
                }

            }
            else
            {
                elementHeights[index] = EditorGUIUtility.singleLineHeight;
            }
            rect.height = elementHeights[index];
            if (rect.Contains(Event.current.mousePosition) && Event.current.rawType == EventType.MouseDown)
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(config.AssetPath);
                EditorGUIUtility.PingObject(obj);
                Selection.activeObject = obj;
            }
            if (rect.Contains(Event.current.mousePosition) && Event.current.rawType == EventType.ContextClick && selects.Contains(index))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("清除选项"), false, OnClickRemove, "删除");
                if (selects.Count == 1)
                {
                    curItemSelect = index;
                    menu.AddItem(new GUIContent("向上插入"), false, LastInsert, "向上插入");
                    menu.AddItem(new GUIContent("向下插入"), false, NextInsert, "向下插入");
                }

                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        private void LastInsert(object sender)
        {
            configs.Insert(curItemSelect,new BundleConfig());
            change = 1;
        }

        private void NextInsert(object sender){
            configs.Insert(curItemSelect+1,new BundleConfig());
            change = 1;
        }

        private void OnClickRemove(object sender)
        {
            removeSelect = true;
        }
        private string OnDragAndDrop(Rect rect)
        {
            if (mouseOverWindow == this)
            {
                //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  Event.current.type == EventType.DragUpdated
                if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) && rect.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                }
                if ((Event.current.type == EventType.DragExited)
                  && rect.Contains(Event.current.mousePosition))
                {
                    // //改变鼠标的外表  
                    // DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        return AssetDatabase.AssetPathToGUID(DragAndDrop.paths[0]);
                    }
                }
            }
            return string.Empty;
        }
    }

}
#endif

