using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SetMaterialAndShader : EditorWindow
{
    private Dictionary<int, Shader> Shaders= new Dictionary<int, Shader>();
    private Dictionary<int, Material> materials=new Dictionary<int, Material>();

    [MenuItem("Tool/SetMaterialAndSetShader")]
    private static void OpenWindow()
    {
        SetMaterialAndShader window = GetWindow<SetMaterialAndShader>();
      //  window.titleContent = new GUIContent("Material Manager");
        window.Show();
    }

    private void OnEnable()
    {

        GetMaterials();
    }

    private void GetMaterials()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] rendererMaterials = renderer.sharedMaterials;

            foreach (Material material in rendererMaterials)
            {
                int materialID = material.GetInstanceID();

                if (!Shaders.ContainsKey(materialID))
                {
                    Shaders.Add(materialID, material.shader);
                }

                if (!materials.ContainsKey(materialID))
                {
                    materials.Add(materialID, material);
                }
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("设置到Lit");
        if (GUILayout.Button("SetMatersToLit"))
        {
            SetShaderToURPLit();
        }
        GUILayout.Label("还原到以前的shader");
        if (GUILayout.Button("RestoreShader"))
        {
            RestoreOriginalShader();
        }
    }

    private void SetShaderToURPLit()
    {
        foreach (KeyValuePair<int, Material> kvp in materials)
        {
            Material material = kvp.Value;
            material.shader = Shader.Find("Universal Render Pipeline/Lit");
        }
    }

    private void RestoreOriginalShader()
    {
        foreach (KeyValuePair<int, Material> kvp in materials)
        {
            Material material = kvp.Value;
            int materialID = kvp.Key;

            if (Shaders.ContainsKey(materialID))
            {
                Shader originalShader = Shaders[materialID];
                material.shader = originalShader;
            }
        }
    }
}
