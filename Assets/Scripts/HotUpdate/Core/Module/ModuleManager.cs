using System;
using System.Collections.Generic;
using Core.HotUpdate;

namespace HotUpdate.Core.Module
{
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// 模块管理器
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private readonly IHotUpdateManager _hotUpdateManager;
        private readonly Dictionary<Type, IModule> _modules = new();

        public ModuleManager(IHotUpdateManager hotUpdateManager)
        {
            _hotUpdateManager = hotUpdateManager;
        }
        
        public async Task InitModules()
        {
            // 初始化所有热更模块
            foreach (var hotAssembly in _hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in hotAssembly.GetTypes())
                {
                    if (!typeof(IModule).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }
                        
                    // 反射创建模块对象
                    var module = (IModule)Activator.CreateInstance(type);
                    // 异步初始化模块
                    await module.InitModuleAsync();
                    // 缓存模块
                    _modules.TryAdd(type, module);
                }
            }
        }

        public T GetModule<T>() where T : class, IModule
        {
            return _modules[typeof(T)] as T;
        }
    }
}
