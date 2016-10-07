Shader "Relief Pack/Terrain2Geometry" {
Properties {
	_Control ("Control1 (RGBA)", 2D) = "red" {} 
	_Control1 ("Control1 (RGBA)", 2D) = "red" {} 
	_Control2 ("Control2 (RGBA)", 2D) = "red" {} 
	_Control3 ("Control3 (RGBA)", 2D) = "red" {} 
	_ColorMapGlobal ("Global Color (RGBA)", 2D) = "grey" {} 
}

/* INIT
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//
//
// POM / PM / SIMPLE shading
//
//
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
SubShader {
	Tags {
		"Queue" = "Geometry+2"
		"RenderType" = "Opaque"
	}
	LOD 700
	Fog { Mode Off }
	//Offset -1,-1
	CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert finalcolor:customFog fullforwardshadows noforwardadd nolightmap
	#pragma target 3.0
	#pragma glsl
	#pragma only_renderers d3d9
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	//#define RTP_POM_SHADING_HI
	
	#include "UnityCG.cginc"
	
	// for geom blend (early exit from sur function)
	//#define COLOR_EARLY_EXIT
	// tangents approximation
	//#define APPROX_TANGENTS
		
	#include "RTP_Base.cginc"

	ENDCG
	
///astar AddBlend
ZWrite Off Fog { Mode Off }
ZTest LEqual	
CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert finalcolor:customFog fullforwardshadows decal:blend noforwardadd nolightmap
	#pragma target 3.0
	#pragma glsl
	#pragma only_renderers d3d9
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	//#define RTP_PM_SHADING
	
	#include "UnityCG.cginc"

	// for geom blend (early exit from sur function)
	//#define COLOR_EARLY_EXIT
	// tangents approximation
	//#define APPROX_TANGENTS
	
	#include "RTP_AddBase.cginc"
	
ENDCG  				
//astar/ // AddBlend

}
// EOF POM / PM / SIMPLE shading
*/ // INIT

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//
//
// CLASSIC shading
//
//
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
SubShader {
	Tags {
		"Queue" = "Geometry+2"
		"RenderType" = "Opaque"
	}
	LOD 100
//	Offset -1,-1
CGPROGRAM
	#pragma surface surf Lambert vertex:vert  noforwardadd nolightmap
	#include "UnityCG.cginc"
	
	#pragma only_renderers d3d9
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	//#define ADDITIONAL_FEATURES_IN_FALLBACKS

	#ifdef ADDITIONAL_FEATURES_IN_FALLBACKS	
		// comment if you don't need global color map
		#define COLOR_MAP
		// if not defined global color map will be blended (lerp)
		#define COLOR_MAP_BLEND_MULTIPLY
		
		#define RTP_SNOW
	#endif
/////////////////////////////////////////////////////////////////////
	
sampler2D _Control, _Control1;
sampler2D _SplatA0,_SplatA1,_SplatA2,_SplatA3;
float4 _TERRAIN_ReliefTransform;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef COLOR_MAP
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
sampler2D _ColorMapGlobal;
#endif
#ifdef RTP_SNOW
float _snow_strength;
float _global_color_brightness_to_snow;
float _snow_slope_factor;
float _snow_edge_definition;
float4 _snow_strength_per_layer0123;
float4 _snow_strength_per_layer4567;
float _snow_height_treshold;
float _snow_height_transition;
fixed3 _snow_color;
float _snow_gloss;
float _snow_specular;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 _uv_Relief;
	float4 snowDir;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_Relief.xy=v.texcoord.xy * _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		o.snowDir.xyz = normalize( mul(_Object2World, float4(v.normal,0)).xyz );
		o.snowDir.w = mul(_Object2World, v.vertex).y;
	#endif	
/////////////////////////////////////////////////////////////////////	

}
void surf (Input IN, inout SurfaceOutput o) {
	float4 splat_control = tex2D(_Control1, IN.uv_Control);
	
	#if defined(COLOR_MAP) || defined(RTP_SNOW)
		float3 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control).rgb;	
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif	

	#ifdef RTP_SNOW
		float snow_val=_snow_strength*2;
		float snow_height_fct=saturate((_snow_height_treshold - IN.snowDir.w)/_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += _snow_strength*dot(1-global_color_value.rgb, _global_color_brightness_to_snow);
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= _snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		snow_val*=snow_val;
		
	 	fixed3 col;
		col = splat_control.r * lerp(tex2D(_SplatA0, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.x );
		col += splat_control.g * lerp(tex2D(_SplatA1, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.y );
		col += splat_control.b * lerp(tex2D(_SplatA2, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.z );
		col += splat_control.a * lerp(tex2D(_SplatA3, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.w );
		
		global_color_value.rgb=lerp(global_color_value.rgb, _snow_color, snow_val);
	#else		
	 	fixed3 col;
		col = splat_control.r * tex2D(_SplatA0, IN._uv_Relief.xy).rgb;
		col += splat_control.g * tex2D(_SplatA1, IN._uv_Relief.xy).rgb;
		col += splat_control.b * tex2D(_SplatA2, IN._uv_Relief.xy).rgb;
		col += splat_control.a * tex2D(_SplatA3, IN._uv_Relief.xy).rgb;
	#endif
	
	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col=lerp(col, col*global_color_value.rgb*2, _GlobalColorMapBlendValues.y);
		#else
			col=lerp(col, global_color_value.rgb, _GlobalColorMapBlendValues.y);
		#endif		
	#endif
		
	o.Albedo = col;
}
ENDCG  

///* AddPass
//Offset -1,-1
ZTest LEqual
CGPROGRAM
	#pragma surface surf Lambert  vertex:vert decal:add noforwardadd nolightmap
	#include "UnityCG.cginc"
	
	#pragma only_renderers d3d9
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	//#define ADDITIONAL_FEATURES_IN_FALLBACKS

	#ifdef ADDITIONAL_FEATURES_IN_FALLBACKS	
		// comment if you don't need global color map
		#define COLOR_MAP
		// if not defined global color map will be blended (lerp)
		#define COLOR_MAP_BLEND_MULTIPLY
		
		#define RTP_SNOW
	#endif
/////////////////////////////////////////////////////////////////////
	
sampler2D _Control, _Control2;
sampler2D _SplatB0,_SplatB1,_SplatB2,_SplatB3;
float4 _TERRAIN_ReliefTransform;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef COLOR_MAP
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
sampler2D _ColorMapGlobal;
#endif
#ifdef RTP_SNOW
float _snow_strength;
float _global_color_brightness_to_snow;
float _snow_slope_factor;
float _snow_edge_definition;
float4 _snow_strength_per_layer0123;
float4 _snow_strength_per_layer4567;
float _snow_height_treshold;
float _snow_height_transition;
fixed3 _snow_color;
float _snow_gloss;
float _snow_specular;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float2 uv_Control : TEXCOORD0;
	float2 _uv_Relief;
	float4 snowDir;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_Relief.xy=v.texcoord.xy * _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		o.snowDir.xyz = normalize( mul(_Object2World, float4(v.normal,0)).xyz );
		o.snowDir.w = mul(_Object2World, v.vertex).y;
	#endif	
/////////////////////////////////////////////////////////////////////	
}
void surf (Input IN, inout SurfaceOutput o) {
	float4 splat_control = tex2D(_Control2, IN.uv_Control);

	#if defined(COLOR_MAP) || defined(RTP_SNOW)
		float3 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control).rgb;	
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif	

	#ifdef RTP_SNOW
		float snow_val=_snow_strength*2;
		float snow_height_fct=saturate((_snow_height_treshold - IN.snowDir.w)/_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += _snow_strength*dot(1-global_color_value.rgb, _global_color_brightness_to_snow);
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= _snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		snow_val*=snow_val;
		
	 	fixed3 col;
		col = splat_control.r * lerp(tex2D(_SplatB0, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.x );
		col += splat_control.g * lerp(tex2D(_SplatB1, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.y );
		col += splat_control.b * lerp(tex2D(_SplatB2, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.z );
		col += splat_control.a * lerp(tex2D(_SplatB3, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.w );
		
		global_color_value.rgb=lerp(global_color_value.rgb, _snow_color, snow_val);
	#else		
	 	fixed3 col;
		col = splat_control.r * tex2D(_SplatB0, IN._uv_Relief.xy).rgb;
		col += splat_control.g * tex2D(_SplatB1, IN._uv_Relief.xy).rgb;
		col += splat_control.b * tex2D(_SplatB2, IN._uv_Relief.xy).rgb;
		col += splat_control.a * tex2D(_SplatB3, IN._uv_Relief.xy).rgb;
	#endif
	
	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col=lerp(col, col*global_color_value.rgb*2, _GlobalColorMapBlendValues.y);
		#else
			col=lerp(col, global_color_value.rgb, _GlobalColorMapBlendValues.y);
		#endif		
	#endif	
	
	o.Albedo = col;
}
ENDCG  
//*/ // AddPass

}
// EOF CLASSIC shading

// Fallback to Diffuse
Fallback "Diffuse"
}
