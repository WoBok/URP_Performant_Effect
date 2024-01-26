Shader "UPR Performant Effect/Blurred Background/Blur Texture Portion" {
    Properties {
        _BlurSize ("Blur Size", float) = 2
    }

    SubShader {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        ZTest Always
        ZWrite Off
        Cull Off

        Pass {
            Name "Blur Horizontal"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM

            #include "BlurTexturePortion.hlsl"
            #pragma vertex VertexBlurVertical
            #pragma fragment Fragment
            ENDHLSL
        }

        Pass {
            Name "Blur Vertical"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM

            #include "BlurTexturePortion.hlsl"
            #pragma vertex VertexBlurHorizontal
            #pragma fragment Fragment

            ENDHLSL
        }
    }
}