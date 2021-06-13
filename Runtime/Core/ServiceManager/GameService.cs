using DrakeFramework.Core;
using UnityEngine;

namespace DrakeFramework
{
	public abstract class GameServiceWrapper<T>:DGFScriptableRef where T:GameService, new()
	{
		public T Service => Game.GetService<T>();
	}


	//abstract baseclass for all game based services
	public abstract class GameService : Service
	{

	}
}
