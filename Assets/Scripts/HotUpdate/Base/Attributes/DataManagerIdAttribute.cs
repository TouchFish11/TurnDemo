using System;

namespace HotUpdate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataManagerIdAttribute : Attribute
    {
        /// <summary>
        /// 该数据管理器的接口映射类型
        /// </summary>
        public Type DataManagerIdMapType { get; }
        
        public DataManagerIdAttribute(Type dataManagerIdType)
        {
            DataManagerIdMapType = dataManagerIdType;
        }
    }
}
