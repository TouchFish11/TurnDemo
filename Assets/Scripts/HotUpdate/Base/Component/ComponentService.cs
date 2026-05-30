using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Components;
using Core.DI;
using Core.HotUpdate;

namespace HotUpdate.Base.Component
{
    public class ComponentService
    {
        [Inject] private IHotUpdateManager _hotUpdateManager;
        
        /// <summary>
        /// 扫描所有热更组件
        /// </summary>
        public void ScanComponents(Dictionary<string, Type> components)
        {
            // 获取热更的程序集
            foreach (var assembly in _hotUpdateManager.GetHotAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(IComponent).IsAssignableFrom(type) || type.IsAbstract)
                        continue;
                    
                    var attr = type.GetCustomAttribute<ComponentIdAttribute>();
                    if (attr != null)
                    {
                        components.TryAdd(attr.ComponentType.Name, attr.ComponentType);
                    }
                }
            }
        }
    }
}
