using System;
using UnityEngine;

namespace Core.AssetBundles.Update.Collection
{
    /// <summary>
    /// AB包缓存信息
    /// </summary>
    [Serializable]
    public class AbPackageCacheInfo
    {
        //AB包名
        [SerializeField] private string _abName;
        //AB包MD5
        [SerializeField] private string _md5;
        //已下载的字节数
        [SerializeField] private long _downloadedBytes;
        //AB包是否下载成功
        [SerializeField] private bool _isSuccess;

        public AbPackageCacheInfo(string abName, string md5, long downloadedBytes)
        {
            _abName = abName;
            _md5 = md5;
            _downloadedBytes = downloadedBytes;
        }

        public AbPackageCacheInfo(string abName, string md5, long downloadedBytes, bool isSuccess)
        {
            _abName = abName;
            _md5 = md5;
            _downloadedBytes = downloadedBytes;
            _isSuccess = isSuccess;
        }

        /// <summary>
        /// AB包名
        /// </summary>
        public string AbName => _abName;

        /// <summary>
        /// AB包MD5
        /// </summary>
        public string Md5 { get { return _md5; } set { _md5 = value; } }

        /// <summary>
        /// AB包是否下载完成
        /// </summary>
        public bool IsSuccess { get { return _isSuccess; } set { _isSuccess = value; } }

        /// <summary>
        /// 已下载的字节数
        /// </summary>
        public long DownloadedBytes { get { return _downloadedBytes; } set { _downloadedBytes = value; } }
    }
}
