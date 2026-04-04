using System;
using System.Collections.Generic;
using Core.Collection;
using Core.Extensions;
using Core.HotUpdate;
using Core.Log;

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
                        if (typeof(IModule).IsAssignableFrom(type) && type.IsInterface && type != typeof(IModule))
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
                
                ListUtility.CollectUniList(uniList);
                
                var uniList2 = ListUtility.GetUniList<IModule>();
                // 字典values转list
                uniList2.AddRange(_modules.Values.ToArray(module => module));
                // 按优先级排序
                uniList2.Sort((m1, m2) =>
                {
                    if (m1.Priority < m2.Priority)
                    {
                        return -1;
                    }

                    return m1.Priority > m2.Priority ? 1 : 0;
                });
                
                // 异步初始化模块
                foreach (var module in uniList2.List)
                {
                    await module.InitModuleAsync();
                }
                
                ListUtility.CollectUniList(uniList2);
        }

        public T GetModule<T>() where T : class, IModule
        {
            if (_modules.TryGetValue(typeof(T), out var module))
            {
                return (T)module;
            }
            
            LogManager.LogWarning($"{nameof(ModuleManager)}.{nameof(GetModule)}：Module {typeof(T).Name} not found");
            return null;
        }
    }
}
