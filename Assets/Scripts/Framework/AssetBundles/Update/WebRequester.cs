using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Framework
{
    /// <summary>
    /// UnityWeb请求者
    /// </summary>
    public class WebRequester : IDisposable
    {
        //UnityWebRequest对象
        private UnityWebRequest _request;
        //文件所属AB包
        private readonly string _abName;
        //文件MD5码
        private readonly string _md5;
        //下载文件名称
        private readonly string _fileName;
        //服务器地址
        private readonly string _url;
        //是否从尾部写入，否则覆盖
        private readonly bool _isAppend;
        //已下载的字节数
        private long _downloadedBytes;
        //下载超时时间
        private float _downloadTimeout;
        //当前请求剩余重试次数
        private int _currentRetryCount;
        //下载协程
        private Coroutine _coroutine;

        /// <summary>
        /// WebRequester构造函数
        /// </summary>
        /// <param name="url">服务器地址</param>
        /// <param name="fileName">下载的文件名(带后缀)</param>
        /// <param name="isAppend">是否从上次下载的数据尾部继续写入。true：继续写入；false：覆盖</param>
        public WebRequester(string url, string fileName, bool isAppend, string abName, string md5)
        {
            this._url = url;
            this._fileName = fileName;
            this._isAppend = isAppend;
            _currentRetryCount = GlobalSettings.Instance.maxRetryCount;
            _abName = abName;
            _md5 = md5;
        }

        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="savePath">本地保存路径</param>
        /// <param name="overCallBack">下载结束回调</param>
        /// <param name="nowDownLoadBytesCallBack">下载进度回调</param>
        public void DownLoad(string savePath, UnityAction<bool> overCallBack, UnityAction<long> nowDownLoadBytesCallBack = null)
        {
            _coroutine = MonoManager.Instance.StartCoroutine(DownLoad_Cor());

            IEnumerator DownLoad_Cor()
            {
                //创建UnityWebRequestGet对象
                this._request = UnityWebRequest.Get(this._url + this._fileName);
                //设置连接超时
                this._request.timeout = GlobalSettings.Instance.connectTimeout;
                //设置下载处理器对象
                this._request.downloadHandler = new DownloadHandlerFile(savePath, this._isAppend);

                //是否存在上次未下载的完成的文件
                if (File.Exists(savePath))
                {
                    //创建文件信息对象
                    FileInfo fileInfo = new FileInfo(savePath);
                    this._downloadedBytes = fileInfo.Length;
                }

                //设置请求头
                this._request.SetRequestHeader("Range", "bytes=" + this._downloadedBytes + "-");
                //发送请求，等待连接阶段完成
                this._request.SendWebRequest();

                //连接成功，进入下载阶段，初始化下载超时判断
                _downloadTimeout = GlobalSettings.Instance.downloadTimeout;
                //上次接收数据时间
                float lastReceiveTime = Time.realtimeSinceStartup;
                //是否下载超时
                bool isDownloadTimeout = false;

                //上一帧下载量
                long lastFrameDownloadBytes = 0;
                //下载未完成，分发进度  //!AssetBundleUpdateManager.Instance.IsPauseDownload && 
                while (!this._request.isDone)
                {
                    //当前下载量，
                    long currentDownloaded = (long)_request.downloadedBytes;
                    //计算与上一帧下载量的插值
                    long delta = currentDownloaded - lastFrameDownloadBytes;
                    // 分发增量
                    if (delta > 0)
                    {
                        nowDownLoadBytesCallBack?.Invoke(delta);
                        /*  
                         *  当前下载量作为上一帧下载量，不能用_request.downloadedBytes直接赋值
                         *  原因：下载是在后台进行的，若当前循环分发增量后下载量增加，且使用_request.downloadedBytes直接赋值，
                         *  那新增的下载量会被作为上一次下载量，导致新增下载量被“丢弃”，没有被下一次分发
                         *  最终导致肉眼可见的进度差异，即增量和不等于总下载量
                         */
                        lastFrameDownloadBytes = currentDownloaded;
                        //接收到数据，重置上次接收时间
                        lastReceiveTime = Time.realtimeSinceStartup;
                    }
                    else if(Time.realtimeSinceStartup - lastReceiveTime >= _downloadTimeout)
                    {
                        //下载超时
                        isDownloadTimeout = true;
                        //终止请求
                        Abort();
                        break;
                    }

                    yield return null;
                }

                if (isDownloadTimeout)
                {
                    LogMgr.Log($"{_fileName}下载超时，长时间（{_downloadTimeout}s）未收到数据");
                    overCallBack?.Invoke(false);
                }
                //检查连接阶段是否失败（包含连接超时）
                else if (_request.result != UnityWebRequest.Result.Success)
                {
                    LogMgr.LogError($"{_fileName}连接失败：{_request.error}，响应码：{_request.responseCode}");
                    overCallBack?.Invoke(false);
                    yield break;
                }
                else
                {
                    // 下载完成后，补充最后一次增量（避免循环退出时遗漏）
                    long finalDelta = (long)_request.downloadedBytes - lastFrameDownloadBytes;
                    if (finalDelta > 0)
                    {
                        nowDownLoadBytesCallBack?.Invoke(finalDelta);
                    }
                    //下载完成, 执行完成回调
                    overCallBack(this._request.result == UnityWebRequest.Result.Success);
                }
            }
        }

        /// <summary>
        /// 减少重试次数
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
        /// 终止请求
        /// </summary>
        public void Abort()
        {
            _request?.Abort();
            MonoManager.Instance.StopCoroutine(_coroutine);
        }

        /// <summary>
        /// 销毁请求对象
        /// </summary>
        public void Dispose()
        {
            _request?.Dispose();
            _request = null;
        }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string FileName => this._fileName;

        /// <summary>
        /// 服务器文件路径
        /// </summary>
        public string Url => this._url;

        /// <summary>
        /// 已下载的字节数
        /// </summary>
        public long DownloadedBytes => this._downloadedBytes;

        /// <summary>
        /// 是否从尾部写入，否则覆盖
        /// </summary>
        public bool IsAppend => this._isAppend;

        /// <summary>
        /// 当前请求剩余重试次数
        /// </summary>
        public int CurrentRetryCount => this._currentRetryCount;

        /// <summary>
        /// 所属AB包名
        /// </summary>
        public string AbName => this._abName;

        /// <summary>
        /// 文件MD5码
        /// </summary>
        public string MD5 => this._md5;
    }
}
