using System;
using System.Diagnostics;
using System.IO;
using Dependencies;
using Helpers;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor
{
    [InitializeOnLoad]
    public static class EditorBootstrap
    {
        private const string Path = "Assets/DGF/Services";

        static EditorBootstrap()
        {
            var stopwatch = Stopwatch.StartNew();
            AssetHelpers.CreateAllFolders(Path);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(ScriptableService).IsAssignableFrom(type) ||
                        type.IsAbstract)
                    {
                        continue;
                    }

                    var assetPath = $"{Path}/{type.Name}.asset";
                    if (File.Exists(assetPath))
                    {
                        continue;
                    }

                    var obj = (ScriptableService) ScriptableObject.CreateInstance(type);

                    AssetDatabase.CreateAsset(obj, assetPath);
                }
            }

            Debug.Log($"Ran {nameof(EditorBootstrap)} in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
        }
    }
}