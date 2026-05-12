using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.AssetBundles.Collection
{
    /// <summary>
    /// AB包缓存信息
    /// </summary>
    [Serializable]
    public class AbPackageCacheInfo
    {
        //AB包名
        [SerializeField] private string _abName;
        //AB包Hash
        [SerializeField] private string _hash;
        //已下载的字节数
        [SerializeField] private long _downloadedBytes;
        //AB包是否下载成功
        [SerializeField] private bool _isSuccess;

        [JsonConstructor]
        public AbPackageCacheInfo(string abName, string hash, long downloadedBytes)
        {
            _abName = abName;
            _hash = hash;
            _downloadedBytes = downloadedBytes;
        }
        
        public AbPackageCacheInfo(string abName, string hash, long downloadedBytes, bool isSuccess)
        {
            _abName = abName;
            _hash = hash;
            _downloadedBytes = downloadedBytes;
            _isSuccess = isSuccess;
        }

        /// <summary>
        /// AB包名
        /// </summary>
        public string AbName => _abName;

        /// <summary>
        /// AB包Hash
        /// </summary>
        public string Hash { get => _hash; set => _hash = value; }

        /// <summary>
        /// AB包是否下载完成
        /// </summary>
        public bool IsSuccess { get => _isSuccess; set => _isSuccess = value; }

        /// <summary>
        /// 已下载的字节数
        /// </summary>
        public long DownloadedBytes { get => _downloadedBytes; set => _downloadedBytes = value; }
    }
}
