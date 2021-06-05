using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
namespace DrakeFramework
{
	public abstract class Session
	{
		public delegate void SessionEventDelegate(Session session);
		public IReadOnlyList<Service> Services { get => sessionServiceManager.Services; }
		private ServiceManager sessionServiceManager;
		public Session()
		{
			sessionServiceManager = new ServiceManager(ReflectHelper.GetSubclassesInAllAssemblies(typeof(SessionService)));
			InitializeSession();
		}

		public T GetService<T>() where T : SessionService
		{
			return sessionServiceManager.GetService<T>();
		}
		//Runs when the session is created (loading screen)
		protected virtual void InitializeSession() { }
		//Run when the session is started (in game)
		protected abstract void StartSession();
		//Run when the player ends the gameplay session
		protected abstract void EndSession();
		//Run when the session instance is destroyed
		protected virtual void DisposeSession() { }
		internal void internal_SessionStart()
		{
			foreach (var service in Services)
			{
				SessionService sessionService = service as SessionService;
				sessionService.internal_OnSessionStart(this);
			}
			StartSession();
		}
		internal void internal_SessionEnd()
		{
			foreach (var service in Services)
			{
				SessionService sessionService = service as SessionService;
				sessionService.internal_OnSessionEnd(this);
			}
			EndSession();
		}
		~Session()
		{
			DisposeSession();
		}
	}

}