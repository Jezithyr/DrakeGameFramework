using UnityEngine;

namespace Dependencies
{
    public abstract class ScriptableService : ScriptableObject, IService
    {
        public virtual void Initialize()
        {
        }

        public virtual void Shutdown()
        {
        }
    }
}