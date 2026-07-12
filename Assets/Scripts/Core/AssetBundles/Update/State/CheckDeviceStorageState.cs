using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Collection;
using Core.AssetBundles.Update.Core;
using Core.AssetBundles.Update.Exception;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 检查设备存储状态
    /// </summary>
    public class CheckDeviceStorageState : UpdateState
    {
        // 预留因子
        private const float ResidualFactor = 1.1f;

        public override Task<UpdateResult> Execute()
        {
            try
            {
                CheckCanDownload();
                return Task.FromResult(updateResultFactory.CreateSuccess());
            }
            catch (DriveShortageInsufficientException driveShortageInsufficientException)
            {
                return Task.FromResult(updateResultFactory.CreateFailure(UpdateResult.EUpdateError.DriveStorage, driveShortageInsufficientException));
            }
            catch (IOException ioException)
            {
                return Task.FromResult(updateResultFactory.CreateFailure(UpdateResult.EUpdateError.Unknown, ioException));
            }
            catch (System.Exception e)
            {
                return Task.FromResult(updateResultFactory.CreateFailure(UpdateResult.EUpdateError.Unknown, e));
            }
        }

        private void CheckCanDownload()
        {
            // 获取需要下载的总字节数
            var downLoadTotalBytes = (ulong)ABPackageCollection.GetTotalDownLoadBytes(
                assetBundleUpdater.GetContext().RemotePackageCollection,
                assetBundleUpdater.GetContext().WaitDownloadCollection
            );

            var availableFreeSpace = StorageHelper.GetAvailableSpace();
            var requireStorageSize = (long)(downLoadTotalBytes * ResidualFactor);
            // 可用空间小于要求大小
            if (availableFreeSpace < requireStorageSize)
            {
                throw new DriveShortageInsufficientException(requireStorageSize, "该路径存储空间不足");
            }
        }

        public override EUpdatePhase UpdatePhase => EUpdatePhase.CheckDeviceStorage;
    }
}
