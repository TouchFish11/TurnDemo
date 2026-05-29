using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Collection;
using Core.DI;
using Core.Serialize.Json;
using Core.Utility;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 更新服务
    /// </summary>
    public class UpdateService
    {
        [Inject] private IJsonManager _jsonManager;
        
        /// <summary>
        /// 将缓存集合写入本地JSON文件，持久化缓存信息，用于下次启动时断点续传
        /// </summary>
        public void WriteCacheFile(AbPackageCacheCollection cachePackageCollection)
        {
            var cacheFilePath = PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName);
            _jsonManager.SaveToJson(cachePackageCollection, cacheFilePath);
        }
        
        /// <summary>
        /// 将缓存集合异步写入本地JSON文件，持久化缓存信息，用于下次启动时断点续传
        /// </summary>
        /// <returns></returns>
        public async Task WriteCacheFileAsync(AbPackageCacheCollection cachePackageCollection)
        {
            var cacheFilePath = PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName);
            await _jsonManager.SaveToJsonAsync(cachePackageCollection, cacheFilePath);
        }
        
        /// <summary>
        /// 取消所有下载请求并保存缓存信息，且将IsPauseDownload设置为true；保存未完成下载的AB包缓存信息,将缓存信息写入本地文件
        /// </summary>
        public void CancelDownload(ABUpdateContext context)
        {
            // 标记暂停下载
            context.IsPauseDownload = true;
            // 终止所有正在下载的请求
            AbortRequests(context.RequesterLoadingList);
            
            // 临时收集所有未完成的请求（失败、下载中、等待）
            var list = new List<ABWebRequester>();
            list.AddRange(context.RequesterFailList);
            list.AddRange(context.RequesterLoadingList);
            list.AddRange(context.RequesterWaitList);
            
            // 遍历列表，保存未完成AB包的缓存信息
            foreach (var abWebRequester in list)
            {
                var abLoadPath = PathUtility.GetAbLoadPath(abWebRequester.AbName.WithAbSuffix());
                // 本地文件不存在则跳过（未开始下载）
                if (!File.Exists(abLoadPath)) 
                    continue;
                // 获取本地文件信息
                var fileInfo = new FileInfo(abLoadPath);
                // 封装缓存信息
                var cacheInfo = new AbPackageCacheInfo(abWebRequester.AbName, abWebRequester.Hash, fileInfo.Length);
                // 更新缓存集合
                UpdateCacheFile(context, cacheInfo);
            }
            
            // 将缓存信息写入本地文件
            WriteCacheFile(context.CachePackageCollection);
        }
        
        /// <summary>
        /// 取消所有下载请求并保存缓存信息，且将IsPauseDownload设置为true；保存未完成下载的AB包缓存信息,将缓存信息异步写入本地文件
        /// </summary>
        /// <returns></returns>
        public async Task CancelDownloadAsync(ABUpdateContext context)
        {
            // 标记暂停下载
            context.IsPauseDownload = true;
            // 终止所有正在下载的请求
            AbortRequests(context.RequesterLoadingList);
            
            // 收集所有未完成的请求（失败、下载中、等待）
            var list = new List<ABWebRequester>();
            list.AddRange(context.RequesterFailList);
            list.AddRange(context.RequesterLoadingList);
            list.AddRange(context.RequesterWaitList);
            
            // 遍历列表，保存未完成AB包的缓存信息
            foreach (var abWebRequester in list)
            {
                var abLoadPath = PathUtility.GetAbLoadPath(abWebRequester.AbName);
                // 本地文件不存在则跳过（未开始下载）
                if (!File.Exists(abLoadPath)) continue;
                // 获取本地文件信息
                var fileInfo = new FileInfo(abLoadPath);
                // 封装缓存信息
                var cacheInfo = new AbPackageCacheInfo(abWebRequester.AbName, abWebRequester.Hash, fileInfo.Length);
                // 更新缓存集合
                UpdateCacheFile(context, cacheInfo);
            }
            
            // 将缓存信息异步写入本地文件
            await WriteCacheFileAsync(context.CachePackageCollection);
        }

        /// <summary>
        /// 更新AB包缓存信息
        /// 若缓存集合中已存在该AB包，则更新Hash、已下载字节数、完成状态；
        /// 若不存在，则添加新的缓存信息到集合
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheInfo">待更新的AB包缓存信息</param>
        public void UpdateCacheFile(ABUpdateContext context, AbPackageCacheInfo cacheInfo)
        {
            // 检查缓存集合中是否已存在该AB包
            if (context.CachePackageCollection.TryGetValue(cacheInfo.AbName, out var aBPackageCacheInfo))
            {
                // 更新已有缓存信息
                aBPackageCacheInfo.Hash = cacheInfo.Hash;
                aBPackageCacheInfo.DownloadedBytes = cacheInfo.DownloadedBytes;
                // 标记是否下载完成（已下载字节数等于远程包总大小）
                aBPackageCacheInfo.IsSuccess = cacheInfo.DownloadedBytes == context.RemotePackageCollection[cacheInfo.AbName].Size;
            }
            else
            {
                // 标记是否下载完成
                cacheInfo.IsSuccess = cacheInfo.DownloadedBytes == context.RemotePackageCollection[cacheInfo.AbName].Size;
                // 添加新缓存信息到集合
                context.CachePackageCollection.TryAdd(cacheInfo.AbName, cacheInfo);
            }
        }

        /// <summary>
        /// 处理失败队列中的请求
        /// 遍历失败请求，若还有重试次数则移回等待队列，减少重试计数
        /// 无重试次数的请求会保留在失败队列
        /// </summary>
        /// <param name="context"></param>
        public void HandleFailRequester(ABUpdateContext context)
        {
            if (context.RequesterFailList.Count <= 0)
            {
                return;
            }
            
            // 获取失败队列首个节点
            var failedRequesterNode = context.RequesterFailList.First;
            // 获取节点对应的请求对象
            var failedRequester = failedRequesterNode.Value;
            while (failedRequesterNode != null)
            {
                // 还有重试次数则重试
                if (failedRequester.CurrentRetryCount != 0)
                {
                    context.RequesterFailList.RemoveFirst();
                    context.RequesterWaitList.AddLast(failedRequesterNode);
                    // 减少重试次数
                    failedRequester.SubRetryCount();
                }
                // 移动到下一个失败请求节点
                failedRequesterNode = failedRequesterNode.Next;
            }
        }
        
        /// <summary>
        /// 终止所有正在下载的请求
        /// </summary>
        private void AbortRequests(LinkedList<ABWebRequester> requesterLoadingList)
        {
            var node = requesterLoadingList.First;
            while (node != null)
            {
                // 先保存下一个节点
                var nextNode = node.Next; 
                node.Value.Abort();
                node = nextNode;
            }
        }
    }
}
