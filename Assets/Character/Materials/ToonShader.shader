Shader "Custom/ToonURP"
{
    Properties
    {
        _BaseMap("Texture", 2D) = "white" {}
        _Color("Color Tint", Color) = (1,1,1,1)
        _Threshold("Shadow Threshold", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST;
            float4 _Color;
            float _Threshold;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.normalWS);
                float3 lightDir = normalize(_MainLightPosition.xyz);
                float NdotL = saturate(dot(normal, lightDir));

                // Toon step function
                float bandSize = 0.15; // You can promote this to a shader property if you want
                float minEdge = saturate(_Threshold - bandSize * 0.5);
                float maxEdge = saturate(_Threshold + bandSize * 0.5);
                float lightAmount = smoothstep(minEdge, maxEdge, NdotL);



                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _Color;
                float3 finalColor = tex.rgb * _MainLightColor.rgb * lightAmount;
                float3 ambient = 0.5 * tex.rgb; // tweak this base level
                finalColor = ambient + finalColor;


                return float4(finalColor, tex.a);
            }

            ENDHLSL
        }
    }

    FallBack Off
}
