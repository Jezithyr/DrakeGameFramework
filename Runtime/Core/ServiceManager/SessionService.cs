using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
namespace DrakeFramework
{	
	public abstract class SessionServiceWrapper<T> : DGFScriptableRef where T : SessionService, new()
	{
		public T Service => Game.Session.GetService<T>();
	}


	//Abstract baseclass for all session based services
	public abstract class SessionService : Service
	{
		private Session session;
		protected Session Session { get => session; }
		protected virtual void OnSessionStart(Session session){
		}
		protected virtual void OnSessionEnd(Session session){
		}
		internal void internal_OnSessionStart(Session session){
			OnSessionStart(session);
		}
		internal void internal_OnSessionEnd(Session session){
			OnSessionEnd(session);
		}
		internal void SetSession(Session newSession)
		{
			session = newSession;
		}

	}
}
