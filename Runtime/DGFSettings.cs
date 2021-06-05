using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;
namespace DrakeFramework.Core
{
	//TODO: rewrite when settings system is properly made


	public class DGFSettings : ScriptableObject
	{

#if UNITY_EDITOR
	public string BaseModName {
   		get { return baseModName; }
   		set { baseModName = value; }
	}
	public string BaseModTag {
   		get { return baseModTag; }
   		set { baseModTag = value; }
	}
#endif
		internal string baseModName = "BaseMod";
		internal string baseModTag = "BASE";
		internal void SetBaseModData()
		{
			Game.baseModName = baseModName;
			Game.baseModTag = baseModTag;
		}
	}
}