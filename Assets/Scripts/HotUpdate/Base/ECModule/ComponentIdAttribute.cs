using System;

namespace HotUpdate.Base.ECModule
{
    /// <summary>
    /// 组件ID特性（ComponentIdAttribute）
    /// 作用于组件类，用于标记组件的唯一标识，供ComponentService根据标识查找对应的组件类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ComponentIdAttribute : Attribute
    {

    }
}