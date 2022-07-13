using Dependencies;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static partial class Game
{
    internal static void Initialize()
    {
        Addressables.LoadAssetsAsync<ScriptableService>("Assets/DGF/Services/TickingManager.asset", IoC.Add).WaitForCompletion();
        IoC.Add(typeof(Game).Assembly);
        IoC.Initialize();
    }

    internal static void Exiting()
    {
    }

    public static void ExitProgram()
    {
        Application.Quit();
    }
}