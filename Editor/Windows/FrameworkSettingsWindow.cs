using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DrakeFramework;
namespace DrakeFramework.Editor
{
	public class FrameworkSettingsWindow : EditorWindow
	{
		private const string dataRoot = "DrakeFramework";
		private const string resourceFolder = "Resources";
		private const string settingsName = "CoreSettings";

		private static string baseModName = "BaseMod";
		private static string baseModTag = "BASE";
		private static GameSettings settings;

		// Add menu named "My Window" to the Window menu
		[MenuItem("Window/DrakeFramework/Settings")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			FrameworkSettingsWindow window = (FrameworkSettingsWindow)EditorWindow.GetWindow(typeof(FrameworkSettingsWindow));
			settings = FindOrCreateSettingsObject();
			if (settings == null) return;
			baseModName = settings.baseModName;
			baseModTag = settings.baseModTag;
			window.Show();
		}
		void OnGUI()
    	{
			if (settings == null) 
			{
				FrameworkSettingsWindow window = (FrameworkSettingsWindow)EditorWindow.GetWindow(typeof(FrameworkSettingsWindow));
				window.Close();
			}
        	GUILayout.Label("Base Settings", EditorStyles.boldLabel);
			baseModName = EditorGUILayout.TextField("BaseMod Name:", baseModName);
			baseModTag = EditorGUILayout.TextField("BaseMod Tag:", baseModTag);
			settings.baseModName = baseModName;
			settings.baseModTag = baseModTag;
        	//groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        	//EditorGUILayout.EndToggleGroup();
    	}

		public override void SaveChanges()
    	{
        	// Your custom save procedures here
			settings = FindOrCreateSettingsObject();
			settings.baseModName = baseModName;
			settings.baseModTag = baseModTag;
       		base.SaveChanges();
    	}

		private static GameSettings FindOrCreateSettingsObject()
		{
			GameSettings settingsAsset = (GameSettings)AssetDatabase.LoadAssetAtPath("Assets/Textures/texture.jpg", typeof(GameSettings));
			if (settingsAsset != null) return settingsAsset;
			if (!AssetDatabase.IsValidFolder("Assets/"+dataRoot))
			{
				AssetDatabase.CreateFolder("Assets", dataRoot);
			}
			if (!AssetDatabase.IsValidFolder("Assets/"+dataRoot+"/"+resourceFolder))
			{
				AssetDatabase.CreateFolder("Assets/"+dataRoot, resourceFolder);
			}

			settingsAsset = ScriptableObject.CreateInstance<GameSettings>();
			settingsAsset.name = "DrakeFrameworkSettings";
        	AssetDatabase.CreateAsset(settingsAsset, "Assets/"+dataRoot+"/"+resourceFolder+"/DGFSettings/"+settingsName+".asset");
        	AssetDatabase.SaveAssets();
			return settingsAsset;
		}

	}
}