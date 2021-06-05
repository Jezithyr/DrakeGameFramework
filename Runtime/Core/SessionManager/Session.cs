using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
namespace DrakeFramework
{
	public abstract class Session
	{
		public delegate void SessionDelegate(Session session);
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
		//Runs before the session is started (loading screen)
		protected virtual void InitializeSession() { }
		//Run when the session is started
		protected abstract void StartSession();
		//Run when the player ends the gameplay session
		protected abstract void EndSession();
		//Run when the session instance is destroyed
		protected virtual void DisposeSession() { }
		internal void internal_SessionStart()
		{
			StartSession();
		}
		internal void internal_SessionEnd()
		{
			EndSession();
		}
		~Session()
		{
			DisposeSession();
		}
	}

}