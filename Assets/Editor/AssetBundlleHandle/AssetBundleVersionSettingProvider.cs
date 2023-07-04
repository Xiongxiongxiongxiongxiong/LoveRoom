#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
namespace AssetBundleTools.Editor
{
    public class AssetBundleVersionSettingProvider : SettingsProvider
{

    private SerializedObject _serializedObject;


    private SerializedProperty _BuildingVersion;

    private SerializedProperty _OldVersionList;

    private SerializedProperty _LastBuildedVersion;

    private SerializedProperty _AutoUpVersion;

    public List<IncludeAssetBundles> buildIncludeBundles;

    public AssetBundleVersionSettingProvider() : base("Project/AssetBundleVersion Settings", SettingsScope.Project) { }

    public override void OnActivate(string searchContext, VisualElement rootElement)
    {
        // EditorStatusWatcher.OnEditorFocused += OnEditorFocused;
        InitGUI(EditorUserBuildSettings.activeBuildTarget);
    }

    public void OnEditorFocused()
    {
        InitGUI(EditorUserBuildSettings.activeBuildTarget);
        Repaint();
    }

    private void InitserializedObj(int curSelect)
    {
        var setting = AssetBundleVersionSetting.Instance;
        _serializedObject?.Dispose();
        BuildTarget target = GetTarget(curSelect);
        _serializedObject = new SerializedObject(setting);
        string targetstr = target.ToString();

        _BuildingVersion = _serializedObject.FindProperty($"{targetstr}.BuildingVersion");
        _OldVersionList = _serializedObject.FindProperty($"{targetstr}.OldVersionList");
        _LastBuildedVersion = _serializedObject.FindProperty($"{targetstr}.LastBuildedVersion");
        _AutoUpVersion = _serializedObject.FindProperty($"{targetstr}.AutoUpVersion");

        buildIncludeBundles = setting.GetVersionInfo(target).buildIncludeBundles;
    }

    private BuildTarget GetTarget(int curSelect)
    {
        switch (curSelect)
        {
            case 0:
                return BuildTarget.iOS;
            case 1:
                return BuildTarget.Android;
            case 2:
                return BuildTarget.WebGL;
            case 3:
                return BuildTarget.StandaloneOSX;
            case 4:
                return BuildTarget.StandaloneWindows;

        }
        return BuildTarget.iOS;
    }

    private void InitGUI(BuildTarget target)
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                curSelect = 0;
                break;
            case BuildTarget.Android:
                curSelect = 1;
                break;
            case BuildTarget.WebGL:
                curSelect = 2;
                break;
            case BuildTarget.StandaloneOSX:
                curSelect = 3;
                break;
            case BuildTarget.StandaloneWindows:
                curSelect = 4;
                break;
            default:
                curSelect = 0;
                break;
        }
        InitserializedObj(curSelect);

    }
    string[] tabBar = { "IOS", "Android", "WebGL", "StandaloneOSX", "StandaloneWindows" };
    int curSelect = 0;
    int lastSelect = -1;
    bool fadeShow;
    Vector2  scrollViewPos = new Vector2(0,0);
    public override void OnGUI(string searchContext)
    {
        if (_serializedObject == null || !_serializedObject.targetObject)
        {
            InitGUI(EditorUserBuildSettings.activeBuildTarget);
        }

        _serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        curSelect = GUILayout.SelectionGrid(curSelect, tabBar, tabBar.Length);
        if (curSelect != lastSelect)
            InitserializedObj(curSelect);
        
        EditorGUILayout.PropertyField(_AutoUpVersion);
        EditorGUILayout.PropertyField(_BuildingVersion);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(_LastBuildedVersion);
        GUI.enabled = true;
        scrollViewPos = EditorGUILayout.BeginScrollView(scrollViewPos);
        fadeShow = EditorGUILayout.BeginFoldoutHeaderGroup(fadeShow, "将需要构建进应用包内的Bundle打勾");
        if (fadeShow)
        {
            if (buildIncludeBundles != null){
                for (int i = 0; i < buildIncludeBundles.Count; i++)
                {
                        
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.LabelField(buildIncludeBundles[i].BundleName);
                        buildIncludeBundles[i].Include = EditorGUILayout.Toggle(buildIncludeBundles[i].Include);
                        EditorGUILayout.EndHorizontal();
                }
            }
            
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        GUI.enabled = false;
        EditorGUILayout.PropertyField(_OldVersionList);
        GUI.enabled = true;
        EditorGUILayout.EndScrollView();
        if (EditorGUI.EndChangeCheck())
        {
            _serializedObject.ApplyModifiedProperties();
            AssetBundleVersionSetting.Save();
        }
        lastSelect = curSelect;
    }




    static AssetBundleVersionSettingProvider Instance;

    [SettingsProvider]
    public static SettingsProvider VersionSetting()
    {
        if (Instance == null) Instance = new AssetBundleVersionSettingProvider();
        return Instance;
    }
}

}
#endif