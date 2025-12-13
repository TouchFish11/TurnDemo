using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Framework
{
    /// <summary>
    /// 下载资源状态
    /// </summary>
    public class DownLoadAssetState : UpdateState
    {
        //上次速度更新时间
        private float _lastSpeedUpdateTime;
        //速度更新间隔
        private float _speedUpdateInterval;
        //是否开始下载
        private bool _isDownloading;

        public DownLoadAssetState(AssetBundleUpdater updater) : base(updater)
        {
            _speedUpdateInterval = GlobalSettings.Instance.SpeedUpdateInterval;
        }

        public override async Task<bool> Execute()
        {
            // 获取当前下载的总字节数
            long downLoadTotalBytes = ABPackageCollection.GetTotalDownLoadBytes(assetBundleUpdater.GetContext().RemotePackageCollection, assetBundleUpdater.GetContext().CachePackageCollection);
            // 定期更新速度
            UpdateSpeed();
            // 下载AB包资源；外部需加锁累加下载字节数
            IsSuceess = await DownLoadAssetsAsync((bytesPerFrame) =>
            {
                // 更新进度
                assetBundleUpdater.GetContext().UpdateProgress(bytesPerFrame, downLoadTotalBytes);
            });

            // 下载结束
            _isDownloading = false;
            if (!IsSuceess)
            {
                LogManager.Log("资源未下载完整");
                FinishUpdate();
                return IsSuceess;
            }

            // 切换至检查资源完整性状态
            assetBundleUpdater.ChangeState(E_UpdatePhase.CheckAssetsIntegrity);
            return IsSuceess;
        }

        /// <summary>
        /// 异步下载资源
        /// </summary>
        /// <param name="proCallBack">下载进度回调</param>
        public async Task<bool> DownLoadAssetsAsync(UnityAction<long> proCallBack)
        {
            // 记录请求的服务器地址
            string serverIp = GlobalSettings.Instance.ResServerIp;
            // 遍历待下载字典
            ABPackageCacheCollection waitDownloadCollection = assetBundleUpdater.GetContext().WaitDownloadCollection;
            foreach (KeyValuePair<string, ABPackageCacheInfo> pair in waitDownloadCollection)
            {
                ABPackageCacheInfo cacheInfo = waitDownloadCollection[pair.Key];
                // 创建请求者
                ABWebRequester abWebRequester = new ABWebRequester(serverIp, cacheInfo.AbName, true, cacheInfo.AbName, cacheInfo.Md5);
                // 监听进度回调
                abWebRequester.OnDownloadProgress += proCallBack;
                // 存储待下载的请求者
                assetBundleUpdater.GetContext().AddRequesterToWait(abWebRequester);
            }

            //记录最大并发数
            int maxConcurrencyNum = GlobalSettings.Instance.MaxConcurrencyNum;

            /*
             * 暂停优先级最高，未暂停则继续下载，暂停则退出下载；
             * 待下载链表中有请求，或者有正在下载的资源,则继续下载；
             * 待下载链表、正在下载链表无请求，但下载失败链表有请求，则暂停下载；
            */
            ABUpdateContext context = assetBundleUpdater.GetContext();
            while (!context.IsPauseDownload && (context.WaitListCount > 0 || context.LoadListCount > 0 ||
                !(context.WaitListCount == 0 && context.LoadListCount == 0 && context.FailListCount >= 0)))
            {
                // 正在下载的资源数小于最大并发数且有要下载的内容，才去下载资源
                while (context.LoadListCount < maxConcurrencyNum && context.WaitListCount > 0)
                {
                    ABWebRequester requester = context.GetFirstRequester();
                    // 取出第一个请求，放入正在下载列表
                    context.AddRequesterToLoad(requester);
                    // 调用请求者的下载方法
                    requester.DownLoadAsync(PathManager.GetAbLoadPath(requester.FileName), (isOver) =>
                    {
                        // 无论是否下载成功，都是下载结束，从正在下载的列表中移除
                        context.RemoveRequesterFromLoad(requester);
                        // 下载成功
                        if (isOver)
                        {
                            LogManager.Log($"下载成功：{requester.FileName}");
                            // 获取文件信息
                            FileInfo fileInfo = new FileInfo(PathManager.GetAbLoadPath(requester.FileName));
                            // 构建记录信息对象
                            ABPackageCacheInfo cacheInfo = new ABPackageCacheInfo(requester.FileName, context.RemotePackageCollection[requester.FileName].Md5, fileInfo.Length);
                            // 更新缓存文件信息
                            assetBundleUpdater.GetContext().UpdateCacheFile(cacheInfo);
                        }
                        // 下载失败，添加到下载失败的列表中
                        else
                        {
                            context.AddRequesterToFail(requester);
                        }
                    });

                    await Task.Yield();
                }

                // 有正在下载的资源
                if (context.LoadListCount > 0)
                {
                    // 等待正在下载数小于最大并发数或者没有要下载的资源
                    await TaskUtility.WaitUntil(() => context.LoadListCount < maxConcurrencyNum || context.WaitListCount == 0);
                }

                // 处理下载失败的任务
                context.HandleFailReqeuster();

                await Task.Yield();
            }

            LogManager.Log("下载结束");

            // 判断全部是否下载成功
            bool isAllSuccess = true;
            foreach (KeyValuePair<string, ABPackageCacheInfo> pair in context.CachePackageCollection)
            {
                ABPackageCacheInfo cacheInfo = context.CachePackageCollection[pair.Key];
                if (!cacheInfo.IsSuccess)
                {
                    isAllSuccess = false;
                    break;
                }
                await Task.Yield();
            }

            // AB包下载结束回调
            return isAllSuccess;
        }

        /// <summary>
        /// 更新速度
        /// </summary>
        /// <returns></returns>
        private async void UpdateSpeed()
        {
            //初始化上次更新时间为当前时间
            _lastSpeedUpdateTime = Time.realtimeSinceStartup;
            //开始下载
            _isDownloading = true;
            while (_isDownloading)
            {
                if (Time.realtimeSinceStartup - _lastSpeedUpdateTime >= _speedUpdateInterval)
                {
                    // 更新速度
                    assetBundleUpdater.GetContext().UpdateSpeed();
                    // 更新当前时间为上次更新时间
                    _lastSpeedUpdateTime = Time.realtimeSinceStartup;
                }
                await Task.Yield();
            }
        }

        public override E_UpdatePhase UpdatePhase => E_UpdatePhase.DownLoadAssets;
    }
}
