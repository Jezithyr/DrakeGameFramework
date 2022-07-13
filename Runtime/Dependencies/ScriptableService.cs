using UnityEngine;

namespace Dependencies
{
    public abstract class ScriptableService : ScriptableObject, IService
    {
        public void Initialize()
        {
        }

        public virtual void Shutdown()
        {
        }
    }
}