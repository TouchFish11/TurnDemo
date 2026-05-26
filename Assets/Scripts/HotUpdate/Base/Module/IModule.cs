using System.Threading.Tasks;

namespace HotUpdate.Base.Module
{
    public interface IModule
    {
        void Register();
        
        Task InitModuleAsync();
    }
}
