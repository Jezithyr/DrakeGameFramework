using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
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

        public static void Initialize()
        {
            var postInject = new List<IPostInject>();
            foreach (var (type, service) in Services)
            {
                Inject(service);

                if (typeof(IPostInject).IsAssignableFrom(type))
                {
                    postInject.Add((IPostInject) service);
                }
            }

            foreach (var service in postInject)
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