Shader "catlikecoding/complex material" 
{
    
    Properties {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("albedo", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1
        _Metallic ("Metallic", Range(0, 1)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _DetailTex ("Detailed Texture", 2D) = "white" {}
        _DetailNormalMap ("Detail Normal", 2D) = "white" {}
        _DetailBumpScale ("Detail BumpScale", Float) = 1
    }

    CustomEditor "MyLightingShaderGUI"

    // CGINCLUDE

    // #define BINORMAL_PER_FRAGMENT

    // ENDCG

    SubShader 
    {

        Pass 
        {
            Tags {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            
            #pragma target 3.0
            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON
            #define FORWARD_BASE_PASS
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
            #pragma multi_compile_fwdadd_fullshadows
            #pragma target 3.0

            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #include "My Lighting.cginc"

            ENDCG
        }

        Pass {
            Tags {
                "LightMode" = "ShadowCaster"
            }

            CGPROGRAM

            #pragma target 3.0
            #pragma multi_compile_shadowcaster
            #pragma vertex MyShadowVertexProgram
            #pragma fragment MyShadowFragmentProgram

            #include "../Shadow/My Shadows.cginc"

            ENDCG
        }
    }
}