using System;
using System.Collections.Generic;
using Dependencies;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Ticking
{
    public class TickingService : ScriptableService
    {
        private delegate void OnUpdateCallback();

        private readonly SortedList<string, IService> services = new();

        public TickingService()
        {
            IoC.ServiceAdded += service => services.Add(service.GetType().FullName!, service);

            foreach (var service in IoC.All())
            {
                services.Add(service.GetType().FullName!, service);
            }
        }

        public override void Initialize()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            var fixedUpdate = new PlayerLoopSystem
            {
                updateDelegate = FixedUpdateLoop,
                type = typeof(OnUpdateCallback)
            };
            var update = new PlayerLoopSystem
            {
                updateDelegate = UpdateLoop,
                type = typeof(OnUpdateCallback)
            };

            InsertBefore<FixedUpdate.ScriptRunBehaviourFixedUpdate>(ref loop, ref fixedUpdate);
            InsertBefore<Update.ScriptRunBehaviourUpdate>(ref loop, ref update);

            PlayerLoop.SetPlayerLoop(loop);
        }

        private void FixedUpdateLoop()
        {
            foreach (var service in services.Values)
            {
                service.FixedUpdate();
            }
        }

        private void UpdateLoop()
        {
            foreach (var service in services.Values)
            {
                service.Update();
            }
        }

        private static void InsertBefore<T>(ref PlayerLoopSystem system, ref PlayerLoopSystem toInsert) where T : struct
        {
            if (!InsertBeforeRecursive<T>(ref system, ref toInsert))
            {
                Debug.LogError($"Could not insert system {toInsert} before {nameof(T)} in system {system}");
            }
        }

        private static bool InsertBeforeRecursive<T>(ref PlayerLoopSystem system, ref PlayerLoopSystem toInsert) where T : struct
        {
            ref var systems = ref system.subSystemList;
            if (systems == null)
            {
                return false;
            }

            var type = typeof(T);

            for (var i = 0; i < systems.Length; i++)
            {
                ref var sub = ref systems[i];
                if (sub.type != type)
                {
                    if (systems.Length > i &&
                        InsertBeforeRecursive<T>(ref sub, ref toInsert))
                    {
                        return true;
                    }

                    continue;
                }

                Array.Resize(ref systems, systems.Length + 1);
                Array.Copy(systems, i, systems, i + 1, systems.Length - 1 - i);
                systems[i] = toInsert;

                return true;
            }

            return false;
        }
    }
}