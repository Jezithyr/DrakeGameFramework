using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Dependencies;
using Editor.Helpers;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor
{
    [InitializeOnLoad]
    public static class EditorBootstrap
    {
        private const string ServicesPath = "Assets/DGF/Services";

        static EditorBootstrap()
        {
            var stopwatch = Stopwatch.StartNew();
            EditorAddressableHelpers.CreateAllFolders(ServicesPath);

            var assetFileNames = new HashSet<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(ScriptableService).IsAssignableFrom(type) ||
                        type.IsAbstract)
                    {
                        continue;
                    }

                    var fileName = $"{type.Name}.asset";
                    assetFileNames.Add(fileName);

                    var assetPath = $"{ServicesPath}/{fileName}";
                    if (File.Exists(assetPath))
                    {
                        continue;
                    }

                    var obj = (ScriptableService) ScriptableObject.CreateInstance(type);
                    AssetDatabase.CreateAsset(obj, assetPath);
                    EditorAddressableHelpers.SetAddressable(assetPath);
                }
            }

            foreach (var file in Directory.EnumerateFiles(ServicesPath, "*.asset"))
            {
                if (!assetFileNames.Contains(Path.GetFileName(file)))
                {
                    AssetDatabase.DeleteAsset(file);
                }
            }

            Debug.Log($"Ran {nameof(EditorBootstrap)} in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
        }
    }
}