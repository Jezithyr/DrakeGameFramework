using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
using DrakeFramework.Entities;

namespace DrakeFramework
{
	public static partial class Game
	{
		private static ECSServiceManager ecsManager;
		public static ECSServiceManager ECS => ecsManager;
		static partial void InitializeECSServices()
		{
			ecsManager = new ECSServiceManager();
		}
		static partial void KillECSManager()
		{
			ecsManager = null;
		}
	}
}
