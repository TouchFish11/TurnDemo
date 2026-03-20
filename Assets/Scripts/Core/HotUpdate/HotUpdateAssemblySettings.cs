using System;

namespace Core.HotUpdate
{
    /// <summary>
    /// 热更新程序集设置
    /// </summary>
    [Serializable]
    public class HotUpdateAssemblySettings
    {
        /// 预先加载的热更程序集名称数组
        public string[] preloadHotUpdateAssemblies;
    }
}
