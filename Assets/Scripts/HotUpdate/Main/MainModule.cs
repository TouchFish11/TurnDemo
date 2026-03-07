using System.Threading.Tasks;
using Core.Service;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;

namespace HotUpdate.Main
{
    public class MainModule : IModule
    {
        public Task InitModuleAsync()
        {
            ServiceLocator.Get<IGameManager>().GameServiceManager.AddRegistrar(new MainRegistrar());
            return Task.CompletedTask;
        }
    }
}
