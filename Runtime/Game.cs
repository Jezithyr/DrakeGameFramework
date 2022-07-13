using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Dependencies;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

public static partial class Game
{
    internal static async Task Initialize()
    {
        var stopwatch = Stopwatch.StartNew();

        await RegisterScriptableServices();
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

    private static async Task RegisterScriptableServices()
    {
        var assets = Directory.GetFiles("Assets/DGF/Services", "*.asset");
        foreach (var asset in assets)
        {
            var path = asset.Replace('\\', '/');
            var service = await Addressables.LoadAssetAsync<ScriptableService>(path).Task;
            IoC.Add(service);
        }
    }
}