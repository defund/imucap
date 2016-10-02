#pragma warning disable 0414

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PixelMapTerrain))]
public class PixelMapTerrainEditor : Editor {

	[MenuItem("Terrain/Pixel Map Terrain")]
	public static void init()
	{
		GameObject newCubeTerrain = new GameObject();
		newCubeTerrain.name = "Pixel Map Terrain";
		newCubeTerrain.AddComponent<PixelMapTerrain>();
	}

	PixelMapTerrain cubeTerrain;
	GameObject  terrain;
	public override void OnInspectorGUI()
	{
		cubeTerrain = (PixelMapTerrain)target;
		terrain = (GameObject)cubeTerrain.gameObject;
		
		cubeTerrain.heightmap = (Texture2D)EditorGUILayout.ObjectField("Heightmap", cubeTerrain.heightmap, typeof(Texture2D), true);
		
		cubeTerrain.mat[0] = (Material)EditorGUILayout.ObjectField("Flatland Material", cubeTerrain.mat[0], typeof(Material), true);
		cubeTerrain.mat[1] = (Material)EditorGUILayout.ObjectField("Sloped Material", cubeTerrain.mat[1], typeof(Material), true);

		if(!cubeTerrain.heightmap)
			return;

		if(cubeTerrain.heightmap.width != cubeTerrain.heightmap.height)
			EditorGUILayout.HelpBox("Uh oh.  Looks like you're attempting to use a non-squared image.  This isn't supported at the moment.", MessageType.Error);

		cubeTerrain.dimensions.blockSize = EditorGUILayout.IntField("Block Size", cubeTerrain.dimensions.blockSize);
		
		cubeTerrain.dimensions.x = EditorGUILayout.IntField("Map Size", cubeTerrain.dimensions.x);
		cubeTerrain.dimensions.y = cubeTerrain.dimensions.x;

		cubeTerrain.slopeTolerance = EditorGUILayout.IntField(new GUIContent("Slope Angle", "The angle at which a polygon will be considered a sloping triangle."), cubeTerrain.slopeTolerance);

		cubeTerrain.dimensions.height = EditorGUILayout.IntField("Map Height", cubeTerrain.dimensions.height);

		// Support for non-sloped edges not yet implemented.  It works, but the UVs aren't great and vertices aren't shared properly
		// cubeTerrain.slopeEdges = EditorGUILayout.Toggle("Slope Edges", cubeTerrain.slopeEdges);

		if(GUILayout.Button("Generate Terrain") && cubeTerrain.heightmap != null)
			cubeTerrain.GenerateTerrain(cubeTerrain.heightmap, cubeTerrain.dimensions, cubeTerrain.mat);

		if(GUI.changed)
			EditorUtility.SetDirty(target);
	}
}