//
// Relief Terrain  -  Parallax mapped material with height blending
// Tomasz Stobierski 2013
//
// distances have the same meaning like in RTP inspector
//
// we're using position (x,z), size (x,z) and tiling of underlying terrain to get right coords of global perlin map and global colormap
// 
// vertex color - g channel controls darkening (on wet)
// vertex color - b channel controls gloss (wet)
//
Shader "Relief Pack/GeometryBlend_PM_HB_COMPLEX" {
    Properties {
		_MainColor ("Main Color (RGB)", Color) = (0.5, 0.5, 0.5, 1)		
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.5
		_ExtrudeHeight ("Extrude Height", Range(0.001,0.1)) = 0.04
		
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap ("Bumpmap", 2D) = "bump" {}
		_HeightMap ("Heightmap (A)", 2D) = "black" {}
		
		_uvBlendScale ("UV blend scale", Float) = 0.2
		_uvBlendAmount ("UV blend amount", Range(0,1)) = 0.5
		_uvBlendSaturation ("UV blend saturation", Range(0,1)) = 0.5
		
		_VerticalTexture ("Vertical texture", 2D) = "grey" {}
		_VerticalTextureTiling ("Vertical texture tiling", Float) = 30
		_VerticalTextureStrength ("Vertical texture amount", Range(0,1)) = 0.5
		_VerticalTextureGlobalBumpInfluence ("Vertical texture perlin distorion", Range(0,0.1)) = 0.01		
		
		_BumpMapPerlin ("Perlin global (special combined RG)", 2D) = "black" {}
		_BumpMapPerlinBlend ("Perlin normal", Range(0,2)) = 0.3
		_BumpMapGlobalScale ("Tiling scale", Range( 0.01,0.25) ) = 0.1
		_TERRAIN_distance_start ("Near distance start", Float) = 2
		_TERRAIN_distance_transition ("Near fade length", Float) = 20
		_TERRAIN_distance_start_bumpglobal ("Far distance start", Float) = 22
		_TERRAIN_distance_transition_bumpglobal ("Far fade length", Float) = 40
		rtp_perlin_start_val ("Perlin start val", Range(0,1)) = 0.3
				
		_ColorMapGlobal ("Global colormap", 2D) = "black" {}
		
		_TERRAIN_PosSize ("Terrain pos (xz to XY) & size(xz to ZW)", Vector) = (0,0,1000,1000)
		_TERRAIN_Tiling ("Terrain tiling (XY) & offset(ZW)", Vector) = (3,3,0,0)
		
		_TERRAIN_HeightMap ("Terrain HeightMap (combined)", 2D) = "white" {}
		_TERRAIN_Control ("Terrain splat controlMap", 2D) = "white" {}
		
		// caustics
		TERRAIN_CausticsTex ("Caustics", 2D) = "black" {}
		TERRAIN_CausticsColor ("Caustics color", Color) = (1,1,1,1)
		TERRAIN_CausticsAnimSpeed ("Caustics anim speed", Float) = 1
		TERRAIN_CausticsTilingScale ("Caustics tiling", Float) = 1	
		TERRAIN_CausticsWaterLevel ("Caustics water level", Float) = 0
		TERRAIN_CausticsWaterDeepFadeLength ("Caustics deep fade", Float) = 15
		TERRAIN_CausticsWaterShallowFadeLength ("Caustics shallow fade", Float) = 4
		TERRAIN_WetnessDark("Wetness darkening (vertex color G)", Range(0,1)) = 0.7
		TERRAIN_WetnessGloss("Wetness glossiness (vertex color B)", Range(0,1)) = 1
		TERRAIN_WetnessSpecularity("Wetness shininess", Range(0.01,1)) = 0.8
    }
    SubShader {
	Tags {
		"Queue" = "Geometry+10"
		"RenderType" = "Opaque"
	}
	
	Offset -1,-1
	ZTest LEqual	
	CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert decal:blend
	#pragma only_renderers d3d9 opengl d3d11
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	#pragma target 3.0
	#pragma glsl
	
	// edges can be heightblended
	#define BLENDING_HEIGHT
	
	// we'll use global colormap
	#define COLOR_MAP
	#define COLOR_MAP_BLEND_MULTIPLY
	// we'll use global perlin
	#define GLOBAL_PERLIN
	// we'll show only global colormap at far distance (no detail)
	//#define SIMPLE_FAR
	
	// uv blend feature like in terrain, here we're using 1 layer for blending purposes
	#define UV_BLEND
	// blend is realised by color multiplication
	#define UV_BLEND_MULTIPLY
	// unlike in terrain we're blending normals, too
	#define UV_BLEND_NORMALS
	
	// vertical texture 
	#define VERTICAL_TEXTURE		
	
	// caustics
	#define RTP_CAUSTICS
	
	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMapPerlin;
		float4 aux;
		float2 globalUV;
		float3 viewDir;
		
		float4 color:COLOR;
	};
	
	half3 _MainColor;		
	float _Shininess;
	float _ExtrudeHeight;
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _HeightMap;
	
	float _uvBlendScale;
	float _uvBlendAmount;
	float _uvBlendSaturation;
	
	sampler2D _VerticalTexture;
	float _VerticalTextureTiling;
	float _VerticalTextureStrength;
	float _VerticalTextureGlobalBumpInfluence;
		
	sampler2D _ColorMapGlobal;
	sampler2D _BumpMapPerlin;
	float _BumpMapGlobalScale;
	float _BumpMapPerlinBlend;
	
	float rtp_perlin_start_val;		
	float _TERRAIN_distance_start;
	float _TERRAIN_distance_transition;
	float _TERRAIN_distance_start_bumpglobal;
	float _TERRAIN_distance_transition_bumpglobal;
	
	float4 _TERRAIN_PosSize;
	float4 _TERRAIN_Tiling;
	
	// set globaly by ReliefTerrain script
	float4 _GlobalColorMapBlendValues;
	float _GlobalColorMapSaturation;
	
	sampler2D _TERRAIN_HeightMap;
	sampler2D _TERRAIN_Control;
	
	// caustics
	float TERRAIN_CausticsAnimSpeed;
	half4 TERRAIN_CausticsColor;
	float TERRAIN_CausticsTilingScale;
	sampler2D TERRAIN_CausticsTex;
	float TERRAIN_CausticsWaterLevel;
	float TERRAIN_CausticsWaterDeepFadeLength;
	float TERRAIN_CausticsWaterShallowFadeLength;
	float TERRAIN_WetnessDark;
	float TERRAIN_WetnessGloss;
	float TERRAIN_WetnessSpecularity;
	
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
		
		float3 Wpos=mul(_Object2World, v.vertex);
		o.aux.xy=Wpos.xz-_TERRAIN_PosSize.xy+_TERRAIN_Tiling.zw;
		o.aux.xy/=_TERRAIN_Tiling.xy;
		o.aux.z=length(_WorldSpaceCameraPos.xyz-Wpos);
		
		o.aux.w = Wpos.y;
				
		o.globalUV=Wpos.xz-_TERRAIN_PosSize.xy;
		o.globalUV/=_TERRAIN_PosSize.zw;
		
		//float far=saturate((o._uv_Relief.w - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		//v.normal.xyz=lerp(v.normal.xyz, float3(0,1,0), far*_FarNormalDamp);
	}
	
	void surf (Input IN, inout SurfaceOutput o) {
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN)
		float _uv_Relief_z=saturate((IN.aux.z - _TERRAIN_distance_start) / _TERRAIN_distance_transition);
		_uv_Relief_z=1-_uv_Relief_z;
		float _uv_Relief_w=saturate((IN.aux.z - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		#endif
		
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN)
		float global_color_blend=lerp( lerp(_GlobalColorMapBlendValues.y, _GlobalColorMapBlendValues.x, _uv_Relief_z*_uv_Relief_z), _GlobalColorMapBlendValues.z, _uv_Relief_w);
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.globalUV);
		#ifdef SIMPLE_FAR	
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*1.3+0.2),_uv_Relief_w));
		#else
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
		#endif
		#endif

      	float tH=tex2D(_HeightMap, IN.uv_MainTex).a;
		#if !defined(RTP_SIMPLE_SHADING) 
	      	float2 uv=IN.uv_MainTex + ParallaxOffset(tH, _ExtrudeHeight, IN.viewDir.xyz)*(1-IN.color.a);
      	#endif
      	float control=(tH+0.01);
		#if !defined(RTP_SIMPLE_SHADING) 
	      	float2 uvMixed=uv;
		#else
	      	float2 uvMixed=IN.uv_MainTex;
		#endif
      	
      	fixed4 col=tex2D(_MainTex, uvMixed);
      	o.Albedo=col.rgb;

      	#ifdef UV_BLEND
			#ifdef UV_BLEND_MULTIPLY
				half3 colBlend=tex2D(_MainTex, uvMixed*_uvBlendScale).rgb;
				colBlend=lerp( (dot(colBlend, 0.3333)).xxx, colBlend, _uvBlendSaturation);
	      		o.Albedo=lerp(o.Albedo, o.Albedo*colBlend*2, _uvBlendAmount);
			#else
	      		o.Albedo=lerp(o.Albedo, tex2D(_MainTex, uvMixed*_uvBlendScale).rgb, _uvBlendAmount);
			#endif      	
      	#endif
      	      	
		#ifdef SIMPLE_FAR      	
		o.Albedo=lerp(o.Albedo, global_color_value.rgb, _uv_Relief_w);
		#endif
      	o.Gloss=col.a;
      	
      	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			o.Albedo=lerp(o.Albedo, o.Albedo*global_color_value.rgb*2, global_color_blend);
		#else
			o.Albedo=lerp(o.Albedo, global_color_value.rgb, global_color_blend);
		#endif      	
		#endif
      	
      	#ifdef UV_BLEND_NORMALS
			o.Normal=UnpackNormal( lerp(tex2D(_BumpMap,uvMixed), tex2D(_BumpMap,uvMixed*_uvBlendScale), _uvBlendAmount) );
		#else
			o.Normal=UnpackNormal( tex2D(_BumpMap,uvMixed) );
		#endif
      	#ifdef GLOBAL_PERLIN
	      	//float perlinmask=tex2D(_BumpMapGlobal, IN.aux.xy/8).r;
			//float4 global_bump_val=tex2D(_BumpMapPerlin, IN.aux.xy*_BumpMapGlobalScale);
			float4 global_bump_val=tex2D(_BumpMapPerlin, IN.uv_MainTex*_BumpMapGlobalScale);
	
			float3 norm_far;
			norm_far.xy = global_bump_val.rg*2-1;
			norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));      	
			
			o.Normal+=norm_far*lerp(rtp_perlin_start_val,1, _uv_Relief_w)*_BumpMapPerlinBlend;	
			o.Normal=normalize(o.Normal);
		#endif	      	
      	o.Specular=_Shininess;
      	
		#ifdef VERTICAL_TEXTURE
			float2 vert_tex_uv=float2(0, IN.aux.w/_VerticalTextureTiling);
			#ifdef GLOBAL_PERLIN
				vert_tex_uv += _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
			#endif
			half3 vert_tex=tex2D(_VerticalTexture, vert_tex_uv).rgb;
			o.Albedo=lerp( o.Albedo, o.Albedo*vert_tex*2, _VerticalTextureStrength );
		#endif
		     	
		#if defined(BLENDING_HEIGHT)
			float4 terrain_coverage=tex2D(_TERRAIN_Control, IN.globalUV);
			float4 splat_control1=terrain_coverage * tex2D(_TERRAIN_HeightMap, IN.aux.xy) * IN.color.a;
			float4 splat_control2=float4(control, 0, 0, 0) * (1-IN.color.a);
			
			float blend_coverage=dot(terrain_coverage, 1);
			if (blend_coverage>0.1) {
			
				splat_control1*=splat_control1;
				splat_control1*=splat_control1;
				splat_control2*=splat_control2;
				splat_control2*=splat_control2;
				
				float normalize_sum=dot(splat_control1, 1)+dot(splat_control2, 1);
				splat_control1 /= normalize_sum;
				splat_control2 /= normalize_sum;		
				
				o.Alpha=dot(splat_control2,1);
				o.Alpha=lerp(1-IN.color.a, o.Alpha, saturate((blend_coverage-0.1)*4) );
			} else {
				o.Alpha=1-IN.color.a;
			}
		#else
			o.Alpha=1-IN.color.a;
		#endif
		
		o.Albedo*=_MainColor*2;	
		o.Albedo*=1-IN.color.g*TERRAIN_WetnessDark;
		float wetGlossAdd=IN.color.b*TERRAIN_WetnessGloss;
		o.Gloss+=wetGlossAdd;
		o.Specular=lerp(o.Specular, TERRAIN_WetnessSpecularity, wetGlossAdd);
		
		#ifdef RTP_CAUSTICS
		{
			float damp_fct_caustics=saturate((IN.aux.w-TERRAIN_CausticsWaterLevel+TERRAIN_CausticsWaterDeepFadeLength)/TERRAIN_CausticsWaterDeepFadeLength);
			float overwater=saturate(-(IN.aux.w-TERRAIN_CausticsWaterLevel-TERRAIN_CausticsWaterShallowFadeLength)/TERRAIN_CausticsWaterShallowFadeLength);
			damp_fct_caustics*=overwater;	
			
			float tim=_Time.x*TERRAIN_CausticsAnimSpeed;
			uvMixed+=IN.aux.xy-IN.uv_MainTex; // topplanar uv+parallax offset
			uvMixed*=TERRAIN_CausticsTilingScale;
			float3 _Emission=tex2D(TERRAIN_CausticsTex, uvMixed+float2(tim, tim) ).rgb;
			_Emission+=tex2D(TERRAIN_CausticsTex, uvMixed+float2(-tim, -tim*0.873) ).rgb;
			_Emission+=tex2D(TERRAIN_CausticsTex, uvMixed*1.1+float2(tim, 0) ).rgb;
			_Emission+=tex2D(TERRAIN_CausticsTex, uvMixed*0.5+float2(0, tim*0.83) ).rgb;
			_Emission=saturate(_Emission-1.6);
			_Emission*=_Emission;
			_Emission*=_Emission;
			_Emission*=damp_fct_caustics;			
			_Emission*=TERRAIN_CausticsColor.rgb*3;
			o.Emission+=_Emission;
		} 
		#endif	
	}
	ENDCG
      
    } 
    Fallback "Decal"
}
