using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;
using UnityEditor;

namespace DrakeFramework.Editor
{
	[InitializeOnLoad]
	public class EditorBootstrap
	{
		
		static EditorBootstrap()
		{

			if (EditorPrefs.GetBool("UsingWrappers", false))
			{
				InitWrapperSystem();
			}
			
		}

		private static void InitWrapperSystem()
		{
			EditorAssetUtil.TryCreateFolderPath("DrakeFramework","Resources","DGFObjects");
			TryCreateSoWrappers(typeof(DGFScriptableRef));
		}


		private static void TryCreateSoWrappers(System.Type baseClass)
		{
			foreach (var wrapperType in ReflectHelper.GetSubclassesInAllAssemblies(baseClass, false))
			{
				Debug.Log(wrapperType);
				EditorAssetUtil.CreateOrGetScriptableAtPath(wrapperType.ToString(),wrapperType,"DrakeFramework","Resources","DGFObjects");
			}
		}


	}
}
