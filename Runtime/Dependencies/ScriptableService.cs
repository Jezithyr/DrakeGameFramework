using UnityEngine;

namespace DGF.Dependencies
{
    [ResetStateOnExitPlayMode]
    public abstract class ScriptableService : ScriptableObject, IService
    {
        public virtual void Initialize()
        {
        }

        public virtual void Shutdown()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void Update()
        {
        }
    }
}