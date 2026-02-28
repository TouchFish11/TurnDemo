using System.Collections.Generic;

namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// AB包损坏异常
    /// </summary>
    public class AssetBunleBrokenException : UpdateException
    {
        public AssetBunleBrokenException(string meg) : base(meg)
        {
        }
    }
}
