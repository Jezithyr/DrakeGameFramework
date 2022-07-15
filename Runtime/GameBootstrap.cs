using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("DrakeGameFrameworkEditor")]

namespace DGF
{
    public static class GameBootstrap
    {
        private const string DrakeSettingsFolder = "DGFSettings";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Bootstrap()
        {
            Debug.Log("Starting DGF");
            Application.quitting += Game.Exiting;
            //loading settings:
            Game.Initialize();
        }
    }
}