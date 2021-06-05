using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrakeFramework;
namespace DrakeFramework.Core
{
    public class TickingManager
    {

	// public sealed partial class GameManager : ScriptableObject
    // {

	// 	public delegate void OnEngineTickDelegate();
	// 	[System.NonSerialized] public Dictionary<float,CustomFixedUpdate> FixedUpdateEvents = new Dictionary<float, CustomFixedUpdate>();

	// 	private float timeMultiplier = 1.0f;
	// 	public float TimeMultiplier{get=>timeMultiplier; //update the time multiplier
	// 		set{
	// 			timeMultiplier = value;
	// 			foreach (var keyPair in FixedUpdateEvents)
	// 			{
	// 				keyPair.Value.TimeMultiplier = value;
	// 			}
	// 		}
		
	// 	}
	// 	private OnEngineTickDelegate OnEngineTickEvent;

	// 	public void RegisterFixedUpdateEvent(float updateRate, CustomFixedUpdate.OnFixedUpdateCallback callback,bool allowTimeMultiplier = true,float MaxAllowedTimeStep = 0)
	// 	{
	// 		if (FixedUpdateEvents.ContainsKey(updateRate))
	// 		{
	// 			FixedUpdateEvents[updateRate].Callback += callback;
	// 			FixedUpdateEvents[updateRate].MaxAllowedTimeStep = MaxAllowedTimeStep;
	// 		} else
	// 		{
	// 			FixedUpdateEvents.Add(updateRate,new CustomFixedUpdate(updateRate,callback,allowTimeMultiplier,MaxAllowedTimeStep));
	// 		}
	// 	}

	// 	public void RemoveFixedUpdateEvent(float updateRate, CustomFixedUpdate.OnFixedUpdateCallback callback)
	// 	{
	// 		CustomFixedUpdate updateInstance;
	// 		if (FixedUpdateEvents.TryGetValue(updateRate,out updateInstance))
	// 		{
	// 			//This may cause issues
	// 			updateInstance.Callback -= callback;
	// 		}
	// 	}

	// 	public void RemoveFixedUpdateCategory(float updateRate)
	// 	{
	// 		if (FixedUpdateEvents.ContainsKey(updateRate))
	// 		{
	// 			FixedUpdateEvents.Remove(updateRate);
	// 		}
	// 		foreach (var keyPair in FixedUpdateEvents)
	// 		{
	// 			keyPair.Value.Update();
	// 		}
	// 	}

	// 	public void RegisterEngineTickEvent(OnEngineTickDelegate callback) { OnEngineTickEvent += callback; }
	// 	public void RemoveEngineTickEvent(OnEngineTickDelegate callback) { OnEngineTickEvent -= callback; }

	// 	private void OnEngineTick()
	// 	{
	// 		if (OnEngineTickEvent != null) OnEngineTick();
	// 		foreach (var keyPair in FixedUpdateEvents)
	// 		{
	// 			keyPair.Value.Update();
	// 		}
	// 	}

	// 	private void SetupTicking() //where the magic happens
    // 	{
	// 	FixedUpdateEvents.Clear();
    //     //get the current update subsystem from the player loop
    //     PlayerLoopSystem unityMainLoop = PlayerLoop.GetCurrentPlayerLoop(); 
    //     PlayerLoopSystem[] unitySubSystems = unityMainLoop.subSystemList;
    //     PlayerLoopSystem[] oldUnityUpdate = unitySubSystems[4].subSystemList;
        
    //     PlayerLoopSystem GameManagerUpdate = new PlayerLoopSystem() //create the new update subsystem
    //     {
    //         updateDelegate = OnEngineTick,
    //         type = typeof(OnEngineTickDelegate)
    //     };
    //     //inject our new update subsystems into our new playerloop

    //     PlayerLoopSystem[] newUnityUpdate = new PlayerLoopSystem[oldUnityUpdate.Length];//use length because we are only adding one element

    //     for (int i = 0; i < oldUnityUpdate.Length; i++)
    //     {
    //         newUnityUpdate[i] = oldUnityUpdate[i];
    //     }
    //     newUnityUpdate[oldUnityUpdate.Length-1] = GameManagerUpdate;

	// 	unitySubSystems[4].subSystemList = newUnityUpdate;
    //     PlayerLoopSystem systemRoot = new PlayerLoopSystem();
    //     systemRoot.subSystemList = unitySubSystems;
    //     PlayerLoop.SetPlayerLoop(systemRoot); //override the old player loop with our new one
    // }

	}
}
