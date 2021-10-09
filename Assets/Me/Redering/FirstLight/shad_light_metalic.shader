Shader "catlikecoding/light/light metalic energy" 
{
    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
    }
    SubShader 
    {

        Pass 
        {
            Tags {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #include "UnityStandardBRDF.cginc"
            #include "UnityStandardUtils.cginc"
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            float4 _Tint;
            sampler2D _MainTex ;
            float4 _MainTex_ST;
            float _Smoothness;
            // float4 _SpecularTint;
            float _Metallic;

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Interpolators {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPosition: TEXCOORD2;
            };

            Interpolators MyVertexProgram (VertexData v)
            {
                Interpolators i;
                i.position = UnityObjectToClipPos(v.position);
                i.worldPosition = mul(unity_ObjectToWorld, v.position);
                i.normal = UnityObjectToWorldNormal(v.normal);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return i;
            } 

            float4  MyFragmentProgram (Interpolators i) :SV_TARGET
            {
                i.normal = normalize(i.normal);
                float3 lightSpecDir = _WorldSpaceLightPos0.xyz;
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

                float3 specularTint = albedo * _Metallic;
                float oneMinusReflectivity = 1 - specularTint;
                albedo *= oneMinusReflectivity;

                float3 diffuse = albedo * lightColor * DotClamped(lightDir, i.normal);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPosition);
                float3 halfVector = normalize(lightDir + viewDir);
                float3 specular = albedo * specularTint * lightColor * pow(DotClamped(halfVector, i.normal),
                _Smoothness * 100
                );
                return float4(diffuse + specular, 1);
            }
            ENDCG
        }
    }
}