using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using Core.AssetBundles.Update.Collection;
using Core.AssetBundles.Update.Enum;
using Core.Extensions;
using Core.Global;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Service;
using Core.Utility;

namespace Core.AssetBundles.Update.State
{
    using Time = UnityEngine.Time;

    /// <summary>
    /// 资源下载状态类
    /// 负责批量下载待更新的AssetBundle资源，支持并发下载、进度回调、下载速度更新
    /// </summary>
    public class DownLoadAssetState : UpdateState
    {
        // 上次更新下载速度的时间戳
        private float _lastSpeedUpdateTime;
        // 下载速度更新间隔（秒）
        private readonly float _speedUpdateInterval;
        // 是否正在下载中
        private bool _isDownloading;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="updater">AssetBundle更新器实例</param>
        public DownLoadAssetState(AssetBundleUpdater updater) : base(updater)
        {
            _speedUpdateInterval = GlobalSettings.Instance.speedUpdateInterval;
        }

        /// <summary>
        /// 执行资源下载核心逻辑
        /// </summary>
        /// <returns>是否下载成功</returns>
        public override async Task<UpdateResult> Execute()
        {
            try
            {
                // 获取需要下载的总字节数
                var downLoadTotalBytes = (ulong)ABPackageCollection.GetTotalDownLoadBytes(
                    assetBundleUpdater.GetContext().RemotePackageCollection,
                    assetBundleUpdater.GetContext().WaitDownloadCollection
                );
                
                // 初始化下载速度更新
                ServiceLocator.Get<IMonoAdapter>().StartCoroutine(UpdateSpeed());
                
                // 异步下载资源，传入进度回调，更新下载进度
                await DownLoadAssetsAsync(bytesPerFrame => assetBundleUpdater.GetContext().UpdateProgress(bytesPerFrame, downLoadTotalBytes));
                
                // 标记下载结束
                _isDownloading = false;
            }
            catch (System.Exception exception)
            {
                return UpdateResult.CreateFailure("资源下载未完成", exception);
            }
            
            return UpdateResult.CreateSuccess();
        }

        /// <summary>
        /// 异步下载AssetBundle资源
        /// 支持并发下载、断点续传、失败重试、进度回调
        /// </summary>
        /// <param name="proCallBack">下载进度回调（参数：本次帧下载的字节数）</param>
        /// <returns>是否全部下载成功</returns>
        public async Task DownLoadAssetsAsync(Action<ulong> proCallBack)
        {
            // 获取资源服务器IP
            var serverIp = GlobalSettings.Instance.resServerIp;
            // 获取待下载的AssetBundle集合
            var waitDownloadCollection = assetBundleUpdater.GetContext().WaitDownloadCollection;
            
            // 初始化所有待下载资源的请求器
            foreach (var pair in waitDownloadCollection)
            {
                var cacheInfo = waitDownloadCollection[pair.Key];
                // 创建AB包下载请求器
                var abWebRequester = ServiceLocator.Get<IPoolManager>().GetData<ABWebRequester>();
                abWebRequester.Init(serverIp, cacheInfo.AbName, true, cacheInfo.AbName, cacheInfo.Hash);
                // 绑定下载进度回调
                abWebRequester.OnDownloadProgress += proCallBack;
                // 将请求器加入待下载队列
                assetBundleUpdater.GetContext().AddRequesterToWait(abWebRequester);
                LogManager.Log($"{abWebRequester.AbName}资源，即将下载");
            }

            // 获取最大并发下载数
            var maxConcurrencyNum = GlobalSettings.Instance.maxConcurrencyNum;
            var context = assetBundleUpdater.GetContext();

            /*
             * 下载循环逻辑：
             * 未暂停下载且存在待下载/正在下载的请求时，持续执行
             * 控制并发数，待下载队列有请求且并发数未达上限时，启动新下载
             * 处理下载失败的请求，更新失败队列
             */
            while (!context.IsPauseDownload && 
                   (context.WaitListCount > 0 || context.LoadListCount > 0 || !(context.WaitListCount == 0 && context.LoadListCount == 0 && context.FailListCount >= 0)))
            {
                // 启动新的下载请求（控制并发数）
                while (context.LoadListCount < maxConcurrencyNum && context.WaitListCount > 0)
                {
                    // 取出待下载队列首个请求器
                    var requester = context.GetFirstRequester();
                    // 加入正在下载队列
                    context.AddRequesterToLoad(requester);
                    // 异步执行下载
                    requester.DownLoadAsync(PathUtility.GetAbLoadPath(requester.FileName), isOver =>
                    {
                        // 下载完成后，从正在下载队列移除
                        context.RemoveRequesterFromLoad(requester);
                        // 下载成功
                        if (isOver)
                        {
                            LogManager.Log($"下载成功：{requester.FileName}");
                            // 获取下载后的文件信息
                            var fileInfo = new FileInfo(PathUtility.GetAbLoadPath(requester.FileName));
                            // 更新缓存信息
                            var cacheInfo = new AbPackageCacheInfo(requester.FileName, context.RemotePackageCollection[requester.FileName].Hash, fileInfo.Length);
                            assetBundleUpdater.GetContext().UpdateCacheFile(cacheInfo);
                        }
                        // 下载失败，加入失败队列
                        else
                        {
                            context.AddRequesterToFail(requester);
                        }
                    });
                    
                    await Task.Yield(); // 帧间等待，避免阻塞主线程
                }

                // 等待正在下载的请求完成（直到并发数低于上限或待下载队列为空）
                if (context.LoadListCount > 0)
                {
                    await TaskUtility.WaitUntil(() => context.LoadListCount < maxConcurrencyNum || context.WaitListCount == 0);
                }

                // 处理下载失败的请求（重试/标记失败）
                context.HandleFailReqeuster();

                await Task.Yield(); // 帧间等待
            }

            await CheckDownloadSuccess(context);
        }

        /// <summary>
        /// 检查下载是否成功
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task CheckDownloadSuccess(ABUpdateContext context)
        {
            // 校验所有缓存包是否下载成功
            foreach (var condition in context.CachePackageCollection.Values.MeetConditions(info => info.IsSuccess))
            {
                if (condition)
                {
                    await Task.Yield();
                }
                else
                {
                    throw new System.Exception("下载不完整，存在缺失资源");
                }
            }
        }

        /// <summary>
        /// 循环更新下载速度
        /// 按配置的间隔时间，持续更新当前下载速度到上下文
        /// </summary>
        private IEnumerator UpdateSpeed()
        {
            // 初始化上次更新时间为当前时间
            _lastSpeedUpdateTime = Time.realtimeSinceStartup;
            _isDownloading = true;
            
            while (_isDownloading)
            {
                // 达到速度更新间隔，执行更新
                if (Time.realtimeSinceStartup - _lastSpeedUpdateTime >= _speedUpdateInterval)
                {
                    assetBundleUpdater.GetContext().UpdateSpeed();
                    _lastSpeedUpdateTime = Time.realtimeSinceStartup;
                }

                yield return null;
            }
        }

        /// <summary>
        /// 当前更新阶段标识
        /// </summary>
        public override EUpdatePhase UpdatePhase => EUpdatePhase.DownLoadAssets;
    }
}