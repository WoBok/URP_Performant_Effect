#ifndef PASS_INCLUDED
#define PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
struct Attributes {
    float4 positionOS : POSITION;
};
struct Varyings {
    float4 positionCS : SV_POSITION;
};
    Varyings Vert(Attributes input) {
        Varyings output = (Varyings)0;

        output.positionCS = TransformObjectToHClip(input.positionOS);
        return output;
    }

    half4 BlurHorizontal(Varyings input) : SV_Target {
        return half4(1, 1, 1, 1);
    }

    half4 BlurVertical(Varyings input) : SV_Target {
        return half4(1, 1, 1, 1);
    }
#endif