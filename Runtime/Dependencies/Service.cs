namespace Dependencies
{
    public abstract class Service : IPostInject
    {
        public virtual void Initialize()
        {
        }

        public virtual void Shutdown()
        {
        }
    }
}