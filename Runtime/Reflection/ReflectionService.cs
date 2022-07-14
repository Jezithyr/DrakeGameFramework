using System;
using System.Collections.Generic;
using Dependencies;

namespace Reflection
{
    public class ReflectionService : IService
    {
        public IEnumerable<Type> GetDerivedTypes<T>() where T : class
        {
            var baseType = typeof(T);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
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
}