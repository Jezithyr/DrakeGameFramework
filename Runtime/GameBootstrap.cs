using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("DrakeGameFrameworkEditor")]

public static class GameBootstrap
{
    private const string DrakeSettingsFolder = "DGFSettings";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Bootstrap()
    {
        Debug.Log("Starting FCR");
        Application.quitting += Game.Exiting;
        //loading settings:
        Game.Initialize();
    }
}