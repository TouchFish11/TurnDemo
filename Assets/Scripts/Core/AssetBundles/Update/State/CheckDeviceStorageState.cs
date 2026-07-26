using System.IO;
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
        
        protected override void OnEnter()
        {
            try
            {
                CheckCanDownload();
                assetBundleUpdater.ChangePhase(EUpdatePhase.DownLoadAssets);
            }
            catch (DriveShortageInsufficientException driveShortageInsufficientException)
            {
                var result = updateResultFactory.CreateFailure(UpdateResult.EUpdateError.DriveStorage, driveShortageInsufficientException);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
            catch (IOException ioException)
            {
                var result = updateResultFactory.CreateFailure(UpdateResult.EUpdateError.Unknown, ioException);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
            catch (System.Exception e)
            {
                var result = updateResultFactory.CreateFailure(UpdateResult.EUpdateError.Unknown, e);
                assetBundleUpdater.GetContext().UpdateOver(result);
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
            if (availableFreeSpace > 0)
            {
                var requireStorageSize = (long)(downLoadTotalBytes * ResidualFactor);
                // 可用空间小于要求大小
                if (availableFreeSpace < requireStorageSize)
                {
                    throw new DriveShortageInsufficientException(requireStorageSize, "该路径存储空间不足");
                }
            }
        }
        
        protected override void OnExit()
        {

        }

        public override EUpdatePhase UpdatePhase => EUpdatePhase.CheckDeviceStorage;
    }
}
