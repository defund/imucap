using UnityEngine;
using System.Collections;

public enum TerrainShaderLod {
	POM,
	PM,
	SIMPLE,
	CLASSIC
}

public enum RTPLodLevel {
	POM_SoftShadows, POM_HardShadows, POM_NoShadows, PM, SIMPLE
}

public class RTP_LODmanager : MonoBehaviour {
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// LOD level (realtime)
	//
	// set below  variables in your game quality settings
	// and call RTP_LODmanager.RefreshLODlevel()
	//
	public TerrainShaderLod RTP_LODlevel=TerrainShaderLod.POM;
	public bool RTP_SHADOWS=true;
	public bool RTP_SOFT_SHADOWS=true;
	
	public bool RTP_NOFORWARDADD=false;
	public bool RTP_NOLIGHTMAP=false;
	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public bool show_first_features=false;
	public bool show_add_features=false;
	
	// feaures (can be switched on/off, shader files are automatically changed, then recompiled)
	public bool RTP_UV_BLEND_FIRST=true;
	public bool RTP_UV_BLEND_ADD=true;
	public bool RTP_DISTANCE_ONLY_UV_BLEND_FIRST=true;
	public bool RTP_DISTANCE_ONLY_UV_BLEND_ADD=true;
	public bool RTP_SUPER_DETAIL_FIRST=true;
	public bool RTP_SUPER_DETAIL_ADD=true;
	public bool RTP_SUPER_DETAIL_MULTS_FIRST=false;
	public bool RTP_SUPER_DETAIL_MULTS_ADD=false;
	public bool RTP_SNOW_FIRST=false;
	public bool RTP_SNOW_ADD=false;
	public bool RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST=false;
	public bool RTP_SNW_CHOOSEN_LAYER_COLOR_ADD=false;
	public int RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_FIRST=7;
	public int RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_ADD=7;
	public bool RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST=false;
	public bool RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD=false;
	public int RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_FIRST=7;
	public int RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_ADD=7;
	public RTPLodLevel MAX_LOD_FIRST=RTPLodLevel.PM;
	public RTPLodLevel MAX_LOD_FIRST_PLUS4=RTPLodLevel.SIMPLE;
	public RTPLodLevel MAX_LOD_ADD=RTPLodLevel.PM;
	
	public bool RTP_SHOW_OVERLAPPED=false;
	
	public bool RTP_TRIPLANAR_FIRST=false;
	public bool RTP_TRIPLANAR_ADD=false;
	
	public bool RTP_NORMALGLOBAL_FIRST = false;
	public bool RTP_NORMALGLOBAL_ADD = false;
	public bool RTP_TREESGLOBAL_FIRST = false;
	public bool RTP_TREESGLOBAL_ADD = false;
	
	public bool RTP_SUPER_SIMPLE = false;
	
	public bool RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST=false;
	public bool RTP_USE_BUMPMAPS_FIRST=true;
	public bool RTP_USE_PERLIN_FIRST=false;
	//public bool RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD=false;
	public bool RTP_USE_BUMPMAPS_ADD=true;
	public bool RTP_USE_PERLIN_ADD=false;
	
	public bool RTP_COLOR_MAP_BLEND_MULTIPLY_FIRST=true;
	public bool RTP_COLOR_MAP_BLEND_MULTIPLY_ADD=true;
	public bool RTP_SIMPLE_FAR_FIRST=true;
	public bool RTP_SIMPLE_FAR_ADD=true;
	
	public bool RTP_CROSSPASS_HEIGHTBLEND=false;
	public int[] UV_BLEND_ROUTE_NUM_FIRST=new int[8] {0,1,2,3,4,5,6,7};
	public int[] UV_BLEND_ROUTE_NUM_ADD=new int[8] {0,1,2,3,4,5,6,7};

	public bool RTP_HARD_CROSSPASS=true;
	public bool RTP_MAPPED_SHADOWS_FIRST=false;
	public bool RTP_MAPPED_SHADOWS_ADD=false;

	public bool RTP_VERTICAL_TEXTURE_FIRST=false;
	public bool RTP_VERTICAL_TEXTURE_ADD=false;
	
	public bool RTP_ADDITIONAL_FEATURES_IN_FALLBACKS=true;
	
	public bool PLATFORM_D3D9=true;
	public bool PLATFORM_D3D11=true;
	public bool PLATFORM_OPENGL=true;
	public bool PLATFORM_GLES=true;
	public bool PLATFORM_FLASH=true;
	
	public bool RTP_4LAYERS_MODE=false;
	public int numLayers; // uzupelniane przez skrypt RTP
	
	public bool ADDPASS_IN_BLENDBASE=false;
	
	public bool RTP_REFLECTION_FIRST=false;
	public bool RTP_REFLECTION_ADD=false;
	public bool RTP_ROTATE_REFLECTION=false;
	
	public bool RTP_WETNESS_FIRST=false;
	public bool RTP_WETNESS_ADD=false;
	public bool RTP_WET_RIPPLE_TEXTURE_FIRST=false;
	public bool RTP_WET_RIPPLE_TEXTURE_ADD=false;
	public bool RTP_CAUSTICS_FIRST=false;
	public bool RTP_CAUSTICS_ADD=false;
	
	public bool SIMPLE_WATER_FIRST=false;
	public bool SIMPLE_WATER_ADD=false;
	
	public bool RTP_USE_EXTRUDE_REDUCTION_FIRST=false;
	public bool RTP_USE_EXTRUDE_REDUCTION_ADD=false;
	
	public bool SHADER_USAGE_FirstPass=false;
	public bool SHADER_USAGE_AddPass=false;
	public bool SHADER_USAGE_AddPassGeom=false;
	public bool SHADER_USAGE_TerrainFarOnly=false;
	public bool SHADER_USAGE_BlendBase=false;
	
	public bool SHADER_USAGE_Terrain2Geometry=false;
	public bool SHADER_USAGE_Terrain2GeometryBlendBase=false;
	
	private Shader terrain_shader;
	private Shader terrain_shader_far;
	private Shader terrain_shader_add;
	private Shader terrain2geom_shader;
	private Shader terrain_geomBlend_shader;
	private Shader terrain2geom_geomBlend_shader;
	private Shader terrain_geomBlend_GeometryBlend_BumpedDetailSnow;
	private Shader geomblend_GeometryBlend_WaterShader_2VertexPaint_HB;
	private Shader geomBlend_GeometryBlend_WaterShader_FlowMap_HB;
	
	// user shaders that visualise actual geometry blended objects
	private Shader terrain_geomBlendActual_shader;
	
	public bool dont_sync=false;
		
	void Awake() {
		RefreshLODlevel();
	}
	
	public void RefreshLODlevel() {
#if UNITY_3_5
		if (terrain_shader==null) terrain_shader=Shader.Find("Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass");
		if (terrain_shader_add==null) terrain_shader_add=Shader.Find("Hidden/TerrainEngine/Splatmap/Lightmap-AddPass");
#else
		if (terrain_shader==null) terrain_shader=Shader.Find("Relief Pack/ReliefTerrain-FirstPass");
		if (terrain_shader_add==null) terrain_shader_add=Shader.Find("Relief Pack/ReliefTerrain-AddPass");
#endif
		if (terrain_shader_far==null) terrain_shader_far=Shader.Find("Relief Pack/ReliefTerrain-FarOnly");
		if (terrain2geom_shader==null) terrain2geom_shader=Shader.Find("Relief Pack/Terrain2Geometry");
		if (terrain_geomBlend_shader==null) terrain_geomBlend_shader=Shader.Find("Relief Pack/ReliefTerrainGeometryBlendBase");
		if (terrain2geom_geomBlend_shader==null) terrain2geom_geomBlend_shader=Shader.Find("Relief Pack/ReliefTerrain2GeometryBlendBase");
		if (terrain_geomBlend_GeometryBlend_BumpedDetailSnow==null) terrain_geomBlend_GeometryBlend_BumpedDetailSnow=Shader.Find("Relief Pack/GeometryBlend_BumpedDetailSnow");
		if (geomblend_GeometryBlend_WaterShader_2VertexPaint_HB==null) geomblend_GeometryBlend_WaterShader_2VertexPaint_HB = Shader.Find("Relief Pack/GeometryBlend_WaterShader_2VertexPaint_HB");
		if (geomBlend_GeometryBlend_WaterShader_FlowMap_HB==null) geomBlend_GeometryBlend_WaterShader_FlowMap_HB = Shader.Find("Relief Pack/GeometryBlend_WaterShader_FlowMap_HB");
		
		int maxLOD;
		if (RTP_LODlevel==TerrainShaderLod.CLASSIC) {
			maxLOD=100;
		} else {
			maxLOD=700;
			if (RTP_LODlevel==TerrainShaderLod.POM) {
				if (RTP_SHADOWS) {
					if (RTP_SOFT_SHADOWS) {
						Shader.EnableKeyword("RTP_POM_SHADING_HI");
						Shader.DisableKeyword("RTP_POM_SHADING_MED");
						Shader.DisableKeyword("RTP_POM_SHADING_LO");
					} else {
						Shader.EnableKeyword("RTP_POM_SHADING_MED");
						Shader.DisableKeyword("RTP_POM_SHADING_HI");
						Shader.DisableKeyword("RTP_POM_SHADING_LO");
					}
				} else {
					Shader.EnableKeyword("RTP_POM_SHADING_LO");
					Shader.DisableKeyword("RTP_POM_SHADING_MED");
					Shader.DisableKeyword("RTP_POM_SHADING_HI");
				}				
				Shader.DisableKeyword("RTP_PM_SHADING");
				Shader.DisableKeyword("RTP_SIMPLE_SHADING");
			} else if (RTP_LODlevel==TerrainShaderLod.PM) {
				Shader.DisableKeyword("RTP_POM_SHADING_HI");
				Shader.DisableKeyword("RTP_POM_SHADING_MED");
				Shader.DisableKeyword("RTP_POM_SHADING_LO");
				Shader.EnableKeyword("RTP_PM_SHADING");
				Shader.DisableKeyword("RTP_SIMPLE_SHADING");
			} else {
				Shader.DisableKeyword("RTP_POM_SHADING_HI");
				Shader.DisableKeyword("RTP_POM_SHADING_MED");
				Shader.DisableKeyword("RTP_POM_SHADING_LO");
				Shader.DisableKeyword("RTP_PM_SHADING");
				Shader.EnableKeyword("RTP_SIMPLE_SHADING");
			}
		}
		if (terrain_shader !=null) terrain_shader.maximumLOD=maxLOD;
		if (terrain_shader_far !=null) terrain_shader_far.maximumLOD=maxLOD;
		if (terrain_shader_add !=null) terrain_shader_add.maximumLOD=maxLOD;
		if (terrain2geom_shader!=null) terrain2geom_shader.maximumLOD=maxLOD;
		if (terrain_geomBlend_shader!=null) terrain_geomBlend_shader.maximumLOD=maxLOD;
		if (terrain2geom_geomBlend_shader!=null) terrain2geom_geomBlend_shader.maximumLOD=maxLOD;
		if (terrain_geomBlend_GeometryBlend_BumpedDetailSnow!=null) terrain_geomBlend_GeometryBlend_BumpedDetailSnow.maximumLOD=maxLOD;
		if (geomblend_GeometryBlend_WaterShader_2VertexPaint_HB!=null) geomblend_GeometryBlend_WaterShader_2VertexPaint_HB.maximumLOD=maxLOD;
		if (geomBlend_GeometryBlend_WaterShader_FlowMap_HB!=null) geomBlend_GeometryBlend_WaterShader_FlowMap_HB.maximumLOD=maxLOD;
	}
	
}
