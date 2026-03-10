using System;
using System.Collections.Generic;
using Core.Collection;
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
            var uniList = ListUtility.GetUniList<Type>();
            // 临时缓存具体模块接口
            foreach (var hotAssembly in _hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in hotAssembly.GetTypes())
                {
                    if (typeof(IModule).IsAssignableFrom(type) && type.IsInterface)
                    {
                        uniList.Add(type);
                    }
                }
            }
            
            // 缓存模块接口类型到模块实例的映射
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
                    foreach (var interfaceType in uniList.List)
                    {
                        if (interfaceType.IsAssignableFrom(type))
                        {
                            _modules.TryAdd(interfaceType, module);
                        }
                    }
                }
            }
            
            
        }

        public T GetModule<T>() where T : class, IModule
        {
            return _modules[typeof(T)] as T;
        }
    }
}
