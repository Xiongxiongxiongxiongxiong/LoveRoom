Shader "JagatLit/Room/VolumeLight"
{
    Properties
    { 
        [HDR] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap("Base Map", 2D) = "white"
        _Sleep("sleep",float) = 30

    }

    SubShader
    {
        Tags {      "RenderPipeline"="UniversalRenderPipeline"
     "RenderType"="Transparent"
     "IgnoreProjector"="True"
     "Queue"="Transparent" }

        Pass
        {

       //  Blend SrcAlpha OneMinusSrcAlpha
       //  Blend One OneMinusSrcAlpha
       Blend SrcAlpha One
         Cull off
         ZWrite Off
        	Tags
            {
                "LightMode" = "UniversalForward"
            }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            // 不加会导致不能合批  就会画三次
            CBUFFER_START(UnityPerMaterial)
                // half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _BaseColor;
                half _Sleep;
            CBUFFER_END


            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv)*_BaseColor;
                half colorA=color.a* max(0.3,sin( _Time.x*_Sleep))  ;
                return half4(color.rgb,colorA);
            }
            ENDHLSL
        }
    }
}
