using System;

namespace Core.DI
{
    /// <summary>
    /// 单例绑定信息
    /// </summary>
    public class BindingInfo
    {
        /// <summary>
        /// 实现类
        /// </summary>
        public Type ImplementationType { get; set; }
        
        /// <summary>
        /// 单例缓存
        /// </summary>
        public object CachedInstance { get; set; }
    }
}
