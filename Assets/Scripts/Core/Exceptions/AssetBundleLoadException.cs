using System;

namespace Core.Exceptions
{
    /// <summary>
    /// AssetBundle加载异常
    /// </summary>
    public class AssetBundleLoadException : ExceptionBase
    {
        /// <summary>
        /// AssetBundle名称
        /// </summary>
        public string AssetBundleName { get; private set; }
        
        public AssetBundleLoadException(string bundleName, int errorCode, string message, Exception inner) : base(errorCode, message, inner)
        {
            AssetBundleName =  bundleName;
        }
    }
}
