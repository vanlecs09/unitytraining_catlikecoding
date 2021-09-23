Shader "catlikecoding/light/mutil light vertex" 
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

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram

            #include "My Lighting.cginc"
            ENDCG
        }

        Pass {
			Tags {
				"LightMode" = "ForwardAdd"
			}
            Blend One One
            ZWrite Off
			CGPROGRAM
            // #pragma multi_compile DIRECTIONAL DIRECTIONAL_COOKIE POINT SPOT
            // #pragma multi_compile_fwdadd
            #pragma multi_compile _ VERTEXLIGHT_ON
			#pragma target 3.0

			#pragma vertex MyVertexProgram
			#pragma fragment MyFragmentProgram
			#include "My Lighting.cginc"

			ENDCG
		}
    }
}