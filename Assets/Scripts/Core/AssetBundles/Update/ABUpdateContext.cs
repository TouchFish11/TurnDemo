using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Collection;
using Core.AssetBundles.Update.Enum;
using Core.DataPersistence.Json;
using Core.Log;
using Core.Service;
using Core.Utility;

namespace Core.AssetBundles.Update
{
    /// <summary>
    /// AssetBundle 更新上下文类
    /// 负责管理AB包更新过程中的所有状态、数据、请求队列及事件回调
    /// </summary>
    public class ABUpdateContext
    {
        /// <summary>
        /// 存储远程服务器端的AB包信息集合
        /// 包含所有需要更新的AB包的元数据（名称、Hash、大小等）
        /// </summary>
        public ABPackageCollection RemotePackageCollection { get; }

        /// <summary>
        /// 存储本地已加载的AB包信息集合
        /// 记录本地已存在的AB包元数据，用于和远程对比判断是否需要更新
        /// </summary>
        public ABPackageCollection LocalPackageCollection { get; }

        /// <summary>
        /// 存储已缓存的AB包信息集合
        /// 记录AB包的下载缓存状态（已下载字节数、Hash、是否下载完成等）
        /// </summary>
        public AbPackageCacheCollection CachePackageCollection { get; }

        /// <summary>
        /// 存储等待下载的AB包信息集合
        /// 标记需要下载但尚未开始的AB包
        /// </summary>
        public AbPackageCacheCollection WaitDownloadCollection { get; }

        /// <summary>
        /// 存储未完成下载的AB包名称列表
        /// 记录所有下载中断/未完成的AB包名称，用于断点续传或重试逻辑
        /// </summary>
        private readonly List<string> _incompleteABList = new();

        /// <summary>
        /// 存储等待下载的网络请求队列
        /// 待执行的AB包下载请求，按添加顺序排队执行
        /// </summary>
        private readonly LinkedList<ABWebRequester> _requesterWaitList = new();

        /// <summary>
        /// 存储下载失败的网络请求队列
        /// 下载失败的请求，用于重试逻辑处理
        /// </summary>
        private readonly LinkedList<ABWebRequester> _requesterFailList = new();

        /// <summary>
        /// 存储正在下载的网络请求队列
        /// 正在执行的AB包下载请求，用于监控下载状态、取消下载等操作
        /// </summary>
        private readonly LinkedList<ABWebRequester> _requesterLoadingList = new();

        /// <summary>
        /// 是否暂停下载
        /// 标记全局下载状态，暂停后新的下载请求不会执行，正在下载的请求可被取消
        /// </summary>
        public bool IsPauseDownload { get; set; }

        /// <summary>
        /// 等待队列中的请求数量
        /// 只读属性，返回待下载请求的总数
        /// </summary>
        public int WaitListCount => _requesterWaitList.Count;

        /// <summary>
        /// 下载中队列的请求数量
        /// 只读属性，返回正在下载的请求总数
        /// </summary>
        public int LoadListCount => _requesterLoadingList.Count;

        /// <summary>
        /// 失败队列中的请求数量
        /// 只读属性，返回下载失败的请求总数
        /// </summary>
        public int FailListCount => _requesterFailList.Count;

        /// <summary>
        /// 未完成队列中的请求数量
        /// 只读属性，返回未完成下载的请求总数（同下载中队列数量）
        /// </summary>
        public int IncompleteListCount => _incompleteABList.Count;

        /// <summary>
        /// 下载进度回调事件
        /// 参数说明：
        /// long: 已下载的总字节数
        /// long: 需下载的总字节数
        /// </summary>
        public event Action<ulong, ulong> OnProgress;

        /// <summary>
        /// 资源检查进度回调事件
        /// 参数说明：
        /// int: 当前检查完成的AB包数量
        /// int: 需检查的AB包总数量
        /// </summary>
        public event Action<int, int> OnCheckProgress;

        /// <summary>
        /// 更新阶段变更回调事件
        /// 参数说明：
        /// E_UpdatePhase: 当前更新所处的阶段（检查、下载、完成等）
        /// </summary>
        public event Action<EUpdatePhase> OnUpdatePhase;

        /// <summary>
        /// 下载速度回调事件
        /// 参数说明：
        /// long: 本次回调周期内的下载字节数（用于计算下载速度）
        /// </summary>
        public event Action<ulong> OnUpdateSpeed;

        /// <summary>
        /// 更新完成回调事件
        /// 所有AB包下载/检查完成后触发
        /// </summary>
        public event Action OnUpdateFinish;
        
        /// <summary>
        /// 更新失败结果事件
        /// </summary>
        public event Action<UpdateResult> OnUpdateFailResult;

        // 当前已下载的字节数（累计）
        private ulong currentDownloadedBytes;
        // 当前帧下载的总字节数（用于计算下载速度）
        private ulong _currentDownloadTotalSizes;
        
        /// <summary>
        /// 构造函数
        /// 初始化所有AB包信息集合和缓存集合
        /// </summary>
        public ABUpdateContext()
        {
            RemotePackageCollection = new ABPackageCollection();
            LocalPackageCollection = new ABPackageCollection();
            WaitDownloadCollection = new AbPackageCacheCollection();
            CachePackageCollection = new AbPackageCacheCollection();
        }

        public void UpdateFailed(UpdateResult updateResult)
        {
            OnUpdateFailResult?.Invoke(updateResult);
        }

        /// <summary>
        /// 将下载请求添加到等待队列
        /// </summary>
        /// <param name="requester">待添加的AB包下载请求对象</param>
        public void AddRequesterToWait(ABWebRequester requester)
        {
            _requesterWaitList.AddLast(new LinkedListNode<ABWebRequester>(requester));
        }

        /// <summary>
        /// 将下载请求添加到下载中队列
        /// </summary>
        /// <param name="requester">待添加的AB包下载请求对象</param>
        public void AddRequesterToLoad(ABWebRequester requester)
        {
            _requesterLoadingList.AddLast(new LinkedListNode<ABWebRequester>(requester));
        }

        /// <summary>
        /// 将下载请求添加到失败队列
        /// </summary>
        /// <param name="requester">待添加的AB包下载请求对象</param>
        public void AddRequesterToFail(ABWebRequester requester)
        {
            _requesterFailList.AddLast(new LinkedListNode<ABWebRequester>(requester));
        }

        /// <summary>
        /// 将AB包名称添加到未完成下载列表
        /// </summary>
        /// <param name="abName">未完成下载的AB包名称</param>
        public void AddABNameToIncomplete(string abName)
        {
            _incompleteABList.Add(abName);
        }

        /// <summary>
        /// 获取等待队列中的第一个下载请求
        /// 获取后会从等待队列移除该请求，用于启动下一个下载任务
        /// </summary>
        /// <returns>等待队列首个AB包下载请求对象</returns>
        public ABWebRequester GetFirstRequester()
        {
            var requester = _requesterWaitList.First;
            _requesterWaitList.RemoveFirst();
            return requester.Value;
        }

        /// <summary>
        /// 从下载中队列移除指定的下载请求
        /// 用于下载完成/失败后清理队列
        /// </summary>
        /// <param name="requester">需要移除的AB包下载请求对象</param>
        /// <returns>移除结果：true=移除成功，false=队列中无该请求</returns>
        public bool RemoveRequesterFromLoad(ABWebRequester requester)
        {
            return _requesterLoadingList.Remove(requester);
        }

        /// <summary>
        /// 处理失败队列中的请求
        /// 遍历失败请求，若还有重试次数则移回等待队列，减少重试计数
        /// 无重试次数的请求会保留在失败队列（可后续手动处理）
        /// </summary>
        public void HandleFailReqeuster()
        {
            if (FailListCount <= 0)
            {
                return;
            }
            
            // 获取失败队列首个节点
            var failedRequesterNode = _requesterFailList.First;
            // 获取节点对应的请求对象
            var failedRequester = failedRequesterNode.Value;
            while (failedRequesterNode != null)
            {
                // 还有重试次数则重试
                if (failedRequester.CurrentRetryCount != 0)
                {
                    _requesterFailList.RemoveFirst();
                    _requesterWaitList.AddLast(failedRequesterNode);
                    // 减少重试次数
                    failedRequester.SubRetryCount();
                }
                // 移动到下一个失败请求节点
                failedRequesterNode = failedRequesterNode.Next;
            }
        }
    
        /// <summary>
        /// 触发更新阶段变更事件
        /// 若当前处于暂停状态，则不触发事件
        /// </summary>
        /// <param name="updatePhase">当前更新阶段枚举值</param>
        public void UpdatePhase(EUpdatePhase updatePhase)
        {
            if (IsPauseDownload)
            {
                return;
            }

            OnUpdatePhase?.Invoke(updatePhase);
        }

        /// <summary>
        /// 更新下载进度并触发进度事件
        /// 累计已下载字节数，更新总下载量，并回调进度事件
        /// </summary>
        /// <param name="bytesPerFrame">当前帧下载的字节数</param>
        /// <param name="downLoadTotalBytes">需要下载的总字节数</param>
        public void UpdateProgress(ulong bytesPerFrame, ulong downLoadTotalBytes)
        {
            // 累计当前帧下载字节数（用于计算下载速度）
            _currentDownloadTotalSizes += bytesPerFrame;
            // 累计总已下载字节数
            currentDownloadedBytes += bytesPerFrame;
            // 触发进度回调
            OnProgress?.Invoke(currentDownloadedBytes, downLoadTotalBytes);
        }

        /// <summary>
        /// 更新资源检查进度并触发检查进度事件
        /// </summary>
        /// <param name="current">已检查完成的AB包数量</param>
        /// <param name="total">需要检查的AB包总数量</param>
        public void UpdateCheckProgress(int current, int total)
        {
            OnCheckProgress?.Invoke(current, total);
        }

        /// <summary>
        /// 触发下载速度回调事件
        /// 回调当前周期的下载字节数后，重置计数
        /// </summary>
        public void UpdateSpeed()
        {
            if (OnUpdateSpeed == null)
            {
                LogManager.Log($"{nameof(OnUpdateSpeed)}事件为空");
            }

            OnUpdateSpeed?.Invoke(_currentDownloadTotalSizes);
            // 重置当前帧下载字节数，用于下一次速度计算
            _currentDownloadTotalSizes = 0;
        }

        /// <summary>
        /// 触发更新完成回调事件
        /// 所有AB包下载/检查逻辑完成后调用
        /// </summary>
        public void UpdateFinish()
        {
            OnUpdateFinish?.Invoke();
        }

        /// <summary>
        /// 从失败队列中获取所有失败请求的缓存信息
        /// 遍历失败请求，读取本地已下载的文件信息，封装为缓存信息对象返回
        /// </summary>
        /// <returns>失败请求对应的AB包缓存信息枚举集合</returns>
        public IEnumerable<AbPackageCacheInfo> GetCacheInfosFromFail()
        {
            // 遍历失败队列首个节点
            var node = _requesterFailList.First;
            while (node != null)
            {
                // 获取AB包本地加载路径对应的文件信息
                var fileInfo = new FileInfo(PathUtility.GetAbLoadPath(node.Value.FileName));
                // 封装缓存信息（名称、Hash、已下载字节数）
                var cacheInfo = new AbPackageCacheInfo(node.Value.AbName, node.Value.Hash, fileInfo.Length);
                // 移动到下一个失败请求节点
                node = node.Next;
                yield return cacheInfo;
            }
        }

        /// <summary>
        /// 取消所有下载请求并保存缓存信息
        /// 标记暂停下载
        /// 终止所有正在下载的请求
        /// 保存未完成下载的AB包缓存信息（断点续传）
        /// 将缓存信息写入本地文件
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task CancelDownload()
        {
            // 标记暂停下载
            IsPauseDownload = true;
            // 终止并释放所有正在下载的请求
            var node = _requesterLoadingList.First;
            while (node != null)
            {
                node.Value.Abort(); // 终止下载请求
                node.Value.Dispose(); // 释放请求资源
                node = node.Next;
            }

            // 临时收集所有未完成的请求（失败、下载中、等待）
            var tempList = new List<ABWebRequester>();
            tempList.AddRange(_requesterFailList);
            tempList.AddRange(_requesterLoadingList);
            tempList.AddRange(_requesterWaitList);

            // 遍历临时列表，保存未完成AB包的缓存信息
            foreach (var abWebRequester in tempList)
            {
                var abLoadPath = PathUtility.GetAbLoadPath(abWebRequester.AbName);
                // 本地文件不存在则跳过（未开始下载）
                if (!File.Exists(abLoadPath))
                {
                    continue;
                }

                // 获取本地文件信息
                var fileInfo = new FileInfo(abLoadPath);
                // 封装缓存信息
                var cacheInfo = new AbPackageCacheInfo(abWebRequester.AbName, abWebRequester.Hash, fileInfo.Length);
                // 更新缓存集合
                UpdateCacheFile(cacheInfo);
            }

            // 将缓存信息写入本地文件（持久化，用于断点续传）
            await WriteCacheFile();
        }

        /// <summary>
        /// 更新AB包缓存信息
        /// 若缓存集合中已存在该AB包，则更新Hash、已下载字节数、完成状态；
        /// 若不存在，则添加新的缓存信息到集合
        /// </summary>
        /// <param name="cacheInfo">待更新的AB包缓存信息</param>
        public void UpdateCacheFile(AbPackageCacheInfo cacheInfo)
        {
            // 检查缓存集合中是否已存在该AB包
            if (CachePackageCollection.TryGetValue(cacheInfo.AbName, out var aBPackageCacheInfo))
            {
                // 更新已有缓存信息
                aBPackageCacheInfo.Hash = cacheInfo.Hash;
                aBPackageCacheInfo.DownloadedBytes = cacheInfo.DownloadedBytes;
                // 标记是否下载完成（已下载字节数等于远程包总大小）
                aBPackageCacheInfo.IsSuccess = cacheInfo.DownloadedBytes == RemotePackageCollection[cacheInfo.AbName].Size;
            }
            else
            {
                // 标记是否下载完成
                cacheInfo.IsSuccess = cacheInfo.DownloadedBytes == RemotePackageCollection[cacheInfo.AbName].Size;
                // 添加新缓存信息到集合
                CachePackageCollection.TryAdd(cacheInfo.AbName, cacheInfo);
            }
        }

        /// <summary>
        /// 将缓存集合写入本地JSON文件
        /// 持久化缓存信息，用于下次启动时断点续传
        /// </summary>
        /// <returns>异步任务</returns>
        public async Task WriteCacheFile()
        {
            var cacheFilePath = PathUtility.GetAbLoadPath(FileUtility.CacheDefaultName);
            await ServiceLocator.Get<IJsonManager>().SaveToJsonAsync(CachePackageCollection, cacheFilePath);
        }

        /// <summary>
        /// 重置所有更新上下文数据
        /// 清空所有集合、队列、事件回调，重置下载计数和暂停状态
        /// 用于重新开始更新流程时清理旧数据
        /// </summary>
        public void ResetData()
        {
            // 清空AB包信息集合
            RemotePackageCollection.Clear();
            LocalPackageCollection.Clear();
            WaitDownloadCollection.Clear();
            CachePackageCollection.Clear();

            // 清空各类队列
            _incompleteABList.Clear();
            _requesterWaitList.Clear();
            _requesterLoadingList.Clear();
            _requesterFailList.Clear();

            // 清空事件回调
            OnProgress = null;
            OnCheckProgress = null;
            OnUpdatePhase = null;
            OnUpdateSpeed = null;
            OnUpdateFinish = null;

            // 重置下载计数和状态
            currentDownloadedBytes = 0;
            IsPauseDownload = false;
        }
        
        ulong totalBytes;

        public void AddBytesTest(ulong totalBytes)
        {
            this.totalBytes += totalBytes;
            LogManager.Log($"下载总量：{this.totalBytes}");
        }
    }
}