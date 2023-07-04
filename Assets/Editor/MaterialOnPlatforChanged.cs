using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;

[InitializeOnLoad]
public class MaterialOnPlatforChanged : IActiveBuildTargetChanged
{
    public int callbackOrder => 0;

    public static void ChangeMaterials(BuildTarget target)
    {
        var guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat.HasProperty("_BakeStrength"))
            {
                var strength = mat.GetFloat("_BakeStrength");
                var targetStrength = strength;
                if (target == BuildTarget.Android || target == BuildTarget.iOS)
                    targetStrength = 5;
                else
                    targetStrength = 1;
                if (targetStrength != strength)
                {
                    mat.SetFloat("_BakeStrength", targetStrength);
                    EditorUtility.SetDirty(mat);
                }
            }
        }
    }

    public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
    {
        ChangeMaterials(newTarget);
    }

    static MaterialOnPlatforChanged()
    {
        ChangeMaterials(EditorUserBuildSettings.activeBuildTarget);
    }
}

