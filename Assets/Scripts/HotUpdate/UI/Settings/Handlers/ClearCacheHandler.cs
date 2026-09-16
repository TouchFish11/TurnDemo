using UnityEngine;

namespace HotUpdate.UI.Settings.Handlers
{
    /// <summary>
    /// 「清理缓存」处理器：清空 Unity 的本地缓存（AssetBundle 等，对应 Caching）。
    /// </summary>
    public class ClearCacheHandler : IButtonSettingHandler
    {
        public void Execute()
        {
            Caching.ClearCache();
        }
    }
}
