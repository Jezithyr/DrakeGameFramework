using System;
using Dependencies;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Ticking
{
    public class TickingManager : ScriptableService
    {
        private delegate void OnUpdateCallback();

        public override void Initialize()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            var fixedUpdate = new PlayerLoopSystem
            {
                updateDelegate = FixedUpdate,
                type = typeof(OnUpdateCallback)
            };
            var update = new PlayerLoopSystem
            {
                updateDelegate = FixedUpdate,
                type = typeof(OnUpdateCallback)
            };

            InsertBefore<FixedUpdate.ScriptRunBehaviourFixedUpdate>(ref loop, ref fixedUpdate);
            InsertBefore<Update.ScriptRunBehaviourUpdate>(ref loop, ref update);

            PlayerLoop.SetPlayerLoop(loop);
        }

        private void FixedUpdate()
        {
        }

        private void Update()
        {
        }

        private static bool InsertBefore<T>(ref PlayerLoopSystem system, ref PlayerLoopSystem toInsert) where T : struct
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
                        InsertBefore<T>(ref sub, ref toInsert))
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