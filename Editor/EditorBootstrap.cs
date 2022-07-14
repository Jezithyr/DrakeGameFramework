using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Dependencies;
using Editor.Helpers;
using Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor
{
    [InitializeOnLoad]
    public static class EditorBootstrap
    {
        static EditorBootstrap()
        {
            Initialize();
        }

        private static async void Initialize()
        {
            var stopwatch = Stopwatch.StartNew();
            await IoC.RegisterAllInitialize();
            EditorAddressableHelpers.CreateAllFolders(IoC.ServicesPath);

            var assetFileNames = new HashSet<string>();
            var reflection = IoC.Get<ReflectionService>();
            foreach (var type in reflection.GetDerivedTypes<ScriptableService>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                var fileName = $"{type.Name}.asset";
                assetFileNames.Add(fileName);

                var assetPath = $"{IoC.ServicesPath}/{fileName}";
                if (File.Exists(assetPath))
                {
                    EditorAddressableHelpers.SetAddressable(assetPath);
                    continue;
                }

                var obj = (ScriptableService) ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(obj, assetPath);
                EditorAddressableHelpers.SetAddressable(assetPath);
            }

            foreach (var file in Directory.EnumerateFiles(IoC.ServicesPath, "*.asset"))
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