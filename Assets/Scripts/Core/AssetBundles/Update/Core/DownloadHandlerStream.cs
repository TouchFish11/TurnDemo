using System.IO;
using Core.Log;
using Core.Service;
using UnityEngine.Networking;

namespace Core.AssetBundles.Update.Core
{
    /// <summary>
    /// 流下载处理器
    /// </summary>
    public class DownloadHandlerStream : DownloadHandlerScript
    {
        private FileStream _fileStream;
        private const int preAllocatedLength = 64 * 1024;  // 64KB
        private static readonly byte[] preAllocatedBuffer = new byte[preAllocatedLength];
        private readonly IAssetBundleUpdater _updater;
        
        /// <summary>
        /// DownloadHandlerStream构造函数
        /// </summary>
        /// <param name="savePath">保存路径</param>
        /// <param name="isAppend">是否追加</param>
        /// <param name="downloadedBytes">已下载字节数，当isAppend为false，忽略此参数</param>
        public DownloadHandlerStream(string savePath, bool isAppend, long downloadedBytes = 0) : base(preAllocatedBuffer)
        {
            _updater = ServiceLocator.Get<IAssetBundleUpdater>();
            if (isAppend)
            {
                _fileStream = new FileStream(savePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, preAllocatedLength, FileOptions.SequentialScan | FileOptions.WriteThrough);
                if (downloadedBytes <= _fileStream.Length)
                {
                    _fileStream.Seek(downloadedBytes, SeekOrigin.Begin);
                }
            }
            else
            {
                _fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, preAllocatedLength, FileOptions.SequentialScan | FileOptions.WriteThrough);
            }
        }
        
        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (_fileStream is not { CanWrite: true } || _updater.GetContext().IsPauseDownload)
            {
                return false;
            }

            try
            {
                _fileStream.Write(data, 0, dataLength);
                return true;
            }
            catch (System.Exception e)
            {
                LogManager.Log($"{nameof(DownloadHandlerStream)}.{nameof(CompleteContent)}：写入异常，文件流，{_fileStream.Name}：已被释放");
                CloseStream();
                LogManager.LogError($"{nameof(DownloadHandlerStream)}.{nameof(ReceiveData)}: {e.Message}");
                return false;
            }
        }

        protected override void CompleteContent()
        {
            LogManager.Log($"{nameof(DownloadHandlerStream)}.{nameof(CompleteContent)}：下载完成，文件流，{_fileStream.Name}：已被释放");
            CloseStream();
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (_fileStream == null)
            {
                return;
            }
            
            LogManager.Log($"已手动暂停文件流，{_fileStream.Name}：已被释放");
            CloseStream();
        }
        
        /// <summary>
        /// 关闭流
        /// </summary>
        private void CloseStream()
        {
            if (_fileStream == null)
            {
                return;
            }
            
            _fileStream.Flush(true);
            _fileStream.Close();
            _fileStream.Dispose();
            _fileStream = null;
        }
    }
}
