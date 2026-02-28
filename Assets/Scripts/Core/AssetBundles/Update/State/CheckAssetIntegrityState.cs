using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Enum;
using Core.AssetBundles.Update.Exception;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 资源完整性校验状态类
    /// 校验已下载的AssetBundle资源的大小和Hash是否与远程一致，处理冗余文件，完成后持久化缓存信息
    /// </summary>
    public class CheckAssetIntegrityState : UpdateState
    {
        private readonly List<(string abName, long downloadedBytes, bool hashSame)> _abBrokenInfos = new();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        public CheckAssetIntegrityState(AssetBundleUpdater updater) : base(updater)
        {

        }

        public override void Enter()
        {
            _abBrokenInfos.Clear();
        }

        /// <summary>
        /// 执行资源完整性校验核心逻辑
        /// </summary>
        /// <returns>是否校验通过</returns>
        public override async Task<UpdateResult> Execute()
        {
            try
            {
                // 执行完整性校验，传入进度回调
                await CheckAssetsIntegrity((cureent, total) =>
                    assetBundleUpdater.GetContext().UpdateCheckProgress(cureent, total));

                // 替换正式清单文件
                var tempListPath = PathUtility.GetAbLoadPath(FileUtility.TempListFileDefaultName);
                var formalListPath = PathUtility.GetAbLoadPath(FileUtility.ListFileDefaultName);
                File.Copy(tempListPath, formalListPath, true);

                // 删除临时清单文件
                File.Delete(tempListPath);

                // 持久化缓存文件（记录已下载的AssetBundle信息）
                await assetBundleUpdater.GetContext().WriteCacheFile();
            }
            catch (AssetBunleBrokenException assetBunleBrokenException)
            {
                return UpdateResult.CreateFailure(UpdateResult.EUpdateError.AssetBunleBroken, assetBunleBrokenException);
            }
            catch (System.Exception exception)
            {
                return UpdateResult.CreateFailure(UpdateResult.EUpdateError.Unknown, exception);
            }
            
            return UpdateResult.CreateSuccess();
        }

        /// <summary>
        /// 校验AssetBundle资源完整性
        /// 对比已下载资源的大小、Hash与远程清单是否一致，标记不完整资源
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
                var hash = await HashUtility.GenerateFileSHA256HashAsync(PathUtility.GetAbLoadPath(cachePair.Value.AbName));
                var hashSame = remoteCollection[cachePair.Key].Hash == hash;
                // 校验条件：已下载字节数 == 远程包大小 且 Hash一致
                if (remoteCollection[cachePair.Key].Size == cachePair.Value.DownloadedBytes && hashSame)
                {
                    continue;
                }
                
                // 校验失败，标记为损坏包
                AddBrokenInfo(cachePair.Key,  cachePair.Value.DownloadedBytes, hashSame);
                
                // 触发校验进度回调
                ++currentProgress;
                onCheckProgress?.Invoke(currentProgress, cacheCollection.Count);
            }

            // 抛出AB包损坏异常
            if (_abBrokenInfos.Count != 0)
            {
                throw new AssetBunleBrokenException(GetAssetBunleBrokenExceptionMessage());
            }
        }

        private void AddBrokenInfo(string abName, long downloadedBytes, bool hashSame)
        {
            var info = (abName, downloadedBytes, hashSame);
            _abBrokenInfos.Add(info);
        }

        private string GetAssetBunleBrokenExceptionMessage()
        {
            var sb = new StringBuilder();
            foreach (var abBrokenInfo in _abBrokenInfos)
            {
                sb.AppendLine($"包名：{abBrokenInfo.abName}，已下载字节数：{abBrokenInfo.downloadedBytes}，Hash是否相等{abBrokenInfo.hashSame}");
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.CheckAssetsIntegrity;
    }
}