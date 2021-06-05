using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using DrakeFramework.Core;
namespace DrakeFramework
{
	public delegate void OnEngineTickCallback();
	public delegate void OnFixedUpdateCallback(float aDeltaTime);
	public class TickingManager //internal constructor to prevent shinanegans
	{
		internal TickingManager()
		{
			SetupTicking();
		}

		private Dictionary<float, CustomFixedUpdate> FixedUpdateEvents = new Dictionary<float, CustomFixedUpdate>();

		private bool suspended = false;
		public bool Active
		{
			get => !suspended; //update the time multiplier
			set
			{
				suspended = !value;
			}

		}
		private float timeMultiplier = 1.0f;
		public float TimeMultiplier
		{
			get => timeMultiplier; //update the time multiplier
			set
			{
				timeMultiplier = value;
				foreach (var keyPair in FixedUpdateEvents)
				{
					keyPair.Value.TimeMultiplier = value;
				}
			}

		}
		private OnEngineTickCallback OnEngineTickEvent;

		public void RegisterFixedUpdateEvent(float updateRate, OnFixedUpdateCallback callback, bool allowTimeMultiplier = true, float MaxAllowedTimeStep = 0)
		{
			if (FixedUpdateEvents.ContainsKey(updateRate))
			{
				FixedUpdateEvents[updateRate].Callback += callback;
				FixedUpdateEvents[updateRate].MaxAllowedTimeStep = MaxAllowedTimeStep;
			}
			else
			{
				FixedUpdateEvents.Add(updateRate, new CustomFixedUpdate(updateRate, callback, allowTimeMultiplier, MaxAllowedTimeStep));
			}
		}

		public void RemoveFixedUpdateEvent(float updateRate, OnFixedUpdateCallback callback)
		{
			CustomFixedUpdate updateInstance;
			if (FixedUpdateEvents.TryGetValue(updateRate, out updateInstance))
			{
				//This may cause issues
				updateInstance.Callback -= callback;
			}
		}

		internal void RemoveFixedUpdateCategory(float updateRate)
		{
			if (FixedUpdateEvents.ContainsKey(updateRate))
			{
				FixedUpdateEvents.Remove(updateRate);
			}
			foreach (var keyPair in FixedUpdateEvents)
			{
				keyPair.Value.Update();
			}
		}

		internal void RegisterEngineTickEvent(OnEngineTickCallback callback) { OnEngineTickEvent += callback; }
		internal void RemoveEngineTickEvent(OnEngineTickCallback callback) { OnEngineTickEvent -= callback; }
		private void OnEngineTick()
		{
			//TODO: add functionality to allow for modules to tick on the menu? (This might not be needed or wanted)
			if (suspended || timeMultiplier <= 0) return; //do not tick if ticking is disabled!
			if (OnEngineTickEvent != null) OnEngineTick();
			foreach (var keyPair in FixedUpdateEvents)
			{
				keyPair.Value.Update();
			}
		}

		private void SetupTicking() //where the magic happens
		{
			FixedUpdateEvents.Clear();
			//get the current update subsystem from the player loop
			PlayerLoopSystem unityMainLoop = PlayerLoop.GetCurrentPlayerLoop();
			PlayerLoopSystem[] unitySubSystems = unityMainLoop.subSystemList;
			PlayerLoopSystem[] oldUnityUpdate = unitySubSystems[4].subSystemList;

			PlayerLoopSystem GameManagerUpdate = new PlayerLoopSystem() //create the new update subsystem
			{
				updateDelegate = OnEngineTick,
				type = typeof(OnEngineTickCallback)
			};
			//inject our new update subsystems into our new playerloop

			PlayerLoopSystem[] newUnityUpdate = new PlayerLoopSystem[oldUnityUpdate.Length];//use length because we are only adding one element

			for (int i = 0; i < oldUnityUpdate.Length; i++)
			{
				newUnityUpdate[i] = oldUnityUpdate[i];
			}
			newUnityUpdate[oldUnityUpdate.Length - 1] = GameManagerUpdate;

			unitySubSystems[4].subSystemList = newUnityUpdate;
			PlayerLoopSystem systemRoot = new PlayerLoopSystem();
			systemRoot.subSystemList = unitySubSystems;
			PlayerLoop.SetPlayerLoop(systemRoot); //override the old player loop with our new one
		}
	}
}
