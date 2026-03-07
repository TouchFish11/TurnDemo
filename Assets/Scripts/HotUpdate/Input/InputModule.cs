using System.Threading.Tasks;
using Core.Components;
using Core.Service;
using HotUpdate.Core.Input;
using HotUpdate.Core.Manager;
using HotUpdate.Core.Module;

namespace HotUpdate.Input
{
    /// <summary>
    /// 输入模块
    /// </summary>
    public class InputModule : IInputModule
    {
        public Task InitModuleAsync()
        {
            ServiceLocator.Get<IGameManager>().GameServiceManager.AddRegistrar(new InputRegistrar());
            return Task.CompletedTask;
        }

        public IInputComponent AddInputComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<InputComponent>();
        }
    }
}
