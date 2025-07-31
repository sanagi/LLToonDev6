#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/BRDF.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/GlobalIllumination.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl"


#include "LLToonInput.hlsl"

#if defined(LIGHTMAP_ON)
    #define DECLARE_LIGHTMAP_OR_SH(lmName, shName, index) float2 lmName : TEXCOORD##index
    #define OUTPUT_LIGHTMAP_UV(lightmapUV, lightmapScaleOffset, OUT) OUT.xy = lightmapUV.xy * lightmapScaleOffset.xy + lightmapScaleOffset.zw;
    #define OUTPUT_SH(normalWS, OUT)
#else
    #define DECLARE_LIGHTMAP_OR_SH(lmName, shName, index) half3 shName : TEXCOORD##index
    #define OUTPUT_LIGHTMAP_UV(lightmapUV, lightmapScaleOffset, OUT)
    #define OUTPUT_SH(normalWS, OUT) OUT.xyz = SampleSHVertex(normalWS)
#endif

//それぞれのライティング色
struct LLLightingData
{
    half4 BaseToonLightingColor; //基本のToonライティング色 + Specular
    half4 AdditionalLightsColor; //追加光
    half4 RimColor; //リムライト
    half4 DarkRimColor; //暗部の反射光
    half4 EmissionColor; //Emission
    half4 GIColor; //GI
    half4 SpecRimEmission; //全体的な輝きコントロール
    half RampOutline; //2影の境界線調整値(メインで計算後アウトラインにに使いまわす)
    half HalfLambert;
};
            
//Toonに必要な要素
struct ToonShadowFactor
{
    float SWeight; //影範囲
    float SFactor; //1影の塗分け範囲
    float SFactorD; //影の塗分け範囲
    half rampS; //1影の境界線調整値
    half rampDS; //2影の境界線調整値
    float HalfLambert; //Halflambert情報
};

//Toonに必要な要素
struct RimFactor
{
    half4 RimColor; //リムライト
    half4 DarkRimColor; //暗部の反射光
};

//LLToonに必要な入力要素
struct LLToonInputData
{
    InputData baseInputData;
    float3 binormal;
    float3 normalOS;
    half4 color;
    float2 matcapUV;
};

//ライト情報からToonシェードに必要な情報を取得しておく
ToonShadowFactor CalculateToonShadowFactor(
    Light light, //ライト情報
    float3 normalWS, //ノーマル情報
    float mask, //ライトマップマスク(?)
    float mainLightShadowArea, //落ち影情報
    /*float2 uv,*/
    float3 pos
    )

{
    ToonShadowFactor tsf;
                
    float3 lightDirWS = normalize(light.direction.xyz);
#if ENABLE_FACE_CHEEK
    lightDirWS = normalize(light.direction.xyz + pos); //顔は常に一定の位置から照らす
#endif    
    float3 fixedlightDirWS = normalize(float3(lightDirWS.x, _FixLightY, lightDirWS.z));
    lightDirWS = _IgnoreLightY ? fixedlightDirWS: lightDirWS;

    tsf.HalfLambert = light.distanceAttenuation * dot(normalWS, lightDirWS) * 0.5f + 0.5f;
/*#if ENABLE_FACE_CHEEK
    float t = (SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).b - 1.0);
    float3 InverseXNormalTex = (SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).b - 1.0) * float3(-1, 1, 1);
    normalWS = lerp(normalWS, InverseXNormalTex, t);
    float originHalfLambert = tsf.HalfLambert;
    float calcHalfLambert = light.distanceAttenuation * dot(normalWS, lightDirWS) * 0.5f + 0.5f;
    tsf.HalfLambert = max(originHalfLambert, calcHalfLambert);
#endif
*/    
    
    //tsf.Lambert *= alwaysShadow;
    
    //影範囲の決定
    tsf.SWeight = tsf.HalfLambert * 0.5  + 1.125;

#if ENABLE_INVERSE_SHADOW //Inverseの時は逆転させとく(分かりやすく)
    _DarkShadowArea = 1.0 - _DarkShadowArea;
#endif    
    //影を塗り分ける範囲
    tsf.SFactor = floor(tsf.SWeight - _ShadowArea);
    tsf.SFactorD = floor(tsf.SWeight - _DarkShadowArea);

    // 境界線の調整
#if ENABLE_CHARA_ON_SHADOW
    tsf.rampS = smoothstep(0, _ShadowSmooth, (tsf.HalfLambert - _ShadowArea));
    tsf.rampDS = smoothstep(0, _DarkShadowSmooth, (tsf.HalfLambert - _DarkShadowArea));
#else    
    tsf.rampS = smoothstep(0, _ShadowSmooth, (tsf.HalfLambert - _ShadowArea) * mainLightShadowArea * mask);
    tsf.rampDS = smoothstep(0, _DarkShadowSmooth, (tsf.HalfLambert - clamp(_DarkShadowArea, 0, _ShadowArea)) * mainLightShadowArea * mask);
#endif

#if ENABLE_INVERSE_SHADOW
    //逆から反射光を入れたいとき
    tsf.rampDS = (clamp((tsf.rampDS - tsf.rampS), 0.0, 1.0)) * _EnableDarkShadow;
    tsf.rampDS = (1.0 - tsf.rampDS) * _EnableDarkShadow;
#endif    
    
    return tsf;
}

//Toonシェーディング + スぺキュラ
half4 ToonBaseLighting(
    float4 baseColor, //ベース色
    half3 shadowColor, //1影色
    half3 darkShadowColor, //2影色
    ToonShadowFactor toonShadowFactor, //Toonシェーディングする用の情報
    out half3 DarkShadowColor
    )
{
    half4 baseToonLightingColor = float4(0,0,0,1);
    
    //影を塗り分ける
    half3 ShallowShadowColor = toonShadowFactor.SFactor * baseColor.rgb + (1 - toonShadowFactor.SFactor) * shadowColor.rgb;
    ShallowShadowColor.rgb = lerp(shadowColor, baseColor.rgb, toonShadowFactor.rampS);

    // 境界線の調整
    DarkShadowColor = toonShadowFactor.SFactorD * ShallowShadowColor + (1 - toonShadowFactor.SFactorD) * darkShadowColor;
    DarkShadowColor.rgb = lerp(darkShadowColor, ShallowShadowColor, toonShadowFactor.rampDS);

    baseToonLightingColor.rgb = _EnableDarkShadow ? DarkShadowColor.rgb : ShallowShadowColor.rgb;
    
    return baseToonLightingColor;
}

half4 ToonBaseLightingAdd(
    float4 baseColor, //ベース色
    half3 shadowColor, //1影色
    half3 darkShadowColor, //2影色
    ToonShadowFactor toonShadowFactor,
    float lightMapMask
    )
{
    half3 dark;
    return ToonBaseLighting(baseColor, shadowColor, darkShadowColor, toonShadowFactor, dark);
}

//Specular計算
half4 LLToonSpecularLighting(
    BRDFData brdfData,
    LLToonInputData llToonInputData,
    float specularMask,
    float specularMaskHigh,
    Light light,
    bool chara,
    float rampS,
    half specularRadiance
    )
{
    half finalSpecularRadiance = specularRadiance <= 0.5 ? 0.5 : specularRadiance; //影でも0.1以下にはしない

    half calRadianceHair = (specularRadiance * 1.75) * (specularRadiance * 1.75);
    half finalSpecularHairRadiance = calRadianceHair <= 0.4 ? 0.4 : calRadianceHair; //影でも0.1以下にはしない
    half4 finalspecularColor = half4(0,0,0,0);
    //SpecDiffuse.rgb *= _BaseColor.rgb;
    
#if ENABLE_HAIR_SPECULAR    
    float3 halfDir = normalize(light.direction + llToonInputData.baseInputData.viewDirectionWS);
    float3 tmpBinormal = normalize(llToonInputData.binormal - llToonInputData.baseInputData.normalWS * llToonInputData.baseInputData.positionWS);
    float dotTH = dot(tmpBinormal, halfDir);
    float sintTH = sqrt(1.0 - dotTH * dotTH);
    float dirAtten = smoothstep(-1.0, 0.0, dotTH);
    half specular = dirAtten * pow(sintTH, _Sharpness) * specularMask;
    finalspecularColor = (_LightSpecColor * specular * rampS * _SpecularIntensity + (1 - rampS) * _LightSpecShadowColor * specular * _SpecularIntensityShadow) * finalSpecularHairRadiance;
    
    /*float3 dotTH = dot(normalize(llToonInputData.baseInputData.normalWS), normalize(llToonInputData.baseInputData.viewDirectionWS));
    float anisoHairFrenel = pow((1.0 - saturate(dotTH)), _Sharpness) * _SpecularIntensity;
    float anisoHair = saturate(1.0 - anisoHairFrenel) * specularMask * dot(light.direction, llToonInputData.baseInputData.normalWS);
    finalspecularColor = _LightSpecColor * anisoHair;
    */
#else
    half directSpecular = DirectBRDFSpecular(brdfData, llToonInputData.baseInputData.normalWS, light.direction, llToonInputData.baseInputData.viewDirectionWS);
    half surfaceSpecular = chara ? 1 : brdfData.specular; //キャラのマテリアルは少し強めにspecular出したい(マスクはここでやってるし)
    half specular = surfaceSpecular * directSpecular  * specularMask * _SpecularIntensity;
    half specularHigh = surfaceSpecular * directSpecular * specularMaskHigh * _SpecularIntensityHigh;
    float4 specColor = lerp(_LightSpecShadowColor, _LightSpecColor, specularRadiance);
    
    finalspecularColor = specColor * (specular + specularHigh) * finalSpecularRadiance;
#endif
    finalspecularColor.a = finalspecularColor.a * _BloomFactor;
    
    return finalspecularColor;
}

/**追加光はURPの追加光計算そのまま→Toonはメインライトで作る
 * \brief 
 * \param brdfData
 * \param llTInput 
 * \param surfaceData 
 * \param meshRenderingLayers 
 * \return 
 */
half4 LLToonAddLighting(
    BRDFData brdfData,
    LLToonInputData llTInput,
    SurfaceData surfaceData,
    uint meshRenderingLayers
    )
{
    //追加光情報
    uint pixelLightCount = GetAdditionalLightsCount();
    half4 addLightColor = half4(0,0,0,0);

    #if USE_CLUSTERED_LIGHTING
    for (uint lightIndex = 0; lightIndex < min(_AdditionalLightsDirectionalCount, MAX_VISIBLE_LIGHTS); lightIndex++)
    {
        Light light = GetAdditionalLight(lightIndex, llTInput.baseInputData.positionWS);
        if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
        {
            addLightColor.rgb += LightingPhysicallyBased(brdfData, brdfData, light,
                                                                  llTInput.baseInputData.normalWS, llTInput.baseInputData.viewDirectionWS,
                                                                  surfaceData.clearCoatMask, true);
        }
    }
    #endif

    LIGHT_LOOP_BEGIN(pixelLightCount)
        Light light = GetAdditionalLight(lightIndex, llTInput.baseInputData.positionWS);

    if (IsMatchingLightLayer(light.layerMask, meshRenderingLayers))
    {
        addLightColor.rgb += LightingPhysicallyBased(brdfData, brdfData, light,
                                                              llTInput.baseInputData.normalWS, llTInput.baseInputData.viewDirectionWS,
                                                              surfaceData.clearCoatMask, true);
    }
    LIGHT_LOOP_END
    return addLightColor;
}

//リムライト
RimFactor LLToonRimLighting(InputData inputData, float2 uv, float lambert, half4 baseColor)
{
    RimFactor Rim;

    float rim = 1 - saturate(dot(inputData.viewDirectionWS, inputData.normalWS));
    float rimMask = SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).r;
    float rimDot = pow(rim, _RimPow) * rimMask;
    rimDot = _EnableLambert * lambert * rimDot + (1 - _EnableLambert) * rimDot;
    float rimIntensity = smoothstep(0, _RimSmooth, rimDot);
    Rim.RimColor = _EnableRim * pow(rimIntensity, 5) * _RimColor * baseColor;
    Rim.RimColor.a = _EnableRim * rimIntensity * _BloomFactor;

    float darkRimDot = pow(rim, _DarkSideRimPow) * rimMask;
    darkRimDot = _EnableLambert * (1 - lambert) * darkRimDot + (1 - _EnableLambert) * darkRimDot;
    float darkRimIntensity = smoothstep(0, _DarkSideRimSmooth, darkRimDot);
    Rim.DarkRimColor = _EnableRimDS * pow(darkRimIntensity, 5) * _DarkSideRimColor * baseColor;;
    Rim.DarkRimColor.a = _EnableRimDS * darkRimIntensity * _BloomFactor;
    return Rim;
}

//Emission
half4 LLToonEmission(float2 uv, half4 baseLightingColor, half3 darkShadowColor, float baseAlpha, half emissionMask)
{
    half4 EmissionColor = half4(0,0,0,0);
    EmissionColor.rgb = _Emission * darkShadowColor * _EmissionColor.rgb * emissionMask - baseLightingColor.rgb * emissionMask * _EnableEmission;
    EmissionColor.a = _EmissionBloomFactor * baseAlpha * emissionMask;
    return EmissionColor;
}

//輝き調整
half4 LLToonBloom(half baseLightingColorAlpha,half rimAlpha, half3 darkShadowColor, half emissionMask)
{
    half4 bloom = half4(0,0,0,0);
    bloom.rgb = pow(darkShadowColor, _DarkEmissionIntensity) * _Emission * emissionMask;
    bloom.a = (baseLightingColorAlpha + rimAlpha/* + RimDS.a*/);
    return bloom;
}

void LLToonLighting (
    LLToonInputData inputData,
    SurfaceData surfaceData,
    float2 uv,
    bool chara,
    out LLLightingData LLToonLightingData,
    float4 screenPos)
{

    //マップによるマスク情報
    float lightMapMask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv).r;
    float specularMask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv).g;
    float emissionMask = SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv).b;
    float GIOffMapMask = SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).g;
    float specularMaskHigh = SAMPLE_TEXTURE2D(_MaskMap2, sampler_MaskMap2, uv).b;
    
    //BRDFデータを計算しておく
    BRDFData brdfData;
    InitializeBRDFData(surfaceData, brdfData);
    // Clear-coat計算
    BRDFData brdfDataClearCoat = CreateClearCoatBRDFData(surfaceData, brdfData);

    //デバッグ表示用
    #if defined(DEBUG_DISPLAY)
    half4 debugColor;

    if (CanDebugOverrideOutputColor(inputData, surfaceData, brdfData, debugColor))
    {
        return debugColor;
    }
    #endif

    //メインライトの情報取得
    half4 shadowMask = CalculateShadowMask(inputData.baseInputData);
    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData.baseInputData, surfaceData);
    Light mainLight = GetMainLight(inputData.baseInputData, shadowMask, aoFactor);
    
    //影を受ける
    float mainLightShadowArea = _ReceiveShadows ? mainLight.shadowAttenuation : 1;
    half NdotL = saturate(dot(inputData.baseInputData.normalWS, mainLight.direction));
    half radianceBase = mainLightShadowArea * lightMapMask * NdotL;
    half3 radiance = chara == true ? mainLight.color : radianceBase * mainLight.color;

    float maskGI = chara ? GIOffMapMask : 1.0f;
    //GIを計算しておく
    // NOTE: We don't apply AO to the GI here because it's done in the lighting calculation below...
    MixRealtimeAndBakedGI(mainLight, inputData.baseInputData.normalWS, inputData.baseInputData.bakedGI);
    LLToonLightingData.GIColor.rgb = GlobalIllumination(brdfData, brdfDataClearCoat, surfaceData.clearCoatMask,
                                              inputData.baseInputData.bakedGI, aoFactor.indirectAmbientOcclusion, inputData.baseInputData.positionWS,
                                              inputData.baseInputData.normalWS, inputData.baseInputData.viewDirectionWS) * maskGI;
    LLToonLightingData.GIColor.a = 0;
    
    LLToonLightingData.AdditionalLightsColor = half4(0,0,0,0); //追加光

    // 基礎色計算
    float4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;
    
    //影色情報を設定
    //2個目のカラーがある場合
    float secondColorMask = 1.0 - SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, uv).a;
    half3 ShadowColor = secondColorMask == 0 ? baseColor.rgb * _ShadowMultColor.rgb : baseColor.rgb * _SceondMaterialShadowColor.rgb;
    half3 DarkShadowColorInput = secondColorMask == 0 ? baseColor.rgb * _DarkShadowMultColor.rgb : baseColor.rgb * _SceondMaterialDarkShadowColor.rgb;

    //落ち影のリム調整
    float RimInShadow = mainLightShadowArea;
    float DarkRimInShadow = mainLightShadowArea > 0.5 ? 1.0 : 0.25;
#if ENABLE_CHARA_ON_SHADOW    
    //落ち影内の場合
    {
        baseColor.rgb = ShadowColor;
#if ENABLE_FACE_CHEEK
        baseColor.rgb = lerp(baseColor, ShadowColor, 0.5);
#endif
        ShadowColor =  lerp(ShadowColor, DarkShadowColorInput, 0.5);
        DarkShadowColorInput = lerp(DarkShadowColorInput, DarkShadowColorInput * 0.75, 0.5);
        RimInShadow = 0;
        DarkRimInShadow = 0.25;
    }
#endif

  
#if ENABLE_ALPHA_CLIPPING
    //clip(baseColor.a - _Cutoff);
#endif

    //レイヤー
    uint meshRenderingLayers = GetMeshRenderingLayer();

    //メインライトのトゥーン要素をキャッシュしておく
    ToonShadowFactor mainLightTSF = CalculateToonShadowFactor(mainLight, inputData.baseInputData.normalWS, lightMapMask, mainLightShadowArea * maskGI, inputData.baseInputData.positionWS.xyz);
    LLToonLightingData.RampOutline = mainLightTSF.rampS;
    LLToonLightingData.HalfLambert = mainLightTSF.HalfLambert;

    half3 DarkShadowColor = baseColor.rgb;

    //基礎Toon
    half4 baseLightingColor = float4(0,0,0,1);
    half4 rimLightColor = float4(0,0,0,1);
    half4 darkRimLightColor = float4(0,0,0,1);
    if (IsMatchingLightLayer(mainLight.layerMask, meshRenderingLayers))
    {
        //ライティング計算(Toon)
        baseLightingColor = ToonBaseLighting(baseColor, ShadowColor, DarkShadowColorInput, mainLightTSF, DarkShadowColor) * INV_PI * _DiffuseIntensity;
        //スぺキュラ足す
        baseLightingColor += _EnableSpecular ? LLToonSpecularLighting(brdfData, inputData, specularMask, specularMaskHigh, mainLight, chara, mainLightTSF.rampS * mainLightShadowArea, radianceBase) : 0;

        //Matcap設定があれ
#if ENABLE_MATCAP_SPECULAR
        baseLightingColor += SAMPLE_TEXTURE2D(_MatCap, sampler_MatCap, inputData.matcapUV) * _MatCapIntensity;
#endif
        
        //全体的にライトの色を載せる
        baseLightingColor.rgb = _WorldLightInfluence * radiance * baseLightingColor.rgb + (1 - _WorldLightInfluence) * baseLightingColor.rgb;

        //リム情報
        RimFactor rim = LLToonRimLighting(inputData.baseInputData, uv, mainLightTSF.HalfLambert, baseColor);
        //全体的にライトの色を載せる
        rimLightColor.rgb = (_WorldLightInfluence * radiance * rim.RimColor.rgb + (1 - _WorldLightInfluence) * rim.RimColor.rgb) * RimInShadow; //落ち影のなかでリムは生成されない
        
        //rimLightColor.rgb = EdgeHighlight(screenPos)* _RimColor;
        darkRimLightColor.rgb = (_WorldLightInfluence * radiance * rim.DarkRimColor.rgb + (1 - _WorldLightInfluence) * rim.DarkRimColor.rgb) * DarkRimInShadow; //逆リムはごく薄くなる
    }
    LLToonLightingData.BaseToonLightingColor = baseLightingColor;
    LLToonLightingData.RimColor = rimLightColor;
    LLToonLightingData.DarkRimColor = darkRimLightColor;
    
#if defined(_ADDITIONAL_LIGHTS)
    //追加光
    LLToonLightingData.AdditionalLightsColor = LLToonAddLighting(brdfData, inputData,surfaceData, meshRenderingLayers);
#endif
    
    // Emission & Bloom
    LLToonLightingData.EmissionColor = LLToonEmission(uv, baseLightingColor, DarkShadowColor, baseColor.a, emissionMask);
    LLToonLightingData.SpecRimEmission = LLToonBloom(baseLightingColor.a, LLToonLightingData.RimColor.a, DarkShadowColor.rgb, emissionMask);

    //finalColor = _WorldLightInfluence * finalColor + (1 - _WorldLightInfluence) * finalColor;
}