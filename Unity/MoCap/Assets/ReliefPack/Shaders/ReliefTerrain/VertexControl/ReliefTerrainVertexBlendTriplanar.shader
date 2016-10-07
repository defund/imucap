#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

//
// Vertex paint / 4 layers version of RTP shader (optionally 2 or 3),
// in triplanar mode (enabled below) can be easily adopted for voxel terrains (I believe)
// most features are disabled by default, but if you need then (for example dynamic snow or so)
// - go on and look around to quickly customize it
//
// (C) Tomasz Stobierski 2013
//
//
// (don't miss line 267 where I defined UV blend routing - UV_BLEND_SRC_2 - means 3rd layer blends with each other to depatternize our structure)
//
Shader "Relief Pack/ReliefTerrainVertexBlendTriplanar" {
	//
	// as RTP has so many features, number of material properties are huge (and not managed via script but default material property inspector)
	// for this reason commented out are those unused (disabled in shader code)
	// Uncomment properties needed and look for #defines section below to add/remove features manually
	// Sometimes they depends on each other and can't be used together, so beware (as you adjust them on your own, not using RTP_LODmanager)
	//
	Properties {
		// tiling
//		_TERRAIN_ReliefTransform ("Tiling transform (XY - mult, ZW - offset)", Vector) = (0.1,0.1,0,0)
		// used in triplanar
		_TERRAIN_ReliefTransformTriplanarZ ("Triplanar tile size", Float) = 3
		
//		// global colormap
//		_ColorMapGlobal ("Global colormap (RGBA)", 2D) = "white" {}
//		_GlobalColorMapBlendValues ("Global colormap blending (XYZ)", Vector) = (0.3,0.6,0.8,0)
//		_GlobalColorMapSaturation ("Global colormap saturation", Range(0,1)) = 0.8
//		_GlobalColorPerLayer0123 ("Global colormap per layer (XYZW - layers 0-3)", Vector) = (1.0, 1.0, 1.0, 1.0)
		
		// like in (RTP ReliefTerrain inspector Settings/Main)
		// offset added when calculating MIP levels (negative - sharpen, positive - blur)
		_RTP_MIP_BIAS ("MIP Bias", Range(-1,1)) = 0
		
		// detail textures / normal maps (combined), heightmap (combined)
		_SplatA0 ("Detailmap 0 (RGB+A spec)", 2D) = "black" {}
		_SplatA1 ("Detailmap 1 (RGB+A spec)", 2D) = "black" {}
		_SplatA2 ("Detailmap 2 (RGB+A spec)", 2D) = "black" {}
		_SplatA3 ("Detailmap 3 (RGB+A spec)", 2D) = "black" {}
		_BumpMap01 ("Bumpmap combined 0+1 (RG+BA)", 2D) = "grey" {}
		_BumpMap23 ("Bumpmap combined 2+3 (RG+BA)", 2D) = "grey" {}
		_TERRAIN_HeightMap ("Heightmap combined (RGBA - layers 0-3)", 2D) = "white" {}
		//PER_LAYER_HEIGHT_MODIFIER0123 ("Extrude reduction per layer (XYZW - layers 0-3)", Vector) = (0, 0, 0, 0)
		
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)		
		_Shininess ("Shininess", Range (0.01, 1)) = 0.5
		_FColor ("Fog color (RGB)", Color) = (1,1,1,0)
		_Fdensity ("Fog density", Float) = 0.002
				
		// perlin normal mapping (channels RG) with wetmask (G) and reflection map (A)
		_BumpMapGlobal ("Perlin normal combined w. water & reflection map (RG+B+A)", 2D) = "black" {}
		// (0.1 means that one perlin tile is 10 detail tiles)
		_BumpMapGlobalScale ("       Perlin normal tiling", Float) = 0.1
		rtp_mipoffset_globalnorm_offset ("       MIP offset", Range(0,5)) = 0
		_MIPmult0123 ("       MIP offset at far distance", Vector) = (0,0,0,0)
		_FarNormalDamp ("       Far normal damp", Range(0,1)) = 0
		_BumpMapGlobalStrength0123 ("       Perlin normal strength per layer (XYZW - layers 0-3)", Vector) = (0.3, 0.3, 0.3, 0.3)
		
		// UV blend
		_blend_multiplier ("UV blend multiplier", Range(0,1)) = 1
		// (0.2 means that one blended tile is 5 detail tiles)
		_MixScale0123 ("       UV blend tiling (XYZW - layers 0-3)", Vector) = (0.2, 0.2, 0.2, 0.2)
		_MixBlend0123 ("       UV blend value (XYZW - layers 0-3)", Vector) = (0.5, 0.5, 0.5, 0.5)
		_MixSaturation0123 ("       UV blend saturation (XYZW - layers 0-3)", Vector) = (1.0, 1.0, 1.0, 1.0)
		
		// specularity per layer
		_Spec0123 ("Layer specularity (XYZW - layers 0-3)", Vector) = (0.5, 0.5, 0.5, 0.5)

		// near / mid / far distance definitions
		_TERRAIN_distance_start ("Near distance fade start", Float) = 2
		_TERRAIN_distance_transition ("       Near distance fade length", Float) = 22
		_TERRAIN_distance_start_bumpglobal ("       Far distance fade start (perlin)", Float) = 24
		_TERRAIN_distance_transition_bumpglobal ("       Far distance fade length", Float) = 50
		
//		// superdetail
//		_SuperDetailTiling ("Superdetail tiling", Float) = 2
//		_SuperDetailStrengthNormal0123 ("       Superdetail strength per layer (XYZW - layers 0-3)", Vector) = (0.5, 0.5, 0.5, 0.5)
		
		// PM/POM
		_TERRAIN_ExtrudeHeight ("Parallax Mapping extrude height", Range(0.001,0.3)) = 0.06
//		// POM
//		_TERRAIN_DIST_STEPS ("       POM linear max steps", Float) = 20
//		_TERRAIN_WAVELENGTH  ("       POM wavelength", Float) = 2

//		// POM self-shadows
//		_TERRAIN_SelfShadowStrength ("       Shadow strength", Range(0,1)) = 0.5
//		_TERRAIN_SHADOW_STEPS ("       POM linear shadow max steps", Float) = 20
//		_TERRAIN_WAVELENGTH_SHADOWS  ("       POM shadow wavelength", Float) = 4
//		_TERRAIN_SHADOW_SMOOTH_STEPS ("       POM shadow smooth steps", Float) = 8
//		_TERRAIN_ShadowSmoothing  ("       POM shadow smoothing value", Float) = 1.5
//		_TERRAIN_ShadowColorization ("       POM shadow colorization", Color) = (0,0.4,0.6,0)

		// reflection
		//
		TERRAIN_ReflColorA ("Reflection color A (Emissive RGB)", Color) = (0.5,0.5,0.5,0)
		TERRAIN_ReflColorB ("       Reflection color B (Diffuse RGBA)", Color) = (0.0,0.5,0.9,0.3)
		TERRAIN_ReflDistortion ("       Reflection distortion", Range(0, 0.2)) = 0.1
		TERRAIN_ReflectionRotSpeed ("       Reflection rotation speed", Range(0, 2)) = 0.3
		TERRAIN_FresnelPow ("       Fresnel pow", Range(0.25,16)) = 2
		TERRAIN_FresnelOffset ("       Fresnel offset", Range(0,0.9)) = 0.25
		// range (0..2)  -  0-1 -> 0 .. detail_texture.a (GlossDry), 1-2 -> detail_texture.a .. 1
		TERRAIN_LayerReflection0123 ("       Layer reflection (XYZW - 0123)", Vector) = (1,1,1,1)

//		// water/wet
//		//
//		TERRAIN_GlobalWetness ("Global wetness", Range(0,1)) = 1
//		TERRAIN_WaterSpecularity ("       Water specularity", Range(0,1)) = 0.6
//		
//		TERRAIN_RippleMap ("       Ripplemap (RGB)", 2D) = "white" {}
//		TERRAIN_RippleScale ("       Ripplemap tiling", Float) = 4
//		TERRAIN_FlowScale ("       Flow strength", Range(0,1)) = 0.5
//		TERRAIN_FlowSpeed ("       Flow speed", Range(0,4)) = 0.5
//		TERRAIN_FlowMipOffset ("       Flow tex MIP offset", Float) = 0
//		
//		TERRAIN_RainIntensity ("       Rain intensity", Range(0,1)) = 1
//		TERRAIN_DropletsSpeed ("       Droplets Speed", Float) = 15
//		TERRAIN_WetDropletsStrength ("       Droplets on wet", Range(0,1)) = 0.1
//		
//		TERRAIN_mipoffset_flowSpeed ("       Flow speed to MIP offset", Range(0,1)) = 0.6
//
//		// water - per layer
//		TERRAIN_LayerWetStrength0123 ("Water strengh per layer (XYZW - 0123)", Vector) = (1,1,1,1)
//
//		TERRAIN_WaterLevel0123 ("       Water level per layer (XYZW - 0123)", Vector) = (0.5,0.5,0.5,0.5)
//		TERRAIN_WaterLevelSlopeDamp0123 ("       Water slope damp (XYZW - 0123)", Vector) = (0.1,0.1,0.1,0.1)
//		TERRAIN_WaterEdge0123 ("       Water edge definition (XYZW - 0123)", Vector) = (1,1,1,1)
//		TERRAIN_WaterGloss0123 ("       Water gloss (XYZW - 0123)", Vector) = (1,1,1,1)
//		TERRAIN_WaterOpacity0123 ("       Water opacity (XYZW - 0123)", Vector) = (0.2,0.2,0.2,0.2)
//		TERRAIN_Refraction0123 ("       Water refraction (XYZW - 0123)", Vector) = (0.02, 0.02, 0.02, 0.02)
//		TERRAIN_WetRefraction0123 ("       Wet refraction factor (XYZW - 0123)", Vector) = (0.5, 0.5, 0.5, 0.5)
//		TERRAIN_Flow0123 ("       Flow strength (XYZW - 0123)", Vector) = (1, 1, 1, 1)
//		TERRAIN_WetSpecularity0123 ("       Wet specularity (XYZW - 0123)", Vector) = (0.5, 0.5, 0.5, 0.5)
//		// range (0..2)  -  0-1 -> 0 .. detail_texture.a (GlossDry), 1-2 -> detail_texture.a .. 1
//		TERRAIN_WetReflection0123 ("       Wet reflection (XYZW - 0123)", Vector) = (1, 1, 1, 1)
//		TERRAIN_WaterColor0 ("       Color layer 0", Color) = (0.5, 0.7, 1, 0.5)
//		TERRAIN_WaterColor1 ("       Color layer 1", Color) = (0.5, 0.7, 1, 0.5)
//		TERRAIN_WaterColor2 ("       Color layer 2", Color) = (0.5, 0.7, 1, 0.5)
//		TERRAIN_WaterColor3 ("       Color layer 3", Color) = (0.5, 0.7, 1, 0.5)

//		// vertical texturing
//		//
//		_VerticalTexture ("Vertical texture (RGB)", 2D) = "grey" {}
//		_VerticalTextureTiling ("       Texture tiling", Float) = 50
//		_VerticalTextureGlobalBumpInfluence ("       Perlin distortion", Range(0,0.3)) = 0.01
//		_VerticalTexture0123 ("       Strength per layer (XYZW - 0123)", Vector) = (0.5, 0.5, 0.5, 0.5)
		
		// snow
		//
		rtp_snow_strength ("Snow strength", Range(0,1)) = 1
		rtp_snow_strength_per_layer0123 ("       Strength per layer (XYZW - 0123)", Vector) = (1, 1, 1, 1)
		rtp_global_color_brightness_to_snow ("       Global color brightness to snow", Range(0,1)) = 1
		rtp_snow_slope_factor ("       Slope damp factor", Range(0,4)) = 2
		rtp_snow_edge_definition ("       Edges definition", Range(0.25,20)) = 2
		// in [m] (where snow start to appear
		rtp_snow_height_treshold ("       Coverage height theshold", Float) = -100
		rtp_snow_height_transition ("       Coverage height length", Float) = 300
		rtp_snow_color("       Color", Color) = (0.9,0.9,1,1)
		rtp_snow_gloss("       Gloss", Range(0,1)) = 0.6
		rtp_snow_specular("       Specularity", Range(0.01,0.99)) = 0.5
		rtp_snow_deep_factor("       Deep factor", Range(0,6)) = 2
		rtp_snow_reflectivness ("       Reflectivness", Range(0,1)) = 0.4
	}
	
	SubShader {
		Tags {
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}
		LOD 700
		Fog {Mode Off }
		ZTest LEqual
		CGPROGRAM
		// remove noforwardadd below if you don't like to compromise additional lighting quality (but with multiple lights in forward we'll have to render in many passes, too)
		#pragma surface surf BlinnPhong vertex:vert finalcolor:customFog fullforwardshadows addshadow noforwardadd
		#pragma target 3.0
		#pragma only_renderers d3d9 opengl d3d11
		#pragma glsl
		// switch between shader level of detail using Shader.SetKeyword or using material keyword handling in Unity4.2
		// you can also add:
		// 		RTP_POM_SHADING_HI (POM with smooth self shadowing)
		// 		RTP_POM_SHADING_MED (POM with hard self shadowing)
		// 		RTP_POM_SHADING_LO (POM w/o self shadowing)
		// note that POM shading doesn't work in triplanar
		#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
		#include "UnityCG.cginc"
		
		// if you're using this shader on arbitrary mesh you can control splat coverage via vertices colors
		// note that you won't be able to blend objects when VERTEX_COLOR_CONTROL is defined
		#define VERTEX_COLOR_CONTROL
		
		// force shader to not use perlin normalmap (causes water, reflections, snow and superdetail to be disabled, too !)
		//#define FORCE_DISABLE_PERLIN
		
		// self explainable - detail colors 2,3 and bumpmap 23 not used then (shader will run a bit faster)
		// R vertex color used only (1st layer), 2nd layer coverage taken as (1-R)
		//#define USE_2_LAYERS_ONLY
		// as above, but we/re using RGB vertex color channels, A is free for other usage like water/snow coverage below
		//#define USE_3_LAYERS_ONLY
		
		// when water or snow is used you can use below defines to specify vertex channel that handles coverage (by default A channel)
		// NOTE that vertex color channel specified interferes with one of the layer splat control (4th by default), so it' only does make sense using with USE_2_LAYERS_ONLY or USE_3_LAYERS_ONLY defines
		//#define VERTEX_COLOR_TO_WATER_COVERAGE IN.color.a
		//#define VERTEX_COLOR_TO_SNOW_COVERAGE IN.color.a
		
		// we're texturing in local space
		#define LOCAL_SPACE_UV
		
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// you might want to tweak below defines by hand (commenting/uncommenting, but it's recommended to leave it for RTP_manager)
//
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// to compute far color basing only on global colormap
//#define SIMPLE_FAR

// uv blending
#define RTP_UV_BLEND
//#define RTP_DISTANCE_ONLY_UV_BLEND
//// comment below detail when not needed
//#define RTP_SUPER_DETAIL
// comment below if you don't use snow features
#define RTP_SNOW
// layer number taken as snow normal for near distance (for deep snow cover)
//#define RTP_SNW_CHOOSEN_LAYER_NORM_0
// layer number taken as snow color/gloss for near distance
//#define RTP_SNW_CHOOSEN_LAYER_COLOR_0

// comment if you don't need global color map
//#define COLOR_MAP
// if not defined global color map will be blended (lerp)
//#define COLOR_MAP_BLEND_MULTIPLY

// when defined you can see where layers 0-3 overlap layers 4-7 in 8 per pass mode. These areas costs higher
//  (note that when RTP_HARD_CROSSPASS is defined you won't see any overlapping areas)
//#define RTP_SHOW_OVERLAPPED
// when defined we don't calculate overlapping 0-3 vs 4-7 layers in 8 layers mode, but take "higher"
// it's recommended to use this define for significantly better performance
// undef it only when you really need smooth transitions between overlapping groups
//#define RTP_HARD_CROSSPASS

// triplanar
#define RTP_TRIPLANAR

// vertical texture
//#define RTP_VERTICAL_TEXTURE

// we use wet (can't be used with superdetail as globalnormal texture BA channels are shared)
//#define RTP_WETNESS
// water droplets
//#define RTP_WET_RIPPLE_TEXTURE
// if defined water won't handle flow nor refractions
//#define SIMPLE_WATER

// reflection map
#define RTP_REFLECTION
//#define RTP_ROTATE_REFLECTION

// if you don't use extrude reduction in layers properties (equals 0 everywhere)
// you can comment below - POM will run a bit faster
//#define USE_EXTRUDE_REDUCTION

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// don't touch below defines
//
#define UV_BLEND_SRC_0 (tex2Dlod(_SplatA0, float4(uvSplat01M.xy, _MixMipActual.xx)).rgb)
#define UV_BLEND_SRC_1 (tex2Dlod(_SplatA1, float4(uvSplat01M.zw, _MixMipActual.yy)).rgb)
#define UV_BLEND_SRC_2 (tex2Dlod(_SplatA2, float4(uvSplat23M.xy, _MixMipActual.zz)).rgb)
#define UV_BLEND_SRC_3 (tex2Dlod(_SplatA3, float4(uvSplat23M.zw, _MixMipActual.ww)).rgb)
//
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//
// for example, when you'd like layer 3 to be source for uv blend on layer 0 you'd set it like this:
//   #define UV_BLEND_ROUTE_LAYER_0 UV_BLEND_SRC_3
// HINT: routing one layer into all will boost performance as only 1 additional texture fetch will be performed in shader (instead of up to 8 texture fetches in default setup)
//
#define UV_BLEND_ROUTE_LAYER_0 UV_BLEND_SRC_2
#define UV_BLEND_ROUTE_LAYER_1 UV_BLEND_SRC_2
#define UV_BLEND_ROUTE_LAYER_2 UV_BLEND_SRC_2
#define UV_BLEND_ROUTE_LAYER_3 UV_BLEND_SRC_2

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define _4LAYERS
// we need it in triplanar to get correct PM and bumpmapping
#define FORCE_APPROX_TANGENTS

// disabling global normal disables water, reflection, snow, vertical texture and superdetail
#if defined(FORCE_DISABLE_PERLIN)
	#if defined(RTP_WETNESS)
		#undef RTP_WETNESS
	#endif
	#if defined(RTP_REFLECTION)
		#undef RTP_REFLECTION
	#endif
	#if defined(RTP_SNOW)
		#undef RTP_SNOW
	#endif
	#if defined(RTP_SUPER_DETAIL)
		#undef RTP_SUPER_DETAIL
	#endif
	#if defined(RTP_VERTICAL_TEXTURE)
		#undef RTP_VERTICAL_TEXTURE
	#endif
#endif

#ifdef COLOR_MAP
	#define  COLOR_DAMP_VAL (global_color_value.a)
#else
	#define  COLOR_DAMP_VAL 1
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

// wyłącz wet ripple kiedy mamy inne opcje wszystkie powlaczane
#if defined(RTP_MAPPED_SHADOWS) && defined(RTP_VERTICAL_TEXTURE) && defined(RTP_WETNESS) && defined(RTP_WET_RIPPLE_TEXTURE)
	#undef RTP_WET_RIPPLE_TEXTURE
#endif

#if defined(SHADER_API_D3D11) 
CBUFFER_START(rtpConstants)
#endif

sampler2D _SplatA0, _SplatA1, _SplatA2, _SplatA3;
sampler2D _BumpMap01, _BumpMap23;
sampler2D _ColorMapGlobal;
sampler2D _BumpMapGlobal;
float2 _terrain_size;
float _BumpMapGlobalScale;
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
float4 _MixScale0123, _MixBlend0123, _MixSaturation0123;
float4 _GlobalColorPerLayer0123;
float4 _Spec0123;
float4 _MIPmult0123;

sampler2D _TERRAIN_HeightMap;
float4 _TERRAIN_HeightMap_TexelSize;
float4 _SplatA0_TexelSize;
float4 _BumpMapGlobal_TexelSize;
float4 _TERRAIN_ReliefTransform;
float _TERRAIN_ReliefTransformTriplanarZ;
float _TERRAIN_DIST_STEPS;
float _TERRAIN_WAVELENGTH;

float _blend_multiplier;

float _Shininess;

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
float TERRAIN_WaterSpecularity;

sampler2D TERRAIN_RippleMap;
float4 TERRAIN_RippleMap_TexelSize;
float TERRAIN_RippleScale; // 4
float TERRAIN_FlowScale; // 1
float TERRAIN_FlowSpeed; // 0 - 3 (0.5)
float TERRAIN_FlowMipOffset; // 0

float TERRAIN_RainIntensity; // 1
float TERRAIN_DropletsSpeed; // 10
float TERRAIN_WetDropletsStrength; // 0-1

float TERRAIN_mipoffset_flowSpeed; // 0-5

// per layer
float4 TERRAIN_LayerWetStrength0123; // 0 - 1 (1)

float4 TERRAIN_WaterLevel0123; // 0 - 2 (0.5)
float4 TERRAIN_WaterLevelSlopeDamp0123; // 0.25 - 32 (2)
float4 TERRAIN_WaterEdge0123; // 1 - 16 (2)
float4 TERRAIN_WaterGloss0123; // 0 - 1 (1)
float4 TERRAIN_WaterOpacity0123; // 0 - 1 (0.3)
float4 TERRAIN_Refraction0123; // 0 - 0.04 (0.01)
float4 TERRAIN_WetRefraction0123; // 0 - 1 (0.25)
float4 TERRAIN_Flow0123; // 0 - 1 (0.1)
float4 TERRAIN_WetSpecularity0123; // 0 - 1 (0.5)
float4 TERRAIN_WetReflection0123; // 0-1 -> 0 .. color_textury.a (GlossDry), 1-2 -> color_textury.a .. 1
float4 TERRAIN_LayerReflection0123; // 0-1 -> 0 .. color_textury.a, 1-2 -> color_textury.a .. 1
half4 TERRAIN_WaterColorR0123;
half4 TERRAIN_WaterColorG0123;
half4 TERRAIN_WaterColorB0123;
half4 TERRAIN_WaterColorA0123;
// used when VERTEX_COLOR_CONTROL is defined
half4	TERRAIN_WaterColor0;
half4	TERRAIN_WaterColor1;
half4	TERRAIN_WaterColor2;
half4	TERRAIN_WaterColor3;


///////////////////////////////////////////

float _TERRAIN_distance_start;
float _TERRAIN_distance_transition;

float _TERRAIN_distance_start_bumpglobal;
float _TERRAIN_distance_transition_bumpglobal;
float4 _BumpMapGlobalStrength0123;
float _FarNormalDamp;

float _RTP_MIP_BIAS;

float4 PER_LAYER_HEIGHT_MODIFIER0123;

float _SuperDetailTiling;
float4 _SuperDetailStrengthNormal0123;

float4 _VerticalTexture_TexelSize;
sampler2D _VerticalTexture;
float _VerticalTextureTiling;
float _VerticalTextureGlobalBumpInfluence;
float4 _VerticalTexture0123;

float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer0123;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;

fixed4 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
float rtp_snow_deep_factor;
float rtp_snow_reflectivness;
#if defined(SHADER_API_D3D11) 
CBUFFER_END
#endif

float rtp_snow_strength;
float TERRAIN_GlobalWetness;

#ifdef UNITY_PASS_PREPASSFINAL
uniform float4 _WorldSpaceLightPosCustom;
#endif

struct Input {
	#if defined(VERTEX_COLOR_CONTROL)
		float2 uv_ColorMapGlobal : TEXCOORD0;
	#else
		float2 uv_Control : TEXCOORD0;
	#endif
	float4 _uv_Relief;
	float4 _uv_Aux;
	
	float4 _viewDir;
	float4 lightDir;
	
	// Geometry blend specific
	float4 color:COLOR;
};

fixed3 _FColor;
float _Fdensity;
void customFog (Input IN, SurfaceOutput o, inout fixed4 color) {
	float g=-log2(1-_Fdensity);
	float f=saturate(exp2(-g*IN._uv_Relief.w));
	color.rgb=lerp(_FColor, color.rgb, f);
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
    float4 Ripple = tex2Dlod(TERRAIN_RippleMap, UV);
    Ripple.xy = Ripple.xy * 2 - 1;

    float DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
    float TimeFrac = DropFrac - 1.0f + Ripple.z;
    float DropFactor = saturate(0.2f + Intensity * 0.8f - DropFrac);
    float FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
    
    return Ripple.xy * FinalFactor * 0.35f;
}
	
void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	
	#if defined(VERTEX_COLOR_CONTROL) && defined(RTP_TRIPLANAR)
		#if defined(LOCAL_SPACE_UV)
			o._uv_Relief.xy=v.vertex.xz /  _TERRAIN_ReliefTransformTriplanarZ;
		#else
			o._uv_Relief.xy=mul(_Object2World, v.vertex).xz /  _TERRAIN_ReliefTransformTriplanarZ;
		#endif
	#else
		o._uv_Relief.xy=v.texcoord.xy * _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
	#endif
	#if defined(RTP_TRIPLANAR)
		#if defined(VERTEX_COLOR_CONTROL) || defined(COLOR_EARLY_EXIT) || !defined(APPROX_TANGENTS)
			#if defined(LOCAL_SPACE_UV)
				o._uv_Relief.z=v.vertex.y / _TERRAIN_ReliefTransformTriplanarZ;
			#else
				o._uv_Relief.z=mul(_Object2World, v.vertex).y/ _TERRAIN_ReliefTransformTriplanarZ;
			#endif				
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
		
	#if defined(APPROX_TANGENTS) || defined(FORCE_APPROX_TANGENTS)
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
	#if defined(RTP_WETNESS) || defined(RTP_REFLECTION) || defined(RTP_SNOW) || defined(RTP_POM_SHADING) || defined(RTP_PM_SHADING) || (!defined(LIGHTMAP_OFF) && defined (DIRLIGHTMAP_OFF))
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

	#if defined(RTP_SNOW) || defined(RTP_WETNESS)
		#if !defined(RTP_POM_SHADING_HI) && !defined(RTP_POM_SHADING_MED)
			// dokładny wektor kierunku (ze znakiem)
//			#ifdef APPROX_TANGENTS
//				o.lightDir.xyz = ( mul (rotation, float3(0,1,0)) ).xyz; // teren jest nierotowalny
//			#else
				o.lightDir.xyz = mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz);
//			#endif
		#else
			#ifdef APPROX_TANGENTS
				o._viewDir.zw = ( mul (rotation, float3(0,1,0)) ).xy; // teren jest nierotowalny
			#else
				o._viewDir.zw = ( mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz) ).xy;
			#endif
		#endif
	#endif
	
	#if defined(RTP_SNOW) || defined(RTP_VERTICAL_TEXTURE)
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
				#if defined(LOCAL_SPACE_UV)
					o._uv_Aux.zw=v.normal.xz;	
				#else
					o._uv_Aux.zw=mul(_Object2World, float4(v.normal,0)).xz;			
				#endif
			#endif
		#endif
	#endif	
		
	#if defined(RTP_POM_SHADING_HI) || defined(RTP_POM_SHADING_MED) || defined(RTP_POM_SHADING_LO)
		float2 stretch_factor=v.normal.yy;
		stretch_factor /= float2(length(v.normal.xy), length(v.normal.zy));
		o._viewDir.xy*=stretch_factor.xy;
		#if defined(RTP_POM_SHADING_HI) || defined(RTP_POM_SHADING_MED)
			o.lightDir.xy*=stretch_factor.xy;
		#endif		
	#endif
	
	float far=saturate((o._uv_Relief.w - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
	v.normal.xyz=lerp(v.normal.xyz, float3(0,1,0), far*_FarNormalDamp);
	
}

void surf (Input IN, inout SurfaceOutput o) {
	o.Normal=float3(0,0,1); o.Albedo=0;	o.Emission=0; o.Gloss=0; o.Specular=1; o.Alpha=0;
	float2 mip_selector;
	
	#if defined(RTP_REFLECTION) && defined(RTP_TRIPLANAR)
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
	
	#ifdef VERTEX_COLOR_CONTROL
		//
		// VERTEX_COLOR_CONTROL - channel adjustement
		// you can adjust normalizing function below to your needs (for example to use 3 layers and leave one vertex color channel for another usage like water/snow coverage)
		// look for (USE_2_LAYERS_ONLY) in shader code below to quickly find where color/normals are calculated and enable/disable them for your needs
		//
		#if defined(USE_2_LAYERS_ONLY)
			float4 splat_controlA;
			splat_controlA.x = IN.color.r;
			splat_controlA.y = 1-IN.color.r;
			splat_controlA.zw = 0;
		#else
			#if defined(USE_3_LAYERS_ONLY)
				// using 3 layers
				float4 splat_controlA;
				splat_controlA.xyz = IN.color.rgb/dot(IN.color.rgb,1);
				splat_controlA.w=0;
			#else
				// using 4 layers
				float4 splat_controlA = IN.color/dot(IN.color,1);
			#endif
		#endif
	#else
		float4 splat_controlA = tex2D(_Control1, IN.uv_Control);
	#endif
 	float total_coverage=dot(splat_controlA, 1);
	#ifdef _4LAYERS
		float4 splat_controlA_normalized=splat_controlA/total_coverage;
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
		tHA=saturate(lerp(tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xy), 1, PER_LAYER_HEIGHT_MODIFIER0123)+0.001);
		splat_control1 *= tHA;
	#else
		tHA=saturate(tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xy)+0.001);
		splat_control1 *= tHA;
	#endif	
	float4 splat_control1_mid=splat_control1*splat_control1;
	float4 splat_control1_close=splat_control1_mid*splat_control1_mid;
	splat_control1=lerp(lerp(splat_control1_mid, splat_control1, _uv_Relief_w), splat_control1_close, _uv_Relief_z);
	#ifdef _4LAYERS
		splat_control1 /= dot(splat_control1, 1);
	#endif
		
	float splat_controlA_coverage=dot(splat_control1, 1);
	
	#ifdef RTP_WETNESS
		float actH=dot(splat_control1, tHA);
	#endif
	
	#ifdef COLOR_MAP
	float global_color_blend=lerp( lerp(_GlobalColorMapBlendValues.y, _GlobalColorMapBlendValues.x, _uv_Relief_z*_uv_Relief_z), _GlobalColorMapBlendValues.z, _uv_Relief_w);
	#if defined(VERTEX_COLOR_CONTROL)
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.uv_ColorMapGlobal);
	#else
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control);
	#endif
	#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*1.4+0.2),_uv_Relief_w));
	#else
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif
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
		#if defined(VERTEX_COLOR_CONTROL)
			half4 TERRAIN_WaterColor=splat_controlA_normalized.x*TERRAIN_WaterColor0;
			TERRAIN_WaterColor+=splat_controlA_normalized.y*TERRAIN_WaterColor1;
			TERRAIN_WaterColor+=splat_controlA_normalized.z*TERRAIN_WaterColor2;
			TERRAIN_WaterColor+=splat_controlA_normalized.w*TERRAIN_WaterColor3;
		#else
			half4 TERRAIN_WaterColor=half4(1,1,1,1);
		#endif
		
		mip_selector=saturate(IN._uv_Relief.w-1);// bug in compiler for forward pass, we have to specify mip level indirectly (can't be treated constant)
		#if defined(VERTEX_COLOR_TO_WATER_COVERAGE)
			float water_mask=VERTEX_COLOR_TO_WATER_COVERAGE;
		#else
			#if defined(VERTEX_COLOR_CONTROL)
				float water_mask=tex2Dlod(_BumpMapGlobal, float4(IN.uv_ColorMapGlobal, mip_selector)).b;
			#else
				float water_mask=tex2Dlod(_BumpMapGlobal, float4(IN.uv_Control, mip_selector)).b;
			#endif
		#endif
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
		#if !defined(FORCE_DISABLE_PERLIN)
		float4 global_bump_val=triplanar_blend_tmp.x*tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.yz*_BumpMapGlobalScale, mip_selectorTRIPLANAR.xx+rtp_mipoffset_globalnorm));
		global_bump_val+=triplanar_blend_tmp.y*tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy*_BumpMapGlobalScale, mip_selector+rtp_mipoffset_globalnorm));
		global_bump_val+=triplanar_blend_tmp.z*tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xz*_BumpMapGlobalScale, mip_selectorTRIPLANAR.yy+rtp_mipoffset_globalnorm));
		#endif
		
		#ifdef FAR_ONLY
			triplanar_blend = pow(abs(triplanar_blend), 64);
		#else
			triplanar_blend = pow(abs(triplanar_blend), lerp(64,4,_uv_Relief_z));
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
		#if !defined(FORCE_DISABLE_PERLIN)
		float4 global_bump_val=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy*_BumpMapGlobalScale, mip_selector+rtp_mipoffset_globalnorm));
		#endif
	#endif	
	
	fixed3 col=0;
	
	float3 norm_far=float3(0,0,1);
	float _BumpMapGlobalStrengthPerLayer=1;
	#if defined(RTP_SNOW) || defined(RTP_WETNESS)
		float perlinmask=tex2Dlod(_BumpMapGlobal, float4(IN._uv_Relief.xy/8, mip_selector+rtp_mipoffset_color-3+_uv_Relief_w*2)).r;
		float3 flat_dir;
		#if !defined(RTP_POM_SHADING_HI) && !defined(RTP_POM_SHADING_MED)		
			flat_dir.xy=IN.lightDir.xyz;
		#else
			flat_dir.xy=IN.viewDir.zw;
			flat_dir.z=sqrt(1 - saturate(dot(flat_dir.xy, flat_dir.xy)));
		#endif
		
		float wetSlope=saturate(1-dot(norm_far, flat_dir.xyz));
	#endif
	
	#if !defined(FORCE_DISABLE_PERLIN)
	norm_far.xy = global_bump_val.rg*2-1;
	norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));
	#endif
	#ifdef _4LAYERS
		_BumpMapGlobalStrengthPerLayer=dot(_BumpMapGlobalStrength0123, splat_control1);
	#endif
		
	#ifdef RTP_SNOW		
		float3 norm_for_snow=norm_far*0.3;
		norm_for_snow.z+=0.7;
	#endif

	float2 IN_uv_Relief_Offset;
	#ifdef RTP_SNOW
		#if defined(VERTEX_COLOR_TO_SNOW_COVERAGE)
			rtp_snow_strength*=VERTEX_COLOR_TO_SNOW_COVERAGE;
		#endif
		float snow_const = 0.5*rtp_snow_strength*perlinmask;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.lightDir.w)/rtp_snow_height_transition)*4;
		snow_height_fct=snow_height_fct<0 ? 0 : snow_height_fct;
		snow_const -= snow_height_fct;
		
		#ifdef _4LAYERS
			float rtp_snow_layer_damp=dot(splat_control1, rtp_snow_strength_per_layer0123);
		#endif	
		
		float snow_val;
		#ifdef COLOR_MAP
			snow_val = snow_const + rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow.xxx)+rtp_snow_strength*2;
		#else
			snow_val = snow_const + rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow+rtp_snow_strength*2;
		#endif
		snow_val *= rtp_snow_layer_damp;
		snow_val -= rtp_snow_slope_factor*saturate(( 1 - dot(norm_for_snow, flat_dir.xyz) ));

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
			blendVal*=dot(_MixBlend0123, splat_control1);
		#endif
		#if !defined(FORCE_DISABLE_PERLIN)
			blendVal*=_blend_multiplier*saturate((global_bump_val.r*global_bump_val.g*2+0.3));
		#else
			blendVal*=_blend_multiplier;
		#endif
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
	 	float4 rayPos = float4(IN._uv_Relief.xy, 1, clamp((mip_selector.x+rtp_mipoffset_height), 0, 6) );
	 	
	 	#ifdef RTP_POM_SHADING
		float3 EyeDirTan = IN_viewDir.xyz;
		float slopeF=1+IN_viewDir.z;
		slopeF*=slopeF;
		slopeF*=slopeF;
		EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*COLOR_DAMP_VAL*_uv_Relief_z*(1-slopeF)); // damp bo kanale a colormapy, odleglosci i skompresowanym kacie obserwaci (poprawia widok zboczy)
		bool hit_flag=false;
		float delta=_TERRAIN_HeightMap_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH/length(EyeDirTan.xy);
		EyeDirTan*=delta;
		bool height;
		
		float dh_prev=0;
		float h_prev=1.001;
		float _h;
		
		float shadow_atten=1;
		#endif
		
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
				if (COLOR_DAMP_VAL>0) {
				for(int i=0; i<_TERRAIN_DIST_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
			 		float4 tH;
					#ifdef USE_EXTRUDE_REDUCTION
						tH=lerp(tex2Dlod(_TERRAIN_HeightMap, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER0123);
					#else
						tH=tex2Dlod(_TERRAIN_HeightMap, rayPos.xyww);
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
						tH=lerp(tex2Dlod(_TERRAIN_HeightMap, rayPos.xyww), 1, PER_LAYER_HEIGHT_MODIFIER0123);
					#else
						tH=tex2Dlod(_TERRAIN_HeightMap, rayPos.xyww);
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
					 			hgtYZ = triplanar_blend.x * dot(splat_control1, lerp(tex2D(_TERRAIN_HeightMap, IN._uv_Relief.yz), 1, PER_LAYER_HEIGHT_MODIFIER0123));
					 			hgtXY = triplanar_blend.y * dot(splat_control1, lerp(tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xy), 1, PER_LAYER_HEIGHT_MODIFIER0123));
					 			hgtXZ = triplanar_blend.z * dot(splat_control1, lerp(tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xz), 1, PER_LAYER_HEIGHT_MODIFIER0123));
							#else
					 			hgtYZ = triplanar_blend.x * dot(splat_control1, tex2D(_TERRAIN_HeightMap, IN._uv_Relief.yz));
					 			hgtXY = triplanar_blend.y * dot(splat_control1, tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xy));
					 			hgtXZ = triplanar_blend.z * dot(splat_control1, tex2D(_TERRAIN_HeightMap, IN._uv_Relief.xz));
							#endif
							#ifdef RTP_WETNESS
								actH=lerp(actH, hgtYZ + hgtXY + hgtXZ, _uv_Relief_z) ;
							#endif
		 				} else {
		 					// no blend case
							#ifdef USE_EXTRUDE_REDUCTION
					 			hgtTRI = dot(splat_control1, lerp(tex2Dlod(_TERRAIN_HeightMap, uvTRI.xyzz), 1, PER_LAYER_HEIGHT_MODIFIER0123));
							#else
					 			hgtTRI = dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap, uvTRI.xyzz));
							#endif
			 				//hgtTRI*=triplanar_blend_simple;
							#ifdef RTP_WETNESS
								actH=lerp(actH, hgtTRI, _uv_Relief_z);
							#endif
		 				}
				#else
					#if defined(RTP_PM_SHADING)
						rayPos.xy += ParallaxOffset(dot(splat_control1, tHA), _TERRAIN_ExtrudeHeight*_uv_Relief_z*COLOR_DAMP_VAL, IN_viewDir.xyz);
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
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection0123)*_uv_Relief_z;
				#else
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection0123);
				#endif
 			#endif
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection0123)*_uv_Relief_z;
				#else
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection0123);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlA_normalized, TERRAIN_LayerWetStrength0123);
				TERRAIN_WetSpecularity=dot(water_splat_control, TERRAIN_WetSpecularity0123);
				TERRAIN_WaterGloss=dot(water_splat_control, TERRAIN_WaterGloss0123);
				#if !defined(VERTEX_COLOR_CONTROL)
				TERRAIN_WaterColor=half4( dot(splat_controlA_normalized, TERRAIN_WaterColorR0123), dot(splat_controlA_normalized, TERRAIN_WaterColorG0123), dot(splat_controlA_normalized, TERRAIN_WaterColorB0123), dot(splat_controlA_normalized, TERRAIN_WaterColorA0123) );
				#endif
				
				float TERRAIN_WaterLevel=dot(water_splat_control, TERRAIN_WaterLevel0123);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlA_normalized, TERRAIN_WaterLevelSlopeDamp0123);
				float TERRAIN_Flow=dot(water_splat_control, TERRAIN_Flow0123);
				float TERRAIN_WaterEdge=dot(water_splat_control, TERRAIN_WaterEdge0123);
				float TERRAIN_Refraction=dot(water_splat_control, TERRAIN_Refraction0123);
				float TERRAIN_WetRefraction=dot(water_splat_control, TERRAIN_WetRefraction0123);
				
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
						flowOffset*=TERRAIN_Flow*max(p, TERRAIN_WetSpecularity)*_uv_Relief_z*TERRAIN_LayerWetStrength;
					#endif
					
					#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
						float2 rippleUV = IN._uv_Relief.xy*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
					  	roff = GetRipple( float4(rippleUV, mip_selector + rtp_mipoffset_ripple), TERRAIN_RainIntensity);
						roff += GetRipple( float4(rippleUV+float2(0.25,0.25), mip_selector + rtp_mipoffset_ripple ), TERRAIN_RainIntensity);
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
							uvTRI1.xy+= ParallaxOffset(hgtYZ, _TERRAIN_ExtrudeHeight*_uv_Relief_z*COLOR_DAMP_VAL, dir);
						#endif
						c = tex2Dlod(_SplatA0, uvTRI1.xyzz); col += _MixBlendtmp.x * c.rgb; tmp_gloss.r=c.a;
						c = tex2Dlod(_SplatA1, uvTRI1.xyzz); col += _MixBlendtmp.y * c.rgb; tmp_gloss.g=c.a;
						#if !defined(USE_2_LAYERS_ONLY)
						c = tex2Dlod(_SplatA2, uvTRI1.xyzz); col += _MixBlendtmp.z * c.rgb; tmp_gloss.b=c.a;
						#if !defined(USE_3_LAYERS_ONLY)
						c = tex2Dlod(_SplatA3, uvTRI1.xyzz); col += _MixBlendtmp.w * c.rgb; tmp_gloss.a=c.a;
						#endif
						#endif
						gloss+=triplanar_blend.x*tmp_gloss;
						
						#ifdef RTP_SNOW
							uvTRI1.z += snow_depth;
						#endif							
						
						normals_combined = tex2Dlod(_BumpMap01, uvTRI1.xyzz).grab*splat_control1.rrgg;  // x<-> y
						#if !defined(USE_2_LAYERS_ONLY)
						normals_combined+=tex2Dlod(_BumpMap23, uvTRI1.xyzz).grab*splat_control1.bbaa;
						#endif
						nA.xy=(normals_combined.rg+normals_combined.ba)*2-1;
						nA.x=triplanar_flip.x ? nA.x:-nA.x;
						nA.xy*=_uv_Relief_z;
						nA.z = sqrt(1 - saturate(dot(nA.xy, nA.xy)));
					}
					if (triplanar_blend.y>0.05) {
						float4 _MixBlendtmp=splat_control1*triplanar_blend.y;
						float4 tmp_gloss;
						#if defined(RTP_PM_SHADING)
							uvTRI2.xy+= ParallaxOffset(hgtXY, _TERRAIN_ExtrudeHeight*_uv_Relief_z*COLOR_DAMP_VAL, IN_viewDir.xyz);
						#endif
						c = tex2Dlod(_SplatA0, uvTRI2.xyzz); col += _MixBlendtmp.x * c.rgb; tmp_gloss.r=c.a;
						c = tex2Dlod(_SplatA1, uvTRI2.xyzz); col += _MixBlendtmp.y * c.rgb; tmp_gloss.g=c.a;
						#if !defined(USE_2_LAYERS_ONLY)
						c = tex2Dlod(_SplatA2, uvTRI2.xyzz); col += _MixBlendtmp.z * c.rgb; tmp_gloss.b=c.a;
						#if !defined(USE_3_LAYERS_ONLY)
						c = tex2Dlod(_SplatA3, uvTRI2.xyzz); col += _MixBlendtmp.w * c.rgb; tmp_gloss.a=c.a;
						#endif
						#endif
						gloss+=triplanar_blend.y*tmp_gloss;
						
						#ifdef RTP_SNOW
							uvTRI2.z += snow_depth;
						#endif							
						
						normals_combined = tex2Dlod(_BumpMap01, uvTRI2.xyzz).rgba*splat_control1.rrgg;
						#if !defined(USE_2_LAYERS_ONLY)
						normals_combined+=tex2Dlod(_BumpMap23, uvTRI2.xyzz).rgba*splat_control1.bbaa;
						#endif
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
							uvTRI3.xy+= ParallaxOffset(hgtXZ, _TERRAIN_ExtrudeHeight*_uv_Relief_z*COLOR_DAMP_VAL, dir);
						#endif
						c = tex2Dlod(_SplatA0, uvTRI3.xyzz); col += _MixBlendtmp.x * c.rgb; tmp_gloss.r=c.a;
						c = tex2Dlod(_SplatA1, uvTRI3.xyzz); col += _MixBlendtmp.y * c.rgb; tmp_gloss.g=c.a;
						#if !defined(USE_2_LAYERS_ONLY)
						c = tex2Dlod(_SplatA2, uvTRI3.xyzz); col += _MixBlendtmp.z * c.rgb; tmp_gloss.b=c.a;
						#if !defined(USE_3_LAYERS_ONLY)
						c = tex2Dlod(_SplatA3, uvTRI3.xyzz); col += _MixBlendtmp.w * c.rgb; tmp_gloss.a=c.a;
						#endif
						#endif
						gloss+=triplanar_blend.z*tmp_gloss;
						
						#ifdef RTP_SNOW
							uvTRI3.z += snow_depth;
						#endif			
										
						normals_combined = tex2Dlod(_BumpMap01, uvTRI3.xyzz).rgba*splat_control1.rrgg;
						#if !defined(USE_2_LAYERS_ONLY)
						normals_combined+=tex2Dlod(_BumpMap23, uvTRI3.xyzz).rgba*splat_control1.bbaa;
						#endif
						nC.xy=(normals_combined.rg+normals_combined.ba)*2-1;
						nC.y=triplanar_flip.y ? nC.y:-nC.y;
						nC.xy*=_uv_Relief_z;
						nC.z = sqrt(1 - saturate(dot(nC.xy, nC.xy)));
					}	
					float3 n=(triplanar_blend.x * nA + triplanar_blend.y * nB + triplanar_blend.z * nC);
					
					o.Normal=n;
				} else {
					//
					// triplanar no blend - simple case
					//
					#ifdef RTP_WETNESS
						uvTRI.xy+=flowOffset;
					#endif
					#if defined(RTP_PM_SHADING)
						uvTRI.xy+= ParallaxOffset(hgtTRI, _TERRAIN_ExtrudeHeight*_uv_Relief_z*COLOR_DAMP_VAL, dirTRI);
					#endif
					float3 uvTRI_tmp=float3(uvTRI.xy, uvTRI.z+rtp_mipoffset_color);
					float4 tmp_gloss;
					c = tex2Dlod(_SplatA0, uvTRI_tmp.xyzz)*splat_control1.x; col += c.rgb; tmp_gloss.r=c.a;
					c = tex2Dlod(_SplatA1, uvTRI_tmp.xyzz)*splat_control1.y; col += c.rgb; tmp_gloss.g=c.a;
					#if !defined(USE_2_LAYERS_ONLY)
					c = tex2Dlod(_SplatA2, uvTRI_tmp.xyzz)*splat_control1.z; col += c.rgb; tmp_gloss.b=c.a;
					#if !defined(USE_3_LAYERS_ONLY)
					c = tex2Dlod(_SplatA3, uvTRI_tmp.xyzz)*splat_control1.w; col += c.rgb; tmp_gloss.a=c.a;
					#endif
					#endif
					gloss=tmp_gloss;
					
					uvTRI_tmp.z=uvTRI.z+rtp_mipoffset_bump;
					
					#ifdef RTP_SNOW
						uvTRI_tmp.z += snow_depth;
					#endif				
					
					float4 normA=tex2Dlod(_BumpMap01, uvTRI_tmp.xyzz).rgba;
					normA = (triplanar_blend.x>=0.95) ? normA.grab : normA;
					float4 normals_combined = normA*splat_control1.rrgg;
					#if !defined(USE_2_LAYERS_ONLY)
					float4 normB=tex2Dlod(_BumpMap23, uvTRI_tmp.xyzz).rgba;
					normB = (triplanar_blend.x>=0.95) ? normB.grab : normB;
					normals_combined += normB*splat_control1.bbaa;
					#endif
					float3 n;
					n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
					if (triplanar_blend.x>=0.95) n.x= triplanar_flip.x ? n.x : -n.x;
					if (triplanar_blend.z>=0.95) 	n.y= triplanar_flip.y ? n.y : -n.y;
					n.xy*=_uv_Relief_z;
					n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
				
					o.Normal=n;
				}
				#if defined(RTP_UV_BLEND) && !defined(RTP_DISTANCE_ONLY_UV_BLEND)
					float4 _MixMipActual=uvTRI.zzzz+rtp_mipoffset_color+log2(_MixScale0123);
					
					float4 uvSplat01M=uvTRI.xyxy*_MixScale0123.xxyy;
					float4 uvSplat23M=uvTRI.xyxy*_MixScale0123.zzww;
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
				#if defined(_4LAYERS)
					rayPos.w=mip_selector.x+rtp_mipoffset_color;
					c = tex2Dlod(_SplatA0, rayPos.xyww); col = splat_control1.x * c.rgb; gloss.r = c.a;
					c = tex2Dlod(_SplatA1, rayPos.xyww); col += splat_control1.y * c.rgb; gloss.g = c.a;
					#if !defined(USE_2_LAYERS_ONLY)
					c = tex2Dlod(_SplatA2, rayPos.xyww); col += splat_control1.z * c.rgb; gloss.b = c.a;
					#if !defined(USE_3_LAYERS_ONLY)
					c = tex2Dlod(_SplatA3, rayPos.xyww); col += splat_control1.w * c.rgb; gloss.a = c.a;				
					#endif
					#endif
				#endif
				//
				// EOF no triplanar
				//
			#endif

			o.Gloss = dot(gloss, splat_control1);
			o.Specular = 0.03+dot(_Spec0123, splat_control1);

			#if defined(RTP_UV_BLEND) && !defined(RTP_TRIPLANAR)
			#ifndef RTP_DISTANCE_ONLY_UV_BLEND
				#if defined(_4LAYERS)
					float4 _MixMipActual=mip_selector.xxxx + rtp_mipoffset_color + log2(_MixScale0123);
					
					float4 uvSplat01M=IN._uv_Relief.xyxy*_MixScale0123.xxyy;
					float4 uvSplat23M=IN._uv_Relief.xyxy*_MixScale0123.zzww;
	
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
				#endif
			#endif
			#endif
			
			#if defined(RTP_UV_BLEND)
				#ifndef RTP_DISTANCE_ONLY_UV_BLEND			
					colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control1, _MixSaturation0123) );
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
				normals_combined = tex2Dlod(_BumpMap01, rayPos.xyww).rgba*splat_control1.rrgg;
				#if !defined(USE_2_LAYERS_ONLY)				
				normals_combined+=tex2Dlod(_BumpMap23, rayPos.xyww).rgba*splat_control1.bbaa;
				#endif
				n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
				n.xy*=_uv_Relief_z;
				n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
				o.Normal=n;
			#else
				// normalne wyliczone powyżej
			#endif	
			
			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatA0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control1, _VerticalTexture0123) );
			#endif
							
			////////////////////////////////
			// water
			//
	        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
		        GlossDry=o.Gloss;
		    #endif
	        #if defined(RTP_WETNESS)
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		       col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=dot(splat_controlA_normalized, TERRAIN_WaterOpacity0123)*p; // gladsze przejscie (po nieskompresowanym splat_control)

				#ifdef RTP_POM_SHADING
		 		#if defined(RTP_SOFT_SHADOWS) || defined(RTP_HARD_SHADOWS)
	 				shadow_atten=lerp(shadow_atten, 1, _WaterOpacity);
				#endif
				#endif
	 					        
				col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        o.Normal.xy+=roff;
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*0.3;
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
				super_detail_norm.xy = super_detail.xy*2-1+o.Normal.xy;
				super_detail_norm.z = sqrt(1 - saturate(dot(super_detail_norm.xy, super_detail_norm.xy)));
				super_detail_norm=normalize(super_detail_norm);
				o.Normal=lerp(o.Normal, super_detail_norm, _uv_Relief_z*dot(splat_control1, _SuperDetailStrengthNormal0123));
			#endif
		
//#ifdef RTP_SNOW
//}
//#endif
			// snow color
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) )
			#if defined(_4LAYERS)
				rayPos.w=mip_selector.x+rtp_mipoffset_color;
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatA0, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatA1, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatA2, rayPos.xyww);
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatA3, rayPos.xyww);
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
				EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*COLOR_DAMP_VAL);
				delta=_TERRAIN_HeightMap_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH_SHADOWS/length(EyeDirTan.xy);
				h_prev=rayPos.z;
				//rayPos.xyz+=EyeDirTan*_TERRAIN_HeightMap_TexelSize.x*2;
				EyeDirTan*=delta;
		
				hit_flag=false;
				dh_prev=0;
				//_TERRAIN_SHADOW_STEPS=min(_TERRAIN_SHADOW_STEPS, ((EyeDirTan.z>0) ? (1-rayPos.z) : rayPos.z) / abs(EyeDirTan.z));
				for(int i=0; i<_TERRAIN_SHADOW_STEPS; i++) {
					rayPos.xyz+=EyeDirTan;
					_h=dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap, rayPos.xyww));
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
						EyeDirTan=IN.lightDir.xyz*_TERRAIN_HeightMap_TexelSize.x*exp2(rayPos.w)*_TERRAIN_WAVELENGTH_SHADOWS;
						EyeDirTan.z/=max(0.001, _TERRAIN_ExtrudeHeight*COLOR_DAMP_VAL);
						float smooth_val=0;
						float break_val=_TERRAIN_ExtrudeHeight*_TERRAIN_ShadowSmoothing;
						for(int i=0; i<_TERRAIN_SHADOW_SMOOTH_STEPS; i++) {
							rayPos.xyz+=EyeDirTan;
							float d=dot(splat_control1, tex2Dlod(_TERRAIN_HeightMap, rayPos.xyww)) - rayPos.z;
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
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*COLOR_DAMP_VAL*(1-snow_depth_lerp));
				#else
					shadow_atten=lerp(1, shadow_atten, _uv_Relief_z*COLOR_DAMP_VAL);
				#endif
			#endif
			#endif
			//
		 	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
		 				
	 		// end of splats 0-3 close
	 	}
		
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
				col=global_color_value.rgb*splat_controlA_coverage;
				o.Gloss=0;
				o.Specular=0.5;
			} else {
#endif			

	 		float4 MIPmult0123=_MIPmult0123*_uv_Relief_w;
			half4 c;
			float4 gloss=0;			
			
			#ifdef RTP_TRIPLANAR
				//
				// triplanar no blend - simple case
				//
				float4 _MixMipActual=uvTRI.zzzz+rtp_mipoffset_color+MIPmult0123;
				float4 tmp_gloss;
				c = tex2Dlod(_SplatA0, float4(uvTRI.xy,_MixMipActual.xx))*splat_control1.x; col += c.rgb; tmp_gloss.r=c.a;
				c = tex2Dlod(_SplatA1, float4(uvTRI.xy,_MixMipActual.yy))*splat_control1.y; col += c.rgb; tmp_gloss.g=c.a;
				#if !defined(USE_2_LAYERS_ONLY)
				c = tex2Dlod(_SplatA2, float4(uvTRI.xy,_MixMipActual.zz))*splat_control1.z; col += c.rgb; tmp_gloss.b=c.a;
				#if !defined(USE_3_LAYERS_ONLY)
				c = tex2Dlod(_SplatA3, float4(uvTRI.xy,_MixMipActual.ww))*splat_control1.w; col += c.rgb; tmp_gloss.a=c.a;
				#endif
				#endif
				gloss=tmp_gloss;
				#if defined(RTP_UV_BLEND) 
					_MixMipActual=uvTRI.zzzz+rtp_mipoffset_color+log2(_MixScale0123);
					
					float4 uvSplat01M=uvTRI.xyxy*_MixScale0123.xxyy;
					float4 uvSplat23M=uvTRI.xyxy*_MixScale0123.zzww;
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
				#if defined(_4LAYERS)
					float4 _MixMipActual=mip_selector.xxxx + rtp_mipoffset_color+MIPmult0123;
					c = tex2Dlod(_SplatA0, float4(IN._uv_Relief.xy, _MixMipActual.xx)); col = splat_control1.x * c.rgb; gloss.r = c.a;
					c = tex2Dlod(_SplatA1, float4(IN._uv_Relief.xy, _MixMipActual.yy)); col += splat_control1.y * c.rgb; gloss.g = c.a;
					#if !defined(USE_2_LAYERS_ONLY)
					c = tex2Dlod(_SplatA2, float4(IN._uv_Relief.xy, _MixMipActual.zz)); col += splat_control1.z * c.rgb; gloss.b = c.a;
					#if !defined(USE_3_LAYERS_ONLY)
					c = tex2Dlod(_SplatA3, float4(IN._uv_Relief.xy, _MixMipActual.ww)); col += splat_control1.w * c.rgb; gloss.a = c.a;				
					#endif
					#endif
				#endif
				//
				// EOF no triplanar
				//
			#endif
			o.Gloss = dot(gloss, splat_control1);
			o.Specular =0.03+dot(_Spec0123, splat_control1);
			
			#if defined(RTP_UV_BLEND) && !defined(RTP_TRIPLANAR)
				#if defined(_4LAYERS)
					_MixMipActual=mip_selector.xxxx+rtp_mipoffset_color+log2(_MixScale0123);
					
					float4 uvSplat01M=IN._uv_Relief.xyxy*_MixScale0123.xxyy;
					float4 uvSplat23M=IN._uv_Relief.xyxy*_MixScale0123.zzww;
					
					half3 colBlend = splat_control1.x * UV_BLEND_ROUTE_LAYER_0;
					colBlend += splat_control1.y * UV_BLEND_ROUTE_LAYER_1;
					colBlend += splat_control1.z * UV_BLEND_ROUTE_LAYER_2;
					colBlend += splat_control1.w * UV_BLEND_ROUTE_LAYER_3;
				#endif
			#endif
			
			#if defined(RTP_UV_BLEND)
				colBlend=lerp((dot(colBlend,0.33333)).xxx, colBlend, dot(splat_control1, _MixSaturation0123) );
				col=lerp(col, col*colBlend*2, blendVal);			
			#endif	
						
#if defined(SIMPLE_FAR) && defined(COLOR_MAP)
			col=lerp(col, global_color_value.rgb*splat_controlA_coverage, _uv_Relief_w);
			o.Gloss*=(1-_uv_Relief_w);
			o.Specular=lerp(o.Specular, 0.5, _uv_Relief_w);
			}
#endif		
	
			#ifdef RTP_VERTICAL_TEXTURE
				float2 vert_tex_uv=float2(0, IN.lightDir.w/_VerticalTextureTiling) + _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
				half3 vert_tex=tex2Dlod(_VerticalTexture, float4(vert_tex_uv, mip_selector-log2( _VerticalTexture_TexelSize.y/(_SplatA0_TexelSize.x*(_TERRAIN_ReliefTransformTriplanarZ/_VerticalTextureTiling)) ))).rgb;
				col=lerp(col, col*vert_tex*2, dot(splat_control1, _VerticalTexture0123) );
			#endif
			
//#ifdef RTP_SNOW
//}
//#endif			

			// snow color
			#if defined(RTP_SNOW) && !defined(RTP_SIMPLE_SHADING) && ( defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2) || defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3) )
			#if defined(_4LAYERS)
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_0)
					half4 csnow = tex2Dlod(_SplatA0, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_1)
					half4 csnow = tex2Dlod(_SplatA1, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_2)
					half4 csnow = tex2Dlod(_SplatA2, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
				#if defined(RTP_SNW_CHOOSEN_LAYER_COLOR_3)
					half4 csnow = tex2Dlod(_SplatA3, float4(IN._uv_Relief.xy, mip_selector + rtp_mipoffset_color));
					GETrtp_snow_TEX
				#endif
			#endif	
			#endif
			// eof snow color
			
			////////////////////////////////
			// water
			//
 			#if defined(RTP_REFLECTION) 
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_LayerReflection=0;
				#else
					TERRAIN_LayerReflection=dot(splat_control1, TERRAIN_LayerReflection0123);
				#endif
 			#endif
			float4 water_splat_control=splat_control1;
			#ifdef RTP_WETNESS
 				#if defined(RTP_SIMPLE_SHADING)
					TERRAIN_WetReflection=0;
				#else
					TERRAIN_WetReflection=dot(water_splat_control, TERRAIN_WetReflection0123);
				#endif
				TERRAIN_LayerWetStrength=dot(splat_controlA_normalized, TERRAIN_LayerWetStrength0123);
				TERRAIN_WetSpecularity=dot(water_splat_control, TERRAIN_WetSpecularity0123);
				TERRAIN_WaterGloss=dot(water_splat_control, TERRAIN_WaterGloss0123);
				#if !defined(VERTEX_COLOR_CONTROL)
				TERRAIN_WaterColor=half4( dot(splat_controlA_normalized, TERRAIN_WaterColorR0123), dot(splat_controlA_normalized, TERRAIN_WaterColorG0123), dot(splat_controlA_normalized, TERRAIN_WaterColorB0123), dot(splat_controlA_normalized, TERRAIN_WaterColorA0123) );
				#endif
								
				float TERRAIN_WaterLevel=dot(water_splat_control, TERRAIN_WaterLevel0123);
				float TERRAIN_WaterLevelSlopeDamp=dot(splat_controlA_normalized, TERRAIN_WaterLevelSlopeDamp0123);
				//float TERRAIN_Flow=dot(water_splat_control, TERRAIN_Flow0123);
				float TERRAIN_WaterEdge=dot(water_splat_control, TERRAIN_WaterEdge0123);
				//float TERRAIN_Refraction=dot(water_splat_control, TERRAIN_Refraction0123);
				//float TERRAIN_WetRefraction=dot(water_splat_control, TERRAIN_WetRefraction0123);
				
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
		        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
		        o.Gloss = lerp(GlossDry, o.Gloss, 0.2);//saturate(_uv_Relief_z+0.2));
		        o.Specular = lerp(o.Specular, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
		        
		        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
		        float _WaterOpacity=dot(splat_controlA_normalized, TERRAIN_WaterOpacity0123)*p;
	 					        
		        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
		        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
		        //o.Normal=normalize(o.Normal);
		  		
				col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*0.3;
	        #endif
			// water
			////////////////////////////////
						
	 	}
	 	
	 	IN_uv_Relief_Offset.xy=IN._uv_Relief.xy;
	}
	
	float3 norm_snowCov=o.Normal;
	o.Normal+=norm_far*_uv_Relief_w*_BumpMapGlobalStrengthPerLayer;	
		
	#ifdef COLOR_MAP
		#ifdef _4LAYERS
			global_color_blend *= dot(splat_control1, _GlobalColorPerLayer0123);
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
		snow_val -= rtp_snow_slope_factor*saturate( 1 - dot( (norm_snowCov+norm_far+norm_for_snow)*0.5 , flat_dir.xyz) );
		
		snow_val=saturate(snow_val);
		snow_val=pow(abs(snow_val), rtp_snow_edge_definition);
		rtp_snow_color_tex=lerp(rtp_snow_color.rgb, rtp_snow_color_tex, _uv_Relief_z);
		
		#ifdef COLOR_MAP
			half3 global_color_value_desaturated=dot(global_color_value.rgb, 0.37);//0.3333333); // będzie trochę jasniej
			#ifdef COLOR_MAP_BLEND_MULTIPLY
				rtp_snow_color_tex=lerp(rtp_snow_color_tex, rtp_snow_color_tex*global_color_value_desaturated.rgb*2, global_color_blend);
			#else
				rtp_snow_color_tex=lerp(rtp_snow_color_tex, global_color_value_desaturated.rgb, global_color_blend);
			#endif
		#endif

		col=lerp( col, rtp_snow_color_tex, snow_val );
		
		#if defined(RTP_SNW_CHOOSEN_LAYER_NORM_0) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_1) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_2) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_3) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_4) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_5) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_6) || defined(RTP_SNW_CHOOSEN_LAYER_NORM_7)
			float3 n;
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_0
				n.xy=tex2Dlod(_BumpMap01, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).rg*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_1
				n.xy=tex2Dlod(_BumpMap01, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).ba*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_2
				n.xy=tex2Dlod(_BumpMap23, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).rg*2-1;
			#endif
			#ifdef RTP_SNW_CHOOSEN_LAYER_NORM_3
				n.xy=tex2Dlod(_BumpMap23, float4(IN_uv_Relief_Offset.xy, mip_selector + rtp_mipoffset_bump)).ba*2-1;
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
		#if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
			o.Gloss=lerp(GlossDry, lerp(rtp_snow_gloss/2, rtp_snow_gloss, _uv_Relief_z), snow_val);
		#else
			o.Gloss=lerp(o.Gloss, lerp(rtp_snow_gloss/2, rtp_snow_gloss, _uv_Relief_z), snow_val);
		#endif
		 #ifdef RTP_REFLECTION
		 	GlossDry=lerp(GlossDry, o.Gloss, snow_val);
		#endif
		o.Specular=lerp(o.Specular, rtp_snow_specular, snow_val);
	#endif
	
	o.Albedo=col;
	float3 norm_edge=o.Normal;
	o.Normal=normalize(o.Normal);
	
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

//	float diff;
//	#if !defined(LIGHTMAP_OFF) && defined (DIRLIGHTMAP_OFF)
//		//IN.lightDir.z*=_TERRAIN_ExtrudeHeight;
//		diff = max (0, dot (o.Normal, IN.lightDir.xyz))*lerp(2,2-_BumpMapGlobalStrengthPerLayer,_uv_Relief_z);
//		diff = lerp(diff, 1, _uv_Relief_w*0.75f); // w dużej odleglosci nakladamy tylko 25% diffa
//		diff = diff*_TERRAIN_LightmapShading+(1-_TERRAIN_LightmapShading);
//		o.Albedo*=diff;
//		o.Gloss*=diff;
//	#endif

} 
	
		ENDCG
	} 
	
	FallBack Off
}
