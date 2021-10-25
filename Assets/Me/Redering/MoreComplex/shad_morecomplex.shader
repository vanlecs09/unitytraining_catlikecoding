﻿Shader "catlikecoding/more complex material"
{

    Properties
    {
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        _MainTex ("albedo", 2D) = "white" {}
        [NoScaleOffset] _NormalMap ("Normals", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1

        [NoScaleOffset] _MetallicMap ("Metallic", 2D) = "white" {}
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _DetailTex ("Detailed Texture", 2D) = "gray" {}
        [NoScaleOffset] _DetailNormalMap ("Detail Normal", 2D) = "bump" {}
        _DetailBumpScale ("Detail BumpScale", Float) = 1
        [NoScaleOffset] _EmissionMap("Emission Texture", 2D) = "black" {}
        [HDR] _Emission ("Emission Color", Color) = (0,0,0,0)
        [NoScaleOffset] _OcclusionMap("Oclusion", 2D) = "black" {}
        _OcclusionStrength("Occlusion Strength", Range(0,1)) = 1
        [NoScaleOffset] _DetailMask ("Detail Mask", 2D) = "white" {}
    }

    // CGINCLUDE

    // #define BINORMAL_PER_FRAGMENT

    // ENDCG

    SubShader
    {

        Pass
        {
            Tags
            {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM
            #pragma target 3.0
            #pragma multi_compile _ SHADOWS_SCREEN
            #pragma multi_compile _ VERTEXLIGHT_ON
            // #pragma multi_compile _ _METALLIC_MAP
            #pragma shader_feature _METALLIC_MAP
            #pragma shader_feature _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC _SMOOTHNESS_UNIFORM
            #pragma shader_feature _EMISSION_MAP
            #pragma shader_feature _OCCLUSION_MAP
            #pragma shader_feature _DETAIL_MASK
            #pragma shader_feature _NORMAP_MAP
            #pragma shader_feature _DETAIL_ALBEDO_MAP
            #pragma shader_feature _DETAIL_NORMAL_MAP
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #define FORWARD_BASE_PASS
            #include "My Lighting.cginc"
            ENDCG
        }

        Pass
        {
            Tags
            {
                "LightMode" = "ForwardAdd"
            }
            Blend One One
            ZWrite Off
            CGPROGRAM
            #pragma multi_compile_fwdadd_fullshadows
            // #pragma multi_compile _ _METALLIC_MAP
            #pragma target 3.0
            #pragma shader_feature _METALLIC_MAP
            #pragma shader_feature _SMOOTHNESS_ALBEDO _SMOOTHNESS_METALLIC _SMOOTHNESS_UNIFORM
            #pragma shader_feature _DETAIL_MASK
            #pragma shader_feature _NORMAP_MAP
            #pragma shader_feature _DETAIL_ALBEDO_MAP
            #pragma shader_feature _DETAIL_NORMAL_MAP
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram
            #include "My Lighting.cginc"
            ENDCG
        }

        Pass
        {
            Tags
            {
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

    CustomEditor "MyLightingShaderGUI"
}