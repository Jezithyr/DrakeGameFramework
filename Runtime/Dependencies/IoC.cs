using System;
using System.Collections.Concurrent;
using System.Reflection;
using Helpers;
using UnityEngine;

namespace Dependencies
{
    public static class IoC
    {
        private static readonly ConcurrentDictionary<Type, IService> Services = new();

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

        public static T? Get<T>() where T : IService
        {
            return (T?) Get(typeof(T));
        }

        public static object? Get(Type type)
        {
            return Services[type];
        }
    }
}