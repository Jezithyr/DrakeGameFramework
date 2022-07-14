using System.Diagnostics;
using System.Threading.Tasks;
using Dependencies;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class Game
{
    internal static async Task Initialize()
    {
        var stopwatch = Stopwatch.StartNew();

        await IoC.RegisterAllInitialize();

        Debug.Log($"Ran initialize in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
    }

    internal static void Exiting()
    {
        IoC.Shutdown();
    }

    public static void ExitProgram()
    {
        Application.Quit();
    }
}