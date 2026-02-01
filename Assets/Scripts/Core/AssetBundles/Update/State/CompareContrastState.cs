using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Collection;
using Core.AssetBundles.Update.Enum;
using Core.DataPersistence.Json;
using Core.Log;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    /// <summary>
    /// 对比校验状态类
    /// 负责对比本地与远程AssetBundle包信息，确定需要下载/删除的资源，同时处理缓存文件校验
    /// </summary>
    public class CompareContrastState : UpdateState
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        public CompareContrastState(AssetBundleUpdater updater) : base(updater)
        {

        }

        /// <summary>
        /// 执行对比校验核心逻辑
        /// </summary>
        /// <returns>是否执行成功</returns>
        public override async Task<bool> Execute()
        {
            // 执行对比校验逻辑
            IsSuceess = await CompareContrastFileInfo();
            if (!IsSuceess)
            {
                LogManager.LogError("资源对比校验失败");
                FinishUpdate(); // 终止更新流程
                return IsSuceess;
            }

            // 切换状态到资源下载阶段
            assetBundleUpdater.ChangeState(EUpdatePhase.DownLoadAssets);
            return IsSuceess;
        }

        /// <summary>
        /// 对比校验AssetBundle文件信息
        /// 1. 对比本地与远程包信息，标记需要下载的包
        /// 2. 删除本地冗余的AssetBundle文件
        /// 3. 加载缓存文件并校验已下载资源的有效性
        /// </summary>
        /// <returns>对比校验是否成功</returns>
        public async Task<bool> CompareContrastFileInfo()
        {
            // 获取上下文各类包集合
            var remoteCollection = assetBundleUpdater.GetContext().RemotePackageCollection; // 远程包信息集合
            var localCollection = assetBundleUpdater.GetContext().LocalPackageCollection;   // 本地包信息集合
            var waitDownloadCollection = assetBundleUpdater.GetContext().WaitDownloadCollection; // 待下载包集合
            var cachePackageCollection = assetBundleUpdater.GetContext().CachePackageCollection;   // 缓存包集合

            // 遍历远程AB包信息集合，对比本地包信息
            foreach (var (abName, abPackageInfo) in remoteCollection)
            {
                // 本地存在该AB包，校验MD5是否一致（不一致则标记为待下载）
                if (localCollection.ContainsKey(abName))
                {
                    // MD5不一致，说明远程包有更新，加入待下载集合
                    if (localCollection[abName].PackageMd5 != abPackageInfo.PackageMd5)
                    {
                        waitDownloadCollection.TryAdd(abName, new AbPackageCacheInfo(abName, abPackageInfo.PackageMd5, abPackageInfo.PackageSize));
                    }
                    // 对比完成后移除本地该包信息（避免后续误判为冗余）
                    localCollection.Remove(abName);
                }
                // 本地不存在该AB包，直接标记为待下载
                else
                {
                    waitDownloadCollection.TryAdd(abName, new AbPackageCacheInfo(abName, abPackageInfo.PackageMd5, abPackageInfo.PackageSize));
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
                try
                {
                    // 反序列化缓存文件到缓存集合
                    var abCacheCollection = JsonManager.Instance.FromJson<AbPackageCacheCollection>(cacheContent);
                    foreach (var (abName, abPackageCacheInfo) in abCacheCollection)
                    {
                        cachePackageCollection.TryAdd(abName, abPackageCacheInfo);
                    }
                }
                catch
                {
                    LogManager.LogError("缓存文件解析失败");
                    return false;
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
                    continue;
                }

                // 缓存中该包MD5与待下载包不一致，说明需要更新，跳过（保留待下载）
                if (cachePackageCollection[waitPair.Key].Md5 != waitPair.Value.Md5)
                {
                    continue;
                }

                // 缓存中该包MD5一致，且已下载完成，标记为无需下载（加入移除列表）
                if (cachePackageCollection[waitPair.Key].IsSuccess)
                {
                    waitRemoveABFileList.Add(waitPair.Key);
                }
                // 缓存中该包未下载完成，继承已下载的字节数（断点续传）
                else
                {
                    waitPair.Value.DownloadedBytes = cachePackageCollection[waitPair.Key].DownloadedBytes;
                }
            }

            // 从待下载集合中移除无需下载的包
            foreach (var abFileName in waitRemoveABFileList)
            {
                waitDownloadCollection.Remove(abFileName);
            }

            return true;
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.CompareContrast;
    }
}