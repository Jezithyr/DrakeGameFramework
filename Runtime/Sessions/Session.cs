using UnityEngine;

namespace Sessions
{
    public abstract class Session : ScriptableObject
    {
        public virtual void Initialize()
        {
        }

        public virtual void Enter()
        {
        }

        public virtual void Exit()
        {
        }
        
        public virtual void Shutdown()
        {
        }

        public virtual void Cleanup()
        {
            
        }

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }
    }
}