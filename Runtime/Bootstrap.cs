using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Drake;

namespace Drake.Core
{
    public static class GameBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		static void Bootstrap()
		{
			Debug.Log("Starting FCR");
			Application.quitting += Game.Exiting;
			Game.Initialize();
		}
    }
}
