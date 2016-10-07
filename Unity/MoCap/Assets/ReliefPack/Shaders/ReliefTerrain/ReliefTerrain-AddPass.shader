Shader "Hidden/TerrainEngine/Splatmap/Lightmap-AddPass" {
Properties {
	_Control ("Control (RGBA)", 2D) = "black" {}
	_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	_Splat2 ("Layer 2 (B)", 2D) = "white" {}
	_Splat1 ("Layer 1 (G)", 2D) = "white" {}
	_Splat0 ("Layer 0 (R)", 2D) = "white" {}
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
		"SplatCount" = "4"
		"Queue" = "Geometry-99"
		"IgnoreProjector"="True"
		"RenderType" = "Opaque"
	}
LOD 700	
//Offset -1,-1
ZTest LEqual
ZWrite Off Fog { Mode Off }
CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert finalcolor:customFog fullforwardshadows decal:blend noforwardadd nolightmap
	#include "UnityCG.cginc"
	   
	#pragma target 3.0
	#pragma glsl
	#pragma only_renderers d3d9
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	//#define RTP_PM_SHADING
	//#define RTP_SIMPLE_SHADING
	
	// for geom blend (early exit from sur function)
	//#define COLOR_EARLY_EXIT
	
	#define APPROX_TANGENTS
	
	// routing global color, global normal, 2nd splat and perlin combined to _Splat0 - _Splat3
	//#define TEX_SPLAT_REDEFINITION

	#include "RTP_AddBase.cginc"

ENDCG  

}
// EOF POM / PM / SIMPLE shading
*/ // INIT


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
		"SplatCount" = "4"
		"Queue" = "Geometry-99"
		"IgnoreProjector"="True"
		"RenderType" = "Opaque"
	}
	LOD 100	
ZTest LEqual
//Offset -1,-1
CGPROGRAM
	#pragma surface surf Lambert vertex:vert decal:add noforwardadd nolightmap
	#include "UnityCG.cginc"
	
	#pragma only_renderers d3d9
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	//#define ADDITIONAL_FEATURES_IN_FALLBACKS
	
	// routing global color, global normal, 2nd splat and perlin combined to _Splat0 - _Splat3
	//#define TEX_SPLAT_REDEFINITION
		
	#ifdef ADDITIONAL_FEATURES_IN_FALLBACKS	
		// comment if you don't need global color map
		#define COLOR_MAP
		// if not defined global color map will be blended (lerp)
		#define COLOR_MAP_BLEND_MULTIPLY
		
		#define RTP_SNOW
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
		sampler2D _Control, _Control3;
	#endif	
/////////////////////////////////////////////////////////////////////

sampler2D _SplatC0,_SplatC1,_SplatC2,_SplatC3;

float4 _TERRAIN_ReliefTransform;
half _Shininess;
float4 _Spec89AB;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef COLOR_MAP
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
sampler2D _ColorMapGlobal;
#endif
#ifdef RTP_SNOW
float rtp_snow_strength;
float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer89AB;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;
fixed4 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
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
		o.snowDir.xyz = normalize( mul(_Object2World, float4(v.normal.xyz,0)).xyz );
		o.snowDir.w = v.vertex.y;
	#endif	
/////////////////////////////////////////////////////////////////////
	
}

void surf (Input IN, inout SurfaceOutput o) {
	float4 splat_control = tex2D(_Control3, IN.uv_Control);
	
 	float total_coverage=dot(splat_control, 1);
	
	#if defined(COLOR_MAP) || defined(RTP_SNOW)
		float global_color_blend=_GlobalColorMapBlendValues.y*total_coverage;
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.uv_Control);
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif	
		
	#ifdef RTP_SNOW
		float snow_val=rtp_snow_strength*2;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.snowDir.w)/rtp_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow);
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= rtp_snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		snow_val*=snow_val;
		
	 	fixed4 col;
		col = splat_control.r * lerp(tex2D(_SplatC0, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer89AB.x );
		col += splat_control.g * lerp(tex2D(_SplatC1, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer89AB.y );
		col += splat_control.b * lerp(tex2D(_SplatC2, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer89AB.z );
		col += splat_control.a * lerp(tex2D(_SplatC3, IN._uv_Relief.xy), rtp_snow_color, snow_val*rtp_snow_strength_per_layer89AB.w );
				
		global_color_value.rgb=lerp(global_color_value.rgb, rtp_snow_color.rgb, snow_val);
	#else		
	 	fixed4 col;
		col = splat_control.r * tex2D(_SplatC0, IN._uv_Relief.xy);
		col += splat_control.g * tex2D(_SplatC1, IN._uv_Relief.xy);
		col += splat_control.b * tex2D(_SplatC2, IN._uv_Relief.xy);
		col += splat_control.a * tex2D(_SplatC3, IN._uv_Relief.xy);
	#endif
	
	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col.rgb=lerp(col.rgb, col.rgb*global_color_value.rgb*2, global_color_blend);
		#else
			col.rgb=lerp(col.rgb, global_color_value.rgb, global_color_blend);
		#endif		
	#endif	
		
	o.Albedo = col.rgb;
	//o.Alpha = 0;
	//o.Gloss = col.a*total_coverage;
	//o.Specular = dot(_Spec89AB, splat_control);
}
ENDCG  

}

Fallback off
}
