using System.Diagnostics;
using System.IO;
using Dependencies;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

public static partial class Game
{
    internal static void Initialize()
    {
        var stopwatch = Stopwatch.StartNew();

        RegisterScriptableServices();
        IoC.Add(typeof(Game).Assembly);
        IoC.Initialize();

        Debug.Log($"Ran initialize in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
    }

    internal static void Exiting()
    {
    }

    public static void ExitProgram()
    {
        Application.Quit();
    }

    private static void RegisterScriptableServices()
    {
        var assets = Directory.GetFiles("Assets/DGF/Services", "*.asset");
        foreach (var asset in assets)
        {
            var path = asset.Replace('\\', '/');
            Addressables.LoadAssetAsync<ScriptableService>(path).Completed += service => IoC.Add(service.Result);
        }
    }
}