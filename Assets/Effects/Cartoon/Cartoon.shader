Shader "UPR Performant Effect/Cartoon" {
    Properties {
        _BaseMap ("Albedo", 2D) = "white" { }

        [Space(15)]
        _LightDirection ("Light Direction", vector) = (0.1, 0.2, 0.1, 0)

        [Header(Outline)]
        [Toggle]OutlineSwitch ("Outline Switch", int) = 1
        _Width ("OutlineWidth", Range(0, 1)) = 0.1
        _OutlineColor ("OutlineColor", Color) = (1, 1, 1, 1)

        [Header(Hightlight)]
        _HightlightMap ("Hightlight Map", 2D) = "white" { }
        _HightlightIntensity ("Hightlight Intensity", Range(0, 10)) = 1
        [Toggle]_HightlightMapInvert ("Hightlight Map Invert", int) = 0

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

        [Header(Normal)]
        [Toggle]NormalSwitch ("Normal Switch", int) = 0
        _NormalMap ("Normap Map", 2D) = "white" { }
        _NormalScale ("Normal Scale", float) = 1

        [Header(Fresnel)]
        [Toggle]FresnelSwitch ("Fresnel Switch", int) = 1
        _FresnelColor ("Fresnel Color", Color) = (1, 1, 1, 0)
        _FresnelPower ("Fresnel Power", Range(0, 8)) = 3

        [Header(Alpha)]
        _Alpah ("Alpha", Range(0, 1)) = 1
        [Toggle]AlphaClipping ("Alpah Clipping", int) = 0
        _AlphaClipThreshold ("Threshold", Range(0, 1)) = 0.5

        [Header(Fog)]
        [Toggle]FogSwitch ("Fog Switch", int) = 0

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
            #pragma multi_compile_fog

            #pragma shader_feature OUTLINESWITCH_ON
            #pragma shader_feature DIFFUSESWITCH_ON
            #pragma shader_feature SPECULARSWITCH_ON
            #pragma shader_feature FRESNELSWITCH_ON
            #pragma shader_feature ALPHACLIPPING_ON
            #pragma shader_feature NORMALSWITCH_ON
            #pragma shader_feature FOGSWITCH_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings {
                float4 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 positionCS : SV_POSITION;
                float4 TtoW0 : TEXCOORD3;
                float4 TtoW1 : TEXCOORD4;
                float4 TtoW2 : TEXCOORD5;
                float fogFactor : TEXCOORD6;
            };
            
            sampler2D _BaseMap;
            sampler2D _HightlightMap;
            sampler2D _NormalMap;
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            half4 _LightDirection;

            #if defined(OUTLINESWITCH_ON)
                float _Width;
                half4 _OutlineColor;
            #endif

            float _HightlightIntensity;
            float _HightlightMapInvert;

            #if defined(DIFFUSESWITCH_ON)
                half4 _FrontLightColor;
                half4 _BackLightColor;
                half _DiffuseFrontIntensity;
                half _DiffuseBackIntensity;
            #endif

            #if defined(SPECULARSWITCH_ON)
                half4 _SpecularColor;
                half _SpecularIntensity;
                half _Smoothness;
            #endif

            #if defined(NORMALSWITCH_ON)
                float4 _NormalMap_ST;
                half _NormalScale;
            #endif

            #if defined(FRESNELSWITCH_ON)
                half4 _FresnelColor;
                half _FresnelPower;
            #endif

            half _Alpah;

            #if defined(ALPHACLIPPING_ON)
                half _AlphaClipThreshold;
            #endif
            
            CBUFFER_END

            Varyings Vertex(Attributes input) {

                Varyings output = (Varyings)0;
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                
                #if defined(DIFFUSESWITCH_ON) || defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON) || defined(OUTLINESWITCH_ON)
                    output.normalWS = mul(input.normalOS, (float3x3)unity_WorldToObject);

                    #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON)
                        output.positionWS = mul(unity_ObjectToWorld, input.positionOS).xyz;
                    #endif

                    #if defined(NORMALSWITCH_ON)
                        half3 worldTangent = TransformObjectToWorldDir(input.tangentOS.xyz);
                        half3 worldBinormal = cross(output.normalWS, worldTangent) * input.tangentOS.w;

                        output.TtoW0 = float4(worldTangent.x, worldBinormal.x, output.normalWS.x, output.positionWS.x);
                        output.TtoW1 = float4(worldTangent.y, worldBinormal.y, output.normalWS.y, output.positionWS.y);
                        output.TtoW2 = float4(worldTangent.z, worldBinormal.z, output.normalWS.z, output.positionWS.z);

                        output.uv.zw = input.texcoord.xy * _NormalMap_ST.xy + _NormalMap_ST.zw;
                    #endif

                #endif

                output.uv.xy = input.texcoord.xy * _BaseMap_ST.xy + _BaseMap_ST.zw;

                #if defined(FOGSWITCH_ON)
                    output.fogFactor = ComputeFogFactor(output.positionCS.z);
                #endif

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target {

                half4 albedo = tex2D(_BaseMap, input.uv.xy);
                half3 color = albedo.rgb;

                #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON) || defined(OUTLINESWITCH_ON)
                    #if defined(NORMALSWITCH_ON)
                        half3 positionWS = float3(input.TtoW0.w, input.TtoW1.w, input.TtoW2.w);
                    #else
                        half3 positionWS = input.positionWS;
                    #endif
                #endif

                #if defined(DIFFUSESWITCH_ON) || defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(NORMALSWITCH_ON) || defined(OUTLINESWITCH_ON)
                    #if defined(NORMALSWITCH_ON)
                        half3 normalWS = UnpackNormal(tex2D(_NormalMap, input.uv.zw));
                        normalWS.xy * _NormalScale;
                        normalWS.z = sqrt(1 - saturate(dot(normalWS.xy, normalWS.xy)));
                        normalWS = normalize(half3(dot(input.TtoW0.xyz, normalWS), dot(input.TtoW1.xyz, normalWS), dot(input.TtoW2.xyz, normalWS)));
                    #else
                        half3 normalWS = normalize(input.normalWS);
                    #endif
                #endif

                #if defined(DIFFUSESWITCH_ON) || defined(SPECULARSWITCH_ON)
                    half3 lightDirWS = normalize(_LightDirection.xyz);
                #endif

                #if defined(DIFFUSESWITCH_ON)
                    half halfLambert = dot(normalWS, lightDirWS) * 0.5 + 0.5;
                    half3 diffuse = _FrontLightColor.rgb * albedo.rgb * halfLambert * _DiffuseFrontIntensity;
                    half oneMinusHalfLambert = 1 - halfLambert;
                    diffuse += _BackLightColor.rgb * albedo.rgb * oneMinusHalfLambert * _DiffuseBackIntensity;
                    color = diffuse;
                #endif

                #if defined(SPECULARSWITCH_ON) || defined(FRESNELSWITCH_ON) || defined(OUTLINESWITCH_ON)
                    half3 viewDir = normalize(_WorldSpaceCameraPos.xyz - positionWS);
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

                #if defined(FOGSWITCH_ON)
                    color.rgb = MixFog(color.rgb, input.fogFactor);
                #endif

                #if defined(OUTLINESWITCH_ON)
                    float outlineFactor = saturate(dot(normalWS, normalize(viewDir)));
                    outlineFactor = step(_Width, outlineFactor);
                    color = lerp(_OutlineColor.rgb, color, outlineFactor);
                    _Alpah = lerp(_OutlineColor.a, _Alpah, outlineFactor);
                #endif

                half4 hightFactor = tex2D(_HightlightMap, input.uv.xy);
                hightFactor = lerp(hightFactor, 1 - hightFactor, _HightlightMapInvert);
                color = color + color * hightFactor.rgb * _HightlightIntensity;

                return half4(color, _Alpah);
            }
            ENDHLSL
        }
    }
}