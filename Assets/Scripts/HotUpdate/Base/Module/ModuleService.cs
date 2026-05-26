using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.DI;
using Core.HotUpdate;

namespace HotUpdate.Base.Module
{
    public class ModuleService
    {
        [Inject] private IHotUpdateManager _hotUpdateManager;
        
        private readonly Dictionary<Type, IModule> _modules = new();

        public ModuleService()
        {
            List<Type> iModules = new();
            List<Type> modules = new();
            
            foreach (var hotAssembly in _hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in hotAssembly.GetTypes())
                {
                    if (typeof(IModule).IsAssignableFrom(type) && type.IsInterface)
                    {
                        iModules.Add(type);
                        continue;
                    }

                    if (typeof(IModule).IsAssignableFrom(type) && !type.IsInterface)
                    {
                        modules.Add(type);
                    }
                }
            }
            
            foreach (var iModule in iModules)
            {
                
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
            foreach (var module in _modules.Values)
            {
                await module.InitModuleAsync();
            }
        }
    }
}
