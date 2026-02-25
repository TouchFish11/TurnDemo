using System;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 资源完整性校验状态类
    /// 校验已下载的AssetBundle资源的大小和MD5是否与远程一致，处理冗余文件，完成后持久化缓存信息
    /// </summary>
    public class CheckAssetIntegrityState : UpdateState
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        public CheckAssetIntegrityState(AssetBundleUpdater updater) : base(updater)
        {

        }

        /// <summary>
        /// 执行资源完整性校验核心逻辑
        /// </summary>
        /// <returns>是否校验通过</returns>
        public override async Task<UpdateResult> Execute()
        {
            try
            {
                await Task.Delay(1000);
                
                // 执行完整性校验，传入进度回调
                await CheckAssetsIntegrity((cureent, total) => assetBundleUpdater.GetContext().UpdateCheckProgress(cureent, total));
                
                // 替换正式清单文件
                var tempListPath = PathUtility.GetAbLoadPath(FileUtility.TempListFileDefaultName);
                var formalListPath = PathUtility.GetAbLoadPath(FileUtility.ListFileDefaultName);
                File.Copy(tempListPath, formalListPath, true);
            
                // 删除临时清单文件
                File.Delete(tempListPath);
            
                // 删除缓存文件
                //File.Delete(PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName));
            
                // 持久化缓存文件（记录已下载的AssetBundle信息）
                await assetBundleUpdater.GetContext().WriteCacheFile();
            }
            catch (System.Exception exception)
            {
                return UpdateResult.CreateFailure("资源完整性校验失败", exception);
            }
            
            return UpdateResult.CreateSuccess();
        }

        /// <summary>
        /// 校验AssetBundle资源完整性
        /// 对比已下载资源的大小、MD5与远程清单是否一致，标记不完整资源
        /// </summary>
        /// <param name="onCheckProgress">校验进度回调（当前校验数/总校验数）</param>
        /// <returns>是否所有资源都完整</returns>
        public async Task CheckAssetsIntegrity(Action<int, int> onCheckProgress)
        {
            var context = assetBundleUpdater.GetContext();
            
            // 先更新失败队列中的缓存信息（标记为未完成）
            foreach (var cacheInfo in context.GetCacheInfosFromFail())
            {
                context.UpdateCacheFile(cacheInfo);
            }

            // 获取远程包集合和缓存包集合
            var remoteCollection = context.RemotePackageCollection;
            var cacheCollection = context.CachePackageCollection;

            var currentProgress = 0;
            // 遍历所有缓存包，校验完整性
            foreach (var cachePair in cacheCollection)
            {
                currentProgress++;
                // 触发校验进度回调
                onCheckProgress?.Invoke(currentProgress, cacheCollection.Count);

                await Task.Yield(); // 帧间等待，避免阻塞主线程

                // 校验条件：已下载字节数 == 远程包大小 且 MD5一致
                if (remoteCollection[cachePair.Key].Size == cachePair.Value.DownloadedBytes &&
                    remoteCollection[cachePair.Key].Hash == cachePair.Value.Md5)
                {
                    continue;
                }

                // 校验失败，标记为不完整资源
                context.AddABNameToIncomplete(cachePair.Key);
            }

            // 最终校验结果：不完整资源列表为空则通过
            if (context.IncompleteListCount != 0)
            {
                throw new System.Exception($"资源不完整，AB包数量：{context.IncompleteListCount}");
            }
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.CheckAssetsIntegrity;
    }
}