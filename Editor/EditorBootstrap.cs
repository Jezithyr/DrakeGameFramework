using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using DGF.Dependencies;
using DGF.Helpers;
using DGF.Reflection;
using DGF.Sessions;
using DGFEditor.Editor.Helpers;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace DGFEditor.Editor
{
    [InitializeOnLoad]
    public static class EditorBootstrap
    {
        private static readonly Dictionary<Type, Dictionary<MemberInfo, object?>> ResettableData = new();

        static EditorBootstrap()
        {
            Initialize();
        }

        private static void Initialize()
        {
            var stopwatch = Stopwatch.StartNew();

            Application.quitting += ResetData;

            CacheResettable();
            UpdateServiceAssets();
            UpdateSessionAssets();

            Debug.Log($"Ran {nameof(EditorBootstrap)} in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
        }

        private static void CacheResettable()
        {
            foreach (var type in ReflectionService.GetAllDerivedTypes<ScriptableService>())
            {
                if (type.IsAbstract || !type.HasAttribute<ResetStateOnExitPlayModeAttribute>())
                {
                    continue;
                }

                var asset = AssetDatabase.LoadAssetAtPath($"{IoC.ServicesAssetPath}/{type.Name}.asset", type);
                var data = new Dictionary<MemberInfo, object?>();
                ResettableData.Add(type, data);

                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                foreach (var member in type.GetFieldsAndProperties(flags))
                {
                    if (member.DeclaringType == typeof(Object))
                    {
                        continue;
                    }

                    var value = member.Value(asset);
                    data.Add(member, value);
                }
            }
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

        private static void ResetData()
        {
            foreach (var (type, data) in ResettableData)
            {
                var asset = AssetDatabase.LoadAssetAtPath($"{IoC.ServicesAssetPath}/{type.Name}.asset", type);

                foreach (var (member, value) in data)
                {
                    member.TrySetValue(asset, value);
                }
            }
        }
    }
}