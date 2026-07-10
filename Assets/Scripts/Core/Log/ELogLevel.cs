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
        Debug = 8,
        Info = 16,
        Exception = 32,
    }
}

