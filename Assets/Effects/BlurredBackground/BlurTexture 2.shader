Shader "UPR Performant Effect/Blurred Background/Blur Texture 2" {
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

            HLSLPROGRAM

            #include "BlurTexture.hlsl"
            #pragma vertex VertexBlurVertical
            #pragma fragment Fragment
            ENDHLSL
        }

        Pass {
            Name "Blur Vertical"

            HLSLPROGRAM

            #include "BlurTexture.hlsl"
            #pragma vertex VertexBlurHorizontal
            #pragma fragment Fragment

            ENDHLSL
        }
    }
}