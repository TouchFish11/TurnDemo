using System.Threading.Tasks;
using Core.Components;
using Core.GlobalEvent;
using Core.Log;
using Core.Service;
using HotUpdate.Core.Input;
using HotUpdate.Core.Module;

namespace HotUpdate.Input
{
    /// <summary>
    /// 输入模块
    /// </summary>
    public class InputModule : IInputModule
    {
        public int Priority => 1;
        
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IMouseManager>(new MouseManager(ServiceLocator.Get<IEventCenter>()));
            LogManager.Log($"{nameof(InputModule)}.{nameof(InitModuleAsync)}:Input module initialization completed");
            return Task.CompletedTask;
        }

        public IInputComponent AddInputComponent(IEntityObject entityObject)
        {
            if (entityObject != null) return entityObject.AddComponent<InputComponent>();
            LogManager.LogError($"{nameof(InputModule)}.{nameof(AddInputComponent)}: entityObject is null)");
            return null;
        }
    }
}
