
using System.Collections.Generic;


namespace DrakeFramework
{
    public class ServiceManager
    {
		private Dictionary<System.Type,Service> serviceTypeLookup = new Dictionary<System.Type, Service>();
		private List<Service> services = new List<Service>();
		public IReadOnlyList<Service> Services {get => services;}

		internal ServiceManager(params System.Type[] startingServiceTypes)
		{
			foreach (var serviceType in startingServiceTypes)
			{
				Service service = ReflectHelper.CreateInstance<Service>(serviceType);
				serviceTypeLookup.Add(service.GetType(), service);
				InsertServiceSorted(service);
			}
		}

		internal ServiceManager(params Service[] startingServices)
		{
			foreach (var service in startingServices)
			{
				serviceTypeLookup.Add(service.GetType(), service);
				InsertServiceSorted(service);
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

		private void InsertServiceSorted(Service service)
		{
			int index = services.FindLastIndex(e => e.Priority < service.Priority);
            	if (index == 0 || index == -1)
            	{
                	services.Insert(0, service);
                	return;
            	}
            	services.Insert(index + 1, service);	
		}
    }
}
