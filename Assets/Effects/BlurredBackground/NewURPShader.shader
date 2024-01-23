Shader "URP Shader/NewURPShader" {
    Properties {
        _BaseColor ("Color", Color) = (0, 1, 1, 1)
    }

    SubShader {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always
        ZWrite Off
        Cull Off
        Pass {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            CBUFFER_END

            half4 Fragment(Varyings input) : SV_Target {
                return _BaseColor;
            }
            ENDHLSL
        }
        Pass {
            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            
            CBUFFER_START(UnityPerMaterial)
            half4 _BaseColor;
            CBUFFER_END

            half4 Fragment(Varyings input) : SV_Target {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}