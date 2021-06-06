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
		internal Dictionary<System.Type, SessionData> managedSessionData = new Dictionary<System.Type, SessionData>();
		public Session()
		{
			sessionServiceManager = new ServiceManager(ReflectHelper.GetSubclassesInAllAssemblies(typeof(SessionService)));
			CreateManagedSessionData();
			InitializeSession();
		}

		private void CreateManagedSessionData()
		{
			System.Type[] sessionDataTypes = ReflectHelper.GetSubclassesInAllAssemblies(typeof(SessionData));
			foreach (var sessionDataType in sessionDataTypes)
			{
				SessionData newData =  ReflectHelper.CreateInstance<SessionData>(sessionDataType);
				newData.internal_OnCreate();
				managedSessionData.Add(sessionDataType,newData);
			}
		}

		public T GetManagedData<T>() where T:SessionData, new()
		{
			return (T)managedSessionData[typeof(T)];
		}
		public T GetService<T>() where T : SessionService, new()
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