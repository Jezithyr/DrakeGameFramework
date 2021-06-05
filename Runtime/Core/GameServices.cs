using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
using DrakeFramework.Entities;
namespace DrakeFramework
{
	public static partial class Game
	{
		public static IReadOnlyList<Service> GameServices { get => gameServiceManager.Services; } //! If this throws an error, something is very wrong
		public static IReadOnlyList<Service> TransientServices { get => transientServiceManager.Services; } //! If this throws an error, something is very wrong
		private static ServiceManager gameServiceManager;
		private static ServiceManager transientServiceManager;

		private static void InitializeServiceManagers()
		{
			gameServiceManager = new ServiceManager(ReflectHelper.GetSubclassesInAllAssemblies(typeof(GameService)));
			transientServiceManager = new ServiceManager();
			InitializeECSServices(); //this may cause issues if ECS is disabled?
		}
		static partial void InitializeECSServices();
		public static T CreateTransientService<T>() where T : TransientService
		{
			return transientServiceManager.GetService<T>();
		}

		public static void KillTransientService<T>() where T : TransientService
		{
			transientServiceManager.KillService(typeof(T));
		}

		public static T GetTransientService<T>() where T : TransientService
		{
			if (transientServiceManager.HasService(typeof(T)))
			{
				return transientServiceManager.GetService<T>();
			}
			return null;
		}

		public static bool HasTransientService<T>() where T : TransientService
		{
			return transientServiceManager.HasService(typeof(T));
		}

		public static bool TryGetTransientService<T>(out T service) where T : TransientService
		{
			if (transientServiceManager.HasService(typeof(T)))
			{
				service = transientServiceManager.GetService<T>();
				return true;
			}
			service = null;
			return false;
		}

		public static T GetService<T>() where T : GameService
		{
			return gameServiceManager.GetService<T>();
		}

	}
}
