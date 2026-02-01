#if DISABLE_ADDRESSABLES

#else
using System;

namespace Framework.Addressable.Update
{
    /// <summary>
    /// 可寻址新器接口
    /// </summary>
    public interface IAddressablesUpdater
    {
        /// <summary>
        /// 检查更新
        /// </summary>
        /// <param name="callback"></param>
        void CheckUpdate(Action<UpdateCallbackData> callback);

        /// <summary>
        /// 更新资源
        /// 当EUpdateState为CheckSuccess时调用该方法以下载资源
        /// </summary>
        /// <param name="callback"></param>
        void UpdateAssets(Action<UpdateCallbackData> callback);

        /// <summary>
        /// 停止更新
        /// </summary>
        void StopUpdate();
    }
}
#endif


