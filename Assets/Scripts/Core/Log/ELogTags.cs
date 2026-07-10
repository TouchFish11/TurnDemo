using System;

namespace Core.Log
{
    /// <summary>
    /// 日志标签
    /// </summary>
    [Flags]
    public enum ELogTags
    {
        None = 0,
        AssetBundle = 1 << 0,
        Asset = 1 << 1,
        UI = 1 << 2,
        Battle = 1 << 3,
        Quest = 1 << 4,
        Activity = 1 << 5,
        Setting = 1 << 6,
        System = 1 << 7,
        HotUpdate = 1 << 8,
    }
}
