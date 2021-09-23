// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "catlikecoding/light/light" 
{
    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader 
    {

        Pass 
        {
            Tags {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            // #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            float4 _Tint;
            sampler2D _MainTex, _DetailTex;
            float4 _MainTex_ST, _DetailTex_ST;

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Interpolators {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            Interpolators MyVertexProgram (VertexData v)
            {
                Interpolators i;
                i.position = UnityObjectToClipPos(v.position);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.normal = mul(
                transpose((float3x3)unity_WorldToObject),
                v.normal
                );
                i.normal = normalize(i.normal);
                return i;
            } 

            float4  MyFragmentProgram (Interpolators i) :SV_TARGET
            {
                i.normal = normalize(i.normal);
                // return dot(float3(0, 1, 0), i.normal); // fixed light above
                // return DotClamped(float3(0, 1, 0), i.normal);
                float3 lightSpecDir = _WorldSpaceLightPos0.xyz;
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
                float3 diffuse = albedo * lightColor * DotClamped(lightDir, i.normal);
                return float4(diffuse, 1);
            }
            ENDCG
        }
    }
}