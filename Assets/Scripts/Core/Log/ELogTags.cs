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
        
        // AOT Core
        Asset = 1 << 1,
        UI = 1 << 2,
        MonoApdater = 1 << 3,
        Pool = 1 << 4,
        System = 1 << 5,
        Task = 1 << 6,
        Scene = 1 << 7,
        SO = 1 << 8,
        Time = 1 << 9,
        Reactive = 1 << 10,
        Utility = 1 << 11,
        Collection = 1 << 12,
        HotUpdate = 1 << 13,
        Network = 1 << 14,
        Music  = 1 << 15,
        
        // AOT Game
        GameLauncher = 1 << 16,
        
        // Hot Game
        GameMassge = 1 << 17,
        Quest = 1 << 18,
        Activity = 1 << 19,
        Setting = 1 << 20,
        GameUpdate = 1 << 21,
        Dialogue = 1 << 22,
        Interact = 1 << 23,
        Icon = 1 << 24,
        Item = 1 << 25,
        Main = 1 << 26,
        Tip =  1 << 27,
        Input = 1 << 28,
        Battle = 1 << 29,
        HotUpdateEntry = 1 << 30,
        Component = 1 << 31,
    }
}
