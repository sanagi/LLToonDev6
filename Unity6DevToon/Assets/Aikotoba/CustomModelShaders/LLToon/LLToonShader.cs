using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class LLToonShader : BaseShaderGUI
    {
        #region Tex

        protected MaterialProperty baseMapProp { get; set; }

        protected MaterialProperty baseColorProp { get; set; }
        
        protected MaterialProperty maskMapProp { get; set; }

        protected MaterialProperty maskMap2Prop { get; set; }
        
        protected MaterialProperty charaShadowProp { get; set; }
        
        protected MaterialProperty matCapProp { get; set; }

        public MaterialProperty bumpMapProp;
        
        #endregion

        #region Shader

        public MaterialProperty bumpScaleProp;
        protected MaterialProperty ShadowMultColorProp { get; set; }
        protected MaterialProperty SceondMaterialShadowColorProp { get; set; }
        protected MaterialProperty SceondMaterialDarkShadowColorProp { get; set; }
        protected MaterialProperty ShadowAreaProp { get; set; }
        protected MaterialProperty ShadowSmoothProp { get; set; }
        protected MaterialProperty DarkShadowAreaProp { get; set; }
        protected MaterialProperty DarkShadowMultiColorProp { get; set; }
        protected MaterialProperty DarkShadowSmoothProp { get; set; }
        protected MaterialProperty EnableDarkShadowProp { get; set; }
        protected MaterialProperty IgnoreLightYProp { get; set; }
        protected MaterialProperty FixLightYProp { get; set; }

        #endregion
        
        #region BRDF
        protected MaterialProperty MetallicProp { get; set; }
        protected MaterialProperty SmoothnessProp { get; set; }
        protected MaterialProperty EnableSpecularProp { get; set; }
        protected MaterialProperty LightSpecColorProperty { get; set; }
        protected MaterialProperty _LightSpecShadowColorProperty { get; set; }
        protected MaterialProperty EnableFaceCheekProperty { get; set; }
        protected MaterialProperty EnableMatCapProperty { get; set; }
        protected MaterialProperty MatCapIntensityProperty { get; set; }
        protected MaterialProperty EnableHairProperty { get; set; }
        protected MaterialProperty EnableCharaOnShadowProperty { get; set; }
        protected MaterialProperty SharpnessProperty { get; set; }
        protected MaterialProperty DiffuseIntensityProperty { get; set; }
        protected MaterialProperty SpecularIntensityProperty { get; set; }
        protected MaterialProperty SpecularHighIntensityProperty { get; set; }
        protected MaterialProperty SpecularIntensityShadowProperty { get; set; }
        
        protected MaterialProperty EnableInverseDarkShadowProperty { get; set; }
        
        #endregion
        
        #region Mirror
        protected MaterialProperty enableMirrorProp { get; set; }
        protected MaterialProperty reflectIntensityProp { get; set; }
        #endregion

        #region BloomEmissiveLighting

        protected MaterialProperty WorldLightInfluenceProp { get; set; }
        protected MaterialProperty GIInfluenceProp { get; set; }
        protected MaterialProperty AddLightInfluenceProp { get; set; }
        protected MaterialProperty LightMapInfluenceProp { get; set; }
        protected MaterialProperty BloomRimSpecularFactorProp { get; set; }
        protected MaterialProperty EnableEmissionProp { get; set; }
        protected MaterialProperty EmissionIntensityProp { get; set; }
        protected MaterialProperty EmissionColorProp { get; set; }
        protected MaterialProperty EmissionBloomFactorProp { get; set; }
        protected MaterialProperty DarkEmissionIntensityProp { get; set; }
        protected MaterialProperty EnableRimProp { get; set; }
        protected MaterialProperty EnableLambertRimProp { get; set; }
        protected MaterialProperty RimColorProp { get; set; }
        protected MaterialProperty RimSmoothProp { get; set; }
        protected MaterialProperty RimPowProp { get; set; }
        protected MaterialProperty EnableEdgeRimProp { get; set; }
        protected MaterialProperty EdgeRimColorProp { get; set; }
        protected MaterialProperty RimEdgeWidthProp { get; set; }
        
        protected MaterialProperty EnableDarkRimProp { get; set; }
        protected MaterialProperty DarkRimColorProp { get; set; }
        protected MaterialProperty DarkRimSmoothProp { get; set; }
        protected MaterialProperty DarkRimPowProp { get; set; }

        protected MaterialProperty enableEmissionOnlyProp { get; set; }
        
        protected MaterialProperty OutlineMaskProp { get; set; }
        protected MaterialProperty OutlineWidthProp { get; set; }
        protected MaterialProperty OutlineLightAffectsProp { get; set; }
        protected MaterialProperty OutlineSaturationProp { get; set; }
        protected MaterialProperty OutlineBrightnessProp { get; set; }
        protected MaterialProperty OutlineStrengthProp { get; set; }
        protected MaterialProperty OutlineSmoothnessProp { get; set; }

        #endregion
        
        readonly MaterialHeaderScopeList m_MaterialScopeListLL = new MaterialHeaderScopeList(uint.MaxValue & ~(uint)ExpandableLL.Advanced);

        protected class CustomStyleLL : BaseShaderGUI.Styles
        {
            public static readonly GUIContent SurfaceOptions = EditorGUIUtility.TrTextContent("Surface Options", "Controls how URP Renders the material on screen.");

            public static readonly GUIContent TexInputs = EditorGUIUtility.TrTextContent("Tex Inputs", "Base Tex, Mask Map.");

            public static readonly GUIContent ShaderInputs = EditorGUIUtility.TrTextContent("ToonShader Inputs", "LLToon Parameter.");

            public static readonly GUIContent BRDFInputs = EditorGUIUtility.TrTextContent("BRDF Inputs", "LLToonBRDF Parameter.");
            
            public static readonly GUIContent BloomEmissiveInputs = EditorGUIUtility.TrTextContent("Bloom Inputs", "BloomEmissiveInputs.");
            
            public static readonly GUIContent OutlineInputs = EditorGUIUtility.TrTextContent("Outline Inputs", "OutlineParamInputs.");
            
            public static readonly GUIContent AdvancedLabel = EditorGUIUtility.TrTextContent("Advanced Options",
                "These settings affect behind-the-scenes rendering and underlying calculations.");
            
            //Tex
            public static readonly GUIContent MaskMapOptions =
                EditorGUIUtility.TrTextContent("MaskMap", "r.lightMap g.specularMap r.emission a.secondMaterialMap.");   
            
            public static readonly GUIContent MaskMap2Options =
                EditorGUIUtility.TrTextContent("MaskMap2", "r.Rim.");   
            
            public static readonly GUIContent CharaShadowMapOptions =
                EditorGUIUtility.TrTextContent("CharaShadowMap", ".");
            
            public static readonly GUIContent MatCapOptions =
                EditorGUIUtility.TrTextContent("MatCapTex", ".");   
            
            //Shader
            public static readonly GUIContent ShadowAreaOptions =
                EditorGUIUtility.TrTextContent("ShadowArea", "");   
            
            public static readonly GUIContent ShadowSmoothOptions =
                EditorGUIUtility.TrTextContent("ShadowSmooth", "");   
            
            public static readonly GUIContent EnableDarkShadowOptions =
                EditorGUIUtility.TrTextContent("Use DarkShadow", ".");
            
            public static readonly GUIContent EnableInverseDarkShadowOptions =
                EditorGUIUtility.TrTextContent("InverseDarkShadow", ".");
            
            public static readonly GUIContent DarkShadowAreaOptions =
                EditorGUIUtility.TrTextContent("DarkShadowArea",".");   
            
            public static readonly GUIContent DarkShadowSmoothOptions =
                EditorGUIUtility.TrTextContent("DarkShadowSmooth", "");   
            
            public static readonly GUIContent IgnoreLightYOptions =
                EditorGUIUtility.TrTextContent("Use FixLightY", ".");
            
            public static readonly GUIContent FixLightYHeightOptions =
                EditorGUIUtility.TrTextContent("LightY Height", ".");
            
            public static readonly GUIContent FixDarkShadowOptions =
                EditorGUIUtility.TrTextContent("Fix Dark Shadow", ".");
            
            public static readonly GUIContent FixDivideOptions =
                EditorGUIUtility.TrTextContent("Fix Divide Value", ".");
            public static readonly GUIContent EnableFaceCheekOptions = EditorGUIUtility.TrTextContent("EnableFaceCheekShade", "");
            
            //BRDF
            public static readonly GUIContent MetallicOptions = EditorGUIUtility.TrTextContent("Metallic", "");
            public static readonly GUIContent SmoothnessOptions = EditorGUIUtility.TrTextContent("Smoothness", "");   
            public static readonly GUIContent EnableSpecularOptions = EditorGUIUtility.TrTextContent("EnableSpecular", "");
            public static readonly GUIContent DiffuseIntensityOptions = EditorGUIUtility.TrTextContent("DiffuseIntensity", "");
            public static readonly GUIContent SpecularIntensityOptions = EditorGUIUtility.TrTextContent("SpeclarIntensity", "");
            public static readonly GUIContent SpecularHighIntensityOptions = EditorGUIUtility.TrTextContent("SpeclarHighIntensity", "");
            public static readonly GUIContent SpecularIntensityShadowOptions = EditorGUIUtility.TrTextContent("SpeclarIntensityShadow", "");
            public static readonly GUIContent MatCapIntensityOptions = EditorGUIUtility.TrTextContent("MatCapIntensity", "");
            public static readonly GUIContent EnableMirrorOptions = EditorGUIUtility.TrTextContent("EnableMirror", "MirrorObject for Transparent.");
            public static readonly GUIContent EnableHairOptions = EditorGUIUtility.TrTextContent("EnableHair", ".");
            public static readonly GUIContent EnableCharaShader = EditorGUIUtility.TrTextContent("OnShadowForChara", ".");
            public static readonly GUIContent MirrorIntensityOptions = EditorGUIUtility.TrTextContent("MirrorIntensity", "Mirror Intensity.");
            public static readonly GUIContent IsBGOptions = EditorGUIUtility.TrTextContent("IsBG", "BG or Others.");
            
            //Emissive,Bloom,Lighting
            public static readonly GUIContent WorldLightInfluenceOptions = EditorGUIUtility.TrTextContent("WorldLightInfluence", "");
            public static readonly GUIContent GIInfluenceOptions = EditorGUIUtility.TrTextContent("GIInfluence", "");
            public static readonly GUIContent AddLightInfluenceOptions = EditorGUIUtility.TrTextContent("AddLightInfluence", "");  
            public static readonly GUIContent LightMapIndluenceOptions = EditorGUIUtility.TrTextContent("LightMapInfluence", "");
            public static readonly GUIContent BloomRimSpecularFactorOptions = EditorGUIUtility.TrTextContent("BloomRimSpecFactor", "");
            public static readonly GUIContent EnableEmissionOptions = EditorGUIUtility.TrTextContent("EnableEmission", "");   
            public static readonly GUIContent EmissionIntensityOptions = EditorGUIUtility.TrTextContent("EmissionIntensity", "");
            public static readonly GUIContent EmissionBloomFactorOptions = EditorGUIUtility.TrTextContent("EmissionBloomFactor", "M");
            public static readonly GUIContent DarkEmissionIntensityOptions = EditorGUIUtility.TrTextContent("DarkEmissionIntensity", "M");
            public static readonly GUIContent EnableRimOptions = EditorGUIUtility.TrTextContent("EnableRim", "");
            public static readonly GUIContent EnableDarkRimOptions = EditorGUIUtility.TrTextContent("EnableDarkRim", "");
            public static readonly GUIContent EnableEdgeRimOptions = EditorGUIUtility.TrTextContent("EnableEdgeRim", "");
            public static readonly GUIContent EnableLambertRimOptions = EditorGUIUtility.TrTextContent("EnableLambertRim", "");
            public static readonly GUIContent RimSmoothOptions = EditorGUIUtility.TrTextContent("Rim Smooth", "");
            public static readonly GUIContent RimPowOptions = EditorGUIUtility.TrTextContent("Rim Pow", "");
            public static readonly GUIContent DarkRimSmoothOptions = EditorGUIUtility.TrTextContent("Dark Rim Intensity", "");
            public static readonly GUIContent EdgeRimWidthOptions = EditorGUIUtility.TrTextContent("EdgeWidth", "");
            public static readonly GUIContent DarkRimPowOptions = EditorGUIUtility.TrTextContent("Rim Pow", "");
            public static readonly GUIContent EnableEmissionOnlyOptions = EditorGUIUtility.TrTextContent("EnableEmissionOnly", "");
            
            //Outline
            public static readonly GUIContent OutlineWidthOptions = EditorGUIUtility.TrTextContent("OutlineWidth", "");   
            public static readonly GUIContent OutlineLightAffectsOptions = EditorGUIUtility.TrTextContent("OutlineLightAffects", "");
            public static readonly GUIContent OutlineSaturationOptions = EditorGUIUtility.TrTextContent("OutlineSaturation", "");
            public static readonly GUIContent OutlineBrightnessOptions = EditorGUIUtility.TrTextContent("OutlineBrightness", "");
            public static readonly GUIContent OutlineStrengthOptions = EditorGUIUtility.TrTextContent("OutlineStrength", "M");
            public static readonly GUIContent OutlineSmoothnessptions = EditorGUIUtility.TrTextContent("OutlineSmoothness", "");
        }
        
        protected enum ExpandableLL
        {
            SurfaceOptions = 1 << 0,
            TexInputs = 1 << 1,
            ShaderInputs = 1 << 2,
            BRDFInputs = 1 << 3,
            BloomEmissiveInputs = 1 << 4,
            Outline = 1 << 5,
            Advanced = 1 << 6,
        }
        
        public void ShaderPropertiesGUILL(Material material)
        {
            m_MaterialScopeListLL.DrawHeaders(materialEditor, material);
        }
        
        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
        {            
            base.OnGUI(materialEditorIn, properties);
            ShaderPropertiesGUILL(materialEditor.target as Material);
        }
        
        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            var filter = (ExpandableLL)materialFilter;

            // Generate the foldouts
            if (filter.HasFlag(ExpandableLL.SurfaceOptions))
                m_MaterialScopeListLL.RegisterHeaderScope(CustomStyleLL.SurfaceOptions, (uint)ExpandableLL.SurfaceOptions, DrawSurfaceOptionsLL);

            if (filter.HasFlag(ExpandableLL.TexInputs))
                m_MaterialScopeListLL.RegisterHeaderScope(CustomStyleLL.TexInputs, (uint)ExpandableLL.TexInputs, DrawTexPropertiesLL);
            
            if (filter.HasFlag(ExpandableLL.ShaderInputs))
                m_MaterialScopeListLL.RegisterHeaderScope(CustomStyleLL.ShaderInputs, (uint)ExpandableLL.ShaderInputs, DrawShaderPropertiesLL);
            
            if (filter.HasFlag(ExpandableLL.BRDFInputs))
                m_MaterialScopeListLL.RegisterHeaderScope(CustomStyleLL.BRDFInputs, (uint)ExpandableLL.BRDFInputs, DrawBRDFPropertiesLL);
            
            if (filter.HasFlag(ExpandableLL.BloomEmissiveInputs))
                m_MaterialScopeListLL.RegisterHeaderScope(CustomStyleLL.BloomEmissiveInputs, (uint)ExpandableLL.BloomEmissiveInputs, DrawBloomEmissivePropertiesLL);
            
            if (filter.HasFlag(ExpandableLL.Outline))
                m_MaterialScopeListLL.RegisterHeaderScope(CustomStyleLL.OutlineInputs, (uint)ExpandableLL.Outline, DrawOutlinePropertiesLL);
            
            if (filter.HasFlag(ExpandableLL.Advanced))
                m_MaterialScopeListLL.RegisterHeaderScope(CustomStyleLL.AdvancedLabel, (uint)ExpandableLL.Advanced, DrawAdvancedOptions);
        }

        static void DrawFloatToggleProperty(GUIContent styles, MaterialProperty prop)
        {
            if (prop == null)
                return;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            bool newValue = EditorGUILayout.Toggle(styles, prop.floatValue == 1.0f);
            if (EditorGUI.EndChangeCheck())
                prop.floatValue = newValue ? 1.0f : 0.0f;
            EditorGUI.showMixedValue = false;
        }

        /// <summary>
        /// Floatの基本
        /// </summary>
        /// <param name="styles"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="prop"></param>
        static void DrawFloatSliderValue(GUIContent styles, float min, float max, MaterialProperty prop)
        {            
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            prop.floatValue= EditorGUILayout.Slider(styles, prop.floatValue, min, max);
            EditorGUI.EndChangeCheck();
        }
        
        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            
        }

        // collect properties from the material properties
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            
            baseMapProp = FindProperty("_BaseMap", properties, false);
            baseColorProp = FindProperty("_BaseColor", properties, false);
            
            bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
            bumpScaleProp = BaseShaderGUI.FindProperty("_BumpScale", properties, false);

            maskMapProp = FindProperty("_MaskMap", properties, false);
            maskMap2Prop = FindProperty("_MaskMap2", properties, false);
            charaShadowProp = FindProperty("_CharaShadowMaskMap", properties, false);
            matCapProp = FindProperty("_MatCap", properties, false);
            
            ShadowMultColorProp = FindProperty("_ShadowMultColor", properties, false);
            SceondMaterialShadowColorProp = FindProperty("_SceondMaterialShadowColor", properties, false);
            SceondMaterialDarkShadowColorProp = FindProperty("_SceondMaterialDarkShadowColor", properties, false);
            ShadowAreaProp = FindProperty("_ShadowArea", properties, false);
            ShadowSmoothProp = FindProperty("_ShadowSmooth", properties, false);
            DarkShadowMultiColorProp = FindProperty("_DarkShadowMultColor", properties, false);
            DarkShadowAreaProp = FindProperty("_DarkShadowArea", properties, false);
            DarkShadowSmoothProp = FindProperty("_DarkShadowSmooth", properties, false);
            EnableDarkShadowProp = FindProperty("_EnableDarkShadow", properties, false);
            IgnoreLightYProp = FindProperty("_IgnoreLightY", properties, false);
            FixLightYProp = FindProperty("_FixLightY", properties, false);

            MetallicProp = FindProperty("_Metallic", properties, false);
            SmoothnessProp = FindProperty("_Smoothness", properties, false);
            EnableSpecularProp = FindProperty("_EnableSpecular", properties, false);
            LightSpecColorProperty = FindProperty("_LightSpecColor", properties, false);
            _LightSpecShadowColorProperty = FindProperty("_LightSpecShadowColor", properties, false);
            EnableFaceCheekProperty = FindProperty("_EnableFaceCheek", properties, false);
            EnableMatCapProperty = FindProperty("_EnableMatCapSpecular", properties, false);
            EnableCharaOnShadowProperty = FindProperty("_EnableCharaOnShadow", properties, false);
            EnableHairProperty = FindProperty("_EnableHairSpecular", properties, false);
            EnableInverseDarkShadowProperty = FindProperty("_EnableDarkInverseShadow", properties, false);
            MatCapIntensityProperty = FindProperty("_MatCapIntensity", properties, false);
            SharpnessProperty = FindProperty("_Sharpness", properties, false);
            DiffuseIntensityProperty = FindProperty("_DiffuseIntensity", properties, false);
            SpecularIntensityProperty = FindProperty("_SpecularIntensity", properties, false);
            SpecularHighIntensityProperty = FindProperty("_SpecularIntensityHigh", properties, false);
            SpecularIntensityShadowProperty = FindProperty("_SpecularIntensityShadow", properties, false);
            enableMirrorProp = FindProperty("_EnableMirror", properties, false);
            reflectIntensityProp = FindProperty("_ReflectIntensity", properties, false);
        
            WorldLightInfluenceProp = FindProperty("_WorldLightInfluence", properties, false);
            GIInfluenceProp = FindProperty("_GIInfluence", properties, false);
            LightMapInfluenceProp = FindProperty("_LightMapInfluence", properties, false);
            BloomRimSpecularFactorProp = FindProperty("_BloomFactor", properties, false);
            EnableEmissionProp = FindProperty("_EnableEmission", properties, false);
            EmissionIntensityProp = FindProperty("_Emission", properties, false);
            EmissionColorProp = FindProperty("_EmissionColor", properties, false);
            EmissionBloomFactorProp = FindProperty("_EmissionBloomFactor", properties, false);
            DarkEmissionIntensityProp = FindProperty("_DarkEmissionIntensity", properties, false);
            EnableRimProp = FindProperty("_EnableRim", properties, false);
            EnableLambertRimProp = FindProperty("_EnableLambert", properties, false);
            RimColorProp = FindProperty("_RimColor", properties, false);
            RimSmoothProp = FindProperty("_RimSmooth", properties, false);
            RimPowProp = FindProperty("_RimPow", properties, false);
            EnableDarkRimProp = FindProperty("_EnableRimDS", properties, false);
            DarkRimColorProp = FindProperty("_DarkSideRimColor", properties, false);
            DarkRimSmoothProp = FindProperty("_DarkSideRimSmooth", properties, false);
            DarkRimPowProp = FindProperty("_DarkSideRimPow", properties, false);
            EnableEdgeRimProp = FindProperty("_EnableEdgeRim", properties, false);
            EdgeRimColorProp = FindProperty("_EdgeRimColor", properties, false);
            RimEdgeWidthProp = FindProperty("_EdgeRimWidth", properties, false);
            
            enableEmissionOnlyProp = FindProperty("_EnableEmissionOnly", properties, false);
            AddLightInfluenceProp = FindProperty("_AddLightIntensity", properties, false);
            
            OutlineMaskProp = FindProperty("_OutlineMask", properties, false);
            OutlineWidthProp = FindProperty("_OutlineWidth", properties, false);
            OutlineLightAffectsProp = FindProperty("_OutlineLightAffects", properties, false);
            OutlineSaturationProp = FindProperty("_OutlineSaturation", properties, false);
            OutlineBrightnessProp = FindProperty("_OutlineBrightness", properties, false);
            OutlineStrengthProp = FindProperty("_OutlineStrength", properties, false);
            OutlineSmoothnessProp = FindProperty("_OutlineSmoothness", properties, false);
        }

        // material changed check
        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, LLToonDetailGUI.SetMaterialKeywords);
            if (material.IsKeywordEnabled("ENABLE_MIRROR"))
            {
                //Blendしない設定にする
                SetMaterialSrcDstBlendPropertiesLL(material, UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.Zero);
            }
            // Normal Map
            if (material.HasProperty("_BumpMap"))
                CoreUtils.SetKeyword(material, ShaderKeywordStrings._NORMALMAP, material.GetTexture("_BumpMap"));
        }
        
#region Surface
        public void DrawSurfaceOptionsLL(Material material)
        {
            DoPopup(Styles.surfaceType, surfaceTypeProp, Styles.surfaceTypeNames);
            if ((surfaceTypeProp != null) && ((SurfaceType)surfaceTypeProp.floatValue == SurfaceType.Transparent))
                DoPopup(Styles.blendingMode, blendModeProp, Styles.blendModeNames);
            
            DoPopup(Styles.cullingText, cullingProp, Styles.renderFaceNames);
            DoPopup(Styles.zwriteText, zwriteProp, Styles.zwriteNames);

            if (ztestProp != null)
                materialEditor.IntPopupShaderProperty(ztestProp, Styles.ztestText.text, Styles.ztestNames, Styles.ztestValues);

            DrawFloatToggleProperty(Styles.alphaClipText, alphaClipProp);

            if ((alphaClipProp != null) && (alphaCutoffProp != null) && (alphaClipProp.floatValue == 1))
                materialEditor.ShaderProperty(alphaCutoffProp, Styles.alphaClipThresholdText, 1);

            DrawFloatToggleProperty(Styles.castShadowText, castShadowsProp);
            DrawFloatToggleProperty(Styles.receiveShadowText, receiveShadowsProp);
        }
        
        /// <summary>
        /// テクスチャ
        /// </summary>
        /// <param name="material"></param>
        public void DrawTexPropertiesLL(Material material)
        {
            if (baseMapProp != null && baseColorProp != null) // Draw the baseMap, most shader will have at least a baseMap
            {
                materialEditor.TexturePropertySingleLine(Styles.baseMap, baseMapProp, baseColorProp);
            }
            
            if (maskMapProp != null)
            {
                materialEditor.TexturePropertySingleLine(CustomStyleLL.MaskMapOptions, maskMapProp);
            }
            
            if (maskMap2Prop != null)
            {
                materialEditor.TexturePropertySingleLine(CustomStyleLL.MaskMap2Options, maskMap2Prop);
            }
            
            if (charaShadowProp != null)
            {
                materialEditor.TexturePropertySingleLine(CustomStyleLL.CharaShadowMapOptions, charaShadowProp);
            }
            
            if (EnableMatCapProperty != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.MatCapOptions, EnableMatCapProperty);
                bool enableMatCapSpecular = EnableMatCapProperty.floatValue == 1.0f;
                if (enableMatCapSpecular)
                {
                    materialEditor.TexturePropertySingleLine(CustomStyleLL.MatCapOptions, matCapProp);
                    DrawFloatSliderValue(CustomStyleLL.MatCapIntensityOptions, 0, 2, MatCapIntensityProperty);
                }
                CoreUtils.SetKeyword(material, "ENABLE_MATCAP_SPECULAR", enableMatCapSpecular);
            }
            if (EnableFaceCheekProperty != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.EnableFaceCheekOptions, EnableFaceCheekProperty);
                bool enableFaceCheekShade = EnableFaceCheekProperty.floatValue == 1.0f;
                CoreUtils.SetKeyword(material, "ENABLE_FACE_CHEEK", enableFaceCheekShade);
            }
            DrawNormalArea(materialEditor, bumpMapProp, bumpScaleProp);
        }

        /// <summary>
        /// シェーダー独自
        /// </summary>
        /// <param name="material"></param>
        public void DrawShaderPropertiesLL(Material material)
        {
            if (ShadowMultColorProp != null)
            {
                materialEditor.ColorProperty(ShadowMultColorProp, "ShadowMultColor");
            }
            
            if (SceondMaterialShadowColorProp != null)
            {
                materialEditor.ColorProperty(SceondMaterialShadowColorProp, "SceondMaterialShadowColor");
            }
            if (SceondMaterialDarkShadowColorProp != null)
            {
                materialEditor.ColorProperty(SceondMaterialDarkShadowColorProp, "SceondMaterialDarkShadowColor");
            }
            //var val = EditorGUILayout.Slider("Shadow Area", ShadowAreaProp.floatValue, 0, 1);
            if (ShadowAreaProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.ShadowAreaOptions, 0, 1, ShadowAreaProp);
            }
            if (ShadowSmoothProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.ShadowSmoothOptions, 0, 1, ShadowSmoothProp);
            }

            if (EnableDarkShadowProp != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.EnableDarkShadowOptions, EnableDarkShadowProp);
                bool enableUseDarkShadow = EnableDarkShadowProp.floatValue == 1.0f;
                if (enableUseDarkShadow && DarkShadowMultiColorProp != null && DarkShadowAreaProp != null && DarkShadowSmoothProp != null)
                {
                    materialEditor.ColorProperty(DarkShadowMultiColorProp, "DarkShadowMultColor");
                    DrawFloatSliderValue(CustomStyleLL.DarkShadowAreaOptions, 0, 1, DarkShadowAreaProp);
                    DrawFloatSliderValue(CustomStyleLL.DarkShadowSmoothOptions, 0, 1, DarkShadowSmoothProp);
                }

                if (EnableInverseDarkShadowProperty != null && enableUseDarkShadow)
                {
                    DrawFloatToggleProperty(CustomStyleLL.EnableInverseDarkShadowOptions, EnableInverseDarkShadowProperty);
                    bool enableUseInverseDarkShadow = EnableInverseDarkShadowProperty.floatValue == 1.0f;
                    CoreUtils.SetKeyword(material, "ENABLE_INVERSE_SHADOW", enableUseInverseDarkShadow);
                }
            }

            if (IgnoreLightYProp != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.IgnoreLightYOptions, IgnoreLightYProp);
                bool ignoreLightY = IgnoreLightYProp.floatValue == 1.0f;
                if (ignoreLightY && FixLightYProp != null)
                {
                    DrawFloatSliderValue(CustomStyleLL.FixLightYHeightOptions, -10, 10, FixLightYProp);
                } 
            }
            if (EnableCharaOnShadowProperty != null )
            {
                DrawFloatToggleProperty(CustomStyleLL.EnableCharaShader, EnableCharaOnShadowProperty);
                CoreUtils.SetKeyword(material, "ENABLE_CHARA_ON_SHADOW", EnableCharaOnShadowProperty.floatValue == 1.0f);
            }
        }

        /// <summary>
        /// BRDF
        /// </summary>
        /// <param name="material"></param>
        public void DrawBRDFPropertiesLL(Material material)
        {
            if (MetallicProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.MetallicOptions, 0, 1, MetallicProp);
            }

            if (SmoothnessProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.SmoothnessOptions, 0, 1, SmoothnessProp);
            }

            if (EnableSpecularProp != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.EnableSpecularOptions, EnableSpecularProp);
                bool enableSpecular = EnableSpecularProp.floatValue == 1.0f;
                if (enableSpecular && LightSpecColorProperty != null)
                {
                    materialEditor.ColorProperty(LightSpecColorProperty, "LightSpecColor");
                }
                if (_LightSpecShadowColorProperty != null)
                {
                    materialEditor.ColorProperty(_LightSpecShadowColorProperty, "ShadowHighlightColor");
                }
            }
            if (EnableHairProperty != null && SharpnessProperty != null && SpecularIntensityProperty != null && SpecularHighIntensityProperty != null && SpecularIntensityShadowProperty != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.EnableHairOptions, EnableHairProperty);
                bool enableHairsSpecular = EnableHairProperty.floatValue == 1.0f;
                if (enableHairsSpecular)
                {
                    materialEditor.FloatProperty(SharpnessProperty, "Sharpness");
                    DrawFloatSliderValue(CustomStyleLL.SpecularIntensityOptions, 0, 2, SpecularIntensityProperty);
                    DrawFloatSliderValue(CustomStyleLL.SpecularHighIntensityOptions, 0, 4, SpecularHighIntensityProperty);
                    DrawFloatSliderValue(CustomStyleLL.SpecularIntensityShadowOptions, 0, 2, SpecularIntensityShadowProperty);
                }
                CoreUtils.SetKeyword(material, "ENABLE_HAIR_SPECULAR", enableHairsSpecular);
            }
            if(enableMirrorProp != null){
                DrawMirrorProperty(material);
            }
        }

        private void DrawMirrorProperty(Material material)
        {
            DrawFloatToggleProperty(CustomStyleLL.EnableMirrorOptions, enableMirrorProp);
            bool enableMirror = false;
            if (enableMirrorProp != null)
            {
                enableMirror = enableMirrorProp.floatValue == 1.0f;
            }
            CoreUtils.SetKeyword(material, "ENABLE_MIRROR", enableMirror);

            if (enableMirror)
            {
                if (reflectIntensityProp != null)
                {
                    DrawFloatSliderValue(CustomStyleLL.MirrorIntensityOptions, 0, 2, reflectIntensityProp);
                }

                //Blendしない設定にする
                SetMaterialSrcDstBlendPropertiesLL(material, UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.Zero);
            }
        }
        
        internal static void SetMaterialSrcDstBlendPropertiesLL(Material material, UnityEngine.Rendering.BlendMode srcBlend, UnityEngine.Rendering.BlendMode dstBlend)
        {
            if (material.HasProperty("_SrcBlend"))
                material.SetFloat("_SrcBlend", (float)srcBlend);

            if (material.HasProperty("_DstBlend"))
                material.SetFloat("_DstBlend", (float)dstBlend);
        }
#endregion

#region ライティング
        protected void DrawBloomEmissivePropertiesLL(Material material)
        {
            if (DiffuseIntensityProperty != null)
            {
                DrawFloatSliderValue(CustomStyleLL.DiffuseIntensityOptions, 0, 10, DiffuseIntensityProperty);
            }

            if (SpecularIntensityProperty != null && EnableHairProperty.floatValue != 1.0f)
            {
                DrawFloatSliderValue(CustomStyleLL.SpecularIntensityOptions, 0, 10, SpecularIntensityProperty);
            }
            
            if (SpecularHighIntensityProperty != null && EnableHairProperty.floatValue != 1.0f)
            {
                DrawFloatSliderValue(CustomStyleLL.SpecularHighIntensityOptions, 0, 20, SpecularHighIntensityProperty);
            }

            if (WorldLightInfluenceProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.WorldLightInfluenceOptions, 0, 1, WorldLightInfluenceProp);
            }

            if (GIInfluenceProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.GIInfluenceOptions, 0, 10, GIInfluenceProp);
            }
            
            if (AddLightInfluenceProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.AddLightInfluenceOptions, 0, 1, AddLightInfluenceProp);
            }
            
            if (LightMapInfluenceProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.LightMapIndluenceOptions, 0, 30, LightMapInfluenceProp);
            }
            
            if (BloomRimSpecularFactorProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.BloomRimSpecularFactorOptions, 0, 1, BloomRimSpecularFactorProp);
            }

            if (EnableEmissionProp != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.EnableEmissionOptions, EnableEmissionProp);
                bool enableEmission = EnableEmissionProp.floatValue == 1.0f;
                if (enableEmission && EmissionIntensityProp != null && EmissionColorProp != null && EmissionBloomFactorProp != null && DarkEmissionIntensityProp != null)
                {
                    materialEditor.ColorProperty(EmissionColorProp, "EmissionColor");
                    DrawFloatSliderValue(CustomStyleLL.EmissionIntensityOptions, 0f, 20f, EmissionIntensityProp);
                    DrawFloatSliderValue(CustomStyleLL.EmissionBloomFactorOptions, 0, 10, EmissionBloomFactorProp);
                    DrawFloatSliderValue(CustomStyleLL.DarkEmissionIntensityOptions, 0, 10, DarkEmissionIntensityProp);
                }
                DrawFloatToggleProperty(CustomStyleLL.EnableEmissionOnlyOptions, enableEmissionOnlyProp);
                bool enableEmissionOnly = false;
                if (enableEmissionOnlyProp != null)
                {
                    enableEmissionOnly = enableEmissionOnlyProp.floatValue == 1.0f;
                }
                CoreUtils.SetKeyword(material, "ENABLE_EMISSION_ONLY", enableEmissionOnly);
            }
            
            if (EnableRimProp != null)
            {
                DrawFloatToggleProperty(CustomStyleLL.EnableRimOptions, EnableRimProp);
                bool enableRim = EnableRimProp.floatValue == 1.0f;
                if (enableRim && EnableLambertRimProp != null && RimColorProp != null && RimSmoothProp != null && RimPowProp != null)
                {
                    materialEditor.ColorProperty(RimColorProp, "RimColor");
                    DrawFloatToggleProperty(CustomStyleLL.EnableLambertRimOptions, EnableLambertRimProp);
                    DrawFloatSliderValue(CustomStyleLL.RimSmoothOptions, 0f, 10f, RimSmoothProp);
                    DrawFloatSliderValue(CustomStyleLL.RimPowOptions, 0, 10, RimPowProp);
                }
                if (EnableDarkRimProp != null)
                {
                    DrawFloatToggleProperty(CustomStyleLL.EnableDarkRimOptions, EnableDarkRimProp);
                    bool enableDarkRim = EnableDarkRimProp.floatValue == 1.0f;
                    if (enableDarkRim && DarkRimColorProp != null && DarkRimPowProp != null && DarkRimSmoothProp != null)
                    {
                        materialEditor.ColorProperty(DarkRimColorProp, "DarkRimColor");
                        DrawFloatSliderValue(CustomStyleLL.DarkRimSmoothOptions, 0f, 10f, DarkRimSmoothProp);
                        DrawFloatSliderValue(CustomStyleLL.RimPowOptions, 0, 10, DarkRimPowProp);
                    }
                }
                if (EnableEdgeRimProp != null)
                {
                    DrawFloatToggleProperty(CustomStyleLL.EnableEdgeRimOptions, EnableEdgeRimProp);
                    bool enableEdgeRim = EnableEdgeRimProp.floatValue == 1.0f;
                    if (enableEdgeRim && EdgeRimColorProp != null && EdgeRimColorProp != null && RimEdgeWidthProp != null)
                    {
                        materialEditor.ColorProperty(EdgeRimColorProp, "EdgeRimColor");
                        DrawFloatSliderValue(CustomStyleLL.EdgeRimWidthOptions, 0f, 10f, RimEdgeWidthProp);
                    }
                    CoreUtils.SetKeyword(material, "ENABLE_EDGE_RIM", enableEdgeRim);
                }                
            }
        }
#endregion

#region Outline

        protected void DrawOutlinePropertiesLL(Material material)
        {
            if (OutlineMaskProp != null)
            {
                materialEditor.TextureProperty(OutlineMaskProp, "OutlineMask");
            }

            if (OutlineWidthProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.OutlineWidthOptions, 0, 50, OutlineWidthProp);
            }
            
            if (OutlineLightAffectsProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.OutlineLightAffectsOptions, 0, 50, OutlineLightAffectsProp);
            }
            
            if (OutlineSaturationProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.OutlineSaturationOptions, 0, 4, OutlineSaturationProp);
            }
            
            if (OutlineBrightnessProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.OutlineBrightnessOptions, 0, 1, OutlineBrightnessProp);
            }
            
            if (OutlineStrengthProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.OutlineStrengthOptions, 0, 1, OutlineStrengthProp);
            }
            
            if (OutlineSmoothnessProp != null)
            {
                DrawFloatSliderValue(CustomStyleLL.OutlineSmoothnessptions, 0, 1, OutlineSmoothnessProp);
            }
        }
        #endregion

// material main advanced options
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            
            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha; 
            if (newShader.name.Contains("Transparent"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Blend", (float)blendMode);

            material.SetFloat("_Surface", (float)surfaceType);
            if (surfaceType == SurfaceType.Opaque)
            {
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            
            if (enableMirrorProp.floatValue == 1.0f)
            {
                //Blendしない設定にする
                SetMaterialSrcDstBlendPropertiesLL(material, UnityEngine.Rendering.BlendMode.One, UnityEngine.Rendering.BlendMode.Zero);
            }

            /*
            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            /*if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            /*SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }
            material.SetFloat("_Blend", (float)blendMode);

            material.SetFloat("_Surface", (float)surfaceType);
            if (surfaceType == SurfaceType.Opaque)
            {
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
            else
            {
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }*/
        }
    }
}
