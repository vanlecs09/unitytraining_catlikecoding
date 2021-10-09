Shader "catlikecoding/Flow/DistortionFlow" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [NoScaleOffset] _FlowMap ("Flow (RG, A noise)", 2D) = "black" {}
        _UJump ("U jump per phase", Range(-0.25, 0.25)) = 0.25
        _VJump ("V jump per phase", Range(-0.25, 0.25)) = 0.25
        _Tiling ("Tiling", Float) = 1
        _Speed ("Speed", Float) = 1
        _FlowStrength ("Flow Strength", Float) = 1
        _FlowOffset ("Flow Offset", Float) = 0
        // [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
        [NoScaleOffset] _DerivHeightMap ("Deriv (AG) Height (B)", 2D) = "black" {}
        _HeightScale ("Height Scale, Constant", Float) = 0.25
        _HeightScaleModulated ("Height Scale, Modulated", Float) = 0.75
        
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        #include "Flow.cginc"

        sampler2D _MainTex;
        sampler2D _FlowMap, _DerivHeightMap;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _UJump, _VJump, _Tiling, _Speed, _FlowStrength, _FlowOffset;
        float _HeightScale, _HeightScaleModulated;

        float3 UnpackDerivativeHeight (float4 textureData) {
            float3 dh = textureData.agb;
            dh.xy = dh.xy * 2 - 1;
            return dh;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            float3 flowVector = tex2D(_FlowMap, IN.uv_MainTex).rgb;
            flowVector.xy = flowVector.xy * 2 - 1;
            flowVector *= _FlowStrength;
            float noise = tex2D(_FlowMap, IN.uv_MainTex).a;
            float time = _Time.y * _Speed + noise;
            // time  = time * Speed;
            float2 jump = float2(_UJump, _VJump);

            float3 uvwA = FlowUVW(IN.uv_MainTex, flowVector.xy, jump, _FlowOffset, _Tiling, time, false);
            float3 uvwB = FlowUVW(IN.uv_MainTex, flowVector.xy, jump, _FlowOffset, _Tiling, time, true);

            fixed4 a = tex2D(_MainTex, uvwA.xy) * uvwA.z;
            float4 b = tex2D(_MainTex, uvwB.xy) * uvwB.z;

            float finalHeightScale =
            flowVector.z * _HeightScaleModulated + _HeightScale;
            float3 dhA =
            UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwA.xy)) * uvwA.z * finalHeightScale;
            float3 dhB =
            UnpackDerivativeHeight(tex2D(_DerivHeightMap, uvwB.xy)) * uvwB.z * finalHeightScale;
            o.Normal = normalize(float3(-(dhA.xy + dhB.xy), pow(dhA.z + dhB.z, 2)));
            // o.Normal = normalize(normalA + normalB);

            float4 c = (a + b) * _Color;
            // o.Albedo = pow(dhA.z + dhB.z, 2) * c.rgb;
            o.Albedo = c.rgb;
            // o.Albedo = float3(flowVector, 0);

            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}