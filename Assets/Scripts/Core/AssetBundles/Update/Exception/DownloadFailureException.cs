namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// 下载失败异常
    /// </summary>
    public class DownloadFailureException : UpdateException
    {
        public DownloadFailureException(string message) : base(message)
        {
        }
    }
}
