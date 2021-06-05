using System;
using UnityEngine;
using DrakeFramework;
namespace DrakeFramework.Core
{
	internal class SessionManager
	{
		internal System.Type sessionClassname = null;
		public System.Type SessionClass { get => sessionClassname; }
		private Session activeSession = null;
		public Session ActiveSession { get => activeSession; }
		private Action onSessionInit;
		private Session.SessionEventDelegate onSessionStart;
		private Session.SessionEventDelegate onSessionEnd;
		internal SessionManager(ModuleManager moduleManager)
		{
			onSessionInit = moduleManager.OnSessionInit;
		}
		internal void RegisterOnSessionStartEvent(Session.SessionEventDelegate callback)
		{
			onSessionStart +=callback;
		}

		internal void DeRegisterOnSessionStartEvent(Session.SessionEventDelegate callback)
		{
			onSessionStart -=callback;
		}

		internal void RegisterOnSessionEndEvent(Session.SessionEventDelegate callback)
		{
			onSessionEnd +=callback;
		}

		internal void DeRegisterOnSessionEndEvent(Session.SessionEventDelegate callback)
		{
			onSessionEnd -=callback;
		}

		public void CreateSession()
		{
			if (sessionClassname == null)
			{
				throw new System.Exception("Invalid Session Class!");
			}
			activeSession = ReflectHelper.CreateInstance<Session>(sessionClassname);
			onSessionInit();
		}

		public void StartSession()
		{
			activeSession.internal_SessionStart();
			if (onSessionStart != null)
			{
				onSessionStart(activeSession);
			}
			
		}

		public void EndSession()
		{
			activeSession.internal_SessionEnd();
			if (onSessionEnd != null)
			{
				onSessionEnd(activeSession);
			}
			activeSession = null;
		}

		private void TryDeRegisterTickingSession(Session session)
		{
			ITickable tickableSession = session as ITickable;
				if (tickableSession != null)
				{
					Debug.Log("Removing Update for service:" +session.ToString());
					Game.Ticking.RemoveUpdateEvent(tickableSession.OnUpdate);
				} else 
				{
					IFixedTickable fixedTickableSession = session as IFixedTickable;
					if (fixedTickableSession != null)
					{
						Debug.Log("Removing FixedUpdate for service:" +session.ToString());
						Game.Ticking.RemoveFixedUpdateEvent(fixedTickableSession.UpdateRate, fixedTickableSession.OnUpdate);
					}
				}
		}
		private void TryRegisterTickingSession(Session session)
		{
			ITickable tickableSession = session as ITickable;
				if (tickableSession != null)
				{
					Debug.Log("Registering Update for service:" +session.ToString());
					Game.Ticking.RegisterUpdateEvent(tickableSession.OnUpdate);
				} else 
				{
					IFixedTickable fixedTickableSession = session as IFixedTickable;
					if (fixedTickableSession != null)
					{
						Debug.Log("Registering FixedUpdate for service:" +session.ToString());
						Game.Ticking.RegisterFixedUpdateEvent(fixedTickableSession.UpdateRate, fixedTickableSession.OnUpdate, fixedTickableSession.CanBeMultiplied, fixedTickableSession.MaxTimestep);
					}
				}
		}

	}
}
