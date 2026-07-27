using System;

namespace HotUpdate.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataProviderIdAttribute : Attribute
    {
        /// <summary>
        /// 该数据提供器的接口映射类型
        /// </summary>
        public Type DataManagerIdMapType { get; }
        
        public DataProviderIdAttribute(Type dataManagerIdType)
        {
            DataManagerIdMapType = dataManagerIdType;
        }
    }
}
