namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// 磁盘空间不足异常
    /// </summary>
    public class DiskSpaceShortageException : UpdateException
    {
        public DiskSpaceShortageException(string message) : base(message)
        {
        
        }
    }
}
