using System.Threading.Tasks;

namespace Game.Module
{
    public interface IModule
    {
        /// <summary>
        /// 优先级，越大越优先
        /// </summary>
        int Priority { get; }
        
        void Register();
        
        Task InitModuleAsync();
    }
}
