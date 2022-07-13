using System;
using System.Diagnostics;
using Dependencies;
using UnityEngine.LowLevel;
using Debug = UnityEngine.Debug;

namespace Ticking
{
    public class TickingManager : ScriptableService
    {
        private delegate void OnUpdateCallback();

        public override void Initialize()
        {
            var stopwatch = Stopwatch.StartNew();
            var unityMainLoop = PlayerLoop.GetCurrentPlayerLoop();
            ref var subSystems = ref unityMainLoop.subSystemList;
            ref var preUpdate = ref subSystems[4];
            ref var oldUnityUpdate = ref preUpdate.subSystemList;

            Array.Resize(ref oldUnityUpdate, oldUnityUpdate.Length + 1);
            oldUnityUpdate[^1] = new PlayerLoopSystem
            {
                updateDelegate = OnEngineTick,
                type = typeof(OnUpdateCallback)
            };

            PlayerLoop.SetPlayerLoop(unityMainLoop);
            Debug.Log($"Ran in {stopwatch.Elapsed.TotalSeconds:F4} seconds");
        }

        private void OnEngineTick()
        {
        }
    }
}