using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DrakeFramework.Editor
{
    public static partial class EditorAssetUtil
    {
		public static T LoadAssetAtAssetDBPath<T>(string asset,params string[] folderNames) where T: Object
		{
			string path = "Assets/";
			foreach (var name in folderNames)
			{
				path+=(folderNames +"/");
			}
			return (T)AssetDatabase.LoadAssetAtPath(path+asset, typeof(T));
		}

		public static Object LoadAssetAtDBPath(string asset, System.Type assetType,params string[] folderNames)
		{
			string path = "Assets/";
			foreach (var name in folderNames)
			{
				path+=(folderNames +"/");
			}
			return AssetDatabase.LoadAssetAtPath(path+asset, assetType);
		}

		public static void TryCreateFolderPath(params string[] folderNames)
		{
			string currentPath = "Assets/";
			if (folderNames.Length == 0) return;
			foreach (var name in folderNames)
			{
				if (!AssetDatabase.IsValidFolder(currentPath+name))
				{
					AssetDatabase.CreateFolder(currentPath, name);
				}
			}
		}
		public static string CreatePath(string assetName, params string[] folderPath)
		{
			string currentPath = "Assets/";
			foreach (var folderName in folderPath)
			{
				currentPath+=(folderName+"/");
			}
			currentPath+= (assetName+".asset");
			return currentPath;
		}
		
		public static T CreateOrGetAssetAtPath<T>(string assetName, params string[] folderPath) where T:ScriptableObject, new()
		{
			TryCreateFolderPath(folderPath);
			T asset = LoadAssetAtAssetDBPath<T>(assetName,folderPath);
			if (asset == null)
			{
				asset = ScriptableObject.CreateInstance<T>();
				asset.name = assetName.Split('.')[0];
				AssetDatabase.CreateAsset(asset, CreatePath(assetName, folderPath));
        		AssetDatabase.SaveAssets();
			}
			return asset;
		}

	
		public static ScriptableObject CreateOrGetScriptableAtPath(string assetName, System.Type assetType,params string[] folderPath)
		{
			TryCreateFolderPath(folderPath);
			ScriptableObject asset = (ScriptableObject)LoadAssetAtDBPath(assetName, assetType, folderPath);
			if (asset == null)
			{
				asset = ScriptableObject.CreateInstance(assetType);
				asset.name = assetName.Split('.')[0];
				AssetDatabase.CreateAsset(asset, CreatePath(assetName, folderPath));
        		AssetDatabase.SaveAssets();
			}
			return asset;
		}
	}
}
