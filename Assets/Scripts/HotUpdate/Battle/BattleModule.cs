using System.Threading.Tasks;
using Core.Service;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;

namespace HotUpdate.Battle
{
    public class BattleModule : IModule
    {
        public Task InitModuleAsync()
        {
            ServiceLocator.Get<IGameManager>().GameServiceManager.AddRegistrar(new BattleRegistrar());
            return Task.CompletedTask;
        }
    }
}
