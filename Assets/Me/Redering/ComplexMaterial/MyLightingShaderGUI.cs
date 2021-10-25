using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System;

public class MyLightingShaderGUI : ShaderGUI
{
    enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    enum SmoothnessSource
    {
        Uniform,
        Albedo,
        Metallic
    }

    struct RenderingSettings
    {
        public RenderQueue Queue;
        public string RenderType;
        public BlendMode SrcBlend, DstBlend;
        public bool ZWrite;

        public static RenderingSettings[] Modes = new RenderingSettings[]
        {
            new RenderingSettings()
            {
                Queue = RenderQueue.Geometry,
                RenderType = "",
                SrcBlend = BlendMode.One,
                DstBlend = BlendMode.Zero,
                ZWrite = true
            },
            new RenderingSettings()
            {
                Queue = RenderQueue.AlphaTest,
                RenderType = "TransparentCutout",
                SrcBlend = BlendMode.One,
                DstBlend = BlendMode.Zero,
                ZWrite = true
            },
            new RenderingSettings()
            {
                Queue = RenderQueue.Transparent,
                RenderType = "Transparent",
                SrcBlend = BlendMode.SrcAlpha,
                DstBlend = BlendMode.OneMinusSrcAlpha,
                ZWrite = false
            },
            new RenderingSettings()
            {
                Queue = RenderQueue.Transparent,
                RenderType = "Transparent",
                SrcBlend = BlendMode.One,
                DstBlend = BlendMode.OneMinusSrcAlpha,
                ZWrite = false
            }
        };
    }

    private Material _target;
    private MaterialEditor _editor;
    private MaterialProperty[] _properties;
    private bool _shouldShowAlphaCutoff;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        _target = materialEditor.target as Material;
        _editor = materialEditor;
        _properties = properties;
        DoRenderingMode();
        DoMain();
        DoSecondary();
    }

    private void DoSecondary()
    {
        GUILayout.Label("Secondary Map", EditorStyles.boldLabel);

        var detailTex = FindProperty("_DetailTex");
        EditorGUI.BeginChangeCheck();
        _editor.TexturePropertySingleLine(MakeLabel(detailTex, "Albedo mutiply by 2 (RGB)"), detailTex);
        if (EditorGUI.EndChangeCheck())
            SetKeyWord("_DETAIL_ALBEDO_MAP", detailTex.textureValue);
        DoSecondaryNormals();
        _editor.TextureScaleOffsetProperty(detailTex);
    }

    void DoSecondaryNormals()
    {
        var map = FindProperty("_DetailNormalMap");
        EditorGUI.BeginChangeCheck();
        _editor.TexturePropertySingleLine(
            MakeLabel(map), map,
            map.textureValue ? FindProperty("_DetailBumpScale") : null
        );
        if (EditorGUI.EndChangeCheck())
            SetKeyWord("_DETAIL_NORMAL_MAP", map.textureValue);
    }

    private void DoMain()
    {
        GUILayout.Label("Main Maps", EditorStyles.boldLabel);
        var mainTex = FindProperty("_MainTex");
        _editor.TexturePropertySingleLine(MakeLabel(mainTex, "Albedo (RBG)"), mainTex, FindProperty("_Tint"));
        if (_shouldShowAlphaCutoff)
            DoAlphaCutOff();
        DoMetallic();
        DoSmoothNess();
        DoNormals();
        DoEmission();
        DoDetaiMask();
        DoOcclusion();
        _editor.TextureScaleOffsetProperty(mainTex);
    }

    private void DoRenderingMode()
    {
        _shouldShowAlphaCutoff = false;
        RenderingMode mode = RenderingMode.Opaque;
        if (IsKeyWordEnable("_RENDERING_CUTOUT"))
        {
            mode = RenderingMode.Cutout;
            _shouldShowAlphaCutoff = true;
        }
        else if (IsKeyWordEnable("_RENDERING_FADE"))
        {
            mode = RenderingMode.Fade;
        }
        else if (IsKeyWordEnable("_RENDERING_TRANSPARENT"))
        {
            mode = RenderingMode.Transparent;
        }
        


        EditorGUI.BeginChangeCheck();
        mode = (RenderingMode) EditorGUILayout.EnumPopup(
            MakeLabel("Rendering Mode"), mode);
        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Rendering Mode");
            SetKeyWord("_RENDERING_CUTOUT", mode == RenderingMode.Cutout);
            SetKeyWord("_RENDERING_FADE", mode == RenderingMode.Fade);
            SetKeyWord("_RENDERING_TRANSPARENT", mode == RenderingMode.Transparent);

            // RenderQueue renderQueue = mode == RenderingMode.Opaque ? RenderQueue.Geometry : RenderQueue.AlphaTest;
            // string renderType = mode == RenderingMode.Opaque ? "" : "TransparentCutout";
            RenderingSettings renderSetting = RenderingSettings.Modes[(int) mode];
            foreach (Material m in _editor.targets)
            {
                m.renderQueue = (int) renderSetting.Queue;
                m.SetOverrideTag("RenderType", renderSetting.RenderType);
                m.SetInt("_SrcBlend", (int) renderSetting.SrcBlend);
                m.SetInt("_DstBlend", (int) renderSetting.DstBlend);
                m.SetInt("_ZWrite", renderSetting.ZWrite ? 1 : 0);
            }
        }
        
        if (mode == RenderingMode.Fade || mode == RenderingMode.Transparent) {
            DoSemitransparentShadows();
        }
    }

    private void DoAlphaCutOff()
    {
        // if (!_shouldShowAlphaCutoff) return;
        var slider = FindProperty("_AlphaCutOff");
        EditorGUI.indentLevel += 2;
        _editor.ShaderProperty(slider, MakeLabel(slider));
        EditorGUI.indentLevel -= 2;
    }

    private void DoSmoothNess()
    {
        var source = SmoothnessSource.Uniform;
        if (IsKeyWordEnable("_SMOOTHNESS_ALBEDO"))
        {
            source = SmoothnessSource.Albedo;
        }
        else if (IsKeyWordEnable("_SMOOTHNESS_METALLIC"))
        {
            source = SmoothnessSource.Metallic;
        }

        var slider = FindProperty("_Smoothness");
        EditorGUI.indentLevel += 2;
        _editor.ShaderProperty(slider, MakeLabel(slider));
        EditorGUI.indentLevel += 1;
        EditorGUI.BeginChangeCheck();
        source = (SmoothnessSource) EditorGUILayout.EnumPopup(
            MakeLabel("Source"), source
        );

        if (EditorGUI.EndChangeCheck())
        {
            RecordAction("Smoothness Source");
            SetKeyWord("_SMOOTHNESS_ALBEDO", source == SmoothnessSource.Albedo);
            SetKeyWord("_SMOOTHNESS_METALLIC", source == SmoothnessSource.Metallic);
            SetKeyWord("_SMOOTHNESS_UNIFORM", source == SmoothnessSource.Uniform);
        }

        EditorGUI.indentLevel -= 3;
    }

    private void DoMetallic()
    {
        var metallicMap = FindProperty("_MetallicMap");
        EditorGUI.BeginChangeCheck();
        _editor.TexturePropertySingleLine(MakeLabel(metallicMap, "Metallic (RBG)"), metallicMap,
            metallicMap.textureValue ? null : FindProperty("_Metallic"));
        if (EditorGUI.EndChangeCheck())
            SetKeyWord("_METALLIC_MAP", metallicMap.textureValue);
    }

    static ColorPickerHDRConfig emissionConfig =
        new ColorPickerHDRConfig(0f, 99f, 1f / 99f, 3f);

    private void DoEmission()
    {
        var map = FindProperty("_EmissionMap");
        EditorGUI.BeginChangeCheck();
        _editor.TexturePropertyWithHDRColor(MakeLabel(map, "Emission (RBG)"), map,
            FindProperty("_Emission"), false);
        if (EditorGUI.EndChangeCheck())
            SetKeyWord("_EMISSION_MAP", map.textureValue);
    }

    private void DoOcclusion()
    {
        var map = FindProperty("_OcclusionMap");
        EditorGUI.BeginChangeCheck();
        _editor.TexturePropertySingleLine(MakeLabel(map, "Occlusion (G)"), map,
            map.textureValue ? FindProperty("_OcclusionStrength") : null);
        if (EditorGUI.EndChangeCheck())
            SetKeyWord("_OCCLUSION_MAP", map.textureValue);
    }

    private void DoNormals()
    {
        MaterialProperty map = FindProperty("_NormalMap");
        Texture tex = map.textureValue;
        EditorGUI.BeginChangeCheck();
        _editor.TexturePropertySingleLine(MakeLabel(map), map, tex ? FindProperty("_BumpScale") : null);
        if (EditorGUI.EndChangeCheck() && tex != map.textureValue)
            SetKeyWord("_NORMAL_MAP", map.textureValue);
    }

    private void DoDetaiMask()
    {
        MaterialProperty map = FindProperty("_DetailMask");
        EditorGUI.BeginChangeCheck();
        _editor.TexturePropertySingleLine(MakeLabel(map, "Detail Mask"), map);
        if (EditorGUI.EndChangeCheck())
            SetKeyWord("_DETAIL_MASK", map.textureValue);
    }
    
    void DoSemitransparentShadows () {
        EditorGUI.BeginChangeCheck();
        bool semitransparentShadows =
            EditorGUILayout.Toggle(
                MakeLabel( "Semitransparent Shadows"),
                IsKeyWordEnable("_SEMITRANSPARENT_SHADOWS")
            );
        if (!semitransparentShadows) {
            _shouldShowAlphaCutoff = true;
        }
        if (EditorGUI.EndChangeCheck()) {
            SetKeyWord("_SEMITRANSPARENT_SHADOWS", semitransparentShadows);
        }
    }

    private MaterialProperty FindProperty(string propertyName)
    {
        return FindProperty(propertyName, _properties);
    }

    private void SetKeyWord(string keyWord, bool state)
    {
        if (state)
        {
            foreach (Material m in _editor.targets)
            {
                m.EnableKeyword(keyWord);
            }
            // this._target.EnableKeyword(keyWord);
        }
        else
        {
            foreach (Material m in _editor.targets)
            {
                m.DisableKeyword(keyWord);
            }
            // this._target.DisableKeyword(keyWord);
        }
    }


    private bool IsKeyWordEnable(string keyWord)
    {
        return _target.IsKeywordEnabled(keyWord);
    }

    private void RecordAction(string label)
    {
        _editor.RegisterPropertyChangeUndo(label);
    }

    static GUIContent staticLabel = new GUIContent();

    private static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }

    private static GUIContent MakeLabel(string displayName)
    {
        staticLabel.text = displayName;
        return staticLabel;
    }
}