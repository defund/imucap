/*
 *	Adds context menu item to create Text Asset
 *	Karl Henkel
 */

using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateTextAsset : Editor {

	[MenuItem("Assets/Create/Text Asset")]
	public static void Create(MenuCommand command)
	{
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);

		if(path == "")
			path = "Assets/";

		// If path is a file, get the parent directory
		if(File.Exists(path))
			path = Path.GetDirectoryName(path);

		path += "/New Text Asset.txt";

		path = AssetDatabase.GenerateUniqueAssetPath(path);

		File.CreateText(path);

		AssetDatabase.ImportAsset(path);

		Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
	}
}
