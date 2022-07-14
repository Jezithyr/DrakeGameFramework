using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dependencies;

namespace Reflection
{
    public class ReflectionService : IService
    {
        public static IEnumerable<Type> GetAllDerivedTypes<T>() where T : class
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetDerivedTypes<T>);
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(Assembly assembly) where T : class
        {
            var baseType = typeof(T);

            foreach (var type in assembly.GetTypes())
            {
                if (baseType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }
    }
}