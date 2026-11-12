Shader "Stylized/Palette Dither"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Tint", Color) = (1, 1, 1, 1)
        _Darkest ("01 Darkest", Color) = (0.1019608, 0.1294118, 0.1607843, 1)
        _Dark ("02 Dark", Color) = (0.1921569, 0.1294118, 0.2156863, 1)
        _Light ("03 Light", Color) = (0.3176471, 0.1568627, 0.2235294, 1)
        _Lightest ("04 Lightest", Color) = (0.4431373, 0.1921569, 0.2549020, 1)
        _Ambient ("Ambient Light", Range(0, 1)) = 1
        _LightStrength ("Direct Light", Range(0, 2)) = 0.75
        _Exposure ("Exposure", Range(0, 32)) = 2
        _DitherScale ("Dither Pixel Size", Range(1, 8)) = 1
        [HideInInspector] _Cull ("Cull", Float) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _Darkest;
                half4 _Dark;
                half4 _Light;
                half4 _Lightest;
                half _Ambient;
                half _LightStrength;
                half _Exposure;
                float _DitherScale;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                half3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs position = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = position.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionWS = position.positionWS;
                return output;
            }

            static const int Bayer4x4[16] = {
                 0,  8,  2, 10,
                12,  4, 14,  6,
                 3, 11,  1,  9,
                15,  7, 13,  5
            };

            float DitherThreshold(float2 screenPosition)
            {
                int2 pixel = (int2)floor(screenPosition / _DitherScale);
                int x = pixel.x % 4;
                int y = pixel.y % 4;
                return (Bayer4x4[y * 4 + x] + 0.5) / 16.0;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half3 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).rgb * _BaseColor.rgb;
                Light mainLight = GetMainLight(TransformWorldToShadowCoord(input.positionWS));
                half diffuse = saturate(dot(normalize(input.normalWS), mainLight.direction));
                half lightLuma = dot(mainLight.color, half3(0.2126, 0.7152, 0.0722));
                half illumination = saturate(_Ambient + _LightStrength * diffuse * lightLuma * mainLight.shadowAttenuation);
                half luminance = saturate(dot(albedo, half3(0.2126, 0.7152, 0.0722)) * illumination * _Exposure);

                float level = luminance * 3.0;
                float threshold = DitherThreshold(input.positionCS.xy);
                int index = min((int)floor(level) + (frac(level) > threshold ? 1 : 0), 3);
                half3 color = index == 0 ? _Darkest.rgb :
                              index == 1 ? _Dark.rgb :
                              index == 2 ? _Light.rgb : _Lightest.rgb;
                return half4(color, 1.0);
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
