using System;
using System.IO;
using System.Threading.Tasks;
using Core.Global;
using Core.Log;
using Core.Service;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Core.AssetBundles.Update
{
    /// <summary>
    /// AssetBundle资源网络请求器
    /// 负责通过UnityWebRequest下载AB包资源，支持断点续传、下载超时检测、下载进度回调、重试次数管理等功能
    /// 实现IDisposable接口，用于释放UnityWebRequest相关资源
    /// </summary>
    public class ABWebRequester : IDisposable
    {
        // UnityWebRequest核心请求对象，用于发起网络下载请求
        private UnityWebRequest _request;
        // AssetBundle包名，标识当前下载的AB包名称
        private readonly string _abName;
        // 文件MD5值，用于后续校验文件完整性（当前类仅存储，校验逻辑需外部实现）
        private readonly string _md5;
        // 要下载的文件名（包含扩展名）
        private readonly string _fileName;
        // 下载基础地址（不包含文件名）
        private readonly string _url;
        // 是否以追加模式写入文件：true-断点续传（在已有文件末尾追加）；false-覆盖写入
        private readonly bool _isAppend;
        // 已下载的字节数（断点续传时记录本地已存在的文件大小）
        private long _downloadedBytes;
        // 下载超时阈值（单位：秒），超过该时间未接收到新数据则判定为超时
        private float _downloadTimeout;
        // 当前剩余重试次数，初始值取自全局配置，下载失败时可递减
        private int _currentRetryCount;

        /// <summary>
        /// 下载进度回调事件（每帧触发，参数为当前帧新增下载的字节数）
        /// 用于外部监听下载速度、总进度等信息
        /// </summary>
        public event Action<long> OnDownloadProgress;

        /// <summary>
        /// 网络请求器构造函数
        /// </summary>
        /// <param name="url">下载基础地址（不包含文件名）</param>
        /// <param name="fileName">要下载的文件名（包含扩展名）</param>
        /// <param name="isAppend">是否断点续传：true-追加写入；false-覆盖写入</param>
        /// <param name="abName">对应的AssetBundle包名</param>
        /// <param name="md5">文件MD5校验值</param>
        public ABWebRequester(string url, string fileName, bool isAppend, string abName, string md5)
        {
            _url = url;
            _fileName = fileName;
            _isAppend = isAppend;
            // 初始化重试次数为全局配置的最大重试次数
            _currentRetryCount = GlobalSettings.Instance.maxRetryCount;
            _abName = abName;
            _md5 = md5;
        }

        /// <summary>
        /// 异步下载文件核心方法
        /// </summary>
        /// <param name="savePath">文件保存的本地完整路径（包含文件名）</param>
        /// <param name="overCallback">下载完成/失败回调（参数为是否下载成功）</param>
        public async void DownLoadAsync(string savePath, UnityAction<bool> overCallback)
        {
            try
            {
                // 初始化UnityWebRequest：创建GET请求（拼接基础地址+文件名）
                _request = UnityWebRequest.Get(_url + _fileName);
                // 设置连接超时时间（取自全局配置）
                _request.timeout = GlobalSettings.Instance.connectTimeout;
                // 设置下载处理器：将数据写入指定路径，指定是否追加模式
                _request.downloadHandler = new DownloadHandlerFile(savePath, _isAppend);

                // 断点续传预处理：如果本地已存在文件，读取文件大小作为已下载字节数
                if (File.Exists(savePath))
                {
                    var fileInfo = new FileInfo(savePath);
                    _downloadedBytes = fileInfo.Length;
                }

                // 设置请求头：Range指定从已下载字节数的位置开始下载（实现断点续传）
                _request?.SetRequestHeader("Range", "bytes=" + _downloadedBytes + "-");
                // 发送网络请求（非阻塞，进入异步等待状态）
                _request?.SendWebRequest();

                // 超时检测初始化：设置下载超时阈值（取自全局配置），记录最后一次接收数据的时间
                _downloadTimeout = GlobalSettings.Instance.downloadTimeout;
                var lastReceiveTime = UnityEngine.Time.realtimeSinceStartup;
                var isDownloadTimeout = false; // 超时标记

                // 下载进度轮询变量：记录上一帧的下载字节数，用于计算当前帧新增下载量
                long lastFrameDownloadBytes = 0;
                // 获取下载上下文（用于判断是否暂停下载）
                var context = ServiceLocator.Get<IAssetBundleUpdater>().GetContext();

                // 下载循环：请求未完成、未暂停时持续轮询
                while (_request != null && !_request.isDone && !context.IsPauseDownload)
                {
                    // 获取当前已下载的总字节数
                    var currentDownloaded = (long)_request?.downloadedBytes;
                    // 计算当前帧新增下载字节数
                    var delta = currentDownloaded - lastFrameDownloadBytes;

                    if (delta > 0)
                    {
                        // 有新数据：触发进度回调，更新上一帧字节数，重置最后接收时间
                        OnDownloadProgress?.Invoke(delta);
                        lastFrameDownloadBytes = currentDownloaded;
                        lastReceiveTime = UnityEngine.Time.realtimeSinceStartup;
                    }
                    else if (UnityEngine.Time.realtimeSinceStartup - lastReceiveTime >= _downloadTimeout)
                    {
                        // 无新数据且超过超时阈值：标记超时，终止请求，退出循环
                        isDownloadTimeout = true;
                        Abort();
                        break;
                    }

                    // 让出当前帧执行权，等待下一帧继续轮询（异步等待，避免阻塞主线程）
                    await Task.Yield();
                }

                // 下载结束后处理：分三种情况（超时、请求失败、请求成功）
                if (isDownloadTimeout)
                {
                    // 超时处理：打印日志，触发失败回调
                    LogManager.Log($"{_fileName}下载超时：超过{_downloadTimeout}秒未接收到数据");
                    overCallback?.Invoke(false);
                }
                else if (_request?.result != UnityWebRequest.Result.Success)
                {
                    // 请求失败：打印错误日志（包含错误信息、响应码），触发失败回调
                    LogManager.LogError($"{_fileName}下载失败：错误信息={_request?.error}，响应码={_request?.responseCode}");
                    overCallback?.Invoke(false);
                }
                else
                {
                    // 请求成功：触发最后一次进度回调（避免循环结束时遗漏最后一批数据）
                    var finalDelta = (long)_request?.downloadedBytes - lastFrameDownloadBytes;
                    if (finalDelta > 0)
                    {
                        OnDownloadProgress?.Invoke(finalDelta);
                    }
                    // 触发成功回调
                    overCallback?.Invoke(_request?.result == UnityWebRequest.Result.Success);
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"下载异常，{e.Message}");
            }
        }

        /// <summary>
        /// 递减剩余重试次数
        /// 重试次数最小为0，避免负数
        /// </summary>
        public void SubRetryCount()
        {
            --_currentRetryCount;
            if (_currentRetryCount <= 0)
            {
                _currentRetryCount = 0;
            }
        }

        /// <summary>
        /// 终止当前下载请求
        /// 同时清空下载进度回调，避免空引用或重复回调
        /// </summary>
        public void Abort()
        {
            _request?.Abort();
            OnDownloadProgress = null;
        }

        /// <summary>
        /// 释放资源（实现IDisposable接口）
        /// 释放UnityWebRequest对象，避免内存泄漏
        /// </summary>
        public void Dispose()
        {
            _request?.Dispose();
            _request = null;
        }

        /// <summary>
        /// 只读属性：获取要下载的文件名
        /// </summary>
        public string FileName => _fileName;

        /// <summary>
        /// 只读属性：获取下载基础地址
        /// </summary>
        public string Url => _url;

        /// <summary>
        /// 只读属性：获取已下载的字节数（断点续传初始值）
        /// </summary>
        public long DownloadedBytes => _downloadedBytes;

        /// <summary>
        /// 只读属性：获取是否以追加模式写入文件
        /// </summary>
        public bool IsAppend => _isAppend;

        /// <summary>
        /// 只读属性：获取当前剩余重试次数
        /// </summary>
        public int CurrentRetryCount => _currentRetryCount;

        /// <summary>
        /// 只读属性：获取对应的AssetBundle包名
        /// </summary>
        public string AbName => _abName;

        /// <summary>
        /// 只读属性：获取文件MD5校验值
        /// </summary>
        public string MD5 => _md5;
    }
}