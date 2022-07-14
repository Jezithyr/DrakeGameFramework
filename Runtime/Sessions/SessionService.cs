using System;
using System.Collections.Generic;
using System.IO;
using Dependencies;
using Ticking;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sessions
{
    public class SessionService : ScriptableService
    {

        public static readonly string SessionAssetPath = "Assets/DGF/Sessions";
        
        private Session activeSession;
        private Dictionary<Type, Session> sessionLookup = new();
        
        [SerializeField]
        private Session initialSession;
        
        public Session InitialSession => initialSession;
        
        public override void Initialize()
        {
            //initialize the dummy default session if there is no session specified: 
            if (initialSession != null)
            {
                activeSession = initialSession;
                initialSession.Enter();
                return;
            }
            var newSession = CreateInstance<DefaultSession>(); //This may cause issues with GC?
            RegisterNewSessionType(typeof(DefaultSession), activeSession);
            activeSession = newSession;
            activeSession.Enter();
        }

        public override void Update()
        {
            activeSession.Update();
        }
        
        public override void FixedUpdate()
        {
            activeSession.FixedUpdate();
        }
        internal void RegisterNewSessionType(Type type, Session sessionInstance)
        {
            sessionLookup.Add(type, sessionInstance);
        }
        public T GetSession<T>() where T : Session, new()
        {
            return (T)sessionLookup[typeof(T)];
        }
        public void ReturnToInitialSession()
        {
            SwitchSession(InitialSession.GetType());
        }
        public void CleanSessionData<T>() where T : Session, new()
        {
            sessionLookup[typeof(T)].Cleanup();
        }
        public void CleanActiveSession()
        {
            activeSession.Cleanup();
        }
        public void SwitchSession(Type sessionType)
        {
            if (typeof(Session).IsAssignableFrom(sessionType)) throw new Exception($"{sessionType} Does not inherit from Session!");
            if (sessionType.IsAbstract) throw new Exception($"SessionType {sessionType} is abstract!");
            activeSession.Exit();
            activeSession = sessionLookup[sessionType];
            activeSession.Enter();
        }
        public void SwitchSession<T>() where T : Session, new()
        {
            activeSession.Exit();
            activeSession = sessionLookup[typeof(T)];
            activeSession.Enter();
        }
        public override void Shutdown()
        {
            activeSession.Exit();
            foreach (var (sessionType,sessionInstance) in sessionLookup)
            {
                sessionInstance.Shutdown();
            }
        }
    }
}
