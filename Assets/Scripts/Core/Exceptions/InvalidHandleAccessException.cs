using System;

namespace Core.Exceptions
{
    /// <summary>
    /// 无效的句柄访问异常
    /// </summary>
    public class InvalidHandleAccessException : ExceptionBase
    {
        public int HandleId { get; private set; }
        
        public int Version { get; private set; }
        
        public InvalidHandleAccessException(int handleId, int version, int errorCode, string message, Exception inner) : base(errorCode, message, inner)
        {
            HandleId = handleId;
            Version = version;
        }
    }
}
