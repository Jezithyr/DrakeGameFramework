using System;


namespace FCRCore
{
    public class SessionManager
    {
		internal System.Type sessionClassname = null;
		public System.Type SessionClass {get => sessionClassname;}
		private Session activeSession = null;
		public Session ActiveSession {get=> activeSession;}

		private Action onSessionInit;
		private Session.SessionDelegate onSessionStart;
		private Session.SessionDelegate onSessionEnd;
		internal SessionManager(ModuleManager moduleManager) 
		{
			onSessionInit = moduleManager.OnSessionInit;
		}
		public void CreateSession()
		{
			if (sessionClassname == null) {
				throw new System.Exception("Invalid Session Class!");
			}
			activeSession = ReflectHelper.CreateInstance<Session>(sessionClassname);
			onSessionInit();
		}

		public void StartSession()
		{
			activeSession.internal_SessionStart();
			onSessionStart(activeSession);
		}

		public void EndSession()
		{
			activeSession.internal_SessionEnd();
			onSessionEnd(activeSession);
			activeSession = null;
		}


    }
}
