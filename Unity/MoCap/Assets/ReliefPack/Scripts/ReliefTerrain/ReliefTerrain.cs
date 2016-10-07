using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
#if UNITY_EDITOR	
using UnityEditor;
#endif

public enum ColorChannels {
	R, G, B, A
}

public enum ReliefTerrainMenuItems {
	Details, Control, CombinedTextures, GeneralSettings
}
public enum ReliefTerrainSettingsItems {
	MainSettings, UVblend, GlobalColor, GlobalNormal, Superdetail, POMSettings, VerticalTex, Snow, Water, Reflections
}
public enum ReliefTerrainDerivedTexturesItems {
	Atlasing, Heightmaps, Bumpmaps, Globalnormal
}
public enum ReliefTerrainControlTexturesItems {
	Compose, Acquire, Controlmaps
}

[AddComponentMenu("Relief Terrain/Engine - Terrain or Mesh")]
[ExecuteInEditMode]
public class ReliefTerrain : MonoBehaviour {
	public Texture2D controlA;
	public Texture2D controlB;
	public Texture2D controlC;
	public string save_path_controlA="";
	public string save_path_controlB="";
	public string save_path_controlC="";
	public string save_path_colormap="";
	public string save_path_BumpGlobalCombined="";
	public string save_path_WetMask="";
	
	public Texture2D NormalGlobal;
	public Texture2D TreesGlobal;
	public Texture2D ColorGlobal;
	public Texture2D BumpGlobalCombined;
	
	public Texture2D TERRAIN_WetMask;

	public Texture2D tmp_globalColorMap;
	public Texture2D tmp_CombinedMap;
	public Texture2D tmp_WaterMap;
	public bool globalColorModifed_flag=false;
	public bool globalCombinedModifed_flag=false;
	public bool globalWaterModifed_flag=false;	
	
	public bool splat_layer_ordered_mode;
	public ColorChannels[] source_controls_channels;
	public int[] splat_layer_seq;
	public float[] splat_layer_boost;
	public bool[] splat_layer_calc;
	public bool[] splat_layer_masked;
	public ColorChannels[] source_controls_mask_channels;
	
	public Texture2D[] source_controls;
	public bool[] source_controls_invert;
	public Texture2D[] source_controls_mask;
	public bool[] source_controls_mask_invert;		
	
	[SerializeField] public ReliefTerrainPresetHolder[] presetHolders;
	
	[SerializeField] public ReliefTerrainGlobalSettingsHolder globalSettingsHolder;
	
#if UNITY_EDITOR	
	[HideInInspector] public static SceneView.OnSceneFunc _SceneGUI;	
#endif

	public void GetGlobalSettingsHolder() {
#if UNITY_EDITOR
		if (globalSettingsHolder!=null) {
			// refresh num tiles in case we've just removed all except for this one
			bool IamTerrain=GetComponent(typeof(Terrain));
			if (IamTerrain) {
				ReliefTerrain[] script_objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
				globalSettingsHolder.numTiles=0;
				for(int p=0; p<script_objs.Length; p++) {
					if (script_objs[p].globalSettingsHolder!=null) {
						if (script_objs[p].globalSettingsHolder!=globalSettingsHolder) {
							//Debug.Log("RTP assert - leaving one globalSettingsHolder...");
							globalSettingsHolder=script_objs[p].globalSettingsHolder;
						}
						if (IamTerrain && script_objs[p].GetComponent(typeof(Terrain))!=null) {
							globalSettingsHolder.numTiles++;
						}
					}
				}	
				if (globalSettingsHolder.numTiles==1 || globalSettingsHolder.useTerrainMaterial) {
					// we don't have to use texture redefinitions
					GetSplatsFromGlobalSettingsHolder();
				}
			}
		}
#endif
		if (globalSettingsHolder==null) {
			//Debug.Log("E"+name);
			ReliefTerrain[] script_objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
			bool IamTerrain=GetComponent(typeof(Terrain));
			for(int p=0; p<script_objs.Length; p++) {
				if (script_objs[p].globalSettingsHolder!=null && ((IamTerrain && script_objs[p].GetComponent(typeof(Terrain))!=null) || (!IamTerrain && script_objs[p].GetComponent(typeof(Terrain))==null))) {
					//Debug.Log ("E2 "+script_objs[p].name);
					globalSettingsHolder=script_objs[p].globalSettingsHolder;
					if (globalSettingsHolder.Get_RTP_LODmanagerScript() && !globalSettingsHolder.Get_RTP_LODmanagerScript().RTP_WETNESS_FIRST && !globalSettingsHolder.Get_RTP_LODmanagerScript().RTP_WETNESS_ADD) {
						BumpGlobalCombined=script_objs[p].BumpGlobalCombined;
						globalCombinedModifed_flag=false;
					}
					break;
				}
			}
			if (globalSettingsHolder==null) {
				// there is no globalSettingsHolder object of my type (terrain/mesh) on the scene - I'm first object
				globalSettingsHolder=new ReliefTerrainGlobalSettingsHolder();
				
				if (IamTerrain) {
					globalSettingsHolder.numTiles=0; // will be set to 1 with incrementation below
					Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
					globalSettingsHolder.splats=new Texture2D[terrainComp.terrainData.splatPrototypes.Length];
					for(int i=0; i<terrainComp.terrainData.splatPrototypes.Length; i++) {
						globalSettingsHolder.splats[i]=terrainComp.terrainData.splatPrototypes[i].texture;
					}
				} else {				
					globalSettingsHolder.splats=new Texture2D[4];
				}
				globalSettingsHolder.numLayers=globalSettingsHolder.splats.Length;
				globalSettingsHolder.ReturnToDefaults();
			} else {
				if (IamTerrain) {
					GetSplatsFromGlobalSettingsHolder();
				}
			}
			
			source_controls_mask=new Texture2D[12];
			source_controls=new Texture2D[12];
			source_controls_channels=new ColorChannels[12];
			source_controls_mask_channels=new ColorChannels[12];
			
			splat_layer_seq=new int[12] {0,1,2,3,4,5,6,7,8,9,10,11};	
			splat_layer_boost=new float[12] {1,1,1,1,1,1,1,1,1,1,1,1};
			splat_layer_calc=new bool[12];
			splat_layer_masked=new bool[12];
			source_controls_invert=new bool[12];
			source_controls_mask_invert=new bool[12];		
			
			if (IamTerrain) globalSettingsHolder.numTiles++;
		}
	}
	
	private void GetSplatsFromGlobalSettingsHolder() {
		SplatPrototype[] splatPrototypes=new SplatPrototype[globalSettingsHolder.numLayers];
		for(int i=0; i<globalSettingsHolder.numLayers; i++) {
			//Debug.Log(""+globalSettingsHolder.splats[i]);
			splatPrototypes[i]=new SplatPrototype();
			splatPrototypes[i].tileSize=new Vector2(3,3);
			splatPrototypes[i].texture=globalSettingsHolder.splats[i];
		}
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		terrainComp.terrainData.splatPrototypes=splatPrototypes;
	}
	
	public void InitTerrainTileSizes() {
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		if (terrainComp) {
			globalSettingsHolder.terrainTileSizeX=terrainComp.terrainData.size.x;
			globalSettingsHolder.terrainTileSizeZ=terrainComp.terrainData.size.z;
		} else {
			globalSettingsHolder.terrainTileSizeX=GetComponent<Renderer>().bounds.size.x;
			globalSettingsHolder.terrainTileSizeZ=GetComponent<Renderer>().bounds.size.z;
		}
	}

	void Awake () {
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		if (terrainComp)  {
		#if UNITY_3_5
			terrainComp.basemapDistance=500000; // to let user not using simple shader (not accessible anyway) at far distance
		#else	
			if (globalSettingsHolder!=null) {
				if (!globalSettingsHolder.useTerrainMaterial) {
					terrainComp.basemapDistance=500000; // to let user not using simple shader (not accessible anyway) at far distance
				} else {
					if (globalSettingsHolder.super_simple_active) {
						terrainComp.basemapDistance=globalSettingsHolder.distance_start_bumpglobal+globalSettingsHolder.distance_transition_bumpglobal;
					} else {
						terrainComp.basemapDistance=globalSettingsHolder.distance_start+globalSettingsHolder.distance_transition;
					}
				}
			}
		#endif		
		}
		RefreshTextures();
		globalSettingsHolder.Refresh();
	}
	
	public void InitArrays() {
		RefreshTextures();
	}
	
	public void RefreshTextures(Material mat=null) { // mat used by geom blend to setup underlying mesh
		GetGlobalSettingsHolder();
		InitTerrainTileSizes();
		if (globalSettingsHolder!=null && BumpGlobalCombined!=null) globalSettingsHolder.BumpGlobalCombinedSize=BumpGlobalCombined.width;
		//Debug.Log ("E"+mat);
		
		// refresh distances
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		if (terrainComp) {
			#if UNITY_3_5
				terrainComp.basemapDistance=500000; // to let user not using simple shader (not accessible anyway) at far distance
			#else	
				if (globalSettingsHolder!=null) {
					if (!globalSettingsHolder.useTerrainMaterial) {
						terrainComp.basemapDistance=500000; // to let user not using simple shader (not accessible anyway) at far distance
						if (terrainComp.materialTemplate!=null) {
							terrainComp.materialTemplate=null;
						}
					} else {
						if (globalSettingsHolder.super_simple_active) {
							terrainComp.basemapDistance=globalSettingsHolder.distance_start_bumpglobal+globalSettingsHolder.distance_transition_bumpglobal;
						} else {
							terrainComp.basemapDistance=globalSettingsHolder.distance_start+globalSettingsHolder.distance_transition;
						}
						if (terrainComp.materialTemplate==null) {
							Material ter_mat;
							Shader ter_shad=Shader.Find("Relief Pack/ReliefTerrain-FirstPass");
							if (ter_shad) {
								ter_mat=new Material(ter_shad);
								ter_mat.name=gameObject.name+" material";
								terrainComp.materialTemplate=ter_mat;
							}
						}
					}
				}
			#endif
			
		}		
		
		#if UNITY_EDITOR	
		if (terrainComp) {
			GetControlMaps();
		}		
		#endif
		
		if (terrainComp && !globalSettingsHolder.useTerrainMaterial && globalSettingsHolder.numTiles>1 && !mat) {
			//
			// local (stored in splat textures)
			//
			SplatPrototype[] s=terrainComp.terrainData.splatPrototypes;
			if (s.Length<4) {
				Debug.Log("RTP must use at least 4 layers !");
				return;
			}
			if (ColorGlobal) s[0].texture=ColorGlobal;
			if (NormalGlobal) s[1].texture=NormalGlobal;
			// splaty w 8 layers mode sa bezuzyteczne, wiec tryb jest nieuzywany kiedy redefiniujemy textury
			//if (!globalSettingsHolder._4LAYERS_SHADER_USED) {
			//	if (controlB) s[2].texture=controlB;
			//} else {
				if (TreesGlobal) s[2].texture=TreesGlobal;
			//}
			if (BumpGlobalCombined) s[3].texture=BumpGlobalCombined;
			
			int add_offset=0;
			if (s.Length>=5 && s.Length<=8 && globalSettingsHolder._4LAYERS_SHADER_USED) {
				add_offset=4;
			} else if (s.Length>8) {
				add_offset=8;
			}
			if (add_offset>0) {
				// add pass present
				if (ColorGlobal) s[0+add_offset].texture=ColorGlobal;
				
				bool crosspass_heightblend=false;
				RTP_LODmanager manager=globalSettingsHolder.Get_RTP_LODmanagerScript();
				if (manager && manager.RTP_CROSSPASS_HEIGHTBLEND) crosspass_heightblend=true;
				if (NormalGlobal) s[1+add_offset].texture=NormalGlobal;
				if (globalSettingsHolder._RTP_LODmanagerScript.RTP_4LAYERS_MODE && crosspass_heightblend) {
					if (controlA) s[2+add_offset].texture=controlA;
				} else {
					if (TreesGlobal) s[2+add_offset].texture=TreesGlobal;
				}
				if (BumpGlobalCombined) s[3+add_offset].texture=BumpGlobalCombined;
			}
			terrainComp.terrainData.splatPrototypes=s;
		} else {
			globalSettingsHolder.use_mat=mat; // mat==null - 1 tile case (all global) or shader is on mesh
			if (!terrainComp && !mat) {
				// RTP shader on mesh (former ReliefTerrain2Geometry)
				if (GetComponent<Renderer>().sharedMaterial==null || GetComponent<Renderer>().sharedMaterial.name!="RTPMaterial") {
					GetComponent<Renderer>().sharedMaterial=new Material(Shader.Find("Relief Pack/Terrain2Geometry"));
					GetComponent<Renderer>().sharedMaterial.name="RTPMaterial";
				}
				globalSettingsHolder.use_mat=GetComponent<Renderer>().sharedMaterial; // local params to mesh material
			}
			#if !UNITY_3_5
				if (terrainComp) {
					if (globalSettingsHolder.useTerrainMaterial) {
						if (terrainComp.materialTemplate!=null) {
							globalSettingsHolder.use_mat=terrainComp.materialTemplate;
						}
					}
				}
			#endif

//			globalSettingsHolder.SetShaderParam("_ColorMapGlobal", ColorGlobal);
//			globalSettingsHolder.SetShaderParam("_NormalMapGlobal", NormalGlobal);
//			globalSettingsHolder.SetShaderParam("_TreesMapGlobal", TreesGlobal);
//			globalSettingsHolder.SetShaderParam("_BumpMapGlobal", BumpGlobalCombined);
			
			globalSettingsHolder.use_mat=null;
		}
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//
		// control maps
		//
		RefreshControlMaps(mat);	
		//
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	}
	
	public void RefreshControlMaps(Material mat=null) {
		globalSettingsHolder.use_mat=mat; // mat==null - 1 tile case (all global) or shader is on mesh
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));		
		if (!terrainComp && !mat) {
			globalSettingsHolder.use_mat=GetComponent<Renderer>().sharedMaterial; // local params to mesh material (sharedMaterial made above if needed)
		}
		#if !UNITY_3_5
			if (terrainComp && !mat) {
				if (globalSettingsHolder.useTerrainMaterial) {
					if (terrainComp.materialTemplate!=null) {
						globalSettingsHolder.use_mat=terrainComp.materialTemplate;
					}
				}
			}
		#endif
		
		globalSettingsHolder.SetShaderParam("_Control1", controlA);
		if (globalSettingsHolder.numLayers>4) {
			if (globalSettingsHolder.Get_RTP_LODmanagerScript() && globalSettingsHolder.Get_RTP_LODmanagerScript().RTP_4LAYERS_MODE && terrainComp && !mat) {
				globalSettingsHolder.SetShaderParam("_Control3", controlB);
				globalSettingsHolder.SetShaderParam("_Control2", controlB); // for FarOnly to work in 4+4 classic mode
			} else {
				globalSettingsHolder.SetShaderParam("_Control2", controlB);
			}
		}
		if (globalSettingsHolder.numLayers>8) {
			globalSettingsHolder.SetShaderParam("_Control3", controlC);
		}
		if (!terrainComp || globalSettingsHolder.useTerrainMaterial || globalSettingsHolder.numTiles<=1 || mat) {
			globalSettingsHolder.SetShaderParam("_ColorMapGlobal", ColorGlobal);
			globalSettingsHolder.SetShaderParam("_NormalMapGlobal", NormalGlobal);
			globalSettingsHolder.SetShaderParam("_TreesMapGlobal", TreesGlobal);
			globalSettingsHolder.SetShaderParam("_BumpMapGlobal", BumpGlobalCombined);		
		}
		globalSettingsHolder.use_mat=null;		
	}
	
	public void GetControlMaps() {
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		if (!terrainComp) {
			Debug.Log("Can't fint terrain component !!!");
			return;
		}
		Type terrainDataType = terrainComp.terrainData.GetType();
		PropertyInfo info = terrainDataType.GetProperty("alphamapTextures", BindingFlags.Instance | BindingFlags.NonPublic);
		if (info!=null) {
			Texture2D[] alphamapTextures=(Texture2D[])info.GetValue(terrainComp.terrainData, null);
			if (alphamapTextures.Length>0) controlA=alphamapTextures[0]; else controlA=null;
			if (alphamapTextures.Length>1) controlB=alphamapTextures[1]; else controlB=null;
			if (alphamapTextures.Length>2) controlC=alphamapTextures[2]; else controlC=null;
		} else{
			Debug.LogError("Can't access alphamapTexture directly...");
		}
	}
	
#if UNITY_EDITOR	
	public void Update() {
		//EditorApplication.playmodeStateChanged=cl;
		if (!Application.isPlaying) {
			if (controlA) {
				RefreshControlMaps();
			}
		}
	}
	
//	void cl() {
//		if (controlA) {
//			RefreshControlMaps(); 
//		}
//	}
	
    void OnApplicationPause(bool pauseStatus) {
		if (controlA) {
			RefreshControlMaps(); 
		}
    }	
	
	public void SetCustomControlMaps() {
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		if (!terrainComp) {
			Debug.Log("Can't fint terrain component !!!");
			return;
		}
		
		if (terrainComp.terrainData.alphamapResolution!=controlA.width) {
			Debug.LogError("Terrain controlmap resolution differs fromrequested control texture...");
			return;
		}
		if (!controlA) return;
		float[,,] splatData=terrainComp.terrainData.GetAlphamaps(0,0,terrainComp.terrainData.alphamapResolution, terrainComp.terrainData.alphamapResolution);
		Color[] cols_control=controlA.GetPixels();
		for(int n=0; n<terrainComp.terrainData.alphamapLayers; n++) {
			int idx=0;
			if (n==4) {
				if (!controlB) return;
				cols_control=controlB.GetPixels();
			} else if (n==8) {
				if (!controlC) return;
				cols_control=controlC.GetPixels();
			}
			int channel_idx=n&3;
			for(int i=0; i<terrainComp.terrainData.alphamapResolution; i++) {
				for(int j=0; j<terrainComp.terrainData.alphamapResolution; j++) {
					splatData[i,j,n]=cols_control[idx++][channel_idx];
				}
			}
		}
		terrainComp.terrainData.SetAlphamaps(0,0, splatData);
	}
	
	public void RecalcControlMaps() {
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		if (!terrainComp) {
			Debug.Log("Can't fint terrain component !!!");
			return;
		}
		
		globalSettingsHolder.RecalcControlMaps(terrainComp, this);
		RefreshTextures();
		globalSettingsHolder.Refresh();
	}
	
	public void RecalcControlMapsForMesh() {
		globalSettingsHolder.RecalcControlMapsForMesh(this);
		RefreshTextures();
		globalSettingsHolder.Refresh();
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
	
	public Texture2D GetSteepnessHeightDirectionTexture(int what=0, GameObject ref_obj=null) {
		Terrain terrainComp = (Terrain)GetComponent(typeof(Terrain));
		if (!terrainComp) {
			Debug.Log("Can't fint terrain component !!!");
			return null;
		}
		Texture2D tex=new Texture2D(terrainComp.terrainData.alphamapResolution, terrainComp.terrainData.alphamapResolution, TextureFormat.RGB24, false);
		//int size=terrainComp.terrainData.alphamapResolution;
		int size=terrainComp.terrainData.heightmapResolution-1;
		Color32[] cols=new Color32[size*size];
		int idx=0;
		float sizef=1.0f/size;
		float[] val_raw_array=new float[size*size];
		if (what==0) {
			// steepness
			for(int j=0; j<size; j++) {
				for(int i=0; i<size; i++) {
					val_raw_array[idx++]=terrainComp.terrainData.GetSteepness(i*sizef, j*sizef);
				}
			}
		} else if (what==1) {
			// height
			for(int j=0; j<size; j++) {
				for(int i=0; i<size; i++) {
					val_raw_array[idx++]=terrainComp.terrainData.GetHeight(i,j);
				}
			}
		} else {
			// direction
			Vector3 ref_dir=Vector3.forward;
			if (ref_obj) {
				ref_dir=ref_obj.transform.forward;
				if (globalSettingsHolder.flat_dir_ref) {
					ref_dir.y=0;
					if (ref_dir.magnitude<0.0000001f) {
						ref_dir=Vector3.forward;
					} else {
						ref_dir.Normalize();
					}
				}
			}
			if (globalSettingsHolder.flip_dir_ref) ref_dir=-ref_dir;
			for(int j=0; j<size; j++) {
				for(int i=0; i<size; i++) {
					Vector3 dir=terrainComp.terrainData.GetInterpolatedNormal(i*sizef, j*sizef);
					if (globalSettingsHolder.flat_dir_ref) {
						dir.y=0;
						if (dir.magnitude<0.0000001f) {
							val_raw_array[idx++]=0;
						} else {
							dir.Normalize();
							val_raw_array[idx++]=Mathf.Clamp01(Vector3.Dot(dir, ref_dir));
						}
					} else {
						val_raw_array[idx++]=Mathf.Clamp01(Vector3.Dot(dir, ref_dir));
					}
				}
			}
		}
		float min=99999;
		float max=0;
		for(idx=0; idx<val_raw_array.Length; idx++) {
			if (val_raw_array[idx]<min) min=val_raw_array[idx];
			if (val_raw_array[idx]>max) max=val_raw_array[idx];
		}
		float norm_val=1.0f/(max-min);
		for(idx=0; idx<val_raw_array.Length; idx++) {
			float val_raw=(val_raw_array[idx]-min)*norm_val;
			byte val=(byte)Mathf.RoundToInt(255*val_raw);
			cols[idx]=new Color32(val,val,val,1);
		}
		tex.SetPixels32(cols);
		tex.Apply(false);
		return tex;
	}
	
	public bool PrepareGlobalNormalsAndSuperDetails(bool wet=false, bool reflection=false) {
		int[] size=new int[3]{0,0,0};
		if (globalSettingsHolder.BumpGlobal)	size[0]=globalSettingsHolder.BumpGlobal.width;
		Texture2D AddChannelA=globalSettingsHolder.SuperDetailA;
		Texture2D AddChannelB=globalSettingsHolder.SuperDetailB;
		if (wet || reflection) {
			AddChannelA=TERRAIN_WetMask;
			AddChannelB=globalSettingsHolder.TERRAIN_ReflectionMap;
		}
		if (AddChannelA) size[1]=AddChannelA.width;
		if (AddChannelB) size[2]=AddChannelB.width;
		for(int i=0; i<3; i++) {
			for(int j=i+1; j<3; j++) {
				if ( (size[i]!=0) && (size[j]!=0) && (size[i]!=size[j]) ) {
					if (wet) {
						EditorUtility.DisplayDialog("Error", "Special combined texture and wet mask texture need the same size (check perlin normal texture size).","OK");
					} else if (reflection) {
						EditorUtility.DisplayDialog("Error", "Special combined texture and reflection map texture need the same size (check perlin normal texture size).","OK");
					} else {
						EditorUtility.DisplayDialog("Error", "Special combined texture and superdetail textures need the same size (check perlin normal texture size).","OK");
					}
					return false;
				}
			}
		}
		if (globalSettingsHolder.BumpGlobal) {
			try { 
				globalSettingsHolder.BumpGlobal.GetPixels(0,0,4,4,0);
			} catch (Exception e) {
				Debug.LogError("Global normalMap has to be marked as isReadable...");
				Debug.LogError(e.Message);
				globalSettingsHolder.activateObject=globalSettingsHolder.BumpGlobal;
				return false;
			}		
		}
		if (AddChannelA) {
			try { 
				AddChannelA.GetPixels(0,0,4,4,0);
			} catch (Exception e) {
				Debug.LogError("Superdetail texture 1 has to be marked as isReadable...");
				Debug.LogError(e.Message);
				globalSettingsHolder.activateObject=AddChannelA;
				return false;
			}		
		}
		if (AddChannelB) {
			try { 
				AddChannelB.GetPixels(0,0,4,4,0);
			} catch (Exception e) {
				Debug.LogError("Superdetail texture 2 has to be marked as isReadable...");
				Debug.LogError(e.Message);
				globalSettingsHolder.activateObject=AddChannelB;
				return false;
			}
		}
		int _size=size[0];
		if (_size==0) _size=size[1];
		if (_size==0) _size=size[2];
		if (_size==0) _size=wet ? 1024 : 256;
		if (BumpGlobalCombined && AssetDatabase.GetAssetPath(BumpGlobalCombined)=="") { DestroyImmediate(BumpGlobalCombined); BumpGlobalCombined=null; };
		BumpGlobalCombined=new Texture2D(_size,_size,TextureFormat.ARGB32,true,true);
		Color32[] norm_cols;
		if (globalSettingsHolder.BumpGlobal) {
			norm_cols=globalSettingsHolder.BumpGlobal.GetPixels32();
		} else {
			norm_cols=new Color32[_size*_size];
			for(int i=0; i<norm_cols.Length; i++) {
				norm_cols[i]=new Color32(128,128,128,128);
			}
		}
		Color32[] det1_cols;
		if (AddChannelA) {
			det1_cols=AddChannelA.GetPixels32();
		} else {
			det1_cols=new Color32[_size*_size];
			Color default_col=wet ? new Color32(0, 0, 0, 0) : new Color32(128,128,128,128);
			for(int i=0; i<norm_cols.Length; i++) {
				det1_cols[i]=default_col;
			}
		}
		Color32[] det2_cols;
		if (AddChannelB) {
			det2_cols=AddChannelB.GetPixels32();
		} else {
			det2_cols=new Color32[_size*_size];
			for(int i=0; i<norm_cols.Length; i++) {
				det2_cols[i]=new Color32(128,128,128,128);
			}
		}
		Color32[] cols=new Color32[_size*_size];
		int det1_channel;
		if (wet || reflection) {
			det1_channel=3;
		} else {
			det1_channel=(int)globalSettingsHolder.SuperDetailA_channel;
		}
		int det2_channel;
		if (reflection) {
			det2_channel=(int)globalSettingsHolder.TERRAIN_ReflectionMap_channel;
		} else {
			det2_channel=(int)globalSettingsHolder.SuperDetailB_channel;
		}
		for(int i=0; i<cols.Length; i++) {
			cols[i]=new Color32(norm_cols[i].a, norm_cols[i].g, 0, 0);
		}
		switch(det1_channel) {
			case 0:
				for(int i=0; i<cols.Length; i++) cols[i].b=det1_cols[i].r;
				break;
			case 1:
				for(int i=0; i<cols.Length; i++) cols[i].b=det1_cols[i].g;
				break;
			case 2:
				for(int i=0; i<cols.Length; i++) cols[i].b=det1_cols[i].b;
				break;
			case 3:
				for(int i=0; i<cols.Length; i++) cols[i].b=det1_cols[i].a;
				break;
		}
		switch(det2_channel) {
			case 0:
				for(int i=0; i<cols.Length; i++) cols[i].a=det2_cols[i].r;
				break;
			case 1:
				for(int i=0; i<cols.Length; i++) cols[i].a=det2_cols[i].g;
				break;
			case 2:
				for(int i=0; i<cols.Length; i++) cols[i].a=det2_cols[i].b;
				break;
			case 3:
				for(int i=0; i<cols.Length; i++) cols[i].a=det2_cols[i].a;
				break;
		}
		BumpGlobalCombined.SetPixels32(cols);
		BumpGlobalCombined.Apply(true, false); // not readable przy publishingu
		BumpGlobalCombined.filterMode=FilterMode.Trilinear;
		
		globalCombinedModifed_flag=true;
		RefreshTextures();

		
		RTP_LODmanager manager=globalSettingsHolder.Get_RTP_LODmanagerScript();
		if (manager && !manager.RTP_WETNESS_FIRST && !manager.RTP_WETNESS_ADD) {
			ReliefTerrain[] objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
			for(int i=0; i<objs.Length; i++) {
				objs[i].BumpGlobalCombined=BumpGlobalCombined;
				objs[i].globalCombinedModifed_flag=true;
				objs[i].RefreshTextures();
			}
		}
		return true;
	}
	
	public int modify_blend(bool upflag) {
		if (globalSettingsHolder.paintHitInfo_flag) {
			if (prepare_tmpTexture(!globalSettingsHolder.paint_wetmask)) {
				int w;
				int h;
				Texture2D tex;
				tex=globalSettingsHolder.paint_wetmask ? tmp_CombinedMap : tmp_globalColorMap;
				w=Mathf.RoundToInt(globalSettingsHolder.paint_size/globalSettingsHolder.terrainTileSizeX * tex.width);
				h=Mathf.RoundToInt(globalSettingsHolder.paint_size/globalSettingsHolder.terrainTileSizeZ * tex.height);
				if (w<1) w=1;
				if (h<1) h=1;
				int _left = Mathf.RoundToInt(globalSettingsHolder.paintHitInfo.textureCoord.x*tex.width-w);
				if (_left<0) _left=0;
				w*=2;
				if (_left+w>=tex.width) _left=tex.width - w;
				int _top = Mathf.RoundToInt(globalSettingsHolder.paintHitInfo.textureCoord.y*tex.height-h);
				if (_top<0) _top=0;
				h*=2;
				if (_top+h>=tex.height) _top=tex.height - h;
				Color[] cols=tex.GetPixels(_left, _top, w, h);
				Color[] cols2=new Color[1];
				if (globalSettingsHolder.paint_wetmask) {
					cols2=tmp_WaterMap.GetPixels(_left, _top, w, h);
				}
				int idx=0;
				float d=upflag ? -1f : 1f;
				float targetBrightness=(globalSettingsHolder.paintColor.r + globalSettingsHolder.paintColor.g + globalSettingsHolder.paintColor.b);
				for(int j=0; j<h; j++) {
					idx=j*w;
					float disty=(2.0f*j/(h-1)-1.0f)*((h-1.0f)/h);
					for(int i=0; i<w; i++) {
						float distx=(2.0f*i/(w-1)-1.0f)*((w-1.0f)/w);
						float dist=1.0f-Mathf.Sqrt(distx*distx+disty*disty);
						if (dist<0) dist=0;
						dist=dist > globalSettingsHolder.paint_smoothness ? 1 : dist/globalSettingsHolder.paint_smoothness;
						if (globalSettingsHolder.paint_wetmask) {
							cols[idx].b += -d*globalSettingsHolder.paint_opacity*dist;
							cols2[idx].a = cols[idx].b;
						} else if (globalSettingsHolder.paint_alpha_flag) {
							cols[idx].a+=d*globalSettingsHolder.paint_opacity*dist;
						} else {
							if (globalSettingsHolder.preserveBrightness) {
								float sourceBrightness=cols[idx].r+cols[idx].g+cols[idx].b;
								float brightnessRatio=sourceBrightness/targetBrightness;
								if (upflag) {
									cols[idx]=Color.Lerp(cols[idx], new Color(brightnessRatio*globalSettingsHolder.paintColor.r, brightnessRatio*globalSettingsHolder.paintColor.g, brightnessRatio*globalSettingsHolder.paintColor.b, cols[idx].a), globalSettingsHolder.paint_opacity*dist);
								} else {
									cols[idx]=Color.Lerp(cols[idx], new Color(brightnessRatio*0.5f, brightnessRatio*0.5f, brightnessRatio*0.5f, cols[idx].a), globalSettingsHolder.paint_opacity*dist);
								}
							} else {
								if (upflag) {
									cols[idx]=Color.Lerp(cols[idx], new Color(globalSettingsHolder.paintColor.r, globalSettingsHolder.paintColor.g, globalSettingsHolder.paintColor.b, cols[idx].a), globalSettingsHolder.paint_opacity*dist);
								} else {
									cols[idx]=Color.Lerp(cols[idx], new Color(0.5f, 0.5f, 0.5f, cols[idx].a), globalSettingsHolder.paint_opacity*dist);
								}
							}
						}
						idx++;
					}
				}
				//Debug.Log (_left+" , "+_top+" , "+w+"  "+h);
				//for(int i=0; i<cols.Length; i++) cols[i]=Color.white;
				tex.SetPixels(_left, _top, w, h, cols);
				tex.Apply(true,false);
				if (globalSettingsHolder.paint_wetmask) {
					tmp_WaterMap.SetPixels(_left, _top, w, h, cols2);
					tmp_WaterMap.Apply(true,false);
				}
			} else {
				return -2;
			}
		}
		return 0;
	}
	
	public bool prepare_tmpTexture(bool color_flag=true) {
		if (color_flag) {
			if (ColorGlobal) {
				Texture2D colorMap=ColorGlobal;
				if (tmp_globalColorMap!=colorMap) {
					AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(colorMap));
					bool sRGBflag=false;
					if (_importer) {
						TextureImporter tex_importer=(TextureImporter)_importer;
						sRGBflag=tex_importer.linearTexture;
						bool reimport_flag=false;
						if (!tex_importer.isReadable) {
							Debug.LogWarning("Texture ("+colorMap.name+") has been reimported as readable.");
							tex_importer.isReadable=true;
							reimport_flag=true;
						}
						if (tex_importer.textureFormat!=TextureImporterFormat.ARGB32) {
							Debug.LogWarning("Texture ("+colorMap.name+") has been reimported as as ARGB32.");
							tex_importer.textureFormat=TextureImporterFormat.ARGB32;
							reimport_flag=true;
						}
						if (reimport_flag) {
							AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(colorMap),  ImportAssetOptions.ForceUpdate);
							ColorGlobal=colorMap;
						}
					}
					try { 
						colorMap.GetPixels(0,0,4,4,0);
					} catch (Exception e) {
						Debug.LogError("Global ColorMap has to be marked as isReadable...");
						Debug.LogError(e.Message);
						return false;
					}
					if (colorMap.format==TextureFormat.Alpha8) {
						tmp_globalColorMap=new Texture2D(colorMap.width, colorMap.height, TextureFormat.Alpha8, true, sRGBflag); 
					} else {
						tmp_globalColorMap=new Texture2D(colorMap.width, colorMap.height, TextureFormat.ARGB32, true, sRGBflag); 
					}
					Color[] cols=colorMap.GetPixels();
					tmp_globalColorMap.SetPixels(cols);
					tmp_globalColorMap.Apply(true,false);
					tmp_globalColorMap.wrapMode=TextureWrapMode.Clamp;
					ColorGlobal=tmp_globalColorMap;
					globalColorModifed_flag=true;
					RefreshTextures();
				}
				return true;
			}
			return false;
		} else {
			if (TERRAIN_WetMask) {
				Texture2D colorMap=TERRAIN_WetMask;
				if (tmp_WaterMap!=colorMap) {
					AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(colorMap));
					if (_importer) {
						TextureImporter tex_importer=(TextureImporter)_importer;
						bool reimport_flag=false;
						if (!tex_importer.isReadable) {
							Debug.LogWarning("Texture ("+colorMap.name+") has been reimported as readable.");
							tex_importer.isReadable=true;
							reimport_flag=true;
						}
						if (tex_importer.textureFormat!=TextureImporterFormat.Alpha8) {
							Debug.LogWarning("Texture ("+colorMap.name+") has been reimported as as Alpha8.");
							tex_importer.grayscaleToAlpha=true;
							tex_importer.textureFormat=TextureImporterFormat.Alpha8;
							reimport_flag=true;
						}
						if (reimport_flag) {
							AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(colorMap),  ImportAssetOptions.ForceUpdate);
							TERRAIN_WetMask=colorMap;
						}
					}
					try { 
						colorMap.GetPixels(0,0,4,4,0);
					} catch (Exception e) {
						Debug.LogError("Wetmask texture has to be marked as isReadable...");
						Debug.LogError(e.Message);
						return false;
					}
					tmp_WaterMap=new Texture2D(colorMap.width, colorMap.height, TextureFormat.Alpha8, true, true);
					Color[] cols=colorMap.GetPixels();
					tmp_WaterMap.SetPixels(cols);
					tmp_WaterMap.Apply(true,false);
					TERRAIN_WetMask=tmp_WaterMap;
					globalWaterModifed_flag=true;
					RefreshTextures();
				}
			} else {
				if (BumpGlobalCombined) {
					TERRAIN_WetMask=new Texture2D(BumpGlobalCombined.width, BumpGlobalCombined.height, TextureFormat.Alpha8, false, true);
				} else if (globalSettingsHolder.TERRAIN_ReflectionMap) {
					TERRAIN_WetMask=new Texture2D(globalSettingsHolder.TERRAIN_ReflectionMap.width, globalSettingsHolder.TERRAIN_ReflectionMap.height, TextureFormat.Alpha8, false, true);
				} else {
					TERRAIN_WetMask=new Texture2D(1024, 1024, TextureFormat.Alpha8, false, true);
				}
				Color32[] cols=new Color32[TERRAIN_WetMask.width*TERRAIN_WetMask.height];
				TERRAIN_WetMask.SetPixels32(cols,0);
				TERRAIN_WetMask.Apply(true,false);
			}
			if (BumpGlobalCombined) {
				Texture2D colorMap=BumpGlobalCombined;
				if (tmp_CombinedMap!=colorMap) {
					AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(colorMap));
					if (_importer) {
						TextureImporter tex_importer=(TextureImporter)_importer;
						bool reimport_flag=false;
						if (!tex_importer.isReadable) {
							Debug.LogWarning("Texture ("+colorMap.name+") has been reimported as readable.");
							tex_importer.isReadable=true;
							reimport_flag=true;
						}
						if (tex_importer.textureFormat!=TextureImporterFormat.ARGB32) {
							Debug.LogWarning("Texture ("+colorMap.name+") has been reimported as as ARGB32.");
							tex_importer.textureFormat=TextureImporterFormat.ARGB32;
							reimport_flag=true;
						}
						if (reimport_flag) {
							AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(colorMap),  ImportAssetOptions.ForceUpdate);
							BumpGlobalCombined=colorMap;
						}
					}
					try { 
						colorMap.GetPixels(0,0,4,4,0);
					} catch (Exception e) {
						Debug.LogError("Special texture (perlin+water/reflection) has to be marked as isReadable...");
						Debug.LogError(e.Message);
						return false;
					}
					tmp_CombinedMap=new Texture2D(colorMap.width, colorMap.height, TextureFormat.ARGB32, true, true);
					Color[] cols=colorMap.GetPixels();
					tmp_CombinedMap.SetPixels(cols);
					tmp_CombinedMap.Apply(true,false);
					tmp_CombinedMap.wrapMode=TextureWrapMode.Repeat;
					BumpGlobalCombined=tmp_CombinedMap;
					globalCombinedModifed_flag=true;
					RefreshTextures();
				}
				return true;
			}
			return false;			
		}
	}
	
	public void RestorePreset(ReliefTerrainPresetHolder holder) {
		controlA=holder.controlA;
		controlB=holder.controlB;
		controlC=holder.controlC;
	
		SetCustomControlMaps();
		
		ColorGlobal=holder.ColorGlobal;
		NormalGlobal=holder.NormalGlobal;
		TreesGlobal=holder.TreesGlobal;
		BumpGlobalCombined=holder.BumpGlobalCombined;
		TERRAIN_WetMask=holder.TERRAIN_WetMask;

		globalColorModifed_flag=holder.globalColorModifed_flag;
		globalCombinedModifed_flag=holder.globalCombinedModifed_flag;
		globalWaterModifed_flag=holder.globalWaterModifed_flag;	
		
		// local textures to splat textures
		RefreshTextures();
		
		// restore global settigns		
		globalSettingsHolder.RestorePreset(holder);
	}
	
	public void SavePreset(ref ReliefTerrainPresetHolder holder) {
		holder.controlA=UnityEngine.Object.Instantiate(controlA) as Texture2D;
		holder.controlB=UnityEngine.Object.Instantiate(controlB) as Texture2D;
		holder.controlC=UnityEngine.Object.Instantiate(controlC) as Texture2D;
		
		holder.ColorGlobal=ColorGlobal;
		holder.NormalGlobal=NormalGlobal;
		holder.TreesGlobal=TreesGlobal;
		holder.BumpGlobalCombined=BumpGlobalCombined;
		holder.TERRAIN_WetMask=TERRAIN_WetMask;
		
		holder.globalColorModifed_flag=globalColorModifed_flag;
		holder.globalCombinedModifed_flag=globalCombinedModifed_flag;
		holder.globalWaterModifed_flag=globalWaterModifed_flag;	
		
		// store global settigns		
		globalSettingsHolder.SavePreset(ref holder);
	}	
#endif

	public ReliefTerrainPresetHolder GetPresetByID(string PresetID) {
		if (presetHolders!=null) {
			for(int i=0; i<presetHolders.Length; i++) {
				if (presetHolders[i].PresetID==PresetID) {
					return presetHolders[i];
				}
			}
		}
		return null;
	}
	
	public ReliefTerrainPresetHolder GetPresetByName(string PresetName) {
		if (presetHolders!=null) {
			for(int i=0; i<presetHolders.Length; i++) {
				if (presetHolders[i].PresetName==PresetName) {
					return presetHolders[i];
				}
			}
		}
		return null;
	}	
	
	public bool InterpolatePresets(string PresetID1, string PresetID2, float t) {
		ReliefTerrainPresetHolder holderA=GetPresetByID(PresetID1);
		ReliefTerrainPresetHolder holderB=GetPresetByID(PresetID2);
		if (holderA==null || holderB==null || holderA.Spec==null || holderB.Spec==null || holderA.Spec.Length!=holderB.Spec.Length) {
			return false;
		}
		globalSettingsHolder.InterpolatePresets(holderA, holderB, t);
		return true;
	}	
}
