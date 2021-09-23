Shader "catlikecoding/light/light Bump map" 
{
    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("Normal", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0
        _DetailTex ("Detail Texture", 2D) = "gray" {}
        [NoScaleOffset] _DetailNormalMap ("Detail Normals", 2D) = "bump" {}
        _DetailBumpScale ("Detail Bump Scale", Float) = 1
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
            sampler2D _MainTex, _DetailTex;
            float4 _MainTex_ST, _DetailTex_ST;
            float _Smoothness;
            // float4 _SpecularTint;
            float _Metallic;
            sampler2D _NormalMap;
            float _BumpScale;
            sampler2D _DetailNormalMap;
            float _DetailBumpScale;

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };
            
            struct Interpolators {
                float4 position : SV_POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 tangent : TEXCOORD2;
                float3 worldPosition: TEXCOORD3;
                #if defined(VERTEXLIGHT_ON)
                    float3 vertexLightColor : TEXCOORD4;
                #endif
            };

            Interpolators MyVertexProgram (VertexData v)
            {
                Interpolators i;
                i.position = UnityObjectToClipPos(v.position);
                i.worldPosition = mul(unity_ObjectToWorld, v.position);
                i.normal = UnityObjectToWorldNormal(v.normal);
                i.tangent = float4(UnityObjectToWorldDir(v.tangent.xyz), v.tangent.w);
                // i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                i.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex);
                return i;
            } 

            void InitializeFragmentNormal(inout Interpolators i) {
                float3 mainNormal = UnpackScaleNormal(tex2D(_NormalMap, i.uv.xy), _BumpScale);
                float3 detailNormal = UnpackScaleNormal(tex2D(_DetailNormalMap, i.uv.zw), _DetailBumpScale);
                float3 tangentSpaceNormal = BlendNormals(mainNormal, detailNormal);
                tangentSpaceNormal = tangentSpaceNormal.xzy;
                // i.normal = BlendNormals(mainNormal, detailNormal);/
                float3 binormal = cross(i.normal, i.tangent.xyz) * (i.tangent.w * unity_WorldTransformParams.w);
                i.normal = normalize(
                tangentSpaceNormal.x * i.tangent +
                tangentSpaceNormal.y * i.normal +
                tangentSpaceNormal.z * binormal
                );
            }

            float4  MyFragmentProgram (Interpolators i) :SV_TARGET
            {
                InitializeFragmentNormal(i);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPosition);

                float3 lightColor = _LightColor0.rgb;
                float3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Tint.rgb;
                albedo *= tex2D(_DetailTex, i.uv.zw) * unity_ColorSpaceDouble;
                // albedo *= tex2D(_HeightMap, i.uv);

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