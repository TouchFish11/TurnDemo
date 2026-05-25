using Core.Components;

namespace HotUpdate.Game.Main
{
    public interface IPlayerManager
    {
        IEntityObject MainPlayer { get; }
        
        System.Threading.Tasks.Task CreatePlayer(uint uid);
        
        void Clear();
    }
}
