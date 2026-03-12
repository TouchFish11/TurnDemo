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
        public int Priority => 0;
        
        public Task InitModuleAsync()
        {
            ServiceLocator.Register<IMouseManager>(new MouseManager(ServiceLocator.Get<IEventCenter>()));
            LogManager.Log($"{nameof(InputModule)}.{nameof(InitModuleAsync)}：初始化完成");
            return Task.CompletedTask;
        }

        public IInputComponent AddInputComponent(IEntityObject entityObject)
        {
            return entityObject.AddComponent<InputComponent>();
        }
    }
}
