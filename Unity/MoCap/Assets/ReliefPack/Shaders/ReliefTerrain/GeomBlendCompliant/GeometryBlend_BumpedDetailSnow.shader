//
// Relief Terrain Geometry Blend shader
// Tomasz Stobierski 2013
//
//  Relief Terrain Pack here consits of  simple geometry blending shaders - only bumped specular
//	If you'd like to replace them with better Parallax Occlusion Mapped with self-shadow - take them from twin product - Relief Shaders Pack
//  - they're specialy tailored for such usage (just copy/paste here bonus file contents)
//
Shader "Relief Pack/GeometryBlend_BumpedDetailSnow" {
Properties {
		//
		// keep in mind that not all of properties are used, depending on shader configuration in defines section below
		//
		_Color ("Color (RGBA)", Color) = (1,1,1,1)
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0, 100)) = 80
		_MainTex ("Main texture (RGBA)", 2D) = "black" {}
		_BumpMap ("Normal map", 2D) = "bump" {}
		//_DetailColor ("Detail Color (RGBA)", Color) = (1, 1, 1, 1)
		_DetailBumpTex ("Detail Normalmap", 2D) = "bump" {}
		_DetailScale ("Detail Normal Scale", Float) = 1

		rtp_snow_mult("Snow multiplicator", Range(0,2)) = 1
		_ColorSnow ("Snow texture (RGBA)", 2D) = "white" {}
		_BumpMapSnow ("Snow Normalmap", 2D) = "bump" {}
		_distance_start("Snow near distance", Float) = 10
		_distance_transition("Snow distance transition length", Range(0,100)) = 20
}


SubShader {
	Tags {
		"Queue" = "Geometry+10"
		"RenderType" = "Opaque"
	}
	LOD 700

Offset -1,-1
ZTest LEqual
CGPROGRAM
#pragma surface surf CustomBlinnPhong vertex:vert fullforwardshadows decal:blend
#pragma target 3.0
#pragma glsl
#pragma only_renderers d3d9
#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING

#include "UnityCG.cginc"

#define detail_map_enabled

/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#define RTP_SNOW
	//#define RTP_SNW_CHOOSEN_LAYER_NORM
	//#define RTP_SNW_CHOOSEN_LAYER_COLOR
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef RTP_SNOW
float rtp_snow_strength;
float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer0123;
float4 rtp_snow_strength_per_layer4567;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;
fixed3 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
float rtp_snow_mult;
float rtp_snow_deep_factor;

sampler2D _ColorSnow;
sampler2D _BumpMapSnow;
sampler2D mipFilterTex;

float4 mipFilterTex_TexelSize;
float4 _MainTex_TexelSize;
float4 _BumpMap_TexelSize;
#endif
////////////////////////////////////////////////////////////////////

sampler2D _MainTex;
sampler2D _BumpMap;
float4 _Color;
float _Shininess;

half _distance_start;
half _distance_transition;

fixed4 _DetailColor;
float _DetailScale;
sampler2D _DetailBumpTex;

float4 _MainTex_ST;

struct Input {
	float4 _uv_MainTex;
	float2 uv_ColorSnow;
	float dist;
	float4 snowDir;
	float4 color:COLOR;
};

	// Custom & reflexLighting
	half3 rtp_customAmbientCorrection;

	#define RTP_BackLightStrength RTP_LightDefVector.x
	#define RTP_ReflexLightDiffuseSoftness RTP_LightDefVector.y
	#define RTP_ReflexLightSpecSoftness RTP_LightDefVector.z
	#define RTP_ReflexLightSpecularity RTP_LightDefVector.w
	float4 RTP_LightDefVector;
	half4 RTP_ReflexLightDiffuseColor;
	half4 RTP_ReflexLightSpecColor;	
	
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
		diff = abs( dot(normForDiffuse, lightDirRefl) )*glossDiff; 
		c.rgb += (s.Albedo * RTP_ReflexLightDiffuseColor.rgb * diff);
		
		// 2nd reflex
		lightDirRefl.x=dot(lightDir.xz, refl_rot.yx);
		lightDirRefl.z=dot(lightDir.xz, refl_rot.zy);	
		diff = abs( dot(normForDiffuse, lightDirRefl) )*glossDiff;
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
	// EOF Custom & reflexLighting
	
void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_MainTex.xy=v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		float3 worldPos=mul (_Object2World, v.vertex).xyz;
		o.dist=distance(worldPos, _WorldSpaceCameraPos);
		o.dist=saturate((o.dist.x - _distance_start) / _distance_transition);
		
		#if (!defined(SHADER_API_OPENGL))
			o._uv_MainTex.zw=o._uv_MainTex.xy / _MainTex_TexelSize.x;
		#endif	
		
		TANGENT_SPACE_ROTATION;
		o.snowDir.xyz = mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz);
		o.snowDir.w = mul(_Object2World, v.vertex).y;
	#endif	
/////////////////////////////////////////////////////////////////////	
}

void surf (Input IN, inout SurfaceOutput o) {
//	o.Emission.rg=frac(IN._uv_MainTex.xy);
//	o.Alpha=1;
//	return;
	
	float4 tex = tex2D(_MainTex, IN._uv_MainTex.xy);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN._uv_MainTex.xy));
	
	#ifndef RTP_SIMPLE_SHADING
	#ifdef detail_map_enabled
		float3 norm_det=UnpackNormal(tex2D(_DetailBumpTex, IN._uv_MainTex.xy*_DetailScale));
		o.Normal+=2*norm_det;//*_DetailColor.a;
		o.Normal=normalize(o.Normal);
	#endif
	#endif
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		float snow_val=rtp_snow_strength*2*rtp_snow_mult;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.snowDir.w)/rtp_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow;
		float3 norm_for_snow=float3(0,0,1);
		snow_val -= rtp_snow_slope_factor*( 1 - dot(norm_for_snow, IN.snowDir.xyz) );

		float snow_depth=snow_val-1;
		snow_depth=snow_depth<0 ? 0:snow_depth*6;		
		
		snow_val -= rtp_snow_slope_factor*( 1 - dot(o.Normal, IN.snowDir.xyz));
		snow_val=saturate(snow_val);
		snow_val=pow(abs(snow_val), rtp_snow_edge_definition);
		
		float snow_depth_lerp=saturate(snow_depth-rtp_snow_deep_factor);
		
		#ifdef RTP_SNW_CHOOSEN_LAYER_COLOR
		#ifndef RTP_SIMPLE_SHADING
			half4 c=tex2D(_ColorSnow, IN.uv_ColorSnow);
			float3 rtp_snow_color_tex=c.rgb;
			rtp_snow_gloss=c.a;
			rtp_snow_color=lerp(rtp_snow_color_tex, rtp_snow_color, IN.dist);
		#endif
		#endif
		
		o.Albedo=lerp( o.Albedo, rtp_snow_color, snow_val );
		
	 	#if (!defined(SHADER_API_OPENGL))
	 		// DX - the trick with mipFilterTex seems not working right on some GPUs (Intel HD3000)
	 		// if your target hardware platform is different you might use mipFilterTex solution only (it's a bit faster than ddx/ddy calls)
			float2 dx = ddx( IN._uv_MainTex.zw );
			float2 dy = ddy( IN._uv_MainTex.zw );
			float d = max( dot( dx, dx ), dot( dy, dy ) );
			float mip_selector=min(0.5*log2(d), 8);
	 	#else
			float mip_selector=min(max(0, tex2D(mipFilterTex, IN._uv_MainTex.zw).a*9-log2(_MainTex_TexelSize.x/mipFilterTex_TexelSize.x)), 8);
		#endif
		float mip_selector_bumpMap=	max(0,mip_selector-log2(_BumpMap_TexelSize.x/_MainTex_TexelSize.x));
		float3 snow_normal=UnpackNormal(tex2Dlod(_BumpMap, float4(IN._uv_MainTex.xy, mip_selector_bumpMap.xx+snow_depth.xx)));
		
		#ifdef RTP_SNW_CHOOSEN_LAYER_NORM
			float3 n=UnpackNormal(tex2D(_BumpMapSnow, IN.uv_ColorSnow));
			snow_normal=lerp(snow_normal, n, snow_depth_lerp );
			snow_normal=normalize(snow_normal);
		#endif
		
		o.Normal=lerp(o.Normal, snow_normal, snow_val);		
		//o.Normal=normalize(o.Normal);
		
		o.Gloss=lerp(o.Gloss, rtp_snow_gloss, snow_val);
		o.Specular=lerp(o.Specular, rtp_snow_specular, snow_val);	
	#endif
/////////////////////////////////////////////////////////////////////
	
	// we need to blend it this way so that point lights work fine with shader
	//o.Alpha=o.Gloss;
	o.Alpha=1-IN.color.a;
}

ENDCG
}


SubShader {
	Tags {
		"Queue" = "Geometry+10"
		"RenderType" = "Opaque"
	}
	LOD 100

Offset -1,-1
ZTest LEqual
CGPROGRAM
#pragma surface surf Lambert vertex:vert decal:blend
#pragma target 3.0
#pragma glsl
#pragma only_renderers d3d9
#pragma debug
#include "UnityCG.cginc"

/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#define RTP_SNOW
/////////////////////////////////////////////////////////////////////

sampler2D _MainTex;
float4 _Color;
float _Shininess;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef RTP_SNOW
float rtp_snow_strength;
float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer0123;
float4 rtp_snow_strength_per_layer4567;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;
fixed3 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
float rtp_snow_mult;
float rtp_snow_deep_factor;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float2 uv_MainTex;
	float4 snowDir;
	float4 color:COLOR;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
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
	float4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a * _Color.a;
	o.Specular = _Shininess;
	
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		float snow_val=rtp_snow_strength*2*rtp_snow_mult;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.snowDir.w)/rtp_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow;
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= rtp_snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		snow_val*=snow_val;
		o.Albedo=lerp( o.Albedo, rtp_snow_color, snow_val );
		
		#if !defined(SHADER_API_FLASH)
			o.Gloss=lerp(o.Gloss, rtp_snow_gloss, snow_val);
			o.Specular=lerp(o.Specular, rtp_snow_specular, snow_val);			
		#endif
	#endif
/////////////////////////////////////////////////////////////////////

	// we need to blend it this way so that point lights work fine with shader
	//o.Alpha=o.Gloss;
	o.Alpha=1-IN.color.a;
	#ifdef UNITY_PASS_PREPASSFINAL
		o.Gloss=0;
	#endif
}
ENDCG

}

// Fallback to Diffuse
Fallback "Diffuse"
}
