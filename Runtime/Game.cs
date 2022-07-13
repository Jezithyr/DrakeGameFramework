using System.Collections.Generic;
using Dependencies;
using Sirenix.OdinInspector.Editor.Modules;
using Ticking;
using UnityEngine;

public static partial class Game
{
	public static TickingManager Ticking { get; private set; }

	internal static void Initialize()
	{
		Ticking = new TickingManager();
	}

	internal static void Exiting()
	{
		Ticking = null;
	}

	public static void ExitProgram()
	{
		Application.Quit();
	}
}