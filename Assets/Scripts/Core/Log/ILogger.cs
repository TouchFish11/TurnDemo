using Core.Net;

namespace Core.Log
{
    public interface ILogger
    {
        bool EnableLog { get; set; }

        void UploadLog(UploadProgressCallBack progressCallBack);

        /// <summary>
        /// 生成日志
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        void GenerateLog(string condition, string stackTrace, ELogLevel type);
    }
}
