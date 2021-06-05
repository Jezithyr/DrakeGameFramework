using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework.Core;
namespace DrakeFramework
{
	//Abstract baseclass for all session based services
	public abstract class SessionService : Service
	{
		private Session session;
		protected Session Session { get => session; }
		internal void SetSession(Session newSession)
		{
			session = newSession;
		}

	}
}
