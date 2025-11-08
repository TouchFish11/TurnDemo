
namespace Framework
{
    /// <summary>
    /// 记录信息类
    /// </summary>
    public class AssetBundleRecordInfo
    {
        //AB包名
        private readonly string _abName;
        //AB包MD5
        private string _md5;
        //已下载的字节数
        private long _downloadedBytes;
        //AB包是否下载成功
        private bool _isSuccess;

        public AssetBundleRecordInfo(string abName, string md5, long downloadedBytes)
        {
            this._abName = abName;
            this._md5 = md5;
            this._downloadedBytes = downloadedBytes;
        }

        public AssetBundleRecordInfo(string abName, string md5, long downloadedBytes, bool isSuccess)
        {
            this._abName = abName;
            this._md5 = md5;
            this._downloadedBytes = downloadedBytes;
            this._isSuccess = isSuccess;
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
