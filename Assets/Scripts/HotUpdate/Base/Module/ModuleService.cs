using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Core.DI;
using Core.HotUpdate;
using Game.Module;

namespace HotUpdate.Base.Module
{
    /// <summary>
    /// 模块服务
    /// </summary>
    public class ModuleService
    {
        private readonly Dictionary<Type, IModule> _modules = new();

        public ModuleService(IHotUpdateManager hotUpdateManager)
        {
            foreach (var hotAssembly in hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in hotAssembly.GetTypes())
                {
                    if (!typeof(IModule).IsAssignableFrom(type) && type.IsClass)
                        continue;

                    var moduleExportAttribute = type.GetCustomAttribute<ModuleExportAttribute>();
                    if (moduleExportAttribute == null)
                        continue;
                    
                    DIContainer.BindSingleton(moduleExportAttribute.ModuleType, type);
                    var module = DIContainer.Resolve(type) as IModule;
                    _modules.Add(moduleExportAttribute.ModuleType, module);
                }
            }
        }

        public void RegisterModules()
        {
            foreach (var module in _modules.Values)
            {
                module.Register();
            }
        }
        
        public async Task InitModulesAsync()
        {
            var sorts = new List<IModule>(_modules.Values);
            sorts.Sort((m1, m2) =>
            {
                if (m1.Priority > m2.Priority)
                    return -1;
                return 1;
            });
            
            foreach (var module in sorts)
            {
                await module.InitModuleAsync();
            }
        }

        public async Task InitModuleAsync(Type moduleType)
        {
            await _modules.GetValueOrDefault(moduleType).InitModuleAsync();
        }
    }
}
