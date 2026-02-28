using System.IO;

namespace Core.AssetBundles.Update.Exception
{
    /// <summary>
    /// 设备空间不足异常
    /// </summary>
    public class DriveShortageInsufficientException : UpdateException
    {
        private DriveInfo _driveInfo;
        
        public DriveShortageInsufficientException(DriveInfo driveInfo, string message) : base(message)
        {
            _driveInfo = driveInfo;
        }
    }
}
