using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;
using DrakeFramework.Core;
using Unity.Entities;

namespace DrakeFramework.Entities
{
	public class ECSServiceManager
	{
		private Dictionary<string, ECSService> ECSServiceWorldLookup = new Dictionary<string, ECSService>();
		private List<ECSService> services = new List<ECSService>();
		public IReadOnlyList<ECSService> Services { get => services; }

		internal ECSServiceManager()
		{
		}

		public bool HasWorld(string name)
		{
			return ECSServiceWorldLookup.ContainsKey(name);
		}

		public void DisposeWorld(string name)
		{
			if (ECSServiceWorldLookup.ContainsKey(name))
			{
				ECSServiceWorldLookup[name].internal_Dispose();

			}
		}

		public EntityManager GetEntityManager(string world)
		{
			return GetWorld(world).Manager;
		}

		public ECSService GetWorld(string world)
		{
			return ECSServiceWorldLookup[world];
		}

		private void InitializeWorlds()
		{
			foreach (var service in services)
			{
				service.internal_Initialize();
			}
		}

		public void DisposeWorlds()
		{
			foreach (var service in services)
			{
				DeRegisterTickingService(service);
				service.internal_Dispose();
			}
		}

		public void ReloadAllWorlds()
		{
			foreach (var service in services)
			{
				service.internal_Reset();
			}
		}
		public void ResetWorld(string Name)
		{
			GetWorld(Name).internal_Reset();
		}

		//Registers all the implementations of a subsystem base class to the specified world
		public void RegisterAllSystemsFromBase<T>(string world) where T : SystemBase
		{
			ECSService service = GetWorld(world);
			System.Type[] types = ReflectHelper.GetSubclassesInAllAssemblies(typeof(T));
			var methodInfo = typeof(ECSServiceManager).GetMethod("RegisterNewSystem");
			foreach (var type in types)
			{
				methodInfo.MakeGenericMethod(type).Invoke(this, new object[] { world });
			}
		}
		public void RegisterNewSystem<T>(string world) where T : SystemBase, new()
		{
			ECSService service = GetWorld(world);
			service.World.AddSystem<T>(new T());
		}

		public T GetOrCreateSystem<T>(string world) where T : SystemBase
		{
			ECSService service = GetWorld(world);
			return service.World.GetOrCreateSystem<T>();
		}

		public T GetExistingSystem<T>(string world) where T : SystemBase
		{
			ECSService service = GetWorld(world);
			return service.World.GetExistingSystem<T>();
		}


		~ECSServiceManager()
		{
			DisposeWorlds();
			ECSServiceWorldLookup.Clear();
			services.Clear();
		}

		internal ECSService InsertWorldSorted(ECSService service)
		{
			int index = services.FindLastIndex(e => e.Priority < service.Priority);
			if (index == 0 || index == -1)
			{
				services.Insert(0, service);
				return service;
			}
			services.Insert(index + 1, service);
			return service;
		}

		public ECSService CreateWorld(string world, int priority, WorldFlags flags)
		{
			if (ECSServiceWorldLookup.ContainsKey(world)) return ECSServiceWorldLookup[world]; //don't add duplicates
			ECSService service = new ECSService(world, priority, flags);
			ECSServiceWorldLookup.Add(world, service);
			InsertWorldSorted(service);
			service.internal_Initialize();
			RegisterTickingService(service);
			return service;
		}

		private void DeRegisterTickingService(Service service)
		{
			ITickable tickableService = service as ITickable;
			if (tickableService != null)
			{
				Debug.Log("Removing Update for service:" + service.ToString());
				Game.Ticking.RemoveUpdateEvent(tickableService.OnUpdate);
			}
		}
		private void RegisterTickingService(Service service)
		{
			ITickable tickableService = service as ITickable;
			if (tickableService != null)
			{
				Debug.Log("Registering Update for service:" + service.ToString());
				Game.Ticking.RegisterUpdateEvent(tickableService.OnUpdate);
			}
		}
	}
}
