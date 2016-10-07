using System;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ReliefTerrainGlobalSettingsHolder {
	
	public bool useTerrainMaterial=false;
	public int numTiles=0;
	
	public int numLayers;
	
	public Texture2D[] splats;
	public Texture2D[] splat_atlases=new Texture2D[2];
	public string save_path_atlasA="";
	public string save_path_atlasB="";
	public string save_path_terrain_steepness="";
	public string save_path_terrain_height="";
	public string save_path_terrain_direction="";
	public string save_path_Bump01="";
	public string save_path_Bump23="";
	public string save_path_Bump45="";
	public string save_path_Bump67="";
	public string save_path_Bump89="";
	public string save_path_BumpAB="";
	public string save_path_HeightMap="";
	public string save_path_HeightMap2="";
	public string save_path_HeightMap3="";
	public string save_path_SSColorCombined="";
		
	public string newPresetName="a preset name...";
	
	public Texture2D activateObject;
	private GameObject _RTP_LODmanager;
	
	public RTP_LODmanager _RTP_LODmanagerScript;
	
	public bool super_simple_active=false;
	public float RTP_MIP_BIAS=0;
	public Color _SpecColor;
	
	public ColorChannels SuperDetailA_channel;
	public ColorChannels SuperDetailB_channel;
	
	public Texture2D Bump01;
	public Texture2D Bump23;
	public Texture2D Bump45;
	public Texture2D Bump67;
	public Texture2D Bump89;
	public Texture2D BumpAB;
	public Texture2D BumpGlobal;
	public int BumpGlobalCombinedSize=1024;
	
	public Texture2D SSColorCombined;
	
	public Texture2D VerticalTexture;
	public float BumpMapGlobalScale;
	public Vector3 GlobalColorMapBlendValues;
	public float _GlobalColorMapNearMIP;
	public float GlobalColorMapSaturation;
	public float GlobalColorMapBrightness;
	public float _FarNormalDamp;

	public float blendMultiplier;
	
	public float terrainTileSizeX;
	public float terrainTileSizeZ;
	
	public Texture2D HeightMap;
	public Vector4 ReliefTransform;
	public float DIST_STEPS;
	public float WAVELENGTH;
	public float ReliefBorderBlend;

	public float ExtrudeHeight;
	public float LightmapShading;

	public float RTP_AOsharpness;
	public float RTP_AOamp;
	public bool colorSpaceLinear;
	
	public float SHADOW_STEPS;
	public float WAVELENGTH_SHADOWS;
	public float SHADOW_SMOOTH_STEPS;
	public float SelfShadowStrength;
	public float ShadowSmoothing;
	public Color ShadowColor=Color.grey;
	
	public float distance_start;
	public float distance_transition;
	public float distance_start_bumpglobal;
	public float distance_transition_bumpglobal;
	public float rtp_perlin_start_val;

	public float trees_shadow_distance_start;
	public float trees_shadow_distance_transition;
	public float trees_shadow_value;
	public float trees_pixel_distance_start;
	public float trees_pixel_distance_transition;
	public float trees_pixel_blend_val;
	public float global_normalMap_multiplier;
	
	public Texture2D HeightMap2;
	public Texture2D HeightMap3;
	public int rtp_mipoffset_globalnorm;
	public float _SuperDetailTiling;
	public Texture2D SuperDetailA;
	public Texture2D SuperDetailB;
	
// reflection
	public Texture2D TERRAIN_ReflectionMap;
	public ColorChannels TERRAIN_ReflectionMap_channel;
	public Color TERRAIN_ReflColorA;
	public Color TERRAIN_ReflColorB;
	public float TERRAIN_ReflDistortion;
	public float TERRAIN_ReflectionRotSpeed;
	public float TERRAIN_FresnelPow;
	public float TERRAIN_FresnelOffset;
	
// water/wet
	public float TERRAIN_GlobalWetness;
	
	public float TERRAIN_WaterSpecularity;
		
	public Texture2D TERRAIN_RippleMap;
	public float TERRAIN_RippleScale;
	public float TERRAIN_FlowScale;
	public float TERRAIN_FlowSpeed;
	public float TERRAIN_FlowMipOffset;
	public float TERRAIN_WetDarkening;
	public float TERRAIN_WetDropletsStrength;

	public float TERRAIN_RainIntensity;
	public float TERRAIN_DropletsSpeed;
	
	public float TERRAIN_mipoffset_flowSpeed;

// caustics
	public float TERRAIN_CausticsAnimSpeed;
	public Color TERRAIN_CausticsColor;
	public GameObject TERRAIN_CausticsWaterLevelRefObject;
	public float TERRAIN_CausticsWaterLevel;
	public float TERRAIN_CausticsWaterLevelByAngle;
	public float TERRAIN_CausticsWaterDeepFadeLength;
	public float TERRAIN_CausticsWaterShallowFadeLength;
	public float TERRAIN_CausticsTilingScale;
	public Texture2D TERRAIN_CausticsTex;
	
	public Color rtp_customAmbientCorrection;
	
	public Vector4 RTP_LightDefVector;
	public Color RTP_ReflexLightDiffuseColor;
	public Color RTP_ReflexLightSpecColor;
	
	
//////////////////////
// layer_dependent arrays
//////////////////////
	public Texture2D[] Bumps;
	public float[] Spec;
	public float[] FarGlossCorrection;
	public float[] MIPmult;
	public float[] MixScale;
	public float[] MixBlend;
	public float[] MixSaturation;
	public float[] GlobalColorPerLayer;
	public float[] PER_LAYER_HEIGHT_MODIFIER;
	public float[] _SuperDetailStrengthMultA;
	public float[] _SuperDetailStrengthMultASelfMaskNear;
	public float[] _SuperDetailStrengthMultASelfMaskFar;
	public float[] _SuperDetailStrengthMultB;
	public float[] _SuperDetailStrengthMultBSelfMaskNear;
	public float[] _SuperDetailStrengthMultBSelfMaskFar;
	public float[] _SuperDetailStrengthNormal;
	public float[] _BumpMapGlobalStrength;
	
	public float[] AO_strength=new float[12]{1,1,1,1,1,1,1,1,1,1,1,1};
	
	public float[] VerticalTextureStrength;
	public float VerticalTextureGlobalBumpInfluence;
	public float VerticalTextureTiling;
	
	public Texture2D[] Heights;
	
	public float[] _snow_strength_per_layer;
	public ProceduralMaterial[] Substances;
	
// wet
	public float[] TERRAIN_LayerWetStrength;
	public float[] TERRAIN_WaterLevel;
	public float[] TERRAIN_WaterLevelSlopeDamp;
	public float[] TERRAIN_WaterEdge;
	public float[] TERRAIN_WaterGloss;
	public float[] TERRAIN_WaterOpacity;
	public float[] TERRAIN_Refraction;
	public float[] TERRAIN_WetRefraction;
	public float[] TERRAIN_Flow;
	public float[] TERRAIN_WetSpecularity;
	public float[] TERRAIN_WetReflection;
	public Color[] TERRAIN_WaterColor;
	public float[] TERRAIN_LayerReflection;
////////////////////
	public float _snow_strength;
	public float _global_color_brightness_to_snow;
	public float _snow_slope_factor;
	public float _snow_edge_definition;
	public float _snow_height_treshold;
	public float _snow_height_transition;
	public Color _snow_color;
	public float _snow_specular;
	public float _snow_gloss;
	public float _snow_reflectivness;
	public float _snow_deep_factor;
	
	public bool _4LAYERS_SHADER_USED=false;
	
	public bool flat_dir_ref=true;
	public bool flip_dir_ref=true;
	public GameObject direction_object;
	
	public bool show_details=false;
	public bool show_details_main=false;
	public bool show_details_atlasing=false;
	public bool show_details_layers=false;
	public bool show_details_uv_blend=false;
	
	public bool show_controlmaps=false;
	public bool show_controlmaps_build=false;
	public bool show_controlmaps_helpers=false;
	public bool show_controlmaps_highcost=false;
	public bool show_controlmaps_splats=false;
	
	public bool show_vert_texture=false;
	
	public bool show_global_color=false;
	
	public bool show_snow=false;
	
	public bool show_global_bump=false;
	public bool show_global_bump_normals=false;
	public bool show_global_bump_superdetail=false;
	
	public ReliefTerrainMenuItems submenu=(ReliefTerrainMenuItems)(0);
	public ReliefTerrainSettingsItems submenu_settings=(ReliefTerrainSettingsItems)(0);
	public ReliefTerrainDerivedTexturesItems submenu_derived_textures=(ReliefTerrainDerivedTexturesItems)(0);
	public ReliefTerrainControlTexturesItems submenu_control_textures=(ReliefTerrainControlTexturesItems)(0);
	public bool show_global_wet_settings=false;
	public bool show_global_reflection_settings=false;
	public int show_active_layer=0;
	
	public bool show_derivedmaps=false;
	
	public bool show_settings=false;
	
	// paint
	public bool undo_flag=false;
	public bool paint_flag=false;
	public float paint_size=0.5f;
	public float paint_smoothness=0;
	public float paint_opacity=1;
	public Color paintColor=new Color(0.5f,0.3f,0,0);
	public bool preserveBrightness=true;
	public bool paint_alpha_flag=false;
	public bool paint_wetmask=false;
	//private Transform underlying_transform;
	//private MeshRenderer underlying_renderer;
	public RaycastHit paintHitInfo;
	public bool paintHitInfo_flag;
	
	private Texture2D dumb_tex;
	
	public Color[] paintColorSwatches;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// constructor - init arrays
	//
	public ReliefTerrainGlobalSettingsHolder() {
		const int cnt=12;
		
		Bumps=new Texture2D[cnt];
		Heights=new Texture2D[cnt];
		
		Spec=new float[cnt];
		FarGlossCorrection=new float[cnt];
		MIPmult=new float[cnt];			
		MixScale=new float[cnt];
		MixBlend=new float[cnt];
		MixSaturation=new float[cnt];
		GlobalColorPerLayer=new float[cnt];
		
		PER_LAYER_HEIGHT_MODIFIER=new float[cnt];
		_snow_strength_per_layer=new float[cnt];

		Substances=new ProceduralMaterial[cnt];
		
		_SuperDetailStrengthMultA=new float[cnt];
		_SuperDetailStrengthMultASelfMaskNear=new float[cnt];
		_SuperDetailStrengthMultASelfMaskFar=new float[cnt];
		_SuperDetailStrengthMultB=new float[cnt];
		_SuperDetailStrengthMultBSelfMaskNear=new float[cnt];
		_SuperDetailStrengthMultBSelfMaskFar=new float[cnt];
		_SuperDetailStrengthNormal=new float[cnt];
		_BumpMapGlobalStrength=new float[cnt];
		
		AO_strength=new float[cnt];
		VerticalTextureStrength=new float[cnt];
		
		TERRAIN_LayerWetStrength=new float[cnt];
		TERRAIN_WaterLevel=new float[cnt];
		TERRAIN_WaterLevelSlopeDamp=new float[cnt];
		TERRAIN_WaterEdge=new float[cnt];
		TERRAIN_WaterGloss=new float[cnt];
		TERRAIN_WaterOpacity=new float[cnt];
		TERRAIN_Refraction=new float[cnt];
		TERRAIN_WetRefraction=new float[cnt];
		TERRAIN_Flow=new float[cnt];
		TERRAIN_WetSpecularity=new float[cnt];
		TERRAIN_WetReflection=new float[cnt];
		TERRAIN_LayerReflection=new float[cnt];
		TERRAIN_WaterColor=new Color[cnt];
#if UNITY_EDITOR
		ReturnToDefaults();
#endif
	}	
	
	public void ReInit(Terrain terrainComp) {
		if (terrainComp.terrainData.splatPrototypes.Length>numLayers) {
			Texture2D[] splats_new=new Texture2D[terrainComp.terrainData.splatPrototypes.Length];
			for(int i=0; i<splats.Length; i++) splats_new[i]=splats[i];
			splats=splats_new;
			splats[terrainComp.terrainData.splatPrototypes.Length-1]=terrainComp.terrainData.splatPrototypes[((terrainComp.terrainData.splatPrototypes.Length-2) >=0) ? (terrainComp.terrainData.splatPrototypes.Length-2) : 0].texture;
		} else if (terrainComp.terrainData.splatPrototypes.Length<numLayers) {
			Texture2D[] splats_new=new Texture2D[terrainComp.terrainData.splatPrototypes.Length];
			for(int i=0; i<splats_new.Length; i++) splats_new[i]=splats[i];
			splats=splats_new;
		}
		numLayers=terrainComp.terrainData.splatPrototypes.Length;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public Material use_mat=null;
	public void SetShaderParam(string name, Texture2D tex) {
		if (!tex) return;
		if (use_mat) {
			use_mat.SetTexture(name, tex);
		} else {
			Shader.SetGlobalTexture(name, tex);
		}
	}
	public void SetShaderParam(string name, Matrix4x4 mtx) {
		if (use_mat) {
			use_mat.SetMatrix(name, mtx);
		} else {
			Shader.SetGlobalMatrix(name, mtx);
		}
	}	
	public void SetShaderParam(string name, Vector4 vec) {
		if (use_mat) {
			use_mat.SetVector(name, vec);
		} else {
			Shader.SetGlobalVector(name, vec);
		}
	}
	public void SetShaderParam(string name, float val) {
		if (use_mat) {
			use_mat.SetFloat(name, val);
		} else {
			Shader.SetGlobalFloat(name, val);
		}
	}
	public void SetShaderParam(string name, Color col) {
		if (use_mat) {
			use_mat.SetColor(name, col);
		} else {
			Shader.SetGlobalColor(name, col);
		}
	}
	
	public RTP_LODmanager Get_RTP_LODmanagerScript() {
		return _RTP_LODmanagerScript;
	}	
	
	public void RefreshAll() {
		Refresh();
		ReliefTerrain[] rts=GameObject.FindObjectsOfType(typeof(ReliefTerrain)) as ReliefTerrain[];
		for(int i=0; i<rts.Length; i++) rts[i].RefreshTextures();
	}
	
	public void Refresh(Material mat=null) {
		if (splats==null) return;
#if UNITY_EDITOR
		if (_RTP_LODmanager==null) {
			if ((_RTP_LODmanager=GameObject.Find("_RTP_LODmanager"))==null) {
				_RTP_LODmanager=new GameObject("_RTP_LODmanager");
				_RTP_LODmanager.AddComponent(typeof(RTP_LODmanager));
				_RTP_LODmanagerScript=(RTP_LODmanager)_RTP_LODmanager.GetComponent(typeof(RTP_LODmanager));
				EditorUtility.DisplayDialog("RTP Notification", "_RTP_LODmanager object added to the scene.\nIts script handles LOD properties of RTP shaders.","OK");
				Selection.activeObject=_RTP_LODmanager;
			}
		}
		if (_RTP_LODmanagerScript==null) {
			_RTP_LODmanagerScript=(RTP_LODmanager)_RTP_LODmanager.GetComponent(typeof(RTP_LODmanager));
		}
		_4LAYERS_SHADER_USED=_RTP_LODmanagerScript.RTP_4LAYERS_MODE;
		
		colorSpaceLinear = ( PlayerSettings.colorSpace==ColorSpace.Linear );
#endif
		// switch for SetShaderParam - when use_mat defined we're injecting param into material
		use_mat=mat;
		
		for(int i=0; i<numLayers; i++) {
			if (i<4) {
				SetShaderParam("_SplatA"+i, splats[i]);
			} else if (i<8) {
				if (_4LAYERS_SHADER_USED) {
					SetShaderParam("_SplatC"+(i-4), splats[i]);
					// potrzebne przy sniegu (firstpass moze korzystac z koloru i bumpmap 4-7)
					SetShaderParam("_SplatB"+(i-4), splats[i]);
				} else {
					SetShaderParam("_SplatB"+(i-4), splats[i]);
				}
			} else if (i<12) {
				SetShaderParam("_SplatC"+(i-8), splats[i]);
			} 
		}
		
		/////////////////////////////////////////////////////////////////////
		//
		// layer independent
		//
		/////////////////////////////////////////////////////////////////////
		
		// custom fog (exp2, unity's fog doesn't work with this shader - too many texture interpolators)
		if (RenderSettings.fog) {
			Shader.SetGlobalFloat("_Fdensity", Mathf.Log(1-RenderSettings.fogDensity)/Mathf.Log(2));
			if (colorSpaceLinear) {
				Shader.SetGlobalColor("_FColor", RenderSettings.fogColor.linear);
			} else {
				Shader.SetGlobalColor("_FColor", RenderSettings.fogColor);
			}
		} else {
			Shader.SetGlobalFloat("_Fdensity", 0);
		}
		
		Shader.SetGlobalFloat("RTP_AOamp", RTP_AOamp);
		Shader.SetGlobalFloat("RTP_AOsharpness", RTP_AOsharpness);
		
		// global
		SetShaderParam("_VerticalTexture", VerticalTexture);
		
		SetShaderParam("_GlobalColorMapBlendValues", GlobalColorMapBlendValues);
		SetShaderParam("_GlobalColorMapSaturation", GlobalColorMapSaturation);
		SetShaderParam("_GlobalColorMapBrightness", GlobalColorMapBrightness);
		SetShaderParam("_GlobalColorMapNearMIP", _GlobalColorMapNearMIP);
		
		SetShaderParam("_RTP_MIP_BIAS", RTP_MIP_BIAS);

		SetShaderParam("_BumpMapGlobalScale", BumpMapGlobalScale);
		SetShaderParam("_FarNormalDamp", _FarNormalDamp);
		
		SetShaderParam("_SpecColor", _SpecColor);
		
		SetShaderParam("_blend_multiplier", blendMultiplier);
		
		SetShaderParam("_TERRAIN_ReliefTransform", ReliefTransform);
		SetShaderParam("_TERRAIN_ReliefTransformTriplanarZ", terrainTileSizeX/ReliefTransform.x);
		SetShaderParam("_TERRAIN_DIST_STEPS", DIST_STEPS);
		SetShaderParam("_TERRAIN_WAVELENGTH", WAVELENGTH);
		
		SetShaderParam("_TERRAIN_ExtrudeHeight", ExtrudeHeight);
		SetShaderParam("_TERRAIN_LightmapShading", LightmapShading);
		
		SetShaderParam("_TERRAIN_SHADOW_STEPS", SHADOW_STEPS);
		SetShaderParam("_TERRAIN_WAVELENGTH_SHADOWS", WAVELENGTH_SHADOWS);
		SetShaderParam("_TERRAIN_SHADOW_SMOOTH_STEPS", SHADOW_SMOOTH_STEPS);
		
		SetShaderParam("_TERRAIN_SelfShadowStrength", SelfShadowStrength);
		SetShaderParam("_TERRAIN_ShadowSmoothing", ShadowSmoothing);
		SetShaderParam("_TERRAIN_ShadowColorization", ShadowColor);
		
		SetShaderParam("_TERRAIN_distance_start", distance_start);
		SetShaderParam("_TERRAIN_distance_transition", distance_transition);
		
		SetShaderParam("_TERRAIN_distance_start_bumpglobal", distance_start_bumpglobal);
		SetShaderParam("_TERRAIN_distance_transition_bumpglobal", distance_transition_bumpglobal);
		SetShaderParam("rtp_perlin_start_val", rtp_perlin_start_val);
		
		SetShaderParam("_TERRAIN_trees_shadow_values", new Vector4(trees_shadow_distance_start, trees_shadow_distance_transition, trees_shadow_value, global_normalMap_multiplier));
		SetShaderParam("_TERRAIN_trees_pixel_values", new Vector4(trees_pixel_distance_start, trees_pixel_distance_transition, trees_pixel_blend_val, 0));
		
		SetShaderParam("_SuperDetailTiling", _SuperDetailTiling);
		
		SetShaderParam("rtp_snow_strength", _snow_strength);
		SetShaderParam("rtp_global_color_brightness_to_snow", _global_color_brightness_to_snow);
		SetShaderParam("rtp_snow_slope_factor", _snow_slope_factor);
		SetShaderParam("rtp_snow_edge_definition", _snow_edge_definition);
		SetShaderParam("rtp_snow_height_treshold", _snow_height_treshold);
		SetShaderParam("rtp_snow_height_transition", _snow_height_transition);
		SetShaderParam("rtp_snow_color", _snow_color);
		SetShaderParam("rtp_snow_specular", _snow_specular);
		SetShaderParam("rtp_snow_gloss", _snow_gloss);
		SetShaderParam("rtp_snow_reflectivness", _snow_reflectivness);
		SetShaderParam("rtp_snow_deep_factor", _snow_deep_factor);
		
		// caustics
		SetShaderParam("TERRAIN_CausticsAnimSpeed", TERRAIN_CausticsAnimSpeed);
		SetShaderParam("TERRAIN_CausticsColor", TERRAIN_CausticsColor);
		if (TERRAIN_CausticsWaterLevelRefObject) TERRAIN_CausticsWaterLevel=TERRAIN_CausticsWaterLevelRefObject.transform.position.y;
		SetShaderParam("TERRAIN_CausticsWaterLevel", TERRAIN_CausticsWaterLevel);
		SetShaderParam("TERRAIN_CausticsWaterLevelByAngle", TERRAIN_CausticsWaterLevelByAngle);
		SetShaderParam("TERRAIN_CausticsWaterDeepFadeLength", TERRAIN_CausticsWaterDeepFadeLength);
		SetShaderParam("TERRAIN_CausticsWaterShallowFadeLength", TERRAIN_CausticsWaterShallowFadeLength);
		SetShaderParam("TERRAIN_CausticsTilingScale", TERRAIN_CausticsTilingScale);
		SetShaderParam("TERRAIN_CausticsTex", TERRAIN_CausticsTex);
		
		if (numLayers>0) {
			int tex_width=512;
			for(int i=0; i<numLayers; i++) {
				if (splats[i]) {
					tex_width=splats[i].width;
					break;
				}
			}
			SetShaderParam("rtp_mipoffset_color", -Mathf.Log(1024.0f/tex_width)/Mathf.Log(2) );
			if (Bump01!=null) {
				tex_width=Bump01.width;
			}
			SetShaderParam("rtp_mipoffset_bump", -Mathf.Log(1024.0f/tex_width)/Mathf.Log(2));
			if (HeightMap) {
				tex_width=HeightMap.width;
			} else if (HeightMap2) {
				tex_width=HeightMap2.width; 
			} else if (HeightMap3) {
				tex_width=HeightMap3.width;
			}
			SetShaderParam("rtp_mipoffset_height", -Mathf.Log(1024.0f/tex_width)/Mathf.Log(2));
			
			tex_width=BumpGlobalCombinedSize;
			SetShaderParam("rtp_mipoffset_globalnorm", -Mathf.Log(1024.0f/(tex_width*BumpMapGlobalScale))/Mathf.Log(2)+rtp_mipoffset_globalnorm);
			SetShaderParam("rtp_mipoffset_superdetail", -Mathf.Log(1024.0f/(tex_width*_SuperDetailTiling))/Mathf.Log(2));
			SetShaderParam("rtp_mipoffset_flow", -Mathf.Log(1024.0f/(tex_width*TERRAIN_FlowScale))/Mathf.Log(2) + TERRAIN_FlowMipOffset);
			if (TERRAIN_RippleMap) {
				tex_width=TERRAIN_RippleMap.width;
			}
			SetShaderParam("rtp_mipoffset_ripple", -Mathf.Log(1024.0f/(tex_width*TERRAIN_RippleScale))/Mathf.Log(2));
			if (TERRAIN_CausticsTex) {
				tex_width=TERRAIN_CausticsTex.width;
			}
			SetShaderParam("rtp_mipoffset_caustics", -Mathf.Log(1024.0f/(tex_width*TERRAIN_CausticsTilingScale))/Mathf.Log(2));
		}
		
		SetShaderParam("TERRAIN_ReflectionMap", TERRAIN_ReflectionMap);
		SetShaderParam("TERRAIN_ReflColorA", TERRAIN_ReflColorA);
		SetShaderParam("TERRAIN_ReflColorB", TERRAIN_ReflColorB);
		SetShaderParam("TERRAIN_ReflDistortion", TERRAIN_ReflDistortion);
		SetShaderParam("TERRAIN_ReflectionRotSpeed", TERRAIN_ReflectionRotSpeed);
		SetShaderParam("TERRAIN_FresnelPow", TERRAIN_FresnelPow);
		SetShaderParam("TERRAIN_FresnelOffset", TERRAIN_FresnelOffset);
		
		SetShaderParam("TERRAIN_GlobalWetness", TERRAIN_GlobalWetness);
		SetShaderParam("TERRAIN_WaterSpecularity", TERRAIN_WaterSpecularity);
		SetShaderParam("TERRAIN_RippleMap", TERRAIN_RippleMap);
		SetShaderParam("TERRAIN_RippleScale", TERRAIN_RippleScale);
		SetShaderParam("TERRAIN_FlowScale",  TERRAIN_FlowScale);
		SetShaderParam("TERRAIN_FlowMipOffset", TERRAIN_FlowMipOffset);
		SetShaderParam("TERRAIN_FlowSpeed", TERRAIN_FlowSpeed);
		SetShaderParam("TERRAIN_RainIntensity", TERRAIN_RainIntensity);
		SetShaderParam("TERRAIN_DropletsSpeed", TERRAIN_DropletsSpeed);
		SetShaderParam("TERRAIN_WetDropletsStrength", TERRAIN_WetDropletsStrength);
		SetShaderParam("TERRAIN_WetDarkening", TERRAIN_WetDarkening);
		SetShaderParam("TERRAIN_mipoffset_flowSpeed", TERRAIN_mipoffset_flowSpeed);
		
		Shader.SetGlobalVector("rtp_customAmbientCorrection", new Vector4(rtp_customAmbientCorrection.r-0.2f, rtp_customAmbientCorrection.g-0.2f, rtp_customAmbientCorrection.b-0.2f, 0)*0.1f);
		
		Shader.SetGlobalVector("RTP_LightDefVector", RTP_LightDefVector);
		Shader.SetGlobalColor("RTP_ReflexLightDiffuseColor", RTP_ReflexLightDiffuseColor);
		Shader.SetGlobalColor("RTP_ReflexLightSpecColor", RTP_ReflexLightSpecColor);
		
		SetShaderParam("_VerticalTextureGlobalBumpInfluence", VerticalTextureGlobalBumpInfluence);
		SetShaderParam("_VerticalTextureTiling", VerticalTextureTiling);

		/////////////////////////////////////////////////////////////////////
		//
		// layer dependent numeric
		//
		/////////////////////////////////////////////////////////////////////
		SetShaderParam("_Spec0123", getVector(Spec, 0,3));
		SetShaderParam("_FarGlossCorrection0123", getVector(FarGlossCorrection, 0,3));
		SetShaderParam("_MIPmult0123", getVector(MIPmult, 0,3));
		SetShaderParam("_MixScale0123", getVector(MixScale, 0,3));
		SetShaderParam("_MixBlend0123", getVector(MixBlend, 0,3));
		SetShaderParam("_MixSaturation0123", getVector(MixSaturation, 0, 3));
		SetShaderParam("_GlobalColorPerLayer0123", getVector(GlobalColorPerLayer, 0, 3));
			
		SetShaderParam("PER_LAYER_HEIGHT_MODIFIER0123",  getVector(PER_LAYER_HEIGHT_MODIFIER, 0,3));
			
		SetShaderParam("rtp_snow_strength_per_layer0123",  getVector(_snow_strength_per_layer, 0,3));

		SetShaderParam("_SuperDetailStrengthMultA0123", getVector(_SuperDetailStrengthMultA, 0,3));
		SetShaderParam("_SuperDetailStrengthMultB0123", getVector(_SuperDetailStrengthMultB, 0,3));
		SetShaderParam("_SuperDetailStrengthNormal0123", getVector(_SuperDetailStrengthNormal, 0,3));
		SetShaderParam("_BumpMapGlobalStrength0123", getVector(_BumpMapGlobalStrength, 0,3));
			
		SetShaderParam("_SuperDetailStrengthMultASelfMaskNear0123", getVector(_SuperDetailStrengthMultASelfMaskNear, 0,3));
		SetShaderParam("_SuperDetailStrengthMultASelfMaskFar0123", getVector(_SuperDetailStrengthMultASelfMaskFar, 0,3));
		SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear0123", getVector(_SuperDetailStrengthMultBSelfMaskNear, 0,3));
		SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar0123", getVector(_SuperDetailStrengthMultBSelfMaskFar, 0,3));
		
		SetShaderParam("TERRAIN_LayerWetStrength0123", getVector(TERRAIN_LayerWetStrength, 0,3));
		SetShaderParam("TERRAIN_WaterLevel0123", getVector(TERRAIN_WaterLevel, 0,3));
		SetShaderParam("TERRAIN_WaterLevelSlopeDamp0123", getVector(TERRAIN_WaterLevelSlopeDamp, 0,3));
		SetShaderParam("TERRAIN_WaterEdge0123", getVector(TERRAIN_WaterEdge, 0,3));
		SetShaderParam("TERRAIN_WaterGloss0123", getVector(TERRAIN_WaterGloss, 0,3));
		SetShaderParam("TERRAIN_WaterOpacity0123", getVector(TERRAIN_WaterOpacity, 0,3));
		SetShaderParam("TERRAIN_Refraction0123", getVector(TERRAIN_Refraction, 0,3));
		SetShaderParam("TERRAIN_WetRefraction0123", getVector(TERRAIN_WetRefraction, 0,3));
		SetShaderParam("TERRAIN_Flow0123", getVector(TERRAIN_Flow, 0,3));
		SetShaderParam("TERRAIN_WetSpecularity0123", getVector(TERRAIN_WetSpecularity, 0,3));
		SetShaderParam("TERRAIN_WetReflection0123", getVector(TERRAIN_WetReflection, 0,3));
		SetShaderParam("TERRAIN_WaterColorR0123", getColorVector(TERRAIN_WaterColor, 0,3, 0));
		SetShaderParam("TERRAIN_WaterColorG0123", getColorVector(TERRAIN_WaterColor, 0,3, 1));
		SetShaderParam("TERRAIN_WaterColorB0123", getColorVector(TERRAIN_WaterColor, 0,3, 2));
		SetShaderParam("TERRAIN_WaterColorA0123", getColorVector(TERRAIN_WaterColor, 0,3, 3));
		SetShaderParam("TERRAIN_LayerReflection0123", getVector(	TERRAIN_LayerReflection, 0,3));
		
		SetShaderParam("RTP_AO_0123", getVector(AO_strength, 0,3));
		SetShaderParam("_VerticalTexture0123", getVector(VerticalTextureStrength, 0,3));

		if ((numLayers>4) && _4LAYERS_SHADER_USED) {
			//
			// przekieruj parametry warstw 4-7 na AddPass
			//
			SetShaderParam("_Spec89AB", getVector(Spec, 4,7));
			SetShaderParam("_FarGlossCorrection89AB", getVector(FarGlossCorrection, 4,7));
			SetShaderParam("_MIPmult89AB", getVector(MIPmult, 4,7));
			SetShaderParam("_MixScale89AB", getVector(MixScale, 4,7));
			SetShaderParam("_MixBlend89AB", getVector(MixBlend, 4,7));
			SetShaderParam("_MixSaturation89AB", getVector(MixSaturation, 4, 7));
			SetShaderParam("_GlobalColorPerLayer89AB", getVector(GlobalColorPerLayer, 4, 7));
			
			SetShaderParam("PER_LAYER_HEIGHT_MODIFIER89AB", getVector(PER_LAYER_HEIGHT_MODIFIER, 4,7));
			
			SetShaderParam("rtp_snow_strength_per_layer89AB",  getVector(_snow_strength_per_layer, 4,7));
			
			SetShaderParam("_SuperDetailStrengthMultA89AB", getVector(_SuperDetailStrengthMultA, 4,7));
			SetShaderParam("_SuperDetailStrengthMultB89AB", getVector(_SuperDetailStrengthMultB, 4,7));
			SetShaderParam("_SuperDetailStrengthNormal89AB", getVector(_SuperDetailStrengthNormal, 4,7));
			SetShaderParam("_BumpMapGlobalStrength89AB", getVector(_BumpMapGlobalStrength, 4,7));
			
			SetShaderParam("_SuperDetailStrengthMultASelfMaskNear89AB", getVector(_SuperDetailStrengthMultASelfMaskNear, 4,7));
			SetShaderParam("_SuperDetailStrengthMultASelfMaskFar89AB", getVector(_SuperDetailStrengthMultASelfMaskFar, 4,7));
			SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear89AB", getVector(_SuperDetailStrengthMultBSelfMaskNear, 4,7));
			SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar89AB", getVector(_SuperDetailStrengthMultBSelfMaskFar, 4,7));
			
			SetShaderParam("TERRAIN_LayerWetStrength89AB", getVector(TERRAIN_LayerWetStrength, 4,7));
			SetShaderParam("TERRAIN_WaterLevel89AB", getVector(TERRAIN_WaterLevel, 4,7));
			SetShaderParam("TERRAIN_WaterLevelSlopeDamp89AB", getVector(TERRAIN_WaterLevelSlopeDamp, 4,7));
			SetShaderParam("TERRAIN_WaterEdge89AB", getVector(TERRAIN_WaterEdge, 4,7));
			SetShaderParam("TERRAIN_WaterGloss89AB", getVector(TERRAIN_WaterGloss, 4,7));
			SetShaderParam("TERRAIN_WaterOpacity89AB", getVector(TERRAIN_WaterOpacity, 4,7));
			SetShaderParam("TERRAIN_Refraction89AB", getVector(TERRAIN_Refraction, 4,7));
			SetShaderParam("TERRAIN_WetRefraction89AB", getVector(TERRAIN_WetRefraction, 4,7));
			SetShaderParam("TERRAIN_Flow89AB", getVector(TERRAIN_Flow, 4,7));
			SetShaderParam("TERRAIN_WetSpecularity89AB", getVector(TERRAIN_WetSpecularity, 4,7));
			SetShaderParam("TERRAIN_WetReflection89AB", getVector(TERRAIN_WetReflection, 4,7));
			SetShaderParam("TERRAIN_WaterColorR89AB", getColorVector(TERRAIN_WaterColor, 4,7, 0));
			SetShaderParam("TERRAIN_WaterColorG89AB", getColorVector(TERRAIN_WaterColor, 4,7, 1));
			SetShaderParam("TERRAIN_WaterColorB89AB", getColorVector(TERRAIN_WaterColor, 4,7, 2));
			SetShaderParam("TERRAIN_WaterColorA89AB", getColorVector(TERRAIN_WaterColor, 4,7, 3));
			SetShaderParam("TERRAIN_LayerReflection89AB", getVector(TERRAIN_LayerReflection, 4,7));
			
			SetShaderParam("RTP_AO_89AB", getVector(AO_strength, 4,7));
			SetShaderParam("_VerticalTexture89AB", getVector(VerticalTextureStrength, 4,7));
		} else {
			SetShaderParam("_Spec4567", getVector(Spec, 4,7));
			SetShaderParam("_FarGlossCorrection4567", getVector(FarGlossCorrection, 4,7));
			SetShaderParam("_MIPmult4567", getVector(MIPmult, 4,7));
			SetShaderParam("_MixScale4567", getVector(MixScale, 4,7));
			SetShaderParam("_MixBlend4567", getVector(MixBlend, 4,7));
			SetShaderParam("_MixSaturation4567", getVector(MixSaturation, 4, 7));
			SetShaderParam("_GlobalColorPerLayer4567", getVector(GlobalColorPerLayer, 4, 7));
			
			SetShaderParam("PER_LAYER_HEIGHT_MODIFIER4567", getVector(PER_LAYER_HEIGHT_MODIFIER, 4,7));
			
			SetShaderParam("rtp_snow_strength_per_layer4567",  getVector(_snow_strength_per_layer, 4,7));
			
			SetShaderParam("_SuperDetailStrengthMultA4567", getVector(_SuperDetailStrengthMultA, 4,7));
			SetShaderParam("_SuperDetailStrengthMultB4567", getVector(_SuperDetailStrengthMultB, 4,7));
			SetShaderParam("_SuperDetailStrengthNormal4567", getVector(_SuperDetailStrengthNormal, 4,7));
			SetShaderParam("_BumpMapGlobalStrength4567", getVector(_BumpMapGlobalStrength, 4,7));
			
			SetShaderParam("_SuperDetailStrengthMultASelfMaskNear4567", getVector(_SuperDetailStrengthMultASelfMaskNear, 4,7));
			SetShaderParam("_SuperDetailStrengthMultASelfMaskFar4567", getVector(_SuperDetailStrengthMultASelfMaskFar, 4,7));
			SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear4567", getVector(_SuperDetailStrengthMultBSelfMaskNear, 4,7));
			SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar4567", getVector(_SuperDetailStrengthMultBSelfMaskFar, 4,7));
			
			SetShaderParam("TERRAIN_LayerWetStrength4567", getVector(TERRAIN_LayerWetStrength, 4,7));
			SetShaderParam("TERRAIN_WaterLevel4567", getVector(TERRAIN_WaterLevel, 4,7));
			SetShaderParam("TERRAIN_WaterLevelSlopeDamp4567", getVector(TERRAIN_WaterLevelSlopeDamp, 4,7));
			SetShaderParam("TERRAIN_WaterEdge4567", getVector(TERRAIN_WaterEdge, 4,7));
			SetShaderParam("TERRAIN_WaterGloss4567", getVector(TERRAIN_WaterGloss, 4,7));
			SetShaderParam("TERRAIN_WaterOpacity4567", getVector(TERRAIN_WaterOpacity, 4,7));
			SetShaderParam("TERRAIN_Refraction4567", getVector(TERRAIN_Refraction, 4,7));
			SetShaderParam("TERRAIN_WetRefraction4567", getVector(TERRAIN_WetRefraction, 4,7));
			SetShaderParam("TERRAIN_Flow4567", getVector(TERRAIN_Flow, 4,7));
			SetShaderParam("TERRAIN_WetSpecularity4567", getVector(TERRAIN_WetSpecularity, 4,7));
			SetShaderParam("TERRAIN_WetReflection4567", getVector(TERRAIN_WetReflection, 4,7));
			SetShaderParam("TERRAIN_WaterColorR4567", getColorVector(TERRAIN_WaterColor, 4,7, 0));
			SetShaderParam("TERRAIN_WaterColorG4567", getColorVector(TERRAIN_WaterColor, 4,7, 1));
			SetShaderParam("TERRAIN_WaterColorB4567", getColorVector(TERRAIN_WaterColor, 4,7, 2));
			SetShaderParam("TERRAIN_WaterColorA4567", getColorVector(TERRAIN_WaterColor, 4,7, 3));
			SetShaderParam("TERRAIN_LayerReflection4567", getVector(TERRAIN_LayerReflection, 4,7));
			
			SetShaderParam("RTP_AO_4567", getVector(AO_strength, 4,7));
			SetShaderParam("_VerticalTexture4567", getVector(VerticalTextureStrength, 4,7));
			
			//
			// AddPass
			//
			SetShaderParam("_Spec89AB", getVector(Spec, 8,11));
			SetShaderParam("_FarGlossCorrection89AB", getVector(FarGlossCorrection, 8,11));
			SetShaderParam("_MIPmult89AB", getVector(MIPmult, 8,11));
			SetShaderParam("_MixScale89AB", getVector(MixScale, 8,11));
			SetShaderParam("_MixBlend89AB", getVector(MixBlend, 8,11));
			SetShaderParam("_MixSaturation89AB", getVector(MixSaturation, 8, 11));
			SetShaderParam("_GlobalColorPerLayer89AB", getVector(GlobalColorPerLayer, 8, 11));
			
			SetShaderParam("PER_LAYER_HEIGHT_MODIFIER89AB", getVector(PER_LAYER_HEIGHT_MODIFIER, 8,11));
			
			SetShaderParam("rtp_snow_strength_per_layer89AB",  getVector(_snow_strength_per_layer, 8,11));
			
			SetShaderParam("_SuperDetailStrengthMultA89AB", getVector(_SuperDetailStrengthMultA, 8,11));
			SetShaderParam("_SuperDetailStrengthMultB89AB", getVector(_SuperDetailStrengthMultB, 8,11));
			SetShaderParam("_SuperDetailStrengthNormal89AB", getVector(_SuperDetailStrengthNormal, 8,11));
			SetShaderParam("_BumpMapGlobalStrength89AB", getVector(_BumpMapGlobalStrength, 8,11));
			
			SetShaderParam("_SuperDetailStrengthMultASelfMaskNear89AB", getVector(_SuperDetailStrengthMultASelfMaskNear, 8,11));
			SetShaderParam("_SuperDetailStrengthMultASelfMaskFar89AB", getVector(_SuperDetailStrengthMultASelfMaskFar, 8,11));
			SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear89AB", getVector(_SuperDetailStrengthMultBSelfMaskNear, 8,11));
			SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar89AB", getVector(_SuperDetailStrengthMultBSelfMaskFar, 8,11));
			
			SetShaderParam("TERRAIN_LayerWetStrength89AB", getVector(TERRAIN_LayerWetStrength,  8,11));
			SetShaderParam("TERRAIN_WaterLevel89AB", getVector(TERRAIN_WaterLevel,  8,11));
			SetShaderParam("TERRAIN_WaterLevelSlopeDamp89AB", getVector(TERRAIN_WaterLevelSlopeDamp,  8,11));
			SetShaderParam("TERRAIN_WaterEdge89AB", getVector(TERRAIN_WaterEdge,  8,11));
			SetShaderParam("TERRAIN_WaterGloss89AB", getVector(TERRAIN_WaterGloss,  8,11));
			SetShaderParam("TERRAIN_WaterOpacity89AB", getVector(TERRAIN_WaterOpacity,  8,11));
			SetShaderParam("TERRAIN_Refraction89AB", getVector(TERRAIN_Refraction,  8,11));
			SetShaderParam("TERRAIN_WetRefraction89AB", getVector(TERRAIN_WetRefraction,  8,11));
			SetShaderParam("TERRAIN_Flow89AB", getVector(TERRAIN_Flow,  8,11));
			SetShaderParam("TERRAIN_WetSpecularity89AB", getVector(TERRAIN_WetSpecularity,  8,11));
			SetShaderParam("TERRAIN_WetReflection89AB", getVector(TERRAIN_WetReflection,  8,11));
			SetShaderParam("TERRAIN_WaterColorR89AB", getColorVector(TERRAIN_WaterColor, 8,11, 0));
			SetShaderParam("TERRAIN_WaterColorG89AB", getColorVector(TERRAIN_WaterColor, 8,11, 1));
			SetShaderParam("TERRAIN_WaterColorB89AB", getColorVector(TERRAIN_WaterColor, 8,11, 2));
			SetShaderParam("TERRAIN_WaterColorA89AB", getColorVector(TERRAIN_WaterColor, 8,11, 3));
			SetShaderParam("TERRAIN_LayerReflection89AB", getVector(TERRAIN_LayerReflection,  8,11));		
			
			SetShaderParam("RTP_AO_89AB", getVector(AO_strength, 8,11));
			SetShaderParam("_VerticalTexture89AB", getVector(VerticalTextureStrength, 8,11));
		}
		
		/////////////////////////////////////////////////////////////////////
		//
		// layer dependent textures
		//
		/////////////////////////////////////////////////////////////////////
		SetShaderParam("_SplatAtlasA", splat_atlases[0]);
		SetShaderParam("_BumpMap01", Bump01);
		SetShaderParam("_BumpMap23", Bump23);
		SetShaderParam("_TERRAIN_HeightMap", HeightMap);
		SetShaderParam("_SSColorCombinedA", SSColorCombined);
		
		if (numLayers>4) {
			SetShaderParam("_SplatAtlasB", splat_atlases[1]);
			SetShaderParam("_TERRAIN_HeightMap2", HeightMap2);
		}
		if ((numLayers>4) && _4LAYERS_SHADER_USED) {
			//
			// przekieruj parametry warstw 4-7 na AddPass
			//
			SetShaderParam("_BumpMap89", Bump45);
			SetShaderParam("_BumpMapAB", Bump67);
			SetShaderParam("_TERRAIN_HeightMap3", HeightMap2);
			// potrzebne przy sniegu (firstpass moze korzystac z koloru i bumpmap 4-7)
			SetShaderParam("_BumpMap45", Bump45);
			SetShaderParam("_BumpMap67", Bump67);
		} else {
			SetShaderParam("_BumpMap45", Bump45);
			SetShaderParam("_BumpMap67", Bump67);
			
			//
			// AddPass
			//
			SetShaderParam("_BumpMap89", Bump89);
			SetShaderParam("_BumpMapAB", BumpAB);
			SetShaderParam("_TERRAIN_HeightMap3", HeightMap3);
		}
		
		use_mat=null;
	}
	
	public Vector4 getVector(float[] vec, int idxA, int idxB) {
		if (vec==null) return Vector4.zero;
		Vector4 ret=Vector4.zero;
		for(int i=idxA; i<=idxB; i++) {
			if (i<vec.Length) {
				ret[i-idxA]=vec[i];
			}
		}
		return ret;
	}
	public Vector4 getColorVector(Color[] vec, int idxA, int idxB, int channel) {
		if (vec==null) return Vector4.zero;
		Vector4 ret=Vector4.zero;
		for(int i=idxA; i<=idxB; i++) {
			if (i<vec.Length) {
				ret[i-idxA]=vec[i][channel];
			}
		}
		return ret;
	}	
	
	public Texture2D get_dumb_tex() {
		if (!dumb_tex) {
			dumb_tex=new Texture2D(32,32,TextureFormat.RGB24,false);
			Color[] cols=dumb_tex.GetPixels();
			for(int i=0; i<cols.Length; i++) {
				cols[i]=Color.white;
			}
			dumb_tex.SetPixels(cols);
			dumb_tex.Apply();
		}
		return dumb_tex;
	}	
	
	public void RestorePreset(ReliefTerrainPresetHolder holder) {
		numLayers=holder.numLayers;
		splats=new Texture2D[holder.splats.Length];
		for(int i=0; i<holder.splats.Length; i++) {
			splats[i]=holder.splats[i];
		}
		
		splat_atlases=new Texture2D[holder.splat_atlases.Length];
		for(int i=0; i<splat_atlases.Length; i++) {
			splat_atlases[i]=holder.splat_atlases[i];
		}
		
		RTP_MIP_BIAS=holder.RTP_MIP_BIAS;
		_SpecColor=holder._SpecColor;
	
		SuperDetailA_channel=holder.SuperDetailA_channel;
		SuperDetailB_channel=holder.SuperDetailB_channel;
	
		Bump01=holder.Bump01;
		Bump23=holder.Bump23;
		Bump45=holder.Bump45;
		Bump67=holder.Bump67;
		Bump89=holder.Bump89;
		BumpAB=holder.BumpAB;
	
		SSColorCombined=holder.SSColorCombined;
		
		BumpGlobal=holder.BumpGlobal;
		
		VerticalTexture=holder.VerticalTexture;
		BumpMapGlobalScale=holder.BumpMapGlobalScale;
		GlobalColorMapBlendValues=holder.GlobalColorMapBlendValues;
		GlobalColorMapSaturation=holder.GlobalColorMapSaturation;
		GlobalColorMapBrightness=holder.GlobalColorMapBrightness;
		_GlobalColorMapNearMIP=holder._GlobalColorMapNearMIP;
		_FarNormalDamp=holder._FarNormalDamp;
	
		blendMultiplier=holder.blendMultiplier;
		
		HeightMap=holder.HeightMap;
		HeightMap2=holder.HeightMap2;
		HeightMap3=holder.HeightMap3;
		
		ReliefTransform=holder.ReliefTransform;
		DIST_STEPS=holder.DIST_STEPS;
		WAVELENGTH=holder.WAVELENGTH;
		ReliefBorderBlend=holder.ReliefBorderBlend;
	
		ExtrudeHeight=holder.ExtrudeHeight;
		LightmapShading=holder.LightmapShading;
		
		SHADOW_STEPS=holder.SHADOW_STEPS;
		WAVELENGTH_SHADOWS=holder.WAVELENGTH_SHADOWS;
		SHADOW_SMOOTH_STEPS=holder.SHADOW_SMOOTH_STEPS;
		SelfShadowStrength=holder.SelfShadowStrength;
		ShadowSmoothing=holder.ShadowSmoothing;
		ShadowColor=holder.ShadowColor;
		
		distance_start=holder.distance_start;
		distance_transition=holder.distance_transition;
		distance_start_bumpglobal=holder.distance_start_bumpglobal;
		distance_transition_bumpglobal=holder.distance_transition_bumpglobal;
		rtp_perlin_start_val=holder.rtp_perlin_start_val;
		
		trees_shadow_distance_start=holder.trees_shadow_distance_start;
		trees_shadow_distance_transition=holder.trees_shadow_distance_transition;
		trees_shadow_value=holder.trees_shadow_value;
		trees_pixel_distance_start=holder.trees_pixel_distance_start;
		trees_pixel_distance_transition=holder.trees_pixel_distance_transition;
		trees_pixel_blend_val=holder.trees_pixel_blend_val;
		global_normalMap_multiplier=holder.global_normalMap_multiplier;
		
		rtp_mipoffset_globalnorm=holder.rtp_mipoffset_globalnorm;
		_SuperDetailTiling=holder._SuperDetailTiling;
		SuperDetailA=holder.SuperDetailA;
		SuperDetailB=holder.SuperDetailB;
		
		// reflection
		TERRAIN_ReflectionMap=holder.TERRAIN_ReflectionMap;
		TERRAIN_ReflectionMap_channel=holder.TERRAIN_ReflectionMap_channel;
		TERRAIN_ReflColorA=holder.TERRAIN_ReflColorA;
		TERRAIN_ReflColorB=holder.TERRAIN_ReflColorB;
		TERRAIN_ReflDistortion=holder.TERRAIN_ReflDistortion;
		TERRAIN_ReflectionRotSpeed=holder.TERRAIN_ReflectionRotSpeed;
		TERRAIN_FresnelPow=holder.TERRAIN_FresnelPow;
		TERRAIN_FresnelOffset=holder.TERRAIN_FresnelOffset;
		
		// water/wet
		TERRAIN_GlobalWetness=holder.TERRAIN_GlobalWetness;
				
		TERRAIN_WaterSpecularity=holder.TERRAIN_WaterSpecularity;
					
		TERRAIN_RippleMap=holder.TERRAIN_RippleMap;
		TERRAIN_RippleScale=holder.TERRAIN_RippleScale;
		TERRAIN_FlowScale=holder.TERRAIN_FlowScale;
		TERRAIN_FlowSpeed=holder.TERRAIN_FlowSpeed;
		TERRAIN_FlowMipOffset=holder.TERRAIN_FlowMipOffset;
		TERRAIN_WetDarkening=holder.TERRAIN_WetDarkening;
		TERRAIN_WetDropletsStrength=holder.TERRAIN_WetDropletsStrength;
			
		TERRAIN_RainIntensity=holder.TERRAIN_RainIntensity;
		TERRAIN_DropletsSpeed=holder.TERRAIN_DropletsSpeed;
				
		TERRAIN_mipoffset_flowSpeed=holder.TERRAIN_mipoffset_flowSpeed;
		
		// caustics
		TERRAIN_CausticsAnimSpeed=holder.TERRAIN_CausticsAnimSpeed;
		TERRAIN_CausticsColor=holder.TERRAIN_CausticsColor;
		TERRAIN_CausticsWaterLevel=holder.TERRAIN_CausticsWaterLevel;
		TERRAIN_CausticsWaterLevelByAngle=holder.TERRAIN_CausticsWaterLevelByAngle;
		TERRAIN_CausticsWaterDeepFadeLength=holder.TERRAIN_CausticsWaterDeepFadeLength;
		TERRAIN_CausticsWaterShallowFadeLength=holder.TERRAIN_CausticsWaterShallowFadeLength;
		TERRAIN_CausticsTilingScale=holder.TERRAIN_CausticsTilingScale;
		TERRAIN_CausticsTex=holder.TERRAIN_CausticsTex;
		
		rtp_customAmbientCorrection=holder.rtp_customAmbientCorrection;
		
		RTP_AOsharpness=holder.RTP_AOsharpness;
		RTP_AOamp=holder.RTP_AOamp;
		RTP_LightDefVector=holder.RTP_LightDefVector;
		RTP_ReflexLightDiffuseColor=holder.RTP_ReflexLightDiffuseColor;
		RTP_ReflexLightSpecColor=holder.RTP_ReflexLightSpecColor;
	
		VerticalTextureGlobalBumpInfluence=holder.VerticalTextureGlobalBumpInfluence;
		VerticalTextureTiling=holder.VerticalTextureTiling;
	
		// snow
		_snow_strength=holder._snow_strength;
		_global_color_brightness_to_snow=holder._global_color_brightness_to_snow;
		_snow_slope_factor=holder._snow_slope_factor;
		_snow_edge_definition=holder._snow_edge_definition;
		_snow_height_treshold=holder._snow_height_treshold;
		_snow_height_transition=holder._snow_height_transition;
		_snow_color=holder._snow_color;
		_snow_specular=holder._snow_specular;
		_snow_gloss=holder._snow_gloss;
		_snow_reflectivness=holder._snow_reflectivness;
		_snow_deep_factor=holder._snow_deep_factor;
		
		//////////////////////
		// layer_dependent arrays
		//////////////////////
		Bumps=new Texture2D[holder.Bumps.Length];
		Spec=new float[holder.Bumps.Length];
		FarGlossCorrection=new float[holder.Bumps.Length];
		MixScale=new float[holder.Bumps.Length];
		MixBlend=new float[holder.Bumps.Length];
		MixSaturation=new float[holder.Bumps.Length];
		GlobalColorPerLayer=new float[holder.Bumps.Length];
		PER_LAYER_HEIGHT_MODIFIER=new float[holder.Bumps.Length];
		_SuperDetailStrengthMultA=new float[holder.Bumps.Length];
		_SuperDetailStrengthMultASelfMaskNear=new float[holder.Bumps.Length];
		_SuperDetailStrengthMultASelfMaskFar=new float[holder.Bumps.Length];
		_SuperDetailStrengthMultB=new float[holder.Bumps.Length];
		_SuperDetailStrengthMultBSelfMaskNear=new float[holder.Bumps.Length];
		_SuperDetailStrengthMultBSelfMaskFar=new float[holder.Bumps.Length];
		_SuperDetailStrengthNormal=new float[holder.Bumps.Length];
		_BumpMapGlobalStrength=new float[holder.Bumps.Length];
		
		AO_strength=new float[holder.Bumps.Length];
		VerticalTextureStrength=new float[holder.Bumps.Length];
		
		Heights=new Texture2D[holder.Bumps.Length];
	
		_snow_strength_per_layer=new float[holder.Bumps.Length];
		Substances=new ProceduralMaterial[holder.Bumps.Length];
		
		// wet
		TERRAIN_LayerWetStrength=new float[holder.Bumps.Length];
		TERRAIN_WaterLevel=new float[holder.Bumps.Length];
		TERRAIN_WaterLevelSlopeDamp=new float[holder.Bumps.Length];
		TERRAIN_WaterEdge=new float[holder.Bumps.Length];
		TERRAIN_WaterGloss=new float[holder.Bumps.Length];
		TERRAIN_WaterOpacity=new float[holder.Bumps.Length];
		TERRAIN_Refraction=new float[holder.Bumps.Length];
		TERRAIN_WetRefraction=new float[holder.Bumps.Length];
		TERRAIN_Flow=new float[holder.Bumps.Length];
		TERRAIN_WetSpecularity=new float[holder.Bumps.Length];
		TERRAIN_WetReflection=new float[holder.Bumps.Length];
		TERRAIN_WaterColor=new Color[holder.Bumps.Length];
		TERRAIN_LayerReflection=new float[holder.Bumps.Length];
		
		for(int i=0; i<holder.Bumps.Length; i++) {
			Bumps[i]=holder.Bumps[i];
			Spec[i]=holder.Spec[i];
			FarGlossCorrection[i]=holder.FarGlossCorrection[i];
			MixScale[i]=holder.MixScale[i];
			MixBlend[i]=holder.MixBlend[i];
			MixSaturation[i]=holder.MixSaturation[i];
			GlobalColorPerLayer[i]=holder.GlobalColorPerLayer[i];
			PER_LAYER_HEIGHT_MODIFIER[i]=holder.PER_LAYER_HEIGHT_MODIFIER[i];
			_SuperDetailStrengthMultA[i]=holder._SuperDetailStrengthMultA[i];
			_SuperDetailStrengthMultASelfMaskNear[i]=holder._SuperDetailStrengthMultASelfMaskNear[i];
			_SuperDetailStrengthMultASelfMaskFar[i]=holder._SuperDetailStrengthMultASelfMaskFar[i];
			_SuperDetailStrengthMultB[i]=holder._SuperDetailStrengthMultB[i];
			_SuperDetailStrengthMultBSelfMaskNear[i]=holder._SuperDetailStrengthMultBSelfMaskNear[i];
			_SuperDetailStrengthMultBSelfMaskFar[i]=holder._SuperDetailStrengthMultBSelfMaskFar[i];
			_SuperDetailStrengthNormal[i]=holder._SuperDetailStrengthNormal[i];
			_BumpMapGlobalStrength[i]=holder._BumpMapGlobalStrength[i];
			
			VerticalTextureStrength[i]=holder.VerticalTextureStrength[i];
			AO_strength[i]=holder.AO_strength[i];
			
			Heights[i]=holder.Heights[i];
		
			_snow_strength_per_layer[i]=holder._snow_strength_per_layer[i];
			Substances[i]=holder.Substances[i];
			
			// wet
			TERRAIN_LayerWetStrength[i]=holder.TERRAIN_LayerWetStrength[i];
			TERRAIN_WaterLevel[i]=holder.TERRAIN_WaterLevel[i];
			TERRAIN_WaterLevelSlopeDamp[i]=holder.TERRAIN_WaterLevelSlopeDamp[i];
			TERRAIN_WaterEdge[i]=holder.TERRAIN_WaterEdge[i];
			TERRAIN_WaterGloss[i]=holder.TERRAIN_WaterGloss[i];
			TERRAIN_WaterOpacity[i]=holder.TERRAIN_WaterOpacity[i];
			TERRAIN_Refraction[i]=holder.TERRAIN_Refraction[i];
			TERRAIN_WetRefraction[i]=holder.TERRAIN_WetRefraction[i];
			TERRAIN_Flow[i]=holder.TERRAIN_Flow[i];
			TERRAIN_WetSpecularity[i]=holder.TERRAIN_WetSpecularity[i];
			TERRAIN_WetReflection[i]=holder.TERRAIN_WetReflection[i];
			TERRAIN_WaterColor[i]=holder.TERRAIN_WaterColor[i];
			TERRAIN_LayerReflection[i]=holder.TERRAIN_LayerReflection[i];
		}
	}
	
	public void SavePreset(ref ReliefTerrainPresetHolder holder) {
		holder.numLayers=numLayers;
		holder.splats=new Texture2D[splats.Length];
		for(int i=0; i<holder.splats.Length; i++) {
			holder.splats[i]=splats[i];
		}
		
		holder.splat_atlases=new Texture2D[splat_atlases.Length];
		for(int i=0; i<splat_atlases.Length; i++) {
			holder.splat_atlases[i]=splat_atlases[i];
		}
		
		holder.RTP_MIP_BIAS=RTP_MIP_BIAS;
		holder._SpecColor=_SpecColor;
	
		holder.SuperDetailA_channel=SuperDetailA_channel;
		holder.SuperDetailB_channel=SuperDetailB_channel;
	
		holder.Bump01=Bump01;
		holder.Bump23=Bump23;
		holder.Bump45=Bump45;
		holder.Bump67=Bump67;
		holder.Bump89=Bump89;
		holder.BumpAB=BumpAB;
	
		holder.SSColorCombined=SSColorCombined;
		
		holder.BumpGlobal=BumpGlobal;
		
		holder.VerticalTexture=VerticalTexture;
		holder.BumpMapGlobalScale=BumpMapGlobalScale;
		holder.GlobalColorMapBlendValues=GlobalColorMapBlendValues;
		holder.GlobalColorMapSaturation=GlobalColorMapSaturation;
		holder.GlobalColorMapBrightness=GlobalColorMapBrightness;
		holder._GlobalColorMapNearMIP=_GlobalColorMapNearMIP;
		holder._FarNormalDamp=_FarNormalDamp;
	
		holder.blendMultiplier=blendMultiplier;
		
		holder.HeightMap=HeightMap;
		holder.HeightMap2=HeightMap2;
		holder.HeightMap3=HeightMap3;
		
		holder.ReliefTransform=ReliefTransform;
		holder.DIST_STEPS=DIST_STEPS;
		holder.WAVELENGTH=WAVELENGTH;
		holder.ReliefBorderBlend=ReliefBorderBlend;
	
		holder.ExtrudeHeight=ExtrudeHeight;
		holder.LightmapShading=LightmapShading;
		
		holder.SHADOW_STEPS=SHADOW_STEPS;
		holder.WAVELENGTH_SHADOWS=WAVELENGTH_SHADOWS;
		holder.SHADOW_SMOOTH_STEPS=SHADOW_SMOOTH_STEPS;
		holder.SelfShadowStrength=SelfShadowStrength;
		holder.ShadowSmoothing=ShadowSmoothing;
		holder.ShadowColor=ShadowColor;
		
		holder.distance_start=distance_start;
		holder.distance_transition=distance_transition;
		holder.distance_start_bumpglobal=distance_start_bumpglobal;
		holder.distance_transition_bumpglobal=distance_transition_bumpglobal;
		holder.rtp_perlin_start_val=rtp_perlin_start_val;
		
		holder.trees_shadow_distance_start=trees_shadow_distance_start;
		holder.trees_shadow_distance_transition=trees_shadow_distance_transition;
		holder.trees_shadow_value=trees_shadow_value;
		holder.trees_pixel_distance_start=trees_pixel_distance_start;
		holder.trees_pixel_distance_transition=trees_pixel_distance_transition;
		holder.trees_pixel_blend_val=trees_pixel_blend_val;
		holder.global_normalMap_multiplier=global_normalMap_multiplier;
		
		holder.rtp_mipoffset_globalnorm=rtp_mipoffset_globalnorm;
		holder._SuperDetailTiling=_SuperDetailTiling;
		holder.SuperDetailA=SuperDetailA;
		holder.SuperDetailB=SuperDetailB;
		
		// reflection
		holder.TERRAIN_ReflectionMap=TERRAIN_ReflectionMap;
		holder.TERRAIN_ReflectionMap_channel=TERRAIN_ReflectionMap_channel;
		holder.TERRAIN_ReflColorA=TERRAIN_ReflColorA;
		holder.TERRAIN_ReflColorB=TERRAIN_ReflColorB;
		holder.TERRAIN_ReflDistortion=TERRAIN_ReflDistortion;
		holder.TERRAIN_ReflectionRotSpeed=TERRAIN_ReflectionRotSpeed;
		holder.TERRAIN_FresnelPow=TERRAIN_FresnelPow;
		holder.TERRAIN_FresnelOffset=TERRAIN_FresnelOffset;
		
		// water/wet
		holder.TERRAIN_GlobalWetness=TERRAIN_GlobalWetness;
				
		holder.TERRAIN_WaterSpecularity=TERRAIN_WaterSpecularity;
					
		holder.TERRAIN_RippleMap=TERRAIN_RippleMap;
		holder.TERRAIN_RippleScale=TERRAIN_RippleScale;
		holder.TERRAIN_FlowScale=TERRAIN_FlowScale;
		holder.TERRAIN_FlowSpeed=TERRAIN_FlowSpeed;
		holder.TERRAIN_FlowMipOffset=TERRAIN_FlowMipOffset;
		holder.TERRAIN_WetDarkening=TERRAIN_WetDarkening;
		holder.TERRAIN_WetDropletsStrength=TERRAIN_WetDropletsStrength;
			
		holder.TERRAIN_RainIntensity=TERRAIN_RainIntensity;
		holder.TERRAIN_DropletsSpeed=TERRAIN_DropletsSpeed;
				
		holder.TERRAIN_mipoffset_flowSpeed=TERRAIN_mipoffset_flowSpeed;
		
		// caustics
		holder.TERRAIN_CausticsAnimSpeed=TERRAIN_CausticsAnimSpeed;
		holder.TERRAIN_CausticsColor=TERRAIN_CausticsColor;
		holder.TERRAIN_CausticsWaterLevel=TERRAIN_CausticsWaterLevel;
		holder.TERRAIN_CausticsWaterLevelByAngle=TERRAIN_CausticsWaterLevelByAngle;
		holder.TERRAIN_CausticsWaterDeepFadeLength=TERRAIN_CausticsWaterDeepFadeLength;
		holder.TERRAIN_CausticsWaterShallowFadeLength=TERRAIN_CausticsWaterShallowFadeLength;
		holder.TERRAIN_CausticsTilingScale=TERRAIN_CausticsTilingScale;
		holder.TERRAIN_CausticsTex=TERRAIN_CausticsTex;
		
		holder.rtp_customAmbientCorrection=rtp_customAmbientCorrection;
		
		holder.RTP_AOsharpness=RTP_AOsharpness;
		holder.RTP_AOamp=RTP_AOamp;
		holder.RTP_LightDefVector=RTP_LightDefVector;
		holder.RTP_ReflexLightDiffuseColor=RTP_ReflexLightDiffuseColor;
		holder.RTP_ReflexLightSpecColor=RTP_ReflexLightSpecColor;
	
		holder.VerticalTextureGlobalBumpInfluence=VerticalTextureGlobalBumpInfluence;
		holder.VerticalTextureTiling=VerticalTextureTiling;
	
		// snow
		holder._snow_strength=_snow_strength;
		holder._global_color_brightness_to_snow=_global_color_brightness_to_snow;
		holder._snow_slope_factor=_snow_slope_factor;
		holder._snow_edge_definition=_snow_edge_definition;
		holder._snow_height_treshold=_snow_height_treshold;
		holder._snow_height_transition=_snow_height_transition;
		holder._snow_color=_snow_color;
		holder._snow_specular=_snow_specular;
		holder._snow_gloss=_snow_gloss;
		holder._snow_reflectivness=_snow_reflectivness;
		holder._snow_deep_factor=_snow_deep_factor;
		
		//////////////////////
		// layer_dependent arrays
		//////////////////////
		holder.Bumps=new Texture2D[numLayers];
		holder.Spec=new float[numLayers];
		holder.FarGlossCorrection=new float[numLayers];
		holder.MixScale=new float[numLayers];
		holder.MixBlend=new float[numLayers];
		holder.MixSaturation=new float[numLayers];
		holder.GlobalColorPerLayer=new float[numLayers];
		holder.PER_LAYER_HEIGHT_MODIFIER=new float[numLayers];
		holder._SuperDetailStrengthMultA=new float[numLayers];
		holder._SuperDetailStrengthMultASelfMaskNear=new float[numLayers];
		holder._SuperDetailStrengthMultASelfMaskFar=new float[numLayers];
		holder._SuperDetailStrengthMultB=new float[numLayers];
		holder._SuperDetailStrengthMultBSelfMaskNear=new float[numLayers];
		holder._SuperDetailStrengthMultBSelfMaskFar=new float[numLayers];
		holder._SuperDetailStrengthNormal=new float[numLayers];
		holder._BumpMapGlobalStrength=new float[numLayers];
		
		holder.VerticalTextureStrength=new float[numLayers];
		holder.AO_strength=new float[numLayers];
		
		holder.Heights=new Texture2D[numLayers];
	
		holder._snow_strength_per_layer=new float[numLayers];
		holder.Substances=new ProceduralMaterial[numLayers];
		
		// wet
		holder.TERRAIN_LayerWetStrength=new float[numLayers];
		holder.TERRAIN_WaterLevel=new float[numLayers];
		holder.TERRAIN_WaterLevelSlopeDamp=new float[numLayers];
		holder.TERRAIN_WaterEdge=new float[numLayers];
		holder.TERRAIN_WaterGloss=new float[numLayers];
		holder.TERRAIN_WaterOpacity=new float[numLayers];
		holder.TERRAIN_Refraction=new float[numLayers];
		holder.TERRAIN_WetRefraction=new float[numLayers];
		holder.TERRAIN_Flow=new float[numLayers];
		holder.TERRAIN_WetSpecularity=new float[numLayers];
		holder.TERRAIN_WetReflection=new float[numLayers];
		holder.TERRAIN_WaterColor=new Color[numLayers];
		holder.TERRAIN_LayerReflection=new float[numLayers];
		
		for(int i=0; i<numLayers; i++) {
			holder.Bumps[i]=Bumps[i];
			holder.Spec[i]=Spec[i];
			holder.FarGlossCorrection[i]=FarGlossCorrection[i];
			holder.MixScale[i]=MixScale[i];
			holder.MixBlend[i]=MixBlend[i];
			holder.MixSaturation[i]=MixSaturation[i];
			holder.GlobalColorPerLayer[i]=GlobalColorPerLayer[i];
			holder.PER_LAYER_HEIGHT_MODIFIER[i]=PER_LAYER_HEIGHT_MODIFIER[i];
			holder._SuperDetailStrengthMultA[i]=_SuperDetailStrengthMultA[i];
			holder._SuperDetailStrengthMultASelfMaskNear[i]=_SuperDetailStrengthMultASelfMaskNear[i];
			holder._SuperDetailStrengthMultASelfMaskFar[i]=_SuperDetailStrengthMultASelfMaskFar[i];
			holder._SuperDetailStrengthMultB[i]=_SuperDetailStrengthMultB[i];
			holder._SuperDetailStrengthMultBSelfMaskNear[i]=_SuperDetailStrengthMultBSelfMaskNear[i];
			holder._SuperDetailStrengthMultBSelfMaskFar[i]=_SuperDetailStrengthMultBSelfMaskFar[i];
			holder._SuperDetailStrengthNormal[i]=_SuperDetailStrengthNormal[i];
			holder._BumpMapGlobalStrength[i]=_BumpMapGlobalStrength[i];
			
			holder.VerticalTextureStrength[i]=VerticalTextureStrength[i];
			holder.AO_strength[i]=AO_strength[i];
			
			holder.Heights[i]=Heights[i];
		
			holder._snow_strength_per_layer[i]=_snow_strength_per_layer[i];
			holder.Substances[i]=Substances[i];
			
			// wet
			holder.TERRAIN_LayerWetStrength[i]=TERRAIN_LayerWetStrength[i];
			holder.TERRAIN_WaterLevel[i]=TERRAIN_WaterLevel[i];
			holder.TERRAIN_WaterLevelSlopeDamp[i]=TERRAIN_WaterLevelSlopeDamp[i];
			holder.TERRAIN_WaterEdge[i]=TERRAIN_WaterEdge[i];
			holder.TERRAIN_WaterGloss[i]=TERRAIN_WaterGloss[i];
			holder.TERRAIN_WaterOpacity[i]=TERRAIN_WaterOpacity[i];
			holder.TERRAIN_Refraction[i]=TERRAIN_Refraction[i];
			holder.TERRAIN_WetRefraction[i]=TERRAIN_WetRefraction[i];
			holder.TERRAIN_Flow[i]=TERRAIN_Flow[i];
			holder.TERRAIN_WetSpecularity[i]=TERRAIN_WetSpecularity[i];
			holder.TERRAIN_WetReflection[i]=TERRAIN_WetReflection[i];
			holder.TERRAIN_WaterColor[i]=TERRAIN_WaterColor[i];
			holder.TERRAIN_LayerReflection[i]=TERRAIN_LayerReflection[i];			
		}
	}
	
	public void InterpolatePresets(ReliefTerrainPresetHolder holderA, ReliefTerrainPresetHolder holderB, float t) {
		RTP_MIP_BIAS=Mathf.Lerp(holderA.RTP_MIP_BIAS, holderB.RTP_MIP_BIAS, t);
		_SpecColor=Color.Lerp(holderA._SpecColor, holderB._SpecColor, t);
	
		BumpMapGlobalScale=Mathf.Lerp(holderA.BumpMapGlobalScale, holderB.BumpMapGlobalScale, t);
		GlobalColorMapBlendValues=Vector3.Lerp(holderA.GlobalColorMapBlendValues, holderB.GlobalColorMapBlendValues, t);
		GlobalColorMapSaturation=Mathf.Lerp(holderA.GlobalColorMapSaturation, holderB.GlobalColorMapSaturation, t);
		GlobalColorMapBrightness=Mathf.Lerp(holderA.GlobalColorMapBrightness, holderB.GlobalColorMapBrightness, t);
		_GlobalColorMapNearMIP=Mathf.Lerp(holderA._GlobalColorMapNearMIP, holderB._GlobalColorMapNearMIP, t);
		_FarNormalDamp=Mathf.Lerp(holderA._FarNormalDamp, holderB._FarNormalDamp, t);
	
		blendMultiplier=Mathf.Lerp(holderA.blendMultiplier, holderB.blendMultiplier, t);
		
		ReliefTransform=Vector4.Lerp(holderA.ReliefTransform, holderB.ReliefTransform, t);
		DIST_STEPS=Mathf.Lerp(holderA.DIST_STEPS, holderB.DIST_STEPS, t);
		WAVELENGTH=Mathf.Lerp(holderA.WAVELENGTH, holderB.WAVELENGTH, t);
		ReliefBorderBlend=Mathf.Lerp(holderA.ReliefBorderBlend, holderB.ReliefBorderBlend, t);
	
		ExtrudeHeight=Mathf.Lerp(holderA.ExtrudeHeight, holderB.ExtrudeHeight, t);
		LightmapShading=Mathf.Lerp(holderA.LightmapShading, holderB.LightmapShading, t);
		
		SHADOW_STEPS=Mathf.Lerp(holderA.SHADOW_STEPS, holderB.SHADOW_STEPS, t);
		WAVELENGTH_SHADOWS=Mathf.Lerp(holderA.WAVELENGTH_SHADOWS, holderB.WAVELENGTH_SHADOWS, t);
		SHADOW_SMOOTH_STEPS=Mathf.Lerp(holderA.SHADOW_SMOOTH_STEPS, holderB.SHADOW_SMOOTH_STEPS, t);
		SelfShadowStrength=Mathf.Lerp(holderA.SelfShadowStrength, holderB.SelfShadowStrength, t);
		ShadowSmoothing=Mathf.Lerp(holderA.ShadowSmoothing, holderB.ShadowSmoothing, t);
		ShadowColor=Color.Lerp(holderA.ShadowColor, holderB.ShadowColor, t);
		
		distance_start=Mathf.Lerp(holderA.distance_start, holderB.distance_start, t);
		distance_transition=Mathf.Lerp(holderA.distance_transition, holderB.distance_transition, t);
		distance_start_bumpglobal=Mathf.Lerp(holderA.distance_start_bumpglobal, holderB.distance_start_bumpglobal, t);
		distance_transition_bumpglobal=Mathf.Lerp(holderA.distance_transition_bumpglobal, holderB.distance_transition_bumpglobal, t);
		rtp_perlin_start_val=Mathf.Lerp(holderA.rtp_perlin_start_val, holderB.rtp_perlin_start_val, t);
		
		trees_shadow_distance_start=Mathf.Lerp(holderA.trees_shadow_distance_start, holderB.trees_shadow_distance_start, t);
		trees_shadow_distance_transition=Mathf.Lerp(holderA.trees_shadow_distance_transition, holderB.trees_shadow_distance_transition, t);
		trees_shadow_value=Mathf.Lerp(holderA.trees_shadow_value, holderB.trees_shadow_value, t);
		trees_pixel_distance_start=Mathf.Lerp(holderA.trees_pixel_distance_start, holderB.trees_pixel_distance_start, t);
		trees_pixel_distance_transition=Mathf.Lerp(holderA.trees_pixel_distance_transition, holderB.trees_pixel_distance_transition, t);
		trees_pixel_blend_val=Mathf.Lerp(holderA.trees_pixel_blend_val, holderB.trees_pixel_blend_val, t);
		global_normalMap_multiplier=Mathf.Lerp (holderA.global_normalMap_multiplier, holderB.global_normalMap_multiplier, t);
		
		_SuperDetailTiling=Mathf.Lerp(holderA._SuperDetailTiling, holderB._SuperDetailTiling, t);
		
		// reflection
		TERRAIN_ReflColorA=Color.Lerp(holderA.TERRAIN_ReflColorA, holderB.TERRAIN_ReflColorA, t);
		TERRAIN_ReflColorB=Color.Lerp(holderA.TERRAIN_ReflColorB, holderB.TERRAIN_ReflColorB, t);
		TERRAIN_ReflDistortion=Mathf.Lerp(holderA.TERRAIN_ReflDistortion, holderB.TERRAIN_ReflDistortion, t);
		TERRAIN_ReflectionRotSpeed=Mathf.Lerp(holderA.TERRAIN_ReflectionRotSpeed, holderB.TERRAIN_ReflectionRotSpeed, t);
		TERRAIN_FresnelPow=Mathf.Lerp(holderA.TERRAIN_FresnelPow, holderB.TERRAIN_FresnelPow, t);
		TERRAIN_FresnelOffset=Mathf.Lerp(holderA.TERRAIN_FresnelOffset, holderB.TERRAIN_FresnelOffset, t);
		
		// water/wet
		TERRAIN_GlobalWetness=Mathf.Lerp(holderA.TERRAIN_GlobalWetness, holderB.TERRAIN_GlobalWetness, t);
				
		TERRAIN_WaterSpecularity=Mathf.Lerp(holderA.TERRAIN_WaterSpecularity, holderB.TERRAIN_WaterSpecularity, t);
					
		TERRAIN_RippleScale=Mathf.Lerp(holderA.TERRAIN_RippleScale, holderB.TERRAIN_RippleScale, t);
		TERRAIN_FlowScale=Mathf.Lerp(holderA.TERRAIN_FlowScale, holderB.TERRAIN_FlowScale, t);
		TERRAIN_FlowSpeed=Mathf.Lerp(holderA.TERRAIN_FlowSpeed, holderB.TERRAIN_FlowSpeed, t);
		TERRAIN_FlowMipOffset=Mathf.Lerp(holderA.TERRAIN_FlowMipOffset, holderB.TERRAIN_FlowMipOffset, t);
		TERRAIN_WetDarkening=Mathf.Lerp (holderA.TERRAIN_WetDarkening, holderB.TERRAIN_WetDarkening, t);
		TERRAIN_WetDropletsStrength=Mathf.Lerp(holderA.TERRAIN_WetDropletsStrength, holderB.TERRAIN_WetDropletsStrength, t);
			
		TERRAIN_RainIntensity=Mathf.Lerp(holderA.TERRAIN_RainIntensity, holderB.TERRAIN_RainIntensity, t);
		TERRAIN_DropletsSpeed=Mathf.Lerp(holderA.TERRAIN_DropletsSpeed, holderB.TERRAIN_DropletsSpeed, t);
				
		TERRAIN_mipoffset_flowSpeed=Mathf.Lerp(holderA.TERRAIN_mipoffset_flowSpeed, holderB.TERRAIN_mipoffset_flowSpeed, t);
		
		TERRAIN_CausticsAnimSpeed=Mathf.Lerp(holderA.TERRAIN_CausticsAnimSpeed, holderB.TERRAIN_CausticsAnimSpeed, t);
		TERRAIN_CausticsColor=Color.Lerp(holderA.TERRAIN_CausticsColor, holderB.TERRAIN_CausticsColor,t);
		TERRAIN_CausticsWaterLevel=Mathf.Lerp(holderA.TERRAIN_CausticsWaterLevel, holderB.TERRAIN_CausticsWaterLevel, t);
		TERRAIN_CausticsWaterLevelByAngle=Mathf.Lerp(holderA.TERRAIN_CausticsWaterLevelByAngle, holderB.TERRAIN_CausticsWaterLevelByAngle, t);
		TERRAIN_CausticsWaterDeepFadeLength=Mathf.Lerp(holderA.TERRAIN_CausticsWaterDeepFadeLength, holderB.TERRAIN_CausticsWaterDeepFadeLength, t);
		TERRAIN_CausticsWaterShallowFadeLength=Mathf.Lerp(holderA.TERRAIN_CausticsWaterShallowFadeLength, holderB.TERRAIN_CausticsWaterShallowFadeLength, t);
		TERRAIN_CausticsTilingScale=Mathf.Lerp (holderA.TERRAIN_CausticsTilingScale, holderB.TERRAIN_CausticsTilingScale, t);
		
		rtp_customAmbientCorrection=Color.Lerp(holderA.rtp_customAmbientCorrection, holderB.rtp_customAmbientCorrection, t);
		
		RTP_AOsharpness=Mathf.Lerp(holderA.RTP_AOsharpness, holderB.RTP_AOsharpness, t);
		RTP_AOamp=Mathf.Lerp(holderA.RTP_AOamp, holderB.RTP_AOamp, t);
		RTP_LightDefVector=Vector4.Lerp(holderA.RTP_LightDefVector, holderB.RTP_LightDefVector, t);
		RTP_ReflexLightDiffuseColor=Color.Lerp(holderA.RTP_ReflexLightDiffuseColor, holderB.RTP_ReflexLightDiffuseColor, t);
		RTP_ReflexLightSpecColor=Color.Lerp(holderA.RTP_ReflexLightSpecColor, holderB.RTP_ReflexLightSpecColor, t);
		
		VerticalTextureGlobalBumpInfluence=Mathf.Lerp(holderA.VerticalTextureGlobalBumpInfluence, holderB.VerticalTextureGlobalBumpInfluence, t);
		VerticalTextureTiling=Mathf.Lerp(holderA.VerticalTextureTiling, holderB.VerticalTextureTiling, t);
	
		// snow
		_snow_strength=Mathf.Lerp(holderA._snow_strength, holderB._snow_strength, t);
		_global_color_brightness_to_snow=Mathf.Lerp(holderA._global_color_brightness_to_snow, holderB._global_color_brightness_to_snow, t);
		_snow_slope_factor=Mathf.Lerp(holderA._snow_slope_factor, holderB._snow_slope_factor, t);
		_snow_edge_definition=Mathf.Lerp(holderA._snow_edge_definition, holderB._snow_edge_definition, t);
		_snow_height_treshold=Mathf.Lerp(holderA._snow_height_treshold, holderB._snow_height_treshold, t);
		_snow_height_transition=Mathf.Lerp(holderA._snow_height_transition, holderB._snow_height_transition, t);
		_snow_color=Color.Lerp(holderA._snow_color, holderB._snow_color, t);
		_snow_specular=Mathf.Lerp(holderA._snow_specular, holderB._snow_specular, t);
		_snow_gloss=Mathf.Lerp(holderA._snow_gloss, holderB._snow_gloss, t);
		_snow_reflectivness=Mathf.Lerp(holderA._snow_reflectivness, holderB._snow_reflectivness, t);
		_snow_deep_factor=Mathf.Lerp(holderA._snow_deep_factor, holderB._snow_deep_factor, t);
		
		//////////////////////
		// layer_dependent arrays
		//////////////////////
		for(int i=0; i<holderA.Spec.Length; i++) {
			if (i<Spec.Length) {
				Spec[i]=Mathf.Lerp(holderA.Spec[i], holderB.Spec[i], t);
				FarGlossCorrection[i]=Mathf.Lerp(holderA.FarGlossCorrection[i], holderB.FarGlossCorrection[i], t);
				MixScale[i]=Mathf.Lerp(holderA.MixScale[i], holderB.MixScale[i], t);
				MixBlend[i]=Mathf.Lerp(holderA.MixBlend[i], holderB.MixBlend[i], t);
				MixSaturation[i]=Mathf.Lerp(holderA.MixSaturation[i], holderB.MixSaturation[i], t);
				GlobalColorPerLayer[i]=Mathf.Lerp(holderA.GlobalColorPerLayer[i], holderB.GlobalColorPerLayer[i], t);
				PER_LAYER_HEIGHT_MODIFIER[i]=Mathf.Lerp(holderA.PER_LAYER_HEIGHT_MODIFIER[i], holderB.PER_LAYER_HEIGHT_MODIFIER[i], t);
				_SuperDetailStrengthMultA[i]=Mathf.Lerp(holderA._SuperDetailStrengthMultA[i], holderB._SuperDetailStrengthMultA[i], t);
				_SuperDetailStrengthMultASelfMaskNear[i]=Mathf.Lerp(holderA._SuperDetailStrengthMultASelfMaskNear[i], holderB._SuperDetailStrengthMultASelfMaskNear[i], t);
				_SuperDetailStrengthMultASelfMaskFar[i]=Mathf.Lerp(holderA._SuperDetailStrengthMultASelfMaskFar[i], holderB._SuperDetailStrengthMultASelfMaskFar[i], t);
				_SuperDetailStrengthMultB[i]=Mathf.Lerp(holderA._SuperDetailStrengthMultB[i], holderB._SuperDetailStrengthMultB[i], t);
				_SuperDetailStrengthMultBSelfMaskNear[i]=Mathf.Lerp(holderA._SuperDetailStrengthMultBSelfMaskNear[i], holderB._SuperDetailStrengthMultBSelfMaskNear[i], t);
				_SuperDetailStrengthMultBSelfMaskFar[i]=Mathf.Lerp(holderA._SuperDetailStrengthMultBSelfMaskFar[i], holderB._SuperDetailStrengthMultBSelfMaskFar[i], t);
				_SuperDetailStrengthNormal[i]=Mathf.Lerp(holderA._SuperDetailStrengthNormal[i], holderB._SuperDetailStrengthNormal[i], t);
				_BumpMapGlobalStrength[i]=Mathf.Lerp(holderA._BumpMapGlobalStrength[i], holderB._BumpMapGlobalStrength[i], t);
				
				AO_strength[i]=Mathf.Lerp(holderA.AO_strength[i], holderB.AO_strength[i], t);
				VerticalTextureStrength[i]=Mathf.Lerp(holderA.VerticalTextureStrength[i], holderB.VerticalTextureStrength[i], t);
				
				_snow_strength_per_layer[i]=Mathf.Lerp(holderA._snow_strength_per_layer[i], holderB._snow_strength_per_layer[i], t);
				
				// wet
				TERRAIN_LayerWetStrength[i]=Mathf.Lerp(holderA.TERRAIN_LayerWetStrength[i], holderB.TERRAIN_LayerWetStrength[i], t);
				TERRAIN_WaterLevel[i]=Mathf.Lerp(holderA.TERRAIN_WaterLevel[i], holderB.TERRAIN_WaterLevel[i], t);
				TERRAIN_WaterLevelSlopeDamp[i]=Mathf.Lerp(holderA.TERRAIN_WaterLevelSlopeDamp[i], holderB.TERRAIN_WaterLevelSlopeDamp[i], t);
				TERRAIN_WaterEdge[i]=Mathf.Lerp(holderA.TERRAIN_WaterEdge[i], holderB.TERRAIN_WaterEdge[i], t);
				TERRAIN_WaterGloss[i]=Mathf.Lerp(holderA.TERRAIN_WaterGloss[i], holderB.TERRAIN_WaterGloss[i], t);
				TERRAIN_WaterOpacity[i]=Mathf.Lerp(holderA.TERRAIN_WaterOpacity[i], holderB.TERRAIN_WaterOpacity[i], t);
				TERRAIN_Refraction[i]=Mathf.Lerp(holderA.TERRAIN_Refraction[i], holderB.TERRAIN_Refraction[i], t);
				TERRAIN_WetRefraction[i]=Mathf.Lerp(holderA.TERRAIN_WetRefraction[i], holderB.TERRAIN_WetRefraction[i], t);
				TERRAIN_Flow[i]=Mathf.Lerp(holderA.TERRAIN_Flow[i], holderB.TERRAIN_Flow[i], t);
				TERRAIN_WetSpecularity[i]=Mathf.Lerp(holderA.TERRAIN_WetSpecularity[i], holderB.TERRAIN_WetSpecularity[i], t);
				TERRAIN_WetReflection[i]=Mathf.Lerp(holderA.TERRAIN_WetReflection[i], holderB.TERRAIN_WetReflection[i], t);
				TERRAIN_WaterColor[i]=Color.Lerp(holderA.TERRAIN_WaterColor[i], holderB.TERRAIN_WaterColor[i], t);
				TERRAIN_LayerReflection[i]=Mathf.Lerp(holderA.TERRAIN_LayerReflection[i], holderB.TERRAIN_LayerReflection[i], t);
			}
		}
	}	
	
	public void ReturnToDefaults(string what="", int layerIdx=-1) {
		// main settings
		if (what=="" || what=="main") {		
			ReliefTransform=new Vector4(200,200,0,0);
			distance_start=5f;
			distance_transition=20f;
			_SpecColor=new Color(200.0f/255.0f, 200.0f/255.0f, 200.0f/255.0f, 1);
			rtp_customAmbientCorrection=new Color(0.2f, 0.2f, 0.2f, 1);
			RTP_LightDefVector=new Vector4(0.05f, 0.5f, 0.5f, 25.0f);
			RTP_ReflexLightDiffuseColor=new Color(202.0f/255.0f, 240.0f/255.0f, 1, 0.2f);
			RTP_ReflexLightSpecColor=new Color(240.0f/255.0f, 245.0f/255.0f, 1, 0.15f);

			ReliefBorderBlend=6;
			LightmapShading=0f;
			RTP_MIP_BIAS=0;		
			RTP_AOsharpness=1.5f;
			RTP_AOamp=0.1f;
		}
		
		//perlin
		if (what=="" || what=="perlin") {
			BumpMapGlobalScale=0.1f;
			_FarNormalDamp=0.2f;
			distance_start_bumpglobal=30f;
			distance_transition_bumpglobal=30f;
			rtp_perlin_start_val=0;
		}
		
		// global color
		if (what=="" || what=="global_color") {
			GlobalColorMapBlendValues=new Vector3(0.2f, 0.4f, 0.5f);
			GlobalColorMapSaturation=1;
			GlobalColorMapBrightness=1;
			_GlobalColorMapNearMIP=0;
			
			trees_shadow_distance_start=50;
			trees_shadow_distance_transition=10;
			trees_shadow_value=0.5f;
			trees_pixel_distance_start=500;
			trees_pixel_distance_transition=10;
			trees_pixel_blend_val=2;
			global_normalMap_multiplier=3;
		}
		
		// uvblend
		if (what=="" || what=="uvblend") {
			blendMultiplier=1;
		}
		
		// POM/PM settings
		if (what=="" || what=="pom/pm") {
			ExtrudeHeight=0.05f;
			DIST_STEPS=20;
			WAVELENGTH=2;
			SHADOW_STEPS=20f;
			WAVELENGTH_SHADOWS=2f;
			SHADOW_SMOOTH_STEPS=6f;
			SelfShadowStrength=0.8f;
			ShadowSmoothing=1f;	
		}
		
		// snow global
		if (what=="" || what=="snow") {		
			_global_color_brightness_to_snow=0.5f;
			_snow_strength=0;
			_snow_slope_factor=2;
			_snow_edge_definition=5;
			_snow_height_treshold=-200;
			_snow_height_transition=1;
			_snow_color=Color.white;
			_snow_specular=0.5f;
			_snow_gloss=0.7f;
			_snow_reflectivness=0.7f;
			_snow_deep_factor=1.5f;
		}
		
		// superdetail
		if (what=="" || what=="superdetail") {
			_SuperDetailTiling=8;
		}
		
		// vertical texturing
		if (what=="" || what=="vertical") {
			VerticalTextureGlobalBumpInfluence=0;
			VerticalTextureTiling=50f;
		}
		
		// reflection
		if (what=="" || what=="reflection") {
			TERRAIN_ReflDistortion=0.05f;
			TERRAIN_ReflectionRotSpeed=0.3f;
			TERRAIN_FresnelPow=2;
			TERRAIN_FresnelOffset=0.2f;
			TERRAIN_ReflColorA=new Color(0.5f,0.5f,0.5f,1);
			TERRAIN_ReflColorB=new Color(0.7f,0.7f,1,0.5f);
		}
		
		// water
		if (what=="" || what=="water") {		
			TERRAIN_WaterSpecularity=0.5f;
			
			TERRAIN_GlobalWetness=1;
			TERRAIN_RippleScale=4;
			TERRAIN_FlowScale=1;
			TERRAIN_FlowSpeed=0.5f;
			TERRAIN_RainIntensity=1;
			TERRAIN_DropletsSpeed=10;		
			
			TERRAIN_mipoffset_flowSpeed=1;	
		}
		
		// caustics
		if (what=="" || what=="caustics") {
			TERRAIN_CausticsAnimSpeed=2;
			TERRAIN_CausticsColor=Color.white;
			TERRAIN_CausticsWaterLevel=30;
			TERRAIN_CausticsWaterLevelByAngle=2;
			TERRAIN_CausticsWaterDeepFadeLength=50;
			TERRAIN_CausticsWaterShallowFadeLength=30;
			TERRAIN_CausticsTilingScale=1;
		}
		
		// layer
		if (what=="" || what=="layer") {
			int b=0;
			int e=numLayers<12 ? numLayers:12;
			if (layerIdx>=0) {
				b=layerIdx;
				e=layerIdx+1;
			}
			for(int j=b; j<e; j++)  {
				Spec[j]=0.3f;
				FarGlossCorrection[j]=0;
				MIPmult[j]=0.0f;
				MixScale[j]=0.2f;
				MixBlend[j]=0.5f;
				MixSaturation[j]=0.3f;
				GlobalColorPerLayer[j]=1.0f;
	
				PER_LAYER_HEIGHT_MODIFIER[j]=0;
				
				_SuperDetailStrengthMultA[j]=0;
				_SuperDetailStrengthMultASelfMaskNear[j]=0;
				_SuperDetailStrengthMultASelfMaskFar[j]=0;
				_SuperDetailStrengthMultB[j]=0;
				_SuperDetailStrengthMultBSelfMaskNear[j]=0;
				_SuperDetailStrengthMultBSelfMaskFar[j]=0;
				_SuperDetailStrengthNormal[j]=0;
				_BumpMapGlobalStrength[j]=0.3f;
				
				_snow_strength_per_layer[j]=1;
				
				VerticalTextureStrength[j]=0.5f;
				AO_strength[j]=1;
				
				TERRAIN_LayerWetStrength[j]=1;
				TERRAIN_WaterLevel[j]=0.5f;
				TERRAIN_WaterLevelSlopeDamp[j]=0.5f;
				TERRAIN_WaterEdge[j]=2;
				TERRAIN_WaterGloss[j]=1;
				TERRAIN_WaterOpacity[j]=0.3f;
				TERRAIN_Refraction[j]=0.01f;
				TERRAIN_WetRefraction[j]=0.2f;
				TERRAIN_Flow[j]=0.3f;
				TERRAIN_WetSpecularity[j]=0.4f;
				TERRAIN_WetReflection[j]=1;			
				TERRAIN_WaterColor[j]=new Color(0.9f,0.9f,1,0.5f);
			}
		}
		
	}
	
#if UNITY_EDITOR
	public bool PrepareNormals() {
		if (Bumps==null) return false;
		Texture2D[] bumps=new Texture2D[(numLayers>8) ? 12 : ((numLayers>4) ? 8 : 4)];
		int i;
		for(i=0; i<bumps.Length; i++) 	bumps[i]=(i<Bumps.Length) ? Bumps[i] : null;
		for(i=0; i<bumps.Length; i++) {
			if (!bumps[i]) {
				if ((i&1)==0) {
					if (bumps[i+1]) {
						bumps[i]=new Texture2D(bumps[i+1].width, bumps[i+1].width,TextureFormat.ARGB32,false,true);
						FillTex(bumps[i], new Color32(128,128,128,128));
					}
				} else {
					if (bumps[i-1]) {
						bumps[i]=new Texture2D(bumps[i-1].width, bumps[i-1].width,TextureFormat.ARGB32,false,true);
						FillTex(bumps[i], new Color32(128,128,128,128));
					}
				}
			}
			if (bumps[i]) {
				try { 
					bumps[i].GetPixels(0,0,4,4,0);
				} catch (Exception e) {
					Debug.LogError("Normal texture "+i+" has to be marked as isReadable...");
					Debug.LogError(e.Message);
					activateObject=bumps[i];
					return false;
				}
			} else {
				bumps[i]=new Texture2D(4,4,TextureFormat.ARGB32,false,true);
				FillTex(bumps[i], new Color32(128,128,128,128));
			}
		}
		if (bumps[0] && bumps[1] && bumps[0].width!=bumps[1].width) {
			Debug.LogError("Normal textures pair 0,1 should have the same size");
			activateObject=bumps[1];
			//Time.timeScale=0; // pause
			return false;
		}
		if (bumps[2] && bumps[3] && bumps[2].width!=bumps[3].width) {
			Debug.LogError("Normal textures pair 2,3 should have the same size");
			activateObject=bumps[3];
			//Time.timeScale=0; // pause
			return false;
		}
		Bump01=CombineNormals(bumps[0], bumps[1]);
		Bump23=CombineNormals(bumps[2], bumps[3]);
		if (bumps.Length>4) {
			if (bumps[4] && bumps[5] && bumps[4].width!=bumps[5].width) {
				Debug.LogError("Normal textures pair 4,5 should have the same size");
				activateObject=bumps[5];
				//Time.timeScale=0; // pause
				return false;
			}
			if (bumps[6] && bumps[7] && bumps[6].width!=bumps[7].width) {
				Debug.LogError("Normal textures pair 6,7 should have the same size");
				activateObject=bumps[7];
				//Time.timeScale=0; // pause
				return false;
			}
			Bump45=CombineNormals(bumps[4], bumps[5]);
			Bump67=CombineNormals(bumps[6], bumps[7]);
		}
		if (bumps.Length>8) {
			if (bumps[8] && bumps[9] && bumps[8].width!=bumps[9].width) {
				Debug.LogError("Normal textures pair 8,9 should have the same size");
				activateObject=bumps[9];
				//Time.timeScale=0; // pause
				return false;
			}
			if (bumps[10] && bumps[11] && bumps[10].width!=bumps[11].width) {
				Debug.LogError("Normal textures pair 10,11 should have the same size");
				activateObject=bumps[11];
				//Time.timeScale=0; // pause
				return false;
			}
			Bump89=CombineNormals(bumps[8], bumps[9]);
			BumpAB=CombineNormals(bumps[10], bumps[11]);
		}
		return true;
	}
	
	private void FillTex(Texture2D tex, Color32 col) {
		Color32[] cols=tex.GetPixels32();
		for(int i=0; i<cols.Length; i++) {
			cols[i].r=col.r;
			cols[i].g=col.g;
			cols[i].b=col.b;
			cols[i].a=col.a;
		}
		tex.SetPixels32(cols);
	}
		
	private Texture2D CombineNormals(Texture2D texA, Texture2D texB) {
		if (!texA) return null;
		Color32[] colsA=texA.GetPixels32();
		Color32[] colsB=texB.GetPixels32();
		Color32[] cols=new Color32[colsA.Length];
		for(int i=0; i<cols.Length; i++) {
			cols[i].r=colsA[i].a;
			cols[i].g=colsA[i].g;
			cols[i].b=colsB[i].a;
			cols[i].a=colsB[i].g;
		}
		Texture2D tex=new Texture2D(texA.width, texA.width, TextureFormat.ARGB32, true, true);
		tex.SetPixels32(cols);
		tex.Apply(true,false);
		//tex.Compress(true); // may try, but quality will be bad...
		//tex.Apply(false,true); // not readable przy publishingu
		tex.filterMode=FilterMode.Trilinear;
		tex.anisoLevel=0;
		return tex;
	}

	public void CopyWaterParams(int src, int tgt) {
		TERRAIN_LayerWetStrength[tgt]=TERRAIN_LayerWetStrength[src];
		TERRAIN_WaterLevel[tgt]=TERRAIN_WaterLevel[src];
		TERRAIN_WaterLevelSlopeDamp[tgt]=TERRAIN_WaterLevelSlopeDamp[src];
		TERRAIN_WaterEdge[tgt]=TERRAIN_WaterEdge[src];
		TERRAIN_WaterGloss[tgt]=TERRAIN_WaterGloss[src];
		TERRAIN_WaterOpacity[tgt]=TERRAIN_WaterOpacity[src];
		TERRAIN_Refraction[tgt]=TERRAIN_Refraction[src];
		TERRAIN_WetRefraction[tgt]=TERRAIN_WetRefraction[src];
		TERRAIN_Flow[tgt]=TERRAIN_Flow[src];
		TERRAIN_WetSpecularity[tgt]=TERRAIN_WetSpecularity[src];
		TERRAIN_WetReflection[tgt]=TERRAIN_WetReflection[src];
		TERRAIN_WaterColor[tgt]=TERRAIN_WaterColor[src];
	}	
	
	public void RecalcControlMaps(Terrain terrainComp, ReliefTerrain rt) {
		float[,,] splatData=terrainComp.terrainData.GetAlphamaps(0,0,terrainComp.terrainData.alphamapResolution, terrainComp.terrainData.alphamapResolution);
		Color[] cols_control;
		float[,] norm_array=new float[terrainComp.terrainData.alphamapResolution,terrainComp.terrainData.alphamapResolution];
		if (rt.splat_layer_ordered_mode) {
			// ordered mode
			for(int k=0; k<terrainComp.terrainData.alphamapLayers; k++) {
				int n=rt.splat_layer_seq[k];
				// value for current layer
				if (rt.splat_layer_calc[n]) {
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[terrainComp.terrainData.alphamapResolution*terrainComp.terrainData.alphamapResolution];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
							for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
								cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
								idx++;
							}
						}
						idx=0;
					}
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							norm_array[i,j]=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n];
							if (norm_array[i,j]>1) norm_array[i,j]=1;
						}
					}
				} else {
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							norm_array[i,j]=splatData[i,j,n];
							if (norm_array[i,j]>1) norm_array[i,j]=1;
						}
					}
				}
				// damp underlying layers
				for(int l=0; l<k; l++) {
					int m=rt.splat_layer_seq[l];
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							splatData[i,j,m]*=(1-norm_array[i,j]);
						}
					}
				}
				// write current layer
				if (rt.splat_layer_calc[n]) {			
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[terrainComp.terrainData.alphamapResolution*terrainComp.terrainData.alphamapResolution];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}						
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
							for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
								cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
								idx++;
							}
						}
						idx=0;
					}						
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							splatData[i,j,n]=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n];
							if (splatData[i,j,n]>1) splatData[i,j,n]=1;
						}
					}
				}
			}
		} else {
			// unordered mode
			for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
				for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
					norm_array[i,j]=0;
				}
			}
			for(int n=0; n<terrainComp.terrainData.alphamapLayers; n++) {
				if (rt.splat_layer_calc[n]) {
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[terrainComp.terrainData.alphamapResolution*terrainComp.terrainData.alphamapResolution];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
							for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
								cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
								idx++;
							}
						}
						idx=0;
					}
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							norm_array[i,j]+=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n];
						}
					}
				} else {
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							norm_array[i,j]+=splatData[i,j,n];
						}
					}
				}
			}
			for(int n=0; n<terrainComp.terrainData.alphamapLayers; n++) {
				if (rt.splat_layer_calc[n]) {			
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[terrainComp.terrainData.alphamapResolution*terrainComp.terrainData.alphamapResolution];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
							for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
								cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
								idx++;
							}
						}
						idx=0;
					}
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							splatData[i,j,n]=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n]/norm_array[i,j];
						}
					}
				} else {
					for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
						for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
							splatData[i,j,n]=splatData[i,j,n]/norm_array[i,j];
						}
					}
				}			
			}
		}
		terrainComp.terrainData.SetAlphamaps(0,0, splatData);			

	}
	
	public void RecalcControlMapsForMesh(ReliefTerrain rt) {
	
		float[,] splatData;
		Color[] cols;
		if (numLayers>4 && rt.controlA!=null && rt.controlB!=null) {
			if (rt.controlA.width!=rt.controlB.width) {
				Debug.LogError("Control maps A&B have to be of the same size for recalculation !");
				return;				
			} else {
				bool exit=false;
				for(int k=0; k<rt.source_controls.Length; k++) {
					if (rt.splat_layer_calc[k] && rt.source_controls[k].width!=rt.controlA.width) {
						Debug.LogError("Source control map "+k+" should be of the control texture size ("+rt.controlA.width+") !");
						exit=true;
					}
				}
				for(int k=0; k<rt.source_controls_mask.Length; k++) {
					if (rt.splat_layer_masked[k] && rt.source_controls_mask[k].width!=rt.controlA.width) {
						Debug.LogError("Source mask control map "+k+" should be of the control texture size ("+rt.controlA.width+") !");
						exit=true;
					}
				}
				if (exit) return;
			}
		}
		if (rt.controlA==null) {
			rt.controlA=new Texture2D(1024, 1024, TextureFormat.ARGB32, true);
			cols=new Color[1024*1024];
			for(int i=0; i<cols.Length; i++) cols[i]=new Color(1,0,0,0);
			rt.controlA.Apply(false,false);
		} else {
			cols=rt.controlA.GetPixels(0);
		}
		splatData=new float[rt.controlA.width*rt.controlA.width, numLayers];
		for(int n=0; n<numLayers; n++) {
			if (n==4) {
				if (rt.controlB==null) {
					rt.controlB=new Texture2D(rt.controlA.width, rt.controlA.width, TextureFormat.ARGB32, true);
					cols=new Color[1024*1024];
					for(int i=0; i<cols.Length; i++) cols[i]=new Color(0,0,0,0);
					rt.controlB.Apply(false,false);
				} else {
					cols=rt.controlB.GetPixels(0);
				}
			}
			if (n==8) {
				if (rt.controlC==null) {
					rt.controlC=new Texture2D(rt.controlA.width, rt.controlA.width, TextureFormat.ARGB32, true);
					cols=new Color[1024*1024];
					for(int i=0; i<cols.Length; i++) cols[i]=new Color(0,0,0,0);
					rt.controlC.Apply(false,false);
				} else {
					cols=rt.controlC.GetPixels(0);
				}
			}
			for(int i=0; i<cols.Length; i++) {
				splatData[i,n]=cols[i][n%4];
			}
		}
		
		Color[] cols_control;
		float[] norm_array=new float[rt.controlA.width*rt.controlA.width];
		if (rt.splat_layer_ordered_mode) {
			// ordered mode
			for(int k=0; k<numLayers; k++) {
				int n=rt.splat_layer_seq[k];
				// value for current layer
				if (rt.splat_layer_calc[n]) {
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[rt.controlA.width*rt.controlA.width];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<rt.controlA.width; i++) {
							for(int j=0; j<rt.controlA.width; j++) {
								cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
								idx++;
							}
						}
						idx=0;
					}
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						norm_array[i]=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n];
						if (norm_array[i]>1) norm_array[i]=1;
					}
				} else {
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						norm_array[i]=splatData[i,n];
						if (norm_array[i]>1) norm_array[i]=1;
					}
				}
				// damp underlying layers
				for(int l=0; l<k; l++) {
					int m=rt.splat_layer_seq[l];
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						splatData[i,m]*=(1-norm_array[i]);
					}
				}
				// write current layer
				if (rt.splat_layer_calc[n]) {			
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[rt.controlA.width*rt.controlA.width];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}						
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
							cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
							idx++;
						}
						idx=0;
					}						
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						splatData[i,n]=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n];
					}
				}
			}
		} else {
			// unordered mode
			for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
				norm_array[i]=0;
			}
			for(int n=0; n<numLayers; n++) {
				if (rt.splat_layer_calc[n]) {
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[rt.controlA.width*rt.controlA.width];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
							cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
							idx++;
						}
						idx=0;
					}
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						norm_array[i]+=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n];
					}
				} else {
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						norm_array[i]+=splatData[i,n];
					}
				}
			}
			for(int n=0; n<numLayers; n++) {
				if (rt.splat_layer_calc[n]) {			
					int idx=0;
					if (rt.source_controls[n]) {
						cols_control=rt.source_controls[n].GetPixels();
					} else {
						cols_control=new Color[rt.controlA.width*rt.controlA.width];
						if (rt.source_controls_invert[n]) {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.black;
						} else {
							for(int i=0; i<cols_control.Length; i++) cols_control[i]=Color.white;
						}
					}
					int channel_idx=(int)rt.source_controls_channels[n];
					// apply mask
					if (rt.splat_layer_masked[n] && rt.source_controls_mask[n]) {
						Color[] cols_mask=rt.source_controls_mask[n].GetPixels();
						idx=0;
						int channel_idx_mask=(int)rt.source_controls_mask_channels[n];
						for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
							cols_control[idx][channel_idx]*=cols_mask[idx][channel_idx_mask];
							idx++;
						}
						idx=0;
					}
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						splatData[i,n]=cols_control[idx++][channel_idx]*rt.splat_layer_boost[n]/norm_array[i];
					}
				} else {
					for(int i=0; i<rt.controlA.width*rt.controlA.width; i++) {
						splatData[i,n]=splatData[i,n]/norm_array[i];
					}
				}			
			}
		}
		
		for(int n=0; n<numLayers; n++) {
			if (n==0) {
				for(int i=0; i<cols.Length; i++) {
					cols[i]=new Color(0,0,0,0);
				}				
			}
			for(int i=0; i<cols.Length; i++) {
				cols[i][n%4]=splatData[i,n];
			}				
			if (n==3) {
				rt.controlA.SetPixels(cols,0);
				rt.controlA.Apply(true, false);
			} else if (n==7) {
				rt.controlB.SetPixels(cols,0);
				rt.controlB.Apply(true, false);
			} else if (n==11) {
				rt.controlC.SetPixels(cols,0);
				rt.controlC.Apply(true, false);
			} else if (n==numLayers-1) {
				if (n<4) {
					rt.controlA.SetPixels(cols,0);
					rt.controlA.Apply(true, false);
				} else if (n<8) {
					rt.controlB.SetPixels(cols,0);
					rt.controlB.Apply(true, false);
				} else {
					rt.controlC.SetPixels(cols,0);
					rt.controlC.Apply(true, false);
				}
			}
		}	

	}		
	public void InvertChannel(Color[] cols, int channel_idx=-1) {
		if (channel_idx<0) {
			for(int idx=0; idx<cols.Length; idx++) {
				cols[idx].r = 1-cols[idx].r;
				cols[idx].g = 1-cols[idx].g;
				cols[idx].b = 1-cols[idx].b;
				cols[idx].a = 1-cols[idx].a;
			}		
		} else {
			for(int idx=0; idx<cols.Length; idx++) {
				cols[idx][channel_idx] = 1-cols[idx][channel_idx];
			}		
		}
	}	
#endif
	
}
