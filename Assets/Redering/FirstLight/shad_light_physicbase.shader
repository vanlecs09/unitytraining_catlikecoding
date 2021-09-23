Shader "catlikecoding/light/light physicbase" 
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
            #pragma target 3.0
            // #include "UnityStandardBRDF.cginc"
            // #include "UnityStandardUtils.cginc"
            #include "UnityPBSLighting.cginc"
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
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPosition);

                float3 lightColor = _LightColor0.rgb;
                float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;

                float3 specularTint;
                float oneMinusReflectivity;
                albedo = DiffuseAndSpecularFromMetallic(
                albedo, _Metallic, specularTint, oneMinusReflectivity
                );
                
                UnityLight light;
                light.color = lightColor;
                light.dir = lightDir;
                light.ndotl = DotClamped(i.normal, lightDir);
                UnityIndirect indirectLight;
                indirectLight.diffuse = 0;
                indirectLight.specular = 0;

                return UNITY_BRDF_PBS(
                albedo, specularTint,
                oneMinusReflectivity, _Smoothness,
                i.normal, viewDir,
                light, indirectLight
                );
            }
            ENDCG
        }
    }
}