using System;

namespace Core.Exceptions
{
    /// <summary>
    /// AB包卸载异常
    /// </summary>
    public class AssetBundleUnloadException : ExceptionBase
    {
        public string AssetBundleName { get; private set; }
        
        public uint RefCount { get; private set; }
    
        public AssetBundleUnloadException(string assetBundleName, uint refCount, int errorCode, string message, Exception inner) : base(errorCode, message, inner)
        {
            AssetBundleName = assetBundleName;
            RefCount = refCount;
        }
    }
}
