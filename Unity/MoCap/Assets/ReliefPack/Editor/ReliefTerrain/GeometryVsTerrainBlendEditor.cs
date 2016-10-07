using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
#if UNITY_EDITOR
using System.IO;
#endif

[CustomEditor (typeof(GeometryVsTerrainBlend))]
public class GeometryVsTerrainBlendEditor : Editor {
#if UNITY_EDITOR	
	private double UpdTim=0;
	
	private Vector3[] cover_verts=new Vector3[2000];
	private	float[] cover_strength=new float[2000];
	private	int[] cover_indices=new int[2000];
	private	int[] cover_tris=new int[600];
	private	int cover_num=0;
	private int cover_num_start_drag=0;
	private double lCovTim=0;
	private bool control_down_flag=false;
	
	private RaycastHit paintHitInfo;
	private bool paintHitInfo_flag;
	private Tool prev_tool;
	
	// icons
	private Texture paintButTexOn;
	private Texture paintButTexOff;
	
	void OnEnable() {
		if (paintButTexOn==null) paintButTexOn=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoPaintOn.png", typeof(Texture)) as Texture;
		if (paintButTexOff==null) paintButTexOff=AssetDatabase.LoadAssetAtPath("Assets/ReliefPack/Editor/ReliefTerrain/icons/icoPaintOff.png", typeof(Texture)) as Texture;
		
		prev_tool=Tools.current;
		
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		if (_target.orig_mesh==null) {
			MeshFilter mf=_target.GetComponent(typeof(MeshFilter)) as MeshFilter;
			//if (AssetDatabase.GetAssetPath(mf.sharedMesh)!="") {
				_target.orig_mesh = mf.sharedMesh;	
			//}
		}
		if (!_target.blendedObject) {
			MeshFilter mf=_target.GetComponent(typeof(MeshFilter)) as MeshFilter;
		  	RaycastHit[] hits;
		    hits=Physics.RaycastAll(_target.transform.position+Vector3.up*20, Vector3.down, 100);
			int o;
			for(o=0; o<hits.Length; o++) {
				if (hits[o].collider.gameObject.GetComponent(typeof(ReliefTerrain))!=null) {
					_target.blendedObject=hits[o].collider.gameObject;
					CheckShaderForBlendCapability();
					_target.MakeMeshCopy();
					_target.pmesh=mf.sharedMesh;
					EditorUtility.SetDirty(target);
					SceneView.RepaintAll();
					break;
				}
			}
		}		
	}
	
	void OnDisable() {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		if (_target) {
			if (_target.paint_flag) {
				 _target.paint_flag=false;
				SceneView.onSceneGUIDelegate -= GeometryVsTerrainBlend._SceneGUI;
			};
			EditorUtility.SetDirty(target);
		}
		Tools.current=prev_tool;
	}
	
	public override void OnInspectorGUI () {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		MeshFilter mf=_target.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (_target.orig_mesh==null) {
			if (AssetDatabase.GetAssetPath(mf.sharedMesh)!="") {
				_target.orig_mesh = mf.sharedMesh;	
				MeshCollider mc=_target.GetComponent(typeof(MeshCollider)) as MeshCollider;
				if (mc) {
					mc.sharedMesh=null;
					mc.sharedMesh=_target.orig_mesh;
				}
				if (_target.blendedObject) {
					CheckShaderForBlendCapability();
					_target.MakeMeshCopy();
					EditorUtility.SetDirty(target);
					SceneView.RepaintAll();
				}
			}
		} else {
			if (mf.sharedMesh!=_target.pmesh) {
				if (AssetDatabase.GetAssetPath(mf.sharedMesh)!="") {
					_target.orig_mesh = mf.sharedMesh;		
					MeshCollider mc=_target.GetComponent(typeof(MeshCollider)) as MeshCollider;
					if (mc) {
						mc.sharedMesh=null;
						mc.sharedMesh=_target.orig_mesh;
					}
					CheckShaderForBlendCapability();
					_target.MakeMeshCopy();
					EditorUtility.SetDirty(target);
					SceneView.RepaintAll();
				}
				_target.pmesh=mf.sharedMesh;
			}
		}
		if (!_target.blendedObject) {
		  	RaycastHit[] hits;
		    hits=Physics.RaycastAll(_target.transform.position+Vector3.up*20, Vector3.down, 100);
			int o;
			for(o=0; o<hits.Length; o++) {
				if (hits[o].collider.gameObject.GetComponent(typeof(ReliefTerrain))!=null) {
					_target.blendedObject=hits[o].collider.gameObject;
					CheckShaderForBlendCapability();
					_target.MakeMeshCopy();
					_target.pmesh=mf.sharedMesh;
					EditorUtility.SetDirty(target);
					SceneView.RepaintAll();
					break;
				}
			}
		}
		
		EditorGUILayout.HelpBox("This script allows you to prepare info for geometry blending shader (encoded in alpha component of vertex color) and modify alpha channel of global ColorMap (extrude height).",MessageType.Info, true);
		
		GameObject go=(GameObject)EditorGUILayout.ObjectField("Blended Object", _target.blendedObject, typeof(GameObject), true);
		GUILayout.Space(5);		
		if (go!=_target.blendedObject) {
			_target.blendedObject=go;
			if (go) {
				CheckShaderForBlendCapability();
				_target.MakeMeshCopy();
			}
			EditorUtility.SetDirty(target);
		}

		EditorGUILayout.BeginVertical("Box");
		//if (AssetDatabase.GetAssetPath( (_target.gameObject.GetComponent(typeof(MeshRenderer)) as MeshRenderer).sharedMaterial)!="") {
			if (GUILayout.Button(new GUIContent("Make material copy", "You can make independent clone\nof material used on object."))) {
				Undo.RegisterUndo( _target.gameObject.GetComponent (typeof(MeshRenderer)), "Geometry Blend Edit");
				_target.MakeMaterialCopy();
			}
			if (_target.blendedObject) {
				if (_target.shader_global_blend_capabilities) {
					if (GUILayout.Button(new GUIContent("Sync material properties", "Will automatically setup terrain/mesh props used in blending material"))) {
						_target.SyncMaterialProps();
					}
				}
				GUILayout.Space(5);
				if (GUILayout.Button(new GUIContent("Rebuild mesh", "(operation needed after modification\nof mesh transform)"))) {
					RegisterUndoForMeshes();
					CheckShaderForBlendCapability();
					_target.MakeMeshCopy();
					EditorUtility.SetDirty(target);
				}
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.BeginVertical();
				if (GUILayout.Button(new GUIContent("Stick to Blended Object along normals"))) {
					RegisterUndoForMeshes();
					if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
					_target.StickToBlendedObject();
					_target.GetComponent<Renderer>().castShadows=false;
					EditorUtility.SetDirty(target);
				}
				if (GUILayout.Button(new GUIContent("Stick to Blended Object downward"))) {
					RegisterUndoForMeshes();
					if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
					_target.StickToBlendedObject(false);
					_target.GetComponent<Renderer>().castShadows=false;
					EditorUtility.SetDirty(target);
				}
				EditorGUILayout.EndVertical();
				EditorGUILayout.LabelField("Distance", GUILayout.Width(60));
				_target.StickOffset=EditorGUILayout.FloatField(_target.StickOffset);
				EditorGUILayout.EndHorizontal();
			
				if (_target.Sticked) {
					if (GUILayout.Button("Export OBJ")) {
						string directory;
						string file;
						if (_target.save_path=="") {
							directory=Application.dataPath;
							file="geom_blend_mesh.obj";
						} else {
							directory=Path.GetDirectoryName(_target.save_path);
							file=Path.GetFileNameWithoutExtension(_target.save_path)+".obj";
						}
						string path = EditorUtility.SaveFilePanel("Save mesh as OBJ file", directory, file, "obj");
						if (path!="") {
							path=path.Substring(Application.dataPath.Length-6);
							_target.save_path=path;
							if (_target.GetComponent(typeof(MeshFilter))) ObjExporter.MeshToFile((MeshFilter)_target.GetComponent(typeof(MeshFilter)), path);
						}										
					}
				}
			
//				if (_target.Sticked && _target.blendedObject && _target.blendedObject.GetComponent(typeof(Terrain)) && _target.blendedObject.GetComponent(typeof(TerrainCollider)) && _target.blendedObject.GetComponent(typeof(ReliefTerrain))) {
//					if (GUILayout.Button(new GUIContent("Unstretch UV"))) {
//						RegisterUndoForMeshes();
//						if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
//						_target.unstretchUV();
//						EditorUtility.SetDirty(target);
//					}
//				}
				
			}
		//}
		EditorGUILayout.EndVertical();
		GUILayout.Space(5);
		
		if (_target.blendedObject && _target.blendedObject.GetComponent(typeof(Collider))!=null  && ((_target.blendedObject.GetComponent(typeof(MeshRenderer))!=null) || (_target.blendedObject.GetComponent(typeof(Terrain))!=null))) {
			EditorGUILayout.BeginVertical("Box");
			//GUILayout.Space(10);
			if (GUILayout.Button(new GUIContent("Reset mesh to its original asset model", "Re-apply original model (below)"))) {
				RegisterUndoForMeshes();
				_target.pmesh=mf.sharedMesh=_target.orig_mesh;
				MeshCollider mc=_target.GetComponent(typeof(MeshCollider)) as MeshCollider;
				if (mc) {
					mc.sharedMesh=null;
					mc.sharedMesh=_target.orig_mesh;
				}
				_target.MakeMeshCopy();
				_target.Sticked=false;
				CheckShaderForBlendCapability();
				EditorUtility.SetDirty(target);
			}
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Original (asset) model", GUILayout.MaxWidth(140));
			_target.orig_mesh=(Mesh)EditorGUILayout.ObjectField(_target.orig_mesh, typeof(Mesh), false);
			GUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
			GUILayout.Space(10);
			
//			if (GUILayout.Button("Clear blending")) {
//				RegisterUndoForMeshes();
//				if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
//				_target.ClearBlend();
//				EditorUtility.SetDirty(target);
//			}
			

			EditorGUILayout.BeginVertical("Box");
			EditorGUI.BeginDisabledGroup(!_target.Sticked);
			if (GUILayout.Button(new GUIContent("Autoblend at edges", "Autoblend painting for vertices\non the edges"))) {
				RegisterUndoForMeshes();
				if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
				_target.PrepareMeshAtEdges();
				EditorUtility.SetDirty(target);
			}
			EditorGUI.EndDisabledGroup();
			EditorGUI.BeginDisabledGroup(_target.Sticked);
			//GUILayout.Space(10);"
			if (GUILayout.Button(new GUIContent("Flatten mesh near the ground", "Blended vertices near given distance (below) will be smoothly punched out"))) {
				RegisterUndoForMeshes();
				if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
				_target.FlattenMesh();
				MeshCollider mc=_target.GetComponent(typeof(MeshCollider)) as MeshCollider;
				if (mc) {
					mc.sharedMesh=null;
					mc.sharedMesh=mf.sharedMesh;
				}
				EditorUtility.SetDirty(target);
			}
			if (GUILayout.Button(new GUIContent("Tessellate mesh on ground intersections","This will introduce additional mesh edges at its ground intersections"))) {
				RegisterUndoForMeshes();
				if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
				_target.TessellateMesh();
				EditorUtility.SetDirty(target);
			}
			if (GUILayout.Button(new GUIContent("Autoblend from vertices distance", "Autoblend painting for vertices\non distance given below"))) {
				RegisterUndoForMeshes();
				if (mf.sharedMesh==_target.orig_mesh) _target.MakeMeshCopy();
				_target.PrepareMesh();
				EditorUtility.SetDirty(target);
			}
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Autoblend / flatten distance", GUILayout.MinWidth(165));
			_target.blend_distance=EditorGUILayout.Slider(_target.blend_distance, 0.02f, 1f);
			EditorGUILayout.EndHorizontal();
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndVertical();
			
			GUILayout.Space(5);
			EditorGUILayout.BeginVertical("Box");			
			bool prev_paint_flag=_target.paint_flag;
			
			if (!_target.paint_flag) {
				Color c=GUI.color;
				GUI.color=new Color(0.9f,1, 0.9f);
				if (GUILayout.Button(new GUIContent("Begin painting (M)",paintButTexOn, "Click to turn on painting"))) {
					_target.paint_flag=true;
				}
				GUI.color=c;
			} else if (_target.paint_flag) {
				Color c=GUI.color;
				GUI.color=new Color(1,0.9f,0.9f);
				if (GUILayout.Button(new GUIContent("End painting (M)",paintButTexOff, "Click to turn off painting"))) {
					_target.paint_flag=false;
				}
				GUI.color=c;
			}
			if (!prev_paint_flag && _target.paint_flag) {
				Tools.current=Tool.View;
				GeometryVsTerrainBlend._SceneGUI = new SceneView.OnSceneFunc(CustomOnSceneGUI);
				SceneView.onSceneGUIDelegate += GeometryVsTerrainBlend._SceneGUI;
			} else if (prev_paint_flag && !_target.paint_flag) {
				Tools.current=prev_tool;
				SceneView.onSceneGUIDelegate -= GeometryVsTerrainBlend._SceneGUI;
			}
			if (prev_paint_flag!=_target.paint_flag) EditorUtility.SetDirty(target);
			if (_target.paint_flag) {
				_target.ClearPaintMesh();
				cover_num=0;
				
				if (_target.paint_mode<1 && (!_target.GetComponent<Collider>() || !_target.GetComponent<Collider>().enabled)) EditorGUILayout.HelpBox("Object doesn't have collider (necessary for painting).",MessageType.Error, true);
				if (!_target.ModifyTris) {
					EditorGUILayout.HelpBox("Hold SHIFT while painting to apply eraser.",MessageType.Info, true);
				}
					
				string[] modes;
				int npaint_mode;
				if (_target.Sticked) {
					if (_target.blendedObject && _target.blendedObject.GetComponent<Collider>()) {// && (_target.blendedObject.collider is TerrainCollider)) {
						modes=new string[4] {"Blend", "Extrude Height", "Add tris", "Del tris"};
						int cur_paint_mode=_target.paint_mode;
						if (_target.ModifyTris) {
							if (_target.BuildMeshFlag) {
								cur_paint_mode=3;
							} else {
								cur_paint_mode=2;
							}
						}
						npaint_mode=GUILayout.SelectionGrid(cur_paint_mode, modes, 4);
						if (npaint_mode==3) {
							npaint_mode=0;
							_target.ModifyTris=true;
							_target.BuildMeshFlag=false;
						} else if (npaint_mode==2) {
							npaint_mode=0;
							_target.ModifyTris=true;
							_target.BuildMeshFlag=true;
						} else {
							_target.ModifyTris=false;
							_target.BuildMeshFlag=false;
						}
					} else {
						modes=new string[3] {"Blend", "Extrude Height", "Del tris"};
						npaint_mode=GUILayout.SelectionGrid(_target.ModifyTris ? 2:_target.paint_mode, modes, 3);
						if (npaint_mode==2) {
							npaint_mode=0;
							_target.ModifyTris=true;
							_target.BuildMeshFlag=false;
						} else {
							_target.ModifyTris=false;
							_target.BuildMeshFlag=false;
						}
					}
				} else {
					modes=new string[2] {"Blend", "Extrude Height"};
					npaint_mode=GUILayout.SelectionGrid(_target.paint_mode, modes, 2);
					_target.ModifyTris=false;
				}
				
				if (npaint_mode!=_target.paint_mode) {
					EditorUtility.SetDirty(target);
				}
				_target.paint_mode=npaint_mode;
				if (_target.paint_mode==1) {
					Texture2D tmp_tex=_target.get_tmpColorMap();
					if (tmp_tex && (tmp_tex.format!=TextureFormat.Alpha8 && tmp_tex.format!=TextureFormat.ARGB32)) {
						EditorGUILayout.HelpBox("Global colormap need to be readable and uncompressed for painting.",MessageType.Error, true);
					}
				}
				if (_target.ModifyTris) {
					EditorGUI.BeginDisabledGroup(!_target.BuildMeshFlag);
					GUILayout.BeginHorizontal();
						GUILayout.Label ("Subdivision", EditorStyles.label );
						_target.addTrisSubdivision = EditorGUILayout.IntSlider(_target.addTrisSubdivision, -3, 3);
					GUILayout.EndHorizontal();	
					GUILayout.BeginHorizontal();
						GUILayout.Label ("Slope range (" + Mathf.RoundToInt(_target.addTrisMinAngle) + "\u00B0 - " + Mathf.RoundToInt(_target.addTrisMaxAngle) + "\u00B0)", EditorStyles.label );
						EditorGUILayout.MinMaxSlider(ref _target.addTrisMinAngle, ref _target.addTrisMaxAngle, 0, 90);
					GUILayout.EndHorizontal();	
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.BeginHorizontal();
					GUILayout.Label ("Area size", EditorStyles.label );
					_target.paint_size = EditorGUILayout.Slider(_target.paint_size, 0.01f, 6);
				GUILayout.EndHorizontal();	
				EditorGUI.BeginDisabledGroup(_target.ModifyTris);
				GUILayout.BeginHorizontal();
					GUILayout.Label ("Area smoothness", EditorStyles.label );
					_target.paint_smoothness = EditorGUILayout.Slider (_target.paint_smoothness, 0.001f, 1);
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label ("Opacity", EditorStyles.label );
					_target.paint_opacity = EditorGUILayout.Slider (_target.paint_opacity, 0, 1);
				GUILayout.EndHorizontal();	
				EditorGUILayout.HelpBox("Mesh blend uses A channel. You might use different if you'd like to paint another material properties.", MessageType.Info, true);
				EditorGUILayout.BeginHorizontal();
					GUILayout.Label ("vertex channel", EditorStyles.label );
					_target.vertex_paint_channel = (ColorChannels)EditorGUILayout.EnumPopup(_target.vertex_paint_channel);
				EditorGUILayout.EndHorizontal();
				if (GUILayout.Button(new GUIContent("Clear channel"))) {
					_target.ClearBlend(_target.vertex_paint_channel);	
				}
				EditorGUI.EndDisabledGroup();
	
				GUILayout.Space(10);
			}
			EditorGUILayout.EndVertical();
			
		} else {
			EditorGUILayout.HelpBox("Select an object (terrain or GameObject with mesh) to be blended with this object.",MessageType.Warning, true);
		}
		
		GUILayout.Space(5);
		EditorGUILayout.HelpBox("Underlying child object renders blended underlying terrain. You might need to select it for debugging purposes.",MessageType.Info, true);
		_target.dont_select_aux_object=!EditorGUILayout.Toggle("Enable child selection", !_target.dont_select_aux_object);
		GUILayout.Space(5);
		
		Event current = Event.current;
		switch(current.type) {
			case EventType.keyDown:
				if (current.keyCode==KeyCode.M) {
					_target.paint_flag=!_target.paint_flag;
					if (_target.paint_flag) {
						Tools.current=Tool.View;
						GeometryVsTerrainBlend._SceneGUI = new SceneView.OnSceneFunc(CustomOnSceneGUI);
						SceneView.onSceneGUIDelegate += GeometryVsTerrainBlend._SceneGUI;
					} else {
						Tools.current=prev_tool;
						SceneView.onSceneGUIDelegate -= GeometryVsTerrainBlend._SceneGUI;
					}
					EditorUtility.SetDirty(target);
				}
			break;
		}
		
		if (EditorApplication.timeSinceStartup>UpdTim) {
			UpdTim=EditorApplication.timeSinceStartup+1;	
			_target.SetupValues();		
		}
		//DrawDefaultInspector();
	}
	
	public void OnSceneGUI() {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		
		if (Event.current.type==EventType.keyDown) {
			if (Event.current.keyCode==KeyCode.M) {
				_target.paint_flag=!_target.paint_flag;
				if (_target.paint_flag) {
					_target.ClearPaintMesh();
					cover_num=0;
					Tools.current=Tool.View;
					GeometryVsTerrainBlend._SceneGUI = new SceneView.OnSceneFunc(CustomOnSceneGUI);
					SceneView.onSceneGUIDelegate += GeometryVsTerrainBlend._SceneGUI;
				} else {
					Tools.current=prev_tool;
					SceneView.onSceneGUIDelegate -= GeometryVsTerrainBlend._SceneGUI;					
				}
				EditorUtility.SetDirty(target);
			}
		}
		
	}
	
	public void CustomOnSceneGUI(SceneView sceneview) {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		
		EditorWindow currentWindow = EditorWindow.mouseOverWindow;
		if(!currentWindow) return;
		
		//Rect winRect = currentWindow.position;
		Event current = Event.current;
		
		if (current.alt) {
			return;
		}		
		if (Event.current.button == 1) {
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
			return;
		}
		
		if (Tools.current!=Tool.View) {
			 _target.paint_flag=false;
			SceneView.onSceneGUIDelegate -= GeometryVsTerrainBlend._SceneGUI;
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
					_target.ClearPaintMesh();
					cover_num=0;
					_target.paint_flag=false;
					Tools.current=prev_tool;
					SceneView.onSceneGUIDelegate -= GeometryVsTerrainBlend._SceneGUI;
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
				get_paint_coverage(current.shift);
				cover_num_start_drag=cover_num;
				// Debug.Log(""+cover_num + "  "+ paintHitInfo_flag + _target.prepare_tmpColorMap());
				if (cover_num>0 || paintHitInfo_flag) {
					if (paintHitInfo_flag) {
						if (!_target.ModifyTris && _target.prepare_tmpColorMap()) {
							Undo.RegisterUndo(_target.get_tmpColorMap(), "Geometry Blend Edit");
							_target.modify_blend(cover_num, cover_indices, cover_strength, !current.shift);
							//current.Use();
						}
					} else {
						if (cover_num_start_drag>0) {
							RegisterUndoForMeshes();
							_target.undo_flag=false;
							if (_target.ModifyTris) {
								if (_target.BuildMeshFlag) {
									_target.add_tris(cover_num, cover_verts);
								} else {
									_target.remove_tris(cover_num, cover_tris);
								}
								cover_num=0;
							} else {
								_target.modify_blend(cover_num, cover_indices, cover_strength, !current.shift);
							}
							EditorUtility.SetDirty(target);
						}
					}
				} else {
					_target.undo_flag=true;
				}
				current.Use();
				_target.RealizePaint_Flag=true;
			break;
			case EventType.mouseDrag:
				get_paint_coverage(current.shift);
				if (cover_num>0 || paintHitInfo_flag) {
					if (_target.undo_flag) {
						if (_target.prepare_tmpColorMap()) {
							if (paintHitInfo_flag) {
								Undo.RegisterUndo(_target.get_tmpColorMap(), "Geometry Blend Edit");
							}
						} else {
							RegisterUndoForMeshes();
						}
						_target.undo_flag=false;
					}
				}
				if (cover_num_start_drag>0 || paintHitInfo_flag) {
					if (!_target.ModifyTris && _target.prepare_tmpColorMap()) {
						_target.modify_blend(cover_num, cover_indices, cover_strength, !current.shift);
						//current.Use();
					}
					if (_target.ModifyTris) {
						if (_target.BuildMeshFlag) {
							_target.add_tris(cover_num, cover_verts);
						} else {
							_target.remove_tris(cover_num, cover_tris);
						}
						cover_num=0;
					}
				}
				_target.RealizePaint_Flag=true;
			break;
			case EventType.mouseMove:
				get_paint_coverage(current.shift);
				if (_target.RealizePaint_Flag && _target.ModifyTris) {
					if (_target.BuildMeshFlag) {
						// optimize mesh after add tris
						_target.RealizePaint_Flag=false;
						MeshFilter mf=_target.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
						RemoveUnusedVerts(mf);
						if (_target.GetComponent<Collider>() && _target.GetComponent<Collider>() is MeshCollider) {
							if (mf) {
								(_target.GetComponent<Collider>() as MeshCollider).sharedMesh=mf.sharedMesh;
							}
						}				
						Transform tr=_target.transform.FindChild("RTP_blend_underlying");
						if (tr!=null) {
							GameObject go=tr.gameObject;
							MeshFilter mf_under=(MeshFilter)go.GetComponent(typeof(MeshFilter));
							Color[] prev_cols=null;
							if (mf_under) {
								prev_cols=mf_under.sharedMesh.colors;
							}
							_target.MakeMeshCopy();
							if (mf_under) {
								Color[] cur_cols=mf_under.sharedMesh.colors;
								for(int j=0; j<prev_cols.Length; j++) {
									cur_cols[j].a=prev_cols[j].a;
								}
								mf_under.sharedMesh.colors=cur_cols;
								if (mf) {
									Color[] colors=mf.sharedMesh.colors;
									for(int j=0; j<prev_cols.Length; j++) {
										colors[j].a=prev_cols[j].a;
									}
									mf.sharedMesh.colors=colors;
								}
							}						
						}
						_target.ClearPaintMesh();
						cover_num=0;
					} else {
						// optimize mesh after remove tris
						_target.RealizePaint_Flag=false;
						MeshFilter mf=_target.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
						RemoveUnusedVerts(mf);
						if (_target.GetComponent<Collider>() && _target.GetComponent<Collider>() is MeshCollider) {
							if (mf) {
								(_target.GetComponent<Collider>() as MeshCollider).sharedMesh=mf.sharedMesh;
							}
						}				
						Transform tr=_target.transform.FindChild("RTP_blend_underlying");
						if (tr!=null) {
							GameObject go=tr.gameObject;
							mf=(MeshFilter)go.GetComponent(typeof(MeshFilter));
							RemoveUnusedVerts(mf);
						}
						_target.ClearPaintMesh();
						cover_num=0;
					}
				}
			break;
		}
		
		int i;
		if (_target.ModifyTris) {
			if (_target.BuildMeshFlag) {
				// add
				Handles.color=Color.green;
				for(i=0; i<cover_num; i++) {
					if (cover_verts[i].x!=0 && cover_verts[i].y!=0 && cover_verts[i].z!=0) {
						Handles.DrawSolidDisc(cover_verts[i], Camera.current.transform.position-cover_verts[i], HandleUtility.GetHandleSize(cover_verts[i])*0.03f);
					}
				}
			} else {
				// remove
				Vector3[] verts=_target.get_vertices();
				int[] tris=_target.get_tris();
				if (verts!=null) {
					Handles.color=Color.red;
					for(i=0; i<cover_num; i+=3) {
						Vector3 pntA=_target.transform.TransformPoint(verts[tris[cover_tris[i]]]);
						Vector3 pntB=_target.transform.TransformPoint(verts[tris[cover_tris[i]+1]]);
						Vector3 pntC=_target.transform.TransformPoint(verts[tris[cover_tris[i]+2]]);
						Handles.DrawLine(pntA, pntB);
						Handles.DrawLine(pntB, pntC);
						Handles.DrawLine(pntA, pntC);
					}
				}
			}
		} else {
			if (current.shift) {
				if (_target.paint_mode<1) {
					for(i=0; i<cover_num; i++) {
						Handles.color=new Color(1,0,0, Mathf.Max(0.1f,cover_strength[i]*_target.paint_opacity));
						Handles.DrawSolidDisc(cover_verts[i], Camera.current.transform.position-cover_verts[i], HandleUtility.GetHandleSize(cover_verts[i])*0.03f);
					}					
				} else {
					if (paintHitInfo_flag) {
						Handles.color=new Color(1,0,0, Mathf.Max(0.1f, _target.paint_opacity*0.5f));
						Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size);
						Handles.color=new Color(1,0,0, Mathf.Max(0.6f, _target.paint_opacity));
						Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size*Mathf.Max(0.3f, 1 -_target.paint_smoothness));
					}
				}
			} else {
				if (_target.paint_mode<1) {
					for(i=0; i<cover_num; i++) {
						Handles.color=new Color(0,1,0,Mathf.Max(0.1f, cover_strength[i]*_target.paint_opacity));
						Handles.DrawSolidDisc(cover_verts[i], Camera.current.transform.position-cover_verts[i], HandleUtility.GetHandleSize(cover_verts[i])*0.03f);
					}
				} else {
					if (paintHitInfo_flag) {
						Handles.color=new Color(0,1,0, Mathf.Max(0.1f, _target.paint_opacity*0.5f));
						Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size);
						Handles.color=new Color(0,1,0, Mathf.Max(0.6f, _target.paint_opacity));
						Handles.DrawWireDisc(paintHitInfo.point, paintHitInfo.normal, _target.paint_size*Mathf.Max(0.3f, 1 -_target.paint_smoothness));
					}
				}
			}		
		}
		
		if (current.shift) current.Use();
	}

	private void get_paint_coverage(bool shift_flag) {
		if (EditorApplication.timeSinceStartup<lCovTim) return;
		lCovTim=EditorApplication.timeSinceStartup+0.02;
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		if (_target.Sticked) {
			if (!_target.blendedObject || !_target.blendedObject.GetComponent<Collider>()) return;
		} else {
			if (!_target.GetComponent<Collider>()) return;
		}
		if (_target.paint_mode<1) {
			Vector3[] vertices=_target.get_vertices();
			Vector3[] normals=_target.get_normals();
			int[] tris=_target.get_tris();
			Ray gui_ray=HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			Vector3 gui_ray_dir=_target.transform.InverseTransformDirection(gui_ray.direction);
			if (_target.ModifyTris) {
				Terrain terrainComp=(Terrain)_target.blendedObject.GetComponent(typeof(Terrain));
				if (_target.BuildMeshFlag) {
					// add tris
					if (terrainComp) {
						float cellW=terrainComp.terrainData.size.x/(terrainComp.terrainData.heightmapResolution-1)*Mathf.Pow(2.0f, -1.0f*_target.addTrisSubdivision);
						float cellH=terrainComp.terrainData.size.z/(terrainComp.terrainData.heightmapResolution-1)*Mathf.Pow(2.0f, -1.0f*_target.addTrisSubdivision);
						int _w=Mathf.RoundToInt((_target.paint_size*2)/cellW+1);
						int _h=Mathf.RoundToInt((_target.paint_size*2)/cellH+1);
						_w=_w>44 ? 44:_w;
						_h=_h>44 ? 44:_h;
						cover_num=0;
						TerrainCollider terrainCollider=(TerrainCollider)_target.blendedObject.GetComponent(typeof(TerrainCollider));
						Vector3 pnt=GetWorldPointFromMouse(terrainCollider);
						Vector3 terrainPosition=terrainComp.GetPosition();
						int centerX=Mathf.RoundToInt( (pnt.x-terrainPosition.x)/cellW );
						int centerZ=Mathf.RoundToInt( (pnt.z-terrainPosition.z)/cellH );
						for(int i=0; i<_w; i++) {
							for(int j=0; j<_h; j++) {
								int idx_i=i-_w/2+centerX;
								int idx_j=j-_h/2+centerZ;
								cover_verts[cover_num]=new Vector3(terrainPosition.x+idx_i*cellW, terrainComp.terrainData.GetInterpolatedHeight(idx_i*cellW/terrainComp.terrainData.size.x, idx_j*cellH/terrainComp.terrainData.size.z)+terrainPosition.y+0.02f, terrainPosition.z+idx_j*cellH);
								cover_num++;
							}
						}
					} else {
						ReliefTerrain geomScript=(ReliefTerrain)_target.blendedObject.GetComponent(typeof(ReliefTerrain));
						if (geomScript.controlA!=null) {
							float cellW=_target.blendedObject.GetComponent<Renderer>().bounds.size.x/(geomScript.controlA.width)*Mathf.Pow(2.0f, -1.0f*_target.addTrisSubdivision);
							float cellH=_target.blendedObject.GetComponent<Renderer>().bounds.size.z/(geomScript.controlA.height)*Mathf.Pow(2.0f, -1.0f*_target.addTrisSubdivision);
							int _w=Mathf.RoundToInt((_target.paint_size*2)/cellW+1);
							int _h=Mathf.RoundToInt((_target.paint_size*2)/cellH+1);
							_w=_w>44 ? 44:_w;
							_h=_h>44 ? 44:_h;
							cover_num=0;
							Collider terrainCollider=(Collider)_target.blendedObject.GetComponent(typeof(Collider));
							Vector3 pnt=GetWorldPointFromMouse(terrainCollider);
							Vector3 terrainPosition=_target.blendedObject.GetComponent<Renderer>().bounds.min;
							int centerX=Mathf.RoundToInt( (pnt.x-terrainPosition.x)/cellW );
							int centerZ=Mathf.RoundToInt( (pnt.z-terrainPosition.z)/cellH );
							for(int i=0; i<_w; i++) {
								for(int j=0; j<_h; j++) {
									int idx_i=i-_w/2+centerX;
									int idx_j=j-_h/2+centerZ;
									cover_verts[cover_num]=new Vector3(terrainPosition.x+idx_i*cellW, 0, terrainPosition.z+idx_j*cellH);
									RaycastHit hit;
									if (terrainCollider.Raycast(new Ray(new Vector3(cover_verts[cover_num].x, _target.blendedObject.GetComponent<Renderer>().bounds.max.y, cover_verts[cover_num].z), Vector3.down), out hit, Mathf.Infinity)) {
										cover_verts[cover_num].y=hit.point.y+0.03f;
									}
									cover_num++;
								}
							}
						}
					}
				} else {
					// remove tris
					if (tris!=null) {
						Vector3 pnt;
						if (_target.Sticked) {
							pnt=GetWorldPointFromMouse(_target.blendedObject.GetComponent<Collider>());
						} else {
							pnt=GetWorldPointFromMouse(_target.GetComponent<Collider>());
						}
						cover_num=0;
						for(int i=0; i<tris.Length; i+=3) {
							Vector3 wPos=T_lw(0.3333333f*(vertices[tris[i]]+vertices[tris[i+1]]+vertices[tris[i+2]]));
							float dist=Vector3.Distance(pnt, wPos);
							if ((cover_num<cover_tris.Length) && (dist<_target.paint_size)) {
								cover_tris[cover_num]=i;
								cover_num++;
							}
						}
					}
				}
			} else {
				if (vertices!=null) {
					Vector3 pnt;
					if (_target.Sticked) {
						pnt=GetWorldPointFromMouse(_target.blendedObject.GetComponent<Collider>());
					} else {
						pnt=GetWorldPointFromMouse(_target.GetComponent<Collider>());
					}
					cover_num=0;
					for(int i=0; i<vertices.Length; i++) {
						Vector3 wPos=T_lw(vertices[i]);
						float dist=Vector3.Distance(pnt, wPos);
						if ((cover_num<cover_verts.Length) && (dist<_target.paint_size) && (Vector3.Dot(gui_ray_dir, normals[i])<=0)) {
							cover_verts[cover_num]=wPos;
							cover_strength[cover_num]=(_target.paint_size-dist*_target.paint_smoothness)/_target.paint_size;
							cover_indices[cover_num]=i;
							cover_num++;
						}
					}
				}
			}
			paintHitInfo_flag=false;
		} else {
			cover_num=0;
	        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
			if (_target.blendedObject && _target.blendedObject.GetComponent<Collider>()) {
				paintHitInfo_flag=_target.blendedObject.GetComponent<Collider>().Raycast(ray, out paintHitInfo, Mathf.Infinity);
				_target.paintHitInfo=paintHitInfo;
				_target.paintHitInfo_flag=paintHitInfo_flag;
			}
		}
		EditorUtility.SetDirty(target);
	}
	
    private Vector3 GetWorldPointFromMouse(Collider obj_collider) {
		float planeLevel = 0;
        var groundPlane = new Plane(Vector3.up, new Vector3(0, planeLevel, 0));

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector3 hit = new Vector3(0,0,0);
        float dist;
		
		RaycastHit rayHit;
        if (obj_collider.Raycast(ray, out rayHit, Mathf.Infinity)) {
			return rayHit.point;
		} else if (groundPlane.Raycast(ray, out dist)) {
            hit = ray.origin + ray.direction.normalized * dist;
		}
        return hit;
    }

	Vector3 T_lw(Vector3 input) {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		return _target.transform.TransformPoint(input);
	}
	Vector3 T_wl(Vector3 input) {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		return _target.transform.InverseTransformPoint(input);
	}	
	
	void RegisterUndoForMeshes() {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;		
		UnityEngine.Object[] objs=new UnityEngine.Object[5];
		objs[0]=(_target.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter);
		objs[1]=(_target.gameObject.GetComponent (typeof(MeshCollider)) as MeshCollider);
		objs[2]=(_target.GetUnderlyingGameObject().GetComponent (typeof(MeshFilter)) as MeshFilter);
		objs[3]=(_target.GetUnderlyingGameObject().GetComponent (typeof(MeshCollider)) as MeshCollider);
		objs[4]=_target;
		Undo.RegisterUndo( objs, "Geometry Blend Edit");
		string nam=(_target.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter).sharedMesh.name;
		_target.pmesh=(_target.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter).sharedMesh=Instantiate((_target.gameObject.GetComponent (typeof(MeshFilter)) as MeshFilter).sharedMesh) as Mesh;
		_target.pmesh.name=nam;
	}

	private void RemoveUnusedVerts(MeshFilter mf) {
		if (!mf) return;
		Mesh sh=mf.sharedMesh;
		Vector3[] vertices=sh.vertices;
		Vector3[] normals=sh.normals;
		Vector4[] tangents=sh.tangents;
		Color[] colors=sh.colors;
		Vector2[] uv=sh.uv;
		Vector2[] uv2=sh.uv2;
		int[] tris=sh.triangles;
		int[] tris_mod=sh.triangles;
		
		ArrayList new_vertices=null;
		if (vertices!=null && vertices.Length>0) new_vertices=new ArrayList();
		ArrayList new_normals=null;
		if (normals!=null && normals.Length>0) new_normals=new ArrayList();
		ArrayList new_tangents=null;
		if (tangents!=null && tangents.Length>0) new_tangents=new ArrayList();
		ArrayList new_colors=null;
		if (colors!=null && colors.Length>0) new_colors=new ArrayList();
		ArrayList new_uv=null;
		if (uv!=null && uv.Length>0) new_uv=new ArrayList();
		ArrayList new_uv2=null;
		if (uv2!=null && uv2.Length>0) new_uv2=new ArrayList();

		for(int i=0; i<vertices.Length; i++) {
			bool unused=true;
			for(int j=0; j<tris.Length; j++) {
				if (tris[j]==i) {
					unused=false;
					break;
				}
			}
			if (unused) {
				for(int j=0; j<tris.Length; j++) {
					if (tris[j]>i)	tris_mod[j]--;
				}
			} else {
				if (new_vertices!=null) new_vertices.Add(vertices[i]);
				if (new_normals!=null) new_normals.Add(normals[i]);
				if (new_tangents!=null) new_tangents.Add(tangents[i]);
				if (new_colors!=null) new_colors.Add(colors[i]);
				if (new_uv!=null) new_uv.Add(uv[i]);
				if (new_uv2!=null) new_uv2.Add(uv2[i]);
			}
		}
		sh.triangles=null;
		if (new_vertices!=null) sh.vertices=(Vector3[])new_vertices.ToArray(typeof(Vector3));
		if (new_normals!=null) sh.normals=(Vector3[])new_normals.ToArray(typeof(Vector3));
		if (new_tangents!=null) sh.tangents=(Vector4[])new_tangents.ToArray(typeof(Vector4));
		if (new_colors!=null) sh.colors=(Color[])new_colors.ToArray(typeof(Color));
		if (new_uv!=null) sh.uv=(Vector2[])new_uv.ToArray(typeof(Vector2));
		if (new_uv2!=null) sh.uv2=(Vector2[])new_uv2.ToArray(typeof(Vector2));
		sh.triangles=tris_mod;
		sh.RecalculateBounds();
		mf.sharedMesh=sh;
	}
	
	private void CheckShaderForBlendCapability() {
		GeometryVsTerrainBlend _target=(GeometryVsTerrainBlend)target;
		
		_target.shader_global_blend_capabilities=false;
		if (_target.GetComponent<Renderer>().sharedMaterial) {
			Shader shad=_target.GetComponent<Renderer>().sharedMaterial.shader;
			string path=AssetDatabase.GetAssetPath(shad);
			if (path!="") {
				string _code = System.IO.File.ReadAllText(path);
				if (_code.IndexOf("_TERRAIN_PosSize")>0) {
					_target.shader_global_blend_capabilities=true;
					_target.SyncMaterialProps();
				}
			}
		}
	}
	
#endif
}
