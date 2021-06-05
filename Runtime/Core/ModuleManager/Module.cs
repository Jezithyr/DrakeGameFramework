using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrakeFramework
{
	public abstract class Module
	{
		public abstract int Priority { get; }
		public abstract string Name { get; }
		internal void internal_Initialize()
		{
			Initialize();
		}
		internal void internal_Unload()
		{
			Unload();
		}
		internal void internal_Load()
		{
			Load();
		}
		internal void internal_SessionStart(Session session)
		{
			OnSessionStart(session);
		}
		internal void internal_SessionEnd(Session session)
		{
			OnSessionEnd(session);
		}
		protected virtual void Load() { }
		protected virtual void Unload() { }
		protected abstract void Initialize();
		protected abstract void OnSessionStart(Session session);
		protected abstract void OnSessionEnd(Session session);
	}
}