using Core.Components;

namespace HotUpdate.Core.Main
{
    public interface IPlayerManager
    {
        IEntityObject MainPlayer { get; }
        
        System.Threading.Tasks.Task CreatePlayer(uint uid);
        
        void Clear();
    }
}
