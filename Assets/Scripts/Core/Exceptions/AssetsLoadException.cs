using System;

namespace Core.Exceptions
{
    /// <summary>
    /// 批量加载资源异常
    /// </summary>
    public class AssetsLoadException : ExceptionBase
    {
        /// <summary>
        /// AB包名称
        /// </summary>
        public string AssetBundleName { get; private set; }
        
        public Type Type { get; private set; }
        
        public AssetsLoadException(string assetBundleName, Type type, int errorCode, string message, Exception inner) : base(errorCode, message, inner)
        {
            AssetBundleName =  assetBundleName;
            Type = type;
        }
    }
}
