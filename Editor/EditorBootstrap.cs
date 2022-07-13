using System;
using System.IO;
using Dependencies;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [InitializeOnLoad]
    public static class EditorBootstrap
    {
        static EditorBootstrap()
        {
            if (!AssetDatabase.IsValidFolder("Assets"))
            {
                AssetDatabase.CreateFolder("Assets", "DGF");
            }

            if (!AssetDatabase.IsValidFolder("Assets/DGF"))
            {
                AssetDatabase.CreateFolder("Assets/DGF", "Services");
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(ScriptableService).IsAssignableFrom(type) ||
                        type.IsAbstract)
                    {
                        continue;
                    }

                    if (File.Exists($"Assets/DGF/Services/{type.Name}.asset"))
                    {
                        continue;
                    }

                    var obj = (ScriptableService) ScriptableObject.CreateInstance(type);

                    AssetDatabase.CreateAsset(obj, $"Assets/DGF/Services/{type.Name}.asset");
                }
            }
        }
    }
}