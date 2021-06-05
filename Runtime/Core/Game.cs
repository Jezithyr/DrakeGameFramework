using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;

namespace DrakeFramework{
	public static partial class Game
	{
		//TODO: move these to a unity menu
		internal static string baseModTag = "BASE";
		public static string BaseModTag => baseModTag;
		internal static string baseModName = "BaseMod";
		public static string BaseModName => baseModName;
		public static ModInfo BaseMod => content.BaseMod;
		public static IReadOnlyList<Service> GameServices { get => gameServiceManager.Services; } //! If this throws an error, something is very wrong
		private static ServiceManager gameServiceManager;
		private static ServiceManager transientServiceManager;
		private static SessionManager sessionManager;
		private static ModuleManager moduleManager;
		private static ContentManager content;
		private static TickingManager ticking;
		public static TickingManager Ticking => ticking;
		public static ContentManager Content => content;
		private static UiManager ui;
		public static UiManager Ui => ui;
		internal static void Initialize()
		{
			ticking = new TickingManager();//This MUST load first
			moduleManager = new ModuleManager(); //This MUST load second
												 //Gets all classes inheriting game server, instanstiates them and injects them into the gameService manager
			content = new ContentManager(); //must load second
			transientServiceManager = new ServiceManager();
			gameServiceManager = new ServiceManager(ReflectHelper.GetSubclassesInAllAssemblies(typeof(GameService)));
			sessionManager = new SessionManager(moduleManager);
			ui = new UiManager(); //must load after content manager
			moduleManager.LoadModules();//this must be last
		}
		internal static void Exiting()
		{
			gameServiceManager = null;
			transientServiceManager = null;
			content = null;
			ticking = null;
			sessionManager = null;
			moduleManager = null;
		}
		public static void ExitProgram()
		{
			Application.Quit();
		}
		public static void RegisterOnSessionStartEvent(Session.SessionEventDelegate callback)
		{
			sessionManager.RegisterOnSessionStartEvent(callback);
		}
		public static void DeRegisterOnSessionStartEvent(Session.SessionEventDelegate callback)
		{
			sessionManager.DeRegisterOnSessionStartEvent(callback);
		}
		public static void RegisterOnSessionEndEvent(Session.SessionEventDelegate callback)
		{
			sessionManager.RegisterOnSessionEndEvent(callback);
		}
		public static void DeRegisterOnSessionEndEvent(Session.SessionEventDelegate callback)
		{
			sessionManager.DeRegisterOnSessionEndEvent(callback);
		}
		public static void CreateSession()
		{
			sessionManager.CreateSession();
		}
		public static void StartSession()
		{
			sessionManager.StartSession();
		}
		public static void EndSession()
		{
			sessionManager.EndSession();
		}
		public static T CreateTransientService<T>() where T:TransientService
		{
			return transientServiceManager.GetService<T>();
		}

		public static void KillTransientService<T>() where T:TransientService
		{
			transientServiceManager.KillService(typeof(T));
		}

		public static T GetTransientService<T>() where T:TransientService
		{
			if (transientServiceManager.HasService(typeof(T))){
				return transientServiceManager.GetService<T>();
			}
			return null;
		}

		public static bool HasTransientService<T>() where T:TransientService
		{
			return transientServiceManager.HasService(typeof(T));
		}

		public static bool TryGetTransientService<T>(out T service) where T:TransientService
		{
			if (transientServiceManager.HasService(typeof(T))){
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
		public static void SetSessionClass<T>() where T : Session
		{
			if (typeof(T).IsAbstract)
			{
				throw new System.Exception("Session class cannot be abstract!");
			}
			sessionManager.sessionClassname = typeof(T);
		}
	}
}