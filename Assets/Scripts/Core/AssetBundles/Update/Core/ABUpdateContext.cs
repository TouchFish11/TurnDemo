using System;
using System.Collections.Generic;
using Core.AssetBundles.Collection;
using Core.Pool;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// AssetBundle 更新上下文类
    /// 负责管理AB包更新过程中的所有状态、数据、请求队列及事件回调
    /// </summary>
    public class ABUpdateContext : IPoolData
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
        /// 存储等待下载的网络请求队列
        /// 待执行的AB包下载请求，按添加顺序排队执行
        /// </summary>
        public LinkedList<ABWebRequester> RequesterWaitList { get; }

        /// <summary>
        /// 存储下载失败的网络请求队列
        /// 下载失败的请求，用于重试逻辑处理
        /// </summary>
        public LinkedList<ABWebRequester> RequesterFailList { get; }

        /// <summary>
        /// 存储正在下载的网络请求队列
        /// 正在执行的AB包下载请求，用于监控下载状态、取消下载等操作
        /// </summary>
        public LinkedList<ABWebRequester> RequesterLoadingList { get; }

        /// <summary>
        /// 是否暂停下载
        /// 标记全局下载状态，暂停后新的下载请求不会执行，正在下载的请求可被取消
        /// </summary>
        public bool IsPauseDownload { get; set; }

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
        /// 更新结束回调事件
        /// </summary>
        public event Action<UpdateResult> OnUpdateOver;

        // 当前已下载的字节数（累计）
        private ulong currentDownloadedBytes;
        // 当前帧下载的总字节数（用于计算下载速度）
        private ulong _currentDownloadTotalSizes;
        
        /// <summary>
        /// 是否存在更新
        /// </summary>
        public bool IsHasUpdate { get; set; }
        
        /// <summary>
        /// 构造函数
        /// 初始化所有AB包信息集合和缓存集合
        /// </summary>
        public ABUpdateContext()
        {
            RequesterWaitList = new LinkedList<ABWebRequester>();
            RequesterFailList = new LinkedList<ABWebRequester>();
            RequesterLoadingList = new LinkedList<ABWebRequester>();
            
            RemotePackageCollection = new ABPackageCollection();
            LocalPackageCollection = new ABPackageCollection();
            WaitDownloadCollection = new AbPackageCacheCollection();
            CachePackageCollection = new AbPackageCacheCollection();
        }

        /// <summary>
        /// 将下载请求添加到等待队列
        /// </summary>
        /// <param name="requester">待添加的AB包下载请求对象</param>
        public void AddRequesterToWait(ABWebRequester requester)
        {
            RequesterWaitList.AddLast(new LinkedListNode<ABWebRequester>(requester));
        }

        /// <summary>
        /// 将下载请求添加到下载中队列
        /// </summary>
        /// <param name="requester">待添加的AB包下载请求对象</param>
        public void AddRequesterToLoad(ABWebRequester requester)
        {
            RequesterLoadingList.AddLast(new LinkedListNode<ABWebRequester>(requester));
        }

        /// <summary>
        /// 将下载请求添加到失败队列
        /// </summary>
        /// <param name="requester">待添加的AB包下载请求对象</param>
        public void AddRequesterToFail(ABWebRequester requester)
        {
            RequesterFailList.AddLast(new LinkedListNode<ABWebRequester>(requester));
        }

        /// <summary>
        /// 获取等待队列中的第一个下载请求
        /// 获取后会从等待队列移除该请求，用于启动下一个下载任务
        /// </summary>
        /// <returns>等待队列首个AB包下载请求对象</returns>
        public ABWebRequester GetFirstRequester()
        {
            var requester = RequesterWaitList.First;
            RequesterWaitList.RemoveFirst();
            return requester.Value;
        }
        
        /// <summary>
        /// 触发更新阶段变更事件
        /// 若当前处于暂停状态，则不触发事件
        /// </summary>
        /// <param name="updatePhase">当前更新阶段枚举值</param>
        public void UpdatePhase(EUpdatePhase updatePhase)
        {
            if (IsPauseDownload) return;
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
            OnUpdateSpeed?.Invoke(_currentDownloadTotalSizes);
            // 重置当前帧下载字节数，用于下一次速度计算
            _currentDownloadTotalSizes = 0;
        }

        /// <summary>
        /// 触发更新完成回调事件
        /// 所有AB包下载/检查逻辑完成后调用
        /// </summary>
        /// <param name="updateResult"></param>
        public void UpdateOver(UpdateResult updateResult)
        {
            OnUpdateOver?.Invoke(updateResult);
        }
        
        /// <summary>
        /// 重置所有更新上下文数据
        /// </summary>
        public void ResetData()
        {
            // 清空AB包信息集合
            RemotePackageCollection.Clear();
            LocalPackageCollection.Clear();
            WaitDownloadCollection.Clear();
            CachePackageCollection.Clear();

            // 清空各类链表
            RequesterWaitList.Clear();
            RequesterLoadingList.Clear();
            RequesterFailList.Clear();

            // 清空事件回调
            OnProgress = null;
            OnCheckProgress = null;
            OnUpdatePhase = null;
            OnUpdateSpeed = null;
            OnUpdateOver = null;

            // 重置下载计数和状态
            currentDownloadedBytes = 0;
            IsPauseDownload = false;
        }
    }
}