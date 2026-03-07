using System;
using System.Collections.Generic;
using Core.HotUpdate;
using Core.Service;

namespace HotUpdate.Core.Module
{
    /// <summary>
    /// 模块管理器
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private readonly Dictionary<Type, IModule> _modules = new();

        public void InitModules()
        {
            foreach (var hotAssembly in ServiceLocator.Get<IHotUpdateManager>().GetHotAssemblies())
            {
                foreach (var type in hotAssembly.GetTypes())
                {
                    if (typeof(IModule).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                    {
                        _modules.Add(type, Activator.CreateInstance(type) as IModule);
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
