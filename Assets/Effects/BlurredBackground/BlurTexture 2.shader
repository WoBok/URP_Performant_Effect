Shader "UPR Performant Effect/Blurred Background/Blur Texture 2" {
    Properties {
        _MainTex ("Texture", 2D) = "white" { }
        _BlurOffset ("Blur Offset", Vector) = (5, 5, 0, 0)
    }

    SubShader {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass {
            Name "Blur Horizontal"

            HLSLPROGRAM

            #include "BlurTexture.hlsl"
            #pragma vertex Vert
            #pragma fragment BlurHorizontal
            ENDHLSL
        }

        Pass {
            Name "Blur Vertical"

            HLSLPROGRAM

            #include "BlurTexture.hlsl"
            #pragma vertex Vert
            #pragma fragment BlurVertical
            ENDHLSL
        }
    }
}