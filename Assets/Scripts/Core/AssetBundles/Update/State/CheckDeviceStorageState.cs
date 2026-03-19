using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Collection;
using Core.AssetBundles.Update.Core;
using Core.AssetBundles.Update.Exception;
using Core.Pool;
using Core.Serialize.Json;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 检查设备存储状态
    /// </summary>
    public class CheckDeviceStorageState : UpdateState
    {
        // 预留因子
        private const float ResidualFactor = 1.1f;
        // 用户设备信息
        private readonly DriveInfo _driveInfo;
        
        public CheckDeviceStorageState(IAssetBundleUpdater assetBundleUpdater, IPoolManager poolManager, IJsonManager jsonManager) : base(assetBundleUpdater, poolManager, jsonManager)
        {
            // 获取路径所在的驱动器（比如C:/、D:/）
            _driveInfo = new DriveInfo(Path.GetPathRoot(PathUtility.LoadAbPath));
        }

        public override Task<UpdateResult> Execute()
        {
            try
            {
                CheckCanDownload();
                return Task.FromResult(UpdateResult.CreateSuccess());
            }
            catch (DriveShortageInsufficientException driveShortageInsufficientException)
            {
                return Task.FromResult(UpdateResult.CreateFailure(UpdateResult.EUpdateError.DriveStorage, driveShortageInsufficientException));
            }
            catch (IOException ioException)
            {
                return Task.FromResult(UpdateResult.CreateFailure(UpdateResult.EUpdateError.Unknown, ioException));
            }
            catch (System.Exception e)
            {
                return Task.FromResult(UpdateResult.CreateFailure(UpdateResult.EUpdateError.Unknown, e));
            }
        }

        private void CheckCanDownload()
        {
            // 获取需要下载的总字节数
            var downLoadTotalBytes = (ulong)ABPackageCollection.GetTotalDownLoadBytes(
                assetBundleUpdater.GetContext().RemotePackageCollection,
                assetBundleUpdater.GetContext().WaitDownloadCollection
            );
            
            // 检查驱动器是否就绪（减少不必要的异常）
            if (!_driveInfo.IsReady)
            {
                throw new IOException($"驱动器未就绪：{_driveInfo.Name}");
            }
            
            var requireStorageSize = downLoadTotalBytes * ResidualFactor;
            // 可用空间小于要求大小
            if (_driveInfo.AvailableFreeSpace < requireStorageSize)
            {
                throw new DriveShortageInsufficientException(_driveInfo, GetDriveShortageInsufficientExceptionMessage());
            }
        }

        private string GetDriveShortageInsufficientExceptionMessage()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"用户设备类型：{_driveInfo.DriveType}，" +
                          $"总空间：{TextUtility.ToByteUnit((ulong)_driveInfo.TotalSize)}，" +
                          $"空闲空间：{TextUtility.ToByteUnit((ulong)_driveInfo.TotalFreeSpace)}，" +
                          $"可用空闲空间：{TextUtility.ToByteUnit((ulong)_driveInfo.TotalFreeSpace)}，" +
                          $"盘符：{_driveInfo.Name}");
            
            return sb.ToString();
        }

        public override EUpdatePhase UpdatePhase => EUpdatePhase.CheckDeviceStorage;
    }
}
