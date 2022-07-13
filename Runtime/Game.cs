using Dependencies;
using UnityEngine;

public static partial class Game
{
	internal static void Initialize()
	{
		IoC.Add(typeof(Game).Assembly);
	}

	internal static void Exiting()
	{
	}

	public static void ExitProgram()
	{
		Application.Quit();
	}
}