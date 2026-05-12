using System;

namespace Core.Exceptions
{
    /// <summary>
    /// 资源加载异常
    /// </summary>
    public class AssetLoadException : ExceptionBase
    {
        /// <summary>
        /// 资源Key
        /// </summary>
        public string AssetKey { get; private set; }
        
        public AssetLoadException(string assetKey, int errorCode, string message, Exception inner) : base(errorCode, message, inner)
        {
            AssetKey =  assetKey;
        }
    }
}
