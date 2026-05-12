using Core.Components;

namespace HotUpdate.Base.Main
{
    public interface IPlayerManager
    {
        IEntityObject MainPlayer { get; }
        
        System.Threading.Tasks.Task CreatePlayer(uint uid);
        
        void Clear();
    }
}
