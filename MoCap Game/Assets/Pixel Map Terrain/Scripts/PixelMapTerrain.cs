using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;

public class PixelMapTerrain : MonoBehaviour {

	// Data
	public Texture2D heightmap;

	// User Settings
	public Dimensions dimensions = new Dimensions();
	public bool slopeEdges = true;
	public int slopeTolerance = 25;						// Polygons with normals greater than this are hills

	// Mesh Information
	public Vector3[] 	vertices;
	public int[] 		triangles;
	public Vector2[] 	uvs;

	// Texture(s)
	public Material[] mat = new Material[2];

	// Public call
	public void GenerateTerrain(Texture2D heightmap, Dimensions dimensions, Material[] mat)
	{
#if UNITY_EDITOR
		heightmap = GrayscaleToAlpha(heightmap, dimensions.x / dimensions.blockSize);
#endif
		// Read initial heights from alpha and apply user set modifier
		float[] heights = HeightFromImage(heightmap, dimensions.height);

		int[] blockedHeights = BlockifyHeights(heights, dimensions.blockSize, dimensions.height);

		// returns new array 4x size, in this format:
		// 0 0 1 1 2 2
		// 0 0 1 1 2 2
		// 3 3 3 3 4 4
		// 5 5 6 6 7 7
		// 5 5 6 6 7 7
		float[] cubedHeights = CubeHeights(blockedHeights);

		// The height array to use
		float[] points = cubedHeights;

		vertices = new Vector3[points.Length];
		triangles = new int[vertices.Length*3*3];
		int width = (int)Mathf.Sqrt(cubedHeights.Length), height = width;
		int v = 0; 
		float xPos = 0, yPos = 0;
		int cubeSize;
		if(slopeEdges)
			cubeSize = dimensions.x / width;
			else
			cubeSize = dimensions.x / (width/2);

		for(int y = 0; y < height; y++)
		{
			for(int x = 0; x < width; x++)
			{
				vertices[v] = new Vector3(xPos, points[v], yPos);

				if(slopeEdges)
					xPos+=cubeSize;
				else
				{
					if(x % 2 == 0)
						xPos += cubeSize;
				}
				
				v++;
			}
			xPos = 0;
			if(slopeEdges)
				yPos += cubeSize;
			else
			{
				if(y%2 == 0)
					yPos += cubeSize;
			}
		}

		width--; 				// terrain doesn't wrap
		height--;				// ... or does it?

		// Create faces
		int one, two, three, four;
		int row = 0;
		int step = width + 1; 	// no.  it doesn't
		int indice = 0;
		List<int> flat = new List<int>();
		List<int> hill = new List<int>();
		for(int y = 0; y < height; y++)
		{
			for(int x = 0; x < width; x++)
			{
				one = x+row;
				two = x+row+1;
				three = x+row+step;
				four = x+row+step+1;

				if( Mathf.Abs(Vector3.Angle(PlaneNormal(vertices[one], vertices[two], vertices[three]), Vector3.up) % 180) > slopeTolerance) {
					hill.Add(one);
					hill.Add(three);
					hill.Add(two);
				} else {
					flat.Add(one);
					flat.Add(three);
					flat.Add(two);
				}

				if( Mathf.Abs(Vector3.Angle(PlaneNormal(vertices[two], vertices[four], vertices[three]), Vector3.up) % 180) > slopeTolerance) {
					hill.Add(two);
					hill.Add(three);
					hill.Add(four);
				} else {
					flat.Add(two);
					flat.Add(three);
					flat.Add(four);
				}

				indice += 6;
			}
			row += step;
		}

		// UV Generation
		uvs = new Vector2[vertices.Length];
		int u = 0;
		v = 0;
		int column = 0;
		width++;
		for(int i = 0; i < uvs.Length; i++)
		{
			row = i / width;
			column = i % width;

			if(row % 2 == 0)
				v = 0;
			else
				v = 1;

			if(column % 2 == 0)
				u = 0;
			else
				u = 1;

			uvs[i] = new Vector2(u,row);				
		}

		Mesh m = new Mesh();
		m.name = gameObject.name + "_mesh";
		m.Clear();
		m.vertices = vertices;

		m.subMeshCount = 2;
		m.SetTriangles(flat.ToArray(), 0);
		m.SetTriangles(hill.ToArray(), 1);
		m.uv = uvs;
		m.RecalculateNormals();
		m.Optimize();
		
		if(gameObject.GetComponent<MeshFilter>())
			DestroyImmediate(gameObject.GetComponent<MeshFilter>());
		if(gameObject.GetComponent<MeshRenderer>())
			DestroyImmediate(gameObject.GetComponent<MeshRenderer>());

		gameObject.AddComponent<MeshFilter>().sharedMesh = m;
		gameObject.AddComponent<MeshRenderer>().sharedMaterials = mat;

#if UNITY_EDITOR
		EditorUtility.UnloadUnusedAssets();
#endif
	}

	public float ColorToFloat(Color color)
	{
		return 255f*(color.r + color.g + color.b);
	}

	public float[] HeightFromImage(Texture2D img, float heightModifier)
	{
		Color[] colors = img.GetPixels();
		int width = img.width, height = img.height;

		float[] values = new float[colors.Length];
		int i = 0;
		for(int y = height-1; y > -1; y--)
		{
			for(int x = width-1; x > -1; x--)
			{
				values[i] = colors[x + (y*height)].a * (float)heightModifier;
				i++;
			}
		}

		return values;
	}

	public void OnDestroy()
	{
		// Unfortunately this doesn't really work.  Oh well.
		DestroyImmediate(gameObject.GetComponent<MeshFilter>().sharedMesh);
#if UNITY_EDITOR
		EditorUtility.UnloadUnusedAssets();
#endif
	}

#if UNITY_EDITOR
	public Texture2D GrayscaleToAlpha(Texture2D img, int size)
	{
		if(AssetDatabase.GetAssetPath( (Texture2D)img) != null)
		{
			TextureImporter tempImporter = TextureImporter.GetAtPath( AssetDatabase.GetAssetPath( (Texture2D)img) ) as TextureImporter;
			tempImporter.isReadable = true;
			tempImporter.textureType = TextureImporterType.Advanced;
			tempImporter.mipmapEnabled = false;
			tempImporter.filterMode = FilterMode.Point;
			tempImporter.textureFormat = TextureImporterFormat.ARGB32;
			tempImporter.npotScale = TextureImporterNPOTScale.ToNearest;
			tempImporter.grayscaleToAlpha = true;
			tempImporter.maxTextureSize = size;
			AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath((Texture2D)img), ImportAssetOptions.ForceUpdate);
		}
		return img;
	}	
#endif

	public int[] BlockifyHeights(float[] heights, int blockSize, int mapHeight)
	{
		int[] blockHeights = new int[heights.Length];

		float div = 1f/(float)mapHeight;

		for(int i = 0; i < heights.Length; i++)
		{
			blockHeights[i] = (int)(heights[i]/div);
		}

		return blockHeights;
	}

	public float[] CubeHeights(int[] heights)
	{
		int width = (int)Mathf.Sqrt(heights.Length), height = (int)Mathf.Sqrt(heights.Length);
		
		// we only need to know width or height, cause ya'know, it's a square
		int cubedWidth = width*2;

		float[] cubifiedHeights = new float[heights.Length*4];

		// for each value, create 4 points in a square around it
		int row = 0;
		int step = cubedWidth;
		int i = 0;
		for(int y = 0; y < height; y++)
		{
			for(int x = 0; x < width; x++)
			{
				cubifiedHeights[x*2+row+0] = heights[i];
				cubifiedHeights[x*2+row+1] = heights[i];
				cubifiedHeights[x*2+row+step] = heights[i];  
				cubifiedHeights[x*2+row+step+1] = heights[i];
				i++;
			}
			row += step*2;
		}

		return cubifiedHeights;
	}

	/*
	 *	Returns the vector normal for a plane
	 */
	public Vector3 PlaneNormal(Vector3 p0, Vector3 p1, Vector3 p2)
	{
		Vector3 cross = Vector3.Cross(p1 - p0, p2 - p0);
		if (cross.magnitude < Mathf.Epsilon)
		    return new Vector3(0f, 0f, 0f); // bad triangle
		else
		{
			cross.Normalize();
			return cross;
		}
	}
}

[System.Serializable]
public class Dimensions
{
	public int x, y;
	public int blockSize;
	public int height;

	public Dimensions()
	{
		x = 128;
		y = 128;
		blockSize = 1;
		height = 2;
	}

	public override string ToString()
	{
		return "Size: (" + x + ", " + y + ")\nBlock Size: " + blockSize + "\nHeight: " + height;
	}
}