namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// 设备空间不足异常
    /// </summary>
    public class DriveShortageInsufficientException : UpdateException
    {
        private long RequireSize { get; }
        
        public DriveShortageInsufficientException(long requireSize, string message) : base(message)
        {
            RequireSize = requireSize;
        }
    }
}
