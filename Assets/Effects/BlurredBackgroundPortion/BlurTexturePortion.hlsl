#ifndef BLURTEXTRUE_INCLUDED
#define BLURTEXTRUE_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

struct Attributes {
    uint vertexID : SV_VertexID;
};
struct Varyings {
    float4 positionCS : SV_POSITION;
    half2 uv[5] : TEXCOORD0;
};

TEXTURE2D_X(_BlitTexture);
SAMPLER(sampler_BlitTexture);
float4 _BlitTexture_TexelSize;
CBUFFER_START(UnityPerMaterial)
float _BlurSize;
CBUFFER_END

Varyings VertexBlurVertical(Attributes input) {
    Varyings output = (Varyings)0;

    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);

    half2 uv = GetFullScreenTriangleTexCoord(input.vertexID);

    output.uv[0] = uv;
    output.uv[1] = uv + half2(0, _BlitTexture_TexelSize.y) * _BlurSize;
    output.uv[2] = uv - half2(0, _BlitTexture_TexelSize.y) * _BlurSize;
    output.uv[3] = uv + half2(0, _BlitTexture_TexelSize.y * 2) * _BlurSize;
    output.uv[4] = uv - half2(0, _BlitTexture_TexelSize.y * 2) * _BlurSize;

    return output;
}
Varyings VertexBlurHorizontal(Attributes input) {
    Varyings output = (Varyings)0;

    output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);

    half2 uv = GetFullScreenTriangleTexCoord(input.vertexID);

    output.uv[0] = uv;
    output.uv[1] = uv + half2(_BlitTexture_TexelSize.x, 0) * _BlurSize;
    output.uv[2] = uv - half2(_BlitTexture_TexelSize.x, 0) * _BlurSize;
    output.uv[3] = uv + half2(_BlitTexture_TexelSize.x * 2, 0) * _BlurSize;
    output.uv[4] = uv - half2(_BlitTexture_TexelSize.x * 2, 0) * _BlurSize;

    return output;
}
half4 Fragment(Varyings input) : SV_Target {
    float weight[3] = {
        0.4026, 0.2442, 0.0545
    };
    
    half3 sum = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv[0]).rgb * weight[0];
    for (int i = 1; i < 3; i++) {
        sum += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv[2 * i - 1]).rgb * weight[i];
        sum += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv[2 * i]).rgb * weight[i];
    }
    return half4(sum, 1);
}
#endif