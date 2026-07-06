using System;

namespace HotUpdate.Base.Component
{
    /// <summary>
    /// 组件ID特性（ComponentIdAttribute）
    /// 作用于组件类，用于标记组件的唯一标识，供ComponentService根据标识查找对应的组件类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ComponentIdAttribute : Attribute
    {
        /// <summary>
        /// 组件的唯一名称/ID（Unique component identifier）
        /// 该值作为组件的唯一标识，确保不同组件的标识不重复
        /// </summary>
        public Type ComponentType { get; private set; }

        /// <summary>
        /// 构造函数：初始化组件ID特性
        /// </summary>
        /// <param name="componentType">组件唯一名称/ID，不可为空或重复</param>
        public ComponentIdAttribute(Type componentType)
        {
            ComponentType = componentType;
        }
    }
}