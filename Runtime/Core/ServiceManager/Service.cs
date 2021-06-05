

namespace DrakeFramework.Core
{
    public abstract class Service
    {
		public abstract int Priority{get;}
		protected abstract void Initialize();
		protected abstract void Dispose();
		protected abstract void Reset();
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
    }
}
