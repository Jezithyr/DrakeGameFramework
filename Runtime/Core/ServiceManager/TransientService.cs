using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;
using DrakeFramework.Core;

namespace DrakeFramework
{
	public abstract class TransientServiceWrapper<T> : DGFScriptableRef where T : TransientService, new()
	{
		public T Service => Game.GetTransientService<T>();
	}
	public abstract class TransientService : Service
	{
		public override int Priority => 0;
	}
}