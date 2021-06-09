using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DrakeFramework
{
	public class UiManager
	{
		//TODO: move these to a unity menu
		private const string UI_PREFAB_PREFIX = "UiPrefabs/";
		private const string UI_SCREEN_PREFIX = "UiScreens/";
		private const string PREFAB_SUFFIX = ".prefab";
		private Dictionary<string, GameObject> UiPrefabs = new Dictionary<string, GameObject>();
		private Dictionary<string, GameObject> ScreenPrefabs = new Dictionary<string, GameObject>();
		private GameObject UiRoot;
		private List<AsyncOperationHandle<GameObject>> prefabHandles = new List<AsyncOperationHandle<GameObject>>();
		private Dictionary<string, GameObject> loadedScreens = new Dictionary<string, GameObject>();
		internal UiManager()
		{
			Game.Content.RegisterOnModsLoadMethod(OnModsLoaded);
			Game.Content.RegisterOnModsUnloadMethod(OnModsUnloaded);
			UiRoot = GameObject.Instantiate(Resources.Load<GameObject>("DGF/UiRoot"));
			UiRoot.name = "Drake.UI";
			Object.DontDestroyOnLoad(UiRoot);
		}

		public bool HasScreen(string screenName)
		{
			return loadedScreens.ContainsKey(UI_SCREEN_PREFIX + screenName + PREFAB_SUFFIX);
		}


		//gets a currently loaded screen game object
		public GameObject GetScreen(string screenName)
		{
			try
			{
				return loadedScreens[UI_SCREEN_PREFIX + screenName + PREFAB_SUFFIX];
			}
			catch (System.Exception)
			{
				Debug.LogWarning("Screen: " + screenName + " not loaded!");
				throw;
			}
		}

		//get the game object for a UIPrefab, only use to instantiate!
		public GameObject GetUiPrefab(string prefabName)
		{
			return UiPrefabs[UI_PREFAB_PREFIX + prefabName + PREFAB_SUFFIX];
		}
		public bool HasUiPrefab(string prefabName)
		{
			return UiPrefabs.ContainsKey(UI_PREFAB_PREFIX + prefabName + PREFAB_SUFFIX);
		}

		//Sets the screen's visibility
		public void SetScreenVisible(string screenName, bool visible)
		{
			if (!HasScreen(screenName))
			{
				Debug.LogWarning("Screen: " + screenName + " not loaded!");
				return;
			}
			loadedScreens[UI_SCREEN_PREFIX + screenName + PREFAB_SUFFIX].SetActive(visible);
		}

		//Hide the specified screen
		public void HideScreen(string screenName)
		{
			SetScreenVisible(screenName, false);
		}

		//Shows the specified screen, will load it if it isn't already loaded
		public void ShowScreen(string screenName)
		{
			if (!HasScreen(screenName))
			{
				LoadScreen(screenName);
			}
			SetScreenVisible(screenName, true);
		}

		//Load a screen into the scene
		public void LoadScreen(string screenName, bool setVisiblityOnLoad = true, bool visibility = true)
		{
			if (HasScreen(screenName))
			{
				Debug.LogWarning("Screen: " + screenName + " already loaded!");
				return;
			}
			try
			{
				string QualifiedName = UI_SCREEN_PREFIX + screenName + PREFAB_SUFFIX;
				GameObject newScreen = GameObject.Instantiate(ScreenPrefabs[QualifiedName]);
				newScreen.transform.SetParent(UiRoot.transform, true);
				newScreen.name = screenName;
				loadedScreens.Add(QualifiedName, newScreen);
				//removing any canvas' on ui prefabs to prevent scaling issues
				if (newScreen.GetComponentInChildren<Canvas>() != null)
				{
					GameObject.Destroy(newScreen.GetComponentInChildren<GraphicRaycaster>());
					GameObject.Destroy(newScreen.GetComponentInChildren<CanvasScaler>());
					GameObject.Destroy(newScreen.GetComponentInChildren<Canvas>());
				}
				
				if (setVisiblityOnLoad)
				{
					SetScreenVisible(screenName, visibility);
				}
			}
			catch (System.Exception)
			{
				Debug.LogError(screenName + " not found in prefab data!");
			}
		}
		public void UnloadAllScreens()
		{
			foreach (var screenData in loadedScreens)
			{
				Object.Destroy(screenData.Value);
			}
			loadedScreens.Clear();
		}

		//unloads a screen from the scene
		public void UnloadScreen(string screenName)
		{
			if (!HasScreen(screenName))
			{
				Debug.LogWarning("Screen: " + screenName + " not loaded!");
				return;
			}
			Object.Destroy(loadedScreens[screenName]);
			loadedScreens.Remove(UI_SCREEN_PREFIX + screenName + PREFAB_SUFFIX);
		}
		//internal helper for when mods are loaded
		private void OnModsLoaded(List<ModInfo> mods)
		{
			foreach (var mod in mods)
			{
				IList<IResourceLocation> UiPrefabLocs = Game.Content.FindAssetsWithLabel("UiPrefabs");
				IList<IResourceLocation> UiScreenLocs = Game.Content.FindAssetsWithLabel("Screens");
				foreach (var loc in UiPrefabLocs)
				{
					Addressables.LoadAssetAsync<GameObject>(loc).Completed += (handle) =>
					{
						GameObject gameObject = handle.Result;
						UiPrefabs[loc.PrimaryKey] = gameObject;
					};
				}
				foreach (var loc in UiScreenLocs)
				{
					Addressables.LoadAssetAsync<GameObject>(loc).Completed += (handle) =>
					{
						GameObject gameObject = handle.Result;
						ScreenPrefabs[loc.PrimaryKey] = gameObject;
					};
				}
			}
		}
		//internal helper for when mods are unloaded
		private void OnModsUnloaded(List<ModInfo> mods)
		{
			ReleasePrefabs();
		}
		//release prefab assets from memory
		private void ReleasePrefabs()
		{
			foreach (var keyPair in UiPrefabs)
			{
				Addressables.ReleaseInstance(keyPair.Value);
			}
			foreach (var keyPair in ScreenPrefabs)
			{
				Addressables.ReleaseInstance(keyPair.Value);
			}
			foreach (var prefabHandle in prefabHandles)
			{
				Addressables.ReleaseInstance(prefabHandle);
			}
			prefabHandles.Clear();
			UiPrefabs.Clear();
			ScreenPrefabs.Clear();
		}

		//destructor to cleanup references
		~UiManager()
		{
			GameObject.Destroy(UiRoot);
			ReleasePrefabs();
		}
	}
}