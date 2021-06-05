using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DrakeGameFrameworkEditor")]
namespace DrakeFramework.Core
{
    public static class GameBootstrap
    {
		private const string DrakeSettingsFolder = "DGFSettings";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		static void Bootstrap()
		{
			Debug.Log("Starting FCR");
			LoadBaseFrameworkSettings();
			Application.quitting += Game.Exiting;
			//loading settings:
			Game.Initialize();
		}

		private static void LoadBaseFrameworkSettings()
		{
			var settingsData = Resources.Load<ScriptableObject>("DGFSettings/CoreSettings");
		}
    }
}
