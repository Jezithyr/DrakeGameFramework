
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;

namespace DrakeFramework.Core
{
    public class ServiceManager
    {
		private Dictionary<System.Type,Service> serviceTypeLookup = new Dictionary<System.Type, Service>();
		private List<Service> services = new List<Service>();
		public IReadOnlyList<Service> Services {get => services;}

		internal ServiceManager(System.Type[] startingServiceTypes)
		{
			foreach (var serviceType in startingServiceTypes)
			{
				Service service = ReflectHelper.CreateInstance<Service>(serviceType);
				serviceTypeLookup.Add(service.GetType(), service);
				InsertServiceSorted(service);
				TryRegisterTickingService(service);
			}
		}

		internal ServiceManager(params Service[] startingServices)
		{
			foreach (var service in startingServices)
			{
				serviceTypeLookup.Add(service.GetType(), service);
				InsertServiceSorted(service);
				TryRegisterTickingService(service);
			}
		}

		public bool HasService(System.Type type)
		{
			return serviceTypeLookup.ContainsKey(type);
		}

		public void KillService(System.Type type)
		{
			if (serviceTypeLookup.ContainsKey(type))
			{
				serviceTypeLookup[type].internal_Dispose();
			}
		}

		public T GetService<T>() where T:Service
		{
			return (T)serviceTypeLookup[typeof(T)];
		}
		public Service GetServiceByType(System.Type type)
		{
			return serviceTypeLookup[type];
		}

		public void InitializeServices()
		{
			foreach (var service in services)
			{
				service.internal_Initialize();
			}
		}

		public void DisposeServices()
		{
			foreach (var service in services)
			{
				TryDeRegisterTickingService(service);
				service.internal_Dispose();
			}
		}

		public void ResetServices()
		{
			foreach (var service in services)
			{
				service.internal_Reset();
			}
		}

		~ServiceManager(){
			DisposeServices();
			serviceTypeLookup.Clear();
			services.Clear();
		}

		internal void InsertServiceSorted(Service service)
		{
			int index = services.FindLastIndex(e => e.Priority < service.Priority);
            	if (index == 0 || index == -1)
            	{
                	services.Insert(0, service);
                	return;
            	}
            	services.Insert(index + 1, service);	
		}

		internal void AddNewService<T>() where T:Service, new()
		{
			if (serviceTypeLookup.ContainsKey(typeof(T))) return; //don't add duplicates
				T service = new T();
				serviceTypeLookup.Add(service.GetType(), service);
				InsertServiceSorted(service);
				service.internal_Initialize();
				TryRegisterTickingService(service);
		}

		private void TryDeRegisterTickingService(Service service)
		{
			ITickable tickableService = service as ITickable;
				if (tickableService != null)
				{
					Debug.Log("Removing Update for service:" +service.ToString());
					Game.Ticking.RemoveUpdateEvent(tickableService.OnUpdate);
				} else 
				{
					IFixedTickable fixedTickableService = service as IFixedTickable;
					if (fixedTickableService != null)
					{
						Debug.Log("Removing FixedUpdate for service:" +service.ToString());
						Game.Ticking.RemoveFixedUpdateEvent(fixedTickableService.UpdateRate, fixedTickableService.OnUpdate);
					}
				}
		}
		private void TryRegisterTickingService(Service service)
		{
			ITickable tickableService = service as ITickable;
				if (tickableService != null)
				{
					Debug.Log("Registering Update for service:" +service.ToString());
					Game.Ticking.RegisterUpdateEvent(tickableService.OnUpdate);
				} else 
				{
					IFixedTickable fixedTickableService = service as IFixedTickable;
					if (fixedTickableService != null)
					{
						Debug.Log("Registering FixedUpdate for service:" +service.ToString());
						Game.Ticking.RegisterFixedUpdateEvent(fixedTickableService.UpdateRate, fixedTickableService.OnUpdate, fixedTickableService.CanBeMultiplied, fixedTickableService.MaxTimestep);
					}
				}
		}
    }
}
