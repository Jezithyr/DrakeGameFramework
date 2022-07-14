namespace Dependencies
{
    public interface IService
    {
        void Initialize() {}
        void Shutdown() {}
        void FixedUpdate() {}
        void Update() {}
    }
}