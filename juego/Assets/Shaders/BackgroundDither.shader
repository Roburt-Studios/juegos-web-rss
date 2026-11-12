Shader "Stylized/Background Dither"
{
    Properties
    {
        _Darkest ("Far", Color) = (0.1019608, 0.1294118, 0.1607843, 1)
        _Dark ("Outer", Color) = (0.1921569, 0.1294118, 0.2156863, 1)
        _Light ("Inner", Color) = (0.3176471, 0.1568627, 0.2235294, 1)
        _Lightest ("Center", Color) = (0.4431373, 0.1921569, 0.2549020, 1)
        _InnerRadius ("Inner Radius", Float) = 8
        _OuterRadius ("Outer Radius", Float) = 55
        _CenterOffset ("Center Offset", Vector) = (0, 0, 0, 0)
        _DitherScale ("Dither Pixel Size", Range(1, 8)) = 2
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Background"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off
            ZWrite On

            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Darkest;
                half4 _Dark;
                half4 _Light;
                half4 _Lightest;
                float _InnerRadius;
                float _OuterRadius;
                float4 _CenterOffset;
                float _DitherScale;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs position = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = position.positionCS;
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
                float2 centerWS = TransformObjectToWorld(float3(0, 0, 0)).xz + _CenterOffset.xy;
                float radius = distance(input.positionWS.xz, centerWS);
                float fade = smoothstep(_InnerRadius, max(_OuterRadius, _InnerRadius + 0.001), radius);
                float level = (1.0 - fade) * 3.0;

                float threshold = DitherThreshold(input.positionCS.xy);
                int index = min((int)floor(level) + (frac(level) > threshold ? 1 : 0), 3);
                half3 color = index == 0 ? _Darkest.rgb :
                              index == 1 ? _Dark.rgb :
                              index == 2 ? _Light.rgb : _Lightest.rgb;
                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
