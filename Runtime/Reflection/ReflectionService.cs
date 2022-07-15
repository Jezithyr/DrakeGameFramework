using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace DGF.Reflection
{
    public static class ReflectionService
    {
        public static IEnumerable<Type> GetAllDerivedTypes<T>() where T : class
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(GetDerivedTypes<T>);
        }

        public static IEnumerable<Type> GetDerivedTypes<T>(this Assembly assembly) where T : class
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

        public static IEnumerable<MemberInfo> GetFieldsAndProperties(this Type type, BindingFlags flags)
        {
            foreach (var field in type.GetFields(flags))
            {
                yield return field;
            }

            foreach (var property in type.GetProperties(flags))
            {
                yield return property;
            }
        }

        [Pure]
        public static object? Value(this MemberInfo info, object obj)
        {
            return info switch
            {
                FieldInfo field => field.GetValue(obj),
                PropertyInfo property => property.GetValue(obj),
                _ => throw new ArgumentOutOfRangeException(nameof(info))
            };
        }

        public static void TrySetValue(this MemberInfo info, object obj, object? value)
        {
            switch (info)
            {
                case FieldInfo field:
                    field.SetValue(obj, value);
                    break;
                case PropertyInfo property:
                    if (property.GetSetMethod(true) is { } setter)
                    {
                        setter.Invoke(obj, new[] {value});
                    }
                    break;
            }
        }
    }
}