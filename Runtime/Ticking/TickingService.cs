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
        private PlayerLoopSystem original;

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
            original = PlayerLoop.GetCurrentPlayerLoop();
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

        public override void Shutdown()
        {
            if (!original.Equals(default(PlayerLoopSystem)))
            {
                PlayerLoop.SetPlayerLoop(original);
            }
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

        private static ref PlayerLoopSystem[]? FindSystem<T>(ref PlayerLoopSystem system, ref PlayerLoopSystem toInsert, out int index) where T : struct
        {
            index = 0;
            ref var systems = ref system.subSystemList;
            if (systems == null)
            {
                return ref systems;
            }

            var type = typeof(T);

            for (var i = 0; i < systems.Length; i++)
            {
                ref var sub = ref systems[i];
                if (sub.type != type)
                {
                    if (systems.Length > i)
                    {
                        ref var systemsFound = ref FindSystem<T>(ref sub, ref toInsert, out index);
                        if (systemsFound != null)
                        {
                            return ref systemsFound;
                        }
                    }

                    continue;
                }

                index = i;
                return ref systems;
            }

            systems = null;
            return ref systems;
        }

        private static void InsertAfter<T>(ref PlayerLoopSystem system, ref PlayerLoopSystem toInsert) where T : struct
        {
            ref var systems = ref FindSystem<T>(ref system, ref toInsert, out var i);
            if (systems == null)
            {
                Debug.LogError($"Could not insert system {toInsert} after {nameof(T)} in system {system}");
                return;
            }

            Array.Resize(ref systems, systems.Length + 1);
            Array.Copy(systems, i + 1, systems, i + 2, systems.Length - 2 - i);
            systems[i + 1] = toInsert;
        }

        private static void InsertBefore<T>(ref PlayerLoopSystem system, ref PlayerLoopSystem toInsert) where T : struct
        {
            ref var systems = ref FindSystem<T>(ref system, ref toInsert, out var i);
            if (systems == null)
            {
                Debug.LogError($"Could not insert system {toInsert} before {nameof(T)} in system {system}");
                return;
            }

            Array.Resize(ref systems, systems.Length + 1);
            Array.Copy(systems, i, systems, i + 1, systems.Length - 1 - i);
            systems[i] = toInsert;
        }
    }
}