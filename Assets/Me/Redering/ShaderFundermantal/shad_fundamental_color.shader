// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "shader fundamental/color" {
    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
    }
    SubShader 
    {
        
        Pass 
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            float4 _Tint;
            struct Interpolators {
                float4 position : SV_POSITION;
                float3 localPosition : TEXCOORD0;
            };

            Interpolators MyVertexProgram (float4 position : POSITION)
            {
                Interpolators i;
                i.localPosition = position.xyz;
                i.position = UnityObjectToClipPos(position);
                return i;
            }

            float4  MyFragmentProgram (Interpolators i) :SV_TARGET
            {
                return float4(i.localPosition + 0.5, 1) * _Tint;
            }
            ENDCG
        }
    }
}