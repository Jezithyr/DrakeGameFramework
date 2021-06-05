using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DrakeFramework;
using DrakeFramework.Core;
namespace DrakeFramework.Editor
{
	public class FrameworkSettingsWindow : EditorWindow
	{
		//TODO: Rewrite this to use the settings system when that gets made
		private const string dataRoot = "DrakeFramework";
		private const string resourceFolder = "Resources";
		private const string settingsName = "CoreSettings";

		private string baseModName = "BaseMod";
		private string baseModTag = "BASE";
		private DGFSettings settings;

		// Add menu named "My Window" to the Window menu
		[MenuItem("Window/DrakeFramework/Settings")]
		static void Init()
		{
			// Get existing open window or if none, make a new one:
			FrameworkSettingsWindow window = (FrameworkSettingsWindow)EditorWindow.GetWindow(typeof(FrameworkSettingsWindow));
			window.Show();
		}
		void Awake()
		{
			settings = FindOrCreateSettingsObject();
			baseModName = settings.BaseModName;
			baseModTag = settings.BaseModTag;
		}
		void OnGUI()
    	{
			if (settings == null) 
			{
				FrameworkSettingsWindow window = (FrameworkSettingsWindow)EditorWindow.GetWindow(typeof(FrameworkSettingsWindow));
				window.Close();
			}
        	GUILayout.Label("Base Settings", EditorStyles.boldLabel);
			settings.BaseModName = EditorGUILayout.TextField("BaseMod Name:", baseModName);
			settings.BaseModTag = EditorGUILayout.TextField("BaseMod Tag:", baseModTag);
			baseModName = settings.BaseModName;
			baseModTag = settings.BaseModTag;
        	//groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        	//EditorGUILayout.EndToggleGroup();
    	}

		private static DGFSettings FindOrCreateSettingsObject()
		{
			DGFSettings settingsAsset = (DGFSettings)AssetDatabase.LoadAssetAtPath( "Assets/"+dataRoot+"/"+resourceFolder+"/DGFSettings/"+settingsName+".asset", typeof(DGFSettings));
			if (settingsAsset != null) return settingsAsset;
			if (!AssetDatabase.IsValidFolder("Assets/"+dataRoot))
			{
				AssetDatabase.CreateFolder("Assets", dataRoot);
			}
			if (!AssetDatabase.IsValidFolder("Assets/"+dataRoot+"/"+resourceFolder))
			{
				AssetDatabase.CreateFolder("Assets/"+dataRoot, resourceFolder);
			}
			if (!AssetDatabase.IsValidFolder("Assets/"+dataRoot+"/"+resourceFolder+"/DGFSettings"))
			{
				AssetDatabase.CreateFolder("Assets/"+dataRoot+"/"+resourceFolder, "DGFSettings");
			}
			if (settingsAsset == null)
			{
				settingsAsset = ScriptableObject.CreateInstance<DGFSettings>();
			}

			settingsAsset.name = "DrakeFrameworkSettings";
        	AssetDatabase.CreateAsset(settingsAsset, "Assets/"+dataRoot+"/"+resourceFolder+"/DGFSettings/"+settingsName+".asset");
        	AssetDatabase.SaveAssets();
			return settingsAsset;
		}

	}
}