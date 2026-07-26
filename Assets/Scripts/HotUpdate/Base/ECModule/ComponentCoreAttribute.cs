using System;

namespace HotUpdate.Base.ECModule
{
    /// <summary>
    /// 组件核心类特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentCoreAttribute : Attribute
    {
        public Type ComponentCore { get; }
        
        public ComponentCoreAttribute(Type componentCore)
        {
            if(!typeof(IComponentCore).IsAssignableFrom(componentCore))
                throw new ArgumentException($"ComponentCore {componentCore.Name} does not implement IComponentCore");
            ComponentCore = componentCore;
        }
    }
}
