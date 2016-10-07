using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using UnityEditor.Callbacks;
#if UNITY_EDITOR	
using System.IO;
#endif

[CustomEditor (typeof(ReliefTerrain))]
public class ReliefTerrainEditor : Editor {
#if UNITY_EDITOR	
	int progress_count_max;
	int progress_count_current;
	const int progress_granulation=1;
	string progress_description="";
	
	private bool dirtyFlag;
	private double changedTime=0;
	public static Texture2D blank_white_tex;
	public static Texture2D blank_black_tex;
	
	private double lCovTim=0;
	private bool control_down_flag=false;
	private RaycastHit paintHitInfo;
	private bool paintHitInfo_flag;
	private Tool prev_tool;
	
	private Texture icoPaintOn;
	private Texture icoPaintOff;
	private Texture icoMicroArrow;
	private Texture icoLayers;
	private Texture icoCoverage;
	private Texture icoCombinedTexutres;
	private Texture icoSettings;
	
	private Texture icoPOM;
	private Texture icoLayersSmall;
	private Texture icoUVBlend;
	private Texture icoPerlinNormal;
	private Texture icoGlobalcolor;
	private Texture icoSuperdetail;
	private Texture icoWater;
	private Texture icoSnow;
	private Texture icoVerticalTexture;
	private Texture icoReflection;
	
	void OnEnable() {
		if (icoPaintOn==null) icoPaintOn=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoPaintOn.png", typeof(Texture)) as Texture;
		if (icoPaintOff==null) icoPaintOff=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoPaintOff.png", typeof(Texture)) as Texture;
		if (icoMicroArrow==null) icoMicroArrow=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoMicroArrow.png", typeof(Texture)) as Texture;
		if (icoLayers==null) icoLayers=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoLayers.png", typeof(Texture)) as Texture;
		if (icoCoverage==null) icoCoverage=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoCoverage.png", typeof(Texture)) as Texture;
		if (icoCombinedTexutres==null) icoCombinedTexutres=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoCombinedTextures.png", typeof(Texture)) as Texture;
		if (icoSettings==null) icoSettings=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoSettings.png", typeof(Texture)) as Texture;
		
		if (icoPOM==null) icoPOM=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoPOM.png", typeof(Texture)) as Texture;
		if (icoLayersSmall==null) icoLayersSmall=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoLayersSmall.png", typeof(Texture)) as Texture;
		if (icoUVBlend==null) icoUVBlend=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoUVBlend.png", typeof(Texture)) as Texture;
		if (icoGlobalcolor==null) icoGlobalcolor=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoGlobalcolor.png", typeof(Texture)) as Texture;
		if (icoPerlinNormal==null) icoPerlinNormal=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoPerlinNormal.png", typeof(Texture)) as Texture;
		if (icoSuperdetail==null) icoSuperdetail=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoSuperdetail.png", typeof(Texture)) as Texture;
		if (icoWater==null) icoWater=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoWater.png", typeof(Texture)) as Texture;
		if (icoSnow==null) icoSnow=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoSnow.png", typeof(Texture)) as Texture;
		if (icoVerticalTexture==null) icoVerticalTexture=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoVerticalTexture.png", typeof(Texture)) as Texture;
		if (icoReflection==null) icoReflection=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoReflection.png", typeof(Texture)) as Texture;
		
		ReliefTerrain _target=(ReliefTerrain)target;
		_target.RefreshTextures();
		if (_target.globalSettingsHolder.BumpGlobal && _target.BumpGlobalCombined) {
			TextureImporter _importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.globalSettingsHolder.BumpGlobal));
			if (_importer) {
				if (_importer.maxTextureSize!=_target.BumpGlobalCombined.width) {
					Debug.LogWarning("Due to rescaling special combined texture superdetail textures, water mask and reflection map have been cleared (you have to reassign them again).");
					_target.TERRAIN_WetMask=null;
					_target.globalSettingsHolder.TERRAIN_ReflectionMap=null;
					_target.globalSettingsHolder.SuperDetailA=null;
					_target.globalSettingsHolder.SuperDetailB=null;
					_target.PrepareGlobalNormalsAndSuperDetails();
				}
			}
		}			
		prev_tool=Tools.current;
	}
	
	void OnDisable() {
		ReliefTerrain _target=(ReliefTerrain)target;
		if (_target) {
			_target.RefreshTextures();			
			if (_target.globalSettingsHolder.paint_flag) {
				 _target.globalSettingsHolder.paint_flag=false;
				SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;
			};
			EditorUtility.SetDirty(target);
		}
		Tools.current=prev_tool;
	}	
	
	public override void OnInspectorGUI () {
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
		if (_target==null) {
			_targetRT.RefreshTextures();
			_target=_targetRT.globalSettingsHolder;
		}
		dirtyFlag=false;
		
		RTP_LODmanager _RTP_LODmanagerScript = _target.Get_RTP_LODmanagerScript();
		
		Terrain terrainComp = (Terrain)_targetRT.GetComponent(typeof(Terrain));
		if (terrainComp) {
			if (_target.numLayers!=terrainComp.terrainData.splatPrototypes.Length) {
				// a layer removed or added
				_target.ReInit(terrainComp);
				ReliefTerrain[] script_objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
				for(int s=0; s<script_objs.Length; s++) {
					script_objs[s].splat_layer_seq=new int[12] {0,1,2,3,4,5,6,7,8,9,10,11};
				}				
			}
			if (!_target.useTerrainMaterial && _target.numTiles>1) {
				// U3 - multiple terrains or U4 w/o material - multiple terrains
				if (_target.numLayers!=4 && _target.numLayers!=8) {
					EditorGUILayout.HelpBox("RTP on multiple terrains w/o materials (available in U4) works only if we have 4 or 8 layers only !", MessageType.Error, true);
					return;
				}
			}
		}
		if (_target.splats==null || _target.numLayers!=_target.splats.Length) {
			EditorGUILayout.HelpBox("Problem with number of layers, try to reassign ReliefTerrain component script on this terrain...", MessageType.Error, true);
			return;
		}
		int sw=512;
		for(int i=0; i<_target.numLayers; i++) {
			if (_target.splats[i]) {
				sw=_target.splats[i].width;
				break;
			}
		}
		for(int i=0; i<_target.numLayers; i++) {
			if (_target.splats[i]==null) {
				Debug.LogError("Splat (detail) texture "+i+" was empty ! Assigning null texture");
				_target.splats[i]=new Texture2D(sw, sw, TextureFormat.ARGB32, true, QualitySettings.activeColorSpace==ColorSpace.Linear);
			}
		}		
			
		if (_RTP_LODmanagerScript) {
			if (!_RTP_LODmanagerScript.RTP_4LAYERS_MODE) {
				if (_target.numLayers<=4) {
					EditorGUILayout.HelpBox("Shaders are set to 8 layers mode while number of layers is less than 5. Add layers or recompile shaders in RTP_LODmanager to not use 8 layers in first pass.", MessageType.Error, true);
				} else {
					if (_target.splat_atlases[0]==null) {
						PrepareAtlases(1);
					}
					if (_target.splat_atlases[1]==null) {
						PrepareAtlases(2);
					}
				}
			}
		}
		if (_RTP_LODmanagerScript) _RTP_LODmanagerScript.numLayers=_target.numLayers;
			
		bool _4LAYERS_SHADER_USED=false;
		if (_RTP_LODmanagerScript) {
			_target._4LAYERS_SHADER_USED=_4LAYERS_SHADER_USED=_RTP_LODmanagerScript.RTP_4LAYERS_MODE;
		}
		
		bool UV_BLEND_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_UV_BLEND_FIRST;
		bool UV_BLEND_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_UV_BLEND_ADD;
		int[] UV_BLEND_ROUTE_NUM_FIRST;
		int[] UV_BLEND_ROUTE_NUM_ADD;
		if (_RTP_LODmanagerScript) {
			UV_BLEND_ROUTE_NUM_FIRST=_RTP_LODmanagerScript.UV_BLEND_ROUTE_NUM_FIRST;
			UV_BLEND_ROUTE_NUM_ADD=_RTP_LODmanagerScript.UV_BLEND_ROUTE_NUM_ADD;
		} else {
			UV_BLEND_ROUTE_NUM_FIRST=new int[8]{0,1,2,3,4,5,6,7};
			UV_BLEND_ROUTE_NUM_ADD=new int[8]{0,1,2,3,4,5,6,7};
		}
		
		bool RTP_USE_EXTRUDE_REDUCTION_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_USE_EXTRUDE_REDUCTION_FIRST;
		bool RTP_USE_EXTRUDE_REDUCTION_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_USE_EXTRUDE_REDUCTION_ADD;
		
		bool RTP_VERTICAL_TEXTURE_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_VERTICAL_TEXTURE_FIRST;
		bool RTP_VERTICAL_TEXTURE_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_VERTICAL_TEXTURE_ADD;
		
		bool RTP_NORMALGLOBAL_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_NORMALGLOBAL_FIRST;
		bool RTP_NORMALGLOBAL_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_NORMALGLOBAL_ADD;
		bool RTP_TREESGLOBAL_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_TREESGLOBAL_FIRST;
		bool RTP_TREESGLOBAL_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_TREESGLOBAL_ADD;
		
		bool RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SUPER_SIMPLE  && _RTP_LODmanagerScript.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST;
		//bool RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SUPER_SIMPLE  && _RTP_LODmanagerScript.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD;
			
		bool SUPER_DETAIL_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SUPER_DETAIL_FIRST;
		bool SUPER_DETAIL_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SUPER_DETAIL_ADD;
		bool SUPER_DETAIL_MULTS_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SUPER_DETAIL_MULTS_FIRST;
		bool SUPER_DETAIL_MULTS_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SUPER_DETAIL_MULTS_ADD;
		
		bool SNOW_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SNOW_FIRST;
		bool SNOW_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_SNOW_ADD;
		
		bool REFLECTION_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_REFLECTION_FIRST;
		bool REFLECTION_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_REFLECTION_ADD;
		bool REFLECTION_ROTATION_ENABLED = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_ROTATE_REFLECTION;
		
		bool WATER_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_WETNESS_FIRST;
		bool WATER_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_WETNESS_ADD;
		bool RIPPLEMAP_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_WET_RIPPLE_TEXTURE_FIRST;
		bool RIPPLEMAP_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_WET_RIPPLE_TEXTURE_ADD;
		
		bool CAUSTICS_ENABLED_FIRST = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_CAUSTICS_FIRST;
		bool CAUSTICS_ENABLED_ADD = _RTP_LODmanagerScript && _RTP_LODmanagerScript.RTP_CAUSTICS_ADD;
		
			
		string[] toolbarStrings;
		GUIContent[] toolbarIcons = new GUIContent[4]{ new GUIContent("Layers", icoLayers), new GUIContent("Coverage", icoCoverage), new GUIContent("Combined\ntexures", icoCombinedTexutres), new GUIContent("Settings", icoSettings) };
		GUILayout.Space(10);			
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		_target.submenu=(ReliefTerrainMenuItems)(GUILayout.Toolbar((int)_target.submenu, toolbarIcons, EditorStyles.miniButton, GUILayout.MaxWidth(370)));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10);			
		
		_target.show_details = _target.submenu==ReliefTerrainMenuItems.Details;
		_target.show_controlmaps = _target.submenu==ReliefTerrainMenuItems.Control;
		_target.show_derivedmaps = _target.submenu==ReliefTerrainMenuItems.CombinedTextures;
		_target.show_settings = _target.submenu==ReliefTerrainMenuItems.GeneralSettings;
		
		if (_target.show_active_layer>=_target.numLayers) {
			_target.show_active_layer=_target.numLayers-1;
		}			
		//Debug.Log (_target.show_active_layer+","+_target.numLayers);
		
		if (_target.show_details) {
			#region Detail maps unfold

			if (_target.numLayers>0) {
				#region Detail maps unfold	- layers
				
				if (!terrainComp) {
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup(_target.numLayers==8);
					if (GUILayout.Button("Add layer")) {
						_target.numLayers++;
						int j=_target.numLayers-1;
						
						ReliefTerrain[] script_objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
						for(int s=0; s<script_objs.Length; s++) {
							script_objs[s].splat_layer_seq[j]=j;
							script_objs[s].splat_layer_boost[j]=1;
							script_objs[s].splat_layer_calc[j]=true;
							script_objs[s].splat_layer_masked[j]=false;
							script_objs[s].source_controls_invert[j]=false;
							script_objs[s].source_controls_mask_invert[j]=false;
						}
						
						_target.Bumps[j]=null;
						_target.Heights[j]=null;
						_target.Substances[j]=null;						
						
						_target.ReturnToDefaults("layer", j);
						
						Texture2D[] splats_new=new Texture2D[_target.numLayers];
						for(int i=0; i<_target.splats.Length; i++) splats_new[i]=_target.splats[i];
						_target.splats=splats_new;
						_target.splats[_target.numLayers-1]=_target.splats[((_target.numLayers-2) >=0) ? (_target.numLayers-2) : 0];
					}
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(_target.numLayers==1);
					if (GUILayout.Button("Remove layer")) {
						_target.numLayers--;
						Texture2D[] splats_new=new Texture2D[_target.numLayers];
						for(int i=0; i<splats_new.Length; i++) splats_new[i]=_target.splats[i];
						_target.splats=splats_new;
						ReliefTerrain[] script_objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
						for(int s=0; s<script_objs.Length; s++) {
							script_objs[s].splat_layer_seq=new int[12] {0,1,2,3,4,5,6,7,8,9,10,11};
						}
						if (_target.show_active_layer>=_target.numLayers) {
							_target.show_active_layer=_target.numLayers-1;
						}								
					}
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
				}
				
				bool handle_substances=false;
				#if UNITY_4_1 || UNITY_4_2 || UNITY_4_3  || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6
					handle_substances=true;
				#else
					if (PlayerSettings.advancedLicense) {
							handle_substances=true;
					}
				#endif
			
				EditorGUILayout.BeginVertical("Box");
				Color skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Choose layer", EditorStyles.boldLabel);
				GUI.color=skin_color;
				EditorGUILayout.HelpBox("Hint: to quickly select a layer - focus on scene view (by click) and press L holding cursor over the layer that should be selected.", MessageType.Info, true);
				
				GUISkin gs=EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
				RectOffset ro1=gs.label.padding;
				RectOffset ro2=gs.label.margin;
				gs.label.padding=new  RectOffset(0, 0, 0, 0);
				gs.label.margin=new  RectOffset(3, 3, 3, 3);
				int thumb_size=50;
				int per_row=Mathf.Max( 4, (Screen.width)/thumb_size - 1 );
				thumb_size=(Screen.width-50-2*per_row)/per_row;
				Color ccol=GUI.contentColor;
				for(int n=0; n<_target.numLayers; n++) {
					if ((n%per_row)==0) EditorGUILayout.BeginHorizontal();
					Color bcol=GUI.backgroundColor;
					if (n==_target.show_active_layer) {
						GUI.contentColor=new Color(1,1,1,1);
						GUI.backgroundColor=new Color(1,1,0,1);
						EditorGUILayout.BeginHorizontal("Box");
						if (_target.splats[n]) {
#if !UNITY_3_5
						GUILayout.Label((Texture2D)AssetPreview.GetAssetPreview(_target.splats[n]), GUILayout.Width(thumb_size-8), GUILayout.Height(thumb_size-8));
#else
						GUILayout.Label((Texture2D)EditorUtility.GetAssetPreview(_target.splats[n]), GUILayout.Width(thumb_size-8), GUILayout.Height(thumb_size-8));
#endif
						} else {
							GUILayout.Label(" ", GUILayout.Width(thumb_size-8), GUILayout.Height(thumb_size-8));
						}
					} else {
						GUI.contentColor=new Color(1,1,1,0.5f);
						if (_target.splats[n]) {
#if !UNITY_3_5
						if (GUILayout.Button((Texture2D)AssetPreview.GetAssetPreview(_target.splats[n]), "Label", GUILayout.Width(thumb_size), GUILayout.Height(thumb_size))) {
#else
						if (GUILayout.Button((Texture2D)EditorUtility.GetAssetPreview(_target.splats[n]), "Label", GUILayout.Width(thumb_size), GUILayout.Height(thumb_size))) {
#endif
							_target.show_active_layer=n;
						}
						} else {
							if (GUILayout.Button(" ", "Label", GUILayout.Width(thumb_size), GUILayout.Height(thumb_size))) {
								_target.show_active_layer=n;
							}
						}
					}
					if (n==_target.show_active_layer) {
						EditorGUILayout.EndHorizontal();
						GUI.backgroundColor=bcol;
					}
					if ((n%per_row)==(per_row-1) || n==_target.numLayers-1) EditorGUILayout.EndHorizontal();
				}
				GUI.contentColor=ccol;
				gs.label.padding=ro1;
				gs.label.margin=ro2;
				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
				
				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Layer properties", EditorStyles.boldLabel);
				GUI.color=skin_color;
				EditorGUILayout.HelpBox("All layers should share THE SAME size for given texture type\n(i.e. all detail textures should have the same size and so on). ", MessageType.Info, true);
				EditorGUILayout.Space();
				{
					int thW=(Screen.width-60)/3;
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Detail", EditorStyles.boldLabel, GUILayout.Width(thW));
					EditorGUILayout.LabelField("Normal", EditorStyles.boldLabel, GUILayout.Width(thW));
					EditorGUILayout.LabelField("Height (A)", EditorStyles.boldLabel, GUILayout.Width(thW));
					EditorGUILayout.EndHorizontal();
				}
				
				{
					int n=_target.show_active_layer;
					EditorGUILayout.BeginHorizontal();
					
					int thW=(Screen.width-60)/3;
					//Debug.Log (_target.splats.Length+"  "+_target.numLayers);
					Texture2D tex=_target.splats[n];
					Texture2D ntex=tex;
					checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_target.splats[n], typeof(Texture2D), false, GUILayout.Width(thW), GUILayout.Height(thW)));
					if (ntex!=tex && ntex) {
						_target.splats[n]=ntex;
						if (_target.numLayers>4 && (!_4LAYERS_SHADER_USED) && (n<8)) {
							if (EditorUtility.DisplayDialog("RTP Notification", "In 8 layers mode you have to recalc atlases to see changes.", "OK, do it now", "Thanks, I'll do it later")) {
								PrepareAtlases((n<4) ? 1:2);
							}
						}
						_target.Refresh();
					}
						
					tex=_target.Bumps[n];
					ntex=tex;
					checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_target.Bumps[n], typeof(Texture2D), false, GUILayout.Width(thW), GUILayout.Height(thW)));
					if (ntex!=tex) {
						Texture2D tex_prev=_target.Bumps[n];
						_target.Bumps[n]=ntex;
						AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
						if (_importer) {
							TextureImporter tex_importer=(TextureImporter)_importer;
							bool changed=false;
							if (!tex_importer.isReadable) {
								Debug.LogWarning("Normal texture "+n+" ("+ntex.name+") has been reimported as readable.");
								tex_importer.isReadable=true;
								changed=true;
							}
							if (!tex_importer.normalmap) {
								Debug.LogWarning("Normal texture "+n+" ("+ntex.name+") has been reimported as normal map type.");
								tex_importer.normalmap=true;
								changed=true;
							}
							if (changed) {
								AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
							}
						}							
						if (!_target.PrepareNormals()) _target.Bumps[n]=tex_prev;
						_target.Refresh();
					}
					
					tex=_target.Heights[n];
					ntex=tex;
					checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_target.Heights[n], typeof(Texture2D), false, GUILayout.Width(thW), GUILayout.Height(thW)));
					if (ntex!=tex) {
						Texture2D tex_prev=_target.Heights[n];
						_target.Heights[n]=ntex;
						AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
						if (_importer) {
							TextureImporter tex_importer=(TextureImporter)_importer;
							bool changed=false;
							if (!tex_importer.isReadable) {
								Debug.LogWarning("Height texture "+n+" ("+ntex.name+") has been reimported as readable.");
								tex_importer.isReadable=true;
								changed=true;
							}
							if (!tex_importer.linearTexture) {
								Debug.LogWarning("Height texture "+n+" ("+ntex.name+") has been reimported as linear.");
								tex_importer.linearTexture=true;
								changed=true;
							}
							if (changed) {
								AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
							}
						}
						if (!PrepareHeights(n)) _target.Heights[n]=tex_prev;
						_target.Refresh();
					}
					EditorGUILayout.EndHorizontal();

					if (handle_substances) {
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Substance", EditorStyles.boldLabel, GUILayout.MaxWidth(75));
							
							ProceduralMaterial subs=_target.Substances[n];
							ProceduralMaterial nsubs=subs;
							checkChange(ref nsubs, (ProceduralMaterial)EditorGUILayout.ObjectField(_target.Substances[n], typeof(ProceduralMaterial), false));
							if (nsubs!=subs) {
								_target.Substances[n]=nsubs;
								#if UNITY_4_1 || UNITY_4_2 || UNITY_4_3  || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6									
								if (nsubs) {
									_target.Substances[n].isReadable=true;
									SubstanceImporter s_importer=(SubstanceImporter)SubstanceImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.Substances[n]));
									if (_target.Substances[n].shader.name!="Parallax Specular") {
										_target.Substances[n].shader=Shader.Find("Parallax Specular");
										s_importer.OnShaderModified(_target.Substances[n]);
									}
									_target.Substances[n].RebuildTexturesImmediately();
									GetSubstanceTex("_Diffuse","_MainTex", n);		
									GetSubstanceTex("_Normal","_BumpMap", n, true);	
									GetSubstanceTex("_Height","_ParallaxMap", n);
									
								}
								#endif
							}
						EditorGUILayout.EndHorizontal();
						EditorGUI.BeginDisabledGroup(_target.Substances[n]==null || _target.Substances[n].isProcessing);
						if (GUILayout.Button("Get textures")) {
							
							#if UNITY_4_1 || UNITY_4_2 || UNITY_4_3  || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6									
								_target.Substances[n].isReadable=true;
							#endif
							SubstanceImporter s_importer=(SubstanceImporter)SubstanceImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.Substances[n]));
							if (_target.Substances[n].shader.name!="Parallax Specular") {
								_target.Substances[n].shader=Shader.Find("Parallax Specular");
								s_importer.OnShaderModified(_target.Substances[n]);
							}
							_target.Substances[n].RebuildTexturesImmediately();
					
							AssetImporter _importer;
							string orig_path;
							string path="";
							bool override_flag;
							byte[] bytes;
							int option;
							
							_importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.splats[n]));
							if (_importer) {
								orig_path=AssetDatabase.GetAssetPath(_target.splats[n]);
								orig_path=orig_path.Substring(0,orig_path.LastIndexOf("/")+1);
							} else {
								orig_path=AssetDatabase.GetAssetPath(_target.Substances[n])+"/";
							}
							
							//
							// diffuse								
							//
							ntex=GetSubstanceTex("_Diffuse","_MainTex", n);		

							path=orig_path+_target.Substances[n].name+"_diffuse_spec"+".png";
							override_flag=AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D))!=null;
							if (override_flag) {
								option=EditorUtility.DisplayDialogComplex("RTP Notification", "Override color/diffuse texture ?", "OK", "Make copy", "Skip texture");
							} else {
								option=0;
							}
							if (option==1) {
								do {
									int suffix_pos=path.LastIndexOf(".");
									path=path.Substring(0,suffix_pos)+"_copy"+path.Substring(suffix_pos);
								} while(AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D))!=null);
							}
							if (option<2) {									
						 		bytes = ntex.EncodeToPNG();
								DestroyImmediate(ntex);
								ntex=null;
							    System.IO.File.WriteAllBytes(path, bytes);
								AssetDatabase.Refresh();
								_importer=AssetImporter.GetAtPath(path);
								if (_importer) {
									TextureImporter tex_importer=(TextureImporter)_importer;
									tex_importer.isReadable=true;
									tex_importer.maxTextureSize=_target.splats[n].width;
									AssetDatabase.ImportAsset(path,  ImportAssetOptions.ForceUpdate);
									ntex=(Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
									if (_target.numLayers>4 && (!_4LAYERS_SHADER_USED) && (n<8)) {
										if (EditorUtility.DisplayDialog("RTP Notification", "In 8 layers mode you have to recalc atlases to see changes.", "OK, do it now", "Thanks, I'll do it later")) {
											PrepareAtlases((n<4) ? 1:2);
										}
									}
								}				
							}
							
							//
							// normal						
							//
							ntex=GetSubstanceTex("_Normal","_BumpMap", n, true);	
							int nwidth=ntex.width;
							path=orig_path+_target.Substances[n].name+"_normal"+".png";
							override_flag=AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D))!=null;
							if (override_flag) {
								option=EditorUtility.DisplayDialogComplex("RTP Notification", "Override normal texture ?", "OK", "Make copy", "Skip texture");
							} else {
								option=0;
							}
							if (option==1) {
								do {
									int suffix_pos=path.LastIndexOf(".");
									path=path.Substring(0,suffix_pos)+"_copy"+path.Substring(suffix_pos);
								} while(AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D))!=null);
							}
							if (option<2) {
						 		bytes = ntex.EncodeToPNG();
								DestroyImmediate(ntex);
								ntex=null;
							    System.IO.File.WriteAllBytes(path, bytes);
								AssetDatabase.Refresh();
								_importer=AssetImporter.GetAtPath(path);
								if (_importer) {
									Texture2D tex_prev=_target.Bumps[n];
									TextureImporter tex_importer=(TextureImporter)_importer;
									tex_importer.isReadable=true;
									int compare_n;
									if ((n&1)==0) {
										if ((n+1)<_target.numLayers) {
											compare_n=n+1;
										} else {
											compare_n=-1;
										}
									} else {
										compare_n=n-1;
									}
									if (compare_n>=0 && _target.Bumps[compare_n]!=null) {
										if (nwidth<_target.Bumps[compare_n].width) {
											TextureImporter tex_importer_tmp=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.Bumps[compare_n].width));
											tex_importer_tmp.maxTextureSize=nwidth;
											Debug.LogWarning("Normal texture "+compare_n+" ("+_target.splats[compare_n].name+") has been reimported with "+tex_importer_tmp.maxTextureSize+" size.");
											AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_target.Bumps[compare_n].width),  ImportAssetOptions.ForceUpdate);
										} else if (nwidth>_target.Bumps[compare_n].width) {
											tex_importer.maxTextureSize=_target.Bumps[compare_n].width;
											Debug.LogWarning("Normal texture "+n+" ("+_target.splats[n].name+") has been imported with "+tex_importer.maxTextureSize+" size.");
										}
									}
									tex_importer.textureType=TextureImporterType.Bump;
									AssetDatabase.ImportAsset(path,  ImportAssetOptions.ForceUpdate);
									ntex=(Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
									_target.Bumps[n]=ntex;
									if (!_target.PrepareNormals()) _target.Bumps[n]=tex_prev;
									_target.Refresh();
								}
							}
							
							//
							// height				
							//
							ntex=GetSubstanceTex("_Height","_ParallaxMap", n);

							path=orig_path+_target.Substances[n].name+"_height"+".png";
							override_flag=AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D))!=null;
							if (override_flag) {
								option=EditorUtility.DisplayDialogComplex("RTP Notification", "Override height texture ?", "OK", "Make copy", "Skip texture");
							} else {
								option=0;
							}
							if (option==1) {
								do {
									int suffix_pos=path.LastIndexOf(".");
									path=path.Substring(0,suffix_pos)+"_copy"+path.Substring(suffix_pos);
								} while(AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D))!=null);
							}
							if (option<2) {
						 		bytes = ntex.EncodeToPNG();
								DestroyImmediate(ntex);
								ntex=null;
							    System.IO.File.WriteAllBytes(path, bytes);
								AssetDatabase.Refresh();
								_importer=AssetImporter.GetAtPath(path);
								if (_importer) {
									Texture2D tex_prev=_target.Heights[n];
									TextureImporter tex_importer=(TextureImporter)_importer;
									tex_importer.isReadable=true;
									tex_importer.linearTexture=true;
									tex_importer.textureFormat=TextureImporterFormat.Alpha8;
									AssetDatabase.ImportAsset(path,  ImportAssetOptions.ForceUpdate);
									ntex=(Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
									_target.Heights[n]=ntex;
									if (!PrepareHeights(n)) _target.Heights[n]=tex_prev;
									_target.Refresh();
								}						
							}
						
						}
							
						EditorGUI.EndDisabledGroup();
					} // handle substances
					
					GUILayout.Space(10);
					
					bool AddPass_flag=(_4LAYERS_SHADER_USED && n>=4) || (n>=8);
						
					EditorGUILayout.BeginVertical("Box");
					float nval;
					nval=_target.Spec[n];
					checkChange(ref nval, EditorGUILayout.Slider("Layer "+n+" specular", _target.Spec[n], 0.01f, 1f, GUILayout.ExpandWidth(true)));
					_target.Spec[n]=nval;
					nval=_target.FarGlossCorrection[n];
					checkChange(ref nval, EditorGUILayout.Slider("Far gloss correction", _target.FarGlossCorrection[n], -1f, 1f, GUILayout.ExpandWidth(true)));
					_target.FarGlossCorrection[n]=nval;
					EditorGUI.BeginDisabledGroup( !(AddPass_flag ? RTP_USE_EXTRUDE_REDUCTION_ADD : RTP_USE_EXTRUDE_REDUCTION_FIRST) );
					nval=_target.PER_LAYER_HEIGHT_MODIFIER[n];
					checkChange(ref nval, EditorGUILayout.Slider("Extrude reduction", _target.PER_LAYER_HEIGHT_MODIFIER[n], 0.0f, 1f, GUILayout.ExpandWidth(true)));
					_target.PER_LAYER_HEIGHT_MODIFIER[n]=nval;
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.BeginHorizontal();	
					EditorGUILayout.LabelField("Far filtering", GUILayout.Width(127));
					nval=_target.MIPmult[n];
					checkChange(ref nval, EditorGUILayout.Slider(_target.MIPmult[n], 0f, 7f, GUILayout.ExpandWidth(true)));
					_target.MIPmult[n]=nval;
					EditorGUILayout.EndHorizontal();	
					EditorGUILayout.BeginHorizontal();	
					EditorGUILayout.LabelField("Heightblend AO", GUILayout.Width(127));
					nval=_target.AO_strength[n];
					checkChange(ref nval, EditorGUILayout.Slider(_target.AO_strength[n], 0f, 1f, GUILayout.ExpandWidth(true)));
					_target.AO_strength[n]=nval;
					EditorGUILayout.EndHorizontal();	
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.BeginHorizontal();	
					if (GUILayout.Button(new GUIContent(icoGlobalcolor, "Global maps settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
						_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
						_target.submenu_settings=ReliefTerrainSettingsItems.GlobalColor;
					}
					EditorGUILayout.LabelField("Global colormap", GUILayout.Width(130));
					nval=_target.GlobalColorPerLayer[n];
					checkChange(ref nval, EditorGUILayout.Slider(_target.GlobalColorPerLayer[n], 0f, 1f, GUILayout.ExpandWidth(true)));
					_target.GlobalColorPerLayer[n]=nval;
					EditorGUILayout.EndHorizontal();	
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.BeginVertical("Box");
					EditorGUI.BeginDisabledGroup( !(AddPass_flag ? UV_BLEND_ENABLED_ADD : UV_BLEND_ENABLED_FIRST) );
					EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(new GUIContent(icoUVBlend, "UV blend settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
							_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
							_target.submenu_settings=ReliefTerrainSettingsItems.UVblend;
						}
						int routed_n=AddPass_flag ? UV_BLEND_ROUTE_NUM_ADD[n-4] : UV_BLEND_ROUTE_NUM_FIRST[n];
						if (routed_n>_target.MixScale.Length-1) routed_n=n; // at init
						nval=_target.MixScale[routed_n+(AddPass_flag?4:0)];
						EditorGUILayout.LabelField("UV blend scale"+((routed_n!=n-(AddPass_flag?4:0)) ? " (from "+routed_n+")":""), GUILayout.Width(130+((routed_n!=n-(AddPass_flag?4:0)) ? 10:0)));
						EditorGUILayout.BeginHorizontal();
							checkChange(ref nval, EditorGUILayout.Slider(_target.MixScale[routed_n+(AddPass_flag?4:0)], 0.01f, 0.5f));
							if (GUILayout.Button(new GUIContent("A","Align to terrain tile"), GUILayout.Width(22), GUILayout.Height(16))) {
								float ratio=Mathf.Round(_target.ReliefTransform.x*nval);
								nval=ratio/_target.ReliefTransform.x;
								EditorUtility.SetDirty(_targetRT);
							}
						EditorGUILayout.EndHorizontal();						
						_target.MixScale[routed_n+(AddPass_flag?4:0)]=nval;
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						nval=_target.MixBlend[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("UV blend val", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target.MixBlend[n], 0, 0.9f));
						_target.MixBlend[n]=nval;
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						nval=_target.MixSaturation[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("UV blend saturation", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target.MixSaturation[n], 0, 1.0f));
						_target.MixSaturation[n]=nval;
					EditorGUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
				
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(new GUIContent(icoPerlinNormal, "Perlin normal settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
							_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
							_target.submenu_settings=ReliefTerrainSettingsItems.GlobalNormal;
						}
						nval=_target._BumpMapGlobalStrength[n];
						EditorGUILayout.LabelField("Perlin normal", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._BumpMapGlobalStrength[n], 0f, 2f));
						_target._BumpMapGlobalStrength[n]=nval;		
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.BeginVertical("Box");
						
					EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(new GUIContent(icoSuperdetail, "Superdetail settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
							_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
							_target.submenu_settings=ReliefTerrainSettingsItems.Superdetail;
						}
						EditorGUI.BeginDisabledGroup( !(AddPass_flag ? SUPER_DETAIL_ENABLED_ADD : SUPER_DETAIL_ENABLED_FIRST) );
						nval=_target._SuperDetailStrengthNormal[n];
						EditorGUILayout.LabelField("Superdetail normal", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._SuperDetailStrengthNormal[n], 0f, 1f));
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();							
					EditorGUI.BeginDisabledGroup( !(AddPass_flag ? SUPER_DETAIL_ENABLED_ADD : SUPER_DETAIL_ENABLED_FIRST)  || !(AddPass_flag ? SUPER_DETAIL_MULTS_ENABLED_ADD : SUPER_DETAIL_MULTS_ENABLED_FIRST));
						_target._SuperDetailStrengthNormal[n]=nval;							
						nval=_target._SuperDetailStrengthMultA[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("Superdetail mult A", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._SuperDetailStrengthMultA[n], 0f, 1f));
						_target._SuperDetailStrengthMultA[n]=nval;	
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();							
						nval=_target._SuperDetailStrengthMultASelfMaskNear[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("  SelfMask near", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._SuperDetailStrengthMultASelfMaskNear[n], 0f, 1f));
						_target._SuperDetailStrengthMultASelfMaskNear[n]=nval;	
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();							
						nval=_target._SuperDetailStrengthMultASelfMaskFar[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("  SelfMask far", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._SuperDetailStrengthMultASelfMaskFar[n], 0f, 1f));
						_target._SuperDetailStrengthMultASelfMaskFar[n]=nval;
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();							
						nval=_target._SuperDetailStrengthMultB[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("Superdetail mult B", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._SuperDetailStrengthMultB[n], 0f, 1f));
						_target._SuperDetailStrengthMultB[n]=nval;							
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();							
						nval=_target._SuperDetailStrengthMultBSelfMaskNear[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("  SelfMask near", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._SuperDetailStrengthMultBSelfMaskNear[n], 0f, 1f));
						_target._SuperDetailStrengthMultBSelfMaskNear[n]=nval;	
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();							
						nval=_target._SuperDetailStrengthMultBSelfMaskFar[n];
						EditorGUILayout.LabelField(" ", GUILayout.Width(16));
						EditorGUILayout.LabelField("  SelfMask far", GUILayout.Width(130));
						checkChange(ref nval, EditorGUILayout.Slider(_target._SuperDetailStrengthMultBSelfMaskFar[n], 0f, 1f));
						_target._SuperDetailStrengthMultBSelfMaskFar[n]=nval;	
					EditorGUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();
						
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(new GUIContent(icoVerticalTexture, "Vertical map settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
							_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
							_target.submenu_settings=ReliefTerrainSettingsItems.VerticalTex;
						}
						EditorGUI.BeginDisabledGroup( !(AddPass_flag ? RTP_VERTICAL_TEXTURE_ADD : RTP_VERTICAL_TEXTURE_FIRST) );
						nval=_target.VerticalTextureStrength[n];
						checkChange(ref nval, EditorGUILayout.Slider("Vertical map strength", _target.VerticalTextureStrength[n], 0f, 1f));
						_target.VerticalTextureStrength[n]=nval;							
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button(new GUIContent(icoSnow, "Snow settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
							_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
							_target.submenu_settings=ReliefTerrainSettingsItems.Snow;
						}
						EditorGUI.BeginDisabledGroup( !(AddPass_flag ? SNOW_ENABLED_ADD : SNOW_ENABLED_FIRST) );
						nval=_target._snow_strength_per_layer[n];
						checkChange(ref nval, EditorGUILayout.Slider("Snow strength", _target._snow_strength_per_layer[n], 0f, 1f));
						_target._snow_strength_per_layer[n]=nval;							
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent(icoWater, "Water settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
						_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
						_target.submenu_settings=ReliefTerrainSettingsItems.Water;
					}
					EditorGUI.BeginDisabledGroup( !(AddPass_flag ? WATER_ENABLED_ADD : WATER_ENABLED_FIRST) );
						
					nval=_target.TERRAIN_LayerWetStrength[n];
					checkChange(ref nval, EditorGUILayout.Slider("Layer wetness", _target.TERRAIN_LayerWetStrength[n], 0f, 1f));
					_target.TERRAIN_LayerWetStrength[n]=nval;							
					EditorGUILayout.EndHorizontal();
						
					EditorGUILayout.HelpBox("Accumulated water color (below water level).\nRealised multiplicative (unless you set high water opacity).\nColor alpha makes water color dependent from Fresnel effect.",MessageType.None, true);
					checkChange(ref _target.TERRAIN_WaterColor[n], EditorGUILayout.ColorField("Water color", _target.TERRAIN_WaterColor[n]));
					checkChange(ref _target.TERRAIN_FresnelPow, EditorGUILayout.Slider("Fresnel exponent (global)", _target.TERRAIN_FresnelPow, 0.5f, 32f));
					checkChange(ref _target.TERRAIN_FresnelOffset, EditorGUILayout.Slider("Fresnel offset (global)", _target.TERRAIN_FresnelOffset, 0, 0.9f));
					GUILayout.Space(6);
						
					nval=_target.TERRAIN_WaterLevel[n];
					checkChange(ref nval, EditorGUILayout.Slider("Water level", _target.TERRAIN_WaterLevel[n]/2f, 0f, 1f)*2f);
					_target.TERRAIN_WaterLevel[n]=nval;		
						
					nval=_target.TERRAIN_WaterLevelSlopeDamp[n];
					checkChange(ref nval, EditorGUILayout.Slider("Water level slope damp", _target.TERRAIN_WaterLevelSlopeDamp[n], 0.1f, 2));
					_target.TERRAIN_WaterLevelSlopeDamp[n]=nval;		
						
					nval=_target.TERRAIN_WaterEdge[n];
					checkChange(ref nval, Mathf.Pow(2, EditorGUILayout.Slider("Water level sharpness", Mathf.Log(_target.TERRAIN_WaterEdge[n])/Mathf.Log(2)/2, 0, 1)*2));
					_target.TERRAIN_WaterEdge[n]=nval;		
						
					GUILayout.Space(6);
						
					nval=_target.TERRAIN_WaterOpacity[n];
					checkChange(ref nval, EditorGUILayout.Slider("Water opacity", _target.TERRAIN_WaterOpacity[n], 0f, 1f));
					_target.TERRAIN_WaterOpacity[n]=nval;		
						
					nval=_target.TERRAIN_WaterGloss[n];
					checkChange(ref nval, EditorGUILayout.Slider("Water gloss", _target.TERRAIN_WaterGloss[n], 0f, 6f));
					_target.TERRAIN_WaterGloss[n]=nval;		
						
					nval=_target.TERRAIN_Refraction[n];
					checkChange(ref nval, EditorGUILayout.Slider("Water refraction", _target.TERRAIN_Refraction[n], 0f, 0.04f));
					_target.TERRAIN_Refraction[n]=nval;		
						
					GUILayout.Space(6);
						
					nval=_target.TERRAIN_Flow[n];
					checkChange(ref nval, EditorGUILayout.Slider("Water flow", _target.TERRAIN_Flow[n], 0f, 1f));
					_target.TERRAIN_Flow[n]=nval;		
						
					GUILayout.Space(3);
					EditorGUILayout.HelpBox("Wet means layer surface covered by thin water layer, but above water level",MessageType.None,true);							
					nval=_target.TERRAIN_WetSpecularity[n];
					checkChange(ref nval, EditorGUILayout.Slider("Wet gloss", _target.TERRAIN_WetSpecularity[n], 0f, 6f));
					_target.TERRAIN_WetSpecularity[n]=nval;		
						
					nval=_target.TERRAIN_WetReflection[n];
					checkChange(ref nval, EditorGUILayout.Slider("Wet reflection", _target.TERRAIN_WetReflection[n]/2f, 0f, 1f)*2f);
					_target.TERRAIN_WetReflection[n]=nval;		
						
					nval=_target.TERRAIN_WetRefraction[n];
					checkChange(ref nval, EditorGUILayout.Slider("Wet refraction factor", _target.TERRAIN_WetRefraction[n], 0f, 1f));
					_target.TERRAIN_WetRefraction[n]=nval;		

					GUILayout.Space(6);
					string[] options=new string[_target.numLayers];
					options[0]="Choose layer to copy water params from";
					for(int k=0; k<_target.numLayers; k++) {
						if (k<=n) {
							if (k<_target.numLayers-1) options[k+1]="layer "+k;
						} else {
							options[k]="layer "+k;
						}
					}
					int idx=EditorGUILayout.Popup(0, options);
					if (idx>0) {
						if (idx-1<n) {
							idx--;
						}
						Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
						_target.CopyWaterParams(idx, n);
					}
						
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.BeginVertical("Box");
					EditorGUILayout.HelpBox("Reflection independent from water effect:\n0 - no reflection\n0.5 - value from detail texture A (specularity) channel\n1 - full reflection",MessageType.None,true);
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(new GUIContent(icoReflection, "Reflection settings"),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16))) {
						_target.submenu=ReliefTerrainMenuItems.GeneralSettings;
						_target.submenu_settings=ReliefTerrainSettingsItems.Reflections;
					}
					EditorGUI.BeginDisabledGroup( !(AddPass_flag ? REFLECTION_ENABLED_ADD : REFLECTION_ENABLED_FIRST) );
					nval=_target.TERRAIN_LayerReflection[n];
					checkChange(ref nval, EditorGUILayout.Slider("Layer reflection", _target.TERRAIN_LayerReflection[n]/2f, 0f, 1f)*2f);
					_target.TERRAIN_LayerReflection[n]=nval;							
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
						
					EditorGUILayout.Space();
				}
				EditorGUILayout.EndVertical();
					
				GUILayout.Space(8);							
				if (GUILayout.Button("Reset layer numeric params to default")) {
					Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
					_target.ReturnToDefaults("layer");
					EditorUtility.SetDirty(_targetRT);
					_target.Refresh();
				}	
					
				#endregion Detail maps unfold	- layers
			}
			
			
			#endregion Detail maps unfold
		}	
		
		//bool nshow_controlmaps=EditorGUILayout.Foldout(_target.show_controlmaps, "Control (alpha) maps");
		//if (nshow_controlmaps) {
		if (_target.show_controlmaps) {
			#region Control maps
			Color skin_color;
    		toolbarStrings = new string[3]{"Compose", "Acquire", "Control maps"};
			GUILayout.Space(6);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			_target.submenu_control_textures=(ReliefTerrainControlTexturesItems )(GUILayout.Toolbar((int)_target.submenu_control_textures, toolbarStrings, EditorStyles.miniButton, GUILayout.MaxWidth(370)));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			
			switch(_target.submenu_control_textures) {
			case ReliefTerrainControlTexturesItems.Compose:
				#region Control maps - compose maps
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Compose splats", EditorStyles.boldLabel);
				GUI.color=skin_color;
				EditorGUILayout.HelpBox("Here you can manipulate alpha maps that control layers coverage,\n1. Source splat is coverage texture and you can choose its channel,\n2. Value taken from coverage texture (can be inverted) is multiplied by given factor,\n3. Result is normalized,\n4. Blank source splats are considered 1 (white),\n5. In case you unset \"active\", input value is taken from current control map.",MessageType.None, true);				
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Layer", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
				EditorGUILayout.LabelField("Source splats", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
				EditorGUILayout.EndHorizontal();		
	
				for(int i=0; i<_target.numLayers; i++) {
					int n=_targetRT.splat_layer_seq[i];
					Texture2D tex=_targetRT.source_controls[n];
					Texture2D ntex=tex;
					EditorGUILayout.BeginHorizontal();
#if !UNITY_3_5
						GUILayout.Label((Texture2D)AssetPreview.GetAssetPreview(_target.splats[n]), GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
#else
						GUILayout.Label((Texture2D)EditorUtility.GetAssetPreview(_target.splats[n]), GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
#endif						
						if (_targetRT.splat_layer_calc[n]) {
							if (_targetRT.source_controls[n]) {
								checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_targetRT.source_controls[n], typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
							} else {
								if (!blank_white_tex) {
									blank_white_tex=new Texture2D(4,4,TextureFormat.ARGB32,false);
									Color[] cols=new Color[16];
									for(int k=0; k<16; k++) cols[k]=Color.white;
									blank_white_tex.SetPixels(cols);
									blank_white_tex.Apply(false, true);
								}
								if (!blank_black_tex) {
									blank_black_tex=new Texture2D(4,4,TextureFormat.ARGB32,false);
									Color[] cols=new Color[16];
									for(int k=0; k<16; k++) cols[k]=Color.black;
									blank_black_tex.SetPixels(cols);
									blank_black_tex.Apply(false, true);
								}
								if (_targetRT.source_controls_invert[n]) {
									ntex=tex=blank_black_tex;
								} else {
									ntex=tex=blank_white_tex;
								}
								checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
								if (ntex==null) {
									_targetRT.source_controls[n]=tex=null;
								}
							}
							if (_targetRT.source_controls[n]==blank_white_tex || ntex==blank_white_tex || _targetRT.source_controls[n]==blank_black_tex || ntex==blank_black_tex || ntex==null) _targetRT.source_controls[n]=ntex=tex=null;
							if (ntex!=tex) {
								int wdth=1024;
								if (_targetRT.controlA!=null) {
									wdth=_targetRT.controlA.width;
									for(int j=0; j<_targetRT.source_controls.Length; j++) {
										Color[] cols=new Color[wdth*wdth];
										bool cols_inited=false;
										if (_targetRT.source_controls[j]) {
											if (_targetRT.source_controls[j].width!=wdth) {
												if (!cols_inited) {
													cols_inited=true;
													for(int u=0; u<cols.Length; u++) cols[u]=Color.white;
												}
												_targetRT.source_controls[j]=new Texture2D(wdth,wdth);
												_targetRT.source_controls[j].SetPixels(cols);
												_targetRT.source_controls[j].Apply(true);
											}
										}
									}
								} else {
									for(int j=0; j<_targetRT.source_controls.Length; j++) {
										if (_targetRT.source_controls[j]) {
											wdth=_targetRT.source_controls[j].width;
											break;
										}
									}
								}
								if (ntex && ntex.width==wdth) {
									_targetRT.source_controls[n]=ntex;
									AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
									if (_importer) {
										TextureImporter tex_importer=(TextureImporter)_importer;
										if (!tex_importer.isReadable) {
											Debug.LogWarning("Source splat texture "+n+" ("+ntex.name+") has been reimported as readable.");
											tex_importer.isReadable=true;
											AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
										}
									}
									if ( _targetRT.source_controls_invert[n] ) {
										Color[] cols=_targetRT.source_controls[n].GetPixels();
										_target.InvertChannel(cols, -1);
										int w=_targetRT.source_controls[n].width;
										_targetRT.source_controls[n]=new Texture2D(w,w);
										_targetRT.source_controls[n].SetPixels(cols);
										_targetRT.source_controls[n].Apply();
									}									
								} else {
									if (_targetRT.controlA) {
										EditorUtility.DisplayDialog("Error", "Source control alpha maps should have "+wdth+" size.","OK");
									} else {
										///////////////////
										if (EditorUtility.DisplayDialog("Error", "Should I reset all source control alpha maps to "+ntex.width+" size?","Yes, please","No, thanks")) {
											wdth=ntex.width;
											Color[] cols=new Color[wdth*wdth];
											for(int u=0; u<cols.Length; u++) cols[u]=Color.white;
											for(int j=0; j<_targetRT.source_controls.Length; j++) {
												if (j!=n) {
													if (_targetRT.source_controls[j].width!=wdth) {
														_targetRT.source_controls[j]=new Texture2D(wdth,wdth);
														_targetRT.source_controls[j].SetPixels(cols);
														_targetRT.source_controls[j].Apply(true);
													}
												} else {
													_targetRT.source_controls[n]=ntex;
												}
											}
										}
										/////////////////										
									}
								}
							}
						} else {
							GUILayout.Label((Texture2D)null, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
						}
						EditorGUILayout.BeginVertical(); // layer props
						EditorGUILayout.BeginHorizontal();
						checkChange(ref _targetRT.splat_layer_calc[n], EditorGUILayout.Toggle(_targetRT.splat_layer_calc[n], GUILayout.MinWidth(10), GUILayout.MaxWidth(10)));
						EditorGUILayout.LabelField("active", GUILayout.MinWidth(40), GUILayout.MaxWidth(40));
						EditorGUILayout.EndHorizontal();
						if (_targetRT.splat_layer_calc[n]) {
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("channel", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
							checkChange(ref _targetRT.source_controls_channels[n], (ColorChannels)EditorGUILayout.EnumPopup(_targetRT.source_controls_channels[n]));
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("multiplier", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
							checkChange(ref _targetRT.splat_layer_boost[n], EditorGUILayout.FloatField(_targetRT.splat_layer_boost[n]));
							EditorGUILayout.EndHorizontal();
						
							EditorGUILayout.BeginHorizontal();
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("masked", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
								checkChange(ref _targetRT.splat_layer_masked[n], EditorGUILayout.Toggle(_targetRT.splat_layer_masked[n], GUILayout.MinWidth(10), GUILayout.MaxWidth(10)));
//									if (_targetRT.splat_layer_masked[n] && !_targetRT.source_controls[n]) {
//										_targetRT.splat_layer_masked[n]=false;
//										EditorUtility.DisplayDialog("Error", "Blank input can not be masked.","OK");
//									}
								EditorGUILayout.EndHorizontal();
								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField("invert", GUILayout.MinWidth(40), GUILayout.MaxWidth(40));
								bool nval=_targetRT.source_controls_invert[n];
								checkChange(ref nval, EditorGUILayout.Toggle(_targetRT.source_controls_invert[n], GUILayout.MinWidth(10), GUILayout.MaxWidth(10)));
								if ( (_targetRT.source_controls_invert[n]!=nval)  && (_targetRT.source_controls[n])) {
									Color[] cols=_targetRT.source_controls[n].GetPixels();
									_target.InvertChannel(cols, -1);
									int w=_targetRT.source_controls[n].width;
									_targetRT.source_controls[n]=new Texture2D(w,w);
									_targetRT.source_controls[n].SetPixels(cols);
									_targetRT.source_controls[n].Apply();
								}
								_targetRT.source_controls_invert[n]=nval;								
//									if (_targetRT.source_controls_invert[n] && !_targetRT.source_controls[n]) {
//										_targetRT.source_controls_invert[n]=false;
//										EditorUtility.DisplayDialog("Error", "Blank input can not be inverted.","OK");
//									}
								EditorGUILayout.EndHorizontal();						
							EditorGUILayout.EndHorizontal();
						}
					
						EditorGUILayout.BeginHorizontal(); // move up/down
						if (GUILayout.Button("Move up")) {
							if (i>0) {
								int tmp=_targetRT.splat_layer_seq[i];
								_targetRT.splat_layer_seq[i]=_targetRT.splat_layer_seq[i-1];
								_targetRT.splat_layer_seq[i-1]=tmp;
							}
							//_targetRT.splat_layer_seq=new int[8]{0,1,2,3,4,5,6,7};
						}
						if (GUILayout.Button("Move down")) {
							if (i<_target.numLayers-1) {
								int tmp=_targetRT.splat_layer_seq[i];
								_targetRT.splat_layer_seq[i]=_targetRT.splat_layer_seq[i+1];
								_targetRT.splat_layer_seq[i+1]=tmp;
							}
						}
						EditorGUILayout.EndHorizontal(); // move up/down
						EditorGUILayout.EndVertical(); // layer props
					
					EditorGUILayout.EndHorizontal();
					// optional mask
					if (	_targetRT.splat_layer_masked[n] && _targetRT.splat_layer_calc[n]) {
						EditorGUILayout.BeginHorizontal();
						GUILayout.Label("Source splat\nmask >", GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
						tex=ntex=_targetRT.source_controls_mask[n];
						checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
						if (ntex!=tex) {
							if (ntex && ntex.width==_targetRT.controlA.width) {
								_targetRT.source_controls_mask[n]=ntex;
								AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
								if (_importer) {
									TextureImporter tex_importer=(TextureImporter)_importer;
									if (!tex_importer.isReadable) {
										Debug.LogWarning("Mask texture "+n+" ("+ntex.name+") has been reimported as readable.");
										tex_importer.isReadable=true;
										AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
									}
								}
								if ( _targetRT.source_controls_mask_invert[n] ) {
									Color[] cols=_targetRT.source_controls_mask[n].GetPixels();
									_target.InvertChannel(cols, -1);
									int w=_targetRT.source_controls_mask[n].width;
									_targetRT.source_controls_mask[n]=new Texture2D(w,w);
									_targetRT.source_controls_mask[n].SetPixels(cols);
									_targetRT.source_controls_mask[n].Apply();
								}									
							} else if (ntex) {
								EditorUtility.DisplayDialog("Error", "Mask texture should have "+_targetRT.controlA.width+" size.","OK");
							} else {
								_targetRT.source_controls_mask[n]=ntex; // null
								_targetRT.splat_layer_masked[n]=false;
							}
						}
						EditorGUILayout.BeginVertical(); // mask props
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("channel", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
						checkChange(ref _targetRT.source_controls_mask_channels[n], (ColorChannels)EditorGUILayout.EnumPopup(_targetRT.source_controls_mask_channels[n]));
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("invert", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
						bool nval=_targetRT.source_controls_mask_invert[n];
						checkChange(ref nval, EditorGUILayout.Toggle(_targetRT.source_controls_mask_invert[n]));
						if ( (_targetRT.source_controls_mask_invert[n]!=nval)  && (_targetRT.source_controls_mask[n])) {
							Color[] cols=_targetRT.source_controls_mask[n].GetPixels();
							_target.InvertChannel(cols, -1);
							int w=_targetRT.source_controls_mask[n].width;
							_targetRT.source_controls_mask[n]=new Texture2D(w,w);
							_targetRT.source_controls_mask[n].SetPixels(cols);
							_targetRT.source_controls_mask[n].Apply();
						}
						_targetRT.source_controls_mask_invert[n]=nval;									
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.EndVertical(); // mask props						
						
						EditorGUILayout.EndHorizontal();
					}
					
				}
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Set all \"active\" flags")) {
					for(int j=0; j<_target.numLayers; j++) {
						_targetRT.splat_layer_calc[j]=true;
					}
				}				
				if (GUILayout.Button("Clear all \"active\" flags")) {
					for(int j=0; j<_target.numLayers; j++) {
						_targetRT.splat_layer_calc[j]=false;
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.HelpBox("In layered mode highest (bottom) layer \"wins\". In unlayered mode order doesn't matter.",MessageType.None, true);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Layered mode", EditorStyles.boldLabel, GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
				checkChange(ref _targetRT.splat_layer_ordered_mode, EditorGUILayout.Toggle(_targetRT.splat_layer_ordered_mode));
				EditorGUILayout.EndHorizontal();
				if (GUILayout.Button("Render Control maps from source splats")) {
					if (EditorUtility.DisplayDialog("Warning", "This will overwrite current splat controlmaps\n(you may backup them using save feature in \"Splatmaps...\" unfold below). Are you sure ?","Yes","Cancel")) {
						if (terrainComp) {
							_targetRT.RecalcControlMaps();
						} else {
							_targetRT.RecalcControlMapsForMesh();
						}
					}
				}
				
				EditorGUILayout.EndVertical();
				#endregion Control maps - compose maps
				break;
			case ReliefTerrainControlTexturesItems.Acquire:
				#region Control maps - acquire mask textures
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Acquire mask textures", EditorStyles.boldLabel);
				GUI.color=skin_color;
				if (terrainComp) {
					EditorGUILayout.HelpBox("Here you can get coverage textures taken from height, steepness or normal direction (relative to reference object's Z axis and Y component of compared direction vectors may be optionally skipped, if not - resultant texture might be considered lightmap when using light as ref direction object).\n\nProduced textures are esp. useful for users that don't use external terrain tools, but only heightmap as base for their work. Textures taken here may serve as splat sources (\"Compose splats\" tab above).\n\nRealistic heightmaps themselves might be acquired from sattelite data - one may google for that, you can also consider making heightmap from scratch using photoshop (use perlin noise for example).",MessageType.None, true);
					if (GUILayout.Button("Save steepness")) {
						Texture2D tex=_targetRT.GetSteepnessHeightDirectionTexture(0);
						if (tex) {
							SaveTexture(ref tex, ref _target.save_path_terrain_steepness, "terrain_steepnessMap.png", 0, TextureImporterFormat.AutomaticCompressed, false, false);
						} else {
							EditorUtility.DisplayDialog("?", "Can't get steepness texture...","OK");
						}
					}
					if (GUILayout.Button("Save height")) {
						Texture2D tex=_targetRT.GetSteepnessHeightDirectionTexture(1);
						if (tex) {
							SaveTexture(ref tex, ref _target.save_path_terrain_height, "terrain_heightMap.png", 0, TextureImporterFormat.AutomaticCompressed, false, false);
						} else {
							EditorUtility.DisplayDialog("?", "Can't get height texture...","OK");
						}
					}
	
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Direction reference", GUILayout.MaxWidth(116));
					checkChange(ref _target.direction_object, (GameObject)EditorGUILayout.ObjectField(_target.direction_object, typeof(GameObject), true));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Skip y component", GUILayout.MaxWidth(116));
					checkChange(ref _target.flat_dir_ref, EditorGUILayout.Toggle(_target.flat_dir_ref));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Flip direction", GUILayout.MaxWidth(116));
					checkChange(ref _target.flip_dir_ref, EditorGUILayout.Toggle(_target.flip_dir_ref));
					EditorGUILayout.EndHorizontal();
					if (GUILayout.Button("Save direction")) {
						Texture2D tex=_targetRT.GetSteepnessHeightDirectionTexture(2,_target.direction_object);
						if (tex) {
							SaveTexture(ref tex, ref _target.save_path_terrain_direction, "terrain_directionMap.png", 0, TextureImporterFormat.AutomaticCompressed, false, false);
						} else {
							EditorUtility.DisplayDialog("?", "Can't get direction texture...","OK");
						}							
					}
				} else {
					EditorGUILayout.HelpBox("Option available for terrains only.",MessageType.Warning, true);
				}
				
				EditorGUILayout.Space();
				EditorGUILayout.EndVertical();
				#endregion Control maps - acquire mask textures					
				break;
			case ReliefTerrainControlTexturesItems.Controlmaps:
				#region Control maps - splats					
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Splatmaps (control maps)", EditorStyles.boldLabel);
				GUI.color=skin_color;
				EditorGUILayout.HelpBox("Here you can see control maps as they are stored and used by Unity terrain system. For terrains you can backup them using save feature. You can drag&drop (select) such splatmaps (ARGB32) to override existing one(s) - for example when you need to restore them from backup or need to apply your own on terrain.",MessageType.None, true);
				if (terrainComp) {
					if (GUILayout.Button("Refresh Controlmap(s) shown from TerrainData")) {
						_targetRT.GetControlMaps();
					}
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Splat 0-3", GUILayout.MaxWidth(65));
				if (terrainComp!=null) {
					bool tmp_dirtyFlag=dirtyFlag;
					checkChange(ref _targetRT.controlA, (Texture2D)EditorGUILayout.ObjectField( _targetRT.controlA, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					if (dirtyFlag && !tmp_dirtyFlag) {
						AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_targetRT.controlA));
						if (_importer) {
							TextureImporter tex_importer=(TextureImporter)_importer;
							if (!tex_importer.isReadable) {
								Debug.LogWarning("Control texture 0 ("+_targetRT.controlA.name+") has been reimported as readable.");
								tex_importer.isReadable=true;
								AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_targetRT.controlA),  ImportAssetOptions.ForceUpdate);
							}
						}					
						_targetRT.SetCustomControlMaps();
					}
				} else {
					checkChange(ref _targetRT.controlA, (Texture2D)EditorGUILayout.ObjectField( _targetRT.controlA, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
				}
				if (_target.numLayers>4) {
					EditorGUILayout.LabelField("Splat 4-7", GUILayout.MaxWidth(65));
					if (terrainComp!=null) {
						bool tmp_dirtyFlag=dirtyFlag;
						checkChange(ref _targetRT.controlB, (Texture2D)EditorGUILayout.ObjectField( _targetRT.controlB, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
						if (dirtyFlag && !tmp_dirtyFlag) {
							AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_targetRT.controlB));
							if (_importer) {
								TextureImporter tex_importer=(TextureImporter)_importer;
								if (!tex_importer.isReadable) {
									Debug.LogWarning("Control texture 1 ("+_targetRT.controlB.name+") has been reimported as readable.");
									tex_importer.isReadable=true;
									AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_targetRT.controlB),  ImportAssetOptions.ForceUpdate);
								}
							}					
							_targetRT.SetCustomControlMaps();
						}
					} else {
						checkChange(ref _targetRT.controlB, (Texture2D)EditorGUILayout.ObjectField( _targetRT.controlB, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					}
				}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Save to file", GUILayout.MinWidth(170), GUILayout.MaxWidth(170))) {
					Texture2D tex=_targetRT.controlA;
					if (tex) {
						SaveTexture(ref tex, ref _targetRT.save_path_controlA, "terrain_splatMapA.png", 0, TextureImporterFormat.ARGB32, true, false, true);
					} else {
						EditorUtility.DisplayDialog("?", "Can't get height texture...","OK");
					}
				}								
				if (_target.numLayers>4) {
					if (GUILayout.Button("Save to file", GUILayout.MinWidth(170), GUILayout.MaxWidth(170))) {
						Texture2D tex=_targetRT.controlB;
						if (tex) {
							SaveTexture(ref tex, ref _targetRT.save_path_controlB, "terrain_splatMapB.png", 0, TextureImporterFormat.ARGB32, true, false, true);
						} else {
							EditorUtility.DisplayDialog("?", "Can't get height texture...","OK");
						}
					}								
				}
				EditorGUILayout.EndHorizontal();
				
				if (_target.numLayers>8) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Splat 8-11", GUILayout.MaxWidth(65));
					if (terrainComp!=null) {
						bool tmp_dirtyFlag=dirtyFlag;
						checkChange(ref _targetRT.controlC, (Texture2D)EditorGUILayout.ObjectField( _targetRT.controlC, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
						if (dirtyFlag && !tmp_dirtyFlag) {
							AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_targetRT.controlC));
							if (_importer) {
								TextureImporter tex_importer=(TextureImporter)_importer;
								if (!tex_importer.isReadable) {
									Debug.LogWarning("Control texture 2 ("+_targetRT.controlC.name+") has been reimported as readable.");
									tex_importer.isReadable=true;
									AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_targetRT.controlC),  ImportAssetOptions.ForceUpdate);
								}
							}					
							_targetRT.SetCustomControlMaps();
						}
					} else {
						checkChange(ref _targetRT.controlC, (Texture2D)EditorGUILayout.ObjectField( _targetRT.controlC, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					}
					EditorGUILayout.EndHorizontal();
				}

				if (_target.numLayers>8) {
					if (GUILayout.Button("Save to file", GUILayout.MinWidth(170), GUILayout.MaxWidth(170))) {
						Texture2D tex=_targetRT.controlC;
						if (tex) {
							SaveTexture(ref tex, ref _targetRT.save_path_controlC, "terrain_splatMapC.png", 0, TextureImporterFormat.ARGB32, true, false, true);
						} else {
							EditorUtility.DisplayDialog("?", "Can't get height texture...","OK");
						}
					}								
				}
				#endregion Control maps - splats					
				EditorGUILayout.EndVertical();
				break;
			}		
			
			#endregion Control maps
		}

		//_target.show_heightmaps=EditorGUILayout.Foldout(_target.show_heightmaps, "Height maps (combined)");
		if (_target.show_derivedmaps) {
			#region Derived textures
			Color skin_color;
    		toolbarStrings = new string[4]{"Atlasing", "Height maps", "Normal maps", "Special"};
			GUILayout.Space(6);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			_target.submenu_derived_textures=(ReliefTerrainDerivedTexturesItems)(GUILayout.Toolbar((int)_target.submenu_derived_textures, toolbarStrings, EditorStyles.miniButton, GUILayout.MaxWidth(370)));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(6);
			EditorGUILayout.HelpBox("Combined texures are what actually feeds shader. They're intended to be reused on multiple terrain tiles. You can also reuse them across scenes.",MessageType.Warning, true);
			switch(_target.submenu_derived_textures) {
			case ReliefTerrainDerivedTexturesItems.Atlasing:
				#region Derived textures - Atlasing features
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Color atlases (necessary in 8 layers in one pass)", EditorStyles.boldLabel);
				GUI.color=skin_color;
				
				if (!_target.splat_atlases[0] && (_target.numLayers>4) && (!_4LAYERS_SHADER_USED)) {
					EditorGUILayout.HelpBox("Terrain detail textures have to be atlased for shader.",MessageType.Error, true);
				}
				EditorGUI.BeginDisabledGroup( _4LAYERS_SHADER_USED );
				if (GUILayout.Button("Prepare atlases from detail textures (may take a while)")) {
					PrepareAtlases( (_target.numLayers>4) ? 3:1);
					_target.Refresh();
				}
				EditorGUI.EndDisabledGroup();
					
				EditorGUILayout.BeginHorizontal();
					
					if (_4LAYERS_SHADER_USED) {
							_target.splat_atlases[0]=null;
							_target.splat_atlases[1]=null;
					}
					if (_target.numLayers>0) {
					EditorGUI.BeginDisabledGroup( _4LAYERS_SHADER_USED );
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Layers 0-3", GUILayout.Width(100));
					checkChange(ref _target.splat_atlases[0], (Texture2D)EditorGUILayout.ObjectField( _target.splat_atlases[0], typeof(Texture2D), false, GUILayout.Height(100), GUILayout.Width(100)));
					if (SaveTexture(ref _target.splat_atlases[0], ref _target.save_path_atlasA, "atlas_texture_layers_0_to_3.png", 100, TextureImporterFormat.AutomaticCompressed, true)) {
						string path=AssetDatabase.GetAssetPath(_target.splat_atlases[0]);
						TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
						textureImporter.wrapMode = TextureWrapMode.Clamp;
						textureImporter.filterMode = FilterMode.Trilinear;
						AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
					}
					EditorGUILayout.EndVertical();
					EditorGUI.EndDisabledGroup();
					}

					if (_target.numLayers>4) {
					EditorGUI.BeginDisabledGroup( _4LAYERS_SHADER_USED );
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Layers 4-7", GUILayout.Width(100));
					checkChange(ref _target.splat_atlases[1], (Texture2D)EditorGUILayout.ObjectField( _target.splat_atlases[1], typeof(Texture2D), false, GUILayout.Height(100), GUILayout.Width(100)));
					if (SaveTexture(ref _target.splat_atlases[1], ref _target.save_path_atlasB, "atlas_texture_layers_4_to_7.png", 100, TextureImporterFormat.AutomaticCompressed, true)) {
						_target.splat_atlases[1].wrapMode=TextureWrapMode.Clamp;
						_target.splat_atlases[1].filterMode=FilterMode.Trilinear;
					}
					EditorGUILayout.EndVertical();
					EditorGUI.EndDisabledGroup();
					}
					
				EditorGUILayout.EndHorizontal();									
				
				EditorGUILayout.EndVertical();
				#endregion Derived textures - Atlasing features		
				break;
			case ReliefTerrainDerivedTexturesItems.Heightmaps:
				#region Derived textures - Heightmaps
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Combined heightmaps", EditorStyles.boldLabel);
				GUI.color=skin_color;
			
				EditorGUILayout.HelpBox("If you specified heightmaps for your layers you don't have to do anything while editing (heightmaps are refreshed as you modify any of separate heightmap). However, you might need to specify combined heightmaps by hand, so you can do it here (don't touch particular heightmaps in a layer properties then).",MessageType.None, true);
				EditorGUILayout.HelpBox("Every channel of height textures below stores heightmap for consecutive detail map (splats 0-3 -> heightmap channels RGBA). To generate such channel combined heightmaps you can also use attached tool (Window / Relief Tools / 4 to 1 texture channels mixer).",MessageType.None, true);
					
				EditorGUILayout.BeginHorizontal();
					
					if (_target.numLayers>0) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Heights 0-3", GUILayout.Width(80));
					checkChange(ref _target.HeightMap, (Texture2D)EditorGUILayout.ObjectField( _target.HeightMap, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.HeightMap, ref _target.save_path_HeightMap, "heightmap_layers_0_to_3.png", 80, TextureImporterFormat.AutomaticCompressed, true,true,true);
					EditorGUILayout.EndVertical();
					}

					if (_target.numLayers>4) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Heights 4-7", GUILayout.Width(80));
					checkChange(ref _target.HeightMap2, (Texture2D)EditorGUILayout.ObjectField( _target.HeightMap2, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.HeightMap2, ref _target.save_path_HeightMap2, "heightmap_layers_4_to_7.png", 80, TextureImporterFormat.AutomaticCompressed, true,true,true);
					EditorGUILayout.EndVertical();
					}
					
					if (_target.numLayers>8) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Heights 8-12", GUILayout.Width(80));
					checkChange(ref _target.HeightMap3, (Texture2D)EditorGUILayout.ObjectField( _target.HeightMap3, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.HeightMap3, ref _target.save_path_HeightMap3, "heightmap_layers_8_to_11.png", 80, TextureImporterFormat.AutomaticCompressed, true,true,true);
					EditorGUILayout.EndVertical();
					}
					
				EditorGUILayout.EndHorizontal();						
				
				EditorGUILayout.EndVertical();
				#endregion Derived textures - Heightmaps
				break;
			case ReliefTerrainDerivedTexturesItems.Bumpmaps:
				#region Derived textures - Bumpmaps
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Combined bumpmaps", EditorStyles.boldLabel);
				GUI.color=skin_color;
			
				EditorGUILayout.HelpBox("Each layer pair share the same normal texture (RG + BA).They're automatically composed/refreshed when you change bump map in a layer. However, you might need to specify combined bumpmaps by hand, so you can do it here (don't touch particular bumpmaps in a layer properties then).",MessageType.None, true);
					
				EditorGUILayout.BeginHorizontal();
					if (_target.numLayers>0) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Bumps 0+1", GUILayout.Width(80));
					checkChange(ref _target.Bump01, (Texture2D)EditorGUILayout.ObjectField( _target.Bump01, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.Bump01, ref _target.save_path_Bump01, "bumpmap_layers01.png", 80, TextureImporterFormat.ARGB32, true,true,true);
					EditorGUILayout.EndVertical();
					}

					if (_target.numLayers>2) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Bumps 2+3", GUILayout.Width(80));
					checkChange(ref _target.Bump23, (Texture2D)EditorGUILayout.ObjectField( _target.Bump23, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.Bump23, ref _target.save_path_Bump23, "bumpmap_layers23.png", 80, TextureImporterFormat.ARGB32, true,true,true);
					EditorGUILayout.EndVertical();
					}
					
					if (_target.numLayers>4) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Bumps 4+5", GUILayout.Width(80));
					checkChange(ref _target.Bump45, (Texture2D)EditorGUILayout.ObjectField( _target.Bump45, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.Bump45, ref _target.save_path_Bump45, "bumpmap_layers45.png", 80, TextureImporterFormat.ARGB32, true,true,true);
					EditorGUILayout.EndVertical();
					}
					
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);
					
				if (_target.numLayers>6) {
				EditorGUILayout.BeginHorizontal();
					if (_target.numLayers>6) {							
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Bumps 6+7", GUILayout.Width(80));
					checkChange(ref _target.Bump67, (Texture2D)EditorGUILayout.ObjectField( _target.Bump67, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.Bump67, ref _target.save_path_Bump67, "bumpmap_layers67.png", 80, TextureImporterFormat.ARGB32, true,true,true);
					EditorGUILayout.EndVertical();
					}
					
					if (_target.numLayers>8) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Bumps 8+9", GUILayout.Width(80));
					checkChange(ref _target.Bump89, (Texture2D)EditorGUILayout.ObjectField( _target.Bump89, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.Bump89, ref _target.save_path_Bump89, "bumpmap_layers89.png", 80, TextureImporterFormat.ARGB32, true,true,true);
					EditorGUILayout.EndVertical();
					}
					
					if (_target.numLayers>10) {
					EditorGUILayout.BeginVertical();
					EditorGUILayout.LabelField("Bumps 10+11", GUILayout.Width(80));
					checkChange(ref _target.BumpAB, (Texture2D)EditorGUILayout.ObjectField( _target.BumpAB, typeof(Texture2D), false, GUILayout.Height(80), GUILayout.Width(80)));
					SaveTexture(ref _target.BumpAB, ref _target.save_path_BumpAB, "bumpmap_layersAB.png", 80, TextureImporterFormat.ARGB32, true,true,true);
					EditorGUILayout.EndVertical();
					}
						
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);
				}
				
				EditorGUILayout.EndVertical();
				#endregion Derived textures - Bumpmaps
				break;						
			case ReliefTerrainDerivedTexturesItems.Globalnormal:
				#region Derived textures - Global normal
				GUILayout.Space(10);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Perlin normal + superdetail / water&reflection", EditorStyles.boldLabel);
				GUI.color=skin_color;
			
				EditorGUILayout.HelpBox("Texture below use perlin normal map specified on channels RG.\n\nChannels BA are used for superdetail additional channels or (mutually exclusive) wet mask (B) and reflection map (A).",MessageType.None, true);
					
				//EditorGUILayout.BeginVertical();
				EditorGUILayout.LabelField("Texture", GUILayout.Width(80));
				bool sync_tex=false;
				if (checkChange(ref _targetRT.BumpGlobalCombined, (Texture2D)EditorGUILayout.ObjectField( _targetRT.BumpGlobalCombined, typeof(Texture2D), false, GUILayout.Height(100), GUILayout.Width(100)))) sync_tex=true;
				if (SaveTexture(ref _targetRT.BumpGlobalCombined, ref _targetRT.save_path_BumpGlobalCombined, "perlin_normal_aux.png", 100, TextureImporterFormat.AutomaticCompressed, true,true,true)) sync_tex=true;
				if (sync_tex) {
					RTP_LODmanager manager=_target.Get_RTP_LODmanagerScript();
					if (manager && !manager.RTP_WETNESS_FIRST && !manager.RTP_WETNESS_ADD) {
						ReliefTerrain[] objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
						for(int i=0; i<objs.Length; i++) {
							objs[i].BumpGlobalCombined=_targetRT.BumpGlobalCombined;
							objs[i].globalCombinedModifed_flag=false;
						}
					}
					_targetRT.globalCombinedModifed_flag=false;
				}
				//EditorGUILayout.EndVertical();
		
				EditorGUILayout.EndVertical();
				#endregion Derived textures - Global normal
				break;						
			} // derived textures submenu switch
			#endregion Derived textures
		}
		
		if (_target.show_settings) {
			#region ALLSettings
    		GUIContent[] toolbarIcons1 = new GUIContent[5]{ new GUIContent("Main", icoLayersSmall ), new GUIContent("UV blend", icoUVBlend), new GUIContent("Global\nmaps", icoGlobalcolor), new GUIContent("Perlin\nnormal", icoPerlinNormal), new GUIContent("S. detail", icoSuperdetail) };
			GUIContent[] toolbarIcons2 = new GUIContent[5]{ new GUIContent("Parallax", icoPOM ), new GUIContent("Vertical\ntexture", icoVerticalTexture), new GUIContent("Snow", icoSnow), new GUIContent("Water &\ncaustic", icoWater), new GUIContent("Reflection", icoReflection) };
			ReliefTerrainSettingsItems prev_submenu_settings=_target.submenu_settings;
			GUILayout.Space(10);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			_target.submenu_settings=(ReliefTerrainSettingsItems)(GUILayout.Toolbar((int)_target.submenu_settings, toolbarIcons1, EditorStyles.miniButton, GUILayout.MaxWidth(370)));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(5);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			_target.submenu_settings=(ReliefTerrainSettingsItems)(GUILayout.Toolbar((int)_target.submenu_settings-5, toolbarIcons2, EditorStyles.miniButton, GUILayout.MaxWidth(370))+5);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10);
			
			if (prev_submenu_settings!=_target.submenu_settings && _target.paint_flag) {
				_target.paint_flag=false;
				Tools.current=prev_tool;
				SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;
				EditorUtility.SetDirty(target);
			}
			
			Color skin_color;
			switch(_target.submenu_settings) {
			case ReliefTerrainSettingsItems.MainSettings:
				#region Settings	 - main settings

				GUILayout.Space(6);
				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
						
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoLayers),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);						
					EditorGUILayout.LabelField("Main settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
				EditorGUILayout.HelpBox("For performance reasons shader uses the same tiling for every detail map.",MessageType.None, true);
				EditorGUILayout.BeginHorizontal();
					checkChange(ref _target.ReliefTransform.x, _target.terrainTileSizeX/EditorGUILayout.FloatField("Tile Size X [m] ("+(Mathf.Round(_target.ReliefTransform.x*100)/100)+")", _target.terrainTileSizeX/_target.ReliefTransform.x));
					if (GUILayout.Button(new GUIContent("A","Align to terrain tile"), GUILayout.Width(22), GUILayout.Height(16))) {
						float ratio=_target.ReliefTransform.x/_target.ReliefTransform.y;
						_target.ReliefTransform.x=Mathf.Round(_target.ReliefTransform.x);
						_target.ReliefTransform.y=_target.ReliefTransform.x/ratio;
						_target.ReliefTransform.y=Mathf.Round(_target.ReliefTransform.y);
						EditorUtility.SetDirty(_targetRT);
					}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();					
					checkChange(ref _target.ReliefTransform.y, _target.terrainTileSizeZ/EditorGUILayout.FloatField("Tile Size Y [m] ("+(Mathf.Round(_target.ReliefTransform.y*100)/100)+")", _target.terrainTileSizeZ/_target.ReliefTransform.y));
					if (GUILayout.Button(new GUIContent("A","Align to terrain tile"), GUILayout.Width(22), GUILayout.Height(16))) {
						float ratio=_target.ReliefTransform.x/_target.ReliefTransform.y;
						_target.ReliefTransform.y=Mathf.Round(_target.ReliefTransform.y);
						_target.ReliefTransform.x=_target.ReliefTransform.y*ratio;
						_target.ReliefTransform.x=Mathf.Round(_target.ReliefTransform.x);
						EditorUtility.SetDirty(_targetRT);
					}
				EditorGUILayout.EndHorizontal();
				float tile_sizex=_target.terrainTileSizeX/_target.ReliefTransform.x;
				float tile_sizey=_target.terrainTileSizeZ/_target.ReliefTransform.y;
				checkChange(ref _target.ReliefTransform.z, EditorGUILayout.FloatField("Tile Offset X [m]", _target.ReliefTransform.z*tile_sizex)/tile_sizex);
				checkChange(ref _target.ReliefTransform.w, EditorGUILayout.FloatField("Tile Offset Y [m]", _target.ReliefTransform.w*tile_sizey)/tile_sizey);
				
				EditorGUILayout.Space();	
				
				EditorGUILayout.LabelField("Near distance values");
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				EditorGUILayout.BeginVertical();
				#if !UNITY_3_5
				float prev_distance_start=_target.distance_start;
				float prev_distance_transition=_target.distance_transition;
				#endif
				checkChange(ref _target.distance_start, EditorGUILayout.Slider("Distance start", _target.distance_start, 0, 20));
				if (_target.distance_start_bumpglobal<_target.distance_start) _target.distance_start_bumpglobal=_target.distance_start;
				checkChange(ref _target.distance_transition, EditorGUILayout.Slider("Fade length", _target.distance_transition, 0, 50));
				#if !UNITY_3_5
					if (_target.useTerrainMaterial) {
						if (prev_distance_start!=_target.distance_start || prev_distance_transition!=_target.distance_transition) {
							Terrain[] terrainComps=(Terrain[])GameObject.FindObjectsOfType(typeof(Terrain));
							for(int ter=0; ter<terrainComps.Length; ter++) {
								if (_target.super_simple_active) {
									terrainComps[ter].basemapDistance=_target.distance_start_bumpglobal+_target.distance_transition_bumpglobal;
								} else {
									terrainComps[ter].basemapDistance=_target.distance_start+_target.distance_transition;
								}
							}
						}
					}
				#endif

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();	
				EditorGUILayout.HelpBox("Note that some texture lookups are MIP bias independent (use regular tex2D call)",MessageType.None, true);
				checkChange(ref _target.RTP_MIP_BIAS, EditorGUILayout.Slider("MIP level bias", _target.RTP_MIP_BIAS, -0.75f, 0.75f));
				EditorGUILayout.EndVertical();
					
				EditorGUILayout.Space();	
				
				#if !UNITY_3_5
					bool reflexDisabled = Camera.main.actualRenderingPath==RenderingPath.DeferredLighting || (Camera.main.actualRenderingPath==RenderingPath.UsePlayerSettings && PlayerSettings.renderingPath==RenderingPath.DeferredLighting);
					if (!reflexDisabled) {
						EditorGUILayout.HelpBox("In deferred rendering reflex lighting settings below are unavailable.",MessageType.Info, true);
					}
				#else
					bool reflexDisabled = false;
					EditorGUILayout.HelpBox("In deferred rendering reflex lighting settings below are unavailable.",MessageType.Info, true);
				#endif
				GUI.color=new Color(1,1,0.5f,1);						
				EditorGUILayout.LabelField("Shading color adjustements", EditorStyles.boldLabel);
				GUI.color=skin_color;
				EditorGUILayout.BeginVertical("Box");
				checkChange(ref _target._SpecColor, EditorGUILayout.ColorField("Specular Color", _target._SpecColor));
				checkChange(ref _target.rtp_customAmbientCorrection	, EditorGUILayout.ColorField("Ambient correction", _target.rtp_customAmbientCorrection));
				EditorGUI.BeginDisabledGroup(reflexDisabled);
				checkChange(ref _target.RTP_LightDefVector.x, EditorGUILayout.Slider("Negative light power", _target.RTP_LightDefVector.x,0,0.6f));
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();
					
				EditorGUILayout.Space();	
				GUI.color=new Color(1,1,0.5f,1);						
				EditorGUILayout.LabelField("Complementary ambience lighting", EditorStyles.boldLabel);
				GUI.color=skin_color;
				EditorGUILayout.BeginVertical("Box");
					
				EditorGUI.BeginDisabledGroup(reflexDisabled);
				checkChange(ref _target.RTP_LightDefVector.y, EditorGUILayout.Slider("Reflex diffuse softness", _target.RTP_LightDefVector.y,0,0.7f));
				checkChange(ref _target.RTP_ReflexLightDiffuseColor, EditorGUILayout.ColorField("Reflex light diffuse", _target.RTP_ReflexLightDiffuseColor));
				checkChange(ref _target.RTP_ReflexLightDiffuseColor.a, EditorGUILayout.Slider("Diffuse strength", _target.RTP_ReflexLightDiffuseColor.a,0,1));
				EditorGUILayout.Space();	
				//checkChange(ref _target.RTP_LightDefVector.z, EditorGUILayout.Slider("Reflex Specular Softness", _target.RTP_LightDefVector.z,0,1));
				checkChange(ref _target.RTP_LightDefVector.w, EditorGUILayout.Slider("Reflex specularity", _target.RTP_LightDefVector.w,2,40));
				checkChange(ref _target.RTP_ReflexLightSpecColor, EditorGUILayout.ColorField("Reflex light specular", _target.RTP_ReflexLightSpecColor));
				checkChange(ref _target.RTP_ReflexLightSpecColor.a, EditorGUILayout.Slider("Specular strength", _target.RTP_ReflexLightSpecColor.a,0,1));
				EditorGUILayout.EndVertical();
				EditorGUI.EndDisabledGroup();
					
				if (terrainComp) {
					if ((terrainComp.lightmapIndex!=255) && (terrainComp.lightmapIndex!=-1) && LightmapSettings.lightmapsMode!=LightmapsMode.SeparateDirectional) {
						EditorGUILayout.Space();	
						EditorGUILayout.BeginVertical("Box");
						EditorGUILayout.HelpBox("Helpful when lightmapping in single mode. Shader can mimic diffuse lighting with normal maps used. In dual lightmap mode (deferred) lower this value to avoid overbright at close distance (you might need put higher Perlin normal strength to compensate it at far distance). ",MessageType.None, true);
						checkChange(ref _target.LightmapShading, EditorGUILayout.Slider("Shading strength for lightmaps", _target.LightmapShading, 0, 2));
						EditorGUILayout.EndVertical();
					}
				} else {
					if ((_targetRT.GetComponent<Renderer>().lightmapIndex!=255) && (_targetRT.GetComponent<Renderer>().lightmapIndex!=-1) && LightmapSettings.lightmapsMode!=LightmapsMode.SeparateDirectional) {
						EditorGUILayout.Space();	
						EditorGUILayout.BeginVertical("Box");
						EditorGUILayout.HelpBox("Helpful when lightmapping in single mode. Shader can mimic diffuse lighting with normal maps used. In dual lightmap mode (deferred) lower this value to avoid overbright at close distance (you might need put higher Perlin normal strength to compensate it at far distance). ",MessageType.None, true);
						checkChange(ref _target.LightmapShading, EditorGUILayout.Slider("Shading strength for lightmaps", _target.LightmapShading, 0, 2));
						EditorGUILayout.EndVertical();
					}
				}
					
				EditorGUILayout.Space();	
				GUI.color=new Color(1,1,0.5f,1);						
				EditorGUILayout.LabelField("Heightblend AO", EditorStyles.boldLabel);
				GUI.color=skin_color;
				EditorGUILayout.BeginVertical("Box");
				checkChange(ref _target.RTP_AOsharpness, EditorGUILayout.Slider("Fake AO 2 HB sharpness", _target.RTP_AOsharpness, 0,10));
				checkChange(ref _target.RTP_AOamp, EditorGUILayout.Slider("Fake AO 2 HB value", _target.RTP_AOamp, 0,2));
				EditorGUILayout.EndVertical();
					
				// presets
				EditorGUILayout.Space();	
				GUI.color=new Color(1,1,0.5f,1);						
				EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
				GUI.color=skin_color;					
				EditorGUILayout.BeginVertical("Box");
				EditorGUILayout.HelpBox("Here you can save / restore object state in preset. You can also use use function InterpolatePresets(PresetID1, PresetID2, float t) via script to blend between numeric settings of 2 presets.",MessageType.Info, true);
				if (_targetRT.presetHolders!=null) {
					for(int k=0; k<_targetRT.presetHolders.Length; k++) {
						EditorGUILayout.BeginHorizontal();
						ReliefTerrainPresetHolder[] holders=null;
						if (GUILayout.Button("restore", GUILayout.Width(60))) {
							if (EditorUtility.DisplayDialog("Warning", "Restore to preset "+_targetRT.presetHolders[k].PresetName+" state ?","Yes","Cancel")) {
								Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
								_target.RestorePreset(_targetRT.presetHolders[k]);
								_targetRT.RefreshTextures();				
								_target.Refresh();									
								EditorUtility.SetDirty(_targetRT);
							}
						}
						if (GUILayout.Button("del", GUILayout.Width(35))) {
							if (EditorUtility.DisplayDialog("Warning", "Remove preset "+_targetRT.presetHolders[k].PresetName+" ?","Yes","Cancel")) {
								Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
								holders=new ReliefTerrainPresetHolder[_targetRT.presetHolders.Length-1];
								int l=0;
								for(int r=0; r<_targetRT.presetHolders.Length; r++) {
									if (r!=k) {
										holders[l++]=_targetRT.presetHolders[r];
									}
								}
							}
						}
						EditorGUILayout.LabelField("Preset " +k+": ", GUILayout.Width(60));
						EditorGUILayout.LabelField(_targetRT.presetHolders[k].PresetName, EditorStyles.boldLabel);
						
						if (holders!=null) {
							_targetRT.presetHolders=holders;
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUI.BeginDisabledGroup(_targetRT.presetHolders!=null && _targetRT.presetHolders.Length==8);
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("New preset name: ", GUILayout.Width(120));	
				_target.newPresetName=EditorGUILayout.TextField(_target.newPresetName);
				EditorGUILayout.EndHorizontal();
				if (GUILayout.Button("Add new preset")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						ReliefTerrainPresetHolder[] holders;
						int k=0;
						if (_targetRT.presetHolders!=null) {
							holders=new ReliefTerrainPresetHolder[_targetRT.presetHolders.Length+1];
							for(; k<_targetRT.presetHolders.Length; k++) {
								holders[k]=_targetRT.presetHolders[k];
							}
						} else {
							holders=new ReliefTerrainPresetHolder[1];
						}
						holders[k]=new ReliefTerrainPresetHolder(_target.newPresetName);
						_target.SavePreset(ref holders[k]);
						_targetRT.presetHolders=holders;
				}
				EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndVertical();						
				// EOF presets
				
				GUILayout.Space(10);
				
				{
					Color c=GUI.color;
					GUI.color=new Color(0.9f,1, 0.9f);
					if (GUILayout.Button(new GUIContent("Refresh all", "Refreshes all terrain object on the scene."), GUILayout.Height (40))) {
						_target.RefreshAll();
					}
					GUI.color=c;
				}
					
				GUILayout.Space(15);							
				if (GUILayout.Button("Reset main settings numeric params to default")) {
					Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
					_target.ReturnToDefaults("main");
					EditorUtility.SetDirty(_targetRT);
					_target.Refresh();
				}	

				#endregion Settings - main settings
	
			break;
					
			case ReliefTerrainSettingsItems.UVblend:
			
				#region Detail maps unfold - UV blend
				// UV blend on/off global properties
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoUVBlend),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);
					EditorGUILayout.LabelField("UV blending (tiling patterns reduction)", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
				if (UV_BLEND_ENABLED_FIRST || UV_BLEND_ENABLED_ADD) {
					EditorGUILayout.HelpBox("UV blend feature can be helpful to remove tiling patterns at mid/far distance.\nIt blends detail maps with themselves using different tiling.\n\nPer layer settings can be found in \"Layers properties\" tab.",MessageType.None, true);
					checkChange(ref _target.blendMultiplier, EditorGUILayout.Slider("Mix Multiplier", _target.blendMultiplier, 0, 2));
					if (GUILayout.Button("Disable UV blend (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_UV_BLEND_FIRST=false;
						_RTP_LODmanagerScript.RTP_UV_BLEND_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
					
					GUILayout.Space(15);							
					if (GUILayout.Button("Reset uv blend settings numeric params to default")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						_target.ReturnToDefaults("uvblend");
						EditorUtility.SetDirty(_targetRT);
						_target.Refresh();
					}								
				} else {
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable UV blend (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_UV_BLEND_FIRST=true;
						_RTP_LODmanagerScript.RTP_UV_BLEND_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
				}
				EditorGUILayout.EndVertical();

				#endregion Detail maps unfold - UV blend						
			break;
					
			case ReliefTerrainSettingsItems.POMSettings:
				#region Settings - POM
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoPOM),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);
					EditorGUILayout.LabelField("Parallax (POM/PM) settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
				//checkChange(ref _target.ExtrudeHeight, EditorGUILayout.Slider("POM/PM Extrude Height", _target.ExtrudeHeight*Mathf.Max(tile_sizex, tile_sizey), 0.001f, 0.3f)/Mathf.Max(tile_sizex, tile_sizey));
				checkChange(ref _target.ExtrudeHeight, EditorGUILayout.Slider("Extrude Height", _target.ExtrudeHeight, 0.001f, 0.3f));
					
				EditorGUILayout.LabelField("Color/position solver (used in POM only)");
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				EditorGUILayout.BeginVertical();				
				checkChange(ref _target.DIST_STEPS, EditorGUILayout.IntSlider("Max search steps", Mathf.FloorToInt(_target.DIST_STEPS), 4, 255));
				checkChange(ref _target.WAVELENGTH, EditorGUILayout.Slider("Step length (in texels)", _target.WAVELENGTH, 0.5f, 16));
				EditorGUILayout.EndVertical();				
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.LabelField("Self-shadow solver (used in POM only)");
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
				EditorGUILayout.BeginVertical();
				EditorGUILayout.HelpBox("To turn off self-shadowing in shader use RTP_LODmanager script (attach it to any scene object).",MessageType.None, true);
				checkChange(ref _target.SHADOW_STEPS, EditorGUILayout.IntSlider("Max search steps", Mathf.FloorToInt(_target.SHADOW_STEPS), 0, 80));
				checkChange(ref _target.WAVELENGTH_SHADOWS, EditorGUILayout.Slider("Step length (in texels)", _target.WAVELENGTH_SHADOWS, 0.5f, 16));
				checkChange(ref _target.SHADOW_SMOOTH_STEPS, EditorGUILayout.IntSlider("Max smoothing steps", Mathf.FloorToInt(_target.SHADOW_SMOOTH_STEPS), 3, 20));
				checkChange(ref _target.SelfShadowStrength, EditorGUILayout.Slider("Self-shadow strength", _target.SelfShadowStrength, 0.1f, 1));
				checkChange(ref _target.ShadowSmoothing, EditorGUILayout.Slider("Self-shadow smoothing", _target.ShadowSmoothing, 0.5f, 8));
				checkChange(ref _target.ShadowColor, EditorGUILayout.ColorField("Shadow colorization", _target.ShadowColor));
				EditorGUILayout.EndVertical();		
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
					
				GUILayout.Space(15);							
				if (GUILayout.Button("Reset POM/PM settings numeric params to default")) {
					Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
					_target.ReturnToDefaults("pom/pm");
					EditorUtility.SetDirty(_targetRT);
					_target.Refresh();
				}	
										
				#endregion Settings - POM
			break;
				
			case ReliefTerrainSettingsItems.GlobalColor:
				#region Settings - Global color
				_target.paint_wetmask=false;
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoGlobalcolor),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);
					EditorGUILayout.LabelField("Global color settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;

				EditorGUILayout.HelpBox("Color is blended via multiplication (easy to find/change in shader code - look for #ifdef COLOR_MAP).\n\nALPHA channel is used to remove locally parallax effects - extrude height (necessary for seamless geometry blending or when objects on the surface seem to hover on at the surface due to high extrude height).\n\nIf you don't use ColorMap - set below blending values to zero (you may use Alpha8 compressing w/o RGB channels, but still need alpha for parallax removing at blend areas - terrain vs mesh).",MessageType.None, true);
				EditorGUILayout.HelpBox("Near / Mid blend is realised at Main settings / Near distance values\n\nFar blend is realised at Perlin normal settings / start&fade distances.\n\nMIP level adjustement allows to take any MIP level below far distance (blended).", MessageType.None, true);
				EditorGUILayout.HelpBox("Note that saturation on far distance might work different when shader doesn't use detail/splat colors there, but only global colormap (refer to LODmanager settings - \"No detail colors at far distance\").", MessageType.None, true);
				EditorGUILayout.BeginHorizontal();
				Texture2D prev_globalColor=_targetRT.ColorGlobal;
				checkChange(ref _targetRT.ColorGlobal, (Texture2D)EditorGUILayout.ObjectField(_targetRT.ColorGlobal, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
				if (_targetRT.ColorGlobal && _targetRT.ColorGlobal!=prev_globalColor) {
					_targetRT.globalColorModifed_flag=false;
					_targetRT.tmp_globalColorMap=null;
					_targetRT.save_path_colormap=AssetDatabase.GetAssetPath(_targetRT.ColorGlobal);
					if (_targetRT.save_path_colormap=="") _targetRT.save_path_colormap="Assets/_output_colormap.png";
					_targetRT.RefreshTextures();
				}
				EditorGUILayout.BeginVertical();
					
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Near blend", GUILayout.MinWidth(80), GUILayout.MaxWidth(80));
					checkChange(ref _target.GlobalColorMapBlendValues.x, EditorGUILayout.Slider(_target.GlobalColorMapBlendValues.x, 0, 1));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Mid blend", GUILayout.MinWidth(80), GUILayout.MaxWidth(80));
					checkChange(ref _target.GlobalColorMapBlendValues.y, EditorGUILayout.Slider(_target.GlobalColorMapBlendValues.y, 0, 1));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Far blend", GUILayout.MinWidth(80), GUILayout.MaxWidth(80));
					checkChange(ref _target.GlobalColorMapBlendValues.z, EditorGUILayout.Slider(_target.GlobalColorMapBlendValues.z, 0, 1));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("MIP level below far", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
					checkChange(ref _target._GlobalColorMapNearMIP, EditorGUILayout.Slider(_target._GlobalColorMapNearMIP, 0, 10));
				EditorGUILayout.EndHorizontal();						
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Saturation", GUILayout.MinWidth(80), GUILayout.MaxWidth(80));
					checkChange(ref _target.GlobalColorMapSaturation, EditorGUILayout.Slider(_target.GlobalColorMapSaturation, 0, 1));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Brightness", GUILayout.MinWidth(80), GUILayout.MaxWidth(80));
					checkChange(ref _target.GlobalColorMapBrightness, EditorGUILayout.Slider(_target.GlobalColorMapBrightness, 0, 2));
				EditorGUILayout.EndHorizontal();
					
				EditorGUILayout.Space();
					
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
					
				if (_targetRT.ColorGlobal) {
					if (_targetRT.globalColorModifed_flag) {
						GUILayout.Space (8);
						if (GUILayout.Button("Colormap uncompressed & readable - save to file")) {
							string directory;
							string file;
							if (_targetRT.save_path_colormap=="") {
								directory=Application.dataPath;
								file="_output_colormap.png";
							} else {
								directory=Path.GetDirectoryName(_targetRT.save_path_colormap);
								file=Path.GetFileNameWithoutExtension(_targetRT.save_path_colormap)+".png";
							}
							string path = EditorUtility.SaveFilePanel("Save edited Global color map", directory, file, "png");
							if (path!="") {
								path=path.Substring(Application.dataPath.Length-6);
								_targetRT.save_path_colormap=path;
						 		byte[] bytes = _targetRT.ColorGlobal.EncodeToPNG();
							    System.IO.File.WriteAllBytes(path, bytes);
								AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
//									TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
//									textureImporter.textureFormat = TextureImporterFormat.ARGB32; 
//									textureImporter.isReadable = true;
//									AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
								_targetRT.ColorGlobal = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
								_targetRT.globalColorModifed_flag=false;
								_targetRT.tmp_globalColorMap=null;
							}										
						}
						GUILayout.Space (4);
					}
				}
					
					
				Color[] avg_cols=new Color[_target.numLayers];
				for(int n=0; n<_target.numLayers; n++) {
					Color[] col=new Color[1] { Color.black };
					Texture2D tex=_target.splats[n];
					string path=AssetDatabase.GetAssetPath(tex);
					if (path!="") {
						AssetImporter _importer=AssetImporter.GetAtPath(path);
						if (_importer) {
							TextureImporter tex_importer=(TextureImporter)_importer;
							if (!tex_importer.isReadable) {
								Debug.LogWarning("Texture ("+tex.name+") has been reimported as readable.");
								tex_importer.isReadable=true;
								AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tex),  ImportAssetOptions.ForceUpdate);
							}
						}							
					}
					col=tex.GetPixels(tex.mipmapCount-1);
					col[0].a=1;
					avg_cols[n]=col[0];
				}						
				if (GUILayout.Button("Construct colormap from detail colors of layers (below)")) {
					if (_targetRT.controlA!=null) {
						Color[] colorMap;
						Color[] cols;
						int u;
						cols=_targetRT.controlA.GetPixels(0);
						colorMap=new Color[cols.Length];
						for(u=0; u<cols.Length; u++) {
							colorMap[u]=new Color(0,0,0,0);
						}
						for(int k=0; k<_target.numLayers; k++) {
							if (k==4) {
								cols=_targetRT.controlB.GetPixels(0);
							} else if (k==8) {
								cols=_targetRT.controlC.GetPixels(0);
							}
							if (cols!=null) {
								for(u=0; u<cols.Length; u++) {
									colorMap[u]+=cols[u][k%4]*avg_cols[k];
								}
							}
						}
						_targetRT.ColorGlobal=new Texture2D(_targetRT.controlA.width, _targetRT.controlA.height, TextureFormat.ARGB32, true);
						_targetRT.ColorGlobal.SetPixels(colorMap,0);
						_targetRT.ColorGlobal.Apply(true);
						_targetRT.globalColorModifed_flag=true;
					}
				}
				EditorGUILayout.BeginHorizontal();
					int thumb_size=32;
					for(int n=0; n<_target.numLayers; n++) {
						Color ccol = GUI.contentColor;
						GUI.contentColor=avg_cols[n];
				        if (GUILayout.Button(_target.get_dumb_tex(), "Label", GUILayout.Width(thumb_size), GUILayout.Height(thumb_size))) {
							_target.paintColor=avg_cols[n];
						}
				        GUI.contentColor = ccol;
					}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();						
				
				int copy_splat_idx=0;
				if ( _4LAYERS_SHADER_USED && (_target.numLayers>4) )  {
					copy_splat_idx=1;
				} else if ( (!_4LAYERS_SHADER_USED) && (_target.numLayers>8) ) {
					copy_splat_idx=2;
				}
				if (copy_splat_idx>0) {
					EditorGUILayout.Space();
					EditorGUILayout.HelpBox("Due to parallax effects to get better transitions between AddPass and FirstPass areas you need to cancel extrusion there.\n\nNote, that any height painting already done by either geometry blend or this script will be overwritten.",MessageType.Warning, true);
					if (GUILayout.Button("Overlap coverage to alpha (flatten overlapped areas)")) {
						colorAlphaFromSplat(copy_splat_idx);	
					}
				}
				
				EditorGUILayout.EndVertical();
					
				//
				// paint tools
				//
				GUILayout.Space(8);
				EditorGUILayout.BeginVertical("Box");		
				skin_color=GUI.color;
				GUI.color=new Color(1,1,0.5f,1);
				EditorGUILayout.LabelField("Paint tools", EditorStyles.boldLabel);
				GUI.color=skin_color;
					
				bool prev_paint_flag=_target.paint_flag;
				
				if (!_target.paint_flag) {
					Color c=GUI.color;
					GUI.color=new Color(0.9f,1, 0.9f);
					if (GUILayout.Button(new GUIContent("Begin painting (M)",icoPaintOn, "Click to turn on painting"))) {
						_target.paint_flag=true;
						_target.paint_wetmask=false;
//							if (!_target.prepare_tmpTexture(true)) {
//								_target.paint_flag=false;
//							}
					}
					if (!_targetRT.GetComponent<Collider>() || !_targetRT.GetComponent<Collider>().enabled) _target.paint_flag=false;
					GUI.color=c;
				} else if (_target.paint_flag) {
					Color c=GUI.color;
					GUI.color=new Color(1,0.9f,0.9f);
					if (GUILayout.Button(new GUIContent("End painting (M)",icoPaintOff, "Click to turn off painting"))) {
						_target.paint_flag=false;
					}
					GUI.color=c;
				}
				if (!prev_paint_flag && _target.paint_flag) {
					Tools.current=Tool.View;
					ReliefTerrain._SceneGUI = new SceneView.OnSceneFunc(CustomOnSceneGUI);
					SceneView.onSceneGUIDelegate += ReliefTerrain._SceneGUI;
				} else if (prev_paint_flag && !_target.paint_flag) {
					Tools.current=prev_tool;
					SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;
				}
				if (prev_paint_flag!=_target.paint_flag) EditorUtility.SetDirty(target);
				if (_target.paint_flag) {
					if (!_targetRT.GetComponent<Collider>() || !_targetRT.GetComponent<Collider>().enabled) EditorGUILayout.HelpBox("Object doesn't have collider (necessary for painting).",MessageType.Error, true);
						
					string[] modes=new string[2] {"Color", "Extrude Height"};
					int npaint_mode=GUILayout.SelectionGrid(_target.paint_alpha_flag ? 1:0, modes, 2);
					if (npaint_mode!=(_target.paint_alpha_flag ? 1:0)) {
						EditorUtility.SetDirty(target);
					}
					_target.paint_alpha_flag = (npaint_mode==0) ? false : true;

					if (_target.paint_alpha_flag) {
						EditorGUILayout.HelpBox("Hold SHIFT while painting to apply eraser.",MessageType.Info, true);
					} else {
						EditorGUILayout.BeginVertical("Box");
						skin_color=GUI.color;
						GUI.color=new Color(1,1,0.5f,1);
						if (_target.paintColorSwatches.Length>0) EditorGUILayout.LabelField("Color swatches", EditorStyles.boldLabel);
						GUI.color=skin_color;
						EditorGUILayout.BeginHorizontal();
							thumb_size=16;
							for(int n=0; n<_target.paintColorSwatches.Length; n++) {
								Color ccol = GUI.contentColor;
								GUI.contentColor=_target.paintColorSwatches[n];
						        if (GUILayout.Button(_target.get_dumb_tex(), "Label", GUILayout.Width(thumb_size), GUILayout.Height(thumb_size))) {
									_target.paintColor=_target.paintColorSwatches[n];
								}
						        GUI.contentColor = ccol;
							}
						EditorGUILayout.EndHorizontal();								
						EditorGUILayout.BeginHorizontal();
							EditorGUI.BeginDisabledGroup(_target.paintColorSwatches.Length==12);
					        if (GUILayout.Button("Add swatch")) {
								Color[] ns=new Color[_target.paintColorSwatches.Length+1];
								for(int k=0; k<_target.paintColorSwatches.Length; k++) ns[k]=_target.paintColorSwatches[k];
								ns[ns.Length-1]=new Color(_target.paintColor.r, _target.paintColor.g, _target.paintColor.b, 1);
								_target.paintColorSwatches=ns;
							}
							EditorGUI.EndDisabledGroup();
							EditorGUI.BeginDisabledGroup(_target.paintColorSwatches.Length==0);
					        if (GUILayout.Button("Remove swatch")) {
								Color[] ns=new Color[_target.paintColorSwatches.Length-1];
								for(int k=0; k<ns.Length; k++) ns[k]=_target.paintColorSwatches[k];
								_target.paintColorSwatches=ns;
							}
							EditorGUI.EndDisabledGroup();
						EditorGUILayout.EndHorizontal();								
								
						_target.paintColor=EditorGUILayout.ColorField("Paint color", _target.paintColor);
						_target.preserveBrightness=EditorGUILayout.Toggle("Preserve luminosity", _target.preserveBrightness);
						EditorGUILayout.EndVertical();
					}
						
					Texture2D tmp_tex=_targetRT.tmp_globalColorMap;
					if (tmp_tex && (tmp_tex.format!=TextureFormat.Alpha8 && tmp_tex.format!=TextureFormat.ARGB32)) {
						EditorGUILayout.HelpBox("Global colormap need to be readable and uncompressed for painting.",MessageType.Error, true);
					}
					GUILayout.BeginHorizontal();
						GUILayout.Label ("Area size", EditorStyles.label );
						_target.paint_size = EditorGUILayout.Slider(_target.paint_size, 0.1f, 6);
					GUILayout.EndHorizontal();	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("Area smoothness", EditorStyles.label );
						_target.paint_smoothness = EditorGUILayout.Slider (_target.paint_smoothness, 0.001f, 1);
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label ("Opacity", EditorStyles.label );
						_target.paint_opacity = EditorGUILayout.Slider (_target.paint_opacity, 0, 1);
					GUILayout.EndHorizontal();	
					GUILayout.Space(10);
				}
				EditorGUILayout.EndVertical();	
					
				GUILayout.Space(10);
				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					GUI.color=new Color(1,1,0.5f,1);
					EditorGUILayout.LabelField("Global normal settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;

				if (RTP_NORMALGLOBAL_FIRST||RTP_NORMALGLOBAL_ADD) {
					EditorGUILayout.HelpBox("Using global normal below we won't use mesh normals - will be treated flat (0,1,0 ), but you can greately improve look when lo-res heightmap or high pixelError settings are used.",MessageType.None, true);
					if (GUILayout.Button("Disable global normal (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_NORMALGLOBAL_FIRST=false;
						_RTP_LODmanagerScript.RTP_NORMALGLOBAL_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}							
						
					EditorGUILayout.BeginHorizontal();
					checkChange(ref _targetRT.NormalGlobal, (Texture2D)EditorGUILayout.ObjectField(_targetRT.NormalGlobal, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					EditorGUILayout.BeginVertical();
						
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Normal strength", GUILayout.MinWidth(100), GUILayout.MaxWidth(100));
						checkChange(ref _target.global_normalMap_multiplier, EditorGUILayout.Slider(_target.global_normalMap_multiplier, 0.5f, 6f));
					EditorGUILayout.EndHorizontal();
						
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();	
				} else {
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable global normal (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_NORMALGLOBAL_FIRST=true;
						_RTP_LODmanagerScript.RTP_NORMALGLOBAL_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
				}
				EditorGUILayout.EndVertical();	
					
				GUILayout.Space(10);
					
				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					GUI.color=new Color(1,1,0.5f,1);
					EditorGUILayout.LabelField("Global trees/shadow settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;

				if (RTP_TREESGLOBAL_FIRST||RTP_TREESGLOBAL_ADD) {
					EditorGUILayout.HelpBox("RGB - trees color at very far distance (billboard trees drawing distance). Blending done via brightness, so black means - no pixel tree.\n\nA channel - shadows (mean to be used when you don't use lightmaps).",MessageType.None, true);
					if (GUILayout.Button("Disable global trees/shadows (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_TREESGLOBAL_FIRST=false;
						_RTP_LODmanagerScript.RTP_TREESGLOBAL_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}							
						
					EditorGUILayout.BeginHorizontal();
					checkChange(ref _targetRT.TreesGlobal, (Texture2D)EditorGUILayout.ObjectField(_targetRT.TreesGlobal, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					EditorGUILayout.BeginVertical();
						
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Trees distance", GUILayout.MinWidth(125), GUILayout.MaxWidth(125));
						checkChange(ref _target.trees_pixel_distance_start, EditorGUILayout.FloatField(_target.trees_pixel_distance_start));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Trees fade length", GUILayout.MinWidth(125), GUILayout.MaxWidth(125));
						checkChange(ref _target.trees_pixel_distance_transition, EditorGUILayout.Slider(_target.trees_pixel_distance_transition, 0, 100));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Trees blending", GUILayout.MinWidth(125), GUILayout.MaxWidth(125));
						checkChange(ref _target.trees_pixel_blend_val, EditorGUILayout.Slider(_target.trees_pixel_blend_val, 0, 10));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Shadows distance", GUILayout.MinWidth(125), GUILayout.MaxWidth(125));
						checkChange(ref _target.trees_shadow_distance_start, EditorGUILayout.FloatField(_target.trees_shadow_distance_start));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Shadows fade length", GUILayout.MinWidth(125), GUILayout.MaxWidth(125));
						checkChange(ref _target.trees_shadow_distance_transition, EditorGUILayout.Slider(_target.trees_shadow_distance_transition, 0, 30));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Shadows blending", GUILayout.MinWidth(125), GUILayout.MaxWidth(125));
						checkChange(ref _target.trees_shadow_value, 1-EditorGUILayout.Slider(1-_target.trees_shadow_value, 0, 1));
					EditorGUILayout.EndHorizontal();
						
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();	
				} else {
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable global trees/shadows (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_TREESGLOBAL_FIRST=true;
						_RTP_LODmanagerScript.RTP_TREESGLOBAL_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
				}
				EditorGUILayout.EndVertical();	
					
				//if (RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST || RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD) {
				if (RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST) {
					EditorGUILayout.BeginVertical("Box");
					skin_color=GUI.color;
					EditorGUILayout.BeginHorizontal();
						GUI.color=new Color(1,1,0.5f,1);
						EditorGUILayout.LabelField("Grayscale detail textures", EditorStyles.boldLabel);
					EditorGUILayout.EndHorizontal();
					GUI.color=skin_color;						
					EditorGUILayout.HelpBox("In massive terrain mode (refer to RTP LOD manager for 4 layers mode) you can use even simplier close-distance detail colors as grayscale and global colorMap to tint them. RGBA channels refers to grayscaled detail textures for layers.",MessageType.None, true);
					if (GUILayout.Button("Disable massive terain mode  & grayscale detail textures")) {
						//_RTP_LODmanagerScript.RTP_SUPER_SIMPLE=false;
						_RTP_LODmanagerScript.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST=false;
						//_RTP_LODmanagerScript.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}	
						
					EditorGUILayout.BeginHorizontal();
					checkChange(ref _target.SSColorCombined, (Texture2D)EditorGUILayout.ObjectField(_target.SSColorCombined, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					EditorGUILayout.BeginVertical();
						
					EditorGUILayout.BeginHorizontal();
						if (GUILayout.Button("Make combined grayscale texture")) {
							MakeCombinedGrayscale();
						}
					EditorGUILayout.EndHorizontal();
						
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();	
					SaveTexture(ref _target.SSColorCombined, ref _target.save_path_SSColorCombined, "terrain_detail_grayscale_combined.png", 100, TextureImporterFormat.DXT5, true);
						
//					} else {
//						EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
//						if (GUILayout.Button("Enable massive terain mode  & grayscale detail textures")) {
//							_RTP_LODmanagerScript.RTP_SUPER_SIMPLE=true;
//							_RTP_LODmanagerScript.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST=true;
//							//_RTP_LODmanagerScript.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD=true;
//							if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
//								Selection.activeObject=_RTP_LODmanagerScript.gameObject;
//								_RTP_LODmanagerScript.dont_sync=true;
//							}
//							//_RTP_LODmanagerScript.RefreshLODlevel();
//							//EditorUtility.SetDirty(_targetRT);
//						}
					EditorGUILayout.EndVertical();	
				}
					
				GUILayout.Space(15);							
				if (GUILayout.Button("Reset global color settings numeric params to default")) {
					Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
					_target.ReturnToDefaults("global_color");
					EditorUtility.SetDirty(_targetRT);
					_target.Refresh();
				}	
					
				#endregion Settings - Global color
			break;
				
			case ReliefTerrainSettingsItems.GlobalNormal:
				#region Settings - Global normal 
				GUILayout.Space(6);
;
				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoPerlinNormal),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);
					EditorGUILayout.LabelField("Perlin normal map settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
			
				EditorGUILayout.HelpBox("Feature below are applied per layer in \"Layer properties\".", MessageType.None, true);
				EditorGUILayout.HelpBox("REMEMBER - this map is also used for water flow animation, water/snow random coverage and superdetail normal mapping.\n\nFar normal damp flattens normals stored in vertices at far distance. It's useful in situation you're using lighting info baked into global colorMap (like in the example scene).", MessageType.None, true);
				
				EditorGUILayout.BeginHorizontal();
				Texture2D tex_bump=_target.BumpGlobal;
				Texture2D ntex=tex_bump;
				checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_target.BumpGlobal, typeof(Texture2D), false, GUILayout.MinHeight(90), GUILayout.MinWidth(90), GUILayout.MaxWidth(90)));
				if (ntex==null) {
					ntex=(Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Textures/ReliefTerrain/perlin_noise.png", typeof(Texture2D));
					_targetRT.TERRAIN_WetMask=null;
					_target.TERRAIN_ReflectionMap=null;
					_target.SuperDetailA=null;
					_target.SuperDetailB=null;
				}
				if (ntex!=tex_bump) {
					Texture2D tex_prev=_target.BumpGlobal;
					_target.BumpGlobal=ntex;
					if (ntex) {
						AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
						if (_importer) {
							TextureImporter tex_importer=(TextureImporter)_importer;
							if (!tex_importer.isReadable) {
								Debug.LogWarning("Global normal texture ("+ntex.name+") has been reimported as readable.");
								tex_importer.isReadable=true;
								AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
							}
						}
					}
					if (!_targetRT.PrepareGlobalNormalsAndSuperDetails()) {
						Debug.LogWarning("Due to rebuilding special combined texture superdetail textures, water mask and reflection map have been cleared (you have to reassign them again).");
						_target.BumpGlobal=tex_prev;
						_targetRT.TERRAIN_WetMask=null;
						_target.TERRAIN_ReflectionMap=null;
						_target.SuperDetailA=null;
						_target.SuperDetailB=null;
						if (_targetRT.PrepareGlobalNormalsAndSuperDetails()) {
							_target.BumpGlobal=ntex;
						}
					}
					_target.Refresh();
				}
				EditorGUILayout.BeginVertical();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Tiling scale ("+(Mathf.Round(_target.ReliefTransform.x*_target.BumpMapGlobalScale*100)/100)+")", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
					EditorGUILayout.BeginHorizontal();
						checkChange(ref _target.BumpMapGlobalScale, EditorGUILayout.Slider(_target.BumpMapGlobalScale, 0.01f, 0.25f));
						if (GUILayout.Button(new GUIContent("A","Align to terrain tile"), GUILayout.Width(22), GUILayout.Height(16))) {
							float ratio=Mathf.Round(_target.ReliefTransform.x*_target.BumpMapGlobalScale);
							_target.BumpMapGlobalScale=ratio/_target.ReliefTransform.x;
							EditorUtility.SetDirty(_targetRT);
						}
					EditorGUILayout.EndHorizontal();						
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("MIP offset", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
					checkChange(ref _target.rtp_mipoffset_globalnorm, EditorGUILayout.IntSlider(_target.rtp_mipoffset_globalnorm, 0, 5));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Distance start", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
					checkChange(ref _target.distance_start_bumpglobal, EditorGUILayout.Slider(_target.distance_start_bumpglobal, 0, 100));
					if (_target.distance_start_bumpglobal<_target.distance_start) _target.distance_start_bumpglobal=_target.distance_start;
				EditorGUILayout.EndHorizontal();
					
				EditorGUI.BeginDisabledGroup( _target.distance_start_bumpglobal>0 );
				if (_target.distance_start_bumpglobal>0) _target.rtp_perlin_start_val=0;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Starting value", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
					checkChange(ref _target.rtp_perlin_start_val, EditorGUILayout.Slider(_target.rtp_perlin_start_val, 0, 1.0f));
				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
					
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Fade length", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
					checkChange(ref _target.distance_transition_bumpglobal, EditorGUILayout.Slider(_target.distance_transition_bumpglobal, 0, 300));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Far normal damp", GUILayout.MinWidth(120), GUILayout.MaxWidth(120));
					checkChange(ref _target._FarNormalDamp, EditorGUILayout.Slider(_target._FarNormalDamp, 0, 1));
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.EndVertical();
					
				GUILayout.Space(15);							
				if (GUILayout.Button("Reset perlin normal settings numeric params to default")) {
					Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
					_target.ReturnToDefaults("perlin");
					EditorUtility.SetDirty(_targetRT);
					_target.Refresh();
				}		
					
				#endregion Settings - Global normal	
			break;					
				
			case ReliefTerrainSettingsItems.Superdetail:
				#region Settings - superdetail					
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoSuperdetail),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);						
					EditorGUILayout.LabelField("Superdetail settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
				
				if (SUPER_DETAIL_ENABLED_FIRST || SUPER_DETAIL_ENABLED_ADD) {
					EditorGUILayout.HelpBox("Superdetail is texture consisting of Perlin normalmap and optional 2 additional multiplicative channels. They're shared on combined texture with water mask and reflection map, so CAN NOT be used (with water/reflection) at the same time.\n\nSuperdetail normal is applied only on near distance.", MessageType.None, true);
						
					if (GUILayout.Button("Disable superdetails (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_SUPER_DETAIL_FIRST=false;
						_RTP_LODmanagerScript.RTP_SUPER_DETAIL_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
					
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Superdetail tiling scale", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._SuperDetailTiling, EditorGUILayout.IntSlider(Mathf.RoundToInt(_target._SuperDetailTiling), 1, 16));
					EditorGUILayout.EndHorizontal();
					
					if (SUPER_DETAIL_MULTS_ENABLED_FIRST || SUPER_DETAIL_MULTS_ENABLED_ADD) {
						GUILayout.Space(8);
						EditorGUILayout.HelpBox("Superdetail multiplicative channles can be applied on both near and far distances with optional self-masking (works the way multiplicative channel masks itself using Perlin normal tiling).\n\nColor multiplication is realised classic way (color x mult_channel x 2) giving opportunity to make it lighten/darken (grey texture gives neutral effect).\n\nOn far distance selfmasking set to 0 means we've got no blending (otherwise we would see distinct superdetail patterns over large areas).", MessageType.None, true);
						EditorGUILayout.BeginHorizontal();
						Texture2D tmp_tex=_target.SuperDetailA;
						ntex=tmp_tex;
						checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_target.SuperDetailA, typeof(Texture2D), false, GUILayout.MinHeight(90), GUILayout.MinWidth(90), GUILayout.MaxWidth(90)));
						if (ntex!=tmp_tex) {
							Texture2D tex_prev=_target.SuperDetailA;
							_target.SuperDetailA=ntex;
							if (ntex) {
								AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
								if (_importer) {
									TextureImporter tex_importer=(TextureImporter)_importer;
									if (!tex_importer.isReadable) {
										Debug.LogWarning("Superdetail 1st texture ("+ntex.name+") has been reimported as readable.");
										tex_importer.isReadable=true;
										AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
									}
								}
							}
							if (!_targetRT.PrepareGlobalNormalsAndSuperDetails()) _target.SuperDetailA=tex_prev;
							_target.Refresh();
						}
						ColorChannels nchannel=ColorChannels.R;
						EditorGUILayout.LabelField("channel", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
						checkChange(ref nchannel, (ColorChannels)EditorGUILayout.EnumPopup(_target.SuperDetailA_channel));
						if (nchannel!=_target.SuperDetailA_channel) {
							_target.SuperDetailA_channel=nchannel;
							_targetRT.PrepareGlobalNormalsAndSuperDetails();
						}
						EditorGUILayout.EndHorizontal();
						
						EditorGUILayout.Space();
						EditorGUILayout.BeginHorizontal();
						tmp_tex=_target.SuperDetailB;
						ntex=tmp_tex;
						checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_target.SuperDetailB, typeof(Texture2D), false, GUILayout.MinHeight(90), GUILayout.MinWidth(90), GUILayout.MaxWidth(90)));
						if (ntex!=tmp_tex) {
							Texture2D tex_prev=_target.SuperDetailB;
							_target.SuperDetailB=ntex;
							if (ntex) {
								AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
								if (_importer) {
									TextureImporter tex_importer=(TextureImporter)_importer;
									if (!tex_importer.isReadable) {
										Debug.LogWarning("Superdetail 2st texture ("+ntex.name+") has been reimported as readable.");
										tex_importer.isReadable=true;
										AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
									}
								}
							}
							if (!_targetRT.PrepareGlobalNormalsAndSuperDetails()) _target.SuperDetailB=tex_prev;
							_target.Refresh();
						}
						EditorGUILayout.LabelField("channel", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
						checkChange(ref nchannel, (ColorChannels)EditorGUILayout.EnumPopup(_target.SuperDetailB_channel));
						if (nchannel!=_target.SuperDetailB_channel) {
							_target.SuperDetailB_channel=nchannel;
							_targetRT.PrepareGlobalNormalsAndSuperDetails();
						}
						EditorGUILayout.EndHorizontal();
					} // superdetail mults
					
					if (SUPER_DETAIL_MULTS_ENABLED_FIRST || SUPER_DETAIL_MULTS_ENABLED_ADD) {
						if (GUILayout.Button("Disable superdetails mult (rebuild shaders via RTP_LODmanager)")) {
							_RTP_LODmanagerScript.RTP_SUPER_DETAIL_MULTS_FIRST=false;
							_RTP_LODmanagerScript.RTP_SUPER_DETAIL_MULTS_ADD=false;
							if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
								Selection.activeObject=_RTP_LODmanagerScript.gameObject;
								_RTP_LODmanagerScript.dont_sync=true;
							}
							//_RTP_LODmanagerScript.RefreshLODlevel();
							//EditorUtility.SetDirty(_targetRT);
						}
					} else {
						EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
						if (GUILayout.Button("Enable superdetails mult (rebuild shaders via RTP_LODmanager)")) {
							_RTP_LODmanagerScript.RTP_SUPER_DETAIL_MULTS_FIRST=true;
							_RTP_LODmanagerScript.RTP_SUPER_DETAIL_MULTS_ADD=true;
							if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
								Selection.activeObject=_RTP_LODmanagerScript.gameObject;
								_RTP_LODmanagerScript.dont_sync=true;
							}
							//_RTP_LODmanagerScript.RefreshLODlevel();
							//EditorUtility.SetDirty(_targetRT);
						}
					}		
					
					GUILayout.Space(15);							
					if (GUILayout.Button("Reset superdetail settings numeric params to default")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						_target.ReturnToDefaults("superdetail");
						EditorUtility.SetDirty(_targetRT);
						_target.Refresh();							
					}	
						
				} else {
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable superdetails (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_SUPER_DETAIL_FIRST=true;
						_RTP_LODmanagerScript.RTP_SUPER_DETAIL_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
				}
				
				EditorGUILayout.EndVertical();
				
				#endregion Settings - superdetail					
			break;	
			case ReliefTerrainSettingsItems.VerticalTex:
				#region Settings - Vertical texture		
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoVerticalTexture),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);						
					EditorGUILayout.LabelField("Vertical texturing", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
				
				if (	RTP_VERTICAL_TEXTURE_FIRST || RTP_VERTICAL_TEXTURE_ADD) {
					EditorGUILayout.HelpBox("This texture is applied along world y axis using V coord. Pixel color is multiplied, controlled per layer (\"Layer properties\" - Vertical map strength).",MessageType.None, true);
					
					EditorGUILayout.BeginHorizontal();
					checkChange(ref _target.VerticalTexture, (Texture2D)EditorGUILayout.ObjectField(_target.VerticalTexture, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					EditorGUILayout.BeginVertical();
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Tiling [m]", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
							checkChange(ref _target.VerticalTextureTiling, EditorGUILayout.FloatField(_target.VerticalTextureTiling));
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField("Offset via Perlin normal", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
							checkChange(ref _target.VerticalTextureGlobalBumpInfluence, EditorGUILayout.Slider(_target.VerticalTextureGlobalBumpInfluence, 0, 0.1f));
						EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
				} // v-tex
				
				if (		RTP_VERTICAL_TEXTURE_FIRST || RTP_VERTICAL_TEXTURE_ADD) {
					if (GUILayout.Button("Disable feature (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_VERTICAL_TEXTURE_FIRST=false;
						_RTP_LODmanagerScript.RTP_VERTICAL_TEXTURE_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
					
					GUILayout.Space(15);							
					if (GUILayout.Button("Reset vertical texture settings numeric params to default")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						_target.ReturnToDefaults("vertical");
						EditorUtility.SetDirty(_targetRT);
						_target.Refresh();
					}	
						
				} else {
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable feature (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_VERTICAL_TEXTURE_FIRST=true;
						_RTP_LODmanagerScript.RTP_VERTICAL_TEXTURE_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
						//_RTP_LODmanagerScript.RefreshLODlevel();
						//EditorUtility.SetDirty(_targetRT);
					}
				}
				
				EditorGUILayout.EndVertical();
					
				#endregion Settings - Vertical texture
			break;
				
			case ReliefTerrainSettingsItems.Snow:
				#region Settings - Snow
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoSnow),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);						
					EditorGUILayout.LabelField("Snow settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
				
				if (SNOW_ENABLED_FIRST || SNOW_ENABLED_ADD) {
#if !UNITY_3_5
					EditorGUILayout.HelpBox("Snow settings are set globally and its params are visible in any shader using RTP dynamic snow feature (Bonus snow shaders). This also means that you can't use different snow settings on multiple terrains (Unity4).",MessageType.Warning, true);
#endif
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Strength", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_strength, EditorGUILayout.Slider(_target._snow_strength, 0, 1));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.HelpBox("Global color influence means the darker is global color the more snow is present.",MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Global color influence", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._global_color_brightness_to_snow, EditorGUILayout.Slider(_target._global_color_brightness_to_snow, 0, 0.3f));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.HelpBox("Reduction by slope is factor for decreasing snow coverage on slopes.",MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Reduction by slope", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_slope_factor, EditorGUILayout.Slider(_target._snow_slope_factor, 0, 10f));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.HelpBox("Height threshold/transition allows to put snow on choosen height only (i.e. high mountais).",MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Height threshold [units]", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_height_treshold, EditorGUILayout.Slider(_target._snow_height_treshold, -200, 7000));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Height transition", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_height_transition, EditorGUILayout.Slider(_target._snow_height_transition, 1, 4000));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space ();
					
					EditorGUILayout.HelpBox("Color / spec / gloss are used for far distance.\nFor near distance you can choose a layer to get its color/normalmap as snow (refer to _RTP_LODmanager game object inspector).",MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Color", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_color, EditorGUILayout.ColorField(_target._snow_color));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Specular", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_specular, EditorGUILayout.Slider(_target._snow_specular, 0.01f, 1f));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Gloss", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_gloss, EditorGUILayout.Slider(_target._snow_gloss, 0.01f, 1f));
					EditorGUILayout.EndHorizontal();
					EditorGUI.BeginDisabledGroup( !(REFLECTION_ENABLED_FIRST || REFLECTION_ENABLED_ADD));
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Reflection", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_reflectivness, EditorGUILayout.Slider(_target._snow_reflectivness, 0.01f, 1f));
					EditorGUILayout.EndHorizontal();
					EditorGUI.EndDisabledGroup();

					EditorGUILayout.Space ();
			
					EditorGUILayout.HelpBox("Edges definition makes snow transition smooth (snow color gradualy blends underlying surface) or sharp.",MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Edges definition", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_edge_definition, EditorGUILayout.Slider(_target._snow_edge_definition, 0.5f, 20f));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Space ();
					EditorGUILayout.HelpBox("Deep factor adjusts how fast covered surface will get dedicated normals (from texture) and extrusion height canceled (in POM/PM shading).",MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Deep factor", GUILayout.MinWidth(140), GUILayout.MaxWidth(140));
						checkChange(ref _target._snow_deep_factor, EditorGUILayout.Slider(_target._snow_deep_factor, 0, 6));
					EditorGUILayout.EndHorizontal();
				
				} // snow
				EditorGUILayout.Space ();
				
				if (SNOW_ENABLED_FIRST || SNOW_ENABLED_ADD) {
					if (GUILayout.Button("Disable snow (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_SNOW_FIRST=false;
						_RTP_LODmanagerScript.RTP_SNOW_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
					
					GUILayout.Space(15);							
					if (GUILayout.Button("Reset snow settings numeric params to default")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						_target.ReturnToDefaults("snow");
						EditorUtility.SetDirty(_targetRT);
						_target.Refresh();
					}	
						
				} else {
					_target._snow_strength=1;
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable snow (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_SNOW_FIRST=true;
						_RTP_LODmanagerScript.RTP_SNOW_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
				}					
				
				EditorGUILayout.EndVertical();
					
				#endregion Settings - Snow
			break;					
				
			case ReliefTerrainSettingsItems.Water:
				#region Settings - Water
				_target.paint_wetmask=true;
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoWater),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);						
					EditorGUILayout.LabelField("Water settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;

				if (WATER_ENABLED_FIRST || WATER_ENABLED_ADD) {
					EditorGUILayout.HelpBox("Water has 3 states due to wetness applied - diffuse color darken for initial water, then surface becomes wet, then water may accumulate in lower parts (lower in terms of heightmap applied).\n\nReflection is taken from reflection map (the same as in Reflection settings). Most params are applied per layer.",MessageType.None, true);
					checkChange(ref _target.TERRAIN_GlobalWetness, EditorGUILayout.Slider("Global wetness", _target.TERRAIN_GlobalWetness, 0, 1));
					checkChange(ref _target.TERRAIN_WaterSpecularity, EditorGUILayout.Slider("Water specularity", _target.TERRAIN_WaterSpecularity, 0.01f, 1f));
					checkChange(ref _target.TERRAIN_FlowSpeed, EditorGUILayout.Slider("Flow speed", _target.TERRAIN_FlowSpeed, 0, 2f));
					EditorGUILayout.BeginHorizontal();
						checkChange(ref _target.TERRAIN_FlowScale, EditorGUILayout.Slider("Flow tex tiling ("+(Mathf.Round(_target.ReliefTransform.x*_target.TERRAIN_FlowScale*100)/100)+")", _target.TERRAIN_FlowScale, 0.25f, 8f));
						if (GUILayout.Button(new GUIContent("A","Align to terrain tile"), GUILayout.Width(22), GUILayout.Height(16))) {
							float ratio=Mathf.Round(_target.ReliefTransform.x*_target.TERRAIN_FlowScale);
							_target.TERRAIN_FlowScale=ratio/_target.ReliefTransform.x;
							EditorUtility.SetDirty(_targetRT);
						}
					EditorGUILayout.EndHorizontal();
					checkChange(ref _target.TERRAIN_FlowMipOffset, EditorGUILayout.Slider("Flow tex filter", _target.TERRAIN_FlowMipOffset/4f, 0, 1)*4f);
					checkChange(ref _target.TERRAIN_mipoffset_flowSpeed, EditorGUILayout.Slider("Filter by flow speed", _target.TERRAIN_mipoffset_flowSpeed/4f, 0, 1)*4);
					checkChange(ref _target.TERRAIN_WetDarkening, EditorGUILayout.Slider("Water surface darkening", _target.TERRAIN_WetDarkening, 0.1f, 0.9f));
					EditorGUILayout.HelpBox("You can exclude any parts from wetness - shelter mask is put on derived/combined texture channel B together with Perlin normal (RG) and reflection map (A) (see Combined textures in RTP main inspector menu tab).\n\nWhen you don't specify texture below it will be treated black(full water).\n\nWhen you start painting the mask will be automatically made for you (with size of Special combined texture).",MessageType.None, true);

					Texture2D tmp_tex=_targetRT.TERRAIN_WetMask;
					ntex=tmp_tex;
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();
					if (checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_targetRT.TERRAIN_WetMask, typeof(Texture2D), false, GUILayout.MinHeight(140), GUILayout.MinWidth(140), GUILayout.MaxWidth(140)))) {
						_targetRT.RefreshTextures();
					}
					if (SaveTexture(ref _targetRT.TERRAIN_WetMask, ref _targetRT.save_path_WetMask, "terrain_wetmask.png", 140, TextureImporterFormat.Alpha8, true)) {
						_targetRT.globalWaterModifed_flag=false;
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.BeginVertical();
						int fill_flag=0;
						if (GUILayout.Button("Dry all terrain")) {
							fill_flag=1;
							_targetRT.RefreshTextures();
						}
						if (GUILayout.Button("Fill all terrain with water")) {
							fill_flag=2;
							_targetRT.RefreshTextures();
						}
						if (fill_flag>0) {
							if (_targetRT.TERRAIN_WetMask) DestroyImmediate(_targetRT.TERRAIN_WetMask, false);
							if (_targetRT.BumpGlobalCombined) {
								_targetRT.TERRAIN_WetMask=new Texture2D(_targetRT.BumpGlobalCombined.width, _targetRT.BumpGlobalCombined.height, TextureFormat.Alpha8, false);
							} else if (_target.TERRAIN_ReflectionMap) {
								_targetRT.TERRAIN_WetMask=new Texture2D(_target.TERRAIN_ReflectionMap.width, _target.TERRAIN_ReflectionMap.height, TextureFormat.Alpha8, false);
							} else {
								_targetRT.TERRAIN_WetMask=new Texture2D(1024, 1024, TextureFormat.Alpha8, false);
							}
							Color32[] cols=new Color32[_targetRT.TERRAIN_WetMask.width*_targetRT.TERRAIN_WetMask.height];
							if (fill_flag==1) {
								for(int k=0; k<cols.Length; k++) cols[k]=Color.white;
							}
							_targetRT.TERRAIN_WetMask.SetPixels32(cols,0);
							_targetRT.TERRAIN_WetMask.Apply(true,false);
							_targetRT.globalWaterModifed_flag=true;
							_targetRT.PrepareGlobalNormalsAndSuperDetails(true,false);
						}
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
					if (ntex!=tmp_tex) {
						Texture2D tex_prev=_targetRT.TERRAIN_WetMask;
						_targetRT.TERRAIN_WetMask=ntex;
						if (ntex) {
							AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
							if (_importer) {
								TextureImporter tex_importer=(TextureImporter)_importer;
								bool modified_flag=false;
								if (!tex_importer.isReadable) {
									Debug.LogWarning("Wet mask texture ("+ntex.name+") has been reimported as readable.");
									tex_importer.isReadable=true;
									modified_flag=true;
								}
								if (tex_importer.textureFormat!=TextureImporterFormat.Alpha8) {
									Debug.LogWarning("Wet mask texture ("+ntex.name+") has been reimported as Alpha8.");
									tex_importer.grayscaleToAlpha=true;
									tex_importer.textureFormat=TextureImporterFormat.Alpha8;
									modified_flag=true;
								}
								if (modified_flag) {
									AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
								}	
							}
						}
						if (!_targetRT.PrepareGlobalNormalsAndSuperDetails(true,false)) _targetRT.TERRAIN_WetMask=tex_prev;
						_target.Refresh();
					}
						
					//
					// paint tools
					//						
					GUILayout.Space(8);
					EditorGUILayout.BeginVertical("Box");		
					skin_color=GUI.color;
					GUI.color=new Color(1,1,0.5f,1);
					EditorGUILayout.LabelField("Wetmask paint tool", EditorStyles.boldLabel);
					GUI.color=skin_color;
						
					prev_paint_flag=_target.paint_flag;
					
					if (!_target.paint_flag) {
						Color c=GUI.color;
						GUI.color=new Color(0.9f,1, 0.9f);
						if (GUILayout.Button(new GUIContent("Begin painting (M)",icoPaintOn, "Click to turn on painting"))) {
							_target.paint_flag=true;
							_target.paint_wetmask=true;
							_target.paint_alpha_flag=true;
							if (!_targetRT.TERRAIN_WetMask) {
								if (_targetRT.BumpGlobalCombined) {
									_targetRT.TERRAIN_WetMask=new Texture2D(_targetRT.BumpGlobalCombined.width, _targetRT.BumpGlobalCombined.height, TextureFormat.Alpha8, false);
								} else if (_target.TERRAIN_ReflectionMap) {
									_targetRT.TERRAIN_WetMask=new Texture2D(_target.TERRAIN_ReflectionMap.width, _target.TERRAIN_ReflectionMap.height, TextureFormat.Alpha8, false);
								} else {
									_targetRT.TERRAIN_WetMask=new Texture2D(1024, 1024, TextureFormat.Alpha8, false);
								}
								Color32[] cols=new Color32[_targetRT.TERRAIN_WetMask.width*_targetRT.TERRAIN_WetMask.height];
								_targetRT.TERRAIN_WetMask.SetPixels32(cols,0);
								_targetRT.TERRAIN_WetMask.Apply(true,false);
							}
							if (!_targetRT.BumpGlobalCombined) {
								if (!_targetRT.PrepareGlobalNormalsAndSuperDetails(true, false)) {
									_target.paint_flag=false;
								}
							}
						}
						if (!_targetRT.GetComponent<Collider>() || !_targetRT.GetComponent<Collider>().enabled) _target.paint_flag=false;
						GUI.color=c;
					} else if (_target.paint_flag) {
						Color c=GUI.color;
						GUI.color=new Color(1,0.9f,0.9f);
						if (GUILayout.Button(new GUIContent("End painting (M)",icoPaintOff, "Click to turn off painting"))) {
							_target.paint_flag=false;
						}
						GUI.color=c;
					}
					if (!prev_paint_flag && _target.paint_flag) {
						Tools.current=Tool.View;
						ReliefTerrain._SceneGUI = new SceneView.OnSceneFunc(CustomOnSceneGUI);
						SceneView.onSceneGUIDelegate += ReliefTerrain._SceneGUI;
					} else if (prev_paint_flag && !_target.paint_flag) {
						Tools.current=prev_tool;
						SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;
					}
					if (prev_paint_flag!=_target.paint_flag) EditorUtility.SetDirty(target);
					if (_target.paint_flag) {
						EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("", GUILayout.MaxWidth(10));
						EditorGUILayout.BeginVertical();			
						
						if (!_targetRT.GetComponent<Collider>() || !_targetRT.GetComponent<Collider>().enabled) EditorGUILayout.HelpBox("Object doesn't have collider (necessary for painting).",MessageType.Error, true);
							
						EditorGUILayout.HelpBox("Hold SHIFT while painting to add water.",MessageType.Info, true);
							
						if (_targetRT.tmp_WaterMap && _targetRT.tmp_WaterMap.format!=TextureFormat.Alpha8) {
							EditorGUILayout.HelpBox("Wet mask need to be readable and uncompressed for painting.",MessageType.Error, true);
						}
						if (_targetRT.tmp_CombinedMap && _targetRT.tmp_CombinedMap.format!=TextureFormat.ARGB32) {
							EditorGUILayout.HelpBox("Special combined map need to be readable and uncompressed for painting.",MessageType.Error, true);
						}
						GUILayout.BeginHorizontal();
							GUILayout.Label ("Area size", EditorStyles.label );
							_target.paint_size = EditorGUILayout.Slider(_target.paint_size, 0.1f, 6);
						GUILayout.EndHorizontal();	
						GUILayout.BeginHorizontal();
							GUILayout.Label ("Area smoothness", EditorStyles.label );
							_target.paint_smoothness = EditorGUILayout.Slider (_target.paint_smoothness, 0.001f, 1);
						GUILayout.EndHorizontal();
						GUILayout.BeginHorizontal();
							GUILayout.Label ("Opacity", EditorStyles.label );
							_target.paint_opacity = EditorGUILayout.Slider (_target.paint_opacity, 0, 1);
						GUILayout.EndHorizontal();	
			
						EditorGUILayout.EndVertical();
						EditorGUILayout.EndHorizontal();
						GUILayout.Space(10);
					}
					EditorGUILayout.EndVertical();	
				
					EditorGUILayout.HelpBox("Below feature simulates drops on the water surface. Texture used for animation is predefined so you don't have to touch it. Ripples appears on accumulated water surface, for wet surface only when \"Rain on wet\" is positive. Ripples can be also damped on slopes via \"Water level slope damp\" layer param.",MessageType.None, true);
					if (RIPPLEMAP_ENABLED_FIRST || RIPPLEMAP_ENABLED_ADD) {
						if (GUILayout.Button("Disable droplets anim (rebuild shaders via RTP_LODmanager)")) {
							_RTP_LODmanagerScript.RTP_WET_RIPPLE_TEXTURE_FIRST=false;
							_RTP_LODmanagerScript.RTP_WET_RIPPLE_TEXTURE_ADD=false;
							if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
								Selection.activeObject=_RTP_LODmanagerScript.gameObject;
								_RTP_LODmanagerScript.dont_sync=true;
							}
						}
					} else {
						//EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
						if (GUILayout.Button("Enable droplets anim (rebuild shaders via RTP_LODmanager)")) {
							_RTP_LODmanagerScript.RTP_WET_RIPPLE_TEXTURE_FIRST=true;
							_RTP_LODmanagerScript.RTP_WET_RIPPLE_TEXTURE_ADD=true;
							if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
								Selection.activeObject=_RTP_LODmanagerScript.gameObject;
								_RTP_LODmanagerScript.dont_sync=true;
							}
						}
					}						
					EditorGUI.BeginDisabledGroup( !(RIPPLEMAP_ENABLED_FIRST || RIPPLEMAP_ENABLED_ADD));
						checkChange(ref _target.TERRAIN_RainIntensity, EditorGUILayout.Slider("Rain intensity", _target.TERRAIN_RainIntensity, 0, 1f));
						checkChange(ref _target.TERRAIN_WetDropletsStrength, EditorGUILayout.Slider("Rain on wet", _target.TERRAIN_WetDropletsStrength, 0, 1));
						checkChange(ref _target.TERRAIN_DropletsSpeed, EditorGUILayout.Slider("Anim speed", _target.TERRAIN_DropletsSpeed, 1, 30f));
						EditorGUILayout.BeginHorizontal();
							checkChange(ref _target.TERRAIN_RippleScale, EditorGUILayout.Slider("Ripple tex tiling ("+(Mathf.Round(_target.ReliefTransform.x*_target.TERRAIN_RippleScale*100)/100)+")", _target.TERRAIN_RippleScale, 0.25f, 8));
							if (GUILayout.Button(new GUIContent("A","Align to terrain tile"), GUILayout.Width(22), GUILayout.Height(16))) {
								float ratio=Mathf.Round(_target.ReliefTransform.x*_target.TERRAIN_RippleScale);
								_target.TERRAIN_RippleScale=ratio/_target.ReliefTransform.x;
								EditorUtility.SetDirty(_targetRT);
							}
						EditorGUILayout.EndHorizontal();						
						if ((RIPPLEMAP_ENABLED_FIRST || RIPPLEMAP_ENABLED_ADD) && _target.TERRAIN_RippleMap==null) {
							_target.TERRAIN_RippleMap=(Texture2D)AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Textures/ReliefTerrain/water_drops_anim.png", typeof(Texture2D));
						}
						checkChange(ref _target.TERRAIN_RippleMap, (Texture2D)EditorGUILayout.ObjectField(_target.TERRAIN_RippleMap, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
					EditorGUI.EndDisabledGroup();
				} else {
					_target.TERRAIN_GlobalWetness=1;
				}
					
				GUILayout.Space(15);
				if (WATER_ENABLED_FIRST || WATER_ENABLED_ADD) {
					if (GUILayout.Button("Disable water (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_WETNESS_FIRST=false;
						_RTP_LODmanagerScript.RTP_WETNESS_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
					
					GUILayout.Space(15);							
					if (GUILayout.Button("Reset water settings numeric params to default")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						_target.ReturnToDefaults("water");
						EditorUtility.SetDirty(_targetRT);
						_target.Refresh();
					}		
						
				} else {
					_target.TERRAIN_GlobalWetness=1;
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable water (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_WETNESS_FIRST=true;
						_RTP_LODmanagerScript.RTP_WETNESS_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
				}						
				
				EditorGUILayout.EndVertical();
					
				#endregion Settings - Water
					
				#region Settings - Caustics
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoWater),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);						
					EditorGUILayout.LabelField("Caustics settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;

				if (CAUSTICS_ENABLED_FIRST || CAUSTICS_ENABLED_ADD) {
					checkChange(ref _target.TERRAIN_CausticsAnimSpeed, EditorGUILayout.Slider("Anim speed", _target.TERRAIN_CausticsAnimSpeed, 0, 10f));
					checkChange(ref _target.TERRAIN_CausticsColor, EditorGUILayout.ColorField("Color", _target.TERRAIN_CausticsColor));
					checkChange(ref _target.TERRAIN_CausticsWaterLevelRefObject, (GameObject)EditorGUILayout.ObjectField("Water Level Ref", _target.TERRAIN_CausticsWaterLevelRefObject, typeof(GameObject), true));
					EditorGUI.BeginDisabledGroup(_target.TERRAIN_CausticsWaterLevelRefObject);
					checkChange(ref _target.TERRAIN_CausticsWaterLevel, EditorGUILayout.FloatField("Water Level", _target.TERRAIN_CausticsWaterLevel));
					EditorGUI.EndDisabledGroup();
					checkChange(ref _target.TERRAIN_CausticsWaterLevelByAngle, EditorGUILayout.Slider("Water level by slope", _target.TERRAIN_CausticsWaterLevelByAngle, 0f, 8f));
					checkChange(ref _target.TERRAIN_CausticsWaterShallowFadeLength, EditorGUILayout.Slider("Shallow fade length", _target.TERRAIN_CausticsWaterShallowFadeLength, 0.1f, 10f));
					checkChange(ref _target.TERRAIN_CausticsWaterDeepFadeLength, EditorGUILayout.Slider("Deep fade length", _target.TERRAIN_CausticsWaterDeepFadeLength, 1f, 100f));
					EditorGUILayout.BeginHorizontal();
						checkChange(ref _target.TERRAIN_CausticsTilingScale, EditorGUILayout.Slider("Texture tiling  ("+(Mathf.Round(_target.ReliefTransform.x*_target.TERRAIN_CausticsTilingScale*100)/100)+")", _target.TERRAIN_CausticsTilingScale, 0.5f, 4f));
						if (GUILayout.Button(new GUIContent("A","Align to terrain tile"), GUILayout.Width(22), GUILayout.Height(16))) {
							float ratio=Mathf.Round(_target.ReliefTransform.x*_target.TERRAIN_CausticsTilingScale);
							_target.TERRAIN_CausticsTilingScale=ratio/_target.ReliefTransform.x;
							EditorUtility.SetDirty(_targetRT);
						}
					EditorGUILayout.EndHorizontal();						
					EditorGUILayout.LabelField("Caustics texture", EditorStyles.boldLabel);
					checkChange(ref _target.TERRAIN_CausticsTex, (Texture2D)EditorGUILayout.ObjectField(_target.TERRAIN_CausticsTex, typeof(Texture2D), false, GUILayout.MinHeight(100), GUILayout.MinWidth(100), GUILayout.MaxWidth(100)));
				}
					
				GUILayout.Space(15);
				if (CAUSTICS_ENABLED_FIRST || CAUSTICS_ENABLED_ADD) {
					if (GUILayout.Button("Disable caustics (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_CAUSTICS_FIRST=false;
						_RTP_LODmanagerScript.RTP_CAUSTICS_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
					
					GUILayout.Space(15);							
					if (GUILayout.Button("Reset caustics settings numeric params to default")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						_target.ReturnToDefaults("caustics");
						EditorUtility.SetDirty(_targetRT);
						_target.Refresh();
					}		
						
				} else {
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable caustics (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_CAUSTICS_FIRST=true;
						_RTP_LODmanagerScript.RTP_CAUSTICS_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
				}						
				
				EditorGUILayout.EndVertical();
					
				#endregion Settings - Caustics
			break;					
				
			case ReliefTerrainSettingsItems.Reflections:
				#region Settings - Reflections
				GUILayout.Space(6);

				EditorGUILayout.BeginVertical("Box");
				skin_color=GUI.color;
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField(new GUIContent(icoReflection),  EditorStyles.miniLabel, GUILayout.Width(16), GUILayout.Height(16));
					GUI.color=new Color(1,1,0.5f,1);						
					EditorGUILayout.LabelField("Reflections settings", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal();
				GUI.color=skin_color;
				
				if (REFLECTION_ENABLED_FIRST || REFLECTION_ENABLED_ADD) {
					EditorGUILayout.HelpBox("Reflection map, realised as grayscale 2D texture (planar mapped sphere) is put on channel A of combined texture together of Perlin normal map (RG) and wetness mask (B).\n\nWhites are mapped on Reflection Color A (emission), blacks are mapped on Reflection Color B (diffuse blend depending on color alpha).\n\nAmount of reflection is set per layer, taken from view angle (Fresnel like effect), layer specularity (alpha channel of diffuse map) and water on the surface.",MessageType.None, true);
						
					EditorGUILayout.BeginHorizontal();
					Texture2D tmp_tex=_target.TERRAIN_ReflectionMap;
					ntex=tmp_tex;
					checkChange(ref ntex, (Texture2D)EditorGUILayout.ObjectField(_target.TERRAIN_ReflectionMap, typeof(Texture2D), false, GUILayout.MinHeight(90), GUILayout.MinWidth(90), GUILayout.MaxWidth(90)));
					if (ntex!=tmp_tex) {
						Texture2D tex_prev=_target.TERRAIN_ReflectionMap;
						_target.TERRAIN_ReflectionMap=ntex;
						if (ntex) {
							AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(ntex));
							if (_importer) {
								TextureImporter tex_importer=(TextureImporter)_importer;
								if (!tex_importer.isReadable) {
									Debug.LogWarning("Reflection map texture ("+ntex.name+") has been reimported as readable.");
									tex_importer.isReadable=true;
									AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(ntex),  ImportAssetOptions.ForceUpdate);
								}
							}
						}
						if (!_targetRT.PrepareGlobalNormalsAndSuperDetails(false, true)) _target.TERRAIN_ReflectionMap=tex_prev;
						_targetRT.RefreshTextures();
						_target.Refresh();
					}
					ColorChannels nchannel=ColorChannels.R;
					EditorGUILayout.LabelField("channel", GUILayout.MinWidth(60), GUILayout.MaxWidth(60));
					checkChange(ref nchannel, (ColorChannels)EditorGUILayout.EnumPopup(_target.TERRAIN_ReflectionMap_channel));
					if (nchannel!=_target.TERRAIN_ReflectionMap_channel) {
						_target.TERRAIN_ReflectionMap_channel=nchannel;
						_targetRT.PrepareGlobalNormalsAndSuperDetails(false,true);
					}
					EditorGUILayout.EndHorizontal();
						
					checkChange(ref _target.TERRAIN_ReflColorA, EditorGUILayout.ColorField("Reflection color A", _target.TERRAIN_ReflColorA));
					checkChange(ref _target.TERRAIN_ReflColorB, EditorGUILayout.ColorField("Reflection color B", _target.TERRAIN_ReflColorB));
					checkChange(ref _target.TERRAIN_ReflDistortion, EditorGUILayout.Slider("Normal based distortion", _target.TERRAIN_ReflDistortion, 0, 0.1f));
					EditorGUILayout.HelpBox("Reflection map can rotate (imitating clouds moving around), option below can be separately enabled in RTP_LODmanager.",MessageType.None, true);
					if (REFLECTION_ROTATION_ENABLED || REFLECTION_ROTATION_ENABLED) {
						if (GUILayout.Button("Disable reflection rotation (rebuild shaders via RTP_LODmanager)")) {
							_RTP_LODmanagerScript.RTP_ROTATE_REFLECTION=false;
							if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
								Selection.activeObject=_RTP_LODmanagerScript.gameObject;
								_RTP_LODmanagerScript.dont_sync=true;
							}
						}
					} else {
						//EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
						if (GUILayout.Button("Enable reflection rotation (rebuild shaders via RTP_LODmanager)")) {
							_RTP_LODmanagerScript.RTP_ROTATE_REFLECTION=true;
							if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
								Selection.activeObject=_RTP_LODmanagerScript.gameObject;
								_RTP_LODmanagerScript.dont_sync=true;
							}
						}
					}						
					EditorGUI.BeginDisabledGroup(!REFLECTION_ROTATION_ENABLED);
					checkChange(ref _target.TERRAIN_ReflectionRotSpeed, EditorGUILayout.Slider("Rotation speed", _target.TERRAIN_ReflectionRotSpeed, 0, 2));
					EditorGUI.EndDisabledGroup();
					EditorGUILayout.Space();
					checkChange(ref _target.TERRAIN_FresnelPow, EditorGUILayout.Slider("Fresnel exponent", _target.TERRAIN_FresnelPow, 0.5f, 32f));
					checkChange(ref _target.TERRAIN_FresnelOffset, EditorGUILayout.Slider("Fresnel offset", _target.TERRAIN_FresnelOffset, 0, 0.9f));
				}
				GUILayout.Space(15);
				if (REFLECTION_ENABLED_FIRST || REFLECTION_ENABLED_ADD) {
					if (GUILayout.Button("Disable reflection (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_REFLECTION_FIRST=false;
						_RTP_LODmanagerScript.RTP_REFLECTION_ADD=false;
						if (EditorUtility.DisplayDialog("", "Go to RTP manager now (to recompile shaders) ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
					
					GUILayout.Space(15);	
					if (GUILayout.Button("Reset reflection settings numeric params to default")) {
						Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
						_target.ReturnToDefaults("reflection");
						EditorUtility.SetDirty(_targetRT);
						_target.Refresh();
					}	
						
				} else {
					EditorGUILayout.HelpBox("This feature is currently disabled.",MessageType.Warning, true);
					if (GUILayout.Button("Enable reflection (rebuild shaders via RTP_LODmanager)")) {
						_RTP_LODmanagerScript.RTP_REFLECTION_FIRST=true;
						_RTP_LODmanagerScript.RTP_REFLECTION_ADD=true;
						if (EditorUtility.DisplayDialog("", "Feature is DISABLED in shader until you recompile it.\nGo to RTP manager to do it now ?","Yes","No")) {
							Selection.activeObject=_RTP_LODmanagerScript.gameObject;
							_RTP_LODmanagerScript.dont_sync=true;
						}
					}
				}	
				
				EditorGUILayout.EndVertical();
					
				#endregion Settings - Reflections
			break;					
				
			}
			
			GUILayout.Space(8);
			//Begin_Global_Indent();
			if (GUILayout.Button("Reset all numeric params to default")) {
				if (EditorUtility.DisplayDialog("Warning", "Are you sure to reset all RTP numeric values to their defaults ?","Yes","Cancel")) {
					Undo.RegisterUndo(_targetRT, "Undo relief terrain edit");
					_target.ReturnToDefaults();
					EditorUtility.SetDirty(_targetRT);
					_target.Refresh();
				}
			}	
			
			#endregion ALLSettings
		}
		
		if (dirtyFlag) {
			EditorUtility.SetDirty(_targetRT);
			_targetRT.RefreshTextures();				
			_target.Refresh();
		}

		GUILayout.Space(10);
		//DrawDefaultInspector();
		if (_target.activateObject) {
			Selection.activeObject=_target.activateObject;
			_target.activateObject=null;
		}


		Event current = Event.current;
		switch(current.type) {
			case EventType.keyDown:
				if (current.keyCode==KeyCode.M) {
					_target.paint_flag=!_target.paint_flag;
					if (!_targetRT.GetComponent<Collider>() || !_targetRT.GetComponent<Collider>().enabled) _target.paint_flag=false;
					if (_target.paint_flag) {
						if (_target.submenu!=ReliefTerrainMenuItems.GeneralSettings || (_target.submenu_settings!=ReliefTerrainSettingsItems.GlobalColor && _target.submenu_settings!=ReliefTerrainSettingsItems.Water)) {
							_target.paint_flag=false;
						}
						if (_target.submenu==ReliefTerrainMenuItems.GeneralSettings) {
							if (_target.submenu_settings==ReliefTerrainSettingsItems.Water) {
								if (!_targetRT.PrepareGlobalNormalsAndSuperDetails(true, true)) {
									_target.paint_flag=false;
								}
								_target.paint_wetmask=true;
								_target.paint_alpha_flag=true;
							}
//							if (_target.submenu_settings==ReliefTerrainSettingsItems.GlobalColor) {
//								if (!_target.prepare_tmpTexture(true)) {
//									_target.paint_flag=false;
//								}
//							}							
						}
					}					
					if (_target.paint_flag) {
						Tools.current=Tool.View;
						ReliefTerrain._SceneGUI = new SceneView.OnSceneFunc(CustomOnSceneGUI);
						SceneView.onSceneGUIDelegate += ReliefTerrain._SceneGUI;
					} else {
						Tools.current=prev_tool;
						SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;
					}
					EditorUtility.SetDirty(target);
				}
			break;
		}			
	}
	
	private void PrepareAtlases(int mask=3) {
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
			
		for(int n=0; n<_target.numLayers; n++) {
			int min_size=9999;
			for(int m=(n/4)*4; (m<((n/4)*4+4)) && (m<_target.numLayers); m++) {
				if (_target.splats[m]) { // czasem moÅ¼e byc bÅ‚Ä™dnie wprowadzony jako null
					if (_target.splats[m].width<min_size) min_size=_target.splats[m].width;
				}
			}		
			AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.splats[n]));
			if (_importer) {
				TextureImporter tex_importer=(TextureImporter)_importer;
				bool reimport_flag=false;
				if (!tex_importer.isReadable) {
					Debug.LogWarning("Detail texture "+n+" ("+_target.splats[n].name+") has been reimported as readable.");
					tex_importer.isReadable=true;
					reimport_flag=true;
				}
				if (_target.splats[n] && _target.splats[n].width>min_size) {
					Debug.LogWarning("Detail texture "+n+" ("+_target.splats[n].name+") has been reimported with "+min_size+" size.");
					tex_importer.maxTextureSize=min_size;
					reimport_flag=true;
				}
				if (reimport_flag) {
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_target.splats[n]),  ImportAssetOptions.ForceUpdate);
				}
			}					
		}				
		
		int i;
		for(i=0; i<_target.numLayers; i++) {
			try { 
				_target.splats[i].GetPixels(0,0,4,4,0);
			} catch (Exception e) {
				Debug.LogError("Splat texture "+i+" has to be marked as isReadable...");
				Debug.LogError(e.Message);
				return;
			}
		}
		int w=_target.splats[0].width;
		for(i=1; i<_target.numLayers; i++) {
			if (_target.splats[i].width!=w) {
				Debug.LogError("For performance reasons - all splats (detail textures) should have the same size");
				Debug.LogError("Detail tex 0 size="+w+" while detail tex "+i+" size="+_target.splats[i].width);
				return;
			}
		}
				
		ResetProgress( ((mask==3)?(8+2*8):(4+1*8)), "Padding textures" );
				
		int num=_target.numLayers<=8 ? _target.numLayers : 8;
		Texture2D[] splats=new Texture2D[num<=4 ? 4:8];
		for(i=0; i<splats.Length; i++) {
			if (((i<4) && (mask&1)>0) || ((i>=4) && (mask&2)>0)) {
				if (i<num) {
					splats[i]=_target.splats[i];
					splats[i]=PadTex(splats[i], 16);
					CheckProgress();							
				} else {
					splats[i]=new Texture2D(_target.splats[num-1].width, _target.splats[num-1].width);
				}
			}
		}
		progress_description="Packing textures";
				
		if ( ((mask&1)>0) && _target.splat_atlases[0] && AssetDatabase.GetAssetPath(_target.splat_atlases[0])=="") {
			DestroyImmediate(_target.splat_atlases[0]);
		}
		if ( ((mask&2)>0) && _target.splat_atlases[1] && AssetDatabase.GetAssetPath(_target.splat_atlases[1])=="") {
			DestroyImmediate(_target.splat_atlases[1]);
		}
		
		if ((mask&1)>0) {		
			_target.splat_atlases[0]=new Texture2D(splats[0].width*2, splats[0].width*2);
			_target.splat_atlases[0].wrapMode=TextureWrapMode.Clamp;
			_target.splat_atlases[0].PackTextures( new Texture2D[4] { splats[0],splats[1],splats[2],splats[3] } , 0, _target.splat_atlases[0].width, false);
			progress_description="Blending corners";
			CheckProgress();
			//if (splats[0].width<=128) BlendMip(_target.splat_atlases[0], 3, 0.5f);
			//if (splats[0].width<=256) BlendMip(_target.splat_atlases[0], 4, 0.5f);
			//if (splats[0].width<=512) BlendMip(_target.splat_atlases[0], 5, 0.5f);
			BlendMip(_target.splat_atlases[0], 0, 0.5f, 16); BlendMip(_target.splat_atlases[0], 0, 0.5f, 15);
			CheckProgress();					
			BlendMip(_target.splat_atlases[0], 1, 0.5f, 8); BlendMip(_target.splat_atlases[0], 1, 0.5f, 7);
			CheckProgress();					
			BlendMip(_target.splat_atlases[0], 2, 0.5f, 4); BlendMip(_target.splat_atlases[0], 2, 0.5f, 3);
			CheckProgress();					
			BlendMip(_target.splat_atlases[0], 3, 0.5f, 2); BlendMip(_target.splat_atlases[0], 3, 0.5f, 1);
			CheckProgress();					
			BlendMip(_target.splat_atlases[0], 4, 0.5f, 2); BlendMip(_target.splat_atlases[0], 4, 0.5f, 1);
			CheckProgress();					
			BlendMip(_target.splat_atlases[0], 5, 0.5f, 1); BlendMip(_target.splat_atlases[0], 5, 0.5f, 0);
			CheckProgress();					
			for(i=6; i<_target.splat_atlases[0].mipmapCount-2; i++) BlendMip(_target.splat_atlases[0], i, 0.5f);
			progress_description="Packing textures";
			CheckProgress();					
			_target.splat_atlases[0].Compress(true);
			_target.splat_atlases[0].Apply(false,false);
			_target.splat_atlases[0].filterMode=FilterMode.Trilinear;
			_target.splat_atlases[0].anisoLevel=0;
		}
				
		if ( ((mask&2)>0) && (num>4)) {
			_target.splat_atlases[1]=new Texture2D(splats[4].width*2, splats[4].width*2);
			_target.splat_atlases[1].wrapMode=TextureWrapMode.Clamp;
			_target.splat_atlases[1].PackTextures( new Texture2D[4] { splats[4],splats[5],splats[6],splats[7] } , 0, _target.splat_atlases[1].width, false);
			progress_description="Blending corners";
			CheckProgress();					
//			if (splats[0].width<=128) BlendMip(_target.splat_atlases[1], 3, 0.5f);
//			if (splats[0].width<=256) BlendMip(_target.splat_atlases[1], 4, 0.5f);
//			if (splats[0].width<=512) BlendMip(_target.splat_atlases[1], 5, 0.5f);
			BlendMip(_target.splat_atlases[1], 0, 0.5f, 16); BlendMip(_target.splat_atlases[1], 0, 0.5f, 15);
			CheckProgress();
			BlendMip(_target.splat_atlases[1], 1, 0.5f, 8); BlendMip(_target.splat_atlases[1], 1, 0.5f, 7);
			CheckProgress();
			BlendMip(_target.splat_atlases[1], 2, 0.5f, 4); BlendMip(_target.splat_atlases[1], 2, 0.5f, 3);
			CheckProgress();
			BlendMip(_target.splat_atlases[1], 3, 0.5f, 2); BlendMip(_target.splat_atlases[1], 3, 0.5f, 1);
			CheckProgress();
			BlendMip(_target.splat_atlases[1], 4, 0.5f, 2); BlendMip(_target.splat_atlases[1], 4, 0.5f, 1);
			CheckProgress();
			BlendMip(_target.splat_atlases[1], 5, 0.5f, 1); BlendMip(_target.splat_atlases[1], 5, 0.5f, 0);
			CheckProgress();
			for(i=6; i<_target.splat_atlases[1].mipmapCount-2; i++) BlendMip(_target.splat_atlases[1], i, 0.5f);
			CheckProgress();
			_target.splat_atlases[1].Compress(true);
			_target.splat_atlases[1].Apply(false,false);
			_target.splat_atlases[1].filterMode=FilterMode.Trilinear;
			_target.splat_atlases[1].anisoLevel=0;
		}
		for(i=0; i<splats.Length; i++) {
			DestroyImmediate(splats[i]);
		}
		EditorUtility.ClearProgressBar();
				
	}
	
	private void MakeCombinedGrayscale() {
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
			
		for(int n=0; n<_target.numLayers; n++) {
			int min_size=9999;
			for(int m=(n/4)*4; (m<((n/4)*4+4)) && (m<_target.numLayers); m++) {
				if (_target.splats[m]) { // czasem moÅ¼e byc bÅ‚Ä™dnie wprowadzony jako null
					if (_target.splats[m].width<min_size) min_size=_target.splats[m].width;
				}
			}		
			AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(_target.splats[n]));
			if (_importer) {
				TextureImporter tex_importer=(TextureImporter)_importer;
				bool reimport_flag=false;
				if (!tex_importer.isReadable) {
					Debug.LogWarning("Detail texture "+n+" ("+_target.splats[n].name+") has been reimported as readable.");
					tex_importer.isReadable=true;
					reimport_flag=true;
				}
				if (_target.splats[n] && _target.splats[n].width>min_size) {
					Debug.LogWarning("Detail texture "+n+" ("+_target.splats[n].name+") has been reimported with "+min_size+" size.");
					tex_importer.maxTextureSize=min_size;
					reimport_flag=true;
				}
				if (reimport_flag) {
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_target.splats[n]),  ImportAssetOptions.ForceUpdate);
				}
			}					
		}				
		
		int i;
		for(i=0; i<_target.numLayers; i++) {
			try { 
				_target.splats[i].GetPixels(0,0,4,4,0);
			} catch (Exception e) {
				Debug.LogError("Splat texture "+i+" has to be marked as isReadable...");
				Debug.LogError(e.Message);
				return;
			}
		}
		int w=_target.splats[0].width;
		for(i=1; i<_target.numLayers; i++) {
			if (_target.splats[i].width!=w) {
				Debug.LogError("For performance reasons - all splats (detail textures) should have the same size");
				Debug.LogError("Detail tex 0 size="+w+" while detail tex "+i+" size="+_target.splats[i].width);
				return;
			}
		}
			
		int num=_target.numLayers<=8 ? _target.numLayers : 8;
		Texture2D[] splats=new Texture2D[4];
		for(i=0; i<splats.Length; i++) {
			if (i<4) {
				if (i<num) {
					splats[i]=_target.splats[i];
				} else {
					splats[i]=new Texture2D(_target.splats[num-1].width, _target.splats[num-1].width);
				}
			}
		}
		Color[] tex0=splats[0].GetPixels(0);
		Color[] tex1=splats[1].GetPixels(0);
		Color[] tex2=splats[2].GetPixels(0);
		Color[] tex3=splats[3].GetPixels(0);
		Color[] result=new Color[tex0.Length];
		for(i=0; i<tex0.Length; i++) {
			result[i]=new Color( tex0[i].grayscale, tex1[i].grayscale, tex2[i].grayscale, tex3[i].grayscale);
		}
		_target.SSColorCombined=new Texture2D(splats[0].width, splats[0].height, TextureFormat.ARGB32, true, true);
		_target.SSColorCombined.SetPixels(result);
		_target.SSColorCombined.Apply(true,false);
		_target.SSColorCombined.Compress(true);
	}
			
	private ushort[][,] ConvertTextureToArray( Texture2D tex) {
		ushort[][,] tab = new ushort[4][,];
      	for (int i = 0; i < 4; i++)
        	tab[i] = new ushort[tex.width, tex.height];
		
		Color32[] cols=tex.GetPixels32();
		for(int j=0; j<tex.height; j++) {
			int idx=j*tex.width;
			for(int i=0; i<tex.width; i++) {
				tab[0][i,j]=cols[idx].r;
				tab[1][i,j]=cols[idx].g;
				tab[2][i,j]=cols[idx].b;
				tab[3][i,j]=cols[idx].a;
				idx++;
			}
		}
		return tab;
	}	
	
	private Texture2D PadTex(Texture2D splat, int padding) {
		int size=splat.width;
		int size_padded=size-padding*2;
		
		ResamplingService resamplingService = new ResamplingService();
		resamplingService.Filter = ResamplingFilters.Lanczos3;
		ushort[][,] input = ConvertTextureToArray((Texture2D)splat);
		ushort[][,] output = resamplingService.Resample(input, size_padded, size_padded);
		for(int c=0; c<4; c++) {
			for(int i=0; i<size_padded; i++) {
				for(int j=0; j<size_padded; j++) {
					if (output[c][i,j]>255) output[c][i,j]=255;
				}
			}
		}
		
		Color32[] cols=new Color32[size*size];
		for(int j=0; j<size; j++) {
			int j_idx1=j*size;
			int j_idx2=((j+size_padded-padding)%size_padded);
			for(int i=0; i<size; i++) {
				int i2=(i+size_padded-padding)%size_padded;
				cols[j_idx1].r=(byte)output[0][i2, j_idx2];
				cols[j_idx1].g=(byte)output[1][i2, j_idx2];
				cols[j_idx1].b=(byte)output[2][i2, j_idx2];
				cols[j_idx1].a=(byte)output[3][i2, j_idx2];
				j_idx1++;
			}
		}
		Texture2D tex=new Texture2D(size,size,TextureFormat.ARGB32,true);
		tex.SetPixels32(cols);
		tex.Apply(true,false);
		return tex;
	}
	
	private void BlendMip(Texture2D atlas, int mip, float blend, int pad_offset=0) {
		Color32[] cols=atlas.GetPixels32(mip);
		int size=Mathf.FloorToInt(Mathf.Sqrt((float)cols.Length));
		int half_size=size/2;
		int idx_offA, idx_offB;
		idx_offA=pad_offset*size;
		idx_offB=(half_size-1-pad_offset)*size;
		for(int i=0; i<size; i++) {
			Color32 colA=cols[idx_offA];
			Color32 colB=cols[idx_offB];
			cols[idx_offA]=Color32.Lerp(colA, colB, blend);
			cols[idx_offB]=Color32.Lerp(colB, colA, blend);
			idx_offA++;
			idx_offB++;
		}
		idx_offA=(half_size+pad_offset)*size;
		idx_offB=(size-1-pad_offset)*size;
		for(int i=0; i<size; i++) {
			Color32 colA=cols[idx_offA];
			Color32 colB=cols[idx_offB];
			cols[idx_offA]=Color32.Lerp(colA, colB, blend);
			cols[idx_offB]=Color32.Lerp(colB, colA, blend);
			idx_offA++;
			idx_offB++;
		}		
		idx_offA=pad_offset;
		idx_offB=half_size-1-pad_offset;
		for(int i=0; i<size; i++) {
			Color32 colA=cols[idx_offA];
			Color32 colB=cols[idx_offB];
			cols[idx_offA]=Color32.Lerp(colA, colB, blend);
			cols[idx_offB]=Color32.Lerp(colB, colA, blend);
			idx_offA+=size;
			idx_offB+=size;
		}
		idx_offA=half_size+pad_offset;
		idx_offB=size-1-pad_offset;
		for(int i=0; i<size; i++) {
			Color32 colA=cols[idx_offA];
			Color32 colB=cols[idx_offB];
			cols[idx_offA]=Color32.Lerp(colA, colB, blend);
			cols[idx_offB]=Color32.Lerp(colB, colA, blend);
			idx_offA+=size;
			idx_offB+=size;
		}
		atlas.SetPixels32(cols,mip);
	}
	
	public void colorAlphaFromSplat(int splat_control_tex_idx) {
		ReliefTerrain _target=(ReliefTerrain)target;
		Color[] cols;
			
		if (!_target.ColorGlobal) return;
		_target.prepare_tmpTexture(true);
		if (!_target.ColorGlobal) return;
		if ((_target.ColorGlobal.format!=TextureFormat.Alpha8) && (_target.ColorGlobal.format!=TextureFormat.ARGB32)) {
			try { 
				_target.ColorGlobal.GetPixels(0,0,4,4,0);
			} catch (Exception e) {
				Debug.LogError("Global ColorMap has to be marked as isReadable...");
				Debug.LogError(e.Message);
			}
			Texture2D tmp_globalColorMap;
			if (_target.ColorGlobal.format==TextureFormat.Alpha8) {
				tmp_globalColorMap=new Texture2D(_target.ColorGlobal.width, _target.ColorGlobal.height, TextureFormat.Alpha8, true); 
			} else {
				tmp_globalColorMap=new Texture2D(_target.ColorGlobal.width, _target.ColorGlobal.height, TextureFormat.ARGB32, true); 
			}
			cols=_target.ColorGlobal.GetPixels();
			_target.ColorGlobal=tmp_globalColorMap;
		} else {
			cols=_target.ColorGlobal.GetPixels();
		}
		
		if (splat_control_tex_idx<_target.globalSettingsHolder.numLayers) {
			
			Texture2D splatA=_target.controlA;
			Texture2D splatB=(_target.globalSettingsHolder.numLayers<=8) ? _target.controlB : _target.controlC;
			float[,] blured_sum=new float[_target.ColorGlobal.width, _target.ColorGlobal.height];
			int idx;
			
			if (splatA.width!=_target.ColorGlobal.width || splatA.height!=_target.ColorGlobal.height) {
				ResamplingService resamplingService = new ResamplingService();
				resamplingService.Filter = ResamplingFilters.Quadratic;
				ushort[][,] input = ConvertTextureToArray((Texture2D)splatA);
				ushort[][,] output = resamplingService.Resample(input, _target.ColorGlobal.width, _target.ColorGlobal.height);
				for(int i=0; i<_target.ColorGlobal.width; i++) {
					for(int j=0; j<_target.ColorGlobal.height; j++) {
						blured_sum[i,j]=(output[0][i,j]+output[1][i,j]+output[2][i,j]+output[3][i,j])/255.0f;
					}
				}
			} else {
				Color32[] cols_splat=splatA.GetPixels32();
				idx=0;
				for(int i=0; i<_target.ColorGlobal.width; i++) {
					for(int j=0; j<_target.ColorGlobal.height; j++) {
						blured_sum[i,j]=(cols_splat[idx].r+cols_splat[idx].g+cols_splat[idx].b+cols_splat[idx].a)/255.0f;
						idx++;
					}
				}
			}
			if (splatB.width!=_target.ColorGlobal.width || splatB.height!=_target.ColorGlobal.height) {
				ResamplingService resamplingService = new ResamplingService();
				resamplingService.Filter = ResamplingFilters.Quadratic;
				ushort[][,] input = ConvertTextureToArray((Texture2D)splatB);
				ushort[][,] output = resamplingService.Resample(input, _target.ColorGlobal.width, _target.ColorGlobal.height);
				for(int i=0; i<_target.ColorGlobal.width; i++) {
					for(int j=0; j<_target.ColorGlobal.height; j++) {
						blured_sum[i,j]=Mathf.Max(blured_sum[i,j], (output[0][i,j]+output[1][i,j]+output[2][i,j]+output[3][i,j])/255.0f);
					}
				}
			} else {
				Color32[] cols_splat=splatB.GetPixels32();
				idx=0;
				for(int i=0; i<_target.ColorGlobal.width; i++) {
					for(int j=0; j<_target.ColorGlobal.height; j++) {
						blured_sum[i,j]=Mathf.Max(blured_sum[i,j], (cols_splat[idx].r+cols_splat[idx].g+cols_splat[idx].b+cols_splat[idx].a)/255.0f);
						idx++;
					}
				}
			}
			for(int i=0; i<_target.ColorGlobal.width; i++) {
				for(int j=0; j<_target.ColorGlobal.height; j++) {
					if (blured_sum[i,j]>1) blured_sum[i,j]=1; else blured_sum[i,j]=blured_sum[i,j];
				}
			}
				
			float[,] blured_sum2=new float[_target.ColorGlobal.width, _target.ColorGlobal.height];
			float[,] tmp_blured_sum;
			for(int blur_cnt=0; blur_cnt<4; blur_cnt++) {
				for(int i=1; i<_target.ColorGlobal.width-1; i++) {
					for(int j=1; j<_target.ColorGlobal.height-1; j++) {
						blured_sum2[i,j]=0.7f*(blured_sum[i+1,j]+blured_sum[i-1,j]+blured_sum[i,j+1]+blured_sum[i,j-1]);
						blured_sum2[i,j]+=0.3f*(blured_sum[i+1,j+1]+blured_sum[i+1,j-1]+blured_sum[i-1,j+1]+blured_sum[i-1,j-1]);
						blured_sum2[i,j]*=0.25f;
					}
				}
				tmp_blured_sum=blured_sum;
				blured_sum=blured_sum2;
				blured_sum2=tmp_blured_sum;
			}
			idx=0;
			for(int i=0; i<_target.ColorGlobal.width; i++) {
				for(int j=0; j<_target.ColorGlobal.height; j++) {
					cols[idx].a=blured_sum[i,j];
					cols[idx].a*=cols[idx].a;
					cols[idx].a*=cols[idx].a;
					idx++;
				}
			}				
			
			_target.ColorGlobal.SetPixels(cols);
			_target.ColorGlobal.Apply(true,false);
		}
	}
	
	private bool checkChange(ref ProceduralMaterial val, ProceduralMaterial nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}
	private bool checkChange(ref float val, float nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}
	private bool checkChange(ref int val, int nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}
	private bool checkChange(ref Texture2D val, Texture2D nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}

	private bool checkChange(ref Cubemap val, Cubemap nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}	
	private bool checkChange(ref Color val, Color nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}
	private bool checkChange(ref ColorChannels val, ColorChannels nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}
	private bool checkChange(ref bool val, bool nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}
	private bool checkChange(ref GameObject val, GameObject nval) {
		bool changed=(nval!=val);
		if (changed) {
			if ((EditorApplication.timeSinceStartup-changedTime)>0.5) {
				changedTime=EditorApplication.timeSinceStartup;
				Undo.RegisterUndo((ReliefTerrain)target, "Undo relief terrain edit");
			}
		}
		dirtyFlag=dirtyFlag || changed;
		val=nval;
		return changed;
	}	
	
	private Texture2D GetSubstanceTex(string gen_name, string shad_name, int n, bool normal_flag=false) {
		Texture2D ntex;
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
		#if UNITY_4_1 || UNITY_4_2 || UNITY_4_3  || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5 || UNITY_5_6									
			Texture[] gen_texs=_target.Substances[n].GetGeneratedTextures();
			ProceduralTexture ptex=(ProceduralTexture)gen_texs[0];
			for(int tidx=1; tidx<gen_texs.Length; tidx++) {
				if (gen_texs[tidx].name.IndexOf(gen_name)>=0) {
					ptex=(ProceduralTexture)gen_texs[tidx];
					break;
				}
			}
			Color32[] cols=ptex.GetPixels32(0,0,ptex.width, ptex.height);
			if (normal_flag) {
				ntex = new Texture2D(ptex.width, ptex.height, TextureFormat.RGB24, false);
				for(int i=0; i<cols.Length; i++) {
					int cz=Mathf.FloorToInt(255-Mathf.Sqrt(cols[i].g*cols[i].g + cols[i].a*cols[i].a));
					if (cz<0) cz=0; else if (cz>255) cz=255;
					cols[i]=new Color32(cols[i].a, cols[i].g, (byte)cz, 0);
				}
			} else {
				ntex = new Texture2D(ptex.width, ptex.height, TextureFormat.ARGB32, false);
			}
			ntex.SetPixels32(cols);
			ntex.Apply(true,false);
		#else
			ProceduralMaterial pm=_target.Substances[n];
			int _sizex=1<<(int)(pm.GetProceduralVector("$outputsize").x);
			int _sizey=1<<(int)(pm.GetProceduralVector("$outputsize").y);
			RenderTexture rt = RenderTexture.GetTemporary(_sizex, _sizey, 32);
			Graphics.SetRenderTarget(rt);
			Graphics.Blit(pm.GetTexture(shad_name), rt);
			Texture2D tmp_tex = new Texture2D(_sizex, _sizey);
			tmp_tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
			Color32[] cols=tmp_tex.GetPixels32();
			DestroyImmediate(tmp_tex); tmp_tex=null;
			if (normal_flag) {
				ntex = new Texture2D(_sizex, _sizey, TextureFormat.RGB24, false);
				for(int i=0; i<cols.Length; i++) {
					int cz=Mathf.FloorToInt(255-Mathf.Sqrt(cols[i].g*cols[i].g + cols[i].a*cols[i].a));
					if (cz<0) cz=0; else if (cz>255) cz=255;
					cols[i]=new Color32(cols[i].a, cols[i].g, (byte)cz, 0);
				}
			} else {
				ntex = new Texture2D(_sizex, _sizey, TextureFormat.ARGB32, false);
			}
			ntex.SetPixels32(cols);
			ntex.Apply(true,false);
			Graphics.SetRenderTarget(null);
			RenderTexture.ReleaseTemporary(rt);
		#endif
		
		return ntex;
	}	
	
	bool PrepareHeights(int num) {
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
		
		Texture2D[] Heights=_target.Heights;
		if (Heights==null) return false;
		
		for(int n=0; n<((Heights.Length<_target.numLayers) ? Heights.Length : _target.numLayers); n++) {
			int min_size=9999;
			for(int m=(n/4)*4; (m<((n/4)*4+4)) && (m<Heights.Length); m++) {
				if (Heights[m]) {
					if (Heights[m].width<min_size) min_size=Heights[m].width;
				}
			}		
			AssetImporter _importer=AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(Heights[n]));
			if (_importer) {
				TextureImporter tex_importer=(TextureImporter)_importer;
				bool reimport_flag=false;
				if (!tex_importer.isReadable) {
					Debug.LogWarning("Height texture "+n+" ("+Heights[n].name+") has been reimported as readable.");
					tex_importer.isReadable=true;
					reimport_flag=true;
				}
				if (!tex_importer.DoesSourceTextureHaveAlpha() && !tex_importer.grayscaleToAlpha) {
					Debug.LogWarning("Height texture "+n+" ("+Heights[n].name+") has been reimported to have alpha channel.");
					tex_importer.grayscaleToAlpha=true;
					tex_importer.textureFormat=TextureImporterFormat.Alpha8;
					reimport_flag=true;
				}
				if (Heights[n] && Heights[n].width>min_size) {
					Debug.LogWarning("Height texture "+n+" ("+Heights[n].name+") has been reimported with "+min_size+" size.");
					tex_importer.maxTextureSize=min_size;
					reimport_flag=true;
				}
				if (reimport_flag) {
					AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(Heights[n]),  ImportAssetOptions.ForceUpdate);
				}
			}					
		}			
		
		Texture2D[] heights=new Texture2D[4];
		int i;
		int _w=256;
		int _w_idx=0;
		int len=_target.numLayers<12 ? _target.numLayers:12;
		if (num>=len) return false;
		num=(num>>2)*4;
		for(i=num; i<num+4; i++)	{
			if (num<4) {
				heights[i]=i<Heights.Length ? Heights[i] : null;
				if (heights[i]) {
					_w=heights[i].width;
					_w_idx=i;
				}
			} else if (num<8) {
				heights[i-4]=i<Heights.Length ? Heights[i] : null;
				if (heights[i-4]) {
					_w=heights[i-4].width;
					_w_idx=i;
				}
			} else {
				heights[i-8]=i<Heights.Length ? Heights[i] : null;
				if (heights[i-8]) {
					_w=heights[i-8].width;
					_w_idx=i;
				}
			}
		}
		for(i=0; i<4; i++) {
			if (!heights[i]) {
				heights[i]=new Texture2D(_w, _w);
				FillTex(heights[i], new Color32(255,255,255,255));
			}
			if (heights[i]) {
				try { 
					heights[i].GetPixels(0,0,4,4,0);
				} catch (Exception e) {
					Debug.LogError("Height texture "+i+" has to be marked as isReadable...");
					Debug.LogError(e.Message);
					_target.activateObject=heights[i];
					return false;
				}
				if (heights[i].width!=_w) {
					Debug.LogError("Height textures should have the same size ! (check layer "+_w_idx+" and "+(num+i)+")");
					_target.activateObject=heights[i];
					return false;
				}
			} else {
				heights[i]=new Texture2D(_w, _w);
				FillTex(heights[i], new Color32(255,255,255,255));
			}
		}
		
		if (num<4) {
			_target.HeightMap=CombineHeights(heights[0], heights[1], heights[2], heights[3]);
		} else if (num<8) {
			_target.HeightMap2=CombineHeights(heights[0], heights[1], heights[2], heights[3]);
		} else if (num<12) {
			_target.HeightMap3=CombineHeights(heights[0], heights[1], heights[2], heights[3]);
		}
		return true;
	}
	
	private Texture2D CombineHeights(Texture2D source_tex0, Texture2D source_tex1, Texture2D source_tex2, Texture2D source_tex3) {
		Texture2D rendered_tex=new Texture2D(source_tex0.width, source_tex0.height, TextureFormat.ARGB32, true, true);
		byte[] colsR=get_alpha_channel(source_tex0);
		byte[] colsG=get_alpha_channel(source_tex1);
		byte[] colsB=get_alpha_channel(source_tex2);
		byte[] colsA=get_alpha_channel(source_tex3);
		Color32[] cols=rendered_tex.GetPixels32();
		for(int i=0; i<cols.Length; i++) {
			cols[i].r=colsR[i];
			cols[i].g=colsG[i];
			cols[i].b=colsB[i];
			cols[i].a=colsA[i];
		}
		rendered_tex.SetPixels32(cols);
		rendered_tex.Apply(true, false);
		//rendered_tex.Compress(true);
		//rendered_tex.Apply(true, true); // (non readable robione przy publishingu)
		rendered_tex.filterMode=FilterMode.Trilinear;
		return rendered_tex;
	}
				
	private byte[] get_alpha_channel(Texture2D source_tex) {
		Color32[] cols=source_tex.GetPixels32();
		byte[] ret=new byte[cols.Length];
		for(int i=0; i<cols.Length; i++) ret[i]=cols[i].a;
		return ret;
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
	
	private bool SaveTexture(ref Texture2D tex, ref string save_path, string default_name, int buttonwidth, TextureImporterFormat textureFormat, bool mipmapEnabled, bool button_triggered=true, bool sRGB_flag=false) {
//		if (tex==null) return;			
//		if (AssetDatabase.GetAssetPath(tex)!="") return;
		EditorGUI.BeginDisabledGroup( tex==null || AssetDatabase.GetAssetPath(tex)!="");
		bool saved=false;
		bool cond;
		if (button_triggered) {
			cond=GUILayout.Button("Save to file", GUILayout.MaxWidth(buttonwidth));
		} else {
			cond=true;
		}
		if (cond) {
			string directory;
			string file;
			if (save_path=="") {
				directory=Application.dataPath;
				file=default_name;
			} else {
				directory=Path.GetDirectoryName(save_path);
				file=Path.GetFileNameWithoutExtension(save_path)+".png";
			}
			string path = EditorUtility.SaveFilePanel("Save texture", directory, file, "png");
			if (path!="") {
				path=path.Substring(Application.dataPath.Length-6);
				save_path=path;
				// kopia, bo mozemy miec skompresowana texture
				Texture2D ntex=new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, true);
				for(int mip=0; mip<tex.mipmapCount; mip++) {
					Color32[] cols=tex.GetPixels32(mip);
					ntex.SetPixels32(cols, mip);
					ntex.Apply(false,false);
				}					
		 		byte[] bytes = ntex.EncodeToPNG();
				ntex=null;
			    System.IO.File.WriteAllBytes(path, bytes);
				AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
				TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
				textureImporter.textureFormat = textureFormat; 
				textureImporter.mipmapEnabled = mipmapEnabled; 
				textureImporter.linearTexture=sRGB_flag;					
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
				tex = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
				saved=true;
			}		
		}
		EditorGUI.EndDisabledGroup();
		return saved;
	}
		
	public void OnSceneGUI() {
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
			
		Event current = Event.current;
			
		if (Event.current.type==EventType.keyDown) {
			if (Event.current.keyCode==KeyCode.M) {
				_target.paint_flag=!_target.paint_flag;
				if (!_targetRT.GetComponent<Collider>() || !_targetRT.GetComponent<Collider>().enabled) _target.paint_flag=false;
				if (_target.paint_flag) {
					if (_target.submenu!=ReliefTerrainMenuItems.GeneralSettings || (_target.submenu_settings!=ReliefTerrainSettingsItems.GlobalColor && _target.submenu_settings!=ReliefTerrainSettingsItems.Water)) {
						_target.paint_flag=false;
					}
					if (_target.submenu==ReliefTerrainMenuItems.GeneralSettings) {
						if (_target.submenu_settings==ReliefTerrainSettingsItems.Water) {
							if (!_targetRT.PrepareGlobalNormalsAndSuperDetails(true, true)) {
								_target.paint_flag=false;
							}
							_target.paint_wetmask=true;
							_target.paint_alpha_flag=true;
						}
//						if (_target.submenu_settings==ReliefTerrainSettingsItems.GlobalColor) {
//							if (!_target.prepare_tmpTexture(true)) {
//								_target.paint_flag=false;
//							}
//						}							
					}
				}						
				if (_target.paint_flag) {
					Tools.current=Tool.View;
					ReliefTerrain._SceneGUI = new SceneView.OnSceneFunc(CustomOnSceneGUI);
					SceneView.onSceneGUIDelegate += ReliefTerrain._SceneGUI;
				} else {
					Tools.current=prev_tool;
					SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;					
				}
				EditorUtility.SetDirty(target);
			}
		}

		if(Event.current.type==EventType.keyDown && Event.current.keyCode==KeyCode.L) {
			current.Use();
			if (_targetRT.GetComponent<Collider>()) {
		        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
				paintHitInfo_flag=_targetRT.GetComponent<Collider>().Raycast(ray, out paintHitInfo, Mathf.Infinity);
				if (paintHitInfo_flag) {
					int max_idx=0;
					float max_val=0;
					if (_targetRT.controlA) {
						Color c=_targetRT.controlA.GetPixelBilinear(paintHitInfo.textureCoord.x,paintHitInfo.textureCoord.y);
						if (c.r>max_val) { max_val=c.r; max_idx=0;	}
						if (c.g>max_val) { max_val=c.g; max_idx=1; }
						if (c.b>max_val) { max_val=c.b; max_idx=2; }
						if (c.a>max_val) { max_val=c.a; max_idx=3; }
					}
					if (_target.numLayers>4 && _targetRT.controlB) {
						Color c=_targetRT.controlB.GetPixelBilinear(paintHitInfo.textureCoord.x,paintHitInfo.textureCoord.y);
						if (c.r>max_val) { max_val=c.r; max_idx=4;	}
						if (c.g>max_val) { max_val=c.g; max_idx=5; }
						if (c.b>max_val) { max_val=c.b; max_idx=6; }
						if (c.a>max_val) { max_val=c.a; max_idx=7; }
					}
					if (_target.numLayers>8 && _targetRT.controlC) {
						Color c=_targetRT.controlC.GetPixelBilinear(paintHitInfo.textureCoord.x,paintHitInfo.textureCoord.y);
						if (c.r>max_val) { max_val=c.r; max_idx=8;	}
						if (c.g>max_val) { max_val=c.g; max_idx=9; }
						if (c.b>max_val) { max_val=c.b; max_idx=10; }
						if (c.a>max_val) { max_val=c.a; max_idx=11; }
					}
					
					_target.submenu=ReliefTerrainMenuItems.Details;
					_target.show_active_layer=max_idx;
				}
			}					
		}
	}	

	public void CustomOnSceneGUI(SceneView sceneview) {
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
		
		EditorWindow currentWindow = EditorWindow.mouseOverWindow;
		if(!currentWindow) return;
		
		Event current = Event.current;
		
		if (current.alt) {
			return;
		}		
		if (Event.current.button == 1) {
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			return;
		}
		
		bool paintOff_flag=(Tools.current!=Tool.View);
		if (_target.paint_wetmask) {
			paintOff_flag = paintOff_flag || _target.submenu!=ReliefTerrainMenuItems.GeneralSettings || _target.submenu_settings!=ReliefTerrainSettingsItems.Water;
		} else {
			paintOff_flag = paintOff_flag || _target.submenu!=ReliefTerrainMenuItems.GeneralSettings || _target.submenu_settings!=ReliefTerrainSettingsItems.GlobalColor;
		}
		if (paintOff_flag) {
			 _target.paint_flag=false;
			SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;
			EditorUtility.SetDirty(target);
			return;
		}		
		
		if (current.type==EventType.layout) {
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			return;
		}
		
		switch(current.type) {
			case EventType.keyDown:
				if (current.keyCode==KeyCode.Escape) {
					_target.paint_flag=false;
					Tools.current=prev_tool;
					SceneView.onSceneGUIDelegate -= ReliefTerrain._SceneGUI;
					EditorUtility.SetDirty(target);
				}
			break;
		}

		if (current.control) {
				if (current.type==EventType.mouseMove) {
					if (control_down_flag) {
						control_down_flag=false;
						EditorUtility.SetDirty(target);
					}
				}
				return;
		}
		control_down_flag=true;
		
		switch(current.type) {
			case EventType.mouseDown:
				get_paint_coverage();
				// Debug.Log(""+cover_verts_num + "  "+ paintHitInfo_flag + _target.prepare_tmpColorMap());
				if (paintHitInfo_flag) {
					if (_targetRT.prepare_tmpTexture(!_target.paint_wetmask)) {
						Undo.RegisterUndo(_targetRT.tmp_globalColorMap, "Geometry Blend Edit");
						_targetRT.modify_blend(!current.shift);
					}
				} else {
					_target.undo_flag=true;
				}
				current.Use();
			break;
			case EventType.mouseDrag:
				get_paint_coverage();
				if (paintHitInfo_flag) {
					if (_target.undo_flag) {
						if (_targetRT.prepare_tmpTexture(!_target.paint_wetmask)) {
							Undo.RegisterUndo(_targetRT.tmp_globalColorMap, "Geometry Blend Edit");
						}
						_target.undo_flag=false;
					}
				}
				if (paintHitInfo_flag) {
					if (_targetRT.prepare_tmpTexture(!_target.paint_wetmask)) {
						_targetRT.modify_blend(!current.shift);
						//current.Use();
					}
				}
			break;
			case EventType.mouseMove:
				get_paint_coverage();
			break;
		}
		
		if (paintHitInfo_flag) {
			bool upflag=_target.paint_wetmask ? !current.shift : current.shift;
			if (upflag) {
				Handles.color=new Color(1,0,0, Mathf.Max(0.1f, _target.paint_opacity*0.5f));
				Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size);
				Handles.color=new Color(1,0,0, Mathf.Max(0.6f, _target.paint_opacity));
				Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size*Mathf.Max(0.3f, 1 -_target.paint_smoothness));
			} else {
				Handles.color=new Color(0,1,0, Mathf.Max(0.1f, _target.paint_opacity*0.5f));
				Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size);
				Handles.color=new Color(0,1,0, Mathf.Max(0.6f, _target.paint_opacity));
				Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size*Mathf.Max(0.3f, 1 -_target.paint_smoothness));
			}			
		}		
		if (current.shift) current.Use();
	}	
		
	private void get_paint_coverage() {
		if (EditorApplication.timeSinceStartup<lCovTim) return;
		lCovTim=EditorApplication.timeSinceStartup+0.02;
		ReliefTerrain _targetRT=(ReliefTerrain)target;
		ReliefTerrainGlobalSettingsHolder _target=_targetRT.globalSettingsHolder;
		if (!_targetRT.GetComponent<Collider>()) return;
			
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		paintHitInfo_flag=_targetRT.GetComponent<Collider>().Raycast(ray, out paintHitInfo, Mathf.Infinity);
		_target.paintHitInfo=paintHitInfo;
		_target.paintHitInfo_flag=paintHitInfo_flag;

		EditorUtility.SetDirty(target);
	}		
		
	private void ResetProgress(int progress_count, string _progress_description="") {
		progress_count_max=progress_count;
		progress_count_current=0;
		progress_description=_progress_description;
	}
	
	private void CheckProgress() {
      if ( ((progress_count_current++) % progress_granulation) == (progress_granulation-1) )
      {
         EditorUtility.DisplayProgressBar( "Processing...", progress_description, 1.0f*progress_count_current/progress_count_max );
      }

	}		
		
#endif
}
