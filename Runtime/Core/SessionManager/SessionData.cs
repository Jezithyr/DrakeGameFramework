using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrakeFramework
{
	public abstract class SessionDataWrapper<T> :DGFScriptableRef where T:SessionData, new()
	{
		public T Data => Game.Session.GetManagedData<T>();
	}

    public abstract class SessionData
    {
		//The order of onCreate is nondeterministic! Don't use it to load dependencies!
		//this is intended for loading scriptable object data or settings/configs!
		protected virtual void OnCreate(){}
		internal void internal_OnCreate()
		{
			OnCreate();
		}
		//add save/load functionality here
    }
}
