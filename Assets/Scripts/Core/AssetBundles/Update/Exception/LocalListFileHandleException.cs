namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// 本地清单文件处理异常
    /// </summary>
    public class LocalListFileHandleException : UpdateException
    {
        public LocalListFileHandleException(string message) : base(message)
        {
        }
    }
}
