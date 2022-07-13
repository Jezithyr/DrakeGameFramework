using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using Helpers;

namespace Dependencies
{
    public static class IoC
    {
        private static readonly ConcurrentDictionary<Type, object> Services = new();

        public static void Add<T>() where T : new()
        {
            Add(new T());
        }

        public static void Add(object service)
        {
            var type = service.GetType();
            if (!Services.TryAdd(type, service))
            {
                throw new ArgumentException($"Service {type} is already added");
            }
        }

        // TODO use IL
        public static T Instantiate<T>()
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance<T>();

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

            return obj;
        }

        public static T? Get<T>()
        {
            return (T?) Get(typeof(T));
        }

        public static object? Get(Type type)
        {
            return Services[type];
        }
    }
}