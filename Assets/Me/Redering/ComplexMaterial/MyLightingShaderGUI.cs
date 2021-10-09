using UnityEngine;
using UnityEditor;
using System;

public class MyLightingShaderGUI : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // base.OnGUI(materialEditor, properties);
        editor = materialEditor;
        this.properties = properties;
        DoMain();
        DoSecondary();
    }

    private void DoSecondary()
    {
        // throw/ new NotImplementedException();
        GUILayout.Label("Secondary Map", EditorStyles.boldLabel);

        MaterialProperty detailTex = FindProperty("_DetailTex");
        editor.TexturePropertySingleLine(MakeLabel(detailTex, "Albedo mutiply by 2 (RGB)"), detailTex);
        DoSecondaryNormals();
        editor.TextureScaleOffsetProperty(detailTex);
    }

    void DoSecondaryNormals () {
		MaterialProperty map = FindProperty("_DetailNormalMap");
		editor.TexturePropertySingleLine(
			MakeLabel(map), map,
			map.textureValue ? FindProperty("_DetailBumpScale") : null
		);
	}

    private void DoMain()
    {
        // throw new NotImplementedException();
        GUILayout.Label("Main Maps", EditorStyles.boldLabel);
        MaterialProperty mainTex = FindProperty("_MainTex");
        // MaterialProperty tint = FindProperty("_Tint");
        // GUIContent albedoLabel = new GUIContent(mainTex.displayName, "Albedo (RBG)");
        editor.TexturePropertySingleLine(MakeLabel(mainTex, "Albedo (RBG)"), mainTex, FindProperty("_Tint"));
        DoMetalic();
        DoMoothNess();
        DoNormals();
        editor.TextureScaleOffsetProperty(mainTex);
    }

    private void DoMoothNess()
    {
        // throw new NotImplementedException();
        MaterialProperty slider = FindProperty("_Smoothness");
        EditorGUI.indentLevel += 2;
        editor.ShaderProperty(slider, MakeLabel(slider));
        EditorGUI.indentLevel -= 2;
    }

    private void DoMetalic()
    {
        MaterialProperty metallic = FindProperty("_Metallic");
        EditorGUI.indentLevel += 2;
        editor.ShaderProperty(metallic, MakeLabel(metallic));    
        EditorGUI.indentLevel -= 2;
    }

    private void DoNormals()
    {
        MaterialProperty map = FindProperty("_NormalMap");
        editor.TexturePropertySingleLine(MakeLabel(map), map, map.textureValue ? FindProperty("_BumpScale") : null);
        // throw new NotImplementedException();
    }

    MaterialProperty FindProperty(string propertyName)
    {
        return FindProperty(propertyName, properties);
    }

    static GUIContent staticLabel = new GUIContent();
    static GUIContent MakeLabel(MaterialProperty property, string tooltip = null)
    {
        staticLabel.text = property.displayName;
        staticLabel.tooltip = tooltip;
        return staticLabel;
    }
}