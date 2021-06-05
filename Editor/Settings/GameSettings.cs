using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;
namespace DrakeFramework.Editor
{
	public class GameSettings : ScriptableObject
	{
		internal string baseModName = "BaseMod";
		internal string baseModTag = "BASE";
		private void Awake()
		{
			Game.baseModName = baseModName;
			Game.baseModTag = baseModTag;
		}
	}
}