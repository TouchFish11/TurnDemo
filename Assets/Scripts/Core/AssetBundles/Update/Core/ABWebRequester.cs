using System;
using System.Collections;
using System.Threading;
using Core.DI;
using Core.Global;
using Core.Log;
using Core.Mono;
using Core.Pool;
using Core.Tasks.Extensions;
using UnityEngine.Networking;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// AssetBundle资源网络请求器
    /// 负责通过UnityWebRequest下载AB包资源，支持断点续传、下载超时检测、下载进度回调、重试次数管理等功能
    /// 实现IDisposable接口，用于释放UnityWebRequest相关资源
    /// </summary>
    public class ABWebRequester : IPoolData
    {
        // Mono适配器
        [Inject] private IMonoAdapter _monoAdapter;
        // AB包更新器
        [Inject] private IAssetBundleUpdater _updater;
        // UnityWebRequest核心请求对象，用于发起网络下载请求
        private UnityWebRequest _request;
        // 取消源
        private CancellationTokenSource _cancellationTokenSource;
        // 是否停止
        private bool _isAbout;

        /// <summary>
        /// 下载进度回调事件（每帧触发，参数为当前帧新增下载的字节数）
        /// 用于外部监听下载速度、总进度等信息
        /// </summary>
        public event Action<ulong> OnDownloadProgress;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="url">下载基础地址（不包含文件名）</param>
        /// <param name="fileName">要下载的文件名（包含扩展名）</param>
        /// <param name="isAppend">是否断点续传：true-追加写入；false-覆盖写入</param>
        /// <param name="abName">对应的AssetBundle包名</param>
        /// <param name="hash">文件Hash校验值</param>
        /// <param name="downloadedBytes">已下载字节数</param>
        public ABWebRequester Init(string url, string fileName, bool isAppend, string abName, string hash, long downloadedBytes)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Url = url;
            FileName = fileName;
            IsAppend = isAppend;
            // 初始化重试次数为全局配置的最大重试次数
            CurrentRetryCount = GlobalSettings.Instance.maxRetryCount;
            AbName = abName;
            Hash = hash;
            DownloadedBytes = downloadedBytes;
            return this;
        }

        /// <summary>
        /// 异步下载文件核心方法
        /// </summary>
        /// <param name="savePath">文件保存的本地完整路径（包含文件名）</param>
        /// <param name="overCallback">下载完成/失败回调（参数为是否下载成功）</param>
        /// <param name="timeout">请求超时时间</param>
        public async void DownLoadAsync(string savePath, Action<bool> overCallback, int timeout)
        {
            var success = false;
            try
            {
                // 初始化UnityWebRequest：创建GET请求
                _request = UnityWebRequest.Get($"{Url}{FileName}");
                // 设置连接超时时间
                _request.timeout = timeout;
                // 设置自定义流下载处理器
                _request.downloadHandler = DIContainer.Create<DownloadHandlerStream>(parameterValues: new object[] { savePath, IsAppend, DownloadedBytes });
                // 设置请求头：Range指定从已下载字节数的位置开始下载
                _request.SetRequestHeader("Range", $"bytes={DownloadedBytes}-");

                // 发送请求
                var asyncOperation = _request.SendWebRequest();
                // 更新进度协程
                _monoAdapter.StartCoroutine(UpdateDownloadProgress(asyncOperation));
                // 发送网络请求
                using var handle = asyncOperation.ToTask(_cancellationTokenSource.Token);
                await handle.Task;

                // 下载结束后处理
                success = _request.result == UnityWebRequest.Result.Success;
                if (!success)
                {
                    // 请求失败：打印错误日志（包含错误信息、响应码），触发失败回调
                    Logger.LogError(TODO, $"[{nameof(ABWebRequester)}]: {FileName} download fail, error={_request?.error}，result={_request?.result}, responseCode={_request?.responseCode}");
                }
            }
            catch (System.Exception e)
            {
                Logger.LogError(TODO, $"[{nameof(ABWebRequester)}]: Download error, {e.Message}");
                success = false;
            }
            finally
            {
                ReleaseRequest();
                // 触发回调
                overCallback?.Invoke(success);
            }
        }

        /// <summary>
        /// 更新下载进度协程
        /// </summary>
        /// <param name="ao"></param>
        /// <returns></returns>
        private IEnumerator UpdateDownloadProgress(UnityWebRequestAsyncOperation ao)
        {
            // 下载进度轮询变量：记录上一帧的下载字节数，用于计算当前帧新增下载量
            ulong lastFrameDownloadBytes = 0;
            ulong currentFrameDownloadBytes;
            // 下载循环：请求未完成、未暂停时持续轮询
            while (_request != null && !_isAbout && !ao.isDone)
            {
                currentFrameDownloadBytes = _request.downloadedBytes;
                // 计算差值
                var deltaBytes =  currentFrameDownloadBytes - lastFrameDownloadBytes;
                // 触发进度回调
                OnDownloadProgress?.Invoke(deltaBytes);
                // 当前帧记录为上一帧
                lastFrameDownloadBytes = currentFrameDownloadBytes;
                yield return null;
            }

            // 停止不传递进度
            if (_isAbout)
            {
                yield break;
            }

            if (_request == null)
                yield break;
            
            currentFrameDownloadBytes = _request.downloadedBytes;
            // 获取上一帧已下载的总字节数
            var finalDelta = currentFrameDownloadBytes - lastFrameDownloadBytes;
            if (finalDelta > 0)
            {
                // 有新数据：触发进度回调
                OnDownloadProgress?.Invoke(finalDelta);
            }
        }

        /// <summary>
        /// 递减剩余重试次数
        /// 重试次数最小为0，避免负数
        /// </summary>
        public void SubRetryCount()
        {
            --CurrentRetryCount;
            if (CurrentRetryCount <= 0)
            {
                CurrentRetryCount = 0;
            }
        }
        
        /// <summary>
        /// 终止当前下载请求
        /// 同时清空下载进度回调，避免空引用或重复回调
        /// </summary>
        public void Abort()
        {
            CancelRequest();
        }

        /// <summary>
        /// 释放请求
        /// </summary>
        private void ReleaseRequest()
        {
            if (_request == null)
                return;
            
            _request.downloadHandler?.Dispose();
            _request.Abort();
            _request.Dispose();
            _request = null;
            
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }

            OnDownloadProgress = null;
        }
        
        /// <summary>
        /// 取消请求
        /// </summary>
        private void CancelRequest()
        {
            _cancellationTokenSource?.Cancel();

            if (_request?.downloadHandler is DownloadHandlerStream handlerStream)
            {
                handlerStream.Pause();
            }

            ReleaseRequest();
            _isAbout = true;
        }
        
        public void ResetData()
        {
            AbName = string.Empty;
            Hash = string.Empty;
            FileName = string.Empty;
            Url = string.Empty;
            IsAppend = false;
            _isAbout = false;
            DownloadedBytes = 0;
            CurrentRetryCount = 0;
        }
        
        /// <summary>
        /// 获取要下载的文件名
        /// 包含拓展名
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 获取下载基础地址
        /// 不包含文件名
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// 获取已下载的字节数
        /// </summary>
        public long DownloadedBytes { get; private set; }

        /// <summary>
        /// 获取是否以追加模式写入文件
        /// true追加，false覆盖
        /// </summary>
        public bool IsAppend { get; private set; }

        /// <summary>
        /// 获取当前剩余重试次数
        /// </summary>
        public int CurrentRetryCount { get; private set; }

        /// <summary>
        /// 获取对应的AssetBundle包名
        /// </summary>
        public string AbName { get; private set; }

        /// <summary>
        /// 获取文件Hash校验值
        /// </summary>
        public string Hash { get; private set; }
    }
}