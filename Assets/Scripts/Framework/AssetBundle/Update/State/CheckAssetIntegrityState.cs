using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 检查资源完整性状态
    /// </summary>
    public class CheckAssetIntegrityState : UpdateState
    {
        public CheckAssetIntegrityState(AssetBundleUpdater updater) : base(updater)
        {

        }

        public override async Task<bool> Execute()
        {
            //检查资源完整性
            IsSuceess = await CheckAssetsIntegrity((cureent, total) => assetBundleUpdater.GetContext().UpdateCheckProgress(cureent, total));

            if (!IsSuceess)
            {
                LogManager.LogError("资源不完整，请重新下载缺失资源");
                FinishUpdate();
                return IsSuceess;
            }

            // 更新本地的AB包清单文件
            File.Copy(PathManager.GetAbLoadPath(FileUtility.TempListFileDefaultName), PathManager.GetAbLoadPath(FileUtility.ListFileDefaultName), true);
            // 删除临时清单文件
            //File.Delete(PathManager.GetAbLoadPath(FileUtility.CacheDefaultName));
            // 删除临时对比文件
            File.Delete(PathManager.GetAbLoadPath(FileUtility.TempListFileDefaultName));
            // 写入缓存文件
            await assetBundleUpdater.GetContext().WriteCacheFile();
            // 切换至完成状态
            assetBundleUpdater.ChangeState(E_UpdatePhase.Finished);
            return IsSuceess;
        }

        /// <summary>
        /// 检查资源完整性
        /// </summary>
        /// <param name="onCheckProgress">结束回调</param>
        public async Task<bool> CheckAssetsIntegrity(UnityAction<int, int> onCheckProgress)
        {
            var context = assetBundleUpdater.GetContext();
            foreach (ABPackageCacheInfo cacheInfo in assetBundleUpdater.GetContext().GetCacheInfosFromFail())
            {
                // 更新缓存文件信息
                context.UpdateCacheFile(cacheInfo);
            }

            ABPackageCollection remoteCollection = assetBundleUpdater.GetContext().RemotePackageCollection;
            ABPackageCacheCollection cacheCollection = assetBundleUpdater.GetContext().CachePackageCollection;

            int currentProgress = 0;
            // 获取当前更新的所有AB包
            foreach (KeyValuePair<string, ABPackageCacheInfo> cachePair in cacheCollection)
            {
                ++currentProgress;
                onCheckProgress?.Invoke(currentProgress, cacheCollection.Count);

                await Task.Yield();

                if (remoteCollection[cachePair.Key].Size == cachePair.Value.DownloadedBytes &&
                    remoteCollection[cachePair.Key].Md5 == cachePair.Value.Md5)
                {
                    continue;
                }

                // 添加到不完整链表中
                assetBundleUpdater.GetContext().AddABNameToIncomplete(cachePair.Key);
            }

            // 根据_incompleteABList判断资源是否完整
            return assetBundleUpdater.GetContext().IncompleteListCount == 0;
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.CheckAssetsIntegrity;
    }
}
