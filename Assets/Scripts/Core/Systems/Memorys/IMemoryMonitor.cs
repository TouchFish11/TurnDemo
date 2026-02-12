namespace Core.Systems.Memorys
{
    public interface IMemoryMonitor
    {
        void Register(IMemoryListener listener);
        
        void Unregister(IMemoryListener listener);
    }
}
