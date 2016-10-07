using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor (typeof(RTP_LODmanager))]
public class RTP_LODmanagerEditor : Editor {
#if UNITY_EDITOR	
	public bool rebuild_noforwardadd=false;
	public bool force_rebuild=false;
	
	public void OnEnable() {
		RTP_LODmanager _target=(RTP_LODmanager)target;
		
		if (!_target.dont_sync) SyncFeatures();
		_target.dont_sync=false;
	}
	
	public void OnDisable() {
		SyncFeatures();
	}
	
	public override void OnInspectorGUI () {
		RTP_LODmanager _target=(RTP_LODmanager)target;
		Color skin_color=GUI.color;
		
		int samplers_left;
		int samplers_used;
		
		if (!_target.SHADER_USAGE_FirstPass && !_target.SHADER_USAGE_Terrain2Geometry) {
			EditorGUILayout.HelpBox("You don't use terrain RTP shaders nor mesh versions. Check shaders needed below and recompile them using choosen features.", MessageType.Error, true);
		} else {
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.HelpBox("LOD level can be adjusted realtime by setting RTP_LODlevel to one of the following enums:\n\n1. TerrainShaderLod.POM\n2. TerrainShaderLod.PM\n3. TerrainShaderLod.SIMPLE\n4. TerrainShaderLod.CLASSIC\n\nwith shadow flags like below and calling function RefreshLODlevel() on manager script (refer to file \"RTP_LODmanager.cs\").\n\nREMEMBER - actual shader LOD level is influenced by MaxLOD param available per pass.",MessageType.Warning, true);
			GUI.color=new Color(1,1,0.5f,1);
			EditorGUILayout.LabelField("LOD level", EditorStyles.boldLabel);
			GUI.color=skin_color;		
			
			_target.RTP_LODlevel=(TerrainShaderLod)EditorGUILayout.EnumPopup(_target.RTP_LODlevel);
			
			EditorGUI.BeginDisabledGroup( _target.RTP_LODlevel!=TerrainShaderLod.POM );
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("RTP_SHADOWS", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
				if ( _target.RTP_LODlevel!=TerrainShaderLod.POM ) {
					EditorGUILayout.Toggle(false);
				} else {
					_target.RTP_SHADOWS=EditorGUILayout.Toggle(_target.RTP_SHADOWS);
				}
			EditorGUILayout.EndHorizontal();
			EditorGUI.BeginDisabledGroup(!_target.RTP_SHADOWS);
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("RTP_SOFT_SHADOWS", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
				if ( _target.RTP_LODlevel!=TerrainShaderLod.POM || !_target.RTP_SHADOWS) {
					EditorGUILayout.Toggle(false);
				} else {
					_target.RTP_SOFT_SHADOWS=EditorGUILayout.Toggle(_target.RTP_SOFT_SHADOWS);
				}		
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
			
			if (GUILayout.Button("Refresh LOD level")) {
				_target.RefreshLODlevel();
				EditorUtility.SetDirty(_target);
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();
		}
		
		EditorGUILayout.BeginVertical("Box");
		EditorGUILayout.HelpBox("Tweaking features below you will need shaders RECOMPILATION (may take a few minutes)  to see changes.\n\nHint: you can build shaders for choosen platforms only (i.e. only d3d9 when you're working on PC - this will compile much faster, remember to enable rest of platforms when releasing).",MessageType.Info, true);
		
		GUI.color=new Color(1,1,0.5f,1);
		EditorGUILayout.LabelField("Used shaders", EditorStyles.boldLabel);
		GUI.color=skin_color;		
		
		ReliefTerrain[] rts=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
		ReliefTerrain rt=null;
		for(int i=0; i<rts.Length; i++) {
			if (rts[i].GetComponent(typeof(Terrain))) {
				rt=rts[i];
				break;
			}
		}
#if !UNITY_3_5
		if (rt!=null && rt.globalSettingsHolder!=null) {
			EditorGUILayout.HelpBox("Unity4 allows to use material on terrain, but this can get insanely slow (CPU&GPU) for multiple terrains. Consider using this if you've got less than 9 terrain tiles ONLY. For 1-4 tiles it's OK and can benefit from additional GPU optimization.", MessageType.Warning, true);
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Use U4 terrain materials", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
				rt.globalSettingsHolder.useTerrainMaterial=EditorGUILayout.Toggle(rt.globalSettingsHolder.useTerrainMaterial);
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(15);
		}
		if (rt && (rt.globalSettingsHolder!=null) && rt.globalSettingsHolder.numTiles>1 && !rt.globalSettingsHolder.useTerrainMaterial) {
#else
		if (rt && (rt.globalSettingsHolder!=null) && rt.globalSettingsHolder.numTiles>1) {
#endif		
			bool prev_RTP_4LAYERS_MODE=_target.RTP_4LAYERS_MODE;
			_target.RTP_4LAYERS_MODE=true;
			if (prev_RTP_4LAYERS_MODE!=_target.RTP_4LAYERS_MODE) {
				CheckAddPassPresent();
			}
		}
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("RTP on terrain - first pass", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.SHADER_USAGE_FirstPass=EditorGUILayout.Toggle(_target.SHADER_USAGE_FirstPass);
			if (!_target.SHADER_USAGE_FirstPass) _target.SHADER_USAGE_AddPass=false;
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("RTP on terrain - add pass", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.SHADER_USAGE_AddPass=EditorGUILayout.Toggle(_target.SHADER_USAGE_AddPass);
		EditorGUILayout.EndHorizontal();
#if !UNITY_3_5
		EditorGUILayout.BeginHorizontal();
			if (rt!=null && rt.globalSettingsHolder!=null) {
				EditorGUI.BeginDisabledGroup(!(_target.SHADER_USAGE_FirstPass && rt.globalSettingsHolder.useTerrainMaterial));
				EditorGUILayout.LabelField("RTP on terrain - far distance", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
				_target.SHADER_USAGE_TerrainFarOnly=EditorGUILayout.Toggle(_target.SHADER_USAGE_FirstPass && rt.globalSettingsHolder.useTerrainMaterial);
				EditorGUI.EndDisabledGroup();
			} else {
				EditorGUILayout.LabelField("RTP on terrain - far distance", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
				_target.SHADER_USAGE_TerrainFarOnly=EditorGUILayout.Toggle(_target.SHADER_USAGE_FirstPass);
			}
		EditorGUILayout.EndHorizontal();
#endif		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("RTP on terrain - mesh blending", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.SHADER_USAGE_BlendBase=EditorGUILayout.Toggle(_target.SHADER_USAGE_BlendBase);
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();		
		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("RTP on mesh", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.SHADER_USAGE_Terrain2Geometry=EditorGUILayout.Toggle(_target.SHADER_USAGE_Terrain2Geometry);
			if ( _target.SHADER_USAGE_Terrain2Geometry && ((_target.numLayers>8) || (_target.numLayers>4 && _target.RTP_4LAYERS_MODE)) ) {
				_target.SHADER_USAGE_AddPassGeom=true;
			} else {
				_target.SHADER_USAGE_AddPassGeom=false;
			}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("RTP on mesh - mesh blending", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.SHADER_USAGE_Terrain2GeometryBlendBase=EditorGUILayout.Toggle(_target.SHADER_USAGE_Terrain2GeometryBlendBase);
		EditorGUILayout.EndHorizontal();
			
		EditorGUILayout.Space();		
		EditorGUILayout.Space();	
		
		GUI.color=new Color(1,1,0.5f,1);
		EditorGUILayout.LabelField("Target platforms", EditorStyles.boldLabel);
		GUI.color=skin_color;		
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("d3d9 (PC)", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.PLATFORM_D3D9=EditorGUILayout.Toggle(_target.PLATFORM_D3D9);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("opengl (Mac)", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.PLATFORM_OPENGL=EditorGUILayout.Toggle(_target.PLATFORM_OPENGL);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("gles (Mobile)", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.PLATFORM_GLES=EditorGUILayout.Toggle(_target.PLATFORM_GLES);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("flash", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.PLATFORM_FLASH=EditorGUILayout.Toggle(_target.PLATFORM_FLASH);
		EditorGUILayout.EndHorizontal();
//#if !UNITY_3_5
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("d3d11 (PC - U4 DX11 mode)", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.PLATFORM_D3D11=EditorGUILayout.Toggle(_target.PLATFORM_D3D11);
		EditorGUILayout.EndHorizontal();
//#endif
		
		EditorGUILayout.Space();		
		
		EditorGUILayout.HelpBox("When checked you'll have only simple lighting in forward rendering path (usable when you've got many lights in scene - performance might get low).", MessageType.None, true);
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("No forward add", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.RTP_NOFORWARDADD=EditorGUILayout.Toggle(_target.RTP_NOFORWARDADD);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.HelpBox("When checked you won't be able to use lightmapping on your terrain (mesh), but you'll have one more feature available that needs texture (rain droplets, caustics, vertical texture, dedicated snow color/normal or heightblend between passes inside addpass).", MessageType.None, true);
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("No lightmaps", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
			_target.RTP_NOLIGHTMAP=EditorGUILayout.Toggle(_target.RTP_NOLIGHTMAP);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();		
		
		if (_target.RTP_SUPER_SIMPLE) _target.RTP_SIMPLE_FAR_FIRST=true;
		if (_target.RTP_SUPER_SIMPLE) _target.RTP_SIMPLE_FAR_ADD=true;
		
		if (_target.SHADER_USAGE_FirstPass || _target.SHADER_USAGE_Terrain2Geometry) {
			samplers_left=1;
			if (_target.RTP_4LAYERS_MODE) samplers_left++;
			if (_target.RTP_NOLIGHTMAP) samplers_left++;
			samplers_used=0;
			if (!_target.RTP_SUPER_SIMPLE) {
				samplers_used+=(_target.RTP_WETNESS_FIRST && _target.RTP_WET_RIPPLE_TEXTURE_FIRST && !_target.SIMPLE_WATER_FIRST) ? 1:0;
				samplers_used+=_target.RTP_VERTICAL_TEXTURE_FIRST ? 1:0;
				samplers_used+=_target.RTP_CAUSTICS_FIRST ? 1:0;
				if (_target.RTP_4LAYERS_MODE && _target.RTP_SNOW_FIRST) {
					samplers_used+=(_target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST && _target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_FIRST>=4) ? 1:0;
					samplers_used+=(_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST && _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_FIRST>=4) ? 1:0;
				}
			} else if (_target.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST) {
				samplers_left+=3;
			}
			samplers_used+=_target.RTP_NORMALGLOBAL_FIRST ? 1:0;
			samplers_used+=_target.RTP_TREESGLOBAL_FIRST ? 1:0;
			
			if (samplers_used>samplers_left) {
				EditorGUILayout.HelpBox("Firstpass WON'T COMPILE. You're using "+samplers_used+" aux textures out of "+samplers_left+" available. Try to disable vertical texture, rain droplets, caustics, global normal/trees or change dedicated color/normal texture for snow. For Add Pass try to disable crosspass heightblend.",MessageType.Error, true);
			}
		}		

		if (_target.SHADER_USAGE_AddPass || _target.SHADER_USAGE_AddPassGeom) {		
			samplers_left=2;
			if (_target.RTP_NOLIGHTMAP) samplers_left++;
			samplers_used=0;
			if (!_target.RTP_SUPER_SIMPLE) {
				samplers_used+=(_target.RTP_WETNESS_ADD && _target.RTP_WET_RIPPLE_TEXTURE_ADD && !_target.SIMPLE_WATER_ADD) ? 1:0;
				samplers_used+=_target.RTP_VERTICAL_TEXTURE_ADD ? 1:0;
				samplers_used+=_target.RTP_CAUSTICS_ADD ? 1:0;
				samplers_used+=_target.RTP_CROSSPASS_HEIGHTBLEND ? 2:0;
				//if (_target.RTP_4LAYERS_MODE && _target.RTP_SNOW_ADD) {
				if (_target.RTP_SNOW_ADD) {
					samplers_used+=(_target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD && _target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_ADD>=4) ? 1:0;
					samplers_used+=(_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD && _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_ADD>=4) ? 1:0;
				}
//			} else if (_target.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD) {
//				samplers_left+=3;
			}
			samplers_used+=_target.RTP_NORMALGLOBAL_ADD ? 1:0;
			samplers_used+=_target.RTP_TREESGLOBAL_ADD ? 1:0;
			if (samplers_used>samplers_left) {
				EditorGUILayout.HelpBox("Addpass WON'T COMPILE. You're using "+samplers_used+" aux textures out of "+samplers_left+" available. Try to disable Crosspass heightblend, vertical texture, rain droplets, caustics, global normal/trees or change dedicated color/normal texture for snow.",MessageType.Error, true);
			}
		}			
		
		GUI.color=new Color(0.9f,1,0.9f,1);
		if (GUILayout.Button("Recompile shaders\nfor given feature set")) {
			RefreshFeatures();
			EditorUtility.SetDirty(_target);
		}
		EditorGUILayout.EndVertical();
		GUI.color=skin_color;		

		if (_target.SHADER_USAGE_FirstPass || _target.SHADER_USAGE_Terrain2Geometry) {
//////////////////////////////////////////////////////////////////////////////////////////////////////
// features - first pass
//		
		EditorGUILayout.BeginVertical("Box");
		GUI.color=new Color(1,1,0.5f,1);
		EditorGUILayout.LabelField("RTP features - First Pass (4 or 8 layers) & Arbitrary Mesh", EditorStyles.boldLabel);
		GUI.color=skin_color;		
		
		if (_target.show_first_features) {
			_target.show_first_features=EditorGUILayout.Foldout(_target.show_first_features, "Hide");
		} else {
			_target.show_first_features=EditorGUILayout.Foldout(_target.show_first_features, "Show");
		}
		
		if (_target.show_first_features) {
			bool _8_layers_disabled=false;
#if !UNITY_3_5				
			if (rt && (rt.globalSettingsHolder!=null) && !rt.globalSettingsHolder.useTerrainMaterial && rt.globalSettingsHolder.numTiles>1) {
				_8_layers_disabled=true;
				EditorGUILayout.HelpBox("8 LAYERS mode impossible for multiple terrains w/o materials (refer to \"Use U4 terrain materials\" above).",MessageType.Warning, true);
			}
#else
			if (rt && (rt.globalSettingsHolder!=null) && rt.globalSettingsHolder.numTiles>1) {
				_8_layers_disabled=true;
				EditorGUILayout.HelpBox("8 LAYERS mode impossible for multiple terrains in U3.",MessageType.Warning, true);
			}
#endif
			EditorGUI.BeginDisabledGroup(_8_layers_disabled);
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("8 LAYERS in first pass", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
				bool RTP_4LAYERS_MODE_prev=_target.RTP_4LAYERS_MODE;
				_target.RTP_4LAYERS_MODE=!EditorGUILayout.Toggle(!_target.RTP_4LAYERS_MODE);
				if (RTP_4LAYERS_MODE_prev!=_target.RTP_4LAYERS_MODE) {
					CheckAddPassPresent();
				}
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
				
			if (!_target.RTP_4LAYERS_MODE) {
				EditorGUILayout.HelpBox("In 8 layers rendered in frist pass (4 layers unchecked above) use below option to significantly speed-up rendering. Overlapping areas of layers 0-3 and 4-7 won't be rendered, but reduced to immediate transitions.",MessageType.None, true);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("  No overlap in 8 layers mode", GUILayout.MinWidth(176), GUILayout.MaxWidth(176));
					_target.RTP_HARD_CROSSPASS=EditorGUILayout.Toggle(_target.RTP_HARD_CROSSPASS);
				EditorGUILayout.EndHorizontal();
				if (!_target.RTP_HARD_CROSSPASS) {
					EditorGUILayout.HelpBox("Hint: organize splats the way areas that overlap are minimized - this will render faster.",MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("debug overlapped", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
						_target.RTP_SHOW_OVERLAPPED=EditorGUILayout.Toggle(_target.RTP_4LAYERS_MODE ? false : _target.RTP_SHOW_OVERLAPPED);
					EditorGUILayout.EndHorizontal();
				}			
				_target.RTP_SUPER_SIMPLE=false;
			}
				
			if (_target.RTP_4LAYERS_MODE) {
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Massive terrain mode takes very simple version of the RTP shader. This lets you use global color and optionaly - global normal, perlin, bumpmaps at close distance (where CLOSE now means - perlin distance !), pixel trees/shadows and very simple close-distance detail colors (grayscale tinted by global colormap).",MessageType.Warning, true);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Massive terrain mode", GUILayout.MinWidth(160), GUILayout.MaxWidth(160));
					_target.RTP_SUPER_SIMPLE=EditorGUILayout.Toggle(_target.RTP_SUPER_SIMPLE);
					if (_target.RTP_SUPER_SIMPLE) {
						_target.RTP_WETNESS_FIRST=false;
						_target.RTP_WETNESS_ADD=false;
					}
				EditorGUILayout.EndHorizontal();
			}
				
			ReliefTerrain[] _script_objs=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
			for(int p=0; p<_script_objs.Length; p++) {
				_script_objs[p].globalSettingsHolder.super_simple_active=_target.RTP_SUPER_SIMPLE;
			}				
				
			EditorGUILayout.Space();
			
			if (!_target.RTP_SUPER_SIMPLE)  {
				EditorGUILayout.HelpBox("Actual shader LOD will be selected when it's lower or equal \"MaxLOD\" below. In triplanar POM shading is unavailable (will be reduced to PM).", MessageType.Warning, true);
				_target.MAX_LOD_FIRST=(RTPLodLevel)EditorGUILayout.EnumPopup("MaxLOD",_target.MAX_LOD_FIRST);
				if (_target.RTP_TRIPLANAR_FIRST && (int)_target.MAX_LOD_FIRST<(int)RTPLodLevel.PM) _target.MAX_LOD_FIRST=RTPLodLevel.PM;
				if (!_target.RTP_4LAYERS_MODE && _target.RTP_HARD_CROSSPASS) {
					EditorGUILayout.HelpBox("\"MaxLOD for 4-7\" has to be lower or equal than \"MaxLOD\" and will be applied to layers 4-7 in 8 layers mode with no overlapping.", MessageType.Warning, true);
					_target.MAX_LOD_FIRST_PLUS4=(RTPLodLevel)EditorGUILayout.EnumPopup("MaxLOD for 4-7", _target.MAX_LOD_FIRST_PLUS4);
					if ((int)_target.MAX_LOD_FIRST_PLUS4<(int)_target.MAX_LOD_FIRST) _target.MAX_LOD_FIRST_PLUS4=_target.MAX_LOD_FIRST;
				}
				if ((int)_target.MAX_LOD_ADD<(int)_target.MAX_LOD_FIRST) {
					_target.MAX_LOD_ADD=_target.MAX_LOD_FIRST;
					EditorUtility.DisplayDialog("RTP Notification", "AddPass MaxLOD level shouldn't be greater than FirstPass MaxLOD.","OK");
				}
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("Box");
			
			EditorGUILayout.HelpBox("This setting speeds-up far distance rendering (we don't use splat detail colors there).", MessageType.None, true);
			EditorGUI.BeginDisabledGroup(_target.RTP_SUPER_SIMPLE);
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("No detail colors at far distance", GUILayout.MinWidth(200), GUILayout.MaxWidth(200));
				_target.RTP_SIMPLE_FAR_FIRST=EditorGUILayout.Toggle(_target.RTP_SIMPLE_FAR_FIRST);
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
			
			EditorGUILayout.Space();	
				
			if (_target.RTP_SUPER_SIMPLE)  {
				//EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Detail colors as grayscale combined", GUILayout.MinWidth(225), GUILayout.MaxWidth(225));
					_target.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST=EditorGUILayout.Toggle(_target.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST);
				EditorGUILayout.EndHorizontal();
				//EditorGUI.EndDisabledGroup();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Use detail bump maps", GUILayout.MinWidth(225), GUILayout.MaxWidth(225));
					_target.RTP_USE_BUMPMAPS_FIRST=EditorGUILayout.Toggle(_target.RTP_USE_BUMPMAPS_FIRST);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Use perlin normal", GUILayout.MinWidth(225), GUILayout.MaxWidth(225));
					_target.RTP_USE_PERLIN_FIRST=EditorGUILayout.Toggle(_target.RTP_USE_PERLIN_FIRST);
				EditorGUILayout.EndHorizontal();
			}
				
			if (!_target.RTP_SUPER_SIMPLE)  {
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("UV blend", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_UV_BLEND_FIRST=EditorGUILayout.Toggle(_target.RTP_UV_BLEND_FIRST);
				EditorGUILayout.EndHorizontal();
				if (_target.RTP_UV_BLEND_FIRST) {
					if (_target.RTP_SIMPLE_FAR_FIRST && _target.RTP_DISTANCE_ONLY_UV_BLEND_FIRST) {
						EditorGUILayout.HelpBox("Using \"No detail colors at far distance\" with option below does not make much sense (result will be almost unnoticeable).", MessageType.Warning, true);
					}
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("   UV blend at distance only", GUILayout.MinWidth(164), GUILayout.MaxWidth(164));
						_target.RTP_DISTANCE_ONLY_UV_BLEND_FIRST=EditorGUILayout.Toggle(_target.RTP_DISTANCE_ONLY_UV_BLEND_FIRST, GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.HelpBox("Here you map UV blend layers so that one layer can be uv blended with another.", MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						string[] options=new string[4];
						for(int k=0; k<(_target.RTP_4LAYERS_MODE ? 4:8); k++) {
							for(int j=0; j<4; j++) {
								options[j]=k+" from "+j;
							}
							_target.UV_BLEND_ROUTE_NUM_FIRST[k]=EditorGUILayout.Popup(_target.UV_BLEND_ROUTE_NUM_FIRST[k], options);
							if (k==3 && !_target.RTP_4LAYERS_MODE) {
								EditorGUILayout.EndHorizontal();
								EditorGUILayout.BeginHorizontal();
							}
						}
					EditorGUILayout.EndHorizontal();
				}			
			}	// super-simple
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Global color blend multiplicative", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
				_target.RTP_COLOR_MAP_BLEND_MULTIPLY_FIRST=EditorGUILayout.Toggle(_target.RTP_COLOR_MAP_BLEND_MULTIPLY_FIRST);
			EditorGUILayout.EndHorizontal();
				
			if (!_target.RTP_SUPER_SIMPLE)  {	
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Layer extrude reduction", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_USE_EXTRUDE_REDUCTION_FIRST=EditorGUILayout.Toggle(_target.RTP_USE_EXTRUDE_REDUCTION_FIRST);
				EditorGUILayout.EndHorizontal();
					
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Superdetail", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_SUPER_DETAIL_FIRST=EditorGUILayout.Toggle(_target.RTP_SUPER_DETAIL_FIRST);
				EditorGUILayout.EndHorizontal();
		
				EditorGUI.BeginDisabledGroup(!_target.RTP_SUPER_DETAIL_FIRST);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("  Mult channels", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					bool prev_superdetail=_target.RTP_SUPER_DETAIL_MULTS_FIRST;
					_target.RTP_SUPER_DETAIL_MULTS_FIRST=EditorGUILayout.Toggle(_target.RTP_SUPER_DETAIL_MULTS_FIRST);
					if (prev_superdetail!=_target.RTP_SUPER_DETAIL_MULTS_FIRST) {
						if (_target.RTP_WETNESS_FIRST) {
							EditorUtility.DisplayDialog("Notification","Turning off water feature", "OK");
							_target.RTP_WETNESS_FIRST=false;
						}
						if (_target.RTP_REFLECTION_FIRST) {
							EditorUtility.DisplayDialog("Notification","Turning off reflections feature", "OK");
							_target.RTP_REFLECTION_FIRST=false;
						}
					}				
				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Dynamic snow", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_SNOW_FIRST=EditorGUILayout.Toggle(_target.RTP_SNOW_FIRST);
				EditorGUILayout.EndHorizontal();
				if (_target.RTP_SNOW_FIRST) {
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Layer for color", GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
						_target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST=EditorGUILayout.Toggle(_target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST, GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
						EditorGUI.BeginDisabledGroup( !_target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST );
							_target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_FIRST=EditorGUILayout.IntSlider(_target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_FIRST,0,7);
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
				
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Layer for normal", GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
						_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST=EditorGUILayout.Toggle(_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST, GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
				
						EditorGUI.BeginDisabledGroup( !_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST );
							_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_FIRST=EditorGUILayout.IntSlider(_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_FIRST,0,7);
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
				}		
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Vertical texture map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_VERTICAL_TEXTURE_FIRST=EditorGUILayout.Toggle(_target.RTP_VERTICAL_TEXTURE_FIRST);
				EditorGUILayout.EndHorizontal();
			} // super-simple
	
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Global normal map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
				_target.RTP_NORMALGLOBAL_FIRST=EditorGUILayout.Toggle(_target.RTP_NORMALGLOBAL_FIRST);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Global trees map (Terrain / World Composer)", GUILayout.MinWidth(270), GUILayout.MaxWidth(270));
				_target.RTP_TREESGLOBAL_FIRST=EditorGUILayout.Toggle(_target.RTP_TREESGLOBAL_FIRST);
			EditorGUILayout.EndHorizontal();

			if (!_target.RTP_SUPER_SIMPLE)  {	
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Water/wetness", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					bool prev_wetness=_target.RTP_WETNESS_FIRST;
					_target.RTP_WETNESS_FIRST=EditorGUILayout.Toggle(_target.RTP_WETNESS_FIRST);
					if (prev_wetness!=_target.RTP_WETNESS_FIRST && _target.RTP_WETNESS_FIRST) {
						if (_target.RTP_SUPER_DETAIL_MULTS_FIRST) {
							EditorUtility.DisplayDialog("Notification","Turning off superdetail mults feature", "OK");
							_target.RTP_SUPER_DETAIL_MULTS_FIRST=false;
						}
					}
					if (!_target.RTP_WETNESS_FIRST) {
						_target.SIMPLE_WATER_FIRST=false;
						_target.RTP_WET_RIPPLE_TEXTURE_FIRST=false;
					}
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Simple water only", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.SIMPLE_WATER_FIRST=EditorGUILayout.Toggle(_target.SIMPLE_WATER_FIRST);
				EditorGUILayout.EndHorizontal();
				
				EditorGUI.BeginDisabledGroup(!_target.RTP_WETNESS_FIRST || _target.SIMPLE_WATER_FIRST);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("  Animated droplets", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
						_target.RTP_WET_RIPPLE_TEXTURE_FIRST=EditorGUILayout.Toggle(_target.RTP_WET_RIPPLE_TEXTURE_FIRST);
					EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Caustics", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_CAUSTICS_FIRST=EditorGUILayout.Toggle(_target.RTP_CAUSTICS_FIRST);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Reflection map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					bool prev_reflection=_target.RTP_REFLECTION_FIRST;
					_target.RTP_REFLECTION_FIRST=EditorGUILayout.Toggle(_target.RTP_REFLECTION_FIRST);
					if (prev_reflection!=_target.RTP_REFLECTION_FIRST) {
						if (_target.RTP_SUPER_DETAIL_MULTS_FIRST) {
							EditorUtility.DisplayDialog("Notification","Turning off superdetail mults feature", "OK");
							_target.RTP_SUPER_DETAIL_MULTS_FIRST=false;
						}
					}				
				EditorGUILayout.EndHorizontal();
				EditorGUI.BeginDisabledGroup(!_target.RTP_REFLECTION_FIRST);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("  Rotate reflection map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
						_target.RTP_ROTATE_REFLECTION=EditorGUILayout.Toggle(_target.RTP_ROTATE_REFLECTION);
					EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				
//				EditorGUI.BeginDisabledGroup(!_target.RTP_4LAYERS_MODE);
//					EditorGUILayout.HelpBox("In 4 layers mode we can use shadow maps that speed-up shadow calculations (one color atlas also needed like in 8 layers mode).",MessageType.None, true);
//					EditorGUILayout.BeginHorizontal();
//						EditorGUILayout.LabelField("Self-shadow maps", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
//						_target.RTP_MAPPED_SHADOWS_FIRST=EditorGUILayout.Toggle(_target.RTP_MAPPED_SHADOWS_FIRST);
//					EditorGUILayout.EndHorizontal();
//				EditorGUI.EndDisabledGroup();
		
//				EditorGUILayout.Space();
//				EditorGUILayout.Space();
				
				EditorGUILayout.HelpBox("Triplanar is only available in first pass that handle 4 layers. In triplanar POM is reduced to PM.",MessageType.None, true);
				EditorGUILayout.BeginHorizontal();
					EditorGUI.BeginDisabledGroup( !_target.RTP_4LAYERS_MODE );
					EditorGUILayout.LabelField("First-Pass Triplanar", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					if (_target.RTP_4LAYERS_MODE) {
						_target.RTP_TRIPLANAR_FIRST=EditorGUILayout.Toggle(_target.RTP_TRIPLANAR_FIRST);
					} else {
						_target.RTP_TRIPLANAR_FIRST=EditorGUILayout.Toggle(false);
					}	
					EditorGUI.EndDisabledGroup();
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
			} // super-simple
		}
		EditorGUILayout.EndVertical(); // features - first pass
		} //EOF if (used firstpass shader or rtp on mesh)
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		if (_target.SHADER_USAGE_AddPass || _target.SHADER_USAGE_AddPassGeom) {		
			
//////////////////////////////////////////////////////////////////////////////////////////////////////
// features - add pass
//		
		EditorGUILayout.BeginVertical("Box");
		GUI.color=new Color(1,1,0.5f,1);
		EditorGUILayout.LabelField("RTP features - Add Pass (4layers)", EditorStyles.boldLabel);
		GUI.color=skin_color;		
		
		if (_target.show_add_features) {
			_target.show_add_features=EditorGUILayout.Foldout(_target.show_add_features, "Hide");
		} else {
			_target.show_add_features=EditorGUILayout.Foldout(_target.show_add_features, "Show");
		}
		
		if (_target.show_add_features) {
			if (!_target.RTP_SUPER_SIMPLE)  {
				EditorGUILayout.HelpBox("Actual shader LOD will be selected when it's lower or equal \"MaxLOD\" below. In triplanar POM shading is unavailable (will be reduced to PM).", MessageType.Warning, true);
				_target.MAX_LOD_ADD=(RTPLodLevel)EditorGUILayout.EnumPopup("MaxLOD",_target.MAX_LOD_ADD);
				if (_target.RTP_TRIPLANAR_ADD && (int)_target.MAX_LOD_ADD<(int)RTPLodLevel.PM) _target.MAX_LOD_ADD=RTPLodLevel.PM;
			}
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("Box");		
			
			if (!_target.RTP_SUPER_SIMPLE) {
				EditorGUILayout.HelpBox("When add pass is present (using 8 layers in 4 layers per pass mode), you can ask shaders to make height blending between passes. Works for terrains only (shader on arbitrary mesh doesn't have such thing as add pass). Doesn't work when 12 layers are used.",MessageType.Warning, true);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Crosspass heightblend", GUILayout.MinWidth(176), GUILayout.MaxWidth(176));
					_target.RTP_CROSSPASS_HEIGHTBLEND=EditorGUILayout.Toggle(_target.RTP_CROSSPASS_HEIGHTBLEND);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
			}			
				
			EditorGUILayout.HelpBox("This setting speeds-up far distance rendering (we don't use splat detail colors there).", MessageType.None, true);
			EditorGUI.BeginDisabledGroup(_target.RTP_SUPER_SIMPLE);
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("No detail colors at far distance", GUILayout.MinWidth(200), GUILayout.MaxWidth(200));
				_target.RTP_SIMPLE_FAR_ADD=EditorGUILayout.Toggle(_target.RTP_SIMPLE_FAR_ADD);
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
				
			EditorGUILayout.Space();
				
			if (_target.RTP_SUPER_SIMPLE)  {
//				//EditorGUI.BeginDisabledGroup(true);
//				EditorGUILayout.BeginHorizontal();
//					EditorGUILayout.LabelField("Detail colors as grayscale combined", GUILayout.MinWidth(225), GUILayout.MaxWidth(225));
//					_target.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD=EditorGUILayout.Toggle(_target.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD);
//				EditorGUILayout.EndHorizontal();
//				//EditorGUI.EndDisabledGroup();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Use detail bump maps", GUILayout.MinWidth(225), GUILayout.MaxWidth(225));
					_target.RTP_USE_BUMPMAPS_ADD=EditorGUILayout.Toggle(_target.RTP_USE_BUMPMAPS_ADD);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Use perlin normal", GUILayout.MinWidth(225), GUILayout.MaxWidth(225));
					_target.RTP_USE_PERLIN_ADD=EditorGUILayout.Toggle(_target.RTP_USE_PERLIN_ADD);
				EditorGUILayout.EndHorizontal();
			}
				
			if (!_target.RTP_SUPER_SIMPLE)  {			
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("UV blend", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_UV_BLEND_ADD=EditorGUILayout.Toggle(_target.RTP_UV_BLEND_ADD);
				EditorGUILayout.EndHorizontal();
				if (_target.RTP_UV_BLEND_ADD) {
					if (_target.RTP_SIMPLE_FAR_ADD && _target.RTP_DISTANCE_ONLY_UV_BLEND_ADD) {
						EditorGUILayout.HelpBox("Using \"No detail colors at far distance\" with option below does not make much sense (result will be almost unnoticeable).", MessageType.Warning, true);
					}
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("   UV blend at distance only", GUILayout.MinWidth(164), GUILayout.MaxWidth(164));
						_target.RTP_DISTANCE_ONLY_UV_BLEND_ADD=EditorGUILayout.Toggle(_target.RTP_DISTANCE_ONLY_UV_BLEND_ADD, GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.HelpBox("Here you map UV blend layers so that one layer can be uv blended with another.", MessageType.None, true);
					EditorGUILayout.BeginHorizontal();
						string[] options=new string[4];
						for(int k=0; k<4; k++) {
							for(int j=0; j<4; j++) {
								options[j]=k+" from "+j;
							}
							_target.UV_BLEND_ROUTE_NUM_FIRST[k]=EditorGUILayout.Popup(_target.UV_BLEND_ROUTE_NUM_FIRST[k], options);
	//						if (k==3 && !_target.RTP_4LAYERS_MODE) {
	//							EditorGUILayout.EndHorizontal();
	//							EditorGUILayout.BeginHorizontal();
	//						}
						}
					EditorGUILayout.EndHorizontal();
				}			
			}	// super-simple
			EditorGUILayout.Space();
			
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Global color blend multiplicative", GUILayout.MinWidth(190), GUILayout.MaxWidth(190));
				_target.RTP_COLOR_MAP_BLEND_MULTIPLY_ADD=EditorGUILayout.Toggle(_target.RTP_COLOR_MAP_BLEND_MULTIPLY_ADD);
			EditorGUILayout.EndHorizontal();
			if (!_target.RTP_SUPER_SIMPLE)  {			
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Layer extrude reduction", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_USE_EXTRUDE_REDUCTION_ADD=EditorGUILayout.Toggle(_target.RTP_USE_EXTRUDE_REDUCTION_ADD);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Superdetail", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_SUPER_DETAIL_ADD=EditorGUILayout.Toggle(_target.RTP_SUPER_DETAIL_ADD);
					
				EditorGUILayout.EndHorizontal();
		
				EditorGUI.BeginDisabledGroup(!_target.RTP_SUPER_DETAIL_ADD);
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("  Mult channels", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					bool prev_superdetail=_target.RTP_SUPER_DETAIL_MULTS_ADD;
					_target.RTP_SUPER_DETAIL_MULTS_ADD=EditorGUILayout.Toggle(_target.RTP_SUPER_DETAIL_MULTS_ADD);
					if (prev_superdetail!=_target.RTP_SUPER_DETAIL_MULTS_ADD) {
						if (_target.RTP_WETNESS_ADD) {
							EditorUtility.DisplayDialog("Notification","Turning off water feature", "OK");
							_target.RTP_WETNESS_ADD=false;
						}
						if (_target.RTP_REFLECTION_ADD) {
							EditorUtility.DisplayDialog("Notification","Turning off reflections feature", "OK");
							_target.RTP_REFLECTION_ADD=false;
						}
					}	
				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Dynamic snow", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_SNOW_ADD=EditorGUILayout.Toggle(_target.RTP_SNOW_ADD);
				EditorGUILayout.EndHorizontal();
				if (_target.RTP_SNOW_ADD) {
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Layer for color", GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
						_target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD=EditorGUILayout.Toggle(_target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD, GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
						EditorGUI.BeginDisabledGroup( !_target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD );
							_target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_ADD=EditorGUILayout.IntSlider(_target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_ADD,0,3);
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
				
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("Layer for normal", GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
						_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD=EditorGUILayout.Toggle(_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD, GUILayout.MinWidth(20), GUILayout.MaxWidth(20));
						EditorGUI.BeginDisabledGroup( !_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD );
							_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_ADD=EditorGUILayout.IntSlider(_target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_ADD,0,3);
						EditorGUI.EndDisabledGroup();
					EditorGUILayout.EndHorizontal();
				}					
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Vertical texture map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_VERTICAL_TEXTURE_ADD=EditorGUILayout.Toggle(_target.RTP_VERTICAL_TEXTURE_ADD);
				EditorGUILayout.EndHorizontal();
			} // super-simple
				
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Global normal map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
				_target.RTP_NORMALGLOBAL_ADD=EditorGUILayout.Toggle(_target.RTP_NORMALGLOBAL_ADD);
			EditorGUILayout.EndHorizontal();
					
			///////////////////////////////////////////////////////////////////////////////////////
			// sprawdz konflikt tekstur (jest uzywana przez crosspass heightblend)
			//
			bool _8_layers_disabled=false;
#if !UNITY_3_5				
			if (rt!=null && rt.globalSettingsHolder!=null && !rt.globalSettingsHolder.useTerrainMaterial && rt.globalSettingsHolder.numTiles>1 && _target.RTP_CROSSPASS_HEIGHTBLEND) {
				_8_layers_disabled=true;
			}
#else
			if (rt && (rt.globalSettingsHolder!=null) && rt.globalSettingsHolder.numTiles>1 && !_target.RTP_SUPER_SIMPLE && _target.RTP_CROSSPASS_HEIGHTBLEND) {
				_8_layers_disabled=true;
			}
#endif
			if (_8_layers_disabled) {
				EditorGUILayout.HelpBox("Feature unavailable in AddPass for multiple terrains with \"Crosspass heightblend\" switched on (above).",MessageType.Warning, true);
				_target.RTP_TREESGLOBAL_ADD=false;
			}
			EditorGUI.BeginDisabledGroup(_8_layers_disabled);					
			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Global trees map (Terrain / World Composer)", GUILayout.MinWidth(270), GUILayout.MaxWidth(270));
				_target.RTP_TREESGLOBAL_ADD=EditorGUILayout.Toggle(_target.RTP_TREESGLOBAL_ADD);
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();					
			///////////////////////////////////////////////////////////////////////////////////////

			if (!_target.RTP_SUPER_SIMPLE)  {	
				//		EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Water/wetness", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					bool prev_wetness=_target.RTP_WETNESS_ADD;
					_target.RTP_WETNESS_ADD=EditorGUILayout.Toggle(_target.RTP_WETNESS_ADD);
					if (prev_wetness!=_target.RTP_WETNESS_ADD && _target.RTP_WETNESS_ADD) {
						if (_target.RTP_SUPER_DETAIL_MULTS_ADD) {
							EditorUtility.DisplayDialog("Notification","Turning off superdetail mults feature", "OK");
							_target.RTP_SUPER_DETAIL_MULTS_ADD=false;
						}
					}
					if (!_target.RTP_WETNESS_ADD) {
						_target.SIMPLE_WATER_ADD=false;
						_target.RTP_WET_RIPPLE_TEXTURE_ADD=false;
					}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Simple water only", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.SIMPLE_WATER_ADD=EditorGUILayout.Toggle(_target.SIMPLE_WATER_ADD);
				EditorGUILayout.EndHorizontal();
				
				EditorGUI.BeginDisabledGroup(!_target.RTP_WETNESS_ADD || _target.SIMPLE_WATER_ADD);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("  Animated droplets", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
						_target.RTP_WET_RIPPLE_TEXTURE_ADD=EditorGUILayout.Toggle(_target.RTP_WET_RIPPLE_TEXTURE_ADD);
					EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
					
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Caustics", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_CAUSTICS_ADD=EditorGUILayout.Toggle(_target.RTP_CAUSTICS_ADD);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Reflection map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					bool prev_reflection=_target.RTP_REFLECTION_ADD;
					_target.RTP_REFLECTION_ADD=EditorGUILayout.Toggle(_target.RTP_REFLECTION_ADD);
					if (prev_reflection!=_target.RTP_REFLECTION_ADD) {
						if (_target.RTP_SUPER_DETAIL_MULTS_ADD) {
							EditorUtility.DisplayDialog("Notification","Turning off superdetail mults feature", "OK");
							_target.RTP_SUPER_DETAIL_MULTS_ADD=false;
						}
					}	
				EditorGUILayout.EndHorizontal();
				EditorGUI.BeginDisabledGroup(!_target.RTP_REFLECTION_ADD);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("  Rotate reflection map", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
						_target.RTP_ROTATE_REFLECTION=EditorGUILayout.Toggle(_target.RTP_ROTATE_REFLECTION);
					EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				
	//			EditorGUI.BeginDisabledGroup(!_target.RTP_4LAYERS_MODE);
	//				EditorGUILayout.HelpBox("In 4 layers mode we can use shadow maps that speed-up shadow calculations (one color atlas also needed like in 8 layers mode).",MessageType.None, true);
	//				EditorGUILayout.BeginHorizontal();
	//					EditorGUILayout.LabelField("Self-shadow maps", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
	//					_target.RTP_MAPPED_SHADOWS_ADD=EditorGUILayout.Toggle(_target.RTP_MAPPED_SHADOWS_ADD);
	//				EditorGUILayout.EndHorizontal();
	//			EditorGUI.EndDisabledGroup();
				
				EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Add-Pass Triplanar", GUILayout.MinWidth(145), GUILayout.MaxWidth(145));
					_target.RTP_TRIPLANAR_ADD=EditorGUILayout.Toggle(_target.RTP_TRIPLANAR_ADD);
				EditorGUILayout.EndHorizontal();
				
				EditorGUILayout.Space();
				
				//if (_target.RTP_4LAYERS_MODE) {
					EditorGUILayout.HelpBox("AddPass is automatically fired for terrain by Unity. However it WON'T be automatically fired for geometry blending base shader when needed.\n\nWhen 2 passes are present on the terrain you SHOULD check this option. DON'T check it when you've got one pass. If number of passes used changed you might need to recompile shaders for your geom blending object to start working fine.", MessageType.Error, true);
					EditorGUILayout.BeginHorizontal();
						EditorGUILayout.LabelField("AddPass for geometry blend base", GUILayout.MinWidth(200), GUILayout.MaxWidth(200));
						_target.ADDPASS_IN_BLENDBASE=EditorGUILayout.Toggle(_target.ADDPASS_IN_BLENDBASE);
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.Space();
				//}
			}
		} // super-simple
		EditorGUILayout.EndVertical(); // features - add pass
//////////////////////////////////////////////////////////////////////////////////////////////////////
		} //EOF if (used addpass shader)
		
		EditorGUILayout.Space();
	
		EditorGUILayout.BeginVertical("Box");

		EditorGUILayout.HelpBox("By default classic shading isn't \"pure classic\" but handles global colorMap and snow (if enabled). Disable this additional features when desired.",MessageType.None, true);
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Additional features in classic fallback", GUILayout.MinWidth(220), GUILayout.MaxWidth(220));
			_target.RTP_ADDITIONAL_FEATURES_IN_FALLBACKS=EditorGUILayout.Toggle(_target.RTP_ADDITIONAL_FEATURES_IN_FALLBACKS);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}
	
	public void RefreshFeatures() {
		force_rebuild=false;
		bool base_changed=RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/RTP_Base.cginc", false, false, false);
		bool add_changed=RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/RTP_AddBase.cginc", false, true, false);
		rebuild_noforwardadd=true;
		force_rebuild=base_changed;
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FirstPass.shader", true, false, false);
		#if !UNITY_3_5
		force_rebuild=base_changed || add_changed;
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FarOnly.shader", true, false, false);
		#endif
		force_rebuild=add_changed;
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-AddPass.shader", true, true, false);
		force_rebuild=base_changed;
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain2Geometry.shader", true, false, true, true);
		force_rebuild=base_changed || add_changed;
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/Internal/ReliefTerrainGeometryBlendBase.shader", true, false, false, true);
		force_rebuild=base_changed || add_changed;
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/Internal/ReliefTerrain2GeometryBlendBase.shader", true, false, true, true);
		rebuild_noforwardadd=false;
		force_rebuild=false;
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/GeomBlendCompliant/GeometryBlend_BumpedDetailSnow.shader", true, false, false);
		RebuildFeaturesInFile("Assets/ReliefPack/Shaders/ReliefTerrain/GeomBlendCompliant/GeometryBlend_POMDetailSnow.shader", true, false, false);
		AssetDatabase.Refresh();
	}
	
	private bool RebuildFeaturesInFile(string shader_path, bool shader_flag=true, bool AddPass_flag=true, bool geom_flag=false, bool blend_base=false) {
		RTP_LODmanager _target=(RTP_LODmanager)target;
		
		bool changed=false;
		
		ReliefTerrain obj=(ReliefTerrain)GameObject.FindObjectOfType(typeof(ReliefTerrain));
		bool addPassPresent=true;
		if (obj) {
			int act_layer_num=obj.globalSettingsHolder.numLayers;
			addPassPresent=false;
			if (act_layer_num<=4) {
				addPassPresent=false;
			} else if (act_layer_num<=8) {
				if (_target.RTP_4LAYERS_MODE) {
					addPassPresent=true;
				} else {
					addPassPresent=false;
				}
			} else {
				addPassPresent=true;
			}
		}
		
		if (System.IO.File.Exists(shader_path)) {
			int idx,sidx;
			bool flag;
			
			string _code_orig = System.IO.File.ReadAllText(shader_path);
			string _code = System.IO.File.ReadAllText(shader_path);
			if (shader_flag) {
				
#if !UNITY_3_5
			ReliefTerrain rt=(ReliefTerrain)GameObject.FindObjectOfType(typeof(ReliefTerrain));
			if (rt) {
				if (	rt.globalSettingsHolder.useTerrainMaterial) {
					if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FirstPass.shader") {
						_code=_code.Replace("Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass", "Relief Pack/ReliefTerrain-FirstPass");
					} else if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-AddPass.shader") {
						_code=_code.Replace("Hidden/TerrainEngine/Splatmap/Lightmap-AddPass", "Relief Pack/ReliefTerrain-AddPass");
					}
				} else {
					Terrain[] terrainComps=(Terrain[])GameObject.FindObjectsOfType(typeof(Terrain));
					for(int i=0; i<terrainComps.Length; i++) {
						terrainComps[i].basemapDistance=20000;
					}
					if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FirstPass.shader") {
						_code=_code.Replace("Relief Pack/ReliefTerrain-FirstPass", "Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass");
					} else if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-AddPass.shader") {
						_code=_code.Replace("Relief Pack/ReliefTerrain-AddPass", "Hidden/TerrainEngine/Splatmap/Lightmap-AddPass");
					}
				}
			}
#endif				
				
				//shader usage
				bool usage_check=false;
				bool used=false;
				if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FirstPass.shader") {
					usage_check=true;
					used=_target.SHADER_USAGE_FirstPass;
				} else if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-AddPass.shader") {
					usage_check=true;
					used=_target.SHADER_USAGE_AddPass || _target.SHADER_USAGE_AddPassGeom;
				} else if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FarOnly.shader") {
					usage_check=true;
					used=_target.SHADER_USAGE_TerrainFarOnly;
				} else if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/Internal/ReliefTerrainGeometryBlendBase.shader") {
					usage_check=true;
					used=_target.SHADER_USAGE_BlendBase;
				} else if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain2Geometry.shader") {
					usage_check=true;
					used=_target.SHADER_USAGE_Terrain2Geometry;
				} else if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/Internal/ReliefTerrain2GeometryBlendBase.shader") {
					usage_check=true;
					used=_target.SHADER_USAGE_Terrain2GeometryBlendBase;
				}
				
				if (usage_check) {
					int astar_replace_begin_idx=0;
					int astar_replace_end_idx=0;
					if (used) {
						//
						// used
						//
						// INIT comment - BEGIN
						sidx=0;
						idx=_code.IndexOf("///* INIT",sidx);
						if (idx<0) idx=_code.IndexOf("/* INIT",sidx);
						if (idx>0) {
							astar_replace_begin_idx=idx+5;
							string _code_beg=_code.Substring(0,idx);
							string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
							_code=_code_beg;
							_code+="///* INIT";
							_code+=_code_end;
						}
						
					// INIT comment - END
					sidx=0;
						idx=_code.IndexOf("//*/ // INIT",sidx);
						if (idx<0) idx=_code.IndexOf("*/ // INIT",sidx);
						if (idx>0) {
							astar_replace_end_idx=idx-5;
							string _code_beg=_code.Substring(0,idx);
							string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
							_code=_code_beg;
							_code+="//*/ // INIT";
							_code+=_code_end;
						}
						if (astar_replace_begin_idx>0 && astar_replace_end_idx>0) {
							_code=_code.Substring(0,astar_replace_begin_idx)+_code.Substring(astar_replace_begin_idx,astar_replace_end_idx-astar_replace_begin_idx).Replace("/astar","/*")+_code.Substring(astar_replace_end_idx);
							_code=_code.Substring(0,astar_replace_begin_idx)+_code.Substring(astar_replace_begin_idx,astar_replace_end_idx-astar_replace_begin_idx).Replace("astar/","*/")+_code.Substring(astar_replace_end_idx);
						}
						
					} else {
						//
						// not used
						//
						// INIT comment - BEGIN
						sidx=0;
						idx=_code.IndexOf("///* INIT",sidx);
						if (idx<0) idx=_code.IndexOf("/* INIT",sidx);
						if (idx>0) {
							astar_replace_begin_idx=idx+5;
							string _code_beg=_code.Substring(0,idx);
							string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
							_code=_code_beg;
							_code+="/* INIT";
							_code+=_code_end;
						}
						
						// INIT comment - END
						sidx=0;
						idx=_code.IndexOf("//*/ // INIT",sidx);
						if (idx<0) idx=_code.IndexOf("*/ // INIT",sidx);
						if (idx>0) {
							astar_replace_end_idx=idx-5;
							string _code_beg=_code.Substring(0,idx);
							string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
							_code=_code_beg;
							_code+="*/ // INIT";
							_code+=_code_end;
						}
						if (astar_replace_begin_idx>0 && astar_replace_end_idx>0) {
							_code=_code.Substring(0,astar_replace_begin_idx)+_code.Substring(astar_replace_begin_idx,astar_replace_end_idx-astar_replace_begin_idx).Replace("/*", "/astar")+_code.Substring(astar_replace_end_idx);
							_code=_code.Substring(0,astar_replace_begin_idx)+_code.Substring(astar_replace_begin_idx,astar_replace_end_idx-astar_replace_begin_idx).Replace("*/", "astar/")+_code.Substring(astar_replace_end_idx);
						}
						
					}
				}
				
				// render targets
				sidx=0;
				do {				
					flag=false;
					idx=_code.IndexOf("#pragma only_renderers",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						_code+="#pragma only_renderers";
						if (_target.PLATFORM_D3D9) _code+=" d3d9";
						if (_target.PLATFORM_OPENGL) _code+=" opengl";
						if (_target.PLATFORM_GLES) _code+=" gles";
						if (_target.PLATFORM_FLASH) _code+=" flash";
//#if !UNITY_3_5
						if (_target.PLATFORM_D3D11) _code+=" d3d11";
//#endif
						_code+=_code_end;
					}
				} while(flag);
			}
			
			// snow
			ChangeShaderDef(ref _code, "RTP_SNOW", AddPass_flag ? _target.RTP_SNOW_ADD : _target.RTP_SNOW_FIRST);
			
			// mapped shadows
			ChangeShaderDef(ref _code, "RTP_MAPPED_SHADOWS", AddPass_flag ? _target.RTP_MAPPED_SHADOWS_ADD : _target.RTP_MAPPED_SHADOWS_FIRST);
			
			
			// snow layer color
			sidx=0;
			do {				
				flag=false;
				idx=_code.IndexOf("//#define RTP_SNW_CHOOSEN_LAYER_COLOR_",sidx);
				if (idx<0) idx=_code.IndexOf("#define RTP_SNW_CHOOSEN_LAYER_COLOR_",sidx);
				if (idx>0) {
					flag=true; sidx=idx+5; // search next
					string _code_beg=_code.Substring(0,idx);
					string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
					_code=_code_beg;
					if ((AddPass_flag ? _target.RTP_SNOW_ADD : _target.RTP_SNOW_FIRST) && (AddPass_flag ? _target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD : _target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST)) {
						_code+="#define RTP_SNW_CHOOSEN_LAYER_COLOR_"+(AddPass_flag ? _target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_ADD : _target.RTP_SNW_CHOOSEN_LAYER_COLOR_NUM_FIRST);
					} else {
						_code+="//#define RTP_SNW_CHOOSEN_LAYER_COLOR_0";
					}
					_code+=_code_end;
				} else {
					// snow layer for objects (geom blend - actual shader)
					idx=_code.IndexOf("//#define RTP_SNW_CHOOSEN_LAYER_COLOR",sidx);
					if (idx<0) idx=_code.IndexOf("#define RTP_SNW_CHOOSEN_LAYER_COLOR",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if ( (_target.RTP_SNOW_FIRST && _target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST) || (_target.RTP_SNOW_ADD && _target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD) ) {
							_code+="#define RTP_SNW_CHOOSEN_LAYER_COLOR";
						} else {
							_code+="//#define RTP_SNW_CHOOSEN_LAYER_COLOR";
						}
						_code+=_code_end;
					}
				}
			} while(flag);

			// snow layer normal
			sidx=0;
			do {				
				flag=false;			
				idx=_code.IndexOf("//#define RTP_SNW_CHOOSEN_LAYER_NORM_",sidx);
				if (idx<0) idx=_code.IndexOf("#define RTP_SNW_CHOOSEN_LAYER_NORM_",sidx);
				if (idx>0) {
					flag=true; sidx=idx+5; // search next
					string _code_beg=_code.Substring(0,idx);
					string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
					_code=_code_beg;
					if ((AddPass_flag ? _target.RTP_SNOW_ADD : _target.RTP_SNOW_FIRST) && (AddPass_flag ? _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD : _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST)) {
						_code+="#define RTP_SNW_CHOOSEN_LAYER_NORM_"+(AddPass_flag ? _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_ADD : _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_NUM_FIRST);
					} else {
						_code+="//#define RTP_SNW_CHOOSEN_LAYER_NORM_0";
					}
					_code+=_code_end;
				} else {
					idx=_code.IndexOf("//#define RTP_SNW_CHOOSEN_LAYER_NORM",sidx);
					if (idx<0) idx=_code.IndexOf("#define RTP_SNW_CHOOSEN_LAYER_NORM",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						// snow layer for objects (geom blend)					
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if ( (_target.RTP_SNOW_FIRST && _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST) || (_target.RTP_SNOW_ADD && _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD) ) {
							_code+="#define RTP_SNW_CHOOSEN_LAYER_NORM";
						} else {
							_code+="//#define RTP_SNW_CHOOSEN_LAYER_NORM";
						}
						_code+=_code_end;
					}
				}
			} while(flag);

			// superdetail
			ChangeShaderDef(ref _code, "RTP_SUPER_DETAIL", AddPass_flag ? _target.RTP_SUPER_DETAIL_ADD : _target.RTP_SUPER_DETAIL_FIRST);

			// superdetail
			ChangeShaderDef(ref _code, "RTP_SUPER_DTL_MULTS", AddPass_flag ? _target.RTP_SUPER_DETAIL_MULTS_ADD : _target.RTP_SUPER_DETAIL_MULTS_FIRST);
			
			// wetness
			ChangeShaderDef(ref _code, "RTP_WETNESS", AddPass_flag ? _target.RTP_WETNESS_ADD : _target.RTP_WETNESS_FIRST);
			
			// caustics
			ChangeShaderDef(ref _code, "RTP_CAUSTICS", AddPass_flag ? _target.RTP_CAUSTICS_ADD : _target.RTP_CAUSTICS_FIRST);
			
			// simple water
			ChangeShaderDef(ref _code, "SIMPLE_WATER", AddPass_flag ? _target.SIMPLE_WATER_ADD : _target.SIMPLE_WATER_FIRST);
			
			// wetness - animated droplets
			ChangeShaderDef(ref _code, "RTP_WET_RIPPLE_TEXTURE", AddPass_flag ? _target.RTP_WET_RIPPLE_TEXTURE_ADD : _target.RTP_WET_RIPPLE_TEXTURE_FIRST);
			
			// reflection map
			ChangeShaderDef(ref _code, "RTP_REFLECTION", AddPass_flag ? _target.RTP_REFLECTION_ADD : _target.RTP_REFLECTION_FIRST);
			
			// refletion map rotation
			ChangeShaderDef(ref _code, "RTP_ROTATE_REFLECTION", _target.RTP_ROTATE_REFLECTION);

			// uv blend
			ChangeShaderDef(ref _code, "RTP_UV_BLEND", AddPass_flag ? _target.RTP_UV_BLEND_ADD : _target.RTP_UV_BLEND_FIRST);
			
			// uv blend at distance only
			ChangeShaderDef(ref _code, "RTP_DISTANCE_ONLY_UV_BLEND", AddPass_flag ? _target.RTP_DISTANCE_ONLY_UV_BLEND_ADD : _target.RTP_DISTANCE_ONLY_UV_BLEND_FIRST);
			
			// extrude reduction
			ChangeShaderDef(ref _code, "USE_EXTRUDE_REDUCTION", AddPass_flag ? _target.RTP_USE_EXTRUDE_REDUCTION_ADD : _target.RTP_USE_EXTRUDE_REDUCTION_FIRST	);
			
			// global colormap mode
			ChangeShaderDef(ref _code, "COLOR_MAP_BLEND_MULTIPLY", AddPass_flag ? _target.RTP_COLOR_MAP_BLEND_MULTIPLY_ADD : _target.RTP_COLOR_MAP_BLEND_MULTIPLY_FIRST	);
			
			// simple far (based on global colormap only)
			ChangeShaderDef(ref _code, "SIMPLE_FAR", AddPass_flag ? _target.RTP_SIMPLE_FAR_ADD : _target.RTP_SIMPLE_FAR_FIRST	);
			
			// debug overlapped
			ChangeShaderDef(ref _code, "RTP_SHOW_OVERLAPPED", _target.RTP_SHOW_OVERLAPPED);	
			
			if (AddPass_flag) {
				// add-pass triplanar (work only in _4LAYER mode thats default for addpass)
				ChangeShaderDef(ref _code, "RTP_TRIPLANAR", _target.RTP_TRIPLANAR_ADD);
			} else {
				if (_target.RTP_4LAYERS_MODE) {			
					// first-pass triplanar (work only in _4LAYER mode)
					ChangeShaderDef(ref _code, "RTP_TRIPLANAR", _target.RTP_TRIPLANAR_FIRST);	
				}
			}
			
			// vertical texture map
			ChangeShaderDef(ref _code, "RTP_VERTICAL_TEXTURE", AddPass_flag ? _target.RTP_VERTICAL_TEXTURE_ADD : _target.RTP_VERTICAL_TEXTURE_FIRST);	
			
			// global normal map
			ChangeShaderDef(ref _code, "RTP_NORMALGLOBAL", AddPass_flag ? _target.RTP_NORMALGLOBAL_ADD : _target.RTP_NORMALGLOBAL_FIRST);	
			
			// super-simple mode
			ChangeShaderDef(ref _code, "SUPER_SIMPLE", _target.RTP_SUPER_SIMPLE);	
			// super-simple mode options
			//ChangeShaderDef(ref _code, "SS_GRAYSCALE_DETAIL_COLORS", AddPass_flag ? _target.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD : _target.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST );
			ChangeShaderDef(ref _code, "SS_GRAYSCALE_DETAIL_COLORS", _target.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST );
			ChangeShaderDef(ref _code, "USE_BUMPMAPS", AddPass_flag ? _target.RTP_USE_BUMPMAPS_ADD : _target.RTP_USE_BUMPMAPS_FIRST );
			ChangeShaderDef(ref _code, "USE_PERLIN", AddPass_flag ? _target.RTP_USE_PERLIN_ADD : _target.RTP_USE_PERLIN_FIRST );	

			// global trees map
			ChangeShaderDef(ref _code, "RTP_TREESGLOBAL", AddPass_flag ? _target.RTP_TREESGLOBAL_ADD : _target.RTP_TREESGLOBAL_FIRST);	
			
			// additional features in fallbacks
			ChangeShaderDef(ref _code, "ADDITIONAL_FEATURES_IN_FALLBACKS", _target.RTP_ADDITIONAL_FEATURES_IN_FALLBACKS);
			
			// hard crosspass
			ChangeShaderDef(ref _code, "RTP_HARD_CROSSPASS", _target.RTP_HARD_CROSSPASS);
			
			// crosspass heightblend
			ChangeShaderDef(ref _code, "RTP_CROSSPASS_HEIGHTBLEND", _target.RTP_CROSSPASS_HEIGHTBLEND);
		
			// 12 layers indication
			ChangeShaderDef(ref _code, "_12LAYERS", (!_target.RTP_4LAYERS_MODE && AddPass_flag));
	
	// texture redefinition
	{
			ReliefTerrain[] rts=(ReliefTerrain[])GameObject.FindObjectsOfType(typeof(ReliefTerrain));
			ReliefTerrain rt=null;
			for(int i=0; i<rts.Length; i++) {
				if (rts[i].GetComponent(typeof(Terrain))) {
					rt=rts[i];
					break;
				}
			}
	#if !UNITY_3_5
			if (rt && (rt.globalSettingsHolder!=null) && rt.globalSettingsHolder.numTiles>1 && !rt.globalSettingsHolder.useTerrainMaterial) {
	#else
			if (rt && (rt.globalSettingsHolder!=null) && rt.globalSettingsHolder.numTiles>1) {
	#endif		
				// textures put at splat maps, multi terrain w/o materials available
				ChangeShaderDef(ref _code, "TEX_SPLAT_REDEFINITION", true);
			} else {
				ChangeShaderDef(ref _code, "TEX_SPLAT_REDEFINITION", false);
			}
	} // texture redefinition
			
			// 4 LAYERS treatment - splat count
			// 4 warstwy - splat count = 4+4
			// 8 warstw w 4 layers mode - splat count = 4+4
			// 8 warstw w 8 layers mode - splat count = 8+4
			// 12 warstw w 8 layers mode - splat count = 4+8
			string splat_count_tag;
			if (addPassPresent) {
				if (AddPass_flag) {
					if (!_target.RTP_4LAYERS_MODE) {
						splat_count_tag="\"SplatCount\" = \"8\"";
					} else {
						splat_count_tag="\"SplatCount\" = \"4\"";
					}
				} else {
					splat_count_tag="\"SplatCount\" = \"4\"";
				}
			} else {
				if (AddPass_flag) {
					splat_count_tag="\"SplatCount\" = \"4\"";
				} else {
					if (!_target.RTP_4LAYERS_MODE) {
						splat_count_tag="\"SplatCount\" = \"8\"";
					} else {
						splat_count_tag="\"SplatCount\" = \"4\"";
					}
				}
			}
			sidx=0;
			do {				
				flag=false;
				idx=_code.IndexOf("\"SplatCount\" = \"4\"",sidx);
				if (idx<0) idx=_code.IndexOf("\"SplatCount\" = \"8\"",sidx);
				if (idx>0) {
					flag=true; sidx=idx+5; // search next
					string _code_beg=_code.Substring(0,idx);
					string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
					_code=_code_beg;
					_code+=splat_count_tag;
					_code+=_code_end;
				}
			} while(flag);
			
			// noforwardadd treatment in all shaders
			if (rebuild_noforwardadd) {
				sidx=0;
				do {					
					flag=false;
					idx=_code.IndexOf("#pragma surface",sidx);
					if (idx>0) {
						sidx=idx+5; flag=true;
						string _code_beg=_code.Substring(0,idx);
						string _code_mid=_code.Substring(idx, _code.IndexOf((char)10, idx+1) - idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						if (_target.RTP_NOFORWARDADD) {
							if (_code_mid.IndexOf(" noforwardadd")<0) {
								_code_mid=_code_mid+" noforwardadd";
							}
						} else {
							if (_code_mid.IndexOf(" noforwardadd")>=0) {
								_code_mid=_code_mid.Replace(" noforwardadd", "");
							}
						}
						_code=_code_beg+_code_mid+_code_end;
					}
				} while(flag);				
			}
			
			// nolightmap treatment in all shaders
			if (rebuild_noforwardadd) {
				sidx=0;
				do {					
					flag=false;
					idx=_code.IndexOf("#pragma surface",sidx);
					if (idx>0) {
						sidx=idx+5; flag=true;
						string _code_beg=_code.Substring(0,idx);
						string _code_mid=_code.Substring(idx, _code.IndexOf((char)10, idx+1) - idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						if (_target.RTP_NOLIGHTMAP) {
							if (_code_mid.IndexOf(" nolightmap")<0) {
								_code_mid=_code_mid+" nolightmap";
							}
						} else {
							if (_code_mid.IndexOf(" nolightmap")>=0) {
								_code_mid=_code_mid.Replace(" nolightmap", "");
							}
						}
						_code=_code_beg+_code_mid+_code_end;
					}
				} while(flag);				
			}
			
			// MaxLOD treatment
			if (rebuild_noforwardadd) { // warunek, aby nie przebudowywac dodatkowych shaderow, tylko te uzywane prez teren i geom)
			sidx=0;
			do {
				flag=false;
				idx=_code.IndexOf("#pragma multi_compile",sidx);
				if (idx>0 && _code.Substring(idx-1, 1)=="/") {
					flag=true;
					sidx=idx+5;
					idx=-1;
				}
				if (idx>0) {
					flag=true; sidx=idx+5; // search next
					string _code_beg=_code.Substring(0,idx);
					string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
					_code=_code_beg;
					if (!AddPass_flag || geom_flag) {
						string add_code="RTP_SIMPLE_SHADING";
						if (!_target.RTP_SUPER_SIMPLE) {
							if ((int)_target.MAX_LOD_FIRST<=(int)RTPLodLevel.PM) add_code="RTP_PM_SHADING "+add_code;
							if ((int)_target.MAX_LOD_FIRST<=(int)RTPLodLevel.POM_NoShadows) add_code="RTP_POM_SHADING_LO "+add_code;
							if ((int)_target.MAX_LOD_FIRST<=(int)RTPLodLevel.POM_HardShadows) add_code="RTP_POM_SHADING_MED "+add_code;
							if ((int)_target.MAX_LOD_FIRST<=(int)RTPLodLevel.POM_SoftShadows) add_code="RTP_POM_SHADING_HI "+add_code;
						}
						add_code="#pragma multi_compile "+add_code;
						_code+=add_code;
					} else if (AddPass_flag) {
						string add_code="RTP_SIMPLE_SHADING";
						if ((int)_target.MAX_LOD_ADD<=(int)RTPLodLevel.PM) add_code="RTP_PM_SHADING "+add_code;
						if ((int)_target.MAX_LOD_ADD<=(int)RTPLodLevel.POM_NoShadows) add_code="RTP_POM_SHADING_LO "+add_code;
						if ((int)_target.MAX_LOD_ADD<=(int)RTPLodLevel.POM_HardShadows) add_code="RTP_POM_SHADING_MED "+add_code;
						if ((int)_target.MAX_LOD_ADD<=(int)RTPLodLevel.POM_SoftShadows) add_code="RTP_POM_SHADING_HI "+add_code;
						add_code="#pragma multi_compile "+add_code;
						_code+=add_code;
					}
					_code+=_code_end;
				}
			} while(flag);
			}
			
			// MaxLOD for layers 4-7 treatment in firstpass 8 layers mode
			sidx=0;
			do {				
				flag=false;
				idx=_code.IndexOf("//#define RTP_47SHADING_",sidx);
				if (idx<0) idx=_code.IndexOf("#define RTP_47SHADING_",sidx);
				if (idx>0) {
					flag=true; sidx=idx+5; // search next
					string _code_beg=_code.Substring(0,idx);
					string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
					_code=_code_beg;
					if (!AddPass_flag || geom_flag) {
						if (_target.MAX_LOD_FIRST_PLUS4==RTPLodLevel.SIMPLE) _code+="#define RTP_47SHADING_SIMPLE";
						if (_target.MAX_LOD_FIRST_PLUS4==RTPLodLevel.PM) _code+="#define RTP_47SHADING_PM";
						if (_target.MAX_LOD_FIRST_PLUS4==RTPLodLevel.POM_NoShadows) _code+="#define RTP_47SHADING_POM_LO";
						if (_target.MAX_LOD_FIRST_PLUS4==RTPLodLevel.POM_HardShadows) _code+="#define RTP_47SHADING_POM_MED";
						if (_target.MAX_LOD_FIRST_PLUS4==RTPLodLevel.POM_SoftShadows) _code+="#define RTP_47SHADING_POM_HI";
					}
					_code+=_code_end;
				}
			} while(flag);
			
			// UV blend routing
			for(int k=0; k<8; k++) {
				RouteUVBlend(ref _code, k, AddPass_flag ? _target.UV_BLEND_ROUTE_NUM_ADD[k] : _target.UV_BLEND_ROUTE_NUM_FIRST[k]);
			}
			
			// 4 LAYERS treatment - _4LAYERS flag
			if (!AddPass_flag) {
				sidx=0;
				do {				
					flag=false;
					idx=_code.IndexOf("//#define _4LAYERS",sidx);
					if (idx<0) idx=_code.IndexOf("#define _4LAYERS",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if (!geom_flag) {
							if (_target.RTP_4LAYERS_MODE) {
								_code+="#define _4LAYERS";
							} else {
								_code+="//#define _4LAYERS";
							}
						} else {
							if (_target.numLayers<=4) {
								_code+="#define _4LAYERS";
							} else {
								_code+="//#define _4LAYERS";
							}
						}
						_code+=_code_end;
					}
				} while(flag);
			}

			if (blend_base) {
				// 4 LAYERS treatment - AddBlend comment BEGIN
				sidx=0;
				do {				
					flag=false;
					idx=_code.IndexOf("///* AddBlend",sidx);
					if (idx<0) idx=_code.IndexOf("/* AddBlend",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if (!_target.ADDPASS_IN_BLENDBASE) {
							_code+="/* AddBlend";
						} else {
							_code+="///* AddBlend";
						}
						_code+=_code_end;
					}
				} while(flag);
				
				// 4 LAYERS treatment - AddBlend comment END
				sidx=0;
				do {				
					flag=false;
					idx=_code.IndexOf("//*/ // AddBlend",sidx);
					if (idx<0) idx=_code.IndexOf("*/ // AddBlend",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if (!_target.ADDPASS_IN_BLENDBASE) {
							_code+="*/ // AddBlend";
						} else {
							_code+="//*/ // AddBlend";
						}
						_code+=_code_end;
					}
				} while(flag);
			}
			
			// 4 LAYERS treatment - AddPass in classic mode comment BEGIN
			sidx=0;
			do {				
				flag=false;
				idx=_code.IndexOf("///* AddPass",sidx);
				if (idx<0) idx=_code.IndexOf("/* AddPass",sidx);
				if (idx>0) {
					flag=true; sidx=idx+5; // search next
					string _code_beg=_code.Substring(0,idx);
					string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
					_code=_code_beg;
					if (!geom_flag) {
						bool cond=_target.RTP_4LAYERS_MODE;
						if (blend_base && addPassPresent) cond=false;
						if (cond) {
							_code+="/* AddPass";
						} else {
							_code+="///* AddPass";
						}
					} else {
						if (!_target.RTP_4LAYERS_MODE) {
							_code+="/* AddPass";
						} else {
							_code+="///* AddPass";
						}
					}
					_code+=_code_end;
				}
			} while(flag);
			
			// 4 LAYERS treatment - AddPass in classic mode comment END
			sidx=0;
			do {				
				flag=false;
				idx=_code.IndexOf("//*/ // AddPass",sidx);
				if (idx<0) idx=_code.IndexOf("*/ // AddPass",sidx);
				if (idx>0) {
					flag=true; sidx=idx+5; // search next
					string _code_beg=_code.Substring(0,idx);
					string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
					_code=_code_beg;
					if (!geom_flag) {				
						bool cond=_target.RTP_4LAYERS_MODE;
						if (blend_base && addPassPresent) cond=false;
						if (cond) {
							_code+="*/ // AddPass";
						} else {
							_code+="//*/ // AddPass";
						}
					} else {
						if (!_target.RTP_4LAYERS_MODE) {
							_code+="*/ // AddPass";
						} else {
							_code+="//*/ // AddPass";
						}
					}
					_code+=_code_end;
				}
			} while(flag);		
			
			// FarOnly - AddPass treatment
			if (shader_path=="Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FarOnly.shader") {
				
				sidx=0;
				do {				
					flag=false;
					idx=_code.IndexOf("///* AddFar",sidx);
					if (idx<0) idx=_code.IndexOf("/* AddFar",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if (!addPassPresent) {
							_code+="/* AddFar";
						} else {
							_code+="///* AddFar";
						}
						_code+=_code_end;
					}
				} while(flag);					
				
				sidx=0;
				do {
					flag=false;
					idx=_code.IndexOf("//*/ // AddFar",sidx);
					if (idx<0) idx=_code.IndexOf("*/ // AddFar",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if (!addPassPresent) {
							_code+="*/ // AddFar";
						} else {
							_code+="//*/ // AddFar";
						}
						_code+=_code_end;
					}
				} while(flag);
						
				sidx=0;
				do {				
					flag=false;
					idx=_code.IndexOf("///* AddPass",sidx);
					if (idx<0) idx=_code.IndexOf("/* AddPass",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if (addPassPresent && _target.RTP_4LAYERS_MODE) {
							_code+="///* AddPass";
						} else {
							_code+="/* AddPass";
						}
						_code+=_code_end;
					}
				} while(flag);					
				
				sidx=0;
				do {
					flag=false;
					idx=_code.IndexOf("//*/ // AddPass",sidx);
					if (idx<0) idx=_code.IndexOf("*/ // AddPass",sidx);
					if (idx>0) {
						flag=true; sidx=idx+5; // search next
						string _code_beg=_code.Substring(0,idx);
						string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
						_code=_code_beg;
						if (addPassPresent && _target.RTP_4LAYERS_MODE) {
							_code+="//*/ // AddPass";
						} else {
							_code+="*/ // AddPass";
						}
						_code+=_code_end;
					}
				} while(flag);
						
			} // EOF FarOnly - AddPass treatment
			
			if (_code_orig!=_code || force_rebuild) {
				//Debug.Log (shader_path);
				System.IO.File.WriteAllText(shader_path, _code);
				changed=true;
			}

		} else {
			//Debug.LogWarning("Can't find "+shader_path+" file");
		}		
		return changed;
	}
	
	private void ChangeShaderDef(ref string _code, string define_name, bool feature) {
		int sidx=0;
		int idx;
		bool flag;
		do {				
			flag=false;
			idx=_code.IndexOf("//#define "+define_name,sidx);
			if (idx>0 && _code.Substring(idx-1, 1)==" ") {
				flag=true;				
				sidx=idx+5;
				idx=-1;
			}
			if (idx<0) idx=_code.IndexOf("#define "+define_name,sidx);
			if (idx>0 && _code.Substring(idx-1, 1)==" ") {
				flag=true;				
				sidx=idx+5;
				idx=-1;
			}				
			if (idx>0) {
				flag=true; sidx=idx+5; // search next
				string _code_beg=_code.Substring(0,idx);
				string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
				_code=_code_beg;
				if (feature) {
					_code+="#define "+define_name;
				} else {
					_code+="//#define "+define_name;
				}
				_code+=_code_end;
			}
		} while(flag);			
	}
	
	private void RouteUVBlend(ref string _code, int num_src, int num_tgt) {
		int sidx=0;
		bool flag;
		do {				
			flag=false;			
			int idx=_code.IndexOf("#define UV_BLEND_ROUTE_LAYER_"+num_src,sidx);
			if (idx>0 && _code.Substring(idx-1, 1)==" ") {
				flag=true;				
				sidx=idx+5;
				idx=-1;
			}
			if (idx>0) {
				flag=true; sidx=idx+5; // search next
				string _code_beg=_code.Substring(0,idx);
				string _code_end=_code.Substring(_code.IndexOf((char)10, idx+1));
				_code=_code_beg;
				_code+="#define UV_BLEND_ROUTE_LAYER_"+num_src+" UV_BLEND_SRC_"+num_tgt;
				_code+=_code_end;
			}
		} while(flag);
	}

	private void SyncFeatures() {
		RTP_LODmanager _target=(RTP_LODmanager)target;
		CheckAddPassPresent();
		SyncFeaturesFromFile("Assets/ReliefPack/Shaders/ReliefTerrain/RTP_Base.cginc", false, false);
		SyncFeaturesFromFile("Assets/ReliefPack/Shaders/ReliefTerrain/RTP_AddBase.cginc", false, true);
		SyncFeaturesFromFile("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FirstPass.shader", true, false);
		SyncFeaturesFromFile("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-AddPass.shader", true, true);
		
		// shader usage
		SyncUsage("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FirstPass.shader", ref _target.SHADER_USAGE_FirstPass);
		SyncUsage("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-AddPass.shader", ref _target.SHADER_USAGE_AddPass);
		SyncUsage("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain-FarOnly.shader", ref _target.SHADER_USAGE_TerrainFarOnly);
		SyncUsage("Assets/ReliefPack/Shaders/ReliefTerrain/Internal/ReliefTerrainGeometryBlendBase.shader", ref _target.SHADER_USAGE_BlendBase);
		
		SyncUsage("Assets/ReliefPack/Shaders/ReliefTerrain/ReliefTerrain2Geometry.shader", ref _target.SHADER_USAGE_Terrain2Geometry);
		SyncUsage("Assets/ReliefPack/Shaders/ReliefTerrain/Internal/ReliefTerrain2GeometryBlendBase.shader", ref _target.SHADER_USAGE_Terrain2GeometryBlendBase);
	}
	
	private void SyncUsage(string shader_path, ref bool usage_flag) {
		if (System.IO.File.Exists(shader_path)) {
			
			string _code = System.IO.File.ReadAllText(shader_path);
			if (_code.IndexOf("///* INIT")>0) {
				usage_flag=true;
			} else if (_code.IndexOf("/* INIT")>0) {
				usage_flag=false;
			}
		}
	}
	
	private void SyncFeaturesFromFile(string shader_path, bool shader_flag, bool addpass_flag) {
		RTP_LODmanager _target=(RTP_LODmanager)target;
		
		if (System.IO.File.Exists(shader_path)) {
			
			string _code = System.IO.File.ReadAllText(shader_path);
			
			_target.PLATFORM_D3D9=(_code.IndexOf(" d3d9")>0);
			_target.PLATFORM_OPENGL=(_code.IndexOf(" opengl")>0);
			_target.PLATFORM_GLES=(_code.IndexOf(" gles")>0);
			_target.PLATFORM_FLASH=(_code.IndexOf(" flash")>0);
			_target.PLATFORM_D3D11=(_code.IndexOf(" d3d11")>0);
			if (addpass_flag) {			
				if (CheckDefine(_code, "//#define RTP_SNOW")) _target.RTP_SNOW_ADD=false;
				else if (CheckDefine(_code, "#define RTP_SNOW")) _target.RTP_SNOW_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_TRIPLANAR")) _target.RTP_TRIPLANAR_ADD=false;
				else if (CheckDefine(_code, "#define RTP_TRIPLANAR")) _target.RTP_TRIPLANAR_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_DISTANCE_ONLY_UV_BLEND")) _target.RTP_DISTANCE_ONLY_UV_BLEND_ADD=false;
				else if (CheckDefine(_code, "#define RTP_DISTANCE_ONLY_UV_BLEND")) _target.RTP_DISTANCE_ONLY_UV_BLEND_ADD=true;
					
				if (CheckDefine(_code, "//#define RTP_SNW_CHOOSEN_LAYER_COLOR_")) _target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD=false;
				else if (CheckDefine(_code, "#define RTP_SNW_CHOOSEN_LAYER_COLOR_")) _target.RTP_SNW_CHOOSEN_LAYER_COLOR_ADD=true;
					
				if (CheckDefine(_code, "//#define RTP_SNW_CHOOSEN_LAYER_NORM_")) _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD=false;
				else if (CheckDefine(_code, "#define RTP_SNW_CHOOSEN_LAYER_NORM_")) _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_SUPER_DETAIL")) _target.RTP_SUPER_DETAIL_ADD=false;
				else if (CheckDefine(_code, "#define RTP_SUPER_DETAIL")) _target.RTP_SUPER_DETAIL_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_SUPER_DTL_MULTS")) _target.RTP_SUPER_DETAIL_MULTS_ADD=false;
				else if (CheckDefine(_code, "#define RTP_SUPER_DTL_MULTS")) _target.RTP_SUPER_DETAIL_MULTS_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_UV_BLEND")) _target.RTP_UV_BLEND_ADD=false;
				else if (CheckDefine(_code, "#define RTP_UV_BLEND")) _target.RTP_UV_BLEND_ADD=true;
				
				if (CheckDefine(_code, "//#define USE_EXTRUDE_REDUCTION")) _target.RTP_USE_EXTRUDE_REDUCTION_ADD=false;
				else if (CheckDefine(_code, "#define USE_EXTRUDE_REDUCTION")) _target.RTP_USE_EXTRUDE_REDUCTION_ADD=true;

				if (CheckDefine(_code, "//#define RTP_VERTICAL_TEXTURE")) _target.RTP_VERTICAL_TEXTURE_ADD=false;
				else if (CheckDefine(_code, "#define RTP_VERTICAL_TEXTURE")) _target.RTP_VERTICAL_TEXTURE_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_NORMALGLOBAL")) _target.RTP_NORMALGLOBAL_ADD=false;
				else if (CheckDefine(_code, "#define RTP_NORMALGLOBAL")) _target.RTP_NORMALGLOBAL_ADD=true;
			
				if (CheckDefine(_code, "//#define RTP_TREESGLOBAL")) _target.RTP_TREESGLOBAL_ADD=false;
				else if (CheckDefine(_code, "#define RTP_TREESGLOBAL")) _target.RTP_TREESGLOBAL_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_WETNESS")) _target.RTP_WETNESS_ADD=false;
				else if (CheckDefine(_code, "#define RTP_WETNESS")) _target.RTP_WETNESS_ADD=true;
				if (CheckDefine(_code, "//#define SIMPLE_WATER")) _target.SIMPLE_WATER_ADD=false;
				else if (CheckDefine(_code, "#define SIMPLE_WATER")) _target.SIMPLE_WATER_ADD=true;
				if (CheckDefine(_code, "//#define RTP_WET_RIPPLE_TEXTURE")) _target.RTP_WET_RIPPLE_TEXTURE_ADD=false;
				else if (CheckDefine(_code, "#define RTP_WET_RIPPLE_TEXTURE")) _target.RTP_WET_RIPPLE_TEXTURE_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_CAUSTICS")) _target.RTP_CAUSTICS_ADD=false;
				else if (CheckDefine(_code, "#define RTP_CAUSTICS")) _target.RTP_CAUSTICS_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_REFLECTION")) _target.RTP_REFLECTION_ADD=false;
				else if (CheckDefine(_code, "#define RTP_REFLECTION")) _target.RTP_REFLECTION_ADD=true;
				
				if (CheckDefine(_code, "//#define RTP_MAPPED_SHADOWS")) _target.RTP_MAPPED_SHADOWS_ADD=false;
				else if (CheckDefine(_code, "#define RTP_MAPPED_SHADOWS")) _target.RTP_MAPPED_SHADOWS_ADD=true;
				
				if (CheckDefine(_code, "//#define COLOR_MAP_BLEND_MULTIPLY")) _target.RTP_COLOR_MAP_BLEND_MULTIPLY_ADD=false;
				else if (CheckDefine(_code, "#define COLOR_MAP_BLEND_MULTIPLY")) _target.RTP_COLOR_MAP_BLEND_MULTIPLY_ADD=true;
				
				if (CheckDefine(_code, "//#define SIMPLE_FAR")) _target.RTP_SIMPLE_FAR_ADD=false;
				else if (CheckDefine(_code, "#define SIMPLE_FAR")) _target.RTP_SIMPLE_FAR_ADD=true;
				
				// super-simple specific
				//if (CheckDefine(_code, "//#define SS_GRAYSCALE_DETAIL_COLORS")) _target.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD=false;
				//else if (CheckDefine(_code, "#define SS_GRAYSCALE_DETAIL_COLORS")) _target.RTP_SS_GRAYSCALE_DETAIL_COLORS_ADD=true;
				
				// super-simple specific
				if (CheckDefine(_code, "//#define USE_BUMPMAPS")) _target.RTP_USE_BUMPMAPS_ADD=false;
				else if (CheckDefine(_code, "#define USE_BUMPMAPS")) _target.RTP_USE_BUMPMAPS_ADD=true;
				
				// super-simple specific
				if (CheckDefine(_code, "//#define USE_PERLIN")) _target.RTP_USE_PERLIN_ADD=false;
				else if (CheckDefine(_code, "#define USE_PERLIN")) _target.RTP_USE_PERLIN_ADD=true;
				
				for(int k=0; k<4; k++) {
					for(int j=0; j<4; j++) {
						if (CheckDefine(_code, "#define UV_BLEND_ROUTE_LAYER_"+k+" UV_BLEND_SRC_"+j)) {
							_target.UV_BLEND_ROUTE_NUM_ADD[k]=j;
							break;
						}
					}
				}
				
			} else {
				
				// used only in first pass
				if (CheckDefine(_code, "//#define _4LAYERS")) _target.RTP_4LAYERS_MODE=false;
				else if (CheckDefine(_code, "#define _4LAYERS")) _target.RTP_4LAYERS_MODE=true;
				
				if (CheckDefine(_code, "//#define RTP_TRIPLANAR")) _target.RTP_TRIPLANAR_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_TRIPLANAR")) _target.RTP_TRIPLANAR_FIRST=true;
				
				// used in both passes
				if (CheckDefine(_code, "//#define RTP_SNOW")) _target.RTP_SNOW_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_SNOW")) _target.RTP_SNOW_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_DISTANCE_ONLY_UV_BLEND")) _target.RTP_DISTANCE_ONLY_UV_BLEND_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_DISTANCE_ONLY_UV_BLEND")) _target.RTP_DISTANCE_ONLY_UV_BLEND_FIRST=true;
					
				if (CheckDefine(_code, "//#define RTP_SNW_CHOOSEN_LAYER_COLOR_")) _target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_SNW_CHOOSEN_LAYER_COLOR_")) _target.RTP_SNW_CHOOSEN_LAYER_COLOR_FIRST=true;
					
				if (CheckDefine(_code, "//#define RTP_SNW_CHOOSEN_LAYER_NORM_")) _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_SNW_CHOOSEN_LAYER_NORM_")) _target.RTP_SNW_CHOOSEN_LAYER_NORMAL_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_SUPER_DETAIL")) _target.RTP_SUPER_DETAIL_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_SUPER_DETAIL")) _target.RTP_SUPER_DETAIL_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_SUPER_DTL_MULTS")) _target.RTP_SUPER_DETAIL_MULTS_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_SUPER_DTL_MULTS")) _target.RTP_SUPER_DETAIL_MULTS_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_UV_BLEND")) _target.RTP_UV_BLEND_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_UV_BLEND")) _target.RTP_UV_BLEND_FIRST=true;
				
				if (CheckDefine(_code, "//#define USE_EXTRUDE_REDUCTION")) _target.RTP_USE_EXTRUDE_REDUCTION_FIRST=false;
				else if (CheckDefine(_code, "#define USE_EXTRUDE_REDUCTION")) _target.RTP_USE_EXTRUDE_REDUCTION_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_VERTICAL_TEXTURE")) _target.RTP_VERTICAL_TEXTURE_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_VERTICAL_TEXTURE")) _target.RTP_VERTICAL_TEXTURE_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_NORMALGLOBAL")) _target.RTP_NORMALGLOBAL_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_NORMALGLOBAL")) _target.RTP_NORMALGLOBAL_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_TREESGLOBAL")) _target.RTP_TREESGLOBAL_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_TREESGLOBAL")) _target.RTP_TREESGLOBAL_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_WETNESS")) _target.RTP_WETNESS_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_WETNESS")) _target.RTP_WETNESS_FIRST=true;
				if (CheckDefine(_code, "//#define SIMPLE_WATER")) _target.SIMPLE_WATER_FIRST=false;
				else if (CheckDefine(_code, "#define SIMPLE_WATER")) _target.SIMPLE_WATER_FIRST=true;
				if (CheckDefine(_code, "//#define RTP_WET_RIPPLE_TEXTURE")) _target.RTP_WET_RIPPLE_TEXTURE_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_WET_RIPPLE_TEXTURE")) _target.RTP_WET_RIPPLE_TEXTURE_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_CAUSTICS")) _target.RTP_CAUSTICS_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_CAUSTICS")) _target.RTP_CAUSTICS_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_REFLECTION")) _target.RTP_REFLECTION_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_REFLECTION")) _target.RTP_REFLECTION_FIRST=true;
				
				if (CheckDefine(_code, "//#define RTP_MAPPED_SHADOWS")) _target.RTP_MAPPED_SHADOWS_FIRST=false;
				else if (CheckDefine(_code, "#define RTP_MAPPED_SHADOWS")) _target.RTP_MAPPED_SHADOWS_FIRST=true;
				
				if (CheckDefine(_code, "//#define COLOR_MAP_BLEND_MULTIPLY")) _target.RTP_COLOR_MAP_BLEND_MULTIPLY_FIRST=false;
				else if (CheckDefine(_code, "#define COLOR_MAP_BLEND_MULTIPLY")) _target.RTP_COLOR_MAP_BLEND_MULTIPLY_FIRST=true;
				
				if (CheckDefine(_code, "//#define SIMPLE_FAR")) _target.RTP_SIMPLE_FAR_FIRST=false;
				else if (CheckDefine(_code, "#define SIMPLE_FAR")) _target.RTP_SIMPLE_FAR_FIRST=true;
				
				// super-simple specific
				if (CheckDefine(_code, "//#define SS_GRAYSCALE_DETAIL_COLORS")) _target.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST=false;
				else if (CheckDefine(_code, "#define SS_GRAYSCALE_DETAIL_COLORS")) _target.RTP_SS_GRAYSCALE_DETAIL_COLORS_FIRST=true;
				
				// super-simple specific
				if (CheckDefine(_code, "//#define USE_BUMPMAPS")) _target.RTP_USE_BUMPMAPS_FIRST=false;
				else if (CheckDefine(_code, "#define USE_BUMPMAPS")) _target.RTP_USE_BUMPMAPS_FIRST=true;
				
				// super-simple specific
				if (CheckDefine(_code, "//#define USE_PERLIN")) _target.RTP_USE_PERLIN_FIRST=false;
				else if (CheckDefine(_code, "#define USE_PERLIN")) _target.RTP_USE_PERLIN_FIRST=true;
				
				for(int k=0; k<8; k++) {
					for(int j=0; j<8; j++) {
						if (CheckDefine(_code, "#define UV_BLEND_ROUTE_LAYER_"+k+" UV_BLEND_SRC_"+j)) {
							_target.UV_BLEND_ROUTE_NUM_FIRST[k]=j;
							break;
						}
					}
				}
				
			}
			
			if (CheckDefine(_code, "//#define SUPER_SIMPLE")) _target.RTP_SUPER_SIMPLE=false;
			else if (CheckDefine(_code, "#define SUPER_SIMPLE")) _target.RTP_SUPER_SIMPLE=true;
			
			if (CheckDefine(_code, "//#define ADDITIONAL_FEATURES_IN_FALLBACKS")) _target.RTP_ADDITIONAL_FEATURES_IN_FALLBACKS=false;
			else if (CheckDefine(_code, "#define ADDITIONAL_FEATURES_IN_FALLBACKS")) _target.RTP_ADDITIONAL_FEATURES_IN_FALLBACKS=true;
			
			if (CheckDefine(_code, "//#define RTP_ROTATE_REFLECTION")) _target.RTP_ROTATE_REFLECTION=false;
			else if (CheckDefine(_code, "#define RTP_ROTATE_REFLECTION")) _target.RTP_ROTATE_REFLECTION=true;
			
			if (CheckDefine(_code, "//#define RTP_SHOW_OVERLAPPED")) _target.RTP_SHOW_OVERLAPPED=false;
			else if (CheckDefine(_code, "#define RTP_SHOW_OVERLAPPED")) _target.RTP_SHOW_OVERLAPPED=true;
			
			if (CheckDefine(_code, "//#define RTP_HARD_CROSSPASS")) _target.RTP_HARD_CROSSPASS=false;
			else if (CheckDefine(_code, "#define RTP_HARD_CROSSPASS")) _target.RTP_HARD_CROSSPASS=true;
			
			if (CheckDefine(_code, "//#define RTP_CROSSPASS_HEIGHTBLEND")) _target.RTP_CROSSPASS_HEIGHTBLEND=false;
			else if (CheckDefine(_code, "#define RTP_CROSSPASS_HEIGHTBLEND")) _target.RTP_CROSSPASS_HEIGHTBLEND=true;
			
		}
	}		
	
	private bool CheckDefine(string _code, string define) {
		int sidx=0;
		bool flag;
		do {
			flag=false;
			int idx=_code.IndexOf(define, sidx);
			if (idx>0) {
				 if(_code.Substring(idx-1, 1)!=" ") {
					return true;
				} else {
					sidx+=5; flag=true;
				}
			}
		} while(flag);
		return false;
	}
	
	private void CheckAddPassPresent() {
		RTP_LODmanager _target=(RTP_LODmanager)target;		
		ReliefTerrain obj=(ReliefTerrain)GameObject.FindObjectOfType(typeof(ReliefTerrain));
		if (obj) {
			int act_layer_num=obj.globalSettingsHolder.numLayers;
			bool addPassPresent=false;
			if (act_layer_num<=4) {
				addPassPresent=false;
			} else if (act_layer_num<=8) {
				if (_target.RTP_4LAYERS_MODE) {
					addPassPresent=true;
				} else {
					addPassPresent=false;
				}
			} else {
				addPassPresent=true;
			}
			_target.ADDPASS_IN_BLENDBASE=addPassPresent;
		}	
	}
#endif
}
