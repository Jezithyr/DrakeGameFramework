
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

public static class ReflectHelper
{
	//TODO: refactor to use iterators and lambdas
	public static System.Type[] GetAllImplementationsOf(System.Type interfaceType)
	{
		return interfaceType.Assembly.GetTypes().Where(p => interfaceType.IsAssignableFrom(p)).ToArray<System.Type>();
	}

	public static System.Type[] GetSubclasses(System.Type baseClass)
	{
		return baseClass.Assembly.GetTypes().Where(type => type.IsSubclassOf(baseClass)).ToArray<System.Type>();
	}
	public static System.Type[] GetSubclasses(System.Type baseClass,Assembly assembly)
	{
		return assembly.GetTypes().Where(type => type.IsSubclassOf(baseClass)).ToArray<System.Type>();
	}
	public static System.Type[] GetSubclassesInAllAssemblies(System.Type baseClass,bool allowAbstract = false)
	{
  		var result = new List<System.Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
         foreach (var assembly in assemblies)
         {
             var types = assembly.GetTypes();
             foreach (var type in types)
             {
                 if (type.IsSubclassOf(baseClass))
				 	if (allowAbstract || !type.IsAbstract)
					{
						result.Add(type);
					}
                     
             }
         }
         return result.ToArray();
	}

	public static System.Type[] GetAllImplementationsOfInAllAssemblies(System.Type interfaceType, bool includeBase = false)
	{
  		var result = new List<System.Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
         foreach (var assembly in assemblies)
         {
             var types = assembly.GetTypes();
             foreach (var type in types)
             {
                 if (interfaceType.IsAssignableFrom(type))
                    if (type != interfaceType)
					{
						result.Add(type);
					} else if (includeBase)
					{
						result.Add(type);
					}
					
             }
         }
         return result.ToArray();
	}

	public static string[] GetSubclassNames(System.Type baseClass)
	{
		return baseClass.Assembly.GetTypes().Where(type => type.IsSubclassOf(baseClass)).ToArray<System.Type>().Select(o => o.ToString()).ToArray();
	}
	public static T CreateInstance<T>(System.Type type)
	{
		return (T)Activator.CreateInstance(type);
	}

	public static string[] GetSubclassNames(System.Type baseClass,Assembly assembly)
	{
		return assembly.GetTypes().Where(type => type.IsSubclassOf(baseClass)).ToArray<System.Type>().Select(o => o.ToString()).ToArray();
	}

	public static T[] CreateInstancesOfTypes<T>(System.Type[] types)
	{
		T[] output = new T[types.Length];
		for (int i = 0; i < types.Length; i++)
		{
			output[i] = (T)Activator.CreateInstance(types[i]);
		}
		return output;
	}

}
