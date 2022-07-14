using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Dependencies;
using Editor.Helpers;
using Reflection;
using Sessions;
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

        private static void Initialize()
        {
            var stopwatch = Stopwatch.StartNew();
            UpdateServiceAssets();
            UpdateSessionAssets();

            Debug.Log($"Ran {nameof(EditorBootstrap)} in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
        }

        private static void UpdateServiceAssets()
        {
            EditorAddressableHelpers.CreateAllFolders(IoC.ServicesAssetPath);

            var assetFileNames = new HashSet<string>();
            foreach (var type in ReflectionService.GetAllDerivedTypes<ScriptableService>())
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                var fileName = $"{type.Name}.asset";
                assetFileNames.Add(fileName);

                var assetPath = $"{IoC.ServicesAssetPath}/{fileName}";
                if (File.Exists(assetPath))
                {
                    EditorAddressableHelpers.SetAddressable(assetPath);
                    continue;
                }

                var obj = (ScriptableService) ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(obj, assetPath);
                EditorAddressableHelpers.SetAddressable(assetPath);
            }

            foreach (var file in Directory.EnumerateFiles(IoC.ServicesAssetPath, "*.asset"))
            {
                if (!assetFileNames.Contains(Path.GetFileName(file)))
                {
                    AssetDatabase.DeleteAsset(file);
                }
            }
        }

        private static void UpdateSessionAssets()
        {
            EditorAddressableHelpers.CreateAllFolders(SessionService.SessionAssetPath);

            var assetFileNames = new HashSet<string>();
            foreach (var type in ReflectionService.GetAllDerivedTypes<Session>())
            {
                if (type.IsAbstract || type == typeof(DefaultSession))
                {
                    continue;
                }

                var fileName = $"{type.Name}.asset";
                assetFileNames.Add(fileName);

                var assetPath = $"{SessionService.SessionAssetPath}/{fileName}";
                if (File.Exists(assetPath))
                {
                    EditorAddressableHelpers.SetAddressable(assetPath);
                    continue;
                }

                var obj = (Session) ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(obj, assetPath);
                EditorAddressableHelpers.SetAddressable(assetPath);
            }

            foreach (var file in Directory.EnumerateFiles(SessionService.SessionAssetPath, "*.asset"))
            {
                if (!assetFileNames.Contains(Path.GetFileName(file)))
                {
                    AssetDatabase.DeleteAsset(file);
                }
            }
        }
    }
}