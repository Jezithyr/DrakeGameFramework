using System;
using System.Collections.Generic;
using DGF.Dependencies;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace DGF.Ticking
{
    public class TickingService : IService
    {
        private delegate void RunFixedUpdate();
        private delegate void RunUpdate();

        private readonly SortedList<string, IService> services = new();
        private PlayerLoopSystem original;

        public bool TickEnabled = true;

        public TickingService()
        {
            IoC.ServiceAdded += service => services.Add(service.GetType().FullName!, service);

            foreach (var service in IoC.All())
            {
                services.Add(service.GetType().FullName!, service);
            }
        }

        public void Initialize()
        {
            original = PlayerLoop.GetCurrentPlayerLoop();
            var loop = PlayerLoop.GetCurrentPlayerLoop();

            var fixedUpdate = new PlayerLoopSystem
            {
                updateDelegate = FixedUpdateLoop,
                type = typeof(RunFixedUpdate)
            };
            var update = new PlayerLoopSystem
            {
                updateDelegate = UpdateLoop,
                type = typeof(RunUpdate)
            };

            InsertBefore<FixedUpdate.ScriptRunBehaviourFixedUpdate>(ref loop, ref fixedUpdate);
            InsertBefore<Update.ScriptRunBehaviourUpdate>(ref loop, ref update);

            PlayerLoop.SetPlayerLoop(loop);
        }

        public void Shutdown()
        {
            if (!original.Equals(default(PlayerLoopSystem)))
            {
                PlayerLoop.SetPlayerLoop(original);
            }
        }

        private void FixedUpdateLoop()
        {
            if (!TickEnabled) return;
            foreach (var service in services.Values)
            {
                service.FixedUpdate();
            }
        }

        private void UpdateLoop()
        {
            if (!TickEnabled) return;
            foreach (var service in services.Values)
            {
                service.Update();
            }
        }

        private static ref PlayerLoopSystem[]? FindSystem<T>(ref PlayerLoopSystem system, out int index) where T : struct
        {
            index = 0;
            ref PlayerLoopSystem[]? systems = ref system.subSystemList;
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
                        ref var systemsFound = ref FindSystem<T>(ref sub, out index);
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
            ref var systems = ref FindSystem<T>(ref system, out var i);
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
            ref var systems = ref FindSystem<T>(ref system, out var i);
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