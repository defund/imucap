//
// Relief Terrain  - Parallax mapped material + water
// Tomasz Stobierski 2013
//
// (water mask taken from vertex color B)
//
Shader "Relief Pack/WaterShader" {
    Properties {
       TERRAIN_LayerReflection ("Layer reflection (water independent)", Range(0,2)) = 1
 	   TERRAIN_FresnelPow ("Fresnel Pow", Range(0.5,32)) = 2
	   TERRAIN_FresnelOffset ("Fresnel Offset", Range(0.0,0.9)) = 0.2
       
       TERRAIN_LayerWetStrength ("Layer wetness", Range(0,1)) = 1
       
       TERRAIN_WaterLevelSlopeDamp ("Water level slope damp", Range(0.25,8)) = 4
	   TERRAIN_ExtrudeHeight ("Extrude Height", Range(0.001,0.08)) = 0.02 
		
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.5
      TERRAIN_WaterSpecularity ("Water Shininess (Specularity)", Range (0.01, 1)) = 0.5
		
      _MainTex ("Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
      _HeightMap ("Heightmap", 2D) = "black" {}
      
      TERRAIN_FlowingMap ("Flowingmap (water bumps)", 2D) = "gray" {}
      TERRAIN_RippleMap ("Ripplemap", 2D) = "gray" {}
      TERRAIN_RippleScale ("Ripple scale", Float) = 1
      
      TERRAIN_WaterLevel ("Water Level", Range(0,2)) = 0.5
      TERRAIN_WaterColor ("Water Color", Color) = (1,1,1,1)
      TERRAIN_WaterEdge ("Water Edge", Range(1, 4)) = 1
      TERRAIN_WaterOpacity ("Water Opacity", Range(0,1)) =0.5
	  TERRAIN_WaterGloss ("Water Gloss", Range(0,1)) = 1
      TERRAIN_DropletsSpeed ("Droplets speed", Float) = 15
      TERRAIN_RainIntensity ("Rain intensity", Range(0,1)) = 1
      TERRAIN_WetDropletsStrength ("Rain on wet", Range(0,1)) = 0
	  TERRAIN_Refraction("Refraction", Range(0,0.04)) = 0.02
	  TERRAIN_WetRefraction ("Wet refraction", Range(0,1)) = 1
	  TERRAIN_Flow ("Flow", Range(0, 1)) = 0.1
	  TERRAIN_FlowScale ("Flow Scale", Float) = 1
	  TERRAIN_FlowSpeed ("Flow Speed", Range(0, 3)) = 0.25
	  TERRAIN_WetSpecularity ("Wet gloss", Range(0, 1)) = 0.1
	  TERRAIN_WetReflection ("Wet reflection", Range(0, 2)) = 0.5
	  
	  _ReflectionMap ("ReflectionMap", 2D) = "gray" {}
	  TERRAIN_ReflColorA ("Reflection Color Hi", Color) = (1,1,1,1)
	  TERRAIN_ReflColorB ("Reflection Color Low", Color) = (1,1,1,1)
	  TERRAIN_ReflDistortion ("Reflection distortion", Range(0,0.1)) = 0.05
	  TERRAIN_ReflectionRotSpeed ("Reflection Rot Speed", Range(0,2)) = 0.3
    }
    SubShader {
	Tags { "RenderType" = "Opaque" }
	CGPROGRAM
	#pragma surface surf BlinnPhong vertex:vert
	#pragma only_renderers d3d9 opengl d3d11
	#pragma glsl
	#pragma target 3.0
	
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	
	#define RTP_REFLECTION
	#define RTP_ROTATE_REFLECTION
	
	#define RTP_WETNESS
	// enabled below if you don't want to use water flow
	//#define SIMPLE_WATER
	#define RTP_WET_RIPPLE_TEXTURE	  
	
	struct Input {
		float2 uv_MainTex;
		
		float3 viewDir;
		float4 _auxDir;
		
		float4 color:COLOR;
	};
	
	float TERRAIN_LayerWetStrength;
	float TERRAIN_WaterLevelSlopeDamp;
	float TERRAIN_ExtrudeHeight;
	
	float _Shininess;
	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D TERRAIN_RippleMap;
	sampler2D TERRAIN_FlowingMap;
	
	sampler2D _HeightMap;
	
	sampler2D _ReflectionMap;
	half4 TERRAIN_ReflColorA;
	half4 TERRAIN_ReflColorB;
	float TERRAIN_ReflDistortion;
	float TERRAIN_ReflectionRotSpeed;
	
	float TERRAIN_DropletsSpeed;
	float TERRAIN_RainIntensity;
	float TERRAIN_WetDropletsStrength;
	float TERRAIN_Refraction;
	float TERRAIN_WetRefraction;
	float TERRAIN_Flow;
	float TERRAIN_FlowScale;
	float TERRAIN_RippleScale;
	float TERRAIN_FlowSpeed;
	float TERRAIN_WetSpecularity;
	float TERRAIN_WetReflection;
	float TERRAIN_LayerReflection; // niezalezne od wody i mokrości
	float TERRAIN_WaterLevel;
	half4 TERRAIN_WaterColor;
	float TERRAIN_WaterEdge;
	float TERRAIN_WaterSpecularity;
	float TERRAIN_WaterGloss;
	float TERRAIN_WaterOpacity;
	float TERRAIN_FresnelPow;
	float TERRAIN_FresnelOffset;
      
	inline float2 GetRipple(float2 UV, float Intensity)
	{
	    float4 Ripple = tex2D(TERRAIN_RippleMap, UV);
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
		#if defined(RTP_REFLECTION) || defined(RTP_WETNESS)
			float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal.xyz );		
			
			float3 viewDir = -ObjSpaceViewDir(v.vertex);
			float3 viewRefl = reflect (viewDir, v.normal);
			float2 refl_vec = normalize(mul((float3x3)_Object2World, viewRefl)).xz;
			#ifdef RTP_ROTATE_REFLECTION
				float3 refl_rot;
				refl_rot.x=sin(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.y=cos(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.z=-refl_rot.x;
				o._auxDir.x=dot(refl_vec, refl_rot.yz);
				o._auxDir.y=dot(refl_vec, refl_rot.xy);
			#else
				o._auxDir.xy=refl_vec;
			#endif
			o._auxDir.xy=o._auxDir.xy*0.5+0.5;
		#endif
		#if defined(RTP_WETNESS)
		o._auxDir.zw = ( mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz) ).xy;		
		#endif
	}
	
	void surf (Input IN, inout SurfaceOutput o) {
      	float3 rayPos;
      	rayPos.z=tex2D(_HeightMap, IN.uv_MainTex).a;
      	#if defined(RTP_SIMPLE_SHADING)
      	rayPos.xy=IN.uv_MainTex;
      	#else
      	rayPos.xy=IN.uv_MainTex + ParallaxOffset(rayPos.z, TERRAIN_ExtrudeHeight, IN.viewDir.xyz);
      	#endif
      	
		float3 flat_dir;
		flat_dir.xy=IN._auxDir.zw;
		flat_dir.z=sqrt(1 - saturate(dot(flat_dir.xy, flat_dir.xy)));
		float wetSlope=1-flat_dir.z;

		float perlinmask=tex2D(TERRAIN_FlowingMap, IN.uv_MainTex/8).a;
		TERRAIN_LayerWetStrength*=saturate(IN.color.b*2 - perlinmask*(1-TERRAIN_LayerWetStrength)*2);
		float2 roff=0;
		wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
		float _RippleDamp=saturate(TERRAIN_LayerWetStrength*2-1)*saturate(1-wetSlope*4);
		TERRAIN_RainIntensity*=_RippleDamp;
		TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
		TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
		TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*rayPos.z);
		TERRAIN_Flow*=TERRAIN_LayerWetStrength*TERRAIN_LayerWetStrength;
		
		float p = saturate((TERRAIN_WaterLevel-rayPos.z)*TERRAIN_WaterEdge);
		p*=p;
		#if !defined(RTP_SIMPLE_SHADING) && !defined(SIMPLE_WATER)
			float2 flowUV=lerp(IN.uv_MainTex, rayPos.xy, 1-p*0.5)*TERRAIN_FlowScale;
			float _Tim=frac(_Time.x*4)*2;
			float ft=abs(frac(_Tim)*2 - 1);
			float2 flowSpeed=clamp((IN._auxDir.zw+0.01)*4,-1,1)/4;
			flowSpeed*=TERRAIN_FlowSpeed*TERRAIN_FlowScale;
			float2 flowOffset=tex2D(TERRAIN_FlowingMap, flowUV+frac(_Tim.xx)*flowSpeed).ag*2-1;
			flowOffset=lerp(flowOffset, tex2D(TERRAIN_FlowingMap, flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.1).ag*2-1, ft);
			flowOffset*=TERRAIN_Flow*max(p, TERRAIN_WetSpecularity)*TERRAIN_LayerWetStrength;
		#else
			float2 flowOffset=0;
			float2 flowSpeed=0;
		#endif
		
		#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
			float2 rippleUV = IN.uv_MainTex*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
		  	roff = GetRipple( rippleUV, TERRAIN_RainIntensity);
			roff += GetRipple( rippleUV+float2(0.25,0.25), TERRAIN_RainIntensity);
		  	roff*=4*_RippleDamp*lerp(TERRAIN_WetDropletsStrength, 1, p);
		  	roff+=flowOffset;
		#else
			roff = flowOffset;
		#endif
		
		#if !defined(RTP_SIMPLE_SHADING)
			rayPos.xy+=TERRAIN_Refraction*roff*max(p, TERRAIN_WetRefraction);
		#endif
		
      	fixed4 col = tex2D (_MainTex, rayPos.xy);
        o.Gloss = col.a;
        float GlossDry=o.Gloss;
        
        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
        o.Specular = lerp(_Shininess, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
        
        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
	        half rim = saturate(1.0+TERRAIN_FresnelOffset - dot (normalize(IN.viewDir), float3(roff,1) ));
	        rim*=saturate(pow (rim, TERRAIN_FresnelPow));
        #endif

        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
        float _WaterOpacity=TERRAIN_WaterOpacity*p;

        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
        
        o.Normal = lerp(UnpackNormal (tex2D (_BumpMap, rayPos.xy)), float3(0,0,1), max(p*0.7, TERRAIN_WaterOpacity));
        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
        o.Normal.xy+=roff;
        o.Normal=normalize(o.Normal);
  		
		col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*0.3;
                
        o.Albedo = col.rgb;
  		
	    #if defined(RTP_REFLECTION)
			float t=tex2D(_ReflectionMap, IN._auxDir.xy + o.Normal.xy*TERRAIN_ReflDistortion).a;
			#ifdef RTP_WETNESS
				float ReflectionStrength=max(TERRAIN_LayerReflection, TERRAIN_WetReflection*TERRAIN_LayerWetStrength);
			#else
				float ReflectionStrength=TERRAIN_LayerReflection;
			#endif
			rim*=max(p*saturate(TERRAIN_WaterGloss+0.3), lerp(GlossDry*saturate(ReflectionStrength), saturate(GlossDry+ReflectionStrength-1), saturate(ReflectionStrength-1)) );
			
			o.Emission = TERRAIN_ReflColorA.rgb*rim*t; // *TERRAIN_ReflColorA.a
			o.Albedo = lerp(o.Albedo, TERRAIN_ReflColorB.rgb, TERRAIN_ReflColorB.a*rim*(1-t));
		#endif    
	}
	ENDCG
      
    } 
    Fallback "Diffuse"
}
