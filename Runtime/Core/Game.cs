using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
using DrakeFramework.Entities;
namespace DrakeFramework
{
	public static partial class Game
	{
		//TODO: move these to a unity menu
		internal static string baseModTag = "BASE";
		public static string BaseModTag => baseModTag;
		internal static string baseModName = "BaseMod";
		public static string BaseModName => baseModName;
		public static ModInfo BaseMod => content.BaseMod;
		public static Session Session => sessionManager.ActiveSession;
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
			InitializeServiceManagers();
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
			KillECSManager();
		}
		static partial void KillECSManager();
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