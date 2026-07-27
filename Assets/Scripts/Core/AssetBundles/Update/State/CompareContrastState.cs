using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Collection;
using Core.AssetBundles.Update.Core;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 对比差异状态类
    /// 负责对比本地与远程AssetBundle包信息，确定需要下载/删除的资源，同时处理缓存文件校验
    /// </summary>
    public class CompareContrastState : UpdateState
    {
        protected override async void OnEnter()
        {
            try
            {
                // 执行对比校验逻辑
                await CompareContrastFileInfo();
                assetBundleUpdater.ChangePhase(EUpdatePhase.CheckDeviceStorage);
            }
            catch (System.Exception exception)
            {
                var result = updateResultFactory.CreateFailure(UpdateResult.EUpdateError.AnalyzeAssetBundle, exception);
                assetBundleUpdater.GetContext().UpdateOver(result);
            }
        }

        /// <summary>
        /// 对比校验AssetBundle文件信息
        /// 对比本地与远程包信息，标记需要下载的包
        /// 删除本地冗余的AssetBundle文件
        /// 加载缓存文件并校验已下载资源的有效性
        /// </summary>
        /// <returns>对比校验是否成功</returns>
        public async Task CompareContrastFileInfo()
        {
            // 获取上下文各类包集合
            var remoteCollection = assetBundleUpdater.GetContext().RemotePackageCollection; // 远程包信息集合
            var localCollection = assetBundleUpdater.GetContext().LocalPackageCollection;   // 本地包信息集合
            var waitDownloadCollection = assetBundleUpdater.GetContext().WaitDownloadCollection; // 待下载包集合
            var cachePackageCollection = assetBundleUpdater.GetContext().CachePackageCollection;   // 缓存包集合

            // 转存本地临时数据
            var abPackageInfos = new Dictionary<string, ABPackageInfo>();
            foreach (var abPackageInfo in localCollection.Values)
            {
                abPackageInfos.Add(abPackageInfo.Name, abPackageInfo);
            }
            
            waitDownloadCollection.Clear();
            // 遍历远程AB包信息集合，对比本地包信息
            foreach (var (abName, abPackageInfo) in remoteCollection)
            {
                // 本地存在该AB包，校验Hash是否一致（不一致则标记为待下载）
                if (localCollection.ContainsKey(abName))
                {
                    // Hash不一致，说明远程包有更新，加入待下载集合
                    if (localCollection[abName].Hash != abPackageInfo.Hash)
                    {
                        waitDownloadCollection.TryAdd(abName, new AbPackageCacheInfo(abName, abPackageInfo.Hash, 0));
                    }
                    // 对比完成后移除本地该包信息（避免后续误判为冗余）
                    localCollection.Remove(abName);
                }
                // 本地不存在该AB包，直接标记为待下载
                else
                {
                    waitDownloadCollection.TryAdd(abName, new AbPackageCacheInfo(abName, abPackageInfo.Hash, 0));
                }
            }

            // 遍历剩余的本地包信息（远程不存在的包），判定为冗余并删除对应文件
            foreach (var (abName, _) in localCollection)
            {
                var abFilePath = PathUtility.GetAbLoadPath(abName);
                // PC平台下删除本地冗余AB文件
                if (File.Exists(abFilePath))
                {
                    File.Delete(abFilePath);
                }
            }

            // 异步读取缓存文件（记录已下载/待下载的AB包信息）
            var cacheFilePath = PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName);
            var cacheContent = await File.ReadAllTextAsync(cacheFilePath);
            if (!string.IsNullOrEmpty(cacheContent))
            {
                cachePackageCollection.Clear();
                // 反序列化缓存文件到缓存集合
                var abCacheCollection = jsonManager.FromJson<AbPackageCacheCollection>(cacheContent);
                foreach (var (abName, abPackageCacheInfo) in abCacheCollection)
                {
                    cachePackageCollection.TryAdd(abName, abPackageCacheInfo);
                }
            }

            // 待移除的AB文件列表（缓存中已存在且无需更新的包）
            var waitRemoveABFileList = new List<string>();

            // 遍历待下载集合，对比缓存集合，过滤无需重复下载的包
            foreach (var waitPair in waitDownloadCollection)
            {
                // 缓存中无该包信息，跳过（需要新下载）
                if (!cachePackageCollection.ContainsKey(waitPair.Key))
                {
                    var path = PathUtility.GetAbLoadPath(waitPair.Key.WithAbSuffix());
                    if (!File.Exists(path))
                        continue;

                    var localLength = new FileInfo(path).Length;
                    // 先判断是否是之前的完整旧包，是就跳过
                    if (localLength == abPackageInfos[waitPair.Key].Size)
                        continue;
                    
                    // 下载了但没有被缓存文件记录的意外情况（App异常关闭），根据文件的实际大小断点续传
                    var packageCacheInfo = new AbPackageCacheInfo(waitPair.Key, string.Empty, localLength);
                    cachePackageCollection.TryAdd(waitPair.Key, packageCacheInfo);
                }

                // 获取缓存文件的单个包信息
                var cachePackageInfo = cachePackageCollection[waitPair.Key];
                // 说明这个包没有下载完成——没有开始下载或下载未完成
                if (cachePackageInfo.Hash == string.Empty && cachePackageInfo.DownloadedBytes < remoteCollection[waitPair.Key].Size)
                {
                    // 更新待下载包信息，从当前位置开始下载
                    waitPair.Value.DownloadedBytes = cachePackageInfo.DownloadedBytes;
                }
                // 最新包下载完成，但是没有进行校验，那就不用下载了，若校验失败会回到这个状态
                else if(cachePackageInfo.Hash == string.Empty && cachePackageInfo.DownloadedBytes == remoteCollection[waitPair.Key].Size)
                {
                    waitRemoveABFileList.Add(waitPair.Key);
                }
                else if(cachePackageInfo.Hash != string.Empty)
                {
                    // 缓存中该包Hash与待下载包不一致，说明需要更新，跳过（保留待下载）
                    if (cachePackageInfo.Hash != waitPair.Value.Hash)
                        continue;
                    
                    // 缓存中该包Hash一致，且已下载完成，标记为无需下载（加入移除列表）
                    if (cachePackageInfo.IsSuccess)
                    {
                        waitRemoveABFileList.Add(waitPair.Key);
                    }
                    // 不会出现hash相同，没有下载完成的情况，因为存在记录的hash说明校验过了，肯定是下载完了的
                }
            }

            // 从待下载集合中移除无需下载的包
            foreach (var abFileName in waitRemoveABFileList)
            {
                waitDownloadCollection.Remove(abFileName);
            }

            // 当前存在下载
            assetBundleUpdater.GetContext().IsHasUpdate = waitDownloadCollection.Count != 0;
        }

        protected override void OnExit()
        {
            
        }
        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.CompareContrast;
    }
}