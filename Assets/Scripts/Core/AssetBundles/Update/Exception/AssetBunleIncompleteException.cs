namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// AB包下载不完整异常
    /// </summary>
    public class AssetBunleIncompleteException : UpdateException
    {
        public AssetBunleIncompleteException(string message) : base(message)
        {
            
        }
    }
}
