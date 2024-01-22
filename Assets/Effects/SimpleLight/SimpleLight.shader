Shader "UPR Performant Effect/Simple Light" {
    Properties {
        _BaseMap ("Albedo", 2D) = "white" { }

        [Space(15)]
        _LightDirection ("Light Direction", vector) = (0.1, 0.2, 0.1, 0)

        [Header(Diffuse)]
        [Toggle]DiffuseSwitch ("Diffuse Switch", int) = 1
        _FrontLightColor ("Front Light Color", Color) = (1, 1, 1, 1)
        _BackLightColor ("Back Light Color", Color) = (1, 1, 1, 1)
        _DiffuseFrontIntensity ("Diffuse Front Intensity", float) = 1
        _DiffuseBackIntensity ("Diffuse Back Intensity", float) = 0.3

        [Header(Specular)]
        [Toggle]SpecularSwitch ("Specular Switch", int) = 1
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularIntensity ("Specular Intensity", Range(0, 10)) = 1
        _Smoothness ("Smoothness", Range(0.03, 2)) = 0.35

        [Header(Fresnel)]
        [Toggle]FresnelSwitch ("Fresnel Switch", int) = 1
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 0)
        _FresnelPower ("Fresnel Power", Range(0, 8)) = 3

        [Header(Alpha)]
        _Alpah ("Alpha", Range(0, 1)) = 1
        [Toggle]AlphaClipping ("Alpah Clipping", int) = 0
        _AlphaClipThreshold ("Threshold", Range(0, 1)) = 0.5

        [Header(Other Settings)]
        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend ("SrcBlend", float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend ("DstBlend", float) = 0
        [Enum(On, 1, Off, 0)]_ZWrite ("ZWrite", float) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest ("ZTest", float) = 4
        [Enum(UnityEngine.Rendering.CullMode)]_Cull ("Cull", float) = 1
    }

    SubShader {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        Pass {
            Tags { "LightMode" = "UniversalForward" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            ZTest [_ZTest]
            Cull[_Cull]

            HLSLPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma shader_feature DIFFUSESWITCH_ON
            #pragma shader_feature SPECULARSWITCH_ON
            #pragma shader_feature FRESNELSWITCH_ON
            #pragma shader_feature ALPHACLIPPING_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings {
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 positionCS : SV_POSITION;
            };
            
            sampler2D _BaseMap;
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _LightDirection;
            half4 _FrontLightColor;
            half4 _BackLightColor;
            half _DiffuseFrontIntensity;
            half _DiffuseBackIntensity;
            half4 _SpecularColor;
            half _SpecularIntensity;
            half _Smoothness;
            half4 _FresnelColor;
            half _FresnelPower;
            half _Alpah;
            half _AlphaClipThreshold;
            CBUFFER_END

            Varyings Vertex(Attributes input) {

                Varyings output = (Varyings)0;
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                
                #if defined(DIFFUSESWITCH_ON) || defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON)
                    output.normalWS = mul(input.normalOS, (float3x3)unity_WorldToObject);
                    #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON)
                        output.positionWS = mul(unity_ObjectToWorld, input.positionOS).xyz;
                    #else
                        output.positionWS = half3(0, 0, 0);
                    #endif
                #endif

                output.uv = input.texcoord.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target {

                half4 albedo = tex2D(_BaseMap, input.uv);
                half3 color = albedo.rgb;

                half3 normalWS = normalize(input.normalWS);
                half3 lightDirWS = normalize(_LightDirection.xyz);

                #if defined(DIFFUSESWITCH_ON)
                    half halfLambert = dot(normalWS, lightDirWS) * 0.5 + 0.5;
                    half3 diffuse = _FrontLightColor.rgb * albedo.rgb * halfLambert * _DiffuseFrontIntensity;
                    half oneMinusHalfLambert = 1 - halfLambert;
                    diffuse += _BackLightColor.rgb * albedo.rgb * oneMinusHalfLambert * _DiffuseBackIntensity;
                    color = diffuse;
                #endif

                #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON)
                    half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - input.positionWS.xyz);
                #endif

                #if defined(SPECULARSWITCH_ON)
                    half3 halfDir = normalize(lightDirWS + viewDir);
                    half3 specular = _SpecularColor.rgb * pow(max(0, dot(normalWS, halfDir)), _Smoothness * 256) * _SpecularIntensity;
                    color += specular;
                #endif

                #if defined(FRESNELSWITCH_ON)
                    half3 fresnel = pow((1 - saturate(dot(normalWS, viewDir))), _FresnelPower) * _FresnelColor.rgb;
                    color += fresnel;
                #endif

                #if defined(ALPHACLIPPING_ON)
                    half alphaTest = albedo.a;
                    clip(alphaTest - _AlphaClipThreshold);
                    _Alpah *= alphaTest;
                #endif

                return half4(color, _Alpah);
            }
            ENDHLSL
        }
    }
}