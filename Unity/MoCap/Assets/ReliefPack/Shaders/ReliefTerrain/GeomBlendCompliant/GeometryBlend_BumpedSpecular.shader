//
// Relief Terrain Geometry Blend shader
// Tomasz Stobierski 2013
//
Shader "Relief Pack/GeometryBlend_BumpedSpecular" {
Properties {
		//
		// keep in mind that not all of properties are used, depending on shader configuration in defines section below
		//
		_Color ("Color (RGBA)", Color) = (1,1,1,1)
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0, 100)) = 80
		_MainTex ("Main texture (RGBA)", 2D) = "black" {}
		_BumpMap ("Normal map", 2D) = "bump" {}
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
#pragma surface surf CustomBlinnPhong fullforwardshadows decal:blend
#pragma target 3.0
#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _BumpMap;
float4 _Color;
float _Shininess;


struct Input {
	float2 uv_MainTex;
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
	
void surf (Input IN, inout SurfaceOutput o) {
	float4 tex = tex2D(_MainTex, IN.uv_MainTex.xy);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex.xy));
	
	// we need to blend it this way so that point lights work fine with shader
	o.Alpha=1-IN.color.a;
}

ENDCG
}

// Fallback to Diffuse
Fallback "Diffuse"
}
