#ifndef UNIVERSAL_LIT_INPUT_INCLUDED
#define UNIVERSAL_LIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
#define _DETAIL
#endif

// NOTE: Lit系のInputをベースに
CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
//float4 _DetailAlbedoMap_ST;
half4 _BaseColor;
//half4 _SpecColor;
half4 _EmissionColor;
float _EnableAlphaClipping;
half _Cutoff;
half _Smoothness;
half _Metallic;
half _BumpScale;
//half _Parallax;
//half _OcclusionStrength;
half _ClearCoatMask;
half _ClearCoatSmoothness;
//half _DetailAlbedoMapScale;
//half _DetailNormalMapScale;
half _Surface;
sampler2D _CameraReflectionTexture;
int _EnableMirror;
float _ReflectIntensity;

//Toon
half _WorldLightInfluence;
half _LightMapInfluence;
half _GIInfluence;
float _BloomFactor;
float _EnableEmission;
float _Emission;
float _EmissionBloomFactor;
half3 _EmissionMapChannelMask;
/*
float4 _FaceShadowMap_ST;
float _FaceShadowMapPow;
float _FaceShadowOffset;
*/
float3 _ShadowMultColor;
half4 _SceondMaterialShadowColor;
half4 _SceondMaterialDarkShadowColor;
float _ShadowArea;
half _ShadowSmooth;
float3 _DarkShadowMultColor;
float _DarkShadowArea;
half _DarkShadowSmooth;
float _EnableDarkShadow;
float _EnableInverseDarkShadow;
float3 _BOXCenter;
float3 _BOXSize;

float _IgnoreLightY;
float _FixLightY;

int _ReceiveShadows;

//half4 _RampArea12;
//half4 _RampArea34;
//half2 _RampArea5;
//float _RampShadowRange;
float _EnableSpecular;
float4 _LightSpecColor;
float4 _LightSpecShadowColor;
float _Shininess;
//float _SpecMulti;
float _EnableMatCapSpecular;
float _EnableFaceCheek;
float _EnableHairSpecular;
float _Sharpness;
float _DiffuseIntensity;
float _SpecularIntensity;
float _SpecularIntensityHigh;
float _SpecularIntensityShadow;
float _MatCapIntensity;
half _AddLightIntensity;

float _EnableLambert;
float _EnableRim;
half4 _RimColor;
float _RimSmooth;
float _RimPow;
float _EnableRimDS;
half4 _DarkSideRimColor;
float _DarkSideRimSmooth;
float _DarkSideRimPow;
float _DarkEmissionIntensity;

float _OutlineWidth;
float _OutlineLightAffects;
//half4 _OutlineColor;
float _OutlineSaturation;
float _OutlineBrightness;
float _OutlineStrength;
float _OutlineSmoothness;
//float3 _ReceiveShadowMultColor;

//half4 _MedColor;
//half4 _LowColor;

//Mirror
//float _Contrast;

/*
//Macheiyora
int _InvertGrad;
float _BlendFactor;
float _GradPower;
float _GradShift;
*/

CBUFFER_END

// NOTE: Do not ifdef the properties for dots instancing, but ifdef the actual usage.
// Otherwise you might break CPU-side as property constant-buffer offsets change per variant.
// NOTE: Dots instancing is orthogonal to the constant buffer above.
#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float4, _BaseColor)
    UNITY_DOTS_INSTANCED_PROP(float4, _SpecColor)
    UNITY_DOTS_INSTANCED_PROP(float4, _EmissionColor)
    UNITY_DOTS_INSTANCED_PROP(float , _Cutoff)
    UNITY_DOTS_INSTANCED_PROP(float , _Smoothness)
    UNITY_DOTS_INSTANCED_PROP(float , _Metallic)
    UNITY_DOTS_INSTANCED_PROP(float , _BumpScale)
    UNITY_DOTS_INSTANCED_PROP(float , _Parallax)
    UNITY_DOTS_INSTANCED_PROP(float , _OcclusionStrength)
    UNITY_DOTS_INSTANCED_PROP(float , _ClearCoatMask)
    UNITY_DOTS_INSTANCED_PROP(float , _ClearCoatSmoothness)
    UNITY_DOTS_INSTANCED_PROP(float , _DetailAlbedoMapScale)
    UNITY_DOTS_INSTANCED_PROP(float , _DetailNormalMapScale)
    UNITY_DOTS_INSTANCED_PROP(float , _Surface)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _BaseColor              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_BaseColor)
#define _SpecColor              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_SpecColor)
#define _EmissionColor          UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata_EmissionColor)
#define _Cutoff                 UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Cutoff)
#define _Smoothness             UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Smoothness)
#define _Metallic               UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Metallic)
#define _BumpScale              UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_BumpScale)
#define _Parallax               UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Parallax)
#define _OcclusionStrength      UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_OcclusionStrength)
#define _ClearCoatMask          UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_ClearCoatMask)
#define _ClearCoatSmoothness    UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_ClearCoatSmoothness)
#define _DetailAlbedoMapScale   UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_DetailAlbedoMapScale)
#define _DetailNormalMapScale   UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_DetailNormalMapScale)
#define _Surface                UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float  , Metadata_Surface)
#endif

TEXTURE2D(_ParallaxMap);        SAMPLER(sampler_ParallaxMap);
TEXTURE2D(_OcclusionMap);       SAMPLER(sampler_OcclusionMap);
TEXTURE2D(_DetailMask);         SAMPLER(sampler_DetailMask);
TEXTURE2D(_DetailAlbedoMap);    SAMPLER(sampler_DetailAlbedoMap);
TEXTURE2D(_DetailNormalMap);    SAMPLER(sampler_DetailNormalMap);
TEXTURE2D(_MetallicGlossMap);   SAMPLER(sampler_MetallicGlossMap);
TEXTURE2D(_SpecGlossMap);       SAMPLER(sampler_SpecGlossMap);
TEXTURE2D(_ClearCoatMap);       SAMPLER(sampler_ClearCoatMap);
TEXTURE2D(_GradMap);            SAMPLER(sampler_GradMap);

//Genshin Add
TEXTURE2D(_FaceShadowMap);  SAMPLER(sampler_FaceShadowMap);
TEXTURE2D(_RampMap);        SAMPLER(sampler_RampMap);
TEXTURE2D(_MetalMap);       SAMPLER(sampler_MetalMap);
TEXTURE2D(_OutlineMask);    SAMPLER(sampler_OutlineMask);
TEXTURE2D(_MaskMap);    SAMPLER(sampler_MaskMap);
TEXTURE2D(_MaskMap2);    SAMPLER(sampler_MaskMap2);
TEXTURE2D(_CharaShadowMaskMap);  SAMPLER(sampler_CharaShadowMap);
TEXTURE2D(_MatCap);  SAMPLER(sampler_MatCap);
//TEXTURE2D(_JitterMap);              SAMPLER(sampler_JitterMap);

#define SAMPLE_METALLICSPECULAR(uv) SAMPLE_TEXTURE2D(_MetallicGlossMap, sampler_MetallicGlossMap, uv).g

half SampleOcclusion(float2 uv)
{
    #ifdef _OCCLUSIONMAP
        // TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
        #if defined(SHADER_API_GLES)
            return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
        #else
            half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
            return LerpWhiteTo(occ, _OcclusionStrength);
        #endif
    #else
        return half(1.0);
    #endif
}


// Returns clear coat parameters
// .x/.r == mask
// .y/.g == smoothness
half2 SampleClearCoat(float2 uv)
{
#if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
    half2 clearCoatMaskSmoothness = half2(_ClearCoatMask, _ClearCoatSmoothness);

#if defined(_CLEARCOATMAP)
    clearCoatMaskSmoothness *= SAMPLE_TEXTURE2D(_ClearCoatMap, sampler_ClearCoatMap, uv).rg;
#endif

    return clearCoatMaskSmoothness;
#else
    return half2(0.0, 1.0);
#endif  // _CLEARCOAT
}

void ApplyPerPixelDisplacement(half3 viewDirTS, inout float2 uv)
{
#if defined(_PARALLAXMAP)
    uv += ParallaxMapping(TEXTURE2D_ARGS(_ParallaxMap, sampler_ParallaxMap), viewDirTS, _Parallax, uv);
#endif
}

// Used for scaling detail albedo. Main features:
// - Depending if detailAlbedo brightens or darkens, scale magnifies effect.
// - No effect is applied if detailAlbedo is 0.5.
half3 ScaleDetailAlbedo(half3 detailAlbedo, half scale)
{
    // detailAlbedo = detailAlbedo * 2.0h - 1.0h;
    // detailAlbedo *= _DetailAlbedoMapScale;
    // detailAlbedo = detailAlbedo * 0.5h + 0.5h;
    // return detailAlbedo * 2.0f;

    // A bit more optimized
    return half(2.0) * detailAlbedo * scale - scale + half(1.0);
}

half3 ApplyDetailAlbedo(float2 detailUv, half3 albedo, half detailMask)
{
#if defined(_DETAIL)
    half3 detailAlbedo = SAMPLE_TEXTURE2D(_DetailAlbedoMap, sampler_DetailAlbedoMap, detailUv).rgb;

    // In order to have same performance as builtin, we do scaling only if scale is not 1.0 (Scaled version has 6 additional instructions)
#if defined(_DETAIL_SCALED)
    detailAlbedo = ScaleDetailAlbedo(detailAlbedo, _DetailAlbedoMapScale);
#else
    detailAlbedo = half(2.0) * detailAlbedo;
#endif

    return albedo * LerpWhiteTo(detailAlbedo, detailMask);
#else
    return albedo;
#endif
}

half3 ApplyDetailNormal(float2 detailUv, half3 normalTS, half detailMask)
{
#if defined(_DETAIL)
#if BUMP_SCALE_NOT_SUPPORTED
    half3 detailNormalTS = UnpackNormal(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, detailUv));
#else
    half3 detailNormalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_DetailNormalMap, sampler_DetailNormalMap, detailUv), _DetailNormalMapScale);
#endif

    // With UNITY_NO_DXT5nm unpacked vector is not normalized for BlendNormalRNM
    // For visual consistancy we going to do in all cases
    detailNormalTS = normalize(detailNormalTS);

    return lerp(normalTS, BlendNormalRNM(normalTS, detailNormalTS), detailMask); // todo: detailMask should lerp the angle of the quaternion rotation, not the normals
#else
    return normalTS;
#endif
}

SamplerState my_linear_clamp_sampler;
//デプス取得
float sampleSceneDepth(float2 uv)
{
    float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, uv);
    return Linear01Depth(sceneDepth, _ZBufferParams) * _ProjectionParams.z;
}

inline void InitializeStandardSurfaceDataLL(float2 uv, out SurfaceData outSurfaceData, float4 screenPosData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
    
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

    float specularMask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv).g;
    outSurfaceData.metallic = _Metallic * specularMask;
    outSurfaceData.specular = _Metallic.rrr * specularMask;

    outSurfaceData.smoothness = albedoAlpha * _Smoothness;
    float2 screenPos = ComputeScreenPos(screenPosData / screenPosData.w).xy;
    float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, my_linear_clamp_sampler, screenPos);
    half depth = Linear01Depth(sceneDepth, _ZBufferParams);

    //depthのコントラスト調整
    float D = max(0, 1.0 - depth - 0.5);
    //D = 1 / (1 + exp(-500.0 * (D-0.5)));
    
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale * D);
    outSurfaceData.occlusion = SampleOcclusion(uv);
    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));

#if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
    half2 clearCoat = SampleClearCoat(uv);
    outSurfaceData.clearCoatMask       = clearCoat.r;
    outSurfaceData.clearCoatSmoothness = clearCoat.g;
#else
    outSurfaceData.clearCoatMask       = half(0.0);
    outSurfaceData.clearCoatSmoothness = half(0.0);
#endif

#if defined(_DETAIL)
    half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv).a;
    float2 detailUv = uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
    outSurfaceData.albedo = ApplyDetailAlbedo(detailUv, outSurfaceData.albedo, detailMask);
    outSurfaceData.normalTS = ApplyDetailNormal(detailUv, outSurfaceData.normalTS, detailMask);
#endif
}

#endif // UNIVERSAL_INPUT_SURFACE_PBR_INCLUDED
