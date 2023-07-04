#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
namespace AssetBundleTools.Editor
{
    public class ShaderVariantPreProcessHandle : IPreprocessShaders
    {
        public int callbackOrder => 0;
        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            //// if(CollectAssetBundle.IsExitShaderName(shader.name)){
            //if (data.Count > 1000)
            //{
            //    data.Clear();
            //}
            // }
            // else{
            //     data.Clear();
            // }
        }
    }
}
#endif