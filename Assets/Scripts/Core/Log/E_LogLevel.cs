using System;

namespace Core.Log
{
    /// <summary>
    /// 日志级别
    /// </summary>
    [Flags]
    public enum ELogLevel : byte
    {
        None = 0,
        Assert = 1,
        Error = 2,
        Warning = 4,
        Log = 8,
        Exception = 16,
    }
}

