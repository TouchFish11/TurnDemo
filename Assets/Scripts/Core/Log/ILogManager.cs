using Core.Net;

namespace Core.Log
{
    /// <summary>
    /// ��־�������ӿ�
    /// </summary>
    public interface ILogManager
    {
        bool EnableLog { get; set; }

        void UploadLog(UploadProgressCallBack progressCallBack);
    }
}
