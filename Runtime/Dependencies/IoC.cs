using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using DGF.Helpers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

namespace DGF.Dependencies
{
    public static class IoC
    {
        public const string ServicesAssetPath = "Assets/DGF/Services";
        private static readonly ConcurrentDictionary<Type, IService> Services = new();
        public static event Action<IService>? ServiceAdded;

        public static void Add<T>() where T : IService, new()
        {
            Add(new T());
        }

        public static void Add(IService service)
        {
            var type = service.GetType();
            if (!Services.TryAdd(type, service))
            {
                throw new ArgumentException($"Service {type} is already added");
            }

            ServiceAdded?.Invoke(service);
        }

        public static void Add(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IService).IsAssignableFrom(type) ||
                    typeof(ScriptableObject).IsAssignableFrom(type) ||
                    type.IsAbstract)
                {
                    continue;
                }

                var service = (IService) Activator.CreateInstance(type);
                Add(service);
            }
        }

        public static IEnumerable<IService> All()
        {
            return Services.Values;
        }

        public static void Initialize()
        {
            foreach (var service in Services.Values)
            {
                Inject(service);
            }

            foreach (var service in Services.Values)
            {
                service.Initialize();
            }
        }

        // TODO use IL
        private static void Inject(object obj)
        {
            var type = obj.GetType();

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!field.HasAttribute<InjectAttribute>())
                {
                    continue;
                }

                var serviceType = field.FieldType;
                var service = Services[serviceType];
                field.SetValue(obj, service);
            }
        }

        public static T Instantiate<T>()
        {
            var obj = Activator.CreateInstance<T>();
            Inject(obj);

            if (obj is IPostInject postInject)
            {
                postInject.Initialize();
            }

            return obj;
        }

        public static T Get<T>() where T : IService
        {
            return (T) Get(typeof(T));
        }

        public static object Get(Type type)
        {
            return Services[type];
        }

        public static async Task RegisterAllInitialize()
        {
            var stopwatch = Stopwatch.StartNew();

            await RegisterScriptableServices();
            Add(typeof(IoC).Assembly);
            Initialize();

            Debug.Log($"Ran {nameof(RegisterAllInitialize)} in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
        }

        public static async Task RegisterScriptableServices()
        {
            var assets = Directory.GetFiles(ServicesAssetPath, "*.asset");
            foreach (var asset in assets)
            {
                var path = asset.Replace('\\', '/');
                var service = await Addressables.LoadAssetAsync<ScriptableService>(path).Task;
                Add(service);
            }
        }

        public static void Shutdown()
        {
            foreach (var service in Services.Values)
            {
                service.Shutdown();
            }
        }
    }
}