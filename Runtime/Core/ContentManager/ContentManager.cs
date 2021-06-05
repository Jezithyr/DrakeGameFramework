using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace DrakeFramework
{
	public class ContentManager
	{
		public delegate void ModsEventDelegate(List<ModInfo> loadedMods);
		public delegate void ModEventDelegate(ModInfo mod);
		private List<ModInfo> loadedMods = new List<ModInfo>();
		private Dictionary<string, short> modTagLookup = new Dictionary<string, short>();
		private Dictionary<string, short> modNameLookup = new Dictionary<string, short>();
		private ModInfo baseMod;
		public ModInfo BaseMod => baseMod;
		private Dictionary<string, IResourceLocation> assetRegistry = new Dictionary<string, IResourceLocation>();
		private List<AsyncOperationHandle> loadedAssets = new List<AsyncOperationHandle>();

		private ModsEventDelegate onModsUnloaded;
		private ModsEventDelegate onModsLoaded;
		internal ContentManager()
		{
			Addressables.InitializeAsync().Completed += handle =>
			{
				baseMod = new ModInfo(handle.Result);
				loadedMods.Add(baseMod);
				SetupModLookupData(baseMod, 0);
				LoadMods();
			};
			//TODO: Load last loaded mods from config
		}
		//get an assets location by name
		public IResourceLocation GetAssetLocation(string assetName, string modTag = Game.BaseModTag)
		{
			return assetRegistry[modTag + "." + assetName];
		}

		//get an asset's location by ref
		public IResourceLocation GetAssetLocation(AssetReference assetReference, string modTag = Game.BaseModTag)
		{
			return GetAssetLocation(assetReference.Asset.name, modTag);
		}

		//Loads an asset by ref
		public AsyncOperationHandle<T> LoadAssetAsync<T>(AssetReference assetRef, string modTag = Game.BaseModTag)
		{
			//not sure if this will work
			return LoadAssetAsync<T>(assetRef, modTag);
		}


		//Loads an asset by name
		public AsyncOperationHandle<T> LoadAssetAsync<T>(string assetName, string modTag = Game.BaseModTag)
		{
			//not sure if this will work
			AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(GetAssetLocation(assetName, modTag));
			loadedAssets.Add(handle);
			return handle;
		}

		//Initiates a tracked asset
		public AsyncOperationHandle<GameObject> InstantiateAssetAsync(AssetReference assetRef, Vector3 position, Quaternion facing, Transform parent, string modTag = Game.BaseModTag)
		{
			InstantiationParameters initParams = new InstantiationParameters(position, facing, parent);
			AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(GetAssetLocation(assetRef, modTag), initParams);
			loadedAssets.Add(handle);
			return handle;
		}

		//Unloads ALL tracked assets. Use caution!
		public void UnloadAllAssets()
		{
			foreach (AsyncOperationHandle handle in loadedAssets)
			{
				if (handle.Result != null)
				{
					Addressables.ReleaseInstance(handle);
				}
			}
			loadedAssets.Clear();
		}
		//gets a laded mod by its tag
		public ModInfo GetModByTag(string modTag)
		{
			try
			{
				return loadedMods[modTagLookup[modTag]];
			}
			catch (System.Exception)
			{
				throw new System.Exception(modTag + " tag could not be found in loaded mods!");
			}
		}
		//gets a laded mod by its name
		public ModInfo GetModByName(string modName)
		{
			try
			{
				return loadedMods[modNameLookup[modName]];
			}
			catch (System.Exception)
			{
				throw new System.Exception(modName + " could not be found in loaded mods!");
			}
		}

		//gets the locations of all assets with a label
		public IList<IResourceLocation> FindAssetsWithLabel(string label)
		{
			//An IResourceLocation "contains enough information to load an asset (what/where/how/dependencies)" (Unity Docs)
			List<IResourceLocation> locations = new List<IResourceLocation>();
			foreach (var mod in loadedMods)
			{
				IList<IResourceLocation> locsOutput;
				//Use the IResourceLocator.Locate function to find IResourceLocation
				if (mod.locator.Locate(label, typeof(object), out locsOutput))
				{
					locations.AddRange(locsOutput);
				}
			}
			return locations;
		}
		//gets the locations of all assets in a mod
		public IList<IResourceLocation> FindModAssets(ModInfo mod)
		{
			//An IResourceLocation "contains enough information to load an asset (what/where/how/dependencies)" (Unity Docs)
			IList<IResourceLocation> locsOutput;
			//Use the IResourceLocator.Locate function to find IResourceLocation
			if (mod.locator.Locate(mod.tag, typeof(object), out locsOutput))
			{
				return locsOutput;
			}
			Debug.LogError("The mod could not be found, double check the addressible group settings");
			return new List<IResourceLocation>();
		}

		//setup the lookup data for a loaded mod
		private void SetupModLookupData(ModInfo mod, int modIndex)
		{
			try
			{
				modTagLookup.Add(mod.tag, (short)modIndex);
			}
			catch (System.Exception)
			{
				throw new System.Exception("Mod tag:" + mod.tag + " already registered by another mod!");
			}

			try
			{
				modNameLookup.Add(mod.modName, (short)modIndex);
			}
			catch (System.Exception)
			{
				throw new System.Exception("Mod name:" + mod.tag + " conflicts with already registered mod!");
			}
		}
		//subscribe to the modload event
		internal void RegisterOnModsLoadMethod(ModsEventDelegate onModsLoad)
		{
			onModsLoaded += onModsLoad;
		}
		//unsubscribe to the modload event
		internal void DeregisterOnModsLoadMethod(ModsEventDelegate onModsLoad)
		{
			onModsLoaded -= onModsLoad;
		}
		//subscribe to the modunload event
		internal void RegisterOnModsUnloadMethod(ModsEventDelegate onModsUnload)
		{
			onModsUnloaded += onModsUnload;
		}
		//unsubscribe to the modunload event
		internal void DeregisterOnModsUnloadMethod(ModsEventDelegate onModsUnload)
		{
			onModsUnloaded -= onModsUnload;
		}
		//unload all mods
		public void UnloadMods()
		{
			loadedMods.Clear();
			modTagLookup.Clear();
			modNameLookup.Clear();
			if (onModsUnloaded != null) onModsUnloaded(loadedMods);
			//remove any lingering assetlocation data
			assetRegistry.Clear();
			//remove any lingering assets from memory
			UnloadAllAssets();
		}
		//reload all mods
		public void ReloadMods()
		{
			UnloadMods();
			//TODO: get loaded mods from somewhere
			LoadModMeta(baseMod);
			LoadMods();
		}
		//load a mod's meta data and add it to the mods list
		public void LoadModMeta(ModInfo mod)
		{
			SetupModLookupData(mod, (short)loadedMods.Count);
			loadedMods.Add(mod);
		}
		//load all the mods
		public void LoadMods()
		{
			foreach (var mod in loadedMods)
			{
				//find all tagged mod assets and add them to the asset registry, 
				//overriding if there is a location already present
				foreach (var loc in FindModAssets(mod))
				{
					assetRegistry[mod.tag + "." + loc.PrimaryKey] = loc;
				}
			}
			if (onModsLoaded != null) onModsLoaded(loadedMods);
		}
	}

	//Struct to contain all mod information
	[System.Serializable]
	public struct ModInfo
	{
		public string modName; //Name of Mod
		public string modAbsolutePath; //Absolute Path of Mod
		public string tag;//mod asset tag, used to filter for mod asset locations
		public FileInfo modFile; //File Information (file size etc.)

		public IResourceLocator locator; //Resource locator of the mod
		private bool _isDefault;

		internal ModInfo(IResourceLocator newLocator)
		{
			modName = Game.BaseModName;
			tag = Game.BaseModTag;
			locator = newLocator;
			modAbsolutePath = "";
			modFile = null;
			_isDefault = true;
		}
		public bool isDefault { get => _isDefault; } //Only the default mod pack will have this checked
	}


	class modLocationEqualityComparaer : IEqualityComparer<IResourceLocation>
	{

		public bool Equals(IResourceLocation x, IResourceLocation y)
		{
			//Check whether the compared objects reference the same data.
			if (Object.ReferenceEquals(x, y)) return true;
			return
			x.PrimaryKey == y.PrimaryKey &&
			 x.ResourceType == y.ResourceType &&
			 x.ProviderId == y.PrimaryKey;
		}

		public int GetHashCode(IResourceLocation obj)
		{
			return obj.PrimaryKey.GetHashCode() * obj.InternalId.GetHashCode() * obj.ProviderId.GetHashCode();
		}
	}
}