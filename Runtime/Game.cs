using System.Diagnostics;
using Dependencies;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Debug = UnityEngine.Debug;

public static partial class Game
{
    internal static void Initialize()
    {
        var stopwatch = Stopwatch.StartNew();

        Addressables.LoadAssetsAsync<ScriptableService>("Assets/DGF/Services/TickingManager.asset", IoC.Add).WaitForCompletion();
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
}