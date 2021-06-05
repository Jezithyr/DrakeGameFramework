

namespace DrakeFramework.Core
{
    public abstract class Service
    {
		public abstract int Priority{get;}
		//called when the service is initialized
		protected abstract void Initialize();
		//called when a service is reset
		protected abstract void Reset();
		//called when the service is destroyed
		protected abstract void Dispose();
		//called when the service is restarted
		protected virtual void Stop(Session session){}
		internal void internal_Initialize(){
			Initialize();
		}
		internal void internal_Dispose()
		{
			Dispose();
		}
		internal void internal_Reset(){
			Reset();
		}
		~Service()
		{
			internal_Dispose();
		}
    }
}
