using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 对比差异状态
    /// </summary>
    public class CompareContrastState : UpdateState
    {
        public CompareContrastState(AssetBundleUpdater updater) : base(updater)
        {

        }

        public override async Task<bool> Execute()
        {
            // 对比差异
            IsSuceess = await CompareContrastFileInfo();
            if (!IsSuceess)
            {
                LogMgr.LogError("差异对比失败");
                FinishUpdate();
                return IsSuceess;
            }

            // 切换至下载资源阶段
            assetBundleUpdater.ChangeState(E_UpdatePhase.DownLoadAssets);
            return IsSuceess;
        }


        /// <summary>
        /// 对比对比文件信息
        /// </summary>
        /// <returns>是否对比完成</returns>
        public async Task<bool> CompareContrastFileInfo()
        {
            ABPackageCollection remoteCollection = assetBundleUpdater.GetContext().RemotePackageCollection;
            ABPackageCollection localCollection = assetBundleUpdater.GetContext().LocalPackageCollection;
            ABPackageCacheCollection waitDownloadCollection = assetBundleUpdater.GetContext().WaitDownloadCollection;
            ABPackageCacheCollection cachePackageCollection = assetBundleUpdater.GetContext().CachePackageCollection;

            // 遍历远端的AB包信息集合
            foreach (KeyValuePair<string, ABPackageInfo> remotePair in remoteCollection)
            {
                // 本地AB包信息集合包含相同AB包名就对比MD5码
                if (localCollection.ContainsKey(remotePair.Key))
                {
                    // 不相等，说明远端是最新的，放入待下载字典
                    if (localCollection[remotePair.Key].Md5 != remotePair.Value.Md5)
                    {
                        waitDownloadCollection.TryAdd(remotePair.Key, new ABPackageCacheInfo(remotePair.Key, remotePair.Value.Md5, remotePair.Value.Size));
                    }
                    // 对比完同名AB包文件，就移除本地AB信息集合中对应内容
                    localCollection.Remove(remotePair.Key);
                }
                // 没有说明是新包，放入待下载集合中
                else
                {
                    waitDownloadCollection.TryAdd(remotePair.Key, new ABPackageCacheInfo(remotePair.Key, remotePair.Value.Md5, remotePair.Value.Size));
                }
            }

            // 遍历本地AB信息集合中是否有剩余的内容，有就说明剩下的AB包是需要删除的资源，先删除后下载
            foreach (KeyValuePair<string, ABPackageInfo> localPair in localCollection)
            {
                // 对于PC平台，删除存在的AB包文件
                if (File.Exists(PathManager.GetAbLoadPath(localPair.Key)))
                {
                    File.Delete(PathManager.GetAbLoadPath(localPair.Key));
                }
            }

            // 异步获取本地的AB包缓存文件内容
            string cache = await File.ReadAllTextAsync(PathManager.GetAbLoadPath(FileUtility.CacheDefaultName));
            if (!string.IsNullOrEmpty(cache))
            {
                // 有内容说明之前更新中断过，需要进行对比，决定下载哪些资源和断点续传
                try
                {
                    // 反序列化本地缓存Json文件
                    ABPackageCacheCollection abCacheCollection = JsonManager.Instance.FromJson<ABPackageCacheCollection>(cache);
                    foreach (KeyValuePair<string, ABPackageCacheInfo> cachePair in abCacheCollection)
                    {
                        cachePackageCollection.TryAdd(cachePair.Key, cachePair.Value);
                    }
                }
                catch
                {
                    LogMgr.LogError("反序列化缓存文件失败");
                    return false;
                }
            }

            // 待移除的AB包文件列表，存储不需要下载的AB包名
            List<string> waitRemoveABFileList = new List<string>();

            // 待下载集合与缓存文件集合进行对比
            // 遍历待下载集合
            foreach (KeyValuePair<string, ABPackageCacheInfo> waitPair in waitDownloadCollection)
            {
                // 如果记录的文件没有AB包名，说明是之前没有下载过的，也要下载
                if (!cachePackageCollection.ContainsKey(waitPair.Key))
                {
                    continue;
                }

                // 如果缓存文件有该AB包名，说明之前下载过该AB包，需判断是否是最新的
                // 若不等于说明待下载集合的资源是最新的，就要下载，覆盖上次旧的AB包资源
                if (cachePackageCollection[waitPair.Key].Md5 != waitPair.Value.Md5)
                {
                    continue;
                }

                // 缓存文件的AB包MD5码等于待下载字典的该AB包的MD5码，说明记录文件的AB包的资源是最新的
                // 判断如果下载完成就不用下载了
                if (cachePackageCollection[waitPair.Key].IsSuccess)
                {
                    // 记录进待移除的AB包文件列表
                    waitRemoveABFileList.Add(waitPair.Key);
                }
                // 若上次下载未完成, 也要继续接着下载
                else
                {
                    waitPair.Value.DownloadedBytes = cachePackageCollection[waitPair.Key].DownloadedBytes;
                }
            }

            // 移除待下载字典中不用下载的AB包名
            for (int i = 0; i < waitRemoveABFileList.Count; i++)
            {
                waitDownloadCollection.Remove(waitRemoveABFileList[i]);
            }

            return true;
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.CompareContrast;
    }
}
