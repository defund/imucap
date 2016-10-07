// Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// you might want to tweak below defines by hand (commenting/uncommenting, but it's recommended to leave it for RTP_manager)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//

// layer count switch
#define _4LAYERS

// to compute far color basing only on global colormap
//#define SIMPLE_FAR

// uv blending
#define RTP_UV_BLEND
//#define RTP_DISTANCE_ONLY_UV_BLEND
//// comment below detail when not needed
#define RTP_SUPER_DETAIL
//#define RTP_SUPER_DTL_MULTS
// comment below if you don't use snow features
#define RTP_SNOW
// layer number taken as snow normal for near distance (for deep snow cover)
//#define RTP_SNW_CHOOSEN_LAYER_NORM_0
// layer number taken as snow color/gloss for near distance
//#define RTP_SNW_CHOOSEN_LAYER_COLOR_0

// comment if you don't need global color map
#define COLOR_MAP
// if not defined global color map will be blended (lerp)
#define COLOR_MAP_BLEND_MULTIPLY

// global normal map (and we will treat normals from mesh as flat (0,1,0))
//#define RTP_NORMALGLOBAL

// global trees/shadow map - used with Terrain Composer / World Composer by Nathaniel Doldersum
//#define RTP_TREESGLOBAL

// when defined you can see where layers 0-3 overlap layers 4-7 in 8 per pass mode. These areas costs higher
//  (note that when RTP_HARD_CROSSPASS is defined you won't see any overlapping areas)
//#define RTP_SHOW_OVERLAPPED
// when defined we don't calculate overlapping 0-3 vs 4-7 layers in 8 layers mode, but take "higher"
// it's recommended to use this define for significantly better performance
// undef it only when you really need smooth transitions between overlapping groups
#define RTP_HARD_CROSSPASS

// firstpass triplanar (works only with _4LAYERS)
//#define RTP_TRIPLANAR

// vertical texture
//#define RTP_VERTICAL_TEXTURE

// we use wet (can't be used with superdetail as globalnormal texture BA channels are shared)
//#define RTP_WETNESS
// water droplets
//#define RTP_WET_RIPPLE_TEXTURE
// if defined water won't handle flow nor refractions
//#define SIMPLE_WATER

//#define RTP_CAUSTICS

// reflection map
//#define RTP_REFLECTION
//#define RTP_ROTATE_REFLECTION

// if you don't use extrude reduction in layers properties (equals 0 everywhere)
// you can comment below - POM will run a bit faster
//#define USE_EXTRUDE_REDUCTION

//
// in 8 layers mode we can use simplier shading for not overplapped (RTP_HARD_CROSSPASS must be defined) 4-7 layers
// available options are:
// RTP_47SHADING_SIMPLE
// RTP_47SHADING_PM
// RTP_47SHADING_POM_LO (POM w/o shadows)
// RTP_47SHADING_POM_MED (POM with hard shadows)
// RTP_47SHADING_POM_HI (POM with soft shadows)
//


// in presence of 2 passes we can do heightblend between passes
//#define RTP_CROSSPASS_HEIGHTBLEND

// must be defined when we use 12 layers
//#define _12LAYERS

/////////////////////////////////////////////////////////////////////
//
// massive terrain - super simple mode
//
// if defined we're using very simple mode (4 layers only !)
// uses global color, global normal (optionaly), pixel trees / shadows (optionaly)
//#define SUPER_SIMPLE

// for super simple mode above:
// use bumpmapping
#define SS_USE_BUMPMAPS
// use perlin
#define SS_USE_PERLIN

//
// below setting isn't reflected in LOD manager, it's only available here (and in RTP_ADDPBase.cginc)
// you can use it to control snow coverage from wet mask (special combined texture channel B)
//
//#defineRTP_SNW_COVERAGE_FROM_WETNESS

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// don't touch below defines
//
// we're using mapped shadows for best performance (available only in 4 layers mode when using atlas)
// not implemented yet
//#define RTP_MAPPED_SHADOWS
#if defined(RTP_MAPPED_SHADOWS)
	#define USE_COLOR_ATLAS
#endif

#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
	#define UV_BLEND_SRC_0 (tex2Dlod(_SplatAtlasC, float4(uvSplat01M.xy, _MixMipActual.xx)).rgb)
	#define UV_BLEND_SRC_1 (tex2Dlod(_SplatAtlasC, float4(uvSplat01M.zw, _MixMipActual.yy)).rgb)
	#define UV_BLEND_SRC_2 (tex2Dlod(_SplatAtlasC, float4(uvSplat23M.xy, _MixMipActual.zz)).rgb)
	#define UV_BLEND_SRC_3 (tex2Dlod(_SplatAtlasC, float4(uvSplat23M.zw, _MixMipActual.ww)).rgb)
//	#define UV_BLEND_SRC_4 (tex2Dlod(_SplatAtlasB, float4(uvSplat01M.xy, _MixMipActual.xx)).rgb)
//	#define UV_BLEND_SRC_5 (tex2Dlod(_SplatAtlasB, float4(uvSplat01M.zw, _MixMipActual.yy)).rgb)
//	#define UV_BLEND_SRC_6 (tex2Dlod(_SplatAtlasB, float4(uvSplat23M.xy, _MixMipActual.zz)).rgb)
//	#define UV_BLEND_SRC_7 (tex2Dlod(_SplatAtlasB, float4(uvSplat23M.zw, _MixMipActual.ww)).rgb)
#else
	#define UV_BLEND_SRC_0 (tex2Dlod(_SplatC0, float4(uvSplat01M.xy, _MixMipActual.xx)).rgb)
	#define UV_BLEND_SRC_1 (tex2Dlod(_SplatC1, float4(uvSplat01M.zw, _MixMipActual.yy)).rgb)
	#define UV_BLEND_SRC_2 (tex2Dlod(_SplatC2, float4(uvSplat23M.xy, _MixMipActual.zz)).rgb)
	#define UV_BLEND_SRC_3 (tex2Dlod(_SplatC3, float4(uvSplat23M.zw, _MixMipActual.ww)).rgb)
#endif
//
// for example, when you'd like layer 3 to be source for uv blend on layer 0 you'd set it like this:
//   #define UV_BLEND_ROUTE_LAYER_0 UV_BLEND_SRC_3
// HINT: routing one layer into all will boost performance as only 1 additional texture fetch will be performed in shader (instead of up to 8 texture fetches in default setup)
//
#define UV_BLEND_ROUTE_LAYER_0 UV_BLEND_SRC_2
#define UV_BLEND_ROUTE_LAYER_1 UV_BLEND_SRC_2
#define UV_BLEND_ROUTE_LAYER_2 UV_BLEND_SRC_2
#define UV_BLEND_ROUTE_LAYER_3 UV_BLEND_SRC_2
#define UV_BLEND_ROUTE_LAYER_4 UV_BLEND_SRC_4
#define UV_BLEND_ROUTE_LAYER_5 UV_BLEND_SRC_5
#define UV_BLEND_ROUTE_LAYER_6 UV_BLEND_SRC_6
#define UV_BLEND_ROUTE_LAYER_7 UV_BLEND_SRC_7

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
#ifdef _4LAYERS
	#ifdef RTP_HARD_CROSSPASS
		#undef RTP_HARD_CROSSPASS
	#endif
#endif

#ifdef COLOR_MAP
	#define  DAMP_COLOR_VAL (global_color_value.a)
#else
	#define  DAMP_COLOR_VAL 1
#endif

#ifdef RTP_POM_SHADING_HI
	#define RTP_POM_SHADING
	#define RTP_SOFT_SHADOWS
#endif
#ifdef RTP_POM_SHADING_MED
	#define RTP_POM_SHADING
	#define RTP_HARD_SHADOWS
#endif
#ifdef RTP_POM_SHADING_LO
	#define RTP_POM_SHADING
	#define RTP_NO_SHADOWS
#endif

// wył. triplanar w trybie 8 layers
#ifndef _4LAYERS
	#ifdef RTP_TRIPLANAR
		#undef RTP_TRIPLANAR
	#endif
#endif

#ifdef RTP_TRIPLANAR
	#ifdef RTP_POM_SHADING
		#define RTP_PM_SHADING
		#undef RTP_POM_SHADING
	#endif
#endif

// (przed poniższym define musi być przynajmniej jedna spacje aby LOD manager tego nie psuł)
#ifdef RTP_SIMPLE_SHADING
	#ifndef RTP_DISTANCE_ONLY_UV_BLEND
         #define RTP_DISTANCE_ONLY_UV_BLEND
	#endif
#endif

#define GETrtp_snow_TEX rtp_snow_color_tex.rgb=csnow.rgb;rtp_snow_gloss=lerp(saturate(csnow.a*rtp_snow_gloss*2), 1, saturate((rtp_snow_gloss-0.5)*2));

// wyłącz kolor selction kiedy zabraknie samplerów
#if defined(_4LAYERS) && (!defined(USE_COLOR_ATLAS) || defined(RTP_MAPPED_SHADOWS))
#if defined(RTP_SNOW) && defined(RTP_VERTICAL_TEXTURE) && defined(RTP_WETNESS) && defined(RTP_WET_RIPPLE_TEXTURE)
	#ifdef RTP_SNW_CHOOSEN_LAYER_COLOR_4
		#undef RTP_SNW_CHOOSEN_LAYER_COLOR_4
	#endif 
	#ifdef RTP_SNW_CHOOSEN_LAYER_COLOR_5
		#undef RTP_SNW_CHOOSEN_LAYER_COLOR_5
	#endif 
	#ifdef RTP_SNW_CHOOSEN_LAYER_COLOR_6
		#undef RTP_SNW_CHOOSEN_LAYER_COLOR_6
	#endif 
	#ifdef RTP_SNW_CHOOSEN_LAYER_COLOR_7
		#undef RTP_SNW_CHOOSEN_LAYER_COLOR_7
	#endif 
#endif
#endif
// wyłącz wet ripple kiedy mamy inne opcje wszystkie powlaczane
#if defined(_4LAYERS) && defined(RTP_MAPPED_SHADOWS) && defined(RTP_VERTICAL_TEXTURE) && defined(RTP_WETNESS) && defined(RTP_WET_RIPPLE_TEXTURE)
	#undef RTP_WET_RIPPLE_TEXTURE
#endif
// potrzebujemy miejsca na dodatkowe samplery cieni
#if defined(RTP_MAPPED_SHADOWS)
	#if !defined(_4LAYERS)
		#undef RTP_MAPPED_SHADOWS
	#else
		#define USE_COLOR_ATLAS
	#endif
#endif

#if defined(SHADER_API_D3D11) 
CBUFFER_START(rtpConstants)
#endif

#ifdef TEX_SPLAT_REDEFINITION
	// texture samplers redefinitions (due to Unity bug that makes Unity4 terrain material unusable)
	#define _Control3 _Control
	#define _ColorMapGlobal _Splat0
	#define _NormalMapGlobal _Splat1
	#define _Control1 _Splat2
	#define _TreesMapGlobal _Splat2
	#define _BumpMapGlobal _Splat3
	sampler2D _Control;
#else
	sampler2D _Control, _Control3, _Control2, _Control1;
#endif

sampler2D _SplatAtlasC, _SplatAtlasB;
sampler2D _SplatC0, _SplatC1, _SplatC2, _SplatC3;
sampler2D _SplatB0, _SplatB1, _SplatB2, _SplatB3;
sampler2D _BumpMap89, _BumpMapAB, _BumpMap45, _BumpMap67;
sampler2D _ColorMapGlobal;
sampler2D _BumpMapGlobal;
float2 _terrain_size;
float _BumpMapGlobalScale;
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
float _GlobalColorMapBrightness;
float _GlobalColorMapNearMIP;
float4 _MixScale89AB, _MixBlend89AB, _MixSaturation89AB;
float4 _MixScale4567, _MixBlend4567, _MixSaturation4567;
float4 _GlobalColorPerLayer89AB, _GlobalColorPerLayer4567;
float4 _Spec89AB;
float4 _Spec4567;
float4 _FarGlossCorrection89AB;
float4 _FarGlossCorrection4567;
float4 _MIPmult89AB;
float4 _MIPmult4567;

sampler2D _TERRAIN_HeightMap3, _TERRAIN_HeightMap2, _TERRAIN_HeightMap;
float4 _TERRAIN_HeightMap3_TexelSize;
float4 _SplatAtlasC_TexelSize;
float4 _SplatAtlasB_TexelSize;
float4 _SplatC0_TexelSize;
float4 _BumpMapGlobal_TexelSize;
float4 _TERRAIN_ReliefTransform;
float _TERRAIN_ReliefTransformTriplanarZ;
float _TERRAIN_DIST_STEPS;
float _TERRAIN_WAVELENGTH;

float _blend_multiplier;

float _TERRAIN_ExtrudeHeight;
float _TERRAIN_LightmapShading;

float _TERRAIN_SHADOW_STEPS;
float _TERRAIN_WAVELENGTH_SHADOWS;
float _TERRAIN_SHADOW_SMOOTH_STEPS;
float _TERRAIN_SelfShadowStrength;
float _TERRAIN_ShadowSmoothing;
half3 _TERRAIN_ShadowColorization;

float rtp_mipoffset_color;
float rtp_mipoffset_bump;
float rtp_mipoffset_height;
float rtp_mipoffset_superdetail;
float rtp_mipoffset_flow;
float rtp_mipoffset_ripple;
float rtp_mipoffset_globalnorm;
float rtp_mipoffset_caustics;

// caustics
float TERRAIN_CausticsAnimSpeed;
half4 TERRAIN_CausticsColor;
float TERRAIN_CausticsWaterLevel;
float TERRAIN_CausticsWaterLevelByAngle;
float TERRAIN_CausticsWaterDeepFadeLength;
float TERRAIN_CausticsWaterShallowFadeLength;
float TERRAIN_CausticsTilingScale;
sampler2D TERRAIN_CausticsTex;

///////////////////////////////////////////
//
// reflection
//
half4 TERRAIN_ReflColorA;
half4 TERRAIN_ReflColorB;
float TERRAIN_ReflDistortion; // 0 - 0.1
float TERRAIN_ReflectionRotSpeed; // 0-2, 0.3
float TERRAIN_FresnelPow; // 0.25 - 16 (zrób skalę log na suwaku)
float TERRAIN_FresnelOffset; // 0-0.9
//
// water/wet
//
// global
float TERRAIN_GlobalWetness; // 0-1

float TERRAIN_Water_LayerBlending;
float TERRAIN_WaterSpecularity;

sampler2D TERRAIN_RippleMap;
float4 TERRAIN_RippleMap_TexelSize;
float TERRAIN_RippleScale; // 4
float TERRAIN_FlowScale; // 1
float TERRAIN_FlowSpeed; // 0 - 3 (0.5)
float TERRAIN_FlowMipOffset; // 0

float TERRAIN_RainIntensity; // 1
float TERRAIN_DropletsSpeed; // 10
float TERRAIN_WetDarkening;
float TERRAIN_WetDropletsStrength; // 0-1

float TERRAIN_mipoffset_flowSpeed; // 0-5

// per layer
float4 TERRAIN_LayerWetStrength89AB; // 0 - 1 (1)
float4 TERRAIN_LayerWetStrength4567;

float4 TERRAIN_WaterLevel89AB; // 0 - 2 (0.5)
float4 TERRAIN_WaterLevel4567;
float4 TERRAIN_WaterLevelSlopeDamp89AB; // 0.25 - 32 (2)
float4 TERRAIN_WaterLevelSlopeDamp4567;
float4 TERRAIN_WaterEdge89AB; // 1 - 16 (2)
float4 TERRAIN_WaterEdge4567;
float4 TERRAIN_WaterGloss89AB; // 0 - 1 (1)
float4 TERRAIN_WaterGloss4567;
float4 TERRAIN_WaterOpacity89AB; // 0 - 1 (0.3)
float4 TERRAIN_WaterOpacity4567;
float4 TERRAIN_Refraction89AB; // 0 - 0.04 (0.01)
float4 TERRAIN_Refraction4567; 
float4 TERRAIN_WetRefraction89AB; // 0 - 1 (0.25)
float4 TERRAIN_WetRefraction4567; 
float4 TERRAIN_Flow89AB; // 0 - 1 (0.1)
float4 TERRAIN_Flow4567;
float4 TERRAIN_WetSpecularity89AB; // 0 - 1 (0.5)
float4 TERRAIN_WetSpecularity4567;
float4 TERRAIN_WetReflection89AB; // 0-1 -> 0 .. color_textury.a (GlossDry), 1-2 -> color_textury.a .. 1
float4 TERRAIN_WetReflection4567;
float4 TERRAIN_LayerReflection89AB; // 0-1 -> 0 .. color_textury.a, 1-2 -> color_textury.a .. 1
float4 TERRAIN_LayerReflection4567; 
half4 TERRAIN_WaterColorR89AB;
half4 TERRAIN_WaterColorR4567;
half4 TERRAIN_WaterColorG89AB;
half4 TERRAIN_WaterColorG4567;
half4 TERRAIN_WaterColorB89AB;
half4 TERRAIN_WaterColorB4567;
half4 TERRAIN_WaterColorA89AB;
half4 TERRAIN_WaterColorA4567;

///////////////////////////////////////////

float _TERRAIN_distance_start;
float _TERRAIN_distance_transition;

float _TERRAIN_distance_start_bumpglobal;
float _TERRAIN_distance_transition_bumpglobal;
float rtp_perlin_start_val;
float4 _BumpMapGlobalStrength89AB;
float4 _BumpMapGlobalStrength4567;
float _FarNormalDamp;

sampler2D _NormalMapGlobal;
sampler2D _TreesMapGlobal;
float4 _TERRAIN_trees_shadow_values;
float3 _TERRAIN_trees_pixel_values;

float _RTP_MIP_BIAS;

float4 PER_LAYER_HEIGHT_MODIFIER89AB;
float4 PER_LAYER_HEIGHT_MODIFIER4567;

float _SuperDetailTiling;
float4 _SuperDetailStrengthMultA89AB;
float4 _SuperDetailStrengthMultA4567;
float4 _SuperDetailStrengthMultB89AB;
float4 _SuperDetailStrengthMultB4567;
float4 _SuperDetailStrengthNormal89AB;
float4 _SuperDetailStrengthNormal4567;

float4 _SuperDetailStrengthMultASelfMaskNear89AB;
float4 _SuperDetailStrengthMultASelfMaskNear4567;
float4 _SuperDetailStrengthMultASelfMaskFar89AB;
float4 _SuperDetailStrengthMultASelfMaskFar4567;
float4 _SuperDetailStrengthMultBSelfMaskNear89AB;
float4 _SuperDetailStrengthMultBSelfMaskNear4567;
float4 _SuperDetailStrengthMultBSelfMaskFar89AB;
float4 _SuperDetailStrengthMultBSelfMaskFar4567;

float4 _VerticalTexture_TexelSize;
sampler2D _VerticalTexture;
float _VerticalTextureTiling;
float _VerticalTextureGlobalBumpInfluence;
float4 _VerticalTexture89AB;
float4 _VerticalTexture4567;

float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer89AB;
float4 rtp_snow_strength_per_layer4567;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;

fixed4 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
float rtp_snow_deep_factor;
float rtp_snow_reflectivness;

half3 rtp_customAmbientCorrection;

#define RTP_BackLightStrength RTP_LightDefVector.x
#define RTP_ReflexLightDiffuseSoftness RTP_LightDefVector.y
#define RTP_ReflexLightSpecSoftness RTP_LightDefVector.z
#define RTP_ReflexLightSpecularity RTP_LightDefVector.w
float4 RTP_LightDefVector;
half4 RTP_ReflexLightDiffuseColor;
half4 RTP_ReflexLightSpecColor;

float RTP_AOamp;
float4 RTP_AO_0123, RTP_AO_89AB, RTP_AO_4567;
float RTP_AOsharpness;
#if defined(SHADER_API_D3D11) 
CBUFFER_END
#endif

float rtp_snow_strength;

#ifdef UNITY_PASS_PREPASSFINAL
uniform float4 _WorldSpaceLightPosCustom;
#endif

struct Input {
	float2 uv_Control : TEXCOORD0;
	float4 _uv_Relief;
	float4 _uv_Aux;
	
	#if !defined(SUPER_SIMPLE)
	float4 _viewDir;
	float4 lightDir;
	#endif
	
	// Geometry blend specific
	float4 color:COLOR;
};

fixed3 _FColor;
float _Fdensity;
void customFog (Input IN, SurfaceOutput o, inout fixed4 color) {
	float f=saturate(exp2(_Fdensity*IN._uv_Relief.w));
	color.rgb=lerp(_FColor, color.rgb, f);
}

inline fixed4 LightingCustomBlinnPhong (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	fixed diff = dot (s.Normal, lightDir);
	float diffBack = diff<0 ? diff*RTP_BackLightStrength : 0;
	diff = saturate(diff);
	
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular*128.0) * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * RTP_ReflexLightDiffuseColor.rgb* diffBack);
	s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
	c.rgb += (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2)  + rtp_customAmbientCorrection*0.5;
	c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
		
	//		
	// reflex lights
	//
	float3 normForDiffuse=lerp(s.Normal, float3(0,0,1), RTP_ReflexLightDiffuseSoftness);
	float3 normForSpec=s.Normal;//lerp(s.Normal, float3(0,0,1), RTP_ReflexLightSpecSoftness);
	//normForSpec=normalize(normForSpec);
	s.Gloss=saturate(s.Gloss);
	float glossDiff=(s.Gloss+1)*RTP_ReflexLightDiffuseColor.a;
	s.Gloss*=RTP_ReflexLightSpecColor.a*8;
	
	// specularity from the opposite view direction
	viewDir.xy=-viewDir.xy;
	h = normalize ( lightDir + viewDir );
	nh = abs(dot (normForSpec, h));
	spec = pow (nh, RTP_ReflexLightSpecularity) * s.Gloss;
	c.rgb += _LightColor0.rgb * RTP_ReflexLightSpecColor.rgb * spec;
	
	lightDir.y=-0.7; // 45 degrees
	lightDir=normalize(lightDir);
	
	float3 lightDirRefl;
	float3 refl_rot;
	refl_rot.x=0.86602540378443864676372317075294;// = sin(+120deg);
	refl_rot.y=-0.5; // = cos(+/-120deg);
	refl_rot.z=-refl_rot.x;
	
	// 1st reflex
	lightDirRefl.x=dot(lightDir.xz, refl_rot.yz);
	lightDirRefl.y=lightDir.y;
	lightDirRefl.z=dot(lightDir.xz, refl_rot.xy);	
	diff = max (0, dot (normForDiffuse, lightDirRefl))*glossDiff; 
	c.rgb += (s.Albedo * RTP_ReflexLightDiffuseColor.rgb * diff);
	
	// 2nd reflex
	lightDirRefl.x=dot(lightDir.xz, refl_rot.yx);
	lightDirRefl.z=dot(lightDir.xz, refl_rot.zy);	
	diff = max (0, dot (normForDiffuse, lightDirRefl))*glossDiff;
	c.rgb += (s.Albedo * RTP_ReflexLightDiffuseColor.rgb * diff);
	
	return c;
}

inline fixed4 LightingCustomBlinnPhong_PrePass (SurfaceOutput s, half4 light)
{
	fixed spec = light.a * s.Gloss;
	
	fixed4 c;
	s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;	
	c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec) + rtp_customAmbientCorrection*0.5;
	c.a = s.Alpha + spec * _SpecColor.a;
	return c;
}

inline half4 LightingCustomBlinnPhong_DirLightmap (SurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
{
	UNITY_DIRBASIS
	half3 scalePerBasisVector;
	
	color.rgb*=rtp_customAmbientCorrection*2+1;
	half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector) + rtp_customAmbientCorrection*0.5;
	
	half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
	half3 h = normalize (lightDir + viewDir);

	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular * 128.0);
	
	// specColor used outside in the forward path, compiled out in prepass
	specColor = lm * _SpecColor.rgb * s.Gloss * spec;
	
	// spec from the alpha component is used to calculate specular
	// in the Lighting*_Prepass function, it's not used in forward
	return half4(lm, spec);
}

inline float3 myObjSpaceLightDir( in float4 v )
{
	#ifdef UNITY_PASS_PREPASSFINAL
		float4 lpos=_WorldSpaceLightPosCustom;
	#else
		float4 lpos=_WorldSpaceLightPos0;
	#endif	
	float3 objSpaceLightPos = mul(_World2Object, lpos).xyz;
	#ifndef USING_LIGHT_MULTI_COMPILE
		return objSpaceLightPos.xyz - v.xyz * lpos.w;
	#else
		#ifndef USING_DIRECTIONAL_LIGHT
		return objSpaceLightPos.xyz * 1.0 - v.xyz;
		#else
		return objSpaceLightPos.xyz;
		#endif
	#endif
}

inline float2 GetRipple(float4 UV, float Intensity)
{
    float4 Ripple2 = tex2Dlod(TERRAIN_RippleMap, UV);
    Ripple2.xy = Ripple2.xy * 2 - 1;

    float DropFrac = frac(Ripple2.w + _Time.x*TERRAIN_DropletsSpeed);
    float TimeFrac = DropFrac - 1.0f + Ripple2.z;
    float DropFactor = saturate(0.2f + Intensity * 0.8f - DropFrac);
    float FinalFactor = DropFactor * Ripple2.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
    
    return Ripple2.xy * FinalFactor * 0.35f;
}
	
void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	
	#if defined(SUPER_SIMPLE)
		//
		// super simple mode
		//
		o._uv_Relief.xy=v.texcoord.xy * _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
		o._uv_Aux.xy=o._uv_Relief.xy*_BumpMapGlobalScale;				
		#ifdef APPROX_TANGENTS
			float3 _Dir=ObjSpaceViewDir(v.vertex);
			float _distance=length(_Dir);
			o._uv_Relief.w=_distance; // terrain isn't scaled
		#else
			o._uv_Relief.w=length(WorldSpaceViewDir(v.vertex)); // but custom geometry could be...
		#endif
		#ifdef APPROX_TANGENTS
			v.tangent.xyz = cross(v.normal, float3(0,0,1));
			v.tangent.w = -1;
		#endif		
		#ifdef RTP_NORMALGLOBAL
			v.normal.xyz=float3(0,1,0);
		#endif
	#else
		//
		// regular mode
		//	
		o._uv_Relief.xy=v.texcoord.xy * _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
		#if defined(RTP_TRIPLANAR)
			#if defined(COLOR_EARLY_EXIT) || !defined(APPROX_TANGENTS)
				o._uv_Relief.z=mul(_Object2World, v.vertex).y/ _TERRAIN_ReliefTransformTriplanarZ;
			#else
				o._uv_Relief.z=v.vertex.y/ _TERRAIN_ReliefTransformTriplanarZ;
			#endif
		#endif
		
		// przeniesione wyzej
		float3 _Dir=ObjSpaceViewDir(v.vertex);
		float _distance=length(_Dir);
		#if defined(RTP_REFLECTION)
			float3 viewRefl = reflect (-_Dir, v.normal);
			float2 refl_vec = normalize(mul((float3x3)_Object2World, viewRefl)).xz;
			float2 worldRefl;
			#ifdef RTP_ROTATE_REFLECTION
				float3 refl_rot;
				refl_rot.x=sin(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.y=cos(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.z=-refl_rot.x;
				worldRefl.x=dot(refl_vec, refl_rot.yz);
				worldRefl.y=dot(refl_vec, refl_rot.xy);
			#else
				worldRefl=refl_vec;
			#endif
			#if defined(RTP_TRIPLANAR)
				// reflection+triplanar - obl. ddx/ddy do pixel shadera
				o._uv_Aux.xy=worldRefl*0.5+0.5;
			#else
				o._uv_Aux.xy=o._uv_Relief.xy*1024*(1+_RTP_MIP_BIAS);
				o._uv_Aux.zw=worldRefl*0.5+0.5;
			#endif
		#else
			o._uv_Aux.xy=o._uv_Relief.xy*1024*(1+_RTP_MIP_BIAS);
		#endif
			
		#ifdef APPROX_TANGENTS
			o._uv_Relief.w=_distance; // terrain isn't scaled
		#else
			o._uv_Relief.w=length(WorldSpaceViewDir(v.vertex)); // but custom geometry could be...
		#endif
			
		#ifdef APPROX_TANGENTS
			#if defined(RTP_SNOW) || defined(RTP_TRIPLANAR)
				// ta aproksymacja lepiej trzyma "pion", w triplanar jest konieczna
				v.tangent.xyz = normalize( cross(v.normal, float3(0, -v.normal.z, v.normal.y)) ); // uproszczony zapis klasycznej aproksymacji w oparciu o wektor (1,0,0)
				v.tangent.w = -1.0;
			#else
				// tak jest lepiej ze wzgl. na POM (i prościej)
			    v.tangent.xy = float2(1, 0) -  v.normal.x*v.normal.xy;
	//		   	//v.tangent.xy=normalize(v.tangent.xy); // z normalizacja roznica nieznaczna ...
			    v.tangent.z=0;
				v.tangent.w = -1;
			#endif	
		#endif
		#if defined(RTP_WETNESS) || defined(RTP_REFLECTION) || defined(RTP_SNOW) ||  defined(RTP_CAUSTICS) || defined(RTP_POM_SHADING) || defined(RTP_PM_SHADING) || (!defined(LIGHTMAP_OFF) && defined (DIRLIGHTMAP_OFF))
			float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal.xyz );
			
			o.lightDir.xyz = mul (rotation, myObjSpaceLightDir(v.vertex));
			o.lightDir.xyz=normalize(o.lightDir.xyz);
			//o.lightDir.z/=_TERRAIN_ExtrudeHeight;
		#endif
		
		#ifdef RTP_POM_SHADING
			float3 EyeDirTan=mul(rotation, _Dir);
			EyeDirTan/=_distance;
			
			//EyeDirTan.z/=_TERRAIN_ExtrudeHeight;
		 	o._viewDir.xy=-EyeDirTan.xy;
		 	//o._viewDir.xy*=_terrain_size;
	 	#else
			#if defined(RTP_PM_SHADING) || defined(RTP_REFLECTION)
				float3 EyeDirTan=mul(rotation, _Dir);
				EyeDirTan/=_distance;
			 	o._viewDir.xy=EyeDirTan.xy;
			 #endif
		#endif
	
		#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS)
			#ifdef APPROX_TANGENTS
				o._viewDir.zw = ( mul (rotation, float3(0,1,0)) ).xy; // teren jest nierotowalny
			#else
				o._viewDir.zw = ( mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz) ).xy;
			#endif
		#endif
		
		#if defined(RTP_SNOW) || defined(RTP_VERTICAL_TEXTURE) || defined(RTP_CAUSTICS)
			#if defined(COLOR_EARLY_EXIT) || !defined(APPROX_TANGENTS)
				o.lightDir.w = mul(_Object2World, v.vertex).y;
			#else
				o.lightDir.w = v.vertex.y;
			#endif
		#endif
		
		#ifdef RTP_TRIPLANAR
			#if defined(COLOR_EARLY_EXIT)
				o._uv_Aux.zw=0;// zaszyte gotowe w kolorze
			#else
				#ifdef APPROX_TANGENTS
					o._uv_Aux.zw=v.normal.xz;
				#else
					o._uv_Aux.zw=mul(_Object2World, float4(v.normal,0)).xz;			
				#endif
			#endif
		#endif
			
		#if defined(RTP_POM_SHADING_HI) || defined(RTP_POM_SHADING_MED) || defined( RTP_POM_SHADING_LO)
			float2 stretch_factor=v.normal.yy;
			stretch_factor /= float2(length(v.normal.xy), length(v.normal.zy));
			o._viewDir.xy*=stretch_factor.xy;
			o.lightDir.xy*=stretch_factor.xy;
		#endif
		
		#ifdef RTP_NORMALGLOBAL
			v.normal.xyz=float3(0,1,0);
		#else
			float far=saturate((o._uv_Relief.w - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
			v.normal.xyz=lerp(v.normal.xyz, float3(0,1,0), far*_FarNormalDamp);
		#endif	
	#endif	
	
}

void surf (Input IN, inout SurfaceOutput o) {
	o.Normal=float3(0,0,1); o.Albedo=0;	o.Emission=0; o.Gloss=0; o.Specular=1; o.Alpha=0;
//	o.Emission.xy=frac(IN._uv_Relief.xy);//tex2D(_SplatA0, IN.uv_Control).rgb;
//	return;
		
	#if defined(SUPER_SIMPLE)
		//
		// super simple mode
		//
		#ifdef COLOR_EARLY_EXIT
			if (IN.color.a<0.002) return;
		#endif
		float4 splat_control = tex2D(_Control3, IN.uv_Control);		
		float total_coverage=dot(splat_control,1);
		splat_control/=total_coverage;
		if (total_coverage<0.002) return;
		
		float _uv_Relief_w=saturate((IN._uv_Relief.w - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		
		#ifdef COLOR_MAP
			float global_color_blend=lerp( _GlobalColorMapBlendValues.x, _GlobalColorMapBlendValues.z, _uv_Relief_w);
			float4 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control);
			#ifndef FAR_ONLY		
			global_color_value.rgb=lerp(tex2Dlod(_ColorMapGlobal, float4(IN.uv_Control, _GlobalColorMapNearMIP.xx)), global_color_value.rgb, _uv_Relief_w);
			#endif
			#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
				//global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*1.4+0.2),_uv_Relief_w));
				global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*2+0.7),_uv_Relief_w));
			#else
				global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
			#endif
			global_color_value.rgb*=_GlobalColorMapBrightness;
		#endif
		
		#ifdef FAR_ONLY
		if (false) {
		#else
		if (_uv_Relief_w<1) {
		#endif
		 	fixed4 col;
			col = splat_control.r * tex2D(_SplatC0, IN._uv_Relief.xy);
			col += splat_control.g * tex2D(_SplatC1, IN._uv_Relief.xy);
			col += splat_control.b * tex2D(_SplatC2, IN._uv_Relief.xy);
			col += splat_control.a * tex2D(_SplatC3, IN._uv_Relief.xy);
			o.Gloss=col.a;
			o.Albedo=lerp(col.rgb, global_color_value.rgb, _uv_Relief_w);
			o.Gloss*=(1-_uv_Relief_w);
			o.Specular = 0.03+dot(_Spec89AB, splat_control);
			o.Specular=lerp(o.Specular, 0.5, _uv_Relief_w);
			
			#if defined(SS_USE_BUMPMAPS)
				float3 n;
				float4 normals_combined;
				normals_combined = tex2D(_BumpMap89, IN._uv_Relief.xy).rgba*splat_control.rrgg;
				normals_combined+=tex2D(_BumpMapAB, IN._uv_Relief.xy).rgba*splat_control.bbaa;
				n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
				n.xy*=1-_uv_Relief_w;
				n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
				o.Normal=n;
			#endif
		} else {
			o.Albedo=global_color_value.rgb;
			o.Gloss=0;
			o.Specular=0.5;
		}
		
		#if defined(SS_USE_PERLIN)
			float4 global_bump_val=tex2D(_BumpMapGlobal, IN._uv_Aux.xy);
			float3 norm_far;
			norm_far.xy = global_bump_val.rg*4-2;
			norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));		
			o.Normal+=norm_far*lerp(rtp_perlin_start_val,1, _uv_Relief_w)*dot(_BumpMapGlobalStrength89AB, splat_control);
		#endif
		
		global_color_blend *= dot(splat_control, _GlobalColorPerLayer89AB);
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			o.Albedo=lerp(o.Albedo, o.Albedo*global_color_value.rgb*2, global_color_blend);
		#else
			o.Albedo=lerp(o.Albedo, global_color_value.rgb, global_color_blend);
		#endif
		
		#ifdef RTP_TREESGLOBAL	
			float4 pixel_trees_val=tex2D(_TreesMapGlobal, IN.uv_Control);
			float pixel_trees_blend_val=saturate((pixel_trees_val.r+pixel_trees_val.g+pixel_trees_val.b)*_TERRAIN_trees_pixel_values.z);
			pixel_trees_blend_val*=saturate((IN._uv_Relief.w - _TERRAIN_trees_pixel_values.x) / _TERRAIN_trees_pixel_values.y);
			o.Albedo=lerp(o.Albedo, pixel_trees_val.rgb, pixel_trees_blend_val);
	
			float pixel_trees_shadow_val=saturate((IN._uv_Relief.w - _TERRAIN_trees_shadow_values.x) / _TERRAIN_trees_shadow_values.y);
			pixel_trees_shadow_val=lerp(1, pixel_trees_val.a, pixel_trees_shadow_val);
			o.Albedo*=lerp(_TERRAIN_trees_shadow_values.z, 1, pixel_trees_shadow_val);
		#endif			
		
		#ifdef RTP_NORMALGLOBAL
			float3 global_norm;
			#if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
				global_norm.xy=tex2D(_NormalMapGlobal, IN.uv_Control).xy * 2 - 1;
			#else
				global_norm.xy=tex2D(_NormalMapGlobal, IN.uv_Control).wy * 2 - 1;
			#endif	
			global_norm.xy*=_TERRAIN_trees_shadow_values.w;
			global_norm.z=sqrt(1 - saturate(dot(global_norm.xy, global_norm.xy)));
			o.Normal+=global_norm;
		#endif
		#if defined(RTP_NORMALGLOBAL) || defined(SS_USE_PERLIN)
			o.Normal=normalize(o.Normal);
		#endif
		o.Alpha=total_coverage;		
		//
		// EOF super simple mode
		//
	#else
	//
	// regular mode
	//	
	float2 mip_selector;
	
	#if (defined(RTP_REFLECTION) && defined(RTP_TRIPLANAR))
		float2 IN_uv_Aux=IN._uv_Relief.xy*1024*(1+_RTP_MIP_BIAS);
		float2 dx = ddx( IN_uv_Aux.xy);
		float2 dy = ddy( IN_uv_Aux.xy);
	#endif
	#if defined(RTP_TRIPLANAR) 
		float2 mip_selectorTRIPLANAR;
		{
			float3 tmpUVZ=IN._uv_Relief.xyz*1024*(1+_RTP_MIP_BIAS);
			float2 dx = ddx( tmpUVZ.yz );
			float2 dy = ddy( tmpUVZ.yz );
			float d = max( dot( dx, dx ), dot( dy, dy ) );
			mip_selectorTRIPLANAR.x=0.5*log2(d);
			dx = ddx( tmpUVZ.xz );
			dy = ddy( tmpUVZ.xz );
			d = max( dot( dx, dx ), dot( dy, dy ) );
			mip_selectorTRIPLANAR.y=0.5*log2(d);
		}
	#endif
	
	#ifdef COLOR_EARLY_EXIT
		if (IN.color.a<0.002) return;
	#endif
	
	float4 splat_controlA = tex2D(_Control3, IN.uv_Control);
 	float total_coverage=dot(splat_controlA, 1);
 	#ifdef _4LAYERS
		float4 splat_controlA_normalized=splat_controlA/total_coverage;
	#else
		float4 splat_controlB = tex2D(_Control2, IN.uv_Control);
	 	total_coverage+=dot(splat_controlB, 1);
		float4 splat_controlA_normalized=splat_controlA/total_coverage;
		float4 splat_controlB_normalized=splat_controlB/total_coverage;
	#endif
	if (	total_coverage<0.001) return;
	
	#ifdef FAR_ONLY
		#define _uv_Relief_z 0
	#else
		float _uv_Relief_z=saturate((IN._uv_Relief.w - _TERRAIN_distance_start) / _TERRAIN_distance_transition);
		_uv_Relief_z=1-_uv_Relief_z;
	#endif
	float _uv_Relief_w=saturate((IN._uv_Relief.w - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
	
	float3 IN_viewDir;
	IN_viewDir.xy=IN._viewDir.xy;
	IN_viewDir.z=sqrt(1 - saturate(dot(IN_viewDir.xy, IN_viewDir.xy)));	

	float4 tHA;
	float4 tHB=0;
	float4 splat_control1 = splat_controlA;
	#ifdef USE_EXTRUDE_REDUCTION
		tHA=saturate(lerp(tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.xy), 1, PER_LAYER_HEIGHT_MODIFIER89AB)+0.001);
		splat_control1 *= tHA;
	#else
		tHA=saturate(tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.xy)+0.001);
		splat_control1 *= tHA;
	#endif	
	float4 splat_control1_mid=splat_control1*splat_control1;
	float4 splat_control1_close=splat_control1_mid*splat_control1_mid;
	splat_control1=lerp(lerp(splat_control1_mid, splat_control1, _uv_Relief_w), splat_control1_close, _uv_Relief_z);
	#ifndef _4LAYERS
		float4 splat_control2 = splat_controlB;
		#ifdef USE_EXTRUDE_REDUCTION
			tHB=saturate(lerp(tex2D(_TERRAIN_HeightMap2, IN._uv_Relief.xy), 1, PER_LAYER_HEIGHT_MODIFIER4567)+0.001);
			splat_control2 *= tHB;
		#else
			tHB=saturate(tex2D(_TERRAIN_HeightMap2, IN._uv_Relief.xy)+0.001);
			splat_control2 *= tHB;
		#endif	
		float4 splat_control2_mid=splat_control2*splat_control2;
		float4 splat_control2_close=splat_control2_mid*splat_control2_mid;
		splat_control2=lerp(lerp(splat_control2_mid, splat_control2, _uv_Relief_w), splat_control2_close, _uv_Relief_z);
					
		{
		float normalize_sum=dot(splat_control1, 1)+dot(splat_control2, 1);
		float instabilityFactor=saturate(normalize_sum*1024);
		splat_control1 = lerp(splat_controlA, splat_control1 / normalize_sum, instabilityFactor);
		splat_control2 = lerp(splat_controlB, splat_control2 / normalize_sum, instabilityFactor);
		}
	#else
		{
		float normalize_sum=dot(splat_control1, 1);
		float instabilityFactor=saturate(normalize_sum*1024);
		splat_control1 = lerp(splat_controlA, splat_control1 / normalize_sum, instabilityFactor);
		}
	#endif
		
	float splat_controlA_coverage=dot(splat_control1, 1);
	#ifndef _4LAYERS
	float splat_controlB_coverage=dot(splat_control2, 1);
	#endif
	
	#ifdef RTP_WETNESS
		float actH=dot(splat_control1, tHA);
		#ifndef _4LAYERS
			actH+=dot(splat_control2, tHB);
		#endif
	#endif
		
	#ifdef COLOR_MAP
		float global_color_blend=lerp( lerp(_GlobalColorMapBlendValues.y, _GlobalColorMapBlendValues.x, _uv_Relief_z*_uv_Relief_z), _GlobalColorMapBlendValues.z, _uv_Relief_w);
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control);
		global_color_value.rgb=lerp(tex2Dlod(_ColorMapGlobal, float4(IN.uv_Control, _GlobalColorMapNearMIP.xx)), global_color_value.rgb, _uv_Relief_w);
		#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			//global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*1.4+0.2),_uv_Relief_w));
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*2+0.7),_uv_Relief_w));
		#else
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
		#endif
		global_color_value.rgb*=_GlobalColorMapBrightness;
	#endif

    #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
        half rim = saturate(1.0+TERRAIN_FresnelOffset - dot (normalize(IN_viewDir), float3(0,0,1) ));
        rim*=saturate(pow (rim, TERRAIN_FresnelPow));
        float p = 0;
        float GlossDry=0;
		float TERRAIN_LayerReflection=0;
		float TERRAIN_WaterGloss=0;
	#endif
	
    #if defined(RTP_WETNESS)
		float TERRAIN_LayerWetStrength=0;
		float TERRAIN_WetReflection=0;
		float TERRAIN_WetSpecularity=0;
		half4 TERRAIN_WaterColor=half4(1,1,1,1);
    #endif		
	#if defined(RTP_WETNESS) || (defined(RTP_SNOW) && defined(RTP_SNW_COVERAGE_FROM_WETNESS))
		mip_selector=saturate(IN._uv_Relief.w-1);// bug in compiler for forward pass, we have to specify mip level indirectly (can't be treated constant)
		float water_mask=tex2Dlod(_BumpMapGlobal, float4(IN.uv_Control*(1-2*_BumpMapGlobal_TexelSize)+_BumpMapGlobal_TexelSize, mip_selector)).b;
    #endif		
	 	
	#if !( defined(RTP_REFLECTION) && defined(RTP_TRIPLANAR) )
		float2 dx = ddx( IN._uv_Aux.xy);
		float2 dy = ddy( IN._uv_Aux.xy);
	#endif
	float d = max( dot( dx, dx ), dot( dy, dy ) );
	mip_selector=0.5*log2(d);
	
	#if defined(RTP_TRIPLANAR) 
		float3 triplanar_blend;
		#if defined(COLOR_EARLY_EXIT) 
			float2 _uv_Aux=IN.color.xz*2-1;
			triplanar_blend.xz = abs(_uv_Aux);
			bool2 triplanar_flip = _uv_Aux<0;
			triplanar_blend.y=abs(IN.color.y*2-1);
		#else
			triplanar_blend.xz = abs(IN._uv_Aux.zw);
			bool2 triplanar_flip = IN._uv_Aux.zw<0;
			triplanar_blend.y=sqrt(1 - saturate(dot(triplanar_blend.xz, triplanar_blend.xz)));
		#endif
		
		float3 triplanar_blend_tmp=triplanar_blend/dot(triplanar_blend,1);
		float4 global_bump_val=triplanar_blend_tmp.x*tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.yz*_BumpMapGlobalScale, mip_selectorTRIPLANAR.xx+rtp_mipoffset_globalnorm));
		global_bump_val+=triplanar_blend_tmp.y*tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy*_BumpMapGlobalScale, mip_selector+rtp_mipoffset_globalnorm));
		global_bump_val+=triplanar_blend_tmp.z*tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xz*_BumpMapGlobalScale, mip_selectorTRIPLANAR.yy+rtp_mipoffset_globalnorm));
		
		#ifdef FAR_ONLY
			triplanar_blend = pow(abs(triplanar_blend), 128);
		#else
			triplanar_blend = pow(abs(triplanar_blend), lerp(128,32,_uv_Relief_z));
		#endif
		triplanar_blend/=dot(abs(triplanar_blend),1);		
		
		bool3 triplanar_blend_vector=triplanar_blend<0.95;
		bool triplanar_blend_flag=all(triplanar_blend_vector);
		float triplanar_blend_simple=max(max(triplanar_blend.x, triplanar_blend.y), triplanar_blend.z);
		#if defined(RTP_SUPER_DETAIL)
			float triplanar_blend_superdetail=saturate(triplanar_blend_simple-0.95)*20;
		#endif
		float3 uvTRI=float3(IN._uv_Relief.xy, mip_selector.x);
		float3 dirTRI=IN_viewDir.xyz;
		//o.Emission.x=triplanar_blend_flag ? 0.1:0;
		uvTRI=(triplanar_blend_simple==triplanar_blend.x) ? float3(IN._uv_Relief.yz, mip_selectorTRIPLANAR.x) : uvTRI;
		dirTRI.xy=(triplanar_blend_simple==triplanar_blend.x) ? IN_viewDir.yx : dirTRI.xy;
		if (triplanar_blend_simple==triplanar_blend.x) dirTRI.y=triplanar_flip.x ? dirTRI.y : -dirTRI.y;
		uvTRI=(triplanar_blend_simple==triplanar_blend.z) ? float3(IN._uv_Relief.xz, mip_selectorTRIPLANAR.y) : uvTRI;
		if (triplanar_blend_simple==triplanar_blend.z) dirTRI.y=triplanar_flip.y ? dirTRI.y : -dirTRI.y;
		//triplanar_blend_simple=1;
	#else
		float4 global_bump_val=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy*_BumpMapGlobalScale, mip_selector+rtp_mipoffset_globalnorm));
	#endif	
	
	fixed3 col=0;
	
	float3 norm_far=float3(0,0,1);
	float _BumpMapGlobalStrengthPerLayer=1;
	
	#if defined(RTP_SNOW) || defined(RTP_WETNESS)
		float perlinmask=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy/8, mip_selector+rtp_mipoffset_color-3+_uv_Relief_w*2)).r;
	#endif
	#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS)
		float3 flat_dir;
		flat_dir.xy=IN._viewDir.zw;
		flat_dir.z=sqrt(1 - saturate(dot(flat_dir.xy, flat_dir.xy)));
		#if defined(RTP_WETNESS)
			float wetSlope=1-dot(norm_far, flat_dir.xyz);
		#endif
	#endif

	#ifdef RTP_CAUSTICS
	float damp_fct_caustics;
	{
		float norm=(1-flat_dir.z);
		norm*=norm;
		norm*=norm;  
		float CausticsWaterLevel=TERRAIN_CausticsWaterLevel+norm*TERRAIN_CausticsWaterLevelByAngle;
		damp_fct_caustics=saturate((IN.lightDir.w-CausticsWaterLevel+TERRAIN_CausticsWaterDeepFadeLength)/TERRAIN_CausticsWaterDeepFadeLength);
		float overwater=saturate(-(IN.lightDir.w-CausticsWaterLevel-TERRAIN_CausticsWaterShallowFadeLength)/TERRAIN_CausticsWaterShallowFadeLength);
		damp_fct_caustics*=overwater;
	}
	#endif	

	norm_far.xy = global_bump_val.rg*3-1.5;
	norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));
	#ifdef _4LAYERS
		_BumpMapGlobalStrengthPerLayer=dot(_BumpMapGlobalStrength89AB, splat_control1);
	#else
		_BumpMapGlobalStrengthPerLayer=dot(_BumpMapGlobalStrength89AB, splat_control1)+dot(_BumpMapGlobalStrength4567, splat_control2);
	#endif
		
	#ifdef RTP_SNOW		
		float3 norm_for_snow=norm_far*0.3;
		norm_for_snow.z+=0.7;
	#endif

	float2 IN_uv_Relief_Offset;
	#ifdef RTP_SNOW
		float snow_const = 0.5*rtp_snow_strength-perlinmask;
		#if defined(RTP_SNW_COVERAGE_FROM_WETNESS)
			snow_const -= water_mask*2;
		#endif		
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.lightDir.w)/rtp_snow_height_transition)*4;
		snow_height_fct=snow_height_fct<0 ? 0 : snow_height_fct;
		snow_const -= snow_height_fct;
		
		#ifdef _4LAYERS
			float rtp_snow_layer_damp=dot(splat_control1, rtp_snow_strength_per_layer89AB);
		#else
			float rtp_snow_layer_damp=dot(splat_control1, rtp_snow_strength_per_layer89AB)+dot(splat_control2, rtp_snow_strength_per_layer4567);
		#endif	
		
		float snow_val;
		#ifdef COLOR_MAP
			snow_val = snow_const + rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow.xxx)+rtp_snow_strength*2;
		#else
			snow_val = snow_const + rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow+rtp_snow_strength*2;
		#endif
		snow_val *= rtp_snow_layer_damp;
		snow_val -= rtp_snow_slope_factor*( 1 - dot(norm_for_snow, flat_dir.xyz) );

		float snow_depth=snow_val-1;
		//bool snow_MayBeNotFullyCovered_flag=(snow_val-rtp_snow_slope_factor)<3; //wyostrzamy warunek (oryginalnie 1) bo ta estymacja nie dziala gdy mamy ostre przejscia pomiędzy materialami i mamy łączenie pomiędzy materiałem ze zredukowanym śniegiem a innym
		snow_depth=snow_depth<0 ? 0:snow_depth*6; 
		
		float snow_depth_lerp=saturate(snow_depth-rtp_snow_deep_factor);

		fixed3 rtp_snow_color_tex=rtp_snow_color.rgb;
	#endif
	
	#ifdef RTP_UV_BLEND
		#ifdef RTP_DISTANCE_ONLY_UV_BLEND
			float blendVal=_uv_Relief_w;
		#else
			float blendVal=(1.0-_uv_Relief_z*0.3);
		#endif
		#ifdef _4LAYERS
			blendVal*=dot(_MixBlend89AB, splat_control1);
		#else
			blendVal*=dot(_MixBlend89AB, splat_control1)+dot(_MixBlend4567, splat_control2);
		#endif
		blendVal*=_blend_multiplier*saturate((global_bump_val.r*global_bump_val.g*2+0.3));
	#endif
	
	#ifdef RTP_POM_SHADING
		IN_viewDir.z=-IN_viewDir.z;
	#endif

	#ifdef FAR_ONLY
	if (false) {
	#else
	if (_uv_Relief_z>0) {
	#endif
 		//////////////////////////////////
 		//
 		// close
 		//
 		//////////////////////////////////
		#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
			float _off=16*_SplatAtlasC_TexelSize.x;
			float _mult=_off*-2+0.5;
			float4 _offMix=_off;
			float4 _multMix=_mult;
			float hi_mip_adjust=(exp2(min(mip_selector.x+rtp_mipoffset_color,6)))*_SplatAtlasC_TexelSize.x; 
			_mult-=hi_mip_adjust;
			_off+=0.5*hi_mip_adjust;
			float4 uvSplat01, uvSplat23;
			float4 uvSplat01M, uvSplat23M;
		#endif

	 	float4 rayPos = float4(IN._uv_Relief.xy, 1, clamp((mip_selector.x+rtp_mipoffset_height), 0, 6) );
	 	
	 	#ifdef RTP_POM_SHADING
		float3 EyeDirTan = IN_viewDir.xyz;
		float slopeF=1+IN_viewDir.z;
		slopeF*=slopeF;
		slopeF*=slopeF;
		EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*DAMP_COLOR_VAL*_uv_Relief_z*(1-slopeF)); // damp bo kanale a colormapy, odleglosci i skompresowanym kacie obserwaci (poprawia widok zboczy)
		bool hit_flag=false;
		float delta=_TERRAIN_HeightMap3_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH/length(EyeDirTan.xy);
		EyeDirTan*=delta;
		bool height;
		
		float dh_prev=0;
		float h_prev=1.001;
		float _h;
		
		float shadow_atten=1;
		#endif
		
		#ifndef _4LAYERS 		
		#ifdef RTP_HARD_CROSSPASS
		 	if (false) {
	 	#else
		 	if (splat_controlA_coverage>0.01 && splat_controlB_coverage>0.01) {
	 	#endif
	 		//////////////////////////////////////////////
	 		//
	 		// splats 0-7 close combined
	 		//
	 		///////////////////////////////////////////////
	 		#ifdef RTP_SHOW_OVERLAPPED
	 		o.Emission.r=1;
	 		#endif

	 		#ifdef RTP_POM_SHADING
				if (DAMP_COLOR_VAL>0) {
				for(int i=0; i<_TERRAIN_DIST_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
			 		float4 tH1, tH2;
					#ifdef USE_EXTRUDE_REDUCTION
						tH1=lerp(tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER89AB);
						tH2=lerp(tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER4567);
					#else
						tH1=tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww);
						tH2=tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww);
					#endif
					_h=saturate(dot(splat_control1, tH1)+dot(splat_control2, tH2));
					hit_flag=_h >= rayPos.z;
					if (hit_flag) break; 
					h_prev=_h;
					dh_prev = rayPos.z - _h;
				}
				}
	
				if (hit_flag) {
					// secant search - 2 steps
					float scl=dh_prev / ((_h-h_prev) - EyeDirTan.z);
					rayPos.xyz-=EyeDirTan*(1 - scl); // back
			 		float4 tH1, tH2;
					#ifdef USE_EXTRUDE_REDUCTION
						tH1=lerp(tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER89AB);
						tH2=lerp(tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER4567);
					#else
						tH1=tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww);
						tH2=tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww);
					#endif
					float _nh=saturate(dot(splat_control1, tH1)+dot(splat_control2, tH2));
					if (_nh >= rayPos.z) {
						EyeDirTan*=scl;
						scl=dh_prev / ((_nh-h_prev) - EyeDirTan.z);
						rayPos.xyz-=EyeDirTan*(1 - scl); // back
					} else {
						EyeDirTan*=(1-scl);
						dh_prev = rayPos.z - _nh;
						scl=dh_prev / ((_h-_nh) - EyeDirTan.z);
						rayPos.xyz+=EyeDirTan*scl; // forth
					}
				}
				#ifdef RTP_WETNESS
					actH=rayPos.z;
				#endif
			#else
				#ifdef RTP_PM_SHADING
					rayPos.xy += ParallaxOffset(dot(splat_control1, tHA)+dot(splat_control2, tHB), _TERRAIN_ExtrudeHeight*_uv_Relief_z*DAMP_COLOR_VAL, IN_viewDir.xyz);
				#endif
				#ifdef RTP_WETNESS
					actH=dot(splat_control1, tHA)+dot(splat_control2, tHB);
				#endif
			#endif
			
			////////////////////////////////
			// water
			//
			float4 water_splat_control1=splat_control1;
			float4 water_splat_control2=splat_control2;
 			#if defined(RTP_REFLECTION) 
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection89AB)+dot(splat_control2, TERRAIN_LayerReflection4567)*_uv_Relief_z;
				#else
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection89AB)+dot(splat_control2, TERRAIN_LayerReflection4567);
				#endif
 			#endif
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=(dot(water_splat_control1, TERRAIN_WetReflection89AB)+dot(water_splat_control2, TERRAIN_WetReflection4567))*_uv_Relief_z;
				#else
					TERRAIN_WetReflection=dot(water_splat_control1, TERRAIN_WetReflection89AB)+dot(water_splat_control2, TERRAIN_WetReflection4567);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlA_normalized, TERRAIN_LayerWetStrength89AB)+dot(splat_controlB_normalized, TERRAIN_LayerWetStrength4567);
				TERRAIN_WetSpecularity=dot(water_splat_control1, TERRAIN_WetSpecularity89AB)+dot(water_splat_control2, TERRAIN_WetSpecularity4567);
				TERRAIN_WaterGloss=dot(water_splat_control1, TERRAIN_WaterGloss89AB)+dot(water_splat_control2, TERRAIN_WaterGloss4567);
				TERRAIN_WaterColor=half4( dot(splat_controlA_normalized, TERRAIN_WaterColorR89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorR4567), dot(splat_controlA_normalized, TERRAIN_WaterColorG89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorG4567), dot(splat_controlA_normalized, TERRAIN_WaterColorB89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorB4567), dot(splat_controlA_normalized, TERRAIN_WaterColorA89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorA4567) );
				
				float TERRAIN_WaterLevel=dot(water_splat_control1, TERRAIN_WaterLevel89AB)+dot(water_splat_control2, TERRAIN_WaterLevel4567);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlA_normalized, TERRAIN_WaterLevelSlopeDamp89AB)+dot(splat_controlB_normalized, TERRAIN_WaterLevelSlopeDamp4567);
				float TERRAIN_Flow=dot(water_splat_control1, TERRAIN_Flow89AB)+dot(water_splat_control2, TERRAIN_Flow4567);
				float TERRAIN_WaterEdge=dot(water_splat_control1, TERRAIN_WaterEdge89AB)+dot(water_splat_control2, TERRAIN_WaterEdge4567);
				float TERRAIN_Refraction=dot(water_splat_control1, TERRAIN_Refraction89AB)+dot(water_splat_control2, TERRAIN_Refraction4567);
				float TERRAIN_WetRefraction=dot(water_splat_control1, TERRAIN_WetRefraction89AB)+dot(water_splat_control2, TERRAIN_WetRefraction4567);
				
				TERRAIN_LayerWetStrength*=saturate(2- water_mask*2-perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
				#ifdef RTP_SNOW
				TERRAIN_LayerWetStrength*=saturate(1-snow_val);
				#endif
				float2 roff=0;
				if (TERRAIN_LayerWetStrength>0) {
					wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
					float _RippleDamp=saturate(TERRAIN_LayerWetStrength*2-1)*saturate(1-wetSlope*4)*_uv_Relief_z;
					TERRAIN_RainIntensity*=_RippleDamp;
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
					TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH);
					TERRAIN_Flow*=TERRAIN_LayerWetStrength*TERRAIN_LayerWetStrength;
					
					p = saturate((TERRAIN_WaterLevel-actH)*TERRAIN_WaterEdge);
					p*=p;
					#if !defined(RTP_SIMPLE_SHADING) && !defined(SIMPLE_WATER)
						float2 flowUV=lerp(IN._uv_Relief.xy, rayPos.xy, 1-p*0.5)*TERRAIN_FlowScale;
						float _Tim=frac(_Time.x*4)*2;
						float ft=abs(frac(_Tim)*2 - 1);
						float2 flowSpeed=clamp((IN._viewDir.zw+0.01)*4,-1,1)/4;
						flowSpeed*=TERRAIN_FlowSpeed*TERRAIN_FlowScale;
						float rtp_mipoffset_add = (1-saturate(dot(flowSpeed, flowSpeed)*TERRAIN_mipoffset_flowSpeed))*TERRAIN_mipoffset_flowSpeed;
						rtp_mipoffset_add+=(1-TERRAIN_LayerWetStrength)*8;
						float2 flowOffset=tex2Dlod(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx)*flowSpeed, mip_selector+rtp_mipoffset_flow+rtp_mipoffset_add)).rg*2-1;
						flowOffset=lerp(flowOffset, tex2Dlod(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.1, mip_selector+rtp_mipoffset_add)).rg*2-1, ft);
						// stały przepływ na płaskim
						//float slowMotionFct=dot(flowSpeed,flowSpeed);
						//slowMotionFct=saturate(slowMotionFct*50);
						//flowOffset=lerp(tex2Dlod(_BumpMapGlobal, float4(flowUV+float2(0,2*_Time.x*TERRAIN_FlowSpeed*TERRAIN_FlowScale), mip_selector+rtp_mipoffset_flow)).rg*2-1, flowOffset, slowMotionFct );
						//
						flowOffset*=TERRAIN_Flow*max(p, saturate(TERRAIN_WetSpecularity))*_uv_Relief_z*TERRAIN_LayerWetStrength;
					#else
						float2 flowOffset=0;
					#endif
					
					#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
						float2 rippleUV = IN._uv_Relief.xy*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
					  	//roff = GetRipple( float4(rippleUV, mip_selector + rtp_mipoffset_ripple), TERRAIN_RainIntensity);
						//roff += GetRipple( float4(rippleUV+float2(0.25,0.25), mip_selector + rtp_mipoffset_ripple ), TERRAIN_RainIntensity);
					    float4 Ripple;
					  	{
					  	 	Ripple = tex2Dlod(TERRAIN_RippleMap, float4(rippleUV, mip_selector + rtp_mipoffset_ripple));
						    Ripple.xy = Ripple.xy * 2 - 1;
						
						    float DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
						    float TimeFrac = DropFrac - 1.0f + Ripple.z;
						    float DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
						    float FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
						  	roff = Ripple.xy * FinalFactor * 0.35f;
						  	
						  	rippleUV+=float2(0.25,0.25);
					  	 	Ripple = tex2Dlod(TERRAIN_RippleMap, float4(rippleUV, mip_selector + rtp_mipoffset_ripple));
						    Ripple.xy = Ripple.xy * 2 - 1;
						
						    DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
						    TimeFrac = DropFrac - 1.0f + Ripple.z;
						    DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
						    FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
						  	roff += Ripple.xy * FinalFactor * 0.35f;
					  	}
					  	roff*=4*_RippleDamp*lerp(TERRAIN_WetDropletsStrength, 1, p);
					  	roff+=flowOffset;
					#else
						roff = flowOffset;
					#endif
					
					#if !defined(RTP_SIMPLE_SHADING)
						rayPos.xy+=TERRAIN_Refraction*roff*max(p, TERRAIN_WetRefraction);
					#endif
				}
			#endif
			// water
			////////////////////////////////			
									
			uvSplat01=frac(rayPos.xy).xyxy*_mult+_off;
			uvSplat01.zw+=float2(0.5,0);
			uvSplat23=uvSplat01.xyxy+float4(0,0.5,0.5,0.5);
			
//#ifdef RTP_SNOW
//if (snow_MayBeNotFullyCovered_flag) {
//#endif		 	

			float4 c;
			float4 gloss;
			float _MipActual=min(mip_selector.x+rtp_mipoffset_color,6);
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, _MipActual.xx)); col = splat_control1.x * c.rgb; gloss.r = c.a;
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, _MipActual.xx)); col += splat_control1.y * c.rgb; gloss.g = c.a;
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, _MipActual.xx)); col += splat_control1.z * c.rgb; gloss.b = c.a;
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, _MipActual.xx)); col += splat_control1.w * c.rgb; gloss.a = c.a;
			
			float hbAO=(dot(splat_control1,RTP_AO_89AB)+dot(splat_control2,RTP_AO_4567))*RTP_AOamp*2;
			float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*hbAO;
			col*=heightblend_AO;
			gloss*=heightblend_AO;
						
			o.Gloss = dot(gloss, splat_control1);
			o.Specular = 0.03+dot(_Spec89AB, splat_control1);
			#ifdef RTP_UV_BLEND
			#ifndef RTP_DISTANCE_ONLY_UV_BLEND
				float4 _MixMipActual=min(_MipActual.xxxx+log2(_MixScale89AB), 6);
				float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
				float4 _multMix89AB=_multMix-hi_mip_adjustMix; // nie chcemy nadpisać tego bo poniżej potrzeba nam tego dla kanałów 4-7
				float4 _offMix89AB=_offMix+0.5*hi_mip_adjustMix;
			
				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale89AB.xxyy)*_multMix89AB+_offMix89AB;
				uvSplat01M.zw+=float2(0.5,0);
				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale89AB.zzww)*_multMix89AB+_offMix89AB;
				uvSplat23M+=float4(0,0.5,0.5,0.5);			
				
				half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
				colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
				colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
				colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
			#endif
			#endif

			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, _MipActual.xx)); col += splat_control2.x * c.rgb; gloss.r = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, _MipActual.xx)); col += splat_control2.y * c.rgb; gloss.g = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, _MipActual.xx)); col += splat_control2.z * c.rgb; gloss.b = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, _MipActual.xx)); col += splat_control2.w * c.rgb; gloss.a = c.a;
			
			heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control2-0.5)*(2+RTP_AOsharpness)),0.25))*hbAO;
			col*=heightblend_AO;
			gloss*=heightblend_AO;
						
			o.Gloss += dot(gloss, splat_control2);
			o.Specular += dot(_Spec4567, splat_control2);
			#ifdef RTP_UV_BLEND
			#ifndef RTP_DISTANCE_ONLY_UV_BLEND
//				_MixMipActual=min(_MipActual.xxxx + log2(_MixScale4567), 6);
//				hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
//				_multMix-=hi_mip_adjustMix;
//				_offMix+=0.5*hi_mip_adjustMix;
//			
//				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale4567.xxyy)*_multMix+_offMix;
//				uvSplat01M.zw+=float2(0.5,0);
//				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale4567.zzww)*_multMix+_offMix;
//				uvSplat23M+=float4(0,0.5,0.5,0.5);			
				
				colBlend += splat_control2.x * UV_BLEND_ROUTE_LAYER_4;
				colBlend += splat_control2.y * UV_BLEND_ROUTE_LAYER_5;
				colBlend += splat_control2.z * UV_BLEND_ROUTE_LAYER_6;
				colBlend += splat_control2.w * UV_BLEND_ROUTE_LAYER_7;
			#endif
			#endif
			
			#if defined(RTP_UV_BLEND)
				#ifndef RTP_DISTANCE_ONLY_UV_BLEND			
					colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control1, _MixSaturation89AB) + dot(splat_control2, _MixSaturation4567));
					col=lerp(col, col*colBlend*2, blendVal);			
				#endif
			#endif			
		 	
			#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
				col=lerp(col, global_color_value.rgb, _uv_Relief_w);
			#endif
						
			float3 n;
			float4 normals_combined;
			rayPos.w=mip_selector.x+rtp_mipoffset_bump;
			#ifdef RTP_SNOW
				rayPos.w += snow_depth;
			#endif				
			normals_combined = tex2Dlod(_BumpMap89, rayPos.xyww).rgba*splat_control1.rrgg;
			normals_combined+=tex2Dlod(_BumpMapAB, rayPos.xyww).rgba*splat_control1.bbaa;
			normals_combined+=tex2Dlod(_BumpMap45, rayPos.xyww).rgba*splat_control2.rrgg;
			normals_combined+=tex2Dlod(_BumpMap67, rayPos.xyww).rgba*splat_control2.bbaa;
			n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
			n.xy*=_uv_Relief_z;
			n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
			o.Normal=n;
	        			
			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatC0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control1, _VerticalTexture89AB) + dot(splat_control2, _VerticalTexture4567));
			#endif
						
			////////////////////////////////
			// water
			//
	        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
		        GlossDry=o.Gloss;
		    #endif
	        #if defined(RTP_WETNESS)
				#ifdef RTP_CAUSTICS
					TERRAIN_WetSpecularity*=1-damp_fct_caustics;
				#endif	        
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=(dot(splat_controlA_normalized, TERRAIN_WaterOpacity89AB)+dot(splat_controlB_normalized, TERRAIN_WaterOpacity4567))*p; // gladsze przejscie (po nieskompresowanym splat_control)

				#ifdef RTP_POM_SHADING
		 		#if defined(RTP_SOFT_SHADOWS) || defined(RTP_HARD_SHADOWS)
	 				shadow_atten=lerp(shadow_atten, 1, _WaterOpacity);
				#endif
				#endif
	 					        
		        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        o.Normal.xy+=roff;
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*TERRAIN_WetDarkening;
	        #endif
			// water
			////////////////////////////////

			#if defined(RTP_SUPER_DETAIL) && !defined(RTP_SIMPLE_SHADING)
				float4 super_detail=tex2Dlod(_BumpMapGlobal, float4(rayPos.xy*_SuperDetailTiling, mip_selector + rtp_mipoffset_superdetail));
				float3 super_detail_norm;
				super_detail_norm.xy = (super_detail.xy*4-2)*(dot(0.8,col.rgb)+0.4)+o.Normal.xy;
				super_detail_norm.z = sqrt(1 - saturate(dot(super_detail_norm.xy, super_detail_norm.xy)));
				super_detail_norm=normalize(super_detail_norm);
				float sdVal=_uv_Relief_z*(dot(splat_control1, _SuperDetailStrengthNormal89AB)+dot(splat_control2, _SuperDetailStrengthNormal4567));
				#if defined(RTP_SNOW)
					sdVal*=saturate(1-snow_depth);
				#endif
				o.Normal=lerp(o.Normal, super_detail_norm, sdVal);				
				#if defined(RTP_SUPER_DTL_MULTS) && !defined(RTP_WETNESS) && !defined(RTP_REFLECTION)
					float near_blend;
					float far_blend;
					float near_far_blend_dist=saturate(_uv_Relief_z-0.5)*2;
					near_blend=lerp(1, global_bump_val.b, dot(splat_control1, _SuperDetailStrengthMultASelfMaskNear89AB) + dot(splat_control2, _SuperDetailStrengthMultASelfMaskNear4567));
					far_blend=lerp(0, global_bump_val.b, dot(splat_control1, _SuperDetailStrengthMultASelfMaskFar89AB) + dot(splat_control2, _SuperDetailStrengthMultASelfMaskFar4567));
					col=lerp(col, col*super_detail.z*2, lerp(far_blend, near_blend, near_far_blend_dist)*(dot(splat_control1, _SuperDetailStrengthMultA89AB)+dot(splat_control2, _SuperDetailStrengthMultA4567)));
					near_blend=lerp(1, global_bump_val.a, dot(splat_control1, _SuperDetailStrengthMultBSelfMaskNear89AB) + dot(splat_control2, _SuperDetailStrengthMultBSelfMaskNear4567));
					far_blend=lerp(0, global_bump_val.a, dot(splat_control1, _SuperDetailStrengthMultBSelfMaskFar89AB) + dot(splat_control2, _SuperDetailStrengthMultBSelfMaskFar4567));
					col=lerp(col, col*super_detail.w*2, lerp(far_blend, near_blend, near_far_blend_dist)*(dot(splat_control1, _SuperDetailStrengthMultB89AB)+dot(splat_control2, _SuperDetailStrengthMultB4567)));
				#endif
			#endif

//#ifdef RTP_SNOW
//}
//#endif
			// snow color
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7) )
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif			
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif			
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif				
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif			
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif				
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color, 6)));
				GETrtp_snow_TEX
			#endif		
			#endif
			// snow color

			IN_uv_Relief_Offset.xy=rayPos.xy;
			
		 	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		 	//
		 	// self shadowing 
		 	//
	 		#if defined(RTP_POM_SHADING) && !defined(RTP_TRIPLANAR)
	 		#if defined(RTP_SOFT_SHADOWS) || defined(RTP_HARD_SHADOWS)
	 			#ifdef RTP_SNOW
	 				rayPos.w=mip_selector.x+rtp_mipoffset_height+snow_depth;
	 			#endif

				EyeDirTan=IN.lightDir.xyz;
				EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*DAMP_COLOR_VAL);
				delta=_TERRAIN_HeightMap3_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH_SHADOWS/length(EyeDirTan.xy);
				h_prev=rayPos.z;
				//rayPos.xyz+=EyeDirTan*_TERRAIN_HeightMap3_TexelSize.x*2;
				EyeDirTan*=delta;

				hit_flag=false;
				dh_prev=0;
				//_TERRAIN_SHADOW_STEPS=min(_TERRAIN_SHADOW_STEPS, ((EyeDirTan.z>0) ? (1-rayPos.z) : rayPos.z) / abs(EyeDirTan.z));
				for(int i=0; i<_TERRAIN_SHADOW_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
					_h=dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww))+dot(splat_control2, tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww));
					hit_flag=_h >= rayPos.z;
					if (hit_flag) break;
					h_prev=_h;
					dh_prev = rayPos.z - _h;
				}
				
				#ifdef RTP_SOFT_SHADOWS
					if (hit_flag) {
						// secant search
						float scl=dh_prev / ((_h-h_prev) - EyeDirTan.z);
						rayPos.xyz-=EyeDirTan*(1 - scl); // back
						EyeDirTan=IN.lightDir.xyz*_TERRAIN_HeightMap3_TexelSize.x*2*_TERRAIN_WAVELENGTH_SHADOWS;
						EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*DAMP_COLOR_VAL);
						float smooth_val=0;
						float break_val=_TERRAIN_ExtrudeHeight*_TERRAIN_ShadowSmoothing;
						for(int i=0; i<_TERRAIN_SHADOW_SMOOTH_STEPS; i++) {
							rayPos.xyz+=EyeDirTan;
							float d=dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww))+dot(splat_control2, tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww)) - rayPos.z;
							smooth_val+=saturate(d);
							if (smooth_val>break_val) break;
						}
						shadow_atten=saturate(1-smooth_val/break_val);
					}
				#else
					shadow_atten=hit_flag ? 0 : shadow_atten;
				#endif
		
				shadow_atten=shadow_atten*_TERRAIN_SelfShadowStrength+(1-_TERRAIN_SelfShadowStrength);
				#ifdef RTP_SNOW
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*DAMP_COLOR_VAL*(1-snow_depth_lerp));
				#else
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*DAMP_COLOR_VAL);
				#endif
			#endif
			#endif		
			//
		 	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
		 	
		 	// end of splats 0-7 close combined			
	 	} else if (splat_controlA_coverage>splat_controlB_coverage)
	 	#endif // !_4LAYERS 		
	 	{
	 		//////////////////////////////////
	 		//
	 		// splats 0-3 close
	 		//
	 		//////////////////////////////////
	 		
			#ifdef RTP_HARD_CROSSPASS
				splat_control1 /= dot(splat_control1, 1);
			#endif

	 		#if defined(RTP_POM_SHADING) && !defined(RTP_TRIPLANAR)
				if (DAMP_COLOR_VAL>0) {
				for(int i=0; i<_TERRAIN_DIST_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
			 		float4 tH;
					#ifdef USE_EXTRUDE_REDUCTION
						tH=lerp(tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER89AB);
					#else
						tH=tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww);
					#endif	
					_h=saturate(dot(splat_control1, tH));
					hit_flag=_h >= rayPos.z;
					if (hit_flag) break;
					h_prev=_h;
					dh_prev = rayPos.z - _h;
				}
				}
							
				if (hit_flag) {
					// secant search - 2 steps
					float scl=dh_prev / ((_h-h_prev) - EyeDirTan.z);
					rayPos.xyz-=EyeDirTan*(1 - scl); // back
			 		float4 tH;
					#ifdef USE_EXTRUDE_REDUCTION
						tH=lerp(tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER89AB);
					#else
						tH=tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww);
					#endif	
					float _nh=saturate(dot(splat_control1, tH));
					if (_nh >= rayPos.z) {
						EyeDirTan*=scl;
						scl=dh_prev / ((_nh-h_prev) - EyeDirTan.z);
						rayPos.xyz-=EyeDirTan*(1 - scl); // back
					} else {
						EyeDirTan*=(1-scl);
						dh_prev = rayPos.z - _nh;
						scl=dh_prev / ((_h-_nh) - EyeDirTan.z);
						rayPos.xyz+=EyeDirTan*scl; // forth
					}
				}
				#ifdef RTP_WETNESS
					actH=rayPos.z;
				#endif
			#else
		 		#ifdef RTP_TRIPLANAR
		 				float hgtXZ, hgtXY, hgtYZ, hgtTRI;
		 				if (triplanar_blend_flag) {
							#ifdef USE_EXTRUDE_REDUCTION
					 			hgtYZ = triplanar_blend.x * dot(splat_control1, lerp(tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.yz), 1, PER_LAYER_HEIGHT_MODIFIER89AB));
					 			hgtXY = triplanar_blend.y * dot(splat_control1, lerp(tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.xy), 1, PER_LAYER_HEIGHT_MODIFIER89AB));
					 			hgtXZ = triplanar_blend.z * dot(splat_control1, lerp(tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.xz), 1, PER_LAYER_HEIGHT_MODIFIER89AB));
							#else
					 			hgtYZ = triplanar_blend.x * dot(splat_control1, tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.yz));
					 			hgtXY = triplanar_blend.y * dot(splat_control1, tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.xy));
					 			hgtXZ = triplanar_blend.z * dot(splat_control1, tex2D(_TERRAIN_HeightMap3, IN._uv_Relief.xz));
							#endif
							#ifdef RTP_WETNESS
								actH=lerp(actH, hgtYZ + hgtXY + hgtXZ, _uv_Relief_z) ;
							#endif
		 				} else {
		 					// no blend case
							#ifdef USE_EXTRUDE_REDUCTION
					 			hgtTRI = dot(splat_control1, lerp(tex2Dlod(_TERRAIN_HeightMap3, uvTRI.xyzz), 1, PER_LAYER_HEIGHT_MODIFIER89AB));
							#else
					 			hgtTRI = dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap3, uvTRI.xyzz));
							#endif
			 				//hgtTRI*=triplanar_blend_simple;
							#ifdef RTP_WETNESS
								actH=lerp(actH, hgtTRI, _uv_Relief_z);
							#endif
		 				}
				#else
					#if defined(RTP_PM_SHADING)
						rayPos.xy += ParallaxOffset(dot(splat_control1, tHA), _TERRAIN_ExtrudeHeight*_uv_Relief_z*DAMP_COLOR_VAL, IN_viewDir.xyz);
					#endif
					#ifdef RTP_WETNESS
						actH=dot(splat_control1, tHA);
					#endif
				#endif				
			#endif
					
			////////////////////////////////
			// water
			//
			float4 water_splat_control=splat_control1;
 			#if defined(RTP_REFLECTION) 
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection89AB)*_uv_Relief_z;
				#else
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection89AB);
				#endif
 			#endif
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection89AB)*_uv_Relief_z;
				#else
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection89AB);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlA_normalized, TERRAIN_LayerWetStrength89AB);
				TERRAIN_WetSpecularity=dot(water_splat_control, TERRAIN_WetSpecularity89AB);
				TERRAIN_WaterGloss=dot(water_splat_control, TERRAIN_WaterGloss89AB);
				TERRAIN_WaterColor=half4( dot(splat_controlA_normalized, TERRAIN_WaterColorR89AB), dot(splat_controlA_normalized, TERRAIN_WaterColorG89AB), dot(splat_controlA_normalized, TERRAIN_WaterColorB89AB), dot(splat_controlA_normalized, TERRAIN_WaterColorA89AB) );
				
				float TERRAIN_WaterLevel=dot(water_splat_control, TERRAIN_WaterLevel89AB);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlA_normalized, TERRAIN_WaterLevelSlopeDamp89AB);
				float TERRAIN_Flow=dot(water_splat_control, TERRAIN_Flow89AB);
				float TERRAIN_WaterEdge=dot(water_splat_control, TERRAIN_WaterEdge89AB);
				float TERRAIN_Refraction=dot(water_splat_control, TERRAIN_Refraction89AB);
				float TERRAIN_WetRefraction=dot(water_splat_control, TERRAIN_WetRefraction89AB);
				
				TERRAIN_LayerWetStrength*=saturate(2- water_mask*2-perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
				#ifdef RTP_SNOW
				TERRAIN_LayerWetStrength*=saturate(1-snow_val);
				#endif
				float2 roff=0;
				float2 flowOffset	=0;
				if (TERRAIN_LayerWetStrength>0) {
					wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
					float _RippleDamp=saturate(TERRAIN_LayerWetStrength*2-1)*saturate(1-wetSlope*4)*_uv_Relief_z;
					TERRAIN_RainIntensity*=_RippleDamp;
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
					TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH);
					TERRAIN_Flow*=TERRAIN_LayerWetStrength*TERRAIN_LayerWetStrength;
					
					p = saturate((TERRAIN_WaterLevel-actH)*TERRAIN_WaterEdge);
					p*=p;
					#if !defined(RTP_SIMPLE_SHADING) && !defined(SIMPLE_WATER)
						float2 flowUV=lerp(IN._uv_Relief.xy, rayPos.xy, 1-p*0.5)*TERRAIN_FlowScale;
						float _Tim=frac(_Time.x*4)*2;
						float ft=abs(frac(_Tim)*2 - 1);
						float2 flowSpeed=clamp((IN._viewDir.zw+0.01)*4,-1,1)/4;
						flowSpeed*=TERRAIN_FlowSpeed*TERRAIN_FlowScale;
						float rtp_mipoffset_add = (1-saturate(dot(flowSpeed, flowSpeed)*TERRAIN_mipoffset_flowSpeed))*TERRAIN_mipoffset_flowSpeed;
						rtp_mipoffset_add+=(1-TERRAIN_LayerWetStrength)*8;
						flowOffset=tex2Dlod(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx)*flowSpeed, mip_selector+rtp_mipoffset_flow+rtp_mipoffset_add)).rg*2-1;
						flowOffset=lerp(flowOffset, tex2Dlod(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.1, mip_selector+rtp_mipoffset_flow+rtp_mipoffset_add)).rg*2-1, ft);
						// stały przepływ na płaskim
						//float slowMotionFct=dot(flowSpeed,flowSpeed);
						//slowMotionFct=saturate(slowMotionFct*50);
						//flowOffset=lerp(tex2Dlod(_BumpMapGlobal, float4(flowUV+float2(0,2*_Time.x*TERRAIN_FlowSpeed*TERRAIN_FlowScale), mip_selector+rtp_mipoffset_flow)).rg*2-1, flowOffset, slowMotionFct );
						//
						flowOffset*=TERRAIN_Flow*max(p, saturate(TERRAIN_WetSpecularity))*_uv_Relief_z*TERRAIN_LayerWetStrength;
					#endif
					
					#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
						float2 rippleUV = IN._uv_Relief.xy*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
					  	//roff = GetRipple( float4(rippleUV, mip_selector + rtp_mipoffset_ripple), TERRAIN_RainIntensity);
						//roff += GetRipple( float4(rippleUV+float2(0.25,0.25), mip_selector + rtp_mipoffset_ripple ), TERRAIN_RainIntensity);
					    float4 Ripple;
					  	{
					  	 	Ripple = tex2Dlod(TERRAIN_RippleMap, float4(rippleUV, mip_selector + rtp_mipoffset_ripple));
						    Ripple.xy = Ripple.xy * 2 - 1;
						
						    float DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
						    float TimeFrac = DropFrac - 1.0f + Ripple.z;
						    float DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
						    float FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
						  	roff = Ripple.xy * FinalFactor * 0.35f;
						  	
						  	rippleUV+=float2(0.25,0.25);
					  	 	Ripple = tex2Dlod(TERRAIN_RippleMap, float4(rippleUV, mip_selector + rtp_mipoffset_ripple));
						    Ripple.xy = Ripple.xy * 2 - 1;
						
						    DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
						    TimeFrac = DropFrac - 1.0f + Ripple.z;
						    DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
						    FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
						  	roff += Ripple.xy * FinalFactor * 0.35f;
					  	}
						
					  	roff*=4*_RippleDamp*lerp(TERRAIN_WetDropletsStrength, 1, p);
					  	roff+=flowOffset;
					#else
						roff = flowOffset;
					#endif
					
					#if !defined(RTP_SIMPLE_SHADING)
						flowOffset=TERRAIN_Refraction*roff*max(p, TERRAIN_WetRefraction);
						#if !defined(RTP_TRIPLANAR)
							rayPos.xy+=flowOffset;
						#endif
					#endif
				}
			#endif
			// water
			///////////////////////////////////////////
			
			#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
				uvSplat01=frac(rayPos.xy).xyxy*_mult+_off;
				uvSplat01.zw+=float2(0.5,0);
				uvSplat23=uvSplat01.xyxy+float4(0,0.5,0.5,0.5);
			#endif			
			
//#ifdef RTP_SNOW
//if (snow_MayBeNotFullyCovered_flag) {
//#endif		 			
	 		
			float4 c;
			float4 gloss=0;
			
			#ifdef RTP_TRIPLANAR
				//
				// triplanar
				//
				if (triplanar_blend_flag) {
					//
					// triplanar blend case
					//
					float4 normals_combined;
					float3 nA,nB,nC;
					nA=nB=nC=float3(0,0,1);
										
					#ifdef RTP_WETNESS
						float3 uvTRI1=float3(IN._uv_Relief.yz+flowOffset, mip_selectorTRIPLANAR.x+rtp_mipoffset_color);
						float3 uvTRI2=float3(IN._uv_Relief.xy+flowOffset, mip_selector.x+rtp_mipoffset_color);
						float3 uvTRI3=float3(IN._uv_Relief.xz+flowOffset, mip_selectorTRIPLANAR.y+rtp_mipoffset_color);
					#else
						float3 uvTRI1=float3(IN._uv_Relief.yz, mip_selectorTRIPLANAR.x+rtp_mipoffset_color);
						float3 uvTRI2=float3(IN._uv_Relief.xy, mip_selector.x+rtp_mipoffset_color);
						float3 uvTRI3=float3(IN._uv_Relief.xz, mip_selectorTRIPLANAR.y+rtp_mipoffset_color);
					#endif
					
					if (triplanar_blend.x>0.05) {
						float4 _MixBlendtmp=splat_control1*triplanar_blend.x;
						float4 tmp_gloss;
						float3 dir=IN_viewDir.yxz;
						dir.y=triplanar_flip.x ? dir.y:-dir.y;
						#if defined(RTP_PM_SHADING)
							uvTRI1.xy+= ParallaxOffset(hgtYZ, _TERRAIN_ExtrudeHeight*_uv_Relief_z*DAMP_COLOR_VAL, dir);
						#endif
						c = tex2Dlod(_SplatC0, uvTRI1.xyzz); col += _MixBlendtmp.x * c.rgb; tmp_gloss.r=c.a;
						c = tex2Dlod(_SplatC1, uvTRI1.xyzz); col += _MixBlendtmp.y * c.rgb; tmp_gloss.g=c.a;
						c = tex2Dlod(_SplatC2, uvTRI1.xyzz); col += _MixBlendtmp.z * c.rgb; tmp_gloss.b=c.a;
						c = tex2Dlod(_SplatC3, uvTRI1.xyzz); col += _MixBlendtmp.w * c.rgb; tmp_gloss.a=c.a;
						gloss+=triplanar_blend.x*tmp_gloss;
						
						#ifdef RTP_SNOW
							uvTRI1.z += snow_depth;
						#endif							
						
						normals_combined = tex2Dlod(_BumpMap89, uvTRI1.xyzz).grab*splat_control1.rrgg;  // x<-> y
						normals_combined+=tex2Dlod(_BumpMapAB, uvTRI1.xyzz).grab*splat_control1.bbaa;
						nA.xy=(normals_combined.rg+normals_combined.ba)*2-1;
						nA.x=triplanar_flip.x ? nA.x:-nA.x;
						nA.xy*=_uv_Relief_z;
						nA.z = sqrt(1 - saturate(dot(nA.xy, nA.xy)));
					}
					if (triplanar_blend.y>0.05) {
						float4 _MixBlendtmp=splat_control1*triplanar_blend.y;
						float4 tmp_gloss;
						#if defined(RTP_PM_SHADING)
							uvTRI2.xy+= ParallaxOffset(hgtXY, _TERRAIN_ExtrudeHeight*_uv_Relief_z*DAMP_COLOR_VAL, IN_viewDir.xyz);
						#endif
						c = tex2Dlod(_SplatC0, uvTRI2.xyzz); col += _MixBlendtmp.x * c.rgb; tmp_gloss.r=c.a;
						c = tex2Dlod(_SplatC1, uvTRI2.xyzz); col += _MixBlendtmp.y * c.rgb; tmp_gloss.g=c.a;
						c = tex2Dlod(_SplatC2, uvTRI2.xyzz); col += _MixBlendtmp.z * c.rgb; tmp_gloss.b=c.a;
						c = tex2Dlod(_SplatC3, uvTRI2.xyzz); col += _MixBlendtmp.w * c.rgb; tmp_gloss.a=c.a;
						gloss+=triplanar_blend.y*tmp_gloss;
						
						#ifdef RTP_SNOW
							uvTRI2.z += snow_depth;
						#endif							
						
						normals_combined = tex2Dlod(_BumpMap89, uvTRI2.xyzz).rgba*splat_control1.rrgg;
						normals_combined+=tex2Dlod(_BumpMapAB, uvTRI2.xyzz).rgba*splat_control1.bbaa;
						nB.xy=(normals_combined.rg+normals_combined.ba)*2-1;
						nB.xy*=_uv_Relief_z;
						nB.z = sqrt(1 - saturate(dot(nB.xy, nB.xy)));
					}
					if (triplanar_blend.z>0.05) {
						float4 _MixBlendtmp=splat_control1*triplanar_blend.z;
						float4 tmp_gloss;
						float3 dir=IN_viewDir.xyz;
						dir.y=triplanar_flip.y ? dir.y:-dir.y;
						#if defined(RTP_PM_SHADING)
							uvTRI3.xy+= ParallaxOffset(hgtXZ, _TERRAIN_ExtrudeHeight*_uv_Relief_z*DAMP_COLOR_VAL, dir);
						#endif
						c = tex2Dlod(_SplatC0, uvTRI3.xyzz); col += _MixBlendtmp.x * c.rgb; tmp_gloss.r=c.a;
						c = tex2Dlod(_SplatC1, uvTRI3.xyzz); col += _MixBlendtmp.y * c.rgb; tmp_gloss.g=c.a;
						c = tex2Dlod(_SplatC2, uvTRI3.xyzz); col += _MixBlendtmp.z * c.rgb; tmp_gloss.b=c.a;
						c = tex2Dlod(_SplatC3, uvTRI3.xyzz); col += _MixBlendtmp.w * c.rgb; tmp_gloss.a=c.a;
						gloss+=triplanar_blend.z*tmp_gloss;
						
						#ifdef RTP_SNOW
							uvTRI3.z += snow_depth;
						#endif			
										
						normals_combined = tex2Dlod(_BumpMap89, uvTRI3.xyzz).rgba*splat_control1.rrgg;
						normals_combined+=tex2Dlod(_BumpMapAB, uvTRI3.xyzz).rgba*splat_control1.bbaa;
						nC.xy=(normals_combined.rg+normals_combined.ba)*2-1;
						nC.y=triplanar_flip.y ? nC.y:-nC.y;
						nC.xy*=_uv_Relief_z;
						nC.z = sqrt(1 - saturate(dot(nC.xy, nC.xy)));
					}	
					float3 n=(triplanar_blend.x * nA + triplanar_blend.y * nB + triplanar_blend.z * nC);
					
					float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*RTP_AOamp*dot(splat_control1,RTP_AO_89AB);
					col*=heightblend_AO;
					gloss*=heightblend_AO;
															
					o.Normal=n;
				} else {
					//
					// triplanar no blend - simple case
					//
					#ifdef RTP_WETNESS
						uvTRI.xy+=flowOffset;
					#endif
					#if defined(RTP_PM_SHADING)
						uvTRI.xy+= ParallaxOffset(hgtTRI, _TERRAIN_ExtrudeHeight*_uv_Relief_z*DAMP_COLOR_VAL, dirTRI);
					#endif
					float3 uvTRI_tmp=float3(uvTRI.xy, uvTRI.z+rtp_mipoffset_color);
					float4 tmp_gloss;
					c = tex2Dlod(_SplatC0, uvTRI_tmp.xyzz)*splat_control1.x; col += c.rgb; tmp_gloss.r=c.a;
					c = tex2Dlod(_SplatC1, uvTRI_tmp.xyzz)*splat_control1.y; col += c.rgb; tmp_gloss.g=c.a;
					c = tex2Dlod(_SplatC2, uvTRI_tmp.xyzz)*splat_control1.z; col += c.rgb; tmp_gloss.b=c.a;
					c = tex2Dlod(_SplatC3, uvTRI_tmp.xyzz)*splat_control1.w; col += c.rgb; tmp_gloss.a=c.a;
					gloss=tmp_gloss;
					
					float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*RTP_AOamp*dot(splat_control1,RTP_AO_89AB);
					col*=heightblend_AO;
					gloss*=heightblend_AO;
										
					uvTRI_tmp.z=uvTRI.z+rtp_mipoffset_bump;
					
					#ifdef RTP_SNOW
						uvTRI_tmp.z += snow_depth;
					#endif				
					
					float4 normA=tex2Dlod(_BumpMap89, uvTRI_tmp.xyzz).rgba;
					normA = (triplanar_blend.x>=0.95) ? normA.grab : normA;
					float4 normB=tex2Dlod(_BumpMapAB, uvTRI_tmp.xyzz).rgba;
					normB = (triplanar_blend.x>=0.95) ? normB.grab : normB;
					float4 normals_combined = normA*splat_control1.rrgg;
					normals_combined += normB*splat_control1.bbaa;
					float3 n;
					n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
					if (triplanar_blend.x>=0.95) n.x= triplanar_flip.x ? n.x : -n.x;
					if (triplanar_blend.z>=0.95) 	n.y= triplanar_flip.y ? n.y : -n.y;
					n.xy*=_uv_Relief_z;
					n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
				
					o.Normal=n;
				}
				#if defined(RTP_UV_BLEND) && !defined(RTP_DISTANCE_ONLY_UV_BLEND)
					float4 _MixMipActual=uvTRI.zzzz+rtp_mipoffset_color+log2(_MixScale89AB);
					
					float4 uvSplat01M=uvTRI.xyxy*_MixScale89AB.xxyy;
					float4 uvSplat23M=uvTRI.xyxy*_MixScale89AB.zzww;
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
					colBlend=lerp(half3(0.5, 0.5, 0.5), colBlend, saturate((triplanar_blend_simple-0.75)*4));										
				#endif				
				//
				// EOF triplanar
				//
			#else
				//
				// no triplanar
				//
				#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
					float _MipActual=min(mip_selector.x + rtp_mipoffset_color, 6);
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, _MipActual.xx)); col = splat_control1.x * c.rgb; gloss.r = c.a;
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, _MipActual.xx)); col += splat_control1.y * c.rgb; gloss.g = c.a;
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, _MipActual.xx)); col += splat_control1.z * c.rgb; gloss.b = c.a;
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, _MipActual.xx)); col += splat_control1.w * c.rgb; gloss.a = c.a;
				#else
					rayPos.w=mip_selector.x+rtp_mipoffset_color;
					c = tex2Dlod(_SplatC0, rayPos.xyww); col = splat_control1.x * c.rgb; gloss.r = c.a;
					c = tex2Dlod(_SplatC1, rayPos.xyww); col += splat_control1.y * c.rgb; gloss.g = c.a;
					c = tex2Dlod(_SplatC2, rayPos.xyww); col += splat_control1.z * c.rgb; gloss.b = c.a;
					c = tex2Dlod(_SplatC3, rayPos.xyww); col += splat_control1.w * c.rgb; gloss.a = c.a;				
				#endif
				
				float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*RTP_AOamp*dot(splat_control1,RTP_AO_89AB);
				col*=heightblend_AO;
				gloss*=heightblend_AO;				
				
				#if !defined(_4LAYERS) && defined(RTP_HARD_CROSSPASS)
					heightblend_AO=1-saturate(1-saturate((splat_controlA_coverage-0.5)*(2+RTP_AOsharpness)))*RTP_AOamp*dot(splat_control1,RTP_AO_89AB)*0.5;
					col*=heightblend_AO;
					gloss*=heightblend_AO;				
					//float heightblend_AO2=1-saturate(1-saturate((splat_controlA_coverage-0.5)*20));
					//col=lerp(global_color_value.rgb*heightblend_AO, col, heightblend_AO2);
				#endif					
				//
				// EOF no triplanar
				//
			#endif

			o.Gloss = dot(gloss, splat_control1);
			o.Specular = 0.03+dot(_Spec89AB, splat_control1);

			#if defined(RTP_UV_BLEND) && !defined(RTP_TRIPLANAR)
			#ifndef RTP_DISTANCE_ONLY_UV_BLEND
				#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
					float4 _MixMipActual=min(_MipActual.xxxx + log2(_MixScale89AB), 6);
					float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
					_multMix-=hi_mip_adjustMix;
					_offMix+=0.5*hi_mip_adjustMix;
					
					uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale89AB.xxyy)*_multMix+_offMix;
					uvSplat01M.zw+=float2(0.5,0);
					uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale89AB.zzww)*_multMix+_offMix;
					uvSplat23M+=float4(0,0.5,0.5,0.5);	
					
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
				#else
					float4 _MixMipActual=mip_selector.xxxx + rtp_mipoffset_color + log2(_MixScale89AB);
					
					float4 uvSplat01M=IN._uv_Relief.xyxy*_MixScale89AB.xxyy;
					float4 uvSplat23M=IN._uv_Relief.xyxy*_MixScale89AB.zzww;
	
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
				#endif
			#endif
			#endif
			
			#if defined(RTP_UV_BLEND)
				#ifndef RTP_DISTANCE_ONLY_UV_BLEND			
					colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control1, _MixSaturation89AB) );
					col=lerp(col, col*colBlend*2, blendVal);			
				#endif
			#endif
		 	
			#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
				col=lerp(col, global_color_value.rgb, _uv_Relief_w);
			#endif
			
			#if !defined(RTP_TRIPLANAR)
				float3 n;
				float4 normals_combined;
				rayPos.w=mip_selector.x+rtp_mipoffset_bump;
				#ifdef RTP_SNOW
					rayPos.w += snow_depth;
				#endif				
				normals_combined = tex2Dlod(_BumpMap89, rayPos.xyww).rgba*splat_control1.rrgg;
				normals_combined+=tex2Dlod(_BumpMapAB, rayPos.xyww).rgba*splat_control1.bbaa;
				n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
				n.xy*=_uv_Relief_z;
				n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
				o.Normal=n;
			#else
				// normalne wyliczone powyżej
			#endif	
			
			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatC0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control1, _VerticalTexture89AB) );
			#endif
							
			////////////////////////////////
			// water
			//
	        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
		        GlossDry=o.Gloss;
		    #endif
	        #if defined(RTP_WETNESS)
				#ifdef RTP_CAUSTICS
					TERRAIN_WetSpecularity*=1-damp_fct_caustics;
				#endif	        
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		       col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=dot(splat_controlA_normalized, TERRAIN_WaterOpacity89AB)*p; // gladsze przejscie (po nieskompresowanym splat_control)

				#ifdef RTP_POM_SHADING
		 		#if defined(RTP_SOFT_SHADOWS) || defined(RTP_HARD_SHADOWS)
	 				shadow_atten=lerp(shadow_atten, 1, _WaterOpacity);
				#endif
				#endif
	 					        
				col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        o.Normal.xy+=roff;
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*TERRAIN_WetDarkening;
	        #endif
			// water
			////////////////////////////////
				
			#if defined(RTP_SUPER_DETAIL) && !defined(RTP_SIMPLE_SHADING)
				#ifdef RTP_TRIPLANAR
					float4 super_detail=tex2Dlod(_BumpMapGlobal, float4(uvTRI.xy*_SuperDetailTiling, uvTRI.zz + rtp_mipoffset_superdetail));
					super_detail=lerp(float4(0.5,0.5,0,0), super_detail, triplanar_blend_superdetail);
				#else
					float4 super_detail=tex2Dlod(_BumpMapGlobal, float4(rayPos.xy*_SuperDetailTiling, mip_selector + rtp_mipoffset_superdetail));
				#endif
				float3 super_detail_norm;				
				super_detail_norm.xy = (super_detail.xy*4-2)*(dot(0.8,col.rgb)+0.4)+o.Normal.xy;
				super_detail_norm.z = sqrt(1 - saturate(dot(super_detail_norm.xy, super_detail_norm.xy)));
				super_detail_norm=normalize(super_detail_norm);
				float sdVal=_uv_Relief_z*dot(splat_control1, _SuperDetailStrengthNormal89AB);
				#if defined(RTP_SNOW)
					sdVal*=saturate(1-snow_depth);
				#endif
				o.Normal=lerp(o.Normal, super_detail_norm, sdVal);		
				#if defined(RTP_SUPER_DTL_MULTS) && !defined(RTP_WETNESS) && !defined(RTP_REFLECTION)
					float near_blend;
					float far_blend;
					float near_far_blend_dist=saturate(_uv_Relief_z-0.5)*2;
					near_blend=lerp(1, global_bump_val.b, dot(splat_control1, _SuperDetailStrengthMultASelfMaskNear89AB));
					far_blend=lerp(0, global_bump_val.b, dot(splat_control1, _SuperDetailStrengthMultASelfMaskFar89AB));
					col=lerp(col, col*super_detail.z*2, lerp(far_blend, near_blend, near_far_blend_dist)*dot(splat_control1, _SuperDetailStrengthMultA89AB));
					near_blend=lerp(1, global_bump_val.a, dot(splat_control1, _SuperDetailStrengthMultBSelfMaskNear89AB));
					far_blend=lerp(0, global_bump_val.a, dot(splat_control1, _SuperDetailStrengthMultBSelfMaskFar89AB));
					col=lerp(col, col*super_detail.w*2, lerp(far_blend, near_blend, near_far_blend_dist)*dot(splat_control1, _SuperDetailStrengthMultB89AB));
				#endif
			#endif
		
//#ifdef RTP_SNOW
//}
//#endif
			// snow color
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7) )
			#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
			#else
				rayPos.w=mip_selector.x+rtp_mipoffset_color;
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
					half4 csnow = tex2Dlod(_SplatB0, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatC0, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
					half4 csnow = tex2Dlod(_SplatB1, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatC1, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
					half4 csnow = tex2Dlod(_SplatB2, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatC2, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
					half4 csnow = tex2Dlod(_SplatB3, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatC3, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
			#endif	
			#endif
			// eof snow color
			
			IN_uv_Relief_Offset.xy=rayPos.xy;
			
		 	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		 	//
		 	// self shadowing 
		 	//
	 		#if defined(RTP_POM_SHADING) && !defined(RTP_TRIPLANAR)
	 		#if defined(RTP_SOFT_SHADOWS) || defined(RTP_HARD_SHADOWS)
	 			#ifdef RTP_SNOW
	 				rayPos.w=mip_selector.x+rtp_mipoffset_height+snow_depth;
	 			#endif
	 			
				EyeDirTan=IN.lightDir.xyz;
				EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*DAMP_COLOR_VAL);
				delta=_TERRAIN_HeightMap3_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH_SHADOWS/length(EyeDirTan.xy);
				h_prev=rayPos.z;
				//rayPos.xyz+=EyeDirTan*_TERRAIN_HeightMap3_TexelSize.x*2;
				EyeDirTan*=delta;
		
				hit_flag=false;
				dh_prev=0;
				//_TERRAIN_SHADOW_STEPS=min(_TERRAIN_SHADOW_STEPS, ((EyeDirTan.z>0) ? (1-rayPos.z) : rayPos.z) / abs(EyeDirTan.z));
				for(int i=0; i<_TERRAIN_SHADOW_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
					_h=dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww));
					hit_flag=_h >= rayPos.z;
					if (hit_flag) break;
					h_prev=_h;
					dh_prev = rayPos.z - _h;
				}
				
				#ifdef RTP_SOFT_SHADOWS
					if (hit_flag) {
						// secant search
						float scl=dh_prev / ((_h-h_prev) - EyeDirTan.z);
						rayPos.xyz-=EyeDirTan*(1 - scl); // back
						EyeDirTan=IN.lightDir.xyz*_TERRAIN_HeightMap3_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH_SHADOWS;
						EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*DAMP_COLOR_VAL);
						float smooth_val=0;
						float break_val=_TERRAIN_ExtrudeHeight*_TERRAIN_ShadowSmoothing;
						for(int i=0; i<_TERRAIN_SHADOW_SMOOTH_STEPS; i++) {
							rayPos.xyz+=EyeDirTan;
							float d=dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap3, rayPos.xyww)) - rayPos.z;
							smooth_val+=saturate(d);
							if (smooth_val>break_val) break;
						}
						shadow_atten=saturate(1-smooth_val/break_val);
					}
				#else
					shadow_atten=hit_flag ? 0 : shadow_atten;
				#endif
		
				shadow_atten=shadow_atten*_TERRAIN_SelfShadowStrength+(1-_TERRAIN_SelfShadowStrength);
				#ifdef RTP_SNOW
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*DAMP_COLOR_VAL*(1-snow_depth_lerp));
				#else
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*DAMP_COLOR_VAL);
				#endif
			#endif
			#endif
			//
		 	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

	 		// end of splats 0-3 close
	 	}
		#ifndef _4LAYERS 		
	 	else {
	 		//////////////////////////////////
	 		//
	 		// splats 4-7 close
	 		//
	 		//////////////////////////////////
	 		
			#ifdef RTP_HARD_CROSSPASS
				splat_control2 /= dot(splat_control2, 1);
			#endif
	 		
	 		#if ( defined(RTP_POM_SHADING) && !defined(RTP_HARD_CROSSPASS) ) || ( defined(RTP_POM_SHADING) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO)) )
				if (DAMP_COLOR_VAL>0) {
				for(int i=0; i<_TERRAIN_DIST_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
			 		float4 tH;
					#ifdef USE_EXTRUDE_REDUCTION
						tH=lerp(tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER4567);
					#else
						tH=tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww);
					#endif						
					_h=saturate(dot(splat_control2, tH));
					hit_flag=_h >= rayPos.z;
					if (hit_flag) break; 
					h_prev=_h;
					dh_prev = rayPos.z - _h;
				}
				}
				
				if (hit_flag) {
					// secant search - 2 steps
					float scl=dh_prev / ((_h-h_prev) - EyeDirTan.z);
					rayPos.xyz-=EyeDirTan*(1 - scl); // back
			 		float4 tH;
					#ifdef USE_EXTRUDE_REDUCTION
						tH=lerp(tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER4567);
					#else
						tH=tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww);
					#endif						
					float _nh=saturate(dot(splat_control2, tH));
					if (_nh >= rayPos.z) {
						EyeDirTan*=scl;
						scl=dh_prev / ((_nh-h_prev) - EyeDirTan.z);
						rayPos.xyz-=EyeDirTan*(1 - scl); // back
					} else {
						EyeDirTan*=(1-scl);
						dh_prev = rayPos.z - _nh;
						scl=dh_prev / ((_h-_nh) - EyeDirTan.z);
						rayPos.xyz+=EyeDirTan*scl; // forth
					}
				}
				#ifdef RTP_WETNESS
					actH=rayPos.z;
				#endif
			#else
		 		#if ( defined(RTP_PM_SHADING) && !defined(RTP_HARD_CROSSPASS) ) || ( defined(RTP_PM_SHADING) && defined(RTP_47SHADING_PM) )
					rayPos.xy += ParallaxOffset(dot(splat_control2, tHB), _TERRAIN_ExtrudeHeight*_uv_Relief_z*DAMP_COLOR_VAL, IN_viewDir.xyz);
				#endif
				#ifdef RTP_WETNESS
					actH=dot(splat_control2, tHB);					
				#endif
			#endif
			
			////////////////////////////////
			// water
			//
			float4 water_splat_control=splat_control2;
 			#if defined(RTP_REFLECTION)
		 		#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_LayerReflection=dot(splat_control2, TERRAIN_LayerReflection4567)*_uv_Relief_z;
				#else
					TERRAIN_LayerReflection=dot(splat_control2, TERRAIN_LayerReflection4567);
				#endif
 			#endif
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection4567)*_uv_Relief_z;
				#else
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection4567);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlB_normalized, TERRAIN_LayerWetStrength4567);
				TERRAIN_WetSpecularity=dot(water_splat_control, TERRAIN_WetSpecularity4567);
				TERRAIN_WaterGloss=dot(water_splat_control, TERRAIN_WaterGloss4567);
				TERRAIN_WaterColor=half4( dot(splat_controlB_normalized, TERRAIN_WaterColorR4567), dot(splat_controlB_normalized, TERRAIN_WaterColorG4567), dot(splat_controlB_normalized, TERRAIN_WaterColorB4567), dot(splat_controlB_normalized, TERRAIN_WaterColorA4567) );
				
				float TERRAIN_WaterLevel=dot(water_splat_control, TERRAIN_WaterLevel4567);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlB_normalized, TERRAIN_WaterLevelSlopeDamp4567);
				float TERRAIN_Flow=dot(water_splat_control, TERRAIN_Flow4567);
				float TERRAIN_WaterEdge=dot(water_splat_control, TERRAIN_WaterEdge4567);
				float TERRAIN_Refraction=dot(water_splat_control, TERRAIN_Refraction4567);
				float TERRAIN_WetRefraction=dot(water_splat_control, TERRAIN_WetRefraction4567);
				
				TERRAIN_LayerWetStrength*=saturate(2- water_mask*2-perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
				#ifdef RTP_SNOW
				TERRAIN_LayerWetStrength*=saturate(1-snow_val);
				#endif
				float2 roff=0;
				if (TERRAIN_LayerWetStrength>0) {
					wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
					float _RippleDamp=saturate(TERRAIN_LayerWetStrength*2-1)*saturate(1-wetSlope*4)*_uv_Relief_z;
					TERRAIN_RainIntensity*=_RippleDamp;
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
					TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH);
					TERRAIN_Flow*=TERRAIN_LayerWetStrength*TERRAIN_LayerWetStrength;
					
					p = saturate((TERRAIN_WaterLevel-actH)*TERRAIN_WaterEdge);
					p*=p;
			 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO) || defined(RTP_47SHADING_PM)) )
				 		#if !defined(RTP_SIMPLE_SHADING) && !defined(SIMPLE_WATER)
							float2 flowUV=lerp(IN._uv_Relief.xy, rayPos.xy, 1-p*0.5)*TERRAIN_FlowScale;
							float _Tim=frac(_Time.x*4)*2;
							float ft=abs(frac(_Tim)*2 - 1);
							float2 flowSpeed=clamp((IN._viewDir.zw+0.01)*4,-1,1)/4;
							flowSpeed*=TERRAIN_FlowSpeed*TERRAIN_FlowScale;
							float rtp_mipoffset_add = (1-saturate(dot(flowSpeed, flowSpeed)*TERRAIN_mipoffset_flowSpeed))*TERRAIN_mipoffset_flowSpeed;
							rtp_mipoffset_add+=(1-TERRAIN_LayerWetStrength)*8;
							float2 flowOffset=tex2Dlod(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx)*flowSpeed, mip_selector+rtp_mipoffset_flow+rtp_mipoffset_add)).rg*2-1;
							flowOffset=lerp(flowOffset, tex2Dlod(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.1, mip_selector+rtp_mipoffset_flow+rtp_mipoffset_add)).rg*2-1, ft);
							// stały przepływ na płaskim
							//float slowMotionFct=dot(flowSpeed,flowSpeed);
							//slowMotionFct=saturate(slowMotionFct*50);
							//flowOffset=lerp(tex2Dlod(_BumpMapGlobal, float4(flowUV+float2(0,2*_Time.x*TERRAIN_FlowSpeed*TERRAIN_FlowScale), mip_selector+rtp_mipoffset_flow)).rg*2-1, flowOffset, slowMotionFct );
							//
							flowOffset*=TERRAIN_Flow*max(p, saturate(TERRAIN_WetSpecularity))*_uv_Relief_z*TERRAIN_LayerWetStrength;
						#else
							float2 flowOffset=0;
						#endif
					#else
						float2 flowOffset=0;
					#endif
					
			 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO) || defined(RTP_47SHADING_PM)) )
						#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
							float2 rippleUV = IN._uv_Relief.xy*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
						  	//roff = GetRipple( float4(rippleUV, mip_selector + rtp_mipoffset_ripple), TERRAIN_RainIntensity);
							//roff += GetRipple( float4(rippleUV+float2(0.25,0.25), mip_selector + rtp_mipoffset_ripple ), TERRAIN_RainIntensity);
						    float4 Ripple;
						  	{
						  	 	Ripple = tex2Dlod(TERRAIN_RippleMap, float4(rippleUV, mip_selector + rtp_mipoffset_ripple));
							    Ripple.xy = Ripple.xy * 2 - 1;
							
							    float DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
							    float TimeFrac = DropFrac - 1.0f + Ripple.z;
							    float DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
							    float FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
							  	roff = Ripple.xy * FinalFactor * 0.35f;
							  	
							  	rippleUV+=float2(0.25,0.25);
						  	 	Ripple = tex2Dlod(TERRAIN_RippleMap, float4(rippleUV, mip_selector + rtp_mipoffset_ripple));
							    Ripple.xy = Ripple.xy * 2 - 1;
							
							    DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
							    TimeFrac = DropFrac - 1.0f + Ripple.z;
							    DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
							    FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
							  	roff += Ripple.xy * FinalFactor * 0.35f;
						  	}
						  	roff*=4*_RippleDamp*lerp(TERRAIN_WetDropletsStrength, 1, p);
						  	roff+=flowOffset;
						#else
							roff = flowOffset;
						#endif
					#else
						roff = flowOffset;
					#endif
					
			 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO) || defined(RTP_47SHADING_PM)) )
			 		#if !defined(RTP_SIMPLE_SHADING)
						rayPos.xy+=TERRAIN_Refraction*roff*max(p, TERRAIN_WetRefraction);
					#endif
					#endif
				}
			#endif
			// water
			////////////////////////////////
			
			uvSplat01=frac(rayPos.xy).xyxy*_mult+_off;
			uvSplat01.zw+=float2(0.5,0);
			uvSplat23=uvSplat01.xyxy+float4(0,0.5,0.5,0.5);

//#ifdef RTP_SNOW
//if (snow_MayBeNotFullyCovered_flag) {
//#endif	 	
			float4 c;
			float4 gloss;
			float _MipActual=min(mip_selector.x + rtp_mipoffset_color,6);
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, _MipActual.xx)); col = splat_control2.x * c.rgb; gloss.r = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, _MipActual.xx)); col += splat_control2.y * c.rgb; gloss.g = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, _MipActual.xx)); col += splat_control2.z * c.rgb; gloss.b = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, _MipActual.xx)); col += splat_control2.w * c.rgb; gloss.a = c.a;
			
			float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control2-0.5)*(2+RTP_AOsharpness)),0.25))*RTP_AOamp*dot(splat_control2,RTP_AO_4567);
			col*=heightblend_AO;
			gloss*=heightblend_AO;
			
			#if !defined(_4LAYERS) && defined(RTP_HARD_CROSSPASS)
				heightblend_AO=1-saturate(1-saturate((splat_controlB_coverage-0.5)*(2+RTP_AOsharpness)))*RTP_AOamp*dot(splat_control2,RTP_AO_4567)*0.5;
				col*=heightblend_AO;
				gloss*=heightblend_AO;				
				//float heightblend_AO2=1-saturate(1-saturate((splat_controlB_coverage-0.5)*20));
				//col=lerp(global_color_value.rgb*heightblend_AO, col, heightblend_AO2);
			#endif
						
			o.Gloss = dot(gloss, splat_control2);
			o.Specular = 0.03+dot(_Spec4567, splat_control2);
			#ifdef RTP_UV_BLEND
			#ifndef RTP_DISTANCE_ONLY_UV_BLEND
//				float4 _MixMipActual=min(_MipActual.xxxx+log2(_MixScale4567), 6);
//				float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
//				_multMix-=hi_mip_adjustMix;
//				_offMix+=0.5*hi_mip_adjustMix;
//
//				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale4567.xxyy)*_multMix+_offMix;
//				uvSplat01M.zw+=float2(0.5,0);
//				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale4567.zzww)*_multMix+_offMix;
//				uvSplat23M+=float4(0,0.5,0.5,0.5);			

				float4 _MixMipActual=min(_MipActual.xxxx+log2(_MixScale89AB), 6);
				float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
				_multMix-=hi_mip_adjustMix;
				_offMix+=0.5*hi_mip_adjustMix;

				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale89AB.xxyy)*_multMix+_offMix;
				uvSplat01M.zw+=float2(0.5,0);
				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale89AB.zzww)*_multMix+_offMix;
				uvSplat23M+=float4(0,0.5,0.5,0.5);			
				
				half3 colBlend = splat_control2.x * UV_BLEND_ROUTE_LAYER_4;
				colBlend += splat_control2.y * UV_BLEND_ROUTE_LAYER_5;
				colBlend += splat_control2.z * UV_BLEND_ROUTE_LAYER_6;
				colBlend += splat_control2.w * UV_BLEND_ROUTE_LAYER_7;
			#endif
			#endif
			
			#if defined(RTP_UV_BLEND)
				#ifndef RTP_DISTANCE_ONLY_UV_BLEND			
					colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control2, _MixSaturation4567) );
					col=lerp(col, col*colBlend*2, blendVal);			
				#endif
			#endif	
		 	
			#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
				col=lerp(col, global_color_value.rgb, _uv_Relief_w);
			#endif
			
			float3 n;
			float4 normals_combined;
			rayPos.w=mip_selector.x+rtp_mipoffset_bump;
			#ifdef RTP_SNOW
				rayPos.w += snow_depth;
			#endif				
			normals_combined = tex2Dlod(_BumpMap45, rayPos.xyww).rgba*splat_control2.rrgg;
			normals_combined+=tex2Dlod(_BumpMap67, rayPos.xyww).rgba*splat_control2.bbaa;
			n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
			n.xy*=_uv_Relief_z;
			n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
			o.Normal=n;
			
			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatC0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control2, _VerticalTexture4567) );
			#endif
			
			////////////////////////////////
			// water
			//
	        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
		        GlossDry=o.Gloss;
		    #endif
	        #if defined(RTP_WETNESS)
				#ifdef RTP_CAUSTICS
					TERRAIN_WetSpecularity*=1-damp_fct_caustics;
				#endif	        
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=dot(splat_controlB_normalized, TERRAIN_WaterOpacity4567)*p; // gladsze przejscie (po nieskompresowanym splat_control)

				#ifdef RTP_POM_SHADING
		 		#if defined(RTP_SOFT_SHADOWS) || defined(RTP_HARD_SHADOWS)
	 				shadow_atten=lerp(shadow_atten, 1, _WaterOpacity);
				#endif
				#endif
	 					        
		        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        o.Normal.xy+=roff;
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*TERRAIN_WetDarkening;
	        #endif
			// water
			////////////////////////////////
		        
	 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO) || defined(RTP_47SHADING_PM)) )
			#if defined(RTP_SUPER_DETAIL)
				float4 super_detail=tex2Dlod(_BumpMapGlobal, float4(rayPos.xy*_SuperDetailTiling, mip_selector+rtp_mipoffset_superdetail));
				float3 super_detail_norm;
				super_detail_norm.xy = (super_detail.xy*4-2)*(dot(0.8,col.rgb)+0.4)+o.Normal.xy;
				super_detail_norm.z = sqrt(1 - saturate(dot(super_detail_norm.xy, super_detail_norm.xy)));
				super_detail_norm=normalize(super_detail_norm);
				float sdVal=_uv_Relief_z*dot(splat_control2, _SuperDetailStrengthNormal4567);
				#if defined(RTP_SNOW)
					sdVal*=saturate(1-snow_depth);
				#endif
				o.Normal=lerp(o.Normal, super_detail_norm, sdVal);	
				#if defined(RTP_SUPER_DTL_MULTS) && !defined(RTP_WETNESS) && !defined(RTP_REFLECTION)
					float near_blend;
					float far_blend;
					float near_far_blend_dist=saturate(_uv_Relief_z-0.5)*2;
					near_blend=lerp(1, global_bump_val.b, dot(splat_control2, _SuperDetailStrengthMultASelfMaskNear4567));
					far_blend=lerp(0, global_bump_val.b, dot(splat_control2, _SuperDetailStrengthMultASelfMaskFar4567));
					col=lerp(col, col*super_detail.z*2, lerp(far_blend, near_blend, near_far_blend_dist)*dot(splat_control2, _SuperDetailStrengthMultA4567));
					near_blend=lerp(1, global_bump_val.a, dot(splat_control2, _SuperDetailStrengthMultBSelfMaskNear4567));
					far_blend=lerp(0, global_bump_val.a, dot(splat_control2, _SuperDetailStrengthMultBSelfMaskFar4567));
					col=lerp(col, col*super_detail.w*2, lerp(far_blend, near_blend, near_far_blend_dist)*dot(splat_control2, _SuperDetailStrengthMultB4567));
				#endif
			#endif
			#endif
			
//#ifdef RTP_SNOW
//}
//#endif

			// snow color
	 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO) || defined(RTP_47SHADING_PM)) )
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7) )
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
			#endif
			#endif
			// eof snow color

		 	IN_uv_Relief_Offset.xy=rayPos.xy;
		 	
		 	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		 	//
		 	// self shadowing 
		 	//

	 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED)) )
	 		#if defined(RTP_POM_SHADING) && !defined(RTP_TRIPLANAR)
	 		#if defined(RTP_SOFT_SHADOWS) || defined(RTP_HARD_SHADOWS)
	 			#ifdef RTP_SNOW
	 				rayPos.w=mip_selector.x+rtp_mipoffset_height+snow_depth;
	 			#endif
	 		
				EyeDirTan=IN.lightDir.xyz;
				EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*DAMP_COLOR_VAL);
				delta=_TERRAIN_HeightMap3_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH_SHADOWS/length(EyeDirTan.xy);
				h_prev=rayPos.z;
				//rayPos.xyz+=EyeDirTan*_TERRAIN_HeightMap3_TexelSize.x*2;
				EyeDirTan*=delta;
		
				hit_flag=false;
				dh_prev=0;
				//_TERRAIN_SHADOW_STEPS=min(_TERRAIN_SHADOW_STEPS, ((EyeDirTan.z>0) ? (1-rayPos.z) : rayPos.z) / abs(EyeDirTan.z));
				for(int i=0; i<_TERRAIN_SHADOW_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
					_h=dot(splat_control2, tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww));
					hit_flag=_h >= rayPos.z;
					if (hit_flag) break;
					h_prev=_h;
					dh_prev = rayPos.z - _h;
				}
				
		 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && defined(RTP_47SHADING_POM_HI) )
					#ifdef RTP_SOFT_SHADOWS
						if (hit_flag) {
							// secant search
							float scl=dh_prev / ((_h-h_prev) - EyeDirTan.z);
							rayPos.xyz-=EyeDirTan*(1 - scl); // back
							EyeDirTan=IN.lightDir.xyz*_TERRAIN_HeightMap3_TexelSize.x*2*_TERRAIN_WAVELENGTH_SHADOWS;
							EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*DAMP_COLOR_VAL);
							float smooth_val=0;
							float break_val=_TERRAIN_ExtrudeHeight*_TERRAIN_ShadowSmoothing;
							for(int i=0; i<_TERRAIN_SHADOW_SMOOTH_STEPS; i++) {
								rayPos.xyz+=EyeDirTan;
								float d=dot(splat_control2, tex2Dlod(_TERRAIN_HeightMap2, rayPos.xyww)) - rayPos.z;
								smooth_val+=saturate(d);
								if (smooth_val>break_val) break;
							}
							shadow_atten=saturate(1-smooth_val/break_val);
						}
					#else
						shadow_atten=hit_flag ? 0 : shadow_atten;
					#endif
				#else
					shadow_atten=hit_flag ? 0 : shadow_atten;
				#endif
		
				shadow_atten=shadow_atten*_TERRAIN_SelfShadowStrength+(1-_TERRAIN_SelfShadowStrength);
				#ifdef RTP_SNOW
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*DAMP_COLOR_VAL*(1-snow_depth_lerp));
				#else
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*DAMP_COLOR_VAL);
				#endif
				
			#endif
			#endif
			#endif
			//
		 	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
		 	
	 		// end of splats 4-7 close		 	
	 	}
		#endif //!_4LAYERS 
		
 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO)) )
			#ifdef RTP_POM_SHADING
			col*=shadow_atten;
			col*=lerp(_TERRAIN_ShadowColorization*2, 1, shadow_atten);
			o.Gloss*=shadow_atten;	
			#endif		 			
		#endif
			
	} else {
 		//////////////////////////////////
 		//
 		// far
 		//
 		//////////////////////////////////
				
		#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
		float _off=16*_SplatAtlasC_TexelSize.x;
		float _mult=_off*-2+0.5;
		float4 _offMix=_off;
		float4 _multMix=_mult;
		float hi_mip_adjust=(exp2(min(mip_selector.x+rtp_mipoffset_color,6)))*_SplatAtlasC_TexelSize.x;
		_mult-=hi_mip_adjust;
		_off+=0.5*hi_mip_adjust;
		float4 uvSplat01, uvSplat23;
		uvSplat01=frac(IN._uv_Relief.xy).xyxy*_mult+_off;
		uvSplat01.zw+=float2(0.5,0);
		uvSplat23=uvSplat01.xyxy+float4(0,0.5,0.5,0.5);
		
		float4 uvSplat01M, uvSplat23M;
		#endif
		
		#if !defined(_4LAYERS) 
		#ifdef RTP_HARD_CROSSPASS
		 	if (false) {
	 	#else
		 	if (splat_controlA_coverage>0.01 && splat_controlB_coverage>0.01) {
	 	#endif
	 		//////////////////////////////////////////////
	 		//
	 		// splats 0-7 far combined
	 		//
	 		///////////////////////////////////////////////
	 		#ifdef RTP_SHOW_OVERLAPPED
	 		o.Emission.r=1;
	 		#endif
	 		
//#ifdef RTP_SNOW
//if (snow_MayBeNotFullyCovered_flag) {
//#endif			
#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			if (_uv_Relief_w==1) {
				col=global_color_value.rgb;
				o.Gloss=0;
				o.Specular=0.5;
			} else {
#endif
					
	 		float4 MIPmult89AB=_MIPmult89AB * _uv_Relief_w;
	 		
			float4 _MipActual=min(mip_selector.x + rtp_mipoffset_color+MIPmult89AB, 6);
			half4 c;
			float4 gloss;
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, _MipActual.xx)); col = splat_control1.x * c.rgb; gloss.r = c.a;
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, _MipActual.yy)); col += splat_control1.y * c.rgb; gloss.g = c.a;
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, _MipActual.zz)); col += splat_control1.z * c.rgb; gloss.b = c.a;
			c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, _MipActual.ww)); col += splat_control1.w * c.rgb; gloss.a = c.a;
			
			float hbAO=(dot(splat_control1,RTP_AO_89AB)+dot(splat_control2,RTP_AO_4567))*RTP_AOamp*2;
			float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*hbAO;
			col*=heightblend_AO;
			gloss*=heightblend_AO;
			
			o.Gloss = dot(gloss, splat_control1);
			o.Specular = 0.03+dot(_Spec89AB, splat_control1);
			#ifdef RTP_UV_BLEND
				float4 _MixMipActual=min(_MipActual.xxxx+log2(_MixScale89AB), 6);
				float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
				float4 _multMix89AB=_multMix - hi_mip_adjustMix; // _multMix / _offMix potrzebujemy poniżej dla kanałów 4-7
				float4 _offMix89AB=_offMix + 0.5*hi_mip_adjustMix;
							
				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale89AB.xxyy)*_multMix89AB+_offMix89AB;
				uvSplat01M.zw+=float2(0.5,0);
				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale89AB.zzww)*_multMix89AB+_offMix89AB;
				uvSplat23M+=float4(0,0.5,0.5,0.5);			
				
				half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
				colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
				colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
				colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
			#endif
						
	 		float4 MIPmult4567=_MIPmult4567 * _uv_Relief_w;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, _MipActual.xx)); col += splat_control2.x * c.rgb; gloss.r = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, _MipActual.yy)); col += splat_control2.y * c.rgb; gloss.g = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, _MipActual.zz)); col += splat_control2.z * c.rgb; gloss.b = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, _MipActual.ww)); col += splat_control2.w * c.rgb; gloss.a = c.a;
			
			heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control2-0.5)*(2+RTP_AOsharpness)),0.25))*hbAO;
			col*=heightblend_AO;
			gloss*=heightblend_AO;
						
			o.Gloss += dot(gloss, splat_control2);
			o.Specular += dot(_Spec4567, splat_control2);
			#ifdef RTP_UV_BLEND
//				_MixMipActual=min(_MipActual.xxxx+log2(_MixScale4567), 6);
//				hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
//				_multMix-=hi_mip_adjustMix;
//				_offMix+=0.5*hi_mip_adjustMix;
//							
//				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale4567.xxyy)*_multMix+_offMix;
//				uvSplat01M.zw+=float2(0.5,0);
//				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale4567.zzww)*_multMix+_offMix;
//				uvSplat23M+=float4(0,0.5,0.5,0.5);			
				
				colBlend += splat_control2.x * UV_BLEND_ROUTE_LAYER_4;
				colBlend += splat_control2.y * UV_BLEND_ROUTE_LAYER_5;
				colBlend += splat_control2.z * UV_BLEND_ROUTE_LAYER_6;
				colBlend += splat_control2.w * UV_BLEND_ROUTE_LAYER_7;
			#endif
			
			#if defined(RTP_UV_BLEND)
				colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control1, _MixSaturation89AB) + dot(splat_control2, _MixSaturation4567));
				col=lerp(col, col*colBlend*2, blendVal);			
			#endif	
						
#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			col=lerp(col, global_color_value.rgb, _uv_Relief_w);
			o.Gloss*=(1-_uv_Relief_w);
			o.Specular=lerp(o.Specular, 0.5, _uv_Relief_w);
			}
#endif
			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatC0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control1, _VerticalTexture89AB) + dot(splat_control2, _VerticalTexture4567));
			#endif
						
//#ifdef RTP_SNOW
//}
//#endif	
			// snow color
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7) )
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif			
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif			
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif				
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif			
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
				half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif				
			#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
				half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
				GETrtp_snow_TEX
			#endif		
			#endif
			// snow color
			
			#if defined(RTP_SUPER_DETAIL) && defined (RTP_SUPER_DTL_MULTS) && !defined(RTP_SIMPLE_SHADING) && !defined(RTP_WETNESS) && !defined(RTP_REFLECTION)
				float4 super_detail=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy*_SuperDetailTiling, mip_selector+rtp_mipoffset_superdetail));

				float far_blend;
				far_blend=lerp(0, global_bump_val.b, dot(splat_control1, _SuperDetailStrengthMultASelfMaskFar89AB) + dot(splat_control2, _SuperDetailStrengthMultASelfMaskFar4567));
				col=lerp(col, col*super_detail.z*2, far_blend*(dot(splat_control1, _SuperDetailStrengthMultA89AB)+dot(splat_control2, _SuperDetailStrengthMultA4567)));
				far_blend=lerp(0, global_bump_val.a, dot(splat_control1, _SuperDetailStrengthMultBSelfMaskFar89AB) + dot(splat_control2, _SuperDetailStrengthMultBSelfMaskFar4567));
				col=lerp(col, col*super_detail.w*2, far_blend*(dot(splat_control1, _SuperDetailStrengthMultB89AB)+dot(splat_control2, _SuperDetailStrengthMultB4567)));
			#endif
			
			////////////////////////////////
			// water
			//
			float4 water_splat_control1=splat_control1;
			float4 water_splat_control2=splat_control2;
 			#if defined(RTP_REFLECTION) 
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_LayerReflection=0;
				#else
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection89AB)+dot(splat_control2, TERRAIN_LayerReflection4567);
				#endif
 			#endif
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=0;
				#else
					TERRAIN_WetReflection=dot(water_splat_control1, TERRAIN_WetReflection89AB)+dot(water_splat_control2, TERRAIN_WetReflection4567);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlA_normalized, TERRAIN_LayerWetStrength89AB)+dot(splat_controlB_normalized, TERRAIN_LayerWetStrength4567);
				TERRAIN_WetSpecularity=dot(water_splat_control1, TERRAIN_WetSpecularity89AB)+dot(water_splat_control2, TERRAIN_WetSpecularity4567);
				TERRAIN_WaterGloss=dot(water_splat_control1, TERRAIN_WaterGloss89AB)+dot(water_splat_control2, TERRAIN_WaterGloss4567);
				TERRAIN_WaterColor=half4( dot(splat_controlA_normalized, TERRAIN_WaterColorR89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorR4567), dot(splat_controlA_normalized, TERRAIN_WaterColorG89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorG4567), dot(splat_controlA_normalized, TERRAIN_WaterColorB89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorB4567), dot(splat_controlA_normalized, TERRAIN_WaterColorA89AB)+dot(splat_controlB_normalized, TERRAIN_WaterColorA4567) );

				float TERRAIN_WaterLevel=dot(water_splat_control1, TERRAIN_WaterLevel89AB)+dot(water_splat_control2, TERRAIN_WaterLevel4567);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlA_normalized, TERRAIN_WaterLevelSlopeDamp89AB)+dot(splat_controlB_normalized, TERRAIN_WaterLevelSlopeDamp4567);
				//float TERRAIN_Flow=dot(water_splat_control1, TERRAIN_Flow89AB)+dot(water_splat_control2, TERRAIN_Flow4567);
				float TERRAIN_WaterEdge=dot(water_splat_control1, TERRAIN_WaterEdge89AB)+dot(water_splat_control2, TERRAIN_WaterEdge4567);
				//float TERRAIN_Refraction=dot(water_splat_control1, TERRAIN_Refraction89AB)+dot(water_splat_control2, TERRAIN_Refraction4567);
				//float TERRAIN_WetRefraction=dot(water_splat_control1, TERRAIN_WetRefraction89AB)+dot(water_splat_control2, TERRAIN_WetRefraction4567);
				
				TERRAIN_LayerWetStrength*=saturate(2- water_mask*2-perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
				#ifdef RTP_SNOW
				TERRAIN_LayerWetStrength*=saturate(1-snow_val);
				#endif
				if (TERRAIN_LayerWetStrength>0) {
					wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
					TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH);
					
					p = saturate((TERRAIN_WaterLevel-actH)*TERRAIN_WaterEdge);
					p*=p;
				}
			#endif
	        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
		        GlossDry=o.Gloss;
		    #endif
	        #if defined(RTP_WETNESS)
				#ifdef RTP_CAUSTICS
					TERRAIN_WetSpecularity*=1-damp_fct_caustics;
				#endif	        
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, 0.2);//saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=(dot(splat_controlA_normalized, TERRAIN_WaterOpacity89AB)+dot(splat_controlB_normalized, TERRAIN_WaterOpacity4567))*p; // gladsze przejscie (po nieskompresowanym splat_control)

		        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*TERRAIN_WetDarkening;
	        #endif
			// water
			////////////////////////////////
						
	 	} else if (splat_controlA_coverage>splat_controlB_coverage)
		#endif // !_4LAYERS
	 	{
	 		//////////////////////////////////////////////
	 		//
	 		// splats 0-3 far
	 		//
	 		///////////////////////////////////////////////
	 		
			#ifdef RTP_HARD_CROSSPASS
				splat_control1 /= dot(splat_control1, 1);
			#endif

//#ifdef RTP_SNOW
//if (snow_MayBeNotFullyCovered_flag) {
//#endif		
#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			if (_uv_Relief_w==1) {
				#ifdef RTP_HARD_CROSSPASS
					col=global_color_value.rgb;
				#else
					col=global_color_value.rgb*splat_controlA_coverage;
				#endif
				o.Gloss=0;
				o.Specular=0.5;
			} else {
#endif			

	 		float4 MIPmult89AB=_MIPmult89AB*_uv_Relief_w;
			half4 c;
			float4 gloss=0;			
			
			#ifdef RTP_TRIPLANAR
				//
				// triplanar no blend - simple case
				//
				float4 _MixMipActual=uvTRI.zzzz+rtp_mipoffset_color+MIPmult89AB;
				float4 tmp_gloss;
				c = tex2Dlod(_SplatC0, float4(uvTRI.xy,_MixMipActual.xx))*splat_control1.x; col += c.rgb; tmp_gloss.r=c.a;
				c = tex2Dlod(_SplatC1, float4(uvTRI.xy,_MixMipActual.yy))*splat_control1.y; col += c.rgb; tmp_gloss.g=c.a;
				c = tex2Dlod(_SplatC2, float4(uvTRI.xy,_MixMipActual.zz))*splat_control1.z; col += c.rgb; tmp_gloss.b=c.a;
				c = tex2Dlod(_SplatC3, float4(uvTRI.xy,_MixMipActual.ww))*splat_control1.w; col += c.rgb; tmp_gloss.a=c.a;
				gloss=tmp_gloss;
				
				float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*RTP_AOamp*dot(splat_control1,RTP_AO_89AB);
				col*=heightblend_AO;
				gloss*=heightblend_AO;
								
				#if defined(RTP_UV_BLEND) 
					_MixMipActual+=log2(_MixScale89AB);
					
					float4 uvSplat01M=uvTRI.xyxy*_MixScale89AB.xxyy;
					float4 uvSplat23M=uvTRI.xyxy*_MixScale89AB.zzww;
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;			
					colBlend=lerp(half3(0.5, 0.5, 0.5), colBlend, saturate((triplanar_blend_simple-0.75)*4));							
				#endif

				//
				// EOF triplanar
				//
			#else
				//
				// no triplanar
				//
				#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
					float4 _MixMipActual=min(mip_selector.xxxx + rtp_mipoffset_color+MIPmult89AB, 6);
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, _MixMipActual.xx)); col = splat_control1.x * c.rgb; gloss.r = c.a;
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, _MixMipActual.yy)); col += splat_control1.y * c.rgb; gloss.g = c.a;
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, _MixMipActual.zz)); col += splat_control1.z * c.rgb; gloss.b = c.a;
					c = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, _MixMipActual.ww)); col += splat_control1.w * c.rgb; gloss.a = c.a;
				#else
					float4 _MixMipActual=mip_selector.xxxx + rtp_mipoffset_color+MIPmult89AB;
					c = tex2Dlod(_SplatC0, float4(IN._uv_Relief.xy, _MixMipActual.xx)); col = splat_control1.x * c.rgb; gloss.r = c.a;
					c = tex2Dlod(_SplatC1, float4(IN._uv_Relief.xy, _MixMipActual.yy)); col += splat_control1.y * c.rgb; gloss.g = c.a;
					c = tex2Dlod(_SplatC2, float4(IN._uv_Relief.xy, _MixMipActual.zz)); col += splat_control1.z * c.rgb; gloss.b = c.a;
					c = tex2Dlod(_SplatC3, float4(IN._uv_Relief.xy, _MixMipActual.ww)); col += splat_control1.w * c.rgb; gloss.a = c.a;				
				#endif
				
				float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*RTP_AOamp*dot(splat_control1,RTP_AO_89AB);
				col*=heightblend_AO;
				gloss*=heightblend_AO;
			
				#if !defined(_4LAYERS) && defined(RTP_HARD_CROSSPASS)
					heightblend_AO=1-saturate(1-saturate((splat_controlA_coverage-0.5)*(2+RTP_AOsharpness)))*RTP_AOamp*dot(splat_control1,RTP_AO_89AB)*0.5;
					col*=heightblend_AO;
					gloss*=heightblend_AO;				
					//float heightblend_AO2=1-saturate(1-saturate((splat_controlA_coverage-0.5)*20));
					//col=lerp(global_color_value.rgb*heightblend_AO, col, heightblend_AO2);
				#endif	
								
				//
				// EOF no triplanar
				//
			#endif
			o.Gloss = dot(gloss, splat_control1);
			o.Specular =0.03+dot(_Spec89AB, splat_control1);
			
			#if defined(RTP_UV_BLEND) && !defined(RTP_TRIPLANAR)
				#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
					_MixMipActual=min(mip_selector.xxxx+rtp_mipoffset_color+log2(_MixScale89AB), 6);
					float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
					_multMix-=hi_mip_adjustMix;
					_offMix+=0.5*hi_mip_adjustMix;
				
					uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale89AB.xxyy)*_multMix+_offMix;
					uvSplat01M.zw+=float2(0.5,0);
					uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale89AB.zzww)*_multMix+_offMix;
					uvSplat23M+=float4(0,0.5,0.5,0.5);		
					
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
				#else
					_MixMipActual=mip_selector.xxxx+rtp_mipoffset_color+log2(_MixScale89AB);
					
					float4 uvSplat01M=IN._uv_Relief.xyxy*_MixScale89AB.xxyy;
					float4 uvSplat23M=IN._uv_Relief.xyxy*_MixScale89AB.zzww;
					
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
				#endif
			#endif
			
			#if defined(RTP_UV_BLEND)
				colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control1, _MixSaturation89AB) );
				col=lerp(col, col*colBlend*2, blendVal);			
			#endif	
						
#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			#ifdef RTP_HARD_CROSSPASS
				col=lerp(col, global_color_value.rgb, _uv_Relief_w);
			#else
				col=lerp(col, global_color_value.rgb*splat_controlA_coverage, _uv_Relief_w);
			#endif
			o.Gloss*=(1-_uv_Relief_w);
			o.Specular=lerp(o.Specular, 0.5, _uv_Relief_w);
			}
#endif		
	
			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatC0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control1, _VerticalTexture89AB) );
			#endif
			
//#ifdef RTP_SNOW
//}
//#endif			

			// snow color
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7) )
			#if !defined(_4LAYERS) || defined(USE_COLOR_ATLAS)
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
			#else
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
					half4 csnow = tex2Dlod(_SplatB0, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatC0, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
					half4 csnow = tex2Dlod(_SplatB1, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatC1, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
					half4 csnow = tex2Dlod(_SplatB2, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatC2, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
					half4 csnow = tex2Dlod(_SplatB3, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatC3, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
			#endif	
			#endif
			// eof snow color
					
			#if defined(RTP_SUPER_DETAIL) && defined (RTP_SUPER_DTL_MULTS) && !defined(RTP_SIMPLE_SHADING) && !defined(RTP_WETNESS) && !defined(RTP_REFLECTION)
				float4 super_detail=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy*_SuperDetailTiling, mip_selector+rtp_mipoffset_superdetail));

				float far_blend;
				far_blend=lerp(0, global_bump_val.b, dot(splat_control1, _SuperDetailStrengthMultASelfMaskFar89AB));
				col=lerp(col, col*super_detail.z*2, far_blend*dot(splat_control1, _SuperDetailStrengthMultA89AB));
				far_blend=lerp(0, global_bump_val.a, dot(splat_control1, _SuperDetailStrengthMultBSelfMaskFar89AB));
				col=lerp(col, col*super_detail.w*2, far_blend*dot(splat_control1, _SuperDetailStrengthMultB89AB));
			#endif
			
			////////////////////////////////
			// water
			//
 			#if defined(RTP_REFLECTION) 
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_LayerReflection=0;
				#else
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection89AB);
				#endif
 			#endif
			float4 water_splat_control=splat_control1;
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=0;
				#else
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection89AB);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlA_normalized, TERRAIN_LayerWetStrength89AB);
				TERRAIN_WetSpecularity=dot(water_splat_control, TERRAIN_WetSpecularity89AB);
				TERRAIN_WaterGloss=dot(water_splat_control, TERRAIN_WaterGloss89AB);
				TERRAIN_WaterColor=half4( dot(splat_controlA_normalized, TERRAIN_WaterColorR89AB), dot(splat_controlA_normalized, TERRAIN_WaterColorG89AB), dot(splat_controlA_normalized, TERRAIN_WaterColorB89AB), dot(splat_controlA_normalized, TERRAIN_WaterColorA89AB) );
				
				float TERRAIN_WaterLevel=dot(water_splat_control, TERRAIN_WaterLevel89AB);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlA_normalized, TERRAIN_WaterLevelSlopeDamp89AB);
				//float TERRAIN_Flow=dot(water_splat_control, TERRAIN_Flow89AB);
				float TERRAIN_WaterEdge=dot(water_splat_control, TERRAIN_WaterEdge89AB);
				//float TERRAIN_Refraction=dot(water_splat_control, TERRAIN_Refraction89AB);
				//float TERRAIN_WetRefraction=dot(water_splat_control, TERRAIN_WetRefraction89AB);
				
				TERRAIN_LayerWetStrength*=saturate(2- water_mask*2-perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
				#ifdef RTP_SNOW
				TERRAIN_LayerWetStrength*=saturate(1-snow_val);
				#endif
				if (TERRAIN_LayerWetStrength>0) {
					wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
					TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH);
					
					p = saturate((TERRAIN_WaterLevel-actH)*TERRAIN_WaterEdge);
					p*=p;
				}
			#endif
	        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
		        GlossDry=o.Gloss;
		    #endif
	        #if defined(RTP_WETNESS)
				#ifdef RTP_CAUSTICS
					TERRAIN_WetSpecularity*=1-damp_fct_caustics;
				#endif	        
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, 0.2);//saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=dot(splat_controlA_normalized, TERRAIN_WaterOpacity89AB)*p;
	 					        
		        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*TERRAIN_WetDarkening;
	        #endif
			// water
			////////////////////////////////
						
	 	}
	 	
	 	#ifndef _4LAYERS
	 	else {
	 		//////////////////////////////////////////////
	 		//
	 		// splats 4-7 far
	 		//
	 		///////////////////////////////////////////////
	 		
			#ifdef RTP_HARD_CROSSPASS
				splat_control2 /= dot(splat_control2, 1);
			#endif
						
//#ifdef RTP_SNOW
//if (snow_MayBeNotFullyCovered_flag) {
//#endif		

#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			if (_uv_Relief_w==1) {
				#ifdef RTP_HARD_CROSSPASS
					col=global_color_value.rgb;
				#else
					col=global_color_value.rgb*splat_controlB_coverage;
				#endif
				o.Gloss=0;
				o.Specular=0.5;
			} else {
#endif	
	 		float4 MIPmult4567=_MIPmult4567*_uv_Relief_w;
			float4 _MixMipActual=min(mip_selector.xxxx + rtp_mipoffset_color+MIPmult4567, 6);
			half4 c;
			float4 gloss;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, _MixMipActual.xx)); col = splat_control2.x * c.rgb; gloss.r = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, _MixMipActual.yy)); col += splat_control2.y * c.rgb; gloss.g = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, _MixMipActual.zz)); col += splat_control2.z * c.rgb; gloss.b = c.a;
			c = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, _MixMipActual.ww)); col += splat_control2.w * c.rgb; gloss.a = c.a;
			
			float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control2-0.5)*(2+RTP_AOsharpness)),0.25))*RTP_AOamp*dot(splat_control2,RTP_AO_4567);
			col*=heightblend_AO;
			gloss*=heightblend_AO;
		
			#if !defined(_4LAYERS) && defined(RTP_HARD_CROSSPASS)
				heightblend_AO=1-saturate(1-saturate((splat_controlB_coverage-0.5)*(2+RTP_AOsharpness)))*RTP_AOamp*dot(splat_control2,RTP_AO_4567)*0.5;
				col*=heightblend_AO;
				gloss*=heightblend_AO;				
				//float heightblend_AO2=1-saturate(1-saturate((splat_controlB_coverage-0.5)*20));
				//col=lerp(global_color_value.rgb*heightblend_AO, col, heightblend_AO2);
			#endif
						
			o.Gloss = dot(gloss, splat_control2);
			o.Specular = 0.03+dot(_Spec4567, splat_control2);
			
			#ifdef RTP_UV_BLEND
//				_MixMipActual=min(_MixMipActual+log2(_MixScale4567), 6);
//				float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
//				_multMix-=hi_mip_adjustMix;
//				_offMix+=0.5*hi_mip_adjustMix;
//
//				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale4567.xxyy)*_multMix+_offMix;
//				uvSplat01M.zw+=float2(0.5,0);
//				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale4567.zzww)*_multMix+_offMix;
//				uvSplat23M+=float4(0,0.5,0.5,0.5);			

				_MixMipActual=min(_MixMipActual+log2(_MixScale89AB), 6);
				float4 hi_mip_adjustMix=exp2(_MixMipActual)*_SplatAtlasC_TexelSize.x;
				_multMix-=hi_mip_adjustMix;
				_offMix+=0.5*hi_mip_adjustMix;

				uvSplat01M=frac(IN._uv_Relief.xyxy*_MixScale89AB.xxyy)*_multMix+_offMix;
				uvSplat01M.zw+=float2(0.5,0);
				uvSplat23M=frac(IN._uv_Relief.xyxy*_MixScale89AB.zzww)*_multMix+_offMix;
				uvSplat23M+=float4(0,0.5,0.5,0.5);	
								
				half3 colBlend = splat_control2.x * UV_BLEND_ROUTE_LAYER_4;
				colBlend += splat_control2.y * UV_BLEND_ROUTE_LAYER_5;
				colBlend += splat_control2.z * UV_BLEND_ROUTE_LAYER_6;
				colBlend += splat_control2.w * UV_BLEND_ROUTE_LAYER_7;
			#endif
			
			#if defined(RTP_UV_BLEND)
				colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control2, _MixSaturation4567));
				col=lerp(col, col*colBlend*2, blendVal);			
			#endif	
						
#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			#ifdef RTP_HARD_CROSSPASS
				col=lerp(col, global_color_value.rgb, _uv_Relief_w);
			#else
				col=lerp(col, global_color_value.rgb*splat_controlB_coverage, _uv_Relief_w);
			#endif
			o.Gloss*=(1-_uv_Relief_w);
			o.Specular=lerp(o.Specular, 0.5, _uv_Relief_w);
			}
#endif	

			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatC0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control2, _VerticalTexture4567) );
			#endif			
			
//#ifdef RTP_SNOW
//}
//#endif		

			// snow color
	 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO) || defined(RTP_47SHADING_PM)) )
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7) )
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_4)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif			
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_5)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat01.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif			
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif				
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_6)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.xy, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif			
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatAtlasC, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif				
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_7)
					half4 csnow = tex2Dlod(_SplatAtlasB, float4(uvSplat23.zw, min(mip_selector + rtp_mipoffset_color,6)));
					GETrtp_snow_TEX
				#endif		
			#endif
			#endif
			// eof snow color
			
	 		#if !defined(RTP_HARD_CROSSPASS) || ( defined(RTP_HARD_CROSSPASS) && (defined(RTP_47SHADING_POM_HI) || defined(RTP_47SHADING_POM_MED) || defined(RTP_47SHADING_POM_LO) || defined(RTP_47SHADING_PM)) )
			#if defined(RTP_SUPER_DETAIL) && defined (RTP_SUPER_DTL_MULTS) && !defined(RTP_SIMPLE_SHADING) && !defined(RTP_WETNESS) && !defined(RTP_REFLECTION)
				float4 super_detail=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy*_SuperDetailTiling, mip_selector+rtp_mipoffset_superdetail));
				
				float far_blend;
				far_blend=lerp(0, global_bump_val.b, dot(splat_control2, _SuperDetailStrengthMultASelfMaskFar4567));
				col=lerp(col, col*super_detail.z*2, far_blend*dot(splat_control2, _SuperDetailStrengthMultA4567));
				far_blend=lerp(0, global_bump_val.a, dot(splat_control2, _SuperDetailStrengthMultBSelfMaskFar4567));
				col=lerp(col, col*super_detail.w*2, far_blend*dot(splat_control2, _SuperDetailStrengthMultB4567));
			#endif
			#endif
						
			////////////////////////////////
			// water
			//
			float4 water_splat_control=splat_control2;
 			#if defined(RTP_REFLECTION) 
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_LayerReflection=0;
				#else
					TERRAIN_LayerReflection=dot(splat_control2, TERRAIN_LayerReflection4567);
				#endif
 			#endif
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=0;
				#else
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection4567);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlB_normalized, TERRAIN_LayerWetStrength4567);
				TERRAIN_WetSpecularity=dot(water_splat_control, TERRAIN_WetSpecularity4567);
				TERRAIN_WaterGloss=dot(water_splat_control, TERRAIN_WaterGloss4567);
				TERRAIN_WaterColor=half4( dot(splat_controlB_normalized, TERRAIN_WaterColorR4567), dot(splat_controlB_normalized, TERRAIN_WaterColorG4567), dot(splat_controlB_normalized, TERRAIN_WaterColorB4567), dot(splat_controlB_normalized, TERRAIN_WaterColorA4567) );
				
				float TERRAIN_WaterLevel=dot(water_splat_control, TERRAIN_WaterLevel4567);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlB_normalized, TERRAIN_WaterLevelSlopeDamp4567);
				//float TERRAIN_Flow=dot(water_splat_control, TERRAIN_Flow4567);
				float TERRAIN_WaterEdge=dot(water_splat_control, TERRAIN_WaterEdge4567);
				//float TERRAIN_Refraction=dot(water_splat_control, TERRAIN_Refraction4567);
				//float TERRAIN_WetRefraction=dot(water_splat_control, TERRAIN_WetRefraction4567);
			
				TERRAIN_LayerWetStrength*=saturate(2- water_mask*2-perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
				#ifdef RTP_SNOW
				TERRAIN_LayerWetStrength*=saturate(1-snow_val);
				#endif
				if (TERRAIN_LayerWetStrength>0) {
					wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
					TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
					TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH);
					
					p = saturate((TERRAIN_WaterLevel-actH)*TERRAIN_WaterEdge);
					p*=p;
				}
			#endif
	        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
		        GlossDry=o.Gloss;
		    #endif
	        #if defined(RTP_WETNESS)
				#ifdef RTP_CAUSTICS
					TERRAIN_WetSpecularity*=1-damp_fct_caustics;
				#endif	        
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, 0.2);//saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=dot(splat_controlB_normalized, TERRAIN_WaterOpacity4567)*p; // gladsze przejscie (po nieskompresowanym splat_control)
	 					        
		        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*TERRAIN_WetDarkening;
	        #endif
			// water
			////////////////////////////////	
							
	 	}
		#endif
	 	
	 	IN_uv_Relief_Offset.xy=IN._uv_Relief.xy;
	}
	
	float3 norm_snowCov=o.Normal;
	o.Normal+=norm_far*lerp(rtp_perlin_start_val,1, _uv_Relief_w)*_BumpMapGlobalStrengthPerLayer;	
	
	#ifdef COLOR_MAP
		#ifdef _4LAYERS
			global_color_blend *= dot(splat_control1, _GlobalColorPerLayer89AB);
		#else
			#ifdef RTP_HARD_CROSSPASS
				global_color_blend *= splat_controlA_coverage>splat_controlB_coverage ? dot(splat_control1, _GlobalColorPerLayer89AB) : dot(splat_control2, _GlobalColorPerLayer4567);
			#else
				global_color_blend *= dot(splat_control1, _GlobalColorPerLayer89AB) + dot(splat_control2, _GlobalColorPerLayer4567);
			#endif		
		#endif
		#ifdef RTP_WETNESS
			global_color_blend*=(1-p);
		#endif
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col=lerp(col, col*global_color_value.rgb*2, global_color_blend);
		#else
			col=lerp(col, global_color_value.rgb, global_color_blend);
		#endif
	#endif
	
	#ifdef RTP_SNOW
		IN_uv_Relief_Offset.xy=lerp(IN_uv_Relief_Offset.xy, IN._uv_Relief.xy, snow_depth_lerp);
	
		#ifdef COLOR_MAP
			snow_val = snow_const + rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow.xxx)+rtp_snow_strength*2;
		#else
			snow_val = snow_const + rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow+rtp_snow_strength*2;
		#endif
		snow_val*=rtp_snow_layer_damp;
		snow_val -= rtp_snow_slope_factor*saturate( 1 - dot( (norm_snowCov*0.7+norm_for_snow*0.3), flat_dir.xyz) - 0*dot( norm_for_snow, flat_dir.xyz));
		
		snow_val=saturate(snow_val);
		snow_val=pow(abs(snow_val), rtp_snow_edge_definition);
		rtp_snow_color_tex=lerp(rtp_snow_color.rgb, rtp_snow_color_tex, _uv_Relief_z);
		#ifdef COLOR_MAP
			half3 global_color_value_desaturated=dot(global_color_value.rgb, 0.37);//0.3333333); // będzie trochę jasniej
			#ifdef COLOR_MAP_BLEND_MULTIPLY
				rtp_snow_color_tex=lerp(rtp_snow_color_tex, rtp_snow_color_tex*global_color_value_desaturated.rgb*2, min(0.4,global_color_blend*0.7) );
			#else
				rtp_snow_color_tex=lerp(rtp_snow_color_tex, global_color_value_desaturated.rgb, min(0.4,global_color_blend*0.7) );
			#endif
		#endif
		col=lerp( col, rtp_snow_color_tex, snow_val );
		
		#if defined(RTP_SNW_CHOOSEN_LAYER_NORM_0) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_1) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_2) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_3) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_4) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_5) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_6) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_7)
			float3 n;
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_0
				n.xy=tex2Dlod(_BumpMap89, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).rg*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_1
				n.xy=tex2Dlod(_BumpMap89, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).ba*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_2
				n.xy=tex2Dlod(_BumpMapAB, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).rg*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_3
				n.xy=tex2Dlod(_BumpMapAB, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).ba*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_4
				n.xy=tex2Dlod(_BumpMap45, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).rg*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_5
				n.xy=tex2Dlod(_BumpMap45, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).ba*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_6
				n.xy=tex2Dlod(_BumpMap67, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).rg*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_7
				n.xy=tex2Dlod(_BumpMap67, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).ba*2-1;
			#endif
			n.xy*=_uv_Relief_z;
			n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
			float3 snow_normal=lerp(o.Normal, n, snow_depth_lerp );
		#else
			float3 snow_normal=o.Normal;
		#endif
				
		snow_normal=norm_for_snow + 2*snow_normal*_uv_Relief_z;
		
		snow_normal=normalize(snow_normal);
		o.Normal=lerp(o.Normal, snow_normal, snow_val);
//		#if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
//			o.Gloss=lerp(GlossDry, lerp(rtp_snow_gloss/2, rtp_snow_gloss, _uv_Relief_z), snow_val);
//		#else
			o.Gloss=lerp(o.Gloss, lerp(rtp_snow_gloss/2, rtp_snow_gloss, _uv_Relief_z), snow_val);
//		#endif
		 #ifdef RTP_REFLECTION
		 	GlossDry=lerp(GlossDry, o.Gloss, snow_val);
		#endif
		o.Specular=lerp(o.Specular, rtp_snow_specular, snow_val);
	#endif
	
	o.Albedo=col;
	float3 norm_edge=o.Normal;
	#ifdef RTP_NORMALGLOBAL
		float3 global_norm;
		#if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
			global_norm.xy=tex2D(_NormalMapGlobal, IN.uv_Control).xy * 2 - 1;
		#else
			global_norm.xy=tex2D(_NormalMapGlobal, IN.uv_Control).wy * 2 - 1;
		#endif	
		global_norm.xy*=_TERRAIN_trees_shadow_values.w;
		global_norm.z=sqrt(1 - saturate(dot(global_norm.xy, global_norm.xy)));
		o.Normal+=global_norm;
	#endif
	o.Normal=normalize(o.Normal);
	
	#ifdef RTP_TREESGLOBAL	
		float4 pixel_trees_val=tex2D(_TreesMapGlobal, IN.uv_Control);
		float pixel_trees_blend_val=saturate((pixel_trees_val.r+pixel_trees_val.g+pixel_trees_val.b)*_TERRAIN_trees_pixel_values.z);
		pixel_trees_blend_val*=saturate((IN._uv_Relief.w - _TERRAIN_trees_pixel_values.x) / _TERRAIN_trees_pixel_values.y);
		o.Albedo=lerp(o.Albedo, pixel_trees_val.rgb, pixel_trees_blend_val);

		float pixel_trees_shadow_val=saturate((IN._uv_Relief.w - _TERRAIN_trees_shadow_values.x) / _TERRAIN_trees_shadow_values.y);
		pixel_trees_shadow_val=lerp(1, pixel_trees_val.a, pixel_trees_shadow_val);
		o.Albedo*=lerp(_TERRAIN_trees_shadow_values.z, 1, pixel_trees_shadow_val);
	#endif	

	
    #if defined(RTP_REFLECTION) && !defined(RTP_SHOW_OVERLAPPED)
	#if defined(RTP_SIMPLE_SHADING)	
	
	#ifdef FAR_ONLY
	if (false) {
	#else
	if (_uv_Relief_z>0) {
	#endif

	#endif
		mip_selector=saturate(mip_selector-10);// bug in compiler for forward pass, we have to specify mip level indirectly (can't be treated constant)
		#if defined(RTP_TRIPLANAR)
			float t=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Aux.xy + norm_edge.xy*TERRAIN_ReflDistortion, mip_selector)).a;
		#else
			float t=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Aux.zw + norm_edge.xy*TERRAIN_ReflDistortion, mip_selector)).a;
		#endif		
		#ifdef RTP_WETNESS
			#if defined(RTP_SIMPLE_SHADING)
				float ReflectionStrength=max(TERRAIN_LayerReflection, TERRAIN_WetReflection*TERRAIN_LayerWetStrength)*_uv_Relief_z;
			#else
				float ReflectionStrength=max(TERRAIN_LayerReflection, TERRAIN_WetReflection*TERRAIN_LayerWetStrength);
			#endif
		#else
			#if defined(RTP_SIMPLE_SHADING)
				float ReflectionStrength=TERRAIN_LayerReflection*_uv_Relief_z;
			#else
				float ReflectionStrength=TERRAIN_LayerReflection;
			#endif
		#endif
		#ifdef RTP_SNOW
			#if defined(RTP_SIMPLE_SHADING)
				ReflectionStrength=lerp(ReflectionStrength, rtp_snow_reflectivness, snow_val)*_uv_Relief_z;
			#else
				ReflectionStrength=lerp(ReflectionStrength, rtp_snow_reflectivness, snow_val);
			#endif
		#endif
		#if defined(RTP_SIMPLE_SHADING)
			rim*=max(p*saturate(TERRAIN_WaterGloss+0.3)*_uv_Relief_z, lerp(GlossDry*saturate(ReflectionStrength), saturate(GlossDry+ReflectionStrength-1), saturate(ReflectionStrength-1)) );
		#else
			rim*=max(p*saturate(TERRAIN_WaterGloss+0.3), lerp(GlossDry*saturate(ReflectionStrength), saturate(GlossDry+ReflectionStrength-1), saturate(ReflectionStrength-1)) );
		#endif
		
		o.Emission = TERRAIN_ReflColorA.rgb*rim*t; // *TERRAIN_ReflColorA.a
		o.Albedo = lerp(o.Albedo, TERRAIN_ReflColorB.rgb, TERRAIN_ReflColorB.a*rim*(1-t));
	#if defined(RTP_SIMPLE_SHADING)	
	}
	#endif
	#endif

	float diff;
	#if !defined(LIGHTMAP_OFF) && defined (DIRLIGHTMAP_OFF)
		//IN.lightDir.z*=_TERRAIN_ExtrudeHeight;
		diff = max (0, dot (o.Normal, IN.lightDir.xyz))*lerp(2,2-_BumpMapGlobalStrengthPerLayer,_uv_Relief_z);
		diff = lerp(diff, 1, _uv_Relief_w*0.75f); // w dużej odleglosci nakladamy tylko 25% diffa
		diff = diff*_TERRAIN_LightmapShading+(1-_TERRAIN_LightmapShading);
		o.Albedo*=diff;
		o.Gloss*=diff;
	#endif
	
	#if defined(RTP_CROSSPASS_HEIGHTBLEND)
		#if defined(_12LAYERS)
//			splat_control1 = tHA*splat_controlA; // 3 (8-11)
//				
//			tHB=saturate(tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xy)+0.001); // 1 (0-3)
//			float4 splat_controlB = tex2D(_Control1, IN.uv_Control);
//			float4 splat_control2 = tHB*splat_controlB;
//			float4 tHC=saturate(tex2D(_TERRAIN_HeightMap2, IN._uv_Relief.xy)+0.001); //2 (4-7)
//			float4 splat_controlC = tex2D(_Control2, IN.uv_Control);
//			float4 splat_control3 = tHC*splat_controlC;
//							
//			splat_control1_mid=splat_control1*splat_control1;
//			splat_control1_close=splat_control1_mid*splat_control1_mid;
//			splat_control1=lerp(lerp(splat_control1_mid, splat_control1, _uv_Relief_w), splat_control1_close, _uv_Relief_z);
//			float4 splat_control2_mid=splat_control2*splat_control2;
//			float4 splat_control2_close=splat_control2_mid*splat_control2_mid;
//			splat_control2=lerp(lerp(splat_control2_mid, splat_control2, _uv_Relief_w), splat_control2_close, _uv_Relief_z);
//			float4 splat_control3_mid=splat_control3*splat_control3;
//			float4 splat_control3_close=splat_control3_mid*splat_control3_mid;
//			splat_control3=lerp(lerp(splat_control3_mid, splat_control3, _uv_Relief_w), splat_control3_close, _uv_Relief_z);
//			
//			float normalize_sum=dot(splat_control1, 1)+dot(splat_control2, 1)+dot(splat_control3, 1);
//			splat_control1 /= normalize_sum;
//			//splat_control2 /= normalize_sum;
//			//splat_control3 /= normalize_sum;
//			o.Alpha=dot(splat_control1,1);
			// we've run out of samplers
			o.Alpha=total_coverage;
		#else
			splat_control1 = tHA*splat_controlA; // 2 (4-7)
			
			tHB=saturate(tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xy)+0.001); // 1 (0-3)
			float4 splat_controlB = tex2D(_Control1, IN.uv_Control);
			float4 splat_control2 = tHB*splat_controlB;
			
			splat_control1_mid=splat_control1*splat_control1;
			splat_control1_close=splat_control1_mid*splat_control1_mid;
			splat_control1=lerp(lerp(splat_control1_mid, splat_control1, _uv_Relief_w), splat_control1_close, _uv_Relief_z);
			float4 splat_control2_mid=splat_control2*splat_control2;
			float4 splat_control2_close=splat_control2_mid*splat_control2_mid;
			splat_control2=lerp(lerp(splat_control2_mid, splat_control2, _uv_Relief_w), splat_control2_close, _uv_Relief_z);
			
			float normalize_sum=dot(splat_control1, 1)+dot(splat_control2, 1);
			splat_control1 /= normalize_sum;
			splat_control2 /= normalize_sum;
			float instabilityFactor=saturate(normalize_sum*2048);
			o.Alpha=lerp(total_coverage, dot(splat_control1,1), instabilityFactor);

			// 0-3			
			float hbAOCrossPass=(dot(splat_control1,RTP_AO_0123)+dot(splat_control2,RTP_AO_89AB))*RTP_AOamp*2;
			float heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control2-0.5)*(2+RTP_AOsharpness)),0.25))*hbAOCrossPass;
			o.Albedo*=heightblend_AO;
			o.Gloss*=heightblend_AO;
			
			// 4-7
//			heightblend_AO=1-saturate(1-dot(saturate(abs(splat_control1-0.5)*(2+RTP_AOsharpness)),0.25))*hbAOCrossPass;
//			o.Albedo*=heightblend_AO;
//			o.Gloss*=heightblend_AO;
						
			//float heightblend_AO=1-saturate(1-saturate(abs(o.Alpha-0.5)*(2+RTP_AOsharpness)))*RTP_AOamp;
			//o.Albedo*=heightblend_AO;
			//o.Gloss*=heightblend_AO;			
		#endif	
	#else
		o.Alpha=total_coverage;
	#endif	
	
	#ifdef _4LAYERS
	o.Gloss=lerp(o.Gloss, saturate(o.Gloss+dot(splat_control1, _FarGlossCorrection89AB)), _uv_Relief_w);
	#else
	o.Gloss=lerp(o.Gloss, saturate(o.Gloss+dot(splat_control1, _FarGlossCorrection89AB) + dot(splat_control2, _FarGlossCorrection4567)), _uv_Relief_w);
	#endif
	
	#ifdef RTP_CAUSTICS
	{
		if (damp_fct_caustics>0 && _uv_Relief_w<1) {
			float tim=_Time.x*TERRAIN_CausticsAnimSpeed;
			IN_uv_Relief_Offset.xy*=TERRAIN_CausticsTilingScale;
			float3 _Emission=tex2Dlod(TERRAIN_CausticsTex, float4(IN_uv_Relief_Offset.xy+float2(tim, tim), mip_selector.xx+rtp_mipoffset_caustics.xx) ).rgb;
			_Emission+=tex2Dlod(TERRAIN_CausticsTex, float4(IN_uv_Relief_Offset.xy+float2(-tim, -tim*0.873), mip_selector.xx+rtp_mipoffset_caustics.xx) ).rgb;
			_Emission+=tex2Dlod(TERRAIN_CausticsTex, float4(IN_uv_Relief_Offset.xy*1.1+float2(tim, 0), mip_selector.xx+rtp_mipoffset_caustics.xx) ).rgb;
			_Emission+=tex2Dlod(TERRAIN_CausticsTex, float4(IN_uv_Relief_Offset.xy*0.5+float2(0, tim*0.83), mip_selector.xx-1+rtp_mipoffset_caustics.xx) ).rgb;
			_Emission=saturate(_Emission-1.6);
			_Emission*=_Emission;
			_Emission*=_Emission;
			_Emission*=TERRAIN_CausticsColor.rgb*3;
			_Emission*=damp_fct_caustics;
			_Emission*=(1-_uv_Relief_w);
			o.Emission+=_Emission;
		}
	} 
	#endif
	// EOF regular mode
	#endif
} 