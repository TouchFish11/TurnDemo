using System;

namespace Core.DI
{
    /// <summary>
    /// 构造参数封装
    /// </summary>
    internal struct Parameter
    {
        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ArgType { get; set; }
        
        /// <summary>
        /// 参数值
        /// </summary>
        public object ArgValue { get; set; }
    }
}
