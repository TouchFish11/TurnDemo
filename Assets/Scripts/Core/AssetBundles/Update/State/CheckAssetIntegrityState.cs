using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Core.AssetBundles.Collection;
using Core.AssetBundles.Update.Core;
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
        // 损坏的AB包信息列表
        private readonly List<(string abName, long downloadedBytes, bool hashSame, string badHash)> _abBrokenInfos = new();
        // 当前进度
        private int currentProgress = 0;
        
        /// <summary>
        /// 远端包集合
        /// </summary>
        private ABPackageCollection RemoteCollection
        {
            get => assetBundleUpdater.GetContext().RemotePackageCollection;
            set
            {
                
            }
        }

        /// <summary>
        /// 缓存文件集合
        /// </summary>
        private AbPackageCacheCollection CacheCollection
        {
            get => assetBundleUpdater.GetContext().CachePackageCollection;
            set
            {
                
            }
        }
        
        protected override async void OnEnter()
        {
            try
            {
                _abBrokenInfos.Clear();
                // 执行完整性校验，传入进度回调
                await CheckAssetsIntegrity((cureent, total) => assetBundleUpdater.GetContext().UpdateCheckProgress(cureent, total));

                // 替换正式清单文件
                var tempListPath = PathUtility.GetAbLoadPath(FileUtility.TempCatalogDefaultName);
                var formalListPath = PathUtility.GetAbLoadPath(FileUtility.CatalogDefaultName);
                File.Copy(tempListPath, formalListPath, true);

                // 删除临时清单文件
                File.Delete(tempListPath);

                // 持久化缓存文件（记录已下载的AssetBundle信息）
                await updateService.WriteCacheFileAsync(assetBundleUpdater.GetContext().CachePackageCollection);
                
                assetBundleUpdater.ChangePhase(EUpdatePhase.Finished);
            }
            catch (AssetBunleBrokenException assetBunleBrokenException)
            {
                var result = updateResultFactory.CreateFailure(UpdateResult.EUpdateError.AssetBunleBroken, assetBunleBrokenException);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
            catch (System.Exception exception)
            {
                var result = updateResultFactory.CreateFailure(UpdateResult.EUpdateError.Unknown, exception);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
        }
        
        /// <summary>
        /// 校验AssetBundle资源完整性
        /// 对比已下载资源的大小、Hash与远程清单是否一致，标记不完整资源
        /// </summary>
        /// <param name="onCheckProgress">校验进度回调（当前校验数/总校验数）</param>
        /// <returns>是否所有资源都完整</returns>
        public async Task CheckAssetsIntegrity(Action<int, int> onCheckProgress)
        {
            var hashTasks = new List<Task>(CacheCollection.Count);
            // 遍历所有缓存包，校验完整性
            foreach (var cachePair in CacheCollection)
            {
                var path = PathUtility.GetAbLoadPath(cachePair.Value.AbName.WithAbSuffix());
                hashTasks.Add(ComputeHashAsync(path, cachePair, onCheckProgress));
            }
            
            await Task.WhenAll(hashTasks);

            // 抛出AB包损坏异常
            if (_abBrokenInfos.Count != 0)
            {
                throw new AssetBunleBrokenException(GetAssetBunleBrokenExceptionMessage());
            }
        }

        private async Task ComputeHashAsync(string path, KeyValuePair<string,AbPackageCacheInfo> cachePair, Action<int, int> onCheckProgress)
        {
            var hash = await HashUtility.GenerateFileSHA256HashAsync(path);
            var hashSame = RemoteCollection[cachePair.Key].Hash == hash;
            // 校验条件：已下载字节数 == 远程包大小 且 Hash一致
            if (RemoteCollection[cachePair.Key].Size != cachePair.Value.DownloadedBytes || !hashSame)
            {
                // 校验失败，标记为损坏包
                AddBrokenInfo(cachePair.Key,  cachePair.Value.DownloadedBytes, hashSame, hash);
            }
                
            // 更新缓存hash信息
            cachePair.Value.Hash = hash;
                
            // 触发校验进度回调
            ++currentProgress;
            onCheckProgress?.Invoke(currentProgress, CacheCollection.Count);
        }

        private void AddBrokenInfo(string abName, long downloadedBytes, bool hashSame, string badHash)
        {
            var info = (abName, downloadedBytes, hashSame, badHash);
            _abBrokenInfos.Add(info);
        }

        private string GetAssetBunleBrokenExceptionMessage()
        {
            var sb = new StringBuilder();
            foreach (var abBrokenInfo in _abBrokenInfos)
            {
                sb.AppendLine($"包名：{abBrokenInfo.abName}，已下载字节数：{abBrokenInfo.downloadedBytes}，Hash是否相等{abBrokenInfo.hashSame}，" + $"损坏包hash：{abBrokenInfo.badHash}");
            }
            return sb.ToString();
        }

        protected override void OnExit()
        {
            currentProgress = 0;
            RemoteCollection = null;
            CacheCollection = null;
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.CheckAssetsIntegrity;
    }
}